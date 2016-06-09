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

using DevExpress.Charts.Native;
using DevExpress.Charts.NotificationCenter;
using DevExpress.Utils.Serializing;
using DevExpress.XtraCharts.Localization;
using DevExpress.XtraCharts.Native;
using System;
using System.ComponentModel;
using System.Globalization;
namespace DevExpress.XtraCharts.Native {
	public abstract class RangeDataBase : ChartElement, IAxisRangeData, IMinMaxValues {
		#region inner classes
		public class Deserializer {
			protected readonly AxisBase axis;
			object minSerializable;
			object maxSerializable;
			double minInternal = double.NaN;
			double maxInternal = double.NaN;
			SideMarginMode autoSideMargins = SideMarginMode.Enable;
			double sideMarginsValue = 0;
			RangeCorrectionMode correctionMode = RangeCorrectionMode.Auto;
			public bool HasValue {
				get {
					return autoSideMargins != SideMarginMode.Enable ||
						!double.IsNaN(minInternal) ||
						!double.IsNaN(maxInternal) ||
						minSerializable != null ||
						maxSerializable != null ||
						correctionMode != RangeCorrectionMode.Auto;
				}
			}
			public bool AutoSideMargins { set { autoSideMargins = value ? SideMarginMode.UserEnable : SideMarginMode.UserDisable; } }
			public double SideMarginsValue { set { sideMarginsValue = value; } }
			public double MinInternal { set { minInternal = value; } }
			public double MaxInternal { set { maxInternal = value; } }
			public object MinValueSerializable { set { minSerializable = value; } get { return minSerializable; } }
			public object MaxValueSerializable { set { maxSerializable = value; } get { return maxSerializable; } }
			public RangeCorrectionMode CorrectionMode { set { correctionMode = value; } }
			public Deserializer(AxisBase axis) {
				this.axis = axis;
			}
			bool TryConvertToNativeValue(object value, CultureInfo culture, double defaultInternal, out object nativeValue) {
				AxisScaleTypeMap map = axis.ScaleTypeMap;
				if (value == null) {
					double valueInternal = defaultInternal;
					if (axis.ScaleTypeMap.ScaleType == ActualScaleType.DateTime)
						valueInternal += ((AxisDateTimeMap)axis.ScaleTypeMap).Min;
					nativeValue = map.InternalToNative(valueInternal);
				}
				else
					nativeValue = map.ConvertValue(value, culture);
				if (!map.IsCompatible(nativeValue)) {
					nativeValue = null;
					return false;
				}
				return true;
			}
			protected virtual bool Deserialize(RangeDataBase range, out string errorMessage) {
				errorMessage = null;
				if (minSerializable != null || maxSerializable != null ||
					!double.IsNaN(minInternal) || !double.IsNaN(maxInternal)) {
					string firstPartOfRangePropertyName = range is WholeRangeData ? "Whole" : "Visual";
					object min;
					bool getMinSuccess = TryGetNativeValue(minSerializable, minInternal, 0, out min);
					if (!getMinSuccess) {
						errorMessage = BuildErrorMessageString(min, firstPartOfRangePropertyName + "Range.MinValue");
						return false;
					}
					object max;
					bool getMaxSuccess = TryGetNativeValue(maxSerializable, maxInternal, 1, out max);
					if (!getMaxSuccess) {
						errorMessage = BuildErrorMessageString(min, firstPartOfRangePropertyName + "Range.MaxValue");
						return false;
					}
					double minValueInternal = axis.ScaleTypeMap.NativeToInternal(min);
					double maxValueInternal = axis.ScaleTypeMap.NativeToInternal(max);
					if (double.IsNaN(minValueInternal) || double.IsNaN(maxValueInternal))
						return false;
					if (minValueInternal > maxValueInternal) {
						object temp = min;
						min = max;
						max = temp;
					}
					range.min = min;
					range.max = max;
					range.minRefined = axis.ScaleTypeMap.NativeToRefined(min);
					range.maxRefined = axis.ScaleTypeMap.NativeToRefined(max);
					range.correctionModeValue = RangeCorrectionMode.Values;
					range.autoMin = false;
					range.autoMax = false;
				}
				range.autoSideMarginsValue = autoSideMargins;
				if (autoSideMargins != SideMarginMode.UserDisable && autoSideMargins != SideMarginMode.Disable)
					range.sideMargins = sideMarginsValue;
				return true;
			}
			string BuildErrorMessageString(object propertyValue, string propertyName) {
				string messageBase = ChartLocalizer.GetString(ChartStringId.MsgIncorrectPropertyValue);
				string propValueAsString = propertyValue == null ? "null" : propertyValue.ToString();
				string message = String.Format(messageBase, propValueAsString, propertyName);
				return message;
			}
			bool TryGetNativeValue(object value, double internalValue, double defaultInternal, out object nativeValue) {
				if (value != null) {
					object result;
					bool successfullConvertion = TryConvertToNativeValue(value, CultureInfo.InvariantCulture, defaultInternal, out result);
					if (!successfullConvertion) {
						nativeValue = null;
						return false;
					}
					else {
						nativeValue = result;
						return true;
					}
				}
				else if (!double.IsNaN(internalValue)) {
					if (axis.ScaleTypeMap.ScaleType == ActualScaleType.DateTime)
						internalValue = minInternal + ((AxisDateTimeMap)axis.ScaleTypeMap).Min;
					nativeValue = axis.ScaleTypeMap.InternalToNative(internalValue);
					return true;
				}
				else {
					if (axis.ScaleTypeMap.ScaleType == ActualScaleType.DateTime)
						defaultInternal = defaultInternal + ((AxisDateTimeMap)axis.ScaleTypeMap).Min;
					nativeValue = axis.ScaleTypeMap.InternalToNative(defaultInternal);
					return true;
				}
			}
			internal void Assign(object obj) {
				Deserializer deserializer = obj as Deserializer;
				if (deserializer == null)
					return;
				this.minSerializable = deserializer.minSerializable;
				this.maxSerializable = deserializer.maxSerializable;
				this.autoSideMargins = deserializer.autoSideMargins;
				this.sideMarginsValue = deserializer.sideMarginsValue;
			}
			public bool TryDeserialize(out string errorMessage, params RangeDataBase[] ranges) {
				errorMessage = null;			   
				for (int i = 0; i < ranges.Length; i++) {
					Deserialize(ranges[i], out errorMessage);
					if (errorMessage != null) {
						return false;
					}
				}
				return true;
			}
		}
		public class RangeChangedEventNotification : Notification {
			readonly RangeSnapshot oldRange;
			readonly RangeSnapshot newRange;
			public RangeDataBase Range {
				get {
					return (RangeDataBase)Sender;
				}
			}
			public RangeSnapshot OldRange {
				get {
					return oldRange;
				}
			}
			public RangeSnapshot NewRange {
				get {
					return newRange;
				}
			}
			public RangeChangedEventNotification(RangeDataBase sender, RangeSnapshot oldRange, RangeSnapshot newRange) : base(sender, null) {
				this.oldRange = oldRange;
				this.newRange = newRange;
			}
		}
		class EventFiring {
			readonly RangeDataBase range;
			RangeSnapshot oldRange;
			RangeSnapshot newRange;
			public bool Changed { get { return newRange != null && oldRange != null && !newRange.IsSame(oldRange); } }
			public EventFiring(RangeDataBase range) {
				this.range = range;
			}
			void Clear() {
				oldRange = null;
				newRange = null;
			}
			public void RunEvent() {
				if (Changed) {
					if (!range.Axis.IsValuesAxis)
						range.Axis.UpdateAutoMeasureUnit(true);
					range.RaiseRangeChanged(oldRange, newRange);
					Clear();
				}
			}
			public void CacheOldValues() {
				if (oldRange == null) {
					Clear();
					oldRange = new RangeSnapshot(range);
				}
			}
			public void CacheNewValues() {
				if (oldRange != null)
					newRange = new RangeSnapshot(range);
			}
		}
		#endregion
		static void ThrowIncorrectPropertyValueException(object propertyValue, string propertyName) {
			string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPropertyValue),
				propertyValue == null ? String.Empty : propertyValue.ToString(), propertyName);
			throw new ArgumentException(message);
		}
		readonly AxisBase axis;
		readonly EventFiring eventFiring;
		SideMarginMode autoSideMarginsValue = SideMarginMode.Enable;
		bool alwaysShowZeroLevel = true;
		object min;
		object max;
		double minInternal = double.NaN;
		double maxInternal = double.NaN;
		double minRefined = double.NaN;
		double maxRefined = double.NaN;
		double sideMarginsMinValue = 0;
		double sideMarginsMaxValue = 0;
		protected double sideMargins;
		bool autoMin = true;
		bool autoMax = true;
		protected RangeCorrectionMode correctionModeValue;
		double Delta { get { return Math.Abs(maxInternal - minInternal); } }
		protected bool CanClearAuto { get { return axis == null || (axis.ActualDateTimeScaleMode != ScaleMode.Automatic); } }
		public AxisBase Axis { get { return axis; } }
		public double SideMarginsMin {
			get { return sideMarginsMinValue; }
			set { sideMarginsMinValue = value; }
		}
		public double SideMarginsMax {
			get { return sideMarginsMaxValue; }
			set { sideMarginsMaxValue = value; }
		}
		public virtual double SideMarginsValue {
			get { return sideMargins; }
			set {
				if (sideMargins != value) {
					SendNotification(new ElementWillChangeNotification(this));
					sideMargins = value;
					autoSideMarginsValue = SideMarginMode.UserDisable;
					RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.RangeChanged | ChartElementChange.NonSpecific));
					if (!axis.IsValuesAxis)
						axis.UpdateAutoMeasureUnit(true);
				}
				else if (value == 0) {
					AutoSideMargins = SideMarginMode.UserDisable;
				}
			}
		}
		public virtual SideMarginMode AutoSideMargins {
			get { return autoSideMarginsValue; }
			set {
				if (value != autoSideMarginsValue) {
					SendNotification(new ElementWillChangeNotification(this));
					autoSideMarginsValue = value;
					RaiseControlChanged();
				}
			}
		}
		public virtual object MinValue {
			get { return min; }
			set {
				CheckMinValue(value);
				if (min != value) {
					if (!axis.Loading) {
						if (!CanClearAuto)
							throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgUnsupportedManualRangeForAutomaticDateTimeScaleMode));
					}
					SendNotification(new ElementWillChangeNotification(this));
					eventFiring.CacheOldValues();
					SideMarginsMin = 0;
					autoMin = false;
					correctionModeValue = RangeCorrectionMode.Values;
					min = value;
					minInternal = double.NaN;
					minRefined = axis.ScaleTypeMap.NativeToRefined(min);
					RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.RangeChanged | ChartElementChange.NonSpecific));
					if (!axis.IsValuesAxis)
						axis.UpdateAutoMeasureUnit(true);
				}
			}
		}
		public virtual object MaxValue {
			get { return max; }
			set {
				CheckMaxValue(value);
				if (max != value) {
					if (!axis.Loading) {
						if (!CanClearAuto)
							throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgUnsupportedManualRangeForAutomaticDateTimeScaleMode));
					}
					SendNotification(new ElementWillChangeNotification(this));
					eventFiring.CacheOldValues();
					SideMarginsMax = 0;
					autoMax = false;
					correctionModeValue = RangeCorrectionMode.Values;
					max = value;
					maxInternal = double.NaN;
					maxRefined = axis.ScaleTypeMap.NativeToRefined(max);
					RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.RangeChanged | ChartElementChange.NonSpecific));
					if (!axis.IsValuesAxis)
						axis.UpdateAutoMeasureUnit(true);
				}
			}
		}
		public virtual double MinValueInternal {
			get { return minInternal; }
			set {
				if (maxInternal != value && !(double.IsNaN(value) && double.IsNaN(maxInternal))) {
					CheckMinValueInternal(value);
					if (!Loading && !CanClearAuto)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgUnsupportedManualRangeForAutomaticDateTimeScaleMode));
					SendNotification(new ElementWillChangeNotification(this));
					eventFiring.CacheOldValues();
					SideMarginsMin = 0;
					autoMin = false;
					correctionModeValue = RangeCorrectionMode.InternalValues;
					min = null;
					minInternal = value;
					minRefined = axis.ScaleTypeMap.InternalToRefinedExact(minInternal);
					RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.RangeChanged | ChartElementChange.NonSpecific));
					if (!axis.IsValuesAxis)
						axis.UpdateAutoMeasureUnit(true);
				}
			}
		}
		public virtual double MaxValueInternal {
			get { return maxInternal; }
			set {
				if (maxInternal != value && !(double.IsNaN(value) && double.IsNaN(maxInternal))) {
					CheckMaxValueInternal(value);
					if (!Loading && !CanClearAuto)
						throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgUnsupportedManualRangeForAutomaticDateTimeScaleMode));
					SendNotification(new ElementWillChangeNotification(this));
					eventFiring.CacheOldValues();
					SideMarginsMax = 0;
					autoMax = false;
					correctionModeValue = RangeCorrectionMode.InternalValues;
					max = null;
					maxInternal = value;
					maxRefined = axis.ScaleTypeMap.InternalToRefinedExact(maxInternal);
					RaiseControlChanged(new ChartElementUpdateInfo(this, ChartElementChange.RangeChanged | ChartElementChange.NonSpecific));
					if (!axis.IsValuesAxis)
						axis.UpdateAutoMeasureUnit(true);
				}
			}
		}
		public bool AutoMin {
			get { return autoMin; }
			set {
				if (value != autoMin) {
					SendNotification(new ElementWillChangeNotification(this));
					autoMin = value;
					correctionModeValue = autoMin && autoMax ? RangeCorrectionMode.Auto : correctionModeValue;
					if (!value) {
						if (!Loading && !CanClearAuto)
							throw new ArgumentException();
					}
					RaiseControlChanged();
				}
			}
		}
		public bool AutoMax {
			get { return autoMax; }
			set {
				if (value != autoMax) {
					SendNotification(new ElementWillChangeNotification(this));
					autoMax = value;
					correctionModeValue = AutoMin && AutoMax ? RangeCorrectionMode.Auto : correctionModeValue;
					if (!value) {
						if (!Loading && !CanClearAuto)
							throw new ArgumentException();
					}
					RaiseControlChanged();
				}
			}
		}
		public RangeCorrectionMode CorrectionMode {
			get { return correctionModeValue; }
			set {
				if (correctionModeValue != value) {
					if (value != RangeCorrectionMode.Auto && !Loading && !CanClearAuto)
						throw new ArgumentException();
					SendNotification(new ElementWillChangeNotification(this));
					eventFiring.CacheOldValues();
					correctionModeValue = value;
					autoMin = value == RangeCorrectionMode.Auto;
					autoMax = value == RangeCorrectionMode.Auto;
					RaiseControlChanged();
					if (!axis.IsValuesAxis)
						axis.UpdateAutoMeasureUnit(true);
				}
			}
		}
		public bool AlwaysShowZeroLevel {
			get { return alwaysShowZeroLevel; }
			set {
				if (alwaysShowZeroLevel != value) {
					SendNotification(new ElementWillChangeNotification(this));
					alwaysShowZeroLevel = value;
					RaiseControlChanged();
				}
			}
		}
		public RangeDataBase(AxisBase axis) : base(axis) {
			this.axis = axis;
			eventFiring = new EventFiring(this);
		}
		#region IAxisRangeData Implementation
		bool IAxisRangeData.Auto { get { return CorrectionMode == RangeCorrectionMode.Auto; } }
		object IAxisRangeData.MinValue { get { return min; } set { min = value; } }
		object IAxisRangeData.MaxValue { get { return max; } set { max = value; } }
		bool IAxisRangeData.AlwaysShowZeroLevel {
			get { return alwaysShowZeroLevel; }
			set { alwaysShowZeroLevel = value; }
		}
		SideMarginMode IAxisRangeData.AutoSideMargins {
			get { return autoSideMarginsValue; }
			set {
				autoSideMarginsValue = value;
				if (value == SideMarginMode.Disable || value == SideMarginMode.UserDisable) {
					sideMarginsMinValue = 0;
					sideMarginsMaxValue = 0;
				}
			}
		}
		double IAxisRangeData.SideMarginsValue {
			get { return sideMargins; }
			set {
				sideMargins = value;
			}
		}
		bool IAxisRangeData.AutoCorrectMax { get { return autoMax; } set { autoMax = value; } }
		bool IAxisRangeData.AutoCorrectMin { get { return autoMin; } set { autoMin = value; } }
		double IAxisRangeData.SideMarginsMin {
			get { return sideMarginsMinValue; }
			set { sideMarginsMinValue = value; }
		}
		double IAxisRangeData.SideMarginsMax {
			get { return sideMarginsMaxValue; }
			set { sideMarginsMaxValue = value; }
		}
		double IAxisRangeData.RefinedMin { get { return minRefined; } set { minRefined = value; } }
		double IAxisRangeData.RefinedMax { get { return maxRefined; } set { maxRefined = value; } }
		RangeCorrectionMode IAxisRangeData.CorrectionMode { get { return correctionModeValue; } set { correctionModeValue = value; } }
		void IAxisRangeData.Reset(bool needUpdate) { Reset(needUpdate); }
		bool IAxisRangeData.Contains(double value) { return Contains(value); }
		void IAxisRangeData.UpdateRange(object min, object max, double internalMin, double internalMax) {
			UpdateRange(min, max, internalMin, internalMax);
		}
		void IAxisRangeData.ApplyState(RangeSnapshot rangeSnapshot) {
			this.minInternal = rangeSnapshot.Min;
			this.maxInternal = rangeSnapshot.Max;
			this.min = rangeSnapshot.MinValue;
			this.max = rangeSnapshot.MaxValue;
		}
		#endregion
		#region IMinMaxValues Implementation
		double IMinMaxValues.Max {
			get { return maxInternal; }
			set { maxInternal = value; }
		}
		double IMinMaxValues.Min {
			get { return minInternal; }
			set { minInternal = value; }
		}
		double IMinMaxValues.Delta { get { return Math.Abs(maxInternal - minInternal); } }
		double IMinMaxValues.CalculateCenter() {
			return MinMaxValues.CalculateCenter(this);
		}
		void IMinMaxValues.Intersection(IMinMaxValues minMaxValues) {
			MinMaxValues intersection = MinMaxValues.Intersection(this, minMaxValues);
			this.maxInternal = intersection.Max;
			this.minInternal = intersection.Min;
		}
		#endregion
		bool Contains(double value) {
			return MinValueInternal <= value && value <= MaxValueInternal;
		}
		void CheckMinValue(object value) {
			if (value == null)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisRangeMinValue));
		}
		void CheckMaxValue(object value) {
			if (value == null)
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisRangeMaxValue));
		}
		void CheckMinValueInternal(double value) {
			if (Double.IsNaN(value) || Double.IsInfinity(value))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisRangeMinValueInternal));
		}
		void CheckMaxValueInternal(double value) {
			if (Double.IsNaN(value) || Double.IsInfinity(value))
				throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisRangeMaxValueInternal));
		}
		void CheckRange(object min, object max, double internalMin, double internalMax) {
			if (min != null && max != null)
				if (((IAxisData)axis).AxisScaleTypeMap.NativeToInternal(min) > ((IAxisData)axis).AxisScaleTypeMap.NativeToInternal(max))
					GenerateAxisRangeException();
			if (!double.IsNaN(internalMin) && !double.IsNaN(internalMax))
				if (internalMin > internalMax)
					GenerateAxisRangeException();
		}
		void GenerateAxisRangeException() {
			throw new ArgumentException(ChartLocalizer.GetString(ChartStringId.MsgIncorrectAxisRange));
		}
		protected void UpdateRange(object min, object max, double internalMin, double internalMax) {
			eventFiring.CacheOldValues();
			this.min = min;
			this.max = max;
			this.minInternal = internalMin;
			this.maxInternal = internalMax;
			eventFiring.CacheNewValues();
		}
		void Assign(AxisRangeData axisRange) {
			if (axisRange == null)
				return;
			if (!axisRange.Auto) {
				this.min = axisRange.MinValue;
				this.max = axisRange.MaxValue;
				if (axis.ScaleTypeMap.ScaleType == ActualScaleType.DateTime) {
					this.minInternal = axisRange.MinValueInternal + ((AxisDateTimeMap)axis.ScaleTypeMap).Min;
					this.maxInternal = axisRange.MaxValueInternal + ((AxisDateTimeMap)axis.ScaleTypeMap).Min;
				}
				else {
					this.minInternal = axisRange.MinValueInternal;
					this.maxInternal = axisRange.MaxValueInternal;
				}
			}
			this.autoSideMarginsValue = axisRange.SideMarginsEnabled ? SideMarginMode.Enable : SideMarginMode.Disable;
		}
		void Assign(RangeDataBase axisRange) {
			if (axisRange == null)
				return;
			this.correctionModeValue = axisRange.correctionModeValue;
			this.min = axisRange.min;
			this.max = axisRange.max;
			this.minInternal = axisRange.minInternal;
			this.maxInternal = axisRange.maxInternal;
			this.minRefined = axisRange.minRefined;
			this.maxRefined = axisRange.maxRefined;
			this.autoSideMarginsValue = axisRange.autoSideMarginsValue;
			this.alwaysShowZeroLevel = axisRange.alwaysShowZeroLevel;
			this.autoMax = axisRange.autoMax;
			this.autoMin = axisRange.autoMin;
			this.sideMarginsMaxValue = axisRange.sideMarginsMaxValue;
			this.sideMarginsMinValue = axisRange.sideMarginsMinValue;
			this.sideMargins = axisRange.sideMargins;
		}
		internal abstract void RaiseRangeChanged(RangeSnapshot oldRange, RangeSnapshot newRange);
		internal abstract Deserializer GetDeserializer();
		public void RaiseChangedEventIfNecessary() {
			eventFiring.RunEvent();
		}
		public virtual void Reset(bool needUpdate) {
			if (needUpdate)
				this.SendNotification(new ElementWillChangeNotification(this));
			autoMax = true;
			autoMin = true;
			correctionModeValue = RangeCorrectionMode.Auto;
			if (needUpdate)
				this.RaiseControlChanged(new RangeResetUpdateInfo(this));
		}
		public virtual void SetRange(object min, object max, double internalMin, double internalMax, bool needUpdate, bool isRangeControl = false) {
			autoMin = false;
			autoMax = false;
			if (min != null || max != null) {
				CheckMinValue(min);
				CheckMaxValue(max);
				correctionModeValue = RangeCorrectionMode.Values;
			}
			if (!double.IsNaN(internalMin) || !double.IsNaN(internalMax)) {
				CheckMinValueInternal(internalMin);
				CheckMaxValueInternal(internalMax);
				correctionModeValue = RangeCorrectionMode.InternalValues;
			}
			CheckRange(min, max, internalMin, internalMax);
			if (needUpdate)
				SendNotification(new ElementWillChangeNotification(this));
			eventFiring.CacheOldValues();
			sideMarginsMinValue = 0;
			sideMarginsMaxValue = 0;
			this.min = min;
			this.max = max;
			this.minInternal = internalMin;
			this.maxInternal = internalMax;
			this.minRefined = axis.ScaleTypeMap.NativeToRefined(min);
			this.maxRefined = axis.ScaleTypeMap.NativeToRefined(max);
			if (needUpdate)
				RaiseControlChanged();
			eventFiring.CacheNewValues();
			RaiseControlChanged(new RangeChangedUpdateInfo(this, this.axis, isRangeControl, !axis.IsValuesAxis));
		}
		public override void Assign(ChartElement obj) {
			RangeDataBase axisRange = obj as RangeDataBase;
			if (axisRange != null) {
				Assign(axisRange);
				return;
			}
			AxisRangeData rangeData = obj as AxisRangeData;
			if (rangeData != null) {
				Assign(rangeData);
				return;
			}
		}
	}
	public class WholeRangeData : RangeDataBase, IWholeAxisRangeData {
		public WholeRangeData(AxisBase axis) : base(axis) {
		}
		internal override RangeDataBase.Deserializer GetDeserializer() {
			return new Deserializer(Axis);
		}
		protected override ChartElement CreateObjectForClone() {
			return new WholeRangeData(Axis);
		}
		internal override void RaiseRangeChanged(RangeSnapshot oldRange, RangeSnapshot newRange) {
			Axis.OnWholeRangeCnanged(oldRange, newRange);
		}
	}
	public class VisualRangeData : RangeDataBase, IVisualAxisRangeData {
		public class VisualRangeDeserializer : Deserializer {
			public VisualRangeDeserializer(AxisBase axis) : base(axis) {
			}
		}
		bool synchronize = false;
		public override object MinValue {
			get { return base.MinValue; }
			set {
				synchronize = false;
				base.MinValue = value;
			}
		}
		public override object MaxValue {
			get { return base.MaxValue; }
			set {
				synchronize = false;
				base.MaxValue = value;
			}
		}
		public override double MinValueInternal {
			get { return base.MinValueInternal; }
			set {
				synchronize = false;
				base.MinValueInternal = value;
			}
		}
		public override double MaxValueInternal {
			get { return base.MaxValueInternal; }
			set {
				synchronize = false;
				base.MaxValueInternal = value;
			}
		}
		public override double SideMarginsValue {
			get { return base.SideMarginsValue; }
			set {
				correctionModeValue = RangeCorrectionMode.Values;
				base.SideMarginsValue = value;
			}
		}
		public bool SynchronizeWithWholeRange {
			get { return synchronize; }
			set {
				if (synchronize != value) {
					SendNotification(new ElementWillChangeNotification(this));
					if (value) {
						UpdateRange(null, null, Double.NaN, Double.NaN);
					}
					synchronize = value;
					RaiseControlChanged();
				}
			}
		}
		public VisualRangeData(AxisBase axis) : base(axis) {
		}
		#region IAxisRangeData
		double IAxisRangeData.SideMarginsValue {
			get { return sideMargins; }
			set {
				sideMargins = value;
			}
		}
		#endregion
		internal override RangeDataBase.Deserializer GetDeserializer() {
			return new VisualRangeDeserializer(Axis);
		}
		protected override ChartElement CreateObjectForClone() {
			return new VisualRangeData(Axis);
		}
		internal override void RaiseRangeChanged(RangeSnapshot oldRange, RangeSnapshot newRange) {
			Axis.OnVisualRangeCnanged(oldRange, newRange);
		}
		public override void Reset(bool needUpdate) {
			synchronize = true;
			base.Reset(needUpdate);
		}
		public override void SetRange(object min, object max, double internalMin, double internalMax, bool needUpdate, bool isRangeControl = false) {
			synchronize = false;
			base.SetRange(min, max, internalMin, internalMax, needUpdate, isRangeControl);
		}
		public void ScrollOrZoomRange(object min, object max, double internalMin, double internalMax) {
			ScrollOrZoomRange(min, max, internalMin, internalMax, false);
		}
		public void ScrollOrZoomRange(object min, object max, double internalMin, double internalMax, bool isRangeControl) {
			SetRange(min, max, internalMin, internalMax, false, isRangeControl);
		}
	}
}
