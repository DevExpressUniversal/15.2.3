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
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Data;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql.QueryBuilder {
	class FilterPresenter<TModel, TView> : PresenterBase<TModel, TView, FilterPresenter<TModel, TView>>,
		IFilterPresenter
		where TView : IFilterView
		where TModel : FilterModel {
		protected readonly TableQuery query;
		readonly IDictionary<string, DBTable> dbTables;
		public FilterPresenter(TModel model, TView view, TableQuery query, IDictionary<string, DBTable> dbTables)
			: base(model, view) {
			this.query = query;
			this.dbTables = dbTables;
		}
		#region Overrides of PresenterBase<FilterModel,IFilterView,FilterPresenter>
		public IParameter CreateParameter(string name, Type type) {
			QueryParameter parameter = new QueryParameter(name, type,
				type.IsValueType ? Activator.CreateInstance(type) : null);
			Model.Parameters.Add(parameter);
			return parameter;
		}
		protected override void InitViewCore() {
			View.FilterString = Model.FilterString;
			View.SetColumnsInfo(GetColumnsInfo(dbTables));
			View.SetExistingParameters(Model.Parameters);
		}
		protected override Task<bool> DoAsync(CancellationToken cancellationToken) {
			return base.DoAsync(cancellationToken).ContinueWith(
				task => {
					if(task.IsFaulted)
						throw task.Exception;
					if(task.Result) {
						Model.FilterString = View.FilterString;
						return true;
					}
					return false;
				}, cancellationToken);
		}
		#endregion
		protected IEnumerable<FilterViewTableInfo> GetColumnsInfo(IDictionary<string, DBTable> tables) {
			return this.query.Tables
				.Where(t => t.SelectedColumns.Count > 0)
				.Select(t => new FilterViewTableInfo(t.ActualName, tables[t.ActualName]
					.Columns
					.Select(c => new FilterViewColumnInfo(c.Name, DBColumn.GetType(c.ColumnType)))));
		}
	}
}
