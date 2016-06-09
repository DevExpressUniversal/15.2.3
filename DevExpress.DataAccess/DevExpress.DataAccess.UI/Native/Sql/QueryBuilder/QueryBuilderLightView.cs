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
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Native.Sql.QueryBuilder;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.LookAndFeel;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList;
namespace DevExpress.DataAccess.UI.Native.Sql.QueryBuilder {
	public class QueryBuilderLightView : QueryBuilderNoSqlView {
		readonly IDisplayNameProvider displayNameProvider;
		public QueryBuilderLightView(IQueryBuilderViewModel viewModel, IWin32Window owner, UserLookAndFeel lookAndFeel, IDisplayNameProvider displayNameProvider, IParameterService parameterService, IServiceProvider propertyGridServices)
			: base(viewModel, owner, lookAndFeel, parameterService, propertyGridServices) {
			this.displayNameProvider = displayNameProvider ?? new DefaultDisplayNameProvider();
			layoutItemReSqlPanel.HideToCustomization();
			splitterItem1.HideToCustomization();
			colAlias.Visible = false;
			gridViewTables.CustomColumnDisplayText += gridViewTables_CustomColumnDisplayText;
			gridViewColumns.CustomColumnDisplayText += gridViewColumns_CustomColumnDisplayText;
			tlSelection.GetNodeDisplayValue += tlSelection_GetNodeDisplayValue;
			gridViewQuery.CustomColumnDisplayText += gridViewQuery_CustomColumnDisplayText;
			repositoryItemButtonEditEditJoin.CustomDisplayText += repositoryItemButtonEditEditJoin_CustomDisplayText;
		} 
		#region Overrides of QueryBuilderView
		protected override string GetColumnsListCaption(string name) {
			try { return base.GetColumnsListCaption(displayNameProvider.GetFieldDisplayName(new[] { name })); }
			catch { return base.GetColumnsListCaption(name); }
		}
		protected override JoinEditorView CreateJoinEditorView() { return new JoinEditorView(this, LookAndFeel, displayNameProvider); }
		protected override FiltersView CreateFilterView() { return new FiltersView(this, LookAndFeel, null, null, null, parameterService, null); }
		protected override IEnumerable<ColumnLookUpDataItem> ColumnLookUpData() {
			return
				base.ColumnLookUpData()
					.Select(item => new AliasedColumnLookUpDataItem(item.Table, item.Column, displayNameProvider));
		}
		#endregion
		void gridViewTables_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e) {
			if(e.Column == gridColName)
				if(e.Value != null)
					e.DisplayText = GetTableDisplayName(e.Value.ToString());
		}
		void gridViewColumns_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e) {
			if(e.Column == colName)
				if(gridViewTables.FocusedRowHandle >= 0 && e.Value != null)
					e.DisplayText =
						GetColumnDisplayName(
							gridViewTables.GetRowCellValue(gridViewTables.FocusedRowHandle, gridColName).ToString(),
							e.Value.ToString());
		}
		void tlSelection_GetNodeDisplayValue(object sender, GetNodeDisplayValueEventArgs e) {
			if(e.Column == colName1)
				if(e.Value != null)
					e.Value = e.Node.ParentNode == null
						? GetTableDisplayName(e.Value.ToString())
						: GetColumnDisplayName(e.Node.ParentNode.GetValue(colName1).ToString(),
							e.Value.ToString());
			if(e.Column == colCondition)
				e.Value = GetConditionStringDisplayText(e.Value as ConditionStringInfo);
		}
		void gridViewQuery_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e) {
			if(e.Column == colTable)
				if(e.Value != null)
					e.DisplayText = GetTableDisplayName(e.Value.ToString());
			if(e.Column == colColumn)
				if(e.ListSourceRowIndex >= 0)
					e.DisplayText =
						GetColumnDisplayName(
							((QueryGridItemData.List)gridControlQuery.DataSource)[e.ListSourceRowIndex].Table,
							e.Value.ToString());
		}
		void repositoryItemButtonEditEditJoin_CustomDisplayText(object sender, CustomDisplayTextEventArgs e) {
			ConditionStringInfo info = e.Value as ConditionStringInfo;
			if(info != null)
				e.DisplayText = GetConditionStringDisplayText(info);
		}
		string GetConditionStringDisplayText(ConditionStringInfo info) {
			if(info == null)
				return string.Empty;
			if(displayNameProvider == null)
				return info.ToString();
			string formatString = DataAccessLocalizer.GetString(info.Format);
			var columns = string.Join(
				DataAccessLocalizer.GetString(DataAccessStringId.QueryDesignerJoinExpressionElementSeparator),
				info.Columns.Select(
					columnInfo =>
						string.Format("[{0}].[{1}]", GetTableDisplayName(columnInfo.Table),
							GetColumnDisplayName(columnInfo.Table, columnInfo.Column))));
			return info.JoinType == null
				? string.Format(formatString, columns)
				: string.Format(formatString, info.JoinType, columns);
		}
		protected override string GetTableDisplayName(string table) {
			if(displayNameProvider == null)
				return table;
			try {
				string displayName = displayNameProvider.GetFieldDisplayName(new[] { table });
				return string.IsNullOrEmpty(displayName) ? table : displayName;
			}
			catch {
				return table;
			}
		}
		protected override string GetColumnDisplayName(string table, string column) {
			if(displayNameProvider == null)
				return column;
			try {
				string displayName = displayNameProvider.GetFieldDisplayName(new[] { table, column });
				return string.IsNullOrEmpty(displayName) ? column : displayName;
			}
			catch(Exception) {
				return column;
			}
		}
	}
	class DefaultDisplayNameProvider : IDisplayNameProvider {
		#region Implementation of IDisplayNameProvider
		public string GetFieldDisplayName(string[] fieldAccessors) {
			if(fieldAccessors.Length == 1)
				return fieldAccessors[0];
			if(fieldAccessors.Length == 2)
				return fieldAccessors[1];
			throw new ArgumentException();
		}
		public string GetDataSourceDisplayName() { throw new NotSupportedException(); }
		#endregion
	}
	public class AliasedColumnLookUpDataItem : ColumnLookUpDataItem {
		public AliasedColumnLookUpDataItem(string table, string column, IDisplayNameProvider displayNameProvider)
			: base(table, column) {
			try {
				TableDisplayName = displayNameProvider.GetFieldDisplayName(new[] { table });
				if(string.IsNullOrEmpty(TableDisplayName))
					TableDisplayName = table;
			}
			catch { TableDisplayName = table; }
			try {
				ColumnDisplayName = displayNameProvider.GetFieldDisplayName(new[] { table, column });
				if(string.IsNullOrEmpty(ColumnDisplayName))
					ColumnDisplayName = column;
			}
			catch { ColumnDisplayName = column; }
		}
	}
}
