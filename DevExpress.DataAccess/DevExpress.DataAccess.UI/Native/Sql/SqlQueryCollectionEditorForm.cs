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

using System;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.UI.Sql;
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Views.Grid;
namespace DevExpress.DataAccess.UI.Native.Sql {
	public partial class SqlQueryCollectionEditorForm<TModel> : XtraForm where TModel : ISqlDataSourceModel, new() {
		readonly SqlQueryCollection queryCollection;
		readonly Action<IWizardCustomization<TModel>> customizationCallback;
		readonly EditQueryContext editQueryContext;
		public SqlQueryCollectionEditorForm() {
			InitializeComponent();
			LocalizeComponent();
		}
		public SqlQueryCollectionEditorForm(SqlQueryCollection sqlQueryCollection, EditQueryContext editQueryContext,  Action<IWizardCustomization<TModel>> callback)
			: this() {
			this.editQueryContext = editQueryContext;
			this.LookAndFeel.ParentLookAndFeel = editQueryContext.LookAndFeel;
			this.queryCollection = sqlQueryCollection;
			this.customizationCallback = callback;
			this.gridControl.DataSource = sqlQueryCollection;
			this.gridView.ValidatingEditor += gridView_ValidatingEditor;
			this.repositoryItemButtonEdit1.ButtonClick += repositoryItemButtonEdit1_ButtonClick;
			this.removeButton.Click += btnRemove_Click;
			this.btnAdd.Click += btnAdd_Click;
			this.gridView.PopupMenuShowing += GridViewPopupMenuShowing;
			this.gridView.RowCountChanged += GridViewRowCountChanged;
		}
		void LocalizeComponent() {
			btnCancel.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Cancel);
			nameColumn.Caption = DataAccessUILocalizer.GetString(DataAccessUIStringId.ParametersColumn_Name);
			btnOk.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_OK);
			removeButton.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Remove);
			btnAdd.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Add);
			Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.SqlQueryCollectionEditorForm_Title);
		}
		void gridView_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e) {
			SqlQuery query = (SqlQuery)this.gridView.GetRow(this.gridView.FocusedRowHandle);
			string oldQueryName = query.Name;
			string newQueryName = (string)e.Value;
			if(string.Equals(oldQueryName, newQueryName, StringComparison.Ordinal))
				return;
			this.queryCollection.DataSource.UpdateRelations(oldQueryName, newQueryName);
			this.queryCollection.DataSource.RenameResultSchemaPart(oldQueryName, newQueryName);
		}
		void repositoryItemButtonEdit1_ButtonClick(object sender, ButtonPressedEventArgs e) {
			if(this.gridView.PostEditor()) {
				SqlQuery query = (SqlQuery)this.gridView.GetRow(this.gridView.FocusedRowHandle);
				if(query.EditQuery(new EditQueryContext(editQueryContext) { Owner = this }, customizationCallback)) {
					this.gridView.RefreshData();
				}
			}
		}
		void GridViewRowCountChanged(object sender, EventArgs e) {
			this.removeButton.Enabled = this.gridView.RowCount > 0;
		}
		void GridViewPopupMenuShowing(object sender, PopupMenuShowingEventArgs e) {
			switch(e.MenuType) {
				case GridMenuType.Column:
				case GridMenuType.Group:
					foreach(DXMenuItem item in e.Menu.Items) {
						if(!(item.Tag is GridStringId))
							continue;
						switch((GridStringId) item.Tag) {
							case GridStringId.MenuColumnBestFit:
							case GridStringId.MenuColumnGroupBox:
							case GridStringId.MenuColumnRemoveColumn:
							case GridStringId.MenuColumnBestFitAllColumns:
							case GridStringId.MenuColumnGroup:
								item.Visible = false;
								break;
						}
					}
					break;
			}
		}
		void btnAdd_Click(object sender, EventArgs e) {
			if(this.queryCollection.DataSource.AddQuery(new EditQueryContext(editQueryContext) { Owner = this }, customizationCallback))
				this.gridView.RefreshData();
		}
		void btnRemove_Click(object sender, EventArgs e) {
			if(this.gridView.SelectedRowsCount < 1)
				return;
			SqlQuery query = (SqlQuery)this.gridView.GetRow(this.gridView.FocusedRowHandle);
			this.queryCollection.DataSource.RemoveResultSchemaPart(query.Name);
			this.gridView.DeleteRow(this.gridView.FocusedRowHandle);
		}
		public SqlQueryCollection Queries {
			get { return (SqlQueryCollection)this.gridView.DataSource; }
		}
	}
}
