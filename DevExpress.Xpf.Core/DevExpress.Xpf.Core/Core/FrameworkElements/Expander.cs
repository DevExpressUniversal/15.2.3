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
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Animation;
using System.Windows.Data;
using DevExpress.Xpf.Core.Native;
using System.Windows.Markup;
using System.Windows.Controls;
using System.ComponentModel;
#if !SILVERLIGHT
#if !DEMO
using DevExpress.Xpf.Utils.Native;
using DevExpress.Xpf.Utils;
#else
using DevExpress.Xpf.Map;
#endif
#endif
#if SILVERLIGHT
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.Core {
#if !SILVERLIGHT
	public class AddDoubleValueConverter : MarkupExtension, IValueConverter {
		#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return (double)value + double.Parse((string)parameter);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			return null;
		}
		#endregion
		public override object ProvideValue(IServiceProvider serviceProvider) {
			return this;
		}
	}
#endif
	public class ExpandCollapseInfoEventArgs : EventArgs {
		public Size Size { get; set; }
		public DXExpander Expander { get; set; }
	}
	public delegate void ExpandCollapseInfoEventHandler(object sender, ExpandCollapseInfoEventArgs e);
#if SILVERLIGHT
	public class DXExpander : Panel {
#else
	public class DXExpander : Decorator {
#endif
		private static readonly object getExpandCollapseInfo = new object();
		enum ExpandMode {
			None,
			FromNearToFar,
			FromFarToNear,
			FromCenterToEdge
		}
		enum AlignmentMode {
			Near,
			Center,
			Far,
			Stretch
		}
		#region inner classes
		class LayoutCalculator {
			public static LayoutCalculator CreateInstance(DXExpander expander, bool stretch) {
				return stretch ? new StretchLayoutCalculator(expander) : new LayoutCalculator(expander);
			}
			double lastAnimationProgress;
			protected DXExpander DXExpander { get; set; }
			internal LayoutCalculator(DXExpander expander) {
				DXExpander = expander;
			}
			protected virtual void MeasureChild(UIElement child, Size constraint) {
				child.Measure(constraint);
			}
			protected virtual Size GetExpandedSize() {
				return DXExpander.GetChild().DesiredSize;
			}
			protected virtual Size GetCurrentSize() {
				return DXExpander.GetChild().DesiredSize;
			}
			public Size MeasureOverride(Size constraint) {
				UIElement child = DXExpander.GetChild();
				if(child != null) {
					MeasureChild(child, constraint);
					Size size = GetExpandedSize();
					double width = CalculateSize(constraint, size, DXExpander.AnimationProgress, (ExpandMode)DXExpander.HorizontalExpand, (AlignmentMode)DXExpander.HorizontalAlignment, HorizontalSizeHelper.Instance);
					double height = CalculateSize(constraint, size, DXExpander.AnimationProgress, (ExpandMode)DXExpander.VerticalExpand, (AlignmentMode)DXExpander.VerticalAlignment, VerticalSizeHelper.Instance);
					if(double.IsInfinity(width) || double.IsNaN(width))
						width = 0.0f;
					if(double.IsInfinity(height) || double.IsNaN(height))
						height = 0.0f;
					return new Size(Math.Max(DXExpander.MinWidth, width), Math.Max(DXExpander.MinHeight, height));
				}
				return new Size();
			}
			public Size ArrangeOverride(Size arrangeSize) {
				FrameworkElement child = DXExpander.GetChild() as FrameworkElement;
				if(child != null) {
					Size size = GetCurrentSize();
					double childWidth = DXExpander.HorizontalAlignment == HorizontalAlignment.Stretch ? arrangeSize.Width : Math.Max(((FrameworkElement)child).MinWidth, size.Width);
					double childHeight = DXExpander.VerticalAlignment == VerticalAlignment.Stretch ? arrangeSize.Height : Math.Max(((FrameworkElement)child).MinHeight, size.Height);
					Size childSize = new Size(childWidth, childHeight);
					double x = CalculatePosition(childSize, arrangeSize, DXExpander.AnimationProgress, (ExpandMode)DXExpander.HorizontalExpand, SizeHelperBase.GetDefineSizeHelper(Orientation.Horizontal));
					double y = CalculatePosition(childSize, arrangeSize, DXExpander.AnimationProgress, (ExpandMode)DXExpander.VerticalExpand, SizeHelperBase.GetDefineSizeHelper(Orientation.Vertical));
					child.Arrange(new Rect(x, y, childWidth, childHeight));
					double width = CalculateSize(arrangeSize, DXExpander.HorizontalAlignment == HorizontalAlignment.Stretch ? arrangeSize : GetExpandedSize(), DXExpander.AnimationProgress, (ExpandMode)DXExpander.HorizontalExpand, (AlignmentMode)DXExpander.HorizontalAlignment, SizeHelperBase.GetDefineSizeHelper(Orientation.Horizontal));
					double height = CalculateSize(arrangeSize, DXExpander.VerticalAlignment == VerticalAlignment.Stretch ? arrangeSize : GetExpandedSize(), DXExpander.AnimationProgress, (ExpandMode)DXExpander.VerticalExpand, (AlignmentMode)DXExpander.VerticalAlignment, SizeHelperBase.GetDefineSizeHelper(Orientation.Vertical));
					UpdateIsRevealed(height);
					if(DXExpander.HorizontalAlignment == HorizontalAlignment.Stretch)
						width = arrangeSize.Width;
					if(DXExpander.VerticalAlignment == VerticalAlignment.Stretch)
						height = arrangeSize.Height;
					return new Size(Math.Max(DXExpander.MinWidth, width), Math.Max(DXExpander.MinHeight, height));
				}
				return new Size();
			}
			protected double GetSize(double minWidth, double width, double maxWidth, double percent) {
				if(!Double.IsNaN(maxWidth) && !Double.IsInfinity(maxWidth))
					width = maxWidth;
				double size = minWidth + (width - minWidth) * percent;
				return double.IsNaN(size) ? 0 : size;
			}
			void UpdateIsRevealed(double height) {
				if(!(bool)DXExpander.GetValue(AllowTracksRevealingProperty))
					return;
				if(DXExpander.AnimationProgress >= lastAnimationProgress) {
					VisualTreeEnumerator en = new VisualTreeEnumerator(DXExpander);
					while(en.MoveNext()) {
						if(GetTracksRevealing(en.Current)) {
							Rect rect = LayoutHelper.GetRelativeElementRect((FrameworkElement)en.Current, DXExpander);
							SetIsRevealed(en.Current, (rect.Top < height || DXExpander.AnimationProgress == 1));
						}
					}
				}
				lastAnimationProgress = DXExpander.AnimationProgress;
			}
			double CalculatePosition(Size size, Size arrangeSize, double percent, ExpandMode expander, SizeHelperBase sizeHelper) {
				double width = DXExpander.HorizontalAlignment == HorizontalAlignment.Stretch ? arrangeSize.Width : DXExpander.DesiredSize.Width;
				double height = DXExpander.VerticalAlignment == VerticalAlignment.Stretch ? arrangeSize.Height : DXExpander.DesiredSize.Height;
				Size exSize = new Size(width, height);
				double position = sizeHelper.GetDefineSize(exSize) - sizeHelper.GetDefineSize(size);
				if(expander == ExpandMode.FromFarToNear) {
					return position;
				} else if(expander == ExpandMode.FromCenterToEdge) {
					return position * 0.5;
				} else {
					return 0.0;
				}
			}
			protected virtual double CalculateSize(Size constraintSize, Size originalSize, double percent, ExpandMode expand, AlignmentMode alignment, SizeHelperBase sizeHelper) {
				double originalDefineSize = sizeHelper.GetDefineSize(originalSize);
				Size minSize = new Size(DXExpander.MinWidth, DXExpander.MinHeight);
				Size maxSize = new Size(DXExpander.MaxWidth, DXExpander.MaxHeight);
				if(expand == ExpandMode.None) {
					return alignment == AlignmentMode.Stretch && !double.IsInfinity(sizeHelper.GetDefineSize(constraintSize)) ? sizeHelper.GetDefineSize(constraintSize) : originalDefineSize;
				} else {
					double defineChildVisibleSize = sizeHelper.GetDefineSize(DXExpander.ChildVisibleSize);
					if(!double.IsNaN(defineChildVisibleSize))
						originalDefineSize = defineChildVisibleSize;
					return GetSize(sizeHelper.GetDefineSize(minSize), originalDefineSize, sizeHelper.GetDefineSize(maxSize), percent);
				}
			}
		}
		class StretchLayoutCalculator : LayoutCalculator {
			Size expanderSize;
			internal StretchLayoutCalculator(DXExpander expander)
				: base(expander) {
			}
			protected override Size GetExpandedSize() {
				return expanderSize;
			}
			protected override Size GetCurrentSize() {
				return DXExpander.DesiredSize;
			}
			protected override void MeasureChild(UIElement child, Size constraint) {
				Size maxSize = new Size(DXExpander.MaxWidth, DXExpander.MaxHeight);
				Size measureSize = new Size(GetFinalSize(constraint, maxSize, HorizontalSizeHelper.Instance), GetFinalSize(constraint, maxSize, VerticalSizeHelper.Instance));
				double measureWidth = CalculateSize(constraint, DXExpander.HorizontalExpand == HorizontalExpandMode.None ? constraint : measureSize, DXExpander.AnimationProgress, (ExpandMode)DXExpander.HorizontalExpand, (AlignmentMode)DXExpander.HorizontalAlignment, SizeHelperBase.GetDefineSizeHelper(Orientation.Horizontal));
				double measureHeight = CalculateSize(constraint, DXExpander.VerticalExpand == VerticalExpandMode.None ? constraint : measureSize, DXExpander.AnimationProgress, (ExpandMode)DXExpander.VerticalExpand, (AlignmentMode)DXExpander.VerticalAlignment, SizeHelperBase.GetDefineSizeHelper(Orientation.Vertical));
				child.Measure(new Size(measureWidth, measureHeight));
				expanderSize = new Size(GetFinalSize(constraint, child.DesiredSize, HorizontalSizeHelper.Instance), GetFinalSize(constraint, child.DesiredSize, VerticalSizeHelper.Instance));
			}
			protected override double CalculateSize(Size constraintSize, Size originalSize, double percent, ExpandMode expander, AlignmentMode alignment, SizeHelperBase sizeHelper) {
				Size minWidth = new Size(DXExpander.MinWidth, DXExpander.MinHeight);
				Size maxWidth = new Size(DXExpander.MaxWidth, DXExpander.MaxHeight);
				ExpandCollapseInfoEventArgs info = DXExpander.RaiseGetExpandCollapseInfo(originalSize);
				if(info != null)
					originalSize = info.Size;
				if(expander != ExpandMode.None)
					return GetSize(sizeHelper.GetDefineSize(minWidth), sizeHelper.GetDefineSize(originalSize), sizeHelper.GetDefineSize(maxWidth), percent);
				return alignment == AlignmentMode.Stretch ? sizeHelper.GetDefineSize(constraintSize) : sizeHelper.GetDefineSize(originalSize);
			}
			double GetFinalSize(Size size1, Size size2, SizeHelperBase sizeHelper) {
				return double.IsInfinity(sizeHelper.GetDefineSize(size1)) ? sizeHelper.GetDefineSize(size2) : sizeHelper.GetDefineSize(size1);
			}
		}
		#endregion
		public static readonly DependencyProperty AllowAddingEventProperty =
			DependencyPropertyManager.Register("AllowAddingEvent", typeof(bool), typeof(DXExpander), new FrameworkPropertyMetadata(false));
		public static readonly DependencyProperty DurationProperty =
			DependencyPropertyManager.Register("Duration", typeof(double), typeof(DXExpander), new FrameworkPropertyMetadata(250.0));
		public static readonly DependencyProperty IsExpandedProperty =
			DependencyPropertyManager.Register("IsExpanded", typeof(bool), typeof(DXExpander), new FrameworkPropertyMetadata(false, new PropertyChangedCallback(OnIsExpandedChanged)));
		public static readonly DependencyProperty StretchChildProperty =
			DependencyPropertyManager.Register("StretchChild", typeof(bool), typeof(DXExpander), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure, OnStretchChildChanged));
		public static readonly DependencyProperty HorizontalExpandProperty =
			DependencyPropertyManager.Register("HorizontalExpand", typeof(HorizontalExpandMode), typeof(DXExpander), new FrameworkPropertyMetadata(HorizontalExpandMode.None, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty VerticalExpandProperty =
			DependencyPropertyManager.Register("VerticalExpand", typeof(VerticalExpandMode), typeof(DXExpander), new FrameworkPropertyMetadata(VerticalExpandMode.FromTopToBottom, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty AnimationProgressProperty =
			DependencyPropertyManager.Register("AnimationProgress", typeof(double), typeof(DXExpander), new FrameworkPropertyMetadata(0.0, FrameworkPropertyMetadataOptions.AffectsMeasure, OnAnimationProgressChanged, OnCoerceAnimationProgress));
		public static readonly DependencyProperty ExpandStoryboardProperty =
			DependencyPropertyManager.Register("ExpandStoryboard", typeof(Storyboard), typeof(DXExpander), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty CollapseStoryboardProperty =
			DependencyPropertyManager.Register("CollapseStoryboard", typeof(Storyboard), typeof(DXExpander), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsMeasure));
		internal static readonly DependencyPropertyKey IsRevealedPropertyKey;
		public static readonly DependencyProperty IsRevealedProperty;
		public static readonly DependencyProperty ExpandingProperty;
		public static readonly DependencyProperty CollapsingProperty;
		public static readonly DependencyProperty TracksRevealingProperty;
		public static readonly DependencyProperty AllowTracksRevealingProperty;
		static void OnIsExpandedChanged(object sender, DependencyPropertyChangedEventArgs e) {
			((DXExpander)sender).SetupAnimation((bool)e.NewValue);
		}
		static void OnAnimationProgressChanged(object sender, DependencyPropertyChangedEventArgs e) {
			((DXExpander)sender).UpdateAnimationProperties((double)e.OldValue);
#if DEBUGTEST && !SILVERLIGHT
			if(((DXExpander)sender).AllowAddingEvent) EventLog.Default.AddEvent(new DependencyPropertyValueSnapshot<DXExpander, double>(AnimationProgressProperty, (DXExpander)sender, (double)e.NewValue));
#endif
		}
		static void OnStretchChildChanged(object sender, DependencyPropertyChangedEventArgs e) {
			((DXExpander)sender).UpdateLayoutCalculator();
		}
		static DXExpander() {
#if !SILVERLIGHT
			ClipToBoundsProperty.OverrideMetadata(typeof(DXExpander), new FrameworkPropertyMetadata(true));
#endif
			IsRevealedPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("IsRevealed", typeof(bool), typeof(DXExpander), new FrameworkPropertyMetadata(true));
			IsRevealedProperty = IsRevealedPropertyKey.DependencyProperty;
			TracksRevealingProperty = DependencyPropertyManager.RegisterAttached("TracksRevealing", typeof(bool), typeof(DXExpander), new FrameworkPropertyMetadata(false));
			ExpandingProperty = DependencyPropertyManager.Register("Expanding", typeof(bool), typeof(DXExpander), new FrameworkPropertyMetadata(false));
			CollapsingProperty = DependencyPropertyManager.Register("Collapsing", typeof(bool), typeof(DXExpander), new FrameworkPropertyMetadata(false));
			AllowTracksRevealingProperty = DependencyPropertyManager.RegisterAttached("AllowTracksRevealing", typeof(bool), typeof(DXExpander), new FrameworkPropertyMetadata(true));
			RegisterDataObjectBaseResetEventHandler();
		}
		#region DataObjectBase.ResetEvent processing
		static void RegisterDataObjectBaseResetEventHandler() {
#if !SL && !DEMO
			EventManager.RegisterClassHandler(typeof(DXExpander), DataObjectBase.ResetEvent, (RoutedEventHandler)OnDataObjectBaseReset);
#endif
		}
#if !SL
		static void OnDataObjectBaseReset(object sender, RoutedEventArgs e) {
			DXExpander expander = sender as DXExpander;
			if(expander != null)
				expander.SkipToFillCurrentAnimation();
		}
#endif
		Storyboard currentAnimation;
#if !SL
		void SkipToFillCurrentAnimation() {
			if(this.currentAnimation != null) {
				this.currentAnimation.SkipToFill(this);
				this.currentAnimation = null;
			}
		}
#endif
		void BeginAnimation(Storyboard sb) {
			this.currentAnimation = sb;
			this.currentAnimation.Begin(this, true);
		}
		#endregion
		internal UIElement GetChild() {
#if SILVERLIGHT
			if(Children.Count > 0)
				return Children[0];
			return null;
#else
			return Child;
#endif
		}
		public static void SetAllowTracksRevealing(DependencyObject element, bool value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(AllowTracksRevealingProperty, value);
		}
		public static bool GetAllowTracksRevealing(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(AllowTracksRevealingProperty);
		}
		public static void SetTracksRevealing(DependencyObject element, bool value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(TracksRevealingProperty, value);
		}
		public static bool GetTracksRevealing(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(TracksRevealingProperty);
		}
		internal static void SetIsRevealed(DependencyObject element, bool value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(IsRevealedPropertyKey, value);
		}
		public static bool GetIsRevealed(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(IsRevealedProperty);
		}
		public DXExpander() {
			UpdateLayoutCalculator();
			Loaded += new RoutedEventHandler(Reveal_Loaded);
		}
		void Reveal_Loaded(object sender, RoutedEventArgs e) {
			loaded = true;
		}
		EventHandlerList events;
		protected EventHandlerList Events {
			get {
				if(events == null)
					events = new EventHandlerList();
				return events;
			}
		}
		protected internal ExpandCollapseInfoEventArgs RaiseGetExpandCollapseInfo(Size size) {
			ExpandCollapseInfoEventArgs e = new ExpandCollapseInfoEventArgs() { Expander = this, Size = size };
			ExpandCollapseInfoEventHandler handler = Events[getExpandCollapseInfo] as ExpandCollapseInfoEventHandler;
			if(handler == null)
				return null;
			handler(this, e);
			return e;
		}
		#region Public Properties
		public event ExpandCollapseInfoEventHandler GetExpandCollapseInfo {
			add { Events.AddHandler(getExpandCollapseInfo, value); }
			remove { Events.RemoveHandler(getExpandCollapseInfo, value); }
		}
#if !DEMO
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXExpanderExpandStoryboard")]
#endif
#endif
		public Storyboard ExpandStoryboard {
			get { return (Storyboard)GetValue(ExpandStoryboardProperty); }
			set { SetValue(ExpandStoryboardProperty, value); }
		}
#if !DEMO
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXExpanderCollapseStoryboard")]
#endif
#endif
		public Storyboard CollapseStoryboard {
			get { return (Storyboard)GetValue(CollapseStoryboardProperty); }
			set { SetValue(CollapseStoryboardProperty, value); }
		}
#if !DEMO
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXExpanderIsExpanded")]
#endif
#endif
		public bool IsExpanded {
			get { return (bool)GetValue(IsExpandedProperty); }
			set { SetValue(IsExpandedProperty, value); }
		}
#if !DEMO
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXExpanderExpanding")]
#endif
#endif
		public bool Expanding {
			get { return (bool)GetValue(ExpandingProperty); }
			set { SetValue(ExpandingProperty, value); }
		}
#if !DEMO
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXExpanderCollapsing")]
#endif
#endif
		public bool Collapsing {
			get { return (bool)GetValue(CollapsingProperty); }
			set { SetValue(CollapsingProperty, value); }
		}
#if !DEMO
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXExpanderStretchChild")]
#endif
#endif
		public bool StretchChild {
			get { return (bool)GetValue(StretchChildProperty); }
			set { SetValue(StretchChildProperty, value); }
		}
#if !DEMO
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXExpanderDuration")]
#endif
#endif
		public double Duration {
			get { return (double)GetValue(DurationProperty); }
			set { SetValue(DurationProperty, value); }
		}
#if !DEMO
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXExpanderHorizontalExpand")]
#endif
#endif
		public HorizontalExpandMode HorizontalExpand {
			get { return (HorizontalExpandMode)GetValue(HorizontalExpandProperty); }
			set { SetValue(HorizontalExpandProperty, value); }
		}
#if !DEMO
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXExpanderVerticalExpand")]
#endif
#endif
		public VerticalExpandMode VerticalExpand {
			get { return (VerticalExpandMode)GetValue(VerticalExpandProperty); }
			set { SetValue(VerticalExpandProperty, value); }
		}
#if !DEMO
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXExpanderAnimationProgress")]
#endif
#endif
		public double AnimationProgress {
			get { return (double)GetValue(AnimationProgressProperty); }
			set { SetValue(AnimationProgressProperty, value); }
		}
#if !DEMO
#if !SL
	[DevExpressXpfCoreLocalizedDescription("DXExpanderAllowAddingEvent")]
#endif
#endif
		public bool AllowAddingEvent {
			get { return (bool)GetValue(AllowAddingEventProperty); }
			set { SetValue(AllowAddingEventProperty, value); }
		}
		private static object OnCoerceAnimationProgress(DependencyObject d, object baseValue) {
			double num = (double)baseValue;
			if(num < 0.0) {
				return 0.0;
			} else if(num > 1.0) {
				return 1.0;
			}
			return baseValue;
		}
		#endregion
		#region Implementation
		bool loaded;
		LayoutCalculator layoutCalculator;
		internal Size ChildVisibleSize { get { return ExpandHelper.GetVisibleSize(GetChild()); } }
		protected override Size MeasureOverride(Size constraint) {
			Size sz = layoutCalculator.MeasureOverride(constraint);
			return sz;
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			Size sz = layoutCalculator.ArrangeOverride(arrangeSize);
			return sz;
		}
		void SetupAnimation(bool isExpanded) {
			if(!loaded) {
				AnimationProgress = isExpanded ? 1 : 0;
				return;
			}
			double currentProgress = AnimationProgress;
			if(isExpanded) {
				currentProgress = 1.0 - currentProgress;
			}
			Storyboard current = isExpanded ? ExpandStoryboard : CollapseStoryboard;
			if(current != null) {
				BeginAnimation(current);
				return;
			}
			DoubleAnimation animation = new DoubleAnimation();
			animation.To = isExpanded ? 1.0 : 0.0;
			animation.Duration = TimeSpan.FromMilliseconds(Duration * currentProgress);
			animation.FillBehavior = FillBehavior.HoldEnd;
			Storyboard progressStory = new Storyboard();
			Storyboard.SetTarget(animation, this);
			Storyboard.SetTargetProperty(animation, new PropertyPath(AnimationProgressProperty));
			progressStory.Children.Add(animation);
			BeginAnimation(progressStory);
		}
		void UpdateAnimationProperties(double oldValue) {
			if(GetChild() != null) {
				Collapsing = 0 < AnimationProgress && AnimationProgress < oldValue;
				Expanding = oldValue < AnimationProgress && AnimationProgress < 1;
			}
		}
		void UpdateLayoutCalculator() {
			layoutCalculator = LayoutCalculator.CreateInstance(this, StretchChild);
		}
		#endregion
	}
	public enum VerticalExpandMode {
		None,
		FromTopToBottom,
		FromBottomToTop,
		FromCenterToEdge,
	}
	public enum HorizontalExpandMode {
		None,
		FromLeftToRight,
		FromRightToLeft,
		FromCenterToEdge,
	}
}
