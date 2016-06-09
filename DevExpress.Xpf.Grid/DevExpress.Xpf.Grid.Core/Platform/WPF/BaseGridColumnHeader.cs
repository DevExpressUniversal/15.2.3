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
using DevExpress.Data;
using DevExpress.Mvvm.Native;
namespace DevExpress.Xpf.Grid {
#if !SL
	public abstract partial class GridColumnHeaderBase {
		void OnColumnPositionChanged() {
			UpdateHasEmptySiblingState();
		}
		protected virtual void UpdateHasEmptySiblingState() { }
		void OnHasBottomElementChanged() { }
		protected virtual void UpdateDesignTimeSelectionControl() { }
	}
	public abstract partial class BaseGridHeader {
		internal const string ColumnHeaderContentTemplateName = "PART_LayoutPanel";
		protected ColumnHeaderDockPanel LayoutPanel { get; private set; }
		protected Decorator ContentBorder { get; private set; }
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			LayoutPanel = GetTemplateChild(ColumnHeaderContentTemplateName) as ColumnHeaderDockPanel;
			ResetPanelChildren();
			HeaderContent = LayoutPanel;
			ContentBorder = GetTemplateChild("PART_Border") as Decorator;
			UpdateHeaderPresenter();
			UpdateHeaderCustomizationArea();
			UpdateHeaderContainer();
			UpdateDesignTimeSelectionControl();
#if DEBUGTEST
			DragDropHelper = new GridDragDropElementHelper(this);
			if(ForceCreateGripper)
				UpdateGripper();
#endif
		}
		protected virtual void ResetPanelChildren() {
			TextBlock = null;
			HeaderPresenter = null;
			HeaderGripper = null;
			HeaderCustomizationArea = null;
		}
		protected bool UseDefaultTemplate(ActualTemplateSelectorWrapper wrapper) {
#if DEBUGTEST
			if(ForceUseCustomTemplate)
				return false;
#endif
			return (wrapper.Template is DefaultDataTemplate || wrapper.Template == null) && (wrapper.Selector == null);
		}
		protected TextBlock TextBlock { get; private set; }
		protected XPFContentControl HeaderPresenter { get; private set; }
		protected void UpdateHeaderPresenter() {
			if(BaseColumn == null || LayoutPanel == null) return;
			if(UseDefaultTemplate((ActualTemplateSelectorWrapper)BaseColumn.ActualHeaderTemplateSelector) && (Column == null || Column.ActualColumnHeaderContentStyle == null))
				UpdateTextBlock();
			else
				UpdateCustomHeaderPresenter();
		}
		void UpdateTextBlock() {
			if(TextBlock == null)
				TextBlock = CreateTextBlock();
			if(HeaderPresenter != null)
				HeaderPresenter = null;
			LayoutPanel.HeaderPresenter = TextBlock;
			TextBlock.Text = GetActualHeaderContent(GetCaption()).ToString();
			TextBlock.VerticalAlignment = System.Windows.VerticalAlignment.Center;
		}
		object GetCaption() {
			return (HeaderPresenterType == HeaderPresenterType.ColumnChooser && Column != null) ? Column.ActualColumnChooserHeaderCaption : BaseColumn.HeaderCaption;
		}
		protected virtual TextBlock CreateTextBlock() {
			TextBlock textBlock = new TextBlock() { Name = "PART_Content" };
			textBlock.TextTrimming = TextTrimming.CharacterEllipsis;
			DevExpress.Xpf.Editors.Helpers.TextBlockService.SetAllowIsTextTrimmed(textBlock, true);
			DevExpress.Xpf.Editors.Helpers.TextBlockService.AddIsTextTrimmedChangedHandler(textBlock, OnIsTextTrimmedChanged);
			return textBlock;
		}
		void OnIsTextTrimmedChanged(object o, RoutedEventArgs e) {
			TextBlock textBlock = o as TextBlock;
			if(textBlock == null || BaseColumn.HeaderToolTip != null)
				return;
			ToolTipService.SetToolTip(textBlock, DevExpress.Xpf.Editors.Helpers.TextBlockService.GetIsTextTrimmed(textBlock) ? textBlock.Text : DependencyProperty.UnsetValue);
		}
		void UpdateCustomHeaderPresenter() {
			if(HeaderPresenter == null)
				HeaderPresenter = CreateCustomHeaderPresenter();
			if(TextBlock != null)
				TextBlock = null;
			LayoutPanel.HeaderPresenter = HeaderPresenter;
			HeaderPresenter.Content = GetActualHeaderContent(GetCaption());
			HeaderPresenter.ContentTemplateSelector = BaseColumn.ActualHeaderTemplateSelector;
			if(Column != null)
				HeaderPresenter.Style = Column.ActualColumnHeaderContentStyle;
			SetGridColumn(HeaderPresenter, BaseColumn);
		}
		protected virtual XPFContentControl CreateCustomHeaderPresenter() {
			return new XPFContentControl();
		}
#if DEBUGTEST
		public static bool ForceCreateGripper { get; set; }
#endif
		void UpdateGripper() {
			if(BaseColumn == null || LayoutPanel == null)
				return;
#if DEBUGTEST
				if(!IsMouseOver && !ForceCreateGripper)
#else
					if(!IsMouseOver)
#endif
					return;
			if(HeaderGripper == null && HeaderPresenterType == HeaderPresenterType.Headers) {
				HeaderGripper = CreateGripper();
				HeaderGripper.Cursor = Cursors.SizeWE;
				LayoutPanel.Children.Add(HeaderGripper);
				new ResizeHelper(new ColumnResizeHelperOwner(this)).Init(HeaderGripper);
			}
			if(HeaderGripper == null) return;
			HeaderGripper.HorizontalAlignment = ((IColumnPropertyOwner)this).GetActualFixedStyle() == FixedStyle.Right ? HorizontalAlignment.Left : HorizontalAlignment.Right;
			HeaderGripper.Visibility = BaseColumn.ActualAllowResizing ? Visibility.Visible : Visibility.Collapsed;
		}
		protected virtual Thumb CreateGripper() {
			return new Thumb();
		}
		protected override void OnMouseEnter(System.Windows.Input.MouseEventArgs e) {
			base.OnMouseEnter(e);
			UpdateGripper();
			if(DragDropHelper == null)
				DragDropHelper = new GridDragDropElementHelper(this);
		}
		protected ContentPresenter HeaderCustomizationArea { get; private set; }
		void UpdateHeaderCustomizationArea() {
			if(Column == null || LayoutPanel == null) return;
			if(UseDefaultTemplate((ActualTemplateSelectorWrapper)Column.ActualHeaderCustomizationAreaTemplateSelector) || HeaderPresenterType == HeaderPresenterType.ColumnChooser) {
				HeaderCustomizationArea = null;
				LayoutPanel.HeaderCustomizationArea = null;
				return;
			}
			if(HeaderCustomizationArea == null) {
				HeaderCustomizationArea = CreateHeaderCustomizationArea();
				LayoutPanel.HeaderCustomizationArea = HeaderCustomizationArea;
			}
			HeaderCustomizationArea.Content = GetColumnData();
			HeaderCustomizationArea.ContentTemplateSelector = Column.ActualHeaderCustomizationAreaTemplateSelector;
		}
		protected virtual ContentPresenter CreateHeaderCustomizationArea() {
			return new ContentPresenter();
		}
		protected virtual void UpdateHasRightSiblingState() {
			if(BaseColumn == null)
				return;
			HasRightSibling = HeaderPresenterType == HeaderPresenterType.Headers && BaseColumn.HasRightSibling;
		}
		protected override void UpdateHasEmptySiblingState() {
			if(BaseColumn == null)
				return;
			HasLeftSibling = ColumnPosition == Grid.ColumnPosition.Left && BaseColumn.HasLeftSibling;
		}
		protected void UpdateHeaderStyleProperty() {			
			if(BaseColumn == null)
				return;
			if(BaseColumn.HeaderStyle == null)
				ClearValue(FrameworkElement.StyleProperty);			
			else
				Style = BaseColumn.HeaderStyle;
		}
		protected virtual void UpdateHeaderContainer() {
			if(LayoutPanel == null || BaseColumn == null)
				return;
			HorizontalAlignment alignment = BaseColumn.HorizontalHeaderContentAlignment;
			LayoutPanel.ContainerAlignment = alignment;
			HeaderPresenter.Do(x => x.HorizontalAlignment = alignment);
		}
		protected override void UpdateDesignTimeSelectionControl() {
			if((DesignTimeSelectionControl == null && !GetIsSelectedInDesignTime(this)) || ContentBorder == null)
				return;
			if(DesignTimeSelectionControl == null && GetIsSelectedInDesignTime(this)) {
				DesignTimeSelectionControl = CreateDesignTimeSelectionControl();
				ContentBorder.Child = DesignTimeSelectionControl;
			}
			else if(DesignTimeSelectionControl != null && !GetIsSelectedInDesignTime(this)) {
				ContentBorder.Child = null;
				DesignTimeSelectionControl = null;
			}
		}
		protected virtual FrameworkElement CreateDesignTimeSelectionControl() {
			return new Control();
		}
	}
	public partial class BaseGridColumnHeader {
		public bool ShowFilterButtonOnHover {
			get { return (bool)GetValue(ShowFilterButtonOnHoverProperty); }
			set { SetValue(ShowFilterButtonOnHoverProperty, value); }
		}
		public static readonly DependencyProperty ShowFilterButtonOnHoverProperty = DependencyProperty.Register("ShowFilterButtonOnHover", typeof(bool), typeof(BaseGridColumnHeader), new PropertyMetadata(true, (d, e) => ((BaseGridColumnHeader)d).UpdateIsFilterButtonVisible()));
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdateIsFilterButtonVisible();
			UpdateSortIndicators();
			UpdateAllowHighlighting();
		}
		protected override void ResetPanelChildren() {
			base.ResetPanelChildren();
			ColumnFilterPopup = null;
			SortIndicator = null;
		}
		PopupBaseEdit columnFilterPopup;
		public PopupBaseEdit ColumnFilterPopup {
			get { return columnFilterPopup; }
			set {
				if(columnFilterPopup == value) return;
				PopupBaseEdit oldValue = columnFilterPopup;
				columnFilterPopup = value;
				OnColumnFilterPopupChanged(oldValue);
			}
		}
		protected void UpdateColumnFilterPopup() {
			if(ColumnFilterPopup != null)
				ColumnFilterPopup = CreateFilterPopup();
		}
		protected virtual void UpdateColumnFilterPopup(bool allowHide) {
			if(Column == null || LayoutPanel == null) return;
			bool isFilterButtonHidden = ShowFilterButtonOnHover && !IsMouseOver && !(ColumnFilterPopup != null && ColumnFilterPopup.IsPopupOpen) && !Column.IsFiltered;
			bool isFilterButtonCollapsed = HeaderPresenterType == Grid.HeaderPresenterType.ColumnChooser || !DesignerHelper.GetValue(this, Column.ActualAllowColumnFiltering, false);
#if DEBUGTEST
			if(ForceSetColumnFilterContainerVisible || !allowHide) {
#else
				if(!allowHide) {
#endif
				isFilterButtonHidden = false;
				isFilterButtonCollapsed = false;
			}
			if(isFilterButtonCollapsed) {
				ColumnFilterPopup = null;
				LayoutPanel.FilterPresenter = null;
				return;
			}
			if(isFilterButtonHidden && HeaderPresenterType != Grid.HeaderPresenterType.GroupPanel) {
				if(ColumnFilterPopup != null) ColumnFilterPopup.Visibility = Visibility.Collapsed;
				return;
			}
			if(ColumnFilterPopup == null)
				ColumnFilterPopup = CreateFilterPopup();
			if(LayoutPanel.FilterPresenter != ColumnFilterPopup)
				LayoutPanel.FilterPresenter = ColumnFilterPopup;
			ColumnFilterPopup.Visibility = (isFilterButtonHidden && HeaderPresenterType == Grid.HeaderPresenterType.GroupPanel) ? Visibility.Hidden : Visibility.Visible;
			ColumnFilterPopup.Style = ColumnFilterPopupStyle;
			SetFilterHitTestAcceptor(ColumnFilterPopup);
		}
		protected virtual PopupBaseEdit CreateFilterPopup() {
			PopupBaseEdit columnFilterPopup = Column.ColumnFilterInfo.CreateColumnFilterPopup();
			columnFilterPopup.IgnorePopupSizeConstraints = true;
			return columnFilterPopup;
		}
		protected void UpdateIsFilterButtonVisible() {
			UpdateColumnFilterPopup(true);
		}
		protected virtual void UpdateActualShowFilterButton() {
			UpdateColumnFilterPopup(true);
		}
		protected virtual void SetFilterHitTestAcceptor(PopupBaseEdit popup) {
		}
#if DEBUGTEST
		public void CreateFilterButtonForce() {
			bool isForceVisible = ForceSetColumnFilterContainerVisible;
			ForceSetColumnFilterContainerVisible = true;
			try {
				UpdateColumnFilterPopup(true);
			}
			finally {
				ForceSetColumnFilterContainerVisible = isForceVisible;
			}
		}
#endif
		protected FrameworkElement SortIndicator { get; private set;}
		void UpdateSortIndicators() {
			if(Column == null || LayoutPanel == null) return;
			if(Column.SortOrder == ColumnSortOrder.None) {
				if(SortIndicator != null) SortIndicator.Visibility = Visibility.Collapsed;
				return;
			}
			if(SortIndicator == null) {
				SortIndicator = CreateSortIndicator();
				LayoutPanel.SortIndicator = SortIndicator;
			}
			UpdateSortIndicator(Column.SortOrder == ColumnSortOrder.Ascending);
			SortIndicator.Visibility = Visibility.Visible;
		}
		protected virtual FrameworkElement CreateSortIndicator() {
			return null;
		}
		protected virtual void UpdateSortIndicator(bool isAscending) {
		}
	}
#endif
	public class ColumnHeaderDockPanel : Panel {
		public static readonly DependencyProperty ContentMarginProperty = DependencyProperty.Register("ContentMargin", typeof(Thickness), typeof(ColumnHeaderDockPanel), new FrameworkPropertyMetadata(new Thickness(0)));
		public static readonly DependencyProperty ContainerAlignmentProperty = DependencyProperty.Register("ContainerAlignment", typeof(HorizontalAlignment), typeof(ColumnHeaderDockPanel), new FrameworkPropertyMetadata(HorizontalAlignment.Left, FrameworkPropertyMetadataOptions.AffectsArrange));
		public Thickness ContentMargin {
			get { return (Thickness)GetValue(ContentMarginProperty); }
			set { SetValue(ContentMarginProperty, value); }
		}
		public HorizontalAlignment ContainerAlignment {
			get { return (HorizontalAlignment)GetValue(ContainerAlignmentProperty); }
			set { SetValue(ContainerAlignmentProperty, value); }
		}
		UIElement headerPresenterCore;
		public UIElement HeaderPresenter {
			get { return headerPresenterCore; }
			set {
				if(headerPresenterCore == value)
					return;
				UIElement oldValue = headerPresenterCore;
				headerPresenterCore = null;
				Children.Remove(oldValue);
				if(value != null) {
					headerPresenterCore = value;
					Children.Add(value);
				}
			}
		}
		UIElement headerCustomizationAreaCore;
		public UIElement HeaderCustomizationArea {
			get { return headerCustomizationAreaCore; }
			set {
				if(headerCustomizationAreaCore == value)
					return;
				UIElement oldValue = headerCustomizationAreaCore;
				headerCustomizationAreaCore = null;
				Children.Remove(oldValue);
				if(value != null) {
					headerCustomizationAreaCore = value;
					Children.Add(value);
				}
			}
		}
		UIElement filterCore;
		public UIElement FilterPresenter {
			get { return filterCore; }
			set {
				if(filterCore == value)
					return;
				UIElement oldValue = filterCore;
				filterCore = null;
				Children.Remove(oldValue);
				if(value != null) {
					filterCore = value;
					Children.Add(value);
				}
			}
		}
		UIElement sortIndicatorCore;
		public UIElement SortIndicator {
			get { return sortIndicatorCore; }
			set {
				if(sortIndicatorCore == value)
					return;
				UIElement oldValue = sortIndicatorCore;
				sortIndicatorCore = null;
				Children.Remove(oldValue);
				if(value != null) {
					sortIndicatorCore = value;
					Children.Add(value);
				}
			}
		}
		bool IsDocked(UIElement element) {
			return element == HeaderPresenter || element == HeaderCustomizationArea || element == FilterPresenter || element == SortIndicator;
		}
		protected override Size MeasureOverride(Size constraint) {
			double width = ContentMargin.Left + ContentMargin.Right;
			double height = 0d;
			foreach(UIElement child in Children)
				if(!IsDocked(child))
					child.Measure(new Size(constraint.Width, constraint.Height));
			if(SortIndicator != null) {
				width = Measure(SortIndicator, width, constraint);
				height = GetHeight(SortIndicator, height);
			}
			if(HeaderCustomizationArea != null) {
				width = Measure(HeaderCustomizationArea, width, constraint);
				height = GetHeight(HeaderCustomizationArea, height);
			}
			if(FilterPresenter != null) {
				width = Measure(FilterPresenter, width, constraint);
				height = GetHeight(FilterPresenter, height);
			}
			if(HeaderPresenter != null) {
				width = Measure(HeaderPresenter, width, constraint);
				height = GetHeight(HeaderPresenter, height);
			}
			height += ContentMargin.Top + ContentMargin.Bottom;
			return new Size(Math.Min(width, constraint.Width), Math.Min(height, constraint.Height));
		}
		double Measure(UIElement element, double width, Size constraint) {
			element.Measure(new Size(Math.Max(0, constraint.Width - width), constraint.Height));
			return width + element.DesiredSize.Width;
		}
		double GetHeight(UIElement element, double currentHeight) {
			return Math.Max(currentHeight, element.DesiredSize.Height);
		}
		protected override Size ArrangeOverride(Size finalSize) {
			double left = ContentMargin.Left;
			double right = ContentMargin.Right;
			double height = Math.Max(0, finalSize.Height - (ContentMargin.Top + ContentMargin.Bottom));
			foreach(UIElement child in Children) {
				if(!IsDocked(child))
					child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
			}
			if(SortIndicator != null)
				right = ArrangeRight(SortIndicator, right, finalSize.Width, height, ContentMargin.Top);
			if(HeaderCustomizationArea != null)
				right = ArrangeRight(HeaderCustomizationArea, right, finalSize.Width, height, ContentMargin.Top);
			double filterHeight = height;
			double filterTop = ContentMargin.Top;
			if(HeaderPresenter != null && FilterPresenter != null) {
				double headerHeight = HeaderPresenter.DesiredSize.Height;
				if(headerHeight > FilterPresenter.DesiredSize.Height && headerHeight < height) {
					filterHeight = headerHeight;
					filterTop = ContentMargin.Top + (height - filterHeight) / 2;
				}
			}
			switch(ContainerAlignment) {
				case System.Windows.HorizontalAlignment.Left:
					if(HeaderPresenter != null)
						left = ArrangeLeft(HeaderPresenter, left, height, ContentMargin.Top);
					if(FilterPresenter != null)
						left = ArrangeLeft(FilterPresenter, left, filterHeight, filterTop);
					break;
				case System.Windows.HorizontalAlignment.Right:
					if(FilterPresenter != null)
						right = ArrangeRight(FilterPresenter, right, finalSize.Width, filterHeight, filterTop);
					if(HeaderPresenter != null)
						right = ArrangeRight(HeaderPresenter, right, finalSize.Width, height, ContentMargin.Top);
					break;
				case System.Windows.HorizontalAlignment.Stretch:
					if(FilterPresenter != null)
						right = ArrangeRight(FilterPresenter, right, finalSize.Width, filterHeight, filterTop);
					if(HeaderPresenter != null)
						left = ArrangeStretch(HeaderPresenter, left, right, finalSize.Width, height);
					break;
				case System.Windows.HorizontalAlignment.Center:
					if(HeaderPresenter != null) {
						double total = (HeaderPresenter.DesiredSize.Width + (FilterPresenter != null ? FilterPresenter.DesiredSize.Width : 0d));
						double indent = Math.Max(0, left + (finalSize.Width - left - right - total) / 2);
						left = ArrangeCenter(HeaderPresenter, height, indent);
					}
					if(FilterPresenter != null)
						left = ArrangeLeft(FilterPresenter, left, filterHeight, filterTop);
					break;
			}
			return finalSize;
		}
		double ArrangeLeft(UIElement element, double left, double height, double top) {
			element.Arrange(new Rect(left, top, element.DesiredSize.Width, height));
			return left + element.DesiredSize.Width;
		}
		double ArrangeRight(UIElement element, double right, double width, double height, double top) {
			element.Arrange(new Rect(Math.Max(0, width - right - element.DesiredSize.Width), top, element.DesiredSize.Width, height));
			return right + element.DesiredSize.Width;
		}
		double ArrangeStretch(UIElement element, double left, double right, double width, double height) {
			element.Arrange(new Rect(left, ContentMargin.Top, Math.Max(0, width - (left + right)), height));
			return width - right;
		}
		double ArrangeCenter(UIElement element, double height, double indent) {
			element.Arrange(new Rect(indent, ContentMargin.Top, element.DesiredSize.Width, height));
			return indent + element.DesiredSize.Width;
		}
	}
}
