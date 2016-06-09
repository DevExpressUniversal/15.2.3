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
using DevExpress.DashboardCommon.Native;
namespace DevExpress.DashboardCommon.DataProcessing {
	public class UnderlyingDataQuery<TSliceQuery> {
		readonly TSliceQuery sliceQuery;
		readonly IList<string> dataMembers;
		readonly IEnumerable<KeyValuePair<DimensionModel, object>> rowValues;
		readonly IEnumerable<KeyValuePair<DimensionModel, object>> columnValues;
		public TSliceQuery SliceQuery { get { return sliceQuery; } }
		public IList<string> DataMembers { get { return dataMembers; } }
		public IEnumerable<KeyValuePair<DimensionModel, object>> RowValues { get { return rowValues; } }
		public IEnumerable<KeyValuePair<DimensionModel, object>> ColumnValues { get { return columnValues; } }
		public UnderlyingDataQuery(TSliceQuery sliceQuery, IList<string> dataMembers, IEnumerable<KeyValuePair<DimensionModel, object>> rowValues, IEnumerable<KeyValuePair<DimensionModel, object>> columnValues) {
			this.sliceQuery = sliceQuery;
			this.dataMembers = dataMembers;
			this.rowValues = rowValues;
			this.columnValues = columnValues;
		}
	}
	public class UnderlyingSliceDataQuery : UnderlyingDataQuery<SliceDataQuery> {
		public static UnderlyingDataQuery<SliceDataQuery> CreateUnderlyingDataQuery(DataDashboardItem item, SliceDataQuery query, IActualParametersProvider provider, IList columnValues, IList rowValues, IList<string> columnNames) {
			ClientHierarchicalMetadata metaData = new ClientHierarchicalMetadata(item.GetMetadata(provider));
			int columnValuesCount = 0;
			DimensionDescriptorInternalCollection columns = null;
			if(!string.IsNullOrEmpty(metaData.ColumnHierarchy) && metaData.DimensionDescriptors.TryGetValue(metaData.ColumnHierarchy, out columns))
				columnValuesCount = Math.Min(columns.Count, columnValues == null ? 0 : columnValues.Count);
			int rowValuesCount = 0;
			DimensionDescriptorInternalCollection rows = null;
			if(!string.IsNullOrEmpty(metaData.RowHierarchy) && metaData.DimensionDescriptors.TryGetValue(metaData.RowHierarchy, out rows))
				rowValuesCount = Math.Min(rows.Count, rowValues == null ? 0 : rowValues.Count);
			IList<string> requiredDataMembers;
			if(columnNames == null || columnNames.Count == 0)
				requiredDataMembers = item.GetDataMembers();
			else
				requiredDataMembers = columnNames;
			requiredDataMembers = requiredDataMembers.Distinct().ToList();
			return new UnderlyingSliceDataQuery(query,
							   rowValuesCount == 0 ? null : rowValues.Cast<object>().Take(rowValuesCount).Zip(rows.Take(rowValuesCount), (a, b) => new KeyValuePair<DimensionModel, object>(GetDimensionModelFromSlice(item, query, b.ID), a)),
							   columnValuesCount == 0 ? null : columnValues.Cast<object>().Take(columnValuesCount).Zip(columns.Take(columnValuesCount), (a, b) => new KeyValuePair<DimensionModel, object>(GetDimensionModelFromSlice(item, query, b.ID), a)),
							   requiredDataMembers);
		}
		static DimensionModel GetDimensionModelFromSlice(DataDashboardItem item, SliceDataQuery query, string id) {
			Dimension dimension = item.DataItemRepository.FindByUniqueName<Dimension>(id);
			foreach(SliceModel slice in query.DataSlices) {
				DimensionModel model = slice.Dimensions.FirstOrDefault(d => d.Name == id || d.DrillDownName == id);
				if(model != null)
					return model;
			}
			return null;
		}
		public UnderlyingSliceDataQuery(SliceDataQuery sliceQuery,
							 IEnumerable<KeyValuePair<DimensionModel, object>> rowValues,
							 IEnumerable<KeyValuePair<DimensionModel, object>> columnValues,
							 IList<string> dataMembers)
			: base(sliceQuery, dataMembers, rowValues, columnValues) {
		}
	}
}
