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

using System.Collections.Generic;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.POCO;
using DevExpress.Mvvm.Native;
using DevExpress.Data.XtraReports.DataProviders;
using System.Collections.ObjectModel;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Data.Browsing.Design;
using System.Collections.Specialized;
namespace DevExpress.Xpf.Reports.UserDesigner.ReportWizard.Pages {
	public class AddGroupingLevelPage : ReportWizardPageBase {
		public static AddGroupingLevelPage Create(ReportWizardModel reportWizardModel, ColumnInfoCache columnsCache) {
			return ViewModelSource.Create(() => new AddGroupingLevelPage(reportWizardModel, columnsCache));
		}
		readonly ColumnInfoCache columnsCache;
		readonly ObservableCollection<ColumnInfo> availableColumns = new ObservableCollection<ColumnInfo>();
		readonly GroupsCollection groups = new GroupsCollection();
		public ObservableCollection<ColumnInfo> AvailableColumns {
			get { return availableColumns; }
		}
		public GroupsCollection Groups {
			get { return groups; }
		}
		public virtual ColumnInfo SelectedColumn { get; set; }
		public virtual GroupingLevelInfo SelectedGroup { get; set; }
		#region ctor & initialization
		protected AddGroupingLevelPage(ReportWizardModel model, ColumnInfoCache columnsCache) : base(model) {
			this.columnsCache = columnsCache;
			FillData();
		}
		#endregion
		public void AddLevel() {
			var column = SelectedColumn;
			var columnIndex = AvailableColumns.IndexOf(SelectedColumn);
			AvailableColumns.Remove(column);
			SelectedColumn = AvailableColumns.IsValidIndex(columnIndex) ? AvailableColumns[columnIndex] : AvailableColumns.FirstOrDefault();
			var insertGroupIndex = SelectedGroup != null ? Groups.IndexOf(SelectedGroup) + 1 : 0;
			Groups.Insert(insertGroupIndex, new GroupingLevelInfo(new[] { column }));
			SelectedGroup = Groups[insertGroupIndex];
			UpdateModel();
		}
		public bool CanAddLevel() {
			if (SelectedColumn == null )
				return false;
			return AvailableColumns.Contains(SelectedColumn) && !Groups.Any(g => g.Columns.Contains(SelectedColumn));
		}
		public void RemoveLevel() {
			var columns = SelectedGroup.Columns;
			var index = Groups.IndexOf(SelectedGroup);
			Groups.Remove(SelectedGroup);
			SelectedGroup = Groups.IsValidIndex(index - 1) ? Groups[index - 1] : Groups.FirstOrDefault();
			columns.ForEach(x => AvailableColumns.Add(x));
			UpdateModel();
		}
		public bool CanRemoveLevel() {
			if (SelectedGroup == null)
				return false;
			return Groups.Contains(SelectedGroup) && !SelectedGroup.Columns.Any(x => AvailableColumns.Contains(x));
		}
		public void AddLevelGroup() {
			var column = SelectedColumn;
			AvailableColumns.Remove(column);
			var columnIndex = AvailableColumns.IndexOf(SelectedColumn);
			SelectedColumn = AvailableColumns.IsValidIndex(columnIndex) ? AvailableColumns[columnIndex] : AvailableColumns.FirstOrDefault();
			var groupIndex = Groups.IndexOf(SelectedGroup);
			var groupColumns = Groups[groupIndex].Columns.Concat(new[] { column }).ToArray();
			Groups[groupIndex] = new GroupingLevelInfo(groupColumns);
			SelectedGroup = Groups[groupIndex];
			UpdateModel();
		}
		public bool CanAddLevelGroup() {
			return SelectedGroup != null && CanAddLevel();
		}
		public void MoveUp() {
			var index = Groups.IndexOf(SelectedGroup);
			Groups.Move(index, index - 1);
			UpdateModel();
		}
		public bool CanMoveUp() {
			return SelectedGroup != null && Groups.FirstOrDefault() != SelectedGroup;
		}
		public void MoveDown() {
			var index = Groups.IndexOf(SelectedGroup);
			Groups.Move(index, index + 1);
			UpdateModel();
		}
		public bool CanMoveDown() {
			return SelectedGroup != null && Groups.LastOrDefault() != SelectedGroup;
		}
		void FillData() {
			var groupingLevels = ReportModel.GroupingLevels ?? new HashSet<string>[0];
			List<string> groupedColumns = new List<string>();
			foreach (var level in groupingLevels) {
				groupedColumns.AddRange(level);
				var groupColumns = columnsCache.Columns.Where(x => level.Contains(x.Name)).ToArray();
				Groups.Add(new GroupingLevelInfo(groupColumns));
			}
			columnsCache.Columns
				.Where(x => !groupedColumns.Contains(x.Name))
				.ForEach(x => AvailableColumns.Add(x));
		}
		void UpdateModel() {
			ReportModel.GroupingLevels = Groups.Select(g =>
				new HashSet<string>(g.Columns.Select(c => c.Name))).ToArray();
			ReportModel.SummaryOptions = new HashSet<ColumnNameSummaryOptions>(AvailableColumns
				.Where(x => CanCreateSummaryForType(x.TypeSpecifics))
				.Select(x => new ColumnNameSummaryOptions(x.Name)));
			ReportModel.Layout = ReportModel.IsGrouped() ? ReportLayout.Stepped : ReportLayout.Columnar;
		}
		bool CanCreateSummaryForType(TypeSpecifics typeSpecifics) {
			return typeSpecifics == TypeSpecifics.Date
				|| typeSpecifics == TypeSpecifics.Integer
				|| typeSpecifics == TypeSpecifics.Float;
		}
		#region overrides
		public override bool CanFinish {
			get { return true; }
		}
		public override bool CanGoForward {
			get { return true; }
		}
		protected override void NavigateToNextPage(WizardController wizardController) {
			wizardController.NavigateTo(ReportModel.SummaryOptions.Count > 0
				? (object)ChooseSummaryOptionsPage.Create(ReportWizardModel, columnsCache)
				: ChooseReportLayoutPage.Create(ReportWizardModel)
			);
		}
		#endregion
		public class GroupsCollection : ObservableCollection<GroupingLevelInfo> {
			protected override void MoveItem(int oldIndex, int newIndex) {
				base.MoveItem(oldIndex, newIndex);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new List<GroupingLevelInfo>(Items), new List<GroupingLevelInfo>(Items)));
			}
			protected override void InsertItem(int index, GroupingLevelInfo item) {
				base.InsertItem(index, item);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new List<GroupingLevelInfo>(Items), new List<GroupingLevelInfo>(Items)));
			}
			protected override void RemoveItem(int index) {
				base.RemoveItem(index);
				OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, new List<GroupingLevelInfo>(Items), new List<GroupingLevelInfo>(Items)));
			}
		}
	}
}
