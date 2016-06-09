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
	partial class ColorScale2Control {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.lcColorScale2 = new DevExpress.XtraLayout.LayoutControl();
			this.clrpckColorMax = new DevExpress.XtraEditors.ColorPickEdit();
			this.clrpckColorMin = new DevExpress.XtraEditors.ColorPickEdit();
			this.pctPreview = new DevExpress.XtraEditors.PictureEdit();
			this.cmbTypeMin = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cmbTypeMax = new DevExpress.XtraEditors.ComboBoxEdit();
			this.tedValueMax = new DevExpress.XtraEditors.TextEdit();
			this.tedValueMin = new DevExpress.XtraEditors.TextEdit();
			this.lcgColorScale = new DevExpress.XtraLayout.LayoutControlGroup();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.lciTypeMax = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciTypeMin = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciValueMin = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciValueMax = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciPreview = new DevExpress.XtraLayout.LayoutControlItem();
			this.sliMinimum = new DevExpress.XtraLayout.SimpleLabelItem();
			this.sliMaximum = new DevExpress.XtraLayout.SimpleLabelItem();
			this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem4 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.lciColorMin = new DevExpress.XtraLayout.LayoutControlItem();
			this.lciColorMax = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.lcColorScale2)).BeginInit();
			this.lcColorScale2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.clrpckColorMax.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.clrpckColorMin.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbTypeMin.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbTypeMax.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tedValueMax.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tedValueMin.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgColorScale)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciTypeMax)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciTypeMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciValueMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciValueMax)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sliMinimum)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.sliMaximum)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciColorMin)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lciColorMax)).BeginInit();
			this.SuspendLayout();
			this.lcColorScale2.AllowCustomization = false;
			this.lcColorScale2.AutoScroll = false;
			this.lcColorScale2.Controls.Add(this.clrpckColorMax);
			this.lcColorScale2.Controls.Add(this.clrpckColorMin);
			this.lcColorScale2.Controls.Add(this.pctPreview);
			this.lcColorScale2.Controls.Add(this.cmbTypeMin);
			this.lcColorScale2.Controls.Add(this.cmbTypeMax);
			this.lcColorScale2.Controls.Add(this.tedValueMax);
			this.lcColorScale2.Controls.Add(this.tedValueMin);
			this.lcColorScale2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lcColorScale2.Location = new System.Drawing.Point(0, 0);
			this.lcColorScale2.Name = "lcColorScale2";
			this.lcColorScale2.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(716, 219, 978, 636);
			this.lcColorScale2.Root = this.lcgColorScale;
			this.lcColorScale2.Size = new System.Drawing.Size(470, 123);
			this.lcColorScale2.TabIndex = 0;
			this.lcColorScale2.Text = "layoutControl1";
			this.clrpckColorMax.EditValue = System.Drawing.Color.Empty;
			this.clrpckColorMax.Location = new System.Drawing.Point(353, 69);
			this.clrpckColorMax.Name = "clrpckColorMax";
			this.clrpckColorMax.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.clrpckColorMax.Properties.ColorAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.clrpckColorMax.Size = new System.Drawing.Size(113, 20);
			this.clrpckColorMax.StyleController = this.lcColorScale2;
			this.clrpckColorMax.TabIndex = 13;
			this.clrpckColorMax.EditValueChanged += new System.EventHandler(this.clrpckMaxColor_EditValueChanged);
			this.clrpckColorMin.EditValue = System.Drawing.Color.Empty;
			this.clrpckColorMin.Location = new System.Drawing.Point(51, 69);
			this.clrpckColorMin.Name = "clrpckColorMin";
			this.clrpckColorMin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.clrpckColorMin.Properties.ColorAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.clrpckColorMin.Size = new System.Drawing.Size(105, 20);
			this.clrpckColorMin.StyleController = this.lcColorScale2;
			this.clrpckColorMin.TabIndex = 12;
			this.clrpckColorMin.EditValueChanged += new System.EventHandler(this.clrpckMinColor_EditValueChanged);
			this.pctPreview.Location = new System.Drawing.Point(51, 93);
			this.pctPreview.Name = "pctPreview";
			this.pctPreview.Properties.ShowMenu = false;
			this.pctPreview.Size = new System.Drawing.Size(415, 26);
			this.pctPreview.StyleController = this.lcColorScale2;
			this.pctPreview.TabIndex = 11;
			this.pctPreview.Paint += new System.Windows.Forms.PaintEventHandler(this.pctPreview_Paint);
			this.cmbTypeMin.EditValue = "Percent";
			this.cmbTypeMin.Location = new System.Drawing.Point(51, 21);
			this.cmbTypeMin.Name = "cmbTypeMin";
			this.cmbTypeMin.Properties.AutoComplete = false;
			this.cmbTypeMin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cmbTypeMin.Properties.DropDownRows = 2;
			this.cmbTypeMin.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbTypeMin.Size = new System.Drawing.Size(105, 20);
			this.cmbTypeMin.StyleController = this.lcColorScale2;
			this.cmbTypeMin.TabIndex = 6;
			this.cmbTypeMax.EditValue = "Percent";
			this.cmbTypeMax.Location = new System.Drawing.Point(353, 21);
			this.cmbTypeMax.Name = "cmbTypeMax";
			this.cmbTypeMax.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cmbTypeMax.Properties.DropDownRows = 2;
			this.cmbTypeMax.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cmbTypeMax.Size = new System.Drawing.Size(113, 20);
			this.cmbTypeMax.StyleController = this.lcColorScale2;
			this.cmbTypeMax.TabIndex = 5;
			this.tedValueMax.Location = new System.Drawing.Point(353, 45);
			this.tedValueMax.Name = "tedValueMax";
			this.tedValueMax.Size = new System.Drawing.Size(113, 20);
			this.tedValueMax.StyleController = this.lcColorScale2;
			this.tedValueMax.TabIndex = 8;
			this.tedValueMin.Location = new System.Drawing.Point(51, 45);
			this.tedValueMin.Name = "tedValueMin";
			this.tedValueMin.Size = new System.Drawing.Size(105, 20);
			this.tedValueMin.StyleController = this.lcColorScale2;
			this.tedValueMin.TabIndex = 7;
			this.lcgColorScale.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.lcgColorScale.GroupBordersVisible = false;
			this.lcgColorScale.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.emptySpaceItem1,
			this.lciTypeMax,
			this.lciTypeMin,
			this.lciValueMin,
			this.lciValueMax,
			this.lciPreview,
			this.sliMinimum,
			this.sliMaximum,
			this.emptySpaceItem3,
			this.emptySpaceItem4,
			this.lciColorMin,
			this.lciColorMax});
			this.lcgColorScale.Location = new System.Drawing.Point(0, 0);
			this.lcgColorScale.Name = "Root";
			this.lcgColorScale.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 2);
			this.lcgColorScale.Size = new System.Drawing.Size(470, 123);
			this.lcgColorScale.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(156, 17);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(146, 72);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.lciTypeMax.Control = this.cmbTypeMax;
			this.lciTypeMax.Location = new System.Drawing.Point(302, 17);
			this.lciTypeMax.Name = "lciTypeMax";
			this.lciTypeMax.Size = new System.Drawing.Size(164, 24);
			this.lciTypeMax.Text = "Type:";
			this.lciTypeMax.TextSize = new System.Drawing.Size(44, 13);
			this.lciTypeMin.Control = this.cmbTypeMin;
			this.lciTypeMin.Location = new System.Drawing.Point(0, 17);
			this.lciTypeMin.Name = "lciTypeMin";
			this.lciTypeMin.Size = new System.Drawing.Size(156, 24);
			this.lciTypeMin.Text = "Type:";
			this.lciTypeMin.TextSize = new System.Drawing.Size(44, 13);
			this.lciValueMin.Control = this.tedValueMin;
			this.lciValueMin.Location = new System.Drawing.Point(0, 41);
			this.lciValueMin.Name = "lciValueMin";
			this.lciValueMin.Size = new System.Drawing.Size(156, 24);
			this.lciValueMin.Text = "Value:";
			this.lciValueMin.TextSize = new System.Drawing.Size(44, 13);
			this.lciValueMax.Control = this.tedValueMax;
			this.lciValueMax.Location = new System.Drawing.Point(302, 41);
			this.lciValueMax.Name = "lciValueMax";
			this.lciValueMax.Size = new System.Drawing.Size(164, 24);
			this.lciValueMax.Text = "Value:";
			this.lciValueMax.TextSize = new System.Drawing.Size(44, 13);
			this.lciPreview.Control = this.pctPreview;
			this.lciPreview.Location = new System.Drawing.Point(0, 89);
			this.lciPreview.Name = "lciPreview";
			this.lciPreview.Size = new System.Drawing.Size(466, 30);
			this.lciPreview.Text = "Preview:";
			this.lciPreview.TextSize = new System.Drawing.Size(44, 13);
			this.sliMinimum.AllowHotTrack = false;
			this.sliMinimum.Location = new System.Drawing.Point(50, 0);
			this.sliMinimum.Name = "sliMinimum";
			this.sliMinimum.Size = new System.Drawing.Size(106, 17);
			this.sliMinimum.Text = "Minimum";
			this.sliMinimum.TextSize = new System.Drawing.Size(44, 13);
			this.sliMaximum.AllowHotTrack = false;
			this.sliMaximum.Location = new System.Drawing.Point(352, 0);
			this.sliMaximum.Name = "sliMaximum";
			this.sliMaximum.Size = new System.Drawing.Size(114, 17);
			this.sliMaximum.Text = "Maximum";
			this.sliMaximum.TextSize = new System.Drawing.Size(44, 13);
			this.emptySpaceItem3.AllowHotTrack = false;
			this.emptySpaceItem3.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceItem3.Name = "emptySpaceItem3";
			this.emptySpaceItem3.Size = new System.Drawing.Size(50, 17);
			this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem4.AllowHotTrack = false;
			this.emptySpaceItem4.Location = new System.Drawing.Point(156, 0);
			this.emptySpaceItem4.Name = "emptySpaceItem4";
			this.emptySpaceItem4.Size = new System.Drawing.Size(196, 17);
			this.emptySpaceItem4.TextSize = new System.Drawing.Size(0, 0);
			this.lciColorMin.Control = this.clrpckColorMin;
			this.lciColorMin.Location = new System.Drawing.Point(0, 65);
			this.lciColorMin.Name = "lciColorMin";
			this.lciColorMin.Size = new System.Drawing.Size(156, 24);
			this.lciColorMin.Text = "Color:";
			this.lciColorMin.TextSize = new System.Drawing.Size(44, 13);
			this.lciColorMax.Control = this.clrpckColorMax;
			this.lciColorMax.Location = new System.Drawing.Point(302, 65);
			this.lciColorMax.Name = "lciColorMax";
			this.lciColorMax.Size = new System.Drawing.Size(164, 24);
			this.lciColorMax.Text = "Color:";
			this.lciColorMax.TextSize = new System.Drawing.Size(44, 13);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lcColorScale2);
			this.Name = "ColorScale2Control";
			this.Size = new System.Drawing.Size(470, 123);
			((System.ComponentModel.ISupportInitialize)(this.lcColorScale2)).EndInit();
			this.lcColorScale2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.clrpckColorMax.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.clrpckColorMin.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pctPreview.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbTypeMin.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cmbTypeMax.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tedValueMax.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tedValueMin.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lcgColorScale)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciTypeMax)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciTypeMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciValueMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciValueMax)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sliMinimum)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.sliMaximum)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciColorMin)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lciColorMax)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraLayout.LayoutControl lcColorScale2;
		private DevExpress.XtraLayout.LayoutControlGroup lcgColorScale;
		private DevExpress.XtraEditors.PictureEdit pctPreview;
		private DevExpress.XtraEditors.ComboBoxEdit cmbTypeMin;
		private DevExpress.XtraEditors.ComboBoxEdit cmbTypeMax;
		private DevExpress.XtraEditors.TextEdit tedValueMax;
		private DevExpress.XtraEditors.TextEdit tedValueMin;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem1;
		private DevExpress.XtraLayout.LayoutControlItem lciTypeMax;
		private DevExpress.XtraLayout.LayoutControlItem lciTypeMin;
		private DevExpress.XtraLayout.LayoutControlItem lciValueMin;
		private DevExpress.XtraLayout.LayoutControlItem lciValueMax;
		private DevExpress.XtraLayout.LayoutControlItem lciPreview;
		private DevExpress.XtraLayout.SimpleLabelItem sliMinimum;
		private DevExpress.XtraLayout.SimpleLabelItem sliMaximum;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem3;
		private DevExpress.XtraLayout.EmptySpaceItem emptySpaceItem4;
		private ColorPickEdit clrpckColorMax;
		private ColorPickEdit clrpckColorMin;
		private XtraLayout.LayoutControlItem lciColorMin;
		private XtraLayout.LayoutControlItem lciColorMax;
	}
}
