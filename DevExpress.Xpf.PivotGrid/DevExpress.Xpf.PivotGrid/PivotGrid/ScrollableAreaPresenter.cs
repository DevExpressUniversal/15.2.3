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
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Xpf.Core.Native;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Xpf.Core;
#if SL
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DevExpress.Xpf.Core.WPFCompatibility;
using VisualTreeHelper = DevExpress.Xpf.Core.HitTestHelper;
using Visual = System.Windows.UIElement;
using DevExpress.Xpf.Editors;
#else
using DevExpress.Xpf.Utils;
using DependencyPropertyManager = System.Windows.DependencyProperty;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	[Flags]
	public enum ScrollingType {
		Vertical = 0x01,
		Horizontal = 0x02,
		Both = Vertical | Horizontal,
	}
	[ContentProperty()]
	public abstract class ScrollableAreaPresenter : Panel, IWeakEventListener
#if SL
		, IPreviewEventsRaiser 
#endif
 {
		#region static stuff
		public static readonly DependencyProperty LeftProperty;
		public static readonly DependencyProperty TopProperty;
		public static readonly DependencyProperty LeftOffsetProperty;
		public static readonly DependencyProperty TopOffsetProperty;
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty IsChildProperty;
		int measureLeft, measureTop;
		double measureLeftOffset, measureTopOffset;
		static ScrollableAreaPresenter() {
			Type ownerType = typeof(ScrollableAreaPresenter);
			LeftProperty = DependencyPropertyManager.Register("Left", typeof(int), ownerType, new UIPropertyMetadata(0, 
																(d, e) => ((ScrollableAreaPresenter)d).OnLeftTopChanged(), (d, e) => ((ScrollableAreaPresenter)d).OnLeftChanging(e)));
			LeftOffsetProperty = DependencyPropertyManager.Register("LeftOffset", typeof(double), ownerType, new UIPropertyMetadata(0d, 
																(d, e) => ((ScrollableAreaPresenter)d).OnLeftTopChanged(), (d, e) => ((ScrollableAreaPresenter)d).OnLeftTopOffsetChanging(e)));
			TopProperty = DependencyPropertyManager.Register("Top", typeof(int), ownerType, new UIPropertyMetadata(0, 
																(d, e) => ((ScrollableAreaPresenter)d).OnLeftTopChanged(), (d, e) => ((ScrollableAreaPresenter)d).OnTopChanging(e)));
			TopOffsetProperty = DependencyPropertyManager.Register("TopOffset", typeof(double), ownerType, new UIPropertyMetadata(0d, 
																(d, e) => ((ScrollableAreaPresenter)d).OnLeftTopChanged(), (d, e) => ((ScrollableAreaPresenter)d).OnLeftTopOffsetChanging(e)));
			ItemTemplateProperty = DependencyPropertyManager.RegisterAttached("ItemTemplate", typeof(ControlTemplate), ownerType,
				new FrameworkPropertyMetadata(null));
			IsChildProperty = DependencyPropertyManager.Register("IsChild", typeof(bool), ownerType,
				new UIPropertyMetadata(false));
		}
		protected virtual object OnLeftChanging(object baseValue) {
			return Math.Max(0, Convert.ToInt32(baseValue));
		}
		protected virtual int LeftCoerce(int newValue) {
			if(newValue < 0)
				newValue = 0;
			return newValue;
		}
		protected virtual object OnTopChanging(object baseValue) {
			return Math.Max(0, Convert.ToInt32(baseValue));
		}
		protected virtual object OnLeftTopOffsetChanging(object baseValue) { 
			return baseValue;
		}
		public static ControlTemplate GetItemTemplate(DependencyObject d) {
			if(d == null)
				throw new ArgumentNullException("d");
			return (ControlTemplate)d.GetValue(ItemTemplateProperty);
		}
		public static void SetItemTemplate(DependencyObject d, ControlTemplate value) {
			if(d == null)
				throw new ArgumentNullException("d");
			d.SetValue(ItemTemplateProperty, value);
		}
		#endregion
		ScrollableItemsList items;
		WeakReference dataReference;
		WeakReference visualItemsReference;
		internal bool areItemsValid;
		protected ScrollableAreaPresenter() {
			Loaded += OnLoaded;
			Unloaded += OnUnloaded;
			SizeChanged += OnSizeChanged;
#if !SL
			ClipToBounds = true;
#else
			FrameworkElementExtensions.SetIsClipped(this, true);
			LostMouseCapture += OnLostMouseCapture;
#endif
		}
		protected new UIElementCollection Children { get { return base.Children; } }
		protected internal virtual PivotGridWpfData Data {
			get {
				if(dataReference == null) {
					PivotGridWpfData data = PivotGridControl.GetDataFromAttachedPivot(this);
					if(data == null)
						return null;
					dataReference = new WeakReference(data);
				}
				return (PivotGridWpfData)dataReference.Target;
			}
		}
		protected PivotVisualItems VisualItems {
			get {
				if(visualItemsReference == null) {
					if(Data == null)
						return null;
					visualItemsReference = new WeakReference(Data.VisualItems);
				}
				return (PivotVisualItems)visualItemsReference.Target;
			}
		}
		protected int MeasureLeft {
			get { return measureLeft; }
			set { measureLeft = value; }
		}
		protected int MeasureTop {
			get { return measureTop; }
			set { measureTop = value; }
		}
		protected double MeasureLeftOffset {
			get { return measureLeftOffset; }
			set { measureLeftOffset = value; }
		}
		protected double MeasureTopOffset {
			get { return measureTopOffset; }
			set { measureTopOffset = value; }
		}				  
		protected PivotGridControl PivotGrid {
			get { return Data != null ? Data.PivotGrid : null; }
		}
		public ControlTemplate ItemTemplate {
			get { return (ControlTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public abstract ScrollingType ScrollingType { get; }
		public ScrollableItemsList Items {
			get {
				EnsureItems();
				return items;
			}
		}
		public int MaxLevel { get { return Items == null ? 0 : Items.MaxLevel; } }
		public int MaxIndex { get { return Items == null ? 0 : Items.MaxIndex; } }
		public int Left {
			get { return (int)GetValue(LeftProperty); }
			set { SetValue(LeftProperty, value); }
		}
		public double LeftOffset {
			get { return (double)GetValue(LeftOffsetProperty); }
			set { SetValue(LeftOffsetProperty, value); }
		}		
		public int Top {
			get { return (int)GetValue(TopProperty); }
			set { SetValue(TopProperty, value); }
		}
		public double TopOffset {
			get { return (double)GetValue(TopOffsetProperty); }
			set { SetValue(TopOffsetProperty, value); ; }
		}
		public bool IsChild {
			get { return (bool)GetValue(IsChildProperty); }
			set { SetValue(IsChildProperty, value); }
		}
		protected void SetItems(ScrollableItemsList value) {
			if(ListsEqual(items, value))
				return;
			items = value;
			ResetChildren();
			OnItemsChanged();
		}
		internal static bool ListsEqual(ScrollableItemsList items, ScrollableItemsList values) {
			if(items == null || values == null)
				return false;
			if(items.Count != values.Count || items.ColumnCount != values.ColumnCount || items.RowCount != values.RowCount)
				return false;
			for(int i = 0; i < items.Count; i++) {
				object item = items.GetRawValue(i);
				object value = values.GetRawValue(i);
				if(item == null || item == value)
					continue;
				return false;
			}
			return items.Count != 0;
		}
		protected int GetRowIndex(int index) {
			return Items.GetRowIndex(index);
		}
		protected bool IsCellValid(int columnIndex, int rowIndex) {
			return columnIndex >= 0 && columnIndex <= MaxIndex &&
				rowIndex >= 0 && rowIndex <= MaxLevel;
		}
		protected internal void EnsureItems() {
			if(areItemsValid || Data == null || VisualItems == null)
				return;
			SetItemsValid(true);
			EnsureVisualItems();
			SetItems(UpdateItems());
		}
		protected void ResetItems() {
			SetItemsValid(false);
		}
		protected virtual void SetItemsValid(bool isValid) {
			areItemsValid = isValid;
		}
		protected virtual void OnItemsChanged() { }
		protected abstract int GetWidthCore(int index);
		protected abstract int GetHeightCore(int level);
		protected internal int GetWidth(int index) {
			EnsureVisualItems();
			return GetWidthCore(index);
		}
		protected internal int GetHeight(int level) {
			EnsureVisualItems();
			return GetHeightCore(level);
		}
		protected abstract ScrollableItemsList UpdateItems();
		protected abstract ScrollableAreaCell CreateCell();
		public virtual int GetTotalWidth() {
			EnsureVisualItems();
			return VisualItems.GetWidthDifference(0, MaxIndex + 1, true);
		}
		public virtual int GetTotalHeight() {
			EnsureVisualItems();
			return VisualItems.GetHeightDifference(0, MaxLevel + 1, false);
		}
		public System.Drawing.Point GetCellIndexByCoord(double x, double y) {
			int columnIndex = GetColumnIndexByCoord(x),
				rowIndex = GetRowIndexByCoord(y);
			return new System.Drawing.Point(columnIndex, rowIndex);
		}
		public int GetColumnIndexByCoord(double x) {
			int start = 0;
			for(int i = Left; i <= MaxIndex; i++) {
				int width = GetWidth(i);
				if(x >= start && x < start + width)
					return i;
				start += width;
			}
			return -1;
		}
		public int GetRowIndexByCoord(double y) {
			int start = 0;
			for(int i = Top; i <= MaxLevel; i++) {
				int height = GetHeight(i);
				if(y >= start && y < start + height)
					return i;
				start += height;
			}
			return -1;
		}
		public ScrollableAreaItemBase GetItemByCoord(int index, int level) {
			for(int i = 0; i < Items.Count; i++) {
				ScrollableAreaItemBase item = Items[i];
				if(item.MinIndex == index && item.MinLevel == level)
					return item;
			}
			return null;
		}
		protected void EnsureVisualItems() {
			if(VisualItems != null)
				VisualItems.EnsureIsCalculated();
		}
		protected internal abstract Size GetItemSize(ScrollableAreaItemBase item);
		protected internal abstract Rect GetItemRect(ScrollableAreaItemBase item);
		protected override Size MeasureOverride(Size availableSize) {
			if(Data == null || VisualItems == null)
				return new Size(0, 0);
			EnsureVisualItems();
			EnsureItems();
			if(areItemsValid)
				MeasureCells();
			if(double.IsInfinity(availableSize.Width) || PivotGrid != null && PivotGrid.HorizontalAlignment != HorizontalAlignment.Stretch && GetTotalWidth() < availableSize.Width) {
				if(Items.Count == 0)
					availableSize.Width = 0;
				else if(double.IsInfinity(Width) || double.IsNaN(Width))
					availableSize.Width = Math.Max(0, GetTotalWidth());
				else
					availableSize.Width = Width;
			}
			if(double.IsInfinity(availableSize.Height) || PivotGrid != null && PivotGrid.VerticalAlignment != VerticalAlignment.Stretch && GetTotalHeight() < availableSize.Height) {
				if(Items.Count == 0)
					availableSize.Height = 0;
				else if(double.IsInfinity(Height) || double.IsNaN(Height))
					availableSize.Height = Math.Max(0, GetTotalHeight());
				else
					availableSize.Height = Height;
			}
			return availableSize;
		}
		protected virtual void MeasureCells() {
			foreach(ScrollableAreaCell cell in Children) {
				Size cellSize = cell.Visibility == Visibility.Collapsed || cell.ValueItem == null ?
					Size.Empty : GetItemSize(cell.ValueItem);
				cell.Measure(cellSize);
			}
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(!areItemsValid)
				return finalSize;
			foreach(ScrollableAreaCell cell in Children) {
				Rect cellRect = cell.Visibility == Visibility.Collapsed || cell.ValueItem == null ?
					new Rect(new Point(0, 0), new Size(0, 0)) : GetItemRect(cell.ValueItem);
				cell.Arrange(cellRect);
			}
			return finalSize;
		}
#if SL
		bool IsLoaded;
#endif
		protected virtual void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			InvalidateMeasure();
#if SL
			IsLoaded = true;
#endif
			InitItemsAndEvents();
		}
#if DEBUGTEST
		protected
#endif
 bool itemsEnsured;
		bool eventsSubscribed;
		protected void InitItemsAndEvents() {
			if(!eventsSubscribed) {
				ResetItems();
				SubscribeEvents();
				EnsureItems();
			}
			InvalidateMeasure();
			if(!itemsEnsured)
				EnsureCellsCount(ActualWidth, ActualHeight);
		}
		protected virtual void OnUnloaded(object sender, System.Windows.RoutedEventArgs e) {
#if SL
			IsLoaded = false;
#else
			RenderSize = new Size(0, 0);
#endif
			UnsubscribeEvents();
			ClearChildren();
		}
		protected virtual void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e) {
			if(e.NewSize.Height == 0 && e.NewSize.Width == 0 || Data == null || VisualItems == null)
				return;
			if(!areItemsValid && !eventsSubscribed) {
				InitItemsAndEvents();
			} else {
				if(DevExpress.Xpf.Core.LayoutUpdatedHelper.GlobalLocker.IsLocked)
					return;
				EnsureCellsCount(ActualWidth, ActualHeight);
				InvalidateMeasure();
			}
		}
		void OnLayoutUpdated(object sender, EventArgs e) {
			EnsureItems();
		}
		Locker updateItemsLocker = new Locker();
		protected internal virtual void EnsureCellsCount(double actualWidth, double actualHeight) {
			if(!IsLoaded) {
				itemsEnsured = false;
				return;
			}
			itemsEnsured = true;
			if(updateItemsLocker.IsLocked)
				return;
			updateItemsLocker.Lock();
			EnsureItems();
			System.Drawing.Rectangle visibleRect = GetVisibleRect(actualWidth, actualHeight);
			Dictionary<ScrollableAreaItemBase, ScrollableAreaCell> visibleItems;
			List<ScrollableAreaCell> unusedItems;
			SpreadItems(GetExistingCells(), visibleRect, out visibleItems, out unusedItems);
			if(Items.Count > 0) {
				for(int i = GetStartIndex(visibleRect); i < Items.Count; i++) {
					ScrollableAreaItemBase item = Items[i];
					if(!item.InRect(visibleRect)) {
						if(item.MinLevel >= visibleRect.Bottom)
							break;
						if(item.MinIndex >= visibleRect.Right) {
							if(item.MinLevel < MaxLevel) {
								int nextRow = GetRowIndex(item.MinLevel + 1);
								i = nextRow - 1;
							} else
								break;
						}
						continue;
					}
					ScrollableAreaCell cell = GetCell(item, visibleItems, unusedItems);
				}
			}
			for(int i = 0; i < unusedItems.Count; i++) {
				unusedItems[i].Visibility = Visibility.Collapsed;
			}
			foreach(ScrollableAreaCell cell in Children) {
				UpdateBorders(cell, visibleRect);
			}
			InvalidateArrange();
			updateItemsLocker.Unlock();
		}
		int GetStartIndex(System.Drawing.Rectangle visibleRect) {
			int topRowIndex = visibleRect.Top;
			int startIndex = GetRowIndex(topRowIndex);
			if(startIndex >= Items.Count)
				return startIndex;
			while(Items[startIndex].MinIndex > visibleRect.Left)
				startIndex = GetRowIndex(--topRowIndex);
			return startIndex;
		}
		protected System.Drawing.Rectangle GetVisibleRect(double actualWidth, double actualHeight) {
			return GetVisibleRect(actualWidth, actualHeight, true);
		}
		protected virtual System.Drawing.Rectangle GetVisibleRect(double actualWidth, double actualHeight, bool allowPartiallyVisible) {
			System.Drawing.Rectangle res = new System.Drawing.Rectangle();
			res.Location = new System.Drawing.Point(Left, Top);
			if(actualHeight != 0 || actualWidth != 0) {
				res.Width = GetViewPortWidth(actualWidth, allowPartiallyVisible);
				res.Height = GetViewPortHeight(actualHeight, allowPartiallyVisible);
			}
			return res;
		}
		protected internal virtual int GetViewPortWidth(double actualWidth, bool allowPartiallyVisible) {
			return GetViewPortWidthCore(actualWidth, allowPartiallyVisible);
		}
		internal int GetViewPortWidthCore(double actualWidth, bool allowPartiallyVisible) {
			double unusedWidth = actualWidth + LeftOffset;
			int res = 0;
			for(int i = Left; i <= MaxIndex && unusedWidth > 0; i++) {
				unusedWidth -= GetWidth(i);
				res++;
			}
			if(!allowPartiallyVisible && unusedWidth < 0)
				res--;
			return Math.Max(res, 1);
		}
		protected internal int GetViewPortHeight(double actualHeight, bool allowPartiallyVisible) {
			double unusedHeight = actualHeight + TopOffset;
			int res = 0;
			for(int i = Top; i <= MaxLevel && unusedHeight > 0; i++) {
				unusedHeight -= GetHeight(i);
				res++;
			}
			if(!allowPartiallyVisible && unusedHeight < 0)
				res--;
			return Math.Max(res, 1);
		}
		protected ScrollableAreaCell GetCell(ScrollableAreaItemBase item, Dictionary<ScrollableAreaItemBase, ScrollableAreaCell> visibleItems, List<ScrollableAreaCell> unusedItems) {
			ScrollableAreaCell res;
			if(visibleItems.TryGetValue(item, out res))
				return res;
			if(unusedItems.Count > 0) {
				res = unusedItems[unusedItems.Count - 1];
				unusedItems.RemoveAt(unusedItems.Count - 1);
				res.Visibility = Visibility.Visible;
				res.ValueItem = item;
				return res;
			}
			res = CreateCell();
			res.ValueItem = item;
			Children.Add(res);
			return res;
		}
		internal void UpdateCellBorders(ScrollableAreaCell cell) {
			UpdateBorders(cell, GetVisibleRect(ActualWidth, ActualHeight));
		}
		protected virtual void UpdateBorders(ScrollableAreaCell cell, System.Drawing.Rectangle visibleRect) {
			ScrollableAreaItemBase item = cell.ValueItem;
			if(cell.Visibility != System.Windows.Visibility.Visible || item == null)
				return;
			UpdateBordersCore(cell, visibleRect);
		}
		protected void UpdateBordersCore(IScrollableAreaCell cell, System.Drawing.Rectangle visibleRect) {
			cell.IsLeftMost = cell.MinIndex <= visibleRect.Left;
			cell.IsTopMost = cell.MinLevel <= visibleRect.Top;
			cell.IsRightMost = cell.MaxIndex == MaxIndex;
			cell.IsBottomMost = cell.MaxLevel == MaxLevel;
			cell.Border = GetBorder(cell);
		}
		protected virtual Thickness GetBorder(IScrollableAreaCell cell) {
			Thickness border = new Thickness(1, 1, 0, 0);
			if(cell.IsTopMost)
				border.Top = 0;
			if(cell.IsLeftMost)
				border.Left = 0;
			if(cell.IsBottomMost)
				border.Bottom = 1;
			if(cell.IsRightMost)
				border.Right = 1;
			return border;
		}
		void SpreadItems(List<ScrollableAreaCell> existingCells, System.Drawing.Rectangle visibleRect,
				out Dictionary<ScrollableAreaItemBase, ScrollableAreaCell> visibleItems, out List<ScrollableAreaCell> unusedItems) {
			visibleItems = new Dictionary<ScrollableAreaItemBase, ScrollableAreaCell>();
			unusedItems = new List<ScrollableAreaCell>();
			foreach(ScrollableAreaCell cell in existingCells) {
				ScrollableAreaItemBase item = cell.ValueItem;
				if(cell.Visibility == Visibility.Collapsed || item == null || !item.InRect(visibleRect))
					unusedItems.Add(cell);
				else
					visibleItems.Add(item, cell);
			}
		}
		List<ScrollableAreaCell> GetExistingCells() {
			List<ScrollableAreaCell> res = new List<ScrollableAreaCell>(Children.Count);
			foreach(ScrollableAreaCell item in Children)
				res.Add(item);
			return res;
		}
		protected virtual void ResetChildren() {
			foreach(ScrollableAreaCell item in Children) {
				item.Visibility = System.Windows.Visibility.Collapsed;
			}
			EnsureCellsCount(ActualWidth, ActualHeight);
		}
		protected virtual void SubscribeEvents() {
			eventsSubscribed = true;
			if(VisualItems == null)
				throw new NullReferenceException("VisualItems");
			if(Data == null)
				throw new NullReferenceException("Data");
			if(PivotGrid != null)
				PivotGrid.BrushChanged += OnPivotBrushesChanged;
			LayoutUpdated += OnLayoutUpdated;
			Data.FieldSizeChanged += OnFieldSizeChanged;
			Data.LayoutChangedEvent += OnDataLayoutChanged;
			VisualItems.Cleared += OnVisualItemsCleared;
#if SL
			MouseLeftButtonDown += OnMouseLeftButtonDown;
			MouseLeftButtonUp += OnMouseLeftButtonUp;
			MouseMove += OnMouseMove;
#endif
		}
		protected ScrollableAreaItemBase GetValueItemBySourceOrCoord(object source, Point position, bool correct, bool isColumnW, bool isColumnH) {
			ScrollableAreaCell item = GetItemBySourceOrCoord(source, position, correct, isColumnW, isColumnH);
			return item != null ? item.ValueItem : null;
		}
		ScrollableAreaCell GetItemBySource(object source) {
			ScrollableAreaCell item = null;
			DependencyObject cell = source as DependencyObject;
			if(cell != null)
				item = LayoutHelper.FindParentObject<ScrollableAreaCell>(source as Visual);
			return item;
		}
		protected internal ScrollableAreaCell GetItemBySourceOrCoord(object source, Point position, bool correct, bool isColumnW, bool isColumnH) {
			ScrollableAreaCell item = GetItemBySource(source);
#if !SL
			UIElement lastVisual = null;
#endif
			if(item == null) {
#if SL
				if(correct)
					position = GetCorrectedHitTestPoint(position, isColumnW, isColumnH);
				position = HitTestHelper.TransformPointToRoot(this, position);
				HitTestResult hitTestResult = VisualTreeHelper.HitTest(this, position, false);
				if(hitTestResult != null && hitTestResult.VisualHit != null)
					item = GetItemBySource(hitTestResult.VisualHit);
#else
				VisualTreeHelper.HitTest(this,
					(d) => {
						UIElement visual = d as UIElement;
						if(visual == null)
							return HitTestFilterBehavior.Continue;
						if(visual.Visibility != System.Windows.Visibility.Visible)
							return HitTestFilterBehavior.ContinueSkipSelfAndChildren;
						lastVisual = visual;
						if(visual is ScrollableAreaCell)
							return LayoutHelper.GetRelativeElementRect(visual, this).Contains(position) ? HitTestFilterBehavior.Stop : HitTestFilterBehavior.ContinueSkipSelfAndChildren;
						else
							return HitTestFilterBehavior.Continue;
					}
					,
					(d) => HitTestResultBehavior.Continue,
					new PointHitTestParameters(position));
				if(lastVisual != null)
					item = GetItemBySource(lastVisual);
#endif
			}
			return item;
		}
		Point GetCorrectedHitTestPoint(Point pt, bool isColumnW, bool isColumnH) {
			int width = VisualItems.GetWidthDifference(Left, MaxIndex + 1, isColumnW) - 1;
			int height = VisualItems.GetHeightDifference(Top, MaxLevel + 1, isColumnH) - 1;
			if(pt.X < 0)
				pt.X = 1;
			if(pt.X > width)
				pt.X = width;
			if(pt.Y < 0)
				pt.Y = 1;
			if(pt.Y > height)
				pt.Y = height;
			return pt;
		}
		protected internal virtual void OnMouseLeftButtonUp(object originalSource, Point position) { }
		protected internal virtual void OnMouseMove(object originalSource, Point position) { }
		protected internal virtual void OnMouseLeftButtonDown(object originalSource, Point position) { }
#if SL
		protected bool isLeftMouseButtonPressed;
		protected bool IsMouseCaptured;
		void OnMouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			isLeftMouseButtonPressed = false;
			IsMouseCaptured = false;
			ReleaseMouseCapture();
			OnMouseLeftButtonUp(e.OriginalSource, e.GetPosition(this));
		}
		void OnMouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
			OnMouseMove(e.OriginalSource, e.GetPosition(this));
		}
		void OnMouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			if(e != null && e.ClickCount == 2) return;
			isLeftMouseButtonPressed = true;
			IsMouseCaptured = true;
			if(PivotGrid != null)
				PivotGrid.EnsureFocus();
			CaptureMouse();
			OnMouseLeftButtonDown(e.OriginalSource, e.GetPosition(this));
		}
		void OnLostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e) {
			isLeftMouseButtonPressed = false;
			IsMouseCaptured = false;
		}
#else
		protected override void OnMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			CaptureMouse();
			base.OnMouseLeftButtonDown(e);
			OnMouseLeftButtonDown(GetSourceFromEventArgs(e), e.GetPosition(this));
		}
		protected static Visual GetSourceFromEventArgs(System.Windows.Input.MouseEventArgs e) {
			Visual source = e.OriginalSource as Visual;
			if(source == null)
				source = e.Source as Visual;
			return source;
		}
		protected override void OnMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseLeftButtonUp(e);
			OnMouseLeftButtonUp(GetSourceFromEventArgs(e), e.GetPosition(this));
			ReleaseMouseCapture();
		}
		protected override void OnMouseMove(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseMove(e);
#if SL
			if(!isLeftMouseButtonPressed) return;
#else
			if(e.LeftButton != System.Windows.Input.MouseButtonState.Pressed)
				return;
#endif
			OnMouseMove(GetSourceFromEventArgs(e), e.GetPosition(this));
		}
#endif
		protected virtual void UnsubscribeEvents() {
			eventsSubscribed = false;
			if(VisualItems != null)
				VisualItems.Cleared -= OnVisualItemsCleared;
			if(Data != null) {
				Data.FieldSizeChanged -= OnFieldSizeChanged;
				Data.LayoutChangedEvent -= OnDataLayoutChanged;
			}
			if(PivotGrid != null)
				PivotGrid.BrushChanged -= OnPivotBrushesChanged;
			LayoutUpdated -= OnLayoutUpdated;
#if SL
			MouseLeftButtonDown -= OnMouseLeftButtonDown;
			MouseLeftButtonUp -= OnMouseLeftButtonUp;
			MouseMove -= OnMouseMove;
#endif
		}
		protected virtual void OnPivotBrushesChanged(object sender, PivotBrushChangedEventArgs e) {
			ClearChildren();
		}
		void ClearChildren() {
			ResetItems();
			items = null;
			if(IsLoaded)
				EnsureItems();
		}
		protected virtual void OnVisualItemsCleared(object sender, EventArgs e) {
			ResetItems();
		}
		protected void OnFieldSizeChanged(object sender, FieldSizeChangedEventArgs e) {
			if(e.Field.Area.ToFieldArea() != FieldArea.FilterArea) {
				EnsureCellsCount(ActualWidth, ActualHeight);
				InvalidateMeasure();
			}
		}
		protected virtual void OnDataLayoutChanged(object sender, EventArgs e) {
			EnsureItems();
			InvalidateMeasure();
		}
		protected virtual void OnLeftTopChanged() {
			OnLeftTopChangedCore();
		}
		internal void OnLeftTopChangedCore() {
			MeasureLeft = Left;
			MeasureTop = Top;
			MeasureLeftOffset = LeftOffset;
			MeasureTopOffset = TopOffset;
			EnsureCellsCount(ActualWidth, ActualHeight);
		}
		public void MakeVisible(int columnIndex, int rowIndex) {
			if(!IsCellValid(columnIndex, rowIndex))
				return;
			System.Drawing.Rectangle visibleRect = GetVisibleRect(ActualWidth, ActualHeight, false);
			if(visibleRect.Contains(columnIndex, rowIndex))
				return;
			Left = MakeVisibleCore(columnIndex, visibleRect.Left, visibleRect.Right);
			Top = MakeVisibleCore(rowIndex, visibleRect.Top, visibleRect.Bottom);
		}
		int MakeVisibleCore(int index, int left, int right) {
			if(index < left)
				return index;
			else
				return index - right + left + 1;
		}
		#region IWeakEventListener Members
		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e) {
			return OnReceiveWeakEvent(managerType, e);
		}
		protected virtual bool OnReceiveWeakEvent(Type managerType, EventArgs e) {
			return false;
		}
		#endregion
#if SL
		#region IPreviewEventsRaiser Members
		void IPreviewEventsRaiser.RaisePreviewKeyDown(System.Windows.Input.KeyEventArgs e) {
		}
		void IPreviewEventsRaiser.RaisePreviewKeyUp(System.Windows.Input.KeyEventArgs e) {
		}
		void IPreviewEventsRaiser.RaisePreviewMouseLeftButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			DependencyObject parent = e.OriginalSource as DependencyObject;
			while(parent != null && parent != this) {
				if(parent is IPreviewEventsRaiser && !(parent is TextEdit))
					return;
				parent = LayoutHelper.GetParent(parent);
			}
			OnMouseLeftButtonDown(this, e);
			e.Handled = true;
		}
		void IPreviewEventsRaiser.RaisePreviewMouseLeftButtonUp(System.Windows.Input.MouseButtonEventArgs e) {
			OnMouseLeftButtonUp(this, e);
		}
		void IPreviewEventsRaiser.RaisePreviewMouseWheel(System.Windows.Input.MouseWheelEventArgs e) {
		}
		void IPreviewEventsRaiser.RaisePreviewTextInput(DevExpress.Xpf.Editors.WPFCompatibility.SLTextCompositionEventArgs e) {
		}
		#endregion
#endif
	}
}
