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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Gauges.Localization;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class ValueIndicatorBase : GaugeDependencyObject, IOwnedElement, IWeakEventListener, IAnimatableElement, IModelSupported, ILayoutCalculator {
		public static readonly DependencyProperty ValueProperty = DependencyPropertyManager.Register("Value",
			typeof(double), typeof(ValueIndicatorBase), new PropertyMetadata(0.0, ValuePropertyChanged));
		public static readonly DependencyProperty AnimationProperty = DependencyPropertyManager.Register("Animation",
			typeof(IndicatorAnimation), typeof(ValueIndicatorBase), new PropertyMetadata(null, AnimationPropertyChanged));
		public static readonly DependencyProperty IsHitTestVisibleProperty = DependencyPropertyManager.Register("IsHitTestVisible",
			typeof(bool), typeof(ValueIndicatorBase), new PropertyMetadata(true));
		public static readonly DependencyProperty IsInteractiveProperty = DependencyPropertyManager.Register("IsInteractive",
			typeof(bool), typeof(ValueIndicatorBase), new PropertyMetadata(false));
		public static readonly DependencyProperty VisibleProperty = DependencyPropertyManager.Register("Visible",
			typeof(bool), typeof(ValueIndicatorBase), new PropertyMetadata(true, IndicatorPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ValueIndicatorBaseValue"),
#endif
		Category(Categories.Data)
		]
		public double Value {
			get { return (double)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ValueIndicatorBaseAnimation"),
#endif
		Category(Categories.Animation)
		]
		public IndicatorAnimation Animation {
			get { return (IndicatorAnimation)GetValue(AnimationProperty); }
			set { SetValue(AnimationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ValueIndicatorBaseIsHitTestVisible"),
#endif
		Category(Categories.Behavior)
		]
		public bool IsHitTestVisible {
			get { return (bool)GetValue(IsHitTestVisibleProperty); }
			set { SetValue(IsHitTestVisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ValueIndicatorBaseIsInteractive"),
#endif
		Category(Categories.Behavior)
		]
		public bool IsInteractive {
			get { return (bool)GetValue(IsInteractiveProperty); }
			set { SetValue(IsInteractiveProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("ValueIndicatorBaseVisible"),
#endif
		Category(Categories.Behavior)
		]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		static void ValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ValueIndicatorBase indicator = d as ValueIndicatorBase;
			if (indicator != null) {
				indicator.anchorValue = indicator.CalcActualValue((double)e.OldValue);
				indicator.RequestAnimation(false);
				IndicatorPropertyChanged(d, e);
				indicator.OnValueChanged((double)e.OldValue, (double)e.NewValue);
				if(indicator.Scale != null)
					indicator.Scale.CheckIndicatorEnterLeaveRange(indicator, (double)e.OldValue, (double)e.NewValue);
			}			
		}
		static void AnimationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ArcScaleIndicator indicator = d as ArcScaleIndicator;
			if (indicator != null) {
				CommonUtils.SubscribePropertyChangedWeakEvent(e.OldValue as IndicatorAnimation, e.NewValue as IndicatorAnimation, indicator);
				indicator.OnAnimationChanged();
			}
		}
		protected static void IndicatorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ValueIndicatorBase indicator = d as ValueIndicatorBase;
			if (indicator != null)
				indicator.Invalidate();
		}
		protected static void PresentationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			ValueIndicatorBase indicator = d as ValueIndicatorBase;
			if (indicator != null && !Object.Equals(e.NewValue, e.OldValue))
				indicator.UpdatePresentation();
		}
		static double defaultAnchorValue = 0.0;
		readonly ValueIndicatorInfo info;
		object owner = null;
		double anchorValue = defaultAnchorValue;
		Storyboard storyboard = null;
		AnimationProgress progress = null;
		bool animationInProgress = false;
		bool isLockedAnimation = false;
		StateIndicatorControl stateIndicator = null;
		public event ValueChangedEventHandler ValueChanged;
		IndicatorAnimation ActualAnimation {
			get {
				if (Animation != null)
					return Animation;
				return new IndicatorAnimation();
			}
		}
		protected object Owner { get { return owner; } }
		protected AnalogGaugeControl Gauge { get { return Scale != null ? Scale.Gauge : null; } }
		protected abstract ValueIndicatorPresentation ActualPresentation { get; }
		protected abstract int ActualZIndex { get; }
		protected internal virtual IEnumerable<ValueIndicatorInfo> Elements { get { yield return info; } }
		internal double ActualValue { get { return CalcActualValue(Value); } }
		internal bool ShouldAnimate {
			get {
				if (!isLockedAnimation) {
					if (Animation != null && Scale != null)
						return Animation.Enable;
					return Gauge != null ? Gauge.EnableAnimation : false;
				}
				return false;
			}
		}
		internal AnimationProgress Progress { get { return progress; } }
		internal Storyboard Storyboard {
			get {
				if (storyboard == null && Scale != null) {
					storyboard = new Storyboard();
					storyboard.Completed += new EventHandler(OnStoryboardCompleted);
					Scale.AddStoryboard(storyboard, this.GetHashCode());
				}
				return storyboard;
			}
		}
		internal Scale Scale { get { return Owner as Scale; } }
		internal bool IsLockedAnimation { get { return isLockedAnimation; } set { isLockedAnimation = value; } }
		internal ValueIndicatorInfo ElementInfo { get { return info; } }
		internal StateIndicatorControl StateIndicator { get { return stateIndicator; } 
			set { stateIndicator = value; } }
		public ValueIndicatorBase() {
			progress = new AnimationProgress(this);
			info = new ValueIndicatorInfo(this, ActualZIndex, ActualPresentation.CreateIndicatorPresentationControl(), ActualPresentation);
		}
		#region IOwnedElement implementation
		object IOwnedElement.Owner {
			get { return owner; }
			set {
				if (StateIndicator != null) {
					stateIndicator.SubscribeRangeCollectionEvents(value as Scale, owner as Scale);
					stateIndicator.UpdateStateIndexByValueIndicator(this);
				}
				owner = value;
				ChangeOwner();
			}
		}
		#endregion
		#region IWeakEventListener implementation
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return PerformWeakEvent(managerType, sender, e);
		}
		#endregion
		#region IAnimatableElement implementation
		bool IAnimatableElement.InProgress { get { return animationInProgress; } }
		void IAnimatableElement.ProgressChanged() {
			Invalidate();
		}
		#endregion
		#region IModelSupported implementation
		void IModelSupported.UpdateModel() {
			UpdatePresentation();
			OnOptionsChanged();
		}
		#endregion
		#region ILayoutCalculator implementation
		ElementLayout ILayoutCalculator.CreateLayout(Size constraint) {
			return Visible ? CreateLayout(constraint) : null;
		}
		void ILayoutCalculator.CompleteLayout(ElementInfoBase elementInfo) {
			CompleteLayout(elementInfo);
		}
		#endregion
		double CalcActualValue(double value) {
			double limitedValue = Scale!= null ? Scale.GetLimitedValue(value) : value;
			return anchorValue + (limitedValue - anchorValue) * progress.ActualProgress;
		}
		void StopAnimation() {
			animationInProgress = false;
		}
		void RequestAnimation(bool shouldResetValue) {
			StopAnimation();
			if (ShouldAnimate)
				Animate(shouldResetValue);
		}
		void PrepareStoryboard() {
			if (Storyboard.Children.Count > 0) {
				Storyboard.Stop();
				Storyboard.Children.Clear();
			}
			ActualAnimation.PrepareStoryboard(Storyboard, this);
		}
		void OnStoryboardCompleted(object sender, EventArgs e) {
			anchorValue = Value;
			StopAnimation();
		}
		void OnAnimationChanged() {
			if (Scale != null && DesignerProperties.GetIsInDesignMode(Scale))
				RequestAnimation(true);
		}
		void OnValueChanged(double oldValue, double newValue) {
			if (ValueChanged != null)
				ValueChanged(this, new ValueChangedEventArgs(oldValue, newValue));
		}
		protected virtual void UpdatePresentation() {
			if(ElementInfo != null) {
				ElementInfo.Presentation = ActualPresentation;
				ElementInfo.PresentationControl = ActualPresentation.CreateIndicatorPresentationControl();
			}
			Invalidate();
		}
		void Invalidate() {
			foreach (ValueIndicatorInfo info in Elements)
				info.Invalidate();
		}
		protected virtual void ChangeOwner() {
			((IModelSupported)this).UpdateModel();			
		}
		protected virtual bool PerformWeakEvent(Type managerType, object sender, EventArgs e) {
			bool success = false;
			if (managerType == typeof(PropertyChangedWeakEventManager)) {
				if((sender is GaugeDependencyObject)) {
					OnOptionsChanged();
					success = true;
				}
				if ((sender is IndicatorAnimation)) {
					OnAnimationChanged();
					success = true;
				}
			}
			return success;
		}
		protected void OnOptionsChanged() {
			foreach (ValueIndicatorInfo info in Elements) {
				info.ZIndex = ActualZIndex;
			}
			Invalidate();
		}
		protected abstract ElementLayout CreateLayout(Size constraint);
		protected abstract void CompleteLayout(ElementInfoBase elementInfo);		
		internal void Animate(bool shouldResetValue) {
			if (shouldResetValue)
				anchorValue = defaultAnchorValue;
			StopAnimation();			
			if (ShouldAnimate) {
				PrepareStoryboard();
				progress.Start();
				animationInProgress = true;
				Storyboard.Begin();
			}
		}
		internal void ClearAnimation() {
			if (storyboard != null) {
				storyboard.Stop();
				storyboard.Completed -= OnStoryboardCompleted;
				Scale.RemoveStoryboard(this.GetHashCode());
				storyboard = null;
			}
		}
	}
	public class ValueIndicatorAutomationPeer : FrameworkElementAutomationPeer, IRangeValueProvider {
		ValueIndicatorBase ValueIndicator {
			get {
				ValueIndicatorPresentationControl presentationControl = Owner as ValueIndicatorPresentationControl;
				if (presentationControl != null)
					return presentationControl.ValueIndicator;
				else
					return null;
			}
		}
		public ValueIndicatorAutomationPeer(FrameworkElement owner)
			: base(owner) {
		}
		protected override string GetClassNameCore() {
			if (ValueIndicator is ArcScaleNeedle)
				return "Needle";
			if (ValueIndicator is ArcScaleMarker || ValueIndicator is LinearScaleMarker)
				return "Marker";
			if (ValueIndicator is ArcScaleRangeBar || ValueIndicator is LinearScaleRangeBar)
				return "RangeBar";
			if (ValueIndicator is LinearScaleLevelBar)
				return "LevelBar";
			return "ValueIndicatorBase";
		}
		protected override string GetLocalizedControlTypeCore() {
			if (ValueIndicator is ArcScaleNeedle)
				return GaugeLocalizer.GetString(GaugeStringId.NeedleLocalizedControlType);
			if (ValueIndicator is ArcScaleMarker || ValueIndicator is LinearScaleMarker)
				return GaugeLocalizer.GetString(GaugeStringId.MarkerLocalizedControlType);
			if (ValueIndicator is ArcScaleRangeBar || ValueIndicator is LinearScaleRangeBar)
				return GaugeLocalizer.GetString(GaugeStringId.RangeBarLocalizedControlType);
			if (ValueIndicator is LinearScaleLevelBar)
				return GaugeLocalizer.GetString(GaugeStringId.LevelBarLocalizedControlType);
			return GaugeLocalizer.GetString(GaugeStringId.ValueIndicatorLocalizedControlType);
		}
		protected override bool IsContentElementCore() {
			return false;
		}
		#region IRangeValueProvider implementation
		bool IRangeValueProvider.IsReadOnly { get { return false; } }
		double IRangeValueProvider.LargeChange { get { return Double.NaN; } }
		double IRangeValueProvider.Maximum {
			get {
				if (ValueIndicator != null && ValueIndicator.Scale != null)
					return Math.Max(ValueIndicator.Scale.StartValue, ValueIndicator.Scale.EndValue);
				else
					return Double.NaN;
			}
		}
		double IRangeValueProvider.Minimum {
			get {
				if (ValueIndicator != null && ValueIndicator.Scale != null)
					return Math.Min(ValueIndicator.Scale.StartValue, ValueIndicator.Scale.EndValue);
				return Double.NaN;
			}
		}
		void IRangeValueProvider.SetValue(double value) {
			if (ValueIndicator != null)
				ValueIndicator.SetValue(ValueIndicatorBase.ValueProperty, value);
		}
		double IRangeValueProvider.SmallChange { get { return Double.NaN; } }
		double IRangeValueProvider.Value {
			get {
				if (ValueIndicator != null)
					return ValueIndicator.Value;
				else
					return Double.NaN;
			}
		}
		#endregion
		public override object GetPattern(PatternInterface patternInterface) {
			if (patternInterface == PatternInterface.RangeValue)
				return this;
			return base.GetPattern(patternInterface);
		}
	}
	public abstract class ValueIndicatorCollection<T> : GaugeDependencyObjectCollection<T> where T : ValueIndicatorBase {
		Scale Scale { get { return Owner as Scale; } }
		public ValueIndicatorCollection(Scale scale) {
			((IOwnedElement)this).Owner = scale;
		}
		protected override void ClearItems() {
			if (Scale != null)
				foreach (ValueIndicatorBase indicator in this)
					Scale.RemoveStoryboard(indicator.GetHashCode());
			base.ClearItems();
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if (Scale != null) {
				if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
					if (e.OldItems != null)
						foreach (object item in e.OldItems)
							Scale.RemoveStoryboard(item.GetHashCode());
				if (Scale.Gauge != null)
					Scale.Gauge.UpdateElements();
			}
		}
	}
	[NonCategorized]
	public class ValueIndicatorInfo : ElementInfoBase {
		readonly ValueIndicatorBase indicator;
		internal ValueIndicatorBase Indicator { get { return indicator; } }
		protected internal override Object HitTestableObject { get { return indicator; } }
		protected internal override object HitTestableParent { get { return indicator.Scale; } }
		protected internal override bool IsHitTestVisible { get { return indicator.IsHitTestVisible; } }
		internal ValueIndicatorInfo(ValueIndicatorBase indicator, int zIndex, PresentationControl presentationControl, PresentationBase presentation)
			: base(indicator, zIndex, presentationControl, presentation) {
			this.indicator = indicator;
		}
	}
}
