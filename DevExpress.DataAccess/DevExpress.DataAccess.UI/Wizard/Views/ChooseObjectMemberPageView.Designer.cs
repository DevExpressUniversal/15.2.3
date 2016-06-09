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
	partial class ChooseObjectMemberPageView {
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
			this.checkEditShowOnlyHighlighted = new DevExpress.XtraEditors.CheckEdit();
			this.layoutControlContent = new DevExpress.XtraLayout.LayoutControl();
			this.gridControlMembers = new DevExpress.XtraGrid.GridControl();
			this.gridViewMembers = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridColumnDescription = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumnIcon = new DevExpress.XtraGrid.Columns.GridColumn();
			this.repositoryItemMemberType = new DevExpress.XtraEditors.Repository.RepositoryItemImageComboBox();
			this.radioGroupEntireObject = new DevExpress.XtraEditors.RadioGroup();
			this.layoutGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemGroupEntireObject = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemShowOnlyHighlighted = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemMembersGrid = new DevExpress.XtraLayout.LayoutControlItem();
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
			((System.ComponentModel.ISupportInitialize)(this.checkEditShowOnlyHighlighted.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).BeginInit();
			this.layoutControlContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridControlMembers)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewMembers)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemberType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.radioGroupEntireObject.Properties)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemGroupEntireObject)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemShowOnlyHighlighted)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemMembersGrid)).BeginInit();
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
			this.buttonNext.TabIndex = 0;
			this.buttonFinish.TabIndex = 1;
			this.panelBaseContent.Controls.Add(this.layoutControlContent);
			this.panelBaseContent.Padding = new System.Windows.Forms.Padding(50, 10, 50, 25);
			this.gridColumnHighlighted.FieldName = "Highlighted";
			this.gridColumnHighlighted.Name = "gridColumnHighlighted";
			this.gridColumnHighlighted.OptionsColumn.AllowIncrementalSearch = false;
			this.checkEditShowOnlyHighlighted.Location = new System.Drawing.Point(2, 281);
			this.checkEditShowOnlyHighlighted.Name = "checkEditShowOnlyHighlighted";
			this.checkEditShowOnlyHighlighted.Properties.Caption = "Show only highlighted members";
			this.checkEditShowOnlyHighlighted.Size = new System.Drawing.Size(502, 19);
			this.checkEditShowOnlyHighlighted.StyleController = this.layoutControlContent;
			this.checkEditShowOnlyHighlighted.TabIndex = 4;
			this.checkEditShowOnlyHighlighted.CheckedChanged += new System.EventHandler(this.checkEditShowAll_CheckedChanged);
			this.layoutControlContent.AllowCustomization = false;
			this.layoutControlContent.Controls.Add(this.gridControlMembers);
			this.layoutControlContent.Controls.Add(this.radioGroupEntireObject);
			this.layoutControlContent.Controls.Add(this.checkEditShowOnlyHighlighted);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(50, 10);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.Root = this.layoutGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(506, 302);
			this.layoutControlContent.TabIndex = 5;
			this.gridControlMembers.Cursor = System.Windows.Forms.Cursors.Default;
			this.gridControlMembers.Enabled = false;
			this.gridControlMembers.Location = new System.Drawing.Point(2, 44);
			this.gridControlMembers.MainView = this.gridViewMembers;
			this.gridControlMembers.Name = "gridControlMembers";
			this.gridControlMembers.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.repositoryItemMemberType});
			this.gridControlMembers.Size = new System.Drawing.Size(502, 233);
			this.gridControlMembers.TabIndex = 5;
			this.gridControlMembers.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridViewMembers});
			this.gridViewMembers.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.gridColumnDescription,
			this.gridColumnIcon,
			this.gridColumnHighlighted});
			gridFormatRule1.ApplyToRow = true;
			gridFormatRule1.Column = this.gridColumnHighlighted;
			gridFormatRule1.Name = "Highlighting";
			formatConditionRuleValue1.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold);
			formatConditionRuleValue1.Appearance.Options.UseFont = true;
			formatConditionRuleValue1.Condition = DevExpress.XtraEditors.FormatCondition.Equal;
			formatConditionRuleValue1.Value1 = true;
			gridFormatRule1.Rule = formatConditionRuleValue1;
			this.gridViewMembers.FormatRules.Add(gridFormatRule1);
			this.gridViewMembers.GridControl = this.gridControlMembers;
			this.gridViewMembers.Name = "gridViewMembers";
			this.gridViewMembers.OptionsBehavior.AllowIncrementalSearch = true;
			this.gridViewMembers.OptionsBehavior.Editable = false;
			this.gridViewMembers.OptionsBehavior.FocusLeaveOnTab = true;
			this.gridViewMembers.OptionsBehavior.ReadOnly = true;
			this.gridViewMembers.OptionsSelection.EnableAppearanceHideSelection = false;
			this.gridViewMembers.OptionsView.ShowColumnHeaders = false;
			this.gridViewMembers.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
			this.gridViewMembers.OptionsView.ShowGroupPanel = false;
			this.gridViewMembers.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewMembers.OptionsView.ShowIndicator = false;
			this.gridViewMembers.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
			this.gridViewMembers.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnIcon, DevExpress.Data.ColumnSortOrder.Ascending),
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnDescription, DevExpress.Data.ColumnSortOrder.Ascending)});
			this.gridViewMembers.CustomDrawCell += new DevExpress.XtraGrid.Views.Base.RowCellCustomDrawEventHandler(this.gridViewMembers_CustomDrawCell);
			this.gridViewMembers.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridViewMembers_FocusedRowChanged);
			this.gridViewMembers.DoubleClick += new System.EventHandler(this.gridViewMembers_DoubleClick);
			this.gridColumnDescription.FieldName = "Description";
			this.gridColumnDescription.Name = "gridColumnDescription";
			this.gridColumnDescription.Visible = true;
			this.gridColumnDescription.VisibleIndex = 1;
			this.gridColumnDescription.Width = 464;
			this.gridColumnIcon.ColumnEdit = this.repositoryItemMemberType;
			this.gridColumnIcon.FieldName = "MemberType";
			this.gridColumnIcon.Name = "gridColumnIcon";
			this.gridColumnIcon.OptionsColumn.AllowFocus = false;
			this.gridColumnIcon.OptionsColumn.AllowIncrementalSearch = false;
			this.gridColumnIcon.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
			this.gridColumnIcon.Visible = true;
			this.gridColumnIcon.VisibleIndex = 0;
			this.gridColumnIcon.Width = 20;
			this.repositoryItemMemberType.AutoHeight = false;
			this.repositoryItemMemberType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.repositoryItemMemberType.Name = "repositoryItemMemberType";
			this.radioGroupEntireObject.AutoSizeInLayoutControl = true;
			this.radioGroupEntireObject.EditValue = true;
			this.radioGroupEntireObject.Location = new System.Drawing.Point(2, 2);
			this.radioGroupEntireObject.Name = "radioGroupEntireObject";
			this.radioGroupEntireObject.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
			this.radioGroupEntireObject.Properties.Appearance.Options.UseBackColor = true;
			this.radioGroupEntireObject.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.radioGroupEntireObject.Properties.Columns = 1;
			this.radioGroupEntireObject.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[] {
			new DevExpress.XtraEditors.Controls.RadioGroupItem(true, "Do not select a member, bind to the entire object."),
			new DevExpress.XtraEditors.Controls.RadioGroupItem(false, "Select a member to bind.")});
			this.radioGroupEntireObject.Size = new System.Drawing.Size(502, 38);
			this.radioGroupEntireObject.StyleController = this.layoutControlContent;
			this.radioGroupEntireObject.TabIndex = 2;
			this.radioGroupEntireObject.SelectedIndexChanged += new System.EventHandler(this.radioGroupMemberType_SelectedIndexChanged);
			this.layoutGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutGroupContent.GroupBordersVisible = false;
			this.layoutGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemGroupEntireObject,
			this.layoutItemShowOnlyHighlighted,
			this.layoutItemMembersGrid});
			this.layoutGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutGroupContent.Name = "layoutGroupContent";
			this.layoutGroupContent.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutGroupContent.Size = new System.Drawing.Size(506, 302);
			this.layoutGroupContent.TextVisible = false;
			this.layoutItemGroupEntireObject.Control = this.radioGroupEntireObject;
			this.layoutItemGroupEntireObject.Location = new System.Drawing.Point(0, 0);
			this.layoutItemGroupEntireObject.Name = "layoutItemGroupEntireObject";
			this.layoutItemGroupEntireObject.Size = new System.Drawing.Size(506, 42);
			this.layoutItemGroupEntireObject.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemGroupEntireObject.TextVisible = false;
			this.layoutItemShowOnlyHighlighted.Control = this.checkEditShowOnlyHighlighted;
			this.layoutItemShowOnlyHighlighted.Location = new System.Drawing.Point(0, 279);
			this.layoutItemShowOnlyHighlighted.Name = "layoutItemShowOnlyHighlighted";
			this.layoutItemShowOnlyHighlighted.Size = new System.Drawing.Size(506, 23);
			this.layoutItemShowOnlyHighlighted.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemShowOnlyHighlighted.TextVisible = false;
			this.layoutItemMembersGrid.Control = this.gridControlMembers;
			this.layoutItemMembersGrid.Location = new System.Drawing.Point(0, 42);
			this.layoutItemMembersGrid.Name = "layoutItemMembersGrid";
			this.layoutItemMembersGrid.Size = new System.Drawing.Size(506, 237);
			this.layoutItemMembersGrid.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemMembersGrid.TextVisible = false;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(48, 22, 48, 22);
			this.Name = "ChooseObjectMemberPageView";
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
			((System.ComponentModel.ISupportInitialize)(this.checkEditShowOnlyHighlighted.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).EndInit();
			this.layoutControlContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridControlMembers)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridViewMembers)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemMemberType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.radioGroupEntireObject.Properties)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemGroupEntireObject)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemShowOnlyHighlighted)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemMembersGrid)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraEditors.CheckEdit checkEditShowOnlyHighlighted;
		protected XtraEditors.RadioGroup radioGroupEntireObject;
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraLayout.LayoutControlGroup layoutGroupContent;
		protected XtraLayout.LayoutControlItem layoutItemGroupEntireObject;
		protected XtraLayout.LayoutControlItem layoutItemShowOnlyHighlighted;
		protected XtraGrid.GridControl gridControlMembers;
		protected XtraGrid.Views.Grid.GridView gridViewMembers;
		protected XtraEditors.Repository.RepositoryItemImageComboBox repositoryItemMemberType;
		protected XtraLayout.LayoutControlItem layoutItemMembersGrid;
		protected XtraGrid.Columns.GridColumn gridColumnIcon;
		protected XtraGrid.Columns.GridColumn gridColumnDescription;
		protected XtraGrid.Columns.GridColumn gridColumnHighlighted;
	}
}
