#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

namespace DevExpress.DashboardWin.Native {
	partial class ScatterChartPointLabelOptionsForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScatterChartPointLabelOptionsForm));
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.lcOptions = new DevExpress.XtraLayout.LayoutControl();
			this.cbPosition = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbOrientation = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbOverlappingMode = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbContentType = new DevExpress.XtraEditors.ComboBoxEdit();
			this.Root = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.ceShowPointLabels = new DevExpress.XtraEditors.CheckEdit();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.lcOptions)).BeginInit();
			this.lcOptions.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOrientation.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOverlappingMode.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbContentType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.Root)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowPointLabels.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.Controls.Add(this.lcOptions);
			this.layoutControl1.Controls.Add(this.btnCancel);
			this.layoutControl1.Controls.Add(this.btnOK);
			this.layoutControl1.Controls.Add(this.ceShowPointLabels);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(637, 29, 450, 350);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.lcOptions.Controls.Add(this.cbPosition);
			this.lcOptions.Controls.Add(this.cbOrientation);
			this.lcOptions.Controls.Add(this.cbOverlappingMode);
			this.lcOptions.Controls.Add(this.cbContentType);
			resources.ApplyResources(this.lcOptions, "lcOptions");
			this.lcOptions.Name = "lcOptions";
			this.lcOptions.Root = this.Root;
			resources.ApplyResources(this.cbPosition, "cbPosition");
			this.cbPosition.Name = "cbPosition";
			this.cbPosition.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbPosition.Properties.Buttons"))))});
			this.cbPosition.Properties.Items.AddRange(new object[] {
			resources.GetString("cbPosition.Properties.Items"),
			resources.GetString("cbPosition.Properties.Items1")});
			this.cbPosition.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPosition.StyleController = this.lcOptions;
			this.cbPosition.SelectedIndexChanged += new System.EventHandler(this.cbPosition_SelectedIndexChanged);
			resources.ApplyResources(this.cbOrientation, "cbOrientation");
			this.cbOrientation.Name = "cbOrientation";
			this.cbOrientation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbOrientation.Properties.Buttons"))))});
			this.cbOrientation.Properties.Items.AddRange(new object[] {
			resources.GetString("cbOrientation.Properties.Items"),
			resources.GetString("cbOrientation.Properties.Items1"),
			resources.GetString("cbOrientation.Properties.Items2")});
			this.cbOrientation.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbOrientation.StyleController = this.lcOptions;
			this.cbOrientation.SelectedIndexChanged += new System.EventHandler(this.cbOrientation_SelectedIndexChanged);
			resources.ApplyResources(this.cbOverlappingMode, "cbOverlappingMode");
			this.cbOverlappingMode.Name = "cbOverlappingMode";
			this.cbOverlappingMode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbOverlappingMode.Properties.Buttons"))))});
			this.cbOverlappingMode.Properties.Items.AddRange(new object[] {
			resources.GetString("cbOverlappingMode.Properties.Items"),
			resources.GetString("cbOverlappingMode.Properties.Items1"),
			resources.GetString("cbOverlappingMode.Properties.Items2")});
			this.cbOverlappingMode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbOverlappingMode.StyleController = this.lcOptions;
			this.cbOverlappingMode.SelectedIndexChanged += new System.EventHandler(this.cbOverlappingMode_SelectedIndexChanged);
			resources.ApplyResources(this.cbContentType, "cbContentType");
			this.cbContentType.Name = "cbContentType";
			this.cbContentType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbContentType.Properties.Buttons"))))});
			this.cbContentType.Properties.Items.AddRange(new object[] {
			resources.GetString("cbContentType.Properties.Items"),
			resources.GetString("cbContentType.Properties.Items1"),
			resources.GetString("cbContentType.Properties.Items2"),
			resources.GetString("cbContentType.Properties.Items3"),
			resources.GetString("cbContentType.Properties.Items4")});
			this.cbContentType.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbContentType.StyleController = this.lcOptions;
			this.cbContentType.SelectedIndexChanged += new System.EventHandler(this.cbContentType_SelectedIndexChanged);
			this.Root.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.Root.GroupBordersVisible = false;
			this.Root.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem4,
			this.layoutControlItem3,
			this.layoutControlItem2,
			this.layoutControlItem8});
			this.Root.Location = new System.Drawing.Point(0, 0);
			this.Root.Name = "Root";
			this.Root.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.Root.Size = new System.Drawing.Size(410, 112);
			this.Root.TextVisible = false;
			this.layoutControlItem4.Control = this.cbOrientation;
			resources.ApplyResources(this.layoutControlItem4, "layoutControlItem4");
			this.layoutControlItem4.Location = new System.Drawing.Point(0, 48);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.layoutControlItem4.Size = new System.Drawing.Size(410, 24);
			this.layoutControlItem4.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.layoutControlItem4.TextSize = new System.Drawing.Size(110, 13);
			this.layoutControlItem4.TextToControlDistance = 10;
			this.layoutControlItem3.Control = this.cbOverlappingMode;
			resources.ApplyResources(this.layoutControlItem3, "layoutControlItem3");
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 24);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.layoutControlItem3.Size = new System.Drawing.Size(410, 24);
			this.layoutControlItem3.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.layoutControlItem3.TextSize = new System.Drawing.Size(110, 13);
			this.layoutControlItem3.TextToControlDistance = 10;
			this.layoutControlItem2.Control = this.cbContentType;
			resources.ApplyResources(this.layoutControlItem2, "layoutControlItem2");
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.layoutControlItem2.Size = new System.Drawing.Size(410, 24);
			this.layoutControlItem2.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.layoutControlItem2.TextSize = new System.Drawing.Size(110, 13);
			this.layoutControlItem2.TextToControlDistance = 10;
			this.layoutControlItem8.Control = this.cbPosition;
			resources.ApplyResources(this.layoutControlItem8, "layoutControlItem8");
			this.layoutControlItem8.Location = new System.Drawing.Point(0, 72);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 2, 2);
			this.layoutControlItem8.Size = new System.Drawing.Size(410, 40);
			this.layoutControlItem8.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.layoutControlItem8.TextSize = new System.Drawing.Size(110, 13);
			this.layoutControlItem8.TextToControlDistance = 10;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl1;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.StyleController = this.layoutControl1;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			resources.ApplyResources(this.ceShowPointLabels, "ceShowPointLabels");
			this.ceShowPointLabels.Name = "ceShowPointLabels";
			this.ceShowPointLabels.Properties.Caption = resources.GetString("ceShowPointLabels.Properties.Caption");
			this.ceShowPointLabels.StyleController = this.layoutControl1;
			this.ceShowPointLabels.CheckedChanged += new System.EventHandler(this.ceShowPointLabels_CheckedChanged);
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.emptySpaceItem1,
			this.emptySpaceItem2,
			this.layoutControlItem5,
			this.layoutControlItem6,
			this.layoutControlItem7});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Size = new System.Drawing.Size(430, 250);
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlItem1.Control = this.ceShowPointLabels;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(25, 25, 25, 20);
			this.layoutControlItem1.Size = new System.Drawing.Size(410, 64);
			resources.ApplyResources(this.layoutControlItem1, "layoutControlItem1");
			this.layoutControlItem1.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.CustomSize;
			this.layoutControlItem1.TextSize = new System.Drawing.Size(110, 13);
			this.layoutControlItem1.TextToControlDistance = 10;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 176);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(260, 54);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(260, 176);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(150, 28);
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.Control = this.btnOK;
			this.layoutControlItem5.Location = new System.Drawing.Point(260, 204);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Size = new System.Drawing.Size(75, 26);
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlItem6.Control = this.btnCancel;
			this.layoutControlItem6.Location = new System.Drawing.Point(335, 204);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Size = new System.Drawing.Size(75, 26);
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem7.Control = this.lcOptions;
			this.layoutControlItem7.Location = new System.Drawing.Point(0, 64);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem7.Size = new System.Drawing.Size(410, 112);
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextVisible = false;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "ScatterChartPointLabelOptionsForm";
			this.ShowIcon = false;
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.lcOptions)).EndInit();
			this.lcOptions.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbPosition.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOrientation.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbOverlappingMode.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbContentType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.Root)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ceShowPointLabels.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		private XtraLayout.LayoutControl layoutControl1;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraEditors.CheckEdit ceShowPointLabels;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
		private XtraLayout.EmptySpaceItem emptySpaceItem2;
		private XtraEditors.SimpleButton btnOK;
		private XtraLayout.LayoutControlItem layoutControlItem5;
		private XtraEditors.SimpleButton btnCancel;
		private XtraLayout.LayoutControlItem layoutControlItem6;
		private XtraLayout.LayoutControl lcOptions;
		private XtraLayout.LayoutControlGroup Root;
		private XtraLayout.LayoutControlItem layoutControlItem7;
		private XtraEditors.ComboBoxEdit cbOrientation;
		private XtraEditors.ComboBoxEdit cbOverlappingMode;
		private XtraEditors.ComboBoxEdit cbContentType;
		private XtraLayout.LayoutControlItem layoutControlItem4;
		private XtraLayout.LayoutControlItem layoutControlItem3;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraEditors.ComboBoxEdit cbPosition;
		private XtraLayout.LayoutControlItem layoutControlItem8;
	}
}
