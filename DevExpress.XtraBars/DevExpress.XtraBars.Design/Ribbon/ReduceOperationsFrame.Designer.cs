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

using DevExpress.XtraBars.Ribbon.Customization;
using System;
using DevExpress.XtraBars.Ribbon.Internal;
using DevExpress.XtraBars.Ribbon.Helpers;
namespace DevExpress.XtraBars.Ribbon.Design {
	partial class ReduceOperationsFrame {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
				RibbonReduceOperationHelper.Ribbon = null;
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
			this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
			this.reduceOperationsList = new DevExpress.XtraEditors.ImageListBoxControl();
			this.cbPages = new DevExpress.XtraEditors.ComboBoxEdit();
			this.ribbonControl1 = new DevExpress.XtraBars.Ribbon.RibbonControl();
			this.bAdd = new DevExpress.XtraBars.BarLargeButtonItem();
			this.popupMenu1 = new DevExpress.XtraBars.PopupMenu(this.components);
			this.bTypeGallery = new DevExpress.XtraBars.BarButtonItem();
			this.bTypeButtonGroups = new DevExpress.XtraBars.BarButtonItem();
			this.bTypeLargeButtons = new DevExpress.XtraBars.BarButtonItem();
			this.bTypeSmallButtonsWithText = new DevExpress.XtraBars.BarButtonItem();
			this.bTypeCollapseGroup = new DevExpress.XtraBars.BarButtonItem();
			this.bRemove = new DevExpress.XtraBars.BarLargeButtonItem();
			this.bMoveUp = new DevExpress.XtraBars.BarLargeButtonItem();
			this.bMoveDown = new DevExpress.XtraBars.BarLargeButtonItem();
			this.barStaticItem1 = new DevExpress.XtraBars.BarStaticItem();
			this.ribbonPage1 = new DevExpress.XtraBars.Ribbon.RibbonPage();
			this.ribbonPageGroup1 = new DevExpress.XtraBars.Ribbon.RibbonPageGroup();
			this.ribbonStatusBar1 = new DevExpress.XtraBars.Ribbon.RibbonStatusBar();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.sLinkCount = new DevExpress.XtraEditors.SpinEdit();
			this.labelControl4 = new DevExpress.XtraEditors.LabelControl();
			this.cbStartLink = new DevExpress.XtraEditors.ComboBoxEdit();
			this.labelControl3 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl2 = new DevExpress.XtraEditors.LabelControl();
			this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
			this.cbBehavior = new DevExpress.XtraEditors.ComboBoxEdit();
			this.cbPageGroup = new DevExpress.XtraEditors.ComboBoxEdit();
			this.ribbonTree = new DevExpress.XtraBars.Ribbon.Customization.RibbonTreeView();
			this.xtraScrollableControl1 = new DevExpress.XtraEditors.XtraScrollableControl();
			this.barAndDockingController1 = new DevExpress.XtraBars.BarAndDockingController(this.components);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).BeginInit();
			this.pnlMain.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).BeginInit();
			this.splitContainerControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).BeginInit();
			this.panelControl2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.reduceOperationsList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPages.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.sLinkCount.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbStartLink.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbBehavior.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPageGroup.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).BeginInit();
			this.SuspendLayout();
			this.lbCaption.Size = new System.Drawing.Size(696, 42);
			this.pnlMain.Controls.Add(this.splitContainerControl1);
			this.pnlMain.Location = new System.Drawing.Point(0, 28);
			this.pnlMain.Size = new System.Drawing.Size(696, 481);
			this.horzSplitter.Location = new System.Drawing.Point(0, 24);
			this.horzSplitter.Size = new System.Drawing.Size(696, 4);
			this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
			this.splitContainerControl1.Name = "splitContainerControl1";
			this.splitContainerControl1.Panel1.Controls.Add(this.panelControl2);
			this.splitContainerControl1.Panel1.Controls.Add(this.ribbonControl1);
			this.splitContainerControl1.Panel1.Text = "Panel1";
			this.splitContainerControl1.Panel2.Controls.Add(this.ribbonTree);
			this.splitContainerControl1.Panel2.Controls.Add(this.xtraScrollableControl1);
			this.splitContainerControl1.Panel2.Text = "Panel2";
			this.splitContainerControl1.Size = new System.Drawing.Size(696, 481);
			this.splitContainerControl1.SplitterPosition = 300;
			this.splitContainerControl1.TabIndex = 0;
			this.splitContainerControl1.Text = "splitContainerControl1";
			this.panelControl2.Controls.Add(this.reduceOperationsList);
			this.panelControl2.Controls.Add(this.cbPages);
			this.panelControl2.Controls.Add(this.panelControl1);
			this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panelControl2.Location = new System.Drawing.Point(0, 75);
			this.panelControl2.Name = "panelControl2";
			this.panelControl2.Size = new System.Drawing.Size(230, 406);
			this.panelControl2.TabIndex = 6;
			this.reduceOperationsList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.reduceOperationsList.Location = new System.Drawing.Point(2, 22);
			this.reduceOperationsList.Name = "reduceOperationsList";
			this.reduceOperationsList.Size = new System.Drawing.Size(226, 270);
			this.reduceOperationsList.TabIndex = 0;
			this.reduceOperationsList.SelectedIndexChanged += new System.EventHandler(this.reduceOperationsList_SelectedIndexChanged);
			this.cbPages.Dock = System.Windows.Forms.DockStyle.Top;
			this.cbPages.Location = new System.Drawing.Point(2, 2);
			this.cbPages.MenuManager = this.ribbonControl1;
			this.cbPages.Name = "cbPages";
			this.cbPages.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbPages.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPages.Properties.SelectedIndexChanged += new System.EventHandler(this.cbPages_Properties_SelectedIndexChanged);
			this.cbPages.Size = new System.Drawing.Size(226, 20);
			this.cbPages.TabIndex = 4;
			this.cbPages.SelectedIndexChanged += new System.EventHandler(this.cbPages_SelectedIndexChanged);
			this.ribbonControl1.Controller = this.barAndDockingController1;
			this.ribbonControl1.DrawGroupsBorder = true;
			this.ribbonControl1.ExpandCollapseItem.Id = 0;
			this.ribbonControl1.Items.AddRange(new DevExpress.XtraBars.BarItem[] {
			this.ribbonControl1.ExpandCollapseItem,
			this.bAdd,
			this.bRemove,
			this.bMoveUp,
			this.bMoveDown,
			this.barStaticItem1,
			this.bTypeGallery,
			this.bTypeButtonGroups,
			this.bTypeLargeButtons,
			this.bTypeSmallButtonsWithText,
			this.bTypeCollapseGroup});
			this.ribbonControl1.Location = new System.Drawing.Point(0, 0);
			this.ribbonControl1.MaxItemId = 6;
			this.ribbonControl1.Name = "ribbonControl1";
			this.ribbonControl1.Pages.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPage[] {
			this.ribbonPage1});
			this.ribbonControl1.RibbonStyle = DevExpress.XtraBars.Ribbon.RibbonControlStyle.Office2013;
			this.ribbonControl1.ShowApplicationButton = DevExpress.Utils.DefaultBoolean.False;
			this.ribbonControl1.ShowPageHeadersMode = DevExpress.XtraBars.Ribbon.ShowPageHeadersMode.Hide;
			this.ribbonControl1.Size = new System.Drawing.Size(230, 75);
			this.ribbonControl1.StatusBar = this.ribbonStatusBar1;
			this.ribbonControl1.ToolbarLocation = DevExpress.XtraBars.Ribbon.RibbonQuickAccessToolbarLocation.Hidden;
			this.bAdd.ButtonStyle = DevExpress.XtraBars.BarButtonStyle.DropDown;
			this.bAdd.Caption = "Add";
			this.bAdd.DropDownControl = this.popupMenu1;
			this.bAdd.Id = 0;
			this.bAdd.LargeGlyph = global::DevExpress.XtraBars.Design.Properties.Resources.Add_32x32;
			this.bAdd.Name = "bAdd";
			this.bAdd.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bAdd_ItemClick);
			this.popupMenu1.ItemLinks.Add(this.bTypeGallery);
			this.popupMenu1.ItemLinks.Add(this.bTypeButtonGroups);
			this.popupMenu1.ItemLinks.Add(this.bTypeLargeButtons);
			this.popupMenu1.ItemLinks.Add(this.bTypeSmallButtonsWithText);
			this.popupMenu1.ItemLinks.Add(this.bTypeCollapseGroup);
			this.popupMenu1.Name = "popupMenu1";
			this.popupMenu1.Ribbon = this.ribbonControl1;
			this.bTypeGallery.Caption = "Gallery";
			this.bTypeGallery.Id = 1;
			this.bTypeGallery.Name = "bTypeGallery";
			this.bTypeGallery.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bTypeGallery_ItemClick);
			this.bTypeButtonGroups.Caption = "Button Groups";
			this.bTypeButtonGroups.Id = 2;
			this.bTypeButtonGroups.Name = "bTypeButtonGroups";
			this.bTypeButtonGroups.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bTypeButtonGroups_ItemClick);
			this.bTypeLargeButtons.Caption = "Large Buttons";
			this.bTypeLargeButtons.Id = 3;
			this.bTypeLargeButtons.Name = "bTypeLargeButtons";
			this.bTypeLargeButtons.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bTypeLargeButtons_ItemClick);
			this.bTypeSmallButtonsWithText.Caption = "Small Buttons With Text";
			this.bTypeSmallButtonsWithText.Id = 4;
			this.bTypeSmallButtonsWithText.Name = "bTypeSmallButtonsWithText";
			this.bTypeSmallButtonsWithText.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bTypeSmallButtonsWithText_ItemClick);
			this.bTypeCollapseGroup.Caption = "Collapse Group";
			this.bTypeCollapseGroup.Id = 5;
			this.bTypeCollapseGroup.Name = "bTypeCollapseGroup";
			this.bTypeCollapseGroup.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bTypeCollapseGroup_ItemClick);
			this.bRemove.Caption = "Remove";
			this.bRemove.Id = 1;
			this.bRemove.LargeGlyph = global::DevExpress.XtraBars.Design.Properties.Resources.Delete_32x32;
			this.bRemove.Name = "bRemove";
			this.bRemove.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bRemove_ItemClick);
			this.bMoveUp.Caption = "Move Up";
			this.bMoveUp.Id = 2;
			this.bMoveUp.LargeGlyph = global::DevExpress.XtraBars.Design.Properties.Resources.Prev_32x32;
			this.bMoveUp.Name = "bMoveUp";
			this.bMoveUp.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bMoveUp_ItemClick);
			this.bMoveDown.Caption = "Move Down";
			this.bMoveDown.Id = 4;
			this.bMoveDown.LargeGlyph = global::DevExpress.XtraBars.Design.Properties.Resources.Next_32x32;
			this.bMoveDown.Name = "bMoveDown";
			this.bMoveDown.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.bMoveDown_ItemClick);
			this.barStaticItem1.AutoSize = DevExpress.XtraBars.BarStaticItemSize.Spring;
			this.barStaticItem1.Caption = "Help";
			this.barStaticItem1.Id = 5;
			this.barStaticItem1.Name = "barStaticItem1";
			this.barStaticItem1.TextAlignment = System.Drawing.StringAlignment.Near;
			this.barStaticItem1.Width = 32;
			this.ribbonPage1.Groups.AddRange(new DevExpress.XtraBars.Ribbon.RibbonPageGroup[] {
			this.ribbonPageGroup1});
			this.ribbonPage1.Name = "ribbonPage1";
			this.ribbonPage1.Text = "ConvertedFromBarManager";
			this.ribbonPageGroup1.ItemLinks.Add(this.bAdd);
			this.ribbonPageGroup1.ItemLinks.Add(this.bRemove);
			this.ribbonPageGroup1.ItemLinks.Add(this.bMoveUp);
			this.ribbonPageGroup1.ItemLinks.Add(this.bMoveDown);
			this.ribbonPageGroup1.Name = "ribbonPageGroup1";
			this.ribbonPageGroup1.ShowCaptionButton = false;
			this.ribbonPageGroup1.Text = "Tools";
			this.ribbonStatusBar1.ItemLinks.Add(this.barStaticItem1);
			this.ribbonStatusBar1.Location = new System.Drawing.Point(0, 509);
			this.ribbonStatusBar1.Name = "ribbonStatusBar1";
			this.ribbonStatusBar1.Ribbon = this.ribbonControl1;
			this.ribbonStatusBar1.Size = new System.Drawing.Size(696, 27);
			this.panelControl1.Controls.Add(this.sLinkCount);
			this.panelControl1.Controls.Add(this.labelControl4);
			this.panelControl1.Controls.Add(this.cbStartLink);
			this.panelControl1.Controls.Add(this.labelControl3);
			this.panelControl1.Controls.Add(this.labelControl2);
			this.panelControl1.Controls.Add(this.labelControl1);
			this.panelControl1.Controls.Add(this.cbBehavior);
			this.panelControl1.Controls.Add(this.cbPageGroup);
			this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panelControl1.Location = new System.Drawing.Point(2, 292);
			this.panelControl1.Name = "panelControl1";
			this.panelControl1.Size = new System.Drawing.Size(226, 112);
			this.panelControl1.TabIndex = 2;
			this.sLinkCount.EditValue = new decimal(new int[] {
			2,
			0,
			0,
			0});
			this.sLinkCount.Location = new System.Drawing.Point(111, 84);
			this.sLinkCount.MenuManager = this.ribbonControl1;
			this.sLinkCount.Name = "sLinkCount";
			this.sLinkCount.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton()});
			this.sLinkCount.Properties.MaxValue = new decimal(new int[] {
			3,
			0,
			0,
			0});
			this.sLinkCount.Properties.MinValue = new decimal(new int[] {
			2,
			0,
			0,
			0});
			this.sLinkCount.Size = new System.Drawing.Size(114, 20);
			this.sLinkCount.TabIndex = 7;
			this.sLinkCount.EditValueChanged += new System.EventHandler(this.sLinkCount_EditValueChanged);
			this.labelControl4.Location = new System.Drawing.Point(5, 87);
			this.labelControl4.Name = "labelControl4";
			this.labelControl4.Size = new System.Drawing.Size(50, 13);
			this.labelControl4.TabIndex = 6;
			this.labelControl4.Text = "Link Count";
			this.cbStartLink.Location = new System.Drawing.Point(111, 58);
			this.cbStartLink.MenuManager = this.ribbonControl1;
			this.cbStartLink.Name = "cbStartLink";
			this.cbStartLink.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbStartLink.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbStartLink.Size = new System.Drawing.Size(114, 20);
			this.cbStartLink.TabIndex = 5;
			this.cbStartLink.SelectedIndexChanged += new System.EventHandler(this.cbStartLink_SelectedIndexChanged);
			this.labelControl3.Location = new System.Drawing.Point(5, 61);
			this.labelControl3.Name = "labelControl3";
			this.labelControl3.Size = new System.Drawing.Size(45, 13);
			this.labelControl3.TabIndex = 4;
			this.labelControl3.Text = "Start Link";
			this.labelControl2.Location = new System.Drawing.Point(5, 35);
			this.labelControl2.Name = "labelControl2";
			this.labelControl2.Size = new System.Drawing.Size(53, 13);
			this.labelControl2.TabIndex = 2;
			this.labelControl2.Text = "PageGroup";
			this.labelControl1.Location = new System.Drawing.Point(5, 9);
			this.labelControl1.Name = "labelControl1";
			this.labelControl1.Size = new System.Drawing.Size(42, 13);
			this.labelControl1.TabIndex = 1;
			this.labelControl1.Text = "Behavior";
			this.cbBehavior.EditValue = "Single";
			this.cbBehavior.Location = new System.Drawing.Point(111, 6);
			this.cbBehavior.MenuManager = this.ribbonControl1;
			this.cbBehavior.Name = "cbBehavior";
			this.cbBehavior.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbBehavior.Properties.Items.AddRange(new object[] {
			"Single",
			"Until Available"});
			this.cbBehavior.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbBehavior.Size = new System.Drawing.Size(114, 20);
			this.cbBehavior.TabIndex = 0;
			this.cbBehavior.SelectedIndexChanged += new System.EventHandler(this.cbBehavior_SelectedIndexChanged);
			this.cbPageGroup.Location = new System.Drawing.Point(111, 32);
			this.cbPageGroup.MenuManager = this.ribbonControl1;
			this.cbPageGroup.Name = "cbPageGroup";
			this.cbPageGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.cbPageGroup.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbPageGroup.Size = new System.Drawing.Size(114, 20);
			this.cbPageGroup.TabIndex = 3;
			this.cbPageGroup.SelectedIndexChanged += new System.EventHandler(this.cbPageGroup_SelectedIndexChanged);
			this.cbPageGroup.TextChanged += new System.EventHandler(this.tePageGroup_TextChanged);
			this.ribbonTree.AllowDrag = true;
			this.ribbonTree.AllowDrop = true;
			this.ribbonTree.AllowGallery = false;
			this.ribbonTree.AllowSkinning = true;
			this.ribbonTree.ComponentChangeService = null;
			this.ribbonTree.DefaultExpandCollapseButtonOffset = 5;
			this.ribbonTree.DesignerHost = null;
			this.ribbonTree.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ribbonTree.DropSelectedNode = null;
			this.ribbonTree.DropTargetNode = null;
			this.ribbonTree.ItemLinks = null;
			this.ribbonTree.Location = new System.Drawing.Point(0, 151);
			this.ribbonTree.Name = "ribbonTree";
			this.ribbonTree.OwnerFrame = null;
			this.ribbonTree.Ribbon = null;
			this.ribbonTree.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.MultiSelectChildrenSameBranch;
			this.ribbonTree.Size = new System.Drawing.Size(461, 330);
			this.ribbonTree.StatusBar = null;
			this.ribbonTree.TabIndex = 0;
			this.xtraScrollableControl1.Dock = System.Windows.Forms.DockStyle.Top;
			this.xtraScrollableControl1.Location = new System.Drawing.Point(0, 0);
			this.xtraScrollableControl1.Name = "xtraScrollableControl1";
			this.xtraScrollableControl1.Size = new System.Drawing.Size(461, 151);
			this.xtraScrollableControl1.TabIndex = 1;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.ribbonStatusBar1);
			this.Name = "ReduceOperationsFrame";
			this.Size = new System.Drawing.Size(696, 536);
			this.Controls.SetChildIndex(this.ribbonStatusBar1, 0);
			this.Controls.SetChildIndex(this.lbCaption, 0);
			this.Controls.SetChildIndex(this.horzSplitter, 0);
			this.Controls.SetChildIndex(this.pnlMain, 0);
			((System.ComponentModel.ISupportInitialize)(this.pnlMain)).EndInit();
			this.pnlMain.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.horzSplitter)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerControl1)).EndInit();
			this.splitContainerControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl2)).EndInit();
			this.panelControl2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.reduceOperationsList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPages.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.popupMenu1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			this.panelControl1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.sLinkCount.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbStartLink.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbBehavior.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.cbPageGroup.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.barAndDockingController1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraEditors.SplitContainerControl splitContainerControl1;
		private DevExpress.XtraEditors.ImageListBoxControl reduceOperationsList;
		private BarLargeButtonItem bAdd;
		private BarLargeButtonItem bRemove;
		private BarLargeButtonItem bMoveUp;
		private BarLargeButtonItem bMoveDown;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private BarStaticItem barStaticItem1;
		private RibbonTreeView ribbonTree;
		private RibbonControl ribbonControl1;
		private RibbonPage ribbonPage1;
		private RibbonPageGroup ribbonPageGroup1;
		private RibbonStatusBar ribbonStatusBar1;
		private PopupMenu popupMenu1;
		private BarButtonItem bTypeGallery;
		private BarButtonItem bTypeButtonGroups;
		private BarButtonItem bTypeLargeButtons;
		private BarButtonItem bTypeSmallButtonsWithText;
		private BarButtonItem bTypeCollapseGroup;
		private DevExpress.XtraEditors.SpinEdit sLinkCount;
		private DevExpress.XtraEditors.LabelControl labelControl4;
		private DevExpress.XtraEditors.ComboBoxEdit cbStartLink;
		private DevExpress.XtraEditors.LabelControl labelControl3;
		private DevExpress.XtraEditors.LabelControl labelControl2;
		private DevExpress.XtraEditors.LabelControl labelControl1;
		private DevExpress.XtraEditors.ComboBoxEdit cbBehavior;
		private DevExpress.XtraEditors.ComboBoxEdit cbPages;
		private DevExpress.XtraEditors.XtraScrollableControl xtraScrollableControl1;
		private DevExpress.XtraEditors.PanelControl panelControl2;
		private DevExpress.XtraEditors.ComboBoxEdit cbPageGroup;
		private BarAndDockingController barAndDockingController1;
	}
}
