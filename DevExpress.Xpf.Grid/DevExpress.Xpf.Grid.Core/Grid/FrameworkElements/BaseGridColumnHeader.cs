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
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Utils;
using Thumb = DevExpress.Xpf.Core.DXThumb;
namespace DevExpress.Xpf.Grid {
	public abstract partial class GridColumnHeaderBase : Button {
		public static readonly DependencyProperty IsSelectedInDesignTimeProperty;
		public static readonly DependencyProperty ColumnPositionProperty;
		public static readonly DependencyProperty HasTopElementProperty;
		public static readonly DependencyProperty HasBottomElementProperty;
		public static readonly DependencyProperty HasRightSiblingProperty;
		public static readonly DependencyProperty HasLeftSiblingProperty;
		static GridColumnHeaderBase() {
			Type ownerType = typeof(GridColumnHeaderBase);
			IsSelectedInDesignTimeProperty = DependencyPropertyManager.RegisterAttached("IsSelectedInDesignTime", typeof(bool), ownerType, new PropertyMetadata(false, OnIsSelectedInDesignTimeChanged)); 
			ColumnPositionProperty = DependencyPropertyManager.Register("ColumnPosition", typeof(ColumnPosition), ownerType, new PropertyMetadata(ColumnPosition.Middle, (d, e) => ((GridColumnHeaderBase)d).OnColumnPositionChanged()));
			HasTopElementProperty = DependencyPropertyManager.Register("HasTopElement", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((GridColumnHeaderBase)d).OnColumnPositionChanged()));
			HasBottomElementProperty = DependencyPropertyManager.Register("HasBottomElement", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((GridColumnHeaderBase)d).OnHasBottomElementChanged()));
			HasRightSiblingProperty = DependencyProperty.Register("HasRightSibling", typeof(bool), typeof(GridColumnHeaderBase), new PropertyMetadata(true));
			HasLeftSiblingProperty = DependencyProperty.Register("HasLeftSibling", typeof(bool), typeof(GridColumnHeaderBase), new PropertyMetadata(false));
		}
		static void OnIsSelectedInDesignTimeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			if(d is GridColumnHeaderBase)
				((GridColumnHeaderBase)d).OnIsSelectedInDesignTimeChanged();
		}
		public static void SetIsSelectedInDesignTime(DependencyObject element, bool value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(IsSelectedInDesignTimeProperty, value);
		}
		public static bool GetIsSelectedInDesignTime(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(IsSelectedInDesignTimeProperty);
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridColumnHeaderBaseColumnPosition")]
#endif
		public ColumnPosition ColumnPosition {
			get { return (ColumnPosition)GetValue(ColumnPositionProperty); }
			set { SetValue(ColumnPositionProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridColumnHeaderBaseHasTopElement")]
#endif
		public bool HasTopElement {
			get { return (bool)GetValue(HasTopElementProperty); }
			set { SetValue(HasTopElementProperty, value); }
		}
#if !SL
	[DevExpressXpfGridCoreLocalizedDescription("GridColumnHeaderBaseHasBottomElement")]
#endif
		public bool HasBottomElement {
			get { return (bool)GetValue(HasBottomElementProperty); }
			set { SetValue(HasBottomElementProperty, value); }
		}
		public bool HasRightSibling {
			get { return (bool)GetValue(HasRightSiblingProperty); }
			set { SetValue(HasRightSiblingProperty, value); }
		}
		public bool HasLeftSibling {
			get { return (bool)GetValue(HasLeftSiblingProperty); }
			set { SetValue(HasLeftSiblingProperty, value); }
		}
		protected FrameworkElement DesignTimeSelectionControl { get; set; }
		void OnIsSelectedInDesignTimeChanged() {
			UpdateDesignTimeSelectionControl();
			OnIsSelectedInDesignTimeChangedCore();
		}
		protected virtual void OnIsSelectedInDesignTimeChangedCore() { }
#if DEBUGTEST
		internal string LastEmptySiblingState;
#endif  
	}
	public partial class BaseGridHeader : GridColumnHeaderBase, IColumnPropertyOwner, ISupportDragDropColumnHeader, ISupportDragDropPlatformIndependent {
		public static readonly DependencyProperty GridColumnProperty;
		public static readonly DependencyProperty DropPlaceOrientationProperty;
		public static readonly DependencyProperty DragElementTemplateProperty;
		internal const string DragElementTemplatePropertyName = "DragElementTemplate";
		static BaseGridHeader() {
			Type ownerType = typeof(BaseGridHeader);
			GridColumnProperty = DependencyPropertyManager.RegisterAttached("GridColumn", typeof(BaseColumn), ownerType, new FrameworkPropertyMetadata(null, OnGridColumnChanged));
			DropPlaceOrientationProperty = DependencyPropertyManager.RegisterAttached("DropPlaceOrientation", typeof(Orientation), ownerType, new PropertyMetadata(Orientation.Horizontal));
			DragElementTemplateProperty = DependencyPropertyManager.RegisterAttached(DragElementTemplatePropertyName, typeof(DataTemplate), ownerType, new PropertyMetadata(null));
		}
		public static void SetGridColumn(DependencyObject element, BaseColumn value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(GridColumnProperty, value);
		}
		public static BaseColumn GetGridColumn(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (BaseColumn)element.GetValue(GridColumnProperty);
		}
		public static void SetDropPlaceOrientation(DependencyObject element, Orientation value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(DropPlaceOrientationProperty, value);
		}
		public static Orientation GetDropPlaceOrientation(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (Orientation)element.GetValue(DropPlaceOrientationProperty);
		}
		public static void SetDragElementTemplate(DependencyObject element, DataTemplate value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(DragElementTemplateProperty, value);
		}
		public static DataTemplate GetDragElementTemplate(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (DataTemplate)element.GetValue(DragElementTemplateProperty);
		}
		static void OnGridColumnChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			BaseGridHeader columnHeader = d as BaseGridHeader;
			if(columnHeader != null)
				columnHeader.OnGridColumnChanged((BaseColumn)e.OldValue);
		}
		public BaseGridHeader() {
			ColumnContentChangedEventHandler = new ColumnContentChangedEventHandler<BaseGridHeader>(this, (owner, o, e) => owner.OnColumnContentChanged(o, e), (h, e) => ((BaseColumn)e).ContentChanged -= h.Handler);
		}
		protected ColumnContentChangedEventHandler<BaseGridHeader> ColumnContentChangedEventHandler { get; set; }
		protected virtual void OnColumnContentChanged(object sender, ColumnContentChangedEventArgs e) {
			if(e.Property == ColumnBase.ActualHeaderCustomizationAreaTemplateSelectorProperty)
				UpdateHeaderCustomizationArea();
			if(e.Property == ColumnBase.ActualAllowResizingProperty || e.Property == ColumnBase.FixedProperty)
				UpdateGripper();
			if(e.Property == BaseColumn.HeaderCaptionProperty || e.Property == BaseColumn.ActualHeaderTemplateSelectorProperty || e.Property == ColumnBase.ActualColumnChooserHeaderCaptionProperty || e.Property == ColumnBase.ActualColumnHeaderContentStyleProperty)
				UpdateHeaderPresenter();
			if(e.Property == BaseColumn.HorizontalHeaderContentAlignmentProperty)
				UpdateHeaderContainer();
			if(e.Property == BaseColumn.HeaderToolTipProperty || e.Property == BaseColumn.ActualHeaderToolTipTemplateProperty)
				UpdateTooltip();
			if(e.Property == BaseColumn.ColumnPositionProperty)
				UpdateColumnPosition();
			if(e.Property == BaseColumn.HasTopElementProperty)
				UpdateHasTopElement();
			if(e.Property == BaseColumn.HasBottomElementProperty)
				UpdateHasBottomElement();
			if(e.Property == BaseColumn.HasRightSiblingProperty)
				UpdateHasRightSiblingState();
			if(e.Property == BaseColumn.HasLeftSiblingProperty)
				UpdateHasEmptySiblingState();
			if(e.Property == BaseColumn.HeaderStyleProperty)
				UpdateHeaderStyleProperty();
		}
		protected virtual void OnGridColumnChanged(BaseColumn oldValue) {
			if(oldValue != null)
				oldValue.ContentChanged -= ColumnContentChangedEventHandler.Handler;
			if(BaseColumn != null)
				BaseColumn.ContentChanged += ColumnContentChangedEventHandler.Handler;
			UpdateGripper();
			UpdateHeaderPresenter();
			UpdateTooltip();
			UpdateColumnPosition();
			UpdateHasTopElement();
			UpdateHasBottomElement();
			UpdateHasRightSiblingState();
			UpdateHasEmptySiblingState();
			UpdateHeaderCustomizationArea();
			UpdateHeaderContainer();
			UpdateHeaderStyleProperty();
		}
		internal BaseColumn BaseColumn { get { return GetGridColumn(this); } }
		internal ColumnBase Column { get { return BaseColumn as ColumnBase; } }
		internal DataViewBase GridView { get { return BaseColumn.ResizeOwner as DataViewBase; } }
		internal DataViewBase RootGridView { get { return GridView.RootView; } }
		protected HeaderPresenterType HeaderPresenterType { get { return ColumnBase.GetHeaderPresenterType(this); } }
		protected Thumb HeaderGripper { get; private set; }
		internal FrameworkElement HeaderContent { get; private set; }
		internal protected virtual bool CanSyncColumnPosition { get; set; }
#if DEBUGTEST
		internal string LastHasRightSiblingState;
		public static bool ForceUseCustomTemplate { get; set; }
#endif  
		GridColumnData GetColumnData() {
			if(Column != null && Column.View != null && Column.View.HeadersData != null)
				return Column.View.HeadersData.GetCellDataByColumn(Column);
			return null;
		}
		protected object GetActualHeaderContent(object caption) {
			return caption == null || (caption is string && string.IsNullOrEmpty(caption as string)) ? " " : caption;
		}
		protected void UpdateColumnPosition() {
			if(BaseColumn == null || !CanSyncColumnPosition) return;
			ColumnPosition = BaseColumn.ColumnPosition;
		}
		protected virtual void UpdateHasTopElement() {
			if(BaseColumn == null) return;
			HasTopElement = HeaderPresenterType == HeaderPresenterType.Headers ? BaseColumn.HasTopElement : false;
		}
		void UpdateHasBottomElement() {
			if(BaseColumn == null) return;
			HasBottomElement = BaseColumn.HasBottomElement;
		}
		protected void UpdateTooltip() {
			ToolTip toolTip = null;
			if(BaseColumn != null && (BaseColumn.HeaderToolTip != null || BaseColumn.ActualHeaderToolTipTemplate != null))
				toolTip = new ToolTip() { Content = BaseColumn.HeaderToolTip, ContentTemplate = BaseColumn.ActualHeaderToolTipTemplate };
			ToolTipService.SetToolTip(this, toolTip);
		}
		#region ISupportDragDrop Members
		FrameworkElement ISupportDragDrop.SourceElement { get { return this; } }
		bool ISupportDragDrop.CanStartDrag(object sender, System.Windows.Input.MouseButtonEventArgs e) {
			throw new NotImplementedException();
		}
		bool ISupportDragDropColumnHeader.SkipHitTestVisibleChecking {
			get { return BaseColumn == null || BaseColumn.View == null ? false : BaseColumn.View.DataControl.DesignTimeAdorner.IsDesignTime; }
		}
		bool ISupportDragDropPlatformIndependent.CanStartDrag(object sender, IndependentMouseButtonEventArgs e) {
			return CanStartDrag((DependencyObject)sender);
		}
		internal bool CanStartDrag(DependencyObject element) {
			if(!BaseColumn.CanStartDragSingleColumn && LayoutHelper.IsChildElement(this, element) && HeaderPresenterType == HeaderPresenterType.Headers)
				return false;
			if(!BaseColumn.ActualAllowMoving)
				return false;
			if(!GridView.CommitEditing())
				return false;
			return GridView.CanStartDrag(element as GridColumnHeaderBase);
		}
		internal BaseColumn draggedColumn;
		IDragElement ISupportDragDrop.CreateDragElement(Point offset) {
			draggedColumn = BaseColumn;
			return GridView.CreateDragElement(this, offset);
		}
		IDropTarget ISupportDragDrop.CreateEmptyDropTarget() {
			return CreateEmptyDropTargetCore();
		}
		protected virtual IDropTarget CreateEmptyDropTargetCore() {
			return GridView.CreateEmptyDropTarget();
		}
		IEnumerable<UIElement> ISupportDragDrop.GetTopLevelDropContainers() {
			return RootGridView.GetTopLevelDropContainers();
		}
		bool ISupportDragDrop.IsCompatibleDropTargetFactory(IDropTargetFactory factory, UIElement dropTargetElement) {
			return IsCompatibleDropTargetFactoryCore(factory, dropTargetElement);
		}
		protected virtual bool IsCompatibleDropTargetFactoryCore(IDropTargetFactory factory, UIElement dropTargetElement) {
			return factory is GridDropTargetFactoryBase && ((GridDropTargetFactoryBase)factory).IsCompatibleDropTargetFactory(dropTargetElement, this);
		}
		FrameworkElement ISupportDragDropColumnHeader.RelativeDragElement {
			get { return HeaderContent; }
		}
		FrameworkElement ISupportDragDropColumnHeader.TopVisual { get { return (FrameworkElement)LayoutHelper.GetTopLevelVisual(RootGridView); } }
		void ISupportDragDropColumnHeader.UpdateLocation(IndependentMouseEventArgs e) {
			GridView.ViewBehavior.UpdateLastPostition(e);
		}
		protected internal virtual DependencyObject CreateDragElementDataContext() {
			return null;
		}
		private DragDropElementHelper dragDropHelper;
		internal DragDropElementHelper DragDropHelper {
			get { return this.dragDropHelper; }
			private set {
				if(this.dragDropHelper != null)
					this.dragDropHelper.Destroy();
				this.dragDropHelper = value;
			}
		}
		#endregion
		#region IColumnPropertyOwner Members
		BaseColumn IColumnPropertyOwner.Column {
			get { return BaseColumn; }
		}
		FixedStyle IColumnPropertyOwner.GetActualFixedStyle() {
			if(BaseColumn == null || GridView == null) return FixedStyle.None;
			if(BaseColumn.ParentBandInternal == null || GridView.DataControl == null || GridView.DataControl.BandsLayoutCore == null)
				return BaseColumn.Fixed;
			return GridView.DataControl.BandsLayoutCore.GetRootBand(BaseColumn.ParentBandInternal).Fixed;
		}
		#endregion
	}
	public partial class BaseGridColumnHeader : BaseGridHeader, IColumnPropertyOwner {
		static readonly DependencyPropertyKey IsInDropAreaPropertyKey;
		public static readonly DependencyProperty IsInDropAreaProperty;
		public static readonly DependencyProperty CorrectDragIndicatorLocationProperty;
		static readonly DependencyPropertyKey DragElementSizePropertyKey;
		public static readonly DependencyProperty DragElementSizeProperty;
		static readonly DependencyPropertyKey DragElementAllowTransparencyPropertyKey;
		public static readonly DependencyProperty DragElementAllowTransparencyProperty;
		public static readonly DependencyProperty ColumnFilterPopupStyleProperty;
		static BaseGridColumnHeader() {
			Type ownerType = typeof(BaseGridColumnHeader);
			IsInDropAreaPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("IsInDropArea", typeof(bool), ownerType, new PropertyMetadata(false));
			IsInDropAreaProperty = IsInDropAreaPropertyKey.DependencyProperty;
			CorrectDragIndicatorLocationProperty = DependencyPropertyManager.RegisterAttached("CorrectDragIndicatorLocation", typeof(bool), ownerType, new PropertyMetadata(true));
			DragElementSizePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("DragElementSize", typeof(Size), ownerType, new PropertyMetadata(default(Size)));
			DragElementSizeProperty = DragElementSizePropertyKey.DependencyProperty;
			DragElementAllowTransparencyPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("DragElementAllowTransparency", typeof(bool), ownerType, new PropertyMetadata(true));
			DragElementAllowTransparencyProperty = DragElementAllowTransparencyPropertyKey.DependencyProperty;
			ColumnFilterPopupStyleProperty = DependencyPropertyManager.RegisterAttached("ColumnFilterPopupStyle", typeof(Style), ownerType, new PropertyMetadata(null, (d, e) => ((BaseGridColumnHeader)d).OnColumnFilterPopupStyleChanged()));
		}
		public static bool GetIsInDropArea(DependencyObject obj) {
			return (bool)obj.GetValue(IsInDropAreaProperty);
		}
		internal static void SetIsInDropArea(DependencyObject obj, bool value) {
			obj.SetValue(IsInDropAreaPropertyKey, value);
		}
		public static bool GetCorrectDragIndicatorLocation(DependencyObject obj) {
			return (bool)obj.GetValue(CorrectDragIndicatorLocationProperty);
		}
		public static void SetCorrectDragIndicatorLocation(DependencyObject obj, bool value) {
			obj.SetValue(CorrectDragIndicatorLocationProperty, value);
		}
		public static Size GetDragElementSize(DependencyObject obj) {
			return (Size)obj.GetValue(DragElementSizeProperty);
		}
		internal static void SetDragElementSize(DependencyObject obj, Size value) {
			obj.SetValue(DragElementSizePropertyKey, value);
		}
		public static bool GetDragElementAllowTransparency(DependencyObject obj) {
			return (bool)obj.GetValue(DragElementAllowTransparencyProperty);
		}
		internal static void SetDragElementAllowTransparency(DependencyObject obj, bool value) {
			obj.SetValue(DragElementAllowTransparencyPropertyKey, value);
		}
		public Style ColumnFilterPopupStyle {
			get { return (Style)GetValue(ColumnFilterPopupStyleProperty); }
			set { this.SetValue(ColumnFilterPopupStyleProperty, value); }
		}
		[Browsable(false)]
		public bool CanSyncWidth { get; set; }
		protected override void OnColumnContentChanged(object sender, ColumnContentChangedEventArgs e) {
			base.OnColumnContentChanged(sender, e);
			if(e.Property == ColumnBase.EditSettingsProperty || e.Property == ColumnBase.FilterPopupModeProperty)
				UpdateColumnFilterPopup();
			if(e.Property == ColumnBase.SortOrderProperty)
				UpdateSortIndicators();
			if(e.Property == ColumnBase.ActualHeaderWidthProperty)
				UpdateWidth();
			if(e.Property == ColumnBase.IsFilteredProperty)
				UpdateIsFilterButtonVisible();
			if(e.Property == ColumnBase.ActualAllowColumnFilteringProperty)
				UpdateActualShowFilterButton();
			if(e.Property == ColumnBase.ColumnPositionProperty)
				UpdateColumnPosition();
			if(e.Property == ColumnBase.HasTopElementProperty)
				UpdateHasTopElement();
			if(e.Property == ColumnBase.AllowSearchHeaderHighlightingProperty)
				UpdateAllowHighlighting();
		}
		protected void UpdateAllowHighlighting() {
			if(TextBlock == null || BaseColumn == null)
				return;
			if(BaseColumn.AllowSearchHeaderHighlighting)
				TextBlock.FontStyle = FontStyles.Italic;
			else
				TextBlock.ClearValue(TextBlock.FontStyleProperty);
		}
		protected override void OnGridColumnChanged(BaseColumn oldValue) {
			base.OnGridColumnChanged(oldValue);
			UpdateIsFilterButtonVisible();
			UpdateActualShowFilterButton();
			UpdateColumnFilterPopup();
			UpdateSortIndicators();
			UpdateWidth();
			UpdateHeaderPresenter();
			UpdateDesignTimeSelection();
			UpdateAllowHighlighting();
		}
		private void UpdateDesignTimeSelection() {
			if(Column == null) return;
			SetIsSelectedInDesignTime(this, GetIsSelectedInDesignTime(Column));
		}
		void OnColumnFilterPopupChanged(PopupBaseEdit oldValue) {
			if(oldValue != null) {
				oldValue.PopupOpening -= ColumnFilterPopupOpening;
				oldValue.PopupOpened -= new RoutedEventHandler(ColumnFilterPopupOpened);
				oldValue.PopupClosed -= new ClosePopupEventHandler(ColumnFilterPopupClosed);
			}
			if(ColumnFilterPopup != null) {
				ColumnFilterPopup.PopupOpening += ColumnFilterPopupOpening;
				ColumnFilterPopup.PopupOpened += new RoutedEventHandler(ColumnFilterPopupOpened);
				ColumnFilterPopup.PopupClosed += new ClosePopupEventHandler(ColumnFilterPopupClosed);
			}
			UpdateIsFilterButtonVisible();
		}
		void ColumnFilterPopupOpening(object sender, OpenPopupEventArgs e) {
			UpdateColumnFilterPopup(false);
			if(string.IsNullOrEmpty(Column.FieldName)) {
				e.Cancel = true;
				return;
			}
			Column.ColumnFilterInfo.OnPopupOpening(ColumnFilterPopup);
		}
		protected override void OnMouseEnter(MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateIsFilterButtonVisible();
		}
		protected override void OnMouseLeave(MouseEventArgs e) {
			base.OnMouseLeave(e);
			UpdateIsFilterButtonVisible();
		}
		void ColumnFilterPopupOpened(object sender, RoutedEventArgs e) {
			Column.ColumnFilterInfo.OnPopupOpened(ColumnFilterPopup);
		}
		void ColumnFilterPopupClosed(object sender, ClosePopupEventArgs e) {
			UpdateIsFilterButtonVisible();
			Column.ColumnFilterInfo.OnPopupClosed(ColumnFilterPopup, e.CloseMode != PopupCloseMode.Cancel);
		}
		void UpdateWidth() {
			if(Column != null && CanSyncWidth && !Double.IsInfinity(Column.ActualHeaderWidth)) Width = Column.ActualHeaderWidth;
		}
		void OnColumnFilterPopupStyleChanged() {
			if(ColumnFilterPopup != null)
				ColumnFilterPopup.Style = ColumnFilterPopupStyle;
		}
		protected override void UpdateHasTopElement() {
			if(Column != null && Column.OwnerControl != null && Column.OwnerControl.BandsLayoutCore != null) {
				if(Column.OwnerControl.BandsLayoutCore.ShowBandsPanel && HeaderPresenterType == HeaderPresenterType.Headers) {
					HasTopElement = true;
					return;
				}
			}
			base.UpdateHasTopElement();
		}
		public static HeaderPresenterType GetHeaderPresenterTypeFromGridColumnHeader(DependencyObject d) {
#if DEBUGTEST
			System.Diagnostics.Debug.Assert(
				d is BaseGridHeader ||
				d is StackVisibleIndexPanel ||
				d is ColumnChooserControlBase ||
				d is Panel); 
#endif
			return GetHeaderPresenterTypeFromLocalValue(d);
		}
		public static HeaderPresenterType GetHeaderPresenterTypeFromLocalValue(DependencyObject d) {
#if DEBUGTEST
			System.Diagnostics.Debug.Assert(System.Windows.DependencyPropertyHelper.GetValueSource(d, ColumnBase.HeaderPresenterTypeProperty).BaseValueSource > BaseValueSource.Inherited);
#endif
			return ColumnBase.GetHeaderPresenterType(d);
		}
		internal static ColumnBase GetColumnByDragElement(UIElement element) {
			return (ColumnBase)GetGridColumn(LayoutHelper.FindParentObject<BaseGridColumnHeader>(element as DependencyObject));
		}
		protected internal override DependencyObject CreateDragElementDataContext() {
			return GridView.HeadersData.CreateCellDataByColumn(Column);
		}
#if DEBUGTEST
		internal static bool ForceSetColumnFilterContainerVisible;
#endif
		protected override void OnIsSelectedInDesignTimeChangedCore() {
			SetIsSelectedInDesignTime(Column, GetIsSelectedInDesignTime(this));
		}
	}
	public class ColumnResizeHelperOwner : IResizeHelperOwner {
		IColumnPropertyOwner columnPropertyOwner;
		public BaseColumn Column {
			get {
				return columnPropertyOwner.Column;
			}
		}
		DataViewBase View {
			get {
				return (Column == null) ? null : Column.ResizeOwner as DataViewBase;
			}
		}
		public ColumnResizeHelperOwner(IColumnPropertyOwner columnPropertyOwner) {
			this.columnPropertyOwner = columnPropertyOwner;
		}
		#region IResizeHelperOwner Members
		double maxWidth = double.MaxValue;
		double IResizeHelperOwner.ActualSize {
			get { return Column.Width; }
			set { SetWidth(value); }
		}
		SizeHelperBase IResizeHelperOwner.SizeHelper { get { return HorizontalSizeHelper.Instance; } }
		void IResizeHelperOwner.ChangeSize(double delta) {
			if(columnPropertyOwner.GetActualFixedStyle() == FixedStyle.Right)
				delta = -delta;
			SetWidth(columnPropertyOwner.ActualWidth + delta);
		}
		void IResizeHelperOwner.OnDoubleClick() {
			ColumnBase column = Column as ColumnBase;
			if(column != null)
				View.ViewBehavior.OnColumnResizerDoubleClick(column);
		}
		void SetWidth(double value) {
			View.ApplyResize(Column, value, maxWidth);
		}
		void IResizeHelperOwner.SetIsResizing(bool isResizing) {
			ColumnBase column = Column as ColumnBase;
			if(isResizing) {
				if(column != null)
					maxWidth = View.ViewBehavior.CalcColumnMaxWidth(column);
			}
			else
				View.ViewBehavior.OnResizingComplete();
		}
		#endregion
	}
	public interface IColumnPropertyOwner {
		BaseColumn Column { get; }
		double ActualWidth { get; }
		FixedStyle GetActualFixedStyle();
	}
}
