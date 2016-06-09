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
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.XtraPivotGrid.Data;
#if SL
using Panel = System.Windows.Controls.Control;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Core.WPFCompatibility;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
#endif
namespace DevExpress.Xpf.PivotGrid.Printing {
	public class PivotPrintFieldValues : FieldValuesPresenter, IVirtualChildCollection
#if SL
		, ISupportInitialize
#endif
	{
		PrintFieldValueCell printCell;
		#region static
		public static readonly DependencyProperty PrintItemsProperty;
		public static readonly DependencyProperty NotHasTopBorderProperty;
		static PivotPrintFieldValues() {
			Type ownerType = typeof(PivotPrintFieldValues);
			PrintItemsProperty = DependencyPropertyManager.Register("PrintItems", typeof(List<ScrollableAreaItemBase>), ownerType,
				new UIPropertyMetadata(null, (d, e) => ((PivotPrintFieldValues)d).OnPrintItemsChanged((List<ScrollableAreaItemBase>)e.NewValue)));
			NotHasTopBorderProperty = DependencyPropertyManager.Register("NotHasTopBorder", typeof(bool), ownerType, new PropertyMetadata(false));
		}
		#endregion
		public PivotPrintFieldValues()
			: base() {
#if SL
				SubscribeEvents();
#endif
		}
		public List<ScrollableAreaItemBase> PrintItems {
			get { return (List<ScrollableAreaItemBase>)GetValue(PrintItemsProperty); }
			set { SetValue(PrintItemsProperty, value); }
		}
		public bool NotHasTopBorder {
			get { return (bool)GetValue(NotHasTopBorderProperty); }
			set { SetValue(NotHasTopBorderProperty, value); }
		}
		PrintFieldValueCell PrintCell {
			get {
				if(Items.Count == 0)
					return null;
				if(printCell == null) {
					printCell = new PrintFieldValueCell();
					Children.Add(printCell);
					printCell.ValueItem = Items[0];
				}
				return printCell;
			}
		}
		void OnPrintItemsChanged(List<ScrollableAreaItemBase> items) {
			UpdateLeftTop(items);
			ResetItems();
			EnsureItems();
		}
		protected override void OnIsColumnChanged() {
			base.OnIsColumnChanged();
			UpdateLeftTop(PrintItems);
		}
		protected override ScrollableAreaCell CreateCell() {
			throw new NotImplementedException();
		}
		protected override System.Drawing.Rectangle GetVisibleRect(double actualWidth, double actualHeight, bool allowPartiallyVisible) {
			if(PrintItems != null && PrintItems.Count == 0) return System.Drawing.Rectangle.Empty;
			int x = PrintItems != null && IsColumn ? PrintItems[0].MinIndex : 0;
			int y = PrintItems != null && !IsColumn ? PrintItems[0].MinLevel : 0;
			return new System.Drawing.Rectangle(x, y, Items.ColumnCount - x, Items.RowCount - y);
		}
		protected override Thickness GetBorder(IScrollableAreaCell cell) {
			PivotPrintPageValueItemsList items = PrintItems as PivotPrintPageValueItemsList;
			if(items != null)
				cell.IsRightMost = cell.MaxIndex == items.EndIndex;
			Thickness border = base.GetBorder(cell);
			if(cell.IsTopMost && !NotHasTopBorder)
				border.Top = 1;
			if(cell.IsLeftMost)
				border.Left = 1;
			return border;
		}
		protected override void UpdateBorders(ScrollableAreaCell cell, System.Drawing.Rectangle visibleRect) {
			base.UpdateBorders(cell, visibleRect);
			PrintFieldValueItem item = cell.ValueItem as PrintFieldValueItem;
			if(cell.Visibility != System.Windows.Visibility.Visible || item == null) return;
			item.Border = cell.Border;
		}
		protected override ScrollableItemsList UpdateItems() {
			if(PrintItems != null)
				return new ScrollableAreaItemBaseListWrapper(VisualItems, PrintItems, IsColumn);
			else
				if(cutMergedLevels)
					return new PrintFieldValuesItemsList(VisualItems, IsColumn);
				else
					return new PrintMergedFieldValuesItemsList(VisualItems, IsColumn);
		}
		protected internal override void EnsureCellsCount(double actualWidth, double actualHeight) {
			EnsureItems();
			InvalidateArrange();
		}
		public override int GetTotalWidth() {
			EnsureVisualItems();
			return VisualItems.GetWidthDifference(Left, MaxIndex + 1, IsColumn);
		}
		public override int GetTotalHeight() {
			EnsureVisualItems();
			return VisualItems.GetHeightDifference(Top, MaxLevel + 1, IsColumn);
		}
		protected override void OnLoaded(object sender, System.Windows.RoutedEventArgs e) {
			SubscribeEvents();
		}
#if !SL
		protected  override 
#endif
		void OnInitialized(EventArgs e) {
			ResetItems();
			EnsureItems();
		}
		protected override void OnItemsChanged() { }
		protected override void OnSizeChanged(object sender, SizeChangedEventArgs e) {
			CurrentWidth = e.NewSize.Width;
		}
		protected override void OnLeftTopChanged() {
			MeasureLeft = Left;
			MeasureTop = Top;
		}
		protected override void SubscribeEvents() {
			SizeChanged += OnSizeChanged;
			CurrentWidth = RenderSize.Width;
		}
		protected override void UnsubscribeEvents() {
			SizeChanged -= OnSizeChanged;
		}
		bool cutMergedLevels {
			get {
				return IsColumn && !Data.PivotGrid.MergeColumnFieldValues ||
					!IsColumn && !Data.PivotGrid.MergeRowFieldValues;
			}
		}
		void UpdateLeftTop(List<ScrollableAreaItemBase> items) {
			if(items == null || items.Count == 0) return;
			InvalidateMeasure();
			if(IsColumn)
				this.Left = items[0].MinIndex;
			else
				this.Top = items[0].MinLevel;
		}
		protected override void ResetChildren() {
			EnsureCellsCount(ActualWidth, ActualHeight);
		}
		protected override Size MeasureOverride(Size availableSize) {
#if SL
			EnsureItems();
#endif
			Size size = base.MeasureOverride(availableSize);
#if SL
			if(size.Width != double.PositiveInfinity)
				CurrentWidth = size.Width;
#endif
			return size;
		}
		protected override void MeasureCells() { }
		protected override void OnPivotBrushesChanged(object sender, PivotBrushChangedEventArgs e) { }
		protected sealed override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize) {
			return finalSize;
		}
		protected internal override Rect GetItemRect(ScrollableAreaItemBase item) {
			return new Rect(
				new Point(
					VisualItems.GetWidthDifference(Left, item.MinIndex, IsColumn),
					VisualItems.GetHeightDifference(Top, item.MinLevel, IsColumn)),
					GetItemSize(item));
		}
		protected internal override Size GetItemSize(ScrollableAreaItemBase item) {
			Size res = new Size();
			res.Height = VisualItems.GetHeightDifference(item.MinLevel, Math.Min(MaxLevel, item.MaxLevel) + 1, IsColumn);
			res.Width = VisualItems.GetWidthDifference(item.MinIndex, item.MaxIndex + 1, IsColumn);
			if(!IsColumn && item.MaxLevel == MaxLevel)
				res.Height++;
			return res;
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			InitItemsAndEvents();
		}
		#region IVirtualChildCollection Members
		DependencyObject IVirtualChildCollection.GetChild(int childIndex) {
			ScrollableAreaItemBase item = GetItemCore(childIndex);
			PrintCell.ValueItem = item;
			PrintCell.InvalidateMeasure();
			PrintCell.InvalidateArrange();
			PrintCell.Measure(GetItemSize(item));
			PrintCell.Arrange(GetItemRect(item));
			DevExpress.Xpf.Core.LayoutUpdatedHelper.GlobalLocker.DoLockedAction(PrintCell.UpdateLayout);
#if SL
			PrintCell.Measure(GetItemSize(item));
			PrintCell.Arrange(GetItemRect(item));
#endif
			return PrintCell;
		}
#if DEBUGTEST
		internal
#endif
		protected ScrollableAreaItemBase GetItemCore(int childIndex) {
			ScrollableAreaItemBase item = Items[childIndex];
			System.Drawing.Rectangle visibleRect = GetVisibleRect(ActualWidth, ActualHeight);
			UpdateBordersCore((PrintFieldValueItem)item, visibleRect);
			return item;
		}
#if !SL //TODO
		protected override void OnChildDesiredSizeChanged(UIElement child) { }
#endif
		int IVirtualChildCollection.GetChildrenCount() {
			return Items.Count;
		}
		#endregion
#if SL
		#region ISupportInitialize Members
		void ISupportInitialize.BeginInit() {
		}
		void ISupportInitialize.EndInit() {
			OnInitialized(null);
		}
		#endregion
#endif
	}
	public class PrintFieldValueItem : FieldValueItem, IScrollableAreaCell, IPivotTextExportSettings {
		public static readonly HorizontalAlignment HorizontalContentAlignment = HorizontalAlignment.Left;
		int minIndex;
		int maxIndex;
		Thickness border;
		public PrintFieldValueItem(PivotFieldValueItem item, PivotVisualItems visualItems) :
			base(item, visualItems) {
			this.minIndex = item.MinLastLevelIndex;
			this.maxIndex = item.MaxLastLevelIndex;
		}
		public PrintFieldValueItem(PivotFieldValueItem item, PivotVisualItems visualItems, int levelIndex) :
			base(item, visualItems) {
			this.minIndex = levelIndex;
			this.maxIndex = levelIndex;
		}
		public PrintFieldValueItem(FieldValueItem item, int minIndex, int maxIndex) :
			base(item.Item, item.VisualItems) {
			this.minIndex = minIndex;
			this.maxIndex = maxIndex;
		}
		public override int MinLevel {
			get { return Item.IsColumn ? Item.StartLevel : minIndex; }
		}
		public override int MaxLevel {
			get { return Item.IsColumn ? Item.EndLevel : maxIndex; }
		}
		public override int MinIndex {
			get { return Item.IsColumn ? minIndex : Item.StartLevel; }
		}
		public override int MaxIndex {
			get { return Item.IsColumn ? maxIndex : Item.EndLevel; }
		}
		public Thickness Border {
			get { return border; }
			set { border = value; }
		}
		protected override bool IsExporting { get { return true; } }
		protected override Brush GetActualBackgroundBrush() {
			if(IsTotalAppearance)
				return PivotGrid.PrintValueTotalBackground;
			else
				return PivotGrid.PrintValueBackground;
		}
		protected override Brush GetActualForegroundBrush() {
			if(IsTotalAppearance)
				return PivotGrid.PrintValueTotalForeground;
			else
				return PivotGrid.PrintValueForeground;
		}
		#region IScrollableAreaCell Members
		bool IScrollableAreaCell.IsLeftMost { get; set; }
		bool IScrollableAreaCell.IsRightMost { get; set; }
		bool IScrollableAreaCell.IsTopMost { get; set; }
		bool IScrollableAreaCell.IsBottomMost { get; set; }
		#endregion
		#region IPivotTextExportSettings Members
		System.Windows.Media.Color IPivotTextExportSettings.Foreground {
			get {
				if(Foreground != null)
					return (Color)Foreground.GetValue(SolidColorBrush.ColorProperty);
				return Colors.Black;
			}
		}
		System.Windows.Media.Color IPivotTextExportSettings.Background {
			get {
				if(Background != null)
					return (Color)Background.GetValue(SolidColorBrush.ColorProperty);
				return Colors.White;
			}
		}
		HorizontalAlignment IPivotTextExportSettings.HorizontalContentAlignment {
			get { return HorizontalContentAlignment; }
		}
		string IPivotTextExportSettings.ValueFormat {
			get { return IsUseNativeFormat ? Field.ValueFormat : null; }
		}
		bool? IPivotTextExportSettings.UseNativeFormat {
			get {
				if(Item.IsCustomDisplayText)
					return false;
				return Field != null ? Field.UseNativeFormat : null;
			}
		}
		object IPivotTextExportSettings.Value {
			get { return IsUseNativeFormat && IsVisible ? Value : DisplayText; }
		}
		bool IsUseNativeFormat {
			get {
				return Field != null && Field.UseNativeFormat != false && !Item.IsCustomDisplayText && !(Field.UseNativeFormat == null && Field.Area == FieldArea.DataArea); 
			}
		}
		#endregion
	}
	[
	DefaultProperty("Content"), ContentProperty("Content"),
	Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)
	]
	public class PrintFieldValueContentPresenter : FieldDataValueContentPresenter {
		protected override void OnDataContextChanged() {
			FieldValueItem item = DataContext as FieldValueItem;
			if(item == null) return;
			if(item.Field != null) {
				ContentTemplate = item.Field.ActualPrintValueTemplate;
				ContentTemplateSelector = item.Field.ActualPrintValueTemplateSelector;
			} else {
				PivotGridControl pivot = item.VisualItems.Data.PivotGrid;
				if(pivot != null) {
					ContentTemplate = pivot.PrintFieldValueTemplate;
					ContentTemplateSelector = pivot.PrintFieldValueTemplateSelector;
				}
			}
		}
	}
	public class PrintFieldValuesItemsList : FieldValuesItemsList {
		List<FieldValueItem> itemsList;
		public PrintFieldValuesItemsList(PivotVisualItems visualItems, bool isColumn)
			: base(visualItems, isColumn) {
		}
		List<FieldValueItem> ItemsList {
			get {
				if(itemsList == null)
					itemsList = new List<FieldValueItem>();
				return itemsList;
			}
		}
		protected override void CreateAndSortItems() {
			for(int i = 0; i < visualItems.GetItemCount(isColumn); i++)
				AddFieldValueItem(visualItems.GetItem(isColumn, i));
			ItemsList.Sort(PivotFieldValueItemItemsComparer.Default);
		}
		void AddFieldValueItem(PivotFieldValueItem item) {
			if(item.MinLastLevelIndex != item.MaxLastLevelIndex)
				for(int j = item.MinLastLevelIndex; j <= item.MaxLastLevelIndex; j++)
					ItemsList.Add(new PrintFieldValueItem(item, visualItems, j));
			else
				ItemsList.Add(new PrintFieldValueItem(item, visualItems));
		}
		public override object GetRawValue(int index) {
			return ItemsList[index].Item;
		}
		public override ScrollableAreaItemBase this[int index] {
			get { return ItemsList[index]; }
		}
		public override int Count { get { return ItemsList.Count; } }
		protected override ScrollableAreaItemBase CreateItem(int index) {
			throw new InvalidOperationException();
		}
		protected override int GetMinLevelAtIndex(int i) {
			return ItemsList[i].MinLevel;
		}
	}
	public class PrintMergedFieldValuesItemsList : FieldValuesItemsList {
		public PrintMergedFieldValuesItemsList(PivotVisualItems visualItems, bool isColumn)
			: base(visualItems, isColumn) {
		}
		protected override ScrollableAreaItemBase CreateItem(int index) {
			return new PrintFieldValueItem(Items[index], visualItems);
		}
		public override ScrollableAreaItemBase this[int index] { get { return CreateItem(index); } }
	}
	public class ScrollableAreaItemBaseListWrapper : ScrollableItemsList {
		readonly List<ScrollableAreaItemBase> items;
		PivotVisualItems visualItems;
		public ScrollableAreaItemBaseListWrapper(PivotVisualItems visualItems, List<ScrollableAreaItemBase> items, bool isColumn) {
			this.items = items;
			this.IsColumn = isColumn;
			this.visualItems = visualItems;
		}
		public bool IsColumn { get; set; }
		public override int ColumnCount {
			get {
				if(Count == 0) return 0;
				return IsColumn ? this[this.Count - 1].MaxIndex + 1 : visualItems.GetLevelCount(IsColumn);
			}
		}
		public override int RowCount {
			get {
				if(Count == 0) return 0;
				return IsColumn ? visualItems.GetLevelCount(IsColumn) : this[this.Count - 1].MaxLevel + 1;
			}
		}
		public override object GetRawValue(int index) {
			return items[index];
		}
		public override ScrollableAreaItemBase this[int index] {
			get { return items[index]; }
		}
		public override int Count {
			get { return items.Count; }
		}
		protected override ScrollableAreaItemBase CreateItem(int index) {
			throw new InvalidOperationException();
		}
	}
}
