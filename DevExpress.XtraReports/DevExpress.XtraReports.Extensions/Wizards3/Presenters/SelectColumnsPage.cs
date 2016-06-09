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
using System.ComponentModel;
using System.Linq;
using DevExpress.Data.Browsing;
using DevExpress.Data.Browsing.Design;
using DevExpress.Data.XtraReports.DataProviders;
using DevExpress.Data.XtraReports.Wizard;
using DevExpress.Data.XtraReports.Wizard.Presenters;
using DevExpress.Data.XtraReports.Wizard.Views;
using DevExpress.DataAccess.Wizard;
using DevExpress.DataAccess.Wizard.Presenters;
using DevExpress.Utils;
using DevExpress.Data.WizardFramework;
namespace DevExpress.XtraReports.Wizards3.Presenters {
	public class SelectColumnsPage<TModel> : WizardPageBase<ISelectColumnsPageView, TModel> 
		where TModel : XtraReportModel 
	{
		#region Inner Classes
		protected class SortedColumnList : IList<ColumnInfo> {
			readonly List<ColumnInfo> innerList = new List<ColumnInfo>();
			readonly Comparison<ColumnInfo> comparison;
			public SortedColumnList(Comparison<ColumnInfo> comparison) {
				this.comparison = comparison;
			}
			public void AddRange(IEnumerable<ColumnInfo> collection) {
				innerList.AddRange(collection);
				SortInnerList();
			}
			public ColumnInfo Find(Predicate<ColumnInfo> predicate) {
				return innerList.Find(predicate);
			}
			void SortInnerList() {
				innerList.Sort(comparison);
			}
			#region IList<ColumnInfo> Members
			public int IndexOf(ColumnInfo item) {
				return innerList.IndexOf(item);
			}
			public void Insert(int index, ColumnInfo item) {
				innerList.Insert(index, item);
				SortInnerList();
			}
			public void RemoveAt(int index) {
				innerList.RemoveAt(index);
				SortInnerList();				
			}
			public ColumnInfo this[int index] {
				get {
					return innerList[index];
				}
				set {
					innerList[index] = value;
					SortInnerList();
				}
			}
			#endregion
			#region ICollection<ColumnInfo> Members
			public void Add(ColumnInfo item) {
				innerList.Add(item);
				SortInnerList();
			}
			public void Clear() {
				innerList.Clear();
			}
			public bool Contains(ColumnInfo item) {
				return innerList.Contains(item);
			}
			public void CopyTo(ColumnInfo[] array, int arrayIndex) {
				innerList.CopyTo(array, arrayIndex);
			}
			public int Count {
				get {
					return innerList.Count;
				}
			}
			public bool IsReadOnly {
				get { return false; }
			}
			public bool Remove(ColumnInfo item) {
				bool result = innerList.Remove(item);
				SortInnerList();
				return result;
			}
			#endregion
			#region IEnumerable<ColumnInfo> Members
			public IEnumerator<ColumnInfo> GetEnumerator() {
				return innerList.GetEnumerator();
			}
			#endregion
			#region IEnumerable Members
			System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
				return innerList.GetEnumerator();
			}
			#endregion
		}
		#endregion
		public SelectColumnsPage(ISelectColumnsPageView view) : base(view)  {
		}
		#region Fields & Properties
		readonly SortedColumnList availableColumns = new SortedColumnList((x, y) => string.Compare(x.DisplayName, y.DisplayName));
		readonly List<ColumnInfo> selectedColumns = new List<ColumnInfo>();
		readonly IColumnInfoCache columnInfoCache;
		protected SortedColumnList AvailableColumns { get { return availableColumns; } }
		protected List<ColumnInfo> SelectedColumns { get { return selectedColumns; } }
		public override bool MoveNextEnabled {
			get { return selectedColumns.Count > 0; }
		}
		public override bool FinishEnabled {
			get { return true; }
		}
		#endregion
		#region ctor
		public SelectColumnsPage(ISelectColumnsPageView view, IColumnInfoCache columnInfoCache)
			: base(view) {
			Guard.ArgumentNotNull(columnInfoCache, "columnInfoCache");
			this.columnInfoCache = columnInfoCache;
			view.ActiveColumnsChanged += new EventHandler(view_ActiveColumnsChanged);
			view.AddColumnsClicked += new EventHandler(view_AddColumnsClicked);
			view.AddAllColumnsClicked += new EventHandler(view_AddAllColumnsClicked);
			view.RemoveColumnsClicked += new EventHandler(view_RemoveColumnsClicked);
			view.RemoveAllColumnsClicked += new EventHandler(view_RemoveAllColumnsClicked);
		}
		#endregion
		#region Methods
		public override Type GetNextPageType() {
			return typeof(AddGroupingLevelPage<TModel>);
		}
		void view_ActiveColumnsChanged(object sender, EventArgs e) {
			RefreshButtons();
		}
		void view_AddColumnsClicked(object sender, EventArgs e) {
			OnAddRemoveButtonClicked(View.GetActiveAvailableColumns(), availableColumns, selectedColumns);
		}
		void view_AddAllColumnsClicked(object sender, EventArgs e) {
			OnAddRemoveButtonClicked(new List<ColumnInfo>(availableColumns), availableColumns, selectedColumns);
		}
		void view_RemoveColumnsClicked(object sender, EventArgs e) {
			OnAddRemoveButtonClicked(View.GetActiveSelectedColumns(), selectedColumns, availableColumns);
		}
		void view_RemoveAllColumnsClicked(object sender, EventArgs e) {
			OnAddRemoveButtonClicked(new List<ColumnInfo>(selectedColumns), selectedColumns, availableColumns);
		}
		void OnAddRemoveButtonClicked(IEnumerable<ColumnInfo> activeColumns, IList<ColumnInfo> from, IList<ColumnInfo> to) {
			MoveColumns(activeColumns, from, to);
			FillViewColumns();
			RefreshButtons();
			RaiseChanged();
		}
		void RefreshButtons() {
			View.EnableAddColumnsButton(View.GetActiveAvailableColumns().Length > 0);
			View.EnableAddAllColumnsButton(availableColumns.Count > 0);
			View.EnableRemoveColumnsButton(View.GetActiveSelectedColumns().Length > 0);
			View.EnableRemoveAllColumnsButton(selectedColumns.Count > 0);
		}
		static void MoveColumns(IEnumerable<ColumnInfo> activeColumns, IList<ColumnInfo> from, IList<ColumnInfo> to) {
			foreach(var column in activeColumns) {
				from.Remove(column);
				to.Add(column);
			}
		}
		public override void Commit() {
			Model.Columns = selectedColumns.Select(c => c.Name).ToArray();
			columnInfoCache.Columns = selectedColumns.ToArray();
		}
		public override void Begin() {
			View.ShowWaitIndicator(true);
			availableColumns.Clear();
			selectedColumns.Clear();
			RefreshView();
			View.ShowWaitIndicator(false);
			DataContextBase dataContext = new DataContextBase();
			PropertyDescriptorCollection properties = dataContext[Model.DataSchema].GetItemProperties();
			List<ColumnInfo> columnList = new List<ColumnInfo>();
			TypeSpecificsService typeSpecificsService = new TypeSpecificsService();
			foreach(PropertyDescriptor property in properties) {
				columnList.Add(new ColumnInfo() { Name = property.Name, DisplayName = property.DisplayName, TypeSpecifics = typeSpecificsService.GetPropertyTypeSpecifics(property) });
			}
			availableColumns.AddRange(columnList);
			selectedColumns.Clear();
			if(Model.Columns != null) {
				selectedColumns.AddRange(
					Model.Columns.Select(c => availableColumns.Find(x => x.Name == c)));
			}
			selectedColumns.ForEach(c => availableColumns.Remove(c));
			RefreshView();		
		}
		protected void RefreshView() {
			FillViewColumns();
			RefreshButtons();
			RaiseChanged();
		}
		void FillViewColumns() {
			View.FillAvailableColumns(availableColumns.ToArray());
			View.FillSelectedColumns(selectedColumns.ToArray());
		}
		#endregion    
	}
}
