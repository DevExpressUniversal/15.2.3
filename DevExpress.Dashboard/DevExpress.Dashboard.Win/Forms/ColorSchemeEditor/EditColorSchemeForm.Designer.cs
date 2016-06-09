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
	partial class EditColorSchemeForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EditColorSchemeForm));
			this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
			this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
			this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
			this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
			this.cbColorScheme = new DevExpress.XtraEditors.LookUpEdit();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.btnRemoveColorScheme = new DevExpress.XtraEditors.SimpleButton();
			this.btnApply = new DevExpress.XtraEditors.SimpleButton();
			this.btnNewValue = new DevExpress.XtraEditors.SimpleButton();
			this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
			this.btnNewColorScheme = new DevExpress.XtraEditors.SimpleButton();
			this.btnOK = new DevExpress.XtraEditors.SimpleButton();
			this.gcColors = new DevExpress.XtraGrid.GridControl();
			this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.Value = new DevExpress.XtraGrid.Columns.GridColumn();
			this.Color = new DevExpress.XtraGrid.Columns.GridColumn();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem6 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem7 = new DevExpress.XtraLayout.LayoutControlItem();
			this.liApply = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbColorScheme.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gcColors)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.liApply)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			this.SuspendLayout();
			this.barAndDockingController1.PropertiesBar.DefaultGlyphSize = new System.Drawing.Size(16, 16);
			this.barAndDockingController1.PropertiesBar.DefaultLargeGlyphSize = new System.Drawing.Size(32, 32);
			this.barManager1.Controller = this.barAndDockingController1;
			this.barManager1.DockControls.Add(this.barDockControlTop);
			this.barManager1.DockControls.Add(this.barDockControlBottom);
			this.barManager1.DockControls.Add(this.barDockControlLeft);
			this.barManager1.DockControls.Add(this.barDockControlRight);
			this.barManager1.Form = this;
			this.barManager1.MaxItemId = 0;
			this.barDockControlTop.CausesValidation = false;
			resources.ApplyResources(this.barDockControlTop, "barDockControlTop");
			this.barDockControlBottom.CausesValidation = false;
			resources.ApplyResources(this.barDockControlBottom, "barDockControlBottom");
			this.barDockControlLeft.CausesValidation = false;
			resources.ApplyResources(this.barDockControlLeft, "barDockControlLeft");
			this.barDockControlRight.CausesValidation = false;
			resources.ApplyResources(this.barDockControlRight, "barDockControlRight");
			resources.ApplyResources(this.cbColorScheme, "cbColorScheme");
			this.cbColorScheme.Name = "cbColorScheme";
			this.cbColorScheme.Properties.AutoHeight = ((bool)(resources.GetObject("cbColorScheme.Properties.AutoHeight")));
			this.cbColorScheme.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
			this.cbColorScheme.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbColorScheme.Properties.Buttons"))))});
			this.cbColorScheme.Properties.NullText = resources.GetString("cbColorScheme.Properties.NullText");
			this.cbColorScheme.Properties.ShowFooter = false;
			this.cbColorScheme.Properties.ShowHeader = false;
			this.cbColorScheme.Properties.ShowLines = false;
			this.cbColorScheme.Properties.UseDropDownRowsAsMaxCount = true;
			this.cbColorScheme.StyleController = this.layoutControl1;
			this.cbColorScheme.EditValueChanged += new System.EventHandler(this.OnSelectedColorSchemeChanged);
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.btnRemoveColorScheme);
			this.layoutControl1.Controls.Add(this.btnApply);
			this.layoutControl1.Controls.Add(this.btnNewValue);
			this.layoutControl1.Controls.Add(this.btnCancel);
			this.layoutControl1.Controls.Add(this.btnNewColorScheme);
			this.layoutControl1.Controls.Add(this.btnOK);
			this.layoutControl1.Controls.Add(this.gcColors);
			this.layoutControl1.Controls.Add(this.cbColorScheme);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(722, 148, 1178, 613);
			this.layoutControl1.OptionsPrint.AppearanceGroupCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("layoutControl1.OptionsPrint.AppearanceGroupCaption.BackColor")));
			this.layoutControl1.OptionsPrint.AppearanceGroupCaption.Font = ((System.Drawing.Font)(resources.GetObject("layoutControl1.OptionsPrint.AppearanceGroupCaption.Font")));
			this.layoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
			this.layoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
			this.layoutControl1.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
			this.layoutControl1.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.layoutControl1.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControl1.OptionsPrint.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControl1.OptionsPrint.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.layoutControl1.OptionsPrint.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControl1.Root = this.layoutControlGroup1;
			resources.ApplyResources(this.btnRemoveColorScheme, "btnRemoveColorScheme");
			this.btnRemoveColorScheme.Name = "btnRemoveColorScheme";
			this.btnRemoveColorScheme.StyleController = this.layoutControl1;
			this.btnRemoveColorScheme.Click += new System.EventHandler(this.btnRemoveColorScheme_Click);
			resources.ApplyResources(this.btnApply, "btnApply");
			this.btnApply.Name = "btnApply";
			this.btnApply.StyleController = this.layoutControl1;
			this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
			resources.ApplyResources(this.btnNewValue, "btnNewValue");
			this.btnNewValue.Name = "btnNewValue";
			this.btnNewValue.StyleController = this.layoutControl1;
			this.btnNewValue.Click += new System.EventHandler(this.btnNewValue_Click);
			resources.ApplyResources(this.btnCancel, "btnCancel");
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.StyleController = this.layoutControl1;
			resources.ApplyResources(this.btnNewColorScheme, "btnNewColorScheme");
			this.btnNewColorScheme.Name = "btnNewColorScheme";
			this.btnNewColorScheme.StyleController = this.layoutControl1;
			this.btnNewColorScheme.Click += new System.EventHandler(this.btnNewColorScheme_Click);
			resources.ApplyResources(this.btnOK, "btnOK");
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Name = "btnOK";
			this.btnOK.StyleController = this.layoutControl1;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			resources.ApplyResources(this.gcColors, "gcColors");
			this.gcColors.Cursor = System.Windows.Forms.Cursors.Default;
			this.gcColors.MainView = this.gridView1;
			this.gcColors.Name = "gcColors";
			this.gcColors.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridView1});
			this.gcColors.MouseUp += new System.Windows.Forms.MouseEventHandler(this.gcColors_MouseUp);
			this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.Value,
			this.Color});
			this.gridView1.GridControl = this.gcColors;
			this.gridView1.Name = "gridView1";
			this.gridView1.OptionsCustomization.AllowColumnMoving = false;
			this.gridView1.OptionsCustomization.AllowFilter = false;
			this.gridView1.OptionsCustomization.AllowGroup = false;
			this.gridView1.OptionsCustomization.AllowQuickHideColumns = false;
			this.gridView1.OptionsCustomization.AllowSort = false;
			this.gridView1.OptionsMenu.EnableColumnMenu = false;
			this.gridView1.OptionsView.ShowGroupPanel = false;
			this.gridView1.OptionsView.ShowIndicator = false;
			this.gridView1.CustomDrawEmptyForeground += new DevExpress.XtraGrid.Views.Base.CustomDrawEventHandler(this.OnCustomDrawEmptyForeground);
			this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.OnGridCellValueChanged);
			this.gridView1.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.OnGridCustomColumnDisplayText);
			resources.ApplyResources(this.Value, "Value");
			this.Value.Name = "Value";
			this.Value.OptionsColumn.AllowEdit = false;
			resources.ApplyResources(this.Color, "Color");
			this.Color.Name = "Color";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem6,
			this.layoutControlItem7,
			this.liApply,
			this.layoutControlItem1,
			this.layoutControlGroup2,
			this.emptySpaceItem1,
			this.layoutControlGroup3});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Padding = new DevExpress.XtraLayout.Utils.Padding(10, 10, 12, 10);
			this.layoutControlGroup1.Size = new System.Drawing.Size(563, 376);
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlItem6.Control = this.btnOK;
			this.layoutControlItem6.Location = new System.Drawing.Point(298, 328);
			this.layoutControlItem6.MaxSize = new System.Drawing.Size(82, 26);
			this.layoutControlItem6.MinSize = new System.Drawing.Size(82, 26);
			this.layoutControlItem6.Name = "layoutControlItem6";
			this.layoutControlItem6.Size = new System.Drawing.Size(82, 26);
			this.layoutControlItem6.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem6.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem6.TextVisible = false;
			this.layoutControlItem7.Control = this.btnCancel;
			this.layoutControlItem7.Location = new System.Drawing.Point(380, 328);
			this.layoutControlItem7.MaxSize = new System.Drawing.Size(82, 26);
			this.layoutControlItem7.MinSize = new System.Drawing.Size(82, 26);
			this.layoutControlItem7.Name = "layoutControlItem7";
			this.layoutControlItem7.Size = new System.Drawing.Size(82, 26);
			this.layoutControlItem7.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem7.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem7.TextVisible = false;
			this.liApply.Control = this.btnApply;
			this.liApply.Location = new System.Drawing.Point(462, 328);
			this.liApply.MaxSize = new System.Drawing.Size(81, 26);
			this.liApply.MinSize = new System.Drawing.Size(81, 26);
			this.liApply.Name = "liApply";
			this.liApply.Size = new System.Drawing.Size(81, 26);
			this.liApply.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.liApply.TextSize = new System.Drawing.Size(0, 0);
			this.liApply.TextVisible = false;
			this.layoutControlItem1.Control = this.gcColors;
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 26);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 10, 2);
			this.layoutControlItem1.Size = new System.Drawing.Size(543, 268);
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextVisible = false;
			this.layoutControlGroup2.GroupBordersVisible = false;
			this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem3,
			this.layoutControlItem4,
			this.layoutControlItem5});
			this.layoutControlGroup2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup2.Name = "layoutControlGroup2";
			this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup2.Size = new System.Drawing.Size(543, 26);
			this.layoutControlGroup2.TextVisible = false;
			this.layoutControlItem3.Control = this.cbColorScheme;
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem3.MaxSize = new System.Drawing.Size(0, 26);
			this.layoutControlItem3.MinSize = new System.Drawing.Size(54, 26);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(346, 26);
			this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem4.Control = this.btnNewColorScheme;
			this.layoutControlItem4.Location = new System.Drawing.Point(410, 0);
			this.layoutControlItem4.MaxSize = new System.Drawing.Size(133, 26);
			this.layoutControlItem4.MinSize = new System.Drawing.Size(133, 26);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Size = new System.Drawing.Size(133, 26);
			this.layoutControlItem4.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlItem5.Control = this.btnRemoveColorScheme;
			this.layoutControlItem5.Location = new System.Drawing.Point(346, 0);
			this.layoutControlItem5.MaxSize = new System.Drawing.Size(64, 26);
			this.layoutControlItem5.MinSize = new System.Drawing.Size(64, 26);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Size = new System.Drawing.Size(64, 26);
			this.layoutControlItem5.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 328);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(298, 26);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlGroup3.GroupBordersVisible = false;
			this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.emptySpaceItem2,
			this.layoutControlItem2});
			this.layoutControlGroup3.Location = new System.Drawing.Point(0, 294);
			this.layoutControlGroup3.Name = "layoutControlGroup3";
			this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup3.Size = new System.Drawing.Size(543, 34);
			this.layoutControlGroup3.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.Location = new System.Drawing.Point(94, 0);
			this.emptySpaceItem2.MaxSize = new System.Drawing.Size(323, 34);
			this.emptySpaceItem2.MinSize = new System.Drawing.Size(323, 34);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(449, 34);
			this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.Control = this.btnNewValue;
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem2.MaxSize = new System.Drawing.Size(94, 34);
			this.layoutControlItem2.MinSize = new System.Drawing.Size(94, 34);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 2, 2, 10);
			this.layoutControlItem2.Size = new System.Drawing.Size(94, 34);
			this.layoutControlItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextVisible = false;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnCancel;
			this.Controls.Add(this.layoutControl1);
			this.Controls.Add(this.barDockControlLeft);
			this.Controls.Add(this.barDockControlRight);
			this.Controls.Add(this.barDockControlBottom);
			this.Controls.Add(this.barDockControlTop);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "EditColorSchemeForm";
			this.ShowIcon = false;
			this.Load += new System.EventHandler(this.OnFormLoad);
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barManager1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbColorScheme.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gcColors)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem6)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem7)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.liApply)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraEditors.LookUpEdit cbColorScheme;
		private XtraEditors.SimpleButton btnOK;
		private XtraEditors.SimpleButton btnCancel;
		private XtraEditors.SimpleButton btnApply;
		private XtraGrid.GridControl gcColors;
		private XtraGrid.Views.Grid.GridView gridView1;
		private XtraGrid.Columns.GridColumn Value;
		private XtraGrid.Columns.GridColumn Color;
		private XtraEditors.SimpleButton btnNewColorScheme;
		private XtraEditors.SimpleButton btnNewValue;
		private XtraEditors.SimpleButton btnRemoveColorScheme;
		private XtraLayout.LayoutControl layoutControl1;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraLayout.LayoutControlItem layoutControlItem5;
		private XtraLayout.LayoutControlItem layoutControlItem6;
		private XtraLayout.LayoutControlItem layoutControlItem7;
		private XtraLayout.LayoutControlItem liApply;
		private XtraLayout.LayoutControlItem layoutControlItem3;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraLayout.LayoutControlItem layoutControlItem4;
		private XtraLayout.LayoutControlGroup layoutControlGroup2;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
		private XtraLayout.EmptySpaceItem emptySpaceItem2;
		private XtraLayout.LayoutControlGroup layoutControlGroup3;
		private XtraBars.BarManager barManager1;
		private XtraBars.BarAndDockingController barAndDockingController1;
		private XtraBars.BarDockControl barDockControlTop;
		private XtraBars.BarDockControl barDockControlBottom;
		private XtraBars.BarDockControl barDockControlLeft;
		private XtraBars.BarDockControl barDockControlRight;
	}
}
