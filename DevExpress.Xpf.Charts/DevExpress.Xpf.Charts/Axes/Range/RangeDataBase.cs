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
using DevExpress.Xpf.Charts.Localization;
using DevExpress.Xpf.Charts.Native;
using System;
using System.Globalization;
namespace DevExpress.Xpf.Charts.Native {
	abstract class RangeDataBase : ChartElement, IAxisRangeData, IMinMaxValues {
		#region inner classes
		class RangeChangeChecker {
			readonly RangeDataBase range;
			RangeSnapshot oldRange;
			RangeSnapshot newRange;
			public bool Changed { get { return newRange != null && oldRange != null && !newRange.IsSame(oldRange); } }
			public RangeChangeChecker(RangeDataBase range) {
				this.range = range;
			}
			void Clear() {
				oldRange = null;
				newRange = null;
			}
			void ClearAnotheChecker() {
				if (range.Axis.WholeRangeData != range)
					range.Axis.WholeRangeData.rangeChangeChecker.Clear();
				else
					range.Axis.VisualRangeData.rangeChangeChecker.Clear();
			}
			public void UpdateMeasureUnit() {
				if (Changed) {
					if (!range.Axis.IsValuesAxis)
						range.Axis.UpdateAutoMeasureUnit();
					Clear();
					ClearAnotheChecker();
				}
			}
			public void СacheOldValues() {
				if (oldRange == null) {
					Clear();
					oldRange = new RangeSnapshot(range);
				}
			}
			public void СacheNewValues() {
				if (oldRange != null)
					newRange = new RangeSnapshot(range);
			}
		}
		#endregion
		protected internal void RaiseControlChanged() {
			RaiseControlChanged(new ChartUpdate(this, ChartElementChange.None));
		}
		protected internal void RaiseControlChanged(ChartUpdate changeInfo) { }
		static void ThrowIncorrectPropertyValueException(object propertyValue, string propertyName) {
			string message = String.Format(ChartLocalizer.GetString(ChartStringId.MsgIncorrectPropertyUsage),
				propertyValue == null ? String.Empty : propertyValue.ToString(), propertyName);
			throw new ArgumentException(message);
		}
		readonly IAxisData axis;
		readonly RangeChangeChecker rangeChangeChecker;
		SideMarginMode autoSideMarginsValue = SideMarginMode.Enable;
		object min;
		object max;
		double minInternal = double.NaN;
		double maxInternal = double.NaN;
		double minRefined = double.NaN;
		double maxRefined = double.NaN;
		double sideMarginsMin = 0;
		double sideMarginsMax = 0;
		double sideMargins;
		bool autoMin = true;
		bool autoMax = true;
		RangeCorrectionMode correctionModeValue;
		Action updateDependencyProperty;
		public AxisBase Axis { get { return axis as AxisBase; } }
		public virtual double SideMarginsValue {
			get { return sideMargins; }
			set {
				if (sideMargins != value && !double.IsNaN(value)) {
					OnSideMarginsValueChanged();
					sideMargins = value;
					autoSideMarginsValue = SideMarginMode.UserDisable;
					ChartElementHelper.Update(this);
					UpdateMeasureUnit();
				}
				else if (value == 0) {
					OnSideMarginsValueChanged();
					AutoSideMargins = SideMarginMode.UserDisable;
					UpdateMeasureUnit();
				}
			}
		}
		public virtual SideMarginMode AutoSideMargins {
			get { return autoSideMarginsValue; }
			set {
				if (value != autoSideMarginsValue) {
					autoSideMarginsValue = value;
					ChartElementHelper.Update(this);
				}
			}
		}
		public virtual object MinValue {
			get { return min; }
			set {
				if (value != null && !value.Equals(min)) {
					СacheOldValues();
					sideMarginsMin = 0;
					autoMin = false;
					correctionModeValue = RangeCorrectionMode.Values;
					min = value;
					minInternal = double.NaN;
					minRefined = axis.AxisScaleTypeMap.NativeToRefined(min);
					if (minRefined>maxRefined)
						maxRefined = axis.AxisScaleTypeMap.NativeToRefined(max);
					ChartElementHelper.Update(this);
					UpdateMeasureUnit();
				}
			}
		}
		public virtual object MaxValue {
			get { return max; }
			set {
				if (value != null && !value.Equals(max)) {
					СacheOldValues();
					sideMarginsMax = 0;
					autoMax = false;
					correctionModeValue = RangeCorrectionMode.Values;
					max = value;
					maxInternal = double.NaN;
					maxRefined = axis.AxisScaleTypeMap.NativeToRefined(max);
					if (minRefined > maxRefined)
						minRefined = axis.AxisScaleTypeMap.NativeToRefined(min);
					ChartElementHelper.Update(this);
					UpdateMeasureUnit();
				}
			}
		}
		public virtual double MinValueInternal {
			get { return minInternal; }
			set {
				if (minInternal != value && !(double.IsNaN(value) && double.IsNaN(minInternal))) {
					СacheOldValues();
					sideMarginsMin = 0;
					autoMin = false;
					correctionModeValue = RangeCorrectionMode.InternalValues;
					min = null;
					minInternal = value;
					minRefined = axis.AxisScaleTypeMap.InternalToRefinedExact(minInternal);
					ChartElementHelper.Update(this);
					UpdateMeasureUnit();
				}
			}
		}
		public virtual double MaxValueInternal {
			get { return maxInternal; }
			set {
				if (maxInternal != value && !(double.IsNaN(value) && double.IsNaN(maxInternal))) {
					СacheOldValues();
					sideMarginsMax = 0;
					autoMax = false;
					correctionModeValue = RangeCorrectionMode.InternalValues;
					max = null;
					maxInternal = value;
					maxRefined = axis.AxisScaleTypeMap.InternalToRefinedExact(maxInternal);
					ChartElementHelper.Update(this);
					UpdateMeasureUnit();
				}
			}
		}
		public bool AutoMin {
			get { return autoMin; }
			set {
				if (value != autoMin) {
					PropertyUpdateInfo<bool> propertyUpdateInfo = new PropertyUpdateInfo<bool>(this, "AutoMin", autoMin, value);
					autoMin = value;
					correctionModeValue = autoMin && autoMax ? RangeCorrectionMode.Auto : correctionModeValue;
					RaiseControlChanged();
				}
			}
		}
		public bool AutoMax {
			get { return autoMax; }
			set {
				if (value != autoMax) {
					PropertyUpdateInfo<bool> propertyUpdateInfo = new PropertyUpdateInfo<bool>(this, "AutoMax", autoMax, value);
					autoMax = value;
					correctionModeValue = autoMin && autoMax ? RangeCorrectionMode.Auto : correctionModeValue;
					RaiseControlChanged();
				}
			}
		}
		public RangeCorrectionMode CorrectionMode {
			get { return correctionModeValue; }
			set {
				if (correctionModeValue != value) {
					СacheOldValues();
					correctionModeValue = value;
					autoMin = value == RangeCorrectionMode.Auto;
					autoMax = value == RangeCorrectionMode.Auto;
					ChartElementHelper.Update(this);
				}
			}
		}
		public virtual bool AlwaysShowZeroLevel { get { return true; } set { } }
		public RangeDataBase(IAxisData axis) {
			ChangeOwner(null, axis as IChartElement);
			this.axis = axis;
			if (!axis.IsValueAxis)
				rangeChangeChecker = new RangeChangeChecker(this);
		}
		#region IAxisRangeData Implementation
		bool IAxisRangeData.Auto { get { return correctionModeValue == RangeCorrectionMode.Auto; } }
		object IAxisRangeData.MinValue { get { return min; } set { min = value; } }
		object IAxisRangeData.MaxValue { get { return max; } set { max = value; } }
		bool IAxisRangeData.AlwaysShowZeroLevel { get { return AlwaysShowZeroLevel; } set { AlwaysShowZeroLevel = value; } }
		SideMarginMode IAxisRangeData.AutoSideMargins {
			get { return autoSideMarginsValue; }
			set {
				autoSideMarginsValue = value;
				if (value == SideMarginMode.Disable || value == SideMarginMode.UserDisable) {
					sideMarginsMin = 0;
					sideMarginsMax = 0;
				}
			}
		}
		double IAxisRangeData.SideMarginsValue { get { return sideMargins; } set { sideMargins = value; } }
		bool IAxisRangeData.AutoCorrectMax { get { return autoMax; } set { autoMax = value; } }
		bool IAxisRangeData.AutoCorrectMin { get { return autoMin; } set { autoMin = value; } }
		double IAxisRangeData.SideMarginsMin { get { return sideMarginsMin; } set { sideMarginsMin = value; } }
		double IAxisRangeData.SideMarginsMax { get { return sideMarginsMax; } set { sideMarginsMax = value; } }
		double IAxisRangeData.RefinedMin { get { return minRefined; } set { minRefined = value; } }
		double IAxisRangeData.RefinedMax { get { return maxRefined; } set { maxRefined = value; } }
		RangeCorrectionMode IAxisRangeData.CorrectionMode { get { return correctionModeValue; } set { correctionModeValue = value; } }
		void IAxisRangeData.Reset(bool needUpdate) { Reset(needUpdate); }
		bool IAxisRangeData.Contains(double value) { return MinValueInternal <= value && value <= MaxValueInternal; }
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
		double IMinMaxValues.Max { get { return maxInternal; } set { maxInternal = value; } }
		double IMinMaxValues.Min { get { return minInternal; } set { minInternal = value; } }
		double IMinMaxValues.Delta { get { return Math.Abs(maxInternal - minInternal); } }
		double IMinMaxValues.CalculateCenter() { return MinMaxValues.CalculateCenter(this); }
		void IMinMaxValues.Intersection(IMinMaxValues minMaxValues) {
			MinMaxValues intersection = MinMaxValues.Intersection(this, minMaxValues);
			this.maxInternal = intersection.Max;
			this.minInternal = intersection.Min;
		}
		#endregion
		void СacheOldValues() {
			if (rangeChangeChecker != null)
				rangeChangeChecker.СacheOldValues();
		}
		void СacheNewValues() {
			if (rangeChangeChecker != null)
				rangeChangeChecker.СacheNewValues();
		}
		internal void UpdateMeasureUnit() {
			if (rangeChangeChecker != null)
				rangeChangeChecker.UpdateMeasureUnit();
		}
		internal void SetUpdateDependencyPropertyDelegate(Action update) { updateDependencyProperty = update; }
		protected void UpdateRange(object min, object max, double internalMin, double internalMax) {
			СacheOldValues();
			this.min = min;
			this.max = max;
			this.minInternal = internalMin;
			this.maxInternal = internalMax;
			INotificationOwner notificationOwner = ((IOwnedElement)this).Owner as INotificationOwner;
			СacheNewValues();
			updateDependencyProperty();
		}
		protected virtual void OnSideMarginsValueChanged() { }
		public virtual void Assign(RangeDataBase axisRange) {
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
			this.autoMax = axisRange.autoMax;
			this.autoMin = axisRange.autoMin;
			this.sideMarginsMax = axisRange.sideMarginsMax;
			this.sideMarginsMin = axisRange.sideMarginsMin;
			this.sideMargins = axisRange.sideMargins;
		}
		public virtual void Reset(bool needUpdate) {
			autoMax = true;
			autoMin = true;
			correctionModeValue = RangeCorrectionMode.Auto;
			if (needUpdate)
				ChartElementHelper.Update((IChartElement)axis, new RangeResetUpdateInfo(this));
		}
		public virtual void SetRange(object min, object max, double internalMin, double internalMax, bool needUpdate, bool isRangeControl = false) {
			autoMin = false;
			autoMax = false;
			if (min != null || max != null)
				correctionModeValue = RangeCorrectionMode.Values;
			if (!double.IsNaN(internalMin) || !double.IsNaN(internalMax))
				correctionModeValue = RangeCorrectionMode.InternalValues;
			СacheOldValues();
			sideMarginsMin = 0;
			sideMarginsMax = 0;
			this.min = min;
			this.max = max;
			this.minInternal = internalMin;
			this.maxInternal = internalMax;
			if (axis.AxisScaleTypeMap != null) {
				if (min != null && max != null) {
					this.minRefined = axis.AxisScaleTypeMap.NativeToRefined(min);
					this.maxRefined = axis.AxisScaleTypeMap.NativeToRefined(max);
				}
				else if (!double.IsNaN(internalMin) && !double.IsNaN(internalMax)) {
					this.minRefined = axis.AxisScaleTypeMap.InternalToRefinedExact(internalMin);
					this.maxRefined = axis.AxisScaleTypeMap.InternalToRefinedExact(internalMax);
				}
			}
			if (needUpdate)
				ChartElementHelper.Update(this);
			СacheNewValues();
			UpdateMeasureUnit();
			updateDependencyProperty();
			ChartElementHelper.Update((IChartElement)this, new RangeChangedUpdateInfo(this, axis, isRangeControl, axis.IsArgumentAxis));
		}
	}
}
