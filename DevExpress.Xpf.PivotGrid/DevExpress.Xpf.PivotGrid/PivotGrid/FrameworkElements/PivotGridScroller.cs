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
using System.Windows;
using System.Windows.Controls.Primitives;
using DevExpress.Xpf.Core;
using System.Windows.Controls;
#if SL
using ScrollViewer = System.Windows.Controls.ContentControl;
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using System.Windows.Media;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
using System.Windows.Input;
using System.Collections.Generic;
using System.Windows.Threading;
using System.IO;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	[
	TemplatePart(Name = TemplatePartCellsBestFitDecorator, Type = typeof(BestFitDecorator)),
	TemplatePart(Name = TemplatePartColumnValuesBestFitDecorator, Type = typeof(BestFitDecorator)),
	TemplatePart(Name = TemplatePartRowValuesBestFitDecorator, Type = typeof(BestFitDecorator)),
	TemplatePart(Name = TemplatePartFilterHeaders, Type = typeof(FieldHeaders)),
	TemplatePart(Name = TemplatePartDataHeaders, Type = typeof(FieldHeaders)),
	TemplatePart(Name = TemplatePartColumnHeaders, Type = typeof(FieldHeaders)),
	TemplatePart(Name = TemplatePartRowHeaders, Type = typeof(FieldHeaders)),
	TemplatePart(Name = TemplatePartColumnValues, Type = typeof(FieldValuesPresenter)),
	TemplatePart(Name = TemplatePartRowValues, Type = typeof(FieldValuesPresenter)),
	TemplatePart(Name = TemplatePartCells, Type = typeof(ScrollableCellsAreaPresenter)),
	]
	public class PivotGridScroller : ScrollViewer {
		internal const string TemplatePartCellsBestFitDecorator = "PART_CellsBestFitControlDecorator",
			TemplatePartColumnValuesBestFitDecorator = "PART_ColumnValuesBestFitControlDecorator",
			TemplatePartRowValuesBestFitDecorator = "PART_RowValuesBestFitControlDecorator",
			TemplatePartFilterHeaders = "PART_FilterHeaders",
			TemplatePartDataHeaders = "PART_DataHeaders",
			TemplatePartColumnHeaders = "PART_ColumnHeaders",
			TemplatePartRowHeaders = "PART_RowHeaders",
			TemplatePartColumnValues = "PART_ColumnValues",
			TemplatePartRowValues = "PART_RowValues",
			TemplatePartCells = "PART_Cells";
		const string HorizontalScrollGroupsName = "HorizontalScrollGroups",
			HorizontalScrollFixedStateName = "Fixed",
			HorizontalScrollUnfixStateName = "Unfixed";
#if !SL
		const string TouchScrollStateName = "Touch";
#endif
		public static readonly DependencyProperty ScrollPartProperty;
		public static readonly DependencyProperty PivotGridProperty;
#if !SL
		public static readonly DependencyProperty ScrollBarModeProperty;
#endif
		static PivotGridScroller() {
			Type ownerType = typeof(PivotGridScroller);
			ScrollPartProperty = DependencyPropertyManager.RegisterAttached("ScrollPart", typeof(IScrollPartInitializer), ownerType, new PropertyMetadata(OnScrollerPropertyChanged));
			PivotGridProperty = DependencyPropertyManager.RegisterAttached("PivotGrid", typeof(PivotGridControl), ownerType, new PropertyMetadata(OnScrollerPropertyChanged));
#if !SL
			ScrollBarModeProperty = DependencyPropertyManager.Register("ScrollBarMode", typeof(ScrollBarMode), typeof(PivotGridScroller), new PropertyMetadata((d, e) => ((PivotGridScroller)d).EnsureVisualState()));
#endif
		}
		public PivotGridScroller() { }
#if !SL
		public ScrollBarMode ScrollBarMode {
			get { return (ScrollBarMode)GetValue(ScrollBarModeProperty); }
			set { SetValue(ScrollBarModeProperty, value); }
		}
#endif
		protected PivotGridControl PivotGrid { get { return (PivotGridControl)GetValue(PivotGridProperty); } }
		protected internal FieldValuesPresenter ColumnValues { get; set; }
		protected internal FieldValuesPresenter RowValues { get; set; }
		protected internal FrameworkElement Resizer { get; set; }
		protected internal ScrollableCellsAreaPresenter Cells { get; set; }
		protected internal FrameworkElement FocusAdorner { get; set; }
		public static void SetScrollPart(DependencyObject element, IScrollPartInitializer value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(ScrollPartProperty, value);
		}
		public static IScrollPartInitializer GetScrollPart(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (IScrollPartInitializer)element.GetValue(ScrollPartProperty);
		}
		public static void SetPivotGrid(DependencyObject element, PivotGridControl value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PivotGridProperty, value);
		}
		public static PivotGridControl GetPivotGrid(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (PivotGridControl)element.GetValue(PivotGridProperty);
		}
#if SL
		protected internal System.Windows.Controls.ScrollViewer Scroller { get; private set; }
		public void ScrollToHorizontalOffset(int x) {
			Scroller.ScrollToHorizontalOffset(x);
		}
		public void ScrollToVerticalOffset(int y) {
			Scroller.ScrollToVerticalOffset(y);
		}
		public double ScrollableHeight {
			get { return Convert.ToInt32(Scroller.ScrollableHeight); }
		}
		public double ScrollableWidth {
			get { return Convert.ToInt32(Scroller.ScrollableWidth); }
		}
		public int ViewportHeight {
			get { return Convert.ToInt32(Scroller.ViewportHeight); }
		}
		public int ViewportWidth {
			get { return Convert.ToInt32(Scroller.ViewportWidth); }
		}
		internal int HorizontalOffset {
			get { return Convert.ToInt32(Scroller.HorizontalOffset); }
		}
		internal int VerticalOffset {
			get { return Convert.ToInt32(Scroller.VerticalOffset); }
		}
#else
		protected override void OnManipulationStarting(ManipulationStartingEventArgs e) {
			e.ManipulationContainer = this;
			e.Handled = true;
			base.OnManipulationStarting(e);
			e.Mode = ManipulationModes.All;
		}
		protected override void OnManipulationDelta(System.Windows.Input.ManipulationDeltaEventArgs e) {
			if(e.CumulativeManipulation.Rotation > 90 && PivotGrid.CanSwapAreas()) {
				PivotGrid.SwapAreas();
				e.Complete();
			}
			int count = 0;
			List<IManipulator> mans = new List<IManipulator>();
			foreach(IManipulator man in e.Manipulators) {
				mans.Add(man);
				count++;
			}
			if(count > 0 && mans[0].GetPosition(Cells).Y < 0) {
				return;
			}
			if(count > 1) {
				ScrollableAreaCell cell1 = Cells.GetItemBySourceOrCoord(null, mans[0].GetPosition(Cells), false, false, true);
				ScrollableAreaCell cell2 = Cells.GetItemBySourceOrCoord(null, mans[1].GetPosition(Cells), false, false, true);
				if(cell1 != null && cell2 != null) {
					int minY = Math.Min(cell1.ValueItem.MinLevel, cell2.ValueItem.MinLevel);
					int maxY = Math.Max(cell1.ValueItem.MinLevel, cell2.ValueItem.MinLevel);
					int minX = Math.Min(cell1.ValueItem.MinIndex, cell2.ValueItem.MinIndex);
					int maxX = Math.Max(cell1.ValueItem.MinIndex, cell2.ValueItem.MinIndex);
					PivotGrid.Selection = new System.Drawing.Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
					StopTouchSelectionTimer();
					return;
				}
			} else {
				ScrollableAreaCell cell1 = null;
				if(mans.Count > 0)
					cell1 = Cells.GetItemBySourceOrCoord(null, mans[0].GetPosition(Cells), false, false, true);
				if(cell1 != null) {
					lastCellX = cell1.ValueItem.MinIndex;
					lastCellY = cell1.ValueItem.MinLevel;
					lastSelection = PivotGrid.Selection;
					if(lastSelection.Width > 1 || lastSelection.Height > 1) {
						StartTouchSelectionTimer();
					}
				}
			}
			base.OnManipulationDelta(e);
		}
		protected override void OnPreviewStylusUp(StylusEventArgs e) {
			StopTouchSelectionTimer();
			base.OnPreviewStylusUp(e);
		}
		protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e) {
			StopTouchSelectionTimer();
			base.OnManipulationCompleted(e);
		}
		void StartTouchSelectionTimer() {
			if(touchSelectionTimer == null) {
				touchSelectionTimer = new DispatcherTimer();
				touchSelectionTimer.IsEnabled = false;
				touchSelectionTimer.Tick += touchSelectionTimerTick;
			}
			touchSelectionTimer.IsEnabled = false;
			touchSelectionTimer.Interval = new TimeSpan(0, 0, 1);
			touchSelectionTimer.IsEnabled = true;
		}
		void StopTouchSelectionTimer() {
			if(touchSelectionTimer != null && touchSelectionTimer.IsEnabled)
				touchSelectionTimer.IsEnabled = false;
		}
		int lastCellX, lastCellY;
		System.Drawing.Rectangle lastSelection;
		DispatcherTimer touchSelectionTimer = null;
		void ClearPivotSelection() {
			PivotGrid.Selection = new System.Drawing.Rectangle(lastCellX, lastCellY, 1, 1);
		}
		void touchSelectionTimerTick(object sender, EventArgs e) {
			DispatcherTimer timer = ((DispatcherTimer)sender);
			timer.Tick -= touchSelectionTimerTick;
			touchSelectionTimer = null;
			timer.IsEnabled = false;
			ClearPivotSelection();
		}
		internal new int HorizontalOffset {
			get { return Convert.ToInt32(base.HorizontalOffset); }
		}
		internal new int VerticalOffset {
			get { return Convert.ToInt32(base.VerticalOffset); }
		}
		protected override Size MeasureOverride(Size constraint) {
			Visibility vertVisibility, horVisibility;
			if(ScrollInfo == null || IsGreater(ScrollInfo.ExtentHeight, ScrollInfo.ViewportHeight))
				vertVisibility = Visibility.Visible;
			else
				vertVisibility = Visibility.Collapsed;
			if(ScrollInfo == null || IsGreater(ScrollInfo.ExtentWidth, ScrollInfo.ViewportWidth))
				horVisibility = Visibility.Visible;
			else
				horVisibility = Visibility.Collapsed;
			UIElement element = (this.VisualChildrenCount > 0) ? (this.GetVisualChild(0) as UIElement) : null;
			if(element == null || vertVisibility != ComputedVerticalScrollBarVisibility || horVisibility != ComputedHorizontalScrollBarVisibility)
				return base.MeasureOverride(constraint);
			element.InvalidateMeasure();
			element.Measure(constraint);
			return element.DesiredSize;
		}
		internal new int ViewportHeight {
			get { return Cells.GetViewPortHeight(Cells.ActualHeight, false); }
		}
		internal new int ViewportWidth {
			get { return Cells.GetViewPortWidth(Cells.ActualWidth, false); }
		}
		bool IsGreater(double val1, double val2) {
			if(val1 <= val2)
				return false;
			double num = ((Math.Abs(val1) + Math.Abs(val2)) + 10.0) * 2.2204460492503131E-16;
			double num2 = val1 - val2;
			return !((-num < num2) && (num > num2));
		}
#endif
		internal void UpdateCellBorders(ScrollableAreaCell cell) {
			Cells.UpdateCellBorders(cell);
			if(cell.IsTopMost) {
				Thickness border = cell.Border;
				border.Top = 1;
				cell.Border = border;
			}
		}
		internal void UpdateCellBorders(ScrollableAreaCell cell, bool isColumn) {
			if(isColumn)
				ColumnValues.UpdateCellBorders(cell);
			else
				RowValues.UpdateCellBorders(cell);
			if(cell.IsTopMost) {
				Thickness border = cell.Border;
				border.Top = 1;
				cell.Border = border;
			}
		}
		internal void MakeCellVisible(System.Drawing.Point cell) {
			Cells.MakeCellVisible(cell);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
#if SL
			Scroller = Content as System.Windows.Controls.ScrollViewer;
#endif
			EnsureVisualState();
		}
		internal void SynchronizeScrolling() {
			if(Cells == null)
				return;
			if(RowValues != null)
				Cells.SynchronizeScrollingWith(RowValues);
			if(ColumnValues != null)
				Cells.SynchronizeScrollingWith(ColumnValues);
		}
		public static void OnScrollerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridControl pivot = PivotGridScroller.GetPivotGrid(d);
			if(pivot == null)
				return;
			IScrollPartInitializer initializer = PivotGridScroller.GetScrollPart(d);
			if(initializer == null)
				return;
			initializer.OnScrollPartChanged(pivot, d);
		}
		protected internal ScrollBar GetVerticalScrollBar() {
#if !SL
			return GetTemplateChild("PART_VerticalScrollBar") as ScrollBar;
#else
			return DevExpress.Xpf.Core.Native.LayoutHelper.FindElementByName(Scroller, "VerticalScrollBar") as ScrollBar;
#endif
		}
		protected internal void EnsureScrollingMode() {
			Cells.EnsureScrollingMode();
		}
		protected internal void EnsureHorizontalScrollMode() {
			if(Cells == null)
				return;
			Cells.EnsureHorizontalScrollingMode();
			EnsureVisualState();
		}
		protected internal void EnsureVisualState() {
			PivotGridControl pivot = PivotGrid;
			if(PivotGrid == null && Cells != null && Cells.Data != null)
				pivot = Cells.Data.PivotGrid;
			if(pivot == null)
				return;
			string state = pivot.FixedRowHeaders ? HorizontalScrollFixedStateName : HorizontalScrollUnfixStateName;
#if !SL
			if(ScrollBarMode == Core.ScrollBarMode.TouchOverlap)
				state = state + TouchScrollStateName;
#endif
			GoToState(state);
		}
		protected virtual void GoToState(string state) {
#if !SL
			ScrollViewer Scroller = this;
#else
			if(Scroller == null)
				return;
#endif
			VisualStateManager.GoToState(Scroller, state, false);
		}
#if SL
		public static RoutedEventHandler GetUpdateScrollStateDelegate() {
			return delegate(object d, RoutedEventArgs e) {
				 ((PivotGridScroller)((FrameworkElement)d).Parent).EnsureVisualState();
			};
		}
#endif
	}
	public interface IScrollPartInitializer {
		void OnScrollPartChanged(PivotGridControl pivot, DependencyObject element);
	}
	public class ScrollerCellsInitializer : IScrollPartInitializer {
		public ScrollerCellsInitializer() { }
		void IScrollPartInitializer.OnScrollPartChanged(PivotGridControl pivot, DependencyObject element) {
			pivot.PivotGridScroller.Cells = (ScrollableCellsAreaPresenter)element;
			pivot.PivotGridScroller.SynchronizeScrolling();
#if SL
			pivot.PivotGridScroller.Scroller.Loaded += PivotGridScroller.GetUpdateScrollStateDelegate();
#endif
		}
	}
	public class ScrollerRowValuesInitializer : IScrollPartInitializer {
		public ScrollerRowValuesInitializer() { }
		void IScrollPartInitializer.OnScrollPartChanged(PivotGridControl pivot, DependencyObject element) {
			pivot.PivotGridScroller.RowValues = (FieldValuesPresenter)element;
			pivot.PivotGridScroller.SynchronizeScrolling();
		}
	}
	public class ScrollerColumnValuesInitializer : IScrollPartInitializer {
		public ScrollerColumnValuesInitializer() { }
		void IScrollPartInitializer.OnScrollPartChanged(PivotGridControl pivot, DependencyObject element) {
			pivot.PivotGridScroller.ColumnValues = (FieldValuesPresenter)element;
			pivot.PivotGridScroller.SynchronizeScrolling();
		}
	}
	public class ScrollerCellsDecoratorInitializer : IScrollPartInitializer {
		public ScrollerCellsDecoratorInitializer() { }
		void IScrollPartInitializer.OnScrollPartChanged(PivotGridControl pivot, DependencyObject element) {
			BestFitDecorator decorator = (BestFitDecorator)element;
			pivot.Data.BestWidthCalculator.CellsDecorator = decorator;
			pivot.Data.BestHeightCalculator.CellsDecorator = decorator;
		}
	}
	public class ScrollerColumnDecoratorInitializer : IScrollPartInitializer {
		public ScrollerColumnDecoratorInitializer() { }
		void IScrollPartInitializer.OnScrollPartChanged(PivotGridControl pivot, DependencyObject element) {
			BestFitDecorator decorator = (BestFitDecorator)element;
			pivot.Data.BestWidthCalculator.ColumnValuesDecorator = decorator;
			pivot.Data.BestHeightCalculator.ColumnValuesDecorator = decorator;
		}
	}
	public class ScrollerRowDecoratorInitializer : IScrollPartInitializer {
		public ScrollerRowDecoratorInitializer() { }
		void IScrollPartInitializer.OnScrollPartChanged(PivotGridControl pivot, DependencyObject element) {
			BestFitDecorator decorator = (BestFitDecorator)element;
			pivot.Data.BestWidthCalculator.RowValuesDecorator = decorator;
			pivot.Data.BestHeightCalculator.RowValuesDecorator = decorator;
		}
	}
	public class ScrollerResizerInitializer : IScrollPartInitializer {
		public ScrollerResizerInitializer() { }
		void IScrollPartInitializer.OnScrollPartChanged(PivotGridControl pivot, DependencyObject element) {
			pivot.PivotGridScroller.Resizer = (FrameworkElement)element;
		}
	}
	public class ScrollerFocusAdornerInitializer : IScrollPartInitializer {
		public ScrollerFocusAdornerInitializer() { }
		void IScrollPartInitializer.OnScrollPartChanged(PivotGridControl pivot, DependencyObject element) {
			pivot.PivotGridScroller.FocusAdorner = (FrameworkElement)element;
		}
	}
#if SL
	public class IPopupContainerInitializer : IScrollPartInitializer {
		public IPopupContainerInitializer() { }
		void IScrollPartInitializer.OnScrollPartChanged(PivotGridControl pivot, DependencyObject element) {
			((IPopupContainer)pivot).PopupContainer = (Decorator)element;
		}
	}
#endif
}
