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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using DevExpress.Utils;
using DevExpress.Skins;
using DevExpress.Utils.Drawing;
using DevExpress.LookAndFeel;
namespace DevExpress.XtraReports.Wizards3.Views {
	[System.ComponentModel.ToolboxItem(false)]
	public partial class SummaryOptionsHeader : UserControl {
		private XtraLayout.LayoutControl layoutControl1;
		private XtraEditors.LabelControl labelControl5;
		private XtraEditors.LabelControl labelControl4;
		private XtraEditors.LabelControl labelControl3;
		private XtraEditors.LabelControl labelControl2;
		private XtraEditors.LabelControl labelControl1;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraLayout.LayoutControlItem layoutControlItem3;
		private XtraLayout.LayoutControlItem layoutControlItem4;
		private XtraLayout.LayoutControlItem layoutControlItem5;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
		public SummaryOptionsHeader() {
			InitializeComponent();
		}
		private void InitializeComponent() {
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.labelControl5 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.Controls.Add(this.labelControl5);
			this.layoutControl1.Controls.Add(this.labelControl4);
			this.layoutControl1.Controls.Add(this.labelControl3);
			this.layoutControl1.Controls.Add(this.labelControl2);
			this.layoutControl1.Controls.Add(this.labelControl1);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(1069, 441, 450, 350);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(350, 30);
			this.layoutControl1.TabIndex = 0;
			this.layoutControl1.Text = "layoutControl1";
			this.labelControl5.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.labelControl5.Location = new System.Drawing.Point(205, 5);
			this.labelControl5.MaximumSize = new System.Drawing.Size(40, 20);
			this.labelControl5.Name = "labelControl5";
			this.labelControl5.Size = new System.Drawing.Size(20, 20);
			this.labelControl5.StyleController = this.layoutControl1;
			this.labelControl5.TabIndex = 8;
			this.labelControl5.Text = "Avg";
			this.labelControl4.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.labelControl4.Location = new System.Drawing.Point(235, 5);
			this.labelControl4.MaximumSize = new System.Drawing.Size(40, 20);
			this.labelControl4.Name = "labelControl4";
			this.labelControl4.Size = new System.Drawing.Size(20, 20);
			this.labelControl4.StyleController = this.layoutControl1;
			this.labelControl4.TabIndex = 7;
			this.labelControl4.Text = "Count";
			this.labelControl3.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.labelControl3.Location = new System.Drawing.Point(265, 5);
			this.labelControl3.MaximumSize = new System.Drawing.Size(40, 20);
			this.labelControl3.Name = "labelControl3";
			this.labelControl3.Size = new System.Drawing.Size(20, 20);
			this.labelControl3.StyleController = this.layoutControl1;
			this.labelControl3.TabIndex = 6;
			this.labelControl3.Text = "Max";
			this.labelControl2.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.labelControl2.Location = new System.Drawing.Point(295, 5);
			this.labelControl2.MaximumSize = new System.Drawing.Size(40, 20);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new System.Drawing.Size(20, 20);
			this.labelControl2.StyleController = this.layoutControl1;
			this.labelControl2.TabIndex = 5;
			this.labelControl2.Text = "Min";
			this.labelControl1.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.labelControl1.Location = new System.Drawing.Point(325, 5);
			this.labelControl1.MaximumSize = new System.Drawing.Size(40, 20);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(20, 20);
			this.labelControl1.StyleController = this.layoutControl1;
			this.labelControl1.TabIndex = 4;
			this.labelControl1.Text = "Sum";
			this.layoutControlGroup1.CustomizationFormText = "layoutControlGroup1";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.layoutControlItem2,
			this.layoutControlItem3,
			this.layoutControlItem4,
			this.layoutControlItem5,
			this.emptySpaceItem1});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "layoutControlGroup1";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup1.Size = new System.Drawing.Size(350, 30);
			this.layoutControlGroup1.Text = "layoutControlGroup1";
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlItem1.Control = this.labelControl1;
			this.layoutControlItem1.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
			this.layoutControlItem1.FillControlToClientArea = false;
			this.layoutControlItem1.Location = new System.Drawing.Point(320, 0);
			this.layoutControlItem1.MinSize = new System.Drawing.Size(30, 23);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(30, 30);
			this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem1.Spacing = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
			this.layoutControlItem1.Text = "layoutControlItem1";
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextToControlDistance = 0;
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem2.Control = this.labelControl2;
			this.layoutControlItem2.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
			this.layoutControlItem2.FillControlToClientArea = false;
			this.layoutControlItem2.Location = new System.Drawing.Point(290, 0);
			this.layoutControlItem2.MinSize = new System.Drawing.Size(30, 23);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(30, 30);
			this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem2.Spacing = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
			this.layoutControlItem2.Text = "layoutControlItem2";
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextToControlDistance = 0;
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem3.Control = this.labelControl3;
			this.layoutControlItem3.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
			this.layoutControlItem3.FillControlToClientArea = false;
			this.layoutControlItem3.Location = new System.Drawing.Point(260, 0);
			this.layoutControlItem3.MinSize = new System.Drawing.Size(30, 23);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(30, 30);
			this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem3.Spacing = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
			this.layoutControlItem3.Text = "layoutControlItem3";
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextToControlDistance = 0;
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem4.Control = this.labelControl4;
			this.layoutControlItem4.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
			this.layoutControlItem4.FillControlToClientArea = false;
			this.layoutControlItem4.Location = new System.Drawing.Point(230, 0);
			this.layoutControlItem4.MinSize = new System.Drawing.Size(30, 23);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Size = new System.Drawing.Size(30, 30);
			this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem4.Spacing = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
			this.layoutControlItem4.Text = "layoutControlItem4";
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextToControlDistance = 0;
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem5.Control = this.labelControl5;
			this.layoutControlItem5.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
			this.layoutControlItem5.FillControlToClientArea = false;
			this.layoutControlItem5.Location = new System.Drawing.Point(200, 0);
			this.layoutControlItem5.MinSize = new System.Drawing.Size(30, 23);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Size = new System.Drawing.Size(30, 30);
			this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem5.Spacing = new DevExpress.XtraLayout.Utils.Padding(3, 3, 3, 3);
			this.layoutControlItem5.Text = "layoutControlItem5";
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextToControlDistance = 0;
			this.layoutControlItem5.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceItem1.MaxSize = new System.Drawing.Size(200, 29);
			this.emptySpaceItem1.MinSize = new System.Drawing.Size(200, 29);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(200, 30);
			this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem1.Text = "emptySpaceItem1";
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(this.layoutControl1);
			this.MaximumSize = new System.Drawing.Size(0, 30);
			this.Name = "SummaryOptionsHeader";
			this.Size = new System.Drawing.Size(350, 30);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			this.ResumeLayout(false);
		}
		void panelVert_Paint(object sender, PaintEventArgs e) {
			var rectangle = new Rectangle(e.ClipRectangle.Left, e.ClipRectangle.Top, 1, this.Parent.Height);
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(UserLookAndFeel.Default)[CommonSkins.SkinLabelLineVert], rectangle);
			ObjectPainter.DrawObject(new GraphicsCache(e), SkinElementPainter.Default, info);
		}
		void panelHor_Paint(object sender, PaintEventArgs e) {
			SkinElementInfo info = new SkinElementInfo(CommonSkins.GetSkin(UserLookAndFeel.Default)[CommonSkins.SkinLabelLine], e.ClipRectangle);
			ObjectPainter.DrawObject(new GraphicsCache(e), SkinElementPainter.Default, info);
		}
	}
}
