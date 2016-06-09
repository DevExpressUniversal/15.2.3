#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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

namespace DevExpress.DashboardWin.Native {
	partial class ParametersEditor {
		private System.ComponentModel.IContainer components = null;
		#region Component Designer generated code
		private void InitializeComponent() {
			this.repositoryItemTextEdit = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
			this.repositoryItemCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
			this.gridColumnValue = new DevExpress.XtraGrid.Columns.GridColumn();
			this.gridColumnParameterName = new DevExpress.XtraGrid.Columns.GridColumn();
			this.parametersEditorGridControl = new DevExpress.XtraGrid.GridControl();
			this.parametersEditorGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
			this.panel2 = new System.Windows.Forms.Panel();
			this.panel1 = new System.Windows.Forms.Panel();
			this.separatorControl1 = new DevExpress.XtraEditors.SeparatorControl();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.parametersEditorGridControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.parametersEditorGridView)).BeginInit();
			this.panel2.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).BeginInit();
			this.SuspendLayout();
			this.repositoryItemTextEdit.AutoHeight = false;
			this.repositoryItemTextEdit.Name = "repositoryItemTextEdit";
			this.repositoryItemTextEdit.NullText = "NULL";
			this.repositoryItemTextEdit.ReadOnly = true;
			this.repositoryItemCheckEdit.AutoHeight = false;
			this.repositoryItemCheckEdit.Name = "repositoryItemCheckEdit";
			this.repositoryItemCheckEdit.CheckedChanged += new System.EventHandler(this.RepositoryItemCheckEditOnCheckedChanged);
			this.gridColumnValue.Caption = "Value";
			this.gridColumnValue.FieldName = "Value";
			this.gridColumnValue.Name = "gridColumnValue";
			this.gridColumnValue.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
			this.gridColumnValue.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
			this.gridColumnValue.OptionsColumn.AllowMove = false;
			this.gridColumnValue.OptionsColumn.AllowShowHide = false;
			this.gridColumnValue.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
			this.gridColumnValue.OptionsFilter.AllowAutoFilter = false;
			this.gridColumnValue.OptionsFilter.AllowFilter = false;
			this.gridColumnValue.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
			this.gridColumnValue.Visible = true;
			this.gridColumnValue.VisibleIndex = 1;
			this.gridColumnValue.Width = 139;
			this.gridColumnParameterName.AppearanceHeader.Options.UseTextOptions = true;
			this.gridColumnParameterName.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
			this.gridColumnParameterName.Caption = "Parameter Name";
			this.gridColumnParameterName.FieldName = "Name";
			this.gridColumnParameterName.Name = "gridColumnParameterName";
			this.gridColumnParameterName.OptionsColumn.AllowEdit = false;
			this.gridColumnParameterName.OptionsColumn.AllowFocus = false;
			this.gridColumnParameterName.OptionsColumn.AllowGroup = DevExpress.Utils.DefaultBoolean.False;
			this.gridColumnParameterName.OptionsColumn.AllowMerge = DevExpress.Utils.DefaultBoolean.False;
			this.gridColumnParameterName.OptionsColumn.AllowMove = false;
			this.gridColumnParameterName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
			this.gridColumnParameterName.OptionsColumn.ReadOnly = true;
			this.gridColumnParameterName.OptionsFilter.AllowAutoFilter = false;
			this.gridColumnParameterName.OptionsFilter.AllowFilter = false;
			this.gridColumnParameterName.OptionsFilter.AllowFilterModeChanging = DevExpress.Utils.DefaultBoolean.False;
			this.gridColumnParameterName.OptionsFilter.FilterBySortField = DevExpress.Utils.DefaultBoolean.False;
			this.gridColumnParameterName.Visible = true;
			this.gridColumnParameterName.VisibleIndex = 0;
			this.gridColumnParameterName.Width = 387;
			this.parametersEditorGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.parametersEditorGridControl.Location = new System.Drawing.Point(0, 0);
			this.parametersEditorGridControl.MainView = this.parametersEditorGridView;
			this.parametersEditorGridControl.Name = "parametersEditorGridControl";
			this.parametersEditorGridControl.Size = new System.Drawing.Size(526, 182);
			this.parametersEditorGridControl.TabIndex = 0;
			this.parametersEditorGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
			this.parametersEditorGridView});
			this.parametersEditorGridControl.Click += new System.EventHandler(this.parametersEditorGridControl_Click);
			this.parametersEditorGridView.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
			this.parametersEditorGridView.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[] {
			this.gridColumnParameterName,
			this.gridColumnValue});
			this.parametersEditorGridView.GridControl = this.parametersEditorGridControl;
			this.parametersEditorGridView.Name = "parametersEditorGridView";
			this.parametersEditorGridView.OptionsMenu.EnableColumnMenu = false;
			this.parametersEditorGridView.OptionsMenu.EnableFooterMenu = false;
			this.parametersEditorGridView.OptionsMenu.EnableGroupPanelMenu = false;
			this.parametersEditorGridView.OptionsView.ShowGroupPanel = false;
			this.parametersEditorGridView.OptionsView.ShowIndicator = false;
			this.panel2.Controls.Add(this.parametersEditorGridControl);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(0, 0);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(526, 182);
			this.panel2.TabIndex = 2;
			this.panel1.Controls.Add(this.separatorControl1);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.panel1.Location = new System.Drawing.Point(0, 182);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(526, 1);
			this.panel1.TabIndex = 1;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			this.separatorControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.separatorControl1.Location = new System.Drawing.Point(0, 0);
			this.separatorControl1.Name = "separatorControl1";
			this.separatorControl1.Padding = new System.Windows.Forms.Padding(0);
			this.separatorControl1.Size = new System.Drawing.Size(526, 1);
			this.separatorControl1.TabIndex = 0;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.AutoSize = true;
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.panel1);
			this.Name = "ParametersEditor";
			this.Size = new System.Drawing.Size(526, 183);
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemTextEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.repositoryItemCheckEdit)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.parametersEditorGridControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.parametersEditorGridView)).EndInit();
			this.panel2.ResumeLayout(false);
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.separatorControl1)).EndInit();
			this.ResumeLayout(false);
		}
		#endregion
		private XtraGrid.GridControl parametersEditorGridControl;
		private XtraGrid.Views.Grid.GridView parametersEditorGridView;
		private XtraGrid.Columns.GridColumn gridColumnParameterName;
		private XtraGrid.Columns.GridColumn gridColumnValue;
		private XtraGrid.Columns.GridColumn gridColumnPassNull;
		private System.Windows.Forms.Panel panel2;
		private XtraEditors.Repository.RepositoryItemTextEdit repositoryItemTextEdit;
		private XtraEditors.Repository.RepositoryItemCheckEdit repositoryItemCheckEdit;
		private System.Windows.Forms.Panel panel1;
		private XtraEditors.SeparatorControl separatorControl1;
	}
}
