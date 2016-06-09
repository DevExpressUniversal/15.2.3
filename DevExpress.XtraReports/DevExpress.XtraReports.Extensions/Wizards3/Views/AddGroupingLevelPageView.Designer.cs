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

namespace DevExpress.XtraReports.Wizards3.Views {
	partial class AddGroupingLevelPageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AddGroupingLevelPageView));
			this.layoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.availableItemsLabel = new System.Windows.Forms.Label();
			this.contentPanel = new System.Windows.Forms.Panel();
			this.availableColumnsListBox = new DevExpress.XtraEditors.ListBoxControl();
			this.addGroupingButton = new DevExpress.XtraEditors.SimpleButton();
			this.combineGroupingButton = new DevExpress.XtraEditors.SimpleButton();
			this.removeGroupingButton = new DevExpress.XtraEditors.SimpleButton();
			this.groupingLevelUpButton = new DevExpress.XtraEditors.SimpleButton();
			this.groupingLevelDownButton = new DevExpress.XtraEditors.SimpleButton();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.groupingColumnsTreeView = new DevExpress.XtraTreeList.TreeList();
			this.dataMemberColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.tableInfoColumn = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).BeginInit();
			this.layoutControlBase.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupBase)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFinishButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNextButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderLabel)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorTop)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviousButton)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBaseContent)).BeginInit();
			this.panelBaseContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBaseContentPanel)).BeginInit();
			this.layoutPanel.SuspendLayout();
			this.contentPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.availableColumnsListBox)).BeginInit();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.groupingColumnsTreeView)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(664, 142, 749, 739);
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.BackColor = ((System.Drawing.Color)(resources.GetObject("layoutControlBase.OptionsPrint.AppearanceGroupCaption.BackColor")));
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Font = ((System.Drawing.Font)(resources.GetObject("layoutControlBase.OptionsPrint.AppearanceGroupCaption.Font")));
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseBackColor = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseFont = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.Options.UseTextOptions = true;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
			this.layoutControlBase.OptionsPrint.AppearanceGroupCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControlBase.OptionsPrint.AppearanceItemCaption.Options.UseTextOptions = true;
			this.layoutControlBase.OptionsPrint.AppearanceItemCaption.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
			this.layoutControlBase.OptionsPrint.AppearanceItemCaption.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.panelBaseContent.Controls.Add(this.layoutPanel);
			resources.ApplyResources(this.panelBaseContent, "panelBaseContent");
			resources.ApplyResources(this.layoutPanel, "layoutPanel");
			this.layoutPanel.Controls.Add(this.availableItemsLabel, 0, 0);
			this.layoutPanel.Controls.Add(this.contentPanel, 0, 1);
			this.layoutPanel.Controls.Add(this.addGroupingButton, 1, 2);
			this.layoutPanel.Controls.Add(this.combineGroupingButton, 1, 3);
			this.layoutPanel.Controls.Add(this.removeGroupingButton, 1, 4);
			this.layoutPanel.Controls.Add(this.groupingLevelUpButton, 1, 6);
			this.layoutPanel.Controls.Add(this.groupingLevelDownButton, 1, 7);
			this.layoutPanel.Controls.Add(this.tableLayoutPanel1, 2, 0);
			this.layoutPanel.Name = "layoutPanel";
			resources.ApplyResources(this.availableItemsLabel, "availableItemsLabel");
			this.availableItemsLabel.Name = "availableItemsLabel";
			this.contentPanel.Controls.Add(this.availableColumnsListBox);
			resources.ApplyResources(this.contentPanel, "contentPanel");
			this.contentPanel.Name = "contentPanel";
			this.layoutPanel.SetRowSpan(this.contentPanel, 8);
			this.availableColumnsListBox.DisplayMember = "DisplayName";
			resources.ApplyResources(this.availableColumnsListBox, "availableColumnsListBox");
			this.availableColumnsListBox.Name = "availableColumnsListBox";
			this.availableColumnsListBox.SelectedIndexChanged += new System.EventHandler(this.availableColumnsListBox_SelectedIndexChanged);
			this.availableColumnsListBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.availableColumnsListBox_MouseDoubleClick);
			resources.ApplyResources(this.addGroupingButton, "addGroupingButton");
			this.addGroupingButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.addGroupingButton.Name = "addGroupingButton";
			this.addGroupingButton.Click += new System.EventHandler(this.addGroupingButton_Click);
			resources.ApplyResources(this.combineGroupingButton, "combineGroupingButton");
			this.combineGroupingButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.combineGroupingButton.Name = "combineGroupingButton";
			this.combineGroupingButton.Click += new System.EventHandler(this.combineGroupingButton_Click);
			resources.ApplyResources(this.removeGroupingButton, "removeGroupingButton");
			this.removeGroupingButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.removeGroupingButton.Name = "removeGroupingButton";
			this.removeGroupingButton.Click += new System.EventHandler(this.removeGroupingButton_Click);
			resources.ApplyResources(this.groupingLevelUpButton, "groupingLevelUpButton");
			this.groupingLevelUpButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.groupingLevelUpButton.Name = "groupingLevelUpButton";
			this.groupingLevelUpButton.Click += new System.EventHandler(this.groupingLevelUpButton_Click);
			resources.ApplyResources(this.groupingLevelDownButton, "groupingLevelDownButton");
			this.groupingLevelDownButton.ImageLocation = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
			this.groupingLevelDownButton.Name = "groupingLevelDownButton";
			this.groupingLevelDownButton.Click += new System.EventHandler(this.groupingLevelDownButton_Click);
			resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
			this.tableLayoutPanel1.Controls.Add(this.groupingColumnsTreeView, 0, 1);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.layoutPanel.SetRowSpan(this.tableLayoutPanel1, 9);
			this.groupingColumnsTreeView.Appearance.Empty.BackColor = ((System.Drawing.Color)(resources.GetObject("groupingColumnsTreeView.Appearance.Empty.BackColor")));
			this.groupingColumnsTreeView.Appearance.Empty.BackColor2 = ((System.Drawing.Color)(resources.GetObject("groupingColumnsTreeView.Appearance.Empty.BackColor2")));
			this.groupingColumnsTreeView.Appearance.Empty.Options.UseBackColor = true;
			this.groupingColumnsTreeView.Appearance.SelectedRow.BorderColor = ((System.Drawing.Color)(resources.GetObject("groupingColumnsTreeView.Appearance.SelectedRow.BorderColor")));
			this.groupingColumnsTreeView.Appearance.SelectedRow.Options.UseBorderColor = true;
			this.groupingColumnsTreeView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.groupingColumnsTreeView.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			this.dataMemberColumn,
			this.tableInfoColumn});
			resources.ApplyResources(this.groupingColumnsTreeView, "groupingColumnsTreeView");
			this.groupingColumnsTreeView.Name = "groupingColumnsTreeView";
			this.groupingColumnsTreeView.OptionsBehavior.AllowExpandOnDblClick = false;
			this.groupingColumnsTreeView.OptionsBehavior.AutoChangeParent = false;
			this.groupingColumnsTreeView.OptionsBehavior.AutoNodeHeight = false;
			this.groupingColumnsTreeView.OptionsBehavior.Editable = false;
			this.groupingColumnsTreeView.OptionsBehavior.ResizeNodes = false;
			this.groupingColumnsTreeView.OptionsMenu.EnableColumnMenu = false;
			this.groupingColumnsTreeView.OptionsMenu.EnableFooterMenu = false;
			this.groupingColumnsTreeView.OptionsPrint.PrintHorzLines = false;
			this.groupingColumnsTreeView.OptionsPrint.PrintVertLines = false;
			this.groupingColumnsTreeView.OptionsSelection.EnableAppearanceFocusedCell = false;
			this.groupingColumnsTreeView.OptionsView.AllowHtmlDrawHeaders = true;
			this.groupingColumnsTreeView.OptionsView.ExpandButtonCentered = false;
			this.groupingColumnsTreeView.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.None;
			this.groupingColumnsTreeView.OptionsView.ShowButtons = false;
			this.groupingColumnsTreeView.OptionsView.ShowColumns = false;
			this.groupingColumnsTreeView.OptionsView.ShowHorzLines = false;
			this.groupingColumnsTreeView.OptionsView.ShowIndicator = false;
			this.groupingColumnsTreeView.OptionsView.ShowRoot = false;
			this.groupingColumnsTreeView.OptionsView.ShowVertLines = false;
			this.groupingColumnsTreeView.TreeLineStyle = DevExpress.XtraTreeList.LineStyle.None;
			this.groupingColumnsTreeView.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.groupingColumnsTreeView_FocusedNodeChanged);
			this.groupingColumnsTreeView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.groupingColumnsTreeView_MouseDoubleClick);
			resources.ApplyResources(this.dataMemberColumn, "dataMemberColumn");
			this.dataMemberColumn.FieldName = "DisplayName";
			this.dataMemberColumn.Name = "dataMemberColumn";
			this.dataMemberColumn.OptionsColumn.AllowEdit = false;
			this.dataMemberColumn.OptionsColumn.AllowMove = false;
			this.dataMemberColumn.OptionsColumn.AllowMoveToCustomizationForm = false;
			this.dataMemberColumn.OptionsColumn.AllowSort = false;
			resources.ApplyResources(this.tableInfoColumn, "tableInfoColumn");
			this.tableInfoColumn.FieldName = "Name";
			this.tableInfoColumn.Name = "tableInfoColumn";
			this.tableInfoColumn.OptionsColumn.AllowEdit = false;
			this.tableInfoColumn.OptionsColumn.AllowMove = false;
			this.tableInfoColumn.OptionsColumn.AllowMoveToCustomizationForm = false;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.MinimumSize = new System.Drawing.Size(480, 270);
			this.Name = "AddGroupingLevelPageView";
			((System.ComponentModel.ISupportInitialize)(this.layoutControlBase)).EndInit();
			this.layoutControlBase.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupBase)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemFinishButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemNextButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemHeaderLabel)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorTop)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreviousButton)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelBaseContent)).EndInit();
			this.panelBaseContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutItemBaseContentPanel)).EndInit();
			this.layoutPanel.ResumeLayout(false);
			this.layoutPanel.PerformLayout();
			this.contentPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.availableColumnsListBox)).EndInit();
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.groupingColumnsTreeView)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private System.Windows.Forms.TableLayoutPanel layoutPanel;
		private System.Windows.Forms.Label availableItemsLabel;
		private XtraTreeList.TreeList groupingColumnsTreeView;
		private XtraTreeList.Columns.TreeListColumn dataMemberColumn;
		private XtraTreeList.Columns.TreeListColumn tableInfoColumn;
		private XtraEditors.ListBoxControl availableColumnsListBox;
		private System.Windows.Forms.Panel contentPanel;
		private XtraEditors.SimpleButton addGroupingButton;
		private XtraEditors.SimpleButton combineGroupingButton;
		private XtraEditors.SimpleButton removeGroupingButton;
		private XtraEditors.SimpleButton groupingLevelUpButton;
		private XtraEditors.SimpleButton groupingLevelDownButton;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
	}
}
