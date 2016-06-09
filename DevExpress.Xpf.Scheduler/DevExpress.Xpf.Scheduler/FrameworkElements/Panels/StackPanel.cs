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
using DevExpress.Xpf.Scheduler.Native;
using DevExpress.XtraScheduler;
using System.Collections.Specialized;
using DevExpress.Xpf.Utils;
using DevExpress.XtraScheduler.Drawing;
using DependencyPropertyHelper = DevExpress.Xpf.Core.Native.DependencyPropertyHelper;
#if SILVERLIGHT
using DevExpress.Xpf.Editors.Controls;
using DevExpress.Xpf.Core;
using Decorator =  DevExpress.Xpf.Core.WPFCompatibility.Decorator;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Scheduler.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#else
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.ComponentModel;
using DevExpress.XtraScheduler.Native;
using DevExpress.Xpf.Scheduler.Internal;
using DevExpress.XtraScheduler.Internal.Diagnostics;
#endif
namespace DevExpress.Xpf.Scheduler.Drawing {
	public class AppointmentControlContainer : DXContentPresenter {
		LayoutPropertyChangedEventHandler appointmentControlPropertyChanged;
		public event LayoutPropertyChangedEventHandler AppointmentControlPropertyChanged {
			add { appointmentControlPropertyChanged += value; }
			remove { appointmentControlPropertyChanged -= value; }
		}
		protected virtual void RaiseAppointmentControlPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if (appointmentControlPropertyChanged != null)
				appointmentControlPropertyChanged(this, e);
		}
		protected override void OnContentChanged(object oldContent, object newContent) {
			VisualAppointmentControl oldAppointmentControl = oldContent as VisualAppointmentControl;
			VisualAppointmentControl newAppointmentControl = newContent as VisualAppointmentControl;
			if (oldAppointmentControl != null)
				oldAppointmentControl.LayoutViewInfo.LayoutPropertyChanged -= new LayoutPropertyChangedEventHandler(LayoutViewInfo_LayoutPropertyChanged);
			if (newAppointmentControl != null)
				newAppointmentControl.LayoutViewInfo.LayoutPropertyChanged += new LayoutPropertyChangedEventHandler(LayoutViewInfo_LayoutPropertyChanged);
		}
		void LayoutViewInfo_LayoutPropertyChanged(object sender, DependencyPropertyChangedEventArgs e) {
			RaiseAppointmentControlPropertyChanged(e);
		}
	}
#if !SL
	public abstract class AnimatedAppointmentsPanel : AnimationPanel2 {
		protected override PanelMeasureResult MeasureItems(List<UIElement> items, Size availableSize) {
			Size result = availableSize;
			if (double.IsInfinity(availableSize.Width))
				result.Width = 300;
			if (double.IsInfinity(availableSize.Height))
				result.Height = 150;
			return new PanelMeasureResult(new List<UIElementMeasureResult>(), result);
		}
		#region SchedulerTimeCellControl
		public ISchedulerTimeCellControl SchedulerTimeCellControl {
			get { return (ISchedulerTimeCellControl)GetValue(SchedulerTimeCellControlProperty); }
			set { SetValue(SchedulerTimeCellControlProperty, value); }
		}
		public static readonly DependencyProperty SchedulerTimeCellControlProperty = DependencyPropertyManager.Register("SchedulerTimeCellControl", typeof(ISchedulerTimeCellControl), typeof(AnimatedAppointmentsPanel),
				new PropertyMetadata(null));
		#endregion
	}
#endif
	public class DragAppointmentsPanel : SchedulerCellsPanelBase {
		protected override Size MeasureOverride(Size availableSize) {
			Size result = availableSize;
			if (double.IsInfinity(availableSize.Width))
				result.Width = 300;
			if (double.IsInfinity(availableSize.Height))
				result.Height = 150;
			return result;
		}
		protected override void ArrangeContentCore() {
			return;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			return finalSize;
		}
	}
	public class AllDayAreaControlContainer : XPFContentControl {
		#region AllowScrolling
		public bool AllowScrolling {
			get { return (bool)GetValue(AllowScrollingProperty); }
			set { SetValue(AllowScrollingProperty, value); }
		}
		public static readonly DependencyProperty AllowScrollingProperty = CreateAllowScrollingProperty();
		static DependencyProperty CreateAllowScrollingProperty() {
			return DevExpress.Xpf.Core.Native.DependencyPropertyHelper.RegisterProperty<AllDayAreaControlContainer, bool>("AllowScrolling", false);
		}
		#endregion
		static AllDayAreaControlContainer() {
#if !SL
			OverridesDefaultStyleProperty.OverrideMetadata(typeof(AllDayAreaControlContainer), new FrameworkPropertyMetadata());
#else
#endif
		}
	}
	public class HorizontalWeekPanel : WeekPanel {
		protected override Rect[] SplitCells(Size availableSize) {
			int totalCellsCount = Children.Count;
			bool isCompressed = IsCompressed && CompressedIndex >= 0;
			int baseCellsCount = isCompressed ? totalCellsCount - 1 : totalCellsCount;
			if (baseCellsCount <= 0)
				return new Rect[] { };
			Rect[] splitedCells = PixelSnappedUniformGridLayoutHelper.SplitSizeDpiAware(availableSize, baseCellsCount, new Thickness(), Orientation.Horizontal);
			List<Rect> splitedCellList = new List<Rect>(splitedCells);
			if (isCompressed) {
				Rect rect = splitedCellList[CompressedIndex];
				Rect[] compressedCellRects = PixelSnappedUniformGridLayoutHelper.SplitDpiAware(rect, 2, new Thickness(), Orientation.Vertical);
				splitedCellList[CompressedIndex] = compressedCellRects[0];
				splitedCellList.Insert(CompressedIndex + 1, compressedCellRects[1]);
			}
			return splitedCellList.ToArray();
		}
		protected override void ApplyInnerContentPaddingToBounds(Rect[] bounds, Thickness contentPadding) {
			int count = Children.Count;
			int lastIndex = Children.Count - 1;
			for (int i = 0; i < count; i++) {
				bool isLeft = i == 0 || (IsCompressed && CompressedIndex == 0 && i == 1);
				bool isRight = i == lastIndex || (IsCompressed && CompressedIndex == lastIndex - 1 && i == lastIndex - 1);
				bool isTop = !IsCompressed || (i != CompressedIndex + 1);
				bool isBottom = !IsCompressed || (i != CompressedIndex);
				Rect rect = bounds[i];
				if (isLeft) {
					rect.X += contentPadding.Left;
					rect.Width -= contentPadding.Left;
				}
				if (isRight) {
					rect.Width -= contentPadding.Right;
				}
				if (isTop) {
					rect.Y += contentPadding.Top;
					rect.Height -= contentPadding.Top;
				}
				if (isBottom)
					rect.Height -= contentPadding.Bottom;
				bounds[i] = rect;
			}
		}
		protected override void RecalculatePositions() {
			ElementPosition panelPosition = SchedulerItemsControl.GetElementPosition(this) ?? ElementPosition.Standalone;
			UIElementCollection children = Children;
			int count = children.Count;
			for (int i = 0; i < count; i++) {
				bool isTopRow = i <= CompressedIndex || i > CompressedIndex + 1;
				bool isBottomRow = i != CompressedIndex;
				bool isFirstColumn = i == 0 || (i == 1 && CompressedIndex == 0);
				bool isLastColumn = (i == count - 1) || (i == count - 2 && CompressedIndex == count - 2);
				ElementPositionCore horizontalElementPosition = ElementPositionPropertyHelper.CalculateCore(panelPosition.HorizontalElementPosition, isFirstColumn, isLastColumn);
				ElementPositionCore verticalElementPosition = ElementPositionPropertyHelper.CalculateCore(panelPosition.VerticalElementPosition, isTopRow, isBottomRow);
				SchedulerItemsControl.SetElementPosition(children[i], new ElementPosition(horizontalElementPosition, verticalElementPosition));
			}
		}
	}
	public class TwoColumnVerticalWeekPanel : WeekPanel {
		static readonly Thickness MiddleCellBorderThicknes = new Thickness(1, 0, 0, 0);
		static readonly Thickness MiddleCellBorderPadding = new Thickness(0, 0, 0, 0);
		static readonly Thickness LeftCellBorderThicknes = new Thickness(0, 0, 0, 0);
		static readonly Thickness LeftCellBorderPadding = new Thickness(1, 0, 0, 0);
		protected override Rect[] SplitCells(Size availableSize) {
			int count = Children.Count;
			if (IsCompressed)
				count--;
			if (count <= 0)
				return new Rect[] { };
			count /= 2;
			Rect[] horizontalRects = PixelSnappedUniformGridLayoutHelper.SplitSizeDpiAware(availableSize, 2, new Thickness(), Orientation.Horizontal);
			Rect[] leftSideRects = PixelSnappedUniformGridLayoutHelper.SplitDpiAware(horizontalRects[0], 3, new Thickness(), Orientation.Vertical);
			Rect[] rightSideRects = PixelSnappedUniformGridLayoutHelper.SplitDpiAware(horizontalRects[1], 3, new Thickness(), Orientation.Vertical);
			List<Rect> resultRects = new List<Rect>(leftSideRects);
			resultRects.AddRange(rightSideRects);
			if (IsCompressed) {
				Rect rect = resultRects[CompressedIndex];
				Rect[] compressedRects = PixelSnappedUniformGridLayoutHelper.SplitDpiAware(rect, 2, new Thickness(), Orientation.Vertical);
				resultRects[CompressedIndex] = compressedRects[0];
				resultRects.Insert(CompressedIndex + 1, compressedRects[1]);
			}
			return resultRects.ToArray();
		}
		protected override void ApplyInnerContentPaddingToBounds(Rect[] bounds, Thickness contentPadding) {
			int count = Children.Count;
			int lastIndex = Children.Count - 1;
			int startTopIndex = count / 2;
			if (CompressedIndex < startTopIndex)
				startTopIndex++;
			for (int i = 0; i < count; i++) {
				bool isLeft = i < startTopIndex;
				bool isRight = i >= startTopIndex;
				bool isTop = i == 0 || i == startTopIndex;
				bool isBottom = i == lastIndex || i == startTopIndex - 1;
				Rect rect = bounds[i];
				if (isLeft) {
					rect.X += contentPadding.Left;
					rect.Width -= contentPadding.Left;
				}
				if (isRight) {
					rect.Width -= contentPadding.Right;
				}
				if (isTop) {
					rect.Y += contentPadding.Top;
					rect.Height -= contentPadding.Top;
				}
				if (isBottom)
					rect.Height -= contentPadding.Bottom;
				bounds[i] = rect;
			}
		}
		protected override void RecalculatePositions() {
			ElementPosition panelPosition = SchedulerItemsControl.GetElementPosition(this) ?? ElementPosition.Standalone;
			int count = Children.Count;
			int startTopIndex = count / 2;
			if (CompressedIndex < startTopIndex)
				startTopIndex++;
			for (int i = 0; i < count; i++) {
				UIElement child = Children[i];
				bool isTopRow = i == 0 || i == startTopIndex;
				bool isBottomRow = (i == startTopIndex - 1) || (i == count - 1);
				bool isFirstColumn = i < startTopIndex;
				ElementPositionCore horizontalElementPosition = ElementPositionPropertyHelper.CalculateCore(panelPosition.HorizontalElementPosition, isFirstColumn, !isFirstColumn);
				ElementPositionCore verticalElementPosition = ElementPositionPropertyHelper.CalculateCore(panelPosition.VerticalElementPosition, isTopRow, isBottomRow);
				ElementPosition elementPositon = new ElementPosition(horizontalElementPosition, verticalElementPosition);
				elementPositon.VerticalWeekHorizontalPosition = isFirstColumn ? ElementRelativePosition.Start : ElementRelativePosition.End;
				if (isTopRow)
					elementPositon.VerticalWeekVerticalPosition = ElementRelativePosition.Start;
				else if (isBottomRow)
					elementPositon.VerticalWeekVerticalPosition = ElementRelativePosition.End;
				else
					elementPositon.VerticalWeekVerticalPosition = ElementRelativePosition.Middle;
				SchedulerItemsControl.SetElementPosition(child, elementPositon);
			}
		}
		protected override Rect GetBoundsCorrectedByError(ErrorBetweenExpectedAndActualBoundsCalculator horizontalErrorCalculator, ErrorBetweenExpectedAndActualBoundsCalculator verticalErrorCalculator, int childIndex) {
			return ChildrenArrangeRects[childIndex];
		}
		protected override void CalculateBoundsError(ErrorBetweenExpectedAndActualBoundsCalculator horizontalErrorCalculator, ErrorBetweenExpectedAndActualBoundsCalculator verticalErrorCalculator, Size expectedSize, int childIndex) {
		}
	}
	public class CompressedStackLinePanel : Panel {
		#region properties
		protected SizeHelperBase SizeHelper { get { return SizeHelperBase.GetDefineSizeHelper(Orientation); } }
		public bool IsCompressed { get { return CompressedIndex >= 0; } }
		public bool IsVertical { get { return Orientation == Orientation.Vertical; } }
		List<CompressedPanelLine> Lines = new List<CompressedPanelLine>();
		protected int ChildrenCount { get { return Children.Count - Lines.Count; } }
		#region CompressedIndex
		public int CompressedIndex {
			get { return (int)GetValue(CompressedIndexProperty); }
			set { SetValue(CompressedIndexProperty, value); }
		}
		public static readonly DependencyProperty CompressedIndexProperty =
			DependencyPropertyManager.Register("CompressedIndex", typeof(int), typeof(CompressedStackLinePanel), new PropertyMetadata(-1));
		#endregion
		#region Orientation
		public Orientation Orientation {
			get { return (Orientation)GetValue(OrientationProperty); }
			set { SetValue(OrientationProperty, value); }
		}
		public static readonly DependencyProperty OrientationProperty =
			DependencyPropertyManager.Register("Orientation", typeof(Orientation), typeof(CompressedStackLinePanel), new PropertyMetadata(Orientation.Horizontal, (d, e) => { ((CompressedStackLinePanel)d).OnOrientationChanged(); }));
		#endregion
		#region LinesBrush
		public Brush LinesBrush {
			get { return (Brush)GetValue(LinesBrushProperty); }
			set { SetValue(LinesBrushProperty, value); }
		}
		public static readonly DependencyProperty LinesBrushProperty =
			DependencyPropertyManager.Register("LinesBrush", typeof(Brush), typeof(CompressedStackLinePanel), new PropertyMetadata(null));
		#endregion
		#region LinesWidth
		public double LinesWidth {
			get { return (double)GetValue(LinesWidthProperty); }
			set { SetValue(LinesWidthProperty, value); }
		}
		public static readonly DependencyProperty LinesWidthProperty =
			DependencyProperty.Register("LinesWidth", typeof(double), typeof(CompressedStackLinePanel), new PropertyMetadata(1.0));
		#endregion
		#endregion
		public CompressedStackLinePanel() {
			SetBindings();
		}
		protected virtual void SetBindings() {
		}
		protected virtual void OnOrientationChanged() {
		}
		#region Arrange
		protected override Size ArrangeOverride(Size finalSize) {
			double integralWidth = 0;
			for (int i = 0; i < ChildrenCount; i++) {
				FrameworkElement child = (FrameworkElement)Children[i];
				integralWidth = ArrangeChild(child, i, finalSize, integralWidth);
			}
			return finalSize;
		}
		protected virtual double ArrangeChild(FrameworkElement child, int i, Size finalSize, double integralWidth) {
			Size childSize = ChildAvailableSize(finalSize, i);
			Point childPosition = ChildPosition(finalSize, i, integralWidth);
			Rect childRect = new Rect(childPosition, childSize);
			child.Arrange(childRect);
			ArrangeLine(GetLine(i), childRect, i, finalSize);
			return CalcIntegralWidth(childSize, childPosition, i);
		}
		protected virtual void ArrangeLine(CompressedPanelLine line, Rect childRect, int i, Size finalSize) {
			if (line == null)
				return;
			Rect pos = GetLinePosition(childRect, i, finalSize);
			line.Arrange(pos);
		}
		protected internal virtual Rect GetLinePosition(Rect childRect, int i, Size finalSize) {
			Size lineDesiredSize = LineSize(childRect.Size(), finalSize, i);
			Rect pos;
			if (!IsVertical) {
				if (i == CompressedIndex)
					pos = new Rect(childRect.BottomLeft(), lineDesiredSize);
				else if (i == CompressedIndex + 1)
					pos = new Rect(new Point(childRect.Right, 0), lineDesiredSize);
				else
					pos = new Rect(childRect.TopRight(), lineDesiredSize);
			}
			else {
				if (i == CompressedIndex)
					pos = new Rect(childRect.TopRight(), lineDesiredSize);
				else if (i == CompressedIndex + 1)
					pos = new Rect(new Point(0, childRect.Bottom), lineDesiredSize);
				else
					pos = new Rect(childRect.BottomLeft(), lineDesiredSize);
			}
			return pos;
		}
		protected virtual internal double CalcIntegralWidth(Size childSize, Point childPosition, int i) {
			if (i == CompressedIndex)
				return SizeHelper.GetDefinePoint(childPosition);
			return SizeHelper.GetDefinePoint(childPosition) + SizeHelper.GetDefineSize(childSize) + LinesWidth;
		}
		protected internal virtual Point ChildPosition(Size finalSize, int i, double integralWidth) {
			Point res = SizeHelper.CreatePoint(
				ChildDefinePoint(finalSize, i, integralWidth),
				ChildSecondaryPoint(finalSize, i));
			return res;
		}
		protected virtual double ChildSecondaryPoint(Size finalSize, int i) {
			if (!IsCompressed || i != CompressedIndex + 1)
				return 0;
			XtraSchedulerDebug.Assert(i != 0);
			double blockSize = ChildSecondarySize(finalSize, CompressedIndex);
			return blockSize + LinesWidth;
		}
		protected virtual double ChildDefinePoint(Size finalSize, int i, double integralWidth) {
			return integralWidth;
		}
		#endregion
		#region Measure
		protected override Size MeasureOverride(Size availableSize) {
			availableSize = XpfSchedulerUtils.ValidateInfinitySize(Children, availableSize);
#if SILVERLIGHT
			PrepareLines();
#endif
			for (int i = 0; i < ChildrenCount; i++) {
				FrameworkElement child = (FrameworkElement)Children[i];
				MeasureChild(child, i, availableSize);
			}
			return availableSize;
		}
		protected virtual void PrepareLines() {
			Lines.Clear();
			Lines.AddRange(GetExistingLines());
			while (ChildrenCount > Lines.Count + 1) {
				CompressedPanelLine line = CreateLine();
				Lines.Add(line);
#if SILVERLIGHT
				Children.Add(line);
#endif
			}
		}
		protected virtual CompressedPanelLine CreateLine() {
			CompressedPanelLine line = new CompressedPanelLine();
			Binding b = new Binding("LinesBrush");
			b.Source = this;
			line.SetBinding(Panel.BackgroundProperty, b);
			return line;
		}
		bool IsPanelLine(UIElement elem) {
			return elem is CompressedPanelLine;
		}
		List<CompressedPanelLine> GetExistingLines() {
			List<CompressedPanelLine> result = new List<CompressedPanelLine>();
			foreach (UIElement element in Children) {
				if (IsPanelLine(element)) {
					result.Add(element as CompressedPanelLine);
				}
			}
			return result;
		}
		protected virtual double ChildDefineSize(Size availableSize, int childIndex) {
			int blocksCount = ChildrenCount - (IsCompressed ? 1 : 0);
			double linesWidth = (blocksCount - 1) * LinesWidth;
			double ownerSize = (SizeHelper.GetDefineSize(availableSize) - linesWidth);
			double size = ownerSize / blocksCount;
			double roundedSize = Math.Floor(size);
			int calculatedIndex = (IsCompressed && childIndex > CompressedIndex) ? childIndex - 1 : childIndex;
			if (calculatedIndex < ownerSize - roundedSize * blocksCount)
				roundedSize++;
			return roundedSize;
		}
		protected virtual double ChildSecondarySize(Size availableSize, int childIndex) {
			double ownerSize = SizeHelper.GetSecondarySize(availableSize);
			if (IsCompressed) {
				if (childIndex == CompressedIndex)
					return Math.Floor((ownerSize - LinesWidth) / 2);
				if (childIndex == CompressedIndex + 1)
					return ownerSize - LinesWidth - Math.Floor((ownerSize - LinesWidth) / 2);
			}
			return ownerSize;
		}
		protected internal virtual Size ChildAvailableSize(Size availableSize, int childIndex) {
			return SizeHelper.CreateSize(
				ChildDefineSize(availableSize, childIndex),
				ChildSecondarySize(availableSize, childIndex));
		}
		CompressedPanelLine GetLine(int index) {
#if SILVERLIGHT
			int i = ChildrenCount + index;
			if(i >= Children.Count) return null;
			return (CompressedPanelLine)Children[i];
#else
			return null;
#endif
		}
		protected virtual Size MeasureChild(FrameworkElement child, int i, Size availableSize) {
			Size res = ChildAvailableSize(availableSize, i);
			child.Measure(res);
			CompressedPanelLine line = GetLine(i);
			if (line != null)
				line.Measure(LineSize(res, availableSize, i));
			return res;
		}
		protected virtual Size LineSize(Size res, Size availableSize, int i) {
			if (i == CompressedIndex)
				return SizeHelper.CreateSize(SizeHelper.GetDefineSize(res), LinesWidth);
			return SizeHelper.CreateSize(LinesWidth, SizeHelper.GetSecondarySize(availableSize));
		}
		#endregion
	}
	public class CompressedPanelLine : Grid {
		public CompressedPanelLine() {
			ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(0, GridUnitType.Star) });
			RowDefinitions.Add(new RowDefinition() { Height = new GridLength(0, GridUnitType.Star) });
			Children.Add(new Rectangle() { Fill = new SolidColorBrush(Colors.Red) });
		}
	}
	public class StretchPanel : Panel {
		protected override Size MeasureOverride(Size availableSize) {
			double requiredWidth = 0;
			double requiredHeight = 0;
			for (int i = 0; i < Children.Count; i++) {
				UIElement elem = Children[i];
				elem.Measure(availableSize);
				requiredWidth = Math.Max(requiredWidth, elem.DesiredSize.Width);
				requiredHeight += elem.DesiredSize.Height;
			}
			Size resultSize = new Size();
			FrameworkElement parentElement = Parent as FrameworkElement;
			resultSize.Width = CalculateRequiredWidth(parentElement, availableSize.Width, requiredWidth);
			resultSize.Height = CalculateRequiredHeight(parentElement, availableSize.Height, requiredHeight);
			return resultSize;
		}
		double CalculateRequiredWidth(FrameworkElement parent, double availableWidth, double requiredWidth) {
			if (Double.IsInfinity(availableWidth)) {
				if (parent != null && parent.ActualWidth > 0)
					return parent.ActualWidth;
				else
					return requiredWidth;
			}
			return Math.Max(availableWidth, requiredWidth);
		}
		double CalculateRequiredHeight(FrameworkElement parent, double availableHeight, double requiredHeight) {
			if (Double.IsInfinity(availableHeight)) {
				if (parent != null && parent.ActualHeight > 0)
					return parent.ActualHeight;
				else
					return requiredHeight;
			}
			return Math.Max(availableHeight, requiredHeight);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			for (int i = 0; i < Children.Count; i++) {
				FrameworkElement elem = Children[i] as FrameworkElement;
				if (elem == null)
					continue;
				double x = CalculateHorizontalCoordinate(finalSize.Width, elem);
				double y = CalculateVerticalCoordinate(finalSize.Height, elem);
				elem.Arrange(new Rect(new Point(x, y), new Size(finalSize.Width - x, finalSize.Height - y)));
			}
			return finalSize;
		}
		double CalculateVerticalCoordinate(double availableHeight, FrameworkElement elem) {
			switch (elem.VerticalAlignment) {
				case VerticalAlignment.Stretch:
				case VerticalAlignment.Top:
					return 0;
				case VerticalAlignment.Center:
					return (availableHeight - elem.DesiredSize.Height) / 2;
				case VerticalAlignment.Bottom:
					return availableHeight - elem.DesiredSize.Height;
				default:
					throw new ArgumentException();
			}
		}
		double CalculateHorizontalCoordinate(double availableWidth, FrameworkElement elem) {
			switch (elem.HorizontalAlignment) {
				case HorizontalAlignment.Stretch:
				case HorizontalAlignment.Left:
					return 0;
				case HorizontalAlignment.Center:
					return (availableWidth - elem.DesiredSize.Width) / 2;
				case HorizontalAlignment.Right:
					return availableWidth - elem.DesiredSize.Width;
				default:
					throw new ArgumentException();
			}
		}
	}
}
