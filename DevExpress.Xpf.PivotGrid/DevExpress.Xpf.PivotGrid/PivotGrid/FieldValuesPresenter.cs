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
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Xpf.Core;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using Thumb = DevExpress.Xpf.Core.DXThumb;
using DevExpress.Xpf.Core.Native;
using System.Windows.Input;
#if SL
using Panel = System.Windows.Controls.Control;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Utils;
using VisualTreeHelper = DevExpress.Xpf.Core.HitTestHelper;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
using DevExpress.Xpf.Utils;
using DevExpress.Utils;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class FieldValuesPresenter : ScrollableAreaPresenter {
		System.Drawing.Rectangle visibleRect;
		#region static stuff
		public static readonly DependencyProperty IsColumnProperty;
		public static readonly DependencyProperty CurrentWidthProperty;
		static FieldValuesPresenter() {
			Type ownerType = typeof(FieldValuesPresenter);
			IsColumnProperty = DependencyPropertyManager.Register("IsColumn", typeof(bool),
				ownerType, new PropertyMetadata(false, OnIsColumnPropertyChanged));
			CurrentWidthProperty = DependencyPropertyManager.Register("CurrentWidth", typeof(double),
				ownerType, new PropertyMetadata(0d));
		}
		static void OnIsColumnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FieldValuesPresenter control = (FieldValuesPresenter)d;
			control.OnIsColumnChanged();
		}
		#endregion
		public FieldValuesPresenter()
			: base() {
			LostMouseCapture += new System.Windows.Input.MouseEventHandler(OnLostMouseCapture);
		}
		protected override void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			base.OnSizeChanged(sender, e);
			CurrentWidth = e.NewSize.Width;
			UpdateVisibleRect();
		}
		void OnLostMouseCapture(object sender, System.Windows.Input.MouseEventArgs e) {
			SelectionScroller.StopScrollTimer();
		}
		public bool IsColumn {
			get { return (bool)GetValue(IsColumnProperty); }
			set { SetValue(IsColumnProperty, value); }
		}
		public double CurrentWidth {
			get { return (double)GetValue(CurrentWidthProperty); }
			set { SetValue(CurrentWidthProperty, value); }
		}
		public override ScrollingType ScrollingType {
			get { return IsColumn ? ScrollingType.Horizontal : ScrollingType.Vertical; }
		}
		protected FieldValueItem GetItem(int index) {
			return (FieldValueItem)Items[index];
		}
		protected override int GetWidthCore(int index) {
			return VisualItems.GetItemWidth(index, IsColumn);
		}
		protected override int GetHeightCore(int level) {
			return VisualItems.GetItemHeight(level, IsColumn);
		}
		protected void BaseOnLeftTopChanged() { base.OnLeftTopChanged(); }
		protected override void OnLeftTopChanged() { }
		protected virtual void OnIsColumnChanged() {
			ResetItems();
		}
		protected internal override void EnsureCellsCount(double actualWidth, double actualHeight) {
			UpdateVisibleRect();
			base.EnsureCellsCount(actualWidth, actualHeight);
			UpdateSelected();
		}
		protected override void SubscribeEvents() {
			base.SubscribeEvents();
			if(VisualItems != null)
				VisualItemsSelectionChangedEventManager.AddListener(VisualItems, this);
			if(PivotGrid != null) {
				PropertyChangedEventManager.AddListener(PivotGrid, this);
			}
		}
		protected override void UnsubscribeEvents() {
			if(VisualItems != null)
				VisualItemsSelectionChangedEventManager.RemoveListener(VisualItems, this);
			if(PivotGrid != null) {
				PropertyChangedEventManager.RemoveListener(PivotGrid, this);
			}
			base.UnsubscribeEvents();
		}
		protected override void OnPivotBrushesChanged(object sender, PivotBrushChangedEventArgs e) {
			if(e.BrushType != PivotBrushType.ValueBrush) return;
			base.OnPivotBrushesChanged(sender, e);
		}
		protected override bool OnReceiveWeakEvent(Type managerType, EventArgs e) {
			bool res = base.OnReceiveWeakEvent(managerType, e);
			if(res)
				return res;
			if(managerType == typeof(VisualItemsSelectionChangedEventManager)) {
				UpdateSelected();
				return true;
			}
			if(managerType == typeof(PropertyChangedEventManager)) {
				PivotPropertyChangedEventArgs args = (PivotPropertyChangedEventArgs)e;
				OnPivotPropertyChanged(args);
				return true;
			}
			return false;
		}
		void OnPivotPropertyChanged(PivotPropertyChangedEventArgs args) {
			if(args.Property == PivotGridControl.AllowResizingProperty)
				UpdateChildren(delegate(UIElement cell, FieldValueItem item) {
					((FieldValueCellElement)cell).UpdateResizers();
				});
			if(args.Property == PivotGridControl.RowTreeOffsetProperty)
				UpdateChildren(delegate(UIElement cell, FieldValueItem item) {
					item.UpdatePadding();
				});
		}
		protected override ScrollableAreaCell CreateCell() {
			ScrollableAreaCell cell = new FieldValueCellElement();
			ScrollableAreaPresenter.SetItemTemplate(cell, ItemTemplate);
			return cell;
		}
		protected override ScrollableItemsList UpdateItems() {
			return new FieldValuesItemsList(VisualItems, IsColumn);
		}
		Size measureSize = Size.Empty;
		protected override Size MeasureOverride(Size availableSize) {
			measureSize = availableSize;
			Size res = base.MeasureOverride(availableSize);
			if(!IsColumn && res.Width == 0 && res.Height > 0)
				res.Width = GetTotalWidth();
			if(IsColumn && res.Height == 0 && res.Width > 0)
				res.Height = GetTotalHeight();
			if(!IsColumn && !double.IsInfinity(availableSize.Width) && areItemsValid) {
				if(Items.Count == 0) {
					if(Data == null || PivotGrid == null)
						availableSize.Width = 0;
					else
						if(PivotGrid.RowTotalsLocation == FieldRowTotalsLocation.Tree)
							availableSize.Width = PivotGrid.RowTreeWidth;
						else
							availableSize.Width = PivotGridField.DefaultWidth;
				} else
					availableSize.Width = Math.Max(0, GetTotalWidth() - MeasureLeftOffset);
				if(availableSize.Width < res.Width)
					res.Width = availableSize.Width;
			}
			return res;
		}
		protected void UpdateVisibleRect() {
			visibleRect = GetVisibleRect(RenderSize.Width, RenderSize.Height);
		}
#if !SL
		protected override void OnManipulationCompleted(ManipulationCompletedEventArgs e) {
			isInTouchSelection = false;
			base.OnManipulationCompleted(e);
		}
		protected override void OnManipulationStarting(ManipulationStartingEventArgs e) {
			e.ManipulationContainer = this;
			e.Handled = true;
			base.OnManipulationStarting(e);
		}
		protected override void OnManipulationDelta(ManipulationDeltaEventArgs e) {
			int count = 0;
			List<IManipulator> mans = new List<IManipulator>();
			foreach(IManipulator man in e.Manipulators) {
				mans.Add(man);
				count++;
			}
			if(count == 1) {
				ScrollableAreaCell cell1 = GetItemBySourceOrCoord(null, mans[0].GetPosition(this), false, IsColumn, IsColumn);
				if(cell1 != null) {
					if(!isInTouchSelection) {
						VisualItems.PerformColumnRowSelection(((FieldValueItem)cell1.ValueItem).Item);
					} else {
						VisualItems.StartAddSelection();
						InvertItemSelection(((FieldValueItem)cell1.ValueItem));
					}
				}
			}
			base.OnManipulationDelta(e);
		}
		bool isInTouchSelection;
		protected override void OnPreviewStylusSystemGesture(StylusSystemGestureEventArgs e) {
			if(e.SystemGesture == SystemGesture.HoldEnter && !e.Handled) {
				e.Handled = true;
				DevExpress.Xpf.Bars.BarPopupBase.ShowElementContextMenu(e.Source);
			}
			base.OnPreviewStylusSystemGesture(e);
		}
		protected override void OnStylusDown(StylusDownEventArgs e) {
			FieldValueCellElement item = GetItemBySourceOrCoord(null, e.GetPosition(this), false, IsColumn, IsColumn) as FieldValueCellElement;
			if(item != null) {
				if(!VisualItems.IsValueMouseDown && !isInTouchSelection) {
					VisualItems.StartColumnRowSelection(((FieldValueItem)item.ValueItem).Item,
						new System.Drawing.Point(IsColumn ? GetColumnIndexByCoord(e.GetPosition(this).X) : 0, IsColumn ? 0 : GetRowIndexByCoord(e.GetPosition(this).Y)));
				} else {
					isInTouchSelection = true;
					VisualItems.StopColumnRowSelection();
					VisualItems.StartAddSelection();
					InvertItemSelection(((FieldValueItem)item.ValueItem));
				}
			}
			base.OnStylusDown(e);
		}
		void InvertItemSelection(FieldValueItem item) {
			int x = IsColumn ? item.Item.MinLastLevelIndex : 0;
			int y = IsColumn ? 0 : item.Item.MinLastLevelIndex;
			int width = IsColumn ? item.Item.MaxLastLevelIndex - x + 1 : PivotGrid.ColumnCount;
			int height = IsColumn ? PivotGrid.RowCount : item.Item.MaxLastLevelIndex - y + 1;
			if(!VisualItems.IsFieldValueSelected(item.Item))
				VisualItems.AddSelection(new System.Drawing.Rectangle(x, y, width, height));
			else
				VisualItems.SubtractSelection(new System.Drawing.Rectangle(x, y, width, height));
		}
		protected override void OnStylusSystemGesture(StylusSystemGestureEventArgs e) {
			int correction = 10;
			if(e.SystemGesture == SystemGesture.Tap && !isInTouchSelection) {
				if(!TryExpand(e, 0, 0))
					if(!TryExpand(e, -correction, -correction))
						if(!TryExpand(e, -correction, correction))
							if(!TryExpand(e, correction, -correction))
								if(!TryExpand(e, correction, correction)) { }
			}
			base.OnStylusSystemGesture(e);
		}
		bool TryExpand(StylusSystemGestureEventArgs e, int x, int y) {
			FieldValueExpandButton button = null;
			Point pos = e.GetPosition(this);
			pos.X = Math.Max(0, pos.X + x);
			pos.Y = Math.Max(0, pos.Y + y);
			HitTestResult result = VisualTreeHelper.HitTest(this, pos);
			if(result != null && result.VisualHit != null) {
				button = LayoutHelper.FindParentObject<FieldValueExpandButton>(result.VisualHit as DependencyObject);
			}
			if(button != null) {
				FieldValueCellElement Item = LayoutHelper.FindParentObject<FieldValueCellElement>(result.VisualHit as DependencyObject);
				if(Item != null && ((FieldValueItem)Item.ValueItem).Item.AllowExpand)
					PivotGrid.Commands.ChangeFieldValueExpanded.Execute(Item.Item.Item, PivotGrid);
			}
			return button != null;
		}
		protected override void OnStylusUp(StylusEventArgs e) {
			VisualItems.StopColumnRowSelection();
			base.OnStylusUp(e);
		}
#endif
		protected internal override void OnMouseLeftButtonDown(object originalSource, Point position) {
			base.OnMouseLeftButtonDown(originalSource, position);
			FieldValueItem item = GetValueItemBySourceOrCoord(originalSource, position, true, IsColumn, IsColumn) as FieldValueItem;
			if(item != null) {
				System.Drawing.Point startPoint = System.Drawing.Point.Empty;
				if(IsColumn)
					startPoint.X = GetColumnIndexByCoord(position.X);
				else
					startPoint.Y = GetRowIndexByCoord(position.Y);
				VisualItems.StartColumnRowSelection(item.Item, startPoint);
				CaptureMouse();
			}
		}
		protected internal override void OnMouseLeftButtonUp(object originalSource, Point position) {
			base.OnMouseLeftButtonUp(originalSource, position);
			VisualItems.StopColumnRowSelection();
			if(IsMouseCaptured)
				ReleaseMouseCapture();
		}
		protected internal override void OnMouseMove(object originalSource, Point position) {
			base.OnMouseMove(originalSource, position);
			if(!IsMouseCaptured || !VisualItems.IsValueMouseDown) return;
			Point pt = position;
			FieldValueItem item = GetValueItemBySourceOrCoord(originalSource, position, true, IsColumn, IsColumn) as FieldValueItem;
			if(item != null)
				VisualItems.PerformColumnRowSelection(item.Item);
			SelectionScroller.StartScrollTimer(new System.Drawing.Point((int)(pt.X + .5), (int)(pt.Y + .5)));
		}
		FieldValueSelectionScroller selectionScroller;
#if DEBUGTEST
		internal
#endif
 FieldValueSelectionScroller SelectionScroller {
			get {
				if(selectionScroller == null)
					selectionScroller = new FieldValueSelectionScroller(PivotGrid.PivotGridScroller.Cells, this);
				return selectionScroller;
			}
		}
		protected void UpdateSelected() {
			UpdateChildren(delegate(UIElement cell, FieldValueItem item) {
				item.IsSelected = VisualItems.IsFieldValueSelected(item.Item);
			});
		}
		delegate void UpdateCell(UIElement cell, FieldValueItem item);
		void UpdateChildren(UpdateCell updateMethod) {
			UIElementCollection items = Children;
			int count = items.Count;
			for(int i = 0; i < count; i++) {
				UIElement child = items[i];
				if(child.Visibility != Visibility.Visible) continue;
				FieldValueItem item = ((ScrollableAreaCell)child).ValueItem as FieldValueItem;
				if(item == null) continue;
				updateMethod(child, item);
			}
		}
		protected override Thickness GetBorder(IScrollableAreaCell cell) {
			Thickness border = base.GetBorder(cell);
			if(cell.IsRightMost && !IsColumn)
				border.Right = 0;
			if(cell.IsBottomMost && IsColumn)
				border.Bottom = 0;
			if(PivotGrid != null && PivotGrid.RowTotalsLocation == FieldRowTotalsLocation.Tree && !IsColumn)
				border.Left = 0;
			return border;
		}
		protected internal override Rect GetItemRect(ScrollableAreaItemBase item) {
			Rect res = new Rect();
			Size size = GetItemSize(item);
			res.Width = size.Width;
			res.Height = size.Height;
			if(IsColumn) {
				res.Y = VisualItems.GetHeightDifference(Top, Math.Max(Top, item.MinLevel), IsColumn) - MeasureTopOffset;
				if(size.Width == measureSize.Width && item.MinIndex <= Left && item.MinLevel != MaxLevel) {
					res.X = 0;
					res.Width = Math.Min(res.Width, VisualItems.GetWidthDifference(Left, item.MaxIndex + 1, IsColumn) - MeasureLeftOffset);
				} else {
					res.X = VisualItems.GetWidthDifference(Left, Math.Max(Left, item.MinIndex), IsColumn) - MeasureLeftOffset;
					if(item.MinIndex != item.MaxIndex && item.MinIndex <= Left && item.MaxIndex > Left) {
						res.X += MeasureLeftOffset;
						res.Width -= MeasureLeftOffset;
					}
				}
			} else {
				if(size.Height == measureSize.Height && item.MinLevel <= Top)
					res.Y = 0;
				else
					res.Y = VisualItems.GetHeightDifference(Top, Math.Max(Top, item.MinLevel), IsColumn) - MeasureTopOffset;			   
				res.X = VisualItems.GetWidthDifference(Left, item.MinIndex, IsColumn) - MeasureLeftOffset;
			}
			return res;
		}
		protected internal override Size GetItemSize(ScrollableAreaItemBase item) {
			Size res = new Size();
			res.Height = GetSafeHeight(VisualItems.GetHeightDifference(Math.Max(Top, item.MinLevel), Math.Min(Top + visibleRect.Height, Math.Min(MaxLevel, item.MaxLevel)) + 1, IsColumn));
			if(IsColumn)
				res.Width = GetSafeWidth(VisualItems.GetWidthDifference(Math.Max(Left, item.MinIndex), Math.Min(Left + visibleRect.Width, item.MaxIndex) + 1, IsColumn));
			else
				res.Width = VisualItems.GetWidthDifference(item.MinIndex, item.MaxIndex + 1, IsColumn);
			return res;
		}
		double GetSafeHeight(double size) {
			return size >= 0 ? Math.Min(size, measureSize.Height) : 0;
		}
		double GetSafeWidth(double size) {
			return size >= 0 ? Math.Min(size, measureSize.Width + LeftOffset) : 0;
		}
		public override int GetTotalHeight() {
			EnsureVisualItems();
			return VisualItems.GetHeightDifference(0, MaxLevel + 1, IsColumn);
		}
		public override int GetTotalWidth() {
			EnsureVisualItems();
			return VisualItems.GetWidthDifference(Left, MaxIndex + 1, IsColumn);
		}
	}
	public class FieldValueItem : ScrollableAreaItemBase {
		#region static
		public static readonly DependencyProperty PaddingProperty;
		static FieldValueItem() {
			Type ownerType = typeof(FieldValueItem);
			PaddingProperty = DependencyPropertyManager.Register("Padding", typeof(Thickness), ownerType, new PropertyMetadata(new Thickness(0)));
		}
#if DEBUGTEST
		public static bool GetIsSelected(DependencyObject d) {
			return (bool)((ScrollableAreaCell)d).ValueItem.GetValue(IsSelectedProperty);
		}
		internal static void SetIsSelected(DependencyObject d, bool value) {
			((ScrollableAreaCell)d).ValueItem.IsSelected = value;
		}
#endif
		#endregion
		readonly PivotFieldValueItem item;
		readonly PivotVisualItems visualItems;
		public FieldValueItem(PivotFieldValueItem item, PivotVisualItems visualItems) {
			if(visualItems == null)
				throw new ArgumentNullException("visualItems");
			if(item == null)
				throw new ArgumentNullException("item");
			this.item = item;
			this.visualItems = visualItems;
			UpdateAppearance();
			UpdatePadding();
		}
		public Thickness Padding {
			get { return (Thickness)GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}
		public PivotFieldValueItem Item { get { return item; } }
		public PivotVisualItems VisualItems { get { return visualItems; } }
		public PivotGridControl PivotGrid { get { return VisualItems.Data.PivotGrid; } }
		protected internal PivotGridWpfData Data { get { return VisualItems.Data; } }
		#region IScrollableAreaItem Members
		public override int MinLevel {
			get { return item.IsColumn ? item.StartLevel : item.MinLastLevelIndex; }
		}
		public override int MaxLevel {
			get { return item.IsColumn ? item.EndLevel : item.MaxLastLevelIndex; }
		}
		public override int MinIndex {
			get { return item.IsColumn ? item.MinLastLevelIndex : item.StartLevel; }
		}
		public override int MaxIndex {
			get { return item.IsColumn ? item.MaxLastLevelIndex : item.EndLevel; }
		}
		public override object Value {
			get { return item.Value; }
		}
		public override string DisplayText {
			get { return item.DisplayText; }
		}
		#endregion
		public override PivotGridField Field {
			get { return Data.GetField(item.Field) ?? VisualItems.Data.DataField; }
		}
		public PivotGridField DataField {
			get { return Data.GetField(item.DataField); }
		}
		public FieldSummaryType? CustomTotalSummaryType {
			get { return item.CustomTotal != null ? (FieldSummaryType?)item.CustomTotal.SummaryType.ToFieldSummaryType() : null; }
		}
		public bool CanShowSortBySummary { get { return item.CanShowSortBySummary; } }
		public bool IsOthersRow { get { return item.IsOthersRow; } }
		public bool IsExpanded { get { return !item.IsCollapsed; } }
		public bool IsColumn { get { return item.IsColumn; } }
		public bool IsLastLevelItem { get { return item.IsLastFieldLevel; } }
		public bool IsGrandTotal { get { return Item.ValueType == PivotGridValueType.GrandTotal; } }
		public bool IsSortedBySummary {
			get {
				return VisualItems.GetIsAnyFieldSortedBySummary(IsColumn, item.Index);
			}
		}
		public Visibility ExpandButtonVisibility {
			get { return item.ShowCollapsedButton ? Visibility.Visible : Visibility.Collapsed; }
		}
		public bool IsTotalAppearance { get { return item.IsTotalAppearance(); } }
		public bool IsCustomTotalAppearance { get { return item.ValueType == PivotGridValueType.CustomTotal; } }
		public bool CanExpand { get { return item.ShowCollapsedButton; } }
		public bool IsDataLocatedInThisArea { get { return item.IsDataFieldsVisible; } }
		public bool IsVisible { get { return item.IsVisible; } }
		public int MinLastLevelIndex { get { return item.MinLastLevelIndex; } }
		public int MaxLastLevelIndex { get { return item.MaxLastLevelIndex; } }
		protected bool AllowResizing {
			get { return PivotGrid != null ? PivotGrid.AllowResizing : true; }
		}
		CellMode ActualMode {
			get {
				CellMode mode = CellMode.None;
				if(IsSelected)
					mode = CellMode.Selected;
				if(IsTotalAppearance)
					mode = mode | CellMode.Tolal;
				return mode;
			}
		}
		protected virtual bool IsExporting { get { return false; } }
		public List<PivotGridField> GetCrossAreaFields() {
			List<PivotFieldItemBase> crossFields = item.GetCrossAreaFields();
			List<PivotGridField> res = new List<PivotGridField>(crossFields.Count);
			for(int i = 0; i < crossFields.Count; i++) {
				res.Add((Data.GetField(crossFields[i])));
			}
			return res;
		}
		protected internal override void UpdateAppearance() {
			if(PivotGrid == null) return;
			PivotCustomValueAppearanceEventArgs e = PivotGrid.RaiseCustomValueAppearance(Item, IsExporting);
			Background = e.Background ?? GetActualBackgroundBrush();
			Foreground = e.Foreground ?? GetActualForegroundBrush();
		}
		protected override Brush GetActualBackgroundBrush() {
			switch(ActualMode) {
				case CellMode.None:
					return PivotGrid.ValueBackground;
				case CellMode.Selected:
					return PivotGrid.ValueSelectedBackground;
				case CellMode.Tolal:
					return PivotGrid.ValueTotalBackground;
				case CellMode.TotalSelected:
					return PivotGrid.ValueTotalSelectedBackground;
				default:
					throw new Exception("CellMode");
			}
		}
		protected override Brush GetActualForegroundBrush() {
			switch(ActualMode) {
				case CellMode.None:
					return PivotGrid.ValueForeground;
				case CellMode.Selected:
					return PivotGrid.ValueSelectedForeground;
				case CellMode.Tolal:
					return PivotGrid.ValueTotalForeground;
				case CellMode.TotalSelected:
					return PivotGrid.ValueTotalSelectedForeground;
				default:
					throw new Exception("CellMode");
			}
		}
		protected internal void UpdatePadding() {
			if(!IsCustomTotalAppearance || IsColumn || PivotGrid.RowTotalsLocation != FieldRowTotalsLocation.Tree) return;
			Padding = new Thickness(PivotGrid.RowTreeOffset, 0, 0, 0);
		}
		public ICommand ChangeFieldValueExpanded {
			get { return PivotGrid.Commands.ChangeFieldValueExpanded; }
		}
	}
	public class SortBySummaryButton : Button {
		public SortBySummaryButton() {
			Loaded += SortBySummaryButton_Loaded;
			DataContextChanged += SortBySummaryButton_DataContextChanged;
		}
		void SortBySummaryButton_Loaded(object sender, System.Windows.RoutedEventArgs e) {
			EnsureState();
		}
		void SortBySummaryButton_DataContextChanged(object sender, System.Windows.DependencyPropertyChangedEventArgs e) {
			EnsureState();
		}
		protected override void OnClick() {
			base.OnClick();
			FieldValueItem item = DataContext as FieldValueItem;
			if(item == null || item.Item == null)
				return;
			List<PivotGridFieldPair> pair = item.VisualItems.GetSortedBySummaryFields(item.IsColumn, item.Item.Index);
			if(pair == null || pair.Count == 0 || item.Data.PivotGrid.IsAsyncInProgress)
				return;
			PivotGridControl pivot = item.Data.PivotGrid;
			pivot.BeginUpdate();
			PivotSortOrder order = pair[0].Field.SortOrder == PivotSortOrder.Ascending ? PivotSortOrder.Descending : PivotSortOrder.Ascending;
			for(int i = 0; i < pair.Count; i++)
				pair[i].Field.SortOrder = order;
			if(pivot.UseAsyncMode)
				pivot.EndUpdateAsync();
			else
				pivot.EndUpdate();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			EnsureState();
		}
		private void EnsureState() {
			FieldValueItem item = DataContext as FieldValueItem;
			if(item == null || item.Item == null || item.Data == null || item.Data.IsLocked)
				return;
			List<PivotGridFieldPair> pair = item.VisualItems.GetSortedBySummaryFields(item.IsColumn, item.Item.Index);
			if(pair == null || pair.Count == 0)
				return;
			PivotSortOrder state = pair[0].FieldItem.SortOrder;
			bool actual = true;
			for(int i = 1; i < pair.Count; i++)
				if(state != pair[i].FieldItem.SortOrder) {
					actual = false;
					break;
				}
			string vstate = actual ? state == PivotSortOrder.Ascending ? "AscSorted" : "DescSorted" : "NotSorted";
			VisualStateManager.GoToState(this, vstate, false);
		}
	}
	public static class PivotFieldValueItemExtensions {
		public static bool IsTotalAppearance(this PivotFieldValueItem item) {
			if(item.IsRowTree)
				return item.ValueType == PivotGridValueType.GrandTotal;
			return item.IsTotal;
		}
	}
	public class FieldValueCellElement : ScrollableAreaCell {
		public const string GripperXTemplateName = "PART_HorizontalGripper",
			GripperYTemplateName = "PART_VerticalGripper";
		#region static
		public static readonly DependencyProperty GripperXVisibilityProperty;
		static readonly DependencyPropertyKey GripperXVisibilityPropertyKey;
		public static readonly DependencyProperty GripperYVisibilityProperty;
		static readonly DependencyPropertyKey GripperYVisibilityPropertyKey;
		static FieldValueCellElement() {
			Type ownerType = typeof(FieldValueCellElement);
			GripperXVisibilityPropertyKey = DependencyPropertyManager.RegisterReadOnly("GripperXVisibility", typeof(Visibility),
				ownerType, new PropertyMetadata(Visibility.Visible));
			GripperXVisibilityProperty = GripperXVisibilityPropertyKey.DependencyProperty;
			GripperYVisibilityPropertyKey = DependencyPropertyManager.RegisterReadOnly("GripperYVisibility", typeof(Visibility),
				ownerType, new PropertyMetadata(Visibility.Visible));
			GripperYVisibilityProperty = GripperYVisibilityPropertyKey.DependencyProperty;
		}
		#endregion
		ResizeHelper resizeHelperX, resizeHelperY;
		Resizer resizerX, resizerY;
		Thumb gripperX, gripperY;
		FieldValueItem valueItem;
		public FieldValueCellElement() {
			Loaded += OnLoaded;
		}
		protected override void SetDefaultStyleKey() {
			this.SetDefaultStyleKey(typeof(FieldValueCellElement));
		}
		void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			UpdateVisualState();
		}
		protected virtual void UpdateVisualState() {
			FieldValueItem item = ValueItem as FieldValueItem;
			if(item == null) return;
			if(item.IsColumn && (item.IsTotalAppearance || item.IsCustomTotalAppearance))
				VisualStateManager.GoToState(this, "Total", false);
			else
				VisualStateManager.GoToState(this, "Normal", false);
		}
#if SL
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if(e.ClickCount != 2)
				return;
#else
		protected override void OnMouseDoubleClick(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnMouseDoubleClick(e);
#endif
			FrameworkElement source = e.OriginalSource as FrameworkElement;
			if(source != null) {
				DependencyObject elem = LayoutHelper.FindLayoutOrVisualParentObject(source, (d) => d as FrameworkElement != null && IsGripper((FrameworkElement)d));
				if(elem != null) return;
			}
			if(Item == null || Item.Data == null)
				return;
			PivotGridWpfData data = Item.Data;
			if(data.IsLocked) return;
			if(Item.Item.AllowExpandOnDoubleClick && data.PivotGrid.AllowExpandOnDoubleClick)
#if !SL
				data.PivotGrid.Commands.ChangeFieldValueExpanded.Execute(Item.Item, data.PivotGrid);
#else //valid in WPF untill pivot has focus
				data.PivotGrid.Commands.ChangeFieldValueExpanded.Execute(Item.Item);
#endif
		}
		bool IsGripper(FrameworkElement element) {
			return element.Name == GripperXTemplateName || element.Name == GripperYTemplateName;
		}
		public Resizer ResizerX { get { return resizerX; } }
		public Resizer ResizerY { get { return resizerY; } }
		public ResizeHelper ResizeHelperX { get { return resizeHelperX; } }
		public ResizeHelper ResizeHelperY { get { return resizeHelperY; } }
		public Thumb GripperX { get { return gripperX; } }
		public Thumb GripperY { get { return gripperY; } }
		public Visibility GripperXVisibility {
			get { return (Visibility)GetValue(GripperXVisibilityProperty); }
			protected set { this.SetValue(GripperXVisibilityPropertyKey, value); }
		}
		public Visibility GripperYVisibility {
			get { return (Visibility)GetValue(GripperYVisibilityProperty); }
			protected set { this.SetValue(GripperYVisibilityPropertyKey, value); }
		}
		public FieldValueItem Item { get { return valueItem; } }
		public bool IsColumn { get { return Item != null ? Item.IsColumn : true; } }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			this.gripperX = GetTemplateChild(GripperXTemplateName) as Thumb;
			this.gripperY = GetTemplateChild(GripperYTemplateName) as Thumb;
			UpdateResizers();
			UpdateVisualState();
		}
		protected override void OnValueItemChanged(ScrollableAreaItemBase oldValue, ScrollableAreaItemBase newValue) {
			base.OnValueItemChanged(oldValue, newValue);
			this.valueItem = (FieldValueItem)newValue;
			UpdateResizers();
			UpdateVisualState();
		}
		protected internal void UpdateResizers() {
			if(resizeHelperX != null) {
				resizeHelperX.Dispose();
				resizeHelperX = null;
			}
			if(resizeHelperY != null) {
				resizeHelperY.Dispose();
				resizeHelperY = null;
			}
			if(Item == null) return;
			PivotGridWpfData data = Item.Data;
			if(data == null) return;
			EndResizing(this.resizerX);
			EndResizing(this.resizerY);
			this.resizerX = Resizer.Create(data, this, Item.Item, Orientation.Horizontal);
			this.resizerY = Resizer.Create(data, this, Item.Item, Orientation.Vertical);
			this.resizeHelperX = new ResizeHelper(ResizerX);
			this.resizeHelperY = new ResizeHelper(ResizerY);
			if(GripperX != null)
				ResizeHelperX.Init(GripperX);
			if(GripperY != null)
				ResizeHelperY.Init(GripperY);
			GripperXVisibility = ResizerX.CanResize ? Visibility.Visible : Visibility.Collapsed;
			GripperYVisibility = ResizerY.CanResize ? Visibility.Visible : Visibility.Collapsed;
		}
		void EndResizing(Resizer resizer) {
			if(resizer != null && resizer.GetIsResizing())
				resizer.SetIsResizing(false);
		}
		protected override Thickness UpdateMarginByRightMost(Thickness margin) {
			if(IsColumn)
				return base.UpdateMarginByRightMost(margin);
			return margin;
		}
	}
	public class FieldValuesItemsList : CellsItemsList {
		protected readonly bool isColumn;
		readonly bool isEmpty;
		readonly int count;
#if DEBUGTEST
		internal
#endif
 PivotFieldValueItem[] items;
		public FieldValuesItemsList(PivotVisualItems visualItems, bool isColumn)
			: base(visualItems) {
			this.isColumn = isColumn;
			isEmpty = visualItems == null || visualItems.Data == null;
			this.count = isEmpty ? 0 : visualItems.GetItemCount(isColumn);
			if(isEmpty) return;
			CreateAndSortItems();
		}
		protected PivotFieldValueItem[] Items {
			get {
				if(items == null)
					items = new PivotFieldValueItem[count];
				return items;
			}
		}
		public override object GetRawValue(int index) {
			return Items[index];
		}
		public override int ColumnCount {
			get {
				return isColumn ? base.ColumnCount : visualItems.GetLevelCount(isColumn);
			}
		}
		public override int RowCount {
			get {
				if(isColumn && Count == 1 && ((FieldValueItem)this[0]).IsGrandTotal)
					return 0;
				return isColumn ? visualItems.GetLevelCount(isColumn) : base.RowCount;
			}
		}
		protected virtual void CreateAndSortItems() {
			if(isEmpty) return;
			for(int i = 0; i < count; i++)
				Items[i] = visualItems.GetItem(isColumn, i);
			Array.Sort<PivotFieldValueItem>(Items, PivotFieldValueItemItemsComparer.Default);
		}
		public override int Count {
			get { return count; }
		}
		protected override ScrollableAreaItemBase CreateItem(int index) {
			return new FieldValueItem(Items[index], visualItems);
		}
		public override int GetRowIndex(int index) {
			return GetRowIndexCore(index);
		}
		protected override int GetMinLevelAtIndex(int i) {
			return Items[i].IsColumn ? Items[i].StartLevel : Items[i].MinLastLevelIndex;
		}
	}
}
