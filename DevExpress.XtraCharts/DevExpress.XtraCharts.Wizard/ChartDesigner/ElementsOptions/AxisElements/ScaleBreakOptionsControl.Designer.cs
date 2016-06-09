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

namespace DevExpress.XtraCharts.Designer.Native {
	partial class ScaleBreakOptionsControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			DevExpress.XtraCharts.Designer.Native.PropertyLink propertyLink1 = new DevExpress.XtraCharts.Designer.Native.PropertyLink();
			DevExpress.XtraCharts.Designer.Native.PropertyLink propertyLink2 = new DevExpress.XtraCharts.Designer.Native.PropertyLink();
			DevExpress.XtraCharts.Designer.Native.PropertyLink propertyLink3 = new DevExpress.XtraCharts.Designer.Native.PropertyLink();
			this.checkEdit1 = new DevExpress.XtraEditors.CheckEdit();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.titleElement1 = new DevExpress.XtraCharts.Designer.Native.TitleElement();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.textEdit1 = new DevExpress.XtraEditors.TextEdit();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.textEdit2 = new DevExpress.XtraEditors.TextEdit();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.GeneralGroup = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.simpleSeparator1 = new DevExpress.XtraLayout.SimpleSeparator();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.uiOptionsBuilder1 = new DevExpress.XtraCharts.Designer.Native.UIOptionsBuilder();
			((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.GeneralGroup)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.uiOptionsBuilder1)).BeginInit();
			this.SuspendLayout();
			this.checkEdit1.Location = new System.Drawing.Point(152, 41);
			this.checkEdit1.Name = "checkEdit1";
			this.checkEdit1.Properties.Caption = "";
			this.checkEdit1.Size = new System.Drawing.Size(136, 19);
			this.checkEdit1.StyleController = this.layoutControl1;
			this.checkEdit1.TabIndex = 6;
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.titleElement1);
			this.layoutControl1.Controls.Add(this.labelControl1);
			this.layoutControl1.Controls.Add(this.checkEdit1);
			this.layoutControl1.Controls.Add(this.labelControl2);
			this.layoutControl1.Controls.Add(this.textEdit1);
			this.layoutControl1.Controls.Add(this.labelControl3);
			this.layoutControl1.Controls.Add(this.textEdit2);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(300, 164);
			this.layoutControl1.TabIndex = 0;
			this.titleElement1.Location = new System.Drawing.Point(12, 12);
			this.titleElement1.Name = "titleElement1";
			this.titleElement1.Size = new System.Drawing.Size(286, 13);
			this.titleElement1.TabIndex = 4;
			this.titleElement1.Title = "GENERAL";
			this.labelControl1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.labelControl1.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.labelControl1.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl1.Location = new System.Drawing.Point(12, 44);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(136, 13);
			this.labelControl1.StyleController = this.layoutControl1;
			this.labelControl1.TabIndex = 5;
			this.labelControl1.Text = "Visible:";
			this.labelControl2.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.labelControl2.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.labelControl2.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl2.Location = new System.Drawing.Point(12, 69);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new System.Drawing.Size(136, 13);
			this.labelControl2.StyleController = this.layoutControl1;
			this.labelControl2.TabIndex = 7;
			this.labelControl2.Text = "Edge 1:";
			this.textEdit1.Location = new System.Drawing.Point(152, 66);
			this.textEdit1.Name = "textEdit1";
			this.textEdit1.Size = new System.Drawing.Size(136, 20);
			this.textEdit1.StyleController = this.layoutControl1;
			this.textEdit1.TabIndex = 8;
			this.labelControl3.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.labelControl3.Appearance.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.labelControl3.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.labelControl3.Location = new System.Drawing.Point(12, 95);
			this.labelControl3.Name = "labelControl3";
			this.labelControl3.Size = new System.Drawing.Size(136, 13);
			this.labelControl3.StyleController = this.layoutControl1;
			this.labelControl3.TabIndex = 9;
			this.labelControl3.Text = "Edge 2:";
			this.textEdit2.Location = new System.Drawing.Point(152, 92);
			this.textEdit2.Name = "textEdit2";
			this.textEdit2.Size = new System.Drawing.Size(136, 20);
			this.textEdit2.StyleController = this.layoutControl1;
			this.textEdit2.TabIndex = 10;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.GeneralGroup,
			this.emptySpaceItem1});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "layoutControlGroup1";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(300, 164);
			this.layoutControlGroup1.TextVisible = false;
			this.GeneralGroup.GroupBordersVisible = false;
			this.GeneralGroup.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.simpleSeparator1,
			this.layoutControlItem2,
			this.layoutControlItem3,
			this.layoutControlItem4,
			this.layoutControlItem5,
			this.layoutControlItem6,
			this.layoutControlItem7});
			this.GeneralGroup.Location = new System.Drawing.Point(0, 0);
			this.GeneralGroup.Name = "GeneralGroup";
			this.GeneralGroup.Size = new System.Drawing.Size(300, 115);
			this.layoutControlItem1.Control = this.titleElement1;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.MaxSize = new System.Drawing.Size(0, 27);
			this.layoutControlItem1.MinSize = new System.Drawing.Size(40, 27);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 2, 12, 2);
			this.layoutControlItem1.Size = new System.Drawing.Size(300, 27);
			this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.simpleSeparator1.AllowHotTrack = false;
			this.simpleSeparator1.Location = new System.Drawing.Point(0, 27);
			this.simpleSeparator1.Name = "simpleSeparator1";
			this.simpleSeparator1.Size = new System.Drawing.Size(300, 11);
			this.simpleSeparator1.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 9);
			this.layoutControlItem2.Control = this.labelControl1;
			this.layoutControlItem2.ControlAlignment = System.Drawing.ContentAlignment.MiddleRight;
			this.layoutControlItem2.FillControlToClientArea = false;
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 38);
			this.layoutControlItem2.MaxSize = new System.Drawing.Size(150, 0);
			this.layoutControlItem2.MinSize = new System.Drawing.Size(150, 1);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 2, 3, 3);
			this.layoutControlItem2.Size = new System.Drawing.Size(150, 25);
			this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem3.Control = this.checkEdit1;
			this.layoutControlItem3.Location = new System.Drawing.Point(150, 38);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 12, 3, 3);
			this.layoutControlItem3.Size = new System.Drawing.Size(150, 25);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem4.Control = this.labelControl2;
			this.layoutControlItem4.ControlAlignment = System.Drawing.ContentAlignment.MiddleRight;
			this.layoutControlItem4.FillControlToClientArea = false;
			this.layoutControlItem4.Location = new System.Drawing.Point(0, 63);
			this.layoutControlItem4.MaxSize = new System.Drawing.Size(150, 0);
			this.layoutControlItem4.MinSize = new System.Drawing.Size(150, 1);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 2, 3, 3);
			this.layoutControlItem4.Size = new System.Drawing.Size(150, 26);
			this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem5.Control = this.textEdit1;
			this.layoutControlItem5.Location = new System.Drawing.Point(150, 63);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 12, 3, 3);
			this.layoutControlItem5.Size = new System.Drawing.Size(150, 26);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem6.Control = this.labelControl3;
			this.layoutControlItem6.ControlAlignment = System.Drawing.ContentAlignment.MiddleRight;
			this.layoutControlItem6.FillControlToClientArea = false;
			this.layoutControlItem6.Location = new System.Drawing.Point(0, 89);
			this.layoutControlItem6.MaxSize = new System.Drawing.Size(150, 0);
			this.layoutControlItem6.MinSize = new System.Drawing.Size(150, 1);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 2, 3, 3);
			this.layoutControlItem6.Size = new System.Drawing.Size(150, 26);
			this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem7.Control = this.textEdit2;
			this.layoutControlItem7.Location = new System.Drawing.Point(150, 89);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 12, 3, 3);
			this.layoutControlItem7.Size = new System.Drawing.Size(150, 26);
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 115);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(300, 49);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl1);
			this.Name = "ScaleBreakOptionsControl";
			propertyLink1.Editor = this.checkEdit1;
			propertyLink1.PropertyName = "Visible";
			propertyLink1.PropertyType = typeof(bool);
			propertyLink1.TitleControl = this.labelControl1;
			propertyLink2.Editor = this.textEdit1;
			propertyLink2.PropertyName = "Edge1";
			propertyLink2.PropertyType = typeof(object);
			propertyLink2.TitleControl = this.labelControl2;
			propertyLink3.Editor = this.textEdit2;
			propertyLink3.PropertyName = "Edge2";
			propertyLink3.PropertyType = typeof(object);
			propertyLink3.TitleControl = this.labelControl3;
			this.PropertyLinks.Add(propertyLink1);
			this.PropertyLinks.Add(propertyLink2);
			this.PropertyLinks.Add(propertyLink3);
			this.Size = new System.Drawing.Size(300, 164);
			this.Titles.Add(this.titleElement1);
			((System.ComponentModel.ISupportInitialize)(this.checkEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.textEdit1.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.textEdit2.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.GeneralGroup)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.simpleSeparator1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.uiOptionsBuilder1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private UIOptionsBuilder uiOptionsBuilder1;
		private XtraLayout.LayoutControl layoutControl1;
		private TitleElement titleElement1;
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.CheckEdit checkEdit1;
		private XtraEditors.LabelControl labelControl2;
		private XtraEditors.TextEdit textEdit1;
		private XtraEditors.LabelControl labelControl3;
		private XtraEditors.TextEdit textEdit2;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraLayout.LayoutControlGroup GeneralGroup;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraLayout.SimpleSeparator simpleSeparator1;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraLayout.LayoutControlItem layoutControlItem3;
		private XtraLayout.LayoutControlItem layoutControlItem4;
		private XtraLayout.LayoutControlItem layoutControlItem5;
		private XtraLayout.LayoutControlItem layoutControlItem6;
		private XtraLayout.LayoutControlItem layoutControlItem7;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
	}
}
