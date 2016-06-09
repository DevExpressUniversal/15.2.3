#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using System.Globalization;
using DevExpress.Charts.Native;
using DevExpress.XtraCharts.Localization;
namespace DevExpress.XtraCharts.Native {
	public abstract class AxisRangeData : ChartElement, IAxisRange {
public abstract bool Auto { get; set; }
		public abstract object MinValue { get; set; }
		public abstract object MaxValue { get; set; }
		public abstract object MinValueSerializable { set; }
		public abstract object MaxValueSerializable { set; }
		public abstract double MinValueInternal { get; set; }
		public abstract double MaxValueInternal { get; set; }
		public abstract bool AlwaysShowZeroLevel { get; set; }
		public abstract bool SideMarginsEnabled { get; set; }
		public abstract double SideMargins { get; }
		protected abstract IAxisRange AxisRangeImplementation { get; }
		#region IAxisRange implementation
		object IAxisRange.MinValue { get { return AxisRangeImplementation.MinValue; } }
		object IAxisRange.MaxValue { get { return AxisRangeImplementation.MaxValue; } }
		double IAxisRange.MinValueInternal { get { return AxisRangeImplementation.MinValueInternal; } }
		double IAxisRange.MaxValueInternal { get { return AxisRangeImplementation.MaxValueInternal; } }
		void IAxisRange.Assign(IAxisRange source) {
			AxisRangeImplementation.Assign(source);
		}
		void IAxisRange.Assign(IAxisRangeData source) {
			AxisRangeImplementation.Assign(source);
		}
		void IAxisRange.UpdateRange(object min, object max, double internalMin, double internalMax) {
			AxisRangeImplementation.UpdateRange(min, max, internalMin, internalMax);
		}
		#endregion
		public AxisRangeData(ChartElement owner)
			: base(owner) {
		}
		public abstract bool ShouldSerializeAuto();
		public abstract bool ShouldSerializeMinValue();
		public abstract bool ShouldSerializeMaxValue();
		public abstract bool ShouldSerializeMinValueInternal();
		public abstract bool ShouldSerializeMaxValueInternal();
		public abstract bool ShouldSerializeAlwaysShowZeroLevel();
		public abstract bool ShouldSerializeSideMarginsEnabled();
		public abstract void ShiftInternalMinMaxValues(double shift);
		public abstract void SetMinMaxValues(object minValue, object maxValue);
		public abstract void SetInternalMinMaxValues(double min, double max);
		public abstract void OnScaleTypeUpdated();
		public abstract object ConvertToNativeValue(object value, string propertyName);
		public abstract void CheckRange(object minValue, object maxValue, double minValueInternal, double maxValueInternal);
		protected void CheckMinValueInternal(double value) {
			if (Double.IsNaN(value) || Double.IsInfinity(value))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisRangeMinValueInternal));
		}
		protected void CheckMaxValueInternal(double value) {
			if (Double.IsNaN(value) || Double.IsInfinity(value))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisRangeMaxValueInternal));
		}
	}
	public class AxisRangeDataEx : AxisRangeData {
		#region IAxisRange implementation
		public class AxisRangeImp : IAxisRange {
			AxisRangeDataEx owner;
			public AxisRangeImp(AxisRangeDataEx owner) {
				this.owner = owner;
			}
			#region IAxisRange implementation
			object IAxisRange.MinValue { get { return owner.minValue; } }
			object IAxisRange.MaxValue { get { return owner.maxValue; } }
			double IAxisRange.MinValueInternal { get { return owner.minValueInternal; } }
			double IAxisRange.MaxValueInternal { get { return owner.maxValueInternal; } }
			void IAxisRange.Assign(IAxisRange source) {
				AxisRangeData rangeData = source as AxisRangeData;
				if (rangeData != null)
					owner.Assign(rangeData);
			}
			void IAxisRange.Assign(IAxisRangeData source) {
				if (source == null)
					return;
				owner.minValue = source.MinValue;
				owner.maxValue = source.MaxValue;
				owner.minValueInternal = source.Min;
				owner.maxValueInternal = source.Max;
				owner.auto = source.CorrectionMode == RangeCorrectionMode.Auto;
				owner.sideMarginsEnabled = source.AutoSideMargins == SideMarginMode.Enable || source.AutoSideMargins == SideMarginMode.UserEnable;				
			}
			void IAxisRange.UpdateRange(object min, object max, double internalMin, double internalMax) {
				owner.minValue = min;
				owner.maxValue = max;
				owner.minValueInternal = internalMin;
				owner.maxValueInternal = internalMax;
			}
			public object Clone() {
				return owner.Clone();
			}
			#endregion
		}
		#endregion
		public const string minValuePropertyName = "MinValue";
		public const string maxValuePropertyName = "MaxValue";
		static void ThrowIncorrectPropertyValueException(object propertyValue, string propertyName) {
			string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPropertyValue),
				propertyValue == null ? String.Empty : propertyValue.ToString(), propertyName);
			throw new ArgumentException(message);
		}
		static void GenerateAxisRangeException() {
			throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisRange));
		}
		public static void CheckNumericalMinMaxValues(object minValue, object maxValue, double minValueInternal, double maxValueInternal) {
			double minValueDouble = minValue == null ? minValueInternal : Convert.ToDouble(minValue);
			double maxValueDouble = maxValue == null ? maxValueInternal : Convert.ToDouble(maxValue);
			if (minValueDouble >= maxValueDouble)
				GenerateAxisRangeException();
		}
		public static void CheckAbstractMinMaxValues(object minValue, object maxValue, double minValueInternal, double maxValueInternal) {
			if (minValue == null || maxValue == null)
				return;
			if (minValue is string) {
				if (!(maxValue is string))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgMinMaxDifferentTypes));
			}
			else if (minValue is DateTime) {
				if (!(maxValue is DateTime))
					throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgMinMaxDifferentTypes));
				if (DateTime.Compare((DateTime)minValue, (DateTime)maxValue) >= 0)
					GenerateAxisRangeException();
			}
			else
				CheckNumericalMinMaxValues(minValue, maxValue, minValueInternal, maxValueInternal);
		}
		readonly AxisBase axis;
		bool auto = true;
		object minValue;
		object maxValue;
		object minValueSerializable;
		object maxValueSerializable;
		double minValueInternal = Double.NaN;
		double maxValueInternal = Double.NaN;
		bool alwaysShowZeroLevel = true;
		bool sideMarginsEnabled = true;
		AxisRangeImp axisRangeImp;
		protected override IAxisRange AxisRangeImplementation { get { return axisRangeImp; } }
		public override bool Auto {
			get { return auto; }
			set {
				if (value != auto) {
					if (value) {
						SendNotification(new ElementWillChangeNotification(this));
						auto = true;
						minValue = null;
						maxValue = null;
						minValueInternal = Double.NaN;
						maxValueInternal = Double.NaN;
					}
					else {
						SendNotification(new ElementWillChangeNotification(this));
						ClearAuto();
					}
					RaiseControlChanged();
				}
			}
		}
		public override object MinValue {
			get { return minValue; }
			set {
				CheckMinValue(value);
				if (!Loading)
					value = ConvertToNativeValue(value, minValuePropertyName);
				SendNotification(new ElementWillChangeNotification(this));
				ClearAuto();
				minValue = value;
				minValueInternal = Double.NaN;
				RaiseControlChanged();
			}
		}
		public override object MaxValue {
			get { return maxValue; }
			set {
				CheckMaxValue(value);
				if (!Loading)
					value = ConvertToNativeValue(value, maxValuePropertyName);
				SendNotification(new ElementWillChangeNotification(this));
				ClearAuto();
				maxValue = value;
				maxValueInternal = Double.NaN;
				RaiseControlChanged();
			}
		}
		public override object MinValueSerializable {
			set {
				if (Loading)
					minValueSerializable = value;
			}
		}
		public override object MaxValueSerializable {
			set {
				if (Loading)
					maxValueSerializable = value;
			}
		}
		public override double MinValueInternal {
			get { return minValueInternal; }
			set {
				CheckMinValueInternal(value);
				SendNotification(new ElementWillChangeNotification(this));
				ClearAuto();
				minValueInternal = value;
				minValue = null;
				RaiseControlChanged();
			}
		}
		public override double MaxValueInternal {
			get { return maxValueInternal; }
			set {
				CheckMaxValueInternal(value);
				SendNotification(new ElementWillChangeNotification(this));
				ClearAuto();
				maxValueInternal = value;
				maxValue = null;
				RaiseControlChanged();
			}
		}
		public override bool AlwaysShowZeroLevel {
			get { return alwaysShowZeroLevel; }
			set {
				if (value != alwaysShowZeroLevel) {
					SendNotification(new ElementWillChangeNotification(this));
					alwaysShowZeroLevel = value;
					RaiseControlChanged();
				}
			}
		}
		public override bool SideMarginsEnabled {
			get { return sideMarginsEnabled; }
			set {
				if (value != sideMarginsEnabled) {
					SendNotification(new ElementWillChangeNotification(this));
					sideMarginsEnabled = value;
					RaiseControlChanged();
				}
			}
		}
		public override double SideMargins {
			get {
				return 0;
			}
		}
		AxisRangeDataEx(ChartElement owner, AxisBase axis) : base(owner) {
			this.axis = axis;
			axisRangeImp = new AxisRangeImp(this);
		}
		public AxisRangeDataEx(AxisRange range) : this(range, range.Axis) {
		}
		public AxisRangeDataEx(ScrollingRange range) : this(range, range.Range.Axis) {
		}
		void CheckMinValue(object value) {
			if (value == null)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisRangeMinValue));
		}
		void CheckMaxValue(object value) {
			if (value == null)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisRangeMaxValue));
		}
		void ClearAuto() {
			if (auto) 
				auto = false;			
		}
		object ConvertToNativeValue(object value, string propertyName, CultureInfo culture) {
			if (value == null)
				ThrowIncorrectPropertyValueException(value, propertyName);
			AxisScaleTypeMap map = axis.ScaleTypeMap;
			object nativeValue = map.ConvertValue(value, culture);
			if (!map.IsCompatible(nativeValue))
				ThrowIncorrectPropertyValueException(value, propertyName);
			return nativeValue;
		}
		public override object ConvertToNativeValue(object value, string propertyName) {
			return ConvertToNativeValue(value, propertyName, CultureInfo.CurrentCulture);
		}
		public override void CheckRange(object minValue, object maxValue, double minValueInternal, double maxValueInternal) {
			switch (axis.ScaleType) {
				case ActualScaleType.Qualitative:
					bool shouldTest = true;
					if (minValue == null)
						minValue = minValueInternal;
					else {
						if (!(minValue is string)) {
							string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPropertyValue),
								minValue, minValuePropertyName);
							throw new ArgumentException(message);
						}
						minValueInternal = axis.ScaleTypeMap.NativeToInternal(minValue);
						if (minValueInternal < 0)
							shouldTest = false;
					}
					if (maxValue == null)
						maxValue = maxValueInternal;
					else {
						if (!(maxValue is string)) {
							string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPropertyValue),
								maxValue, maxValuePropertyName);
							throw new ArgumentException(message);
						}
						maxValueInternal = axis.ScaleTypeMap.NativeToInternal(maxValue);
						if (maxValueInternal < 0)
							shouldTest = false;
					}
					if (shouldTest && maxValueInternal <= minValueInternal)
						GenerateAxisRangeException();
					break;
				case ActualScaleType.DateTime:
					if (minValue == null)
						minValue = minValueInternal;
					else {
						if (!(minValue is DateTime)) {
							string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPropertyValue),
								minValue, minValuePropertyName);
							throw new ArgumentException(message);
						}
						minValueInternal = axis.ScaleTypeMap.NativeToInternal(minValue);
					}
					if (maxValue == null)
						maxValue = maxValueInternal;
					else {
						if (!(maxValue is DateTime)) {
							string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPropertyValue),
								maxValue, maxValuePropertyName);
							throw new ArgumentException(message);
						}
						maxValueInternal = axis.ScaleTypeMap.NativeToInternal(maxValue);
					}
					if (maxValueInternal <= minValueInternal)
						GenerateAxisRangeException();
					break;
				default:
					CheckNumericalMinMaxValues(minValue, maxValue, minValueInternal, maxValueInternal);
					break;
			}
		}
		public override void SetMinMaxValues(object minValue, object maxValue) {
			if (Loading) {
				auto = false;
				this.minValue = minValue;
				this.maxValue = maxValue;
			}
			else {
				CheckMinValue(minValue);
				CheckMaxValue(maxValue);
				minValue = ConvertToNativeValue(minValue, minValuePropertyName);
				maxValue = ConvertToNativeValue(maxValue, maxValuePropertyName);
				CheckRange(minValue, maxValue, minValueInternal, maxValueInternal);
				SendNotification(new ElementWillChangeNotification(this));
				auto = false;
				this.minValue = minValue;
				this.maxValue = maxValue;
				minValueInternal = Double.NaN;
				maxValueInternal = Double.NaN;
				RaiseControlChanged();
			}
		}
		public override void SetInternalMinMaxValues(double min, double max) {
			CheckMinValueInternal(min);
			CheckMaxValueInternal(max);
			CheckAbstractMinMaxValues(min, max, minValueInternal, maxValueInternal);
			SendNotification(new ElementWillChangeNotification(this));
			auto = false;
			minValueInternal = min;
			maxValueInternal = max;
			minValue = null;
			maxValue = null;
			RaiseControlChanged();
		}
		public override void ShiftInternalMinMaxValues(double shift) {
			if (!Double.IsNaN(minValueInternal))
				minValueInternal += shift;
			if (!Double.IsNaN(maxValueInternal))
				maxValueInternal += shift;
		}
		public override void OnScaleTypeUpdated() {
			if (!auto)
				try {
					if (minValue != null) {
						if (minValueSerializable == minValue) {
							minValue = ConvertToNativeValue(minValue, minValuePropertyName, CultureInfo.InvariantCulture);
							if (this.Owner != null && this.Owner is ScrollingRange)
								axis.WholeRangeData.MinValue = minValue;
							else {
								double min = axis.ScaleTypeMap.NativeToInternal(minValue);
								Axis2D axis2D = axis as Axis2D;
								XYDiagram2D diagram2D = axis2D != null ? axis2D.Diagram as XYDiagram : null;
								if (diagram2D == null)
									axis.WholeRangeData.MinValue = minValue;
								else if ((!diagram2D.EnableAxisXScrolling && !axis.IsValuesAxis) ||
								   (!diagram2D.EnableAxisYScrolling && axis.IsValuesAxis)) {
									((VisualRangeData)axis.WholeRangeData).SetRange(minValue, axis.WholeRangeData.MaxValue, min, axis.WholeRangeData.Max, false);
								}
								((VisualRangeData)axis.VisualRangeData).SetRange(minValue, axis.VisualRangeData.MaxValue, min, axis.VisualRangeData.Max, false);
							}
							minValueSerializable = null;
						}
						else {
							minValue = ConvertToNativeValue(minValue, minValuePropertyName);
						}
					}
					if (maxValue != null)
						if (maxValueSerializable == maxValue) {
							maxValue = ConvertToNativeValue(maxValue, maxValuePropertyName, CultureInfo.InvariantCulture);
							if (this.Owner != null && this.Owner is ScrollingRange)
								axis.WholeRangeData.MaxValue = maxValue;
							else {
								double max = axis.ScaleTypeMap.NativeToInternal(maxValue);
								Axis2D axis2D = axis as Axis2D;
								XYDiagram2D diagram2D = axis2D != null ? axis2D.Diagram as XYDiagram : null;
								if (diagram2D == null)
									axis.WholeRangeData.MaxValue = maxValue;
								else if ((!diagram2D.EnableAxisXScrolling && !axis.IsValuesAxis) ||
								   (!diagram2D.EnableAxisYScrolling && axis.IsValuesAxis)) {
									((VisualRangeData)axis.WholeRangeData).SetRange(axis.WholeRangeData.MinValue, maxValue, axis.WholeRangeData.Min, max, false);
								}
								((VisualRangeData)axis.VisualRangeData).SetRange(axis.VisualRangeData.MinValue, maxValue, axis.VisualRangeData.Min, max, false);
							}
							maxValueSerializable = null;
						}
						else {
							maxValue = ConvertToNativeValue(maxValue, maxValuePropertyName);
						}
				}
				catch {
					LockChanges();
					try {
						Auto = true;
					}
					finally {
						UnlockChanges();
					}
				}
		}
		protected internal override bool ShouldSerialize() {
			return ShouldSerializeAuto() || ShouldSerializeMinValue() || ShouldSerializeMaxValue() || ShouldSerializeMinValueInternal() ||
				 ShouldSerializeMaxValueInternal() || ShouldSerializeAlwaysShowZeroLevel() || ShouldSerializeSideMarginsEnabled();
		}
		public override bool ShouldSerializeAuto() {
			return false;
		}
		public override bool ShouldSerializeMinValue() {
			return false;
		}
		public override bool ShouldSerializeMaxValue() {
			return false;
		}
		public override bool ShouldSerializeMinValueInternal() {
			return false;
		}
		public override bool ShouldSerializeMaxValueInternal() {
			return false;
		}
		public override bool ShouldSerializeAlwaysShowZeroLevel() {
			return false;
		}
		public override bool ShouldSerializeSideMarginsEnabled() {
			return false;
		}
		public override void Assign(ChartElement obj) {
			base.Assign(obj);
			AxisRangeDataEx axisRangeDataEx = obj as AxisRangeDataEx;
			if (axisRangeDataEx != null) {
				auto = axisRangeDataEx.Auto;
				minValue = axisRangeDataEx.minValue;
				maxValue = axisRangeDataEx.maxValue;
				minValueInternal = axisRangeDataEx.minValueInternal;
				maxValueInternal = axisRangeDataEx.maxValueInternal;
				alwaysShowZeroLevel = axisRangeDataEx.AlwaysShowZeroLevel;
				sideMarginsEnabled = axisRangeDataEx.SideMarginsEnabled;				
			}
			else {
				AxisRangeData data = obj as AxisRangeData;
				if (data != null) {
					auto = data.Auto;
					minValue = data.MinValue;
					maxValue = data.MaxValue;
					minValueInternal = data.MinValueInternal;
					maxValueInternal = data.MaxValueInternal;
					alwaysShowZeroLevel = data.AlwaysShowZeroLevel;
					sideMarginsEnabled = data.SideMarginsEnabled;					
				}
			}
		}
		protected override ChartElement CreateObjectForClone() {
			return new AxisRangeDataEx(null, null);
		}
	}
}
