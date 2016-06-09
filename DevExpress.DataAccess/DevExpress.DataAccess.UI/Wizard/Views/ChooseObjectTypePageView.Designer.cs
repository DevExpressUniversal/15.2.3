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

namespace DevExpress.DataAccess.UI.Wizard.Views {
	partial class ChooseObjectTypePageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			DevExpress.XtraTreeList.FilterCondition filterCondition1 = new DevExpress.XtraTreeList.FilterCondition();
			DevExpress.XtraTreeList.StyleFormatConditions.StyleFormatCondition styleFormatCondition1 = new DevExpress.XtraTreeList.StyleFormatConditions.StyleFormatCondition();
			this.treeListColumnHighlighted = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.treeList = new DevExpress.XtraTreeList.TreeList();
			this.treeListColumnNodeType = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.treeListColumnOrder = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.treeListColumnName = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.treeListColumnData = new DevExpress.XtraTreeList.Columns.TreeListColumn();
			this.checkEditShowOnlyHighlighted = new DevExpress.XtraEditors.CheckEdit();
			this.layoutControlContent = new DevExpress.XtraLayout.LayoutControl();
			this.layoutGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemTreeList = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemShowOnlyHighlighted = new DevExpress.XtraLayout.LayoutControlItem();
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
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.panelAdditionalButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.treeList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditShowOnlyHighlighted.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).BeginInit();
			this.layoutControlContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemTreeList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemShowOnlyHighlighted)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(664, 142, 749, 739);
			this.layoutControlBase.OptionsView.UseDefaultDragAndDropRendering = false;
			this.layoutControlBase.Controls.SetChildIndex(this.panelAdditionalButtons, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.panelBaseContent.Controls.Add(this.layoutControlContent);
			this.panelBaseContent.Padding = new System.Windows.Forms.Padding(50, 10, 50, 25);
			this.treeListColumnHighlighted.FieldName = "Highlighted";
			this.treeListColumnHighlighted.Name = "treeListColumnHighlighted";
			this.treeListColumnHighlighted.OptionsColumn.AllowEdit = false;
			this.treeListColumnHighlighted.SortOrder = System.Windows.Forms.SortOrder.Descending;
			this.treeList.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[] {
			this.treeListColumnNodeType,
			this.treeListColumnOrder,
			this.treeListColumnName,
			this.treeListColumnHighlighted,
			this.treeListColumnData});
			filterCondition1.Column = this.treeListColumnHighlighted;
			filterCondition1.Condition = DevExpress.XtraTreeList.FilterConditionEnum.Equals;
			filterCondition1.Value1 = false;
			this.treeList.FilterConditions.AddRange(new DevExpress.XtraTreeList.FilterCondition[] {
			filterCondition1});
			styleFormatCondition1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			styleFormatCondition1.Appearance.Options.UseFont = true;
			styleFormatCondition1.ApplyToRow = true;
			styleFormatCondition1.Column = this.treeListColumnHighlighted;
			styleFormatCondition1.Condition = DevExpress.XtraGrid.FormatConditionEnum.Equal;
			styleFormatCondition1.Name = "Highlighting";
			styleFormatCondition1.Value1 = true;
			this.treeList.FormatConditions.AddRange(new DevExpress.XtraTreeList.StyleFormatConditions.StyleFormatCondition[] {
			styleFormatCondition1});
			this.treeList.ImageIndexFieldName = "NodeType";
			this.treeList.Location = new System.Drawing.Point(0, 0);
			this.treeList.Name = "treeList";
			this.treeList.OptionsBehavior.AllowIncrementalSearch = true;
			this.treeList.OptionsBehavior.Editable = false;
			this.treeList.OptionsBehavior.ExpandNodesOnIncrementalSearch = true;
			this.treeList.OptionsBehavior.ReadOnly = true;
			this.treeList.OptionsView.FocusRectStyle = DevExpress.XtraTreeList.DrawFocusRectStyle.RowFocus;
			this.treeList.OptionsView.ShowColumns = false;
			this.treeList.OptionsView.ShowHorzLines = false;
			this.treeList.OptionsView.ShowIndicator = false;
			this.treeList.OptionsView.ShowVertLines = false;
			this.treeList.Size = new System.Drawing.Size(506, 279);
			this.treeList.TabIndex = 1;
			this.treeList.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.treeList_FocusedNodeChanged);
			this.treeList.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.treeList_CustomDrawNodeCell);
			this.treeList.DoubleClick += new System.EventHandler(this.treeList_DoubleClick);
			this.treeListColumnNodeType.FieldName = "NodeType";
			this.treeListColumnNodeType.Name = "treeListColumnNodeType";
			this.treeListColumnOrder.FieldName = "Order";
			this.treeListColumnOrder.Name = "treeListColumnOrder";
			this.treeListColumnOrder.SortOrder = System.Windows.Forms.SortOrder.Ascending;
			this.treeListColumnName.FieldName = "Name";
			this.treeListColumnName.Name = "treeListColumnName";
			this.treeListColumnName.OptionsColumn.AllowEdit = false;
			this.treeListColumnName.SortOrder = System.Windows.Forms.SortOrder.Ascending;
			this.treeListColumnName.Visible = true;
			this.treeListColumnName.VisibleIndex = 0;
			this.treeListColumnName.Width = 393;
			this.treeListColumnData.FieldName = "Data";
			this.treeListColumnData.Name = "treeListColumnData";
			this.checkEditShowOnlyHighlighted.Location = new System.Drawing.Point(2, 281);
			this.checkEditShowOnlyHighlighted.Margin = new System.Windows.Forms.Padding(0);
			this.checkEditShowOnlyHighlighted.Name = "checkEditShowOnlyHighlighted";
			this.checkEditShowOnlyHighlighted.Properties.Caption = "Show only highlighted types";
			this.checkEditShowOnlyHighlighted.Size = new System.Drawing.Size(504, 19);
			this.checkEditShowOnlyHighlighted.StyleController = this.layoutControlContent;
			this.checkEditShowOnlyHighlighted.TabIndex = 2;
			this.checkEditShowOnlyHighlighted.CheckedChanged += new System.EventHandler(this.checkEditShowAll_CheckedChanged);
			this.layoutControlContent.AllowCustomization = false;
			this.layoutControlContent.Controls.Add(this.checkEditShowOnlyHighlighted);
			this.layoutControlContent.Controls.Add(this.treeList);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(50, 10);
			this.layoutControlContent.Margin = new System.Windows.Forms.Padding(0);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(703, 149, 931, 779);
			this.layoutControlContent.Root = this.layoutGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(506, 302);
			this.layoutControlContent.TabIndex = 3;
			this.layoutGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupContent.GroupBordersVisible = false;
			this.layoutGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemTreeList,
			this.layoutItemShowOnlyHighlighted});
			this.layoutGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupContent.Name = "layoutGroupContent";
			this.layoutGroupContent.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutGroupContent.Size = new System.Drawing.Size(506, 302);
			this.layoutGroupContent.TextVisible = false;
			this.layoutItemTreeList.Control = this.treeList;
			this.layoutItemTreeList.Location = new System.Drawing.Point(0, 0);
			this.layoutItemTreeList.Name = "layoutItemTreeList";
			this.layoutItemTreeList.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutItemTreeList.Size = new System.Drawing.Size(506, 279);
			this.layoutItemTreeList.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemTreeList.TextVisible = false;
			this.layoutItemShowOnlyHighlighted.Control = this.checkEditShowOnlyHighlighted;
			this.layoutItemShowOnlyHighlighted.Location = new System.Drawing.Point(0, 279);
			this.layoutItemShowOnlyHighlighted.Name = "layoutItemShowOnlyHighlighted";
			this.layoutItemShowOnlyHighlighted.Padding = new DevExpress.XtraLayout.Utils.Padding(2, 0, 2, 2);
			this.layoutItemShowOnlyHighlighted.Size = new System.Drawing.Size(506, 23);
			this.layoutItemShowOnlyHighlighted.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemShowOnlyHighlighted.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(48, 22, 48, 22);
			this.Name = "ChooseObjectTypePageView";
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
			((System.ComponentModel.ISupportInitialize)(this.layoutItemAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.panelAdditionalButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.treeList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditShowOnlyHighlighted.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).EndInit();
			this.layoutControlContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemTreeList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemShowOnlyHighlighted)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraTreeList.TreeList treeList;
		protected XtraEditors.CheckEdit checkEditShowOnlyHighlighted;
		protected XtraTreeList.Columns.TreeListColumn treeListColumnName;
		protected XtraTreeList.Columns.TreeListColumn treeListColumnHighlighted;
		protected XtraTreeList.Columns.TreeListColumn treeListColumnData;
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraLayout.LayoutControlGroup layoutGroupContent;
		protected XtraLayout.LayoutControlItem layoutItemTreeList;
		protected XtraLayout.LayoutControlItem layoutItemShowOnlyHighlighted;
		protected XtraTreeList.Columns.TreeListColumn treeListColumnNodeType;
		protected XtraTreeList.Columns.TreeListColumn treeListColumnOrder;
	}
}
