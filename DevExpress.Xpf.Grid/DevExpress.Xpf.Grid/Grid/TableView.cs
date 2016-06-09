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
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Markup;
using DevExpress.Core;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Grid.Printing;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Printing.BrickCollection;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.DataNodes;
using System.Windows.Media;
using DevExpress.Xpf.Bars;
using System.Collections.ObjectModel;
using System.Text;
using System.Linq;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Editors.Helpers;
#if SL
using PropertyDescriptor = DevExpress.Data.Browsing.PropertyDescriptor;
using Control = DevExpress.Xpf.Core.WPFCompatibility.SLControl;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.Xpf.Grid {
	public abstract class GridTableViewBehaviorBase : TableViewBehavior {
		protected GridTableViewBehaviorBase(DataViewBase view) 
			: base(view) {
		}
		internal sealed override GridColumnHeaderBase CreateGridColumnHeader() {
			return new GridColumnHeader();
		}
		internal override BestFitControlBase CreateBestFitControl(ColumnBase column) {
#if SL
			return new BestFitControl(View, column);
#else
			return CreateElement(() => new LightweightBestFitControl(View, column), () => new BestFitControl(View, column), UseLightweightTemplates.All) as BestFitControlBase;
#endif
		}
		internal sealed override FrameworkElement CreateGridTotalSummaryControl() {
			return new GridTotalSummary();
		}
		internal override FrameworkElement CreateGroupFooterSummaryControl() {
			return new GroupFooterSummaryControl();
		}
		protected internal override IndicatorState GetIndicatorState(RowData rowData) {
			if(rowData is GroupSummaryRowData)
				return IndicatorState.None;
			return base.GetIndicatorState(rowData);
		}
		#region master detail
		internal override DetailHeaderControlBase CreateDetailHeaderElement() {
			return new DetailHeaderControl();
		}
		internal override DetailHeaderControlBase CreateDetailContentElement() {
			return new DetailContentControl();
		}
		internal override DetailTabHeadersControlBase CreateDetailTabHeadersElement() {
			return new DetailTabHeadersControl();
		}
		internal override DetailRowControlBase CreateDetailColumnHeadersElement() {
			return new DetailColumnHeadersControl();
		}
		internal override DetailRowControlBase CreateDetailTotalSummaryElement() {
			return new DetailTotalSummaryControl();
		}
		internal override DetailRowControlBase CreateDetailFixedTotalSummaryElement() {
			return new DetailFixedTotalSummaryControl();
		}
		internal override DetailRowControlBase CreateDetailNewItemRowElement() {
			return new DetailNewItemRowControl();
		}
		#endregion        
#if !SL
		internal override Style GetActualCellStyle(ColumnBase column) {
			Style style = base.GetActualCellStyle(column);
			return ValidateStyle(style, typeof(GridCellContentPresenter), typeof(LightweightCellEditor), UseLightweightRows ? WrongCellStyleTargetTypeError : WrongCellStyleTargetTypeErrorUnoptimized);
		}
		internal override void ValidateRowStyle(Style newStyle) {
			if(!canChangeUseLightweightTemplates)
				ValidateStyle(newStyle, typeof(GridRowContent), typeof(RowControl), UseLightweightRows ? WrongRowStyleTargetTypeError : WrongRowStyleTargetTypeErrorUnoptimized);
		}
		const string HelpLinkString = "http://go.devexpress.com/xpf-optimized-mode.aspx";
		const string LearnMoreMessage = " To learn more about the grid's optimized mode, see " + HelpLinkString;
		internal const string WrongCellStyleTargetTypeError = "The CellStyle target type is not supported in the grid's optimized mode. Either disable the optimized mode or change the target type to LightweightCellEditor." + LearnMoreMessage;
		internal const string WrongCellStyleTargetTypeErrorUnoptimized = "The CellStyle target type is not supported in the grid's unoptimized mode.";
		internal const string WrongRowStyleTargetTypeError = "The RowStyle target type is not supported in the grid's optimized mode. Either disable the optimized mode or change the target type to RowControl." + LearnMoreMessage;
		internal const string WrongRowStyleTargetTypeErrorUnoptimized = "The RowStyle target type is not supported in the grid's unoptimized mode.";
		internal const string RowDetailsUnoptimizedModeError = "RowDetailsTemplate and RowDetailsTemplateSelector is only supported in the grid's optimized mode.";
		internal const string RowDecorationTemplateInOptimizedModeError = "RowDecorationTemplate is not supported in the grid's optimized mode." + LearnMoreMessage;
		internal const string GroupValueContentStyleInOptimizedModeError = "GroupValueContentStyle is not supported in the grid's optimized mode." + LearnMoreMessage;
		internal const string GroupSummaryContentStyleInOptimizedModeError = "GroupSummaryContentStyle  is not supported in the grid's optimized mode." + LearnMoreMessage;
		internal const string DefaultDataRowTemplateInOptimizedModeError = "CellItemsControl (that is typically used in DefaultDataRowTemplate) is not supported in the grid's optimized mode." + LearnMoreMessage;
		protected bool UseLightweightRows {
			get { return UseLightweightTemplatesHasFlag(UseLightweightTemplates.Row); }
		}
		protected virtual Style ValidateStyle(Style style, Type normalTargetType, Type optimizedTargetType, string errorMessage) {
			if(style == null)
				return null;
			if(style is DefaultStyle) {
				if(UseLightweightRows)
					return null;
			} else if(!View.IsDesignTime) {
				Type expectedTargetType = UseLightweightRows ? optimizedTargetType : normalTargetType;
				if(View.IsInitialized && style.TargetType != null && !style.TargetType.IsAssignableFrom(expectedTargetType) && !DataViewBase.DisableOptimizedModeVerification)
					throw new InvalidOperationException(errorMessage);
			}
			return style;
		}
		protected override void UpdateActualRowDetailsTemplateSelector() {
			base.UpdateActualRowDetailsTemplateSelector();
			if(!UseLightweightTemplatesHasFlag(UseLightweightTemplates.Row))
				throw new InvalidOperationException(RowDetailsUnoptimizedModeError);
		}
		protected internal override void OnRowDecorationTemplateChanged() {
			base.OnRowDecorationTemplateChanged();
			if(View.IsDesignTime) return;
			if(!(TableView.RowDecorationTemplate == null || TableView.RowDecorationTemplate is DefaultControlTemplate) && UseLightweightTemplatesHasFlag(UseLightweightTemplates.Row) && !DataViewBase.DisableOptimizedModeVerification)
				throw new InvalidOperationException(RowDecorationTemplateInOptimizedModeError);
		}
		protected internal override void OnCellItemsControlLoaded() {
			base.OnCellItemsControlLoaded();
			if(View.IsDesignTime) return;
			if(UseOptimizedTemplate && !DataViewBase.DisableOptimizedModeVerification)
				throw new InvalidOperationException(DefaultDataRowTemplateInOptimizedModeError);
		}
		protected virtual bool UseOptimizedTemplate { get { return UseLightweightTemplatesHasFlag(UseLightweightTemplates.Row); } }
#endif
	}
#if !SL
	public class DefaultStyle : Style {
		public DefaultStyle() { }
		public DefaultStyle(Type targetType) : base(targetType) { }
		public DefaultStyle(Type targetType, Style basedOn) : base(targetType, basedOn) { }
	}
#endif
	public class GridTableViewBehavior : GridTableViewBehaviorBase {
		public GridTableViewBehavior(TableView view)
			: base(view) {
		}
		protected TableView GridTableView { get { return (TableView)View; } }
		protected internal override Style AutoFilterRowCellStyle { get { return GridTableView.AutoFilterRowCellStyle; } }
		internal override bool IsNewItemRowVisible { get { return GridTableView.NewItemRowPosition == NewItemRowPosition.Top; } }
		protected internal override bool IsNewItemRowFocused { get { return View.FocusedRowHandle == GridControl.NewItemRowHandle; } }
		protected internal override bool IsNewItemRowEditing { get { return GridTableView.GridDataProvider.IsNewItemRowEditing; } }
		protected internal override bool IsAdditionalRow(int rowHandle) { return base.IsAdditionalRow(rowHandle) && !(View.ShouldDisplayBottomRow && rowHandle == GridControl.NewItemRowHandle); }
		protected internal override bool IsAdditionalRowData(RowData rowData) {
			return base.IsAdditionalRowData(rowData) || rowData == GridTableView.NewItemRowData;
		}
		protected internal override bool IsAlternateRow(int rowHandle) {
			if(GridTableView.AlternationCount < 1 || GridTableView.ActualAlternateRowBackground == null) return false;
			return rowHandle % GridTableView.AlternationCount == (GridTableView.AlternationCount - 1);
		}
		protected internal override System.Windows.Media.Brush ActualAlternateRowBackground { get { return GridTableView.ActualAlternateRowBackground; } } 
		internal override int GetValueForSelectionAnchorRowHandle(int value) {
			if(value == GridControl.NewItemRowHandle) {
				if(GridTableView.NewItemRowPosition == NewItemRowPosition.Top)
					return GridTableView.Grid.GetRowHandleByVisibleIndex(0);
			}
			return base.GetValueForSelectionAnchorRowHandle(value);
		}
		protected internal override void UpdateAdditionalRowsData() {
			if(IsNewItemRowVisible) GridTableView.NewItemRowData.UpdateData();
			base.UpdateAdditionalRowsData();
		}
		DependencyObject newItemRowState = new RowStateObject();
		protected internal override DependencyObject GetRowState(int rowHandle) {
			if(rowHandle == GridControl.NewItemRowHandle)
				return newItemRowState;
			return base.GetRowState(rowHandle);
		}
		internal override bool CheckNavigationStyle(int newValue) {
			return base.CheckNavigationStyle(newValue) && (newValue != GridControl.NewItemRowHandle);
		}
		protected internal override void UpdateRowData(UpdateRowDataDelegate updateMethod, bool updateInvisibleRows = true, bool updateFocusedRow = true) {
			base.UpdateRowData(updateMethod, updateInvisibleRows, updateFocusedRow);
			if(updateInvisibleRows || IsNewItemRowVisible) updateMethod(GridTableView.NewItemRowData);
		}
		protected internal override RowData GetRowData(int rowHandle) {
			if(rowHandle == GridControl.NewItemRowHandle && GridTableView.NewItemRowPosition != NewItemRowPosition.Bottom)
				return GridTableView.NewItemRowData;
			return base.GetRowData(rowHandle);
		}
		protected override void UpdateAdditionalFocusedRowDataCore() {
			if(IsNewItemRowFocused)
				View.FocusedRowData = GridTableView.NewItemRowData;
			base.UpdateAdditionalFocusedRowDataCore();
		}
		protected override int GetAdditionalRowHandle() {
			return IsNewItemRowVisible ? GridControl.NewItemRowHandle : base.GetAdditionalRowHandle();
		}
		protected override bool CanNavigateToAdditionalRow(bool allowNavigateToAutoFilterRow) {
			return base.CanNavigateToAdditionalRow(allowNavigateToAutoFilterRow) || IsNewItemRowVisible;
		}
		protected internal override double GetFixedNoneContentWidth(double totalWidth, int rowHandle) {
			if(View.DataControl != null && !HasFixedLeftElements && !View.DataControl.IsGroupRowHandleCore(rowHandle) && !IsAdditionalRowCore(rowHandle))
				return totalWidth - ViewInfo.RightGroupAreaIndent - FirstColumnIndent;
			if(IsAdditionalRowCore(rowHandle) && !HasFixedLeftElements)
				return TableView.ShowIndicator ? totalWidth : totalWidth - TableView.LeftDataAreaIndent;
			return totalWidth;
		}
		internal override int GetTopRow(int pageVisibleTopRowIndex) {
			return GridTableView.Grid.GetDataRowHandleByGroupRowHandle(GridTableView.Grid.GetRowHandleByVisibleIndex(pageVisibleTopRowIndex));
		}
		internal override CustomBestFitEventArgsBase RaiseCustomBestFit(ColumnBase column, BestFitMode bestFitMode) {
			CustomBestFitEventArgs e = new CustomBestFitEventArgs((GridColumn)column, bestFitMode);
			GridTableView.RaiseCustomBestFit(e);
			return e;
		}
		internal override void UpdateActualExpandDetailButtonWidth() {
			View.UpdateAllDependentViews(view => ((TableView)view).ActualExpandDetailButtonWidth = GridTableView.ExpandDetailButtonWidth);
		}
		internal override void UpdateActualDetailMargin() {
			View.UpdateAllDependentViews(view => ((TableView)view).ActualDetailMargin = GridTableView.DetailMargin);
		}
		protected internal override void OnFocusedRowCellModified() {
			if (IsNewItemRowFocused && !GridTableView.GridDataProvider.IsNewItemRowEditing)
				GridTableView.AddNewRow();
			base.OnFocusedRowCellModified();
		}
		internal override GridColumnData GetGroupSummaryColumnData(int rowHandle, IBestFitColumn column) {
			ColumnBase dataColumn = (ColumnBase)column;
			RowData rowData = null;
			GridGroupSummaryColumnData summaryData = null;
			if(View.VisualDataTreeBuilder.GroupSummaryRows.TryGetValue(rowHandle, out rowData))
				summaryData = ((GroupSummaryRowData)rowData).SafeGetGroupSummaryColumnData(dataColumn);
			if(summaryData == null) {
				GroupSummaryRowData groupSummaryRowData = new GroupSummaryRowData(View.VisualDataTreeBuilder, new RowHandle(rowHandle));
				groupSummaryRowData.AssignFrom(rowHandle);
				summaryData = groupSummaryRowData.SafeGetGroupSummaryColumnData(dataColumn);
				if(summaryData == null) {
					summaryData = new GridGroupSummaryColumnData(groupSummaryRowData);
					groupSummaryRowData.UpdateGridGroupSummaryData(dataColumn, summaryData);
				}
			}
			return summaryData;
		}
		internal override MouseMoveSelectionBase GetMouseMoveSelection(IDataViewHitInfo hitInfo) {
			if(GridTableView.ShowCheckBoxSelectorColumn && GridTableView.RetainSelectionOnClickOutsideCheckBoxSelector)
				return MouseMoveSelectionNone.Instance;
			return base.GetMouseMoveSelection(hitInfo);
		}
		protected internal override FrameworkElement GetAdditionalRowElement(int rowHandle) {
			if(rowHandle == DataControlBase.NewItemRowHandle && IsNewItemRowVisible && !View.IsRootView) {
				RowDataBase newItemRowData = DataControl.DataControlParent.GetNewItemRowData();
				if(newItemRowData != null && newItemRowData.WholeRowElement != null)
					return LayoutHelper.FindElementByName(newItemRowData.WholeRowElement, "PART_NewItemRow");
			}
			return base.GetAdditionalRowElement(rowHandle);
		}
	}
	public partial class TableView : GridViewBase, ITableView, IGroupSummaryDisplayMode, IDetailElement<DataViewBase> {
		public static readonly DependencyProperty NewItemRowPositionProperty;
		internal const string NewItemRowDataPropertyName = "NewItemRowData";
		#region static
		#region common
		public static readonly DependencyProperty ColumnBandChooserTemplateProperty;
		static readonly DependencyPropertyKey FixedNoneContentWidthPropertyKey;
		public static readonly DependencyProperty FixedNoneContentWidthProperty;
		static readonly DependencyPropertyKey TotalSummaryFixedNoneContentWidthPropertyKey;
		public static readonly DependencyProperty TotalSummaryFixedNoneContentWidthProperty;
		static readonly DependencyPropertyKey VerticalScrollBarWidthPropertyKey;
		public static readonly DependencyProperty VerticalScrollBarWidthProperty;
		static readonly DependencyPropertyKey FixedLeftContentWidthPropertyKey;
		public static readonly DependencyProperty FixedLeftContentWidthProperty;
		static readonly DependencyPropertyKey FixedRightContentWidthPropertyKey;
		public static readonly DependencyProperty FixedRightContentWidthProperty;
		static readonly DependencyPropertyKey TotalGroupAreaIndentPropertyKey;
		public static readonly DependencyProperty TotalGroupAreaIndentProperty;
		public static readonly DependencyProperty ExtendScrollBarToFixedColumnsProperty;
		static readonly DependencyPropertyKey IndicatorHeaderWidthPropertyKey;
		public static readonly DependencyProperty IndicatorHeaderWidthProperty;
		static readonly DependencyPropertyKey ActualDataRowTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualDataRowTemplateSelectorProperty;
		public static readonly DependencyProperty RowStyleProperty;
		public static readonly DependencyProperty ShowAutoFilterRowProperty;
		public static readonly DependencyProperty AllowCascadeUpdateProperty;
		public static readonly DependencyProperty AllowPerPixelScrollingProperty;
		public static readonly DependencyProperty ScrollAnimationDurationProperty;
		public static readonly DependencyProperty ScrollAnimationModeProperty;
		public static readonly DependencyProperty AllowScrollAnimationProperty;
		public static readonly DependencyProperty AutoWidthProperty;
		public static readonly DependencyProperty UseGroupShadowIndentProperty;
		public static readonly DependencyProperty LeftDataAreaIndentProperty;
		public static readonly DependencyProperty RightDataAreaIndentProperty;
		static readonly DependencyPropertyKey FixedLeftVisibleColumnsPropertyKey;
		public static readonly DependencyProperty FixedLeftVisibleColumnsProperty;
		static readonly DependencyPropertyKey FixedRightVisibleColumnsPropertyKey;
		public static readonly DependencyProperty FixedRightVisibleColumnsProperty;
		static readonly DependencyPropertyKey FixedNoneVisibleColumnsPropertyKey;
		public static readonly DependencyProperty FixedNoneVisibleColumnsProperty;
		static readonly DependencyPropertyKey HorizontalViewportPropertyKey;
		public static readonly DependencyProperty HorizontalViewportProperty;
		public static readonly DependencyProperty FixedLineWidthProperty;
		public static readonly DependencyProperty ShowVerticalLinesProperty;
		public static readonly DependencyProperty ShowHorizontalLinesProperty;
		public static readonly DependencyProperty RowDecorationTemplateProperty;
		public static readonly DependencyProperty DefaultDataRowTemplateProperty;
		public static readonly DependencyProperty DataRowTemplateProperty;
		public static readonly DependencyProperty DataRowTemplateSelectorProperty;
		public static readonly DependencyProperty RowIndicatorContentTemplateProperty;
		public static readonly DependencyProperty AllowResizingProperty;
		public static readonly DependencyProperty AllowHorizontalScrollingVirtualizationProperty;
		static readonly DependencyPropertyKey ScrollingVirtualizationMarginPropertyKey;
		public static readonly DependencyProperty ScrollingVirtualizationMarginProperty;
		static readonly DependencyPropertyKey ScrollingHeaderVirtualizationMarginPropertyKey;
		public static readonly DependencyProperty ScrollingHeaderVirtualizationMarginProperty;
		public static readonly DependencyProperty RowMinHeightProperty;
		public static readonly DependencyProperty HeaderPanelMinHeightProperty;
		public static readonly DependencyProperty AutoMoveRowFocusProperty;
		public static readonly DependencyProperty BestFitMaxRowCountProperty;
		public static readonly DependencyProperty BestFitModeProperty;
		public static readonly DependencyProperty BestFitAreaProperty;
		public static readonly DependencyProperty AllowBestFitProperty;
		public static readonly RoutedEvent CustomBestFitEvent;
		public static readonly DependencyProperty ShowIndicatorProperty;
		static readonly DependencyPropertyKey ActualShowIndicatorPropertyKey;
		public static readonly DependencyProperty ActualShowIndicatorProperty;
		public static readonly DependencyProperty IndicatorWidthProperty;
		static readonly DependencyPropertyKey ActualIndicatorWidthPropertyKey;
		public static readonly DependencyProperty ActualIndicatorWidthProperty;
		static readonly DependencyPropertyKey ShowTotalSummaryIndicatorIndentPropertyKey;
		public static readonly DependencyProperty ShowTotalSummaryIndicatorIndentProperty;
		public static readonly DependencyProperty ExpandDetailButtonWidthProperty;
		static readonly DependencyPropertyKey ActualExpandDetailButtonWidthPropertyKey;
		public static readonly DependencyProperty ActualExpandDetailButtonWidthProperty;
		public static readonly DependencyProperty DetailMarginProperty;
		static readonly DependencyPropertyKey ActualDetailMarginPropertyKey;
		public static readonly DependencyProperty ActualDetailMarginProperty;
		static readonly DependencyPropertyKey ActualExpandDetailHeaderWidthPropertyKey;
		public static readonly DependencyProperty ActualExpandDetailHeaderWidthProperty;
		static readonly DependencyPropertyKey ExpandColumnPositionPropertyKey;
		public static readonly DependencyProperty ExpandColumnPositionProperty;
		public static readonly DependencyProperty FocusedRowBorderTemplateProperty;
		public static readonly DependencyProperty MultiSelectModeProperty;
		public static readonly DependencyProperty UseIndicatorForSelectionProperty;
		public static readonly DependencyProperty AllowFixedColumnMenuProperty;
		public static readonly DependencyProperty AllowScrollHeadersProperty;
		public static readonly DependencyProperty ShowBandsPanelProperty;
		public static readonly DependencyProperty AllowChangeColumnParentProperty;
		public static readonly DependencyProperty AllowChangeBandParentProperty;
		public static readonly DependencyProperty ShowBandsInCustomizationFormProperty;
		public static readonly DependencyProperty AllowBandMovingProperty;
		public static readonly DependencyProperty AllowBandResizingProperty;
		public static readonly DependencyProperty AllowAdvancedVerticalNavigationProperty;
		public static readonly DependencyProperty AllowAdvancedHorizontalNavigationProperty;
		public static readonly DependencyProperty ColumnChooserBandsSortOrderComparerProperty;
		public static readonly DependencyProperty BandHeaderTemplateProperty;
		public static readonly DependencyProperty BandHeaderTemplateSelectorProperty;
		public static readonly DependencyProperty BandHeaderToolTipTemplateProperty;
		public static readonly DependencyProperty PrintBandHeaderStyleProperty;
		static readonly DependencyPropertyKey HasDetailViewsPropertyKey;
		public static readonly DependencyProperty HasDetailViewsProperty;
		public static readonly DependencyProperty ShowDetailButtonsProperty;
		public static readonly DependencyProperty AllowMasterDetailProperty;
		static readonly DependencyPropertyKey ActualShowDetailButtonsPropertyKey;
		public static readonly DependencyProperty ActualShowDetailButtonsProperty;
		static readonly DependencyPropertyKey IsDetailButtonVisibleBindingContainerPropertyKey;
		public static readonly DependencyProperty IsDetailButtonVisibleBindingContainerProperty;
		#endregion
		public static readonly RoutedEvent InitNewRowEvent;
		public static readonly RoutedEvent CustomScrollAnimationEvent;
		public static readonly RoutedEvent RowDoubleClickEvent;
		public static readonly RoutedEvent ShowingGroupFooterEvent;
		public static readonly DependencyProperty PrintRowTemplateProperty;
		public static readonly DependencyProperty PrintGroupFooterTemplateProperty;
		public static readonly DependencyProperty PrintAutoWidthProperty;
		public static readonly DependencyProperty PrintColumnHeadersProperty;
		public static readonly DependencyProperty PrintBandHeadersProperty;
		public static readonly DependencyProperty PrintGroupFootersProperty;
		public static readonly DependencyProperty AllowPrintDetailsProperty;
		public static readonly DependencyProperty AllowPrintEmptyDetailsProperty;
		public static readonly DependencyProperty PrintAllDetailsProperty;
		public static readonly DependencyProperty PrintColumnHeaderStyleProperty;
		public static readonly DependencyProperty PrintGroupFooterStyleProperty;
		public static readonly DependencyProperty PrintDetailTopIndentProperty;
		public static readonly DependencyProperty PrintDetailBottomIndentProperty;
		public static readonly DependencyProperty LeftGroupAreaIndentProperty;
		public static readonly DependencyProperty RightGroupAreaIndentProperty;
		public static readonly DependencyProperty PrintGroupSummaryDisplayModeProperty;
		public static readonly DependencyProperty AutoFilterRowCellStyleProperty;
		public static readonly DependencyProperty NewItemRowCellStyleProperty;
		public static readonly DependencyProperty AllowFixedGroupsProperty;
		public static readonly DependencyProperty GroupSummaryDisplayModeProperty;
		public static readonly DependencyProperty GroupColumnSummaryItemTemplateProperty;
		public static readonly DependencyProperty GroupColumnSummaryContentStyleProperty;
		public static readonly DependencyProperty GroupBandSummaryContentStyleProperty;
		public static readonly DependencyProperty AllowGroupSummaryCascadeUpdateProperty;
		public static readonly DependencyProperty VerticalScrollbarVisibilityProperty;
		public static readonly DependencyProperty HorizontalScrollbarVisibilityProperty;
		public static readonly DependencyProperty AlternateRowBackgroundProperty;
		protected static readonly DependencyPropertyKey ActualAlternateRowBackgroundPropertyKey;
		public static readonly DependencyProperty ActualAlternateRowBackgroundProperty;
		public static readonly DependencyProperty EvenRowBackgroundProperty;
		public static readonly DependencyProperty UseEvenRowBackgroundProperty;
		public static readonly DependencyProperty AlternationCountProperty;
		public static readonly DependencyProperty GroupFooterRowStyleProperty;
		public static readonly DependencyProperty GroupFooterRowTemplateProperty;
		public static readonly DependencyProperty GroupFooterRowTemplateSelectorProperty;
		protected static readonly DependencyPropertyKey ActualGroupFooterRowTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualGroupFooterRowTemplateSelectorProperty;
		public static readonly DependencyProperty GroupFooterSummaryContentStyleProperty;
		public static readonly DependencyProperty GroupFooterSummaryItemTemplateProperty;
		public static readonly DependencyProperty GroupFooterSummaryItemTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualGroupFooterSummaryItemTemplateSelectorPropertyKey;
		public static readonly DependencyProperty ActualGroupFooterSummaryItemTemplateSelectorProperty;
		public static readonly DependencyProperty ShowGroupFootersProperty;
		public static readonly DependencyProperty ShowCheckBoxSelectorInGroupRowProperty;
		public static readonly DependencyProperty ShowCheckBoxSelectorColumnProperty;
		public static readonly DependencyProperty CheckBoxSelectorColumnWidthProperty;
		public static readonly DependencyProperty CheckBoxSelectorColumnHeaderTemplateProperty;
		public static readonly DependencyProperty RetainSelectionOnClickOutsideCheckBoxSelectorProperty;
		public static readonly DependencyProperty ShowDataNavigatorProperty;
		public static readonly DependencyProperty AllowPartialGroupingProperty;
		static TableView() {
			Type ownerType = typeof(TableView);
			#region common
			ColumnBandChooserTemplateProperty = TableViewBehavior.RegisterColumnBandChooserTemplateProperty(ownerType);
			FixedNoneContentWidthPropertyKey = TableViewBehavior.RegisterFixedNoneContentWidthProperty(ownerType);
			FixedNoneContentWidthProperty = FixedNoneContentWidthPropertyKey.DependencyProperty;
			TotalSummaryFixedNoneContentWidthPropertyKey = TableViewBehavior.RegisterTotalSummaryFixedNoneContentWidthProperty(ownerType);
			TotalSummaryFixedNoneContentWidthProperty = TotalSummaryFixedNoneContentWidthPropertyKey.DependencyProperty;
			VerticalScrollBarWidthPropertyKey = TableViewBehavior.RegisterVerticalScrollBarWidthProperty(ownerType);
			VerticalScrollBarWidthProperty = VerticalScrollBarWidthPropertyKey.DependencyProperty;
			FixedLeftContentWidthPropertyKey = TableViewBehavior.RegisterFixedLeftContentWidthProperty(ownerType);
			FixedLeftContentWidthProperty = FixedLeftContentWidthPropertyKey.DependencyProperty;
			FixedRightContentWidthPropertyKey = TableViewBehavior.RegisterFixedRightContentWidthProperty(ownerType);
			FixedRightContentWidthProperty = FixedRightContentWidthPropertyKey.DependencyProperty;
			TotalGroupAreaIndentPropertyKey = TableViewBehavior.RegisterTotalGroupAreaIndentProperty(ownerType);
			TotalGroupAreaIndentProperty = TotalGroupAreaIndentPropertyKey.DependencyProperty;
			IndicatorHeaderWidthPropertyKey = TableViewBehavior.RegisterIndicatorHeaderWidthProperty(ownerType);
			IndicatorHeaderWidthProperty = IndicatorHeaderWidthPropertyKey.DependencyProperty;
			ActualDataRowTemplateSelectorPropertyKey = TableViewBehavior.RegisterActualDataRowTemplateSelectorProperty(ownerType);
			ActualDataRowTemplateSelectorProperty = ActualDataRowTemplateSelectorPropertyKey.DependencyProperty;
			BestFitMaxRowCountProperty = DependencyPropertyManager.Register("BestFitMaxRowCount", typeof(int), ownerType, new FrameworkPropertyMetadata(-1, null, (d, baseValue) => CoerceBestFitMaxRowCount(Convert.ToInt32(baseValue))));
			BestFitModeProperty = DependencyPropertyManager.Register("BestFitMode", typeof(BestFitMode), ownerType, new FrameworkPropertyMetadata(BestFitMode.Default));
			BestFitAreaProperty = DependencyPropertyManager.Register("BestFitArea", typeof(BestFitArea), ownerType, new FrameworkPropertyMetadata(BestFitArea.All));
			CustomBestFitEvent = EventManager.RegisterRoutedEvent("CustomBestFit", RoutingStrategy.Direct, typeof(CustomBestFitEventHandler), ownerType);
			FocusedRowBorderTemplateProperty = TableViewBehavior.RegisterFocusedRowBorderTemplateProperty(ownerType);
			AutoWidthProperty = TableViewBehavior.RegisterAutoWidthProperty(ownerType);
			UseGroupShadowIndentProperty = TableViewBehavior.RegisterUseGroupShadowIndentProperty(ownerType);
			LeftDataAreaIndentProperty = TableViewBehavior.RegisterLeftDataAreaIndentProperty(ownerType);
			RightDataAreaIndentProperty = TableViewBehavior.RegisterRightDataAreaIndentProperty(ownerType);
			ShowAutoFilterRowProperty = TableViewBehavior.RegisterShowAutoFilterRowProperty(ownerType);
			AllowCascadeUpdateProperty = TableViewBehavior.RegisterAllowCascadeUpdateProperty(ownerType);
			AllowPerPixelScrollingProperty = TableViewBehavior.RegisterAllowPerPixelScrollingProperty(ownerType);
			ScrollAnimationDurationProperty = TableViewBehavior.RegisterScrollAnimationDurationProperty(ownerType);
			ScrollAnimationModeProperty = TableViewBehavior.RegisterScrollAnimationModeProperty(ownerType);
			AllowScrollAnimationProperty = TableViewBehavior.RegisterAllowScrollAnimationProperty(ownerType);
			ExtendScrollBarToFixedColumnsProperty = TableViewBehavior.RegisterExtendScrollBarToFixedColumnsProperty(ownerType);
			FixedLeftVisibleColumnsPropertyKey = TableViewBehavior.RegisterFixedLeftVisibleColumnsProperty<GridColumn>(ownerType);
			FixedLeftVisibleColumnsProperty = FixedLeftVisibleColumnsPropertyKey.DependencyProperty;
			FixedRightVisibleColumnsPropertyKey = TableViewBehavior.RegisterFixedRightVisibleColumnsProperty<GridColumn>(ownerType);
			FixedRightVisibleColumnsProperty = FixedRightVisibleColumnsPropertyKey.DependencyProperty;
			FixedNoneVisibleColumnsPropertyKey = TableViewBehavior.RegisterFixedNoneVisibleColumnsProperty<GridColumn>(ownerType);
			FixedNoneVisibleColumnsProperty = FixedNoneVisibleColumnsPropertyKey.DependencyProperty;
			HorizontalViewportPropertyKey = TableViewBehavior.RegisterHorizontalViewportProperty(ownerType);
			HorizontalViewportProperty = HorizontalViewportPropertyKey.DependencyProperty;
			FixedLineWidthProperty = TableViewBehavior.RegisterFixedLineWidthProperty(ownerType);
			ShowVerticalLinesProperty = TableViewBehavior.RegisterShowVerticalLinesProperty(ownerType);
			ShowHorizontalLinesProperty = TableViewBehavior.RegisterShowHorizontalLinesProperty(ownerType);
			RowDecorationTemplateProperty = TableViewBehavior.RegisterRowDecorationTemplateProperty(ownerType);
			DefaultDataRowTemplateProperty = TableViewBehavior.RegisterDefaultDataRowTemplateProperty(ownerType);
			DataRowTemplateProperty = TableViewBehavior.RegisterDataRowTemplateProperty(ownerType);
			DataRowTemplateSelectorProperty = TableViewBehavior.RegisterDataRowTemplateSelectorProperty(ownerType);
			RowIndicatorContentTemplateProperty = TableViewBehavior.RegisterRowIndicatorContentTemplateProperty(ownerType);
			AllowResizingProperty = TableViewBehavior.RegisterAllowResizingProperty(ownerType);
			AllowHorizontalScrollingVirtualizationProperty = TableViewBehavior.RegisterAllowHorizontalScrollingVirtualizationProperty(ownerType);
			RowStyleProperty = TableViewBehavior.RegisterRowStyleProperty(ownerType);
			ScrollingVirtualizationMarginPropertyKey = TableViewBehavior.RegisterScrollingVirtualizationMarginProperty(ownerType);
			ScrollingVirtualizationMarginProperty = ScrollingVirtualizationMarginPropertyKey.DependencyProperty;
			ScrollingHeaderVirtualizationMarginPropertyKey = TableViewBehavior.RegisterScrollingHeaderVirtualizationMarginProperty(ownerType);
			ScrollingHeaderVirtualizationMarginProperty = ScrollingHeaderVirtualizationMarginPropertyKey.DependencyProperty;
			RowMinHeightProperty = TableViewBehavior.RegisterRowMinHeightProperty(ownerType);
			HeaderPanelMinHeightProperty = TableViewBehavior.RegisterHeaderPanelMinHeightProperty(ownerType);
			AutoMoveRowFocusProperty = TableViewBehavior.RegisterAutoMoveRowFocusProperty(ownerType);
			AllowBestFitProperty = TableViewBehavior.RegisterAllowBestFitProperty(ownerType);
			ShowIndicatorProperty = TableViewBehavior.RegisterShowIndicatorProperty(ownerType);
			ActualShowIndicatorPropertyKey = TableViewBehavior.RegisterActualShowIndicatorProperty(ownerType);
			ActualShowIndicatorProperty = ActualShowIndicatorPropertyKey.DependencyProperty;
			IndicatorWidthProperty = TableViewBehavior.RegisterIndicatorWidthProperty(ownerType);
			ActualIndicatorWidthPropertyKey = TableViewBehavior.RegisterActualIndicatorWidthPropertyKey(ownerType);
			ActualIndicatorWidthProperty = ActualIndicatorWidthPropertyKey.DependencyProperty;
			ShowTotalSummaryIndicatorIndentPropertyKey = TableViewBehavior.RegisterShowTotalSummaryIndicatorIndentPropertyKey(ownerType);
			ShowTotalSummaryIndicatorIndentProperty = ShowTotalSummaryIndicatorIndentPropertyKey.DependencyProperty;
			AlternateRowBackgroundProperty = TableViewBehavior.RegisterAlternateRowBackgroundProperty(ownerType);
			ActualAlternateRowBackgroundPropertyKey = TableViewBehavior.RegisterActualAlternateRowBackgroundProperty(ownerType);
			ActualAlternateRowBackgroundProperty = ActualAlternateRowBackgroundPropertyKey.DependencyProperty;
			EvenRowBackgroundProperty = TableViewBehavior.RegisterEvenRowBackgroundProperty(ownerType);
			UseEvenRowBackgroundProperty = TableViewBehavior.RegisterUseEvenRowBackgroundProperty(ownerType);
			AlternationCountProperty = TableViewBehavior.RegisterAlternationCountProperty(ownerType);
			ExpandDetailButtonWidthProperty = DependencyPropertyManager.Register("ExpandDetailButtonWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(16d, OnExpandDetailButtonWidthChanged));
			ActualExpandDetailButtonWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualExpandDetailButtonWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(16d));
			ActualExpandDetailButtonWidthProperty = ActualExpandDetailButtonWidthPropertyKey.DependencyProperty;
			DetailMarginProperty = DependencyPropertyManager.Register("DetailMargin", typeof(Thickness), ownerType, new FrameworkPropertyMetadata(new Thickness(16d, 0, 0, 0), OnDetailMarginChanged));
			ActualDetailMarginPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualDetailMargin", typeof(Thickness), ownerType, new FrameworkPropertyMetadata(new Thickness(16d, 0, 0, 0)));
			ActualDetailMarginProperty = ActualDetailMarginPropertyKey.DependencyProperty;
			ActualExpandDetailHeaderWidthPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualExpandDetailHeaderWidth", typeof(double), ownerType, new PropertyMetadata(0d));
			ActualExpandDetailHeaderWidthProperty = ActualExpandDetailHeaderWidthPropertyKey.DependencyProperty;
			MultiSelectModeProperty = TableViewBehavior.RegisterMultiSelectModeProperty(ownerType);
			UseIndicatorForSelectionProperty = TableViewBehavior.RegisterUseIndicatorForSelectionProperty(ownerType);
			AllowFixedColumnMenuProperty = TableViewBehavior.RegisterAllowFixedColumnMenuProperty(ownerType);
			AllowScrollHeadersProperty = TableViewBehavior.RegisterAllowScrollHeadersProperty(ownerType);
			ShowBandsPanelProperty = TableViewBehavior.RegisterShowBandsPanelProperty(ownerType);
			AllowChangeColumnParentProperty = TableViewBehavior.RegisterAllowChangeColumnParentProperty(ownerType);
			AllowChangeBandParentProperty = TableViewBehavior.RegisterAllowChangeBandParentProperty(ownerType);
			ShowBandsInCustomizationFormProperty = TableViewBehavior.RegisterShowBandsInCustomizationFormProperty(ownerType);
			AllowBandMovingProperty = TableViewBehavior.RegisterAllowBandMovingProperty(ownerType);
			AllowBandResizingProperty = TableViewBehavior.RegisterAllowBandResizingProperty(ownerType);
			AllowAdvancedVerticalNavigationProperty = TableViewBehavior.RegisterAllowAdvancedVerticalNavigationProperty(ownerType);
			AllowAdvancedHorizontalNavigationProperty = TableViewBehavior.RegisterAllowAdvancedHorizontalNavigationProperty(ownerType);
			ColumnChooserBandsSortOrderComparerProperty = TableViewBehavior.RegisterColumnChooserBandsSortOrderComparerProperty(ownerType);
			BandHeaderTemplateProperty = TableViewBehavior.RegisterBandHeaderTemplateProperty(ownerType);
			BandHeaderTemplateSelectorProperty = TableViewBehavior.RegisterBandHeaderTemplateSelectorProperty(ownerType);
			BandHeaderToolTipTemplateProperty = TableViewBehavior.RegisterBandHeaderToolTipTemplateProperty(ownerType);
			PrintBandHeaderStyleProperty = TableViewBehavior.RegisterPrintBandHeaderStyleProperty(ownerType);
			HasDetailViewsPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasDetailViews", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			HasDetailViewsProperty = HasDetailViewsPropertyKey.DependencyProperty;
			ShowDetailButtonsProperty = DependencyPropertyManager.Register("ShowDetailButtons", typeof(DefaultBoolean), ownerType, new FrameworkPropertyMetadata(DefaultBoolean.Default, (d, e) => ((TableView)d).OnShowDetailButtonsChanged()));
			AllowMasterDetailProperty = DependencyPropertyManager.Register("AllowMasterDetail", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((TableView)d).OnAllowMasterDetailChanged()));
			ActualShowDetailButtonsPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualShowDetailButtons", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((TableView)d).OnActualShowDetailButtonsChanged()));
			ActualShowDetailButtonsProperty = ActualShowDetailButtonsPropertyKey.DependencyProperty;
			IsDetailButtonVisibleBindingContainerPropertyKey = DependencyPropertyManager.RegisterReadOnly("IsDetailButtonVisibleBindingContainer", typeof(BindingContainer), ownerType, new FrameworkPropertyMetadata(null));
			IsDetailButtonVisibleBindingContainerProperty = IsDetailButtonVisibleBindingContainerPropertyKey.DependencyProperty;
			#endregion
			NewItemRowPositionProperty = DependencyPropertyManager.Register("NewItemRowPosition", typeof(NewItemRowPosition), ownerType, new FrameworkPropertyMetadata(NewItemRowPosition.None, OnNewItemRowPositionChanged));
			InitNewRowEvent = EventManager.RegisterRoutedEvent("InitNewRow", RoutingStrategy.Direct, typeof(InitNewRowEventHandler), ownerType);
			CustomScrollAnimationEvent = TableViewBehavior.RegisterCustomScrollAnimationEvent(ownerType);
			RowDoubleClickEvent = EventManager.RegisterRoutedEvent("RowDoubleClick", RoutingStrategy.Direct, typeof(RowDoubleClickEventHandler), ownerType);
			ShowingGroupFooterEvent = EventManager.RegisterRoutedEvent("ShowingGroupFooter", RoutingStrategy.Direct, typeof(ShowingGroupFooterEventHandler), ownerType);
			GroupFooterRowStyleProperty = DependencyPropertyManager.Register("GroupFooterRowStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			GroupFooterRowTemplateProperty = DependencyPropertyManager.Register("GroupFooterRowTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((TableView)d).UpdateActualGroupFooterRowTemplateSelector()));
			GroupFooterRowTemplateSelectorProperty = DependencyPropertyManager.Register("GroupFooterRowTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((TableView)d).UpdateActualGroupFooterRowTemplateSelector()));
			ActualGroupFooterRowTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGroupFooterRowTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualGroupFooterRowTemplateSelectorProperty = ActualGroupFooterRowTemplateSelectorPropertyKey.DependencyProperty;
			GroupFooterSummaryContentStyleProperty = DependencyPropertyManager.Register("GroupFooterSummaryContentStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			GroupFooterSummaryItemTemplateProperty = DependencyPropertyManager.Register("GroupFooterSummaryItemTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((TableView)d).UpdateActualGroupFooterSummaryItemTemplateSelector()));
			GroupFooterSummaryItemTemplateSelectorProperty = DependencyPropertyManager.Register("GroupFooterSummaryItemTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((TableView)d).UpdateActualGroupFooterSummaryItemTemplateSelector()));
			ActualGroupFooterSummaryItemTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualGroupFooterSummaryItemTemplateSelector", typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualGroupFooterSummaryItemTemplateSelectorProperty = ActualGroupFooterSummaryItemTemplateSelectorPropertyKey.DependencyProperty;
			PrintRowTemplateProperty = DependencyPropertyManager.Register("PrintRowTemplate", typeof(DataTemplate), ownerType, new UIPropertyMetadata(null));
			PrintGroupFooterTemplateProperty = DependencyPropertyManager.Register("PrintGroupFooterTemplate", typeof(DataTemplate), ownerType, new UIPropertyMetadata(null));
			PrintAutoWidthProperty = DependencyPropertyManager.Register("PrintAutoWidth", typeof(bool), ownerType, new UIPropertyMetadata(true));
			PrintColumnHeadersProperty = DependencyPropertyManager.Register("PrintColumnHeaders", typeof(bool), ownerType, new UIPropertyMetadata(true));
			PrintBandHeadersProperty = DependencyPropertyManager.Register("PrintBandHeaders", typeof(bool), ownerType, new UIPropertyMetadata(true));
			PrintGroupFootersProperty = DependencyPropertyManager.Register("PrintGroupFooters", typeof(bool), ownerType, new UIPropertyMetadata(true));
			AllowPrintDetailsProperty = DependencyPropertyManager.Register("AllowPrintDetails", typeof(DefaultBoolean), ownerType, new UIPropertyMetadata(DefaultBoolean.Default));
			AllowPrintEmptyDetailsProperty = DependencyPropertyManager.Register("AllowPrintEmptyDetails", typeof(DefaultBoolean), ownerType, new UIPropertyMetadata(DefaultBoolean.Default));
			PrintAllDetailsProperty = DependencyPropertyManager.Register("PrintAllDetails", typeof(DefaultBoolean), ownerType, new UIPropertyMetadata(DefaultBoolean.Default));
			PrintColumnHeaderStyleProperty = DependencyPropertyManager.Register("PrintColumnHeaderStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, OnUpdateColumnsAppearance));
			PrintGroupFooterStyleProperty = DependencyPropertyManager.Register("PrintGroupFooterStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			PrintDetailTopIndentProperty = DependencyPropertyManager.Register("PrintDetailTopIndent", typeof(double), ownerType, new FrameworkPropertyMetadata(GridPrintingHelper.DefaultDetailTopIndent, OnUpdateColumnsAppearance));
			PrintDetailBottomIndentProperty = DependencyPropertyManager.Register("PrintDetailBottomIndent", typeof(double), ownerType, new FrameworkPropertyMetadata(GridPrintingHelper.DefaultDetailBottomIndent, OnUpdateColumnsAppearance));
			LeftGroupAreaIndentProperty = DependencyPropertyManager.Register("LeftGroupAreaIndent", typeof(double), ownerType, new FrameworkPropertyMetadata(0d, (d, e) => ((TableView)d).RebuildVisibleColumns()));
			RightGroupAreaIndentProperty = DependencyPropertyManager.Register("RightGroupAreaIndent", typeof(double), ownerType, new FrameworkPropertyMetadata(0d, (d, e) => ((TableView)d).RebuildVisibleColumns()));
			PrintGroupSummaryDisplayModeProperty = DependencyPropertyManager.Register("PrintGroupSummaryDisplayMode", typeof(GroupSummaryDisplayMode), ownerType, new PropertyMetadata(GroupSummaryDisplayMode.Default));
			AutoFilterRowCellStyleProperty = DependencyPropertyManager.Register("AutoFilterRowCellStyle", typeof(Style), ownerType, new PropertyMetadata(null, OnUpdateColumnsAppearance));
			NewItemRowCellStyleProperty = DependencyPropertyManager.Register("NewItemRowCellStyle", typeof(Style), ownerType, new PropertyMetadata(null, OnUpdateColumnsAppearance));
			AllowFixedGroupsProperty = DependencyPropertyManager.Register("AllowFixedGroups", typeof(DefaultBoolean), ownerType, new PropertyMetadata(DefaultBoolean.Default, (d, e) => ((TableView)d).OnAllowFixedGroupsChanged()));
			GroupSummaryDisplayModeProperty = DependencyPropertyManager.Register("GroupSummaryDisplayMode", typeof(GroupSummaryDisplayMode), ownerType, new PropertyMetadata(GroupSummaryDisplayMode.Default, (d, e) => ((TableView)d).OnGroupSummaryDisplayModeChanged()));
			GroupColumnSummaryItemTemplateProperty = DependencyPropertyManager.Register("GroupColumnSummaryItemTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null, (d, e) => ((TableView)d).UpdateGroupSummaryTemplates()));
			GroupColumnSummaryContentStyleProperty = DependencyPropertyManager.Register("GroupColumnSummaryContentStyle", typeof(Style), ownerType, new PropertyMetadata(null, (d, e) => ((TableView)d).UpdateGroupSummaryTemplates()));
			GroupBandSummaryContentStyleProperty = DependencyPropertyManager.Register("GroupBandSummaryContentStyle", typeof(Style), ownerType, new PropertyMetadata(null));
			AllowGroupSummaryCascadeUpdateProperty = DependencyPropertyManager.Register("AllowGroupSummaryCascadeUpdate", typeof(bool), ownerType, new PropertyMetadata(false));
			VerticalScrollbarVisibilityProperty = DependencyPropertyManager.Register("VerticalScrollbarVisibility", typeof(ScrollBarVisibility), ownerType, new PropertyMetadata(ScrollBarVisibility.Visible));
			HorizontalScrollbarVisibilityProperty = DependencyPropertyManager.Register("HorizontalScrollbarVisibility", typeof(ScrollBarVisibility), ownerType, new PropertyMetadata(ScrollBarVisibility.Auto));
			ExpandColumnPositionPropertyKey = DependencyPropertyManager.RegisterReadOnly("ExpandColumnPosition", typeof(ColumnPosition), ownerType, new FrameworkPropertyMetadata(ColumnPosition.Middle));
			ExpandColumnPositionProperty = ExpandColumnPositionPropertyKey.DependencyProperty;
			ShowGroupFootersProperty = DependencyPropertyManager.Register("ShowGroupFooters", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((TableView)d).OnGroupSummaryDisplayModeChanged()));
			AllowPartialGroupingProperty = DependencyPropertyManager.Register("AllowPartialGrouping", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((TableView)d).OnAllowPartialGroupingChanged()));
			ShowDataNavigatorProperty = DependencyPropertyManager.Register("ShowDataNavigator", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));			
			RegisterClassCommandBindings();
			RegisterSerializationEvents();
			CloneDetailHelper.RegisterKnownPropertyKeys(ownerType, FixedNoneContentWidthPropertyKey, ActualShowIndicatorPropertyKey, ActualIndicatorWidthPropertyKey, ActualExpandDetailButtonWidthPropertyKey, IsDetailButtonVisibleBindingContainerPropertyKey, ActualDetailMarginPropertyKey, FixedLeftContentWidthPropertyKey, FixedRightContentWidthPropertyKey, TotalGroupAreaIndentPropertyKey);
			#if !SL
			UseLightweightTemplatesProperty = TableViewBehavior.RegisterUseLightweightTemplatesProperty(ownerType);
			RowDetailsTemplateProperty = TableViewBehavior.RegisterRowDetailsTemplateProperty(ownerType);
			RowDetailsTemplateSelectorProperty = TableViewBehavior.RegisterRowDetailsTemplateSelectorProperty(ownerType);
			ActualRowDetailsTemplateSelectorPropertyKey = TableViewBehavior.RegisterActualRowDetailsTemplateSelectorProperty(ownerType);
			ActualRowDetailsTemplateSelectorProperty = ActualRowDetailsTemplateSelectorPropertyKey.DependencyProperty;
			RowDetailsVisibilityModeProperty = TableViewBehavior.RegisterRowDetailsVisibilityModeProperty(ownerType);
			#endif
			ShowCheckBoxSelectorInGroupRowProperty = DependencyProperty.Register("ShowCheckBoxSelectorInGroupRow", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((TableView)d).OnShowCheckBoxSelectorInGroupRowChanged()));
			ShowCheckBoxSelectorColumnProperty = DependencyProperty.Register("ShowCheckBoxSelectorColumn", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((TableView)d).UpdateIsCheckBoxSelectorColumnVisible()));
			CheckBoxSelectorColumnWidthProperty = DependencyProperty.Register("CheckBoxSelectorColumnWidth", typeof(double), ownerType, new FrameworkPropertyMetadata(75d, (d, e) => ((TableView)d).OnCheckBoxSelectorColumnWidthChanged()));
			CheckBoxSelectorColumnHeaderTemplateProperty = DependencyProperty.Register("CheckBoxSelectorColumnHeaderTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((TableView)d).OnCheckBoxSelectorColumnHeaderTemplateChanged()));
			RetainSelectionOnClickOutsideCheckBoxSelectorProperty = DependencyProperty.Register("RetainSelectionOnClickOutsideCheckBoxSelector", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
		}
		static partial void RegisterClassCommandBindings();
		static partial void RegisterSerializationEvents();
		static void OnNewItemRowPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((TableView)d).OnNewItemRowPositionChanged((NewItemRowPosition)e.OldValue);
		}
		static void OnExpandDetailButtonWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TableView view = ((TableView)d);
			if(view.DetailMargin.Left != view.ExpandDetailButtonWidth)
				view.DetailMargin = new Thickness((double)e.NewValue, 0, 0, 0);
			OnDetailMarginChanged((ITableView)view, e);
		}
		static void OnDetailMarginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TableView view = ((TableView)d);
			if(view.ExpandDetailButtonWidth != view.DetailMargin.Left)
				view.ExpandDetailButtonWidth = view.DetailMargin.Left;
			OnDetailMarginChanged((ITableView)view, e);
		}
		static void OnDetailMarginChanged(ITableView view, DependencyPropertyChangedEventArgs e) {
			view.TableViewBehavior.UpdateActualExpandDetailButtonWidth();
			view.TableViewBehavior.UpdateActualDetailMargin();
			if(view.ViewBase.OriginationView == null && view.ViewBase.DataControl != null)
				view.ViewBase.DataControl.UpdateAllDetailViewIndents();
			view.ViewBase.RebuildVisibleColumns();
		}
		internal override void UpdateActualFadeSelectionOnLostFocus(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TableViewBehavior.OnFadeSelectionOnLostFocusChanged(d, e);
		}
		#endregion
		[Browsable(false), CloneDetailMode(CloneDetailMode.Skip), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public GridTableViewLayoutCalculatorFactory LayoutCalculatorFactory {
			get { return ViewInfo.LayoutCalculatorFactory; }
			set { ViewInfo.LayoutCalculatorFactory = value; }
		}
		protected internal override bool CanAdjustScrollbar() {
			return base.CanAdjustScrollbar() && !Grid.IsAsyncOperationInProgress;
		}
		protected internal TableViewBehavior TableViewBehavior { get { return (TableViewBehavior)ViewBehavior; } }
		protected internal GridViewInfo ViewInfo { get { return TableViewBehavior.ViewInfo; } }
		protected internal override bool UseMouseUpFocusedEditorShowModeStrategy { get { return IsMultiCellSelection; } }
		public TableView(): this(null, null, null) { }
		public TableView(MasterNodeContainer masterRootNode, MasterRowsContainer masterRootDataItem, DataControlDetailDescriptor detailDescriptor)
			: base(masterRootNode, masterRootDataItem, detailDescriptor) {
			this.SetDefaultStyleKey(typeof(TableView));
			NewItemRowData = new AdditionalRowData(visualDataTreeBuilder) { RowHandle = new RowHandle(GridControl.NewItemRowHandle) };
			bandMenuControllerValue = CreateMenuControllerLazyValue();
			CheckBoxSelectorColumn = CreateCheckBoxSelectorColumn();
		}
		#region common public properties
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewFixedNoneContentWidth"),
#endif
 CloneDetailMode(CloneDetailMode.Force)]
		public double FixedNoneContentWidth {
			get { return (double)GetValue(FixedNoneContentWidthProperty); }
			private set { this.SetValue(FixedNoneContentWidthPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TableViewTotalSummaryFixedNoneContentWidth")]
#endif
		public double TotalSummaryFixedNoneContentWidth {
			get { return (double)GetValue(TotalSummaryFixedNoneContentWidthProperty); }
			private set { this.SetValue(TotalSummaryFixedNoneContentWidthPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewVerticalScrollBarWidth"),
#endif
 EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public double VerticalScrollBarWidth {
			get { return (double)GetValue(VerticalScrollBarWidthProperty); }
			private set { this.SetValue(VerticalScrollBarWidthPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewFixedLeftContentWidth"),
#endif
 CloneDetailMode(CloneDetailMode.Force)]
		public double FixedLeftContentWidth {
			get { return (double)GetValue(FixedLeftContentWidthProperty); }
			private set { this.SetValue(FixedLeftContentWidthPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewFixedRightContentWidth"),
#endif
 CloneDetailMode(CloneDetailMode.Force)]
		public double FixedRightContentWidth {
			get { return (double)GetValue(FixedRightContentWidthProperty); }
			private set { this.SetValue(FixedRightContentWidthPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewTotalGroupAreaIndent"),
#endif
 CloneDetailMode(CloneDetailMode.Force)]
		public double TotalGroupAreaIndent {
			get { return (double)GetValue(TotalGroupAreaIndentProperty); }
			private set { this.SetValue(TotalGroupAreaIndentPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TableViewIndicatorHeaderWidth")]
#endif
		public double IndicatorHeaderWidth {
			get { return (double)GetValue(IndicatorHeaderWidthProperty); }
			private set { this.SetValue(IndicatorHeaderWidthPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewColumnBandChooserTemplate"),
#endif
 Category(Categories.Appearance)]
		public ControlTemplate ColumnBandChooserTemplate {
			get { return (ControlTemplate)GetValue(ColumnBandChooserTemplateProperty); }
			set { SetValue(ColumnBandChooserTemplateProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TableViewActualDataRowTemplateSelector")]
#endif
		public DataTemplateSelector ActualDataRowTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualDataRowTemplateSelectorProperty); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewFocusedRowBorderTemplate"),
#endif
 Category(Categories.Appearance)]
		public ControlTemplate FocusedRowBorderTemplate {
			get { return (ControlTemplate)GetValue(FocusedRowBorderTemplateProperty); }
			set { SetValue(FocusedRowBorderTemplateProperty, value); }
		}
		[Obsolete("Use the DataControlBase.SelectionMode property instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), XtraSerializableProperty, Category(Categories.OptionsSelection), CloneDetailMode(CloneDetailMode.Skip)]
		public TableViewSelectMode MultiSelectMode {
			get { return (TableViewSelectMode)GetValue(MultiSelectModeProperty); }
			set { SetValue(MultiSelectModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewUseIndicatorForSelection"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsSelection)]
		public bool UseIndicatorForSelection {
			get { return (bool)GetValue(UseIndicatorForSelectionProperty); }
			set { SetValue(UseIndicatorForSelectionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowFixedColumnMenu"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBehavior)]
		public bool AllowFixedColumnMenu {
			get { return (bool)GetValue(AllowFixedColumnMenuProperty); }
			set { SetValue(AllowFixedColumnMenuProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowScrollHeaders"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBehavior)]
		public bool AllowScrollHeaders {
			get { return (bool)GetValue(AllowScrollHeadersProperty); }
			set { SetValue(AllowScrollHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewShowGroupFooters"),
#endif
 XtraSerializableProperty, Category(Categories.Appearance)]
		public bool ShowGroupFooters {
			get { return (bool)GetValue(ShowGroupFootersProperty); }
			set { SetValue(ShowGroupFootersProperty, value); }
		}
		[ XtraSerializableProperty, Category(Categories.OptionsBehavior)]
		public bool AllowPartialGrouping {
			get { return (bool)GetValue(AllowPartialGroupingProperty); }
			set { SetValue(AllowPartialGroupingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewShowBandsPanel"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool ShowBandsPanel {
			get { return (bool)GetValue(ShowBandsPanelProperty); }
			set { SetValue(ShowBandsPanelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewShowDataNavigator"),
#endif
 XtraSerializableProperty, Category(Categories.View)]
		public bool ShowDataNavigator {
			get { return (bool)GetValue(ShowDataNavigatorProperty); }
			set { SetValue(ShowDataNavigatorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowChangeColumnParent"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool AllowChangeColumnParent {
			get { return (bool)GetValue(AllowChangeColumnParentProperty); }
			set { SetValue(AllowChangeColumnParentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowChangeBandParent"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool AllowChangeBandParent {
			get { return (bool)GetValue(AllowChangeBandParentProperty); }
			set { SetValue(AllowChangeBandParentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewShowBandsInCustomizationForm"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool ShowBandsInCustomizationForm {
			get { return (bool)GetValue(ShowBandsInCustomizationFormProperty); }
			set { SetValue(ShowBandsInCustomizationFormProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowBandMoving"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool AllowBandMoving {
			get { return (bool)GetValue(AllowBandMovingProperty); }
			set { SetValue(AllowBandMovingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowBandResizing"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool AllowBandResizing {
			get { return (bool)GetValue(AllowBandResizingProperty); }
			set { SetValue(AllowBandResizingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowAdvancedVerticalNavigation"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool AllowAdvancedVerticalNavigation {
			get { return (bool)GetValue(AllowAdvancedVerticalNavigationProperty); }
			set { SetValue(AllowAdvancedVerticalNavigationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowAdvancedHorizontalNavigation"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool AllowAdvancedHorizontalNavigation {
			get { return (bool)GetValue(AllowAdvancedHorizontalNavigationProperty); }
			set { SetValue(AllowAdvancedHorizontalNavigationProperty, value); }
		}
		[Browsable(false)]
		public IComparer<BandBase> ColumnChooserBandsSortOrderComparer {
			get { return (IComparer<BandBase>)GetValue(ColumnChooserBandsSortOrderComparerProperty); }
			set { SetValue(ColumnChooserBandsSortOrderComparerProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewBandHeaderTemplate"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public DataTemplate BandHeaderTemplate {
			get { return (DataTemplate)GetValue(BandHeaderTemplateProperty); }
			set { SetValue(BandHeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewBandHeaderTemplateSelector"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public DataTemplateSelector BandHeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(BandHeaderTemplateSelectorProperty); }
			set { SetValue(BandHeaderTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewBandHeaderToolTipTemplate"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public DataTemplate BandHeaderToolTipTemplate {
			get { return (DataTemplate)GetValue(BandHeaderToolTipTemplateProperty); }
			set { SetValue(BandHeaderToolTipTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewPrintBandHeaderStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style PrintBandHeaderStyle {
			get { return (Style)GetValue(PrintBandHeaderStyleProperty); }
			set { SetValue(PrintBandHeaderStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewScrollingVirtualizationMargin"),
#endif
 Browsable(false)]
		public Thickness ScrollingVirtualizationMargin {
			get { return (Thickness)GetValue(ScrollingVirtualizationMarginProperty); }
			internal set { this.SetValue(ScrollingVirtualizationMarginPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewScrollingHeaderVirtualizationMargin"),
#endif
 Browsable(false)]
		public Thickness ScrollingHeaderVirtualizationMargin {
			get { return (Thickness)GetValue(ScrollingHeaderVirtualizationMarginProperty); }
			internal set { this.SetValue(ScrollingHeaderVirtualizationMarginPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewRowStyle"),
#endif
 Category(Categories.Appearance)]
		public Style RowStyle {
			get { return (Style)GetValue(RowStyleProperty); }
			set { SetValue(RowStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewShowAutoFilterRow"),
#endif
 Category(Categories.OptionsFilter), XtraSerializableProperty]
		public bool ShowAutoFilterRow {
			get { return (bool)GetValue(ShowAutoFilterRowProperty); }
			set { SetValue(ShowAutoFilterRowProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowCascadeUpdate"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool AllowCascadeUpdate {
			get { return (bool)GetValue(AllowCascadeUpdateProperty); }
			set { SetValue(AllowCascadeUpdateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowPerPixelScrolling"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool AllowPerPixelScrolling {
			get { return (bool)GetValue(AllowPerPixelScrollingProperty); }
			set { SetValue(AllowPerPixelScrollingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewScrollAnimationDuration"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public double ScrollAnimationDuration {
			get { return (double)GetValue(ScrollAnimationDurationProperty); }
			set { SetValue(ScrollAnimationDurationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewScrollAnimationMode"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public ScrollAnimationMode ScrollAnimationMode {
			get { return (ScrollAnimationMode)GetValue(ScrollAnimationModeProperty); }
			set { SetValue(ScrollAnimationModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowScrollAnimation"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool AllowScrollAnimation {
			get { return (bool)GetValue(AllowScrollAnimationProperty); }
			set { SetValue(AllowScrollAnimationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewExtendScrollBarToFixedColumns"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool ExtendScrollBarToFixedColumns {
			get { return (bool)GetValue(ExtendScrollBarToFixedColumnsProperty); }
			set { SetValue(ExtendScrollBarToFixedColumnsProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TableViewAutoFilterRowData")]
#endif
		public RowData AutoFilterRowData { get { return ((TableViewBehavior)ViewBehavior).AutoFilterRowData; } }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowHorizontalScrollingVirtualization"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AllowHorizontalScrollingVirtualization {
			get { return (bool)GetValue(AllowHorizontalScrollingVirtualizationProperty); }
			set { SetValue(AllowHorizontalScrollingVirtualizationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewRowMinHeight"),
#endif
 Category(Categories.Appearance)]
		public double RowMinHeight {
			get { return (double)GetValue(RowMinHeightProperty); }
			set { SetValue(RowMinHeightProperty, value); }
		}
		public double HeaderPanelMinHeight {
			get { return (double)GetValue(HeaderPanelMinHeightProperty); }
			set { SetValue(HeaderPanelMinHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewRowDecorationTemplate"),
#endif
 Category(Categories.Appearance)]
		public ControlTemplate RowDecorationTemplate {
			get { return (ControlTemplate)GetValue(RowDecorationTemplateProperty); }
			set { SetValue(RowDecorationTemplateProperty, value); }
		}
		[Category(Categories.Appearance), Browsable(false)]
		public DataTemplate DefaultDataRowTemplate {
			get { return (DataTemplate)GetValue(DefaultDataRowTemplateProperty); }
			set { SetValue(DefaultDataRowTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewDataRowTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate DataRowTemplate {
			get { return (DataTemplate)GetValue(DataRowTemplateProperty); }
			set { SetValue(DataRowTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewDataRowTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector DataRowTemplateSelector {
			get { return (DataTemplateSelector)GetValue(DataRowTemplateSelectorProperty); }
			set { SetValue(DataRowTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewRowIndicatorContentTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate RowIndicatorContentTemplate {
			get { return (DataTemplate)GetValue(RowIndicatorContentTemplateProperty); }
			set { SetValue(RowIndicatorContentTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAutoMoveRowFocus"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AutoMoveRowFocus {
			get { return (bool)GetValue(AutoMoveRowFocusProperty); }
			set { SetValue(AutoMoveRowFocusProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewBestFitMaxRowCount"),
#endif
 Category(Categories.BestFit), XtraSerializableProperty]
		public int BestFitMaxRowCount {
			get { return (int)GetValue(BestFitMaxRowCountProperty); }
			set { SetValue(BestFitMaxRowCountProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewBestFitMode"),
#endif
 Category(Categories.BestFit), XtraSerializableProperty]
		public BestFitMode BestFitMode {
			get { return (BestFitMode)GetValue(BestFitModeProperty); }
			set { SetValue(BestFitModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewBestFitArea"),
#endif
 Category(Categories.BestFit), XtraSerializableProperty]
		public BestFitArea BestFitArea {
			get { return (BestFitArea)GetValue(BestFitAreaProperty); }
			set { SetValue(BestFitAreaProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowBestFit"),
#endif
 Category(Categories.BestFit), XtraSerializableProperty]
		public bool AllowBestFit {
			get { return (bool)GetValue(AllowBestFitProperty); }
			set { SetValue(AllowBestFitProperty, value); }
		}
		[Category(Categories.BestFit)]
		public event CustomBestFitEventHandler CustomBestFit {
			add { AddHandler(CustomBestFitEvent, value); }
			remove { RemoveHandler(CustomBestFitEvent, value); }
		}		
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewShowIndicator"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool ShowIndicator {
			get { return (bool)GetValue(ShowIndicatorProperty); }
			set { SetValue(ShowIndicatorProperty, value); }
		}
		[CloneDetailMode(CloneDetailMode.Force)]
		public bool ActualShowIndicator {
			get { return (bool)GetValue(ActualShowIndicatorProperty); }
			protected set { this.SetValue(ActualShowIndicatorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewIndicatorWidth"),
#endif
 Category(Categories.Appearance), XtraSerializableProperty]
		public double IndicatorWidth {
			get { return (double)GetValue(IndicatorWidthProperty); }
			set { SetValue(IndicatorWidthProperty, value); }
		}
		[CloneDetailMode(CloneDetailMode.Force)]
		public double ActualIndicatorWidth {
			get { return (double)GetValue(ActualIndicatorWidthProperty); }
			protected set { this.SetValue(ActualIndicatorWidthPropertyKey, value); }
		}
		public double ExpandDetailButtonWidth {
			get { return (double)GetValue(ExpandDetailButtonWidthProperty); }
			set { SetValue(ExpandDetailButtonWidthProperty, value); }
		}
		[CloneDetailMode(CloneDetailMode.Force)]
		public double ActualExpandDetailButtonWidth {
			get { return (double)GetValue(ActualExpandDetailButtonWidthProperty); }
			internal set { this.SetValue(ActualExpandDetailButtonWidthPropertyKey, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public Thickness DetailMargin {
			get { return (Thickness)GetValue(DetailMarginProperty); }
			set { SetValue(DetailMarginProperty, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false), CloneDetailMode(CloneDetailMode.Force)]
		public Thickness ActualDetailMargin {
			get { return (Thickness)GetValue(ActualDetailMarginProperty); }
			internal set { this.SetValue(ActualDetailMarginPropertyKey, value); }
		}
		public double ActualExpandDetailHeaderWidth {
			get { return (double)GetValue(ActualExpandDetailHeaderWidthProperty); }
			protected set { this.SetValue(ActualExpandDetailHeaderWidthPropertyKey, value); }
		}
		public ColumnPosition ExpandColumnPosition {
			get { return (ColumnPosition)GetValue(ExpandColumnPositionProperty); }
			protected set { this.SetValue(ExpandColumnPositionPropertyKey, value); }
		}
		public bool ShowTotalSummaryIndicatorIndent {
			get { return (bool)GetValue(ShowTotalSummaryIndicatorIndentProperty); }
			protected set { this.SetValue(ShowTotalSummaryIndicatorIndentPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewShowVerticalLines"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool ShowVerticalLines {
			get { return (bool)base.GetValue(ShowVerticalLinesProperty); }
			set { SetValue(ShowVerticalLinesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewShowHorizontalLines"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool ShowHorizontalLines {
			get { return (bool)base.GetValue(ShowHorizontalLinesProperty); }
			set { SetValue(ShowHorizontalLinesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAutoWidth"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AutoWidth {
			get { return (bool)base.GetValue(AutoWidthProperty); }
			set { SetValue(AutoWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowResizing"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AllowResizing {
			get { return (bool)GetValue(AllowResizingProperty); }
			set { SetValue(AllowResizingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewFixedLineWidth"),
#endif
 Category(Categories.Appearance), XtraSerializableProperty]
		public double FixedLineWidth {
			get { return (double)base.GetValue(FixedLineWidthProperty); }
			set { SetValue(FixedLineWidthProperty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool UseGroupShadowIndent {
			get { return (bool)GetValue(UseGroupShadowIndentProperty); }
			set { SetValue(UseGroupShadowIndentProperty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double LeftDataAreaIndent {
			get { return (double)GetValue(LeftDataAreaIndentProperty); }
			set { SetValue(LeftDataAreaIndentProperty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double RightDataAreaIndent {
			get { return (double)GetValue(RightDataAreaIndentProperty); }
			set { SetValue(RightDataAreaIndentProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TableViewFixedLeftVisibleColumns")]
#endif
		public IList<GridColumn> FixedLeftVisibleColumns {
			get { return (IList<GridColumn>)GetValue(FixedLeftVisibleColumnsProperty); }
			private set { this.SetValue(FixedLeftVisibleColumnsPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TableViewFixedRightVisibleColumns")]
#endif
		public IList<GridColumn> FixedRightVisibleColumns {
			get { return (IList<GridColumn>)GetValue(FixedRightVisibleColumnsProperty); }
			private set { this.SetValue(FixedRightVisibleColumnsPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TableViewFixedNoneVisibleColumns")]
#endif
		public IList<GridColumn> FixedNoneVisibleColumns {
			get { return (IList<GridColumn>)GetValue(FixedNoneVisibleColumnsProperty); }
			private set { this.SetValue(FixedNoneVisibleColumnsPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TableViewHorizontalViewport")]
#endif
		public double HorizontalViewport {
			get { return (double)GetValue(HorizontalViewportProperty); }
			private set { this.SetValue(HorizontalViewportPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAutoFilterRowCellStyle"),
#endif
 Category(Categories.Appearance)]
		public Style AutoFilterRowCellStyle {
			get { return (Style)GetValue(AutoFilterRowCellStyleProperty); }
			set { SetValue(AutoFilterRowCellStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewNewItemRowCellStyle"),
#endif
 Category(Categories.Appearance)]
		public Style NewItemRowCellStyle {
			get { return (Style)GetValue(NewItemRowCellStyleProperty); }
			set { SetValue(NewItemRowCellStyleProperty, value); }
		}
		public bool HasDetailViews {
			get { return (bool)GetValue(HasDetailViewsProperty); }
			private set { this.SetValue(HasDetailViewsPropertyKey, value); }
		}
		public DefaultBoolean ShowDetailButtons {
			get { return (DefaultBoolean)GetValue(ShowDetailButtonsProperty); }
			set { SetValue(ShowDetailButtonsProperty, value); }
		}
		public bool AllowMasterDetail {
			get { return (bool)GetValue(AllowMasterDetailProperty); }
			set { SetValue(AllowMasterDetailProperty, value); }
		}
		public bool ActualShowDetailButtons {
			get { return (bool)GetValue(ActualShowDetailButtonsProperty); }
			private set { this.SetValue(ActualShowDetailButtonsPropertyKey, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never), CloneDetailMode(CloneDetailMode.Force)]
		public BindingContainer IsDetailButtonVisibleBindingContainer {
			get { return (BindingContainer)GetValue(IsDetailButtonVisibleBindingContainerProperty); }
			private set { this.SetValue(IsDetailButtonVisibleBindingContainerPropertyKey, value); }
		}
		BindingBase isDetailButtonVisibleBinding;
		[DefaultValue(null)]
		public BindingBase IsDetailButtonVisibleBinding {
			get { return isDetailButtonVisibleBinding; }
			set {
				if(isDetailButtonVisibleBinding == value)
					return;
				isDetailButtonVisibleBinding = value; 
				IsDetailButtonVisibleBindingContainer = isDetailButtonVisibleBinding != null ? new BindingContainer(isDetailButtonVisibleBinding) : null;
			}
		}
		[Category(Categories.Appearance)]
		public Brush AlternateRowBackground {
			get { return (Brush)GetValue(AlternateRowBackgroundProperty); }
			set { SetValue(AlternateRowBackgroundProperty, value); }
		}
		public Brush ActualAlternateRowBackground {
			get { return (Brush)GetValue(ActualAlternateRowBackgroundProperty); }
			protected set { this.SetValue(ActualAlternateRowBackgroundPropertyKey, value); }
		}
		[Category(Categories.Appearance)]
		public Brush EvenRowBackground {
			get { return (Brush)GetValue(EvenRowBackgroundProperty); }
			set { SetValue(EvenRowBackgroundProperty, value); }
		}
		[Category(Categories.Appearance)]
		public bool UseEvenRowBackground {
			get { return (bool)GetValue(UseEvenRowBackgroundProperty); }
			set { SetValue(UseEvenRowBackgroundProperty, value); }
		}
		protected internal override void UpdateAlternateRowBackground() {
			ActualAlternateRowBackground = AlternateRowBackground ?? (UseEvenRowBackground ? EvenRowBackground : null);
		}
		[Category(Categories.Appearance)]
		public int AlternationCount {
			get { return (int)GetValue(AlternationCountProperty); }
			set { SetValue(AlternationCountProperty, value); }
		}
#if !SL
		[Browsable(false)]
		public bool ShouldSerializeFixedLeftVisibleColumns(XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeFixedRightVisibleColumns(XamlDesignerSerializationManager manager) {
			return false;
		}
		[Browsable(false)]
		public bool ShouldSerializeFixedNoneVisibleColumns(XamlDesignerSerializationManager manager) {
			return false;
		}
#endif
		#endregion
		#region public properties
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double LeftGroupAreaIndent {
			get { return (double)GetValue(LeftGroupAreaIndentProperty); }
			set { SetValue(LeftGroupAreaIndentProperty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double RightGroupAreaIndent {
			get { return (double)GetValue(RightGroupAreaIndentProperty); }
			set { SetValue(RightGroupAreaIndentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewPrintRowTemplate"),
#endif
 Category(Categories.AppearancePrint)]
		public DataTemplate PrintRowTemplate {
			get { return (DataTemplate)GetValue(PrintRowTemplateProperty); }
			set { SetValue(PrintRowTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewPrintAutoWidth"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public bool PrintAutoWidth {
			get { return (bool)GetValue(PrintAutoWidthProperty); }
			set { SetValue(PrintAutoWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewPrintColumnHeaders"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public bool PrintColumnHeaders {
			get { return (bool)GetValue(PrintColumnHeadersProperty); }
			set { SetValue(PrintColumnHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewPrintBandHeaders"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public bool PrintBandHeaders {
			get { return (bool)GetValue(PrintBandHeadersProperty); }
			set { SetValue(PrintBandHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewPrintGroupFooters"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public bool PrintGroupFooters {
			get { return (bool)GetValue(PrintGroupFootersProperty); }
			set { SetValue(PrintGroupFootersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowPrintDetails"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public DefaultBoolean AllowPrintDetails {
			get { return (DefaultBoolean)GetValue(AllowPrintDetailsProperty); }
			set { SetValue(AllowPrintDetailsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowPrintEmptyDetails"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public DefaultBoolean AllowPrintEmptyDetails {
			get { return (DefaultBoolean)GetValue(AllowPrintEmptyDetailsProperty); }
			set { SetValue(AllowPrintEmptyDetailsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowPrintEmptyDetails"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public DefaultBoolean PrintAllDetails {
			get { return (DefaultBoolean)GetValue(PrintAllDetailsProperty); }
			set { SetValue(PrintAllDetailsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewPrintGroupFooterTemplate"),
#endif
 Category(Categories.AppearancePrint)]
		public DataTemplate PrintGroupFooterTemplate {
			get { return (DataTemplate)GetValue(PrintGroupFooterTemplateProperty); }
			set { SetValue(PrintGroupFooterTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewPrintColumnHeaderStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style PrintColumnHeaderStyle {
			get { return (Style)GetValue(PrintColumnHeaderStyleProperty); }
			set { SetValue(PrintColumnHeaderStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewPrintGroupFooterStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style PrintGroupFooterStyle {
			get { return (Style)GetValue(PrintGroupFooterStyleProperty); }
			set { SetValue(PrintGroupFooterStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewPrintDetailTopIndent"),
#endif
 Category(Categories.AppearancePrint)]
		public double PrintDetailTopIndent {
			get { return (double)GetValue(PrintDetailTopIndentProperty); }
			set { SetValue(PrintDetailTopIndentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewPrintDetailBottomIndent"),
#endif
 Category(Categories.AppearancePrint)]
		public double PrintDetailBottomIndent {
			get { return (double)GetValue(PrintDetailBottomIndentProperty); }
			set { SetValue(PrintDetailBottomIndentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewPrintGroupSummaryDisplayMode"),
#endif
 Category(Categories.AppearancePrint), XtraSerializableProperty]
		public GroupSummaryDisplayMode PrintGroupSummaryDisplayMode {
			get { return (GroupSummaryDisplayMode)GetValue(PrintGroupSummaryDisplayModeProperty); }
			set { SetValue(PrintGroupSummaryDisplayModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewNewItemRowPosition"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public NewItemRowPosition NewItemRowPosition {
			get { return (NewItemRowPosition)GetValue(NewItemRowPositionProperty); }
			set { SetValue(NewItemRowPositionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowFixedGroups"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public DefaultBoolean AllowFixedGroups {
			get { return (DefaultBoolean)GetValue(AllowFixedGroupsProperty); }
			set { SetValue(AllowFixedGroupsProperty, value); }
		}
		[ Category(Categories.OptionsView), XtraSerializableProperty]
		public GroupSummaryDisplayMode GroupSummaryDisplayMode {
			get { return (GroupSummaryDisplayMode)GetValue(GroupSummaryDisplayModeProperty); }
			set { SetValue(GroupSummaryDisplayModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewGroupColumnSummaryItemTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate GroupColumnSummaryItemTemplate {
			get { return (DataTemplate)GetValue(GroupColumnSummaryItemTemplateProperty); }
			set { SetValue(GroupColumnSummaryItemTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewGroupBandSummaryContentStyle"),
#endif
 Category(Categories.Appearance)]
		public Style GroupBandSummaryContentStyle {
			get { return (Style)GetValue(GroupBandSummaryContentStyleProperty); }
			set { SetValue(GroupBandSummaryContentStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewGroupColumnSummaryItemTemplate"),
#endif
 Category(Categories.Appearance)]
		public Style GroupColumnSummaryContentStyle {
			get { return (Style)GetValue(GroupColumnSummaryContentStyleProperty); }
			set { SetValue(GroupColumnSummaryContentStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewAllowGroupSummaryCascadeUpdate"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool AllowGroupSummaryCascadeUpdate {
			get { return (bool)GetValue(AllowGroupSummaryCascadeUpdateProperty); }
			set { SetValue(AllowGroupSummaryCascadeUpdateProperty, value); }
		}
		[CloneDetailMode(CloneDetailMode.Skip)]
		public ScrollBarVisibility VerticalScrollbarVisibility {
			get { return (ScrollBarVisibility)GetValue(VerticalScrollbarVisibilityProperty); }
			set { SetValue(VerticalScrollbarVisibilityProperty, value); }
		}
		[CloneDetailMode(CloneDetailMode.Skip)]
		public ScrollBarVisibility HorizontalScrollbarVisibility {
			get { return (ScrollBarVisibility)GetValue(HorizontalScrollbarVisibilityProperty); }
			set { SetValue(HorizontalScrollbarVisibilityProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TableViewNewItemRowData")]
#endif
		public RowData NewItemRowData { get; private set; }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewGroupFooterRowTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate GroupFooterRowTemplate {
			get { return (DataTemplate)GetValue(GroupFooterRowTemplateProperty); }
			set { SetValue(GroupFooterRowTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewGroupFooterRowTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector GroupFooterRowTemplateSelector {
			get { return (DataTemplateSelector)GetValue(GroupFooterRowTemplateSelectorProperty); }
			set { SetValue(GroupFooterRowTemplateSelectorProperty, value); }
		}
		public DataTemplateSelector ActualGroupFooterRowTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualGroupFooterRowTemplateSelectorProperty); }
			protected set { this.SetValue(ActualGroupFooterRowTemplateSelectorPropertyKey, value); }
		}
		[ Category(Categories.Appearance)]
		public Style GroupFooterSummaryContentStyle {
			get { return (Style)GetValue(GroupFooterSummaryContentStyleProperty); }
			set { SetValue(GroupFooterSummaryContentStyleProperty, value); }
		}
		[ Category(Categories.Appearance)]
		public DataTemplate GroupFooterSummaryItemTemplate {
			get { return (DataTemplate)GetValue(GroupFooterSummaryItemTemplateProperty); }
			set { SetValue(GroupFooterSummaryItemTemplateProperty, value); }
		}
		[ DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector GroupFooterSummaryItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(GroupFooterSummaryItemTemplateSelectorProperty); }
			set { SetValue(GroupFooterSummaryItemTemplateSelectorProperty, value); }
		}
		public DataTemplateSelector ActualGroupFooterSummaryItemTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualGroupFooterSummaryItemTemplateSelectorProperty); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TableViewGroupFooterRowStyle"),
#endif
 Category(Categories.Appearance)]
		public Style GroupFooterRowStyle {
			get { return (Style)GetValue(GroupFooterRowStyleProperty); }
			set { SetValue(GroupFooterRowStyleProperty, value); }
		}
		[Category(Categories.Data)]
		public event InitNewRowEventHandler InitNewRow {
			add { AddHandler(InitNewRowEvent, value); }
			remove { RemoveHandler(InitNewRowEvent, value); }
		}
		[Category(Categories.OptionsView)]
		public event CustomScrollAnimationEventHandler CustomScrollAnimation {
			add { AddHandler(CustomScrollAnimationEvent, value); }
			remove { RemoveHandler(CustomScrollAnimationEvent, value); }
		}
		[Category("Behavior")]
		public event RowDoubleClickEventHandler RowDoubleClick {
			add { AddHandler(RowDoubleClickEvent, value); }
			remove { RemoveHandler(RowDoubleClickEvent, value); }
		}
		[Category("Behavior")]
		public event ShowingGroupFooterEventHandler ShowingGroupFooter {
			add { AddHandler(ShowingGroupFooterEvent, value); }
			remove { RemoveHandler(ShowingGroupFooterEvent, value); }
		}
		public TableViewCommands TableViewCommands { get { return (TableViewCommands)Commands; } }
		public bool ShowCheckBoxSelectorColumn {
			get { return (bool)GetValue(ShowCheckBoxSelectorColumnProperty); }
			set { SetValue(ShowCheckBoxSelectorColumnProperty, value); }
		}
		public bool ShowCheckBoxSelectorInGroupRow {
			get { return (bool)GetValue(ShowCheckBoxSelectorInGroupRowProperty); }
			set { SetValue(ShowCheckBoxSelectorInGroupRowProperty, value); }
		}
		public double CheckBoxSelectorColumnWidth {
			get { return (double)GetValue(CheckBoxSelectorColumnWidthProperty); }
			set { SetValue(CheckBoxSelectorColumnWidthProperty, value); }
		}
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public DataTemplate CheckBoxSelectorColumnHeaderTemplate {
			get { return (DataTemplate)GetValue(CheckBoxSelectorColumnHeaderTemplateProperty); }
			set { SetValue(CheckBoxSelectorColumnHeaderTemplateProperty, value); }
		}
		public bool RetainSelectionOnClickOutsideCheckBoxSelector {
			get { return (bool)GetValue(RetainSelectionOnClickOutsideCheckBoxSelectorProperty); }
			set { SetValue(RetainSelectionOnClickOutsideCheckBoxSelectorProperty, value); }
		}
		#endregion
		internal override bool AllowFixedGroupsCore { get { return AllowFixedGroups.GetValue(((ITableView)RootView).AllowPerPixelScrolling || DataProviderBase.IsServerMode || DataProviderBase.IsAsyncServerMode); } }
		protected override DataViewBehavior CreateViewBehavior() {
			return new GridTableViewBehavior(this);
		}
		protected internal GridDataProvider GridDataProvider { get { return (GridDataProvider)DataProviderBase; } }
		internal override Style GetNewItemRowCellStyle { get { return NewItemRowCellStyle; } }
		Lazy<BarManagerMenuController> bandMenuControllerValue;
		internal BarManagerMenuController BandMenuController { get { return bandMenuControllerValue.Value; } }
		[Browsable(false)]
		public BarManagerActionCollection BandMenuCustomizations { get { return BandMenuController.ActionContainer.Actions; } }
		public void MovePrevRow(bool allowNavigateToAutoFilterRow) {
			TableViewBehavior.MovePrevRow(allowNavigateToAutoFilterRow);
		}
		protected override bool CanNavigateToNewItemRow { get { return NewItemRowPosition == NewItemRowPosition.Top && !TableViewBehavior.IsNewItemRowFocused; } }
		protected override int FixedNoneColumnsCount { get { return FixedNoneVisibleColumns.Count; } } 
		internal override bool IsNewItemRowHandle(int rowHandle) {
			return rowHandle == GridControl.NewItemRowHandle;
		}
		internal override bool PrintAllGroupsCore { get { return PrintAllGroups; } }
		void OnNewItemRowPositionChanged(NewItemRowPosition oldValue) {
			if(FocusedRowHandle == GridControl.NewItemRowHandle && HasValidationError)
				CurrentCellEditor.CancelEditInVisibleEditor();
			if(DataControl != null)
				DataControl.ValidateMasterDetailConsistency();
			if(DataProviderBase != null)
				DataProviderBase.InvalidateVisibleIndicesCache();
			if(TableViewBehavior.IsNewItemRowVisible && DataControl != null)
				NewItemRowData.UpdateData();
			else if(NewItemRowPosition == NewItemRowPosition.None && TableViewBehavior.IsNewItemRowFocused && Grid != null)
				SetFocusedRowHandle(Grid.GetRowHandleByVisibleIndex(0));
			if(DataPresenter != null && DataProviderBase != null) DataPresenter.ClearInvisibleItems();
			if((oldValue == NewItemRowPosition.Bottom || NewItemRowPosition == NewItemRowPosition.Bottom) && DataControl != null)
				DataControl.RaiseVisibleRowCountChanged();
			if(IsNewItemRowFocused && DataControl != null)
				OnFocusedRowHandleChangedCore(DataControlBase.InvalidRowHandle);
			InvalidateParentTree();
		}
		protected override bool IsFirstNewRow() {
			return NewItemRowPosition == Xpf.Grid.NewItemRowPosition.Top && FocusedRowHandle == DataControlBase.NewItemRowHandle;
		}
		public void AddNewRow() {
			GridDataProvider.AddNewRow();
			GridDataProvider.BeginCurrentRowEdit();
			RowData newItemRowData = GetRowData(GridControl.NewItemRowHandle);
			if(newItemRowData != null) newItemRowData.UpdateData();
			if(NewItemRowPosition == Xpf.Grid.NewItemRowPosition.None) {
				object row = DataControl.CurrentItem;
				GridDataProvider.EndCurrentRowEdit();
				DataControl.SetCurrentItemCore(row);
				GridDataProvider.BeginCurrentRowEdit();
			} else
				ScrollIntoView(DataControlBase.NewItemRowHandle);	  
		}
		void OnNewItemRowChanged() {
			if(IsNewItemRowFocused && !FocusedRowHandleChangedLocker.IsLocked) {
				DataControl.UpdateCurrentItem();
				SelectionStrategy.OnFocusedRowDataChanged();
			}
		}
		internal void OnStartNewItemRow() {
			OnNewItemRowChanged();
			RaiseEventInOriginationView(new InitNewRowEventArgs(TableView.InitNewRowEvent, this, GridControl.NewItemRowHandle));
			GetRowData(GridControl.NewItemRowHandle).Do(data => data.UpdateData());
		}
		internal void OnEndNewItemRow() {
			GetRowData(GridControl.NewItemRowHandle).Do(data => data.UpdateData());
			OnNewItemRowChanged();
		}
		protected internal override void MoveNextNewItemRowCell() {
			if(ViewBehavior.NavigationStrategyBase.IsEndNavigationIndex(this)) {
				if(!CommitEditing())
					return;
				MoveFirstNavigationIndexCore(false);
				if(IsBottomNewItemRowFocused)
					ScrollIntoView(FocusedRowHandle);
			}
		}
		protected override SelectionStrategyBase CreateSelectionStrategy() {
			if(ActualAllowCellMerge)
				return new CellMergeSelectionStrategy(this);
			DataViewBase rootView = RootView;
			TableView tableView = this;
			if(rootView.NavigationStyle == GridViewNavigationStyle.None)
				return new SelectionStrategyNavigationNone(this);
			if(!tableView.IsMultiSelection)
				return new TableViewSelectionStrategyNone(this);
			if(tableView.GetIsCheckBoxSelectorColumnVisible())
				return new SelectionStrategyCheckBoxRow(this);
			if(tableView.IsMultiRowSelection || rootView.NavigationStyle == GridViewNavigationStyle.Row)
				return new SelectionStrategyRow(this);
			return new SelectionStrategyCell(this);
		}
		protected override DataViewCommandsBase CreateCommandsContainer() {
			return new TableViewCommands(this);
		}
		protected internal override FrameworkElement CreateGroupControl(GroupRowData rowData) {
			return TableViewBehavior.CreateElement(() => new GroupRowControl(rowData), () => new GroupGridRow(), DevExpress.Xpf.Grid.UseLightweightTemplates.GroupRow);
		}
		protected override ControlTemplate GetRowFocusedRectangleTemplate() {
			return FocusedRowBorderTemplate;
		}
		protected internal override MultiSelectMode GetSelectionMode() {
			return SelectionModeHelper.ConvertToMultiSelectMode((TableViewSelectMode)GetValue(MultiSelectModeProperty));
		}
		#region HitTest
		public TableViewHitInfo CalcHitInfo(DependencyObject d) {
			return TableViewHitInfo.CalcHitInfo(d, this);
		}
		public TableViewHitInfo CalcHitInfo(Point hitTestPoint) {
			return TableViewHitInfo.CalcHitInfo(hitTestPoint, this);
		}
		internal override IDataViewHitInfo CalcHitInfoCore(DependencyObject source) {
			return CalcHitInfo(source);
		}
		#endregion
		public FrameworkElement GetGroupFooterRowElementByRowHandle(int rowHandle) {
			RowData rowData;
			if(VisualDataTreeBuilder.GroupSummaryRows.TryGetValue(rowHandle, out rowData)) 
				return rowData.WholeRowElement;
			return null;
		}
		protected internal FrameworkElement GetGroupFooterSummaryElementByRowHandleAndColumn(int rowHandle, ColumnBase column) {
			FrameworkElement rowElement = GetGroupFooterRowElementByRowHandle(rowHandle);
			if(rowElement == null)
				return null;
			return LayoutHelper.FindElement(rowElement, new Predicate<FrameworkElement>(delegate(FrameworkElement element) { return element is GroupFooterSummaryControl && ((GridGroupSummaryColumnData)((GroupFooterSummaryControl)element).DataContext).Column == column && element.Visibility == Visibility.Visible; }));
		}
		protected internal override void UpdateUseLightweightTemplates() {
			if(RootView as ITableView == null)
				return;
			UseLightweightTemplates = ((ITableView)RootView).UseLightweightTemplates;
			TableViewBehavior.UpdateActualDataRowTemplateSelector();
		}
		#region cell selection
		public void SelectCell(int rowHandle, GridColumn column) {
			TableViewBehavior.SelectCell(rowHandle, column);
		}
		public void UnselectCell(int rowHandle, GridColumn column) {
			TableViewBehavior.UnselectCell(rowHandle, column);
		}
		public void SelectCells(int startRowHandle, GridColumn startColumn, int endRowHandle, GridColumn endColumn) {
			TableViewBehavior.SelectCells(startRowHandle, startColumn, endRowHandle, endColumn);
		}
		public void UnselectCells(int startRowHandle, GridColumn startColumn, int endRowHandle, GridColumn endColumn) {
			TableViewBehavior.UnselectCells(startRowHandle, startColumn, endRowHandle, endColumn);
		}
		public bool IsCellSelected(int rowHandle, ColumnBase column) {
			return TableViewBehavior.IsCellSelected(rowHandle, column);
		}
		public IList<GridCell> GetSelectedCells() {
			return new SimpleBridgeList<GridCell, CellBase>(TableViewBehavior.GetSelectedCells());
		}
		protected internal virtual void ShowNewItemRow(NewItemRowPosition? position) {
			NewItemRowPosition = position.HasValue ? position.Value : NewItemRowPosition.Top;
			if(NewItemRowPosition != NewItemRowPosition.None && !IsNewItemRowFocused) {
				SetFocusedRowHandle(DataControlBase.NewItemRowHandle);
				if(VisibleColumns.Count > 0 && DataControl != null)
					DataControl.CurrentColumn = VisibleColumns[0];
			}
		}
		#endregion
		#region CopyRows
		public void CopySelectedCellsToClipboard() {
			ClipboardController.CopyCellsToClipboard(GetSelectedCells());
		}
		public void CopyCellsToClipboard(IEnumerable<GridCell> gridCells) {
			ClipboardController.CopyCellsToClipboard(gridCells);
		}
		public void CopyCellsToClipboard(int startRowHandle, GridColumn startColumn, int endRowHandle, GridColumn endColumn) {
			TableViewBehavior.CopyCellsToClipboard(startRowHandle, startColumn, endRowHandle, endColumn);
		}
		protected void CopyCellsToClipboardCore(IEnumerable<CellBase> gridCells) {
			CopyCellsToClipboard(new SimpleEnumerableBridge<GridCell, CellBase>(gridCells));
		}
		#endregion
		#region IPrintableControl Members
		protected internal override DataTemplate GetPrintRowTemplate() {
			return PrintRowTemplate;
		}
		protected override IRootDataNode CreateRootNode(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize) {
			return GridPrintingHelper.CreatePrintingTreeNode(this, usablePageSize);
		}
		protected override void CreateRootNodeAsync(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize) {
			GridPrintingHelper.CreatePrintingTreeNodeAsync(this, usablePageSize);
		}
		protected override void PagePrintedCallback(IEnumerator pageBrickEnumerator, Dictionary<IVisualBrick, IOnPageUpdater> brickUpdaters) {
			GridPrintingHelper.UpdatePageBricks(pageBrickEnumerator, brickUpdaters, false, (PrintTotalSummary && ShowTotalSummary) || (PrintFixedTotalSummary && ShowFixedTotalSummary));
		}
		protected internal override PrintingDataTreeBuilderBase CreatePrintingDataTreeBuilder(double totalHeaderWidth, ItemsGenerationStrategyBase itemsGenerationStrategy, MasterDetailPrintInfo masterDetailPrintInfo, BandsLayoutBase bandsLayout) {
			return new GridPrintingDataTreeBuilder(this, totalHeaderWidth, itemsGenerationStrategy, bandsLayout, masterDetailPrintInfo);
		}
		#endregion
		#region best fit
		public void BestFitColumn(GridColumn column) {
			TableViewBehavior.BestFitColumn(column);
		}
		public void BestFitColumns() {
			TableViewBehavior.BestFitColumns();
		}
		public double CalcColumnBestFitWidth(GridColumn column) {
			return TableViewBehavior.CalcColumnBestFitWidthCore(column);
		}
		protected internal virtual void RaiseCustomBestFit(CustomBestFitEventArgs e) {
			RaiseEventInOriginationView(e);
		}
		protected internal override void RaiseCustomScrollAnimation(CustomScrollAnimationEventArgs e) {
			e.RoutedEvent = TableView.CustomScrollAnimationEvent;
			base.RaiseCustomScrollAnimation(e);
		}
		internal override bool GetAllowGroupSummaryCascadeUpdate { get { return AllowGroupSummaryCascadeUpdate; } }
		internal bool CanBestFitColumn(object commandParameter) {
			GridColumn column = (GridColumn)GetColumnByCommandParameter(commandParameter);
			return column != null && TableViewBehavior.CanBestFitColumn(column);
		}
		#endregion
		#region ITableView
		TableViewBehavior ITableView.TableViewBehavior { get { return TableViewBehavior; } }
		double ITableView.FixedNoneContentWidth { get { return FixedNoneContentWidth; } set { FixedNoneContentWidth = value; } }
		double ITableView.TotalSummaryFixedNoneContentWidth { get { return TotalSummaryFixedNoneContentWidth; } set { TotalSummaryFixedNoneContentWidth = value; } }
		double ITableView.VerticalScrollBarWidth { get { return VerticalScrollBarWidth; } set { VerticalScrollBarWidth = value; } }
		double ITableView.FixedLeftContentWidth { get { return FixedLeftContentWidth; } set { FixedLeftContentWidth = value; } }
		double ITableView.FixedRightContentWidth { get { return FixedRightContentWidth; } set { FixedRightContentWidth = value; } }
		double ITableView.TotalGroupAreaIndent { get { return TotalGroupAreaIndent; } set { TotalGroupAreaIndent = value; } }
		double ITableView.IndicatorHeaderWidth { get { return IndicatorHeaderWidth; } set { IndicatorHeaderWidth = value; } }
		Thickness ITableView.ScrollingVirtualizationMargin { get { return ScrollingVirtualizationMargin; } set { ScrollingVirtualizationMargin = value; } }
		Thickness ITableView.ScrollingHeaderVirtualizationMargin { get { return ScrollingHeaderVirtualizationMargin; } set { ScrollingHeaderVirtualizationMargin = value; } }
		DependencyPropertyKey ITableView.ActualDataRowTemplateSelectorPropertyKey { get { return ActualDataRowTemplateSelectorPropertyKey; } }
		bool ITableView.IsCheckBoxSelectorColumnVisible { get { return IsCheckBoxSelectorColumnVisibleCore; } }
		bool ITableView.IsEditing { get { return IsEditing; } }
		DataViewBase ITableView.ViewBase { get { return this; } }
		bool ITableView.ActualAllowTreeIndentScrolling { get { return false; } }
		IList<ColumnBase> ITableView.ViewportVisibleColumns { get; set; }
		void ITableView.SetHorizontalViewport(double value) {
			HorizontalViewport = value;
		}
		void ITableView.SetFixedLeftVisibleColumns(IList<ColumnBase> columns) {
			FixedLeftVisibleColumns = ConvertToGridColumnsList(columns);
		}
		void ITableView.SetFixedNoneVisibleColumns(IList<ColumnBase> columns) {
			FixedNoneVisibleColumns = ConvertToGridColumnsList(columns);
		}
		void ITableView.SetFixedRightVisibleColumns(IList<ColumnBase> columns) {
			FixedRightVisibleColumns = ConvertToGridColumnsList(columns);
		}
		void ITableView.CopyCellsToClipboard(IEnumerable<CellBase> gridCells) {
			CopyCellsToClipboardCore(gridCells);
		}
		CellBase ITableView.CreateGridCell(int rowHandle, ColumnBase column) {
			return new GridCell(rowHandle, (GridColumn)column);
		}
		ITableViewHitInfo ITableView.CalcHitInfo(DependencyObject d) {
			return TableViewHitInfo.CalcHitInfo(d, this);
		}
		void ITableView.SetActualShowIndicator(bool showIndicator) {
			ActualShowIndicator = showIndicator;
		}
		void ITableView.SetActualIndicatorWidth(double indicatorWidth) {
			ActualIndicatorWidth = indicatorWidth;
		}
		void ITableView.SetActualExpandDetailHeaderWidth(double expandDetailHeaderWidth) {
			ActualExpandDetailHeaderWidth = expandDetailHeaderWidth;
		}
		void ITableView.SetActualDetailMargin(Thickness detailMargin) {
			ActualDetailMargin = detailMargin;
		}
		void ITableView.SetActualFadeSelectionOnLostFocus(bool fadeSelectionOnLostFocus) {
			ActualFadeSelectionOnLostFocus = fadeSelectionOnLostFocus;
		}
		void ITableView.SetShowTotalSummaryIndicatorIndent(bool showTotalSummaryIndicatorIndent) {
			this.ShowTotalSummaryIndicatorIndent = showTotalSummaryIndicatorIndent;
		}
		void ITableView.RaiseRowDoubleClickEvent(ITableViewHitInfo hitInfo
#if !SL
			, MouseButton changedButton
#endif
			) {
			RaiseEventInOriginationView(new RowDoubleClickEventArgs((GridViewHitInfoBase)hitInfo
#if !SL
				, changedButton
#endif
				, this
				) { RoutedEvent = RowDoubleClickEvent });
		}
		void ITableView.SetExpandColumnPosition(ColumnPosition position) {
			ExpandColumnPosition = position;
		}
		#endregion
		void OnAllowFixedGroupsChanged() {
			if(Grid != null)
				OnDataReset();
		}
		void OnGroupSummaryDisplayModeChanged() {
			if(Grid != null)
				OnDataReset();
		}
		void OnAllowPartialGroupingChanged() {
			if(DataControl == null) return;
			DataControl.UpdateAllowPartialGrouping();
			RebuildVisibleColumns();
		}
		internal override bool AllowPartialGroupingCore {
			get {
				return AllowPartialGrouping;
			}
		}
		internal override void PerformUpdateGroupSummaryDataAction(Action action) {
			if(GroupSummaryDisplayMode == GroupSummaryDisplayMode.AlignByColumns ||  ShowGroupFooters) {
				action();
			}
		}
		protected internal override bool ShowGroupSummaryFooter { get { return ShowGroupFooters && GridDataProvider.GroupedColumnCount > 0; } }
		#region MasterDetail
		#region IDetailElement Members
		DataViewBase IDetailElement<DataViewBase>.CreateNewInstance(params object[] args) {
			Type realType = this.GetType();
			return Activator.CreateInstance(realType, (MasterNodeContainer)args[0], (MasterRowsContainer)args[1], (DataControlDetailDescriptor)args[2]) as DataViewBase;
		}
		#endregion
		internal override bool AllowMasterDetailCore { get { return AllowMasterDetail; } }
		void OnAllowMasterDetailChanged() {
			OnShowDetailButtonsChanged();
			if(DataControl != null)
				OnDataReset();
		}
		void OnShowDetailButtonsChanged() {
			ActualShowDetailButtons = AllowMasterDetail && (HasDetailViews && (ShowDetailButtons != DefaultBoolean.False) || (ShowDetailButtons == DefaultBoolean.True));
			if(Grid != null)
				Grid.UpdateAllDetailViewIndents();
			RebuildVisibleColumns();
		}
		void OnActualShowDetailButtonsChanged() {
			TableViewBehavior.UpdateViewRowData(x => x.UpdateClientDetailExpandButtonVisibility());
		}
		internal void UpdateHasDetailViews() {
			HasDetailViews = Grid.DetailDescriptor != null;
			OnShowDetailButtonsChanged();
		}
		internal override void BindDetailContainerNextLevelItemsControl(ItemsControl itemsControl) {
			itemsControl.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("View.DataControl.DetailDescriptor.DataControlDetailDescriptors") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent) });
			itemsControl.SetBinding(ItemsControl.VisibilityProperty, new Binding("View.AllowMasterDetail") { RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent), Converter = new BoolToVisibilityConverter() });
		}
#if DEBUGTEST
		internal bool ForceAllowMultiSelectionInMasterDetail = false;
#endif
		internal override void ThrowNotSupportedInMasterDetailException() {
			if(
#if DEBUGTEST
				!ForceAllowMultiSelectionInMasterDetail && 
#endif
				IsMultiCellSelection || GetIsCheckBoxSelectorColumnVisible())
				throw new NotSupportedInMasterDetailException(NotSupportedInMasterDetailException.MultiSelectionNotSupported);
		}
		internal override void ThrowNotSupportedInDetailException() {
			if(ShowAutoFilterRow)
				throw new NotSupportedInMasterDetailException(NotSupportedInMasterDetailException.AutoFilterRowNotSupported);
		}
		#endregion
		protected internal override bool ShouldDisplayBottomRow { get { return NewItemRowPosition == NewItemRowPosition.Bottom; } }
		protected void UpdateFixedNoneContentWidth(RowData rowData) {
			TableViewBehavior.UpdateFixedNoneContentWidth(rowData);
		}
		protected internal override int CalcGroupSummaryVisibleRowCount() {
			if(ShowGroupSummaryFooter)
				return GridDataProvider.VisibleIndicesProvider.VisibleGroupSummaryRowCount;  
			return base.CalcGroupSummaryVisibleRowCount();
		}
		protected internal virtual bool RaiseShowingGroupFooter(int rowHandle, int level){
			ShowingGroupFooterEventArgs e = new ShowingGroupFooterEventArgs(ShowingGroupFooterEvent, this, rowHandle, level);
			RaiseEventInOriginationView(e);
			return e.Allow;
		}
		void UpdateActualGroupFooterRowTemplateSelector() {
			ActualGroupFooterRowTemplateSelector = new ActualTemplateSelectorWrapper(GroupFooterRowTemplateSelector, GroupFooterRowTemplate);
		}
		void UpdateActualGroupFooterSummaryItemTemplateSelector() {
			DataControlOriginationElementHelper.UpdateActualTemplateSelector(this, OriginationView, ActualGroupFooterSummaryItemTemplateSelectorPropertyKey, GroupFooterSummaryItemTemplateSelector, GroupFooterSummaryItemTemplate);
		}
		[Browsable(false)]
		public bool ShouldSerializeColumnChooserBandsSortOrderComparer(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
		protected override void OnDataControlChanged(DataControlBase oldValue) {
			base.OnDataControlChanged(oldValue);
			oldValue.Do(dataControl => RemoveCheckBoxSelectorColumn(dataControl));
			DataControl.Do(dataControl => AddCheckBoxSelectorColumn(dataControl));
			UpdateIsCheckBoxSelectorColumnVisible();
		}
		void AddCheckBoxSelectorColumn(DataControlBase dataControl) {
			if(!GetIsCheckBoxSelectorColumnVisible() || CheckBoxSelectorColumn.OwnerControl == dataControl)
				return;
			CheckBoxSelectorColumn.OwnerControl = dataControl;
			dataControl.AddChild(CheckBoxSelectorColumn);
		}
		void RemoveCheckBoxSelectorColumn(DataControlBase dataControl) {
			dataControl.RemoveChild(CheckBoxSelectorColumn);
			CheckBoxSelectorColumn.OwnerControl = null;
		}
		internal override ICommand GetColumnCommand(ColumnBase column) {
			if(CheckBoxSelectorColumn == column)
				return TableViewCommands.ToggleRowsSelection;
			return base.GetColumnCommand(column);
		}
		internal override void OnMultiSelectModeChanged() {
			UpdateIsCheckBoxSelectorColumnVisible();
			UpdateActualAllowCellMergeCore();
			base.OnMultiSelectModeChanged();
		}
		internal override bool IsExpandButton(IDataViewHitInfo hitInfo) {
			TableViewHitInfo tableViewHitInfo = (TableViewHitInfo)hitInfo;
			return tableViewHitInfo.HitTest == TableViewHitTest.GroupRowButton || tableViewHitInfo.HitTest == TableViewHitTest.MasterRowButton;
		}
		internal override void UpdateColumns(Action<ColumnBase> updateColumnDelegate) {
			if(IsCheckBoxSelectorColumnVisibleCore)
				updateColumnDelegate(CheckBoxSelectorColumn);
			base.UpdateColumns(updateColumnDelegate);
		}
		internal override Type GetColumnType(ColumnBase column, DataProviderBase dataProvider = null) {
			if(column == CheckBoxSelectorColumn)
				return typeof(bool);
			return base.GetColumnType(column, dataProvider);
		}
		protected internal override bool CanSortColumnCore(ColumnBase column, string fieldName, bool prohibitColumnProperty) {
			if(column == CheckBoxSelectorColumn)
				return false;
			return base.CanSortColumnCore(column, fieldName, prohibitColumnProperty);
		}
		#region CheckBoxSelectorColumn
		public const string CheckBoxSelectorColumnName = "DX$CheckboxSelectorColumn";
		internal static bool IsCheckBoxSelectorColumn(string fieldName) {
			return fieldName == CheckBoxSelectorColumnName;
		}
#if DEBUGTEST
		public 
#else
		internal
#endif
		GridColumn CheckBoxSelectorColumn { get; private set; }
		bool isCheckBoxSelectorColumnVisibleCore;
		bool IsCheckBoxSelectorColumnVisibleCore {
			get { return isCheckBoxSelectorColumnVisibleCore; }
			set {
				if(isCheckBoxSelectorColumnVisibleCore != value) {
					isCheckBoxSelectorColumnVisibleCore = value;
					OnIsCheckBoxSelectorColumnVisibleChanged();
				}
			}
		}
		GridColumn CreateCheckBoxSelectorColumn() {
			GridColumn column = new GridColumn();
			column.FieldName = CheckBoxSelectorColumnName;
			column.Header = GetLocalizedString(GridControlStringId.CheckboxSelectorColumnCaption);
			column.Width = CheckBoxSelectorColumnWidth;
			column.FixedWidth = true;
			column.ShowInColumnChooser = false;
			column.AllowResizing = DefaultBoolean.False;
			column.AllowSorting = DefaultBoolean.False;
			column.AllowGrouping = DefaultBoolean.False;
			column.AllowMoving = DefaultBoolean.False;
			column.AllowColumnFiltering = DefaultBoolean.False;
			column.AllowEditing = DefaultBoolean.False;
			column.AllowConditionalFormattingMenu = false;
			column.FieldType = typeof(bool);
			column.VisibleIndex = 0;
			column.HeaderTemplate = CheckBoxSelectorColumnHeaderTemplate;
			column.HorizontalHeaderContentAlignment = System.Windows.HorizontalAlignment.Center;
			column.AllowPrinting = false;
			return column;
		}
		internal override void PatchVisibleColumns(IList<ColumnBase> visibleColumns, bool hasFixedLeftColumns) {
			if(!IsCheckBoxSelectorColumnVisibleCore)
				return;
			if(hasFixedLeftColumns)
				CheckBoxSelectorColumn.Fixed = FixedStyle.Left;
			else
				CheckBoxSelectorColumn.Fixed = FixedStyle.None;
			visibleColumns.Insert(0, CheckBoxSelectorColumn);
		}
		protected override int? CompareGroupedColumns(BaseColumn x, BaseColumn y) {
			if(!AllowPartialGroupingCore)
				return null;
			if(x == CheckBoxSelectorColumn || y == CheckBoxSelectorColumn)
				return null;
			GridColumn column1 = x as GridColumn;
			GridColumn column2 = y as GridColumn;
			if(column1 == null || column2 == null || !(column1.IsGrouped || column2.IsGrouped))
				return null;
			if(!column1.IsGrouped)
				return 1;
			if(!column2.IsGrouped)
				return -1;
			return Comparer<int>.Default.Compare(column1.GroupIndex, column2.GroupIndex);
		}
		internal override int AdjustVisibleIndex(ColumnBase column, int visibleIndex) {
			int newVisibleIndex = base.AdjustVisibleIndex(column, visibleIndex);
			if(IsCheckBoxSelectorColumnVisibleCore && newVisibleIndex == 0)
				newVisibleIndex = 1;
			return newVisibleIndex;
		}
		void UpdateIsCheckBoxSelectorColumnVisible() {
			IsCheckBoxSelectorColumnVisibleCore = GetIsCheckBoxSelectorColumnVisible();
		}
		bool GetIsCheckBoxSelectorColumnVisible() {
			return ShowCheckBoxSelectorColumn && IsMultiRowSelection;
		}
		void OnIsCheckBoxSelectorColumnVisibleChanged() {
			UpdateCheckBoxSelectorColumnOwnerControl();
			ResetSelectionStrategy();
			RebuildVisibleColumns();
			UpdateActualShowCheckBoxSelectorInGroupRow();
			if(IsCheckBoxSelectorColumnVisibleCore) {
				CheckBoxSelectorColumn.UpdateViewInfo();
				DataControl.Do(x => x.CurrentColumn = CheckBoxSelectorColumn);
			}
		}
		void UpdateCheckBoxSelectorColumnOwnerControl() {
			if(DataControl == null)
				return;
			if(IsCheckBoxSelectorColumnVisibleCore)
				AddCheckBoxSelectorColumn(DataControl);
			else
				RemoveCheckBoxSelectorColumn(DataControl);
		}
		internal void ToggleRowsSelection() {
			if(RequestUIUpdate())
				SelectionStrategy.ToggleRowsSelection();
		}
		void OnShowCheckBoxSelectorInGroupRowChanged() {
			UpdateActualShowCheckBoxSelectorInGroupRow();
			if(ShowCheckBoxSelectorInGroupRow && ActualShowCheckBoxSelectorInGroupRow) {
				UpdateRowData(rowData => {
					if(rowData is GroupRowData) {
						((GroupRowData)rowData).UpdateAllItemsSelected();
					}
				});
			}
		}
		void UpdateActualShowCheckBoxSelectorInGroupRow() {
			ActualShowCheckBoxSelectorInGroupRow = ShowCheckBoxSelectorInGroupRow && IsCheckBoxSelectorColumnVisibleCore;
		}
		void OnCheckBoxSelectorColumnWidthChanged() {
			CheckBoxSelectorColumn.Width = CheckBoxSelectorColumnWidth;
		}
		void OnCheckBoxSelectorColumnHeaderTemplateChanged() {
			CheckBoxSelectorColumn.HeaderTemplate = CheckBoxSelectorColumnHeaderTemplate;
		}
		#endregion
		protected override bool IsGroupRowOptimized { get { return TableViewBehavior.UseLightweightTemplatesHasFlag(Xpf.Grid.UseLightweightTemplates.GroupRow); } }
	}
}
