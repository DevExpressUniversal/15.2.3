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
using System.ComponentModel;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.PivotGrid.Internal;
#if SL
using Panel = System.Windows.Controls.Control;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DevExpress.Xpf.Utils;
#endif
namespace DevExpress.Xpf.PivotGrid.Printing {
	public class PivotPrintCells : CellsAreaPresenter, IVirtualChildCollection {
		PivotPrintCell printCell;
		#region static 
		public static readonly DependencyProperty BottomProperty;
		public static readonly DependencyProperty RightProperty;
		public static readonly DependencyProperty PageIndexProperty;
		static PivotPrintCells() { 
			Type ownerType = typeof(PivotPrintCells);
			BottomProperty = DependencyPropertyManager.Register("Bottom", typeof(int), ownerType,
				new UIPropertyMetadata(-1, (d, e) => ((PivotPrintCells)d).OnBottomChanged((int)e.NewValue)));
			RightProperty = DependencyPropertyManager.Register("Right", typeof(int), ownerType,
				new UIPropertyMetadata(-1, (d, e) => ((PivotPrintCells)d).OnRightChanged((int)e.NewValue)));
			PageIndexProperty = DependencyPropertyManager.Register("PageIndex", typeof(int), ownerType,
				new UIPropertyMetadata(-1, (d, e) => ((PivotPrintCells)d).OnPageIndexChanged((int)e.NewValue)));
		}
		PivotPrintCell PrintCell {
			get {
				if(Items.Count == 0)
					return null;
				if(printCell == null) {
					printCell = new PivotPrintCell();
					Children.Add(printCell);
					printCell.ValueItem = Items[0];
				}
				return printCell;
			}
		}
		void OnPageIndexChanged(int newValue) {
			PivotPrintPage page = DataContext as PivotPrintPage;
			if(page == null)
				return;
			if(Right != page.Right)
				Right = page.Right;
			if(Left != page.Left)
				Left = page.Left;
			if(Bottom != page.Bottom)
				Bottom = page.Bottom;
			if(Top != page.Top)
				Top = page.Top;
			InvalidateMeasure();
			ResetItems();
			EnsureItems();
			CreateCells();
		}
		#endregion
		public PivotPrintCells()
			: base() { }
		public int Bottom {
			get { return (int)GetValue(BottomProperty); }
			set { SetValue(BottomProperty, value); }
		}
		public int Right {
			get { return (int)GetValue(RightProperty); }
			set { SetValue(RightProperty, value); }
		}
		public int PageIndex {
			get { return (int)GetValue(PageIndexProperty); }
			set { SetValue(PageIndexProperty, value); }
		}
		bool Paged {
			get { return Bottom != -1 && Right != -1; }
		}
		protected override ScrollableAreaCell CreateCell() {
			throw new NotImplementedException();
		}
		protected override Thickness GetBorder(IScrollableAreaCell cell) {
			Thickness border =  base.GetBorder(cell);
			if(cell.IsTopMost)
				border.Top = 1;
			if(cell.IsLeftMost)
				border.Left = 1;
			if(!Data.PivotGrid.PrintHorzLines && !cell.IsTopMost) {
				border.Top = 0;
			}
			if(!Data.PivotGrid.PrintVertLines && !cell.IsLeftMost) {
				border.Left = 0;
			}			
			return border;
		}
		protected override ScrollableItemsList UpdateItems() {
#if SL
			if(DataContext == null)
				return null;
#endif
			CheckBounds();
			return new PrintCellsItemsList(VisualItems, Left, Right + 1, Top, Bottom + 1);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			InitItemsAndEvents();
		}
		protected override System.Drawing.Rectangle GetVisibleRect(double actualWidth, double actualHeight, bool allowPartiallyVisible) {
			int x = Paged ? Left : 0;
			int y = Paged ? Top : 0;
			return new System.Drawing.Rectangle(x, y, Items.ColumnCount - x, Items.RowCount - y);
		}
		public override int GetTotalWidth() {
			return VisualItems.GetWidthDifference(Left, MaxIndex + 1, true);
		}
		public override int GetTotalHeight() {
			return VisualItems.GetHeightDifference(Top, MaxLevel + 1, false);
		}
		protected internal override void EnsureCellsCount(double actualWidth, double actualHeight) { }
		protected override void OnItemsChanged() { }
		protected override void OnSizeChanged(object sender, SizeChangedEventArgs e) { }
		protected override void OnLeftTopChanged() {
			MeasureLeft = Left;
			MeasureTop = Top;
		}
		protected override void SubscribeEvents() { }
		protected override void UnsubscribeEvents() { }
		protected internal void CreateCells() {
			EnsureItems();
			InvalidateArrange();
		}
		void OnBottomChanged(int newValue) { }
		void OnRightChanged(int newValue) { }
		void CheckBounds() {
			if(Top == -1 && Bottom >= 0 || Bottom == -1 && Top >= 0 || Top > Bottom || Left > Right)
				throw new ArgumentException();
		}
		protected override void ResetChildren() { }
		protected override void MeasureCells() { }
		protected sealed override Size ArrangeOverride(Size finalSize) {
			return finalSize;
		}
		protected override void OnPivotBrushesChanged(object sender, PivotBrushChangedEventArgs e) { }
		#region IVisualTreeWalker Members
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
			UpdateBordersCore((IScrollableAreaCell)item, visibleRect);
			return item;
		}
		protected internal override Size GetItemSize(ScrollableAreaItemBase item) {
			Size size = base.GetItemSize(item);
			if(item.MaxLevel == MaxLevel)
				size.Height++;
			return size;
		}
#if !SL //TODO
		protected override void OnChildDesiredSizeChanged(UIElement child) { }
#endif
		int IVirtualChildCollection.GetChildrenCount() {
			return Items.Count;
		}
		#endregion
#if DEBUGTEST
		internal PivotPrintCell this[int index] {
			get {
				IVirtualChildCollection collection = this;
				if(collection.GetChildrenCount() <= index) {
					if(Items.Count <= index)
						throw new Exception("No such item");
					else
						throw new Exception("Items doesn't loaded");
				}
				return (PivotPrintCell)collection.GetChild(index);
			}
		}
#endif
	}
	public class PrintCellsAreaItem : CellsAreaItem, IScrollableAreaCell, IPivotTextExportSettings {
		Thickness border;
		public PrintCellsAreaItem(PivotVisualItems visualItems, int columnIndex, int rowIndex) :
			base(visualItems, columnIndex, rowIndex) {
		}
		public override object Value {
			get {
				return IsUseNativeFormat ? Item.Value : DisplayText;
			}
		}
		public string ValueFormat {
			get {
				return IsUseNativeFormat ? Field.CellFormat : null;
			}
		}
		public bool? UseNativeFormat {
			get {
				if(Item.IsCustomDisplayText && (Field == null || Field.UseNativeFormat != true))
					return false;
				return Field == null ? null : Field.UseNativeFormat;
			}
		}
		public Thickness Border {
			get { return border; }
			set { border = value; }
		}
		bool IsUseNativeFormat {
			get { return Field != null && Field.UseNativeFormat != false && (Field.UseNativeFormat == true || !Item.IsCustomDisplayText); }
		}
		protected override bool IsExporting { get { return true; } }
		protected override Brush GetActualBackgroundBrush() {
			if(IsTotalAppearance)
				return PivotGrid.PrintCellTotalBackground;
			else
				return PivotGrid.PrintCellBackground;
		}
		protected override Brush GetActualForegroundBrush() {
			if(IsTotalAppearance)
				return PivotGrid.PrintCellTotalForeground;
			else
				return PivotGrid.PrintCellForeground;
		}
		#region IScrollableAreaCell Members
		bool IScrollableAreaCell.IsLeftMost { get; set; }
		bool IScrollableAreaCell.IsRightMost { get; set; }
		bool IScrollableAreaCell.IsTopMost { get; set; }
		bool IScrollableAreaCell.IsBottomMost { get; set; }
		#endregion
		#region IPivotTextExportSettings Members
		Color IPivotTextExportSettings.Foreground {
			get {
				if(Foreground != null)
					return (Color)Foreground.GetValue(SolidColorBrush.ColorProperty);
				return Colors.Black;
			}
		}
		Color IPivotTextExportSettings.Background {
			get {
				if(Background != null)
					return (Color)Background.GetValue(SolidColorBrush.ColorProperty);
				return Colors.White;
			}
		}
		#endregion
	}
	[
	DefaultProperty("Content"), ContentProperty("Content"),
	Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)
	]
	public class PrintCellContentPresenter : FieldDataCellContentPresenter {
		protected override void OnDataContextChanged() {
			CellsAreaItem item = DataContext as CellsAreaItem;
			if(item == null) return;
			if(item.Field != null) {
				if(ContentTemplateSelector != item.Field.ActualPrintCellTemplateSelector)
					ContentTemplateSelector = item.Field.ActualPrintCellTemplateSelector;
				if(ContentTemplate != item.Field.ActualPrintCellTemplate)
					ContentTemplate = item.Field.ActualPrintCellTemplate;
			}
		}
	}
	public class PrintCellsItemsList : CellsItemsList {
		int left, right, top, bottom;
		public PrintCellsItemsList(PivotVisualItems visualItems)
			: this(visualItems, 0, visualItems.GetLastLevelItemCount(true), 0, visualItems.GetLastLevelItemCount(false)) { }
		public PrintCellsItemsList(PivotVisualItems visualItems, int left, int right, int top, int bottom)
			: base(visualItems) {
			this.left = left;
			this.right = right;
			this.top = top;
			this.bottom = bottom;
		}
		int width { get { return right - left; } }
		int height { get { return bottom - top; } }
		public override int ColumnCount { get { return right; } }
		public override int RowCount { get { return bottom; } }
		public override int Count { get { return width * height; } }
		protected override ScrollableAreaItemBase CreateItem(int index) {
			int columnIndex = index % width,
				 rowIndex = index / width;
			return new PrintCellsAreaItem(visualItems, columnIndex + left, rowIndex + top);
		}
		public override ScrollableAreaItemBase this[int index] { get { return CreateItem(index); } }
		public override object GetRawValue(int index) {
			return null;
		}
	}
}
