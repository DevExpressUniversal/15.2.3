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
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.XtraGauges.Core.Base;
using DevExpress.XtraGauges.Core.Drawing;
using DevExpress.XtraGauges.Core.Primitive;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraGauges.Core.Model {
	public enum LogarithmicBase { Binary, Decimal, Exponential, Custom }	
	public abstract class BaseScaleProvider : BaseProvider, IConvertibleScale {
		float minValueCore;
		float maxValueCore;
		float valueCore;
		bool autoRescalingCore;
		event EventHandler MinMaxValueChangedCore;
		event EventHandler ValueChangedCore;
		event CustomRescalingEventHandler CustomRescalingCore;
		bool rescalingBestValuesCore;
		float rescalingThresholdMinCore;
		float rescalingThresholdMaxCore;
		public BaseScaleProvider(OwnerChangedAction ownerChanged)
			: base(ownerChanged) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.autoRescalingCore = false;
			this.minValueCore = 0f;
			this.maxValueCore = 1f;
			this.rescalingBestValuesCore = false;
			this.rescalingThresholdMinCore = 0.05f;
			this.rescalingThresholdMaxCore = 0.05f;
		}
		protected override void OnDispose() {
			if(IsAnimating)
				CancelAnimation();
			AnimationCompleted = null;
			Animating = null;
			base.OnDispose();
		}
		public abstract bool IsDiscrete { get; }
		public abstract bool IsEmpty { get; }
		bool logarithmicCore;
		public bool IsLogarithmic {
			get { return logarithmicCore; }
		}
		public bool Logarithmic{
			get {
				return logarithmicCore;
			}
			set {
				if(logarithmicCore != value) {
					logarithmicCore = value;
					OnChangeLogarithmicProperty();
				}
			}
		}		
		protected virtual void OnChangeLogarithmicProperty() { }
		LogarithmicBase logarithmicBase = LogarithmicBase.Binary;
		public LogarithmicBase LogarithmicBase {
			get { return logarithmicBase; }
			set {
				if(logarithmicBase != value) {
					logarithmicBase = value;					
					OnChangeLogarithmicProperty();
				}
			}
		}
		protected void LogarithmicBaseToFloat(){
			switch(logarithmicBase) {
				case LogarithmicBase.Binary: customLogarithmicBaseCore = 2; break;
				case LogarithmicBase.Decimal: customLogarithmicBaseCore = 10; break;
				case LogarithmicBase.Exponential: customLogarithmicBaseCore = (float)Math.E; break;
			}
		}
		float customLogarithmicBaseCore = 2;
		protected float AbsoluteLogarithmicBase {
			get { return customLogarithmicBaseCore < 1 ? 1 / customLogarithmicBaseCore : customLogarithmicBaseCore; }
		}
		public float CustomLogarithmicBase {
			get { return customLogarithmicBaseCore; }
			set {
				if(value > 0 && value != 1 && customLogarithmicBaseCore != value) {
					customLogarithmicBaseCore = value;
					OnChangeLogarithmicProperty();
				}
			}
		}
		public event EventHandler ValueChanged {
			add {
				if(this.IsEmpty) return;
				ValueChangedCore += value;
			}
			remove {
				if(this.IsEmpty) return;
				ValueChangedCore -= value;
			}
		}
		public event EventHandler MinMaxValueChanged {
			add {
				if(this.IsEmpty) return;
				MinMaxValueChangedCore += value;
			}
			remove {
				if(this.IsEmpty) return;
				MinMaxValueChangedCore -= value;
			}
		}
		public event CustomRescalingEventHandler CustomRescaling {
			add {
				if(this.IsEmpty) return;
				CustomRescalingCore += value;
			}
			remove {
				if(this.IsEmpty) return;
				CustomRescalingCore -= value;
			}
		}
		protected void RaiseValueChanged(float prev, float current) {
			if(ValueChangedCore != null)
				ValueChangedCore(GetOwner(), new ValueChangedEventArgs(prev, current));
		}
		protected void RaiseMinMaxValueChanged(bool isMin, float prev, float current) {
			if(MinMaxValueChangedCore != null)
				MinMaxValueChangedCore(GetOwner(), new MinMaxValueChangedEventArgs(isMin, prev, current));
		}
		protected void RaiseCustomRescaling(CustomRescalingEventArgs ea) {
			if(CustomRescalingCore != null) CustomRescalingCore(ea);
		}
		public float Percent {
			get { return ValueToPercent(Value); }
		}
		public float Value {
			get {
				if(IsAnimating)
					return animationValue;
				return valueCore; }
			set {
				localValueCore = value;
				value = CheckValueBounds(value);
				if(Value == value) return;
				SetValueCore(value);
			}
		}
		float localValueCore;
		public virtual float GetInternalValue() {
			return localValueCore;
		}
		void SetValueCore(float value) {
			float prevValue = Value;
			StartAnimation(prevValue, value);
			this.valueCore = value;
			RaiseValueChanged(prevValue, Value);
		}
		protected float CoerceValueCore(float value) {
			float min = Math.Min(MinValue, MaxValue);
			float max = Math.Max(MinValue, MaxValue);
			return !float.IsNaN(value) ? Math.Min(Math.Max(value, min), max) : min;
		}
		float CheckValueBounds(float value) {
			float checkedValue = AutoRescaling ? value : CoerceValueCore(value);
			if(AutoRescaling) {
				float minLimit = MinValue + this.ScaleLength * RescalingThresholdMin;
				bool fNeedRescaleMinValue = CheckValueOutOfMinLimit(value, MinValue, MaxValue, minLimit);
				if(fNeedRescaleMinValue) {
					float newMinValue = minValueCore - (minLimit - value);
					if(RescalingBestValues) {
						newMinValue = (float)ScaleLimitCalculator.GetNearestBound(newMinValue, ScaleLength, RescalingThresholdMin, RescalingThresholdMax, false);
					}
					minValueCore = newMinValue;
				}
				float maxLimit = MaxValue - this.ScaleLength * RescalingThresholdMax;
				bool fNeedRescaleMaxValue = CheckValueOutOfMaxLimit(value, MinValue, MaxValue, maxLimit);
				if(fNeedRescaleMaxValue) {
					float newMaxValue = maxValueCore + (value - maxLimit);
					if(RescalingBestValues) {
						newMaxValue = (float)ScaleLimitCalculator.GetNearestBound(newMaxValue, ScaleLength, RescalingThresholdMin, RescalingThresholdMax, true);
					}
					maxValueCore = newMaxValue;
				}
				if(fNeedRescaleMinValue || fNeedRescaleMaxValue) {
					CustomRescalingEventArgs e = new CustomRescalingEventArgs(checkedValue, MinValue, MaxValue);
					RaiseCustomRescaling(e);
					this.minValueCore = e.MinValue;
					this.maxValueCore = e.MaxValue;
					checkedValue = e.Value;
					OnObjectChanged("MinMax");
				}
			}
			return checkedValue;
		}
		bool CheckValueOutOfMinLimit(float value, float min, float max, float minLimit) {
			return (value >= min && value < minLimit) || (value >= minLimit && value < min) || (max > min ? (value <= min) : (value >= min));
		}
		bool CheckValueOutOfMaxLimit(float value, float min, float max, float maxLimit) {
			return (value >= maxLimit && value < max) || (value >= max && value < maxLimit) || (max > min ? (value >= max) : (value <= max));
		}
		public bool AutoRescaling {
			get { return autoRescalingCore; }
			set {
				if(AutoRescaling == value) return;
				this.autoRescalingCore = value;
				OnObjectChanged("AutoRescaling");
			}
		}
		public bool RescalingBestValues {
			get { return rescalingBestValuesCore; }
			set {
				if(RescalingBestValues == value) return;
				this.rescalingBestValuesCore = value;
				if(AutoRescaling) OnObjectChanged("RescalingBestValues");
			}
		}
		public float RescalingThresholdMin {
			get { return rescalingThresholdMinCore; }
			set {
				if(RescalingThresholdMin == value) return;
				this.rescalingThresholdMinCore = value;
				if(AutoRescaling) OnObjectChanged("RescalingThresholdMin");
			}
		}
		public float RescalingThresholdMax {
			get { return rescalingThresholdMaxCore; }
			set {
				if(RescalingThresholdMax == value) return;
				this.rescalingThresholdMaxCore = value;
				if(AutoRescaling) OnObjectChanged("RescalingThresholdMax");
			}
		}
		public float MinValue {
			get { return minValueCore; }
			set {
				if(MinValue == value || float.IsNaN(value)) return;
				SetMinValueCore(value);
				OnObjectChanged("MinValue");
			}
		}
		public float MaxValue {
			get { return maxValueCore; }
			set {
				if(MaxValue == value || float.IsNaN(value)) return;
				SetMaxValueCore(value);
				OnObjectChanged("MaxValue");
			}
		}
		void SetMinValueCore(float value) {
			float prev = minValueCore;
			this.minValueCore = value;
			CoerceValue();
			RaiseMinMaxValueChanged(true, prev, minValueCore);
		}
		void SetMaxValueCore(float value) {
			float prev = maxValueCore;
			this.maxValueCore = value;
			CoerceValue();
			RaiseMinMaxValueChanged(false, prev, maxValueCore);
		}
		protected void CoerceValue() {
			if(IsUpdateLocked) return;
			float coerceResult = CoerceValueCore(GetInternalValue());
			if(coerceResult != Value) {
				SetValueCore(coerceResult);
			}
		}
		internal float MaxValueCore{
			get { return (CustomLogarithmicBase < 1 && IsLogarithmic) ? MinValue : MaxValue; }
		}
		internal float MinValueCore {
			get { return (CustomLogarithmicBase < 1 && IsLogarithmic) ? MaxValue : MinValue; }
		}
		public float ScaleLength {
			get { return MaxValueCore - MinValueCore; }
		}
		public float PercentToValue(float percent) {
			if(IsLogarithmic) return PercentToValueLogarithmicScale(percent);
			return MinValueCore + percent * ScaleLength;
		}
		public float ValueToPercent(float value) {
			float diff = value - MinValueCore;
			if(diff == 0) return 0;
			if(IsLogarithmic && ScaleLength != 0f && value != MaxValueCore && value != MinValueCore ) return ValueToPercentLogarithmicScale(value);
			return (ScaleLength != 0f) ? diff / ScaleLength : (diff > 0 ? float.PositiveInfinity : float.NegativeInfinity);
		}
		protected virtual float ValueToPercentLogarithmicScale(float value){ return 0; }
		protected virtual float PercentToValueLogarithmicScale(float percent) { return 0; }
		#region Animation
		float animationValue;
		float animationFrom;
		float animationTo;
		int frameDelay = 10000;
		int frameCount = 1000;
		EasingMode easingModeCore;
		IEasingFunction easingFunctionCore;
		AnimationInfo animationInfo;
		bool enableAnimationCore;
		public IEasingFunction EasingFunction {
			get { return easingFunctionCore; }
			set { easingFunctionCore = value; }
		}
		public EasingMode EasingMode {
			get { return easingModeCore; }
			set { easingModeCore = value; }
		}
		public event EventHandler AnimationCompleted;
		public event EventHandler Animating;
		public bool EnableAnimation {
			get { return enableAnimationCore; }
			set {
				if(EnableAnimation == value) return;
				enableAnimationCore = value;
				if(IsAnimating && !value)
					CancelAnimation();
			}
		}
		public bool IsAnimating {
			get { return animationInfo != null; }
		}
		public int FrameDelay {
			get { return frameDelay; } 
			set { frameDelay = value;} 
		}
		public int FrameCount {
			get { return frameCount; }
			set { frameCount = value; }
		}
		protected void StartAnimation(float from, float to) {
			if(!EnableAnimation) return;
			if(IsAnimating)
				CancelAnimation();
			animationValue = from;
			animationFrom = from;
			animationTo = to;
			animationInfo = new AnimationInfo();
			Animation.Utils_Animator.Current.AddObject(animationInfo, this, FrameDelay, FrameCount, OnAnimation);
		}
		protected void EndAnimation() {
			if(!IsAnimating) return;
			ClearAnimation();
			RaiseAnimationCompleted();
		}
		protected void CancelAnimation() {
			Animation.Utils_Animator.RemoveObject(animationInfo);
			ClearAnimation();
		}
		protected void RaiseAnimationCompleted() {
			if(AnimationCompleted != null)
				AnimationCompleted(this, EventArgs.Empty);
		}
		protected virtual float CheckAnimatedValue(float animatedValue) {
			return animatedValue;
		}
		void OnAnimation(Animation.Utils_AnimationInfo info) {
			if(info.CurrentFrame != 0) {
				double normalizedTime = (double)info.CurrentFrame / (double)info.FrameCount;
				float current = (float)DevExpress.Data.Utils.EaseHelper.Ease(DevExpress.Data.Utils.EaseHelper.GetEasingMode((int)EasingMode), EasingFunction, normalizedTime);
				float prevValue = animationValue;
				animationValue = CheckAnimatedValue(animationFrom + (animationTo - animationFrom) * current);
				if(Animating != null)
					Animating(GetOwner(), new ValueChangedEventArgs(prevValue, animationValue));
			}
			if(info.IsFinalFrame)
				EndAnimation();
		}
		void ClearAnimation() {
			animationInfo = null;
		}
		class AnimationInfo : Animation.IAnimationID { }
		#endregion
	}
	public abstract class BaseDiscreteScaleProvider : BaseScaleProvider, IDiscreteScale {
		int minorTickCountCore;
		MinorTickmarkProvider minorTickmarkCore;
		MinorTickmarkCollection minorTicksCore;
		int majorTickCountCore;
		MajorTickmarkProvider majorTickmarkCore;
		MajorTickmarkCollection majorTicksCore;
		ScaleLabelCollection labelsCore;
		ScaleRangeCollection rangesCore;
		BaseScaleAppearance appearanceCore;
		event EventHandler GeometryChangedCore;
		event CustomTickmarkTextEventHandler CustomTickmarkTextCore;
		public BaseDiscreteScaleProvider(OwnerChangedAction scaleChanged)
			: base(scaleChanged) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.appearanceCore = new BaseScaleAppearance();
			this.Appearance.Changed += OnAppearanceChanged;
			this.minorTickmarkCore = CreateMinorTickmarkCore();
			this.majorTickmarkCore = CreateMajorTickmarkCore();
			this.minorTicksCore = new MinorTickmarkCollection();
			this.majorTicksCore = new MajorTickmarkCollection();
			this.minorTickCountCore = 1;
			this.majorTickCountCore = 11;
			this.minorTickmarkCore.Changed += OnMinorTickmarkChanged;
			this.majorTickmarkCore.Changed += OnMajorTickmarkChanged;
		}
		protected override void OnDispose() {
			Ref.Dispose(ref minorTickmarkCore);
			Ref.Dispose(ref majorTickmarkCore);
			if(MinorTickmarks != null) {
				MinorTickmarks.DisposeItems();
				MinorTickmarks.Dispose();
				minorTicksCore = null;
			}
			if(MajorTickmarks != null) {
				MajorTickmarks.DisposeItems();
				MajorTickmarks.Dispose();
				majorTicksCore = null;
			}
			Ref.Dispose(ref appearanceCore);
			if(Labels != null) {
				Labels.CollectionChanged -= OnLabelsCollectionChanged;
				Labels.Dispose();
				labelsCore = null;
			}
			if(Ranges != null) {
				Ranges.CollectionChanged -= OnRangesCollectionChanged;
				Ranges.Dispose();
				rangesCore = null;
			}
			GeometryChangedCore = null;
			CustomTickmarkTextCore = null;
			base.OnDispose();
		}
		protected internal void InitLabelsInternal(ScaleLabelCollection ownerLabels) {
			this.labelsCore = (ownerLabels != null) ? ownerLabels : CreateDefaultLabels();
			Labels.CollectionChanged += OnLabelsCollectionChanged;
		}
		protected internal void InitRangesInternal(ScaleRangeCollection ownerRanges) {
			this.rangesCore = (ownerRanges != null) ? ownerRanges : CreateDefaultRanges();
			Ranges.CollectionChanged += OnRangesCollectionChanged;
		}
		protected abstract ScaleRangeCollection CreateDefaultRanges();
		protected virtual ScaleLabelCollection CreateDefaultLabels() {
			return new ScaleLabelCollection(this);
		}
		public IScaleRange CreateRange() {
			return Ranges.CreateRange() as IScaleRange;
		}
		public IScaleLabel CreateLabel() {
			return Labels.CreateLabel() as IScaleLabel;
		}
		public event EventHandler GeometryChanged {
			add {
				if(IsEmpty) return;
				GeometryChangedCore += value;
			}
			remove {
				if(IsEmpty) return;
				GeometryChangedCore -= value;
			}
		}
		public event CustomTickmarkTextEventHandler CustomTickmarkText {
			add {
				if(IsEmpty) return;
				CustomTickmarkTextCore += value;
			}
			remove {
				if(IsEmpty) return;
				CustomTickmarkTextCore -= value;
			}
		}
		void OnLabelsCollectionChanged(CollectionChangedEventArgs<ILabel> ea) {
			switch(ea.ChangedType) {
				case ElementChangedType.ElementAdded:
				case ElementChangedType.ElementRemoved:
				case ElementChangedType.ElementUpdated:
					OnObjectChanged("Labels");
					break;
			}
		}
		void OnRangesCollectionChanged(CollectionChangedEventArgs<IRange> ea) {
			switch(ea.ChangedType) {
				case ElementChangedType.ElementAdded:
				case ElementChangedType.ElementRemoved:
				case ElementChangedType.ElementUpdated:
					OnObjectChanged("Ranges");
					break;
			}
		}
		void OnMinorTickmarkChanged(object sender, EventArgs e) {
			ResetMinorTickmarks();
			OnObjectChanged("MinorTickmarks");
		}
		void OnMajorTickmarkChanged(object sender, EventArgs e) {
			ResetMajorTickmarks();
			OnObjectChanged("MajorTickmarks");
		}
		void OnAppearanceChanged(object sender, EventArgs e) {
			OnObjectChanged("Appearance");
		}
		protected override void OnChangeLogarithmicProperty() {
			base.OnChangeLogarithmicProperty();
			ResetMinorTickmarks();
			ResetMajorTickmarks();
			OnObjectChanged("MinorTickmarks");			
			OnObjectChanged("MajorTickmarks");
		}
		protected virtual MinorTickmarkProvider CreateMinorTickmarkCore() {
			return new MinorTickmarkProvider();
		}
		protected virtual MajorTickmarkProvider CreateMajorTickmarkCore() {
			return new MajorTickmarkProvider();
		}
		public void UpdateTickmarks() {
			ResetMinorTickmarks();
			ResetMajorTickmarks();
			CalculateTickmarks();
		}
		public sealed override bool IsDiscrete {
			get { return true; }
		}
		public IMinorTickmark MinorTickmark {
			get { return minorTickmarkCore; }
		}
		public MinorTickmarkCollection MinorTickmarks {
			get { return minorTicksCore; }
		}
		public IMajorTickmark MajorTickmark {
			get { return majorTickmarkCore; }
		}
		public MajorTickmarkCollection MajorTickmarks {
			get { return majorTicksCore; }
		}
		public int TickCount {
			get {
				if(IsLogarithmic)
					return LogarithmicTickCount;				
				return 1 + Math.Max(1, MajorTickCount - 1) * (MinorTickCount + 1);
			}
		}
		int LogarithmicTickCount {
			get {
				return logarithmicMinorTicksCore.Count; 
			}
		}
		List<float> logarithmicMinorTicksCore = new List<float>();
		internal List<float> LogarithmicMinorTicks { get { return logarithmicMinorTicksCore; } }
		List<float> logarithmicMajorValuesCore = new List<float>();
		internal List<float> LogarithmicMajorValues { get { return logarithmicMajorValuesCore; } }
		internal int Course {
			get { return Math.Sign(MaxValueCore - MinValueCore); }
		}
		void AddMajorInMinorTicks() {
			if(MajorTickmark.AllowTickOverlap) {
				for(int i = 0; i < logarithmicMajorValuesCore.Count; i++) {
					if(logarithmicMajorValuesCore[i] * Course >= MinValueCore * Course && Course * logarithmicMajorValuesCore[i] <= Course * MaxValueCore)
						logarithmicMinorTicksCore.Add(i);
				}
			}
		}
		void CalculateLogarithmicTickMarks() {
			if(LogarithmicBase != LogarithmicBase.Custom)
				LogarithmicBaseToFloat();
			CalculateLogarithmicMajorValues();
			CalculateLogarithmicMinorTicks();
		}
		void CalculateLogarithmicMinorTicks() {
			logarithmicMinorTicksCore.Clear();			
			AddMajorInMinorTicks();
			for(int i = 1; i < logarithmicMajorValuesCore.Count; i++) {				
				if(logarithmicMajorValuesCore[i] != 0 && logarithmicMajorValuesCore[i - 1] != 0) {
					CalculateLogarithmicThisIntervalMinorTicks(i);
				}
			}
			logarithmicMinorTicksCore.Sort();
		}
		void CalculateLogarithmicThisIntervalMinorTicks(int index) {
			int sign = Math.Sign(logarithmicMajorValuesCore[index]);
			int i = sign == -1 ? index : index - 1;
			float delta = (logarithmicMajorValuesCore[index] - logarithmicMajorValuesCore[index - 1]) / (MinorTickCount + 1);
			for(int j = 1; j <= MinorTickCount; j++) {
				float thisValue = logarithmicMajorValuesCore[i] + sign * delta * j;
				if(Course * thisValue > Course * MinValueCore && Course * MaxValueCore > Course * thisValue)					
					logarithmicMinorTicksCore.Add(CalculateLogarithmicMinorTick(thisValue, logarithmicMajorValuesCore[i], sign, index));
			}
		}
		internal float CalculateLogarithmicMinorTick(float thisValue, float prevValue, int sign, int index) {
			if(thisValue == 0 || prevValue == 0) return LogarithmicMajorValues.IndexOf(0);
			float minor = (CalcLogarithmic(thisValue) - CalcLogarithmic(prevValue)) * Course;
			if(sign == -1) minor = 1 - Math.Abs(minor);
			minor = Math.Abs(minor) + index - 1;
			return minor;
		}
		internal float CalculateLevelValue(float thisIndex, int sign) {
			int index = (int)Math.Ceiling(thisIndex);
			thisIndex = thisIndex - index + 1;
			if(sign == -1) thisIndex = 1 - Math.Abs(thisIndex);
			thisIndex = (float)(thisIndex - 1 + Math.Log(LogarithmicMajorValues[index], AbsoluteLogarithmicBase) * Course) / Course;
			return thisIndex;
		}
		protected float CalcLogarithmic(double value) {
			return ((float)Math.Log(Math.Abs(value), AbsoluteLogarithmicBase));
		}
		void CalculateLogarithmicMajorValuesDifferentSign(int sign, int minLevel, int maxLevel) {		   
			int start = 0, end = minLevel;
			if(maxLevel < 0 && minLevel > maxLevel) start = maxLevel;
			if(start > end && end < 0) logarithmicMajorValuesCore.Add((float)Math.Pow(AbsoluteLogarithmicBase, end) * sign);
			else CalculateLogarithmicMajorValues(sign, start, end);
		}
		void CalculateLogarithmicMajorValues(int sign, int start, int end) {
			while(start <= end) {
				logarithmicMajorValuesCore.Add((float)Math.Pow(AbsoluteLogarithmicBase, start) * sign);
				start += 1;
			}
		}
		void CalculateLogarithmicMajorValuesOneSign(int minLevel, int maxLevel) {
			int start = minLevel, end = maxLevel;
			if(start > end) {
				end = minLevel;
				start = maxLevel;
			}
			if(start < 0 && (MaxValue == 0 || MinValue == 0)) logarithmicMajorValuesCore.Add((float)Math.Pow(AbsoluteLogarithmicBase, start) * SignMax);
			else CalculateLogarithmicMajorValues(SignMin, start, end);
		}
		int SignMax { get { return Math.Sign(MaxValueCore != 0 ? MaxValueCore : 1 * Math.Sign(MinValueCore)); } }
		int SignMin { get { return Math.Sign(MinValueCore != 0 ? MinValueCore : 1 * Math.Sign(MaxValueCore)); } }		  
		void CalculateLogarithmicMajorValues() {
			logarithmicMajorValuesCore.Clear();
			int maxLevel = (Course * MaxValueCore >= 0 && Course * MaxValueCore > Course * MinValueCore) ? (int)Math.Ceiling(CalcLogarithmic(MaxValueCore != 0 ? MaxValueCore : 1)) : (int)Math.Floor(CalcLogarithmic(MaxValueCore));
			int minLevel = (Course * MinValueCore >= 0 && Course * MaxValueCore > Course * MinValueCore) ? (int)Math.Floor(CalcLogarithmic(MinValueCore != 0 ? MinValueCore : 1)) : (int)Math.Ceiling(CalcLogarithmic(MinValueCore));
			if(SignMax + SignMin == 0) {
				CalculateLogarithmicMajorValuesDifferentSign(SignMin, minLevel, maxLevel);
				logarithmicMajorValuesCore.Add(0);
				CalculateLogarithmicMajorValuesDifferentSign(SignMax, maxLevel, minLevel);
			}
			else {
				if(MinValue == MaxValue) {
					logarithmicMajorValuesCore.Add(MinValueCore);
					logarithmicMajorValuesCore.Add(MaxValueCore);
				}
				else {
					CalculateLogarithmicMajorValuesOneSign(minLevel, maxLevel);
					if(MaxValueCore == 0 || MinValueCore == 0)
						logarithmicMajorValuesCore.Add(0);
				}
			}
			logarithmicMajorValuesCore.Sort();
			if(MinValueCore > MaxValueCore) logarithmicMajorValuesCore.Reverse();
		}
		public int MinorTickCount {
			get { return minorTickCountCore; }
			set {
				if(MinorTickCount == value || value < 0) return;
				minorTickCountCore = value;
				ResetMinorTickmarks();
				OnObjectChanged("MinorTickCount");
			}
		}
		public RangeCollection Ranges {
			get { return rangesCore; }
		}
		public LabelCollection Labels {
			get { return labelsCore; }
		}
		public BaseScaleAppearance Appearance {
			get { return appearanceCore; }
		}	  
		internal float GetPercentOffsetStart {
			get {
				int startIndex = GetStartIndex;
				return startIndex == 0 ? 0 : CalculateLogarithmicMinorTick(MinValueCore, logarithmicMajorValuesCore[startIndex - 1], 1, 1);
			}
		}
		internal float GetPercentOffsetEnd {
			get {
				int endIndex = GetEndIndex;
				return endIndex == logarithmicMajorValuesCore.Count ? 0 : CalculateLogarithmicMinorTick(MaxValueCore, logarithmicMajorValuesCore[endIndex], 1, 1);
			}
		}
		internal int GetStartIndex {
			get {
				return Course * MinValueCore > Course * logarithmicMajorValuesCore[0] ? 1 : 0;
			}
		}
		internal int GetEndIndex {
			get {
				return Course * MaxValueCore < Course * logarithmicMajorValuesCore[logarithmicMajorValuesCore.Count - 1] ? logarithmicMajorValuesCore.Count - 1 : logarithmicMajorValuesCore.Count;
			}
		}
		internal float GetLengthScale {
			get { return (MajorTickCount + (GetPercentOffsetEnd == 0 ? 0 : (1 - GetPercentOffsetEnd)) + (GetPercentOffsetStart == 0 ? 0 : (1 - GetPercentOffsetStart))); }
		}
		public int MajorTickCount {
			get {
				if(IsLogarithmic) {
					int count = 0;
					if(logarithmicMajorValuesCore.Count != 0) {
						if(Course * MaxValueCore < Course * logarithmicMajorValuesCore[logarithmicMajorValuesCore.Count - 1]) count++;
						if(Course * MinValueCore > Course * logarithmicMajorValuesCore[0]) count++;
					}
					majorTickCountCore = logarithmicMajorValuesCore.Count - count;
				}
				return majorTickCountCore; }
			set {
				if(MajorTickCount == value || value < 0) return;
				majorTickCountCore = value;
				ResetMinorTickmarks();
				ResetMajorTickmarks();
				OnObjectChanged("MajorTickCount");
			}
		}
		protected override void OnUpdateObjectCore() {
			Labels.UpdateLabels();
			Ranges.UpdateRanges();
			CalculateTickmarks();
			base.OnUpdateObjectCore();
		}
		protected void RaiseGeometryChanged(string[] geometryProperties, PropertyChangedEventArgs e) {
			if(!CanRaiseGeometryChanged(geometryProperties, e.PropertyName)) return;
			if(GeometryChangedCore != null)
				GeometryChangedCore(this, e);
		}
		bool CanRaiseGeometryChanged(string[] geometryProperties, string propName) {
			if(propName == "EndUpdate") return true;
			for(int i = 0; i < geometryProperties.Length; i++) {
				if(propName == geometryProperties[i]) return true;
			}
			return false;
		}
		protected void ResetMinorTickmarks() {
			MinorTickmarks.DisposeItems();
			MinorTickmarks.Clear();
		}
		protected void ResetMajorTickmarks() {
			MajorTickmarks.DisposeItems();
			MajorTickmarks.Clear();
		}
		protected void CalculateTickmarks() {
			if(IsLogarithmic) {
				ResetMajorTickmarks();
				ResetMinorTickmarks();
				CalculateLogarithmicTickMarks();
			}
			CalculateMinorTickmarksCore();
			CalculateMajorTickmarksCore();
		}
		protected void CalculateMinorTickmarksCore() {			
			if(MinorTickmarks.Count == 0) InitializeMinorTickmarks();
			PointF2D[] origins = new PointF2D[TickCount];
			PointF2D[] orientationPoints = new PointF2D[TickCount];
			if(IsLogarithmic) OnCalculateTickmarksParamsLogarithmic(origins, orientationPoints, false);
			else OnCalculateTickmarksParams(origins, orientationPoints);
			int count = 0;
			foreach(MinorTickmarkProvider tick in MinorTickmarks) {
				tick.BeginUpdate();
				tick.Origin = origins[count];
				tick.Orientation = orientationPoints[count];
				tick.EndUpdate();
				count++;
			}
		}
		protected void InitializeMinorTickmarks() {
			for(int i = 0; i < TickCount; i++) {
				MinorTickmarks.Add(MinorTickmark.Clone() as IMinorTickmark);
			}
		}
		BrushObject actualTickmarckAppearanceBrushCore;
		protected internal BrushObject ActualTickmarkAppearanceBrush {
			get { return actualTickmarckAppearanceBrushCore;}
			set { actualTickmarckAppearanceBrushCore = value;}
		}
		protected internal void UpdateTickmarkAppearance() {
			CalculateMajorTickmarksCore();
			ActualTickmarkAppearanceBrush = null;
		}
		protected void CalculateMajorTickmarksCore() {			
			if(MajorTickmarks.Count == 0) InitializeMajorTickmarks();			
			PointF2D[] origins = new PointF2D[MajorTickCount];
			PointF2D[] orientationPoints = new PointF2D[MajorTickCount];
			if(IsLogarithmic)
				OnCalculateTickmarksParamsLogarithmic(origins, orientationPoints, true);
			else OnCalculateTickmarksParams(origins, orientationPoints);
			int count = 0;
			foreach(MajorTickmarkProvider tick in MajorTickmarks) {
				tick.BeginUpdate();
				tick.Origin = origins[count];
				tick.Orientation = orientationPoints[count];
				tick.Text = OnCalculateTickmarkText((IsLogarithmic && Course * MinValueCore > Course * logarithmicMajorValuesCore[0]) ? count + 1 : count, MajorTickmark.Multiplier, MajorTickmark.Addend);
				if(ActualTickmarkAppearanceBrush != null)
					tick.TextShape.AppearanceText.TextBrush = ActualTickmarkAppearanceBrush;
				tick.EndUpdate();
				count++;
			}
		}
		protected void InitializeMajorTickmarks() {
			for(int i = 0; i < MajorTickCount; i++) {
				MajorTickmarks.Add(MajorTickmark.Clone() as IMajorTickmark);
			}
		}
		protected abstract void OnCalculateTickmarksParams(PointF2D[] starts, PointF2D[] ends);
		protected abstract void OnCalculateTickmarksParamsLogarithmic(PointF2D[] starts, PointF2D[] ends, bool Major);
		protected string OnCalculateTickmarkText(int labelNum, float multiplier, float addend) {
			if(MajorTickCount < 1) return String.Empty;
			float value = MinValueCore;
			if(IsLogarithmic)
				value = logarithmicMajorValuesCore[labelNum];
			else {
				if(MajorTickCount > 1)
					value = MinValueCore + (ScaleLength * (float)labelNum) / (float)(MajorTickCount - 1);
			}
			float percent = ValueToPercent(value);
			string text = String.Format(MajorTickmark.FormatString, value * multiplier + addend, percent);
			CustomTickmarkTextEventArgs ea = new CustomTickmarkTextEventArgs(text, value);
			RaiseCustomTickmarkText(ea);
			return ea.Result;
		}
		protected void RaiseCustomTickmarkText(CustomTickmarkTextEventArgs e) {
			if(CustomTickmarkTextCore != null) CustomTickmarkTextCore(e);
		}
		internal object XtraCreateLabelsItem(XtraItemEventArgs e) {
			XtraPropertyInfo propInfo = e.Item;
			ScaleLabelCollection collection = e.Collection as ScaleLabelCollection;
			if(propInfo == null || collection == null) return null;
			BeginUpdate();
			string name = (string)propInfo.ChildProperties["Name"].Value;
			ILabel label = collection[name] ?? collection.CreateLabel();
			label.Name = name;
			if(!collection.Contains(label))
				collection.Add(label);
			CancelUpdate();
			return label;
		}
		internal object XtraCreateRangesItem(XtraItemEventArgs e) {
			XtraPropertyInfo propInfo = e.Item;
			ScaleRangeCollection collection = e.Collection as ScaleRangeCollection;
			if(propInfo == null || collection == null) return null;
			BeginUpdate();
			string name = (string)propInfo.ChildProperties["Name"].Value;
			IRange range = collection[name] ?? collection.CreateRange();
			range.Name = name;
			if(!collection.Contains(range))
				collection.Add(range);
			CancelUpdate();
			return range;
		}
	}
	public class LinearScaleProvider : BaseDiscreteScaleProvider, ILinearScale {
		PointF2D startPointCore;
		PointF2D endPointCore;
		static string[] geometryProperties = new string[] { "StartPoint", "EndPoint" };
		public LinearScaleProvider(OwnerChangedAction scaleChanged)
			: base(scaleChanged) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.startPointCore = PointF2D.Empty;
			this.endPointCore = new PointF2D(100f, 0f);
		}
		protected override ScaleRangeCollection CreateDefaultRanges() {
			return new LinearScaleRangeCollection(this);
		}
		public PointF2D StartPoint {
			get { return startPointCore; }
			set {
				if(StartPoint == value) return;
				startPointCore = value;
				OnObjectChanged("StartPoint");
			}
		}
		public PointF2D EndPoint {
			get { return endPointCore; }
			set {
				if(EndPoint == value) return;
				endPointCore = value;
				OnObjectChanged("EndPoint");
			}
		}
		protected override float CheckAnimatedValue(float animatedValue) {
			return CoerceValueCore(animatedValue);
		}
		protected override void RaiseChanged(EventArgs e) {
			base.RaiseChanged(e);
			PropertyChangedEventArgs pe = e as PropertyChangedEventArgs;
			if(pe != null)
				RaiseGeometryChanged(geometryProperties, pe);
		}
		public PointF PercentToPoint(float percent) {
			float pointX = StartPoint.X + percent * (EndPoint.X - StartPoint.X);
			float pointY = StartPoint.Y + percent * (EndPoint.Y - StartPoint.Y);
			return new PointF(pointX, pointY);
		}
		public float PointToPercent(PointF point) {
			return PointToPercent(point, PointF.Empty, PointF.Empty);
		}
		protected float PointToPercent(PointF point, PointF increStartScale, PointF increEndScale) {
			PointF startPoint = new PointF(StartPoint.X + increStartScale.X, StartPoint.Y + increStartScale.Y);
			PointF endPoint = new PointF(EndPoint.X - increEndScale.X, EndPoint.Y - increEndScale.Y);
			bool horz = Math.Abs(endPoint.X - startPoint.X) > Math.Abs(endPoint.Y - startPoint.Y);
			float diff = horz ? (point.X - startPoint.X) : (point.Y - startPoint.Y);
			float range = horz ? (endPoint.X - startPoint.X) : (endPoint.Y - startPoint.Y);
			return (range != 0f) ? diff / range : (diff > 0 ? float.PositiveInfinity : float.NegativeInfinity);
		}
		protected override void OnCalculateTickmarksParams(PointF2D[] origins, PointF2D[] orientations) {
			int count = origins.Length;
			if(count < 1) return;
			PointF scaleVector, tickmarkVector;
			float scaleLength;
			CalculateScaleParametrs(out scaleVector, out scaleLength, out tickmarkVector, PointF.Empty, PointF.Empty);
			if(count == 1) {
				CalculateTickmarkCore(origins, orientations, scaleVector, tickmarkVector, 0, 0);
				return;
			}
			for(int i = 0; i < count; i++) {
				float percent = (float)i / ((float)count - 1f);
				CalculateTickmarkCore(origins, orientations, scaleVector, tickmarkVector, i, percent);
			}
		}   
		PointF IncreScale(float percentOffset) {
			float MajorTickX = CalculatePartOfTheInterval((StartPoint.X - EndPoint.X) / (GetLengthScale - 1), percentOffset);
			float MajorTickY = CalculatePartOfTheInterval((StartPoint.Y - EndPoint.Y) / (GetLengthScale - 1), percentOffset);
			return new PointF(MajorTickX, MajorTickY);
		}
		float CalculatePartOfTheInterval(float lengthMajorTick, float percentOffset) {
			return percentOffset != 0 ? percentOffset * lengthMajorTick : 0;
		}
		void CalculateScaleParametrs(out PointF scaleVector, out float scaleLength, out PointF tickmarkVector, PointF increStartScale, PointF increEndScale) {
			scaleVector = new PointF(EndPoint.X - StartPoint.X - (increEndScale.X + increStartScale.X), EndPoint.Y - StartPoint.Y - (increEndScale.Y + increStartScale.Y));
			scaleLength = (float)Math.Sqrt(Math.Pow(scaleVector.X, 2) + Math.Pow(scaleVector.Y, 2));
			tickmarkVector = new PointF(scaleVector.X / scaleLength, scaleVector.Y / scaleLength);
		}
		PointF IncreStartScale { get { return IncreScale(GetPercentOffsetStart); } }
		PointF IncreEndScale { get { return IncreScale(GetPercentOffsetEnd); } }
		protected override void OnCalculateTickmarksParamsLogarithmic(PointF2D[] origins, PointF2D[] orientations, bool Major) {
			int count = origins.Length;
			if(count < 1) return;
			PointF scaleVector, tickmarkVector;
			float scaleLength;
			CalculateScaleParametrs(out scaleVector, out scaleLength, out tickmarkVector, IncreStartScale, IncreEndScale);			
			if(Major) {
				for(int i = GetStartIndex; i < GetEndIndex; i++) 
					CalculateTickmark(origins, orientations, scaleVector, tickmarkVector, GetStartIndex != 0 ? i - 1 : i, i);		
			}
			else {
				for(int i = 0; i < LogarithmicMinorTicks.Count; i++) 
					CalculateTickmark(origins, orientations, scaleVector, tickmarkVector, i, LogarithmicMinorTicks[i]);				
			}
		}
		protected void CalculateTickmark(PointF2D[] origins, PointF2D[] orientations, PointF scaleVector, PointF tickmarkVector, int index, float value) {
			float percent = value / ((float)LogarithmicMajorValues.Count - 1f);
			CalculateTickmarkCore(origins, orientations, scaleVector, tickmarkVector, index, percent);
		}
		protected override float PercentToValueLogarithmicScale(float percent) {			
			PointF point = PercentToPoint(percent);
			if(LogarithmicMajorValues.Count != 0) {
				float value = PointToPercent(point, IncreStartScale, IncreEndScale);
				value *= ((float)LogarithmicMajorValues.Count - 1f);
				value = CalculateLevelValue(value, 1);
				return (float)Math.Pow(AbsoluteLogarithmicBase, value);
			}
			return float.NaN;
		}
		protected override float ValueToPercentLogarithmicScale(float value) {
			float realPercent = 0;
			PointF scaleVector, tickmarkVector;
			float scaleLength;
			if(LogarithmicMajorValues.Count != 0) {
				CalculateScaleParametrs(out scaleVector, out scaleLength, out tickmarkVector, IncreStartScale, IncreEndScale);
				PointF2D[] origins = new PointF2D[1];
				PointF2D[] orientations = new PointF2D[1];
				for(int i = 0; i < LogarithmicMajorValues.Count - 1; i++) {
					if(value * Course > LogarithmicMajorValues[i] * Course && value * Course < LogarithmicMajorValues[i + 1] * Course) {
						if(LogarithmicMajorValues[i] == 0 || LogarithmicMajorValues[i + 1] == 0) {
							float thisValue = LogarithmicMajorValues.IndexOf(0);
							if(CalcLogarithmic(value) > -1) thisValue = CalculateLogarithmicMinorTick(value, 1, -Math.Sign(value), i + 1);
							CalculateTickmark(origins, orientations, scaleVector, tickmarkVector, 0, thisValue);
						}
						else
							CalculateTickmark(origins, orientations, scaleVector, tickmarkVector, 0, CalculateLogarithmicMinorTick(value, LogarithmicMajorValues[i], 1, i + 1));
						realPercent = PointToPercent(origins[0]);
					}
					if(value == LogarithmicMajorValues[i]) {
						CalculateTickmark(origins, orientations, scaleVector, tickmarkVector, 0, i);
						realPercent = PointToPercent(origins[0]);
					}
				}
			}
			return realPercent;
		}
		void CalculateTickmarkCore(PointF2D[] origins, PointF2D[] orientations, PointF scaleVector, PointF tickmarkVector, int i, float percent) {
			PointF offsetStart = IsLogarithmic ? IncreScale(GetPercentOffsetStart) : PointF.Empty;
			float pointX = StartPoint.X + offsetStart.X + percent * scaleVector.X;
			float pointY = StartPoint.Y + offsetStart.Y + percent * scaleVector.Y;
			origins[i] = new PointF2D(pointX, pointY);
			orientations[i] = new PointF2D(pointX - tickmarkVector.Y, pointY + tickmarkVector.X);
		}
		public sealed override bool IsEmpty {
			get { return this == ScaleFactory.EmptyLinearScale; }
		}
	}
	public class ArcScaleProvider : BaseDiscreteScaleProvider, IArcScale {
		float radiusXCore;
		float radiusYCore;
		PointF2D centerPointCore;
		float startAngleCore;
		float endAngleCore;
		static string[] geometryProperties = new string[] { "Center", "RadiusX", "RadiusY", "StartAngle", "EndAngle" };		
		public ArcScaleProvider(OwnerChangedAction scaleChanged)
			: base(scaleChanged) {
		}
		protected override ScaleRangeCollection CreateDefaultRanges() {
			return new ArcScaleRangeCollection(this);
		}
		protected override void OnCreate() {
			base.OnCreate();
			this.endAngleCore = 360;
			this.radiusXCore = 100;
			this.radiusYCore = 100;
		}
		public PointF2D Center {
			get { return centerPointCore; }
			set {
				if(Center == value) return;
				centerPointCore = value;
				OnObjectChanged("Center");
			}
		}
		public float RadiusX {
			get { return radiusXCore; }
			set {
				if(RadiusX == value || value < 0) return;
				radiusXCore = value;
				OnObjectChanged("RadiusX");
			}
		}
		public float RadiusY {
			get { return radiusYCore; }
			set {
				if(RadiusY == value || value < 0) return;
				radiusYCore = value;
				OnObjectChanged("RadiusY");
			}
		}
		public float StartAngle {
			get { return startAngleCore; }
			set {
				if(StartAngle == value) return;
				startAngleCore = value;
				OnObjectChanged("StartAngle");
			}
		}
		public float EndAngle {
			get { return endAngleCore; }
			set {
				if(EndAngle == value) return;
				endAngleCore = value;
				OnObjectChanged("EndAngle");
			}
		}
		protected override void RaiseChanged(EventArgs e) {
			base.RaiseChanged(e);
			PropertyChangedEventArgs pe = e as PropertyChangedEventArgs;
			if(pe != null)
				RaiseGeometryChanged(geometryProperties, pe);
		}
		public PointF NeedleVector {
			get {
				float range = EndAngle - StartAngle;
				float alpha = StartAngle + ValueToPercent(Value) * range;
				return MathHelper.GetRadiusVector(RadiusX, RadiusY, alpha);
			}
		}
		public PointF PercentToPoint(float percent) {
			float range = EndAngle - StartAngle;
			float alpha = StartAngle + percent * range;
			PointF radius = MathHelper.GetRadiusVector(RadiusX, RadiusY, alpha);
			return new PointF(Center.X + radius.X, Center.Y + radius.Y);
		}
		public float PointToPercent(PointF point) {
			double range = EndAngle - StartAngle;
			return PointToPercent(point, range);
		}
		protected float PointToPercent(PointF point, double range) {			
			double alpha = MathHelper.CalcEllipticalRadiusVectorAngle(Center, point, RadiusX, RadiusY) / Math.PI * 180d;
			double diff = (alpha - StartAngle) % 360d;
			if(diff > Math.Abs(range)) return Math.Abs((360 - (alpha - StartAngle)) % 360d) > Math.Abs((alpha - EndAngle) % 360d) ? 1 : 0;
			return (range != 0f) ? (float)(diff / range) : (diff > 0 ? float.PositiveInfinity : float.NegativeInfinity);
		}
		protected override void OnCalculateTickmarksParams(PointF2D[] origins, PointF2D[] orientations) {
			int count = origins.Length;
			if(count < 1) return;
			if(count == 1) {
				CalculateTickmarcsCore(origins, orientations, 0, StartAngle);
				return;
			}
			float range = EndAngle - StartAngle;
			for(int i = 0; i < count; i++) {
				float alpha = StartAngle + ((float)i / (float)(count - 1)) * range;
				CalculateTickmarcsCore(origins, orientations, i, alpha);
			}
		}
		float IncreScale(float percentOffset) {
			float MajorTick = ReducesMajorTick(Math.Abs(EndAngle - StartAngle) / (GetLengthScale - 1), percentOffset);
			return MajorTick;
		}
		float ReducesMajorTick(float lengthMajorTick, float percentOffset) {
			return percentOffset != 0 ? percentOffset * lengthMajorTick : 0;
		}
		float OffsetStart { get { return IncreScale(GetPercentOffsetStart); } }
		float OffsetEnd { get { return IncreScale(GetPercentOffsetEnd); } }
		protected override void OnCalculateTickmarksParamsLogarithmic(PointF2D[] origins, PointF2D[] orientations, bool Major) {
			int count = origins.Length;
			if(count < 1) return;
			float offsetStart = OffsetStart;
			float range = CalcRangeScale();
			if(Major) {
				for(int i = GetStartIndex; i < GetEndIndex; i++) 
					CalculateTickmark(origins, orientations, GetStartIndex != 0 ? i - 1 : i, i, range, offsetStart);
			}
			else {
				for(int i = 0; i < LogarithmicMinorTicks.Count; i++)
					CalculateTickmark(origins, orientations, i, LogarithmicMinorTicks[i], range, offsetStart);	  
			}
		}
		protected void CalculateTickmark(PointF2D[] origins, PointF2D[] orientations, int index, float value, float range, float offsetStart) {
			float alpha = StartAngle - ((EndAngle > StartAngle ? 1 : -1) * offsetStart) + (value / ((float)LogarithmicMajorValues.Count - 1f)) * range;
			CalculateTickmarcsCore(origins, orientations, index, alpha);
		}
		float CalcRangeScale() {
			int turn = EndAngle > StartAngle ? 1 : -1;
			return (EndAngle + turn * OffsetEnd) - (StartAngle - turn * OffsetStart);
		}
		protected override float PercentToValueLogarithmicScale(float percent) {
			PointF point = PercentToPoint(percent);
			if(LogarithmicMajorValues.Count != 0) {
				float range = CalcRangeScale();
				float value1 = PointToPercent(point, range);
				float alpha = StartAngle + value1 * range;
				float value = ((alpha - StartAngle + OffsetStart)/ range)*((float)LogarithmicMajorValues.Count - 1f);
				if(value > LogarithmicMajorValues.Count - 1f) value = LogarithmicMajorValues.Count - 1f;
				return (float)Math.Pow(AbsoluteLogarithmicBase, CalculateLevelValue(value, 1));
			}
			return 0;
		}
		protected override float ValueToPercentLogarithmicScale(float value) {
			float realPercent = 0;
			PointF2D[] origins = new PointF2D[1];
			PointF2D[] orientations = new PointF2D[1];
			if(LogarithmicMajorValues.Count != 0) {
				float offsetStart = OffsetStart;
				float range = CalcRangeScale();
				for(int i = 0; i < LogarithmicMajorValues.Count - 1; i++) {
					if(value * Course > LogarithmicMajorValues[i] * Course && value * Course < LogarithmicMajorValues[i + 1] * Course) {
						if(LogarithmicMajorValues[i] == 0 || LogarithmicMajorValues[i + 1] == 0){
							float thisValue = LogarithmicMajorValues.IndexOf(0);
							if(CalcLogarithmic(value) > -1) thisValue = CalculateLogarithmicMinorTick(value, 1, -Math.Sign(value), i + 1);
							CalculateTickmark(origins, orientations, 0, thisValue, range, offsetStart);
						}
						else
							CalculateTickmark(origins, orientations, 0, CalculateLogarithmicMinorTick(value, LogarithmicMajorValues[i], 1, i + 1), range, offsetStart);
						realPercent = PointToPercent(origins[0]);
					}
					if(value == LogarithmicMajorValues[i]) {
						CalculateTickmark(origins, orientations, 0, i, range, offsetStart);
						realPercent = PointToPercent(origins[0]);
					}
				}
			}
			return realPercent;
		}
		void CalculateTickmarcsCore(PointF2D[] origins, PointF2D[] orientations, int i, float alpha) {
			PointF vector = MathHelper.GetRadiusVector(RadiusX, RadiusY, alpha);
			origins[i] = new PointF2D(vector.X + Center.X, vector.Y + Center.Y);
			orientations[i] = new PointF2D(vector.X + vector.X + Center.X, vector.Y + vector.Y + Center.Y);
		}
		public sealed override bool IsEmpty {
			get { return this == ScaleFactory.EmptyArcScale; }
		}
	}
	[TypeConverter(typeof(LinearLogarithmicScaleObjectTypeConverter))]
	public class LinearScale : BaseScaleIndependentComponent<LinearScaleProvider>,
		ILinearScale, ISupportAssign<LinearScale>, ILabelColorProvider {
		PolylineShape scaleLineShape;
		public LinearScale() : base() { }
		public LinearScale(string name) : base(name) { }
		protected override void OnCreate() {
			base.OnCreate();
			scaleLineShape = new PolylineShape(new PointF[2]);
			scaleLineShape.Name = PredefinedShapeNames.LinearScaleShape;
			Provider.InitLabelsInternal(CreateLabels());
			Provider.InitRangesInternal(CreateRanges());
		}
		protected override void OnDispose() {
			Ref.Dispose(ref scaleLineShape);
			base.OnDispose();
		}
		protected override LinearScaleProvider CreateProvider() {
			return new LinearScaleProvider(OnScaleIndependentComponentChanged);
		}
		protected sealed override void PrepareDelayedCalculation() {
			if(IsUpdateLocked) Provider.UpdateTickmarks();
		}
		protected sealed override void CalculateScaleIndependentComponent() {
			scaleLineShape.BeginUpdate();
			scaleLineShape.Appearance.BorderWidth = Appearance.Width;
			scaleLineShape.Points[0] = StartPoint;
			scaleLineShape.Points[1] = EndPoint;
			scaleLineShape.EndUpdate();
			GetDefaultAppearance();
			if(UseColorScheme && MajorTickmark.TextShape.AppearanceText.TextBrush == BrushObject.Empty) {
				Color actualColor = GetLabelShemeColor();
				SetActualAppearance(actualColor);
			}
			if(!UseColorScheme && MajorTickmark.TextShape.AppearanceText.TextBrush == BrushObject.Empty) {
				SetActualAppearance();
			}
			Provider.UpdateTickmarkAppearance();
		}
		protected virtual BaseTextAppearance GetDefaultAppearance() {
			return MajorTickmark.TextShape.AppearanceText;
		}
		protected void SetActualAppearance() {
			Provider.ActualTickmarkAppearanceBrush = GetDefaultAppearance().TextBrush;
		}
		protected void SetActualAppearance(Color actualColor) {
			Provider.ActualTickmarkAppearanceBrush = new SolidBrushObject(actualColor);
		}
		bool useColorSchemeCore = true;
		[XtraSerializableProperty,  DefaultValue(true)]
		[Category("Appearance")]
		public bool UseColorScheme {
			get { return useColorSchemeCore; }
			set {
				if(useColorSchemeCore == value)
					return;
				useColorSchemeCore = value;
				if(Provider != null) {
					CalculateScaleIndependentComponent();
					UpdateComponent();
				}
			}
		}
		protected virtual void UpdateComponent() { }
		public virtual Color GetLabelShemeColor() {
			return Color.Empty;
		}
		protected virtual ScaleLabelCollection CreateLabels() {
			return new ScaleLabelCollection(this);
		}
		protected virtual ScaleRangeCollection CreateRanges() {
			return new LinearScaleRangeCollection(this);
		}
		public event EventHandler ValueChanged {
			add { Provider.ValueChanged += value; }
			remove { if(!IsDisposing) Provider.ValueChanged -= value; }
		}
		public event EventHandler MinMaxValueChanged {
			add { Provider.MinMaxValueChanged += value; }
			remove { if(!IsDisposing) Provider.MinMaxValueChanged -= value; }
		}
		public event EventHandler GeometryChanged {
			add { Provider.GeometryChanged += value; }
			remove { if(!IsDisposing) Provider.GeometryChanged -= value; }
		}
		public event EventHandler Animating {
			add { Provider.Animating += value; }
			remove { if(!IsDisposing) Provider.Animating -= value; }
		}
		public event EventHandler AnimationCompleted {
			add { Provider.AnimationCompleted += value; }
			remove { if(!IsDisposing) Provider.AnimationCompleted -= value; }
		}
		public event CustomTickmarkTextEventHandler CustomTickmarkText {
			add { Provider.CustomTickmarkText += value; }
			remove { if(!IsDisposing) Provider.CustomTickmarkText -= value; }
		}
		public event CustomRescalingEventHandler CustomRescaling {
			add { Provider.CustomRescaling += value; }
			remove { if(!IsDisposing) Provider.CustomRescaling -= value; }
		}
		protected override void OnShaderChangedCore() {
			Provider.UpdateTickmarks();
			base.OnShaderChangedCore();
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleIsEmpty")]
#endif
		public bool IsEmpty {
			get { return Provider.IsEmpty; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleIsDiscrete")]
#endif
		public bool IsDiscrete {
			get { return Provider.IsDiscrete; }
		}
		public bool IsLogarithmic {
			get { return Provider.IsLogarithmic; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleIsAnimating")]
#endif
		public virtual bool IsAnimating {
			get { return Provider.IsAnimating; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleLogarithmic"),
#endif
 DefaultValue(false),
		RefreshProperties(RefreshProperties.All)]
		[Category("Scale")]		
		[XtraSerializableProperty]
		public bool Logarithmic {
			get { return Provider.Logarithmic; }
			set { Provider.Logarithmic = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleCustomLogarithmicBase"),
#endif
 DefaultValue(2)]
		[Category("Scale")]
		[XtraSerializableProperty]
		public float CustomLogarithmicBase {
			get { return Provider.CustomLogarithmicBase; }
			set { Provider.CustomLogarithmicBase = value; }
		}
		[ DefaultValue(LogarithmicBase.Binary),
		RefreshProperties(RefreshProperties.All)]
		[Category("Scale")]
		[XtraSerializableProperty]
		public LogarithmicBase LogarithmicBase {
			get { return Provider.LogarithmicBase; }
			set { Provider.LogarithmicBase = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleAppearance"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseScaleAppearance Appearance {
			get { return Provider.Appearance; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleStartPoint"),
#endif
XtraSerializableProperty]
		[Category("Geometry")]
		public PointF2D StartPoint {
			get { return Provider.StartPoint; }
			set { Provider.StartPoint = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleEndPoint"),
#endif
XtraSerializableProperty]
		[Category("Geometry")]
		public PointF2D EndPoint {
			get { return Provider.EndPoint; }
			set { Provider.EndPoint = value; }
		}
		public PointF PercentToPoint(float percent) {
			return Provider.PercentToPoint(percent);
		}
		public float PointToPercent(PointF point) {
			return Provider.PointToPercent(point);
		}
		public float GetInternalValue() {
			return Provider.GetInternalValue();
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleAutoRescaling"),
#endif
XtraSerializableProperty, DefaultValue(false)]
		[Category("AutoRescaling")]
		public bool AutoRescaling {
			get { return Provider.AutoRescaling; }
			set { Provider.AutoRescaling = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScalePercent"),
#endif
DefaultValue(0f)]
		public float Percent {
			get { return Provider.Percent; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleValue"),
#endif
Bindable(true)]
		[DefaultValue(0f)]
		[Category("Scale")]
		[XtraSerializableProperty(1)]
		public float Value {
			get { return Provider.Value; }
			set { Provider.Value = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleMinValue"),
#endif
DefaultValue(0f)]
		[Category("Scale")]
		[XtraSerializableProperty(0)]
		public float MinValue {
			get { return Provider.MinValue; }
			set { Provider.MinValue = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleMaxValue"),
#endif
DefaultValue(1f)]
		[Category("Scale")]
		[XtraSerializableProperty(0)]
		public float MaxValue {
			get { return Provider.MaxValue; }
			set { Provider.MaxValue = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleRescalingThresholdMin"),
#endif
XtraSerializableProperty, DefaultValue(0.05f)]
		[Category("AutoRescaling")]
		public float RescalingThresholdMin {
			get { return Provider.RescalingThresholdMin; }
			set { Provider.RescalingThresholdMin = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleRescalingThresholdMax"),
#endif
XtraSerializableProperty, DefaultValue(0.05f)]
		[Category("AutoRescaling")]
		public float RescalingThresholdMax {
			get { return Provider.RescalingThresholdMax; }
			set { Provider.RescalingThresholdMax = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleRescalingBestValues"),
#endif
XtraSerializableProperty, DefaultValue(false)]
		[Category("AutoRescaling")]
		public bool RescalingBestValues {
			get { return Provider.RescalingBestValues; }
			set { Provider.RescalingBestValues = value; }
		}
		[Browsable(false)]
		public float ScaleLength {
			get { return Provider.ScaleLength; }
		}
		public float PercentToValue(float percent) {
			return Provider.PercentToValue(percent);
		}
		public float ValueToPercent(float value) {
			return Provider.ValueToPercent(value);
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleMinorTickmark"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Tickmarks")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IMinorTickmark MinorTickmark {
			get { return Provider.MinorTickmark; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleMinorTickCount"),
#endif
DefaultValue(1)]
		[Category("Tickmarks")]
		[XtraSerializableProperty]
		public int MinorTickCount {
			get { return Provider.MinorTickCount; }
			set { Provider.MinorTickCount = value; }
		}
		[Browsable(false)]
		public int TickCount {
			get { return Provider.TickCount; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleMajorTickmark"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Tickmarks")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IMajorTickmark MajorTickmark {
			get { return Provider.MajorTickmark; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleMajorTickCount"),
#endif
DefaultValue(11)]
		[Category("Tickmarks")]
		[XtraSerializableProperty]
		public int MajorTickCount {
			get { return Provider.MajorTickCount; }
			set { Provider.MajorTickCount = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleLabels"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Scale")]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		public virtual LabelCollection Labels {
			get { return Provider.Labels; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("LinearScaleRanges"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Scale")]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		public virtual RangeCollection Ranges {
			get { return Provider.Ranges; }
		}
		public IScaleRange CreateRange() {
			return Provider.CreateRange();
		}
		public IScaleLabel CreateLabel() {
			return Provider.CreateLabel();
		}
		protected override void OnLoadShapes() {
			base.OnLoadShapes();
			OnLoadScaleLineShape();
			OnLoadRangeShapes();
			OnLoadMinorTickmarkShapes();
			OnLoadMajorTickmarkShapes();
			OnLoadLabelShapes();
		}
		protected void OnLoadLabelShapes() {
			foreach(ILabel label in Provider.Labels) {
				Shapes.Add(label.TextShape.Clone());
			}
		}
		protected void OnLoadRangeShapes() {
			foreach(IRange range in Provider.Ranges) {
				Shapes.Add(range.Shape.Clone());
			}
		}
		protected void OnLoadMajorTickmarkShapes() {
			int counter = -1;
			bool showTick = MajorTickmark.ShowTick;
			bool showText = MajorTickmark.ShowText;
			foreach(IMajorTickmark tick in Provider.MajorTickmarks) {
				counter++;
				if(counter == 0 && !MajorTickmark.ShowFirst) continue;
				if(MajorTickCount > 0 && (counter == MajorTickCount - 1) && !MajorTickmark.ShowLast) continue;
				if(showTick) Shapes.Add(tick.Shape);
				if(showText) Shapes.Add(tick.TextShape);
			}
		}
		protected void OnLoadMinorTickmarkShapes() {
			int counter = -1;
			bool showTick = MinorTickmark.ShowTick;
			foreach(IMinorTickmark tick in Provider.MinorTickmarks) {
				counter++;
				if(counter == 0 && !MinorTickmark.ShowFirst) continue;
				if(TickCount > 0 && (counter == TickCount - 1) && !MinorTickmark.ShowLast) continue;
				if(!IsLogarithmic && (counter % (MinorTickCount + 1) == 0) && !MajorTickmark.AllowTickOverlap) continue;			   
				if(showTick) Shapes.Add(tick.Shape);
			}
		}
		protected void OnLoadScaleLineShape() {
			scaleLineShape.Appearance.BorderBrush = Appearance.Brush.Clone() as BrushObject;
			Shapes.Add(scaleLineShape);
		}
		public void Assign(LinearScale source) {
			BeginUpdate();
			AssignPrimitiveProperties(source);
			if(source != null) {
				this.MinValue = source.MinValue;
				this.MaxValue = source.MaxValue;
				this.Value = source.Value;
				this.AutoRescaling = source.AutoRescaling;
				this.RescalingThresholdMin = source.RescalingThresholdMin;
				this.RescalingThresholdMax = source.RescalingThresholdMax;
				this.RescalingBestValues = source.RescalingBestValues;
				this.Appearance.Assign(source.Appearance);
				this.UseColorScheme = source.UseColorScheme;
				this.MinorTickmark.Assign(source.MinorTickmark);
				this.MajorTickmark.Assign(source.MajorTickmark);
				this.MinorTickCount = source.MinorTickCount;
				this.MajorTickCount = source.MajorTickCount;
				this.Labels.Assign(source.Labels);
				this.Ranges.Assign(source.Ranges);
				this.Logarithmic = source.Logarithmic;
				this.LogarithmicBase = source.LogarithmicBase;
				this.CustomLogarithmicBase = source.CustomLogarithmicBase;
				this.StartPoint = source.StartPoint;
				this.EndPoint = source.EndPoint;
			}
			EndUpdate();
		}
		public bool IsDifferFrom(LinearScale source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
							(this.MinValue != source.MinValue) ||
							(this.MaxValue != source.MaxValue) ||
							(this.Value != source.Value) ||
							(this.AutoRescaling != source.AutoRescaling) ||
							(this.RescalingThresholdMin != source.RescalingThresholdMin) ||
							(this.RescalingThresholdMax != source.RescalingThresholdMax) ||
							(this.RescalingBestValues != source.RescalingBestValues) ||
							this.Appearance.IsDifferFrom(source.Appearance) ||
							(this.UseColorScheme != source.UseColorScheme) ||
							this.MinorTickmark.IsDifferFrom(source.MinorTickmark) ||
							this.MajorTickmark.IsDifferFrom(source.MajorTickmark) ||
							(this.TickCount != source.TickCount) ||
							(this.MajorTickCount != source.MajorTickCount) ||
							(this.Labels.IsDifferFrom(source.Labels)) ||
							(this.Ranges.IsDifferFrom(source.Ranges)) ||
							(this.StartPoint != source.StartPoint) ||
							(this.EndPoint != source.EndPoint) || (this.Logarithmic != source.Logarithmic) || (this.LogarithmicBase != source.LogarithmicBase) || (this.CustomLogarithmicBase != source.CustomLogarithmicBase);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateLabelsItem(XtraItemEventArgs e) {
			return Provider.XtraCreateLabelsItem(e);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateRangesItem(XtraItemEventArgs e) {
			return Provider.XtraCreateRangesItem(e);
		}
	}
	[TypeConverter(typeof(ArcLogarithmicScaleObjectTypeConverter))]
	public class ArcScale : BaseScaleIndependentComponent<ArcScaleProvider>,
		IArcScale, ISupportAssign<ArcScale>, ILabelColorProvider {
		ArcShape scaleArcShape;
		public ArcScale() : base() { }
		public ArcScale(string name)
			: base(name) {
		}
		protected override void OnCreate() {
			base.OnCreate();
			scaleArcShape = new ArcShape();
			scaleArcShape.Name = PredefinedShapeNames.ArcScaleShape;
			Provider.InitLabelsInternal(CreateLabels());
			Provider.InitRangesInternal(CreateRanges());
		}
		protected override void OnDispose() {
			Ref.Dispose(ref scaleArcShape);
			base.OnDispose();
		}
		protected override ArcScaleProvider CreateProvider() {
			return new ArcScaleProvider(OnScaleIndependentComponentChanged);
		}
		protected virtual ScaleRangeCollection CreateRanges() {
			return new ArcScaleRangeCollection(this);
		}
		protected virtual ScaleLabelCollection CreateLabels() {
			return new ScaleLabelCollection(this);
		}
		protected sealed override void PrepareDelayedCalculation() {
			if(IsUpdateLocked) Provider.UpdateTickmarks();
		}
		protected sealed override void CalculateScaleIndependentComponent() {
			scaleArcShape.BeginUpdate();
			scaleArcShape.Appearance.BorderWidth = Appearance.Width;
			scaleArcShape.Box = new RectangleF(Center.X - RadiusX, Center.Y - RadiusY, RadiusX * 2f, RadiusY * 2f);
			scaleArcShape.StartAngle = StartAngle;
			scaleArcShape.EndAngle = EndAngle;
			scaleArcShape.EndUpdate();
			if(MajorTickmark.TextShape.AppearanceText.TextBrush == BrushObject.Empty) {
				if(UseColorScheme) {
					Color actualColor = GetLabelShemeColor();
					SetActualAppearance(actualColor);
				}
				else
					SetActualAppearance();
			}
			Provider.UpdateTickmarkAppearance();
		}
		protected virtual BaseTextAppearance GetDefaultAppearance() {
			return MajorTickmark.TextShape.AppearanceText;
		}
		protected void SetActualAppearance() {
			Provider.ActualTickmarkAppearanceBrush = GetDefaultAppearance().TextBrush;
		}
		protected void SetActualAppearance(Color actualColor) {
			Provider.ActualTickmarkAppearanceBrush = new SolidBrushObject(actualColor);
		}
		bool useColorSchemeCore = true;
		[XtraSerializableProperty,  DefaultValue(true)]
		[Category("Appearance")]
		public bool UseColorScheme {
			get { return useColorSchemeCore;}
			set {
				if(useColorSchemeCore == value)
					return;
				useColorSchemeCore = value;
				if(Provider != null) {
					CalculateScaleIndependentComponent();
					UpdateComponent();
				}
			}
		}
		protected virtual void UpdateComponent() { }
		protected Rectangle GetCustomizationBounds() {
			BasePrimitiveInfo vInfo = Self.ViewInfo as BasePrimitiveInfo;
			RectangleF r = (scaleArcShape == null) ? vInfo.RelativeBoundBox :
				MathHelper.CalcRelativeBoundBox(ShapeHelper.GetShapeBounds(scaleArcShape, true), vInfo.LocalTransform);
			r.Inflate(r.Width * 0.01f, r.Height * 0.01f);
			return Rectangle.Round(r);
		}
		public void Assign(ArcScale source) {
			BeginUpdate();
			AssignPrimitiveProperties(source);
			if(source != null) {
				this.MinValue = source.MinValue;
				this.MaxValue = source.MaxValue;
				this.Value = source.Value;
				this.AutoRescaling = source.AutoRescaling;
				this.RescalingThresholdMin = source.RescalingThresholdMin;
				this.RescalingThresholdMax = source.RescalingThresholdMax;
				this.RescalingBestValues = source.RescalingBestValues;
				this.Appearance.Assign(source.Appearance);
				this.UseColorScheme = source.UseColorScheme;
				this.MinorTickmark.Assign(source.MinorTickmark);
				this.MajorTickmark.Assign(source.MajorTickmark);
				this.MinorTickCount = source.MinorTickCount;
				this.MajorTickCount = source.MajorTickCount;
				this.Labels.Assign(source.Labels);
				this.Ranges.Assign(source.Ranges);
				this.Logarithmic = source.Logarithmic;
				this.LogarithmicBase = source.LogarithmicBase;
				this.CustomLogarithmicBase = source.CustomLogarithmicBase;
				this.Center = source.Center;
				this.RadiusX = source.RadiusX;
				this.RadiusY = source.RadiusY;
				this.StartAngle = source.StartAngle;
				this.EndAngle = source.EndAngle;
			}
			EndUpdate();
		}
		public bool IsDifferFrom(ArcScale source) {
			return (source == null) ? true : IsDifferFromPrimitive(source) ||
							(this.MinValue != source.MinValue) ||
							(this.MaxValue != source.MaxValue) ||
							(this.Value != source.Value) ||
							(this.AutoRescaling != source.AutoRescaling) ||
							(this.RescalingThresholdMin != source.RescalingThresholdMin) ||
							(this.RescalingThresholdMax != source.RescalingThresholdMax) ||
							(this.RescalingBestValues != source.RescalingBestValues) ||
							this.Appearance.IsDifferFrom(source.Appearance) ||
							(this.UseColorScheme != source.UseColorScheme) ||
							this.MinorTickmark.IsDifferFrom(source.MinorTickmark) ||
							this.MajorTickmark.IsDifferFrom(source.MajorTickmark) ||
							(this.MinorTickCount != source.MinorTickCount) ||
							(this.MajorTickCount != source.MajorTickCount) ||
							(this.Labels.IsDifferFrom(source.Labels)) ||
							(this.Ranges.IsDifferFrom(source.Ranges)) ||
							(this.Center != source.Center) ||
							(this.RadiusX != source.RadiusX) ||
							(this.RadiusY != source.RadiusY) ||
							(this.StartAngle != source.StartAngle) ||
							(this.EndAngle != source.EndAngle) || (this.Logarithmic != source.Logarithmic) || (this.LogarithmicBase != source.LogarithmicBase) || (this.CustomLogarithmicBase != source.CustomLogarithmicBase);
		}
		protected override void OnShaderChangedCore() {
			Provider.UpdateTickmarks();
			base.OnShaderChangedCore();
		}
		public event EventHandler ValueChanged {
			add { Provider.ValueChanged += value; }
			remove { if(!IsDisposing) Provider.ValueChanged -= value; }
		}
		public event EventHandler MinMaxValueChanged {
			add { Provider.MinMaxValueChanged += value; }
			remove { if(!IsDisposing) Provider.MinMaxValueChanged -= value; }
		}
		public event EventHandler GeometryChanged {
			add { Provider.GeometryChanged += value; }
			remove { if(!IsDisposing) Provider.GeometryChanged -= value; }
		}
		public event EventHandler Animating {
			add { Provider.Animating += value; }
			remove { if(!IsDisposing) Provider.Animating -= value; }
		}
		public event EventHandler AnimationCompleted {
			add { Provider.AnimationCompleted += value; }
			remove { if(!IsDisposing) Provider.AnimationCompleted -= value; }
		}
		public event CustomTickmarkTextEventHandler CustomTickmarkText {
			add { Provider.CustomTickmarkText += value; }
			remove { if(!IsDisposing) Provider.CustomTickmarkText -= value; }
		}
		public event CustomRescalingEventHandler CustomRescaling {
			add { Provider.CustomRescaling += value; }
			remove { if(!IsDisposing) Provider.CustomRescaling -= value; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleIsEmpty")]
#endif
		public bool IsEmpty {
			get { return Provider.IsEmpty; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleIsDiscrete")]
#endif
		public bool IsDiscrete {
			get { return Provider.IsDiscrete; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleIsLogarithmic")]
#endif
		public bool IsLogarithmic {
			get { return Provider.IsLogarithmic; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleIsAnimating")]
#endif
		public virtual bool IsAnimating {
			get { return Provider.IsAnimating; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleLogarithmic"),
#endif
 DefaultValue(false),
		RefreshProperties(RefreshProperties.All)]
		[Category("Scale")]
		[XtraSerializableProperty(1)]
		public bool Logarithmic {
			get { return Provider.Logarithmic; }
			set { Provider.Logarithmic = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleCustomLogarithmicBase"),
#endif
 DefaultValue(2f)]
		[Category("Scale")]
		[XtraSerializableProperty]
		public float CustomLogarithmicBase {
			get { return Provider.CustomLogarithmicBase; }
			set { Provider.CustomLogarithmicBase = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleLogarithmicBase"),
#endif
 DefaultValue(LogarithmicBase.Binary),
		RefreshProperties(RefreshProperties.All)]
		[Category("Scale")]
		[XtraSerializableProperty]
		public LogarithmicBase LogarithmicBase {
			get { return Provider.LogarithmicBase; }
			set { Provider.LogarithmicBase = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleAppearance"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public BaseScaleAppearance Appearance {
			get { return Provider.Appearance; }
		}
#if !SL
	[DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleNeedleVector")]
#endif
		public PointF NeedleVector {
			get { return Provider.NeedleVector; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleAutoRescaling"),
#endif
XtraSerializableProperty, DefaultValue(false)]
		[Category("AutoRescaling")]
		public bool AutoRescaling {
			get { return Provider.AutoRescaling; }
			set { Provider.AutoRescaling = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleCenter"),
#endif
XtraSerializableProperty]
		[Category("Geometry")]
		public PointF2D Center {
			get { return Provider.Center; }
			set { Provider.Center = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleRadiusX"),
#endif
DefaultValue(100f)]
		[Category("Geometry")]
		[XtraSerializableProperty]
		public float RadiusX {
			get { return Provider.RadiusX; }
			set { Provider.RadiusX = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleRadiusY"),
#endif
DefaultValue(100f)]
		[Category("Geometry")]
		[XtraSerializableProperty]
		public float RadiusY {
			get { return Provider.RadiusY; }
			set { Provider.RadiusY = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleStartAngle"),
#endif
DefaultValue(0f)]
		[Category("Geometry")]
		[XtraSerializableProperty]
		public float StartAngle {
			get { return Provider.StartAngle; }
			set { Provider.StartAngle = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleEndAngle"),
#endif
DefaultValue(360f)]
		[Category("Geometry")]
		[XtraSerializableProperty]
		public float EndAngle {
			get { return Provider.EndAngle; }
			set { Provider.EndAngle = value; }
		}
		public PointF PercentToPoint(float percent) {
			return Provider.PercentToPoint(percent);
		}
		public float PointToPercent(PointF point) {
			return Provider.PointToPercent(point);
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScalePercent"),
#endif
DefaultValue(0f)]
		public float Percent {
			get { return Provider.Percent; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleValue"),
#endif
Bindable(true)]
		[XtraSerializableProperty(1)]
		[Category("Scale")]
		[DefaultValue(0f), RefreshProperties(RefreshProperties.All)]
		public float Value {
			get { return Provider.Value; }
			set { Provider.Value = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleMinValue"),
#endif
DefaultValue(0f), RefreshProperties(RefreshProperties.All)]
		[Category("Scale")]
		[XtraSerializableProperty(0)]
		public float MinValue {
			get { return Provider.MinValue; }
			set { Provider.MinValue = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleMaxValue"),
#endif
DefaultValue(1f), RefreshProperties(RefreshProperties.All)]
		[Category("Scale")]
		[XtraSerializableProperty(0)]
		public float MaxValue {
			get { return Provider.MaxValue; }
			set { Provider.MaxValue = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleRescalingThresholdMin"),
#endif
XtraSerializableProperty, DefaultValue(0.05f)]
		[Category("AutoRescaling")]
		public float RescalingThresholdMin {
			get { return Provider.RescalingThresholdMin; }
			set { Provider.RescalingThresholdMin = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleRescalingThresholdMax"),
#endif
XtraSerializableProperty, DefaultValue(0.05f)]
		[Category("AutoRescaling")]
		public float RescalingThresholdMax {
			get { return Provider.RescalingThresholdMax; }
			set { Provider.RescalingThresholdMax = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleRescalingBestValues"),
#endif
XtraSerializableProperty, DefaultValue(false)]
		[Category("AutoRescaling")]
		public bool RescalingBestValues {
			get { return Provider.RescalingBestValues; }
			set { Provider.RescalingBestValues = value; }
		}
		[Browsable(false)]
		public float ScaleLength {
			get { return Provider.ScaleLength; }
		}
		public float PercentToValue(float percent) {
			return Provider.PercentToValue(percent);
		}
		public float ValueToPercent(float value) {
			return Provider.ValueToPercent(value);
		}
		public float GetInternalValue() {
			return Provider.GetInternalValue();
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleMinorTickmark"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Tickmarks")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IMinorTickmark MinorTickmark {
			get { return Provider.MinorTickmark; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleMinorTickCount"),
#endif
DefaultValue(1)]
		[Category("Tickmarks")]
		[XtraSerializableProperty]
		public int MinorTickCount {
			get { return Provider.MinorTickCount; }
			set { Provider.MinorTickCount = value; }
		}
		[Browsable(false)]
		public int TickCount {
			get { return Provider.TickCount; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleMajorTickmark"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Tickmarks")]
		[XtraSerializableProperty(XtraSerializationVisibility.Content)]
		public IMajorTickmark MajorTickmark {
			get { return Provider.MajorTickmark; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleMajorTickCount"),
#endif
DefaultValue(11)]
		[Category("Tickmarks")]
		[XtraSerializableProperty]
		public int MajorTickCount {
			get { return Provider.MajorTickCount; }
			set { Provider.MajorTickCount = value; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleLabels"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Scale")]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		public virtual LabelCollection Labels {
			get { return Provider.Labels; }
		}
		[
#if !SL
	DevExpressXtraGaugesCoreLocalizedDescription("ArcScaleRanges"),
#endif
DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		[Category("Scale")]
		[XtraSerializableProperty(XtraSerializationVisibility.Collection, true)]
		public virtual RangeCollection Ranges {
			get { return Provider.Ranges; }
		}
		public IScaleRange CreateRange() {
			return Provider.CreateRange();
		}
		public IScaleLabel CreateLabel() {
			return Provider.CreateLabel();
		}
		protected override void OnLoadShapes() {
			base.OnLoadShapes();
			OnLoadScaleArcShape();
			OnLoadRangeShapes();
			OnLoadMinorTickmarkShapes();
			OnLoadMajorTickmarkShapes();
			OnLoadLabelShapes();
		}
		protected void OnLoadRangeShapes() {
			foreach(IRange range in Provider.Ranges) {
				Shapes.Add(range.Shape.Clone());
			}
		}
		protected void OnLoadLabelShapes() {
			foreach(ILabel label in Provider.Labels) {
				Shapes.Add(label.TextShape.Clone());
			}
		}
		protected void OnLoadMajorTickmarkShapes() {
			int counter = -1;
			bool showTick = MajorTickmark.ShowTick;
			bool showText = MajorTickmark.ShowText;
			foreach(IMajorTickmark tick in Provider.MajorTickmarks) {
				counter++;
				if(counter == 0 && !MajorTickmark.ShowFirst) continue;
				if(MajorTickCount > 0 && (counter == MajorTickCount - 1) && !MajorTickmark.ShowLast) continue;
				if(showTick) Shapes.Add(tick.Shape);
				if(showText) Shapes.Add(tick.TextShape);
			}
		}
		protected void OnLoadMinorTickmarkShapes() {
			int counter = -1;
			bool showTick = MinorTickmark.ShowTick;
			foreach(IMinorTickmark tick in Provider.MinorTickmarks) {
				counter++;
				if(counter == 0 && !MinorTickmark.ShowFirst) continue;
				if(TickCount > 0 && (counter == TickCount - 1) && !MinorTickmark.ShowLast) continue;
				if(!IsLogarithmic && (counter % (MinorTickCount + 1) == 0) && !MajorTickmark.AllowTickOverlap) continue;
				if(showTick) Shapes.Add(tick.Shape);
			}
		}
		protected void OnLoadScaleArcShape() {
			scaleArcShape.Appearance.BorderBrush = Appearance.Brush.Clone() as BrushObject;
			Shapes.Add(scaleArcShape);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateLabelsItem(XtraItemEventArgs e) {
			return Provider.XtraCreateLabelsItem(e);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public object XtraCreateRangesItem(XtraItemEventArgs e) {
			return Provider.XtraCreateRangesItem(e);
		}
		public virtual Color GetLabelShemeColor() {
			return Color.Empty;
		}
	}
	public static class ScaleLimitCalculator {
		static double NormalizationThreshold = 0.001d;
		public static double GetNearestBound(float value, float scaleLength, float minLimitFactor, float maxLimitFactor, bool fIsMaxBound) {
			double minLimit = value + (scaleLength > 0 ? -scaleLength * minLimitFactor : scaleLength * minLimitFactor);
			double maxLimit = value + (scaleLength > 0 ? scaleLength * maxLimitFactor : -scaleLength * maxLimitFactor);
			double findFrom = fIsMaxBound ? value : minLimit;
			double findTo = fIsMaxBound ? maxLimit : value;
			int p = Math.Max(CalcNormalizationPrecesion(findFrom), CalcNormalizationPrecesion(findTo));
			double normalizationMultiplier = Math.Pow(10d, p);
			double minInt = Math.Round(scaleLength > 0 ? findFrom : findTo, p) * normalizationMultiplier;
			double maxInt = Math.Round(scaleLength > 0 ? findTo : findFrom, p) * normalizationMultiplier;
			double[] divizors = GetDivizors(Math.Max(maxInt, minInt));
			double[] nearests = new double[divizors.Length];
			for(int i = 0; i < divizors.Length; i++) {
				double nearest = fIsMaxBound ? maxInt : minInt;
				nearests[i] = double.NaN;
				while(true) {
					nearest += (fIsMaxBound ? -1d : 1d);
					double remainder = Math.IEEERemainder(Math.Abs(nearest), divizors[i]);
					if(Math.Abs(remainder) < NormalizationThreshold) {
						nearests[i] = nearest;
						break;
					}
					if(fIsMaxBound && nearest < minInt) break;
					if(!fIsMaxBound && nearest > maxInt) break;
				}
			}
			double minF = Double.MaxValue;
			double result = nearests[0];
			for(int i = 0; i < divizors.Length; i++) {
				if(double.IsNaN(nearests[i])) continue;
				double f = Math.Abs((nearests[i] - (fIsMaxBound ? maxInt : minInt)) / divizors[i]);
				if(f < minF) {
					minF = f;
					result = nearests[i];
				}
			}
			result = result / normalizationMultiplier;
			return double.IsNaN(result) ? value : result;
		}
		static double[] GetDivizors(double max) {
			List<double> divizors = new List<double>();
			divizors.AddRange(new double[] { 5, 10, 20 });
			int count = 0;
			while(true) {
				double newDivisor = divizors[count] * 10d;
				if(newDivisor > max) break;
				divizors.Add(newDivisor);
				count++;
			}
			return divizors.ToArray();
		}
		internal static int CalcNormalizationPrecesion(double value) {
			if(value == 0) return 0;
			int precesion = 0;
			while(precesion < 15) {
				double rounded = Math.Round(value, precesion);
				double remainder = (rounded - value) / value;
				if(double.IsNaN(remainder)) break;
				if(Math.Abs(remainder) < NormalizationThreshold) break;
				precesion++;
			}
			return precesion;
		}
	}
	public class LogarithmicScaleObjectTypeConverter : ExpandableObjectConverter {		
		public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes) {
			return FilterPropertiesHelper.FilterProperties(base.GetProperties(context, value, attributes), GetExpectedProperties(value));
		}
		protected virtual string[] GetExpectedProperties() {
			return new string[] { };
		}
		string[] GetExpectedProperties(object value) {
			List<string> filteredProperties = new List<string>();
			ILogarithmicBase scale = value as ILogarithmicBase;
			filteredProperties.Add("(Name)");
			filteredProperties.AddRange(GetExpectedProperties());
			if(scale != null && scale.Logarithmic) {
				filteredProperties.Add("LogarithmicBase");
				if(scale.LogarithmicBase == LogarithmicBase.Custom)
					filteredProperties.Add("CustomLogarithmicBase");
			}
			return filteredProperties.ToArray();
		}
	}
	public class LinearLogarithmicScaleObjectTypeConverter : LogarithmicScaleObjectTypeConverter {
		protected override string[] GetExpectedProperties() {
			return new string[]{
				"DataBindings", "Tag",
				"Name","ZOrder",
				"Shader",
				"AppearanceScale" , "AppearanceMinorTickmark", "AppearanceMajorTickmark", "AppearanceTickmarkTextBackground", "AppearanceTickmarkText", "UseColorScheme",
				"StartPoint","EndPoint",
				"MinValue","MaxValue","Value","Logarithmic",
				"AutoRescaling", "RescalingThresholdMin", "RescalingThresholdMax", "RescalingBestValues",
				"MinorTickmark", "MinorTickCount",
				"MajorTickmark", "MajorTickCount",
				"Labels","Ranges"
			};
		}
	}
	public class ArcLogarithmicScaleObjectTypeConverter : LogarithmicScaleObjectTypeConverter {
		protected override string[] GetExpectedProperties() {
			return new string[]{
				"DataBindings", "Tag",
				"Name","ZOrder",
				"Shader",
				"AppearanceScale" , "AppearanceMinorTickmark", "AppearanceMajorTickmark", "AppearanceTickmarkTextBackground", "AppearanceTickmarkText", "UseColorScheme",
				"Center","RadiusX","RadiusY",
				"StartAngle","EndAngle",
				"MinValue","MaxValue","Value","Logarithmic",
				"AutoRescaling", "RescalingThresholdMin", "RescalingThresholdMax", "RescalingBestValues",
				"MinorTickmark", "MinorTickCount",
				"MajorTickmark", "MajorTickCount",
				"Labels","Ranges"
			};
		}
	}
}
