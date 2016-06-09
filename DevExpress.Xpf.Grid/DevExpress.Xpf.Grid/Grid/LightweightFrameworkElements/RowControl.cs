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
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Core;
using System.Linq;
using System.Windows.Data;
using System.ComponentModel;
using System.Collections;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core.Native;
using System.Collections.Generic;
using DevExpress.Xpf.Editors.Internal;
using System.Collections.Specialized;
using DevExpress.Xpf.Grid.HitTest;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.ConditionalFormatting;
namespace DevExpress.Xpf.Grid {
	public class RowControl : Control, IRowStateClient, IFocusedRowBorderObject, IGridDataRow, IConditionalFormattingClient<RowControl> {
		public SelectionState SelectionState {
			get { return (SelectionState)GetValue(SelectionStateProperty); }
			set { SetValue(SelectionStateProperty, value); }
		}
		public static readonly DependencyProperty SelectionStateProperty =
			DependencyProperty.Register("SelectionState", typeof(SelectionState), typeof(RowControl), new PropertyMetadata(SelectionState.None));
		public bool ShowHorizontalLine {
			get { return (bool)GetValue(ShowHorizontalLineProperty); }
			set { SetValue(ShowHorizontalLineProperty, value); }
		}
		public static readonly DependencyProperty ShowHorizontalLineProperty =
			DependencyProperty.Register("ShowHorizontalLine", typeof(bool), typeof(RowControl), new PropertyMetadata(true));
		public bool ShowVerticalLines {
			get { return (bool)GetValue(ShowVerticalLinesProperty); }
			set { SetValue(ShowVerticalLinesProperty, value); }
		}
		public static readonly DependencyProperty ShowVerticalLinesProperty =
			DependencyProperty.Register("ShowVerticalLines", typeof(bool), typeof(RowControl), new PropertyMetadata(true));
		public bool IsAlternateRow {
			get { return (bool)GetValue(IsAlternateRowProperty); }
			set { SetValue(IsAlternateRowProperty, value); }
		}
		public static readonly DependencyProperty IsAlternateRowProperty =
			DependencyProperty.Register("IsAlternateRow", typeof(bool), typeof(RowControl), new PropertyMetadata(false));
		public bool FadeSelection {
			get { return (bool)GetValue(FadeSelectionProperty); }
			set { SetValue(FadeSelectionProperty, value); }
		}
		public static readonly DependencyProperty FadeSelectionProperty =
			DependencyProperty.Register("FadeSelection", typeof(bool), typeof(RowControl), new PropertyMetadata(false));
		public bool ShowBottomLine {
			get { return (bool)GetValue(ShowBottomLineProperty); }
			set { SetValue(ShowBottomLineProperty, value); }
		}
		public static readonly DependencyProperty ShowBottomLineProperty =
			DependencyProperty.Register("ShowBottomLine", typeof(bool), typeof(RowControl), new PropertyMetadata(false));
		public Brush RowFitBorderBrush {
			get { return (Brush)GetValue(RowFitBorderBrushProperty); }
			set { SetValue(RowFitBorderBrushProperty, value); }
		}
		public static readonly DependencyProperty RowFitBorderBrushProperty =
			DependencyProperty.Register("RowFitBorderBrush", typeof(Brush), typeof(RowControl), new PropertyMetadata(null, (d, e) => ((RowControl)d).UpdateFitContentBorderBrush()));
		public bool ShowRowBreak {
			get { return (bool)GetValue(ShowRowBreakProperty); }
			set { SetValue(ShowRowBreakProperty, value); }
		}
		public static readonly DependencyProperty ShowRowBreakProperty =
			DependencyProperty.Register("ShowRowBreak", typeof(bool), typeof(RowControl), new PropertyMetadata(false, (d, e) => ((RowControl)d).UpdateIndicatorShowRowBreak()));
		const int IndicatorPosition = 0;
		const int LeftDetailIndentPosition = IndicatorPosition + 1;
		const int GroupOffsetPosition = LeftDetailIndentPosition + 1;
		const int DetailExpandButtonPosition = GroupOffsetPosition + 1;
		const int FixedLeftPosition = DetailExpandButtonPosition + 1;
		const int FixedLeftDelimiterPosition = FixedLeftPosition + 1;
		const int FixedNonePosition = FixedLeftDelimiterPosition + 1;
		const int FixedRightDelimiterPosition = FixedNonePosition + 1;
		const int FixedRightPosition = FixedRightDelimiterPosition + 1;
		const int FitContentPosition = FixedRightPosition + 1;
		const int RightDetailIndentPosition = FitContentPosition + 1;
		public BrushSet CellForegroundBrushes {
			get { return (BrushSet)GetValue(CellForegroundBrushesProperty); }
			set { SetValue(CellForegroundBrushesProperty, value); }
		}
		public static readonly DependencyProperty CellForegroundBrushesProperty =
			DependencyProperty.Register("CellForegroundBrushes", typeof(BrushSet), typeof(RowControl), new PropertyMetadata(null, (d, e) => ((RowControl)d).OnCellForegroundBrushesChanged((BrushSet)e.OldValue)));
		void OnCellForegroundBrushesChanged(BrushSet oldValue) {
			if(oldValue != null)
				rowData.UpdateCellForegroundAppearance();
		}
		public BrushSet CellBackgroundBrushes {
			get { return (BrushSet)GetValue(CellBackgroundBrushesProperty); }
			set { SetValue(CellBackgroundBrushesProperty, value); }
		}
		public static readonly DependencyProperty CellBackgroundBrushesProperty =
			DependencyProperty.Register("CellBackgroundBrushes", typeof(BrushSet), typeof(RowControl), new PropertyMetadata(null, (d, e) => ((RowControl)d).OnCellBackgroundBrushesChanged((BrushSet)e.OldValue)));
		void OnCellBackgroundBrushesChanged(BrushSet oldValue) {
			if(oldValue != null)
				rowData.UpdateCellBackgroundAppearance();
		}
		internal readonly RowData rowData;
		static RowControl() {
			Type ownerType = typeof(RowControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			GridViewHitInfoBase.HitTestAcceptorProperty.OverrideMetadata(ownerType, new PropertyMetadata(new RowTableViewHitTestAcceptor()));
		}
		public RowControl(RowData rowData) {
			this.rowData = rowData;
			formattingHelper = new ConditionalFormattingHelper<RowControl>(this, BackgroundProperty);
			rowData.SetRowStateClient(this);
		}
		readonly ConditionalFormattingHelper<RowControl> formattingHelper;
		protected internal CellsControl CellsControl { get; protected set; }
		CellsControl fixedLeftCellsControl;
		CellsControl fixedRightCellsControl;
		RowFixedLineSeparatorControl leftSeparator;
		RowFixedLineSeparatorControl rightSeparator;
		RowFitBorder fitContent;
		RowIndicator indicator;
		FrameworkElement offsetPresenter;
		GridDetailExpandButtonContainer detailExpandButtonContainer;
		DetailRowsIndentControl detailLeftIndentControl;
		DetailRowsIndentRightControl detailRightIndentControl;
		ContentPresenter contentPresenter;
		RowDetailsControl detailContentPresenter;
		IndentScroller indentScroller;
		EditFormContainer editFormContainerCore;
		EditFormContainer EditFormContainer {
			get { return editFormContainerCore; }
			set {
				if(editFormContainerCore != value) {
					editFormContainerCore = value;
					OnEditFormContainerChanged();
				}
			}
		}
		protected virtual bool AllowTreeIndentScrolling { get { return TableView.ActualAllowTreeIndentScrolling; } }
		bool ShowHorizontalLines { get { return TableView.ShowHorizontalLines; } }
		protected virtual bool ShowDetails { get { return rowData.RowHandle != null ? TableView.UseRowDetailsTemplate(rowData.RowHandle.Value) : false; } }
		FrameworkElement IFocusedRowBorderObject.RowDataContent { get { return backgroundBorder; } }
		double IFocusedRowBorderObject.LeftIndent { get { return CalculateRowContentIndent(); } }
		RowData IGridDataRow.RowData { get { return rowData; } }
		void IGridDataRow.UpdateContentLayout() {
			UpdateCellsState();
		}
		DataControlBase DataControl { get { return rowData.View.DataControl; } }
		internal BandsLayoutBase BandsLayout { get { return DataControl != null ? DataControl.BandsLayoutCore : null; } }
		internal bool IsBandedLayout { get { return BandsLayout != null; } }
		void UpdateCellsState() {
			UpdateCellsState(CellsControl);
			UpdateCellsState(fixedLeftCellsControl);
			UpdateCellsState(fixedRightCellsControl);
		}
		void UpdateCellsState(CellsControl cellsControl) {
			if(cellsControl == null) return;
			cellsControl.InvalidateMeasure();
			if(cellsControl.Panel == null) return;
			cellsControl.Panel.InvalidateMeasure();
		}
		void UpdateTriggerErrorState() {
		}
		void UpdateFocusWithinState() {
			DataViewBase rootView = rowData.View.RootView;
			if(rootView.DataControl != null)
				FadeSelection = FadeSelectionHelper.IsFadeNeeded(rootView.ActualFadeSelectionOnLostFocus, rootView.DataControl.IsKeyboardFocusWithin) && SelectionState != Grid.SelectionState.None;
		}
		void UpdateShowBottomLine() {
			this.ShowBottomLine = rowData.GetShowBottomLine();
		}
#if DEBUGTEST
		internal FrameworkElement GetTemplateChildInternal(string name) {
			return (FrameworkElement)GetTemplateChild(name);
		}
		internal CellsControl FixedRightCellsControlForTests { get { return fixedRightCellsControl; } }
		internal CellsControl FixedLeftCellsControlForTests { get { return fixedLeftCellsControl; } }
		internal RowFixedLineSeparatorControl LeftSeparatorForTests { get { return leftSeparator; } }
		internal RowFixedLineSeparatorControl RightSeparatorForTest { get { return rightSeparator; } }
		internal RowFitBorder FitContentForTests { get { return fitContent; } }
		internal RowIndicator IndicatorForTests { get { return indicator; } }
		internal RowOffsetPresenter OffsetPresenterForTests { get { return (RowOffsetPresenter)offsetPresenter; } }
		internal RowMarginControl RowMarginControlForTests { get { return (RowMarginControl)offsetPresenter; } }
		internal GridDetailExpandButtonContainer DetailExpandButtonContainerForTests { get { return detailExpandButtonContainer; } }
		internal DetailRowsIndentControl DetailLeftIndentControlForTests { get { return detailLeftIndentControl; } }
		internal DetailRowsIndentRightControl DetailRightIndentControlForTests { get { return detailRightIndentControl; } }
		internal System.Windows.Controls.Grid LayoutPanelForTests { get { return layoutPanel; } }
		internal IndentScroller IndentScrollerForTests { get { return indentScroller; } }
		internal EditFormContainer EditFormContainerForTests { get { return EditFormContainer; } }
#endif
		System.Windows.Controls.Grid layoutPanel;
		Border backgroundBorder;
		Border bottomLine;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			layoutPanel = (System.Windows.Controls.Grid)GetTemplateChild("PART_LayoutPanel");
			backgroundBorder = (Border)GetTemplateChild("Background");
			bottomLine = GetTemplateChild("BottomLine") as Border;
			UpdateBottomLineMargin(backgroundBorder, true);
			CreateContent();
		}
		void CreateContent() {
			ClearElements();
			if(UseTemplate)
				CreateTemplateContent();
			else
				CreateDefaultContent();
			oldUseTemplate = UseTemplate;
		}
		void ClearElements() {
			layoutPanel.Children.Clear();
			layoutPanel.ColumnDefinitions.Clear();
			layoutPanel.RowDefinitions.Clear();
			CellsControl = null;
			fixedLeftCellsControl = null;
			fixedRightCellsControl = null;
			leftSeparator = null;
			rightSeparator = null;
			fitContent = null;
			indicator = null;
			offsetPresenter = null;
			detailExpandButtonContainer = null;
			detailLeftIndentControl = null;
			detailRightIndentControl = null;
			contentPresenter = null;
			indentScroller = null;
			EditFormContainer = null;
		}
		protected override Size MeasureOverride(Size constraint) {
			Size size = base.MeasureOverride(constraint);
#if DEBUGTEST
			if(GridRow.RoundRowSize)
#endif
				size = new Size(Math.Ceiling(size.Width), Math.Ceiling(size.Height));
			return size;
		}
		bool oldUseTemplate;
		bool UseTemplate { get { return TableView.TableViewBehavior.UseDataRowTemplate(rowData); } }
		ITableView TableView { get { return (ITableView)rowData.View; } }
		protected virtual void CreateDefaultContent() {
			for(int i = 0; i < FitContentPosition; i++) {
				layoutPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
			}
			layoutPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			layoutPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
			if(AllowTreeIndentScrolling) {
				indentScroller = new IndentScroller();
				AddPanelElement(indentScroller, GroupOffsetPosition);
			}
			UpdateIndicator();
			UpdateOffsetPresenter();
			UpdateFitContent();
			CellsControl = CreateAndInitFixedNoneCellsControl(FixedNonePosition, x => x.FixedNoneCellData, x => x.FixedNoneVisibleBands);
			UpdateFixedNoneContentWidth();
			UpdateFixedLeftCellData(null);
			UpdateFixedRightCellData(null);
			UpdateScrollingMargin();
			UpdateDetailExpandButton();
			UpdateLeftDetailViewIndents();
			UpdateRightDetailViewIndents();
			UpdateDetails();
		}
		void UpdateOffsetPresenter() {
			if(rowData.Level > 0 || rowData.View.IsRowMarginControlVisible)
				UpdateOffsetPresenterLevel();
		}
		void UpdateFitContent() {
			fitContent = new RowFitBorder();
			GridPopupMenu.SetGridMenuType(fitContent, GridMenuType.RowCell);
			UpdateFitContentContextMenu();
			AddPanelElement(fitContent, FitContentPosition);
			UpdateFitContentBorderBrush();
			UpdateBottomLineMargin(fitContent, !ShowDetails);
		}
		void UpdateFitContentContextMenu() {
			if(fitContent != null)
				DevExpress.Xpf.Bars.BarManager.SetDXContextMenu(fitContent, rowData.View.DataControlMenu);
		}
		void UpdateIndicator() {
			indicator = new RowIndicator();
			GridPopupMenu.SetGridMenuType(indicator, GridMenuType.RowCell);
			System.Windows.Controls.Grid.SetRowSpan(indicator, 2);
			UpdateIndicatorContextMenu();
			AddPanelElement(indicator, IndicatorPosition);
			UpdateIndicatorWidth();
			if(!((ITableView)rowData.View).ActualShowIndicator)
				UpdateIndicatorVisibility();
			if(rowData.IndicatorState != IndicatorState.None)
				UpdateIndicatorState();
			if(rowData.HasValidationErrorInternal)
				UpdateRowValidationError();
			UpdateIndicatorShowRowBreak();
		}
		void UpdateIndicatorShowRowBreak() {
			if(indicator != null)
				indicator.ShowRowBreak = ShowRowBreak;
		}
		void UpdateIndicatorContextMenu() {
			if(indicator != null)
				DevExpress.Xpf.Bars.BarManager.SetDXContextMenu(indicator, rowData.View.DataControlMenu);
		}
		protected virtual void CreateTemplateContent() {
			for(int i = 0; i < FixedLeftPosition; i++) {
				layoutPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
			}
			layoutPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
			UpdateIndicator();
			UpdateOffsetPresenter();
			contentPresenter = new DataContentPresenter() { Content = rowData, ContentTemplateSelector = TableView.ActualDataRowTemplateSelector };
			UpdateBottomLineMargin(contentPresenter, true);
			AddPanelElement(contentPresenter, FixedLeftPosition);
		}
		protected virtual void UpdateOffsetPresenterLevel() {
			if(offsetPresenter == null && layoutPanel != null) {
				offsetPresenter = GridRowHelper.CreateRowOffsetContent(rowData, this);
				System.Windows.Controls.Grid.SetRowSpan(offsetPresenter, 2);
#if DEBUGTEST
				if(offsetPresenter != null)
					DevExpress.Xpf.Grid.Tests.GridTestHelper.SetSkipChildrenWhenStoreVisualTree(offsetPresenter, true);
#endif
				if(AllowTreeIndentScrolling)
					indentScroller.AddScrollableElement(offsetPresenter, 0);
				else
					AddPanelElement(offsetPresenter, GroupOffsetPosition);
			}
		}
		void UpdateDetailExpandButton() {
			bool actualShowDetailButtons = ((ITableView)rowData.View).ActualShowDetailButtons;
			if(actualShowDetailButtons && detailExpandButtonContainer == null && layoutPanel != null) {
				detailExpandButtonContainer = new GridDetailExpandButtonContainer();
				AddPanelElement(detailExpandButtonContainer, DetailExpandButtonPosition);
			}
			if(detailExpandButtonContainer != null) {
				FrameworkElementHelper.SetIsVisible(detailExpandButtonContainer, actualShowDetailButtons);
			}
		}
		void UpdateLeftDetailViewIndents() {
			var indents = rowData.DetailIndents;
			if(indents != null && detailLeftIndentControl == null && layoutPanel != null) {
				detailLeftIndentControl = new DetailRowsIndentControl();
				AddPanelElement(detailLeftIndentControl, LeftDetailIndentPosition);
			}
			if(detailLeftIndentControl != null) {
				detailLeftIndentControl.Visibility = DetailMarginVisibilityConverter.GetDetailMarginControlVisibility(indents, Side.Left);
				detailLeftIndentControl.ItemsSource = indents;
			}
		}
		void UpdateRightDetailViewIndents() {
			var indents = rowData.DetailIndents;
			if(indents != null && detailRightIndentControl == null && layoutPanel != null) {
				detailRightIndentControl = new DetailRowsIndentRightControl();
				AddPanelElement(detailRightIndentControl, RightDetailIndentPosition);
			}
			if(detailRightIndentControl != null) {
				detailRightIndentControl.Visibility = DetailMarginVisibilityConverter.GetDetailMarginControlVisibility(indents, Side.Right);
				detailRightIndentControl.ItemsSourceToReverse = indents;
			}
		}
		void UpdateDetails() {
			if(layoutPanel == null) return;
			if(ShowDetails) {
				if(detailContentPresenter == null) {
					UpdateRowDefinitions();
					detailContentPresenter = new RowDetailsControl() { Content = rowData };
					UpdateDetailContentPresenterRow();
					System.Windows.Controls.Grid.SetColumnSpan(detailContentPresenter, FitContentPosition - FixedLeftPosition + 1);
					AddPanelElement(detailContentPresenter, FixedLeftPosition);
					UpdateBottomLineMargin(detailContentPresenter, true);
				}
				detailContentPresenter.Visibility = Visibility.Visible;
				detailContentPresenter.ContentTemplateSelector = TableView.ActualRowDetailsTemplateSelector;
			} else {
				if(detailContentPresenter != null)
					detailContentPresenter.Visibility = Visibility.Collapsed;
			}
			UpdateBottomLineMargin();
		}
		void UpdateDetailContentPresenterRow() {
			if(detailContentPresenter != null)
				System.Windows.Controls.Grid.SetRow(detailContentPresenter, ShowInlineEditForm ? 2 : 1);
		}
		void UpdateIndicatorWidth() {
			if(indicator != null)
				indicator.Width = ((ITableView)rowData.View).ActualIndicatorWidth;
		}
		void UpdateIndicatorVisibility() {
			if(indicator != null)
				indicator.Visibility = ((ITableView)rowData.View).ActualShowIndicator ? Visibility.Visible : Visibility.Collapsed;
		}
		void UpdateIndicatorState() {
			if(indicator != null)
				indicator.IndicatorState = rowData.IndicatorState;
		}
		void UpdateIndicatorContentTemplate() {
			if(indicator != null)
				indicator.UpdateContent();
		}
		void UpdateRowValidationError() {
			BaseEditHelper.SetValidationError(this, rowData.ValidationErrorInternal);
		}
		void UpdateFixedNoneContentWidth() {
			if(AllowTreeIndentScrolling)
				indentScroller.Do(x => x.Width = TableView.FixedNoneContentWidth);
			else
				CellsControl.Do(x => x.Width = rowData.FixedNoneContentWidth);
		}
		protected void AddPanelElement(FrameworkElement element, int position) {
			if(element != null && layoutPanel != null) {
				layoutPanel.Children.Add(element);
				System.Windows.Controls.Grid.SetColumn(element, position);
			}
		}
		void UpdateBottomLineMargin() {
			bool useDataRowMargin = !(ShowDetails || ShowInlineEditForm);
			UpdateBottomLineMargin(CellsControl, useDataRowMargin);
			UpdateBottomLineMargin(fixedLeftCellsControl, useDataRowMargin);
			UpdateBottomLineMargin(fixedRightCellsControl, useDataRowMargin);
			UpdateBottomLineMargin(leftSeparator, useDataRowMargin);
			UpdateBottomLineMargin(rightSeparator, useDataRowMargin);
			UpdateBottomLineMargin(backgroundBorder, useDataRowMargin);
			UpdateBottomLineMargin(fitContent, useDataRowMargin);
			UpdateBottomLineMargin(detailContentPresenter, true);
		}
		void UpdateBottomLineMargin(FrameworkElement element, bool useMargin) {
			if(element != null) {
				double bottomMargin = 0d;
				if(useMargin && (ShowHorizontalLines || rowData.ShowBottomLine))
					bottomMargin = (bottomLine == null ? default(Thickness) : bottomLine.BorderThickness).Bottom;
				element.Margin = new Thickness(0, 0, 0, bottomMargin);
			}
		}
		void UpdateScrollingMargin() {
			Thickness offset = ((ITableView)rowData.View).ScrollingVirtualizationMargin;
			if(AllowTreeIndentScrolling)
				indentScroller.Do(x => x.SetScrollOffset(offset));
			else
				CellsControl.Do(x => x.SetPanelOffset(offset));
		}
		void UpdateView(CellsControl cellsControl) {
			if(cellsControl != null)
				DataControlBase.SetCurrentView(cellsControl, rowData.View);
		}
		void UpdateCellData(CellsControl cellsControl) {
			if(cellsControl != null)
				cellsControl.UpdateItemsSource();
		}
		void UpdateCellsPanel(CellsControl cellsControl) {
			if(cellsControl != null)
				cellsControl.UpdatePanel();
		}
		void InvalidateCellsPanel(CellsControl cellsControl) {
			if(cellsControl == null) return;
			cellsControl.InvalidateArrange();
			cellsControl.InvalidatePanel();
		}
		void UpdateBands(CellsControl cellsControl) {
			if(cellsControl != null)
				cellsControl.UpdateBands();
		}
		void UpdateFixedLeftCellData(IList<GridColumnData> oldValue) {
			InitFixedLeftCellsControl();
			SubscribeFixedCellDataChanged(oldValue, rowData.FixedLeftCellData, OnFixedLeftCellDataCollectionChanged);
		}
		void UpdateFixedRightCellData(IList<GridColumnData> oldValue) {
			InitFixedRightCellsControl();
			SubscribeFixedCellDataChanged(oldValue, rowData.FixedRightCellData, OnFixedRightCellDataCollectionChanged);
		}
		void SubscribeFixedCellDataChanged(IList<GridColumnData> oldValue, IList<GridColumnData> newValue, NotifyCollectionChangedEventHandler handler) {
			if(oldValue != null)
				((INotifyCollectionChanged)oldValue).CollectionChanged -= handler;
			if(newValue != null)
				((INotifyCollectionChanged)newValue).CollectionChanged += handler;
		}
		void InitFixedLeftCellsControl() {
			CreateAndInitCellsControl(ref fixedLeftCellsControl, FixedLeftPosition, ref leftSeparator, FixedLeftDelimiterPosition, x => x.FixedLeftCellData, x => x.FixedLeftVisibleBands, x => x.FixedLeftVisibleColumns, FixedStyle.Left);
		}
		void InitFixedRightCellsControl() {
			CreateAndInitCellsControl(ref fixedRightCellsControl, FixedRightPosition, ref rightSeparator, FixedRightDelimiterPosition, x => x.FixedRightCellData, x => x.FixedRightVisibleBands, x => x.FixedRightVisibleColumns, FixedStyle.Right);
		}
		void CreateAndInitCellsControl(ref CellsControl cellsControl, int position, ref RowFixedLineSeparatorControl separator, int separatorPosition, Func<RowData, IList<GridColumnData>> getCellDataFunc, Func<BandsLayoutBase, IList<BandBase>> getFixedBandsFunc, Func<TableViewBehavior, IList<ColumnBase>> getFixedColumnsFunc, FixedStyle fixedStyle) {
			IList<GridColumnData> newCellsValue = getCellDataFunc(rowData);
			IList<BandBase> newBandsValue = BandsLayout != null ? getFixedBandsFunc(BandsLayout) : null;
			if(cellsControl == null && layoutPanel != null && ((newCellsValue != null && newCellsValue.Count > 0) || (newBandsValue != null))) {
				cellsControl = CreateAndInitCellsControl(position, getCellDataFunc, getFixedBandsFunc, fixedStyle);
				TableViewProperties.SetFixedAreaStyle(cellsControl, fixedStyle);
				separator = new RowFixedLineSeparatorControl(getFixedColumnsFunc, getFixedBandsFunc);
				UpdateFixedSeparatorHitTestAcceptor(separator, separatorPosition);
				separator.Width = ((ITableView)rowData.View).FixedLineWidth;
				UpdateFixedSeparatorShowVertialLines(separator);
				UpdateFixedSeparatorWidth(separator);
				UpdateFixedSeparatorVisibility(separator);
				UpdateBottomLineMargin(separator, !ShowDetails);
				AddPanelElement(separator, separatorPosition);
			}
		}
		void UpdateFixedSeparatorShowVertialLines(RowFixedLineSeparatorControl separator) {
			if(separator != null)
				separator.ShowVerticalLines = ((ITableView)rowData.View).ShowVerticalLines;
		}
		void UpdateShowVertialLines() {
			ShowVerticalLines = ((ITableView)rowData.View).ShowVerticalLines;
		}
		void UpdateFitContentBorderBrush() {
			if(fitContent != null)
				fitContent.BorderBrush = RowFitBorderBrush;
		}
		void UpdateFixedSeparatorWidth(RowFixedLineSeparatorControl separator) {
			if(separator != null)
				separator.Width = ((ITableView)rowData.View).FixedLineWidth;
		}
		void UpdateFixedSeparatorVisibility(RowFixedLineSeparatorControl separator) {
			if(separator != null)
				separator.UpdateVisibility(rowData.View.DataControl);
		}
		void UpdateFixedSeparatorHitTestAcceptor(RowFixedLineSeparatorControl separator, int separatorPosition) {
			TableViewHitTestAcceptorBase hitTestAcceptor = null;
			if(separatorPosition == FixedLeftDelimiterPosition)
				hitTestAcceptor = new FixedLeftDivTableViewHitTestAcceptor();
			else if(separatorPosition == FixedRightDelimiterPosition)
				hitTestAcceptor = new FixedRightDivTableViewHitTestAcceptor();
			GridViewHitInfoBase.SetHitTestAcceptor(separator, hitTestAcceptor);
		}
		void OnFixedLeftCellDataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(!UseTemplate)
				InitFixedLeftCellsControl();
		}
		void OnFixedRightCellDataCollectionChanged(object sender, NotifyCollectionChangedEventArgs e) {
			if(!UseTemplate)
				InitFixedRightCellsControl();
		}
		CellsControl CreateAndInitCellsControl(int position, Func<RowData, IList<GridColumnData>> getCellDataFunc, Func<BandsLayoutBase, IList<BandBase>> getBandsFunc, FixedStyle fixedStyle) {
			var result = CreateCellsControl(getCellDataFunc, getBandsFunc);
			UpdateView(result);
			UpdateBottomLineMargin(result, !ShowDetails);
			if(AllowTreeIndentScrolling && fixedStyle == FixedStyle.None)
				indentScroller.AddScrollableElement(result, 1);
			else
				AddPanelElement(result, position);
			if(IsBandedLayout || rowData.View.ActualAllowCellMerge)
				UpdateCellsPanel(result);
			UpdateCellData(result);
			return result;
		}
		protected virtual CellsControl CreateCellsControl(Func<RowData, IList<GridColumnData>> getCellDataFunc, Func<BandsLayoutBase, IList<BandBase>> getBandsFunc) {
			return new CellsControl(this, getCellDataFunc, getBandsFunc);
		}
		protected CellsControl CreateAndInitFixedNoneCellsControl(int position, Func<RowData, IList<GridColumnData>> getCellDataFunc, Func<BandsLayoutBase, IList<BandBase>> getBandsFunc) {
			var cellsControl = CreateAndInitCellsControl(position, getCellDataFunc, getBandsFunc, FixedStyle.None);
			FocusRectPresenter.SetIsHorizontalScrollHost(cellsControl, true);
			return cellsControl;
		}
		double CalculateRowContentIndent() {
			if(layoutPanel == null)
				return 0d;
			double indent = 0d;
			for(int i = 0; i < FixedLeftPosition; i++) {
				indent += layoutPanel.ColumnDefinitions[i].ActualWidth;
			}
			return indent;
		}
		bool ShowInlineEditForm { get { return rowData.IsInlineEditFormVisible; } }
		EditFormShowMode appliedEditingMode = EditFormShowMode.None;
		Locker editFormCloseLocker = new Locker();
		void UpdateInlineEditForm(object editFormData) {
			editFormCloseLocker.DoLockedAction(() => UpdateInlineEditFormCore(editFormData));
		}
		void UpdateInlineEditFormCore(object editFormData) {
			if(layoutPanel == null)
				return;
			UpdateRowDefinitions();
			if(ShowInlineEditForm) {
				int containerRow = 0;
				int containerColumnSpan = 1;
				if(TableView.EditFormShowMode == EditFormShowMode.InlineHideRow) {
					ClearElements();
					for(int i = 0; i < FixedLeftPosition; i++)
						layoutPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Auto });
					layoutPanel.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
					UpdateIndicator();
					UpdateOffsetPresenter();
				} else {
					containerRow = 1;
					containerColumnSpan = FitContentPosition - FixedLeftPosition + 1;
				}
				if(EditFormContainer == null) {
					EditFormContainer = new EditFormContainer();
					System.Windows.Controls.Grid.SetRow(EditFormContainer, containerRow);
					System.Windows.Controls.Grid.SetColumnSpan(EditFormContainer, containerColumnSpan);
					EditFormContainer.ShowMode = TableView.EditFormShowMode;
					AddPanelElement(EditFormContainer, FixedLeftPosition);
				}
				EditFormContainer.ContentTemplate = TableView.EditFormTemplate;
				EditFormContainer.Content = editFormData;
				EditFormContainer.Visibility = Visibility.Visible;
			} else {
				if(EditFormContainer != null) {
					if(appliedEditingMode == EditFormShowMode.InlineHideRow)
						CreateContent();
					else
						EditFormContainer.Visibility = Visibility.Collapsed;
				}
			}
			appliedEditingMode = TableView.EditFormShowMode;
			UpdateDetailContentPresenterRow();
			UpdateBottomLineMargin();
		}
		void OnEditFormContainerChanged() {
			editFormCloseLocker.DoIfNotLocked(() => {
				if(EditFormContainer == null)
					rowData.View.Do(x => x.EditFormManager.OnInlineFormClosed());
			});
		}
		void UpdateRowDefinitions() {
			if(layoutPanel == null)
				return;
			int rowCount = 1;
			if(ShowInlineEditForm)
				rowCount++;
			if(ShowDetails)
				rowCount++;
			if(rowCount > 1) {
				RowDefinitionCollection rowDefinitions = layoutPanel.RowDefinitions;
				for(int i = rowDefinitions.Count; i < rowCount; i++)
					rowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, i == 0 ? GridUnitType.Star : GridUnitType.Auto) });
			}
		}
		#region IRowStateClient
		void IRowStateClient.UpdateRowHandle(RowHandle rowHandle) {
			DataViewBase.SetRowHandle(this, rowHandle);
		}
		void IRowStateClient.UpdateSelectionState(SelectionState selectionState) {
			this.SelectionState = selectionState;
			UpdateCellsState();
			UpdateFocusWithinState();
		}
		void IRowStateClient.UpdateIsFocused() {
			UpdateDetails();
		}
		void IRowStateClient.UpdateScrollingMargin() {
			UpdateScrollingMargin();
		}
		void IRowStateClient.UpdateFixedNoneCellData() {
			UpdateCellData(CellsControl);
		}
		void IRowStateClient.UpdateView() {
			UpdateView(CellsControl);
			UpdateBands(CellsControl);
			UpdateView(fixedLeftCellsControl);
			UpdateView(fixedRightCellsControl);
			UpdateFitContentContextMenu();
			UpdateIndicatorContextMenu();
		}
		void IRowStateClient.UpdateFixedLeftCellData(IList<GridColumnData> oldValue) {
			UpdateFixedLeftCellData(oldValue);
		}
		void IRowStateClient.UpdateFixedRightCellData(IList<GridColumnData> oldValue) {
			UpdateFixedRightCellData(oldValue);
		}
		void IRowStateClient.UpdateHorizontalLineVisibility() {
			this.ShowHorizontalLine = ShowHorizontalLines;
			UpdateBottomLineMargin();
		}
		void IRowStateClient.UpdateVerticalLineVisibility() {
			UpdateFixedSeparatorShowVertialLines(leftSeparator);
			UpdateFixedSeparatorShowVertialLines(rightSeparator);
			UpdateShowVertialLines();
		}
		void IRowStateClient.UpdateFixedLineWidth() {
			UpdateFixedSeparatorWidth(leftSeparator);
			UpdateFixedSeparatorWidth(rightSeparator);
		}
		void IRowStateClient.UpdateFixedLineVisibility() {
			UpdateFixedSeparatorVisibility(leftSeparator);
			UpdateFixedSeparatorVisibility(rightSeparator);
		}
		void IRowStateClient.UpdateFixedNoneContentWidth() {
			UpdateFixedNoneContentWidth();
		}
		void IRowStateClient.UpdateIndicatorWidth() {
			UpdateIndicatorWidth();
		}
		void IRowStateClient.UpdateShowIndicator() {
			UpdateIndicatorVisibility();
		}
		void IRowStateClient.UpdateIndicatorState() {
			UpdateIndicatorState();
		}
		void IRowStateClient.UpdateIndicatorContentTemplate() {
			UpdateIndicatorContentTemplate();
		}
		void IRowStateClient.UpdateContent() {
			if(layoutPanel == null) return;
			if(oldUseTemplate != UseTemplate)
				CreateContent();
			else
				if(contentPresenter != null) contentPresenter.ContentTemplateSelector = TableView.ActualDataRowTemplateSelector;
		}
		void IRowStateClient.UpdateValidationError() {
			UpdateTriggerErrorState();
			UpdateRowValidationError();
		}
		void IRowStateClient.UpdateCellsPanel() {
			UpdateCellsPanel(CellsControl);
			UpdateCellsPanel(fixedLeftCellsControl);
			UpdateCellsPanel(fixedRightCellsControl);
		}
		void IRowStateClient.InvalidateCellsPanel() {
			if(!rowData.View.ActualAllowCellMerge) return;
			InvalidateCellsPanel(CellsControl);
			InvalidateCellsPanel(fixedLeftCellsControl);
			InvalidateCellsPanel(fixedRightCellsControl);
		}
		void IRowStateClient.UpdateFixedNoneBands() {
			UpdateBands(CellsControl);
		}
		void IRowStateClient.UpdateFixedLeftBands() {
			InitFixedLeftCellsControl();
			UpdateBands(fixedLeftCellsControl);
			UpdateFixedSeparatorVisibility(leftSeparator);
		}
		void IRowStateClient.UpdateFixedRightBands() {
			InitFixedRightCellsControl();
			UpdateBands(fixedRightCellsControl);
			UpdateFixedSeparatorVisibility(rightSeparator);
		}
		void IRowStateClient.UpdateAlternateBackground() {
			this.IsAlternateRow = rowData.AlternateRow;
		}
		void IRowStateClient.UpdateFocusWithinState() {
			UpdateFocusWithinState();
		}
		void IRowStateClient.UpdateLevel() {
			UpdateOffsetPresenterLevel();
			UpdateShowBottomLine();
			UpdateCellsState(fixedLeftCellsControl);
			if(AllowTreeIndentScrolling)
				UpdateCellsState(CellsControl);
		}
		void IRowStateClient.UpdateDetailExpandButtonVisibility() {
			UpdateDetailExpandButton();
		}
		void IRowStateClient.UpdateDetailViewIndents() {
		}
		void IRowStateClient.UpdateRowPosition() {
			UpdateShowBottomLine();
		}
		void IRowStateClient.UpdateMinHeight() {
			MinHeight = ((ITableView)rowData.View).RowMinHeight;
		}
		void IRowStateClient.UpdateRowStyle() {
			Style newStyle = ((ITableView)rowData.View).RowStyle;
			if(newStyle is DefaultStyle)
				newStyle = null;
			if(Style != newStyle)
				Style = newStyle;
		}
		void IRowStateClient.UpdateAppearance() {
			formattingHelper.UpdateConditionalAppearance();
		}
		void IRowStateClient.UpdateDetails() {
			UpdateDetails();
			UpdateBottomLineMargin();
		}
		void IRowStateClient.UpdateIndentScrolling() {
			CreateContent();
		}
		void IRowStateClient.UpdateShowRowBreak() {
			ShowRowBreak = rowData.ShowRowBreak;
		}
		void IRowStateClient.UpdateInlineEditForm(object editFormData) {
			UpdateInlineEditForm(editFormData);
		}
		#endregion
		#region IConditionalFormattingClient
		FormatValueProvider? IConditionalFormattingClient<RowControl>.GetValueProvider(string fieldName) {
			return (rowData.RowHandle != null && DataControl != null) ? (FormatValueProvider?)rowData.GetValueProvider(fieldName) : null;
		}
		ConditionalFormattingHelper<RowControl> IConditionalFormattingClient<RowControl>.FormattingHelper { get { return formattingHelper; } }
		IList<FormatConditionBaseInfo> IConditionalFormattingClient<RowControl>.GetRelatedConditions() {
			var tableView = TableView;
			return tableView != null ? tableView.FormatConditions.GetInfoByFieldName(string.Empty) : null;
		}
		bool IConditionalFormattingClient<RowControl>.IsReady { get { return true; } }
		bool IConditionalFormattingClient<RowControl>.IsSelected { get { return SelectionState != SelectionState.None; } }
		void IConditionalFormattingClient<RowControl>.UpdateBackground() {
		}
		void IConditionalFormattingClient<RowControl>.UpdateDataBarFormatInfo(DataBarFormatInfo info) {
		}
		Locker IConditionalFormattingClient<RowControl>.Locker { get { return rowData.conditionalFormattingLocker; } }
		#endregion
	}
	public class RowDetailsControl : ContentControl {
		static RowDetailsControl() {
			Type ownerType = typeof(RowDetailsControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
		}
	}
}
