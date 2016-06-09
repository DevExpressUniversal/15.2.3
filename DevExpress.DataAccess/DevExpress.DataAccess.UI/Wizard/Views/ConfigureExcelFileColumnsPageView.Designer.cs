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
	partial class ConfigureExcelFileColumnsPageView {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		private void InitializeComponent() {
			this.layoutControlContent = new DevExpress.XtraLayout.LayoutControl();
			this.separatorBottom = new DevExpress.XtraEditors.LabelControl();
			this.gridControl = new DevExpress.XtraGrid.GridControl();
			this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.gridColumnName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumnType = new DevExpress.XtraGrid.Columns.GridColumn();
			this.comboBoxType = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.gridColumnSelected = new DevExpress.XtraGrid.Columns.GridColumn();
			this.checkSelected = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
			this.layoutControlGroupContent = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemGrid = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutItemSeparatorBottom = new DevExpress.XtraLayout.LayoutControlItem();
			this.layoutControlButtons = new DevExpress.XtraLayout.LayoutControl();
			this.buttonPreview = new DevExpress.XtraEditors.SimpleButton();
			this.layoutControlGroupButtons = new DevExpress.XtraLayout.LayoutControlGroup();
			this.layoutItemPreview = new DevExpress.XtraLayout.LayoutControlItem();
			this.emptySpaceButtons = new DevExpress.XtraLayout.EmptySpaceItem();
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
			this.panelAdditionalButtons.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).BeginInit();
			this.layoutControlContent.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.checkSelected)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupContent)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemGrid)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorBottom)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlButtons)).BeginInit();
			this.layoutControlButtons.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupButtons)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreview)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceButtons)).BeginInit();
			this.SuspendLayout();
			this.layoutControlBase.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(864, 200, 749, 738);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonFinish, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonNext, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.buttonPrevious, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelBaseContent, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.labelHeader, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.separatorTop, 0);
			this.layoutControlBase.Controls.SetChildIndex(this.panelAdditionalButtons, 0);
			this.separatorTop.Margin = new System.Windows.Forms.Padding(0, 7, 0, 0);
			this.layoutItemSeparatorTop.MinSize = new System.Drawing.Size(10, 6);
			this.panelBaseContent.Controls.Add(this.layoutControlContent);
			this.panelAdditionalButtons.Controls.Add(this.layoutControlButtons);
			this.layoutControlContent.AllowCustomization = false;
			this.layoutControlContent.Controls.Add(this.separatorBottom);
			this.layoutControlContent.Controls.Add(this.gridControl);
			this.layoutControlContent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlContent.Location = new System.Drawing.Point(0, 0);
			this.layoutControlContent.Name = "layoutControlContent";
			this.layoutControlContent.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(655, 160, 852, 736);
			this.layoutControlContent.Root = this.layoutControlGroupContent;
			this.layoutControlContent.Size = new System.Drawing.Size(606, 337);
			this.layoutControlContent.TabIndex = 0;
			this.separatorBottom.LineLocation = DevExpress.XtraEditors.LineLocation.Center;
			this.separatorBottom.LineOrientation = DevExpress.XtraEditors.LabelLineOrientation.Horizontal;
			this.separatorBottom.LineVisible = true;
			this.separatorBottom.Location = new System.Drawing.Point(0, 324);
			this.separatorBottom.Name = "separatorBottom";
			this.separatorBottom.Padding = new System.Windows.Forms.Padding(0, 0, 0, 13);
			this.separatorBottom.Size = new System.Drawing.Size(606, 13);
			this.separatorBottom.StyleController = this.layoutControlContent;
			this.separatorBottom.TabIndex = 5;
			this.gridControl.Location = new System.Drawing.Point(0, 0);
			this.gridControl.MainView = this.gridView;
			this.gridControl.Margin = new System.Windows.Forms.Padding(0);
			this.gridControl.Name = "gridControl";
			this.gridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.comboBoxType,
			this.checkSelected});
			this.gridControl.Size = new System.Drawing.Size(606, 324);
			this.gridControl.TabIndex = 4;
			this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridView});
			this.gridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.gridColumnName,
			this.gridColumnType,
			this.gridColumnSelected});
			this.gridView.GridControl = this.gridControl;
			this.gridView.Name = "gridView";
			this.gridView.OptionsCustomization.AllowGroup = false;
			this.gridView.OptionsView.ShowGroupPanel = false;
			this.gridView.OptionsView.ShowIndicator = false;
			this.gridView.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView_CellValueChanged);
			this.gridView.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gridView1_CustomColumnDisplayText);
			this.gridView.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gridView1_ValidatingEditor);
			this.gridColumnName.Caption = "Name";
			this.gridColumnName.FieldName = "Name";
			this.gridColumnName.Name = "gridColumnName";
			this.gridColumnName.Visible = true;
			this.gridColumnName.VisibleIndex = 1;
			this.gridColumnName.Width = 264;
			this.gridColumnType.Caption = "Type";
			this.gridColumnType.ColumnEdit = this.comboBoxType;
			this.gridColumnType.FieldName = "Type";
			this.gridColumnType.Name = "gridColumnType";
			this.gridColumnType.Visible = true;
			this.gridColumnType.VisibleIndex = 2;
			this.gridColumnType.Width = 269;
			this.comboBoxType.AutoHeight = false;
			this.comboBoxType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.comboBoxType.Name = "comboBoxType";
			this.comboBoxType.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.comboBoxType.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.cbeType_CustomDisplayText);
			this.gridColumnSelected.Caption = "Selected";
			this.gridColumnSelected.ColumnEdit = this.checkSelected;
			this.gridColumnSelected.FieldName = "Selected";
			this.gridColumnSelected.Name = "gridColumnSelected";
			this.gridColumnSelected.Visible = true;
			this.gridColumnSelected.VisibleIndex = 0;
			this.gridColumnSelected.Width = 53;
			this.checkSelected.AutoHeight = false;
			this.checkSelected.Name = "checkSelected";
			this.checkSelected.CheckedChanged += new System.EventHandler(this.ceSelected_CheckedChanged);
			this.layoutControlGroupContent.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroupContent.GroupBordersVisible = false;
			this.layoutControlGroupContent.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemGrid,
			this.layoutItemSeparatorBottom});
			this.layoutControlGroupContent.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroupContent.Name = "Root";
			this.layoutControlGroupContent.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroupContent.Size = new System.Drawing.Size(606, 337);
			this.layoutControlGroupContent.TextVisible = false;
			this.layoutItemGrid.Control = this.gridControl;
			this.layoutItemGrid.Location = new System.Drawing.Point(0, 0);
			this.layoutItemGrid.Name = "layoutItemGrid";
			this.layoutItemGrid.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutItemGrid.Size = new System.Drawing.Size(606, 324);
			this.layoutItemGrid.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemGrid.TextVisible = false;
			this.layoutItemSeparatorBottom.Control = this.separatorBottom;
			this.layoutItemSeparatorBottom.Location = new System.Drawing.Point(0, 324);
			this.layoutItemSeparatorBottom.MinSize = new System.Drawing.Size(10, 13);
			this.layoutItemSeparatorBottom.Name = "layoutItemSeparatorBottom";
			this.layoutItemSeparatorBottom.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutItemSeparatorBottom.Size = new System.Drawing.Size(606, 13);
			this.layoutItemSeparatorBottom.SizeConstraintsType = DevExpress.XtraLayout.SizeConstraintsType.Custom;
			this.layoutItemSeparatorBottom.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemSeparatorBottom.TextVisible = false;
			this.layoutControlButtons.Controls.Add(this.buttonPreview);
			this.layoutControlButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.layoutControlButtons.Location = new System.Drawing.Point(0, 0);
			this.layoutControlButtons.Name = "layoutControlButtons";
			this.layoutControlButtons.OptionsCustomizationForm.DesignTimeCustomizationFormPositionAndSize = new System.Drawing.Rectangle(578, 229, 1083, 772);
			this.layoutControlButtons.Root = this.layoutControlGroupButtons;
			this.layoutControlButtons.Size = new System.Drawing.Size(424, 26);
			this.layoutControlButtons.TabIndex = 0;
			this.buttonPreview.Location = new System.Drawing.Point(12, 2);
			this.buttonPreview.Name = "buttonPreview";
			this.buttonPreview.Size = new System.Drawing.Size(80, 22);
			this.buttonPreview.StyleController = this.layoutControlButtons;
			this.buttonPreview.TabIndex = 4;
			this.buttonPreview.Text = "&Preview...";
			this.buttonPreview.Click += new System.EventHandler(this.btnPreview_Click);
			this.layoutControlGroupButtons.EnableIndentsWithoutBorders = DevExpress.Utils.DefaultBoolean.True;
			this.layoutControlGroupButtons.GroupBordersVisible = false;
			this.layoutControlGroupButtons.Items.AddRange(new DevExpress.XtraLayout.BaseLayoutItem[] {
			this.layoutItemPreview,
			this.emptySpaceButtons});
			this.layoutControlGroupButtons.Location = new System.Drawing.Point(0, 0);
			this.layoutControlGroupButtons.Name = "layoutControlGroupButtons";
			this.layoutControlGroupButtons.Padding = new DevExpress.XtraLayout.Utils.Padding(0, 0, 0, 0);
			this.layoutControlGroupButtons.Size = new System.Drawing.Size(424, 26);
			this.layoutControlGroupButtons.TextVisible = false;
			this.layoutItemPreview.Control = this.buttonPreview;
			this.layoutItemPreview.Location = new System.Drawing.Point(0, 0);
			this.layoutItemPreview.Name = "layoutItemPreview";
			this.layoutItemPreview.Padding = new DevExpress.XtraLayout.Utils.Padding(12, 2, 2, 2);
			this.layoutItemPreview.Size = new System.Drawing.Size(94, 26);
			this.layoutItemPreview.TextSize = new System.Drawing.Size(0, 0);
			this.layoutItemPreview.TextVisible = false;
			this.emptySpaceButtons.AllowHotTrack = false;
			this.emptySpaceButtons.Location = new System.Drawing.Point(94, 0);
			this.emptySpaceButtons.Name = "emptySpaceButtons";
			this.emptySpaceButtons.Size = new System.Drawing.Size(330, 26);
			this.emptySpaceButtons.TextSize = new System.Drawing.Size(0, 0);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "ConfigureExcelFileColumnsPageView";
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
			this.panelAdditionalButtons.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlContent)).EndInit();
			this.layoutControlContent.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.comboBoxType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.checkSelected)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupContent)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemGrid)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemSeparatorBottom)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutControlButtons)).EndInit();
			this.layoutControlButtons.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.layoutControlGroupButtons)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.layoutItemPreview)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.emptySpaceButtons)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();
		}
		#endregion
		protected XtraLayout.LayoutControl layoutControlContent;
		protected XtraGrid.GridControl gridControl;
		protected XtraGrid.Views.Grid.GridView gridView;
		protected XtraGrid.Columns.GridColumn gridColumnName;
		protected XtraGrid.Columns.GridColumn gridColumnType;
		protected XtraLayout.LayoutControlGroup layoutControlGroupContent;
		protected XtraLayout.LayoutControlItem layoutItemGrid;
		protected XtraEditors.Repository.RepositoryItemComboBox comboBoxType;
		protected XtraLayout.LayoutControl layoutControlButtons;
		protected XtraLayout.LayoutControlGroup layoutControlGroupButtons;
		protected XtraEditors.SimpleButton buttonPreview;
		protected XtraLayout.LayoutControlItem layoutItemPreview;
		protected XtraLayout.EmptySpaceItem emptySpaceButtons;
		protected XtraEditors.LabelControl separatorBottom;
		protected XtraLayout.LayoutControlItem layoutItemSeparatorBottom;
		protected XtraGrid.Columns.GridColumn gridColumnSelected;
		protected XtraEditors.Repository.RepositoryItemCheckEdit checkSelected;
	}
}
