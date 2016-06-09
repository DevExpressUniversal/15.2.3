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

namespace DevExpress.XtraBars.Ribbon {
	partial class RibbonCustomizationUserControl {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RibbonCustomizationUserControl));
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.leftHeaderPanel = new DevExpress.XtraEditors.PanelControl();
			this.lblChooseCommand = new DevExpress.XtraEditors.LabelControl();
			this.cbeCommands = new DevExpress.XtraEditors.ComboBoxEdit();
			this.rightHeaderPanel = new DevExpress.XtraEditors.PanelControl();
			this.cbeFilter = new DevExpress.XtraEditors.ComboBoxEdit();
			this.lblCustomizeRibbon = new DevExpress.XtraEditors.LabelControl();
			this.smallTableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.rightPanel = new DevExpress.XtraEditors.PanelControl();
			this.rightTreeView = new DevExpress.XtraBars.Ribbon.Customization.RunTimeRibbonTreeView();
			this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
			this.newOptionsDropDownButton = new DevExpress.XtraEditors.DropDownButton();
			this.exportOptionsDropDownButton = new DevExpress.XtraEditors.DropDownButton();
			this.btnRename = new DevExpress.XtraEditors.SimpleButton();
			this.leftPanel = new DevExpress.XtraEditors.PanelControl();
			this.leftTreeView = new DevExpress.XtraBars.Ribbon.Customization.RunTimeRibbonTreeViewOriginalView();
			this.middlePanel = new DevExpress.XtraEditors.PanelControl();
			this.btnDown = new DevExpress.XtraEditors.SimpleButton();
			this.images = new DevExpress.Utils.ImageCollection(this.components);
			this.btnRemoveItem = new DevExpress.XtraEditors.SimpleButton();
			this.btnUp = new DevExpress.XtraEditors.SimpleButton();
			this.btnAddItem = new DevExpress.XtraEditors.SimpleButton();
			this.tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.leftHeaderPanel)).BeginInit();
			this.leftHeaderPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbeCommands.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.rightHeaderPanel)).BeginInit();
			this.rightHeaderPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbeFilter.Properties)).BeginInit();
			this.smallTableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rightPanel)).BeginInit();
			this.rightPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).BeginInit();
			this.panelControl1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.leftPanel)).BeginInit();
			this.leftPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.middlePanel)).BeginInit();
			this.middlePanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.images)).BeginInit();
			this.SuspendLayout();
			this.tableLayoutPanel.AccessibleDescription = null;
			this.tableLayoutPanel.AccessibleName = null;
			resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
			this.tableLayoutPanel.BackgroundImage = null;
			this.tableLayoutPanel.Controls.Add(this.leftHeaderPanel, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.rightHeaderPanel, 2, 0);
			this.tableLayoutPanel.Controls.Add(this.smallTableLayoutPanel, 2, 1);
			this.tableLayoutPanel.Controls.Add(this.leftPanel, 0, 1);
			this.tableLayoutPanel.Controls.Add(this.middlePanel, 1, 1);
			this.tableLayoutPanel.Font = null;
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.leftHeaderPanel.AccessibleDescription = null;
			this.leftHeaderPanel.AccessibleName = null;
			resources.ApplyResources(this.leftHeaderPanel, "leftHeaderPanel");
			this.leftHeaderPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.leftHeaderPanel.Controls.Add(this.lblChooseCommand);
			this.leftHeaderPanel.Controls.Add(this.cbeCommands);
			this.leftHeaderPanel.Name = "leftHeaderPanel";
			this.lblChooseCommand.AccessibleDescription = null;
			this.lblChooseCommand.AccessibleName = null;
			resources.ApplyResources(this.lblChooseCommand, "lblChooseCommand");
			this.lblChooseCommand.Name = "lblChooseCommand";
			resources.ApplyResources(this.cbeCommands, "cbeCommands");
			this.cbeCommands.BackgroundImage = null;
			this.cbeCommands.EditValue = null;
			this.cbeCommands.Name = "cbeCommands";
			this.cbeCommands.Properties.AccessibleDescription = null;
			this.cbeCommands.Properties.AccessibleName = null;
			this.cbeCommands.Properties.AutoHeight = ((bool)(resources.GetObject("cbeCommands.Properties.AutoHeight")));
			this.cbeCommands.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbeCommands.Properties.Buttons"))))});
			this.cbeCommands.Properties.NullValuePrompt = resources.GetString("cbeCommands.Properties.NullValuePrompt");
			this.cbeCommands.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("cbeCommands.Properties.NullValuePromptShowForEmptyValue")));
			this.cbeCommands.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.cbeCommands.SelectedValueChanged += new System.EventHandler(this.commands_SelectedValueChanged);
			this.rightHeaderPanel.AccessibleDescription = null;
			this.rightHeaderPanel.AccessibleName = null;
			resources.ApplyResources(this.rightHeaderPanel, "rightHeaderPanel");
			this.rightHeaderPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rightHeaderPanel.Controls.Add(this.cbeFilter);
			this.rightHeaderPanel.Controls.Add(this.lblCustomizeRibbon);
			this.rightHeaderPanel.Name = "rightHeaderPanel";
			resources.ApplyResources(this.cbeFilter, "cbeFilter");
			this.cbeFilter.BackgroundImage = null;
			this.cbeFilter.EditValue = null;
			this.cbeFilter.Name = "cbeFilter";
			this.cbeFilter.Properties.AccessibleDescription = null;
			this.cbeFilter.Properties.AccessibleName = null;
			this.cbeFilter.Properties.AutoHeight = ((bool)(resources.GetObject("cbeFilter.Properties.AutoHeight")));
			this.cbeFilter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(((DevExpress.XtraEditors.Controls.ButtonPredefines)(resources.GetObject("cbeFilter.Properties.Buttons"))))});
			this.cbeFilter.Properties.NullValuePrompt = resources.GetString("cbeFilter.Properties.NullValuePrompt");
			this.cbeFilter.Properties.NullValuePromptShowForEmptyValue = ((bool)(resources.GetObject("cbeFilter.Properties.NullValuePromptShowForEmptyValue")));
			this.cbeFilter.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.lblCustomizeRibbon.AccessibleDescription = null;
			this.lblCustomizeRibbon.AccessibleName = null;
			resources.ApplyResources(this.lblCustomizeRibbon, "lblCustomizeRibbon");
			this.lblCustomizeRibbon.Name = "lblCustomizeRibbon";
			this.smallTableLayoutPanel.AccessibleDescription = null;
			this.smallTableLayoutPanel.AccessibleName = null;
			resources.ApplyResources(this.smallTableLayoutPanel, "smallTableLayoutPanel");
			this.smallTableLayoutPanel.BackgroundImage = null;
			this.smallTableLayoutPanel.Controls.Add(this.rightPanel, 0, 0);
			this.smallTableLayoutPanel.Controls.Add(this.panelControl1, 0, 1);
			this.smallTableLayoutPanel.Font = null;
			this.smallTableLayoutPanel.Name = "smallTableLayoutPanel";
			this.rightPanel.AccessibleDescription = null;
			this.rightPanel.AccessibleName = null;
			resources.ApplyResources(this.rightPanel, "rightPanel");
			this.rightPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.rightPanel.Controls.Add(this.rightTreeView);
			this.rightPanel.Name = "rightPanel";
			this.rightTreeView.AccessibleDescription = null;
			this.rightTreeView.AccessibleName = null;
			this.rightTreeView.AllowDrag = true;
			this.rightTreeView.AllowDrop = true;
			this.rightTreeView.AllowGallery = true;
			resources.ApplyResources(this.rightTreeView, "rightTreeView");
			this.rightTreeView.BackgroundImage = null;
			this.rightTreeView.CheckBoxes = true;
			this.rightTreeView.ComponentChangeService = null;
			this.rightTreeView.DefaultExpandCollapseButtonOffset = 5;
			this.rightTreeView.DesignerHost = null;
			this.rightTreeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
			this.rightTreeView.DropSelectedNode = null;
			this.rightTreeView.DropTargetNode = null;
			this.rightTreeView.Font = null;
			this.rightTreeView.FullRowSelect = true;
			this.rightTreeView.HideSelection = false;
			this.rightTreeView.ItemLinks = null;
			this.rightTreeView.Name = "rightTreeView";
			this.rightTreeView.OwnerFrame = null;
			this.rightTreeView.Ribbon = null;
			this.rightTreeView.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.Standard;
			this.rightTreeView.ShowLines = false;
			this.rightTreeView.StatusBar = null;
			this.rightTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rightTreeView_MouseUp);
			this.rightTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.rightTreeView_AfterSelect);
			this.rightTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.rightTreeView_NodeMouseClick);
			this.panelControl1.AccessibleDescription = null;
			this.panelControl1.AccessibleName = null;
			resources.ApplyResources(this.panelControl1, "panelControl1");
			this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.panelControl1.Controls.Add(this.newOptionsDropDownButton);
			this.panelControl1.Controls.Add(this.exportOptionsDropDownButton);
			this.panelControl1.Controls.Add(this.btnRename);
			this.panelControl1.Name = "panelControl1";
			this.newOptionsDropDownButton.AccessibleDescription = null;
			this.newOptionsDropDownButton.AccessibleName = null;
			resources.ApplyResources(this.newOptionsDropDownButton, "newOptionsDropDownButton");
			this.newOptionsDropDownButton.BackgroundImage = null;
			this.newOptionsDropDownButton.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Show;
			this.newOptionsDropDownButton.Name = "newOptionsDropDownButton";
			this.exportOptionsDropDownButton.AccessibleDescription = null;
			this.exportOptionsDropDownButton.AccessibleName = null;
			resources.ApplyResources(this.exportOptionsDropDownButton, "exportOptionsDropDownButton");
			this.exportOptionsDropDownButton.BackgroundImage = null;
			this.exportOptionsDropDownButton.DropDownArrowStyle = DevExpress.XtraEditors.DropDownArrowStyle.Show;
			this.exportOptionsDropDownButton.ImageIndex = 0;
			this.exportOptionsDropDownButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleRight;
			this.exportOptionsDropDownButton.Name = "exportOptionsDropDownButton";
			this.btnRename.AccessibleDescription = null;
			this.btnRename.AccessibleName = null;
			resources.ApplyResources(this.btnRename, "btnRename");
			this.btnRename.BackgroundImage = null;
			this.btnRename.Name = "btnRename";
			this.btnRename.Click += new System.EventHandler(this.btnRename_Click);
			this.leftPanel.AccessibleDescription = null;
			this.leftPanel.AccessibleName = null;
			resources.ApplyResources(this.leftPanel, "leftPanel");
			this.leftPanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.leftPanel.Controls.Add(this.leftTreeView);
			this.leftPanel.Name = "leftPanel";
			this.leftTreeView.AccessibleDescription = null;
			this.leftTreeView.AccessibleName = null;
			this.leftTreeView.AllowDrag = true;
			this.leftTreeView.AllowDrop = true;
			this.leftTreeView.AllowGallery = true;
			resources.ApplyResources(this.leftTreeView, "leftTreeView");
			this.leftTreeView.BackgroundImage = null;
			this.leftTreeView.ComponentChangeService = null;
			this.leftTreeView.DefaultExpandCollapseButtonOffset = 5;
			this.leftTreeView.DesignerHost = null;
			this.leftTreeView.DrawMode = System.Windows.Forms.TreeViewDrawMode.OwnerDrawAll;
			this.leftTreeView.DropSelectedNode = null;
			this.leftTreeView.DropTargetNode = null;
			this.leftTreeView.Font = null;
			this.leftTreeView.FullRowSelect = true;
			this.leftTreeView.HideSelection = false;
			this.leftTreeView.IsCustomizationMode = false;
			this.leftTreeView.ItemLinks = null;
			this.leftTreeView.Name = "leftTreeView";
			this.leftTreeView.OwnerFrame = null;
			this.leftTreeView.Ribbon = null;
			this.leftTreeView.SelectionMode = DevExpress.Utils.Design.DXTreeSelectionMode.Standard;
			this.leftTreeView.ShowLines = false;
			this.leftTreeView.StatusBar = null;
			this.leftTreeView.ViewType = DevExpress.XtraBars.Ribbon.Customization.RunTimeRibbonTreeViewOriginalView.TreeViewType.Default;
			this.leftTreeView.MouseUp += new System.Windows.Forms.MouseEventHandler(this.rightTreeView_MouseUp);
			this.leftTreeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.rightTreeView_AfterSelect);
			this.leftTreeView.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.rightTreeView_NodeMouseClick);
			this.middlePanel.AccessibleDescription = null;
			this.middlePanel.AccessibleName = null;
			resources.ApplyResources(this.middlePanel, "middlePanel");
			this.middlePanel.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.middlePanel.Controls.Add(this.btnDown);
			this.middlePanel.Controls.Add(this.btnRemoveItem);
			this.middlePanel.Controls.Add(this.btnUp);
			this.middlePanel.Controls.Add(this.btnAddItem);
			this.middlePanel.Name = "middlePanel";
			this.btnDown.AccessibleDescription = null;
			this.btnDown.AccessibleName = null;
			resources.ApplyResources(this.btnDown, "btnDown");
			this.btnDown.BackgroundImage = null;
			this.btnDown.ImageIndex = 3;
			this.btnDown.ImageList = this.images;
			this.btnDown.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnDown.Name = "btnDown";
			this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
			resources.ApplyResources(this.images, "images");
			this.images.ImageStream = ((DevExpress.Utils.ImageCollectionStreamer)(resources.GetObject("images.ImageStream")));
			this.images.Images.SetKeyName(0, "ArrowLeft.png");
			this.images.Images.SetKeyName(1, "ArrowRight.png");
			this.images.Images.SetKeyName(2, "ArrowUp.png");
			this.images.Images.SetKeyName(3, "ArrowDown.png");
			this.btnRemoveItem.AccessibleDescription = null;
			this.btnRemoveItem.AccessibleName = null;
			resources.ApplyResources(this.btnRemoveItem, "btnRemoveItem");
			this.btnRemoveItem.BackgroundImage = null;
			this.btnRemoveItem.ImageIndex = 0;
			this.btnRemoveItem.ImageList = this.images;
			this.btnRemoveItem.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnRemoveItem.Name = "btnRemoveItem";
			this.btnRemoveItem.Click += new System.EventHandler(this.btnRemoveItem_Click);
			this.btnUp.AccessibleDescription = null;
			this.btnUp.AccessibleName = null;
			resources.ApplyResources(this.btnUp, "btnUp");
			this.btnUp.BackgroundImage = null;
			this.btnUp.ImageIndex = 2;
			this.btnUp.ImageList = this.images;
			this.btnUp.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnUp.Name = "btnUp";
			this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
			this.btnAddItem.AccessibleDescription = null;
			this.btnAddItem.AccessibleName = null;
			resources.ApplyResources(this.btnAddItem, "btnAddItem");
			this.btnAddItem.BackgroundImage = null;
			this.btnAddItem.ImageIndex = 1;
			this.btnAddItem.ImageList = this.images;
			this.btnAddItem.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.btnAddItem.Name = "btnAddItem";
			this.btnAddItem.Click += new System.EventHandler(this.btnAddItem_Click);
			this.AccessibleDescription = null;
			this.AccessibleName = null;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackgroundImage = null;
			this.Controls.Add(this.tableLayoutPanel);
			this.Name = "RibbonCustomizationUserControl";
			this.tableLayoutPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.leftHeaderPanel)).EndInit();
			this.leftHeaderPanel.ResumeLayout(false);
			this.leftHeaderPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbeCommands.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.rightHeaderPanel)).EndInit();
			this.rightHeaderPanel.ResumeLayout(false);
			this.rightHeaderPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.cbeFilter.Properties)).EndInit();
			this.smallTableLayoutPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.rightPanel)).EndInit();
			this.rightPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.panelControl1)).EndInit();
			this.panelControl1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.leftPanel)).EndInit();
			this.leftPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.middlePanel)).EndInit();
			this.middlePanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.images)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private DevExpress.XtraEditors.PanelControl leftHeaderPanel;
		private DevExpress.XtraEditors.LabelControl lblChooseCommand;
		private DevExpress.XtraEditors.ComboBoxEdit cbeCommands;
		private DevExpress.XtraEditors.PanelControl rightHeaderPanel;
		private DevExpress.XtraEditors.ComboBoxEdit cbeFilter;
		private DevExpress.XtraEditors.LabelControl lblCustomizeRibbon;
		private System.Windows.Forms.TableLayoutPanel smallTableLayoutPanel;
		private DevExpress.XtraEditors.PanelControl rightPanel;
		private DevExpress.XtraBars.Ribbon.Customization.RunTimeRibbonTreeView rightTreeView;
		private DevExpress.XtraEditors.PanelControl panelControl1;
		private DevExpress.XtraEditors.DropDownButton exportOptionsDropDownButton;
		private DevExpress.XtraEditors.SimpleButton btnRename;
		private DevExpress.XtraEditors.PanelControl leftPanel;
		private DevExpress.XtraBars.Ribbon.Customization.RunTimeRibbonTreeViewOriginalView leftTreeView;
		private DevExpress.Utils.ImageCollection images;
		private DevExpress.XtraEditors.DropDownButton newOptionsDropDownButton;
		private DevExpress.XtraEditors.PanelControl middlePanel;
		private DevExpress.XtraEditors.SimpleButton btnDown;
		private DevExpress.XtraEditors.SimpleButton btnRemoveItem;
		private DevExpress.XtraEditors.SimpleButton btnUp;
		private DevExpress.XtraEditors.SimpleButton btnAddItem;
	}
}
