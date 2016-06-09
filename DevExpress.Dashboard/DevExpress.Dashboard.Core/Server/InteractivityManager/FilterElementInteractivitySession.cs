#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.DataProcessing;
namespace DevExpress.DashboardCommon.Server {
	public abstract class FilterElementInteractivitySession : InteractivitySessionBase<FilterElementDashboardItem>, IInteractivitySession, ISelectionController,
		IMasterFilterController, IMasterFilterStateValidator, IFilterElementDefaultValuesInquirer {
		IList<ISelectionRow> allSelectionValues = new List<ISelectionRow>();
		bool isDefaultInitialized;
		DimensionValueSet actualDimensionValuesSet;
		protected override bool AllowEmptyFilterValues { get { return true; } }
		protected override IMasterFilterParameters MasterFilterParameters { get { return this; } }
		protected override Dimension CurrentDrillDownDimension { get { return null; } }
		protected IEnumerable<ISelectionRow> AllSelectionValues { get { return allSelectionValues; } set { allSelectionValues = value.ToList(); } }
		protected abstract IEnumerable<ISelectionRow> ActualSelected { get; set; }
		protected abstract bool IsEmptyCriteria { get; }
		protected DimensionValueSet ActualDimensionValuesSet {
			get {
				if (actualDimensionValuesSet == null) {
					OrderedDictionary<Dimension, IList<object>> valueStore = new OrderedDictionary<Dimension, IList<object>>();
					IList<ISelectionColumn> dimensionValues = ActualSelected.AsValuesSet().AsSelectionColumns().ToList();
					for (int i = 0; i < dimensionValues.Count; i++)
						valueStore.Add(InteractivityDimensions[i], dimensionValues[i].ToList());
					actualDimensionValuesSet = new DimensionValueSet(valueStore);
				}
				return actualDimensionValuesSet;
			}
		}
		protected bool UseCriteriaOptimization { get { return DashboardItem.UseCriteriaOptimization; } }
		event EventHandler<FilterElementDefaultValuesEventArgs> FilterElementDefaultValues;
		protected FilterElementInteractivitySession(FilterElementDashboardItem dashboardItem)
			: base(dashboardItem) {
		}
		protected void InitializeDefaultValues() {
			ActualSelected = ValidateValues(RequestDefaultSelection(ValidateValues(AllSelectionValues)));
			InvalidateActualSelection();
		}
		protected abstract IEnumerable<ISelectionRow> ValidateValues(IEnumerable<ISelectionRow> selection);
		protected abstract bool IsValidSelection(IEnumerable<ISelectionRow> selection);
		protected abstract void PrepareMasterFilterState(MasterFilterState state);
		protected abstract void ApplyMasterFilterState(MasterFilterState state);
		IEnumerable<ISelectionRow> RequestDefaultSelection(IEnumerable<ISelectionRow> initialSelected) {
			if (FilterElementDefaultValues != null) {
				List<ISelectionRow> selected = new List<ISelectionRow>(initialSelected);
				DashboardDataSet availableSelection = GetAvailableSelections(AllSelectionValues.AsValuesSet(), null);
				FilterElementDefaultValuesEventArgs args = new FilterElementDefaultValuesEventArgs(DashboardItem, availableSelection, selected);
				FilterElementDefaultValues(this, args);
				if (args.Changed) {
					selected.Clear();
					foreach (DashboardDataRow row in args.FilterValues)
						selected.Add(row.ToList().Cast<object>().AsSelectionRow());
				}
				return selected;
			}
			else
				return initialSelected;
		}
		void InvalidateActualSelection() {
			ActualSelected = null;
			actualDimensionValuesSet = null;
		}
		void SetSelectedValues(IEnumerable<ISelectionRow> valuesSet) {
			if (!IsValidSelection(valuesSet))
				throw new ArgumentException("Invalid selection values count");
			ActualSelected = valuesSet;
			InvalidateActualSelection();
		}
		#region ISelectionController
		IValuesSet ISelectionController.Selection { get { return SelectedValuesInternal; } }
		void ISelectionController.UpdateSelection(MultiDimensionalDataProvider dataProvider) {
			if (InteractivityDimensions.Count > 0) {
				AllSelectionValues = Helper.GetAllSelectionValues(dataProvider.Data, InteractivityDimensions);
				if (allSelectionValues.Count > 0 && !isDefaultInitialized) {
					InitializeDefaultValues();
					isDefaultInitialized = true;
				}
			}
			else
				AllSelectionValues = Enumerable.Empty<ISelectionRow>();
			InvalidateActualSelection();
		}
		#endregion
		#region IInteractivitySession
		IMasterFilterController IInteractivitySession.MasterFilterController { get { return this; } }
		IDrillDownController IInteractivitySession.DrillDownController { get { return null; } }
		ISelectionController IInteractivitySession.SelectionController { get { return this; } }
		ISingleFilterDefaultValueInquirer IInteractivitySession.SingleFilterDefaultValueInquirer { get { return null; } }
		IFilterElementDefaultValuesInquirer IInteractivitySession.FilterElementDefaultValuesInquirer { get { return this; } }
		IRangeFilterDefaultValueInquirer IInteractivitySession.RangeFilterDefaultValueInquirer { get { return null; } }
		#endregion
		#region IMasterFilterController
		bool IMasterFilterController.CanSetMasterFilter { get { return true; } }
		bool IMasterFilterController.CanClearMasterFilter { get { return false; } }
		bool IMasterFilterController.IsClearMasterFilterSupported { get { return false; } }
		IList<string> IMasterFilterController.AffectedItems { get { return AffectedItems; } }
		bool IMasterFilterController.IsMultipleMasterFilterEnabled { get { return IsMultipleMasterFilterEnabled; } }
		bool IMasterFilterController.IsSingleMasterFilterEnabled { get { return IsSingleMasterFilterEnabled; } }
		IMasterFilterStateValidator IMasterFilterController.Validator { get { return this; } }
		IMasterFilterDataPreparer IMasterFilterController.DataPreparer {  get { return null; } }
		void IMasterFilterController.SetMasterFilter(IValuesSet valueSetList) {
			IValuesSet actualRows = GetActualRows(valueSetList);
			if (actualRows != null)
				SetSelectedValues(actualRows);
		}
		void IMasterFilterController.ClearMasterFilter() {
			throw new InvalidOperationException("Clearing of MasterFilter isn't supported for FilterElement");
		}
		void IMasterFilterController.PrepareMasterFilterState(DashboardItemState state) {
			MasterFilterState masterFilterState = new MasterFilterState();
			PrepareMasterFilterState(masterFilterState);
			state.MasterFilterState = masterFilterState; 
		}
		void IMasterFilterController.SetMasterFilterState(DashboardItemState state) {
			MasterFilterState masterFilterState = state.MasterFilterState;
			if (masterFilterState != null) {
				isDefaultInitialized = true;
				ApplyMasterFilterState(state.MasterFilterState);
				InvalidateActualSelection();
			}
		}
		IEnumerable<DimensionFilterValues> IMasterFilterController.GetFilterValues(IDataSourceInfoProvider dataSourceInfoPrvider, IDataSessionProvider sessionProvider) {
			return GetDisplayFilterValuesInternal(dataSourceInfoPrvider, sessionProvider);
		}
		#endregion
		#region IMasterFilterParameters
		DimensionValueSet IMasterFilterParameters.Values { get { return ActualDimensionValuesSet; } }
		Dictionary<Dimension, MasterFilterRange> IMasterFilterParameters.Ranges { get { return null; } }
		bool IMasterFilterParameters.IsExcludingAllFilter { get { return ActualSelected.Count() == 0 && AllSelectionValues.Count() != 0; } }
		bool IMasterFilterParameters.EmptyCriteria { get { return UseCriteriaOptimization && IsEmptyCriteria; } }
		#endregion
		#region IMasterFilterStateValidator
		void IMasterFilterStateValidator.ValidateMasterFilterState(IDataSourceInfoProvider dataInfoProvider, IDataSessionProvider dataSessionProvider, IActualParametersProvider parametersProvider) {
			DataSourceInfo dataInfo = dataInfoProvider.GetDataSourceInfo(DashboardItem.DataSourceName, DashboardItem.DataMember);
			if (dataInfo != null) {
				IDataSession dataSession = dataSessionProvider.GetDataSession(DashboardItem.ComponentName, dataInfo, DashboardItem.FakeDataSource);
				if (dataSession != null) {
					HierarchicalDataParams hierarchicalData = dataSession.GetData(((ISliceDataQueryProvider)DashboardItem).GetDataQuery(dataSessionProvider)).HierarchicalData;
					if (hierarchicalData != null) {
						MultidimensionalDataDTO dataDTO = new MultidimensionalDataDTO { HierarchicalDataParams = hierarchicalData };
						MultiDimensionalDataProvider dataProvider = new MultiDimensionalDataProvider(DashboardItem, dataDTO, parametersProvider);
						((ISelectionController)this).UpdateSelection(dataProvider);
					}
				}
			}
		}
		#endregion
		#region IFilterElementDefaultValuesInquirer
		event EventHandler<FilterElementDefaultValuesEventArgs> IFilterElementDefaultValuesInquirer.FilterElementDefaultValues {
			add { FilterElementDefaultValues += value; }
			remove { FilterElementDefaultValues -= value; }
		}
		#endregion        
	}
}
