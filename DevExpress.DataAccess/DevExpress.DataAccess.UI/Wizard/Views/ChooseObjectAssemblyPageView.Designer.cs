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
	partial class ChooseObjectAssemblyPageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			DevExpress.XtraGrid.GridFormatRule gridFormatRule1 = new DevExpress.XtraGrid.GridFormatRule();
			DevExpress.XtraEditors.FormatConditionRuleValue formatConditionRuleValue1 = new DevExpress.XtraEditors.FormatConditionRuleValue();
			this.gridColumnHighlighted = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridControl = new DevExpress.XtraGrid.GridControl();
			this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridColumnPriority = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumnName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumnVersion = new DevExpress.XtraGrid.Columns.GridColumn();
			this.checkEditShowOnlyHighlighted = new DevExpress.XtraEditors.CheckEdit();
			this.layoutControlContent = new DevExpress.XtraLayout.LayoutControl();
			this.separatorMiddle = new DevExpress.XtraEditors.LabelControl();
			this.layoutGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemGridControl = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemShowOnlyHighlighted = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemSeparatorMiddle = new DevExpress.XtraLayout.LayoutControlItem();
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
			((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditShowOnlyHighlighted.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).BeginInit();
			this.layoutControlContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemGridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemShowOnlyHighlighted)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorMiddle)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(664, 142, 749, 739);
			this.layoutControlBase.OptionsView.UseDefaultDragAndDropRendering = false;
			this.layoutControlBase.Controls.SetChildIndex(this.panelAdditionalButtons, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.panelBaseContent.Controls.Add(this.layoutControlContent);
			this.panelBaseContent.Padding = new System.Windows.Forms.Padding(0, 0, 0, 25);
			this.gridColumnHighlighted.FieldName = "Highlighted";
			this.gridColumnHighlighted.Name = "gridColumnHighlighted";
			this.gridControl.Cursor = System.Windows.Forms.Cursors.Default;
			this.gridControl.Location = new System.Drawing.Point(0, 0);
			this.gridControl.MainView = this.gridView;
			this.gridControl.Name = "gridControl";
			this.gridControl.Size = new System.Drawing.Size(606, 285);
			this.gridControl.TabIndex = 0;
			this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridView});
			this.gridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.gridColumnHighlighted,
			this.gridColumnPriority,
			this.gridColumnName,
			this.gridColumnVersion});
			gridFormatRule1.ApplyToRow = true;
			gridFormatRule1.Column = this.gridColumnHighlighted;
			gridFormatRule1.Name = "Highlighting";
			formatConditionRuleValue1.Appearance.FontStyleDelta = System.Drawing.FontStyle.Bold;
			formatConditionRuleValue1.Appearance.Options.UseFont = true;
			formatConditionRuleValue1.Condition = DevExpress.XtraEditors.FormatCondition.Equal;
			formatConditionRuleValue1.Value1 = true;
			gridFormatRule1.Rule = formatConditionRuleValue1;
			this.gridView.FormatRules.Add(gridFormatRule1);
			this.gridView.GridControl = this.gridControl;
			this.gridView.Name = "gridView";
			this.gridView.OptionsBehavior.AllowIncrementalSearch = true;
			this.gridView.OptionsBehavior.Editable = false;
			this.gridView.OptionsBehavior.ReadOnly = true;
			this.gridView.OptionsNavigation.UseTabKey = false;
			this.gridView.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
			this.gridView.OptionsView.ShowGroupPanel = false;
			this.gridView.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnHighlighted, DevExpress.Data.ColumnSortOrder.Descending),
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnPriority, DevExpress.Data.ColumnSortOrder.Descending),
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnName, DevExpress.Data.ColumnSortOrder.Ascending)});
			this.gridView.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gridView_CustomDrawCell);
			this.gridView.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridView1_FocusedRowChanged);
			this.gridView.DoubleClick += new System.EventHandler(this.gridView_DoubleClick);
			this.gridColumnPriority.FieldName = "Priority";
			this.gridColumnPriority.Name = "gridColumnPriority";
			this.gridColumnName.Caption = "Name";
			this.gridColumnName.FieldName = "Name";
			this.gridColumnName.Name = "gridColumnName";
			this.gridColumnName.Visible = true;
			this.gridColumnName.VisibleIndex = 0;
			this.gridColumnName.Width = 566;
			this.gridColumnVersion.Caption = "Version";
			this.gridColumnVersion.FieldName = "Version";
			this.gridColumnVersion.Name = "gridColumnVersion";
			this.gridColumnVersion.Visible = true;
			this.gridColumnVersion.VisibleIndex = 1;
			this.gridColumnVersion.Width = 130;
			this.checkEditShowOnlyHighlighted.Location = new System.Drawing.Point(2, 291);
			this.checkEditShowOnlyHighlighted.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.checkEditShowOnlyHighlighted.Name = "checkEditShowOnlyHighlighted";
			this.checkEditShowOnlyHighlighted.Properties.Caption = "Show only highlighted assemblies";
			this.checkEditShowOnlyHighlighted.Size = new System.Drawing.Size(602, 19);
			this.checkEditShowOnlyHighlighted.StyleController = this.layoutControlContent;
			this.checkEditShowOnlyHighlighted.TabIndex = 1;
			this.checkEditShowOnlyHighlighted.CheckedChanged += new System.EventHandler(this.checkEditShowAll_CheckedChanged);
			this.layoutControlContent.AllowCustomization = false;
			this.layoutControlContent.Controls.Add(this.separatorMiddle);
			this.layoutControlContent.Controls.Add(this.gridControl);
			this.layoutControlContent.Controls.Add(this.checkEditShowOnlyHighlighted);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(0, 0);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(655, 147, 1037, 786);
			this.layoutControlContent.Root = this.layoutGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(606, 312);
			this.layoutControlContent.TabIndex = 2;
			this.separatorMiddle.AutoSizeMode = DevExpress.XtraEditors.LabelAutoSizeMode.None;
			this.separatorMiddle.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.separatorMiddle.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.separatorMiddle.LineVisible = true;
			this.separatorMiddle.Location = new System.Drawing.Point(0, 285);
			this.separatorMiddle.Name = "separatorMiddle";
			this.separatorMiddle.Size = new System.Drawing.Size(606, 4);
			this.separatorMiddle.StyleController = this.layoutControlContent;
			this.separatorMiddle.TabIndex = 4;
			this.layoutGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupContent.GroupBordersVisible = false;
			this.layoutGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemGridControl,
			this.layoutItemShowOnlyHighlighted,
			this.layoutItemSeparatorMiddle});
			this.layoutGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupContent.Name = "layoutGroupContent";
			this.layoutGroupContent.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutGroupContent.Size = new System.Drawing.Size(606, 312);
			this.layoutGroupContent.TextVisible = false;
			this.layoutItemGridControl.Control = this.gridControl;
			this.layoutItemGridControl.Location = new System.Drawing.Point(0, 0);
			this.layoutItemGridControl.Name = "layoutItemGridControl";
			this.layoutItemGridControl.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutItemGridControl.Size = new System.Drawing.Size(606, 285);
			this.layoutItemGridControl.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemGridControl.TextVisible = false;
			this.layoutItemShowOnlyHighlighted.Control = this.checkEditShowOnlyHighlighted;
			this.layoutItemShowOnlyHighlighted.Location = new System.Drawing.Point(0, 289);
			this.layoutItemShowOnlyHighlighted.Name = "layoutItemShowOnlyHighlighted";
			this.layoutItemShowOnlyHighlighted.Size = new System.Drawing.Size(606, 23);
			this.layoutItemShowOnlyHighlighted.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemShowOnlyHighlighted.TextVisible = false;
			this.layoutItemSeparatorMiddle.Control = this.separatorMiddle;
			this.layoutItemSeparatorMiddle.Location = new System.Drawing.Point(0, 285);
			this.layoutItemSeparatorMiddle.MinSize = new System.Drawing.Size(10, 3);
			this.layoutItemSeparatorMiddle.Name = "layoutItemSeparatorMiddle";
			this.layoutItemSeparatorMiddle.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutItemSeparatorMiddle.Size = new System.Drawing.Size(606, 4);
			this.layoutItemSeparatorMiddle.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutItemSeparatorMiddle.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemSeparatorMiddle.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(48, 22, 48, 22);
			this.Name = "ChooseObjectAssemblyPageView";
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
			((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditShowOnlyHighlighted.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).EndInit();
			this.layoutControlContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemGridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemShowOnlyHighlighted)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorMiddle)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraGrid.GridControl gridControl;
		protected XtraGrid.Views.Grid.GridView gridView;
		protected XtraGrid.Columns.GridColumn gridColumnHighlighted;
		protected XtraGrid.Columns.GridColumn gridColumnName;
		protected XtraGrid.Columns.GridColumn gridColumnVersion;
		protected XtraEditors.CheckEdit checkEditShowOnlyHighlighted;
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraLayout.LayoutControlGroup layoutGroupContent;
		protected XtraLayout.LayoutControlItem layoutItemGridControl;
		protected XtraLayout.LayoutControlItem layoutItemShowOnlyHighlighted;
		protected XtraGrid.Columns.GridColumn gridColumnPriority;
		protected XtraEditors.LabelControl separatorMiddle;
		protected XtraLayout.LayoutControlItem layoutItemSeparatorMiddle;
	}
}
