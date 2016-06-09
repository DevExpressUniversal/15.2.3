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
using DevExpress.Utils;
using DevExpress.Data.Filtering;
using DevExpress.Data.Helpers;
using DevExpress.Data;
using System.ComponentModel;
using DevExpress.XtraEditors;
namespace DevExpress.XtraBars.Docking2010.Customization {
	class SearchTileBarItemCollection : TileItemCollection, IDataControllerSort, ISupportSearchDataAdapter {
		SearchDataAdapter adapter;
		public new SearchTileBarGroup Owner { get { return base.Owner as SearchTileBarGroup; } }
		public int VisibleCount { get { return GetVisibleCount(); } }
		public SearchTileBarItemCollection(SearchTileBarGroup group)
			: base(group) {
			adapter = new SearchDataAdapter<SearchTileBarItem>();
			adapter.SetDataSource(this.InnerList);
			adapter.SortClient = this;
			adapter.SortInfo.ClearAndAddRange(new DataColumnSortInfo[] { new DataColumnSortInfo(adapter.Columns["Text"]) });
		}
		public override TileItem this[int index] {
			get { return adapter.GetValueAtIndex(string.Empty, index) as TileItem; }
			set { }
		}
		public DevExpress.Data.Filtering.CriteriaOperator FilterCriteria {
			get { return adapter.FilterCriteria; }
			set {
				adapter.FilterCriteria = value;
				adapter.DoRefresh();
			}
		}
		protected virtual int GetVisibleCount() {
			if(Owner == null ) return 0;
			return adapter.VisibleListSourceRowCount;
		}
		protected override IEnumerator<ITileItem> GetEnumeratorCore() {
			int visibleCount = Owner.VisibleRowCount;
			for(int i = 0; i < visibleCount; i++) {
				SearchTileBarItem searchItem = adapter.GetValueAtIndex(string.Empty, i) as SearchTileBarItem;
				if(searchItem != null)
					yield return searchItem;
			}
			if(Owner.AllowShowTileBarExpand)
				yield return Owner.ExpandButton;
		}
		protected override void Dispose(bool disposing) {
			base.Dispose(disposing);
			Ref.Dispose(ref adapter);
		}
		#region IDataControllerSort Members
		void IDataControllerSort.AfterGrouping() { }
		void IDataControllerSort.AfterSorting() { }
		void IDataControllerSort.BeforeGrouping() { }
		void IDataControllerSort.BeforeSorting() { }
		ExpressiveSortInfo.Row IDataControllerSort.GetCompareRowsMethodInfo() { return null; }
		string IDataControllerSort.GetDisplayText(int listSourceRow, DataColumnInfo info, object value, string columnName) {
			throw new NotImplementedException();
		}
		string[] IDataControllerSort.GetFindByPropertyNames() { return new string[] { "Text" }; }
		ExpressiveSortInfo.Cell IDataControllerSort.GetSortCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType, ColumnSortOrder order) {
			Func<SearchObjectFake, bool> func = CriteriaCompiler.ToPredicate<SearchObjectFake>(FilterCriteria, CriteriaCompilerDescriptor.Get<SearchObjectFake>());
			if(func == null) return null;
			Func<ExpressiveSortInfo.CondencedAndSourceIndicesPair, ExpressiveSortInfo.CondencedAndSourceIndicesPair, object, object, int?> comparer
			   = (r1, r2, v1, v2)
			   =>
			   {
				   SearchObjectFake c1 = new SearchObjectFake((string)v1);
				   SearchObjectFake c2 = new SearchObjectFake((string)v2);
				   if(func(c1))
					   return func(c2) ? 0 : -1;
				   if(func(c2))
					   return func(c1) ? 0 : 1;
				   return null;
			   };
			return new ExpressiveSortInfo.Cell(true, null, null, comparer, false, true);
		}
		ExpressiveSortInfo.Cell IDataControllerSort.GetSortGroupCellMethodInfo(DataColumnInfo dataColumnInfo, Type baseExtractorType) { return null; }
		bool? IDataControllerSort.IsEqualGroupValues(int listSourceRow1, int listSourceRow2, object value1, object value2, DataColumnInfo sortColumn) { return false; }
		bool IDataControllerSort.RequireDisplayText(DataColumnInfo column) { return false; }
		void IDataControllerSort.SubstituteSortInfo(SubstituteSortInfoEventArgs args) { }
		class SearchObjectFake {
			public SearchObjectFake(string text) {
				Text = text;
			}
			public string Text { get; private set; }
			public object Tag { get; private set; }
			public string Content { get; private set; }
		}
		#endregion
		#region ISupportSearchDataAdapter Members
		bool ISupportSearchDataAdapter.AdapterEnabled {
			get { return true; }
			set { }
		}
		int ISupportSearchDataAdapter.GetSourceIndex(int filteredIndex) { return adapter.GetListSourceRowIndex(filteredIndex); }
		int ISupportSearchDataAdapter.GetVisibleIndex(int index) { return index; }
		#endregion
	}
	class SearchTileBarCriteriaProvider : SearchControlCriteriaProviderBase {
		string[] columns = new string[] { "Text", "Tag", "Content" };
		protected override Data.Filtering.CriteriaOperator CalcActiveCriteriaOperatorCore(SearchControlQueryParamsEventArgs args, Data.Helpers.FindSearchParserResults result) {
			if(!CheckResult(result)) return null;
			return DxFtsContainsHelper.Create(columns, result, args.FilterCondition);
		}
		protected bool CheckResult(Data.Helpers.FindSearchParserResults result) {
			int count = 0;
			foreach(string column in columns) {
				if(result.ContainsField(column))					
					count++;
			}
			return count == result.FieldCount;
		}
		protected override Data.Helpers.FindSearchParserResults CalcResultCore(SearchControlQueryParamsEventArgs args) {
			return new FindSearchParser().Parse(args.SearchText);
		}
		protected override void DisposeCore() { }
	}
}
