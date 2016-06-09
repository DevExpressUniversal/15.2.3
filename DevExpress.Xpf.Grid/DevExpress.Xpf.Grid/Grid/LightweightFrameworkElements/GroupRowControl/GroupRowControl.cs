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
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid.GroupRowLayout;
namespace DevExpress.Xpf.Grid {
	public interface IGroupRow {
		FrameworkElement RowElement { get; }
	}
	public class GroupRowControl : Control, IGroupRow, IGroupRowStateClient, IFixedGroupElement, IFocusedRowBorderObject {
		#region DependencyProperties
		public double FocusOffset {
			get { return (double)GetValue(FocusOffsetProperty); }
			set { SetValue(FocusOffsetProperty, value); }
		}
		public static readonly DependencyProperty FocusOffsetProperty =
			DependencyProperty.Register("FocusOffset", typeof(double), typeof(GroupRowControl), new PropertyMetadata(0.0));
		public SelectionState SelectionState {
			get { return (SelectionState)GetValue(SelectionStateProperty); }
			set { SetValue(SelectionStateProperty, value); }
		}
		public static readonly DependencyProperty SelectionStateProperty =
			DependencyProperty.Register("SelectionState", typeof(SelectionState), typeof(GroupRowControl), new PropertyMetadata(SelectionState.None));
		public bool FadeSelection {
			get { return (bool)GetValue(FadeSelectionProperty); }
			set { SetValue(FadeSelectionProperty, value); }
		}
		public static readonly DependencyProperty FadeSelectionProperty =
			DependencyProperty.Register("FadeSelection", typeof(bool), typeof(GroupRowControl), new PropertyMetadata(false));
		public Brush RowFitBorderBrush {
			get { return (Brush)GetValue(RowFitBorderBrushProperty); }
			set { SetValue(RowFitBorderBrushProperty, value); }
		}
		public static readonly DependencyProperty RowFitBorderBrushProperty =
			DependencyProperty.Register("RowFitBorderBrush", typeof(Brush), typeof(GroupRowControl), new PropertyMetadata(null, (d, e) => ((GroupRowControl)d).UpdateFitContentBorderBrush()));
		public Thickness BottomLineMargin {
			get { return (Thickness)GetValue(BottomLineMarginProperty); }
			set { SetValue(BottomLineMarginProperty, value); }
		}
		public static readonly DependencyProperty BottomLineMarginProperty =
			DependencyProperty.Register("BottomLineMargin", typeof(Thickness), typeof(GroupRowControl), new PropertyMetadata(new Thickness(0), (d, e) => ((GroupRowControl)d).UpdateBottomLineMargin()));
		#endregion
		#region Fields
		bool oldUseTemplate;
		GroupRowControlPanel layoutPanel;
		protected UIElement rootPanel;
		Border backgroundBorder;
		protected Border backgroundFocusedBorder;
		RowIndicator indicator;
		FrameworkElement offsetPresenter;
		GroupRowExpandButton groupExpandButton;
		GroupRowCheckBoxSelector checkBoxSelector;
		IGroupValuePresenter groupValuePresenter = new NullGroupValuePresenter();
		GroupRowDefaultSummaryControl summaryDefaultControl;
		SummaryAlignByColumnsController summaryAlignByColumnsController;
		DetailRowsIndentControl detailLeftIndentControl;
		RowFitBorder fitContent;
		protected Border bottomLine;
		ContentPresenter contentPresenter;
		protected readonly GroupRowData rowData;
		#endregion
		#region Positions
		IndexDefinition IndicatorPosition = new IndexDefinition(0, 0, 0);
		IndexDefinition LeftDetailIndentPosition = new IndexDefinition(0, 0, 1);
		IndexDefinition GroupOffsetPosition = new IndexDefinition(0, 0, 2);
		IndexDefinition ContentPresenterPosition = new IndexDefinition(0, 0, 3);
		IndexDefinition GroupExpandButtonPosition = new IndexDefinition(1, 0, 0);
		IndexDefinition CheckBoxSelectorPosition = new IndexDefinition(1, 0, 1);
		IndexDefinition GroupValuePresenterPosition = new IndexDefinition(1, 0, 2);
		IndexDefinition ColumnSummaryPosition = new IndexDefinition(1, 1, 0);
		IndexDefinition FitContentPosition = new IndexDefinition(2, 0, 0);
		IndexDefinition DefaultSummaryPosition = new IndexDefinition(2, 0, 1);
		#endregion
		GridViewBase View { get { return (GridViewBase)rowData.View; } }
		TableView ViewTable { get { return rowData.View as TableView; } }
		GridColumn Column { get { return (GridColumn)rowData.GroupValue.Column; } }
		RowsContainer LogicalItemsContainer { get { return rowData.RowsContainer; } }
		static GroupRowControl() {
			Type ownerType = typeof(GroupRowControl);
			DefaultStyleKeyProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(ownerType));
			DataControlPopupMenu.GridMenuTypeProperty.OverrideMetadata(ownerType, new FrameworkPropertyMetadata(GridMenuType.GroupRow));
			GridViewHitInfoBase.HitTestAcceptorProperty.OverrideMetadata(ownerType, new PropertyMetadata(new DevExpress.Xpf.Grid.HitTest.GroupRowTableViewHitTestAcceptor()));
		}
		public GroupRowControl(GroupRowData rowData) {
			this.rowData = rowData;
			rowData.SetGroupRowStateClient(this);
		}
		protected override void OnPreviewMouseRightButtonDown(System.Windows.Input.MouseButtonEventArgs e) {
			base.OnPreviewMouseRightButtonDown(e);
			AssignContextMenu();
		}
		internal void AssignContextMenu() {
			DevExpress.Xpf.Bars.BarManager.SetDXContextMenu(this, View.DataControlMenu);
		}
		protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e) {
			base.OnMouseLeftButtonDown(e);
			if(e.ClickCount == 2)
				RowExpandedCommand();
		}
		#region DEBUGTEST
#if DEBUGTEST
		internal UIElement RootPanelForTests { get { return rootPanel; } }
		internal Panel LayoutPanelForTests { get { return layoutPanel; } }
		internal SummaryAlignByColumnsController GroupSummaryAlignByColumnsControllerForTests { get { return summaryAlignByColumnsController; } }
		internal RowIndicator IndicatorForTests { get { return indicator; } }
		internal GroupRowExpandButton GroupExpandButtonForTests { get { return groupExpandButton; } }
		internal GroupValueContentPresenter GroupValueContentPresenterForTests { get { return groupValuePresenter as GroupValueContentPresenter; } }
		internal GroupValuePresenter GroupValuePresenterForTests { get { return groupValuePresenter as GroupValuePresenter; } }
		internal GroupRowDefaultSummaryControl GroupSummaryDefaultLayoutControlForTests { get { return summaryDefaultControl; } }
		internal ContentPresenter ContentPresenterForTests { get { return contentPresenter; } }
		internal RowsContainer LogicalItemsContainerForTests { get { return LogicalItemsContainer; } }
		internal DetailRowsIndentControl DetailLeftIndentControlForTests { get { return detailLeftIndentControl; } }
		internal RowFitBorder FitContentForTests { get { return fitContent; } }
		internal Border BackgroundBorderForTests { get { return backgroundBorder; } }
		internal FrameworkElement OffsetPresenterForTest { get { return offsetPresenter; } }
		internal FrameworkElement GetTemplateChildInternal(string name) {
			return (FrameworkElement)GetTemplateChild(name);
		}
		internal double GetLayoutSummaryPanelLeftIndentForTests() {
			return GetLayoutSummaryPanelLeftIndent();
		}
#endif
		#endregion
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
			rootPanel = (System.Windows.Controls.Grid)GetTemplateChild("PART_RootPanel");
			layoutPanel = (GroupRowControlPanel)GetTemplateChild("PART_LayoutPanel");
			backgroundBorder = (Border)GetTemplateChild("Background");
			UpdateBottomLineMargin(backgroundBorder);
			if(FocusOffset > 0) {
				backgroundFocusedBorder = (Border)GetTemplateChild("BackgroundFocused");
				bottomLine = (Border)GetTemplateChild("BottomLine");
				UpdateBackgroundFocusedBorderVisibility();
			}
			CreateContent();
		}
		protected override Size MeasureOverride(Size constraint) {
			Size size = base.MeasureOverride(constraint);
#if DEBUGTEST
			if(GridRow.RoundRowSize)
#endif
				size = new Size(Math.Ceiling(size.Width), Math.Ceiling(size.Height));
			return size;
		}
		protected override Size ArrangeOverride(Size arrangeBounds) {
			UpdateLayoutSummaryPanelClip(arrangeBounds);
			return base.ArrangeOverride(arrangeBounds);
		}
		void AddPanelElement(FrameworkElement element, IndexDefinition index) {
			if(element == null || layoutPanel == null)
				return;
			layoutPanel.Groups.Add(new Column(element), index);
			element.SizeChanged += LayoutPanelElementSizeChanged;
		}
		void LayoutPanelElementSizeChanged(object sender, SizeChangedEventArgs e) {
			UpdateLayoutSummaryPanelClip(e.NewSize);
		}
		void RemovePanelElement(IndexDefinition index) {
			if(layoutPanel == null)
				return;
			Column column = layoutPanel.Groups.Get(index);
			if(column == null)
				return;
			var element = column.Element as FrameworkElement;
			if(element != null)
				element.SizeChanged -= LayoutPanelElementSizeChanged;
			layoutPanel.Groups.Remove(index);
		}
		void CreateContent() {
			ClearElements();
			oldUseTemplate = UseGroupRowTemplate();
			if(oldUseTemplate)
				CreateTemplateContent();
			else
				CreateDefaultContent();
		}
		void CreateDefaultContent() {
			Group group1 = new Group();
			group1.Add(new Layer(), 0);
			Group group2 = new Group();
			group2.Add(new Layer(), 0);
			group2.Add(new Layer(), 1);
			Group group3 = new Group();
			group3.Add(new Layer(), 0);
			layoutPanel.Groups.Add(group1, 0);
			layoutPanel.Groups.Add(group2, 1);
			layoutPanel.Groups.Add(group3, 2);
			UpdateIndicator();
			UpdateOffsetPresenter();
			UpdateCheckBoxSelector();
			UpdateGroupValuePresenter();
			UpdateSummary();
			UpdateGroupExpandButton();
			UpdateLeftDetailViewIndents();
		}
		void CreateTemplateContent() {
			var group = new Group();
			group.Add(new Layer(), 0);
			layoutPanel.Groups.Add(group, 0);
			UpdateIndicator();
			UpdateOffsetPresenter();
			contentPresenter = new GroupGridRowPresenter() { Content = rowData, ContentTemplateSelector = View.ActualGroupRowTemplateSelector };
			AddPanelElement(contentPresenter, ContentPresenterPosition);
		}
		void ClearElements() {
			layoutPanel.ResetGroups();
			summaryAlignByColumnsController = null;
			indicator = null;
			offsetPresenter = null;
			groupExpandButton = null;
			checkBoxSelector = null;
			groupValuePresenter = null;
			summaryDefaultControl = null;
			detailLeftIndentControl = null;
			contentPresenter = null;
			groupValuePresenter = new NullGroupValuePresenter();
			fitContent = null;
		}
		bool UseGroupRowTemplate() {
			return !IsNullOrDefaultTemplate(View.GroupRowTemplate) ||
				View.GroupRowTemplateSelector != null && !IsNullOrDefaultTemplate(View.GroupRowTemplateSelector.SelectTemplate(rowData, null));
		}
		void UpdateContent() {
			if(layoutPanel == null)
				return;
			if(oldUseTemplate != UseGroupRowTemplate())
				CreateContent();
			else if(contentPresenter != null)
				contentPresenter.ContentTemplateSelector = View.ActualGroupRowTemplateSelector;
		}
		void UpdateFadeSelection() {
			DataViewBase rootView = View.RootView;
			if(rootView.DataControl != null)
				FadeSelection = FadeSelectionHelper.IsFadeNeeded(rootView.ActualFadeSelectionOnLostFocus, rootView.DataControl.IsKeyboardFocusWithin) && SelectionState != SelectionState.None;
		}
		protected void UpdateBackgroundFocusedBorderVisibility() {
			if(backgroundFocusedBorder != null) {
				switch(SelectionState) {
					case SelectionState.Focused:
					case SelectionState.Selected:
						backgroundFocusedBorder.Visibility = Visibility.Visible;
						break;
					default:
						backgroundFocusedBorder.Visibility = Visibility.Collapsed;
						break;
				}
			}
		}
		protected virtual void IsPreviewExpandedChanged() { }
		protected virtual void UpdateCardLayoutChanged() { }
		protected virtual void RowPositionChange() { }
		#region Offset
		protected virtual double CalcLevelOffset(){
			if (ViewTable != null)
				return ViewTable.LeftGroupAreaIndent * rowData.Level - (ViewTable.ActualShowDetailButtons && ViewTable.ActualExpandDetailHeaderWidth <= ViewTable.LeftGroupAreaIndent * rowData.Level ? ViewTable.ActualExpandDetailHeaderWidth : 0);
			return 0;
		}
		double CalcOffset() {
			double focusOffset = FocusOffset;
			if(indicator != null && indicator.Visibility == Visibility.Visible && ViewTable != null)
				focusOffset += ViewTable.ActualIndicatorWidth;
			return focusOffset;
		}
		double CalcFullOffset() {
			double focusOffset = CalcOffset();
			if(rowData.Level > 0)
				focusOffset += CalcLevelOffset();
			if(detailLeftIndentControl != null)
				foreach(var items in detailLeftIndentControl.ItemsSource) {
					DetailIndent detail = items as DetailIndent;
					if(detail != null)
						focusOffset += detail.Width;
				}
			return focusOffset;
		}
		void UpdateOffsetPresenter() {
			if(layoutPanel != null && (rowData.Level > 0 || View.IsRowMarginControlVisible)) {
				if(offsetPresenter == null) {
					offsetPresenter = new GroupRowOffsetPresenter();
					AddPanelElement(offsetPresenter, GroupOffsetPosition);
#if DEBUGTEST
					if(offsetPresenter != null)
						DevExpress.Xpf.Grid.Tests.GridTestHelper.SetSkipChildrenWhenStoreVisualTree(offsetPresenter, true);
#endif
				}
			}
			if(offsetPresenter != null) {
				if(rowData.Level > 0)
					offsetPresenter.Width = CalcLevelOffset();
				else {
					RemovePanelElement(GroupOffsetPosition);
					offsetPresenter = null;
				}
			}
			SetOffsetBackgroundFocus();		   
		}
		protected virtual void SetOffsetBackgroundFocus(){
			 if(FocusOffset > 0) {
				Thickness oldMargin = backgroundFocusedBorder.Margin;
				backgroundFocusedBorder.Margin = new Thickness(CalcFullOffset(), oldMargin.Top, oldMargin.Right, oldMargin.Bottom);
				UpdateBottomLineOffset();
			}
		}
		protected void UpdateBottomLineOffset() {
			if(FocusOffset > 0 && bottomLine!= null) {
				if(rowData.IsRowExpanded)
					bottomLine.Margin = new Thickness(CalcFullOffset(), bottomLine.Margin.Top, bottomLine.Margin.Right, bottomLine.Margin.Bottom);
				else
					bottomLine.Margin = new Thickness(0, bottomLine.Margin.Top, bottomLine.Margin.Right, bottomLine.Margin.Bottom);
			}
		}
		#endregion
		#region Indicator
		void UpdateIndicator() {
			if(ViewTable != null) {
				indicator = new GroupRowIndicator();
				UpdateIndicatorWidth();
				if(!ViewTable.ActualShowIndicator)
					UpdateIndicatorVisibility();
				if(rowData.IndicatorState != IndicatorState.None)
					UpdateIndicatorState();
				AddPanelElement(indicator, IndicatorPosition);
			}
		}
		void UpdateIndicatorWidth() {
			if(indicator != null && ViewTable != null)
				indicator.Width = ViewTable.ActualIndicatorWidth;
		}
		void UpdateIndicatorVisibility() {
			if(indicator != null && ViewTable != null)
				indicator.Visibility = ViewTable.ActualShowIndicator ? Visibility.Visible : Visibility.Collapsed;
		}
		void UpdateIndicatorState() {
			if(indicator != null)
				indicator.IndicatorState = rowData.IndicatorState;
		}
		void UpdateIndicatorContentTemplate() {
			if(indicator != null)
				indicator.UpdateContent();
		}
		#endregion
		#region ExpandButton
		void UpdateGroupExpandButton() {
			if(groupExpandButton == null && layoutPanel != null) {
				groupExpandButton = new GroupRowExpandButton();
				groupExpandButton.Command = new DelegateCommand(RowExpandedCommand);
				UpdateRowIsExpanded();
				AddPanelElement(groupExpandButton, GroupExpandButtonPosition);
			}
		}
		void RowExpandedCommand() {
			GridCommands.ChangeGroupExpanded.Execute(rowData.RowHandle.Value, View);
		}
		protected void UpdateRowIsExpanded() {
			if(groupExpandButton != null)
				groupExpandButton.IsChecked = rowData.IsRowExpanded;
			if(LogicalItemsContainer != null)
				LogicalItemsContainer.AnimationProgress = rowData.IsRowExpanded ? 1 : 0;
			UpdateBottomLineOffset();
		}
		#endregion
		#region CheckBoxSelector
		void UpdateCheckBoxSelector() {
			if(checkBoxSelector == null && layoutPanel != null && View.ActualShowCheckBoxSelectorInGroupRow) {
				checkBoxSelector = new GroupRowCheckBoxSelector();
				AddPanelElement(checkBoxSelector, CheckBoxSelectorPosition);
			}
			if(checkBoxSelector != null)
				checkBoxSelector.Visibility = View.ActualShowCheckBoxSelectorInGroupRow ? Visibility.Visible : Visibility.Collapsed;
		}
		#endregion
		#region GroupValuePresenter
		void UpdateGroupValuePresenter() {
			bool useTemplate = UseGroupValueTemplate();
			if(useTemplate != groupValuePresenter.UseTemplate) {
				RemovePanelElement(GroupValuePresenterPosition);
				if(useTemplate)
					groupValuePresenter = new GroupValueContentPresenter();
				else
					groupValuePresenter = new GroupValuePresenter();
				AddPanelElement(groupValuePresenter.Element, GroupValuePresenterPosition);
			}
			UpdateGroupValuePresenterContent();
			UpdateGroupValuePresenterTemplateSelector();
		}
		void UpdateGroupValuePresenterContent() {
			groupValuePresenter.ValueData = rowData.GroupValue;
		}
		void UpdateGroupValuePresenterTemplateSelector() {
			if(Column != null)
				groupValuePresenter.ContentTemplateSelector = Column.ActualGroupValueTemplateSelector;
		}
		bool UseGroupValueTemplate() {
			return !IsNullOrDefaultTemplate(View.GroupValueTemplate) || View.GroupValueTemplateSelector != null && !IsNullOrDefaultTemplate(View.GroupValueTemplateSelector.SelectTemplate(rowData.GroupValue, null)) ||
				Column != null && (!IsNullOrDefaultTemplate(Column.GroupValueTemplate) || Column.GroupValueTemplateSelector != null && !IsNullOrDefaultTemplate(Column.GroupValueTemplateSelector.SelectTemplate(rowData.GroupValue, null)));
		}
		static bool IsNullOrDefaultTemplate(DataTemplate template) {
			return template == null || template is DefaultDataTemplate;
		}
		#endregion
		#region GroupSummary
		void UpdateSummary() {
			if(layoutPanel == null || rowData.GroupSummaryData == null || rowData.GroupSummaryData.Count == 0) {
				RemoveDefaultSummary();
				RemoveSummaryAlignByColumns();
			} else {
				GroupSummaryDisplayMode displayMode = ViewTable != null ? ViewTable.GroupSummaryDisplayMode : GroupSummaryDisplayMode.Default;
				if(displayMode == GroupSummaryDisplayMode.Default) {
					RemoveSummaryAlignByColumns();
					UpdateGroupSummaryDefault();
				} else if(displayMode == GroupSummaryDisplayMode.AlignByColumns) {
					RemoveDefaultSummary();
					UpdateGroupSummaryAlignByColumns();
				}
			}
		}
		void UpdateGroupSummaryDefault() {
			if(summaryDefaultControl == null) {
				summaryDefaultControl = new GroupRowDefaultSummaryControl();
				AddPanelElement(summaryDefaultControl, DefaultSummaryPosition);
			}
			ActualTemplateSelectorWrapper selector = (ActualTemplateSelectorWrapper)View.ActualGroupSummaryItemTemplateSelector;
			if(IsNullOrDefaultTemplate(selector.Template) && selector.Selector == null)
				selector = null;
			summaryDefaultControl.ItemTemplateSelector = selector;
			summaryDefaultControl.ItemsSource = rowData.GroupSummaryData;
		}
		void UpdateGroupSummaryAlignByColumns() {
			if(summaryAlignByColumnsController == null) {
				summaryAlignByColumnsController = new SummaryAlignByColumnsController(View.DataControl);
				AddPanelElement(summaryAlignByColumnsController.LayoutPanel, ColumnSummaryPosition);
			}
			if(fitContent == null) {
				fitContent = new RowFitBorder();
				UpdateFitContent();
				AddPanelElement(fitContent, FitContentPosition);
			}
			summaryAlignByColumnsController.UpdateData(rowData);
			if (ViewTable != null)
				summaryAlignByColumnsController.UpdateGroupColumnSummaryItemTemplate(IsNullOrDefaultTemplate(ViewTable.GroupColumnSummaryItemTemplate) && ViewTable.GroupColumnSummaryContentStyle == null);
		}
		void RemoveDefaultSummary() {
			if(summaryDefaultControl != null) {
				RemovePanelElement(DefaultSummaryPosition);
				summaryDefaultControl = null;
			}
		}
		void RemoveSummaryAlignByColumns() {
			if(summaryAlignByColumnsController != null) {
				RemovePanelElement(ColumnSummaryPosition);
				summaryAlignByColumnsController = null;
			}
			if(fitContent != null) {
				RemovePanelElement(FitContentPosition);
				fitContent = null;
			}
		}
		void UpdateFitContent(){
			UpdateFitContentBorderBrush();
			UpdateBottomLineMargin(fitContent);							
		}
		void UpdateFitContentBorderBrush() {
			if(fitContent != null) {
				fitContent.BorderBrush = RowFitBorderBrush;
			}
		}	   
		void UpdateBottomLineMargin() {
			UpdateBottomLineMargin(backgroundBorder);		  
			UpdateBottomLineMargin(fitContent);
		}
		void UpdateBottomLineMargin(FrameworkElement element) {
			if(element != null)
				element.Margin = BottomLineMargin;
		}
		Rect oldGroupSummaryRect;
		void UpdateLayoutSummaryPanelClip(Size arrangeBounds) {
			if(summaryAlignByColumnsController == null)
				return;
			UIElement panel = summaryAlignByColumnsController.LayoutPanel;
			double leftIndent = GetLayoutSummaryPanelLeftIndent();
			Rect groupSummaryRect = new Rect(leftIndent, 0, Math.Max(0, panel.DesiredSize.Width - leftIndent), Math.Max(panel.DesiredSize.Height, arrangeBounds.Height));
			if(groupSummaryRect == oldGroupSummaryRect)
				return;
			oldGroupSummaryRect = groupSummaryRect;
			panel.Clip = new RectangleGeometry(groupSummaryRect);
			summaryAlignByColumnsController.SetLeftIndent(leftIndent);
		}
		double GetLayoutSummaryPanelLeftIndent() {
			double leftIndent = 0;
			if(offsetPresenter != null)
				leftIndent += offsetPresenter.DesiredSize.Width;
			if(groupExpandButton != null)
				leftIndent += groupExpandButton.DesiredSize.Width;
			if(groupValuePresenter.Element != null)
				leftIndent += groupValuePresenter.Element.DesiredSize.Width;
			if(checkBoxSelector != null)
				leftIndent += checkBoxSelector.DesiredSize.Width;
			return leftIndent;
		}
		void UpdateBands(FixedStyle fixedStyle) {
			if(summaryAlignByColumnsController != null)
				summaryAlignByColumnsController.UpdateBands(fixedStyle);
		}
		#endregion
		#region MasterDetail
		void UpdateLeftDetailViewIndents() {
			IList<DetailIndent> indents = rowData.DetailIndents;
			if(indents != null && detailLeftIndentControl == null && layoutPanel != null) {
				detailLeftIndentControl = new DetailRowsIndentControl();
				AddPanelElement(detailLeftIndentControl, LeftDetailIndentPosition);
			}
			if(detailLeftIndentControl != null) {
				detailLeftIndentControl.Visibility = DetailMarginVisibilityConverter.GetDetailMarginControlVisibility(indents, Side.Left);
				detailLeftIndentControl.ItemsSource = indents;
				UpdateOffsetPresenter();
			}
		}
		#endregion
		#region IGroupRow Members
		FrameworkElement IGroupRow.RowElement {
			get { return this; }
		}
		#endregion
		#region IGroupRowStateClient Members
		void IGroupRowStateClient.UpdateGroupValue() {
			UpdateGroupValuePresenterContent();
		}
		void IGroupRowStateClient.UpdateIsRowExpanded() {
			UpdateRowIsExpanded();
		}
		void IGroupRowStateClient.UpdateSummary() {
			UpdateSummary();
		}
		void IGroupRowStateClient.UpdateGroupValueTemplateSelector() {
			UpdateGroupValuePresenter();
		}
		void IGroupRowStateClient.UpdateGroupRowTemplateSelector() {
			UpdateContent();
		}
		void IGroupRowStateClient.UpdateGroupRowStyle() {
			Style newStyle = View.GroupRowStyle;
			if(newStyle is DefaultStyle)
				newStyle = null;
			if(Style != newStyle)
				Style = newStyle;
		}
		void IGroupRowStateClient.UpdateCheckBoxSelector() {
			UpdateCheckBoxSelector();
		}
		void IGroupRowStateClient.UpdateIsReady() {
			if(summaryDefaultControl != null)
				summaryDefaultControl.UpdateIsReady();
		}
		void IGroupRowStateClient.UpdateIsRowVisible() {
			if(rootPanel != null)
				rootPanel.Visibility = rowData.IsRowVisible ? Visibility.Visible : Visibility.Collapsed;
		}
		void IGroupRowStateClient.UpdateCardLayout() {
			UpdateCardLayoutChanged();		   
		}
		void IGroupRowStateClient.UpdateIsPreviewExpanded() {
			IsPreviewExpandedChanged();
		}
		#endregion
		#region IRowStateClient Members
		void IRowStateClient.InvalidateCellsPanel() {
			return;
		}
		void IRowStateClient.UpdateAlternateBackground() {
			return;
		}
		void IRowStateClient.UpdateAppearance() {
			return;
		}
		void IRowStateClient.UpdateCellsPanel() {
			if(summaryAlignByColumnsController != null)
				summaryAlignByColumnsController.UpdatePanel();
		}
		void IRowStateClient.UpdateContent() {
			return;
		}
		void IRowStateClient.UpdateDetailExpandButtonVisibility() {
			return;
		}
		void IRowStateClient.UpdateDetailViewIndents() {
			return;
		}
		void IRowStateClient.UpdateDetails() {
			UpdateBottomLineMargin();		  
		}
		void IRowStateClient.UpdateFixedLeftBands() {
			UpdateBands(FixedStyle.Left);
			if(summaryAlignByColumnsController != null)
				summaryAlignByColumnsController.UpdateFixedLeftSeparatorVisibility();
		}
		void IRowStateClient.UpdateFixedLeftCellData(IList<GridColumnData> oldValue) {
			return;
		}
		void IRowStateClient.UpdateFixedLineVisibility() {
			if(summaryAlignByColumnsController != null)
				summaryAlignByColumnsController.UpdateFixedSeparatorVisibility();
		}
		void IRowStateClient.UpdateFixedLineWidth() {
			if(summaryAlignByColumnsController != null)
				summaryAlignByColumnsController.UpdateFixedSeparatorWidth();
		}
		void IRowStateClient.UpdateFixedNoneBands() {
			UpdateBands(FixedStyle.None);
		}
		void IRowStateClient.UpdateFixedNoneCellData() {
			return;
		}
		void IRowStateClient.UpdateFixedNoneContentWidth() {
			return;
		}
		void IRowStateClient.UpdateFixedRightBands() {
			UpdateBands(FixedStyle.Right);
			if(summaryAlignByColumnsController != null)
				summaryAlignByColumnsController.UpdateFixedRightSeparatorVisibility();
		}
		void IRowStateClient.UpdateFixedRightCellData(IList<GridColumnData> oldValue) {
			return;
		}
		void IRowStateClient.UpdateFocusWithinState() {
			UpdateFadeSelection();
		}
		void IRowStateClient.UpdateHorizontalLineVisibility() {			
			UpdateBottomLineMargin();			
		}
		void IRowStateClient.UpdateIndentScrolling() {
			return;
		}
		void IRowStateClient.UpdateIndicatorContentTemplate() {
			UpdateIndicatorContentTemplate();
		}
		void IRowStateClient.UpdateIndicatorState() {
			UpdateIndicatorState();
		}
		void IRowStateClient.UpdateIndicatorWidth() {
			UpdateIndicatorWidth();
		}
		void IRowStateClient.UpdateIsFocused() {
			return;
		}
		void IRowStateClient.UpdateLevel() {
			UpdateIndicatorVisibility();
			UpdateOffsetPresenter();
			if (summaryAlignByColumnsController != null)
				summaryAlignByColumnsController.InvalidateFixedLeft();
		}
		void IRowStateClient.UpdateMinHeight() {
			return;
		}
		void IRowStateClient.UpdateRowHandle(RowHandle rowHandle) {
			DataViewBase.SetRowHandle(this, rowHandle);
		}
		void IRowStateClient.UpdateRowPosition() {
			RowPositionChange();
			return;
		}
		void IRowStateClient.UpdateRowStyle() {
			return;
		}
		void IRowStateClient.UpdateScrollingMargin() {
			if(summaryAlignByColumnsController != null && ViewTable != null)
				summaryAlignByColumnsController.SetScrollingMargin(ViewTable.ScrollingHeaderVirtualizationMargin);
		}
		void IRowStateClient.UpdateSelectionState(SelectionState selectionState) {
			SelectionState = selectionState;
			UpdateBackgroundFocusedBorderVisibility();
			UpdateFadeSelection();
			UpdateFitContent();
		}
		void IRowStateClient.UpdateShowIndicator() {
			UpdateIndicatorVisibility();
			UpdateOffsetPresenter();
		}
		void IRowStateClient.UpdateValidationError() {
			return;
		}
		void IRowStateClient.UpdateVerticalLineVisibility() {
			if(summaryAlignByColumnsController != null)
				summaryAlignByColumnsController.UpdateFixedSeparatorShowVertialLines();
		}
		void IRowStateClient.UpdateView() {
			return;
		}
		void IRowStateClient.UpdateShowRowBreak() { }
		void IRowStateClient.UpdateInlineEditForm(object editFormData) { }
		#endregion
		#region IFixedGroupElement Members
		FixedGroupElement fixedGroupElementCore;
		IFixedGroupElement FixedGroupElement {
			get {
				if(fixedGroupElementCore == null)
					fixedGroupElementCore = new FixedGroupElement(() => rowData);
				return fixedGroupElementCore;
			}
		}
		double IFixedGroupElement.GetLeftMargin(bool drawAdornerUnderWholeGroup) {
			return FixedGroupElement.GetLeftMargin(drawAdornerUnderWholeGroup);
		}
		double IFixedGroupElement.GetRightMargin(bool drawAdornerUnderWholeGroup) {
			return FixedGroupElement.GetRightMargin(drawAdornerUnderWholeGroup);
		}
		#endregion
		#region IFocusedRowBorderObject Members
		double IFocusedRowBorderObject.LeftIndent {
			get { return indicator == null ? 0d : indicator.ActualWidth; }
		}
		FrameworkElement IFocusedRowBorderObject.RowDataContent {
			get { return FocusOffset > 0 ? (FrameworkElement)backgroundBorder : this; }
		}
		#endregion
	}
}
