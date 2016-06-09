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
using System.Linq;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Grid.Printing;
using DevExpress.Xpf.Printing.Native;
using DevExpress.Xpf.Utils;
using DevExpress.XtraPrinting.DataNodes;
using System.Collections.Generic;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting;
using System.Collections;
using DevExpress.Xpf.Printing;
using System.Windows.Media;
using DevExpress.Utils;
using DevExpress.Xpf.Core.Native;
using DevExpress.Mvvm.Native;
using System.Reflection;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.ConditionalFormatting;
using Grid = System.Windows.Controls.Grid;
using System.Windows.Markup;
namespace DevExpress.Xpf.Grid {
	public static class CardViewPrintingHelper {
		#region Constants
		#endregion
		public static readonly DependencyProperty PrintCardInfoProperty;
		static readonly DependencyPropertyKey PrintCardInfoPropertyKey;
		public static readonly DependencyProperty CardViewPrintCellInfoProperty;
		static readonly DependencyPropertyKey CardViewPrintCellInfoPropertyKey;
		static CardViewPrintingHelper() {
			PrintCardInfoPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("PrintCardInfo", typeof(CardViewPrintRowInfo), typeof(CardViewPrintingHelper), new FrameworkPropertyMetadata(null));
			PrintCardInfoProperty = PrintCardInfoPropertyKey.DependencyProperty;
			CardViewPrintCellInfoPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("CardViewPrintCellInfo", typeof(CardViewPrintCellInfo), typeof(CardViewPrintingHelper), new FrameworkPropertyMetadata(null));
			CardViewPrintCellInfoProperty = CardViewPrintCellInfoPropertyKey.DependencyProperty;
		}
		public static Thickness GetActualPrintCardMargin(Thickness printCardMargin, bool isFristCard) {
			return isFristCard ? new Thickness(0, printCardMargin.Top, printCardMargin.Right, printCardMargin.Bottom) : printCardMargin;
		}
#if DEBUGTEST
		public
#else 
		internal
#endif
		static IRootDataNode CreatePrintingTreeNode(CardView view, Size usablePageSize, ItemsGenerationStrategyBase itemsGenerationStrategy = null) {
			Size pageSize = new Size(usablePageSize.Width, 0);
			CardViewPrintingDataTreeBuilder treeBuilder = view.CreatePrintingDataTreeBuilder(pageSize.Width, itemsGenerationStrategy);
			treeBuilder.View.layoutUpdatedLocker.DoLockedAction(treeBuilder.GenerateAllItems);
			DoAfterGenerateNodeTreeAction(treeBuilder);
			return new CardViewRootPrintingNode(treeBuilder, usablePageSize);
		}
#if DEBUGTEST
		public
#else 
		internal
#endif
		static void CreatePrintingTreeNodeAsync(CardView view, Size usablePageSize) {
			ItemsGenerationAsyncServerModeStrategyAsync itemsGenerationStrategy = new ItemsGenerationAsyncServerModeStrategyAsync(view);
			itemsGenerationStrategy.StartFetchingAllFilteredAndSortedRows(() => {
				IRootDataNode printingNode = CreatePrintingTreeNode(view, usablePageSize, itemsGenerationStrategy);
				view.RaiseCreateRootNodeCompleted(printingNode);
			});
		}
		static void DoAfterGenerateNodeTreeAction(CardViewPrintingDataTreeBuilder treeBuilder) {
#if DEBUGTEST
			if(CardViewPrintingDataTreeBuilder.AfterGenerateNodeTreeAction != null) {
				CardViewPrintingDataTreeBuilder.CurrentPrintingTreeBuilder = treeBuilder;
				try {
					CardViewPrintingDataTreeBuilder.AfterGenerateNodeTreeAction();
				}
				finally {
					CardViewPrintingDataTreeBuilder.CurrentPrintingTreeBuilder = null;
				}
			}
#endif
		}
		public static CardViewPrintRowInfo GetPrintCardInfo(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (CardViewPrintRowInfo)element.GetValue(PrintCardInfoProperty);
		}
		internal static void SetPrintCardInfo(DependencyObject element, CardViewPrintRowInfo value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PrintCardInfoPropertyKey, value);
		}
		public static CardViewPrintCellInfo GetCardViewPrintCellInfo(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (CardViewPrintCellInfo)element.GetValue(CardViewPrintCellInfoProperty);
		}
		internal static void SetCardViewPrintCellInfo(DependencyObject element, CardViewPrintCellInfo value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(CardViewPrintCellInfoPropertyKey, value);
		}
		public static void UpdatePageBricks(IEnumerator pageBrickEnumerator, Dictionary<IVisualBrick, IOnPageUpdater> pageBrickUpdaters, bool updateTopRowBricks, bool skipUpdateLastRowBricks) {
			if(pageBrickUpdaters.Count == 0)
				return;
			VisualBrick currentPageBrick = null;
			List<VisualBrick> allBricks = new List<VisualBrick>();
			while(pageBrickEnumerator.MoveNext()) {
				currentPageBrick = pageBrickEnumerator.Current as VisualBrick;
				if(currentPageBrick == null)
					continue;
				allBricks.Add(currentPageBrick);
				if(updateTopRowBricks) {
					TextBrick textBrick = currentPageBrick as TextBrick;
					if(textBrick != null && textBrick.Rect.Y == 0.0)
						textBrick.Sides |= BorderSide.Top;
				}
			}
			GridPrintingHelper.UpdateTopBorders(pageBrickUpdaters, allBricks);
		}
	}
	public class CardViewPrintCellInfo : PrintInfoBase {
		string displayText;
		public string DisplayText {
			get {
				return displayText;
			}
			internal set {
				if(displayText == value) return;
				displayText = value;
				OnPropertyChanged("DisplayText");
			}
		}
	}
	public class PrintTotalSummaryItem : PrintInfoBase {
		private string totalSummaryText;
		public string TotalSummaryText {
			get { return totalSummaryText; }
			set {
				if(totalSummaryText == value) return;
				totalSummaryText = value;
				OnPropertyChanged("TotalSummaryText");
			}
		}
		Style printTotalSummaryStyle;
		public Style PrintTotalSummaryStyle {
			get { return printTotalSummaryStyle; }
			set {
				if(printTotalSummaryStyle == value) return;
				printTotalSummaryStyle = value;
				OnPropertyChanged("PrintTotalSummaryStyle");
			}
		}
	}
	public class CardViewPrintRowInfo : PrintRowInfoBase {
		string totalSummaryText;
		public string TotalSummaryText {
			get { return totalSummaryText; }
			set {
				if(totalSummaryText == value) return;
				totalSummaryText = value;
				OnPropertyChanged("TotalSummaryText");
			}
		}
		Style printTotalSummarySeparatorStyle;
		public Style PrintTotalSummarySeparatorStyle {
			get { return printTotalSummarySeparatorStyle; }
			set {
				if(printTotalSummarySeparatorStyle == value) return;
				printTotalSummarySeparatorStyle = value;
				OnPropertyChanged("PrintTotalSummarySeparatorStyle");
			}
		}
		private List<PrintTotalSummaryItem> totalSummaries;
		public List<PrintTotalSummaryItem> TotalSummaries {
			get { return totalSummaries; }
			set {
				if(totalSummaries == value) return;
				totalSummaries = value;
				OnPropertyChanged("TotalSummaries");
			}
		}
		private bool fixedTotalSummaryTopBorderVisible;
		public bool FixedTotalSummaryTopBorderVisible {
			get { return fixedTotalSummaryTopBorderVisible; }
			set {
				if(fixedTotalSummaryTopBorderVisible == value) return;
				fixedTotalSummaryTopBorderVisible = value;
				OnPropertyChanged("FixedTotalSummaryTopBorderVisible");
			}
		}
		Thickness printCardMargin;
		public Thickness PrintCardMargin {
			get { return printCardMargin; }
			set {
				if(printCardMargin == value) return;
				printCardMargin = value;
				OnPropertyChanged("PrintCardMargin");
			}
		}
		double printCardWidth = Double.NaN;
		public double PrintCardWidth {
			get { return printCardWidth; }
			set {
				if(printCardWidth == value) return;
				printCardWidth = value;
				OnPropertyChanged("PrintCardWidth");
			}
		}
		int printMaximumCardColumns = -1;
		public int PrintMaximumCardColumns {
			get { return printMaximumCardColumns; }
			set {
				if(printMaximumCardColumns == value) return;
				printMaximumCardColumns = value;
				OnPropertyChanged("PrintMaximumCardColumns");
			}
		}
		bool printAutoCardWidth;
		public bool PrintAutoCardWidth {
			get { return printAutoCardWidth; }
			set {
				if(printAutoCardWidth == value) return;
				printAutoCardWidth = value;
				OnPropertyChanged("PrintAutoCardWidth");
			}
		}
		bool isGroupBottomBorderVisible;
		public bool IsGroupBottomBorderVisible {
			get { return isGroupBottomBorderVisible; }
			set {
				if(isGroupBottomBorderVisible == value) return;
				isGroupBottomBorderVisible = value;
				OnPropertyChanged("IsGroupBottomBorderVisible");
			}
		}
		private bool isPrevGroupRowCollapsed;
		public bool IsPrevGroupRowCollapsed {
			get { return isPrevGroupRowCollapsed; }
			set {
				if(isPrevGroupRowCollapsed == value) return;
				isPrevGroupRowCollapsed = value;
				OnPropertyChanged("IsPrevGroupRowCollapsed");
			}
		}
		DataTemplate printCardTemplate;
		public DataTemplate PrintCardTemplate {
			get {
				return printCardTemplate;
			}
			internal set {
				if(printCardTemplate == value) return;
				printCardTemplate = value;
				OnPropertyChanged("PrintCardTemplate");
			}
		}
		DataTemplate printCardRowIndentTemplate;
		public DataTemplate PrintCardRowIndentTemplate {
			get {
				return printCardRowIndentTemplate;
			}
			internal set {
				if(printCardRowIndentTemplate == value) return;
				printCardRowIndentTemplate = value;
				OnPropertyChanged("PrintCardRowIndentTemplate");
			}
		}
		DataTemplate printCardContentTemplate;
		public DataTemplate PrintCardContentTemplate {
			get {
				return printCardContentTemplate;
			}
			internal set {
				if(printCardContentTemplate == value) return;
				printCardContentTemplate = value;
				OnPropertyChanged("PrintCardContentTemplate");
			}
		}
		DataTemplate printCardHeaderTemplate;
		public DataTemplate PrintCardHeaderTemplate {
			get {
				return printCardHeaderTemplate;
			}
			internal set {
				if(printCardHeaderTemplate == value) return;
				printCardHeaderTemplate = value;
				OnPropertyChanged("PrintCardHeaderTemplate");
			}
		}
		double printCardsRowWidth;
		public double PrintCardsRowWidth {
			get { return printCardsRowWidth; }
			set {
				if(printCardsRowWidth == value) return;
				printCardsRowWidth = value;
				OnPropertyChanged("PrintCardsRowWidth");
			}
		}
	}
	public class CardViewPrintingCellItemsControl : CachedItemsControl {
	}
	public class PrintCardCellEditor : PrintCellEditorBase {
		protected override IBaseEdit CreateEditor(Editors.Settings.BaseEditSettings settings) {
			return settings.CreateEditor(false, EditorColumn, GetEditorOptimizationMode());
		}
		protected override bool OptimizeEditorPerformance { get { return false; } }
		protected override void InitializeBaseEdit(IBaseEdit newEdit, InplaceEditorBase.BaseEditSourceType newBaseEditSourceType) {
			base.InitializeBaseEdit(newEdit, newBaseEditSourceType);
			((BaseEdit)editCore).Style = GridPrintingHelper.GetPrintCellInfo(CellData).PrintCellStyle;
			TextEdit edit = newEdit as TextEdit;
			if(edit != null) {
				edit.TextWrapping = TextWrapping.Wrap;
				edit.PrintTextWrapping = TextWrapping.Wrap;
			}
			newEdit.ShouldDisableExcessiveUpdatesInInplaceInactiveMode = !((BaseEdit)editCore).AllowUpdateTextBlockWhenPrinting;
			if(Background != null)
				UpdateBackground();
		}
		protected override void UpdateDisplayTemplate(bool updateForce = false) { }
		protected override bool ShouldSyncCellContentPresenterProperties { get { return false; } }
	}
	public class CardViewPrintRowPanel : FrameworkElement {
		public static readonly DependencyProperty ItemTemplateProperty;
		public static readonly DependencyProperty RowDataProperty;
		public static readonly DependencyProperty RowProperty;
		public static readonly DependencyProperty RowIndentControlTemplateProperty;
		public static readonly DependencyProperty IsFirstCardInRowProperty;
		public static readonly DependencyProperty PrintMaximumCardColumnsProperty;
		public static readonly DependencyProperty PrintAutoCardWidthProperty;
		static CardViewPrintRowPanel() {
			ItemTemplateProperty = DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(CardViewPrintRowPanel), new PropertyMetadata(null));
			RowDataProperty = DependencyProperty.Register("RowData", typeof(RowData), typeof(CardViewPrintRowPanel), new PropertyMetadata(null));
			RowProperty = DependencyProperty.Register("Row", typeof(object), typeof(CardViewPrintRowPanel), new PropertyMetadata(null, (d, e) => ((CardViewPrintRowPanel)d).OnRowChanged()));
			RowIndentControlTemplateProperty = DependencyProperty.Register("RowIndentControlTemplate", typeof(DataTemplate), typeof(CardViewPrintRowPanel), new PropertyMetadata(null));
			IsFirstCardInRowProperty = DependencyProperty.RegisterAttached("IsFirstCardInRow", typeof(bool), typeof(CardViewPrintRowPanel), new PropertyMetadata(false));
			PrintMaximumCardColumnsProperty = DependencyProperty.RegisterAttached("PrintMaximumCardColumns", typeof(int), typeof(CardViewPrintRowPanel), new PropertyMetadata(-1));
			PrintAutoCardWidthProperty = DependencyProperty.RegisterAttached("PrintAutoCardWidth", typeof(bool), typeof(CardViewPrintRowPanel), new PropertyMetadata(false));
		}
		void OnRowChanged() {
			InvalidateMeasure();
		}
		public static bool GetIsFirstCardInRow(DependencyObject obj) {
			return (bool)obj.GetValue(IsFirstCardInRowProperty);
		}
		public static void SetIsFirstCardInRow(DependencyObject obj, bool value) {
			obj.SetValue(IsFirstCardInRowProperty, value);
		}
		readonly List<ContentControl> VisualChildrenCache;
		readonly List<ContentControl> VisualChildren;
		ContentControl RowIndentControl;
		public CardViewPrintRowPanel() {
			VisualChildrenCache = new List<ContentControl>();
			VisualChildren = new List<ContentControl>();
		}
		public RowData RowData {
			get { return (RowData)GetValue(RowDataProperty); }
			set { SetValue(RowDataProperty, value); }
		}
		public object Row {
			get { return (object)GetValue(RowProperty); }
			set { SetValue(RowProperty, value); }
		}
		public DataTemplate ItemTemplate {
			get { return (DataTemplate)GetValue(ItemTemplateProperty); }
			set { SetValue(ItemTemplateProperty, value); }
		}
		public DataTemplate RowIndentControlTemplate {
			get { return (DataTemplate)GetValue(RowIndentControlTemplateProperty); }
			set { SetValue(RowIndentControlTemplateProperty, value); }
		}
		public int PrintMaximumCardColumns {
			get { return (int)GetValue(PrintMaximumCardColumnsProperty); }
			set { SetValue(PrintMaximumCardColumnsProperty, value); }
		}
		public bool PrintAutoCardWidth {
			get { return (bool)GetValue(PrintAutoCardWidthProperty); }
			set { SetValue(PrintAutoCardWidthProperty, value); }
		}
		protected override int VisualChildrenCount { get { return VisualChildren.Count == 0 ? 0 : VisualChildren.Count + 1; } }
		protected override Visual GetVisualChild(int index) {
			if(index == 0) return RowIndentControl;
			return VisualChildren[index - 1];
		}
		RowData GetNextRowData() {
			CardViewPrintingDataTreeBuilder treeBuilder = (CardViewPrintingDataTreeBuilder)RowData.treeBuilder;
			if(LastPrintRowData == null) {
				return CreateCardData(treeBuilder, RowData.node);
			}
			int index = treeBuilder.AllNodes.IndexOf((DataRowNode)LastPrintRowData.node) + 1;
			if(index >= treeBuilder.AllNodes.Count)
				return null;
			DataRowNode node = treeBuilder.AllNodes[index];
			GroupNode groupNode = node as GroupNode;
			if(groupNode != null) {
				UpdateAfterGroupRowHandle(treeBuilder, index);
				return null;
			}
			return CreateCardData(treeBuilder, node);
		}
		CardData CreateCardData(CardViewPrintingDataTreeBuilder treeBuilder, RowNode node) {
			RowDataBase rowDataBase = node.CreateRowData();
			rowDataBase.AssignFromInternal(null, treeBuilder.ReusingRowData.parentNodeContainer, node, false);
			treeBuilder.UpdateRowData((RowData)rowDataBase);
			return (CardData)rowDataBase;
		}
		void UpdateAfterGroupRowHandle(CardViewPrintingDataTreeBuilder treeBuilder, int currentIndex) {
			for(int i = currentIndex + 1; i < treeBuilder.AllNodes.Count; i++) {
				DataRowNode rowNode = treeBuilder.AllNodes[i] as DataRowNode;
				GroupNode groupNode = rowNode as GroupNode;
				if(groupNode != null)
					continue;
				afterGroupRowHandle = rowNode.RowHandle.Value;
				return;
			}
		}
		int afterGroupRowHandle = int.MaxValue;
		int lastRowHandle = int.MaxValue;
		RowData LastPrintRowData = null;
		Size RemergeCurrentRow(Size availableSize) {
			Size rowIndentSize = GetRowIndentControlSize(availableSize);
			Size freeSize = new Size(Math.Max(0, availableSize.Width - rowIndentSize.Width), availableSize.Height);
			availableSize = new Size(freeSize.Width, availableSize.Height);
			Size returnSize = new Size(availableSize.Width, Double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height);
			int i = 0;
			int maximum = PrintMaximumCardColumns > 0 ? PrintMaximumCardColumns : int.MaxValue;
			while(true) {
				if(i == VisualChildren.Count) {
					RowData data = GetNextRowData();
					if(data == null)
						break;
					AddVisualChildFromCache();
					ContentControl newElement = VisualChildren[i];
					newElement.Content = data;
				}
				ContentControl visualElement =  VisualChildren[i];
				RowData rowData = (RowData)visualElement.Content;
				SetIsFirstCardInRow(rowData, i == 0);
				visualElement.Measure(availableSize);
				if((visualElement.DesiredSize.Width > freeSize.Width && i > 0) || i == maximum) {
					LastPrintRowData = (RowData)VisualChildren[i - 1].Content;
					PutVisualChildInCache(visualElement);
					break;
				}
				freeSize = new Size(Math.Max(0, freeSize.Width - visualElement.DesiredSize.Width), freeSize.Height);
				returnSize = new Size(returnSize.Width, Math.Max(returnSize.Height, visualElement.DesiredSize.Height));
				LastPrintRowData = rowData;
				i++;
			}
			int generatedRowsCount = i;
			int removeElementsCount = VisualChildren.Count - i;
			while(removeElementsCount > 0) {
				PutVisualChildInCache(VisualChildren.Last());
				removeElementsCount--;
			}
			return returnSize;
		}
		Size GetRowIndentControlSize(Size availableSize) {
			if(RowIndentControl == null) {
				RowIndentControl = new ContentControl();
				RowIndentControl.ContentTemplate = RowIndentControlTemplate;
				AddVisualChild(RowIndentControl);
				AddLogicalChild(RowIndentControl);
			}
			RowIndentControl.Content = DataContext;
			RowIndentControl.Measure(availableSize);
			return RowIndentControl.DesiredSize;
		}
		protected override Size MeasureOverride(Size availableSize) {
			if(RowData.RowHandle.Value == lastRowHandle)
				return RemergeCurrentRow(availableSize);
			if(RowData.RowHandle.Value == afterGroupRowHandle)
				LastPrintRowData = null;
			lastRowHandle = RowData.RowHandle.Value;
			Size rowIndentSize = GetRowIndentControlSize(availableSize);
			Size freeSize = new Size(Math.Max(0, availableSize.Width - rowIndentSize.Width), availableSize.Height);
			availableSize = new Size(freeSize.Width, availableSize.Height);
			Size returnSize = new Size(availableSize.Width, Double.IsInfinity(availableSize.Height) ? 0 : availableSize.Height);
			int i = 0;
			int maximum = PrintMaximumCardColumns > 0 ? PrintMaximumCardColumns : int.MaxValue;
			while(true) {
				RowData data = GetNextRowData();
				if(data == null)
					break;
				SetIsFirstCardInRow(data, i == 0);
				if(i == VisualChildren.Count)
					AddVisualChildFromCache();
				ContentControl visualElement = VisualChildren[i];
				visualElement.Content = data;
				visualElement.Measure(availableSize);
				if((visualElement.DesiredSize.Width > freeSize.Width && i > 0) || i == maximum) {
					PutVisualChildInCache(visualElement);
					break;
				}
				freeSize = new Size(Math.Max(0, freeSize.Width - visualElement.DesiredSize.Width), freeSize.Height);
				returnSize = new Size(returnSize.Width, Math.Max(returnSize.Height, visualElement.DesiredSize.Height));
				LastPrintRowData = data;
				i++;
			}
			int generatedRowsCount = i;
			int removeElementsCount = VisualChildren.Count - i;
			while(removeElementsCount > 0) {
				PutVisualChildInCache(VisualChildren.Last());
				removeElementsCount--;
			}
			return returnSize;
		}
		protected override Size ArrangeOverride(Size finalSize) {
			if(VisualChildren.Count == 0)
				return finalSize;
			RowIndentControl.Arrange(new Rect(0, 0, RowIndentControl.DesiredSize.Width, RowIndentControl.DesiredSize.Height));
			double x = RowIndentControl.DesiredSize.Width;
			foreach(ContentControl presenter in VisualChildren) {
				presenter.Arrange(new Rect(x, 0, presenter.DesiredSize.Width, presenter.DesiredSize.Height));
				x += presenter.DesiredSize.Width;
			}
			return finalSize;
		}
		void PutVisualChildInCache(ContentControl control) {
			VisualChildrenCache.Add(control);
			RemoveVisualChild(control);
			VisualChildren.Remove(control);
			RemoveLogicalChild(control);
			control.Content = null;
		}
		void AddVisualChildFromCache() {
			ContentControl control = null;
			if(VisualChildrenCache.Count > 0) {
				control = VisualChildrenCache[0];
				VisualChildrenCache.RemoveAt(0);
				VisualChildren.Add(control);
				AddVisualChild(control);
				AddLogicalChild(control);
			}
			else {
				control = new ContentControl();
				control.ContentTemplate = ItemTemplate;
				VisualChildren.Add(control);
				AddVisualChild(control);
				AddLogicalChild(control);
			}
		}
	}
	public class GroupRowOuterMarginConverter : IMultiValueConverter {
		public Thickness ExpandedMargin { get; set; }
		public Thickness ExpandedIsLastMargin { get; set; }
		public Thickness CollapsedMargin { get; set; }
		public Thickness CollapsedIsLastMargin { get; set; }
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if((values.Length != 2) || !(values[0] is bool) || !(values[1] is bool))
				return default(Thickness);
			bool isLast = (bool)values[0];
			bool isExpanded = (bool)values[1];
			if(isLast && isExpanded) return ExpandedIsLastMargin;
			if(!isLast && isExpanded) return ExpandedMargin;
			if(isLast && !isExpanded) return CollapsedIsLastMargin;
			if(!isLast && !isExpanded) return CollapsedMargin;
			return default(Thickness);
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class CardViewPrintFixedTotalSummaryBorderConverter : IMultiValueConverter {
		public bool IsLeft { get; set; }
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(values.Length != 2 || !(values[0] is string) || !(values[1] is bool))
				return default(Thickness);
			string summaryText = values[0].ToString();
			bool topBorderVisible = (bool)values[1];
			switch(IsLeft) {
				case true: return new Thickness(1, topBorderVisible ? 1 : 0, String.IsNullOrWhiteSpace(summaryText) ? 1 : 0, 1);
				case false:
				default: return new Thickness(String.IsNullOrWhiteSpace(summaryText) ? 1 : 0, topBorderVisible ? 1 : 0, 1, 1);
			}
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PrintCardMarginConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(values.Length != 2 || !(values[0] is Thickness) || !(values[1] is bool))
				return default(Thickness);
			Thickness printCardMargin = (Thickness)values[0];
			bool isFristCard = (bool)values[1];
			return CardViewPrintingHelper.GetActualPrintCardMargin(printCardMargin, isFristCard);
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class CardViewPrintGroupPositionToBorderConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(values.Length != 3 || !(values[0] is PrintGroupRowCellInfo) || !(values[1] is bool) || !(values[2] is bool))
				return new Thickness(0);
			PrintGroupRowCellInfo info = (PrintGroupRowCellInfo)values[0];
			bool isPrevGroupRowCollapsed = (bool)values[1];
			bool isGroupBottomBorderVisible = (bool)values[2];
			int bottomBorder = isGroupBottomBorderVisible ? 1 : 0;
			int topBorder = isPrevGroupRowCollapsed ? 0 : 1; 
			int leftBound = info.IsLeftGroupSummaryValueEmpty ? 1 : 0;
			int rightBound = info.IsRightGroupSummaryValueEmpty ? 1 : 0;
			switch(info.Position) {
				case PrintGroupCellPosition.Default: return new Thickness(leftBound, topBorder, rightBound, bottomBorder);
				case PrintGroupCellPosition.Last: return new Thickness(leftBound, topBorder, 1, bottomBorder);
				case PrintGroupCellPosition.None: return new Thickness(0, topBorder, 0, bottomBorder);
				case PrintGroupCellPosition.Separator: return new Thickness(0, topBorder, rightBound, bottomBorder);
			}
			return null;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class CardPrintCellContentPresenter : CardCellContentPresenter { }
}
