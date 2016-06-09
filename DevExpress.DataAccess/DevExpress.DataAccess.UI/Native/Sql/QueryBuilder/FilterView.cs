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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.UI.Localization;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.Utils.UI;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Filtering;
using DevExpress.XtraEditors.Repository;
namespace DevExpress.DataAccess.UI.Native.Sql.QueryBuilder {
	public partial class FilterView : XtraForm, IFilterView {
		internal class TableFilterColumn : FilterColumn {
			readonly string tableName;
			readonly string tableDisplayName;
			readonly List<IBoundProperty> children;
			public TableFilterColumn(FilterViewTableInfo info, IDisplayNameProvider displayNameProvider) {
				this.tableName = info.TableName;
				if(displayNameProvider != null)
					try {
						string displayName = displayNameProvider.GetFieldDisplayName(new[] { info.TableName });
						if(!string.IsNullOrEmpty(displayName))
							tableDisplayName = displayName;
					}
					catch {
						tableDisplayName = null;
					}
				this.children = info.Columns.Select(c => new ColumnFilterColumn(this, c, displayNameProvider)).ToList<IBoundProperty>();
			}
			#region Overrides of FilterColumn
			public override string ColumnCaption { get { return tableDisplayName ?? tableName; } }
			public override string FieldName { get { return this.tableName; } }
			public override Type ColumnType { get { return typeof(object); } }
			public override RepositoryItem ColumnEditor { get { return null; } }
			public override Image Image { get { return null; } }
			public override bool IsAggregate { get { return true; } }
			public override bool HasChildren { get { return true; } }
			public override List<IBoundProperty> Children { get { return this.children; } }
			#endregion
		}
		internal class ColumnFilterColumn : FilterColumn {
			IBoundProperty parent;
			readonly string columnName;
			readonly string columnDisplayName;
			readonly Type columnType;
			public ColumnFilterColumn(TableFilterColumn parent, FilterViewColumnInfo info, IDisplayNameProvider displayNameProvider) {
				this.parent = parent;
				this.columnName = info.ColumnName;
				this.columnType = info.ColumnType;
				if(displayNameProvider != null)
					try {
						string displayName =
							displayNameProvider.GetFieldDisplayName(new[] { parent.FieldName, info.ColumnName });
						columnDisplayName = !string.IsNullOrEmpty(displayName) ? displayName : null;
					}
					catch {
						columnDisplayName = null;
					}
				else
					columnDisplayName = null;
			}
			#region Overrides of FilterColumn
			public override string ColumnCaption { get { return columnDisplayName ?? columnName; } }
			public override string FieldName { get { return this.columnName; } }
			public override Type ColumnType { get { return this.columnType; } }
			public override RepositoryItem ColumnEditor { get { return ColumnType == typeof(DateTime) ? new RepositoryItemDateEdit() : new RepositoryItemTextEdit(); } }
			public override Image Image { get { return null; } }
			public override IBoundProperty Parent { get { return this.parent; } set { this.parent = value; } }
			#endregion
		}
		internal class QueryBuilderFilterColumnCollection : FilterColumnCollection {
			public QueryBuilderFilterColumnCollection(IEnumerable<FilterViewTableInfo> info, IDisplayNameProvider displayNameProvider) {
				foreach(FilterViewTableInfo tableInfo in info)
					Add(new TableFilterColumn(tableInfo, displayNameProvider));
			}
			protected override string GetParentFieldName(ref string name) {
				foreach(TableFilterColumn table in this)
					foreach(ColumnFilterColumn column in table.Children.Cast<ColumnFilterColumn>()) {
						if(column.GetFullName() != name)
							continue;
						name = column.FieldName;
						return table.FieldName;
					}
				return null;
			}
		}
		internal class QueryBuilderParametersCreator : IParameterCreator {
			readonly IFilterPresenter presenter;
			public QueryBuilderParametersCreator(IFilterPresenter presenter) {
				this.presenter = presenter;
			}
			public IParameter CreateParameter(string name, Type type) {
				return this.presenter.CreateParameter(name, type);
			}
		}
		internal static FilterColumn FindFirstSelectableNode(IEnumerable<FilterColumn> columns) {
			if(columns == null)
				return null;
			foreach(FilterColumn column in columns) {
				if(!column.HasChildren)
					return column;
				FilterColumn child = FindFirstSelectableNode(column.Children.Cast<FilterColumn>());
				if(child != null)
					return child;
			}
			return null;
		}
		IWin32Window owner;
		protected IWin32Window ParentWindow { get { return owner; } }
		IDisplayNameProvider displayNameProvider;
		protected IDisplayNameProvider DisplayNameProvider { get { return displayNameProvider; } }
		IFilterPresenter presenter;
		protected IFilterPresenter Presenter { get { return presenter; } }
		public FilterView(IWin32Window owner, UserLookAndFeel lookAndFeel) 
			: this(owner, lookAndFeel, null, null, null) { }
		public FilterView(IWin32Window owner, UserLookAndFeel lookAndFeel, IDisplayNameProvider displayNameProvider) 
			: this(owner, lookAndFeel, displayNameProvider, null) { }
		public FilterView(IWin32Window owner, UserLookAndFeel lookAndFeel, IDisplayNameProvider displayNameProvider, FilterControl filterControl) 
			: this(owner, lookAndFeel, displayNameProvider, filterControl, null) {}
		public FilterView(IWin32Window owner, UserLookAndFeel lookAndFeel, IDisplayNameProvider displayNameProvider, FilterControl filterControl, IParameterService parameterService)
			: this(owner, lookAndFeel, displayNameProvider, filterControl, parameterService, null) {}
		public FilterView(IWin32Window owner, UserLookAndFeel lookAndFeel, IDisplayNameProvider displayNameProvider, FilterControl filterControl, IParameterService parameterService, IServiceProvider propertyGridServices) {
			this.owner = owner;
			this.displayNameProvider = displayNameProvider;
			this.filterControl = filterControl ?? new QueryFilterControl(parameterService, propertyGridServices);
			LookAndFeel.ParentLookAndFeel = lookAndFeel;
			InitializeComponent();
			LocalizeComponent();
			this.filterControl.AllowCreateDefaultClause = false;
			this.filterControl.MenuManager = barManager;
		}
		protected FilterView() : this(null, null) {}
		#region Implementation of IView<in IFilterPresenter>
		public void BeginUpdate() {
			SuspendLayout();
		}
		public void EndUpdate() {
			ResumeLayout();
		}
		public void RegisterPresenter(IFilterPresenter presenter) {
			this.presenter = presenter;
		}
		public void Start() {
			ShowDialog(this.owner);
		}
		public void Stop() {
			Close();
		}
		public void Warning(string message) {
			XtraMessageBox.Show(LookAndFeel, this, message,
				DataAccessUILocalizer.GetString(DataAccessUIStringId.MessageBoxWarningTitle), MessageBoxButtons.OK,
				MessageBoxIcon.Warning);
		}
		public event EventHandler Ok;
		public event EventHandler Cancel;
		#endregion
		#region Implementation of IFilterView
		public string FilterString { get { return this.filterControl.FilterString; } set { this.filterControl.FilterString = value; } }
		public void SetColumnsInfo(IEnumerable<FilterViewTableInfo> info) {
			this.filterControl.SetFilterColumnsCollection(new QueryBuilderFilterColumnCollection(info, displayNameProvider));
			this.filterControl.SetDefaultColumn(FindFirstSelectableNode(this.filterControl.FilterColumns.Cast<FilterColumn>()));
		}
		public virtual void SetExistingParameters(IEnumerable<IParameter> parameters) {
			this.filterControl.SourceControl = new FilterFormParametersOwner(new QueryBuilderParametersCreator(this.presenter), parameters.ToList());
		}
		#endregion
		protected void RaiseOk() {
			if(Ok != null)
				Ok(this, EventArgs.Empty);
		}
		protected void RaiseCancel() {
			if(Cancel != null)
				Cancel(this, EventArgs.Empty);
		}
		void LocalizeComponent() {
			buttonCancel.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_Cancel);
			buttonOk.Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.Button_OK);
			Text = DataAccessUILocalizer.GetString(DataAccessUIStringId.FiltersView);
		}
		void btnOk_Click(object sender, EventArgs e) {
			RaiseOk();
		}
		void btnCancel_Click(object sender, EventArgs e) {
			RaiseCancel();
		}
		void FilterView_FormClosing(object sender, FormClosingEventArgs e) {
			RaiseCancel();
		}
	}
}
