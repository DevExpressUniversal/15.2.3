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
using DevExpress.DataAccess.Localization;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
namespace DevExpress.DataAccess.Native.Sql.QueryBuilder {
	public class JoinEditorPresenter : PresenterBase<RelationInfoList, IJoinEditorView, JoinEditorPresenter>, IJoinEditorPresenter {
		readonly DBSchema dbSchema;
		readonly IDictionary<string, string> tableAliases;
		readonly string joiningTable;
		public JoinEditorPresenter(RelationInfoList model, IJoinEditorView view, DBSchema dbSchema, IDictionary<string, string> tableAliases, string joiningTable) : base(model, view) {
			this.dbSchema = dbSchema;
			this.tableAliases = tableAliases;
			this.joiningTable = joiningTable;
		}
		#region Overrides of PresenterBase<RelationInfoList,IJoinEditorView,JoinEditorPresenter>
		protected override Task<bool> DoAsync(CancellationToken cancellationToken) {
			return base.DoAsync(cancellationToken).ContinueWith(
				task => {
					if(task.IsFaulted)
						throw task.Exception;
					if(task.Result) {
						Model.RemoveAll(rel => string.Equals(rel.NestedTable, this.joiningTable, StringComparison.Ordinal));
						Model.AddRange(View.GetColumns().Select(keyValuePair => new RelationInfo(keyValuePair.Key, this.joiningTable, View.JoinType, keyValuePair.Value)));
						return true;
					}
					return false;
				}, cancellationToken);
		}
		protected override void InitViewCore() {
			View.JoinType = ModelJoinType;
			View.InitLookups(this.joiningTable, FindTableColumns(this.joiningTable),
				this.tableAliases.Keys.Where(t => !string.Equals(t, this.joiningTable, StringComparison.Ordinal)).ToDictionary(t => t, FindTableColumns));
			var columns = new Dictionary<string, RelationInfo.RelationColumnInfoList>();
			foreach(RelationInfo rel in Model.Where(rel => string.Equals(rel.NestedTable, this.joiningTable, StringComparison.Ordinal))) {
				RelationInfo.RelationColumnInfoList value;
				if(columns.TryGetValue(rel.ParentTable, out value))
					columns[rel.ParentTable] = new RelationInfo.RelationColumnInfoList(value.Union(rel.KeyColumns));
				else
					columns.Add(rel.ParentTable, rel.KeyColumns);
			}
			View.SetColumns(columns);
		}
		IEnumerable<string> FindTableColumns(string tableAlias) {
			return this.dbSchema.Tables.Union(this.dbSchema.Views).First(t => string.Equals(t.Name, this.tableAliases[tableAlias], StringComparison.Ordinal)).Columns.Select(c => c.Name);
		}
		protected override string Validate() {
			var columns = View.GetColumns();
			if(columns == null || columns.Count == 0 || columns.Values.Any(list => list.Any(rel => string.IsNullOrEmpty(rel.ParentKeyColumn) || string.IsNullOrEmpty(rel.NestedKeyColumn))))
				return DataAccessLocalizer.GetString(DataAccessStringId.QueryBuilderJoinEditorMissingData);
			return null;
		}
		#endregion
		JoinType ModelJoinType {
			get {
				RelationInfo relationInfo = Model.FirstOrDefault(r => string.Equals(r.NestedTable, this.joiningTable, StringComparison.Ordinal));
				if(relationInfo == null)
					return JoinType.Inner;
				return relationInfo.JoinType;
			}
		}
	}
}
