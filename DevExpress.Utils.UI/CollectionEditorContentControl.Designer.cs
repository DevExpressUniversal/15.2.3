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

namespace DevExpress.Utils.UI {
	partial class CollectionEditorContentControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CollectionEditorContentControl));
			DevExpress.XtraLayout.ColumnDefinition columnDefinition9 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition10 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition11 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition5 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition8 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition2 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition3 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition4 = new DevExpress.XtraLayout.RowDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition1 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition2 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition3 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition4 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition5 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition6 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.ColumnDefinition columnDefinition7 = new DevExpress.XtraLayout.ColumnDefinition();
			DevExpress.XtraLayout.RowDefinition rowDefinition1 = new DevExpress.XtraLayout.RowDefinition();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.buttonDown = new DevExpress.XtraEditors.SimpleButton();
			this.buttonAdd = new DevExpress.XtraEditors.SimpleButton();
			this.buttonRemove = new DevExpress.XtraEditors.SimpleButton();
			this.buttonUp = new DevExpress.XtraEditors.SimpleButton();
			this.propertyGrid = new DevExpress.XtraReports.Design.PropertyGridUserControl();
			this.tv = new DevExpress.XtraTreeList.Native.XtraTreeView();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.grpButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItemAdd = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItemRemove = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItemDown = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItemUp = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.tv)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemAdd)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemRemove)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDown)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemUp)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			this.SuspendLayout();
			this.layoutControl1.AllowCustomization = false;
			this.layoutControl1.Controls.Add(this.buttonDown);
			this.layoutControl1.Controls.Add(this.buttonAdd);
			this.layoutControl1.Controls.Add(this.buttonRemove);
			this.layoutControl1.Controls.Add(this.buttonUp);
			this.layoutControl1.Controls.Add(this.propertyGrid);
			this.layoutControl1.Controls.Add(this.tv);
			resources.ApplyResources(this.layoutControl1, "layoutControl1");
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(800, 118, 1006, 727);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.buttonDown.Image = ((System.Drawing.Image)(resources.GetObject("buttonDown.Image")));
			this.buttonDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.buttonDown, "buttonDown");
			this.buttonDown.Name = "buttonDown";
			this.buttonDown.StyleController = this.layoutControl1;
			this.buttonDown.Click += new System.EventHandler(this.buttonDown_Click);
			this.buttonAdd.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleRight;
			resources.ApplyResources(this.buttonAdd, "buttonAdd");
			this.buttonAdd.Name = "buttonAdd";
			this.buttonAdd.StyleController = this.layoutControl1;
			this.buttonAdd.Click += new System.EventHandler(this.buttonAdd_Click);
			resources.ApplyResources(this.buttonRemove, "buttonRemove");
			this.buttonRemove.Name = "buttonRemove";
			this.buttonRemove.StyleController = this.layoutControl1;
			this.buttonRemove.Click += new System.EventHandler(this.buttonRemove_Click);
			this.buttonUp.Image = ((System.Drawing.Image)(resources.GetObject("buttonUp.Image")));
			this.buttonUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			resources.ApplyResources(this.buttonUp, "buttonUp");
			this.buttonUp.Name = "buttonUp";
			this.buttonUp.StyleController = this.layoutControl1;
			this.buttonUp.Click += new System.EventHandler(this.buttonUp_Click);
			this.propertyGrid.AllowGlyphSkinning = false;
			resources.ApplyResources(this.propertyGrid, "propertyGrid");
			this.propertyGrid.Name = "propertyGrid";
			this.propertyGrid.SelectedObject = null;
			this.propertyGrid.SelectedObjects = new object[0];
			this.propertyGrid.ServiceProvider = null;
			this.propertyGrid.ShowCategories = true;
			this.propertyGrid.ShowDescription = true;
			this.tv.DraggedNode = null;
			resources.ApplyResources(this.tv, "tv");
			this.tv.Name = "tv";
			this.tv.OptionsBehavior.Editable = false;
			this.tv.OptionsPrint.PrintHorzLines = false;
			this.tv.OptionsPrint.PrintVertLines = false;
			this.tv.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.tv.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.None;
			this.tv.OptionsView.ShowColumns = false;
			this.tv.OptionsView.ShowHorzLines = false;
			this.tv.OptionsView.ShowIndicator = false;
			this.tv.OptionsView.ShowVertLines = false;
			this.tv.AfterFocusNode += new DevExpress.XtraTreeList.NodeEventHandler(this.tv_AfterSelect);
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlGroup3,
			this.layoutControlItem4});
			this.layoutControlGroup1.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			columnDefinition9.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition9.Width = 48D;
			columnDefinition10.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition10.Width = 2D;
			columnDefinition11.SizeType = System.Windows.Forms.SizeType.Percent;
			columnDefinition11.Width = 52D;
			this.layoutControlGroup1.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition9,
			columnDefinition10,
			columnDefinition11});
			rowDefinition5.Height = 100D;
			rowDefinition5.SizeType = System.Windows.Forms.SizeType.Percent;
			this.layoutControlGroup1.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition5});
			this.layoutControlGroup1.Size = new System.Drawing.Size(518, 305);
			this.layoutControlGroup1.TextVisible = false;
			this.layoutControlGroup3.GroupBordersVisible = false;
			this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem3,
			this.grpButtons});
			this.layoutControlGroup3.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.layoutControlGroup3.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup3.Name = "layoutControlGroup3";
			columnDefinition8.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition8.Width = 238D;
			this.layoutControlGroup3.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition8});
			rowDefinition2.Height = 100D;
			rowDefinition2.SizeType = System.Windows.Forms.SizeType.Percent;
			rowDefinition3.Height = 5D;
			rowDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
			rowDefinition4.Height = 26D;
			rowDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.layoutControlGroup3.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition2,
			rowDefinition3,
			rowDefinition4});
			this.layoutControlGroup3.Size = new System.Drawing.Size(238, 285);
			this.layoutControlItem3.Control = this.tv;
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(238, 254);
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextVisible = false;
			this.grpButtons.GroupBordersVisible = false;
			this.grpButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItemAdd,
			this.layoutControlItemRemove,
			this.layoutControlItemDown,
			this.layoutControlItemUp,
			this.emptySpaceItem1});
			this.grpButtons.LayoutMode = DevExpress.XtraLayout.Utils.LayoutMode.Table;
			this.grpButtons.Location = new System.Drawing.Point(0, 259);
			this.grpButtons.Name = "grpButtons";
			this.grpButtons.OptionsTableLayoutGroup.AutoSizeDefaultDefinitionLength = 8;
			columnDefinition1.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition1.Width = 80D;
			columnDefinition2.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition2.Width = 4D;
			columnDefinition3.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition3.Width = 80D;
			columnDefinition4.SizeType = System.Windows.Forms.SizeType.AutoSize;
			columnDefinition4.Width = 10D;
			columnDefinition5.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition5.Width = 30D;
			columnDefinition6.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition6.Width = 4D;
			columnDefinition7.SizeType = System.Windows.Forms.SizeType.Absolute;
			columnDefinition7.Width = 30D;
			this.grpButtons.OptionsTableLayoutGroup.ColumnDefinitions.AddRange(new DevExpress.XtraLayout.ColumnDefinition[] {
			columnDefinition1,
			columnDefinition2,
			columnDefinition3,
			columnDefinition4,
			columnDefinition5,
			columnDefinition6,
			columnDefinition7});
			rowDefinition1.Height = 26D;
			rowDefinition1.SizeType = System.Windows.Forms.SizeType.AutoSize;
			this.grpButtons.OptionsTableLayoutGroup.RowDefinitions.AddRange(new DevExpress.XtraLayout.RowDefinition[] {
			rowDefinition1});
			this.grpButtons.OptionsTableLayoutItem.RowIndex = 2;
			this.grpButtons.Size = new System.Drawing.Size(238, 26);
			this.layoutControlItemAdd.Control = this.buttonAdd;
			this.layoutControlItemAdd.Location = new System.Drawing.Point(0, 0);
			this.layoutControlItemAdd.Name = "layoutControlItemAdd";
			this.layoutControlItemAdd.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItemAdd.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItemAdd.TextVisible = false;
			this.layoutControlItemRemove.Control = this.buttonRemove;
			this.layoutControlItemRemove.Location = new System.Drawing.Point(84, 0);
			this.layoutControlItemRemove.Name = "layoutControlItemRemove";
			this.layoutControlItemRemove.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItemRemove.Size = new System.Drawing.Size(80, 26);
			this.layoutControlItemRemove.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItemRemove.TextVisible = false;
			this.layoutControlItemDown.Control = this.buttonDown;
			this.layoutControlItemDown.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItemDown.Location = new System.Drawing.Point(208, 0);
			this.layoutControlItemDown.Name = "layoutControlItemDown";
			this.layoutControlItemDown.OptionsTableLayoutItem.ColumnIndex = 6;
			this.layoutControlItemDown.Size = new System.Drawing.Size(30, 26);
			this.layoutControlItemDown.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItemDown.TextVisible = false;
			this.layoutControlItemDown.TrimClientAreaToControl = false;
			this.layoutControlItemUp.Control = this.buttonUp;
			this.layoutControlItemUp.ControlAlignment = System.Drawing.ContentAlignment.MiddleCenter;
			this.layoutControlItemUp.Location = new System.Drawing.Point(174, 0);
			this.layoutControlItemUp.Name = "layoutControlItemUp";
			this.layoutControlItemUp.OptionsTableLayoutItem.ColumnIndex = 4;
			this.layoutControlItemUp.Size = new System.Drawing.Size(30, 26);
			this.layoutControlItemUp.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItemUp.TextVisible = false;
			this.layoutControlItemUp.TrimClientAreaToControl = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.Location = new System.Drawing.Point(164, 0);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.OptionsTableLayoutItem.ColumnIndex = 3;
			this.emptySpaceItem1.Size = new System.Drawing.Size(10, 26);
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.Control = this.propertyGrid;
			this.layoutControlItem4.Location = new System.Drawing.Point(240, 0);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.OptionsTableLayoutItem.ColumnIndex = 2;
			this.layoutControlItem4.Size = new System.Drawing.Size(258, 285);
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextVisible = false;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.layoutControl1);
			this.MinimumSize = new System.Drawing.Size(518, 220);
			this.Name = "CollectionEditorContentControl";
			this.Load += new System.EventHandler(this.CollectionEditorContentControl_Load);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.tv)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.grpButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemAdd)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemRemove)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemDown)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItemUp)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		public DevExpress.XtraReports.Design.PropertyGridUserControl propertyGrid;
		public DevExpress.XtraTreeList.Native.XtraTreeView tv;
		protected DevExpress.XtraEditors.SimpleButton buttonDown;
		protected DevExpress.XtraEditors.SimpleButton buttonAdd;
		protected DevExpress.XtraEditors.SimpleButton buttonRemove;
		protected DevExpress.XtraEditors.SimpleButton buttonUp;
		protected DevExpress.XtraLayout.LayoutControl layoutControl1;
		protected DevExpress.XtraLayout.LayoutControlGroup layoutControlGroup1;
		private DevExpress.XtraLayout.LayoutControlItem layoutControlItem3;
		protected DevExpress.XtraLayout.LayoutControlGroup grpButtons;
		protected DevExpress.XtraLayout.LayoutControlItem layoutControlItemAdd;
		protected DevExpress.XtraLayout.LayoutControlItem layoutControlItemRemove;
		protected DevExpress.XtraLayout.LayoutControlItem layoutControlItemUp;
		protected DevExpress.XtraLayout.LayoutControlItem layoutControlItemDown;
		private XtraLayout.LayoutControlGroup layoutControlGroup3;
		protected XtraLayout.EmptySpaceItem emptySpaceItem1;
		private XtraLayout.LayoutControlItem layoutControlItem4;
	}
}
