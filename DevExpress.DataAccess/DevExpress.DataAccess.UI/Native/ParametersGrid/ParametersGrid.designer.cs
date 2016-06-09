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

namespace DevExpress.DataAccess.UI.Native.ParametersGrid {
	public partial class ParametersGrid {
		System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Component Designer generated code
		void InitializeComponent() {
			this.gridControl = new DevExpress.XtraGrid.GridControl();
			this.gridView = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.nameColumn = new DevExpress.XtraGrid.Columns.GridColumn();
			this.typeColumn = new DevExpress.XtraGrid.Columns.GridColumn();
			this.typeComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			this.isExprColumn = new DevExpress.XtraGrid.Columns.GridColumn();
			this.isExprCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
			this.valueColumn = new DevExpress.XtraGrid.Columns.GridColumn();
			this.valueComboBox = new DevExpress.XtraEditors.Repository.RepositoryItemComboBox();
			((System.ComponentModel.ISupportInitialize)(this.gridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.typeComboBox)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.isExprCheckEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.valueComboBox)).BeginInit();
			this.SuspendLayout();
			this.gridControl.Cursor = System.Windows.Forms.Cursors.Default;
			this.gridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridControl.Location = new System.Drawing.Point(2, 2);
			this.gridControl.MainView = this.gridView;
			this.gridControl.Name = "gridControl";
			this.gridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
			this.typeComboBox,
			this.valueComboBox,
			this.isExprCheckEdit});
			this.gridControl.Size = new System.Drawing.Size(507, 311);
			this.gridControl.TabIndex = 5;
			this.gridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gridView});
			this.gridView.ActiveFilterEnabled = false;
			this.gridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.gridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.nameColumn,
			this.typeColumn,
			this.isExprColumn,
			this.valueColumn});
			this.gridView.GridControl = this.gridControl;
			this.gridView.Name = "gridView";
			this.gridView.OptionsBehavior.AutoExpandAllGroups = true;
			this.gridView.OptionsCustomization.AllowColumnMoving = false;
			this.gridView.OptionsCustomization.AllowFilter = false;
			this.gridView.OptionsCustomization.AllowQuickHideColumns = false;
			this.gridView.OptionsNavigation.AutoFocusNewRow = true;
			this.gridView.OptionsNavigation.UseTabKey = false;
			this.gridView.OptionsView.ShowGroupPanel = false;
			this.gridView.OptionsView.ShowIndicator = false;
			this.gridView.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.GridViewCustomRowCellEdit);
			this.gridView.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.GridViewPopupMenuShowing);
			this.gridView.ShownEditor += new System.EventHandler(this.GridViewShownEditor);
			this.gridView.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.GridViewCustomColumnDisplayText);
			this.gridView.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.GridViewValidatingEditor);
			this.nameColumn.Caption = "Name";
			this.nameColumn.FieldName = "Name";
			this.nameColumn.Name = "nameColumn";
			this.nameColumn.Visible = true;
			this.nameColumn.VisibleIndex = 0;
			this.typeColumn.Caption = "Type";
			this.typeColumn.ColumnEdit = this.typeComboBox;
			this.typeColumn.FieldName = "Type";
			this.typeColumn.Name = "typeColumn";
			this.typeColumn.Visible = true;
			this.typeColumn.VisibleIndex = 1;
			this.typeComboBox.AutoHeight = false;
			this.typeComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.typeComboBox.ImmediatePopup = true;
			this.typeComboBox.Name = "typeComboBox";
			this.typeComboBox.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
			this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.typeComboBox_SelectedIndexChanged);
			this.typeComboBox.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.typeComboBox_CustomDisplayText);
			this.isExprColumn.Caption = "Expression";
			this.isExprColumn.ColumnEdit = this.isExprCheckEdit;
			this.isExprColumn.FieldName = "IsExpression";
			this.isExprColumn.Name = "isExprColumn";
			this.isExprColumn.Visible = true;
			this.isExprColumn.VisibleIndex = 2;
			this.isExprCheckEdit.AutoHeight = false;
			this.isExprCheckEdit.Name = "isExprCheckEdit";
			this.isExprCheckEdit.CheckedChanged += new System.EventHandler(this.isExprCheckEdit_CheckedChanged);
			this.valueColumn.Caption = "Value";
			this.valueColumn.ColumnEdit = this.valueComboBox;
			this.valueColumn.FieldName = "Value";
			this.valueColumn.Name = "valueColumn";
			this.valueColumn.Visible = true;
			this.valueColumn.VisibleIndex = 3;
			this.valueComboBox.AutoHeight = false;
			this.valueComboBox.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
			this.valueComboBox.Name = "valueComboBox";
			this.valueComboBox.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.valueComboBox_EditValueChanging);
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.gridControl);
			this.Name = "ParametersGrid";
			this.Padding = new System.Windows.Forms.Padding(2);
			this.Size = new System.Drawing.Size(511, 315);
			((System.ComponentModel.ISupportInitialize)(this.gridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gridView)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.typeComboBox)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.isExprCheckEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.valueComboBox)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraGrid.GridControl gridControl;
		private XtraGrid.Columns.GridColumn nameColumn;
		private XtraGrid.Columns.GridColumn typeColumn;
		private XtraEditors.Repository.RepositoryItemComboBox typeComboBox;
		private XtraGrid.Columns.GridColumn isExprColumn;
		private XtraGrid.Columns.GridColumn valueColumn;
		private XtraEditors.Repository.RepositoryItemComboBox valueComboBox;
		private XtraEditors.Repository.RepositoryItemCheckEdit isExprCheckEdit;
		protected XtraGrid.Views.Grid.GridView gridView;
	}
}
