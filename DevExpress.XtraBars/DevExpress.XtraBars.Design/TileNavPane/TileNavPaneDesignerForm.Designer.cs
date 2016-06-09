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

namespace DevExpress.XtraBars.Design {
	partial class TileNavPaneDesignerForm {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.tbCategories = new DevExpress.XtraBars.Navigation.TileBar();
			this.tbItems = new DevExpress.XtraBars.Navigation.TileBar();
			this.pgCategory = new System.Windows.Forms.PropertyGrid();
			this.tabControl = new DevExpress.XtraTab.XtraTabControl();
			this.tabCategory = new DevExpress.XtraTab.XtraTabPage();
			this.tabItem = new DevExpress.XtraTab.XtraTabPage();
			this.pgItem = new System.Windows.Forms.PropertyGrid();
			this.tabSubItems = new DevExpress.XtraTab.XtraTabPage();
			this.pgSubItem = new System.Windows.Forms.PropertyGrid();
			this.tbSubItems = new DevExpress.XtraBars.Navigation.TileBar();
			this.layoutControl1 = new DevExpress.XtraLayout.LayoutControl();
			this.subitemsTilesController = new DevExpress.XtraBars.Design.TileBarControlButtons();
			this.itemsTilesController = new DevExpress.XtraBars.Design.TileBarControlButtons();
			this.catTilesController = new DevExpress.XtraBars.Design.TileBarControlButtons();
			this.layoutControlItem10 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup1 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.splitterItem1 = new DevExpress.XtraLayout.SplitterItem();
			this.splitterItem2 = new DevExpress.XtraLayout.SplitterItem();
			this.layoutControlGroup2 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.emptySpaceItem3 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem3 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlItem5 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup3 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem2 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem2 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem4 = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlGroup4 = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutControlItem1 = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceItem1 = new DevExpress.XtraLayout.EmptySpaceItem();
			this.layoutControlItem17 = new DevExpress.XtraLayout.LayoutControlItem();
			this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).BeginInit();
			this.tabControl.SuspendLayout();
			this.tabCategory.SuspendLayout();
			this.tabItem.SuspendLayout();
			this.tabSubItems.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).BeginInit();
			this.layoutControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
			this.splitContainerControl1.SuspendLayout();
			this.SuspendLayout();
			this.tbCategories.AllowDrag = false;
			this.tbCategories.AllowSelectedItem = true;
			this.tbCategories.AppearanceText.Font = new System.Drawing.Font("Segoe UI Semilight", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbCategories.AppearanceText.Options.UseFont = true;
			this.tbCategories.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(225)))), ((int)(((byte)(230)))));
			this.tbCategories.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
			this.tbCategories.IndentBetweenGroups = 0;
			this.tbCategories.Location = new System.Drawing.Point(13, 39);
			this.tbCategories.Name = "tbCategories";
			this.tbCategories.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
			this.tbCategories.ShowText = true;
			this.tbCategories.Size = new System.Drawing.Size(866, 190);
			this.tbCategories.TabIndex = 0;
			this.tbCategories.Text = "Categories";
			this.tbCategories.SelectedItemChanged += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tbCategories_SelectedItemChanged);
			this.tbItems.AllowDrag = false;
			this.tbItems.AllowSelectedItem = true;
			this.tbItems.AppearanceText.Font = new System.Drawing.Font("Segoe UI Semilight", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbItems.AppearanceText.Options.UseFont = true;
			this.tbItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(225)))), ((int)(((byte)(230)))));
			this.tbItems.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
			this.tbItems.IndentBetweenGroups = 0;
			this.tbItems.Location = new System.Drawing.Point(13, 266);
			this.tbItems.Name = "tbItems";
			this.tbItems.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
			this.tbItems.ShowText = true;
			this.tbItems.Size = new System.Drawing.Size(866, 187);
			this.tbItems.TabIndex = 1;
			this.tbItems.Text = "Items";
			this.tbItems.SelectedItemChanged += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tbItems_SelectedItemChanged);
			this.pgCategory.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgCategory.Location = new System.Drawing.Point(0, 0);
			this.pgCategory.Name = "pgCategory";
			this.pgCategory.Size = new System.Drawing.Size(291, 662);
			this.pgCategory.TabIndex = 2;
			this.pgCategory.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.propertyGrid1_PropertyValueChanged);
			this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl.Location = new System.Drawing.Point(0, 0);
			this.tabControl.Name = "tabControl";
			this.tabControl.SelectedTabPage = this.tabCategory;
			this.tabControl.Size = new System.Drawing.Size(297, 690);
			this.tabControl.TabIndex = 3;
			this.tabControl.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[] {
			this.tabCategory,
			this.tabItem,
			this.tabSubItems});
			this.tabCategory.Controls.Add(this.pgCategory);
			this.tabCategory.Name = "tabCategory";
			this.tabCategory.Size = new System.Drawing.Size(291, 662);
			this.tabCategory.Text = "Category";
			this.tabItem.Controls.Add(this.pgItem);
			this.tabItem.Name = "tabItem";
			this.tabItem.Size = new System.Drawing.Size(291, 662);
			this.tabItem.Text = "Item";
			this.pgItem.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgItem.Location = new System.Drawing.Point(0, 0);
			this.pgItem.Name = "pgItem";
			this.pgItem.Size = new System.Drawing.Size(291, 662);
			this.pgItem.TabIndex = 0;
			this.pgItem.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgItem_PropertyValueChanged);
			this.tabSubItems.Controls.Add(this.pgSubItem);
			this.tabSubItems.Name = "tabSubItems";
			this.tabSubItems.Size = new System.Drawing.Size(291, 662);
			this.tabSubItems.Text = "SubItem";
			this.pgSubItem.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pgSubItem.Location = new System.Drawing.Point(0, 0);
			this.pgSubItem.Name = "pgSubItem";
			this.pgSubItem.Size = new System.Drawing.Size(291, 662);
			this.pgSubItem.TabIndex = 0;
			this.pgSubItem.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this.pgSubItem_PropertyValueChanged);
			this.tbSubItems.AllowDrag = false;
			this.tbSubItems.AllowSelectedItem = true;
			this.tbSubItems.AppearanceText.Font = new System.Drawing.Font("Segoe UI Semilight", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tbSubItems.AppearanceText.Options.UseFont = true;
			this.tbSubItems.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(223)))), ((int)(((byte)(225)))), ((int)(((byte)(230)))));
			this.tbSubItems.DropDownOptions.BeakColor = System.Drawing.Color.Empty;
			this.tbSubItems.IndentBetweenGroups = 0;
			this.tbSubItems.Location = new System.Drawing.Point(13, 490);
			this.tbSubItems.Name = "tbSubItems";
			this.tbSubItems.ScrollMode = DevExpress.XtraEditors.TileControlScrollMode.ScrollButtons;
			this.tbSubItems.ShowText = true;
			this.tbSubItems.Size = new System.Drawing.Size(866, 187);
			this.tbSubItems.TabIndex = 11;
			this.tbSubItems.Text = "Subitems";
			this.tbSubItems.SelectedItemChanged += new DevExpress.XtraEditors.TileItemClickEventHandler(this.tbSubItems_SelectedItemChanged);
			this.layoutControl1.Controls.Add(this.subitemsTilesController);
			this.layoutControl1.Controls.Add(this.itemsTilesController);
			this.layoutControl1.Controls.Add(this.catTilesController);
			this.layoutControl1.Controls.Add(this.tbSubItems);
			this.layoutControl1.Controls.Add(this.tbCategories);
			this.layoutControl1.Controls.Add(this.tbItems);
			this.layoutControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControl1.HiddenItems.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem10});
			this.layoutControl1.Location = new System.Drawing.Point(0, 0);
			this.layoutControl1.Name = "layoutControl1";
			this.layoutControl1.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(477, 353, 1195, 575);
			this.layoutControl1.Root = this.layoutControlGroup1;
			this.layoutControl1.Size = new System.Drawing.Size(892, 690);
			this.layoutControl1.TabIndex = 14;
			this.layoutControl1.Text = "layoutControl1";
			this.subitemsTilesController.Location = new System.Drawing.Point(115, 462);
			this.subitemsTilesController.Name = "subitemsTilesController";
			this.subitemsTilesController.Size = new System.Drawing.Size(766, 26);
			this.subitemsTilesController.TabIndex = 28;
			this.itemsTilesController.Location = new System.Drawing.Point(115, 238);
			this.itemsTilesController.Name = "itemsTilesController";
			this.itemsTilesController.Size = new System.Drawing.Size(766, 26);
			this.itemsTilesController.TabIndex = 27;
			this.catTilesController.Location = new System.Drawing.Point(109, 11);
			this.catTilesController.Name = "catTilesController";
			this.catTilesController.Size = new System.Drawing.Size(772, 26);
			this.catTilesController.TabIndex = 26;
			this.layoutControlItem10.CustomizationFormText = "layoutControlItem10";
			this.layoutControlItem10.Location = new System.Drawing.Point(0, 557);
			this.layoutControlItem10.Name = "layoutControlItem10";
			this.layoutControlItem10.Size = new System.Drawing.Size(872, 24);
			this.layoutControlItem10.Text = "layoutControlItem10";
			this.layoutControlItem10.TextSize = new System.Drawing.Size(50, 20);
			this.layoutControlItem10.TextToControlDistance = 5;
			this.layoutControlGroup1.CustomizationFormText = "Root";
			this.layoutControlGroup1.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroup1.GroupBordersVisible = false;
			this.layoutControlGroup1.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.splitterItem1,
			this.splitterItem2,
			this.layoutControlGroup2,
			this.layoutControlGroup3,
			this.layoutControlGroup4});
			this.layoutControlGroup1.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup1.Name = "Root";
			this.layoutControlGroup1.Size = new System.Drawing.Size(892, 690);
			this.layoutControlGroup1.Text = "Root";
			this.layoutControlGroup1.TextVisible = false;
			this.splitterItem1.AllowHotTrack = true;
			this.splitterItem1.CustomizationFormText = "splitterItem1";
			this.splitterItem1.Location = new System.Drawing.Point(0, 222);
			this.splitterItem1.Name = "splitterItem1";
			this.splitterItem1.Size = new System.Drawing.Size(872, 5);
			this.splitterItem2.AllowHotTrack = true;
			this.splitterItem2.CustomizationFormText = "splitterItem2";
			this.splitterItem2.Location = new System.Drawing.Point(0, 446);
			this.splitterItem2.Name = "splitterItem2";
			this.splitterItem2.Size = new System.Drawing.Size(872, 5);
			this.layoutControlGroup2.CustomizationFormText = "layoutControlGroup2";
			this.layoutControlGroup2.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.emptySpaceItem3,
			this.layoutControlItem3,
			this.layoutControlItem5});
			this.layoutControlGroup2.Location = new System.Drawing.Point(0, 451);
			this.layoutControlGroup2.Name = "layoutControlGroup2";
			this.layoutControlGroup2.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup2.Size = new System.Drawing.Size(872, 219);
			this.layoutControlGroup2.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup2.Text = "layoutControlGroup2";
			this.layoutControlGroup2.TextVisible = false;
			this.emptySpaceItem3.AllowHotTrack = false;
			this.emptySpaceItem3.CustomizationFormText = "emptySpaceItem3";
			this.emptySpaceItem3.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceItem3.MaxSize = new System.Drawing.Size(0, 26);
			this.emptySpaceItem3.MinSize = new System.Drawing.Size(104, 26);
			this.emptySpaceItem3.Name = "emptySpaceItem3";
			this.emptySpaceItem3.Size = new System.Drawing.Size(104, 26);
			this.emptySpaceItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem3.Text = "emptySpaceItem3";
			this.emptySpaceItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.Control = this.tbSubItems;
			this.layoutControlItem3.CustomizationFormText = "layoutControlItem3";
			this.layoutControlItem3.Location = new System.Drawing.Point(0, 26);
			this.layoutControlItem3.MinSize = new System.Drawing.Size(104, 24);
			this.layoutControlItem3.Name = "layoutControlItem3";
			this.layoutControlItem3.Size = new System.Drawing.Size(870, 191);
			this.layoutControlItem3.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem3.Text = "layoutControlItem3";
			this.layoutControlItem3.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem3.TextToControlDistance = 0;
			this.layoutControlItem3.TextVisible = false;
			this.layoutControlItem5.Control = this.subitemsTilesController;
			this.layoutControlItem5.CustomizationFormText = "layoutControlItem5";
			this.layoutControlItem5.Location = new System.Drawing.Point(104, 0);
			this.layoutControlItem5.Name = "layoutControlItem5";
			this.layoutControlItem5.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem5.Size = new System.Drawing.Size(766, 26);
			this.layoutControlItem5.Text = "layoutControlItem5";
			this.layoutControlItem5.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem5.TextToControlDistance = 0;
			this.layoutControlItem5.TextVisible = false;
			this.layoutControlGroup3.CustomizationFormText = "layoutControlGroup3";
			this.layoutControlGroup3.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem2,
			this.emptySpaceItem2,
			this.layoutControlItem4});
			this.layoutControlGroup3.Location = new System.Drawing.Point(0, 227);
			this.layoutControlGroup3.Name = "layoutControlGroup3";
			this.layoutControlGroup3.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup3.Size = new System.Drawing.Size(872, 219);
			this.layoutControlGroup3.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup3.Text = "layoutControlGroup3";
			this.layoutControlGroup3.TextVisible = false;
			this.layoutControlItem2.Control = this.tbItems;
			this.layoutControlItem2.CustomizationFormText = "layoutControlItem2";
			this.layoutControlItem2.Location = new System.Drawing.Point(0, 26);
			this.layoutControlItem2.Name = "layoutControlItem2";
			this.layoutControlItem2.Size = new System.Drawing.Size(870, 191);
			this.layoutControlItem2.Text = "layoutControlItem2";
			this.layoutControlItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem2.TextToControlDistance = 0;
			this.layoutControlItem2.TextVisible = false;
			this.emptySpaceItem2.AllowHotTrack = false;
			this.emptySpaceItem2.CustomizationFormText = "emptySpaceItem2";
			this.emptySpaceItem2.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceItem2.MaxSize = new System.Drawing.Size(0, 26);
			this.emptySpaceItem2.MinSize = new System.Drawing.Size(104, 26);
			this.emptySpaceItem2.Name = "emptySpaceItem2";
			this.emptySpaceItem2.Size = new System.Drawing.Size(104, 26);
			this.emptySpaceItem2.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem2.Text = "emptySpaceItem2";
			this.emptySpaceItem2.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.Control = this.itemsTilesController;
			this.layoutControlItem4.CustomizationFormText = "layoutControlItem4";
			this.layoutControlItem4.Location = new System.Drawing.Point(104, 0);
			this.layoutControlItem4.Name = "layoutControlItem4";
			this.layoutControlItem4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem4.Size = new System.Drawing.Size(766, 26);
			this.layoutControlItem4.Text = "layoutControlItem4";
			this.layoutControlItem4.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem4.TextToControlDistance = 0;
			this.layoutControlItem4.TextVisible = false;
			this.layoutControlGroup4.CustomizationFormText = "layoutControlGroup4";
			this.layoutControlGroup4.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutControlItem1,
			this.emptySpaceItem1,
			this.layoutControlItem17});
			this.layoutControlGroup4.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroup4.Name = "layoutControlGroup4";
			this.layoutControlGroup4.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup4.Size = new System.Drawing.Size(872, 222);
			this.layoutControlGroup4.Spacing = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroup4.Text = "layoutControlGroup4";
			this.layoutControlGroup4.TextVisible = false;
			this.layoutControlItem1.Control = this.tbCategories;
			this.layoutControlItem1.CustomizationFormText = "layoutControlItem1";
			this.layoutControlItem1.Location = new System.Drawing.Point(0, 26);
			this.layoutControlItem1.Name = "layoutControlItem1";
			this.layoutControlItem1.Size = new System.Drawing.Size(870, 194);
			this.layoutControlItem1.Text = "layoutControlItem1";
			this.layoutControlItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem1.TextToControlDistance = 0;
			this.layoutControlItem1.TextVisible = false;
			this.emptySpaceItem1.AllowHotTrack = false;
			this.emptySpaceItem1.CustomizationFormText = "emptySpaceItem1";
			this.emptySpaceItem1.Location = new System.Drawing.Point(0, 0);
			this.emptySpaceItem1.MaxSize = new System.Drawing.Size(0, 26);
			this.emptySpaceItem1.MinSize = new System.Drawing.Size(10, 26);
			this.emptySpaceItem1.Name = "emptySpaceItem1";
			this.emptySpaceItem1.Size = new System.Drawing.Size(98, 26);
			this.emptySpaceItem1.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.emptySpaceItem1.Text = "emptySpaceItem1";
			this.emptySpaceItem1.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem17.Control = this.catTilesController;
			this.layoutControlItem17.CustomizationFormText = "layoutControlItem17";
			this.layoutControlItem17.Location = new System.Drawing.Point(98, 0);
			this.layoutControlItem17.MaxSize = new System.Drawing.Size(0, 26);
			this.layoutControlItem17.MinSize = new System.Drawing.Size(100, 26);
			this.layoutControlItem17.Name = "layoutControlItem17";
			this.layoutControlItem17.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlItem17.Size = new System.Drawing.Size(772, 26);
			this.layoutControlItem17.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutControlItem17.Text = "layoutControlItem17";
			this.layoutControlItem17.TextSize = new System.Drawing.Size(0, 0);
			this.layoutControlItem17.TextToControlDistance = 0;
			this.layoutControlItem17.TextVisible = false;
			this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
			this.splitContainerControl1.Name = "splitContainerControl1";
			this.splitContainerControl1.Panel1.Controls.Add(this.layoutControl1);
			this.splitContainerControl1.Panel1.Text = "Panel1";
			this.splitContainerControl1.Panel2.Controls.Add(this.tabControl);
			this.splitContainerControl1.Panel2.Text = "Panel2";
			this.splitContainerControl1.Size = new System.Drawing.Size(1194, 690);
			this.splitContainerControl1.SplitterPosition = 892;
			this.splitContainerControl1.TabIndex = 15;
			this.splitContainerControl1.Text = "splitContainerControl1";
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1194, 690);
			this.Controls.Add(this.splitContainerControl1);
			this.Name = "TileNavPaneDesignerForm";
			this.Text = "TileNavPane Designer";
			((System.ComponentModel.ISupportInitialize)(this.tabControl)).EndInit();
			this.tabControl.ResumeLayout(false);
			this.tabCategory.ResumeLayout(false);
			this.tabItem.ResumeLayout(false);
			this.tabSubItems.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControl1)).EndInit();
			this.layoutControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem10)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitterItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem5)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup3)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem2)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroup4)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceItem1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlItem17)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
			this.splitContainerControl1.ResumeLayout(false);
			this.ResumeLayout(false);
		}
		#endregion
		private XtraBars.Navigation.TileBar tbCategories;
		private XtraBars.Navigation.TileBar tbItems;
		private System.Windows.Forms.PropertyGrid pgCategory;
		private XtraTab.XtraTabControl tabControl;
		private XtraTab.XtraTabPage tabCategory;
		private XtraTab.XtraTabPage tabItem;
		private System.Windows.Forms.PropertyGrid pgItem;
		private XtraTab.XtraTabPage tabSubItems;
		private System.Windows.Forms.PropertyGrid pgSubItem;
		private Navigation.TileBar tbSubItems;
		private XtraLayout.LayoutControl layoutControl1;
		private XtraLayout.LayoutControlGroup layoutControlGroup1;
		private XtraLayout.LayoutControlItem layoutControlItem1;
		private XtraLayout.LayoutControlItem layoutControlItem2;
		private XtraLayout.LayoutControlItem layoutControlItem3;
		private XtraLayout.SplitterItem splitterItem1;
		private XtraLayout.SplitterItem splitterItem2;
		private XtraEditors.SplitContainerControl splitContainerControl1;
		private XtraLayout.LayoutControlItem layoutControlItem10;
		private XtraLayout.EmptySpaceItem emptySpaceItem1;
		private XtraLayout.EmptySpaceItem emptySpaceItem2;
		private XtraLayout.EmptySpaceItem emptySpaceItem3;
		private XtraLayout.LayoutControlGroup layoutControlGroup2;
		private XtraLayout.LayoutControlGroup layoutControlGroup3;
		private XtraLayout.LayoutControlGroup layoutControlGroup4;
		private TileBarControlButtons catTilesController;
		private XtraLayout.LayoutControlItem layoutControlItem17;
		private TileBarControlButtons subitemsTilesController;
		private TileBarControlButtons itemsTilesController;
		private XtraLayout.LayoutControlItem layoutControlItem5;
		private XtraLayout.LayoutControlItem layoutControlItem4;
	}
}
