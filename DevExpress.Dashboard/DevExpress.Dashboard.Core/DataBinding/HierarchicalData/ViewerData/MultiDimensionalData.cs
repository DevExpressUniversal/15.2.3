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
using System.Collections.ObjectModel;
using System.Linq;
using DevExpress.DashboardCommon.DataBinding.HierarchicalData;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DashboardCommon.DataProcessing;
namespace DevExpress.DashboardCommon.ViewerData {
	public class MultiDimensionalData {
		readonly DataStorageHelper storageHelper;
		ClientHierarchicalMetadata metadata;
		Dictionary<string, DataAxis> axes = new Dictionary<string, DataAxis>();
		Dictionary<string, DimensionDescriptorCollection> dimensionDescriptors = new Dictionary<string, DimensionDescriptorCollection>();
		MeasureDescriptorCollection measureDescriptors;
		MeasureDescriptorCollection colorMeasureDescriptors;
		MeasureDescriptorCollection formatConditionMeasureDescriptors;
		MeasureDescriptorCollection normalizedValueMeasureDescriptors;
		MeasureDescriptorCollection zeroPositionMeasureDescriptors;
		DeltaDescriptorCollection deltaDescriptors;
		bool initialized = false;
		internal MultiDimensionalData(HierarchicalDataParams dataParams, ClientHierarchicalMetadata metadata) {
			this.storageHelper = dataParams != null ? new DataStorageHelper(dataParams) : null;
			this.metadata = metadata;
		}
		protected MultiDimensionalData(MultiDimensionalData original, Dictionary<string, DataAxis> axes) {
			this.storageHelper = original.storageHelper;
			this.metadata = original.Metadata;
			this.axes = axes;
			this.dimensionDescriptors = original.DimensionDescriptors;
			this.measureDescriptors = original.GetMeasures();
			this.colorMeasureDescriptors = original.ColorMeasureDescriptors;
			this.formatConditionMeasureDescriptors = original.formatConditionMeasureDescriptors;
			this.normalizedValueMeasureDescriptors = original.normalizedValueMeasureDescriptors;
			this.zeroPositionMeasureDescriptors = original.zeroPositionMeasureDescriptors;
			this.deltaDescriptors = original.GetDeltas();
			this.initialized = true;
		}
		internal Dictionary<string, DataAxis> Axes {
			get {
				if(!initialized)
					Initialize();
				return axes;
			}
		}
		MeasureDescriptorCollection MeasureDescriptors {
			get {
				if(!initialized)
					Initialize();
				return measureDescriptors;
			}
		}
		MeasureDescriptorCollection ColorMeasureDescriptors {
			get {
				if(!initialized)
					Initialize();
				return colorMeasureDescriptors;
			}
		}
		MeasureDescriptorCollection FormatConditionMeasureDescriptors {
			get {
				if(!initialized)
					Initialize();
				return formatConditionMeasureDescriptors;
			}
		}
		MeasureDescriptorCollection NormalizedValueMeasureDescriptors {
			get {
				if(!initialized)
					Initialize();
				return normalizedValueMeasureDescriptors;
			}
		}
		MeasureDescriptorCollection ZeroPositionMeasureDescriptors {
			get {
				if(!initialized)
					Initialize();
				return zeroPositionMeasureDescriptors;
			}
		}
		DeltaDescriptorCollection DeltaDescriptors {
			get {
				if(!initialized)
					Initialize();
				return deltaDescriptors;
			}
		}
		Dictionary<string, DimensionDescriptorCollection> DimensionDescriptors {
			get {
				if(!initialized)
					Initialize();
				return dimensionDescriptors;
			}
		}
		public virtual bool IsEmpty { get { return (storageHelper == null || storageHelper.IsEmpty) && (metadata.DimensionDescriptors.Count == 0 || metadata.MeasureDescriptors.Count == 0); } } 
		public IList<string> GetDataMembers() {
			return new List<string>(metadata.DataSourceColumns);
		}
		public IList<string> GetAxisNames() {
			return metadata.GetHierarchyNames();
		}
		public DimensionDescriptorCollection GetDimensions(string axisName) {
			DimensionDescriptorCollection dimensions;
			if(DimensionDescriptors.TryGetValue(axisName, out dimensions)) {
				return dimensions;
			}
			throw new ArgumentException(DashboardLocalizer.GetString(DashboardStringId.MessageWrongAxisName));
		}
		public MeasureDescriptorCollection GetMeasures() {
			return MeasureDescriptors;
		}
		public DeltaDescriptorCollection GetDeltas() {
			return DeltaDescriptors;
		}
		public DataAxis GetAxis(string axisName) {
			DataAxis axis;
			if(!Axes.TryGetValue(axisName, out axis)) {
				throw new ArgumentException(DashboardLocalizer.GetString(DashboardStringId.MessageWrongAxisName));
			}
			return axis;
		}
		public ReadOnlyCollection<AxisPoint> GetAxisPoints(string axisName) {
			AxisPoint axisRoot = GetAxisRoot(axisName);
			if(axisRoot != null)
				return new ReadOnlyCollection<AxisPoint>(axisRoot.GetAxisPoints().ToList());
			return null;
		}
		public AxisPoint GetAxisPointByUniqueValues(string axisName, object[] values) {
			DevExpress.Utils.Guard.ArgumentNotNull(values, "values");
			DataAxis axis;
			if(!Axes.TryGetValue(axisName, out axis))
				throw new ArgumentException(DashboardLocalizer.GetString(DashboardStringId.MessageWrongAxisName));
			return axis.RootPoint.GetAxisPointByUniqueValues(values);
		}
		internal IEnumerable<AxisPoint> GetAxisPointsByUniqueValues(string axisName, List<KeyValuePair<string, object>> values) { 
			DevExpress.Utils.Guard.ArgumentNotNull(values, "values");
			DataAxis axis;
			if(!Axes.TryGetValue(axisName, out axis))
				throw new ArgumentException(DashboardLocalizer.GetString(DashboardStringId.MessageWrongAxisName));
			return axis.RootPoint.GetAxisPointsByUniqueValues(values);
		}
		public AxisPoint GetAxisRoot(string axisName) {
			DataAxis axis;
			if(!Axes.TryGetValue(axisName, out axis))
				throw new ArgumentException(DashboardLocalizer.GetString(DashboardStringId.MessageWrongAxisName));
			return axis.RootPoint;
		}
		internal MeasureValue GetValue(string measureId) {
			MeasureDescriptor measureDescriptor = GetMeasureDescriptorByID(measureId);
			return GetValue(measureDescriptor);
		}
		public MeasureValue GetValue(MeasureDescriptor measure) {
			DevExpress.Utils.Guard.ArgumentNotNull(measure, "measure");
			MeasureDescriptorInternal internalDescriptor = measure.InternalDescriptor;
			object value = GetCellValue(internalDescriptor.ID);
			return new MeasureValue(value, internalDescriptor.Format);
		}
		internal DeltaValue GetDeltaValue(string deltaId) {
			DeltaDescriptor deltaDescriptor = GetDeltaDescriptorById(deltaId);
			return GetDeltaValue(deltaDescriptor);
		}
		public DeltaValue GetDeltaValue(DeltaDescriptor delta) {
			return GetDeltaValue(null, delta);
		}
		public MultiDimensionalData GetSlice(AxisPoint axisPoint) {
			DevExpress.Utils.Guard.ArgumentNotNull(axisPoint, "axis point");
			Dictionary<string, DataAxis> newAxes = new Dictionary<string, DataAxis>(Axes);
			string sliceAxisName = axisPoint.AxisName;
			newAxes[sliceAxisName] = new DataAxis(sliceAxisName, axisPoint.GetChildDimensions(), axisPoint);
			return CreateInstance(newAxes);
		}
		protected virtual MultiDimensionalData CreateInstance(Dictionary<string, DataAxis> newAxes) {
			return new MultiDimensionalData(this, newAxes);
		}
		public MultiDimensionalData GetSlice(string axisName, object[] values) {
			DevExpress.Utils.Guard.ArgumentNotNull(values, "values");
			AxisPoint item = GetAxisPointByUniqueValues(axisName, values);
			if(item != null)
				return GetSlice(item);
			return null;
		}
		public MultiDimensionalData GetSlice(AxisPointTuple tuple) {
			MultiDimensionalData data = this;
			foreach(AxisPoint axisPoint in tuple.AxisPoints) {
				data = data.GetSlice(axisPoint);
			}
			return data;
		}
		public AxisPointTuple CreateTuple(object value) {
			return CreateTuple(axes.First().Key, value);
		}
		public AxisPointTuple CreateTuple(string axisName, object value) {
			Dictionary<string, object> axisValues = new Dictionary<string, object>();
			axisValues.Add(axisName, value);
			return CreateTuple(axisValues);
		}
		public AxisPointTuple CreateTuple(Dictionary<string, object> axisValues) {
			IList<AxisPoint> axisPoints = new List<AxisPoint>();
			foreach(KeyValuePair<string, object> pair in axisValues) {
				axisPoints.Add(GetAxisPointByUniqueValues(pair.Key, (object[])pair.Value));
			}
			return CreateTuple(axisPoints.ToArray());
		}
		public AxisPointTuple CreateTuple(params AxisPoint[] axisPoints) {
			return new AxisPointTuple(axisPoints);
		}
		public AxisPointTuple CreateTuple(IList<AxisPoint> axisPoints) {
			return new AxisPointTuple(axisPoints);
		}
		internal ClientHierarchicalMetadata Metadata { get { return metadata; } }
		MeasureValue GetMeasureValue(string id, NumericFormatViewModel format, AxisPoint axisPoint = null) {
			object value = GetCellValue(id, axisPoint);
			return new MeasureValue(value, new ValueFormatViewModel(format));
	   }
		void Initialize() {
			InitializeDescriptors();
			InitializeAxes();
			initialized = true;
		}
		protected void InitializeMeasureDescriptors() {
			IList<MeasureDescriptor> measureList = metadata.MeasureDescriptors.Select(internalDescriptor => {
				return new MeasureDescriptor(internalDescriptor);
			}).ToList();
			measureDescriptors = new MeasureDescriptorCollection(measureList);
			IList<MeasureDescriptor> colorMeasureList = metadata.ColorMeasureDescriptors.Select(internalDescriptor => {
				return new MeasureDescriptor(internalDescriptor);
			}).ToList();
			colorMeasureDescriptors = new MeasureDescriptorCollection(colorMeasureList);
			IList<MeasureDescriptor> formatConditionMeasureList = metadata.FormatConditionMeasureDescriptors.Select(internalDescriptor => {
				return new MeasureDescriptor(internalDescriptor);
			}).ToList();
			formatConditionMeasureDescriptors = new MeasureDescriptorCollection(formatConditionMeasureList);
			IList<MeasureDescriptor> normalizedValueMeasureList = metadata.NormalizedValueMeasureDescriptors.Select(internalDescriptor => {
				return new MeasureDescriptor(internalDescriptor);
			}).ToList();
			normalizedValueMeasureDescriptors = new MeasureDescriptorCollection(normalizedValueMeasureList);
			IList<MeasureDescriptor> zeroPositionValueMeasureList = metadata.ZeroPositionMeasureDescriptors.Select(internalDescriptor => {
				return new MeasureDescriptor(internalDescriptor);
			}).ToList();
			zeroPositionMeasureDescriptors = new MeasureDescriptorCollection(zeroPositionValueMeasureList);
		}
		void InitializeDescriptors() {
			InitializeMeasureDescriptors();
			IList<DeltaDescriptor> deltaList = metadata.DeltaDescriptors.Select(internalDescriptor => {
				return new DeltaDescriptor(internalDescriptor);
			}).ToList();
			deltaDescriptors = new DeltaDescriptorCollection(deltaList);
			foreach(KeyValuePair<string, DimensionDescriptorInternalCollection> internalDictionaryRecord in metadata.DimensionDescriptors) {
				DimensionDescriptorInternalCollection internalDescriptorCollection = internalDictionaryRecord.Value;
				DimensionDescriptor[] descriptors = new DimensionDescriptor[internalDescriptorCollection.Count];
				int i = 0;
				foreach(DimensionDescriptorInternal internalDescriptor in internalDescriptorCollection) {
					descriptors[i] = new DimensionDescriptor(internalDescriptor);
					i++;
				}
				dimensionDescriptors.Add(internalDictionaryRecord.Key, new DimensionDescriptorCollection(descriptors));
			}
		}
		void InitializeAxes() {
			if(storageHelper == null)
				return;
			storageHelper.Initialize(metadata.ColumnHierarchy, metadata.RowHierarchy, dimensionDescriptors);
			foreach(var root in storageHelper.RootPoints) {
				axes.Add(root.Key, new DataAxis(root.Key, dimensionDescriptors[root.Key], root.Value));
			}
		}
		internal DimensionDescriptor GetDimensionDescriptorByID(string axisName, string id) {
			return DimensionDescriptors[axisName].FirstOrDefault(descriptor => descriptor.ID == id);
		}
		internal DimensionDescriptor GetDimensionDescriptorByID(string id) {
			return DimensionDescriptors.Values
				.SelectMany(descriptors => descriptors)
				.FirstOrDefault(descriptor => descriptor.ID == id);
		}
		internal MeasureDescriptor GetMeasureDescriptorByID(string id) {
			return ColorMeasureDescriptors.FirstOrDefault(descriptor => descriptor.ID == id) ?? MeasureDescriptors.FirstOrDefault(descriptor => descriptor.ID == id);
		}
		internal MeasureDescriptor GetFormatConditionMeasureDescriptorByID(string id) {
			return FormatConditionMeasureDescriptors.FirstOrDefault(descriptor => descriptor.ID == id) ?? MeasureDescriptors.FirstOrDefault(descriptor => descriptor.ID == id);
		}
		internal MeasureDescriptor GetNormalizedValueMeasureDescriptorByID(string id) {
			return NormalizedValueMeasureDescriptors.FirstOrDefault(descriptor => descriptor.ID == id) ?? MeasureDescriptors.FirstOrDefault(descriptor => descriptor.ID == id);
		}
		internal MeasureDescriptor GetZeroPositionMeasureDescriptorByID(string id) {
			return ZeroPositionMeasureDescriptors.FirstOrDefault(descriptor => descriptor.ID == id) ?? MeasureDescriptors.FirstOrDefault(descriptor => descriptor.ID == id);
		}
		internal DeltaDescriptor GetDeltaDescriptorById(string id) {
			return DeltaDescriptors.FirstOrDefault(descriptor => descriptor.ID == id);
		}
		internal ReadOnlyCollection<AxisPoint> GetAxisPointsByDimensionWithRagged(DimensionDescriptor dimension) {
			DevExpress.Utils.Guard.ArgumentNotNull(dimension, "dimension");
			AxisPoint axisRoot = GetAxisRoot(dimension);
			if(axisRoot != null)
				return new ReadOnlyCollection<AxisPoint>(axisRoot.GetAxisPointsByDimension(dimension, true));
			return null;
		}
		public ReadOnlyCollection<AxisPoint> GetAxisPointsByDimension(DimensionDescriptor dimension) {
			return GetAxisPointsByDimension(dimension, false);
		}
		internal ReadOnlyCollection<AxisPoint> GetAxisPointsByDimension(DimensionDescriptor dimension, bool includeRaggedHierarchy) {
			DevExpress.Utils.Guard.ArgumentNotNull(dimension, "dimension");
			AxisPoint axisRoot = GetAxisRoot(dimension);
			if(axisRoot != null)
				return new ReadOnlyCollection<AxisPoint>(axisRoot.GetAxisPointsByDimension(dimension, includeRaggedHierarchy));
			return null;
		}
		internal ReadOnlyCollection<AxisPoint> GetAxisPointsByDimensionId(string id) {
			if(string.IsNullOrEmpty(id)) return null;
			DimensionDescriptor descriptor = GetDimensionDescriptorByID(id);
			return descriptor != null ? GetAxisPointsByDimension(descriptor) : null;
		}
		internal MeasureValue GetValue(AxisPoint axisPoint, MeasureDescriptor measure) {
			return GetValue(axisPoint, null, measure);
		}
		internal MeasureValue GetValue(AxisPoint axisPoint, string measureId) {
			return GetValue(axisPoint, null, measureId);
		}
		internal MeasureValue GetValue(AxisPoint axis1Point, AxisPoint axis2Point, MeasureDescriptor measure) {
			DevExpress.Utils.Guard.ArgumentNotNull(measure, "measure");
			MeasureDescriptorInternal internalDescriptor = measure.InternalDescriptor;
			object value = GetCellValue(internalDescriptor.ID, axis1Point, axis2Point);
			return new MeasureValue(value, internalDescriptor.Format);
		}
		internal MeasureValue GetValue(AxisPoint axis1Point, AxisPoint axis2Point, string id) {
			MeasureDescriptor measure = GetMeasureDescriptorByID(id);
			return GetValue(axis1Point, axis2Point, measure);
		}
		internal DeltaValue GetDeltaValue(AxisPoint axisPoint, string deltaId) {
			DeltaDescriptor deltaDescriptor = GetDeltaDescriptorById(deltaId);
			return GetDeltaValue(axisPoint, deltaDescriptor);
		}
		internal DeltaValue GetDeltaValue(AxisPoint axisPoint, DeltaDescriptor delta) {
			DevExpress.Utils.Guard.ArgumentNotNull(delta, "delta");
			DeltaDescriptorInternal internalDescriptor = delta.InternalDescriptor;
			MeasureValue absoluteVariation = internalDescriptor.AbsoluteVariationID != null ? GetMeasureValue(internalDescriptor.AbsoluteVariationID, internalDescriptor.AbsoluteVariationFormat, axisPoint) : null;
			MeasureValue percentVariation = internalDescriptor.PercentVariationID != null ? GetMeasureValue(internalDescriptor.PercentVariationID, internalDescriptor.PercentVariationFormat, axisPoint) : null;
			MeasureValue percentOfTarget = internalDescriptor.PercentOfTargetID != null ? GetMeasureValue(internalDescriptor.PercentOfTargetID, internalDescriptor.PercentOfTargetFormat, axisPoint) : null;
			MeasureValue actualValue = internalDescriptor.ActualValueID != null ? GetMeasureValue(internalDescriptor.ActualValueID, internalDescriptor.ActualValueFormat, axisPoint) : null;
			MeasureValue targetValue = internalDescriptor.TargetValueID != null ? GetMeasureValue(internalDescriptor.TargetValueID, internalDescriptor.ActualValueFormat, axisPoint) : null;
			object isGoodValue = internalDescriptor.IsGoodID != null ? GetCellValue(internalDescriptor.IsGoodID, axisPoint) : null;
			object indicatorTypeValue = internalDescriptor.IndicatorTypeID != null ? GetCellValue(internalDescriptor.IndicatorTypeID, axisPoint) : null;
			bool isGood = isGoodValue == null || DashboardSpecialValues.IsErrorValue(isGoodValue) ? false : (bool)isGoodValue;
			IndicatorType indicatorType = indicatorTypeValue == null || DashboardSpecialValues.IsErrorValue(indicatorTypeValue) ? IndicatorType.None : (IndicatorType)indicatorTypeValue;
			return new DeltaValue(absoluteVariation, percentVariation, percentOfTarget, actualValue, targetValue, 
				isGood, indicatorType, internalDescriptor.DeltaValueType);
		}
		internal object GetColorValue(AxisPoint columnAxisPoint, AxisPoint rowAxisPoint, MeasureDescriptor measure) {
			DevExpress.Utils.Guard.ArgumentNotNull(measure, "measure");
			return GetCellValue(measure.InternalDescriptor.ID, columnAxisPoint, rowAxisPoint);
		}
		internal DataStorage GetDataStorage() {
			return storageHelper.Storage;
		}
		AxisPoint GetAxisRoot(DimensionDescriptor dimension) {
			foreach(KeyValuePair<string, DimensionDescriptorCollection> axis in DimensionDescriptors) {
				if(axis.Value.Contains(dimension))
					return (AxisPoint)Axes[axis.Key].RootPoint;
			}
			return null;
		}
		internal void AddValue(AxisPoint axis1Point, AxisPoint axis2Point, MeasureDescriptor measureDescriptor, object value) {
			MeasureDescriptorInternal descriptor = measureDescriptor.InternalDescriptor;
			if(!metadata.MeasureDescriptors.Contains(descriptor) && !metadata.ColorMeasureDescriptors.Contains(descriptor) &&
			   !metadata.FormatConditionMeasureDescriptors.Contains(descriptor) && !metadata.NormalizedValueMeasureDescriptors.Contains(descriptor)) {
				metadata.MeasureDescriptors.Add(descriptor);
				InitializeMeasureDescriptors();
			}
			AddValue(axis1Point, axis2Point, descriptor.ID, value);
		}
		void AddValue(AxisPoint axis1Point, AxisPoint axis2Point, string measureId, object value) {
			AxisPoint point1;
			AxisPoint point2;
			CalculatePoints(out point1, out point2, axis1Point, axis2Point);
			storageHelper.SetValue(point1, point2, measureId, value);
		}
		internal void AddAxis(DataAxis axis) {
			if(!initialized)
				Initialize();
			axes.Add(axis.Name, axis);
			dimensionDescriptors.Add(axis.Name, axis.Dimensions);
		}
		protected virtual object GetCellValue(AxisPoint point1, AxisPoint point2, string id) {
			return storageHelper.GetValue(point1, point2, id);
		}
		object GetCellValue(string id, params AxisPoint[] points) {
			AxisPoint point1, point2;
			CalculatePoints(out point1, out point2, points);
			return GetCellValue(point1, point2, id);
		}
		void CalculatePoints(out AxisPoint point1, out AxisPoint point2, params AxisPoint[] points) {
			AxisPoint axis1Point = points.Length > 0 ? points[0] : null;
			AxisPoint axis2Point = points.Length > 1 ? points[1] : null;
			if(axis1Point != null && axis2Point != null && axis1Point.AxisName == axis2Point.AxisName)
				throw new Exception("Axis points indexes cannot be calculated because of equal axes");
			point1 = axis1Point;
			point2 = axis2Point;
			AxisPoint columnRootAxisPoint = null;
			AxisPoint rowRootAxisPoint = null;
			if(!String.IsNullOrEmpty(metadata.ColumnHierarchy)) {
				DataAxis axis = null;
				if(Axes.TryGetValue(metadata.ColumnHierarchy, out axis))
					columnRootAxisPoint = axis.RootPoint;
			}
			if(!String.IsNullOrEmpty(metadata.RowHierarchy)) {
				DataAxis axis = null;
				if(Axes.TryGetValue(metadata.RowHierarchy, out axis))
					rowRootAxisPoint = axis.RootPoint;
			}
			AxisPoint currentColumnPoint = null;
			AxisPoint currentRowPoint = null;
			if(axis1Point != null) {
				if(axis1Point.AxisName == metadata.ColumnHierarchy)
					currentColumnPoint = axis1Point;
				else if(axis1Point.AxisName == metadata.RowHierarchy)
					currentRowPoint = axis1Point;
			}
			if(axis2Point != null) {
				if(axis2Point.AxisName == metadata.ColumnHierarchy)
					currentColumnPoint = axis2Point;
				else if(axis2Point.AxisName == metadata.RowHierarchy)
					currentRowPoint = axis2Point;
			}
			point1 = currentColumnPoint ?? columnRootAxisPoint;
			point2 = currentRowPoint ?? rowRootAxisPoint;
		}
	}
}
