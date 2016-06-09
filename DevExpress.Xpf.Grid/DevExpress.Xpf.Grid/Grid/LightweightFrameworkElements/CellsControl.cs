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
using DevExpress.Xpf.Core;
using System.Linq;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Editors;
using System.Collections.Generic;
using DevExpress.Xpf.Core.Native;
using System.Windows.Media;
using DevExpress.Mvvm.Native;
using DevExpress.Xpf.Grid.HitTest;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Data;
using DevExpress.Data;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.ConditionalFormatting;
namespace DevExpress.Xpf.Grid {
	public abstract class LightweightCellEditorBase : CellEditor {
		public static readonly DependencyProperty ForegroundProperty = TextBlock.ForegroundProperty.AddOwner(typeof(LightweightCellEditorBase));
		public Brush Foreground {
			get { return (Brush)GetValue(ForegroundProperty); }
			set { SetValue(ForegroundProperty, value); }
		}
		public static readonly DependencyProperty FontSizeProperty = TextBlock.FontSizeProperty.AddOwner(typeof(LightweightCellEditorBase));
		public double FontSize {
			get { return (double)GetValue(FontSizeProperty); }
			set { SetValue(FontSizeProperty, value); }
		}
		public static readonly DependencyProperty FontFamilyProperty = TextBlock.FontFamilyProperty.AddOwner(typeof(LightweightCellEditorBase));
		public FontFamily FontFamily {
			get { return (FontFamily)GetValue(FontFamilyProperty); }
			set { SetValue(FontFamilyProperty, value); }
		}
	}
	public class LightweightCellEditor : LightweightCellEditorBase, IGridCellEditorOwner, IChrome, ISupportLoadingAnimation, IOrderPanelElement, IConditionalFormattingClient<LightweightCellEditor>, ISupportHorizonalContentAlignment {
		#region properties
		public Brush Background {
			get { return (Brush)GetValue(BackgroundProperty); }
			set { SetValue(BackgroundProperty, value); }
		}
		public static readonly DependencyProperty BackgroundProperty =
			DependencyProperty.Register("Background", typeof(Brush), typeof(LightweightCellEditor), new PropertyMetadata(null, (d, e) => ((LightweightCellEditor)d).UpdateBackgroundFromStyle()));
		public Brush BorderBrush {
			get { return (Brush)GetValue(BorderBrushProperty); }
			set { SetValue(BorderBrushProperty, value); }
		}
		public static readonly DependencyProperty BorderBrushProperty =
			DependencyProperty.Register("BorderBrush", typeof(Brush), typeof(LightweightCellEditor), new PropertyMetadata(null, (d, e) => ((LightweightCellEditor)d).UpdateBorderFromStyle()));
		public Thickness Padding {
			get { return (Thickness)GetValue(PaddingProperty); }
			set { SetValue(PaddingProperty, value); }
		}
		public static readonly DependencyProperty PaddingProperty =
			DependencyProperty.Register("Padding", typeof(Thickness), typeof(LightweightCellEditor), new PropertyMetadata(default(Thickness), (d, e) => ((LightweightCellEditor)d).UpdatePaddingFromStyle()));
		public SelectionState SelectionState {
			get { return (SelectionState)GetValue(SelectionStateProperty); }
			private set { SetValue(SelectionStatePropertyKey, value); }
		}
		static readonly DependencyPropertyKey SelectionStatePropertyKey =
			DependencyProperty.RegisterReadOnly("SelectionState", typeof(SelectionState), typeof(LightweightCellEditor), new PropertyMetadata(SelectionState.None));
		public static readonly DependencyProperty SelectionStateProperty = SelectionStatePropertyKey.DependencyProperty;
		public SelectionState RowSelectionState {
			get { return (SelectionState)GetValue(RowSelectionStateProperty); }
			private set { SetValue(RowSelectionStatePropertyKey, value); }
		}
		static readonly DependencyPropertyKey RowSelectionStatePropertyKey =
			DependencyProperty.RegisterReadOnly("RowSelectionState", typeof(SelectionState), typeof(LightweightCellEditor), new PropertyMetadata(SelectionState.None));
		public static readonly DependencyProperty RowSelectionStateProperty = RowSelectionStatePropertyKey.DependencyProperty;
		#endregion
		internal readonly CellsControl cellsControl;
		static readonly RenderTemplate backgroundTemplate;
		static LightweightCellEditor() {
			Type ownerType = typeof(LightweightCellEditor);
			GridViewHitInfoBase.HitTestAcceptorProperty.OverrideMetadata(ownerType, new PropertyMetadata(new RowCellTableViewHitTestAcceptor()));
			DataControlPopupMenu.GridMenuTypeProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(GridMenuType.RowCell));
			backgroundTemplate = new RenderTemplate() { RenderTree = new RenderBorder() };
		}
		static bool DataContextCoerceCallbackRegistered = false;
		static void RegisterDataContextCoerceCallback() {
			if(DataContextCoerceCallbackRegistered)
				return;
			DataContextCoerceCallbackRegistered = true;
			DataContextProperty.OverrideMetadata(typeof(LightweightCellEditor), new FrameworkPropertyMetadata(DataContextProperty.DefaultMetadata.DefaultValue, null, (d, v) => ((LightweightCellEditor)d).CoerceDataContext(v)));
		}
#if DEBUGTEST
		internal int CoerceDataContextCallCount { get; private set; }
#endif
		object CoerceDataContext(object baseValue) {
#if DEBUGTEST
			CoerceDataContextCallCount++;
#endif
			if(!HasStyle)
				return baseValue;
			return CellData ?? baseValue;
		}
		public LightweightCellEditor(CellsControl cellsControl) {
			formattingHelper = new ConditionalFormattingHelper<LightweightCellEditor>(this);
			this.cellsControl = cellsControl;
			GridCellEditorOwner = this;
			this.border = (RenderBorderContext)ChromeHelper.CreateContext(this, backgroundTemplate);
			this.conditionalFormatContentRenderHelper = new ConditionalFormatContentRenderHelper<LightweightCellEditor>(this);
			UpdateBorderFromBrushSet();
		}
		protected override void NullEditorInEditorDataContext() {
			base.NullEditorInEditorDataContext();
			UpdateDataContext(force: false);
		}
		protected override void SetEditorInEditorDataContext() {
			base.SetEditorInEditorDataContext();
			UpdateDataContext(force: false);
		}
		void UpdateDataContext(bool force) {
			if(force || HasStyle) {
				RegisterDataContextCoerceCallback();
				CoerceValue(DataContextProperty);
			}
		}
		public override void OnColumnChanged(ColumnBase oldValue, ColumnBase newValue) {
			base.OnColumnChanged(oldValue, newValue);
			UpdateStyle();
			UpdateConditionalAppearance();
		}
		protected override void OnPreviewMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnPreviewMouseRightButtonDown(e);
			AssignContextMenu();
		}
		internal void AssignContextMenu() {
			DevExpress.Xpf.Bars.BarManager.SetDXContextMenu(this, View.DataControlMenu);
		}
		protected override void OnColumnContentChanged(object sender, ColumnContentChangedEventArgs e) {
			base.OnColumnContentChanged(sender, e);
			if(e.Property == ColumnBase.ActualCellStyleProperty)
				UpdateStyle();
		}
		Style styleCore;
		void UpdateStyle() {
			Style newStyle = Column != null ? Column.ActualCellStyle : null;
			if(newStyle is DefaultStyle)
				newStyle = null;
			if(newStyle != styleCore) {
				styleCore = newStyle;
				if(newStyle != null)
					UpdateDataContext(force: true);
				Style = newStyle;
				if(newStyle == null)
					UpdateDataContext(force: true);
				UpdateBackgroundFromBrushSet();
				UpdateBorderFromBrushSet();
				UpdateForegroundFromBrushSet();
				UpdateBackgroundFromStyle();
				UpdateBorderFromStyle();
				UpdateForegroundFromStyle();
				UpdatePaddingFromStyle();
			}
		}
		internal InplaceBaseEdit InplaceBaseEdit { get { return Content as InplaceBaseEdit; } }
		protected override EditorOptimizationMode GetEditorOptimizationMode() {
			return EditorOptimizationMode.Simple;
		}
		internal void SetBorderState(GridCellData cellData, SelectionState rowSelectionState) {
			Thickness thickness = GetCellBorderThickness(cellData);
			border.BorderThickness = thickness;
			UpdateChildMargin();
			double leftIndent = RowData != null && Column != null ? RowData.GetRowIndent(Column) : 0d;
			conditionalFormatContentRenderHelper.SetMargin(new Thickness(leftIndent + thickness.Left, thickness.Top, thickness.Right, thickness.Bottom));
			if(RowSelectionState != rowSelectionState) {
				RowSelectionState = rowSelectionState;
				UpdateBorderFromBrushSet();
				UpdateConditionalAppearance();
			}
		}
		Thickness GetCellBorderThickness(GridCellData cellData) {
			Thickness thickness = default(Thickness);
			ITableView tableView = (ITableView)cellData.View;
			if(tableView.ShowVerticalLines) {
				if(cellData.Column.HasLeftSibling && cellData.Column.ColumnPosition == ColumnPosition.Left)
					thickness.Left = 1;
				if(cellData.Column.HasRightSibling)
					thickness.Right = 1;
			}
			if(tableView.ShowHorizontalLines && cellData.Column.HasTopElement) {
				thickness.Top = 1;
			}
			return thickness;
		}
		Thickness childMargin;
		void UpdateChildMargin() {
			if(border.BorderThickness.HasValue) {
				Thickness value = border.BorderThickness.Value;
				if(childMargin != value) {
					childMargin = value;
					InvalidateMeasure();
				}
			}
		}
		protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved) {
			base.OnVisualChildrenChanged(visualAdded, visualRemoved);
			UpdateChildMargin();
		}
		bool HasStyle { get { return styleCore != null; } }
		void UpdatePaddingFromStyle() {
			if(HasStyle)
				border.Padding = Padding;
		}
		void UpdateBorderFromStyle() {
			if(HasStyle)
				border.BorderBrush = BorderBrush;
		}
		void UpdateBackgroundFromStyle() {
			if(HasStyle)
				border.Background = formattingHelper.CoerceBackground(Background);
		}
		readonly ConditionalFormattingHelper<LightweightCellEditor> formattingHelper;
		protected override void UpdateConditionalAppearance() {
			base.UpdateConditionalAppearance();
			formattingHelper.UpdateConditionalAppearance();
		}
		void UpdateForegroundFromStyle() {
			if(HasStyle)
				ClearValue(TextBlock.ForegroundProperty);
		}
		void UpdateBorderFromBrushSet() {
			if(!HasStyle)
				border.BorderBrush = GetBorderBrushFromBrushSet();
		}
		Brush GetBorderBrushFromBrushSet() {
			string brushName = "BorderBrush";
			if(RowSelectionState == SelectionState.Focused)
				brushName += "FocusedRow";
			return cellsControl.RowControl.CellBackgroundBrushes.GetBrush(brushName);
		}
		void UpdateBackgroundFromBrushSet() {
			if(!HasStyle)
				border.Background = formattingHelper.CoerceBackground(cellsControl.RowControl.CellBackgroundBrushes.GetBrush(SelectionState.ToString()));
		}
		void UpdateForegroundFromBrushSet() {
			if(!HasStyle)
				cellsControl.RowControl.CellForegroundBrushes.ApplyForeground(this, SelectionState.ToString());
		}
		protected override void OnEditorPreviewLostKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e) {
			if(e.NewFocus == null)
				e.Handled = true;
			else
				base.OnEditorPreviewLostKeyboardFocus(sender, e);
		}
		protected override void OnEditorActivated(object sender, RoutedEventArgs e) {
			base.OnEditorActivated(sender, e);
			UpdateDataBarFormatInfo();
			RowData.RaiseResetEvents();
		}
		protected override void OnShowEditor() {
			base.OnShowEditor();
			UpdateConditionalAppearance();
		}
		protected override void OnHiddenEditor(bool closeEditor) {
			base.OnHiddenEditor(closeEditor);
			UpdateDataBarFormatInfo();
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			InplaceBaseEdit edit = GetTemplateChild("PART_Editor") as InplaceBaseEdit;
			if(edit != null && Column.EditSettings != null)
				edit.SetSettings(Column.EditSettings);
		}
		#region IGridCellEditorOwner
		bool IGridCellEditorOwner.CanRefreshContent { get { return CanRefreshContentCore; } }
		protected virtual bool CanRefreshContentCore { get { return LayoutHelper.IsChildElement(RowData.RowElement, this); } }
		DependencyObject IGridCellEditorOwner.EditorRoot { get { return this; } }
		ColumnBase IGridCellEditorOwner.AssociatedColumn { get { return Column; } }
		void IGridCellEditorOwner.SynProperties(GridCellData cellData) {
		}
		void IGridCellEditorOwner.UpdateCellState() { }
		LoadingAnimationHelper loadingAnimationHelper;
		internal LoadingAnimationHelper LoadingAnimationHelper {
			get {
				if(loadingAnimationHelper == null)
					loadingAnimationHelper = new LoadingAnimationHelper(this);
				return loadingAnimationHelper;
			}
		}
		DataViewBase ISupportLoadingAnimation.DataView { get { return View; } }
		FrameworkElement Child {
			get {
				if(InplaceBaseEdit != null) return InplaceBaseEdit;
				if(VisualChildrenCount > 0)
					return GetVisualChild(0) as FrameworkElement;
				return null;
			}
		}
		FrameworkElement ISupportLoadingAnimation.Element { get { return Child; } }
		bool ISupportLoadingAnimation.IsGroupRow { get { return false; } }
		bool ISupportLoadingAnimation.IsReady { get { return RowData.IsReady; } }
		void IGridCellEditorOwner.UpdateIsReady() {
			LoadingAnimationHelper.ApplyAnimation();
			UpdateConditionalAppearance();
		}
		void IGridCellEditorOwner.OnViewChanged() { }
		void IGridCellEditorOwner.SetSelectionState(SelectionState state) {
			if(this.SelectionState != state) {
				this.SelectionState = state;
				UpdateBackgroundFromBrushSet();
				UpdateForegroundFromBrushSet();
			}
		}
		void IGridCellEditorOwner.SetIsFocusedCell(bool isFocusedCell) {
			IsFocusedCell = isFocusedCell;
		}
		void IGridCellEditorOwner.UpdateCellBackgroundAppearance() {
			UpdateBackgroundFromBrushSet();
			UpdateBorderFromBrushSet();
		}
		void IGridCellEditorOwner.UpdateCellForegroundAppearance() {
			UpdateForegroundFromBrushSet();
		}
		#endregion
		#region rendering
		readonly RenderBorderContext border;
#if DEBUGTEST
		internal RenderBorderContext BorderForTests { get { return border; } }
		internal ConditionalFormatContentRenderHelper<LightweightCellEditor> ConditionalFormatContentRenderHelperForTests { get { return conditionalFormatContentRenderHelper; } }
#endif
#if DEBUGTEST
		public static int MeasureCount;
		public static int ArrangeCount;
#endif
		protected override Size MeasureOverride(Size constraint) {
#if DEBUGTEST
			MeasureCount++;
#endif
			conditionalFormatContentRenderHelper.Measure(constraint);
			double verticalMargin = GetVerticalMargin(childMargin);
			double horizontalMargin = GetHorizontalMargin(childMargin);
			var res = base.MeasureOverride(new Size(Math.Max(0, constraint.Width - horizontalMargin), Math.Max(0, constraint.Height - verticalMargin)));
			return new Size(res.Width + horizontalMargin, res.Height + verticalMargin);
		}
		protected override Size ArrangeOverride(Size finalSize) {
#if DEBUGTEST
			ArrangeCount++;
#endif
			border.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
			conditionalFormatContentRenderHelper.Arrange(finalSize);
			if(Child != null) {
				Child.Arrange(new Rect(childMargin.Left, childMargin.Top, Math.Max(0, finalSize.Width - GetHorizontalMargin(childMargin)), Math.Max(0, finalSize.Height - GetVerticalMargin(childMargin))));
				return finalSize;
			}
			return base.ArrangeOverride(finalSize);
		}
		double GetVerticalMargin(Thickness margin) {
			return margin.Top + margin.Bottom;
		}
		double GetHorizontalMargin(Thickness thickness) {
			return thickness.Left + thickness.Right;
		}
		protected override void OnRender(DrawingContext dc) {
			border.Render(dc);
			conditionalFormatContentRenderHelper.Render(dc);
		}
		FrameworkRenderElementContext IChrome.Root { get { return null; } }
		void IChrome.GoToState(string stateName) { }
		void IChrome.AddChild(FrameworkElement element) {
			throw new NotSupportedException();
		}
		void IChrome.RemoveChild(FrameworkElement element) {
			throw new NotSupportedException();
		}
		#endregion
		#region IOrderPanelElement
		int visibleIndex = OrderPanelBase.InvisibleIndex;
		int IOrderPanelElement.VisibleIndex {
			get { return visibleIndex; }
			set { visibleIndex = value; }
		}
		#endregion
		#region IConditionalFormattingClient
		ConditionalFormattingHelper<LightweightCellEditor> IConditionalFormattingClient<LightweightCellEditor>.FormattingHelper { get { return formattingHelper; } }
		bool IConditionalFormattingClient<LightweightCellEditor>.IsSelected { get { return IsEditorVisible || RowSelectionState != SelectionState.None || (SelectionState != SelectionState.None && SelectionState != SelectionState.CellMerge); } }
		IList<FormatConditionBaseInfo> IConditionalFormattingClient<LightweightCellEditor>.GetRelatedConditions() {
			var tableView = ((ITableView)View);
			return (tableView != null && Column != null) ? tableView.FormatConditions.GetInfoByFieldName(Column.FieldName) : null;
		}
		internal static readonly ServiceSummaryItem[] EmptySummaries = new ServiceSummaryItem[0];
		FormatValueProvider? IConditionalFormattingClient<LightweightCellEditor>.GetValueProvider(string fieldName) {
			if(DataControl == null)
				return null;
			return RowData.GetValueProvider(fieldName);
		}
		bool IConditionalFormattingClient<LightweightCellEditor>.IsReady { get { return RowData.IsReady; } }
		void IConditionalFormattingClient<LightweightCellEditor>.UpdateBackground() {
			UpdateBackgroundFromStyle();
			UpdateBackgroundFromBrushSet();
		}
		readonly ConditionalFormatContentRenderHelper<LightweightCellEditor> conditionalFormatContentRenderHelper;
		DataBarFormatInfo info;
#if DEBUGTEST
		internal int UpdateDataBarFormatInfoCountForTests { get; private set; }
#endif
		void IConditionalFormattingClient<LightweightCellEditor>.UpdateDataBarFormatInfo(DataBarFormatInfo info) {
#if DEBUGTEST
			UpdateDataBarFormatInfoCountForTests++;
#endif
			if(!object.Equals(this.info, info)) {
				this.info = info;
				UpdateDataBarFormatInfo();
			}
		}
		void UpdateDataBarFormatInfo() {
			conditionalFormatContentRenderHelper.UpdateDataBarFormatInfo(IsEditorVisible ? null : info);
		}
		Locker IConditionalFormattingClient<LightweightCellEditor>.Locker { get { return RowData.conditionalFormattingLocker; } }
		#endregion
		#region ISupportHorizonalContentAlignment
		HorizontalAlignment ISupportHorizonalContentAlignment.HorizonalContentAlignment { get { return Edit.HorizontalContentAlignment; } }
		#endregion
		protected override InplaceEditorBase ReraiseMouseEventEditor { get { return Owner.CurrentCellEditor; } }
	}
	public class CellsControl : CellItemsControlBase {
		static readonly ControlTemplate OrdinarPanelTemplate;
		static readonly ControlTemplate BandsPanelTemplate;
		static readonly ControlTemplate CellMergingPanelTemplate;
		static CellsControl() {
			var ordinarPanelFactory = new FrameworkElementFactory(typeof(StackVisibleIndexPanel));
			ordinarPanelFactory.SetValue(OrderPanelBase.ArrangeAccordingToVisibleIndexProperty, true);
			ordinarPanelFactory.SetValue(OrderPanelBase.OrientationProperty, Orientation.Horizontal);
			OrdinarPanelTemplate = CreatePanelTemplate(ordinarPanelFactory);
			ItemsPanelProperty.OverrideMetadata(typeof(CellsControl), new PropertyMetadata(OrdinarPanelTemplate));
			var bandsPanelFactory = new FrameworkElementFactory(typeof(BandsCellsPanel));
			BandsPanelTemplate = CreatePanelTemplate(bandsPanelFactory);
			var cellMergingPanelTemplate = new FrameworkElementFactory(typeof(CellMergingPanel));
			cellMergingPanelTemplate.SetValue(OrderPanelBase.ArrangeAccordingToVisibleIndexProperty, true);
			cellMergingPanelTemplate.SetValue(OrderPanelBase.OrientationProperty, Orientation.Horizontal);
			CellMergingPanelTemplate = CreatePanelTemplate(cellMergingPanelTemplate);
		}
		static ControlTemplate CreatePanelTemplate(FrameworkElementFactory panelFactory) {
			ControlTemplate template = new ControlTemplate(typeof(ItemsControlBase));
			template.VisualTree = panelFactory;
			template.Seal();
			return template;
		}
		public CellsControl(RowControl rowControl, Func<RowData, IList<GridColumnData>> getCellDataFunc, Func<BandsLayoutBase, IList<BandBase>> getBandsFunc) {
			this.RowControl = rowControl;
			this.getCellDataFunc = getCellDataFunc;
			this.getBandsFunc = getBandsFunc;
		}
		readonly Func<RowData, IList<GridColumnData>> getCellDataFunc;
		readonly Func<BandsLayoutBase, IList<BandBase>> getBandsFunc;
		internal RowControl RowControl { get; private set; }
		protected override Size ArrangeOverride(Size arrangeBounds) {
			Size size = base.ArrangeOverride(arrangeBounds);
			if(View.ActualAllowCellMerge) {
				double maxOffset = View.RootDataPresenter.LastConstraint.Height;
				double width = TableViewProperties.GetFixedAreaStyle(this) == FixedStyle.None ? RowControl.rowData.FixedNoneContentWidth : size.Width;
				Clip = new RectangleGeometry(new Rect(0, -maxOffset, width, maxOffset + size.Height));
			} else {
				Clip = new RectangleGeometry(new Rect(0, 0, size.Width, size.Height));
			}
			return size;
		}
		internal void UpdateItemsSource() {
			ItemsSource = getCellDataFunc(RowControl.rowData);
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			UpdatePanelOffset();
			UpdateBands();
		}
		internal void UpdateBands() {
			BandsCellsPanel panel = Panel as BandsCellsPanel;
			if(panel != null)
				panel.Bands = getBandsFunc(RowControl.BandsLayout);
		}
		protected override FrameworkElement CreateChildCore(GridCellData cellData) {
			return new LightweightCellEditor(this) { RowData = cellData.RowData };
		}
		protected override void ValidateElementCore(FrameworkElement element, GridCellData cellData) {
			LightweightCellEditor cellEditor = (LightweightCellEditor)element;
			cellEditor.CellData = (EditGridCellData)cellData;
			cellEditor.Column = cellData.Column;
			cellData.OnEditorContentUpdated();
			UpdateElementWidth(element, cellData);
			cellData.SyncLeftMargin(element);
			GridColumn.SetNavigationIndex(element, GridColumn.GetVisibleIndex(cellData.Column));
			cellEditor.SetBorderState(cellData, RowControl.rowData.SelectionState);
		}
		protected virtual void UpdateElementWidth(FrameworkElement element, GridCellData cellData) {
			element.Width = cellData.GetActualCellWidth();
		}
		Thickness panelOffset;
		internal void SetPanelOffset(Thickness panelOffset) {
			if(this.panelOffset != panelOffset) {
				this.panelOffset = panelOffset;
				UpdatePanelOffset();
			}
		}
		void UpdatePanelOffset() {
			if(Panel != null)
				Panel.Margin = panelOffset;
		}
		protected internal virtual void UpdatePanel() {
			Template = GetTemplate();
		}
		internal void InvalidatePanel() {
			if(Panel != null)
				Panel.InvalidateArrange();
		}
		ControlTemplate GetTemplate() {
			if(RowControl.IsBandedLayout)
				return BandsPanelTemplate;
			return View.ActualAllowCellMerge ? CellMergingPanelTemplate : OrdinarPanelTemplate;
		}
	}
}
