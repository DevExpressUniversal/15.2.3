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

namespace DevExpress.XtraScheduler.Design.Wizards {
	partial class WizPageSetupCustomMappings<T> {
		private System.ComponentModel.IContainer components = null;
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}
		#region Windows Form Designer generated code
		private void InitializeComponent() {
			this.lvSourceFields = new DevExpress.XtraGrid.GridControl();
			this.gvSourceFields = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colDataField = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colType = new DevExpress.XtraGrid.Columns.GridColumn();
			this.btnPromoteFieldsToCustomMapping = new DevExpress.XtraEditors.SimpleButton();
			this.btnPromoteFieldToCustomMapping = new DevExpress.XtraEditors.SimpleButton();
			this.btnRevokeCustomFields = new DevExpress.XtraEditors.SimpleButton();
			this.btnRevokeCustomField = new DevExpress.XtraEditors.SimpleButton();
			this.buttonPanel = new System.Windows.Forms.Panel();
			this.gvCustomFields = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.colCustomField = new DevExpress.XtraGrid.Columns.GridColumn();
			this.colDataFieldName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.lvCustomFields = new DevExpress.XtraGrid.GridControl();
			this.gridPanel = new System.Windows.Forms.TableLayoutPanel();
			this.btnCorrectCustomFieldName = new DevExpress.XtraEditors.CheckEdit();
			this.lblDescription = new DevExpress.XtraEditors.LabelControl();
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lvSourceFields)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.gvSourceFields)).BeginInit();
			this.buttonPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.gvCustomFields)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.lvCustomFields)).BeginInit();
			this.gridPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.btnCorrectCustomFieldName.Properties)).BeginInit();
			this.SuspendLayout();
			this.titleLabel.Text = "Custom Properties Mappings";
			this.subtitleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.subtitleLabel.Size = new System.Drawing.Size(378, 26);
			this.subtitleLabel.Text = "Select a field in the left pane and double-click it to move it to the right pane," +
	" or use the buttons. In the right pane you can specify custom field names.";
			this.headerPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.headerPicture.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.headerSeparator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.headerSeparator.Location = new System.Drawing.Point(0, 59);
			this.lvSourceFields.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lvSourceFields.Location = new System.Drawing.Point(3, 48);
			this.lvSourceFields.MainView = this.gvSourceFields;
			this.lvSourceFields.Name = "lvSourceFields";
			this.lvSourceFields.Size = new System.Drawing.Size(197, 199);
			this.lvSourceFields.TabIndex = 5;
			this.lvSourceFields.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gvSourceFields});
			this.gvSourceFields.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.colDataField,
			this.colType});
			this.gvSourceFields.GridControl = this.lvSourceFields;
			this.gvSourceFields.Name = "gvSourceFields";
			this.gvSourceFields.OptionsBehavior.Editable = false;
			this.gvSourceFields.OptionsCustomization.AllowFilter = false;
			this.gvSourceFields.OptionsCustomization.AllowGroup = false;
			this.gvSourceFields.OptionsCustomization.AllowSort = false;
			this.gvSourceFields.OptionsSelection.MultiSelect = true;
			this.gvSourceFields.OptionsView.ShowGroupPanel = false;
			this.gvSourceFields.OptionsView.ShowIndicator = false;
			this.gvSourceFields.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.colDataField, DevExpress.Data.ColumnSortOrder.Ascending)});
			this.gvSourceFields.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gvSourceFields_SelectionChanged);
			this.gvSourceFields.DoubleClick += new System.EventHandler(this.gvSourceFields_DoubleClick);
			this.colDataField.Caption = "Data Field";
			this.colDataField.FieldName = "Name";
			this.colDataField.Name = "colDataField";
			this.colDataField.Visible = true;
			this.colDataField.VisibleIndex = 0;
			this.colDataField.Width = 100;
			this.colType.Caption = "Type";
			this.colType.FieldName = "TypeName";
			this.colType.Name = "colType";
			this.colType.Visible = true;
			this.colType.VisibleIndex = 1;
			this.colType.Width = 72;
			this.btnPromoteFieldsToCustomMapping.Location = new System.Drawing.Point(3, 67);
			this.btnPromoteFieldsToCustomMapping.Name = "btnPromoteFieldsToCustomMapping";
			this.btnPromoteFieldsToCustomMapping.Size = new System.Drawing.Size(51, 23);
			this.btnPromoteFieldsToCustomMapping.TabIndex = 6;
			this.btnPromoteFieldsToCustomMapping.Text = ">>";
			this.btnPromoteFieldsToCustomMapping.Click += new System.EventHandler(this.btnPromoteFieldsToCustomMapping_Click);
			this.btnPromoteFieldToCustomMapping.Location = new System.Drawing.Point(3, 38);
			this.btnPromoteFieldToCustomMapping.Name = "btnPromoteFieldToCustomMapping";
			this.btnPromoteFieldToCustomMapping.Size = new System.Drawing.Size(51, 23);
			this.btnPromoteFieldToCustomMapping.TabIndex = 6;
			this.btnPromoteFieldToCustomMapping.Text = ">";
			this.btnPromoteFieldToCustomMapping.Click += new System.EventHandler(this.btnPromoteFieldToCustomMapping_Click);
			this.btnRevokeCustomFields.Location = new System.Drawing.Point(3, 139);
			this.btnRevokeCustomFields.Name = "btnRevokeCustomFields";
			this.btnRevokeCustomFields.Size = new System.Drawing.Size(51, 23);
			this.btnRevokeCustomFields.TabIndex = 6;
			this.btnRevokeCustomFields.Text = "<<";
			this.btnRevokeCustomFields.Click += new System.EventHandler(this.btnRevokeCustomFields_Click);
			this.btnRevokeCustomField.Location = new System.Drawing.Point(3, 110);
			this.btnRevokeCustomField.Name = "btnRevokeCustomField";
			this.btnRevokeCustomField.Size = new System.Drawing.Size(51, 23);
			this.btnRevokeCustomField.TabIndex = 6;
			this.btnRevokeCustomField.Text = "<";
			this.btnRevokeCustomField.Click += new System.EventHandler(this.btnRevokeCustomField_Click);
			this.buttonPanel.Controls.Add(this.btnRevokeCustomFields);
			this.buttonPanel.Controls.Add(this.btnRevokeCustomField);
			this.buttonPanel.Controls.Add(this.btnPromoteFieldsToCustomMapping);
			this.buttonPanel.Controls.Add(this.btnPromoteFieldToCustomMapping);
			this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonPanel.Location = new System.Drawing.Point(206, 48);
			this.buttonPanel.Name = "buttonPanel";
			this.buttonPanel.Size = new System.Drawing.Size(61, 199);
			this.buttonPanel.TabIndex = 6;
			this.gvCustomFields.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.colCustomField,
			this.colDataFieldName});
			this.gvCustomFields.GridControl = this.lvCustomFields;
			this.gvCustomFields.Name = "gvCustomFields";
			this.gvCustomFields.OptionsCustomization.AllowFilter = false;
			this.gvCustomFields.OptionsCustomization.AllowGroup = false;
			this.gvCustomFields.OptionsCustomization.AllowSort = false;
			this.gvCustomFields.OptionsSelection.MultiSelect = true;
			this.gvCustomFields.OptionsView.ShowGroupPanel = false;
			this.gvCustomFields.OptionsView.ShowIndicator = false;
			this.gvCustomFields.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[] {
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.colCustomField, DevExpress.Data.ColumnSortOrder.Ascending)});
			this.gvCustomFields.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gvCustomFields_SelectionChanged);
			this.gvCustomFields.DoubleClick += new System.EventHandler(this.gvCustomFields_DoubleClick);
			this.colCustomField.Caption = "Custom Field";
			this.colCustomField.FieldName = "Name";
			this.colCustomField.Name = "colCustomField";
			this.colCustomField.Visible = true;
			this.colCustomField.VisibleIndex = 0;
			this.colCustomField.Width = 100;
			this.colDataFieldName.Caption = "Data Field";
			this.colDataFieldName.FieldName = "Member";
			this.colDataFieldName.Name = "colDataFieldName";
			this.colDataFieldName.OptionsColumn.AllowEdit = false;
			this.colDataFieldName.OptionsColumn.ReadOnly = true;
			this.colDataFieldName.Visible = true;
			this.colDataFieldName.VisibleIndex = 1;
			this.colDataFieldName.Width = 72;
			this.lvCustomFields.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.lvCustomFields.Location = new System.Drawing.Point(273, 48);
			this.lvCustomFields.MainView = this.gvCustomFields;
			this.lvCustomFields.Name = "lvCustomFields";
			this.lvCustomFields.Size = new System.Drawing.Size(198, 199);
			this.lvCustomFields.TabIndex = 5;
			this.lvCustomFields.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.gvCustomFields});
			this.gridPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.gridPanel.ColumnCount = 3;
			this.gridPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.gridPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 67F));
			this.gridPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.gridPanel.Controls.Add(this.lvCustomFields, 2, 2);
			this.gridPanel.Controls.Add(this.lvSourceFields, 0, 2);
			this.gridPanel.Controls.Add(this.buttonPanel, 1, 2);
			this.gridPanel.Controls.Add(this.btnCorrectCustomFieldName, 0, 0);
			this.gridPanel.Controls.Add(this.lblDescription, 0, 1);
			this.gridPanel.Location = new System.Drawing.Point(3, 60);
			this.gridPanel.Name = "gridPanel";
			this.gridPanel.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this.gridPanel.RowCount = 3;
			this.gridPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
			this.gridPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
			this.gridPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.gridPanel.Size = new System.Drawing.Size(494, 250);
			this.gridPanel.TabIndex = 8;
			this.btnCorrectCustomFieldName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.btnCorrectCustomFieldName.EditValue = true;
			this.btnCorrectCustomFieldName.Location = new System.Drawing.Point(3, 3);
			this.btnCorrectCustomFieldName.Name = "btnCorrectCustomFieldName";
			this.btnCorrectCustomFieldName.Properties.Caption = "Correct custom field name.";
			this.btnCorrectCustomFieldName.Size = new System.Drawing.Size(197, 19);
			this.btnCorrectCustomFieldName.TabIndex = 7;
			this.btnCorrectCustomFieldName.CheckedChanged += new System.EventHandler(this.OnBtnCorrectCustomFieldNameCheckedChanged);
			this.lblDescription.Appearance.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.gridPanel.SetColumnSpan(this.lblDescription, 3);
			this.lblDescription.Location = new System.Drawing.Point(3, 28);
			this.lblDescription.Name = "lblDescription";
			this.lblDescription.Size = new System.Drawing.Size(53, 13);
			this.lblDescription.TabIndex = 9;
			this.lblDescription.Text = "Description";
			this.Controls.Add(this.gridPanel);
			this.Margin = new System.Windows.Forms.Padding(0);
			this.Name = "WizPageSetupCustomMappings";
			this.Padding = new System.Windows.Forms.Padding(0, 0, 20, 0);
			this.Controls.SetChildIndex(this.headerPanel, 0);
			this.Controls.SetChildIndex(this.headerSeparator, 0);
			this.Controls.SetChildIndex(this.titleLabel, 0);
			this.Controls.SetChildIndex(this.subtitleLabel, 0);
			this.Controls.SetChildIndex(this.headerPicture, 0);
			this.Controls.SetChildIndex(this.gridPanel, 0);
			((System.ComponentModel.ISupportInitialize)(this.headerPicture)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lvSourceFields)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.gvSourceFields)).EndInit();
			this.buttonPanel.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.gvCustomFields)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.lvCustomFields)).EndInit();
			this.gridPanel.ResumeLayout(false);
			this.gridPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.btnCorrectCustomFieldName.Properties)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private DevExpress.XtraGrid.GridControl lvSourceFields;
		private DevExpress.XtraGrid.Views.Grid.GridView gvSourceFields;
		private DevExpress.XtraEditors.SimpleButton btnPromoteFieldsToCustomMapping;
		private DevExpress.XtraEditors.SimpleButton btnPromoteFieldToCustomMapping;
		private DevExpress.XtraEditors.SimpleButton btnRevokeCustomFields;
		private DevExpress.XtraEditors.SimpleButton btnRevokeCustomField;
		private DevExpress.XtraGrid.Columns.GridColumn colDataField;
		private DevExpress.XtraGrid.Columns.GridColumn colType;
		private System.Windows.Forms.Panel buttonPanel;
		private XtraGrid.Views.Grid.GridView gvCustomFields;
		private XtraGrid.Columns.GridColumn colCustomField;
		private XtraGrid.Columns.GridColumn colDataFieldName;
		private XtraGrid.GridControl lvCustomFields;
		private System.Windows.Forms.TableLayoutPanel gridPanel;
		private XtraEditors.CheckEdit btnCorrectCustomFieldName;
		private XtraEditors.LabelControl lblDescription;
	}
}
