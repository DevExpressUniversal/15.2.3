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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using DevExpress.Data.Summary;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Selection;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using WarningException = System.Exception;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using Visual = System.Windows.UIElement;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using FrameworkContentElement = System.Windows.DependencyObject;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using RoutedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventArgs;
using RoutedEventHandler = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEventHandler;
using ContentPresenter = DevExpress.Xpf.Core.XPFContentPresenter;
#else
using System.Timers;
#endif
namespace DevExpress.Xpf.PivotGrid.Internal {
	public class CellsAreaItem : ScrollableAreaItemBase {
		#region statics
		static readonly DependencyPropertyKey IsFocusedPropertyKey;
		public static readonly DependencyProperty IsFocusedProperty;
		static CellsAreaItem() {
			Type ownerType = typeof(CellsAreaItem);
			IsFocusedPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsFocused", typeof(bool), ownerType,
				new PropertyMetadata(false));
			IsFocusedProperty = IsFocusedPropertyKey.DependencyProperty;
		}
#if DEBUGTEST
		public static bool GetIsSelected(DependencyObject d) {
			return (bool)((ScrollableAreaCell)d).ValueItem.GetValue(IsSelectedProperty);
		}
		internal static void SetIsSelected(DependencyObject d, bool value) {
			((ScrollableAreaCell)d).ValueItem.IsSelected = value;
		}
		public static bool GetIsFocused(DependencyObject d) {
			return (bool)((ScrollableAreaCell)d).ValueItem.GetValue(IsFocusedProperty);
		}
		internal static void SetIsFocused(DependencyObject d, bool value) {
			((ScrollableAreaCell)d).ValueItem.SetValue(IsFocusedPropertyKey, value);
		}
#endif
		#endregion
		PivotVisualItems visualItems;
		int columnIndex, rowIndex;
		PivotGridCellItem item;
		object columnTotalValue, rowTotalValue;
		public CellsAreaItem(PivotVisualItems visualItems, int columnIndex, int rowIndex) {
			this.visualItems = visualItems;
			this.columnIndex = columnIndex;
			this.rowIndex = rowIndex;
			UpdateAppearance();
		}
		public PivotGridControl PivotGrid { get { return VisualItems.Data.PivotGrid; } }
		public PivotGridCellItem Item {
			get {
				if(item == null)
					item = visualItems.CreateCellItem(columnIndex, rowIndex);
				return item;
			}
		}
		protected PivotVisualItems VisualItems { get { return visualItems; } }
		public override PivotGridField Field {
			get {
				PivotFieldItemBase item = Item.DataField;
				return item != null ? ((PivotFieldItem)item).Wrapper : visualItems.Data.DataField;
			}
		}
		public int ColumnIndex { get { return columnIndex; } }
		public int RowIndex { get { return rowIndex; } }
		public bool IsFocused {
			get { return (bool)GetValue(IsFocusedProperty); }
			internal set { this.SetValue(IsFocusedPropertyKey, value); }
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
		protected internal override void UpdateAppearance() {
			PivotGridControl pivot = visualItems.Data.PivotGrid;
			if(pivot == null)
				return;
			PivotCustomCellAppearanceEventArgs e = pivot.RaiseCustomCellAppearance(Item, IsExporting);
			Background = e.Background ?? GetActualBackgroundBrush();
			Foreground = e.Foreground ?? GetActualForegroundBrush();
		}
		protected override Brush GetActualBackgroundBrush() {
			switch(ActualMode) {
				case CellMode.None:
					return PivotGrid.CellBackground;
				case CellMode.Selected:
					return PivotGrid.CellSelectedBackground;
				case CellMode.Tolal:
					return PivotGrid.CellTotalBackground;
				case CellMode.TotalSelected:
					return PivotGrid.CellTotalSelectedBackground;
				default:
					throw new Exception("CellMode");
			}
		}
		protected override Brush GetActualForegroundBrush() {
			switch(ActualMode) {
				case CellMode.None:
					return PivotGrid.CellForeground;
				case CellMode.Selected:
					return PivotGrid.CellSelectedForeground;
				case CellMode.Tolal:
					return PivotGrid.CellTotalForeground;
				case CellMode.TotalSelected:
					return PivotGrid.CellTotalSelectedForeground;
				default:
					throw new Exception("CellMode");
			}
		}
		#region IScrollableAreaItem Members
		public override int MinLevel {
			get { return rowIndex; }
		}
		public override int MaxLevel {
			get { return rowIndex; }
		}
		public override int MinIndex {
			get { return columnIndex; }
		}
		public override int MaxIndex {
			get { return columnIndex; }
		}
		public override object Value {
			get { return Item.Value; }
		}
		public override string DisplayText {
			get { return Item.Text; }
		}
		#endregion
		public object ColumnTotalValue {
			get {
				if(columnTotalValue == null)
					columnTotalValue = visualItems.GetColumnTotalValue(Item, true);
				return columnTotalValue;
			}
		}
		public object RowTotalValue {
			get {
				if(rowTotalValue == null)
					rowTotalValue = visualItems.GetRowTotalValue(Item, true);
				return rowTotalValue;
			}
		}
		public object ColumnValue {
			get { return Item.ColumnFieldValueItem != null ? Item.ColumnFieldValueItem.Value : null; }
		}
		public object ColumnValueDisplayText {
			get { return Item.ColumnFieldValueItem != null ? Item.ColumnFieldValueItem.Text : null; }
		}
		public object RowValue {
			get { return Item.RowFieldValueItem != null ? Item.RowFieldValueItem.Value : null; }
		}
		public object RowValueDisplayText {
			get { return Item.RowFieldValueItem != null ? Item.RowFieldValueItem.Text : null; }
		}
		public HorizontalAlignment HorizontalContentAlignment {
			get {
				string str = Item.Text; 
				if(!object.ReferenceEquals(Value, null) && SummaryItemTypeHelper.IsNumericalType(Value.GetType()))
					return Field != null && Field.KpiType.NeedGraphic() ? HorizontalAlignment.Center : HorizontalAlignment.Right;
				return HorizontalAlignment.Left;
			}
		}
		public bool IsTotalAppearance { get { return Item.IsTotalAppearance || Item.IsCustomTotalAppearance || Item.IsGrandTotalAppearance; } }
		public override string ToString() {
			return string.Format("CellsAreaItem {0},{1},{2},{3}", MinIndex, MaxIndex, MinLevel, MaxLevel);
		}
	}
#if !SL
	public abstract class PivotContentPresenter : ContentPresenter {
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			if(e.Property == DataContextProperty)
				OnDataContextChanged();
		}
		protected virtual void OnDataContextChanged() { }
	}
#else
	public abstract class PivotContentPresenter : ContentControl {
		public PivotContentPresenter() {
			VerticalContentAlignment = System.Windows.VerticalAlignment.Stretch;
			HorizontalContentAlignment = System.Windows.HorizontalAlignment.Stretch;
			IsTabStop = false;
		}
		public new static readonly DependencyProperty ContentTemplateProperty =
			DependencyPropertyManager.Register("ContentTemplate", typeof(DataTemplate), typeof(PivotContentPresenter), new PropertyMetadata(null, (d, e) => ((PivotContentPresenter)d).OnContentTemplateSelectorChanged()));
		public static readonly DependencyProperty ContentTemplateSelectorProperty =
			DependencyPropertyManager.Register("ContentTemplateSelector", typeof(DataTemplateSelector), typeof(PivotContentPresenter), new PropertyMetadata(null, (d,e) => ((PivotContentPresenter)d).OnContentTemplateSelectorChanged()));
		public new static readonly DependencyProperty DataContextProperty =
			DependencyPropertyManager.Register("DataContext", typeof(object), typeof(PivotContentPresenter), new PropertyMetadata(null, (d, e) => ((PivotContentPresenter)d).OnDataContextChanged()));
		public new DataTemplate ContentTemplate {
			get { return (DataTemplate)GetValue(ContentTemplateProperty); }
			set { SetValue(ContentTemplateProperty, value); }
		}
		public DataTemplateSelector ContentTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ContentTemplateSelectorProperty); }
			set { SetValue(ContentTemplateSelectorProperty, value); }
		}
		public new object DataContext {
			get { return (object)GetValue(DataContextProperty); }
			set { SetValue(DataContextProperty, value); }
		}
		void OnContentTemplateSelectorChanged() {
			UpdateContentTemplate();
		}
		protected virtual void OnDataContextChanged() {
			base.DataContext = this.DataContext;
			UpdateContentTemplate();
		}
		protected bool isOwnTemplate;
		protected void UpdateContentTemplate() {
			if(ContentTemplate != null && !isOwnTemplate) {
				base.ContentTemplate = ContentTemplate;
				return;
			}
			if(DataContext == null && ContentTemplateSelector != null)
				return;
			if(ContentTemplateSelector == null) {
				if(isOwnTemplate)
					base.ContentTemplate = null;
				return;
			}
			isOwnTemplate = true;
			base.ContentTemplate = ContentTemplateSelector.SelectTemplate(DataContext, this);
		}
	}
#endif
	public abstract class ScrollableAreaItemContentPresenter : PivotContentPresenter {
		protected abstract string TemplateString { get; }
		protected abstract string TemplateSelectorString { get; }
		protected ScrollableAreaItemContentPresenter() { }
		protected override void OnDataContextChanged() {
			base.OnDataContextChanged();
			UpdateContentTemplateCore(DataContext as ScrollableAreaItemBase);
		}
		int bindHache;
		bool binded;
		protected void UpdateContentTemplateCore(ScrollableAreaItemBase item) {
			if(item == null) {
				if(!binded)
					return;
				binded = false;
#if !SL
				BindingOperations.ClearBinding(this, ContentTemplateProperty);
				BindingOperations.ClearBinding(this, ContentTemplateSelectorProperty);
#else
				ClearValue(ContentTemplateProperty);
				ClearValue(ContentTemplateSelectorProperty);
				isOwnTemplate = false;
#endif
			} else {
				if(binded && item.Field.GetHashCode() == bindHache)
					return;
				binded = true;
#if SL
				isOwnTemplate = false;
#endif
				bindHache = item.Field.GetHashCode();
				SetBinding(ContentTemplateProperty, new Binding(TemplateString) {
					Mode = BindingMode.OneWay,
					Source = item.Field
				});
				SetBinding(ContentTemplateSelectorProperty, new Binding(TemplateSelectorString) {
					Mode = BindingMode.OneWay,
					Source = item.Field
				});
			}
		}
	}
	[
	DefaultProperty("Content"), ContentProperty("Content"),
	Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)
	]
	public class FieldDataValueContentPresenter : ScrollableAreaItemContentPresenter {
		protected override string TemplateString { get { return "ActualValueTemplate"; } }
		protected override string TemplateSelectorString { get { return "ActualValueTemplateSelector"; } }
	}
	[
	DefaultProperty("Content"), ContentProperty("Content"),
	Localizability(LocalizationCategory.None, Readability = Readability.Unreadable)
	]
	public class FieldDataCellContentPresenter : ScrollableAreaItemContentPresenter {
		protected override string TemplateString { get { return "ActualCellTemplate"; } }
		protected override string TemplateSelectorString { get { return "ActualCellTemplateSelector"; } }
	}
	public class CellsItemsList : ScrollableItemsList {
		protected readonly PivotVisualItems visualItems;
		readonly bool isEmpty;
		readonly int count;
		public CellsItemsList(PivotVisualItems visualItems) {
			this.visualItems = visualItems;
			this.isEmpty = !(visualItems != null && visualItems.Data != null &&
				visualItems.Data.GetFieldsByArea(FieldArea.DataArea, false).Count > 0);
			this.count = isEmpty ? 0 : ColumnCount * RowCount;
		}
		public override int RowCount {
			get { return visualItems == null ? 0 : visualItems.RowCount; }
		}
		public override int ColumnCount {
			get { return visualItems == null ? 0 : visualItems.ColumnCount; }
		}
		public override int Count {
			get { return count; }
		}
		protected override ScrollableAreaItemBase CreateItem(int index) {
			int columnIndex = index % ColumnCount,
				rowIndex = index / ColumnCount;
			return new CellsAreaItem(visualItems, columnIndex, rowIndex);
		}
		public override int GetRowIndex(int index) {
			return index * ColumnCount;
		}
	}
}
