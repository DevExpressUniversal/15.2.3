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
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.WizardFramework;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.Data.XtraReports.Wizard.Views;
using DevExpress.Utils;
namespace DevExpress.Data.XtraReports.Wizard.Presenters {
	public class AddGroupingLevelPage<TModel> : WizardPageBase<IAddGroupingLevelPageView, TModel> where TModel : ReportModel {
		readonly IColumnInfoCache columnInfoCache;
		readonly List<ColumnInfo> availableColumns = new List<ColumnInfo>();
		readonly List<GroupingLevelInfo> groupingLevels = new List<GroupingLevelInfo>();
		string[] summaryColumnNames = new string[] { };
		public override bool FinishEnabled { get { return true; } }
		public override bool MoveNextEnabled { get { return true; } }
		public AddGroupingLevelPage(IAddGroupingLevelPageView view, IColumnInfoCache columnInfoCache) : base(view) {
			Guard.ArgumentNotNull(columnInfoCache, "columnInfoCache");
			this.columnInfoCache = columnInfoCache;
			view.ActiveAvailableColumnsChanged += view_ActiveAvailableColumnsChanged;
			view.ActiveGroupingLevelChanged += view_ActiveGroupingLevelChanged;
			view.AddGroupingLevelClicked += view_AddGroupingLevelClicked;
			view.RemoveGroupingLevelClicked += view_RemoveGroupingLevelClicked;
			view.CombineGroupingLevelClicked += view_CombineGroupingLevelClicked;
			view.GroupingLevelUpClicked += view_GroupingLevelUpClicked;
			view.GroupingLevelDownClicked += view_GroupingLevelDownClicked;
		}
		void view_ActiveGroupingLevelChanged(object sender, EventArgs e) {
			RefreshButtons();
		}
		void view_ActiveAvailableColumnsChanged(object sender, EventArgs e) {
			RefreshButtons();
		}
		void view_CombineGroupingLevelClicked(object sender, EventArgs e) {
			ColumnInfo[] activeColumns = View.GetActiveAvailableColumns();
			for(int i = 0; i < activeColumns.Length; i++) {
				availableColumns.Remove(activeColumns[i]);
			}
			GroupingLevelInfo activeGrouping = View.GetActiveGroupingLevel();
			GroupingLevelInfo newGrouping = new GroupingLevelInfo(activeColumns.Union(activeGrouping.Columns).ToArray());
			int index = groupingLevels.IndexOf(activeGrouping);
			groupingLevels[index] = newGrouping;
			UpdateSummaryColumnNames();
			RefreshView();
			View.SetActiveGroupingLevel(newGrouping);
		}
		void view_AddGroupingLevelClicked(object sender, EventArgs e) {
			ColumnInfo[] activeColumns = View.GetActiveAvailableColumns();
			for(int i = 0; i < activeColumns.Length; i++) {
				availableColumns.Remove(activeColumns[i]);
				groupingLevels.Add(new GroupingLevelInfo(new ColumnInfo[] { activeColumns[i] }));
			}
			UpdateSummaryColumnNames();
			RefreshView();
		}
		void view_RemoveGroupingLevelClicked(object sender, EventArgs e) {
			GroupingLevelInfo grouping = View.GetActiveGroupingLevel();
			groupingLevels.Remove(grouping);
			availableColumns.AddRange(grouping.Columns);
			UpdateSummaryColumnNames();
			RefreshView();
		}
		void view_GroupingLevelUpClicked(object sender, EventArgs e) {
			MoveActiveGroupingLevel(false);
		}
		void view_GroupingLevelDownClicked(object sender, EventArgs e) {
			MoveActiveGroupingLevel(true);
		}
		void MoveActiveGroupingLevel(bool moveDown) {
			var groupingLevel = View.GetActiveGroupingLevel();
			var groupingLevelIndex = groupingLevels.LastIndexOf(groupingLevel);
			int indexDelta = moveDown ? +1 : -1;
			groupingLevels[groupingLevelIndex] = groupingLevels[groupingLevelIndex + indexDelta];
			groupingLevels[groupingLevelIndex + indexDelta] = groupingLevel;
			RefreshView();
			View.SetActiveGroupingLevel(groupingLevel);
			RefreshButtons();
		}
		private void RefreshButtons() {
			View.EnableAddGroupingLevelButton(View.GetActiveAvailableColumns().Length > 0);
			View.EnableRemoveGroupingLevelButton(View.GetActiveGroupingLevel() != null);
			View.EnableCombineGroupingLevelButton(View.GetActiveAvailableColumns().Length > 0 && View.GetActiveGroupingLevel() != null);
			var activeGroupingLevel = View.GetActiveGroupingLevel();
			View.EnableGroupingLevelUp(activeGroupingLevel != null && groupingLevels.IndexOf(activeGroupingLevel) > 0);
			View.EnableGroupingLevelDown(activeGroupingLevel != null && groupingLevels.IndexOf(activeGroupingLevel) < groupingLevels.Count - 1);
		}
		private void FillViewControls() {
			View.FillAvailableColumns(availableColumns.ToArray());
			View.FillGroupingLevels(groupingLevels.ToArray());
		}
		public override Type GetNextPageType() {
			return summaryColumnNames.Length > 0 ? typeof(ChooseSummaryOptionsPage<TModel>) : typeof(ChooseReportLayoutPage<TModel>);
		}
		public override void Begin() {
			availableColumns.Clear();
			groupingLevels.Clear();
			availableColumns.AddRange(columnInfoCache.Columns);
			if(Model.GroupingLevels != null) {
				for(int i = 0; i < Model.GroupingLevels.Length; i++) {
					var groupingLevel = Model.GroupingLevels[i];
					var groupingColumns = availableColumns.Where(x => groupingLevel.Contains(x.Name)).ToArray();
					var groupingLevelInfo = new GroupingLevelInfo(groupingColumns);
					groupingLevels.Add(groupingLevelInfo);
				}
				foreach(var groupingLevel in groupingLevels) {
					foreach(var column in groupingLevel.Columns) {
						availableColumns.Remove(column);
					}
				}
			}
			summaryColumnNames = Model.SummaryOptions.Select(x => x.ColumnName).ToArray();
			RefreshView();
		}
		public override void Commit() {
			HashSet<string>[] result = new HashSet<string>[groupingLevels.Count];
			for(int i = 0; i < groupingLevels.Count; i++) {
				result[i] = new HashSet<string>(groupingLevels[i].Columns.Select(c => c.Name));
			}
			Model.GroupingLevels = result;
			Model.SummaryOptions = new HashSet<ColumnNameSummaryOptions>(summaryColumnNames.Select(c => new ColumnNameSummaryOptions(c)));
			Model.Layout = Model.IsGrouped() ? ReportLayout.Stepped : ReportLayout.Columnar;
		}
		void UpdateSummaryColumnNames() {
			if(groupingLevels.Count == 0) {
				summaryColumnNames = new string[] { };
				return;
			}
			var result = from c in Model.Columns
						 where
							groupingLevels.All(level => !level.ContainsColumn(c)) &&
							CanCreateSummaryForType(availableColumns.Find(x => x.Name == c).TypeSpecifics)
							select c;
			summaryColumnNames = result.ToArray();
		}
		bool CanCreateSummaryForType(TypeSpecifics typeSpecifics) {
			return
				typeSpecifics == TypeSpecifics.Date ||
				typeSpecifics == TypeSpecifics.Integer ||
				typeSpecifics == TypeSpecifics.Float;
		}
		private void RefreshView() {
			FillViewControls();
			RefreshButtons();
			RaiseChanged();
		}
	}
}
