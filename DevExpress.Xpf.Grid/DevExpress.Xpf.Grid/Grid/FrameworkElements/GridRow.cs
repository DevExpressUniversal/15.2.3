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
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Core;
using System.Windows.Data;
using System.ComponentModel;
using DevExpress.Xpf.Grid.HitTest;
using System.Collections;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Grid.Native;
#if !SILVERLIGHT
using DevExpress.Mvvm.Native;
using DevExpress.Mvvm.UI.Interactivity;
#else
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public class GridRow : 
#if !SL
		Control
#else
		SLControl
#endif
		, IFocusedRowBorderObject, IGridDataRow {
#if DEBUGTEST
		internal static double? DebugTestFixedHeight = null;
#endif
		public static readonly DependencyProperty RowPositionProperty = DependencyPropertyManager.Register("RowPosition", typeof(RowPosition), typeof(GridRow), new FrameworkPropertyMetadata(RowPosition.Bottom, (d, e) => ((GridRow)d).OnRowPositionChanged()));
		protected virtual void OnRowPositionChanged() { }
		public RowPosition RowPosition {
			get { return (RowPosition)GetValue(RowPositionProperty); }
			set { SetValue(RowPositionProperty, value); }
		}
		public GridRow() {
#if DEBUGTEST
			if(DebugTestFixedHeight.HasValue)
				Height = DebugTestFixedHeight.Value;
#endif
			this.SetDefaultStyleKey(typeof(GridRow));
			RowData.SetRowHandleBinding(this);
#if SL
			this.SetValue(ThemeManager.ApplyApplicationThemeProperty, true);
#endif
		}
#if DEBUGTEST
		internal static bool RoundRowSize = true;
#endif
		protected override Size MeasureOverride(Size constraint) {
			Size size = base.MeasureOverride(constraint);
#if DEBUGTEST
			if(RoundRowSize)
#endif
				size = new Size(Math.Ceiling(size.Width), Math.Ceiling(size.Height));
			return size;
		}
		public FrameworkElement RowDataContent { get; set; }
		double IFocusedRowBorderObject.LeftIndent { get { return 0d; } }
		internal GridRowContent DataRowContainer { get; private set; }
		ContentPresenter rowOffsetPresenter;
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			DataRowContainer = GetTemplateChild("PART_DataRow") as GridRowContent;			
			RowDataContent = GetTemplateChild("PART_DataRowContent") as FrameworkElement;
			RowData = DataContext as RowData;
			RowData.PropertyChanged += new PropertyChangedEventHandler(rowData_PropertyChanged);
			rowOffsetPresenter = GetTemplateChild("PART_RowOffsetPresenter") as ContentPresenter;
			UpdateRowOffsetPresenter();
			SetBinding(RowPositionProperty, new Binding("RowPosition"));
		}
		RowData RowData { get; set; }
		void rowData_PropertyChanged(object sender, PropertyChangedEventArgs e) {
			if(e.PropertyName == "Level") {				
				UpdateRowOffsetPresenter();
			}
		}
		void UpdateRowOffsetPresenter() {
			if(rowOffsetPresenter == null || RowData == null || rowOffsetPresenter.Content != null) return;
			FrameworkElement rowOffsetContent = GridRowHelper.CreateRowOffsetContent(RowData, DataRowContainer);
			if(rowOffsetContent != null) {
				rowOffsetPresenter.Content = rowOffsetContent;
			}
		}
		RowData IGridDataRow.RowData {
			get { return DataContext as RowData; }
		}
		void IGridDataRow.UpdateContentLayout() {
			if(RowData != null && RowData.View != null)
				RowData.View.UpdateContentLayout();
		}
	}
	internal static class GridRowHelper {
		public static Control CreateRowOffsetContent(RowData rowData, Control backgroundSource) {
			Control result = null;
			if(rowData.View.IsRowMarginControlVisible) {
				RowMarginControl rowMarginControl = new RowMarginControl();
				rowMarginControl.SetBinding(RowMarginControl.NextRowLevelProperty, new Binding("NextRowLevel"));
				result = rowMarginControl;
			} else if(rowData.Level != 0) {
				result = new RowOffsetPresenter();
			}
			if(result != null)
				result.SetBinding(Control.BorderBrushProperty, new Binding("BorderBrush") { Source = backgroundSource });
			return result;
		}
	}
	public class GridRowContent : ContentControl {
		public static readonly DependencyProperty CurrentHeightProperty =
			DependencyPropertyManager.Register("CurrentHeight", typeof(double), typeof(GridRowContent), new FrameworkPropertyMetadata(0.0));
#if !SL
	[DevExpressXpfGridLocalizedDescription("GridRowContentCurrentHeight")]
#endif
		public double CurrentHeight {
			get { return (double)GetValue(CurrentHeightProperty); }
			set { SetValue(CurrentHeightProperty, value); }
		}
		public GridRowContent() {
			this.SetDefaultStyleKey(typeof(GridRowContent));
			SizeChanged += new SizeChangedEventHandler(GridRowContent_SizeChanged);
		}
		void GridRowContent_SizeChanged(object sender, SizeChangedEventArgs e) {
			CurrentHeight = e.NewSize.Height;
		}		
	}
	public class TotalSummaryLeftIndentControl : Control {
		public TotalSummaryLeftIndentControl() {
			this.SetDefaultStyleKey(typeof(TotalSummaryLeftIndentControl));
		}
	}
	public class GridTotalSummaryScrollablePart : GridScrollablePart {
		public GridTotalSummaryScrollablePart() {
			this.SetDefaultStyleKey(typeof(GridTotalSummaryScrollablePart));
		}
	}
	public class GridScrollablePart : Control {
		public static readonly DependencyProperty FixedLeftContentProperty;
		public static readonly DependencyProperty FixedRightContentProperty;
		public static readonly DependencyProperty FixedNoneContentProperty;
		public static readonly DependencyProperty FitContentProperty;
		public static readonly DependencyProperty FitLeftContentProperty;
		public static readonly DependencyProperty ScrollingMarginProperty;
		public static readonly DependencyProperty FixedNoneContentWidthProperty;
		public static readonly DependencyProperty FixedColumnsDelimiterTemplateProperty;
		public static readonly DependencyProperty FixedLeftVisibleColumnsProperty;
		public static readonly DependencyProperty FixedRightVisibleColumnsProperty;
		public static readonly DependencyProperty FixedLineWidthProperty;
		static GridScrollablePart() {
			FixedLeftContentProperty = DependencyPropertyManager.Register("FixedLeftContent", typeof(object), typeof(GridScrollablePart), new UIPropertyMetadata(null, (d, e) => ((GridScrollablePart)d).UpdateFixedParts()));
			FixedRightContentProperty = DependencyPropertyManager.Register("FixedRightContent", typeof(object), typeof(GridScrollablePart), new UIPropertyMetadata(null, (d, e) => ((GridScrollablePart)d).UpdateFixedParts()));
			FixedNoneContentProperty = DependencyPropertyManager.Register("FixedNoneContent", typeof(object), typeof(GridScrollablePart), new UIPropertyMetadata(null, (d, e) => ((GridScrollablePart)d).OnFixedNoneContentChanged()));
			FitContentProperty = DependencyPropertyManager.Register("FitContent", typeof(object), typeof(GridScrollablePart), new UIPropertyMetadata(null, (d, e) => ((GridScrollablePart)d).OnFitContentChanged()));
			FitLeftContentProperty = DependencyPropertyManager.Register("FitLeftContent", typeof(object), typeof(GridScrollablePart), new UIPropertyMetadata(null, (d, e) => ((GridScrollablePart)d).OnFitLeftContentChanged()));
			ScrollingMarginProperty = DependencyPropertyManager.Register("ScrollingMargin", typeof(Thickness), typeof(GridScrollablePart), new FrameworkPropertyMetadata(new Thickness(0), (d, e) => ((GridScrollablePart)d).OnScrollingMarginChanged()));
			FixedNoneContentWidthProperty = DependencyPropertyManager.Register("FixedNoneContentWidth", typeof(double), typeof(GridScrollablePart), new FrameworkPropertyMetadata(0d, (d, e) => ((GridScrollablePart)d).OnFixedNoneContentWidthChanged()));
			FixedColumnsDelimiterTemplateProperty = DependencyPropertyManager.Register("FixedColumnsDelimiterTemplate", typeof(DataTemplate), typeof(GridScrollablePart), new FrameworkPropertyMetadata(null, (d, e) => ((GridScrollablePart)d).OnFixedColumnsDelimiterTemplateChanged()));
			FixedLeftVisibleColumnsProperty = DependencyPropertyManager.Register("FixedLeftVisibleColumns", typeof(object), typeof(GridScrollablePart), new FrameworkPropertyMetadata(null, (d, e) => ((GridScrollablePart)d).UpdateFixedParts()));
			FixedRightVisibleColumnsProperty = DependencyPropertyManager.Register("FixedRightVisibleColumns", typeof(object), typeof(GridScrollablePart), new FrameworkPropertyMetadata(null, (d, e) => ((GridScrollablePart)d).UpdateFixedParts()));
			FixedLineWidthProperty = DependencyPropertyManager.Register("FixedLineWidth", typeof(double), typeof(GridScrollablePart), new FrameworkPropertyMetadata(0d, (d, e) => ((GridScrollablePart)d).OnFixedLineWidthChanged()));
		}
		ContentPresenter fitContent, fitLeftContent, fixedRightContent, fixedLeftLineContent, fixedRightLineContent;
		protected ContentPresenter fixedNoneContentInternal, fixedLeftContentInternal;
		protected FrameworkElement fixedNoneContentCellsBorder, scrollablePartPanel;
		public GridScrollablePart() {
			this.SetDefaultStyleKey(typeof(GridScrollablePart));
		}
		public override void OnApplyTemplate() {
			if(fixedLeftContentInternal != null) fixedLeftContentInternal.Content = null;
			if(fitContent != null) fitContent.Content = null;
			if(fitLeftContent != null) fitLeftContent.Content = null;
			if(fixedRightContent != null) fixedRightContent.Content = null;
			if(fixedNoneContentInternal != null) fixedNoneContentInternal.Content = null;
			fixedLeftContentInternal = GetTemplateChild("PART_FixedLeftContent") as ContentPresenter;
			fitContent = GetTemplateChild("PART_FitContent") as ContentPresenter;
			fitLeftContent = GetTemplateChild("PART_FitLeftContent") as ContentPresenter;
			fixedRightContent = GetTemplateChild("PART_FixedRightContent") as ContentPresenter;
			fixedNoneContentInternal = GetTemplateChild("PART_FixedNoneContent") as ContentPresenter;
			OnFixedNoneContentChanged();
			fixedNoneContentCellsBorder = GetTemplateChild("PART_FixedNoneCellsBorder") as FrameworkElement;
			OnFixedNoneContentWidthChanged();
			OnFitContentChanged();
			OnFitLeftContentChanged();
			fixedLeftLineContent = GetTemplateChild("PART_FixedLeftLinePlaceHolder") as ContentPresenter;
			fixedRightLineContent = GetTemplateChild("PART_FixedRightLinePlaceHolder") as ContentPresenter;
			UpdateFixedParts();
			OnFixedLineWidthChanged();
			OnScrollingMarginChanged();
			OnFixedColumnsDelimiterTemplateChanged();
			scrollablePartPanel = GetTemplateChild("PART_ScrollablePartPanel") as FrameworkElement;
			base.OnApplyTemplate();
		}
		public object FixedLeftContent {
			get { return GetValue(FixedLeftContentProperty); }
			set { SetValue(FixedLeftContentProperty, value); }
		}
		public object FixedRightContent {
			get { return GetValue(FixedRightContentProperty); }
			set { SetValue(FixedRightContentProperty, value); }
		}
		public object FixedNoneContent {
			get { return GetValue(FixedNoneContentProperty); }
			set { SetValue(FixedNoneContentProperty, value); }
		}
		public object FitContent {
			get { return GetValue(FitContentProperty); }
			set { SetValue(FitContentProperty, value); }
		}
		public object FitLeftContent {
			get { return GetValue(FitLeftContentProperty); }
			set { SetValue(FitLeftContentProperty, value); }
		}
		public Thickness ScrollingMargin {
			get { return (Thickness)GetValue(ScrollingMarginProperty); }
			set { SetValue(ScrollingMarginProperty, value); }
		}
		public double FixedNoneContentWidth {
			get { return (double)GetValue(FixedNoneContentWidthProperty); }
			set { SetValue(FixedNoneContentWidthProperty, value); }
		}
		public DataTemplate FixedColumnsDelimiterTemplate {
			get { return (DataTemplate)GetValue(FixedColumnsDelimiterTemplateProperty); }
			set { SetValue(FixedColumnsDelimiterTemplateProperty, value); }
		}
		public object FixedLeftVisibleColumns {
			get { return GetValue(FixedLeftVisibleColumnsProperty); }
			set { SetValue(FixedLeftVisibleColumnsProperty, value); }
		}
		public object FixedRightVisibleColumns {
			get { return GetValue(FixedRightVisibleColumnsProperty); }
			set { SetValue(FixedRightVisibleColumnsProperty, value); }
		}
		public double FixedLineWidth {
			get { return (double)GetValue(FixedLineWidthProperty); }
			set { SetValue(FixedLineWidthProperty, value); }
		}
		protected ContentPresenter FixedNoneContentCore { get { return fixedNoneContentInternal; } }
		void OnFixedNoneContentChanged() {
			if(fixedNoneContentInternal != null) fixedNoneContentInternal.Content = FixedNoneContent;
		}
		void OnFitContentChanged() {
			if(fitContent != null) fitContent.Content = FitContent;
		}
		void OnFitLeftContentChanged() {
			if(fitLeftContent != null) fitLeftContent.Content = FitLeftContent;
		}
		protected virtual void UpdateFixedParts() {
			if(HasFixedLeftColumns) {
				if(fixedLeftContentInternal != null && fixedLeftContentInternal.Content == null)
					fixedLeftContentInternal.Content = FixedLeftContent;
				if(fixedLeftLineContent != null) fixedLeftLineContent.Visibility = Visibility.Visible;
			} else {
				if(fixedLeftLineContent != null) fixedLeftLineContent.Visibility = Visibility.Collapsed;
			}
			if(HasFixedRightColumns) {
				if(fixedRightContent != null && fixedRightContent.Content == null)
					fixedRightContent.Content = FixedRightContent;
				if(fixedRightLineContent != null) fixedRightLineContent.Visibility = Visibility.Visible;
			} else {
				if(fixedRightLineContent != null) fixedRightLineContent.Visibility = Visibility.Collapsed;
			}
		}
		protected virtual bool HasFixedLeftColumns { get { return FixedLeftVisibleColumns is IList && ((IList)FixedLeftVisibleColumns).Count > 0; } }
		protected bool HasFixedRightColumns { get { return FixedRightVisibleColumns is IList && ((IList)FixedRightVisibleColumns).Count > 0; } }
		void OnFixedLineWidthChanged() {
			if(fixedLeftLineContent != null) fixedLeftLineContent.Width = FixedLineWidth;
			if(fixedRightLineContent != null) fixedRightLineContent.Width = FixedLineWidth;
		}
		protected virtual void OnFixedNoneContentWidthChanged() {
			if(fixedNoneContentCellsBorder == null)
				return;
			if(fixedNoneContentCellsBorder.Width != FixedNoneContentWidth) {
				if(scrollablePartPanel != null)
					scrollablePartPanel.InvalidateMeasure();
			}
			fixedNoneContentCellsBorder.Width = FixedNoneContentWidth;
		}
		protected virtual void OnScrollingMarginChanged() {
			if(fixedNoneContentInternal != null) fixedNoneContentInternal.Margin = ScrollingMargin;
		}
		void OnFixedColumnsDelimiterTemplateChanged() {
			if(fixedLeftLineContent != null) fixedLeftLineContent.ContentTemplate = FixedColumnsDelimiterTemplate;
			if(fixedRightLineContent != null) fixedRightLineContent.ContentTemplate = FixedColumnsDelimiterTemplate;
		}
	}
	public class GridGroupFooterScrollablePart : GridScrollablePart {
		public static readonly DependencyProperty LevelProperty;
		static GridGroupFooterScrollablePart() {
			LevelProperty = DependencyPropertyManager.Register("Level", typeof(int), typeof(GridGroupFooterScrollablePart), new FrameworkPropertyMetadata(0, (d, e) => ((GridGroupFooterScrollablePart)d).UpdateMargin()));
		}
		public int Level {
			get { return (int)GetValue(LevelProperty); }
			set { SetValue(LevelProperty, value); }
		}
		private void UpdateMargin() {
			OnScrollingMarginChanged();
		}
		protected virtual GroupSummaryRowData RowData { get { return DataContext as GroupSummaryRowData; } }
		protected ITableView TableView { get { return RowData != null ? RowData.View as ITableView : null; } }
		protected override bool HasFixedLeftColumns { get { return TableView == null ? false : TableView.TableViewBehavior.FixedLeftVisibleColumns != null && TableView.TableViewBehavior.FixedLeftVisibleColumns.Count > 0; } }
		protected override void OnScrollingMarginChanged() {
			if(FixedNoneContentCore != null) {
				if(TableView != null && !HasFixedLeftColumns && TableView.ViewportVisibleColumns != null && TableView.ViewportVisibleColumns.Count > 0 && TableView.ViewportVisibleColumns[0].VisibleIndex > 0)
					FixedNoneContentCore.Margin = new Thickness(ScrollingMargin.Left - RowData.Offset, ScrollingMargin.Top, ScrollingMargin.Right, ScrollingMargin.Bottom);
				else 
					FixedNoneContentCore.Margin = ScrollingMargin;
			}
		}
	}
	public class FitContentContainer : Decorator {
		protected override Size MeasureOverride(Size constraint) {
			base.MeasureOverride(constraint);
			return Size.Empty;
		}
	}
	public class GroupValueContentPresenter : GridDataContentPresenter, IGroupValuePresenter {
		public GroupValueContentPresenter() {
			this.SetDefaultStyleKey(typeof(GroupValueContentPresenter));
			GridViewHitInfoBase.SetHitTestAcceptor(this, new GroupValueTableViewHitTestAcceptor());
		}
		#region IGroupValuePresenter Members
		GridGroupValueData IGroupValuePresenter.ValueData {
			get { return Content as GridGroupValueData; }
			set { Content = value; }
		}
		bool? IGroupValuePresenter.UseTemplate { get { return true; } }
		FrameworkElement IGroupValuePresenter.Element { get { return this; } }
		#endregion
	}
	public class GroupSummaryContentPresenter : GridDataContentPresenter, IDefaultGroupSummaryItem {
		public GroupSummaryContentPresenter() {
			this.SetDefaultStyleKey(typeof(GroupSummaryContentPresenter));
			GridViewHitInfoBase.SetHitTestAcceptor(this, new GroupSummaryTableViewHitTestAcceptor());
		}
		#region IDefaultGroupSummaryItem Members
		GridGroupSummaryData IDefaultGroupSummaryItem.ValueData {
			get { return Content as GridGroupSummaryData; }
			set { Content = value; }
		}
		#endregion
	}
	public class TotalSummaryContentPresenter : GridDataContentPresenter {
		public TotalSummaryContentPresenter() {
			this.SetDefaultStyleKey(typeof(TotalSummaryContentPresenter));
		}
	}
	public class FitRowContentPresenter : ContentControl {
		public FitRowContentPresenter() {
			this.SetDefaultStyleKey(typeof(FitRowContentPresenter));
		}
	}
	public class FitRowLeftContentPresenter : ContentControl {
		public FitRowLeftContentPresenter() {
			this.SetDefaultStyleKey(typeof(FitRowLeftContentPresenter));
		}
	}
	public class FixedDelimiter : ContentControl {
		public FixedDelimiter() {
			this.SetDefaultStyleKey(typeof(FixedDelimiter));
		}
	}
	public class RowsDelimiter : ContentControl {
		public RowsDelimiter() {
			this.SetDefaultStyleKey(typeof(RowsDelimiter));
		}
	}
}
