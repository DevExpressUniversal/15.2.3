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
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Utils;
namespace DevExpress.DashboardCommon.Server {
	public abstract class InteractivitySessionBase<TDashboardItem> where TDashboardItem : DataDashboardItem {
		protected TDashboardItem DashboardItem { get; private set; }
		protected bool IsMasterFilterEnabled { get { return DashboardItem.IsMasterFilterEnabled; } }
		protected bool IsDrillDownEnabled { get { return DashboardItem.IsDrillDownEnabled; } }
		protected IList<Dimension> InteractivityDimensions { get { return DashboardItem.SelectionDimensionList; } }
		protected virtual IList<Dimension> ActualInteractivityDimensions { get { return InteractivityDimensions; } }
		protected abstract bool AllowEmptyFilterValues { get; }
		protected abstract IMasterFilterParameters MasterFilterParameters { get; }
		protected abstract Dimension CurrentDrillDownDimension { get; }
		protected virtual IValuesSet SelectedValuesInternal {
			get {
				IList<Dimension> dimensions = new List<Dimension>();
				if (IsDrillDownEnabled && CurrentDrillDownDimension != null)
					dimensions = new List<Dimension> { CurrentDrillDownDimension };
				else if (IsMasterFilterEnabled)
					dimensions = MasterFilterParameters.Values.Dimensions;
				IEnumerable<ISelectionRow> rows = MasterFilterParameters.Values.GetValuesSet(dimensions);
				IEnumerable<ISelectionRow> convertedRows = rows
					.Select(row => row.Select(DashboardSpecialValuesInternal.ToSpecialUniqueValue))
					.Select(row => row.AsSelectionRow());
				return convertedRows.AsValuesSet();
			}
		}
		protected IList<string> AffectedItems { get { return DashboardItem.GetAffectedDashboardItemsByMasterFilterActions(); } }
		protected bool IsMultipleMasterFilterEnabled { get { return DashboardItem.MultipleMasterFilterEnabled; } }
		protected bool IsSingleMasterFilterEnabled { get { return DashboardItem.SingleMasterFilterEnabled; } }
		protected InteractivitySessionBase(TDashboardItem dashboardItem) {
			Guard.ArgumentNotNull(dashboardItem, "dashboardItem");
			DashboardItem = dashboardItem;
		}
		protected IValuesSet GetActualRows(IValuesSet valueSetList) {
			if (valueSetList.IsEmpty && !AllowEmptyFilterValues)
				throw new ArgumentException(DashboardLocalizer.GetString(DashboardStringId.MessageMasterFilterIncorrectNumberOfValues));
			if (IsMasterFilterEnabled) {
				IList<Dimension> dimensions = ActualInteractivityDimensions;
				return valueSetList
					.AsSelectionRows()
					.Select(row => row.Select((value, i) => DashboardItem.GetActualValue(dimensions[i], value)))
					.Select(x => x.AsSelectionRow(dimensions.Count)).AsValuesSet();
			}
			return null;
		}
		protected DashboardDataSet GetAvailableSelections(IValuesSet allSelectionValues, MultiDimensionalData data) {
			if (IsDrillDownEnabled || IsMasterFilterEnabled)
				return GetSelectionValuesCore(allSelectionValues, data);
			return null;
		}
		protected FormattableValue GetMasterFilterValue(Dimension dimension, object value) {
			IDashboardDataSource dataSource = DashboardItem.DataSource;
			return new FormattableValue {
				Value = dataSource.GetOlapDimensionValueDisplayText(dimension.DataMember, value, DashboardItem.DataMember),
				Type = MasterFilterValueType.Value,
				Format = dataSource != null && !dataSource.GetIsSpecificValueFormatSupported() ? new ValueFormatViewModel { DataType = ValueDataType.String } : dimension.CreateValueFormatViewModel()
			};
		}
		protected virtual IEnumerable<DimensionFilterValues> GetDisplayFilterValuesInternal(IDataSourceInfoProvider dataSourceInfoPrvider, IDataSessionProvider sessionProvider) {
			List<DimensionFilterValues> values = new List<DimensionFilterValues>();
			IMasterFilterController controller = (IMasterFilterController)this;
			DimensionValueSet valueSet = controller.Values;
			if(valueSet != null && valueSet.Dimensions.Count > 0) {
				foreach(Dimension dimension in valueSet.Dimensions) {
					IList<FormattableValue> formattableValues = new List<FormattableValue>();
					for(int j = 0; j < valueSet.Count; j++) {
						object value = valueSet.GetValue(dimension, j);
						if(!DashboardSpecialValues.IsOlapNullValue(value)) {
							formattableValues.Add(GetMasterFilterValue(dimension, value));
						}
					}
					values.Add(GetDimensionFilterValues(dimension.DisplayName, formattableValues));
				}
			}
			return values;
		}
		protected DimensionFilterValues GetDimensionFilterValues(string dimensionDisplayName, IEnumerable<FormattableValue> formatableValues) {
			DimensionFilterValues filterValues = new DimensionFilterValues();
			filterValues.Values = new List<FormattableValue>();
			filterValues.Name = dimensionDisplayName;
			filterValues.Truncated = false;
			filterValues.Values.AddRange(formatableValues);
			return filterValues;
		}
		DashboardDataSet GetSelectionValuesCore(IValuesSet allSelectionValues, MultiDimensionalData data) {
			DashboardDataSetInternal dataSetInternal = new DashboardDataSetInternal(DashboardItem.GetSelectionDataMemberDisplayNames());
			List<Dimension> interactivityDimensions = CurrentDrillDownDimension != null ?
				new List<Dimension> { CurrentDrillDownDimension } :
				new List<Dimension>(InteractivityDimensions);
			if (InteractivityDimensions.Count > 0) {
				IValuesSet valuesSet = allSelectionValues ?? Helper.GetAllSelectionValues(data, interactivityDimensions);
				foreach (ISelectionRow row in valuesSet.AsSelectionRows())
					dataSetInternal.AddRow(new DashboardDataRowInternal(dataSetInternal, row.ToList()));
			}
			return new DashboardDataSet(dataSetInternal);
		}
	}
}
