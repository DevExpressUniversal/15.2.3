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
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data.Browsing;
using DevExpress.Data.Filtering;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.PivotGrid.ServerMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Data;
namespace DevExpress.DashboardCommon.Native {
	public static class DataSourceHelper {
		public static bool ContainsDisplayMember(IDataSourceSchema dataSourceSchema, string displayMember, string valueMember) {
			return !string.IsNullOrEmpty(displayMember) && valueMember != displayMember && dataSourceSchema.GetField(displayMember) != null;
		}
		public static bool IsNumericMeasure(IDashboardDataSource dataSource, string dataMember, Measure measure) {
			if(dataSource == null || measure == null)
				return false;
			if(dataSource.GetIsOlap())
				return true;
			if(measure.SummaryType == SummaryType.Max || measure.SummaryType == SummaryType.Min)
				return DataBindingHelper.IsNumericType(dataSource.GetFieldSourceType(measure.DataMember, dataMember));
			return true;
		}
		public static DataFieldType GetApproximatedDataType(IDashboardDataSource dataSource, string dataMember, Measure measure) {
			if(measure == null || dataSource == null || (measure.SummaryType != SummaryType.Max && measure.SummaryType != SummaryType.Min))
				return DataFieldType.Unknown;
			return DataBindingHelper.GetDataFieldType(dataSource.GetFieldSourceType(measure.DataMember, dataMember));
		}
		public static DataFieldType GetMeasureDataType(IDashboardDataSource dataSource, string dataMember, Measure measure) {
			Type type;
			if(dataSource == null || measure == null)
				type = typeof(object);
			IPivotGridDataSource ds = dataSource.GetPivotDataSource(dataMember);
			if(ds == null) {
				type = GetSummaryType(measure.SummaryType, () => dataSource.GetFieldSourceType(measure.DataMember, dataMember));
			} else {
				if(dataSource.GetIsOlap())
					type = dataSource.GetFieldSourceType(measure.DataMember, dataMember);
				type = GetSummaryType(measure.SummaryType, () => dataSource.GetFieldSourceType(measure.DataMember, dataMember));
			}
			return DataBindingHelper.GetDataFieldType(type);
		}
		public static Type GetSummaryType(SummaryType summaryType, Func<Type> sourceType) {
			switch(summaryType) {
				case SummaryType.Average:
				case SummaryType.Sum:
					return typeof(decimal);
				case SummaryType.Count:
				case SummaryType.CountDistinct:
					return typeof(int);
				case SummaryType.Max:
				case SummaryType.Min:
					return sourceType();
				case SummaryType.StdDev:
				case SummaryType.StdDevp:
				case SummaryType.Var:
				case SummaryType.Varp:
					return typeof(double);
				default:
					throw new Exception("Unknown summary type.");
			}
		}
		public static DataFieldType GetDimensionDataType(IDashboardDataSource dataSource, string dataMember, Dimension dimension) {
			return DataBindingHelper.GetDataFieldType(GetDimensionType(dataSource, dataMember, dimension));
		}
		public static Type GetDimensionType(IDashboardDataSource dataSource, string dataMember, Dimension dimension) {
			if(dimension == null || dataSource == null)
				return typeof(object);
			IDataSourceSchema schema = dataSource.GetDataSourceSchema(dataMember);
			if(schema == null)
				return typeof(object);
			Type dataFieldType = schema.GetFieldSourceType(dimension.DataMember);
			GroupIntervalInfo info = GroupIntervalInfo.GetInstance(dimension, DataBindingHelper.GetDataFieldType(dataFieldType));
			Type type = (info == null || info.DataType == null) ? GetActualType(dimension, info, dataFieldType) : info.DataType;
			if(dimension.TopNOptions.ActualShowOthers && type != typeof(string))
				type = typeof(object);
			return type;
		}
		public static DataFieldType GetActualDataType(Dimension dimension, GroupIntervalInfo info, Type dataFieldType) {
			return DataBindingHelper.GetDataFieldType(GetActualType(dimension, info, dataFieldType));
		}
		static Type GetActualType(Dimension dimension, GroupIntervalInfo info, Type dataFieldType) {
			if(info != null) {
				if(info.PivotGroupInterval == PivotGroupInterval.DateDayOfWeek)
					return typeof(int);
				PivotGroupInterval interval = info.PivotGroupInterval;
				if(interval != PivotGroupInterval.Default) {
					if(interval == PivotGroupInterval.Alphabetical)
						return typeof(string);
					if(
						interval == PivotGroupInterval.Date ||
						interval == PivotGroupInterval.DateMonthYear ||
						interval == PivotGroupInterval.DateQuarterYear ||
						interval == PivotGroupInterval.DateHour ||
						interval == PivotGroupInterval.DateHourMinute ||
						interval == PivotGroupInterval.DateHourMinuteSecond
					)
						return typeof(DateTime);
					if(interval == PivotGroupInterval.Custom || interval == PivotGroupInterval.DateDayOfWeek)
						return typeof(object);
					return typeof(int);
				}
			}
			return dataFieldType;
		}
		public static IEnumerable<object> GetUniqueValues(IDashboardDataSource dataSource, Dimension dimension, string dataMember, IActualParametersProvider provider) {
			if(dataSource == null || dimension == null || string.IsNullOrEmpty(dimension.DataMember))
				return null;
			if(dataSource.GetIsOlap()) {
				try {
					PivotGridData data = new PivotGridData();
					PivotGridFieldBase field = new PivotGridFieldBase(dimension.DataMember, PivotArea.FilterArea);
					SortingOptions actualSortingOptions = dimension.GetActualSortingOptions();
					DimensionSortOrder actualSortOrder = actualSortingOptions.SortOrder;
					if(actualSortOrder == DimensionSortOrder.None) {
						field.SortMode = PivotSortMode.None;
					} else {
						DimensionSortMode? actualSortMode = actualSortingOptions.SortMode;
						field.SortMode = actualSortMode.HasValue && actualSortMode != DimensionSortMode.Value ? (PivotSortMode)actualSortMode : PivotSortMode.Default;
						field.SortOrder = (PivotSortOrder)actualSortOrder;
					}
					data.Fields.Add(field);
					data.PivotDataSource = dataSource.GetPivotDataSource(dataMember);
					return data.GetSortedUniqueValues(field);
				} catch { } 
			}
			if(!dataSource.GetShouldProvideFakeData() && dataSource.IsConnected) {
				DataSourceInfo dataSourceInfo = new DataSourceInfo(dataSource, dataMember);
				DataSourceModel model = new DataSourceModel(dataSourceInfo, dataSource.GetListSource(dataMember), dataSource.GetPivotDataSource(dataMember));
				IDataSession session = DataSessionFactory.Default.RequestSession(model);
				int counter = 0;
				Dictionary<DataItem, string> ids = new Dictionary<DataItem, string>();
				ItemModelBuilder builder = new ItemModelBuilder(dataSourceInfo, (a) => {
					string name;
					if(ids.TryGetValue(a, out name))
						return name;
					do
						name = "Dim" + counter++;
					while(dimension.DataMember == name || ids.ContainsValue(name));
					ids[a] = name;
					return name;
				}, provider);
				SliceDataQueryBuilder queryBuilder = SliceDataQueryBuilder.CreateEmpty(builder, new Dimension[0], null);
				IEnumerable<Dimension> dims = new Dimension[] { dimension };
				queryBuilder.AddSlice(dims, new Measure[0]);
				queryBuilder.SetAxes(dims, new Dimension[0]);
				DataStorage storage = session.GetData(queryBuilder.FinalQuery()).HierarchicalData.Storage;
				StorageColumn column = storage.GetColumn(ids[dimension]);
				return storage.First().Select((row) => row[column].MaterializedValue).ToList();
			}
			return null;
		}
		public static List<ParameterValueViewModel> GetDynamicLookupValues(IDashboardDataSource dataSource, string dataMember, string displayMember, string valueMember) {			
			HashSet<object[]> res = new HashSet<object[]>(Helper.EnumerableObjectComparer);
			if(dataSource.IsConnected) { 
				List<string> dataItems = new List<string>(2);
				if(!string.IsNullOrEmpty(valueMember))
					dataItems.Add(valueMember);
				if(!string.IsNullOrEmpty(displayMember))
					dataItems.Add(displayMember);
				if(dataItems.Count > 0) {
					DataSourceInfo dataSourceInfo = new DataSourceInfo(dataSource, dataMember);
					DataSourceModel model = new DataSourceModel(dataSourceInfo, dataSource.GetListSource(dataMember), dataSource.GetPivotDataSource(dataMember));
					IDataSession session = DataSessionFactory.Default.RequestSession(model);
					string dimensionFieldName = dataItems.First();
					string dimensionName = string.Format("ValueMember_{0}", dimensionFieldName);
					string measureFieldName = null;
					string measureName = null;
					if(dataItems.Count > 1) {
						measureFieldName = dataItems[1];
						measureName = string.Format("DisplayMember_{0}", measureFieldName);
					}
					Dimension dimension = new Dimension(dimensionName, dimensionFieldName);
					Measure measure = null;
					if(!string.IsNullOrEmpty(measureName))
						measure = new Measure(measureName, measureFieldName) { SummaryType = SummaryType.Min };
					ItemModelBuilder builder = new ItemModelBuilder(dataSourceInfo, (a) => {
						return a == dimension ? dimensionName : measureName;
					}, new EmptyParametersProvider());
					SliceDataQueryBuilder queryBuilder = SliceDataQueryBuilder.CreateEmpty(builder, new Dimension[0], null);
					Dimension[] sliceDimensions = new Dimension[] { dimension };
					Measure[] measures;
					if(string.IsNullOrEmpty(measureName))
						measures = new Measure[0];
					else
						measures = new Measure[] { measure };
					queryBuilder.AddSlice(sliceDimensions, measures);
					queryBuilder.SetAxes(sliceDimensions, new Dimension[0]);
					DataStorage storage = session.GetData(queryBuilder.FinalQuery()).HierarchicalData.Storage;
					StorageColumn columnValueMember = storage.GetColumn(dimensionName);
					StorageColumn columnDisplayMember = null;
					if(measureName != null)
						columnDisplayMember = storage.GetColumn(measureName);
					return storage.First().Select((row) => {
						ParameterValueViewModel viewModel = new ParameterValueViewModel() {
							Value = row[columnValueMember].MaterializedValue,
						};
						object displayValue = null;
						if(columnDisplayMember != null)
							displayValue = row[columnDisplayMember].MaterializedValue;
						if(displayValue == null)
							viewModel.DisplayText = string.Empty;
						else
							viewModel.DisplayText = displayValue.ToString();
						return viewModel;
					}).ToList();
				}
			}
			return new List<ParameterValueViewModel>();
		}
		public static IOLAPMember GetOlapDimensionMember(IPivotOLAPDataSource olap, string columnName, string memberName) {
			if(olap != null) {
				IOLAPMember member = olap.GetMemberByUniqueName(columnName, memberName);
				if(member != null && member.Column != null && !member.IsTotal)
					return member;
				else
					return null;
			}
			return null;
		}
	}
}
