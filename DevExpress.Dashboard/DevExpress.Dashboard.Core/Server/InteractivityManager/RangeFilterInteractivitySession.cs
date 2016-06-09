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
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Server {
	public class RangeFilterInteractivitySession : IInteractivitySession, IRangeInteractivitySession, ISelectionController, IMasterFilterController, IRangeFilterDefaultValueInquirer {
		#region static
		static bool CheckValueInRange(object value, object start, object end) {
			int startResult = Comparer.Default.Compare(value, start);
			int endResult = Comparer.Default.Compare(value, end);
			if (startResult >= 0 && endResult <= 0)
				return true;
			return false;
		}
		#endregion
		readonly RangeFilterDashboardItem dashboardItem;
		object startValue;
		object endValue;
		Dictionary<Dimension, MasterFilterRange> MasterFilterRanges {
			get {
				if (Argument != null)
					return new Dictionary<Dimension, MasterFilterRange> { { Argument, new MasterFilterRange { 
						Left = ActualMinValue, 
						Right = ActualMaxValue, 
						Start = startValue, 
						End = endValue } } };
				return null;
			}
		}
		object ActualMinValue {
			get {
				if (startValue == null || endValue == null)
					return null;
				if (MinValue == null)
					return startValue;
				if (DataBindingHelper.IsNumericType(MinValue.GetType())) {
					double min, start, end;
					if (Double.TryParse(MinValue.ToString(), out min) && Double.TryParse(startValue.ToString(), out start) && Double.TryParse(endValue.ToString(), out end))
						if (CheckValueInRange(min, start, end))
							return MinValue;
				}
				else {
					try {
						if (CheckValueInRange(MinValue, startValue, endValue))
							return MinValue;
					}
					catch {
					}
				}
				return startValue;
			}
		}
		object ActualMaxValue {
			get {
				if (startValue == null || endValue == null)
					return null;
				if (MaxValue == null)
					return endValue;
				if (DataBindingHelper.IsNumericType(MaxValue.GetType())) {
					double max, start, end;
					if (Double.TryParse(MaxValue.ToString(), out max) && Double.TryParse(startValue.ToString(), out start) && Double.TryParse(endValue.ToString(), out end))
						if (CheckValueInRange(max, start, end))
							return MaxValue;
				}
				else {
					try {
						if (CheckValueInRange(MaxValue, startValue, endValue))
							return MaxValue;
					}
					catch {
					}
				}
				return endValue;
			}
		}
		object MinValue { get { return dashboardItem.MinValue; } }
		object MaxValue { get { return dashboardItem.MaxValue; } }
		Dimension Argument { get { return dashboardItem.Argument; } }
		event EventHandler<RangeFilterDefaultValueEventArgs> RangeFilterDefaultValue;
		public RangeFilterInteractivitySession(RangeFilterDashboardItem dashboardItem) {
			Guard.ArgumentNotNull(dashboardItem, "dashboardItem");
			this.dashboardItem = dashboardItem;
		}
		void SetRange(object minValue, object maxValue) {
			dashboardItem.SetMinMaxValues(minValue, maxValue);
		}
		void ClearRange() {
			dashboardItem.ClearRange();
		}
		RangeFilterSelection GetDefaultRangeSelection() {
			if (RangeFilterDefaultValue != null) {
				RangeFilterDefaultValueEventArgs args = new RangeFilterDefaultValueEventArgs(dashboardItem.ComponentName);
				RangeFilterDefaultValue(this, args);
				return args.Range;
			}
			return null;
		}
		IEnumerable<DimensionFilterValues> GetDisplayFilterValues() {
			IList<DimensionFilterValues> values = new List<DimensionFilterValues>();
			IDictionary<Dimension, MasterFilterRange> ranges = ((IMasterFilterController)this).Ranges;
			if(ranges == null)
				return values;
			foreach(KeyValuePair<Dimension, MasterFilterRange> pair in ranges)
				if(pair.Value.IsEmpty)
					return values;
			DimensionFilterValues dimensionFilterValues;
			foreach(KeyValuePair<Dimension, MasterFilterRange> pair in ranges) {
				dimensionFilterValues = new DimensionFilterValues();
				Dimension dimension = pair.Key;
				dimensionFilterValues.Name = dimension.DisplayName;
				dimensionFilterValues.Truncated = false;
				MasterFilterRange filterRange = pair.Value;
				FormattableValue range = new FormattableValue {
					RangeLeft = filterRange.Left,
					RangeRight = filterRange.Right,
					Type = MasterFilterValueType.Range,
					Format = dimension.CreateValueFormatViewModel()
				};
				dimensionFilterValues.Values = new List<FormattableValue>();
				dimensionFilterValues.Values.Add(range);
				values.Add(dimensionFilterValues);
			}
			return values;
		}
		#region ISelectionController
		IValuesSet ISelectionController.Selection { 
			get {
				Dictionary<Dimension, MasterFilterRange> masterFilterRanges = MasterFilterRanges;
				if (masterFilterRanges != null)
					return masterFilterRanges.First().Value.SelectedValues;
				return null;
			}
		}
		void ISelectionController.UpdateSelection(MultiDimensionalDataProvider dataProvider) {
			if (Argument != null) {
				MultiDimensionalData data = dataProvider.Data;
				if (data != null) {
					AxisPoint point = data.Axes[RangeFilterDashboardItem.xmlArgument].RootPoint;
					if (point != null && point.ChildItems.Count > 0) {
						startValue = point.ChildItems.First().Value;
						endValue = point.ChildItems.Last().Value;
						if (!DashboardSpecialValuesInternal.IsDashboardSpecialValue(startValue) && !DashboardSpecialValuesInternal.IsDashboardSpecialValue(endValue)) {
							RangeFilterSelection defaultSelection = GetDefaultRangeSelection();
							if (defaultSelection != null)
								SetRange(defaultSelection.Minimum, defaultSelection.Maximum);
							return;
						}
					}
				}
			}
			startValue = null;
			endValue = null;
		}
		#endregion
		#region IMasterFilterController
		bool IMasterFilterController.CanSetMasterFilter { get { return Argument != null; } }
		bool IMasterFilterController.CanClearMasterFilter { 
			get {
				Dictionary<Dimension, MasterFilterRange> ranges = MasterFilterRanges;
				return ranges != null && !ranges.First().Value.IsEmpty; 
			}
		}
		bool IMasterFilterController.IsClearMasterFilterSupported { get { return true; } }
		IList<string> IMasterFilterController.AffectedItems { get { return dashboardItem.GetAffectedDashboardItemsByMasterFilterActions(); } }
		bool IMasterFilterController.IsMultipleMasterFilterEnabled { get { return dashboardItem.MultipleMasterFilterEnabled; } }
		bool IMasterFilterController.IsSingleMasterFilterEnabled { get { return dashboardItem.SingleMasterFilterEnabled; } }
		IMasterFilterStateValidator IMasterFilterController.Validator { get { return null; } }
		IMasterFilterDataPreparer IMasterFilterController.DataPreparer { get { return null; } }
		void IMasterFilterController.SetMasterFilter(IValuesSet valueSetList) {
			if (valueSetList != null && valueSetList.ColumnByIndex(0).Count() == 1 && valueSetList.ColumnsCount == 2)
				SetRange(valueSetList.RowByIndex(0)[0], valueSetList.RowByIndex(0)[1]);
		}
		void IMasterFilterController.ClearMasterFilter() {
			ClearRange();
		}
		void IMasterFilterController.PrepareMasterFilterState(DashboardItemState state) {
			Dictionary<Dimension, MasterFilterRange> masterFilterRanges = MasterFilterRanges;
			if (masterFilterRanges != null && masterFilterRanges.Count > 0 && !masterFilterRanges.Single().Value.IsEmpty)
				state.RangeFilterState = new RangeFilterDashboardItemState(MinValue, MaxValue);
		}
		void IMasterFilterController.SetMasterFilterState(DashboardItemState state) {
			RangeFilterDashboardItemState rangeState = state.RangeFilterState;
			if (rangeState != null)
				SetRange(rangeState.MinValue, rangeState.MaxValue);
			else
				ClearRange();
		}
		IEnumerable<DimensionFilterValues> IMasterFilterController.GetFilterValues(IDataSourceInfoProvider dataSourceInfoPrvider, IDataSessionProvider sessionProvider) {
			return GetDisplayFilterValues();
		}
		#endregion
		#region IMasterFilterParameters
		DimensionValueSet IMasterFilterParameters.Values { get { return null; } }
		Dictionary<Dimension, MasterFilterRange> IMasterFilterParameters.Ranges { get { return MasterFilterRanges; } }
		bool IMasterFilterParameters.IsExcludingAllFilter { get { return false; } }
		bool IMasterFilterParameters.EmptyCriteria { get { return false; } }
		#endregion
		#region IInteractivitySession
		ISelectionController IInteractivitySession.SelectionController { get { return this; } }
		IMasterFilterController IInteractivitySession.MasterFilterController { get { return this; } }
		IDrillDownController IInteractivitySession.DrillDownController { get { return null; } }
		ISingleFilterDefaultValueInquirer IInteractivitySession.SingleFilterDefaultValueInquirer { get { return null; } }
		IFilterElementDefaultValuesInquirer IInteractivitySession.FilterElementDefaultValuesInquirer { get { return null; } }
		IRangeFilterDefaultValueInquirer IInteractivitySession.RangeFilterDefaultValueInquirer { get { return this; } }
		#endregion
		#region IRangeInteractivitySession
		RangeFilterSelection IRangeInteractivitySession.GetEntireRange() {
			return new RangeFilterSelection(startValue, endValue);
		}
		#endregion
		#region IRangeFilterDefaultValueInquirer
		event EventHandler<RangeFilterDefaultValueEventArgs> IRangeFilterDefaultValueInquirer.RangeFilterDefaultValue {
			add { RangeFilterDefaultValue += value; }
			remove { RangeFilterDefaultValue -= value; }
		}
		#endregion
	}
}
