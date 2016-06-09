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
	partial class NewColorRecordDialog {
		private XtraEditors.LabelControl labelControl1;
		private XtraEditors.SimpleButton btnOK;
		private XtraEditors.SimpleButton btnCancel;
		private System.ComponentModel.IContainer components = null;
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewColorRecordDialog));
			this.btnRemove = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.dataFieldBrowser = new DevExpress.DashboardWin.Native.DataFieldsBrowser();
			this.cbSummaryType = new DevExpress.XtraEditors.ImageComboBoxEdit();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.btnAdd = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.lbMeasureDefinitions = new DevExpress.XtraEditors.ListBoxControl();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem8 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem9 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbSummaryType.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lbMeasureDefinitions)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).BeginInit();
			this.SuspendLayout();
			resources.ApplyResources(this.btnRemove, "btnRemove");
			this.btnRemove.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnRemove.Name = "btnRemove";
			this.btnRemove.StyleController = this.layoutControl1;
			this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
			this.layoutControl1.AllowCustomization = false;
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Controls.Add(this.labelControl2);
			this.layoutControl1.Controls.Add(this.dataFieldBrowser);
			this.layoutControl1.Controls.Add(this.cbSummaryType);
			this.layoutControl1.Controls.Add(this.btnRemove);
			this.layoutControl1.Controls.Add(this.labelControl1);
			this.layoutControl1.Controls.Add(this.btnAdd);
			this.layoutControl1.Controls.Add(this.btnOK);
			this.layoutControl1.Controls.Add(this.btnCancel);
			this.layoutControl1.Controls.Add(this.lbMeasureDefinitions);
			this.layoutControl1.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem4});
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(640, 122, 1098, 794);
			this.layoutControl1.Root = this.layoutControlGroup1;
			resources.ApplyResources(this.labelControl2, "labelControl2");
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.StyleController = this.layoutControl1;
			this.dataFieldBrowser.AllowGlyphSkinning = false;
			resources.ApplyResources(this.dataFieldBrowser, "dataFieldBrowser");
			this.dataFieldBrowser.Name = "dataFieldBrowser";
			resources.ApplyResources(this.cbSummaryType, "cbSummaryType");
			this.cbSummaryType.Name = "cbSummaryType";
			this.cbSummaryType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbSummaryType.Properties.Buttons"))))});
			this.cbSummaryType.StyleController = this.layoutControl1;
			this.cbSummaryType.EditValueChanged += new System.EventHandler(this.cbSummaryType_EditValueChanged);
			resources.ApplyResources(this.labelControl1, "labelControl1");
			this.labelControl1.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.labelControl1.LineVisible = true;
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.StyleController = this.layoutControl1;
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.StyleController = this.layoutControl1;
			this.btnAdd.Click += new System.EventHandler(this.OnBtnAddClick);
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.StyleController = this.layoutControl1;
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl1;
			resources.ApplyResources(this.lbMeasureDefinitions, "lbMeasureDefinitions");
			this.lbMeasureDefinitions.Name = "lbMeasureDefinitions";
			this.lbMeasureDefinitions.StyleController = this.layoutControl1;
			this.lbMeasureDefinitions.SelectedIndexChanged += new System.EventHandler(this.OnMeasureDefinitionsSelectedIndexChanged);
			this.layoutControlItem4.Control = this.labelControl1;
			this.layoutControlItem4.Location = new System.Drawing.Point(0, 225);
			this.layoutControlItem4.MaxSize = new System.Drawing.Size(0, 17);
			this.layoutControlItem4.MinSize = new System.Drawing.Size(14, 17);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Size = new System.Drawing.Size(487, 17);
			this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.emptySpaceItem1,
			this.layoutControlItem2,
			this.layoutControlItem3,
			this.layoutControlGroup2});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 11, 10);
			this.layoutControlGroup1.Size = new System.Drawing.Size(457, 313);
			this.layoutControlGroup1.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 251);
			this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 41);
			this.emptySpaceItem1.MinSize = new System.Drawing.Size(104, 41);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(286, 41);
			this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.Control = this.btnOK;
			this.layoutControlItem2.Location = new System.Drawing.Point(286, 251);
			this.layoutControlItem2.MaxSize = new System.Drawing.Size(75, 41);
			this.layoutControlItem2.MinSize = new System.Drawing.Size(75, 41);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 17, 2);
			this.layoutControlItem2.Size = new System.Drawing.Size(75, 41);
			this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			this.layoutControlItem3.Control = this.btnCancel;
			this.layoutControlItem3.Location = new System.Drawing.Point(361, 251);
			this.layoutControlItem3.MaxSize = new System.Drawing.Size(76, 41);
			this.layoutControlItem3.MinSize = new System.Drawing.Size(76, 41);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 17, 2);
			this.layoutControlItem3.Size = new System.Drawing.Size(76, 41);
			this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlGroup2.GroupBordersVisible = false;
			this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem6,
			this.layoutControlItem1,
			this.layoutControlItem5,
			this.emptySpaceItem3,
			this.layoutControlItem7,
			this.layoutControlItem8,
			this.emptySpaceItem2,
			this.layoutControlItem9});
			this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup2.Name = "layoutControlGroup2";
			this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup2.Size = new System.Drawing.Size(437, 251);
			this.layoutControlItem6.Control = this.labelControl2;
			this.layoutControlItem6.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem6.MaxSize = new System.Drawing.Size(54, 17);
			this.layoutControlItem6.MinSize = new System.Drawing.Size(54, 17);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Size = new System.Drawing.Size(437, 17);
			this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem1.Control = this.dataFieldBrowser;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 17);
			this.layoutControlItem1.MaxSize = new System.Drawing.Size(209, 0);
			this.layoutControlItem1.MinSize = new System.Drawing.Size(209, 5);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(209, 234);
			this.layoutControlItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlItem5.Control = this.lbMeasureDefinitions;
			this.layoutControlItem5.Location = new System.Drawing.Point(257, 17);
			this.layoutControlItem5.MaxSize = new System.Drawing.Size(180, 0);
			this.layoutControlItem5.MinSize = new System.Drawing.Size(180, 4);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Size = new System.Drawing.Size(180, 202);
			this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.emptySpaceItem3.AllowHotTrack = false;
			this.emptySpaceItem3.Location = new System.Drawing.Point(209, 17);
			this.emptySpaceItem3.MaxSize = new System.Drawing.Size(48, 22);
			this.emptySpaceItem3.MinSize = new System.Drawing.Size(48, 22);
			this.emptySpaceItem3.Name = "emptySpaceItem3";
			this.emptySpaceItem3.Size = new System.Drawing.Size(48, 22);
			this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.Control = this.btnAdd;
			this.layoutControlItem7.Location = new System.Drawing.Point(209, 39);
			this.layoutControlItem7.MaxSize = new System.Drawing.Size(48, 31);
			this.layoutControlItem7.MinSize = new System.Drawing.Size(48, 31);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 2, 2);
			this.layoutControlItem7.Size = new System.Drawing.Size(48, 31);
			this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextVisible = false;
			this.layoutControlItem8.Control = this.btnRemove;
			this.layoutControlItem8.Location = new System.Drawing.Point(209, 70);
			this.layoutControlItem8.MaxSize = new System.Drawing.Size(48, 31);
			this.layoutControlItem8.MinSize = new System.Drawing.Size(48, 31);
			this.layoutControlItem8.Name = "layoutControlItem8";
			this.layoutControlItem8.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 2, 2);
			this.layoutControlItem8.Size = new System.Drawing.Size(48, 31);
			this.layoutControlItem8.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem8.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem8.TextVisible = false;
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(209, 101);
			this.emptySpaceItem2.MaxSize = new System.Drawing.Size(48, 0);
			this.emptySpaceItem2.MinSize = new System.Drawing.Size(48, 145);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(48, 150);
			this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem9.Control = this.cbSummaryType;
			this.layoutControlItem9.Location = new System.Drawing.Point(257, 219);
			this.layoutControlItem9.MaxSize = new System.Drawing.Size(180, 32);
			this.layoutControlItem9.MinSize = new System.Drawing.Size(180, 32);
			this.layoutControlItem9.Name = "layoutControlItem9";
			this.layoutControlItem9.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 10, 2);
			this.layoutControlItem9.Size = new System.Drawing.Size(180, 32);
			this.layoutControlItem9.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			resources.ApplyResources(this.layoutControlItem9, "layoutControlItem9");
			this.layoutControlItem9.TextAlignMode = DevExpress.XtraLayout.TextAlignModeItem.AutoSize;
			this.layoutControlItem9.TextSize = new System.Drawing.Size(73, 13);
			this.layoutControlItem9.TextToControlDistance = 10;
			this.AcceptButton = this.btnOK;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.layoutControl1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "NewColorRecordDialog";
			this.ShowIcon = false;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnFormClosing);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.cbSummaryType.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lbMeasureDefinitions)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem8)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem9)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraLayout.LayoutControl layoutControl1;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraLayout.LayoutControlItem layoutControlItem4;
		private XtraEditors.ListBoxControl lbMeasureDefinitions;
		private XtraEditors.ImageComboBoxEdit cbSummaryType;
		private XtraEditors.SimpleButton btnAdd;
		private XtraEditors.SimpleButton btnRemove;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraLayout.LayoutControlItem layoutControlItem3;
		private DataFieldsBrowser dataFieldBrowser;
		private XtraLayout.LayoutControlItem layoutControlItem5;
		private XtraLayout.LayoutControlItem layoutControlItem7;
		private XtraLayout.LayoutControlItem layoutControlItem8;
		private XtraLayout.LayoutControlItem layoutControlItem9;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraEditors.LabelControl labelControl2;
		private XtraLayout.EmptySpaceItem emptySpaceItem2;
		private XtraLayout.EmptySpaceItem emptySpaceItem3;
		private XtraLayout.LayoutControlItem layoutControlItem6;
		private XtraLayout.LayoutControlGroup layoutControlGroup2;
	}
}
