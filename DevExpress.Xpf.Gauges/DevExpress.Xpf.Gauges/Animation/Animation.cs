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
using System.Windows;
using System.Windows.Media.Animation;
using DevExpress.Xpf.Gauges.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.Gauges {
	public abstract class AnimationBase : GaugeDependencyObject {
		public static readonly DependencyProperty EnableProperty = DependencyPropertyManager.Register("Enable",
			typeof(bool), typeof(AnimationBase), new PropertyMetadata(true, NotifyPropertyChanged));
		[
		Category(Categories.Behavior)
		]
		public bool Enable {
			get { return (bool)GetValue(EnableProperty); }
			set { SetValue(EnableProperty, value); }
		}
	}
	public class IndicatorAnimation : AnimationBase {
		public static readonly DependencyProperty DurationProperty = DependencyPropertyManager.Register("Duration",
			typeof(TimeSpan), typeof(IndicatorAnimation), new PropertyMetadata(TimeSpan.FromMilliseconds(800), NotifyPropertyChanged));
		public static readonly DependencyProperty EasingFunctionProperty = DependencyPropertyManager.Register("EasingFunction",
			typeof(IEasingFunction), typeof(IndicatorAnimation), new PropertyMetadata(null, NotifyPropertyChanged));
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("IndicatorAnimationDuration"),
#endif
		Category(Categories.Behavior)
		]
		public TimeSpan Duration {
			get { return (TimeSpan)GetValue(DurationProperty); }
			set { SetValue(DurationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGaugesLocalizedDescription("IndicatorAnimationEasingFunction"),
#endif
		Category(Categories.Behavior)
		]
		public IEasingFunction EasingFunction {
			get { return (IEasingFunction)GetValue(EasingFunctionProperty); }
			set { SetValue(EasingFunctionProperty, value); }
		}
		IEasingFunction GetDefaultEasingFunction() {
			return null;
		}
		Timeline CreateTimeline() {
			DoubleAnimation timeline = new DoubleAnimation();
			timeline.From = 0;
			timeline.To = 1.0;
			timeline.Duration = Duration;
			timeline.BeginTime = TimeSpan.Zero;
			timeline.EasingFunction = EasingFunction != null ? EasingFunction : GetDefaultEasingFunction();
			return timeline;
		}
		internal void PrepareStoryboard(Storyboard storyboard, ValueIndicatorBase indicator) {
			Timeline timeline = CreateTimeline();
			Storyboard.SetTarget(timeline, indicator.Progress);
			Storyboard.SetTargetProperty(timeline, new PropertyPath(AnimationProgress.ProgressProperty.GetName()));
			storyboard.Children.Add(timeline);
			storyboard.BeginTime = TimeSpan.Zero;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new IndicatorAnimation();
		}
	}
	public abstract class SymbolsAnimation : AnimationBase {
		public static readonly DependencyProperty RefreshTimeProperty = DependencyPropertyManager.Register("RefreshTime",
			typeof(TimeSpan), typeof(SymbolsAnimation), new PropertyMetadata(TimeSpan.FromMilliseconds(400), NotifyPropertyChanged));
		[
		Category(Categories.Behavior)
		]
		public TimeSpan RefreshTime {
			get { return (TimeSpan)GetValue(RefreshTimeProperty); }
			set { SetValue(RefreshTimeProperty, value); }
		}
		protected internal abstract bool ShouldReplay { get; } 
		Timeline CreateTimeline(TimeSpan duration) {
			DoubleAnimation timeline = new DoubleAnimation();
			timeline.From = 0;
			timeline.To = 1.0;
			timeline.Duration = duration;
			timeline.BeginTime = TimeSpan.Zero;
			return timeline;
		}
		void PrepareStoryboard(Storyboard storyboard, SymbolViewInternal symbolViewInternal, int animationInterval, bool firstPlay) {
			symbolViewInternal.Progress.IntervalCount = animationInterval;
			symbolViewInternal.Progress.InitialOffset = firstPlay ? GetInitialOffset(symbolViewInternal) : 0;
			TimeSpan duration = TimeSpan.FromMilliseconds(animationInterval * RefreshTime.TotalMilliseconds);
			Timeline timeline = CreateTimeline(duration);
			Storyboard.SetTarget(timeline, symbolViewInternal.Progress);
			Storyboard.SetTargetProperty(timeline, new PropertyPath(AnimationProgress.ProgressProperty.GetName()));
			storyboard.Children.Add(timeline);
			storyboard.BeginTime = TimeSpan.Zero;
			storyboard.RepeatBehavior = firstPlay ? new RepeatBehavior(1) : RepeatBehavior.Forever;
		}
		void PrepareStates(SymbolViewInternal symbolViewInternal, List<SymbolState> states, int animationIntervalCount, bool firstPlay) {
			if(firstPlay)
				PrepareStatesForFirstPlay(symbolViewInternal, states);
			while (states.Count < animationIntervalCount)
				if (symbolViewInternal.Gauge.TextDirection == TextDirection.RightToLeft)
					states.Insert(0, symbolViewInternal.GetEmptySymbolState());
				else
					states.Add(symbolViewInternal.GetEmptySymbolState());
		}
		protected virtual int GetInitialOffset(SymbolViewInternal symbolViewInternal) {
			return 0;
		}
		protected virtual void PrepareStatesForFirstPlay(SymbolViewInternal symbolViewInternal, List<SymbolState> states) {		 
		}
		protected abstract int GetAnimationIntervalCount(SymbolViewInternal symbolViewInternal, bool firstPlay);
		internal void Prepare(Storyboard storyboard, SymbolViewInternal symbolViewInternal, List<SymbolState> states, bool firstPlay) {
			int intervalCount = GetAnimationIntervalCount(symbolViewInternal, firstPlay);
			PrepareStoryboard(storyboard, symbolViewInternal, intervalCount, firstPlay);
			PrepareStates(symbolViewInternal, states, intervalCount, firstPlay);
		}
		protected internal abstract List<SymbolState> AnimateSymbolsStates(List<SymbolState> states, SymbolViewInternal symbolViewInternal);
		protected internal virtual void RaiseCompletedEvent() {
		}
	}
	public enum CreepingLineDirection {
		LeftToRight,
		RightToLeft
	}
	public class CreepingLineAnimation : SymbolsAnimation {
		const int defaultRepeatSpaces = 3;
		const int defaultEndSpaces = 0;
		[Obsolete(ObsoleteMessages.StartSpacesProperty), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static readonly DependencyProperty StartSpacesProperty = DependencyPropertyManager.Register("StartSpaces",
			typeof(int), typeof(CreepingLineAnimation), new PropertyMetadata(-1, StartSpacesPropertyChanged));
		[Obsolete(ObsoleteMessages.AdditionalSpacesProperty), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public static readonly DependencyProperty AdditionalSpacesProperty = DependencyPropertyManager.Register("AdditionalSpaces",
			typeof(int), typeof(CreepingLineAnimation), new PropertyMetadata(-1, AdditionalSpacesPropertyChanged));
		static void StartSpacesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CreepingLineAnimation animation = d as CreepingLineAnimation;
			if (animation != null) 
				animation.InitialMoves = (int)e.NewValue;			
		}
		static void AdditionalSpacesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CreepingLineAnimation animation = d as CreepingLineAnimation;
			if (animation != null) {
				int value = (int)e.NewValue;
				if (value < 0) {
					animation.RepeatSpaces = defaultRepeatSpaces;
					animation.FinalMoves = defaultEndSpaces;
				} else {
					animation.RepeatSpaces = value;
					animation.FinalMoves = value;
				}
			}
		}
		public static readonly DependencyProperty InitialMovesProperty = DependencyPropertyManager.Register("InitialMoves",
			typeof(int), typeof(CreepingLineAnimation), new PropertyMetadata(-1, NotifyPropertyChanged));
		public static readonly DependencyProperty FinalMovesProperty = DependencyPropertyManager.Register("FinalMoves",
			typeof(int), typeof(CreepingLineAnimation), new PropertyMetadata(defaultEndSpaces, NotifyPropertyChanged), FinalMovesValidation);
		public static readonly DependencyProperty RepeatSpacesProperty = DependencyPropertyManager.Register("RepeatSpaces",
			typeof(int), typeof(CreepingLineAnimation), new PropertyMetadata(defaultRepeatSpaces, NotifyPropertyChanged), RepeatSpacesValidation);		
		public static readonly DependencyProperty RepeatProperty = DependencyPropertyManager.Register("Repeat",
			typeof(bool), typeof(CreepingLineAnimation), new PropertyMetadata(false, NotifyPropertyChanged));
		public static readonly DependencyProperty DirectionProperty = DependencyPropertyManager.Register("Direction",
			typeof(CreepingLineDirection), typeof(CreepingLineAnimation), new PropertyMetadata(CreepingLineDirection.RightToLeft, NotifyPropertyChanged));
		static bool RepeatSpacesValidation(object value) {
			return (int)value > -1;
		}
		static bool FinalMovesValidation(object value) {
			return (int)value > -1;
		}
		[Obsolete(ObsoleteMessages.StartSpacesProperty), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int StartSpaces {
			get { return (int)GetValue(StartSpacesProperty); }
			set { SetValue(StartSpacesProperty, value); }
		}
		[Obsolete(ObsoleteMessages.AdditionalSpacesProperty), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int AdditionalSpaces {
			get { return (int)GetValue(AdditionalSpacesProperty); }
			set { SetValue(AdditionalSpacesProperty, value); }
		}
		[
		Category(Categories.Behavior)
		]
		public bool Repeat {
			get { return (bool)GetValue(RepeatProperty); }
			set { SetValue(RepeatProperty, value); }
		}
		[
		Category(Categories.Behavior)
		]
		public CreepingLineDirection Direction {
			get { return (CreepingLineDirection)GetValue(DirectionProperty); }
			set { SetValue(DirectionProperty, value); }
		}
		[
		Category(Categories.Behavior)
		]
		public int InitialMoves {
			get { return (int)GetValue(InitialMovesProperty); }
			set { SetValue(InitialMovesProperty, value); }
		}
		[
		Category(Categories.Behavior)
		]
		public int FinalMoves {
			get { return (int)GetValue(FinalMovesProperty); }
			set { SetValue(FinalMovesProperty, value); }
		}
		[
		Category(Categories.Behavior)
		]
		public int RepeatSpaces {
			get { return (int)GetValue(RepeatSpacesProperty); }
			set { SetValue(RepeatSpacesProperty, value); }
		}
		public event CreepingLineAnimationCompletedEventHandler CreepingLineAnimationCompleted;
		protected internal override bool ShouldReplay { get { return Repeat; } }
		int GetActualStartSpaces(SymbolViewInternal symbolViewInternal) {
			if (InitialMoves < 0) {
				if ((symbolViewInternal.Gauge.TextDirection == TextDirection.LeftToRight && Direction == CreepingLineDirection.LeftToRight) ||
					(symbolViewInternal.Gauge.TextDirection == TextDirection.RightToLeft && Direction == CreepingLineDirection.RightToLeft))
					return symbolViewInternal.SeparateTextToSymbols(symbolViewInternal.Gauge.Text).Count;
				return symbolViewInternal.Gauge.ActualSymbolCount;
			}
			return InitialMoves;
		}
		protected override GaugeDependencyObject CreateObject() {
			return new CreepingLineAnimation();
		}
		protected override int GetAnimationIntervalCount(SymbolViewInternal symbolViewInternal, bool firstPlay) {
			List<string> symbols = symbolViewInternal.SeparateTextToSymbols(symbolViewInternal.Gauge.Text);
			int startSpaces = GetActualStartSpaces(symbolViewInternal);
			if (firstPlay) 
				return Repeat ? startSpaces : startSpaces + FinalMoves;
			int count = Repeat ? symbols.Count + RepeatSpaces : symbols.Count;
			return Math.Max(symbolViewInternal.Gauge.ActualSymbolCount, count);
		}
		protected override void PrepareStatesForFirstPlay(SymbolViewInternal symbolViewInternal, List<SymbolState> states) {
			int symbolCount = GetActualStartSpaces(symbolViewInternal);
			for (int i = 0; i < symbolCount; i++) {
				if (Direction == CreepingLineDirection.LeftToRight)
					states.Add(symbolViewInternal.GetEmptySymbolState());
				else
					states.Insert(0, symbolViewInternal.GetEmptySymbolState());
			}
			for (int i = 0; i < FinalMoves; i++) {
				if (Direction == CreepingLineDirection.RightToLeft)
					states.Add(symbolViewInternal.GetEmptySymbolState());
				else
					states.Insert(0, symbolViewInternal.GetEmptySymbolState());
			}
		}
		protected override int GetInitialOffset(SymbolViewInternal symbolViewInternal) {
			if ((Direction == CreepingLineDirection.LeftToRight && symbolViewInternal.Gauge.TextDirection == TextDirection.LeftToRight) ||
				Direction == CreepingLineDirection.RightToLeft && symbolViewInternal.Gauge.TextDirection == TextDirection.RightToLeft)
				return -GetActualStartSpaces(symbolViewInternal) - FinalMoves;
			return 0;
		}
		protected internal override List<SymbolState> AnimateSymbolsStates(List<SymbolState> states, SymbolViewInternal symbolViewInternal) {
			List<SymbolState> result = new List<SymbolState>();
			int offset = Direction == CreepingLineDirection.LeftToRight ? -symbolViewInternal.Progress.ActualIntegerProgress : symbolViewInternal.Progress.ActualIntegerProgress;
			for (int i = 0; i < states.Count; i++) {
				int index = i + offset;
				if (index < 0)
					index += (int)(Math.Ceiling((-index / (double)states.Count)) * states.Count);
				result.Add(states[index % states.Count]);
			}
			return result;
		}
		protected internal override void RaiseCompletedEvent() {
			if (CreepingLineAnimationCompleted != null)
				CreepingLineAnimationCompleted(this, new EventArgs());
		}
	}
	public class BlinkingAnimation : SymbolsAnimation {
		public static readonly DependencyProperty SymbolsStatesProperty = DependencyPropertyManager.Register("SymbolsStates",
		   typeof(StatesMask), typeof(BlinkingAnimation), new PropertyMetadata(new StatesMask(), NotifyPropertyChanged));
		[
		Category(Categories.Behavior)
		]
		public StatesMask SymbolsStates {
			get { return (StatesMask)GetValue(SymbolsStatesProperty); }
			set { SetValue(SymbolsStatesProperty, value); }
		}
		protected internal override bool ShouldReplay { get { return true; } }
		protected override GaugeDependencyObject CreateObject() {
			return new BlinkingAnimation();
		}
		protected override int GetAnimationIntervalCount(SymbolViewInternal symbolViewInternal, bool firstPlay) {
			return 2;
		}
		protected internal override List<SymbolState> AnimateSymbolsStates(List<SymbolState> symbolsState, SymbolViewInternal symbolViewInternal) {
			List<SymbolState> result = new List<SymbolState>();
			for (int i = 0; i < symbolsState.Count; i++) {
				bool mask = (SymbolsStates.States != null && i < SymbolsStates.States.Length) ? SymbolsStates.States[i] : true;
				if (mask && symbolViewInternal.Progress.IntegerProgress > 0)
					result.Add(new SymbolState(string.Empty, symbolsState[i].Segments.Length, false));
				else
					result.Add(symbolsState[i]);
			}
			return result;
		}
	}
}
namespace DevExpress.Xpf.Gauges.Native {
	public class AnimationProgress : DependencyObject {
		public static readonly DependencyProperty ProgressProperty = DependencyPropertyManager.Register("Progress",
			typeof(double), typeof(AnimationProgress), new PropertyMetadata(1.0, ProgressPropertyChanged));
		public double Progress {
			get { return (double)GetValue(ProgressProperty); }
			set { SetValue(ProgressProperty, value); }
		}
		static void ProgressPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			AnimationProgress animationProgress = d as AnimationProgress;
			if (animationProgress != null)
				animationProgress.OnProgressChanged();
		}
		readonly IAnimatableElement element;
		protected IAnimatableElement Element { get { return element; } }
		protected bool InProgress {
			get { return element != null ? element.InProgress : false; }
		}
		public double ActualProgress {
			get {
				if (InProgress)
					return Progress;
				return 1.0;
			}
		}
		public AnimationProgress(IAnimatableElement element) {
			this.element = element;
		}
		protected void RaiseProgressChanged() {
			if (element != null)
				element.ProgressChanged();
		}
		protected virtual void OnProgressChanged() {
			RaiseProgressChanged();
		}
		public void Start() {
			Progress = 0.0;
			OnProgressChanged();
			RaiseProgressChanged();
		}
		public void Finish() {
			Progress = 1.0;
		}
	}
	public class IntegerAnimationProgress : AnimationProgress {
		public static readonly DependencyProperty InitialOffsetProperty = DependencyPropertyManager.Register("InitialOffset",
			typeof(int), typeof(IntegerAnimationProgress), new PropertyMetadata(0));
		public static readonly DependencyProperty IntervalCountProperty = DependencyPropertyManager.Register("IntervalCount",
			typeof(int), typeof(IntegerAnimationProgress), new PropertyMetadata(1));
		public static readonly DependencyProperty IntegerProgressProperty = DependencyPropertyManager.Register("IntegerProgress",
			typeof(int), typeof(IntegerAnimationProgress), new PropertyMetadata(-1, ProgressPropertyChanged));
		public int InitialOffset {
			get { return (int)GetValue(InitialOffsetProperty); }
			set { SetValue(InitialOffsetProperty, value); }
		}
		public int IntervalCount {
			get { return (int)GetValue(IntervalCountProperty); }
			set { SetValue(IntervalCountProperty, value); }
		}
		public int IntegerProgress {
			get { return (int)GetValue(IntegerProgressProperty); }
			set { SetValue(IntegerProgressProperty, value); }
		}
		static void ProgressPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if (e.NewValue != e.OldValue) {
				IntegerAnimationProgress animationProgress = d as IntegerAnimationProgress;
				if (animationProgress != null)
					animationProgress.RaiseProgressChanged();
			}
		}
		public int ActualIntegerProgress {
			get { return InitialOffset + IntegerProgress; }
		}
		public IntegerAnimationProgress(IAnimatableElement element)
			: base(element) {
		}
		protected override void OnProgressChanged() {
			IntegerProgress = (int)Math.Floor(ActualProgress * IntervalCount);			
		}
	}
}
