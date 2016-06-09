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
using DevExpress.XtraPrinting.Native;
namespace DevExpress.DataAccess.Native.Sql.QueryBuilder {
	class FiltersPresenter : FilterPresenter<FiltersModel, IFiltersView> {
		readonly IDictionary<string, DBTable> groupDbTables;
		const int suggestedTopRecordsCount = 100;
		public FiltersPresenter(FiltersModel model, IFiltersView view, TableQuery query,
			IDictionary<string, DBTable> dbTables, Dictionary<string, DBTable> groupDbTables)
			: base(model, view, query, dbTables) {
			this.groupDbTables = groupDbTables;
			View.ChangeRecordsLimitationState += ChangeIsRecordLimitationStateHandler;
		}
		#region Overrides of PresenterBase<FilterModel,IFilterView,FilterPresenter>
		protected override void InitViewCore() {
			base.InitViewCore();
			View.TopRecords = Model.TopRecords;
			View.SkipRecords = Model.SkipRecords;
			View.RecordsLimitationEnabled = Model.TopRecords != 0 || Model.SkipRecords != 0;
			View.GroupFilterEnabled = query.Grouping.Count != 0 || !string.IsNullOrEmpty(query.GroupFilterString);
			View.SetGroupColumnsInfo(GetColumnsInfo(groupDbTables));
			View.GroupFilterString = Model.GroupFilterString;
		}
		protected override Task<bool> DoAsync(CancellationToken cancellationToken) {
			return base.DoAsync(cancellationToken).ContinueWith(
				task => {
					if(task.IsFaulted)
						throw task.Exception;
					if(task.Result) {
						Model.GroupFilterString = GroupFilterPatcher.CutOffTableNames(View.GroupFilterString, query);
						Model.SkipRecords = View.SkipRecords;
						Model.TopRecords = View.TopRecords;
						View.ChangeRecordsLimitationState -= ChangeIsRecordLimitationStateHandler;
						return true;
					}
					return false;
				}, cancellationToken);
		}
		void ChangeIsRecordLimitationStateHandler(object sender, EventArgs e) {
			View.TopEditEnabled = View.RecordsLimitationEnabled;
			if (View.TopEditEnabled && View.TopRecords == 0)
				View.TopRecords = suggestedTopRecordsCount;
			View.SkipEditEnabled = ((query.Sorting.Count != 0) && View.RecordsLimitationEnabled) || View.SkipRecords > 0;
		}
		#endregion
	}
}
