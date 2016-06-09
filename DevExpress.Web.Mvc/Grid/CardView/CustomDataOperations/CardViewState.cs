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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.Data;
using DevExpress.Data.IO;
using DevExpress.Utils;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
namespace DevExpress.Web.Mvc {
	public class CardViewCustomBindingArgsBase : EventArgs {
		public CardViewCustomBindingArgsBase(CardViewModel state) {
			State = state;
			FilterExpression = state.TotalFilterExpression;
		}
		public CardViewModel State { get; protected set; }
		public string FilterExpression { get; protected set; }
	}
	public class CardViewCustomBindingGetDataCardCountArgs : CardViewCustomBindingArgsBase {
		public CardViewCustomBindingGetDataCardCountArgs(CardViewModel state) : base(state) { }
		public CardViewCustomBindingGetDataCardCountArgs(CardViewModel state, string filterExpression)
			: base(state) {
			FilterExpression = filterExpression;
		}
		public int DataCardCount { get; set; }
	}
	public class CardViewCustomBindingDataArgsBase : CardViewCustomBindingArgsBase {
		public CardViewCustomBindingDataArgsBase(CardViewModel state)
			: base(state) {
		}
		public IEnumerable Data { get; set; }
	}
	public class CardViewCustomBindingGetDataArgs : CardViewCustomBindingDataArgsBase {
		public CardViewCustomBindingGetDataArgs(CardViewModel state, int startDataCardIndex, int count)
			: base(state) {
			StartDataCardIndex = startDataCardIndex;
			DataCardCount = count;
		}
		public int StartDataCardIndex { get; protected set; }
		public int DataCardCount { get; protected set; }
	}
	public class CardViewCustomBindingGetSummaryValuesArgs : CardViewCustomBindingDataArgsBase {
		public CardViewCustomBindingGetSummaryValuesArgs(CardViewModel state, List<CardViewSummaryItemState> summaryItems)
			: base(state) {
			SummaryItems = summaryItems;
		}
		public List<CardViewSummaryItemState> SummaryItems { get; protected set; }
	}
	public class CardViewCustomBindingGetUniqueHeaderFilterValuesArgs : CardViewCustomBindingArgsBase {
		public CardViewCustomBindingGetUniqueHeaderFilterValuesArgs(CardViewModel state, string fieldName, string filterExpression)
			: base(state) {
			FieldName = fieldName;
		}
		public string FieldName { get; protected set; }
		public IEnumerable Data { get; set; }
	}
	public delegate void CardViewCustomBindingGetDataCardCountHandler(CardViewCustomBindingGetDataCardCountArgs e);
	public delegate void CardViewCustomBindingGetDataHandler(CardViewCustomBindingGetDataArgs e);
	public delegate void CardViewCustomBindingGetSummaryValuesHandler(CardViewCustomBindingGetSummaryValuesArgs e);
	public delegate void CardViewCustomBindingGetUniqueHeaderFilterValuesHandler(CardViewCustomBindingGetUniqueHeaderFilterValuesArgs e);
	public class CardViewModel: GridBaseViewModel {
		public CardViewModel()
			: base() {
		}
		protected internal CardViewModel(MVCxCardView cardView)
			: base(cardView) {
		}
		public new bool IsFilterApplied { get { return base.IsFilterApplied; } set { base.IsFilterApplied = value; } }
		public new string KeyFieldName { get { return base.KeyFieldName; } set { base.KeyFieldName = value; } }
		public new string FilterExpression { get { return base.FilterExpression; } set { base.FilterExpression = value; } }
		public new string AppliedFilterExpression { get { return base.AppliedFilterExpression; } }
		public new CardViewColumnStateCollection Columns { get { return (CardViewColumnStateCollection)base.Columns; } }
		public new CardViewPagerState Pager { get { return (CardViewPagerState)base.Pager; } }
		public new CardViewSearchPanelState SearchPanel { get { return (CardViewSearchPanelState)base.SearchPanel; } }
		public new CardViewSummaryItemStateCollection TotalSummary { get { return (CardViewSummaryItemStateCollection)base.TotalSummary; } }
		public Layout LayoutMode { get; set; }
		public new ReadOnlyCollection<CardViewColumnState> SortedColumns {
			get { return base.SortedColumns.OfType<CardViewColumnState>().ToList().AsReadOnly(); }
		}
		protected MVCxCardView CardView { get { return (MVCxCardView)Grid; } }
		protected internal new CardViewCustomOperationHelper CustomOperationHelper {
			get { return (CardViewCustomOperationHelper)base.CustomOperationHelper; }
			set { CustomOperationHelper = value; }
		}
		public void ApplyPagingState(CardViewPagerState pagerState) {
			base.ApplyPagingState(pagerState);
		}
		public void ApplySortingState(CardViewColumnState columnState) {
			base.ApplySortingState(columnState);
		}
		public void ApplySortingState(CardViewColumnState columnState, bool reset) {
			base.ApplySortingState(columnState, reset);
		}
		public void ApplyFilteringState(CardViewColumnState columnState) {
			base.ApplyFilteringState(columnState);
		}
		public void ApplyFilteringState(CardViewFilteringState filteringState) {
			base.ApplyFilteringState(filteringState);
		}
		public void ProcessCustomBinding(
				CardViewCustomBindingGetDataCardCountHandler getDataCardCountMethod,
				CardViewCustomBindingGetDataHandler getDataMethod) {
			ProcessCustomBinding(getDataCardCountMethod, getDataMethod, null, null);
		}
		public void ProcessCustomBinding(
				CardViewCustomBindingGetDataCardCountHandler getDataCardCountMethod,
				CardViewCustomBindingGetDataHandler getDataMethod,
				CardViewCustomBindingGetSummaryValuesHandler getSummaryValuesMethod) {
			ProcessCustomBinding(getDataCardCountMethod, getDataMethod, getSummaryValuesMethod, null);
		}
		public void ProcessCustomBinding(
			 CardViewCustomBindingGetDataCardCountHandler getDataCardCountMethod,
			 CardViewCustomBindingGetDataHandler getDataMethod,
			 CardViewCustomBindingGetUniqueHeaderFilterValuesHandler getUniqueHeaderFilterValuesMethod) {
			ProcessCustomBinding(getDataCardCountMethod, getDataMethod, null, getUniqueHeaderFilterValuesMethod);
		}
		public void ProcessCustomBinding(
				CardViewCustomBindingGetDataCardCountHandler getDataCardCountMethod,
				CardViewCustomBindingGetDataHandler getDataMethod,
				CardViewCustomBindingGetSummaryValuesHandler getSummaryValuesMethod,
				CardViewCustomBindingGetUniqueHeaderFilterValuesHandler getUniqueHeaderFilterValuesMethod) {
			CustomOperationHelper.ProcessCustomBinding(getDataCardCountMethod, getDataMethod, getSummaryValuesMethod, getUniqueHeaderFilterValuesMethod);
		}
		protected override IGridColumnStateCollection CreateColumns() {
			return new CardViewColumnStateCollection(this);
		}
		protected override GridBasePagerState CreatePager() {
			return new CardViewPagerState(this);
		}
		protected override GridBaseSearchPanelState CreateSearchPanel() {
			return new CardViewSearchPanelState();
		}
		protected override IGridSummaryItemStateCollection CreateSummary() {
			return new CardViewSummaryItemStateCollection();
		}
		public void SortBy(CardViewColumnState column, bool reset) {
			base.SortBy(column, reset);
		}
		protected override GridBaseCustomOperationHelper CreateCustomOperationHelper() {
			return new CardViewCustomOperationHelper(this);
		}
		protected internal static CardViewModel Load(string name) {
			return Load<CardViewModel>(name);
		}
		protected override void SaveCore(TypedBinaryWriter writer) {
			base.SaveCore(writer);
			writer.WriteObject(CardView.Settings.LayoutMode);
		}
		protected override void LoadCore(TypedBinaryReader reader) {
			base.LoadCore(reader);
			LayoutMode = reader.ReadObject<Layout>();
		}
		protected override void SavePager(TypedBinaryWriter writer) {
			base.SavePager(writer);
			writer.WriteObject(CardView.SettingsPager.SettingsTableLayout.ColumnCount);
			writer.WriteObject(CardView.SettingsPager.SettingsTableLayout.RowsPerPage);
			writer.WriteObject(CardView.SettingsPager.SettingsFlowLayout.ItemsPerPage);
		}
		protected override void LoadPager(TypedBinaryReader reader) {
			base.LoadPager(reader);
			Pager.SettingsTableLayout.ColumnCount = reader.ReadObject<int>();
			Pager.SettingsTableLayout.RowsPerPage = reader.ReadObject<int>();
			Pager.SettingsFlowLayout.ItemsPerPage = reader.ReadObject<int>();
		}
		protected internal override void Assign(GridBaseViewModel source) {
			CardViewModel viewModel = source as CardViewModel;
			if(viewModel != null) {
				LayoutMode = viewModel.LayoutMode;
			}
			base.Assign(source);
		}
	}
	public class CardViewPagerState: GridBasePagerState {
		public CardViewPagerState()
			: base() {
			Initialize();
			if(MvcUtils.ModelBinderProcessing)
				GridViewCustomOperationCallbackHelper.ProcessModelBinding(this, CardViewModel.Load(CurrentStateKey));
		}
		internal CardViewPagerState(CardViewModel owner)
			: base(owner) {
			Initialize();
			ViewModel = owner;
		}
		void Initialize() {
			SettingsTableLayout = new CardViewTableLayoutSettings(null);
			SettingsFlowLayout = new CardViewFlowLayoutSettings(null);
		}
		public new int PageIndex { get { return base.PageIndex; } set { base.PageIndex = value; } }
		public CardViewTableLayoutSettings SettingsTableLayout { get; private set; }
		public CardViewFlowLayoutSettings SettingsFlowLayout { get; private set; }
		protected CardViewModel ViewModel { get; private set; }
		protected internal override int PageSize {
			get {
				if(ViewModel.LayoutMode == Layout.Table)
					return SettingsTableLayout.ItemsPerPage;
				return SettingsFlowLayout.ItemsPerPage;
			}
			set {
				if(ViewModel.LayoutMode == Layout.Table)
					SettingsTableLayout.RowsPerPage = value;
				else
					SettingsFlowLayout.ItemsPerPage = value;
			}
		}
		public override void Assign(GridBasePagerState source) {
			CardViewPagerState pagerState = source as CardViewPagerState;
			if(pagerState != null) {
				SettingsTableLayout.Assign(pagerState.SettingsTableLayout);
				SettingsFlowLayout.Assign(pagerState.SettingsFlowLayout);
			}
			base.Assign(source);
		}
	}
	public class CardViewFilteringState: GridBaseFilteringState {
		public CardViewFilteringState()
			: base() {
			if(MvcUtils.ModelBinderProcessing)
				GridViewCustomOperationCallbackHelper.ProcessModelBinding(this, CardViewModel.Load);
		}
		public new ReadOnlyCollection<CardViewColumnState> ModifiedColumns { get { return base.ModifiedColumns.OfType<CardViewColumnState>().ToList().AsReadOnly(); } }
		public new string FilterExpression { get { return base.FilterExpression; } set { base.FilterExpression = value; } }
		public new bool IsFilterApplied { get { return base.IsFilterApplied; } set { base.IsFilterApplied = value; } }
		public new string SearchPanelFilter { get { return base.SearchPanelFilter; } set { base.SearchPanelFilter = value; } }
	}
	public class CardViewSearchPanelState: GridBaseSearchPanelState {
		public new string ColumnNames { get { return base.ColumnNames; } set { base.ColumnNames = value; } }
		public new string Filter { get { return base.Filter; } set { base.Filter = value; } }
		public new GridViewSearchPanelGroupOperator GroupOperator { get { return base.GroupOperator; } set { base.GroupOperator = value; } }
	}
	public class CardViewSummaryItemState : GridBaseSummaryItemState {
		public new string FieldName { get { return base.FieldName; } set { base.FieldName = value; } }
		public new SummaryItemType SummaryType { get { return base.SummaryType; } set { base.SummaryType = value; } }
		public new string Tag { get { return base.Tag; } set { base.Tag = value; } }
	}
	public class CardViewColumnState : GridBaseColumnState {
		public CardViewColumnState()
			: base() {
			if(MvcUtils.ModelBinderProcessing)
				GridViewCustomOperationCallbackHelper.ProcessModelBinding(this, CardViewModel.Load(CurrentStateKey));
		}
		public CardViewColumnState(string fieldName)
			: base(fieldName) {
		}
		public new string FieldName { get { return base.FieldName; } set { base.FieldName = value; } }
		public new ColumnSortOrder SortOrder { get { return base.SortOrder; } set { base.SortOrder = value; } }
		public new string FilterExpression { get { return base.FilterExpression; } set { base.FilterExpression = value; } }
		public new int SortIndex { get { return base.SortIndex; } set { base.SortIndex = value; } }
		public new DefaultBoolean AllowFilterBySearchPanel { get { return base.AllowFilterBySearchPanel; } set { base.AllowFilterBySearchPanel = value; } }
		public new void ForceType(Type columnType) { base.ForceType(columnType); }
	}
	public class CardViewColumnStateCollection : GridBaseColumnStateCollection<CardViewColumnState> {
		internal CardViewColumnStateCollection(CardViewModel cardViewModel)
			: base(cardViewModel) {
		}
		public new CardViewColumnState this[string fieldName] { get { return base[fieldName]; } }
		public new CardViewColumnState Add() {
			return base.Add();
		}
		public new CardViewColumnState Add(string fieldName) {
			return base.Add(fieldName);
		}
		public new CardViewColumnState Add(CardViewColumnState column) {
			return base.Add(column);
		}
		protected override CardViewColumnState CreateItem(string fieldName) {
			return new CardViewColumnState(fieldName);
		}
	}
	public class CardViewSummaryItemStateCollection : GridBaseSummaryItemStateCollection<CardViewSummaryItemState> {
		public new CardViewSummaryItemState Add(CardViewSummaryItemState state) {
			base.Add(state);
			return state;
		}
		protected internal override CardViewSummaryItemState CreateNewItem() {
			return new CardViewSummaryItemState();
		}
	}
}
