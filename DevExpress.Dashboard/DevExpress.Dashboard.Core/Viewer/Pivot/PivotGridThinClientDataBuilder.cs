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

using System.Collections.Generic;
using System.Linq;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.PivotGrid.Internal.ThinClientDataSource;
namespace DevExpress.DashboardCommon.Viewer {
	public class ThinClientFieldValueItemsPairReferenceKey : PairReferenceKey<ThinClientFieldValueItem, ThinClientFieldValueItem> {
		public ThinClientFieldValueItemsPairReferenceKey(ThinClientFieldValueItem first, ThinClientFieldValueItem second) : base(first, second) { }
	}
	public class PivotGridThinClientDataCollection {
		Dictionary<int, ThinClientValueItem> data;
		public ThinClientValueItem this[int index] { get { return data[index]; } set { data[index] = value; } }
		public Dictionary<int, ThinClientValueItem> Data { get { return data; } }
		public PivotGridThinClientDataCollection() {
			data = new Dictionary<int, ThinClientValueItem>();
		}
	}
	public struct ConditionalFormattingAxisPointCacheKey {
		readonly string formatConditionalMeasureId;
		readonly AxisPoint slicePoint;
		public AxisPoint SlicePoint { get { return slicePoint; } }
		public string FormatConditionalMeasureId { get { return formatConditionalMeasureId; } }
		public ConditionalFormattingAxisPointCacheKey(AxisPoint slicePoint, string formatConditionalMeasureId) {
			this.slicePoint = slicePoint;
			this.formatConditionalMeasureId = formatConditionalMeasureId;
		}
		public override int GetHashCode() {
			return SlicePoint.GetHashCode();
		}
		public override bool Equals(object obj) {
			ConditionalFormattingAxisPointCacheKey key = (ConditionalFormattingAxisPointCacheKey)obj;
			if(FormatConditionalMeasureId != key.FormatConditionalMeasureId || !SlicePoint.Equals(key.SlicePoint))
				return false;
			return true;
		}
	}
	public class PivotGridThinClientDataBuilder {
		readonly MultiDimensionalData mdData;
		readonly PivotFormatConditionalStyleSettingsProvider styleSettingsProvider;
		Dictionary<int, ThinClientFieldValueItem> columnValuesByIndex = new Dictionary<int, ThinClientFieldValueItem>();
		Dictionary<int, ThinClientFieldValueItem> rowValuesByIndex = new Dictionary<int, ThinClientFieldValueItem>();
		Dictionary<int, List<object>> columnValues = new Dictionary<int, List<object>>();
		Dictionary<int, List<object>> rowValues = new Dictionary<int, List<object>>();
		List<ThinClientFieldValueItem> columnsHierarchy = new List<ThinClientFieldValueItem>();
		List<ThinClientFieldValueItem> rowsHierarchy = new List<ThinClientFieldValueItem>();
		Dictionary<ThinClientFieldValueItemsPairReferenceKey, PivotGridThinClientDataCollection> data = new Dictionary<ThinClientFieldValueItemsPairReferenceKey, PivotGridThinClientDataCollection>();
		public PivotGridThinClientDataBuilder(MultiDimensionalData mdData, string[] measureIds, ConditionalFormattingModel cfModel)
			: this(mdData, null, null, measureIds, cfModel) {
		}
		public PivotGridThinClientDataBuilder(MultiDimensionalData mdData, AxisPoint columnAxisPoint, AxisPoint rowAxisPoint, string[] measureIds, ConditionalFormattingModel cfModel) {
			this.mdData = mdData;
			Initialize(columnAxisPoint, rowAxisPoint, measureIds);
			this.styleSettingsProvider = new PivotFormatConditionalStyleSettingsProvider(cfModel, mdData);
		}
		public Dictionary<ThinClientFieldValueItemsPairReferenceKey, PivotGridThinClientDataCollection> GetData() {
			return data;
		}
		public PivotGridThinClientData GetClientData() {
			PivotGridThinClientData clientData = new PivotGridThinClientData(columnsHierarchy, rowsHierarchy);
			foreach(KeyValuePair<ThinClientFieldValueItemsPairReferenceKey, PivotGridThinClientDataCollection> pair in data) {
				foreach(KeyValuePair<int, ThinClientValueItem> subPair in pair.Value.Data)
					clientData.AddCell(pair.Key.First, pair.Key.Second, subPair.Key, subPair.Value);
			}
			return clientData;
		}
		public List<ThinClientFieldValueItem> GetExpandHierarchy(bool isColumn) {
			return isColumn ? columnsHierarchy : rowsHierarchy;
		}
		public Dictionary<ThinClientFieldValueItem, object[]> GetValues(bool isColumn) {
			Dictionary<int, ThinClientFieldValueItem> valuesByIndex = isColumn ? rowValuesByIndex : columnValuesByIndex;
			return (isColumn ? rowValues : columnValues).ToDictionary(val => valuesByIndex[val.Key], val => val.Value.ToArray());
		}
		public void FillStyleSettings(PivotCustomDrawCellEventArgsBase args, AxisPoint columnPoint, AxisPoint rowPoint, string dataId) {
			styleSettingsProvider.FillStyleSettings(columnPoint, rowPoint, args, dataId);
		}
		public void OnCollapseStateChanged(bool isColumn, object[] values, bool collapse) {
			styleSettingsProvider.UpdateCollapseStateCache(isColumn, values, collapse);
		}
		public AxisPoint CorrectAxisPoint(AxisPoint point) { 
			return point != null ? mdData.GetAxisPointByUniqueValues(point.AxisName, point.RootPath.Select(p => p.UniqueValue).ToArray()) : null;
		}
		void Initialize(AxisPoint columnAxisPoint, AxisPoint rowAxisPoint, string[] measureIds) {
			if(mdData.IsEmpty)
				return;
			Dictionary<string, MeasureDescriptor> measuresRepository = new Dictionary<string, MeasureDescriptor>();
			foreach(string measureId in measureIds) {
				MeasureDescriptor measure = mdData.GetMeasureDescriptorByID(measureId);
				if(measure != null)
					measuresRepository.Add(measureId, measure);
			}
			if(columnAxisPoint == null)
				columnAxisPoint = mdData.GetAxisRoot(DashboardDataAxisNames.PivotColumnAxis);
			if(rowAxisPoint == null)
				rowAxisPoint = mdData.GetAxisRoot(DashboardDataAxisNames.PivotRowAxis);
			columnsHierarchy = GetFieldValueHierarchy(columnAxisPoint, columnValuesByIndex, columnValues);
			rowsHierarchy = GetFieldValueHierarchy(rowAxisPoint, rowValuesByIndex, rowValues);
			IList<AxisPoint> allColumnPoints = columnAxisPoint.GetAllAxisPoints();
			IList<AxisPoint> allRowPoints = rowAxisPoint.GetAllAxisPoints();
			int columnGrandTotalIndex = columnAxisPoint.Index;
			int rowGrandTotalIndex = rowAxisPoint.Index;
			foreach(AxisPoint columnPoint in allColumnPoints) {
				int columnIndex = columnPoint.Index;
				bool isColumnGrandTotal = columnIndex == columnGrandTotalIndex;
				ThinClientFieldValueItem column = isColumnGrandTotal ? null : columnValuesByIndex[columnIndex];
				foreach(AxisPoint rowPoint in allRowPoints) {
					int rowIndex = rowPoint.Index;
					bool isRowGrandTotal = rowIndex == rowGrandTotalIndex;
					ThinClientFieldValueItem row = isRowGrandTotal ? null : rowValuesByIndex[rowIndex];
					ThinClientFieldValueItemsPairReferenceKey key = new ThinClientFieldValueItemsPairReferenceKey(column, row);
					PivotGridThinClientDataCollection cellSet = new PivotGridThinClientDataCollection();
					int index = 0;
					foreach(string measureId in measureIds) {
						MeasureDescriptor measure;
						if(measuresRepository.TryGetValue(measureId, out measure)) {
							MeasureValue measureValue = mdData.GetValue(columnPoint, rowPoint, measure);
							cellSet[index] = new ThinClientValueItem(measureValue.Value);
						}
						index++;
					}
					data[key] = cellSet;
				}
			}
		}
		List<ThinClientFieldValueItem> GetFieldValueHierarchy(AxisPoint axisPoint, Dictionary<int, ThinClientFieldValueItem> fieldValuesByIndex, Dictionary<int, List<object>> values) {
			List<ThinClientFieldValueItem> result = new List<ThinClientFieldValueItem>();
			Dictionary<int, int> parentIndexes = new Dictionary<int, int>();
			foreach(AxisPoint point in axisPoint.GetAllAxisPoints()) {
				if(point.Index != axisPoint.Index) {
					int index = point.Index;
					fieldValuesByIndex[index] = new ThinClientFieldValueItem(new ThinClientValueItem(point.UniqueValue, point.DisplayText) { Tag = point });
					if(point.Parent.Index > axisPoint.Index)
						parentIndexes[index] = point.Parent.Index;
				}
			}
			foreach(KeyValuePair<int, ThinClientFieldValueItem> columnPair in fieldValuesByIndex) {
				int parentIndex;
				values.Add(columnPair.Key, new List<object> { columnPair.Value.Value.Value });
				if(parentIndexes.TryGetValue(columnPair.Key, out parentIndex)) {
					ThinClientFieldValueItem parentFieldItem = fieldValuesByIndex[parentIndex];
					if(parentFieldItem.Children == null)
						parentFieldItem.Children = new List<ThinClientFieldValueItem>();
					parentFieldItem.Children.Add(columnPair.Value);
					values[columnPair.Key].Add(parentFieldItem.Value.Value);
					while(parentIndexes.TryGetValue(parentIndex, out parentIndex))
						values[columnPair.Key].Add(fieldValuesByIndex[parentIndex].Value.Value);
					values[columnPair.Key].Reverse();
				}
				else
					result.Add(columnPair.Value);
			}
			return result;
		}
	}
}
