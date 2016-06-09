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
using System.Windows.Media;
using DevExpress.Data.PivotGrid;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DependencyPropertyManager = System.Windows.DependencyProperty;
using System.Linq;
using DevExpress.Xpf.Editors.Native;
namespace DevExpress.Xpf.PivotGrid.Internal {
	public abstract class ScrollableAreaItemBase : DependencyObject {
		#region static
		public static readonly DependencyProperty ForegroundProperty;
		public static readonly DependencyProperty BackgroundProperty;
		static readonly DependencyPropertyKey IsSelectedPropertyKey;
		public static readonly DependencyProperty IsSelectedProperty;
		static ScrollableAreaItemBase() {
			Type ownerType = typeof(ScrollableAreaItemBase);
			IsSelectedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsSelected", typeof(bool), ownerType,
				new PropertyMetadata(false, (d, e) => ((ScrollableAreaItemBase)d).UpdateAppearance()));
			IsSelectedProperty = IsSelectedPropertyKey.DependencyProperty;
			ForegroundProperty = DependencyPropertyManager.Register("Foreground", typeof(Brush), ownerType, new PropertyMetadata(null));
			BackgroundProperty = DependencyPropertyManager.Register("Background", typeof(Brush), ownerType, new PropertyMetadata(null));
		}
		#endregion
		public Brush Background {
			get { return (Brush)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}
		public Brush Foreground {
			get { return (Brush)GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}
		public bool IsSelected {
			get { return (bool)GetValue(IsSelectedProperty); }
			internal set { this.SetValue(IsSelectedPropertyKey, value); }
		}
		protected internal abstract void UpdateAppearance();
		protected abstract Brush GetActualBackgroundBrush();
		protected abstract Brush GetActualForegroundBrush();
		public abstract int MinLevel { get; }
		public abstract int MaxLevel { get; }
		public abstract int MinIndex { get; }
		public abstract int MaxIndex { get; }
		public abstract object Value { get; }
		public abstract string DisplayText { get; }
		public abstract PivotGridField Field { get; }
		public bool InRect(System.Drawing.Rectangle rect) {
			return (InRange(MinLevel, rect.Top, rect.Bottom) || InRange(MaxLevel, rect.Top, rect.Bottom) || InRange(rect.Top, MinLevel, MaxLevel)) &&
				(InRange(MinIndex, rect.Left, rect.Right) || InRange(MaxIndex, rect.Left, rect.Right) || InRange(rect.Left, MinIndex, MaxIndex));
		}
		static bool InRange(int value, int left, int right) {
			return value >= left && value < right;
		}
	}
	public interface IScrollableAreaCell {
		bool IsLeftMost { get; set; }
		bool IsRightMost { get; set; }
		bool IsTopMost { get; set; }
		bool IsBottomMost { get; set; }
		int MaxIndex { get; }
		int MinIndex { get; }
		int MaxLevel { get; }
		int MinLevel { get; }
		Thickness Border { get; set; }
	}
	public class CellElement : ScrollableAreaCell, IConditionalFormattingClient<CellElement>, IChrome, IFormatInfoProvider, ISupportHorizonalContentAlignment {
		static readonly RenderTemplate backgroundTemplate = new RenderTemplate() { RenderTree = new RenderBorder() };
		readonly ConditionalFormattingHelper<CellElement> formattingHelper;
		readonly ConditionalFormatContentRenderHelper<CellElement> conditionalFormatContentRenderHelper;
		readonly Locker locker = new Locker();
		readonly RenderBorderContext border;
#if DEBUGTEST 
		public RenderBorderContext TestBorder { get { return border; } }
#endif
		public CellElement() {
			formattingHelper = new ConditionalFormattingHelper<CellElement>(this);
			conditionalFormatContentRenderHelper = new ConditionalFormatContentRenderHelper<CellElement>(this);
			border = (RenderBorderContext)ChromeHelper.CreateContext(this, backgroundTemplate);
		}
		protected override void SetDefaultStyleKey() {
			this.SetDefaultStyleKey(typeof(CellElement));
		}
		protected override void OnValueItemChanged(ScrollableAreaItemBase oldValue, ScrollableAreaItemBase newValue) {
			base.OnValueItemChanged(oldValue, newValue);
			UpdatePaint();
		}
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if(e.Property == CellElement.BorderBrushProperty || e.Property == CellElement.BorderProperty) {
				UpdatePaint();
				InvalidateMeasure();
			}
		}
		public void UpdatePaint() {
			if(ValueItem == null)
				return;
			conditionalFormatContentRenderHelper.SetMargin(Border);
			formattingHelper.UpdateConditionalAppearance();
			border.Background = ValueItem.Background;
			border.BorderThickness = Border;
			border.BorderBrush = BorderBrush;
			SetValue(TextBlock.ForegroundProperty, ValueItem.Foreground);
		}
		CellsAreaItem CellsAreaItem { get { return (CellsAreaItem)ValueItem; } }
		protected override void OnRender(DrawingContext drawingContext) {
			border.Render(drawingContext);
			conditionalFormatContentRenderHelper.Render(drawingContext);
		}
		protected override Size MeasureOverride(Size constraint) {
			conditionalFormatContentRenderHelper.Measure(constraint);
			return base.MeasureOverride(constraint);
		}
		protected override Size ArrangeOverride(Size arrangeSize) {
			border.Arrange(new Rect(0, 0, arrangeSize.Width, arrangeSize.Height));
			conditionalFormatContentRenderHelper.Arrange(arrangeSize);
			return base.ArrangeOverride(arrangeSize);
		}
		public ConditionalFormattingHelper<CellElement> FormattingHelper {
			get { return formattingHelper; }
		}
		IList<FormatConditionBaseInfo> IConditionalFormattingClient<CellElement>.GetRelatedConditions() {
			if(ValueItem != null && ValueItem.Field != null)
				return ValueItem.Field.PivotGrid.FormatConditions.GetInfoByFieldName(CellsAreaItem.Field.Name, CellsAreaItem);
			return null;
		}
		FormatValueProvider? IConditionalFormattingClient<CellElement>.GetValueProvider(string fieldName) {
			return new FormatValueProvider(this, ValueItem.Value, ValueItem.Field.Name);
		}
		object IFormatInfoProvider.GetCellValue(string fieldName) {
			PivotGridData data = ValueItem.Field.Data;
			PivotGridFieldBase field = data.GetFieldByNameOrDataControllerColumnName(fieldName);
			if(field == null || !field.Visible)
				return false;
			switch(field.Area) {
				case PivotArea.RowArea:
					return data.VisualItems.GetFieldValue(field, CellsAreaItem.RowIndex);
				case PivotArea.ColumnArea:
					return data.VisualItems.GetFieldValue(field, CellsAreaItem.ColumnIndex);
				case PivotArea.DataArea:
					return data.GetCellValue(CellsAreaItem.Item.ColumnFieldIndex, CellsAreaItem.Item.RowFieldIndex, field);
				case PivotArea.FilterArea:
					return null;
				default:
					throw new ArgumentException("fieldDescriptor.Field.Area");
			}
		}
		object IFormatInfoProvider.GetCellValueByListIndex(int listIndex, string fieldName) {
			throw new NotImplementedException();
		}
		object IFormatInfoProvider.GetTotalSummaryValue(string fieldName, ConditionalFormatSummaryType summaryType) {
			return ValueItem.Field.Data.GetAggregation(fieldName, ((CellsAreaItem)ValueItem).Item, summaryType);
		}
		static PivotValueComparerBase comparer = new PivotValueComparerBase();
		Data.ValueComparer IFormatInfoProvider.ValueComparer {
			get { return comparer; }
		}
		bool IConditionalFormattingClient<CellElement>.IsReady {
			get { return true; }
		}
		bool IConditionalFormattingClient<CellElement>.IsSelected {
			get { return ValueItem.IsSelected; }
		}
		Locker IConditionalFormattingClient<CellElement>.Locker {
			get { return locker; }
		}
		void IConditionalFormattingClient<CellElement>.UpdateBackground() {
			ValueItem.Background = formattingHelper.CoerceBackground(ValueItem.Background);
		}
		void IConditionalFormattingClient<CellElement>.UpdateDataBarFormatInfo(DataBarFormatInfo info) {
			conditionalFormatContentRenderHelper.UpdateDataBarFormatInfo(info);
		}
		void IChrome.AddChild(FrameworkElement element) {
		}
		void IChrome.GoToState(string stateName) {
		}
		void IChrome.RemoveChild(FrameworkElement element) {
		}
		FrameworkRenderElementContext IChrome.Root {
			get { return null; }
		}
		HorizontalAlignment ISupportHorizonalContentAlignment.HorizonalContentAlignment {
			get { return CellsAreaItem != null ? CellsAreaItem.HorizontalContentAlignment : HorizontalAlignment.Right; }
		}
	}
	public abstract class ScrollableAreaCell : Control, IScrollableAreaCell {
		#region static stuff
		public static readonly DependencyProperty ValueItemProperty;
		public static readonly DependencyProperty IsLeftMostProperty;
		public static readonly DependencyProperty IsTopMostProperty;
		public static readonly DependencyProperty IsRightMostProperty;
		public static readonly DependencyProperty IsBottomMostProperty;
		public static readonly DependencyProperty BorderProperty;
		static ScrollableAreaCell() {
			Type ownerType = typeof(ScrollableAreaCell);
			ValueItemProperty = DependencyPropertyManager.Register("ValueItem", typeof(ScrollableAreaItemBase), ownerType,
					new FrameworkPropertyMetadata(null, OnValueItemPropertyChanged));
			IsLeftMostProperty = DependencyPropertyManager.Register("IsLeftMost", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			IsTopMostProperty = DependencyPropertyManager.Register("IsTopMost", typeof(bool), ownerType,
					new FrameworkPropertyMetadata(false, (d, e) => ((ScrollableAreaCell)d).UpdateMarginByMost()));
			IsRightMostProperty = DependencyPropertyManager.Register("IsRightMost", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((ScrollableAreaCell)d).UpdateMarginByMost()));
			IsBottomMostProperty = DependencyPropertyManager.Register("IsBottomMost", typeof(bool), ownerType,
					new FrameworkPropertyMetadata(false, (d, e) => ((ScrollableAreaCell)d).UpdateMarginByMost()));
			BorderProperty = DependencyPropertyManager.Register("Border", typeof(Thickness), ownerType, new FrameworkPropertyMetadata(new Thickness(0)));
		}
		static void OnValueItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((ScrollableAreaCell)d).OnValueItemChanged((ScrollableAreaItemBase)e.OldValue, (ScrollableAreaItemBase)e.NewValue);
		}
		public static ScrollableAreaItemBase GetValueItem(DependencyObject d) {
			if(d == null)
				throw new ArgumentNullException("d");
			ScrollableAreaCell cell = LayoutHelper.FindParentObject<ScrollableAreaCell>(d);
			return cell != null ? cell.ValueItem : null;
		}
		#endregion
		public ScrollableAreaCell() {
			SetDefaultStyleKey();
		}
		protected virtual void SetDefaultStyleKey() {
			this.SetDefaultStyleKey(typeof(ScrollableAreaCell));
		}
		void UpdateMarginByMost() {
			Thickness margin = new Thickness(0);
			if(IsTopMost)
				margin.Top = 1;
			if(IsBottomMost)
				margin.Bottom = -1;
			if(IsRightMost)
				margin = UpdateMarginByRightMost(margin);
			Margin = margin;
		}
		protected virtual Thickness UpdateMarginByRightMost(Thickness margin) {
			margin.Right = -1;
			return margin;
		}
		public ScrollableAreaItemBase ValueItem {
			get { return (ScrollableAreaItemBase)GetValue(ValueItemProperty); }
			set { SetValue(ValueItemProperty, value); }
		}
		public bool IsLeftMost {
			get { return (bool)GetValue(IsLeftMostProperty); }
			set { SetValue(IsLeftMostProperty, value); }
		}
		public bool IsTopMost {
			get { return (bool)GetValue(IsTopMostProperty); }
			set { SetValue(IsTopMostProperty, value); }
		}
		public bool IsRightMost {
			get { return (bool)GetValue(IsRightMostProperty); }
			set { SetValue(IsRightMostProperty, value); }
		}
		public bool IsBottomMost {
			get { return (bool)GetValue(IsBottomMostProperty); }
			set { SetValue(IsBottomMostProperty, value); }
		}
		public Thickness Border {
			get { return (Thickness)GetValue(BorderProperty); }
			set { SetValue(BorderProperty, value); }
		}
		protected virtual void OnValueItemChanged(ScrollableAreaItemBase oldValue, ScrollableAreaItemBase newValue) {
			DataContext = ValueItem;
		}
		#region IScrollableAreaCell members
		int IScrollableAreaCell.MaxIndex { get { return ValueItem.MaxIndex; } }
		int IScrollableAreaCell.MinIndex { get { return ValueItem.MinIndex; } }
		int IScrollableAreaCell.MaxLevel { get { return ValueItem.MaxLevel; } }
		int IScrollableAreaCell.MinLevel { get { return ValueItem.MinLevel; } }
		#endregion
	}
	public enum CellMode {
		None = 0,
		Tolal = 1,
		Selected = 2,
		TotalSelected = Tolal | Selected,
	}
}
