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

using DevExpress.DashboardCommon.Native;
using System;
using System.Linq;
using System.Collections.Generic;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewerData;
namespace DevExpress.DashboardCommon.Server {
	public class GeoPointMapInteractivitySession : InteractivitySession, IMasterFilterDataPreparer {
		GeoPointMapDashboardItemBase MapItem { get { return (GeoPointMapDashboardItemBase)DashboardItem; } }
		public GeoPointMapInteractivitySession(GeoPointMapDashboardItemBase item) :
			base(item) {
		}
		protected override IValuesSet SelectedValuesInternal {
			get {
				return MapItem.PrepareSelectedValues(base.SelectedValuesInternal);
			}
		}
		protected override IMasterFilterDataPreparer PreparerInternal { get { return this; } }
		protected override Dictionary<Dimension, MasterFilterRange> GetRanges() {
			DimensionValueSet masterFilterValues = MasterFilterValues;
			if(!DashboardItem.IsOlapDataSource && masterFilterValues.Count > 1) {
				Dictionary<Dimension, MasterFilterRange> ranges = new Dictionary<Dimension, MasterFilterRange>();
				foreach (Dimension dimension in masterFilterValues.Dimensions) {
					double min = Double.MaxValue;
					double max = Double.MinValue;
					for (int i = 0; i < masterFilterValues.Count; i++) {
						try {
							double value = Convert.ToDouble(masterFilterValues.GetValue(dimension, i));
							if(value < min)
								min = value;
							if(value > max)
								max = value;
						}
						catch { }
					}
					if(min != Double.MaxValue && max != Double.MinValue)
						ranges.Add(dimension, new MasterFilterRange() {
							Left = min,
							Right = max
						});
				}
				return ranges.Count > 0 ? ranges : null;
			}
			return null;
		}
		protected override IValuesSet PrepareBeforeSetMasterFilter(IValuesSet valueSet) {
			return MapItem.PrepareBeforeSetMasterFilter(valueSet);
		}
		protected override IEnumerable<DimensionFilterValues> GetDisplayFilterValuesInternal(IDataSourceInfoProvider dataSourceInfoPrvider, IDataSessionProvider sessionProvider) { 
			Dictionary<Dimension, DimensionFilterValues> values = new Dictionary<Dimension, DimensionFilterValues>();
			IMasterFilterController masterFilterController = (IMasterFilterController)this;
			List<Tuple<object, object>> selectedValues = new List<Tuple<object, object>>();
			DimensionValueSet valueSet = masterFilterController.Values;
			GeoPointMapDashboardItemBase mapItem = MapItem;
			Dimension latitude = mapItem.Latitude;
			Dimension longitude = mapItem.Longitude;
			DataStorage storage = GetDataStorage(dataSourceInfoPrvider, sessionProvider);
			if(storage == null || (valueSet == null || valueSet.Dimensions.Count != 2 || !valueSet.Dimensions.Contains(latitude) || !valueSet.Dimensions.Contains(longitude)))
				return values.Values;
			for(int valueIndex = 0; valueIndex < valueSet.Count; valueIndex++)
				selectedValues.Add(new Tuple<object, object>(valueSet.GetValue(latitude, valueIndex), valueSet.GetValue(longitude, valueIndex)));
			if(selectedValues.Count == 0)
				return values.Values;
			IEnumerable<Dimension> tooltipDimensions = mapItem.TooltipDimensions;
			IList<Dimension> sliceDimensions = new List<Dimension> { latitude, longitude };
			sliceDimensions.AddRange(tooltipDimensions);
			StorageSlice storageSlice = storage.FirstOrDefault(slice => sliceDimensions.All(dim => slice.KeyColumns.Select(col => col.Name).Contains(dim.ActualId)));
			if(storageSlice == null)
				return values.Values;
			StorageColumn latitudeColumn = storageSlice.KeyColumns.First(col => col.Name == latitude.ActualId);
			StorageColumn longitudeColumn = storageSlice.KeyColumns.First(col => col.Name == longitude.ActualId);
			foreach(StorageRow row in storageSlice) {
				for(int valueIndex = 0; valueIndex < selectedValues.Count; valueIndex++) {
					object latitudeValue = selectedValues[valueIndex].Item1;
					object longitudeValue = selectedValues[valueIndex].Item2;
					object latitudeFilterValue = row[latitudeColumn].MaterializedValue;
					object longitudeFilterValue = row[longitudeColumn].MaterializedValue;
					if((latitudeFilterValue.Equals(latitudeValue)) && (longitudeFilterValue.Equals(longitudeValue))) {
						foreach(Dimension dim in tooltipDimensions) {
							DimensionFilterValues dimValues;
							if(!values.TryGetValue(dim, out dimValues)) {
								dimValues = new DimensionFilterValues();
								dimValues.Name = dim.DisplayName;
								dimValues.Values = new List<FormattableValue>();
								values[dim] = dimValues;
							}
							StorageColumn tooltipColumn = storageSlice.KeyColumns.First(col => col.Name == dim.ActualId);
							dimValues.Values.Add(GetMasterFilterValue(dim, row[tooltipColumn].MaterializedValue));
						}
					}
				}
			}
			return values.Values;
		}
		DataStorage GetDataStorage(IDataSourceInfoProvider dataSourceInfoPrvider, IDataSessionProvider sessionProvider) {
			DataStorage dataStorage = null;
			DataSourceInfo dataInfo = dataSourceInfoPrvider.GetDataSourceInfo(DashboardItem.DataSourceName, DashboardItem.DataMember);
			if(dataInfo != null) {
				IDataSession dataSession = sessionProvider.GetDataSession(DashboardItem.ComponentName, dataInfo, DashboardItem.FakeDataSource);
				if(dataSession != null) {
					HierarchicalDataParams hierarchicalData = dataSession.GetData(((ISliceDataQueryProvider)DashboardItem).GetDataQuery(sessionProvider)).HierarchicalData;
					if(hierarchicalData != null) {
						dataStorage = hierarchicalData.Storage;
					}
				}
			}
			return dataStorage;
		}
		void IMasterFilterDataPreparer.PrepareData(IDataSourceInfoProvider dataInfoProvider, IDataSessionProvider dataSessionProvider, IActualParametersProvider parametersProvider) {
			if(MapItem.EnableClustering && !MapItem.HasClusteredResult) {
				DataSourceInfo dataInfo = dataInfoProvider.GetDataSourceInfo(DashboardItem.DataSourceName, DashboardItem.DataMember);
				if(dataInfo != null) {
					IDataSession dataSession = dataSessionProvider.GetDataSession(DashboardItem.ComponentName, dataInfo, DashboardItem.FakeDataSource);
					if(dataSession != null) {
						ISliceDataQueryProvider queryProvider = DashboardItem;
						SliceDataQuery sliceDataQuery = queryProvider.GetDataQuery(parametersProvider);
						DataQueryResult queryResult = dataSession.GetData(sliceDataQuery);
						queryProvider.PrepareData(queryResult, sliceDataQuery, parametersProvider, null, null);
					}
				}
			}
		}
	}
}
