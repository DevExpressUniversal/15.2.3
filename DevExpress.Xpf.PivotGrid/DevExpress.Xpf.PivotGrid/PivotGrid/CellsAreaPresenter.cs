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
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public abstract class CellsAreaPresenter : ScrollableAreaPresenter {
		static internal readonly string DragBorderName = "DragBorder";
		#region static stuff
		public static readonly RoutedEvent LeftTopChangedEvent;
		public static readonly DependencyProperty RowsLeftProperty;
		public static readonly DependencyProperty RowsLeftOffsetProperty;
		static CellsAreaPresenter() {
			Type ownerType = typeof(CellsAreaPresenter);
			LeftTopChangedEvent = EventManager.RegisterRoutedEvent("LeftTopChanged", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			RowsLeftProperty = DependencyPropertyManager.Register("RowsLeft", typeof(int), ownerType, new PropertyMetadata(0, (d, e) => ((CellsAreaPresenter)d).OnLeftTopChanged()));
			RowsLeftOffsetProperty = DependencyPropertyManager.Register("RowsLeftOffset", typeof(double), ownerType, new PropertyMetadata(0d, (d, e) => ((CellsAreaPresenter)d).OnLeftTopChanged()));
		}
		#endregion
		FocusedCellAdorner focusAdorner;
		public CellsAreaPresenter() : base() {
			this.focusAdorner = new FocusedCellAdorner();
		}
		public FocusedCellAdorner FocusAdorner {
			get { return focusAdorner; }
		}
		public int RowsLeft {
			get { return (int)GetValue(RowsLeftProperty); }
			set { SetValue(RowsLeftProperty, value); }
		}
		public double RowsLeftOffset {
			get { return (double)GetValue(RowsLeftOffsetProperty); }
			set { SetValue(RowsLeftOffsetProperty, value); }
		}
		public event RoutedEventHandler LeftTopChanged {
			add { this.AddHandler(LeftTopChangedEvent, value); }
			remove { this.RemoveHandler(LeftTopChangedEvent, value); }
		}
		public override ScrollingType ScrollingType {
			get { return ScrollingType.Both; }
		}
		protected override Size ArrangeOverride(Size finalSize) {
			Size size = base.ArrangeOverride(finalSize);
			if(FocusAdorner != null)
				FocusAdorner.UpdateBounds();
			return size;
		}
		protected override int GetWidthCore(int index) {
			return VisualItems != null ? VisualItems.GetItemWidth(index, true) : PivotGridField.DefaultWidth;
		}
		protected override int GetHeightCore(int level) {
			return VisualItems != null ? VisualItems.GetItemHeight(level, false) : PivotGridField.DefaultHeight;
		}
		protected override ScrollableItemsList UpdateItems() {
			EnsureVisualItems();
			return new CellsItemsList(VisualItems);
		}
		protected internal void EnsureTopLeft(double width, double height) {
			if((Left > 0 || LeftOffset > 0 || RowsLeft > 0 || RowsLeftOffset > 0) && ActualWidth > VisualItems.GetWidthDifference(Left, VisualItems.ColumnCount, true) - LeftOffset)
				this.CoerceValue(LeftProperty);
			if(Top > 0 && ActualHeight > VisualItems.GetHeightDifference(Top, MaxLevel + 1, false) - TopOffset)
				this.CoerceValue(TopProperty);
		}
		protected internal override void EnsureCellsCount(double actualWidth, double actualHeight) {
			base.EnsureCellsCount(actualWidth, actualHeight);
			UpdateSelected();
			UpdateFocused();
			UpdateConditionalAppearance();
		}
		protected internal override Size GetItemSize(ScrollableAreaItemBase item) {
			Size res = new Size();
			res.Height = VisualItems.GetHeightDifference(item.MinLevel, item.MaxLevel + 1, false);
			res.Width = VisualItems.GetWidthDifference(item.MinIndex, item.MaxIndex + 1, true);
			return res;
		}
		protected internal override Rect GetItemRect(ScrollableAreaItemBase item) {
			return
				new Rect(new Point(
					VisualItems.GetWidthDifference(MeasureLeft, item.MinIndex, true) - MeasureLeftOffset,
					VisualItems.GetHeightDifference(MeasureTop, item.MinLevel, false) - MeasureTopOffset),
					GetItemSize(item));
		}
		protected override bool OnReceiveWeakEvent(Type managerType, EventArgs e) {
			bool res = base.OnReceiveWeakEvent(managerType, e);
			if(res)
				return res;
			if(managerType == typeof(VisualItemsSelectionChangedEventManager)) {
				UpdateSelected();
				UpdateConditionalAppearance();
				return true;
			}
			if(managerType == typeof(VisualItemsFocusedCellChangedEventManager)) {
				UpdateFocused();
				UpdateConditionalAppearance();
				return true;
			}
			return false;
		}
		protected void UpdateSelected() {
			VisualItems.CorrectSelection();
			UpdateChildren(delegate(UIElement cell, CellsAreaItem item) {
				item.IsSelected = VisualItems.IsCellSelected(item.ColumnIndex, item.RowIndex);
			});
		}
		protected void UpdateFocused() {
			UIElement focusedElement = null;
			UpdateChildren(delegate(UIElement cell, CellsAreaItem item) {
				bool focused = VisualItems.IsCellFocused(item.ColumnIndex, item.RowIndex);
				item.IsFocused = focused;
				if(focused)
					focusedElement = cell;
			});
			bool pivotFocused = PivotGrid.IsFocused;
#if SL
			pivotFocused |= PivotGrid.IsFocused || LayoutHelper.IsChildElement(PivotGrid, FocusManager.GetFocusedElement() as DependencyObject);
#endif
			if(PivotGrid == null || !pivotFocused || !PivotGrid.DrawFocusedCellRect)
				FocusAdorner.ClearAdorner();
			else
				FocusAdorner.SetAdornerRect(focusedElement as ScrollableAreaCell, PivotGrid);
		}
		protected void UpdateConditionalAppearance() {
			UpdateChildren((cell, item) => {
				((CellElement)cell).UpdatePaint();
			});
		}
		delegate void UpdateCell(UIElement cell, CellsAreaItem item);
		void UpdateChildren(UpdateCell updateMethod) {
			UIElementCollection items = Children;
			int count = items.Count;
			for(int i = 0; i < count; i++) {
				FrameworkElement child = items[i] as FrameworkElement;
				if(child.Visibility == Visibility.Collapsed)
					continue;
				CellsAreaItem item = ((ScrollableAreaCell)child).ValueItem as CellsAreaItem;
				if(item == null)
					continue;
				updateMethod(child, item);
			}
		}
#if !SL
		Timer timer;
		bool IsDoubleTap() {
			if(timer == null) {
				timer = new Timer();
				timer.Interval = 700;
				timer.Elapsed += timer_Elapsed;
				timer.Start();
				return false;
			} else
				return true;
		}
		void timer_Elapsed(object sender, ElapsedEventArgs e) {
			if(timer != null)
				timer.Elapsed -= timer_Elapsed;
			timer = null;
		}
		protected override void OnPreviewTouchDown(TouchEventArgs e) {
			base.OnPreviewTouchDown(e);
			if(IsDoubleTap() && PivotGrid != null) {
				ScrollableAreaCell cell = GetItemBySourceOrCoord(e.Source, e.GetTouchPoint(this).Position, false, false, false);
				CellsAreaItem item = null;
				if(cell != null)
					item = cell.ValueItem as CellsAreaItem;
				if(item != null && item.Item != null)
					PivotGrid.RaiseCellDblClick(cell, item.Item, MouseButton.Left);
			}
		}
#endif
	}
	public class FocusedCellAdorner {
		WeakReference adornedElement;
		WeakReference pivotGrid;
		public FocusedCellAdorner() {
		}
		ScrollableAreaCell Element {
			get {
				return adornedElement == null || !adornedElement.IsAlive ? null : adornedElement.Target as ScrollableAreaCell;
			}
			set {
				adornedElement = new WeakReference(value);
			}
		}
		PivotGridControl PivotGrid {
			get { return pivotGrid == null ? null : pivotGrid.Target as PivotGridControl; }
			set { pivotGrid = new WeakReference(value); }
		}
#if DEBUGTEST
		public bool? IsEmpty(PivotGridControl pivotGrid) {
			if(IsArgsNull(pivotGrid))
				return null;
			return pivotGrid.PivotGridScroller.FocusAdorner.Visibility == Visibility.Collapsed;
		}
#endif
		public void SetAdornerRect(ScrollableAreaCell element, PivotGridControl pivotGrid) {
			Element = element;
			PivotGrid = pivotGrid;
			if(element == null)
				return;
			UpdateBounds(true);
		}
		public void UpdateBounds(bool makeVisible = false) {
			if(Element == null) {
				ClearAdorner();
				return;
			}
			if(IsArgsNull(PivotGrid))
				return;
			FrameworkElement focusAdorner = PivotGrid.PivotGridScroller.FocusAdorner;
			if(!makeVisible && focusAdorner.Visibility == Visibility.Collapsed)
				return;
			ScrollableAreaCell cell = Element;
			if(!LayoutHelper.IsChildElement(PivotGrid.PivotGridScroller, Element))
				return;
			Rect rect2 = LayoutHelper.GetRelativeElementRect(Element, PivotGrid.PivotGridScroller);
			Rect cellsRect = LayoutHelper.GetRelativeElementRect(PivotGrid.PivotGridScroller.Cells, PivotGrid.PivotGridScroller);
			if(rect2.X < cellsRect.X) {
				rect2.Width = Math.Max(0, rect2.Width - cellsRect.X + rect2.X);
				rect2.X = cellsRect.X;
			}
			if(rect2.Y < cellsRect.Y) {
				rect2.Height = Math.Max(0, rect2.Height - cellsRect.Y + rect2.Y);
				rect2.Y = cellsRect.Y;
			}
			rect2.Width = Math.Max(rect2.Width - cell.Border.Left - cell.Border.Right, 0);
			rect2.Height = Math.Max(rect2.Height - cell.Border.Top - cell.Border.Bottom, 0);
			rect2.X += cell.Border.Left;
			rect2.Y += cell.Border.Top;
			rect2.Height = Math.Min(rect2.Height, Math.Max(0, cellsRect.Bottom - rect2.Y));
			rect2.Width = Math.Min(rect2.Width, Math.Max(0, cellsRect.Right - rect2.X));
			if(IsRectActual(focusAdorner, rect2))
				return;
			ClearAdorner();
			focusAdorner.Margin = new Thickness(rect2.X, rect2.Y, 0, 0);
			focusAdorner.Width = rect2.Width;
			focusAdorner.Height = rect2.Height;
			focusAdorner.Visibility = Visibility.Visible;
		}
		public void ClearAdorner() {
			if(IsArgsNull(PivotGrid))
				return;
			PivotGrid.PivotGridScroller.FocusAdorner.Visibility = Visibility.Collapsed;
		}
		bool IsRectActual(FrameworkElement focusAdorner, Rect rect2) {
			return rect2.Width == focusAdorner.Width && rect2.Height == focusAdorner.Height && rect2.X == focusAdorner.Margin.Left && rect2.Y == focusAdorner.Margin.Top && focusAdorner.Visibility == Visibility.Visible;
		}
		bool IsArgsNull(PivotGridControl pivotGrid) {
			return pivotGrid == null || pivotGrid.PivotGridScroller == null || pivotGrid.PivotGridScroller.FocusAdorner == null || pivotGrid.PivotGridScroller.Cells == null;
		}
	}
}
