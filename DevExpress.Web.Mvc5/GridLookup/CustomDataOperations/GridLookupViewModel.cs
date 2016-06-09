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
using System.ComponentModel;
using DevExpress.Data.IO;
using DevExpress.Web.Internal;
using DevExpress.Web.Mvc.Internal;
namespace DevExpress.Web.Mvc {
	public delegate void GridViewCustomBindingGetRowValuesHandler(GridViewCustomBindingGetRowValuesArgs e);
	public class GridViewCustomBindingGetRowValuesArgs: GridViewCustomBindingArgsBase {
		public GridViewCustomBindingGetRowValuesArgs(GridViewModel state, IEnumerable<object> keyValues)
			: base(state) {
			KeyValues = keyValues;
		}
		public IEnumerable<object> KeyValues { get; private set; }
		public IEnumerable<object> RowValues { get; set; }
	}
	public class GridLookupViewModel: GridViewModel {
		protected internal static string 
			GridPostfixClientID = "_DDD_gv",
			GridPostfix = "$DDD$gv";
		public GridLookupViewModel()
			: base() {
		}
		protected internal GridLookupViewModel(MVCxGridLookup gridLookup)
			: base(null) {
			GridLookup = gridLookup;
		}
		public string TextFormatString { get; set; }
		public IncrementalFilteringMode IncrementalFilteringMode { get; set; }
		public new GridLookupColumnStateCollection Columns { get { return (GridLookupColumnStateCollection)base.Columns; } }
		protected MVCxGridLookup GridLookup { get; private set; }
		protected internal override ASPxGridBase Grid { get { return GridLookup.GridView; } }
		protected internal new GridLookupCustomOperationHelper CustomOperationHelper { get { return (GridLookupCustomOperationHelper)base.CustomOperationHelper; } }
		protected internal new static GridLookupViewModel Load(string name) {
			return Load<GridLookupViewModel>(name);
		}
		public void ProcessCustomBinding(
			GridViewCustomBindingGetDataRowCountHandler getDataRowCountMethod,
			GridViewCustomBindingGetDataHandler getDataMethod,
			GridViewCustomBindingGetRowValuesHandler getRowValueMethod) {
			ProcessCustomBinding(getDataRowCountMethod, getDataMethod, null, null, null, getRowValueMethod);
		}
		public void ProcessCustomBinding(
				GridViewCustomBindingGetDataRowCountHandler getDataRowCountMethod,
				GridViewCustomBindingGetDataHandler getDataMethod,
				GridViewCustomBindingGetGroupingInfoHandler getGroupingInfoMethod,
				GridViewCustomBindingGetRowValuesHandler getRowValueMethod) {
			ProcessCustomBinding(getDataRowCountMethod, getDataMethod, null, getGroupingInfoMethod, null, getRowValueMethod);
		}
		public void ProcessCustomBinding(
				GridViewCustomBindingGetDataRowCountHandler getDataRowCountMethod,
				GridViewCustomBindingGetDataHandler getDataMethod,
				GridViewCustomBindingGetSummaryValuesHandler getSummaryValuesMethod,
				GridViewCustomBindingGetGroupingInfoHandler getGroupingInfoMethod,
				GridViewCustomBindingGetUniqueHeaderFilterValuesHandler getUniqueHeaderFilterValuesMethod,
				GridViewCustomBindingGetRowValuesHandler getRowValueMethod) {
			if(MvcUtils.IsCallback())
				ExpandCollapseInfo = GridViewCustomOperationCallbackHelper.GetExpandCollapseInfo();
			CustomOperationHelper.ProcessCustomBinding(getDataRowCountMethod, getDataMethod, getSummaryValuesMethod, getGroupingInfoMethod, getUniqueHeaderFilterValuesMethod, getRowValueMethod);
		}
		public void ApplyFilteringState(GridLookupFilteringState filteringState) {
			ApplyFilteringState((GridViewFilteringState)filteringState);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		protected new void ApplyFilteringState(GridViewFilteringState filteringState) {
			base.ApplyFilteringState(filteringState);
		}
		protected internal override void Assign(GridBaseViewModel source) {
			base.Assign(source);
			var viewModel = source as GridLookupViewModel;
			if(viewModel != null) {
				TextFormatString = viewModel.TextFormatString;
				IncrementalFilteringMode = viewModel.IncrementalFilteringMode;
			}
		}
		protected override void SaveCore(TypedBinaryWriter writer) {
			base.SaveCore(writer);
			writer.WriteObject(GridLookup.TextFormatString);
			writer.WriteObject((int)GridLookup.IncrementalFilteringMode);
		}
		protected override void LoadCore(TypedBinaryReader reader) {
			base.LoadCore(reader);
			TextFormatString = reader.ReadObject<string>();
			IncrementalFilteringMode = (IncrementalFilteringMode)reader.ReadObject<int>();
		}
		protected override GridBaseCustomOperationHelper CreateCustomOperationHelper() {
			return new GridLookupCustomOperationHelper(this);
		}
		protected override IGridColumnStateCollection CreateColumns() {
			return new GridLookupColumnStateCollection(this);
		}
		protected override GridBasePagerState CreatePager() {
			return new GridLookupPagerState(this);
		}
	}
	public class GridLookupFilteringState : GridViewFilteringState {
		public GridLookupFilteringState()
			: base() {
			if(MvcUtils.ModelBinderProcessing)
				GridLookupCustomOperationCallbackHelper.ProcessModelBinding(this);
		}
		internal new GridLookupViewModel GridViewModel { 
			get { return (GridLookupViewModel)base.GridViewModel;}
			set { base.GridViewModel = value; }
		}
	}
	public class GridLookupPagerState : GridViewPagerState {
		public GridLookupPagerState()
			: base() {
		}
		internal GridLookupPagerState(GridLookupViewModel owner)
			: base(owner) {
		}
		protected override string CurrentStateKey { get { return GridLookupCustomOperationCallbackHelper.GridStateKey; } }
	}
	public class GridLookupColumnState : GridViewColumnState {
		public GridLookupColumnState()
			: base() {
		}
		public GridLookupColumnState(string fieldName)
			: base(fieldName) {
		}
		protected override string CurrentStateKey { get { return GridLookupCustomOperationCallbackHelper.GridStateKey; } }
	}
	public class GridLookupColumnStateCollection : GridViewColumnStateCollection {
		internal GridLookupColumnStateCollection(GridLookupViewModel lookupViewModel)
			: base(lookupViewModel) {
		}
		public new GridLookupColumnState Add() {
			return Add(string.Empty);
		}
		public new GridLookupColumnState  Add(string fieldName) {
			return Add(new GridLookupColumnState(fieldName));
		}
		public GridLookupColumnState Add(GridLookupColumnState column) {
			return (GridLookupColumnState)base.Add(column);
		}
		protected override GridViewColumnState CreateItem(string fieldName) {
			return new GridLookupColumnState(fieldName);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		new GridViewColumnState Add(GridViewColumnState column) {
			return base.Add(column);
		}
	}
}
