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

using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewerData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.DataBinding.HierarchicalData {
	public class LevelInfo {
		public StorageColumn[] SliceColumns { get; set; }
		public StorageColumn LevelColumn { get; set; }
		public StorageSlice Slice { get; set; }
		public int LevelNumber { get; set; }
	}
	public class ServerDimensionValue {
		object value;
		object uniqueValue;
		string displayText;
		internal ServerDimensionValue(object value, object uniqueValue, string displayText) {
			this.value = value;
			this.uniqueValue = uniqueValue;
			this.displayText = displayText;
		}
		public object Value { get { return value; } }
		public object UniqueValue { get { return uniqueValue; } }
		public string DisplayText { get { return displayText; } }
		public override bool Equals(object obj) {
			ServerDimensionValue serverValue = obj as ServerDimensionValue;
			if(serverValue != null)
				return Object.Equals(this.uniqueValue, serverValue.uniqueValue);
			return false;
		}
		public override int GetHashCode() {
			return uniqueValue.GetHashCode();
		}
	}
	public class DataStorageHelper {
		#region internal structs
		struct CacheKey : IEquatable<CacheKey> {
			StorageValue[] values;
			public CacheKey(StorageValue[] values) {
				this.values = values;
			}
			public override int GetHashCode() {
				return HashcodeHelper.GetCompositeHashCode(values);
			}
			public override bool Equals(object obj) {
				throw new NotSupportedException();
			}
			public bool Equals(CacheKey other) {
				if(values.Length != other.values.Length)
					return false;
				for(int i = 0; i < values.Length; i++)
					if(!values[i].Equals(other.values[i]))
						return false;
				return true;
			}
			public override string ToString() {
				StringBuilder builder = new StringBuilder();
				values.ForEach(v => builder.Append(v));
				return builder.ToString();
			}
		}
		struct AxisPointCrossingKey : IEquatable<AxisPointCrossingKey> {
			AxisPoint point1;
			AxisPoint point2;
			string measureId;
			public AxisPointCrossingKey(string measureId, AxisPoint point1, AxisPoint point2) {
				this.measureId = measureId;
				this.point1 = point1;
				this.point2 = point2;
			}
			public override int GetHashCode() {
				return measureId.GetHashCode()
					^ (point1 == null ? 0 : point1.GetInstanceHashCode())
					^ (point2 == null ? 0 : point2.GetInstanceHashCode());
			}
			public override bool Equals(object obj) {
				throw new NotSupportedException();
			}
			public bool Equals(AxisPointCrossingKey other) {
				return ((point1 == other.point1 && point2 == other.point2)
					|| (point2 == other.point1 && point1 == other.point2))
					 && measureId == other.measureId;
			}
		}
		struct SliceLevelKey : IEquatable<SliceLevelKey> {
			readonly StorageColumn column1;
			readonly StorageColumn column2;
			public SliceLevelKey(LevelInfo levelInfo1, LevelInfo levelInfo2) {
				this.column1 = levelInfo1 != null ? levelInfo1.LevelColumn : null;
				this.column2 = levelInfo2 != null ? levelInfo2.LevelColumn : null;
			}
			public override int GetHashCode() {
				return HashcodeHelper.GetCompositeHashCode(column1, column2);
			}
			public override bool Equals(object obj) {
				throw new NotSupportedException();
			}
			public bool Equals(SliceLevelKey other) {
				return (column1 == other.column1) && (column2 == other.column2);
			}
		}
		#endregion
		#region AxisBuilder
		class AxisBuilder {
			readonly DataStorage storage;
			readonly List<SliceDescription> sortOrderSlices;
			readonly StorageColumn valueStorageColumn;
			readonly StorageColumn displayTextStorageColumn;
			readonly Dictionary<string, Dictionary<CacheKey, AxisPoint>> pointsCache;
			readonly Dictionary<string, List<StorageColumn>> axisStorageColumns;
			readonly Dictionary<string, AxisPoint> rootPoints = new Dictionary<string, AxisPoint>();
			readonly IDictionary<string, DimensionDescriptorCollection> axes;
			public Dictionary<AxisPointCrossingKey, object> MeasureValues { get; private set; }
			public Dictionary<string, AxisPoint> RootPoints { get { return rootPoints; } }
			public AxisBuilder(HierarchicalDataParams hDataParams, IDictionary<string, DimensionDescriptorCollection> axes) {
				this.storage = hDataParams.Storage;
				this.valueStorageColumn = hDataParams.Storage.GetColumn(DataStorageGenerator.ValueStorageColumnName);
				this.displayTextStorageColumn = hDataParams.Storage.GetColumn(DataStorageGenerator.DisplayTextStorageColumnName);
				this.sortOrderSlices = hDataParams.SortOrderSlices.Select(signature => new SliceDescription(signature)).ToList();
				this.axisStorageColumns = axes.ToDictionary(a => a.Key, a => a.Value.Select(d => storage.GetColumn(d.ID)).ToList());
				this.MeasureValues = new Dictionary<AxisPointCrossingKey, object>();
				this.rootPoints = new Dictionary<string, AxisPoint>();
				this.axes = axes;
				this.pointsCache = new Dictionary<string, Dictionary<CacheKey, AxisPoint>>();
			}
			public void Initialize() {
				foreach(var axis in axes)
					RootPoints[axis.Key] = GenerateAxis(axis.Key, axis.Value);
				LoadMeasures();
			}
			AxisPoint GenerateAxis(string axisName, DimensionDescriptorCollection dimensions) {
				Dictionary<CacheKey, AxisPoint> cache = new Dictionary<CacheKey, AxisPoint>();
				pointsCache[axisName] = cache;
				List<StorageColumn> axisColumns = axisStorageColumns[axisName];
				List<LevelInfo> levelInfos = axisColumns.Select((column, level) => CreateLevelInfo(axisColumns.Take(level + 1), column, level)).ToList();
				AxisPoint root = new AxisPoint(axisName, null, new ServerDimensionValue(null, null, null), CreateLevelInfo(new StorageColumn[0], null, -1));
				cache.Add(new CacheKey(new StorageValue[0]), root);
				for(int i = 0; i < levelInfos.Count; i++) {
					StorageSlice slice = levelInfos[i].Slice;
					if(slice == null || !IsSortOrderSlice(slice))
						continue;
					int keyCount = slice.KeyColumns.Count();
					foreach(StorageRow row in slice) {
						AxisPoint[] pointHierarchy = new AxisPoint[keyCount];
						for(int level = 0; level < pointHierarchy.Length; level++) {
							AxisPoint point;
							StorageValue[] values = new StorageValue[level + 1];
							for(int colIndex = 0; colIndex <= level; colIndex++)
								values[colIndex] = row[axisColumns[colIndex]];
							if(DashboardSpecialValues.IsOlapNullValue(values[level].MaterializedValue))
								break;
							CacheKey key = new CacheKey(values);
							bool exists = cache.TryGetValue(key, out point);
							if(!exists) {
								StorageColumn column = levelInfos[level].LevelColumn;
								point = new AxisPoint(axisName, dimensions[level], GetServerDimensionValue(column, row[column]), levelInfos[level]);
								cache.Add(key, point);
								AxisPoint parentPoint = level == 0 ? root : pointHierarchy[level - 1];
								parentPoint.ChildItemsInternal.Add(point);
								point.Parent = parentPoint;
							}
							pointHierarchy[level] = point;
						}
					}
				}
				IndexAxis(root);
				return root;
			}
			void LoadMeasures() {
				var axis1PointCache = pointsCache.Keys.Count > 0 ? pointsCache.ElementAt(0).Value : new Dictionary<CacheKey, AxisPoint>();
				var axis2PointCache = pointsCache.Keys.Count > 1 ? pointsCache.ElementAt(1).Value : new Dictionary<CacheKey, AxisPoint>();
				foreach(StorageSlice slice in storage) {
					List<StorageColumn> axis1Columns = slice.KeyColumns.Intersect(axisStorageColumns.Values.Count > 0 ? axisStorageColumns.Values.ElementAt(0) : new List<StorageColumn>()).ToList();
					List<StorageColumn> axis2Columns = slice.KeyColumns.Intersect(axisStorageColumns.Values.Count > 1 ? axisStorageColumns.Values.ElementAt(1) : new List<StorageColumn>()).ToList();
					List<StorageColumn> measureColumns = slice.MeasureColumns.ToList();
					StorageValue[] axis1Values = new StorageValue[axis1Columns.Count];
					StorageValue[] axis2Values = new StorageValue[axis2Columns.Count];
					foreach(StorageRow row in slice) {
						for(int i = 0; i < axis1Columns.Count; i++)
							axis1Values[i] = row[axis1Columns[i]];
						for(int i = 0; i < axis2Columns.Count; i++)
							axis2Values[i] = row[axis2Columns[i]];
						AxisPoint axis1Point = ResolveAxisPoint(TruncateRaggedHierarchyValues(axis1Values), axis1PointCache);
						AxisPoint axis2Point = ResolveAxisPoint(TruncateRaggedHierarchyValues(axis2Values), axis2PointCache);
						foreach(StorageColumn column in measureColumns) {
							AxisPointCrossingKey key = new AxisPointCrossingKey(column.Name, axis1Point, axis2Point);
							MeasureValues[key] = row[column].MaterializedValue;
						}
					}
				}
			}
			AxisPoint ResolveAxisPoint(StorageValue[] values, Dictionary<CacheKey, AxisPoint> axisPoints) {
				if(values.Length == 0)
					return null;
				CacheKey key = new CacheKey(values);
				AxisPoint result;
				return axisPoints.TryGetValue(key, out result) ? result : null;
			}
			bool IsSortOrderSlice(StorageSlice slice) {
				if(sortOrderSlices.Count == 0)
					return true; 
				else
					return sortOrderSlices.Any(sliceDescr => sliceDescr.IsSignatureEqual(slice.KeyColumns.Select(c => c.Name)));
			}
			LevelInfo CreateLevelInfo(IEnumerable<StorageColumn> sliceColumns, StorageColumn column, int level) {
				return new LevelInfo() {
					LevelColumn = column,
					SliceColumns = sliceColumns.ToArray(),
					Slice = storage.GetSliceIfExists(sliceColumns),
					LevelNumber = level
				};
			}
			StorageValue[] TruncateRaggedHierarchyValues(StorageValue[] values) {
				int count = 0;
				for(int i = values.Length - 1; i >= 0; i--)
					if(DashboardSpecialValues.IsOlapNullValue(values[i].MaterializedValue))
						count++;
					else
						break;
				if(count == 0)
					return values;
				else {
					StorageValue[] truncatedValues = new StorageValue[values.Length - count];
					Array.Copy(values, truncatedValues, truncatedValues.Length);
					return truncatedValues;
				}
			}
			ServerDimensionValue GetServerDimensionValue(StorageColumn column, StorageValue storageValue) {
				StorageSlice slice = storage.GetSliceIfExists(new[] { column });
				StorageRow rowToFind = new StorageRow();
				rowToFind[column] = storageValue;
				StorageRow? row = slice == null ? null : slice.FindRow(rowToFind);
				object uniqueValue = storageValue.MaterializedValue;
				object value = row.HasValue && valueStorageColumn != null ? row.Value[valueStorageColumn].MaterializedValue : null;
				string displayText = row.HasValue && displayTextStorageColumn != null ? (String)row.Value[displayTextStorageColumn].MaterializedValue : null;
				return new ServerDimensionValue(value ?? uniqueValue, uniqueValue, displayText);
			}
			void IndexAxis(AxisPoint root) {
				int counter = -1;
				ActEachItem(root, (item) => {
					item.Index = counter++;
				});
			}
			void ActEachItem(AxisPoint item, Action<AxisPoint> action) {
				action(item);
				foreach(AxisPoint child in item.ChildItems)
					ActEachItem(child, action);
			}
		}
		#endregion
		readonly DataStorage storage;
		readonly HierarchicalDataParams hDataParams;
		readonly Dictionary<SliceLevelKey, StorageSlice> levelToSlices = new Dictionary<SliceLevelKey, StorageSlice>();
		Dictionary<AxisPointCrossingKey, object> measureValues;
		public bool IsEmpty { get { return storage.IsEmpty; } }
		public DataStorage Storage { get { return storage; } }
		public Dictionary<string, AxisPoint> RootPoints { get; private set; }
		public DataStorageHelper(HierarchicalDataParams hDataParams) {
			this.hDataParams = hDataParams;
			this.storage = hDataParams.Storage;
		}
		public HierarchicalDataParams GetNewParams() {
			return hDataParams;
		}
		public void Initialize(string colAxisName, string rowAxisName, IDictionary<string, DimensionDescriptorCollection> axes) {
			AxisBuilder builder = new AxisBuilder(hDataParams, axes);
			builder.Initialize();
			RootPoints = builder.RootPoints;
			measureValues = builder.MeasureValues;
		}
		public object GetValue(AxisPoint point1, AxisPoint point2, string valueId) {
			object result;
			AxisPointCrossingKey key = GetAxisPointCrossingKey(point1, point2, valueId);
			return measureValues.TryGetValue(key, out result) ? result : null;
		}
		public void SetValue(AxisPoint point1, AxisPoint point2, string valueId, object value) {
			StorageRow row = GetSliceRow(point1, point2, true).Value;
			row[storage.CreateColumn(valueId, false)] = StorageValue.CreateUnbound(value);
			measureValues[GetAxisPointCrossingKey(point1, point2, valueId)] = value;
		}
		AxisPointCrossingKey GetAxisPointCrossingKey(AxisPoint point1, AxisPoint point2, string valueId) {
			point1 = (point1 != null && point1.Level.LevelNumber == -1) ? null : point1;
			point2 = (point2 != null && point2.Level.LevelNumber == -1) ? null : point2;
			return new AxisPointCrossingKey(valueId, point1, point2);
		}
		StorageSlice GetSlice(IEnumerable<StorageColumn> columns, bool create) {
			if(create)
				return storage.GetSlice(columns);
			else
				return storage.GetSliceIfExists(columns);
		}
		StorageColumn[] GetColumns(LevelInfo level) {
			return level == null ? new StorageColumn[0] : level.SliceColumns;
		}
		StorageRow? GetSliceRow(AxisPoint point1, AxisPoint point2, bool create) {
			LevelInfo level1 = point1 == null ? null : point1.Level;
			LevelInfo level2 = point2 == null ? null : point2.Level;
			StorageSlice slice = GetSlice(level1, level2, create);
			StorageRow rowToFind = new StorageRow();
			if(point1 != null)
				rowToFind = AddAxisPointValuesToRow(point1, rowToFind);
			if(point2 != null)
				rowToFind = AddAxisPointValuesToRow(point2, rowToFind);
			StorageRow? row = slice.FindRow(rowToFind);
			if(row.HasValue)
				return row;
			else
				return create ? slice.AddRow(rowToFind) : (StorageRow?)null;
		}
		StorageRow AddAxisPointValuesToRow(AxisPoint point, StorageRow row) {
			if(point.Parent == null)
				return row;
			else {
				row[point.Level.LevelColumn] = StorageValue.CreateUnbound(point.UniqueValue);
				return AddAxisPointValuesToRow(point.Parent, row);
			}
		}
		StorageSlice GetSlice(LevelInfo level1, LevelInfo level2, bool create) {
			StorageSlice slice = null;
			SliceLevelKey sliceKey = new SliceLevelKey(level1, level2);
			bool exists = levelToSlices.TryGetValue(sliceKey, out slice);
			bool addToDictionary = slice == null && (create || !exists);
			if(addToDictionary) {
				IEnumerable<StorageColumn> sliceSignature = GetColumns(level1).Concat(GetColumns(level2));
				slice = GetSlice(sliceSignature, create);
				levelToSlices[sliceKey] = slice;
			}
			return slice;
		}
	}
}
