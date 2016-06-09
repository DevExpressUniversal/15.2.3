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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.ViewerData;
namespace DevExpress.DashboardCommon.Server {
	public class InteractivitySession : InteractivitySessionBase<DataDashboardItem>, IInteractivitySession, ISelectionController, IMasterFilterController, IDrillDownController,
		ISingleFilterDefaultValueInquirer {
		readonly DimensionValueSet masterFilterValues;
		readonly Dictionary<Dimension, object> drillDownValues = new Dictionary<Dimension, object>();
		readonly OrderedDictionary<Dimension, IList<object>> valueStore = new OrderedDictionary<Dimension, IList<object>>();
		int drillDownLevel = 0;
		protected DimensionValueSet MasterFilterValues { get { return masterFilterValues; } }
		protected override bool AllowEmptyFilterValues { get { return false; } }
		protected override IList<Dimension> ActualInteractivityDimensions {
			get {
				IList<Dimension> interactivityDimensions = InteractivityDimensions;
				if(IsDrillDownEnabled && interactivityDimensions.Count != 0)
					return new List<Dimension> { interactivityDimensions[drillDownLevel] };
				else
					return interactivityDimensions;
			}
		}
		protected override IMasterFilterParameters MasterFilterParameters { get { return this; } }
		protected override Dimension CurrentDrillDownDimension { get { return CurrentDrillDownDimensionInternal; } }
		protected virtual IMasterFilterDataPreparer PreparerInternal {  get { return null; } }
		DashboardItemMasterFilterMode MasterFilterMode { get { return DashboardItem.MasterFilterMode; } }
		bool CanPerformMasterFilter { get { return !UseFakeData && IsMasterFilterEnabled; } }
		bool CanPerformDrillDown { get { return !UseFakeData && IsDrillDownEnabled && drillDownLevel < InteractivityDimensions.Count - 1; } }
		bool CanPerformDrillUp { get { return !UseFakeData && drillDownLevel > 0; } }
		bool UseFakeData { get { return DashboardItem.UseFakeData; } }
		Dimension CurrentDrillDownDimensionInternal {
			get {
				IList<Dimension> dimensions = ActualInteractivityDimensions;
				return dimensions.Count == 1 ? dimensions[0] : null;
			}
		}
		event EventHandler<SingleFilterDefaultValueEventArgs> SingleFilterDefaultValue;
		public InteractivitySession(DataDashboardItem dashboardItem)
			: base(dashboardItem) {
			masterFilterValues = new DimensionValueSet(valueStore);
		}
		protected virtual Dictionary<Dimension, MasterFilterRange> GetRanges() {
			return null;
		}
		void SetValues(IList<Dimension> dimensions, IValuesSet valuesSet) {
			ClearValues();
			IList<ISelectionColumn> columns = valuesSet.AsSelectionColumns().ToList();
			for(int i = 0; i < columns.Count; i++)
				valueStore.Add(dimensions[i], columns[i].ToList());
		}
		void ClearValues() {
			foreach(Dimension dimension in ActualInteractivityDimensions)
				if(valueStore.ContainsKey(dimension))
					valueStore.Remove(dimension);
		}
		void PerformDrillDownInternal(object value) {
			drillDownValues.Add(InteractivityDimensions[drillDownLevel], DashboardSpecialValuesInternal.FromSpecialValue(value));
			drillDownLevel++;
		}
		bool CanPerformOlapDrillDown(Dimension dimension, object value) {
			if(DashboardItem.IsOlapDataSource) {
				string columnName = dimension.DataMember;
				string memberName = (value ?? string.Empty).ToString();
				return DashboardItem.DataSource.GetOlapDimensionMember(columnName, memberName, DashboardItem.DataMember) != null;
			}
			return true;
		}
		object GetInteractivityActualValue(object value) {
			return DashboardItem.GetActualValue(ActualInteractivityDimensions[0], value);
		}
		#region ISelectionController
		IValuesSet ISelectionController.Selection { get { return SelectedValuesInternal; } }
		void ISelectionController.UpdateSelection(MultiDimensionalDataProvider dataProvider) {
			if(MasterFilterMode == DashboardItemMasterFilterMode.Single && SelectedValuesInternal.RowsCount == 0) {
				DashboardDataSet availableSelections = GetAvailableSelections(null, dataProvider.Data);
				DashboardDataRow currentSelection = availableSelections.RowCount > 0 ? availableSelections[0] : null;
				if(currentSelection != null) {
					SingleFilterDefaultValueEventArgs e = new SingleFilterDefaultValueEventArgs(DashboardItem.ComponentName, availableSelections, currentSelection);
					if(SingleFilterDefaultValue != null)
						SingleFilterDefaultValue(this, e);
					if(e.FilterValue != null)
						currentSelection = e.FilterValue[0];
					DashboardDataRow selection = e.FilterValue != null ? e.FilterValue[0] : currentSelection;
					if(selection != null && selection.Length > 0) {
						IValuesSet valueSet = new ISelectionRow[] { selection.ToList().Cast<object>().AsSelectionRow() }.AsValuesSet();
						((IMasterFilterController)this).SetMasterFilter(valueSet);
					}
				}
			}
		}
		protected virtual IValuesSet PrepareBeforeSetMasterFilter(IValuesSet valueSet) {
			return valueSet;
		}
		#endregion
		#region IMasterFilterController
		bool IMasterFilterController.CanSetMasterFilter { get { return CanPerformMasterFilter && InteractivityDimensions.Count > 0; } }
		bool IMasterFilterController.CanClearMasterFilter { get { return CanPerformMasterFilter && (masterFilterValues.FilterLength > drillDownLevel); } }
		bool IMasterFilterController.IsClearMasterFilterSupported { get { return MasterFilterMode == DashboardItemMasterFilterMode.Multiple; } }
		IList<string> IMasterFilterController.AffectedItems { get { return AffectedItems; } }
		bool IMasterFilterController.IsMultipleMasterFilterEnabled { get { return IsMultipleMasterFilterEnabled; } }
		bool IMasterFilterController.IsSingleMasterFilterEnabled { get { return IsSingleMasterFilterEnabled; } }
		IMasterFilterStateValidator IMasterFilterController.Validator { get { return null; } }
		IMasterFilterDataPreparer IMasterFilterController.DataPreparer { get { return PreparerInternal; } }
		void IMasterFilterController.SetMasterFilter(IValuesSet valueSetList) {
			if(CanPerformMasterFilter) {
				IValuesSet actualRows = GetActualRows(valueSetList);
				actualRows = PrepareBeforeSetMasterFilter(actualRows);
				if(actualRows != null)
					SetValues(ActualInteractivityDimensions, actualRows);
			}
		}
		void IMasterFilterController.ClearMasterFilter() {
			if(CanPerformMasterFilter)
				ClearValues();
		}
		void IMasterFilterController.PrepareMasterFilterState(DashboardItemState state) {
			if(valueStore.Count > 0) {
				MasterFilterState masterFilterState = new MasterFilterState();
				masterFilterState.IsSelectedValues = true;
				masterFilterState.ValuesSet = masterFilterValues.GetValuesSet(InteractivityDimensions);
				state.MasterFilterState = masterFilterState;
			}
		}
		void IMasterFilterController.SetMasterFilterState(DashboardItemState state) {
			valueStore.Clear();
			MasterFilterState masterFilterState = state.MasterFilterState;
			if(masterFilterState != null) {
				if(!state.MasterFilterState.IsSelectedValues) 
					throw new InvalidOperationException();
				IValuesSet valueSetList = masterFilterState.ValuesSet;
				IList<Dimension> dimensions = new List<Dimension>();
				for(int i = 0; i < valueSetList.ColumnsCount; i++)
					dimensions.Add(InteractivityDimensions[i]);
				SetValues(dimensions, valueSetList);
			}
		}
		IEnumerable<DimensionFilterValues> IMasterFilterController.GetFilterValues(IDataSourceInfoProvider dataSourceInfoPrvider, IDataSessionProvider sessionProvider) {
			return GetDisplayFilterValuesInternal(dataSourceInfoPrvider, sessionProvider);
		}
		#endregion 
		#region IMasterFilterParameters
		DimensionValueSet IMasterFilterParameters.Values { get { return masterFilterValues; } }
		Dictionary<Dimension, MasterFilterRange> IMasterFilterParameters.Ranges { get { return GetRanges(); } }
		bool IMasterFilterParameters.IsExcludingAllFilter { get { return false; } }
		bool IMasterFilterParameters.EmptyCriteria { get { return false; } }
		#endregion
		#region IDrillDownController
		bool IDrillDownController.IsDrillDownEnabled { get { return IsDrillDownEnabled; } }
		bool IDrillDownController.CanPerformDrillDown { get { return CanPerformDrillDown; } }
		bool IDrillDownController.CanPerformDrillUp { get { return CanPerformDrillUp; } }
		IList<DimensionFilterValues> IDrillDownController.DrillDownValues {
			get {
				if(drillDownValues == null || drillDownValues.Count == 0)
					return null;
				List<DimensionFilterValues> drillDownState = new List<DimensionFilterValues>();
				foreach(KeyValuePair<Dimension, object> pair in drillDownValues) {
					Dimension dimension = pair.Key;
					object value = DashboardSpecialValuesInternal.ToSpecialUniqueValue(pair.Value);
					drillDownState.Add(GetDimensionFilterValues(dimension.DisplayName, new[] { DashboardItem.GetDimensionFormatableValue(dimension, value) }));
				}
				return drillDownState;
			}
		}
		IList<object> IDrillDownController.DrillDownUniqueValues {
			get {
				if (drillDownValues == null || drillDownValues.Count == 0)
					return null;
				List<object> uniqueValues = new List<object>();
				foreach (KeyValuePair<Dimension, object> pair in drillDownValues)
					uniqueValues.Add(DashboardSpecialValuesInternal.ToSpecialUniqueValue(pair.Value));
				return uniqueValues;
			}
		}
		bool IDrillDownController.IsDrillDownAvailable(object value) {
			if (IsMasterFilterEnabled) {
				object actualValue = GetInteractivityActualValue(value);
				IValuesSet values = SelectedValuesInternal;
				return (values.ColumnsCount == 1) && (values.ColumnByIndex(0).Contains(actualValue));
			}
			return true;
		}
		bool IDrillDownController.IsDrillUpAvailable() {
			if (IsMasterFilterEnabled)
				return SelectedValuesInternal.IsEmpty;
			return true;
		}
		void IDrillDownController.PerformDrillDown(object value) {
			if (!CanPerformDrillDown || !CanPerformOlapDrillDown(ActualInteractivityDimensions[0], value))
				return;
			PerformDrillDownInternal(GetInteractivityActualValue(value));
		}
		void IDrillDownController.PerformDrillUp() {
			if (!CanPerformDrillUp)
				return;
			drillDownLevel--;
			drillDownValues.Remove(InteractivityDimensions[drillDownLevel]);
		}
		void IDrillDownController.PrepareDrillDownState(DashboardItemState state) {
			List<object> values = drillDownValues.Values.ToList<object>();
			int count = values.Count;
			if (count > 0) {
				object[] array = new object[count];
				values.CopyTo(array, 0);
				state.DrillDownState = array;
			}
		}
		void IDrillDownController.SetDrillDownState(DashboardItemState state) {
			drillDownValues.Clear();
			drillDownLevel = 0;
			IList drillDownState = state.DrillDownState;
			if (drillDownState != null) {
				foreach (object value in drillDownState)
					PerformDrillDownInternal(value);
			}
		}
		#endregion
		#region IDrillDownParameters
		Dictionary<Dimension, object> IDrillDownParameters.Values { get { return drillDownValues; } }
		Dimension IDrillDownParameters.CurrentDrillDownDimension { get { return CurrentDrillDownDimensionInternal; } }
		int? IDrillDownParameters.CurrentDrillDownLevel {
			get {
				if (IsDrillDownEnabled) {
					IList<Dimension> dimensions = DashboardItem.SelectionDimensionList;
					return dimensions.IndexOf(CurrentDrillDownDimensionInternal);
				}
				return null;
			}
		}
		#endregion
		#region IInteractivitySession
		ISelectionController IInteractivitySession.SelectionController { get { return this; } }
		IMasterFilterController IInteractivitySession.MasterFilterController { get { return this; } }
		IDrillDownController IInteractivitySession.DrillDownController { get { return this; } }
		ISingleFilterDefaultValueInquirer IInteractivitySession.SingleFilterDefaultValueInquirer { get { return this; } }
		IFilterElementDefaultValuesInquirer IInteractivitySession.FilterElementDefaultValuesInquirer { get { return null; } }
		IRangeFilterDefaultValueInquirer IInteractivitySession.RangeFilterDefaultValueInquirer { get { return null; } }
		#endregion
		#region ISingleFilterDefaultValueInquirer
		event EventHandler<SingleFilterDefaultValueEventArgs> ISingleFilterDefaultValueInquirer.SingleFilterDefaultValue { 
			add { SingleFilterDefaultValue += value; }
			remove { SingleFilterDefaultValue -= value; }
		}
		#endregion
	}
}
