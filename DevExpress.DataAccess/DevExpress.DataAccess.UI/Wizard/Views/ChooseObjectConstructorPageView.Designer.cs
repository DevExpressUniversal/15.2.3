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
	partial class ChooseObjectConstructorPageView {
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
			this.layoutControlContent = new DevExpress.XtraLayout.LayoutControl();
			this.gridControlCtors = new DevExpress.XtraGrid.GridControl();
			this.gridViewCtors = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridColumnParameters = new DevExpress.XtraGrid.Columns.GridColumn();
			this.checkEditShowOnlyHighlighted = new DevExpress.XtraEditors.CheckEdit();
			this.layoutGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemShowAll = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemCtorsGrid = new DevExpress.XtraLayout.LayoutControlItem();
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
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).BeginInit();
			this.layoutControlContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridControlCtors)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewCtors)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditShowOnlyHighlighted.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemShowAll)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCtorsGrid)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(864, 200, 749, 738);
			this.layoutControlBase.Controls.SetChildIndex(this.panelAdditionalButtons, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.panelBaseContent.Controls.Add(this.layoutControlContent);
			this.panelBaseContent.Padding = new System.Windows.Forms.Padding(50, 10, 50, 25);
			this.gridColumnHighlighted.FieldName = "Highlighted";
			this.gridColumnHighlighted.Name = "gridColumnHighlighted";
			this.layoutControlContent.AllowCustomization = false;
			this.layoutControlContent.Controls.Add(this.gridControlCtors);
			this.layoutControlContent.Controls.Add(this.checkEditShowOnlyHighlighted);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(50, 10);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(635, 146, 818, 578);
			this.layoutControlContent.Root = this.layoutGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(506, 302);
			this.layoutControlContent.TabIndex = 3;
			this.gridControlCtors.Cursor = System.Windows.Forms.Cursors.Default;
			this.gridControlCtors.Location = new System.Drawing.Point(2, 2);
			this.gridControlCtors.MainView = this.gridViewCtors;
			this.gridControlCtors.Name = "gridControlCtors";
			this.gridControlCtors.Size = new System.Drawing.Size(502, 275);
			this.gridControlCtors.TabIndex = 4;
			this.gridControlCtors.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridViewCtors});
			this.gridViewCtors.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.gridColumnHighlighted,
			this.gridColumnParameters});
			gridFormatRule1.ApplyToRow = true;
			gridFormatRule1.Column = this.gridColumnHighlighted;
			gridFormatRule1.Name = "Highlighting";
			formatConditionRuleValue1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			formatConditionRuleValue1.Appearance.Options.UseFont = true;
			formatConditionRuleValue1.Condition = DevExpress.XtraEditors.FormatCondition.Equal;
			formatConditionRuleValue1.Value1 = true;
			gridFormatRule1.Rule = formatConditionRuleValue1;
			this.gridViewCtors.FormatRules.Add(gridFormatRule1);
			this.gridViewCtors.GridControl = this.gridControlCtors;
			this.gridViewCtors.Name = "gridViewCtors";
			this.gridViewCtors.OptionsBehavior.Editable = false;
			this.gridViewCtors.OptionsBehavior.ReadOnly = true;
			this.gridViewCtors.OptionsSelection.EnableAppearanceHideSelection = false;
			this.gridViewCtors.OptionsView.GroupFooterShowMode = DevExpress.XtraGrid.Views.Grid.GroupFooterShowMode.Hidden;
			this.gridViewCtors.OptionsView.ShowColumnHeaders = false;
			this.gridViewCtors.OptionsView.ShowDetailButtons = false;
			this.gridViewCtors.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
			this.gridViewCtors.OptionsView.ShowGroupExpandCollapseButtons = false;
			this.gridViewCtors.OptionsView.ShowGroupPanel = false;
			this.gridViewCtors.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewCtors.OptionsView.ShowIndicator = false;
			this.gridViewCtors.OptionsView.ShowPreviewRowLines = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewCtors.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.True;
			this.gridViewCtors.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gridViewCtors_CustomDrawCell);
			this.gridViewCtors.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewCtors_FocusedRowChanged);
			this.gridViewCtors.DoubleClick += new System.EventHandler(this.gridViewCtors_DoubleClick);
			this.gridColumnParameters.FieldName = "Parameters";
			this.gridColumnParameters.Name = "gridColumnParameters";
			this.gridColumnParameters.Visible = true;
			this.gridColumnParameters.VisibleIndex = 0;
			this.checkEditShowOnlyHighlighted.Location = new System.Drawing.Point(2, 281);
			this.checkEditShowOnlyHighlighted.Name = "checkEditShowOnlyHighlighted";
			this.checkEditShowOnlyHighlighted.Properties.Caption = "Show only highlighted constructors";
			this.checkEditShowOnlyHighlighted.Size = new System.Drawing.Size(502, 19);
			this.checkEditShowOnlyHighlighted.StyleController = this.layoutControlContent;
			this.checkEditShowOnlyHighlighted.TabIndex = 1;
			this.checkEditShowOnlyHighlighted.CheckedChanged += new System.EventHandler(this.checkEditShowAll_CheckedChanged);
			this.layoutGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupContent.GroupBordersVisible = false;
			this.layoutGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemShowAll,
			this.layoutItemCtorsGrid});
			this.layoutGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupContent.Name = "layoutGroupContent";
			this.layoutGroupContent.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutGroupContent.Size = new System.Drawing.Size(506, 302);
			this.layoutGroupContent.TextVisible = false;
			this.layoutItemShowAll.Control = this.checkEditShowOnlyHighlighted;
			this.layoutItemShowAll.Location = new System.Drawing.Point(0, 279);
			this.layoutItemShowAll.Name = "layoutItemShowAll";
			this.layoutItemShowAll.Size = new System.Drawing.Size(506, 23);
			this.layoutItemShowAll.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemShowAll.TextVisible = false;
			this.layoutItemCtorsGrid.Control = this.gridControlCtors;
			this.layoutItemCtorsGrid.Location = new System.Drawing.Point(0, 0);
			this.layoutItemCtorsGrid.Name = "layoutItemCtorsGrid";
			this.layoutItemCtorsGrid.Size = new System.Drawing.Size(506, 279);
			this.layoutItemCtorsGrid.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemCtorsGrid.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(48, 22, 48, 22);
			this.Name = "ChooseObjectConstructorPageView";
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
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).EndInit();
			this.layoutControlContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridControlCtors)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewCtors)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkEditShowOnlyHighlighted.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemShowAll)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemCtorsGrid)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraEditors.CheckEdit checkEditShowOnlyHighlighted;
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraLayout.LayoutControlGroup layoutGroupContent;
		protected XtraLayout.LayoutControlItem layoutItemShowAll;
		protected XtraGrid.GridControl gridControlCtors;
		protected XtraGrid.Views.Grid.GridView gridViewCtors;
		protected XtraLayout.LayoutControlItem layoutItemCtorsGrid;
		protected XtraGrid.Columns.GridColumn gridColumnHighlighted;
		protected XtraGrid.Columns.GridColumn gridColumnParameters;
	}
}
