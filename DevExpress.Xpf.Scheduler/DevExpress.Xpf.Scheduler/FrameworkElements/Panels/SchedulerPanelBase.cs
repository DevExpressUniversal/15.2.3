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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;
using DevExpress.XtraScheduler;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.Xpf.Core;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
using DevExpress.Xpf.Scheduler.Internal;
using System.Diagnostics;
using DevExpress.Xpf.Scheduler.Drawing;
#if SILVERLIGHT
 using DevExpress.Xpf.Editors.Controls;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#else
using System.Collections.Specialized;
using System.Windows.Input;
using DevExpress.XtraScheduler.Drawing;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Utils;
using System.Windows.Documents;
using System.Collections;
using DevExpress.XtraScheduler.Native;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public interface ISchedulerObservablePanel {
		event EventHandler Measuring;
		event EventHandler Measured;
		event EventHandler Arranging;
		event EventHandler Arranged;
		event EventHandler VisualChildrenChanged;
	}
	public abstract class SchedulerPanelCoreBase : Panel {
		bool isReady;
		protected SchedulerPanelCoreBase() {
			this.Unloaded += new RoutedEventHandler(OnUnloaded);
		}
		public bool IsReady { get { return isReady; } }
		void OnUnloaded(object sender, RoutedEventArgs e) {
			this.isReady = false;
			InvalidateMeasure();
		}
		protected sealed override Size MeasureOverride(Size availableSize) {
			this.isReady = true;
			return DoMeasureOverride(availableSize);
		}
		protected virtual Size DoMeasureOverride(Size availableSize) {
			return base.MeasureOverride(availableSize);
		}
	}
	public abstract class SchedulerPanelBase : SchedulerPanelCoreBase, ISupportElementPosition, ISchedulerObservablePanel {
#if SL
		int lastChildrenCount = 0;
#endif
		bool isAttachedPositionsValid;
		#region AttachedPropertiesCalculatorProperty
		public static readonly DependencyProperty AttachedPropertiesCalculatorProperty = DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<SchedulerPanelBase, ISchedulerItemsControlAttachedPropertiesCalculator>("AttachedPropertiesCalculator", null, (d, e) => d.OnAttachedPropertiesCalculatorChanged(e.OldValue, e.NewValue));
		public ISchedulerItemsControlAttachedPropertiesCalculator AttachedPropertiesCalculator { get { return (ISchedulerItemsControlAttachedPropertiesCalculator)GetValue(AttachedPropertiesCalculatorProperty); } set { SetValue(AttachedPropertiesCalculatorProperty, value); } }
		#endregion
		#region ApplyInnerContentPadding
		public bool ApplyInnerContentPadding {
			get { return (bool)GetValue(ApplyInnerContentPaddingProperty); }
			set { SetValue(ApplyInnerContentPaddingProperty, value); }
		}
		public static readonly DependencyProperty ApplyInnerContentPaddingProperty =
			DependencyPropertyManager.Register("ApplyInnerContentPadding", typeof(bool), typeof(SchedulerPanelBase), new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.AffectsMeasure));
		#endregion
		#region OnMeasuring
		public event EventHandler Measuring;
		public virtual void RaiseMeasuring() {
			if (Measuring != null)
				Measuring(this, EventArgs.Empty);
		}
		#endregion
		#region OnMeasured
		public event EventHandler Measured;
		public virtual void RaiseMeasured() {
			if (Measured != null)
				Measured(this, EventArgs.Empty);
		}
		#endregion
		#region OnArranging
		public event EventHandler Arranging;
		public virtual void RaiseArranging() {
			if (Arranging != null)
				Arranging(this, EventArgs.Empty);
		}
		#endregion
		#region OnArranged
		public event EventHandler Arranged;
		public virtual void RaiseArranged() {
			if (Arranged != null)
				Arranged(this, EventArgs.Empty);
		}
		#endregion
		#region VisualChildrenChanged
		public event EventHandler VisualChildrenChanged;
		public virtual void RaiseVisualChildrenChanged() {
			if (VisualChildrenChanged != null)
				VisualChildrenChanged(this, EventArgs.Empty);
		}
		#endregion
		protected SchedulerPanelBase() {
#if SL
			LayoutUpdated += OnLayoutUpdated;
#endif
		}
		protected virtual Thickness GetActualContentPadding() {
			Thickness result = new Thickness();
			if (!ApplyInnerContentPadding)
				return result;
			ElementPosition elementPosition = SchedulerItemsControl.GetElementPosition(this);
			if (elementPosition == null)
				return result;
			return elementPosition.InnerContentPadding;
		}
#if SL
		void OnLayoutUpdated(object sender, EventArgs e) {
			CheckChildren();
		}
		void CheckChildren() {
			int childrenCount = Children.Count;
			if (childrenCount != lastChildrenCount) {
				lastChildrenCount = childrenCount;
				OnVisualChildrenChangedCore();
			}
		}
#else
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			OnVisualChildrenChangedCore();
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
		}
#endif
		protected virtual void OnVisualChildrenChangedCore() {
			isAttachedPositionsValid = false;
			RaiseVisualChildrenChanged();
		}
		protected virtual void InvalidateAttachedProperties() {
			isAttachedPositionsValid = false;
			InvalidateMeasure();
		}
		protected virtual void OnAttachedPropertiesCalculatorChanged(ISchedulerItemsControlAttachedPropertiesCalculator iSchedulerItemsControlAttachedPropertiesCalculator, ISchedulerItemsControlAttachedPropertiesCalculator iSchedulerItemsControlAttachedPropertiesCalculator_2) {
			InvalidateAttachedProperties();
		}
		protected virtual Size MeasureOverrideCore(Size availableSize) {
			return base.DoMeasureOverride(availableSize);
		}
		protected sealed override Size DoMeasureOverride(Size availableSize) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.Panel, "->SchedulerPanelBase.DoMeasureOverride: {0}-({1})", Name, GetType().Name);
#if SL
			CheckChildren();
#endif
			RaiseMeasuring();
			if (!isAttachedPositionsValid) {
				RecalculatePositions();
			}
			isAttachedPositionsValid = true;
			Size result = MeasureOverrideCore(availableSize);
			RaiseMeasured();
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.Panel);
			return result;
		}
		protected virtual void RecalculatePositions() {
			ISchedulerItemsControlAttachedPropertiesCalculator calculator = AttachedPropertiesCalculator;
			if (calculator == null)
				return;
			int count = Children.Count;
			for (int i = 0; i < count; i++) {
				calculator.SetAttachedProperties(Children[i], i, count);
			}
		}
		protected sealed override Size ArrangeOverride(Size finalSize) {
			RaiseArranging();
			Size result = ArrangeOverrideCore(finalSize);
			RaiseArranged();
			return result;
		}
		protected virtual Size ArrangeOverrideCore(Size finalSize) {
			return base.ArrangeOverride(finalSize);
		}
		#region ISupportElementPosition Members
		void ISupportElementPosition.OnRecalculatePostions(ElementPosition oldValue, ElementPosition newValue) {
			InvalidateAttachedProperties();
		}
		#endregion
	}
	public abstract class SchedulerPanel : SchedulerPanelBase, ISchedulerObservablePanel {
		protected class LayoutData {
			Size availableSize = Size.Empty;
			Size finalSize = Size.Empty;
			public Size AvailableSize { get { return availableSize; } set { availableSize = value; } }
			public bool HasAvailableSize { get { return AvailableSize != Size.Empty; } }
			public Size FinalSize { get { return finalSize; } set { finalSize = value; } }
			public bool HasFinalSize { get { return finalSize != Size.Empty; } }
		}
		readonly LayoutData layoutData = new LayoutData();
		protected LayoutData LayoutDataInfo { get { return layoutData; } }
		#region OnMeasure
		EventHandler onMeasure;
		public event EventHandler OnMeasure { add { onMeasure += value; } remove { onMeasure -= value; } }
		protected virtual void RaiseOnMeasure() {
			if (onMeasure != null)
				onMeasure(this, EventArgs.Empty);
		}
		#endregion
		#region OnArrange
		EventHandler onArrange;
		public event EventHandler OnArrange { add { onArrange += value; } remove { onArrange -= value; } }
		protected virtual void RaiseOnArrange() {
			if (onArrange != null)
				onArrange(this, EventArgs.Empty);
		}
		#endregion
		protected override Size MeasureOverrideCore(Size availableSize) {
			RaiseOnMeasure();
			this.layoutData.AvailableSize = availableSize;
			return availableSize;
		}
		protected override Size ArrangeOverrideCore(Size finalSize) {
			RaiseOnArrange();
			if (this.layoutData.FinalSize != finalSize) {
				SizeChangedOnArrange();
			}
			this.layoutData.FinalSize = finalSize;
			return finalSize;
		}
		protected virtual void SizeChangedOnArrange() {
		}
		public virtual void RecalculateLayout() {
			if (this.layoutData.HasAvailableSize)
				Measure(this.layoutData.AvailableSize);
			if (this.layoutData.HasFinalSize) {
				Point location = new Point(0, 0);
				UIElement parent = VisualTreeHelper.GetParent(this) as UIElement;
				if (parent != null)
					location = this.TranslatePoint(location, parent);
				Arrange(new Rect(location, this.layoutData.FinalSize));
			}
		}
	}
	public enum BorderStatus { NotDefined = 0, Yes = 1, No = 2 };
	public static class ElementPositionPropertyHelper {
		public static ElementPositionCore CalculateCore(ElementPositionCore panelPosition, bool isFirst, bool isLast) {
			ElementPositionCore result = new ElementPositionCore();
			if (panelPosition.IsStart && isFirst) {
				result.ElementPosition |= ElementRelativePosition.Start;
				result.OuterSeparator |= (panelPosition.OuterSeparator & OuterSeparator.Start) | (panelPosition.OuterSeparator & OuterSeparator.NoStart) | (panelPosition.OuterSeparator & OuterSeparator.NoStartCorner);
			}
			if (panelPosition.IsEnd && isLast) {
				result.ElementPosition |= ElementRelativePosition.End;
				result.OuterSeparator |= (panelPosition.OuterSeparator & OuterSeparator.End) | (panelPosition.OuterSeparator & OuterSeparator.NoEnd) | (panelPosition.OuterSeparator & OuterSeparator.NoEndCorner);
			}
			if (result.ElementPosition == ElementRelativePosition.NotDefined)
				result.ElementPosition = ElementRelativePosition.Middle;
			result.InnerSeparator = panelPosition.InnerSeparator;
			return result;
		}
		public static void RecalculatePositionsForPanel(Panel owner, Orientation orientation) {
			ElementPosition panelPosition = SchedulerItemsControl.GetElementPosition(owner) ?? ElementPosition.Standalone;
			UIElementCollection children = owner.Children;
			int count = owner.Children.Count;
			for (int i = 0; i < count; i++) {
				bool isHorizontallyFirst = orientation == Orientation.Horizontal ? i == 0 : true;
				bool isHorizontallyLast = orientation == Orientation.Horizontal ? i == count - 1 : true;
				bool isVerticallyFirst = orientation == Orientation.Vertical ? i == 0 : true;
				bool isVerticallyLast = orientation == Orientation.Vertical ? i == count - 1 : true;
				ElementPositionCore horizontalElementPosition = CalculateCore(panelPosition.HorizontalElementPosition, isHorizontallyFirst, isHorizontallyLast);
				ElementPositionCore verticalElementPosition = CalculateCore(panelPosition.VerticalElementPosition, isVerticallyFirst, isVerticallyLast);
				ElementPosition position = new ElementPosition(horizontalElementPosition, verticalElementPosition);
				SchedulerItemsControl.SetElementPosition(children[i], position);
			}
		}
	}
	public class PixelSnappedUniformGrid : SchedulerPanel {
		const int MaxWrongArrangeCount = 180;
		LoadedUnloadedSubscriber loadedUnloadedSubscriber;
		int wrongArrangeCount = 0;
		Size oldFinalSize = new Size();
		public PixelSnappedUniformGrid() {
			this.loadedUnloadedSubscriber = new LoadedUnloadedSubscriber(this, SubscribeEvents, UnsubscribeEvents);
		}
		#region Orientation
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public static readonly DependencyProperty OrientationProperty =
			DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(PixelSnappedUniformGrid), new PropertyMetadata(Orientation.Horizontal));
		#endregion
		#region Events
		#region ElementPositionRecalculated
		EventHandler elementPositionRecalculated;
		public event EventHandler ElementPositionRecalculated {
			add { elementPositionRecalculated += value; }
			remove { elementPositionRecalculated -= value; }
		}
		#endregion
		#endregion
		void SubscribeEvents(FrameworkElement fe) {
			NavigationButtonPairControl nbPairControl = FindNavigationButtonPairControl();
			if (nbPairControl != null)
				nbPairControl.SubscribeEvent(this);
		}
		void UnsubscribeEvents(FrameworkElement fe) {
			NavigationButtonPairControl nbPairControl = FindNavigationButtonPairControl();
			if (nbPairControl != null)
				nbPairControl.UnsubscribeEvent(this);
		}
		NavigationButtonPairControl FindNavigationButtonPairControl() {
			DependencyObject parent = this;
			while (parent != null) {
				parent = VisualTreeHelper.GetParent(parent);
				NavigationButtonPairControl navigationButtonPairControl = parent as NavigationButtonPairControl;
				if (navigationButtonPairControl != null)
					return navigationButtonPairControl;
			}
			return null;
		}
		void RaiseElementPositionRecalculatedEvent(EventArgs args) {
			if (elementPositionRecalculated != null)
				elementPositionRecalculated(this, args);
		}
		protected override void RecalculatePositions() {
			if (AttachedPropertiesCalculator != null) {
				base.RecalculatePositions();
				return;
			}
			ElementPositionPropertyHelper.RecalculatePositionsForPanel(this, Orientation);
			RaiseElementPositionRecalculatedEvent(new EventArgs());
		}
		protected override Size MeasureOverrideCore(Size availableSize) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.PixelSnappedUniformGrid, "->PixelSnappedUniformGrid.MeasureOverrideCore: {0}, {1}, {2}", Name, GetHashCode(), availableSize);
			base.MeasureOverrideCore(availableSize);
			int count = Children.Count;
			Rect[] childrenRects;
			childrenRects = PixelSnappedUniformGridLayoutHelper.SplitSizeDpiAware(availableSize, count, GetActualContentPadding(), Orientation);
			double maxWidth = 0;
			double maxHeight = 0;
			for (int i = 0; i < count; i++) {
				Size calculatedChildSize = childrenRects[i].Size();
				Children[i].Measure(calculatedChildSize);
				maxWidth = Math.Max(Children[i].DesiredSize.Width, maxWidth);
				maxHeight = Math.Max(Children[i].DesiredSize.Height, maxHeight);
			}
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.PixelSnappedUniformGrid);
			this.wrongArrangeCount = 0;
			return new Size(double.IsInfinity(availableSize.Width) ? maxWidth : availableSize.Width, double.IsInfinity(availableSize.Height) ? maxHeight : availableSize.Height);
		}
		protected override Size ArrangeOverrideCore(Size finalSize) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.PixelSnappedUniformGrid, "->PixelSnappedUniformGrid.ArrangeOverrideCore [begin]: {0}, {1}, {2}, {3}, wrongArrangeCount=", Name, GetHashCode(), finalSize, Orientation, this.wrongArrangeCount);
			bool invalidated = false;
			if (this.oldFinalSize != finalSize) {
				this.oldFinalSize = finalSize;
				this.wrongArrangeCount = 0;
			}
			base.ArrangeOverrideCore(finalSize);
			int count = Children.Count;
			Rect[] childrenBounds = PixelSnappedUniformGridLayoutHelper.SplitSizeDpiAware(finalSize, count, GetActualContentPadding(), Orientation);
			if (wrongArrangeCount > (MaxWrongArrangeCount - 1)) {
				SchedulerLogger.Trace(XpfLoggerTraceLevel.PixelSnappedUniformGrid, "!!! exceeding the allowed number of measurements !!!");
				SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.PixelSnappedUniformGrid);
				return finalSize;
			}
			bool isArrangeErrorDetected = false;
			ErrorBetweenExpectedAndActualBoundsCalculator errorCalculator = new ErrorBetweenExpectedAndActualBoundsCalculator(Orientation);
			for (int i = 0; i < count; i++) {
				UIElement child = Children[i];
				FrameworkElement fe = child as FrameworkElement;
				SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.PixelSnappedUniformGrid, "| process child[{0}]: {1}-({2}), Hash={3}", i, fe.Name, child.GetType().Name, child.GetHashCode());
				Rect calculatedChildrenBounds = childrenBounds[i];
				Point childLocation = errorCalculator.CorrectPoint(calculatedChildrenBounds.Location());
				Size childSize = calculatedChildrenBounds.Size();
				Size actualChildSize = (i == count - 1) ? errorCalculator.CorrectLastItemSize(childSize) : childSize;
				calculatedChildrenBounds = new Rect(childLocation, actualChildSize);
				child.Arrange(calculatedChildrenBounds);
				if (fe != null)
					childSize = ApplyMargin(childSize, fe.Margin);
				errorCalculator.CalculateError(child.RenderSize, childSize);
				Rect realElementBounds = LayoutHelper.GetRelativeElementRect(child, this);
				SchedulerLogger.Trace(XpfLoggerTraceLevel.PixelSnappedUniformGrid, "calcBounds={0}, realBounds={1}, DesiredSize={2}, TotalError={3} Hash={4}, rearrangeCount={5}", calculatedChildrenBounds, realElementBounds, child.DesiredSize, errorCalculator.TotalError, GetHashCode(), wrongArrangeCount);
				SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.PixelSnappedUniformGrid);
				if (realElementBounds != calculatedChildrenBounds) {
					if (!invalidated) {
						isArrangeErrorDetected = true;
						SchedulerLogger.Trace(XpfLoggerTraceLevel.PixelSnappedUniformGrid, "Arrange error detected - InvalidateArrange()", i);
					}
				}
			}
			if (isArrangeErrorDetected && wrongArrangeCount < MaxWrongArrangeCount) {
				InvalidateArrange();
				invalidated = true;
				wrongArrangeCount++;
			}
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.PixelSnappedUniformGrid, "<-PixelSnappedUniformGrid.ArrangeOverrideCore [end]: {0}, {1}, {2}", Name, GetHashCode(), finalSize);
			return finalSize;
		}
		Size ApplyMargin(Size size, Thickness margin) {
			double width = margin.Left + margin.Right;
			double height = margin.Top + margin.Bottom;
			return new Size(Math.Max(0, size.Width - width), Math.Max(0, size.Height - height));
		}
	}
	public class PixelSnappedUniformGridLayoutHelper {
		public static Rect[] SplitDpiAware(Rect availableRect, int cellCount, Thickness contentPadding, Orientation orientation) {
#if !SL
			if (cellCount <= 0)
				return new Rect[] { new Rect(new Point(), availableRect.Size()) };
			availableRect = DpiIndependentUtils.ToPixel(availableRect);
			contentPadding = DpiIndependentUtils.ToPixel(contentPadding);
			Rect[] resultRects = Split(availableRect, cellCount, contentPadding, orientation);
			int count = resultRects.Length;
			for(int i = 0; i < count; i++) 
				resultRects[i] = DpiIndependentUtils.ToLogical(resultRects[i]);
			return resultRects;
#else
			return Split(availableRect, cellCount, contentPadding, orientation);
#endif
		}
		public static Rect[] Split(Rect availableRect, int cellCount, Thickness contentPadding, Orientation orientation) {
			if (cellCount <= 0)
				return new Rect[] { new Rect(new Point(), availableRect.Size()) };
			SizeHelperBase sizeHelper = SizeHelperBase.GetDefineSizeHelper(orientation);
			Rect[] cells = new Rect[cellCount];
			double availablePrimarySize = sizeHelper.GetDefineSize(availableRect.Size());
			double availableSecondarySize = sizeHelper.GetSecondarySize(availableRect.Size());
			if (double.IsInfinity(availablePrimarySize)) {
				Point zeroPoint = sizeHelper.CreatePoint(0, 0);
				Size size = sizeHelper.CreateSize(availablePrimarySize, availableSecondarySize);
				Rect rect = new Rect(zeroPoint, size);
				for (int i = 0; i < cellCount; i++)
					cells[i] = rect;
				return cells;
			}
			int ceilAvailablePrimarySize = (int)sizeHelper.GetDefineSize(availableRect.Size());
			double lastCellRemainder = availablePrimarySize - ceilAvailablePrimarySize;
			int primarySizePerCell = ceilAvailablePrimarySize / cellCount;
			int remainder = ceilAvailablePrimarySize - primarySizePerCell * cellCount;
			double primaryInflatePadding = (orientation == Orientation.Horizontal) ? contentPadding.Left : contentPadding.Top;
			double primaryDeflatePadding = (orientation == Orientation.Horizontal) ? contentPadding.Right : contentPadding.Bottom;
			double secondaryInflatePadding = (orientation == Orientation.Horizontal) ? contentPadding.Top : contentPadding.Left;
			double secondaryDeflatePadding = (orientation == Orientation.Horizontal) ? contentPadding.Bottom : contentPadding.Right;
			double secondarySizeExceptPaddings = availableSecondarySize - (secondaryInflatePadding + secondaryDeflatePadding);
			double offset = sizeHelper.GetDefinePoint(availableRect.Location());
			double secondaryOffset = sizeHelper.GetSecondaryPoint(availableRect.Location());
			for (int i = 0; i < cellCount; i++, remainder--) {
				double primarySize = primarySizePerCell + ((remainder > 0) ? 1 : 0);
				double finalPrimarySizeExceptPaddings = primarySize;
				double finalOffset = offset;
				if (i == 0) {
					finalPrimarySizeExceptPaddings -= primaryInflatePadding;
					finalOffset += primaryInflatePadding;
				}
				else if (i == cellCount - 1) {
					finalPrimarySizeExceptPaddings -= primaryDeflatePadding;
					finalPrimarySizeExceptPaddings += lastCellRemainder;
				}
				Point location = sizeHelper.CreatePoint(finalOffset, secondaryOffset + secondaryInflatePadding);
				Size size = sizeHelper.CreateSize(Math.Max(0, finalPrimarySizeExceptPaddings), Math.Max(0, secondarySizeExceptPaddings));
				cells[i] = new Rect(location, size);
				offset += primarySize;
			}
			return cells;
		}
		public static Rect[] SplitSizeDpiAware(Size availableSize, int cellCount, Thickness contentPadding, Orientation orientation) {
			return SplitDpiAware(new Rect(XpfSchedulerUtils.ZeroPoint, availableSize), cellCount, contentPadding, orientation);
		}
	}
#if !SL
	public class AnimationPixelSnappedUniformGrid : AnimationPanel2 {
		#region Orientation
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public static readonly DependencyProperty OrientationProperty =
			DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(AnimationPixelSnappedUniformGrid), new PropertyMetadata(Orientation.Horizontal));
		#endregion
		public static Rect[] SplitHorizontally(Size arrangeSize, int cellCount) {
			if (cellCount <= 0)
				return new Rect[] { new Rect(new Point(), arrangeSize) };
			Rect[] cells = new Rect[cellCount];
			int offset = 0;
			double height = arrangeSize.Height;
			int columnsAreaWidth = (int)arrangeSize.Width;
			double lastCellRemainder = arrangeSize.Width - columnsAreaWidth;
			int columnWidth = columnsAreaWidth / cellCount;
			int remainder = columnsAreaWidth - columnWidth * cellCount;
			for (int i = 0; i < cellCount; i++, remainder--) {
				int width = columnWidth + ((remainder > 0) ? 1 : 0);
				cells[i] = new Rect(offset, 0, width, height);
				offset += width;
			}
			Rect lastCellBounds = cells[cellCount - 1];
			lastCellBounds.Width += lastCellRemainder;
			cells[cellCount - 1] = lastCellBounds;
			return cells;
		}
		public static Size[] SplitSizeHorizontally(Size arrangeSize, int cellCount) {
			if (cellCount <= 0)
				return new Size[] { arrangeSize };
			Size[] cells = new Size[cellCount];
			double height = arrangeSize.Height;
			int columnsAreaWidth = (int)arrangeSize.Width;
			double lastCellRemainder = arrangeSize.Width - columnsAreaWidth;
			int columnWidth = columnsAreaWidth / cellCount;
			int remainder = columnsAreaWidth - columnWidth * cellCount;
			for (int i = 0; i < cellCount; i++, remainder--) {
				int width = columnWidth + ((remainder > 0) ? 1 : 0);
				cells[i] = new Size(width, height);
			}
			Size lastCellSize = cells[cellCount - 1];
			lastCellSize.Width += lastCellRemainder;
			cells[cellCount - 1] = lastCellSize;
			return cells;
		}
		public static Rect[] SplitVertically(Size availableSize, int cellCount) {
			if (cellCount <= 0)
				return new Rect[] { new Rect(new Point(), availableSize) };
			Rect[] cells = new Rect[cellCount];
			int offset = 0;
			double width = availableSize.Width;
			int columnsAreaHeight = (int)availableSize.Height;
			double lastCellRemainder = availableSize.Height - columnsAreaHeight;
			int columnHeight = columnsAreaHeight / cellCount;
			int remainder = columnsAreaHeight - columnHeight * cellCount;
			for (int i = 0; i < cellCount; i++, remainder--) {
				int height = columnHeight + ((remainder > 0) ? 1 : 0);
				cells[i] = new Rect(0, offset, width, height);
				offset += height;
			}
			Rect lastCellBounds = cells[cellCount - 1];
			lastCellBounds.Height += lastCellRemainder;
			cells[cellCount - 1] = lastCellBounds;
			return cells;
		}
		public static Size[] SplitSizeVertically(Size availableSize, int cellCount) {
			if (cellCount <= 0)
				return new Size[] { availableSize };
			Size[] cells = new Size[cellCount];
			double width = availableSize.Width;
			int columnsAreaHeight = (int)availableSize.Height;
			double lastCellRemainder = availableSize.Height - columnsAreaHeight;
			int columnHeight = columnsAreaHeight / cellCount;
			int remainder = columnsAreaHeight - columnHeight * cellCount;
			for (int i = 0; i < cellCount; i++, remainder--) {
				int height = columnHeight + ((remainder > 0) ? 1 : 0);
				cells[i] = new Size(width, height);
			}
			Size lastCellSize = cells[cellCount - 1];
			lastCellSize.Height += lastCellRemainder;
			cells[cellCount - 1] = lastCellSize;
			return cells;
		}
		protected override PanelMeasureResult MeasureItems(List<UIElement> items, Size availableSize) {
			availableSize = XpfSchedulerUtils.ValidateInfinitySize(items, availableSize);
			int count = items.Count;
			Size[] childrenSizes;
			if (Orientation == Orientation.Horizontal)
				childrenSizes = SplitSizeHorizontally(availableSize, count);
			else
				childrenSizes = SplitSizeVertically(availableSize, count);
			List<UIElementMeasureResult> result = new List<UIElementMeasureResult>();
			for (int i = 0; i < count; i++) {
				Children[i].Measure(childrenSizes[i]);
				result.Add(new UIElementMeasureResult(childrenSizes[i], Children[i]));
			}
			return new PanelMeasureResult(result, availableSize);
		}
		protected override Size MeasureOverride(Size availableSize) {
			return base.MeasureOverride(availableSize);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			return base.ArrangeOverride(finalSize);
		}
		protected override PanelArrangeResult ArrangeItems(List<UIElement> items, Size finalSize) {
			int count = items.Count;
			Rect[] childrenBounds;
			if (Orientation == Orientation.Horizontal)
				childrenBounds = SplitHorizontally(finalSize, count);
			else
				childrenBounds = SplitVertically(finalSize, count);
			List<UIElementArrangeResult> result = new List<UIElementArrangeResult>();
			for (int i = 0; i < count; i++) {
				result.Add(new UIElementArrangeResult(childrenBounds[i], items[i]));
				items[i].Arrange(childrenBounds[i]);
			}
			return new PanelArrangeResult(result, finalSize);
		}
	}
#endif
	public abstract class WeekPanel : SchedulerPanel {
		const double epsilon = 2.2204460492503131E-16;
		Size lastArrangeSize;
		Rect[] childrenArrangeRects;
		#region Properties
		#region IsCompressed
		public static readonly DependencyProperty IsCompressedProperty = CreateIsCompressedProperty();
		static DependencyProperty CreateIsCompressedProperty() {
			return DependencyPropertyHelper.RegisterProperty<WeekPanel, bool>("IsCompressed", false, OnIsCompressedChanged);
		}
		static void OnIsCompressedChanged(WeekPanel panel, DependencyPropertyChangedEventArgs<bool> e) {
			panel.InvalidateLayout();
		}
		public bool IsCompressed {
			get { return (bool)GetValue(IsCompressedProperty); }
			set { SetValue(IsCompressedProperty, value); }
		}
		#endregion
		protected Rect[] ChildrenArrangeRects { get { return childrenArrangeRects; } }
		#region CompressedIndex
		public int CompressedIndex {
			get { return (int)GetValue(CompressedIndexProperty); }
			set { SetValue(CompressedIndexProperty, value); }
		}
		public static readonly DependencyProperty CompressedIndexProperty =
			DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<WeekPanel, int>("CompressedIndex", -1, (d, e) => d.OnCompressedIndexChanged(e.OldValue, e.NewValue));
		#endregion
		#endregion
		#region Measure
		protected override Size MeasureOverrideCore(Size availableSize) {
			base.MeasureOverrideCore(availableSize);
			availableSize = ValidateInfinitySize(availableSize);
			MeasureCells(availableSize);
			return availableSize;
		}
		private Size ValidateInfinitySize(Size availableSize) {
			return XpfSchedulerUtils.ValidateInfinitySize(Children, availableSize);
		}
		protected internal virtual void MeasureCells(Size availableSize) {
			int count = Children.Count;
			Rect[] bounds = SplitCells(availableSize);
			ApplyInnerContentPaddingToBounds(bounds, GetActualContentPadding());
			for (int i = 0; i < count; i++) {
				Rect finalRect = bounds[i];
#if SILVERLIGHT
				Children[i].Measure(RectHelper.Size(finalRect));
#else
				Children[i].Measure(finalRect.Size);
#endif
			}
		}
		protected abstract void ApplyInnerContentPaddingToBounds(Rect[] bounds, Thickness contentPadding);
		#endregion
		#region Arrange
		protected override Size ArrangeOverrideCore(Size finalSize) {
			base.ArrangeOverrideCore(finalSize);
			ArrangeCells(finalSize);
			return finalSize;
		}
		protected internal virtual void ArrangeCells(Size finalSize) {
			SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.WeekPanel, "->WeekPanel.ArrangeCells: {0}-({1})", VisualElementHelper.GetElementName(this), VisualElementHelper.GetTypeName(this));
			int count = Children.Count;
			if (lastArrangeSize != finalSize || ChildrenArrangeRects.Length != count) {
				this.childrenArrangeRects = SplitCells(finalSize);
				lastArrangeSize = finalSize;
				ApplyInnerContentPaddingToBounds(ChildrenArrangeRects, GetActualContentPadding());
			}
			ErrorBetweenExpectedAndActualBoundsCalculator horizontalErrorCalculator = new ErrorBetweenExpectedAndActualBoundsCalculator(Orientation.Horizontal);
			ErrorBetweenExpectedAndActualBoundsCalculator verticalErrorCalculator = new ErrorBetweenExpectedAndActualBoundsCalculator(Orientation.Vertical);
			for (int i = 0; i < count; i++) {
				UIElement child = Children[i];
				SchedulerLogger.TraceOpenGroup(XpfLoggerTraceLevel.WeekPanel, "| process child[{0}]: {1}-({2})", i, VisualElementHelper.GetElementName(child), VisualElementHelper.GetTypeName(child));
				SchedulerLogger.Trace(XpfLoggerTraceLevel.WeekPanel, "calculated rect:{0}", ChildrenArrangeRects[i]);
				Rect bounds = GetBoundsCorrectedByError(horizontalErrorCalculator, verticalErrorCalculator, i);
				child.Arrange(bounds);
				CalculateBoundsError(horizontalErrorCalculator, verticalErrorCalculator, bounds.Size(), i);
				SchedulerLogger.Trace(XpfLoggerTraceLevel.WeekPanel, "corrected rect:{0}, RenderSize:{1}, TotalError:{2}", ChildrenArrangeRects[i], child.RenderSize, horizontalErrorCalculator.TotalError);
				SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.WeekPanel);
			}
			SchedulerLogger.TraceCloseGroup(XpfLoggerTraceLevel.WeekPanel);
		}
		protected virtual Rect GetBoundsCorrectedByError(ErrorBetweenExpectedAndActualBoundsCalculator horizontalErrorCalculator, ErrorBetweenExpectedAndActualBoundsCalculator verticalErrorCalculator, int childIndex) {
			Point calculatedLocation = horizontalErrorCalculator.CorrectPoint(ChildrenArrangeRects[childIndex].Location());
			Size calculatedSize = ChildrenArrangeRects[childIndex].Size();
			if (IsRightmostCell(childIndex))
				calculatedSize = horizontalErrorCalculator.CorrectLastItemSize(calculatedSize);
			if (IsCompressedCell(childIndex))
				calculatedLocation = verticalErrorCalculator.CorrectPoint(calculatedLocation);
			if (IsBottomCompressedCell(childIndex))
				calculatedSize = verticalErrorCalculator.CorrectLastItemSize(calculatedSize);
			return new Rect(calculatedLocation, calculatedSize);
		}
		protected virtual void CalculateBoundsError(ErrorBetweenExpectedAndActualBoundsCalculator horizontalErrorCalculator, ErrorBetweenExpectedAndActualBoundsCalculator verticalErrorCalculator, Size expectedSize, int childIndex) {
			Size childActualSize = Children[childIndex].RenderSize;
			horizontalErrorCalculator.CalculateError(childActualSize, expectedSize);
			if (IsCompressedCell(childIndex))
				verticalErrorCalculator.CalculateError(childActualSize, expectedSize);
		}
		bool IsRightmostCell(int i) {
			int cellCount =  Children.Count;
			if (i == cellCount - 1)
				return true;
			bool isCompressedCell = IsCompressedCell(i);
			if (isCompressedCell  && (i == cellCount - 2))
				return true;
			return false;
		}
		bool IsCompressedCell(int i) {
			if (!IsCompressed)
				return false;
			if (i == CompressedIndex || i == CompressedIndex + 1)
				return true;
			return false;
		}
		bool IsBottomCompressedCell(int i) {
			if (!IsCompressed)
				return false;
			return i == CompressedIndex + 1;
		}
		#endregion
		protected virtual Rect GetChildFinalRect(Size finalSize, Rect normalizedRect) {
			double left = GetSnappedPosition(finalSize.Width, normalizedRect.Left);
			double right = GetSnappedPosition(finalSize.Width, normalizedRect.Right);
			double top = GetSnappedPosition(finalSize.Height, normalizedRect.Top);
			double bottom = GetSnappedPosition(finalSize.Height, normalizedRect.Bottom);
			return new Rect(new Point(left, top), new Point(right, bottom));
		}
		double GetSnappedPosition(double size, double normalizedPosition) {
			if (Math.Abs(normalizedPosition) <= epsilon || Math.Abs(normalizedPosition - 1.0) <= epsilon)
				return size * normalizedPosition;
			else
				return (int)(size * normalizedPosition);
		}
		protected virtual void InvalidateLayout() {
			this.lastArrangeSize = Size.Empty;
			this.InvalidateMeasure();
		}
		void InvalidateChildrenMeasure() {
			for (int i = 0; i < Children.Count; i++) {
				Children[i].InvalidateMeasure();
			}
		}
		protected virtual void OnCompressedIndexChanged(int oldValue, int newValue) {
			InvalidateLayout();
		}
		protected abstract Rect[] SplitCells(Size availableSize);
	}
	public class GanttViewResourcePanel : Panel {
		protected class ResourceAccessor {
			static readonly ResourceAccessor empty = new ResourceAccessor(null);
			public static ResourceAccessor Empty { get { return empty; } }
			readonly ItemsControl itemsControl;
			public ResourceAccessor(ItemsControl itemsControl) {
				this.itemsControl = itemsControl;
			}
			public bool IsEmpty { get { return Object.ReferenceEquals(this, Empty); } }
			public VisualGanttResource GetResource(int index) {
				if (itemsControl != null)
					return itemsControl.Items[index] as VisualGanttResource;
				return null;
			}
		}
		const int maxCollapsedNodeHeight = 20;
		protected override Size MeasureOverride(Size availableSize) {
			Rect[] childrenRects = SplitAvailableSize(availableSize);
			int childrenCount = Children.Count;
			double maxWidth = 0;
			double maxHeight = 0;
			for (int i = 0; i < childrenCount; i++) {
				Children[i].Measure(childrenRects[i].Size());
				maxWidth = Math.Max(Children[i].DesiredSize.Width, maxWidth);
				maxHeight = Math.Max(Children[i].DesiredSize.Height, maxHeight);
			}
			return new Size(double.IsInfinity(availableSize.Width) ? maxWidth : availableSize.Width, double.IsInfinity(availableSize.Height) ? maxHeight : availableSize.Height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Rect[] childrenBounds = SplitAvailableSize(finalSize);
			int childrenCount = Children.Count;
			for (int i = 0; i < childrenCount; i++)
				Children[i].Arrange(childrenBounds[i]);
			return finalSize;
		}
		protected virtual Rect[] SplitAvailableSize(Size availableSize) {
			ItemsControl itemsControl = ItemsControl.GetItemsOwner(this);
			if (double.IsInfinity(availableSize.Height))
				return GetRectsForInfinityHeigth(new ResourceAccessor(itemsControl), availableSize);
			if (itemsControl == null)
				return SplitAvailableSizeUniformly(availableSize);
			return SplitAvailableSizeCore(new ResourceAccessor(itemsControl), availableSize);
		}
		protected Rect[] SplitAvailableSizeCore(ResourceAccessor accessor, Size availableSize) {
			int collapsedNodesCount = CalculateCollapsedNodesCount(accessor);
			int itemsCount = Children.Count;
			int expandedNodesCount = itemsCount - collapsedNodesCount;
			if (expandedNodesCount == 0)
				return SplitAvailableSizeUniformly(availableSize);
			int expandedNodesTotalHeight = (int)(availableSize.Height - (collapsedNodesCount * maxCollapsedNodeHeight));
			int expandedNodeHeight = (int)(expandedNodesTotalHeight / expandedNodesCount);
			if (expandedNodeHeight > 0 && expandedNodeHeight > maxCollapsedNodeHeight) {
				int remainder = expandedNodesTotalHeight % expandedNodesCount;
				return SplitAvailableSizeCore(availableSize, accessor, expandedNodeHeight, remainder);
			}
			return SplitAvailableSizeUniformly(availableSize);
		}
		protected Rect[] SplitAvailableSizeUniformly(Size availableSize) {
			int itemsCount = Children.Count;
			int nodeHeight = (int)(availableSize.Height / itemsCount);
			int remainder = (int)availableSize.Height % itemsCount;
			return SplitAvailableSizeCore(availableSize, ResourceAccessor.Empty, nodeHeight, remainder);
		}
		protected Rect[] SplitAvailableSizeCore(Size availableSize, ResourceAccessor accessor, int height, int remainder) {
			int itemsCount = Children.Count;
			Rect[] rects = new Rect[itemsCount];
			double verticalOffset = 0;
			for (int i = 0; i < itemsCount; i++) {
				int addition = remainder > 0 ? 1 : 0;
				VisualGanttResource resource = accessor.GetResource(i);
				double actualHeight;
				if (resource == null || resource.IsExpanded) {
					actualHeight = height + addition;
					remainder--;
				}
				else
					actualHeight = maxCollapsedNodeHeight;
				Point location = new Point(0, verticalOffset);
				Size size = new Size(availableSize.Width, actualHeight);
				rects[i] = new Rect(location, size);
				verticalOffset += actualHeight;
			}
			return rects;
		}
		protected Rect[] GetRectsForInfinityHeigth(ResourceAccessor accessor, Size availableSize) {
			int itemsCount = Children.Count;
			Rect[] rects = new Rect[itemsCount];
			for (int i = 0; i < itemsCount; i++) {
				VisualGanttResource resource = accessor.GetResource(i);
				double height = resource.IsExpanded ? availableSize.Height : maxCollapsedNodeHeight;
				Point location = new Point(0, 0);
				Size size = new Size(availableSize.Width, height);
				rects[i] = new Rect(location, size);
			}
			return rects;
		}
		int CalculateCollapsedNodesCount(ResourceAccessor accessor) {
			if (accessor.IsEmpty)
				return 0;
			int result = 0;
			int count = Children.Count;
			for (int i = 0; i < count; i++) {
				if (!accessor.GetResource(i).IsExpanded)
					result++;
			}
			return result;
		}
	}
}
namespace DevExpress.Xpf.Scheduler.Native {
	public interface ISupportElementPosition {
		void OnRecalculatePostions(ElementPosition oldValue, ElementPosition newValue);
	}
	public class ErrorBetweenExpectedAndActualBoundsCalculator {
		double totalError;
		readonly SizeHelperBase sizeHelper;
		public ErrorBetweenExpectedAndActualBoundsCalculator(Orientation orientation) {
			this.totalError = 0;
			this.sizeHelper = SizeHelperBase.GetDefineSizeHelper(orientation);
		}
		public double TotalError { get { return totalError; } private set { totalError = value; } }
		protected SizeHelperBase SizeHelper { get { return sizeHelper; } }
		public void CalculateError(Size actualSize, Size expectedSize) {
			double expectedDefineSize = SizeHelper.GetDefineSize(expectedSize);
			double actualDefineSize = SizeHelper.GetDefineSize(actualSize);
			double actualError = -expectedDefineSize + actualDefineSize;
			if (actualError > 0)
				actualError = 0;
			TotalError += actualError;
		}
		public Point CorrectPoint(Point expectedPoint) {
			double location = SizeHelper.GetDefinePoint(expectedPoint);
			location += TotalError;
			return sizeHelper.CreatePoint(location, sizeHelper.GetSecondaryPoint(expectedPoint));
		}
		public Size CorrectLastItemSize(Size expectedSize) {
			double size = SizeHelper.GetDefineSize(expectedSize);
			double correctedSize = size - TotalError;
			if (correctedSize < 0) {
				SchedulerLogger.Trace(XpfLoggerTraceLevel.Warning, "->ErrorBetweenExpectedAndActualBoundsCalculator.ErrorBetweenExpectedAndActualBoundsCalculator: wrong size {0}!!!!!", size);
				correctedSize = 0;
			}
			return SizeHelper.CreateSize(correctedSize, SizeHelper.GetSecondarySize(expectedSize));
		}
	}
}
