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
#if SL
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Editors.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility;
#endif
namespace DevExpress.Xpf.Grid {
	public static class GridPrintingHelper {
		public const double GroupIndent = 20d;
		public const double DefaultDetailTopIndent = 4d;
		public const double DefaultDetailBottomIndent = 10d;
		public const bool DefaultAllowPrintDetails = true;
		public const bool DefaultAllowPrintEmptyDetails = false;
		public const bool DefaultPrintAllDetails = false;
		public static readonly DependencyProperty PrintColumnWidthProperty;
		static readonly DependencyPropertyKey PrintColumnWidthPropertyKey;
		public static readonly DependencyProperty PrintGroupRowInfoProperty;
		static readonly DependencyPropertyKey PrintGroupRowInfoPropertyKey;
		public static readonly DependencyProperty PrintGroupSummaryInfoProperty;
		static readonly DependencyPropertyKey PrintGroupSummaryInfoPropertyKey;
		public static readonly DependencyProperty PrintFixedFooterTextLeftProperty;
		static readonly DependencyPropertyKey PrintFixedFooterTextLeftPropertyKey;
		public static readonly DependencyProperty PrintFixedFooterTextRightProperty;
		static readonly DependencyPropertyKey PrintFixedFooterTextRightPropertyKey;
		public static readonly DependencyProperty PrintCellInfoProperty;
		static readonly DependencyPropertyKey PrintCellInfoPropertyKey;
		public static readonly DependencyProperty PrintRowInfoProperty;
		static readonly DependencyPropertyKey PrintRowInfoPropertyKey;
		public static readonly DependencyProperty PrintHasLeftSiblingProperty;
		static readonly DependencyPropertyKey PrintHasLeftSiblingPropertyKey;
		public static readonly DependencyProperty PrintHasRightSiblingProperty;
		static readonly DependencyPropertyKey PrintHasRightSiblingPropertyKey;
		public static readonly DependencyProperty PrintColumnPositionProperty;
		static readonly DependencyPropertyKey PrintColumnPositionPropertyKey;
		public static readonly DependencyProperty PrintBandInfoProperty;
		static readonly DependencyPropertyKey PrintBandInfoPropertyKey;
		static GridPrintingHelper() {
			PrintColumnWidthPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("PrintColumnWidth", typeof(double), typeof(GridPrintingHelper), new FrameworkPropertyMetadata(0d, null, new CoerceValueCallback(CoercePrintColumnWidth)));
			PrintColumnWidthProperty = PrintColumnWidthPropertyKey.DependencyProperty;
			PrintGroupRowInfoPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("PrintGroupRowInfo", typeof(PrintGroupRowInfo), typeof(GridPrintingHelper), new FrameworkPropertyMetadata(null));
			PrintGroupRowInfoProperty = PrintGroupRowInfoPropertyKey.DependencyProperty;
			PrintGroupSummaryInfoPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("PrintGroupSummaryInfo", typeof(PrintGroupSummaryInfo), typeof(GridPrintingHelper), new FrameworkPropertyMetadata(null));
			PrintGroupSummaryInfoProperty = PrintGroupSummaryInfoPropertyKey.DependencyProperty;
			PrintFixedFooterTextLeftPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("PrintFixedFooterTextLeft", typeof(string), typeof(GridPrintingHelper), new FrameworkPropertyMetadata(null));
			PrintFixedFooterTextLeftProperty = PrintFixedFooterTextLeftPropertyKey.DependencyProperty;
			PrintFixedFooterTextRightPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("PrintFixedFooterTextRight", typeof(string), typeof(GridPrintingHelper), new FrameworkPropertyMetadata(null));
			PrintFixedFooterTextRightProperty = PrintFixedFooterTextRightPropertyKey.DependencyProperty;
			PrintCellInfoPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("PrintCellInfo", typeof(PrintCellInfo), typeof(GridPrintingHelper), new FrameworkPropertyMetadata(null));
			PrintCellInfoProperty = PrintCellInfoPropertyKey.DependencyProperty;
			PrintRowInfoPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("PrintRowInfo", typeof(PrintRowInfo), typeof(GridPrintingHelper), new FrameworkPropertyMetadata(null));
			PrintRowInfoProperty = PrintRowInfoPropertyKey.DependencyProperty;
			PrintColumnPositionPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("PrintColumnPosition", typeof(ColumnPosition), typeof(GridPrintingHelper), new FrameworkPropertyMetadata(ColumnPosition.Standalone));
			PrintColumnPositionProperty = PrintColumnPositionPropertyKey.DependencyProperty;
			PrintHasLeftSiblingPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("PrintHasLeftSibling", typeof(bool), typeof(GridPrintingHelper), new FrameworkPropertyMetadata(false));
			PrintHasLeftSiblingProperty = PrintHasLeftSiblingPropertyKey.DependencyProperty;
			PrintHasRightSiblingPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("PrintHasRightSibling", typeof(bool), typeof(GridPrintingHelper), new FrameworkPropertyMetadata(false));
			PrintHasRightSiblingProperty = PrintHasRightSiblingPropertyKey.DependencyProperty;
			PrintBandInfoPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("PrintBandInfo", typeof(PrintBandInfo), typeof(GridPrintingHelper), new FrameworkPropertyMetadata(null));
			PrintBandInfoProperty = PrintBandInfoPropertyKey.DependencyProperty;
		}
		public static PrintRowInfo GetPrintRowInfo(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (PrintRowInfo)element.GetValue(PrintRowInfoProperty);
		}
		internal static void SetPrintRowInfo(DependencyObject element, PrintRowInfo value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PrintRowInfoPropertyKey, value);
		}
		public static PrintCellInfo GetPrintCellInfo(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (PrintCellInfo)element.GetValue(PrintCellInfoProperty);
		}
		internal static void SetPrintCellInfo(DependencyObject element, PrintCellInfo value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PrintCellInfoPropertyKey, value);
		}
		public static PrintGroupRowInfo GetPrintGroupRowInfo(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (PrintGroupRowInfo)element.GetValue(PrintGroupRowInfoProperty);
		}
		public static PrintGroupSummaryInfo GetPrintGroupSummaryInfo(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (PrintGroupSummaryInfo)element.GetValue(PrintGroupSummaryInfoProperty);
		}
		public static string GetPrintFixedFooterTextLeft(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (string)element.GetValue(PrintFixedFooterTextLeftProperty);
		}
		public static string GetPrintFixedFooterTextRight(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (string)element.GetValue(PrintFixedFooterTextRightProperty);
		}
		public static PrintBandInfo GetPrintBandInfo(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (PrintBandInfo)element.GetValue(PrintBandInfoProperty);
		}
		internal static void SetPrintGroupRowInfo(DependencyObject element, PrintGroupRowInfo value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PrintGroupRowInfoPropertyKey, value);
		}
		internal static void SetPrintGroupSummaryInfo(DependencyObject element, PrintGroupSummaryInfo value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PrintGroupSummaryInfoPropertyKey, value);
		}
		internal static void SetPrintFixedFooterTextLeft(DependencyObject element, string value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PrintFixedFooterTextLeftPropertyKey, value);
		}
		internal static void SetPrintFixedFooterTextRight(DependencyObject element, string value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PrintFixedFooterTextRightPropertyKey, value);
		}
		internal static void SetPrintBandInfo(DependencyObject element, PrintBandInfo value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PrintBandInfoPropertyKey, value);
		}
		static object CoercePrintColumnWidth(DependencyObject d, object value) {
			double width = (double)value;
			if(width < 0.0) width = 0.0;
			return width;
		}
		public static double GetPrintColumnWidth(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (double)element.GetValue(PrintColumnWidthProperty);
		}
		internal static void SetPrintColumnWidth(DependencyObject element, double value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PrintColumnWidthPropertyKey, value);
		}
		public static bool GetPrintHasLeftSibling(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(PrintHasLeftSiblingProperty);
		}
		internal static void SetPrintHasLeftSibling(DependencyObject element, bool value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PrintHasLeftSiblingPropertyKey, value);
		}
		public static bool GetPrintHasRightSibling(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(PrintHasRightSiblingProperty);
		}
		internal static void SetPrintHasRightSibling(DependencyObject element, bool value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PrintHasRightSiblingPropertyKey, value);
		}
		public static ColumnPosition GetPrintColumnPosition(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (ColumnPosition)element.GetValue(PrintColumnPositionProperty);
		}
		internal static void SetPrintColumnPosition(DependencyObject element, ColumnPosition value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(PrintColumnPositionPropertyKey, value);
		}
		static void CalcPrintColumnsAutoWidthLayout(ITableView view, Size pageSize) {
			view.TableViewBehavior.CreatePrintViewInfo().CreateColumnsLayoutCalculator(true).CalcActualLayout(pageSize, PrintLayoutAssigner.Printing, false, false, true);
		}
		static void CalcPrintColumnsBandsLayout(ITableView view, Size pageSize, BandsLayoutBase bandsLayout) {
			view.TableViewBehavior.CreatePrintViewInfo(bandsLayout).CreateColumnsLayoutCalculator(view.PrintAutoWidth).CalcActualLayout(pageSize, PrintLayoutAssigner.Printing, false, false, true);
		}
		static void CalcPrintColumnWidths(ITableView view) {
			foreach(ColumnBase column in view.ViewBase.VisibleColumnsCore)
				GridPrintingHelper.SetPrintColumnWidth(column, column.ActualHeaderWidth);
		}
		static double CalcTotalPrintHeaderWidth(ITableView view, BandsLayoutBase bandsLayout) {
			double width = 0.0;
			if(bandsLayout == null) {
				foreach(ColumnBase column in view.ViewBase.PrintableColumns)
					width += GridPrintingHelper.GetPrintColumnWidth(column);
			} else {
				foreach(BandBase band in bandsLayout.VisibleBands)
					width += GridPrintingHelper.GetPrintColumnWidth(band);
			}
			return width;
		}
		public static void UpdatePageBricks(IEnumerator pageBrickEnumerator, Dictionary<IVisualBrick, IOnPageUpdater> pageBrickUpdaters, bool updateTopRowBricks, bool skipUpdateLastRowBricks) {
			if(pageBrickUpdaters.Count == 0)
				return;
			Dictionary<int, List<VisualBrick>> printedBricksGroupedByBand = new Dictionary<int, List<VisualBrick>>();
			VisualBrick currentPageBrick = null;
			double maxBrickY = Double.MinValue;
			List<VisualBrick> allBricks = new List<VisualBrick>();
			while(pageBrickEnumerator.MoveNext()) {
				currentPageBrick = pageBrickEnumerator.Current as VisualBrick;
				if(currentPageBrick == null)
					continue;
				allBricks.Add(currentPageBrick);
				int parentId = default(int);
				if(!currentPageBrick.TryGetAttachedValue<int>(BrickAttachedProperties.ParentID, out parentId))
					continue;
				if(!printedBricksGroupedByBand.ContainsKey(parentId))
					printedBricksGroupedByBand[parentId] = new List<VisualBrick>();
				printedBricksGroupedByBand[parentId].Add(currentPageBrick);
				if(updateTopRowBricks) {
					TextBrick textBrick = currentPageBrick as TextBrick;
					if(textBrick != null && textBrick.Rect.Y == 0.0)
						textBrick.Sides |= BorderSide.Top;
				}
				if(currentPageBrick.Rect.Y > maxBrickY)
					maxBrickY = currentPageBrick.Rect.Y;
			}
			UpdateTopBorders(pageBrickUpdaters, allBricks);
			if(currentPageBrick == null)
				return;
			IVisualBrick lastBrick = pageBrickUpdaters.Keys.Last<IVisualBrick>();
			IEnumerable<List<VisualBrick>> reversedPageBandCollection = printedBricksGroupedByBand.Values.Reverse<List<VisualBrick>>();
			foreach(var bandBricks in reversedPageBandCollection) {
				foreach(VisualBrick visualBrick in bandBricks) {
					IOnPageUpdater updater;
					if(skipUpdateLastRowBricks && lastBrick == visualBrick && lastBrick.Rect.Y != currentPageBrick.Rect.Y)
						continue;
					if(pageBrickUpdaters.TryGetValue(visualBrick, out updater) && updater is LastOnPageUpdater) {
						if(visualBrick.Rect.Bottom < maxBrickY || Math.Abs(visualBrick.Rect.Bottom - maxBrickY) <= 0.01)
							return;
						updater.Update(visualBrick);
						return;
					}
				}
			}
			return;
		}
		public static void UpdateTopBorders(Dictionary<IVisualBrick, IOnPageUpdater> pageBrickUpdaters, List<VisualBrick> bricks) {
			foreach(KeyValuePair<IVisualBrick, IOnPageUpdater> kvp in pageBrickUpdaters) {
				VisualBrick vb = kvp.Key as VisualBrick;
				IOnPageUpdater footerRowUpdater = kvp.Value as FooterRowTobBorgerOnPageUpdater;
				if(footerRowUpdater != null) {
					for(int i = 0; i < bricks.Count; i++) {
						if(bricks[i] == vb) {
							float y = bricks[i].Rect.Y;
							UpdateNeedTopRowIfNeed(vb, footerRowUpdater, bricks, pageBrickUpdaters, y);
							break;
						}
						if(bricks[i] is PanelBrick) {
							float y = bricks[i].Rect.Y;
							foreach(VisualBrick brick in ((PanelBrick)bricks[i]).Bricks) {
								if(brick == vb) {
									UpdateNeedTopRowIfNeed(vb, footerRowUpdater, bricks, pageBrickUpdaters, y);
									break;
								}
							}
						}
					}
					continue;
				}
				IOnPageUpdater topBorderUpdater = kvp.Value as TopBorderOnPageUpdater;
				if(topBorderUpdater == null)
					continue;
				topBorderUpdater.Update(vb);
			}
		}
		static void UpdateNeedTopRowIfNeed(VisualBrick vb, IOnPageUpdater footerRowUpdater, List<VisualBrick> bricks, Dictionary<IVisualBrick, IOnPageUpdater> pageBrickUpdaters, float y) {
			List<VisualBrick> prevBricks = GetAllBricks(bricks.Where(brick => IsPrevBrick(brick, y)).ToList());
			if(prevBricks.Count == 0)
				return;
			int currentDetailLevel = ((InfoProviderOnPageUpdaterBase)footerRowUpdater).DetailLevel;
			foreach(VisualBrick brick in prevBricks) {
				IOnPageUpdater updater = null;
				if(!pageBrickUpdaters.TryGetValue(brick, out updater))
					continue;
				InfoProviderOnPageUpdaterBase rowUpdater = updater as InfoProviderOnPageUpdaterBase;
				if(rowUpdater == null)
					continue;
				if(rowUpdater.DetailLevel <= currentDetailLevel)
					continue;
				footerRowUpdater.Update(vb);
				return;
			}
		}
#if DEBUGTEST
		public class Debug_VisualBrick : VisualBrick {
			public static int HashCoreFireCount = 0;
			public static bool UseDebugBricks = false;
			VisualBrick source;
			public Debug_VisualBrick(VisualBrick brick) {
				source = brick;
			}
			public override int GetHashCode() {
				HashCoreFireCount++;
				return source.GetHashCode();
			}
			public override bool Equals(object obj) {
				HashCoreFireCount++;
				return source.Equals(obj);
			}
		}
#endif
		static VisualBrick GetBrick(VisualBrick brick) {
#if DEBUGTEST
			if(!Debug_VisualBrick.UseDebugBricks)
				return brick;
			return new Debug_VisualBrick(brick);
#else
			return brick;
#endif
		}
		static List<VisualBrick> GetAllBricks(List<VisualBrick> bricks) {
			List<VisualBrick> result = new List<VisualBrick>();
			foreach(VisualBrick brick in bricks) {
				result.Add(GetBrick(brick));
				PanelBrick panelBrick = brick as PanelBrick;
				if(panelBrick == null)
					continue;
				foreach(VisualBrick childBrick in panelBrick.Bricks)
					result.Add(GetBrick(childBrick));
			}
			return result;
		}
		static bool IsPrevBrick(VisualBrick brick, float y) {
			return Math.Floor(brick.Rect.Y + brick.Rect.Height) == Math.Floor(y);
		}
#if DEBUGTEST
		public
#else 
			internal
#endif
 static IRootDataNode CreatePrintingTreeNode(ITableView view, Size usablePageSize, MasterDetailPrintInfo masterDetailPrintInfo = null, ItemsGenerationStrategyBase itemsGenerationStrategy = null) {
			Size pageSize = new Size(usablePageSize.Width, 0);
			BandsLayoutBase bandsLayout = view.ViewBase.DataControl.BandsLayoutCore.Return(layout => layout.CloneAndFillEmptyBands(), null);
			CalcPrintLayout(view, pageSize, bandsLayout);
			double totalHeaderWidth = CalcTotalPrintHeaderWidth(view, bandsLayout);
			PrintingDataTreeBuilderBase treeBuilder = ((DataViewBase)view).CreatePrintingDataTreeBuilder(totalHeaderWidth, itemsGenerationStrategy, masterDetailPrintInfo, bandsLayout) as PrintingDataTreeBuilderBase;
			treeBuilder.View.layoutUpdatedLocker.DoLockedAction(treeBuilder.GenerateAllItems);
			DoAfterGenerateNodeTreeAction(treeBuilder);
			return new GridRootPrintingNode(treeBuilder, usablePageSize);
		}
		static void CalcPrintLayout(ITableView view, Size pageSize, BandsLayoutBase bandsLayout) {
			if(bandsLayout != null) {
				CalcPrintColumnsBandsLayout(view, pageSize, bandsLayout);
			}
			else if(view.PrintAutoWidth) {
				CalcPrintColumnsAutoWidthLayout(view, pageSize);
			}
			else {
				CalcPrintColumnWidths(view);
			}
		}
		static void DoAfterGenerateNodeTreeAction(PrintingDataTreeBuilderBase treeBuilder) {
#if DEBUGTEST
			if(PrintingDataTreeBuilder.AfterGenerateNodeTreeAction != null) {
				PrintingDataTreeBuilder.CurrentPrintingTreeBuilder = treeBuilder;
				try {
					PrintingDataTreeBuilder.AfterGenerateNodeTreeAction();
				}
				finally {
					PrintingDataTreeBuilder.CurrentPrintingTreeBuilder = null;
				}
			}
#endif
		}
#if DEBUGTEST
		public
#else
		internal
#endif
		static void CreatePrintingTreeNodeAsync(TableView view, Size usablePageSize, MasterDetailPrintInfo masterDetailPrintInfo = null) {
			ItemsGenerationAsyncServerModeStrategyAsync itemsGenerationStrategy = new ItemsGenerationAsyncServerModeStrategyAsync(view);
			itemsGenerationStrategy.StartFetchingAllFilteredAndSortedRows(() => {
				IRootDataNode printingNode = CreatePrintingTreeNode(view, usablePageSize, masterDetailPrintInfo, itemsGenerationStrategy);
				view.RaiseCreateRootNodeCompleted(printingNode);
			});
		}
	}
	public class PrintCellEditorBase : CellEditor {
#if !SL
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty BackgroundProperty = DependencyProperty.Register("Background", typeof(Brush), typeof(PrintCellEditorBase), new PropertyMetadata(null, (d, e) => ((PrintCellEditor)d).UpdateBackground()));
		internal Brush Background { get { return (Brush)GetValue(BackgroundProperty); } }
		protected void UpdateBackground() {
			(editCore as BaseEdit).Do(x => x.Background = (Brush)GetValue(BackgroundProperty));
		}
#endif
	}
	public class PrintCellEditor : PrintCellEditorBase
#if !SL
		, IConditionalFormattingClient<PrintCellEditor> 
#endif
		{
		public static readonly DependencyProperty IsTopBorderVisibleProperty = DependencyPropertyManager.Register("IsTopBorderVisible", typeof(bool), typeof(PrintCellEditor), new FrameworkPropertyMetadata(false, (d, e) => ((PrintCellEditor)d).OnIsTopBorderVisibleChanged()));
		public static readonly DependencyProperty DetailLevelProperty = DependencyPropertyManager.Register("DetailLevel", typeof(int), typeof(PrintCellEditor), new FrameworkPropertyMetadata(0, (d, e) => ((PrintCellEditor)d).OnIsTopBorderVisibleChanged()));
		public PrintCellEditor() {
#if !SL
			formattingHelper = new ConditionalFormattingHelper<PrintCellEditor>(this, BackgroundProperty);
#endif
		}
		public bool IsTopBorderVisible {
			get { return (bool)GetValue(IsTopBorderVisibleProperty); }
			set { SetValue(IsTopBorderVisibleProperty, value); }
		}
		public int DetailLevel {
			get { return (int)GetValue(DetailLevelProperty); }
			set { SetValue(DetailLevelProperty, value); }
		}
#if DEBUGTEST
		public IOnPageUpdater GetOnPageUpdater() {
			return ExportSettings.GetOnPageUpdater((BaseEdit)editCore);
		}
#endif
		bool isPageUpdaterCreated = false;
		void OnIsTopBorderVisibleChanged() {
			if(editCore == null)
				return;
			if(isPageUpdaterCreated)
				ClearUpdater();
			if(IsTopBorderVisible)
				ExportSettings.SetOnPageUpdater((BaseEdit)editCore, new TopBorderOnPageUpdater() { DetailLevel = this.DetailLevel });
			else
				ExportSettings.SetOnPageUpdater((BaseEdit)editCore, new InfoProviderOnPageUpdater() { DetailLevel = this.DetailLevel });
			isPageUpdaterCreated = true;
		}
		protected override IBaseEdit CreateEditor(Editors.Settings.BaseEditSettings settings) {
			return settings.CreateEditor(false, EditorColumn, GetEditorOptimizationMode());
		}
		public override void OnApplyTemplate() {
			base.OnApplyTemplate();
#if DEBUGTEST
			if(editCore == null)
				editCore = new TextEdit();
#endif
			OnIsTopBorderVisibleChanged();
		}
		void ClearUpdater() {
			ExportSettings.SetOnPageUpdater(this, null);
			isPageUpdaterCreated = false;
		}
		protected override bool OptimizeEditorPerformance { get { return false; } }
		protected override void InitializeBaseEdit(IBaseEdit newEdit, InplaceEditorBase.BaseEditSourceType newBaseEditSourceType) {
			base.InitializeBaseEdit(newEdit, newBaseEditSourceType);
			((BaseEdit)editCore).Style = GridPrintingHelper.GetPrintCellInfo(CellData).PrintCellStyle;
			newEdit.ShouldDisableExcessiveUpdatesInInplaceInactiveMode = !((BaseEdit)editCore).AllowUpdateTextBlockWhenPrinting;
#if !SL
			if(Background != null)
				UpdateBackground();
#endif
			UpdatePrintingMergeValue();
		}
		protected internal override void UpdatePrintingMergeValue() {
			if(!View.ActualAllowCellMerge) return;
			Dictionary<ColumnBase, int> mergeValues = RowData.DataRowNode.PrintInfo.MergeValues;
			int value;
			ExportSettings.SetMergeValue(editCore as DependencyObject, RowData.DataRowNode.PrintInfo.MergeValues.TryGetValue(Column, out value) ?  (object)value : null);
		}
		protected override void UpdateDisplayTemplate(bool updateForce = false) { }
		protected override DataTemplate SelectTemplate() { return null; }
		protected override bool ShouldSyncCellContentPresenterProperties { get { return false; } }
#if !SL
		protected override void UpdateConditionalAppearance() {
			base.UpdateConditionalAppearance();
			formattingHelper.UpdateConditionalAppearance();
		}
		#region IConditionalFormattingClient
		ConditionalFormattingHelper<PrintCellEditor> formattingHelper;
		ConditionalFormattingHelper<PrintCellEditor> IConditionalFormattingClient<PrintCellEditor>.FormattingHelper { get { return formattingHelper; } }
		bool IConditionalFormattingClient<PrintCellEditor>.IsSelected { get { return false; } }
		IList<FormatConditionBaseInfo> IConditionalFormattingClient<PrintCellEditor>.GetRelatedConditions() {
			var tableView = View as ITableView;
			return (tableView != null && Column != null) ? GetRelatedConditionsCore(tableView, Column.FieldName) : null;
		}
		static IList<FormatConditionBaseInfo> GetRelatedConditionsCore(ITableView tableView, string fieldName) {
			var rowConditions = tableView.FormatConditions.GetInfoByFieldName(string.Empty);
			var cellConditions = tableView.FormatConditions.GetInfoByFieldName(fieldName);
			if(rowConditions == null && cellConditions == null)
				return null;
			if(rowConditions != null) {
				return cellConditions != null ? rowConditions.Concat(cellConditions).ToList() : rowConditions;
			}
			return cellConditions;
		}
		internal static readonly ServiceSummaryItem[] EmptySummaries = new ServiceSummaryItem[0];
		FormatValueProvider? IConditionalFormattingClient<PrintCellEditor>.GetValueProvider(string fieldName) {
			return RowData.GetValueProvider(fieldName);
		}
		bool IConditionalFormattingClient<PrintCellEditor>.IsReady { get { return true; } }
		void IConditionalFormattingClient<PrintCellEditor>.UpdateBackground() {
		}
		void IConditionalFormattingClient<PrintCellEditor>.UpdateDataBarFormatInfo(DataBarFormatInfo info) {
		}
		Locker IConditionalFormattingClient<PrintCellEditor>.Locker { get { return RowData.conditionalFormattingLocker; } }
		#endregion
#endif
	}
	[DXToolboxBrowsable(false)]
	public class PrintTextEdit : TextEdit {
		public static readonly DependencyProperty IsTopBorderVisibleProperty = DependencyPropertyManager.Register("IsTopBorderVisible", typeof(bool), typeof(PrintTextEdit), new FrameworkPropertyMetadata(false, (d, e) => ((PrintTextEdit)d).OnIsTopBorderVisibleChanged()));
		public static readonly DependencyProperty IsBottomRowProperty = DependencyPropertyManager.Register("IsBottomRow", typeof(bool), typeof(PrintTextEdit), new FrameworkPropertyMetadata(false, (d, e) => ((PrintTextEdit)d).OnIsTopBorderVisibleChanged()));
		public static readonly DependencyProperty DetailLevelProperty = DependencyPropertyManager.Register("DetailLevel", typeof(int), typeof(PrintTextEdit), new FrameworkPropertyMetadata(0, (d, e) => ((PrintTextEdit)d).OnIsTopBorderVisibleChanged()));
		public bool IsTopBorderVisible {
			get { return (bool)GetValue(IsTopBorderVisibleProperty); }
			set { SetValue(IsTopBorderVisibleProperty, value); }
		}
		public bool IsBottomRow {
			get { return (bool)GetValue(IsBottomRowProperty); }
			set { SetValue(IsBottomRowProperty, value); }
		}
		public int DetailLevel {
			get { return (int)GetValue(DetailLevelProperty); }
			set { SetValue(DetailLevelProperty, value); }
		}
		bool isPageUpdaterCreated = false;
		void OnIsTopBorderVisibleChanged() {
			IOnPageUpdater updater;
			if(isPageUpdaterCreated)
				ClearUpdater();
			if(IsTopBorderVisible) {
				if(IsBottomRow)
					updater = new FooterRowTobBorgerOnPageUpdater();
				else
					updater = new TopBorderOnPageUpdater();
			}
			else {
				updater = new InfoProviderOnPageUpdater();
			}
			((InfoProviderOnPageUpdaterBase)updater).DetailLevel = DetailLevel;
			ExportSettings.SetOnPageUpdater(this, updater);
			isPageUpdaterCreated = true;
		}
		void ClearUpdater() {
			ExportSettings.SetOnPageUpdater(this, null);
			isPageUpdaterCreated = false;
		}
	}
	[DXToolboxBrowsable(false)]
	public class PrintFixedTotalSummarySeparator : PrintTextEdit {
		public static readonly DependencyProperty EditValueLeftSideProperty = DependencyPropertyManager.Register("EditValueLeftSide", typeof(string), typeof(PrintFixedTotalSummarySeparator), new FrameworkPropertyMetadata(null, (d, e) => ((PrintFixedTotalSummarySeparator)d).OnSideEditValueChanged()));
		public static readonly DependencyProperty EditValueRightSideProperty = DependencyPropertyManager.Register("EditValueRightSide", typeof(string), typeof(PrintFixedTotalSummarySeparator), new FrameworkPropertyMetadata(null, (d, e) => ((PrintFixedTotalSummarySeparator)d).OnSideEditValueChanged()));
		public static readonly DependencyProperty ActualVisibilityProperty = DependencyPropertyManager.Register("ActualVisibility", typeof(Visibility), typeof(PrintFixedTotalSummarySeparator), new PropertyMetadata(Visibility.Visible));
		public string EditValueLeftSide {
			get { return (string)GetValue(EditValueLeftSideProperty); }
			set { SetValue(EditValueLeftSideProperty, value); }
		}
		public string EditValueRightSide {
			get { return (string)GetValue(EditValueRightSideProperty); }
			set { SetValue(EditValueRightSideProperty, value); }
		}
		public Visibility ActualVisibility {
			get { return (Visibility)GetValue(ActualVisibilityProperty); }
			set { SetValue(ActualVisibilityProperty, value); }
		}
		void OnSideEditValueChanged() {
			if(!String.IsNullOrEmpty(EditValueLeftSide) && !String.IsNullOrEmpty(EditValueRightSide)) {
				ActualVisibility = Visibility.Visible;
				return;
			}
			if(String.IsNullOrEmpty(EditValueLeftSide) && String.IsNullOrEmpty(EditValueRightSide)) {
				ActualVisibility = Visibility.Visible;
				return;
			}
			ActualVisibility = Visibility.Collapsed;
		}
	}
	public abstract class FillControl : Control {
		internal static readonly Thickness EmptyThickness = new Thickness();
	}
	public class PartialGroupingLineControl : FillControl { 
	}
	public class RowDataBottomIndentControl : FillControl {
	}
	public class RowDataBottomLastIndentControl : FillControl {
	}
	public class HeaderFillControl : FillControl {
	}
	public class RowDataFillControl : FillControl {
	}
	public class CellFillControl : FillControl {
	}
	public class GroupRowContentControl : FillControl {
	}
	public class PrintingCellItemsControl : CachedItemsControl {
	}
	public class PrintingHeaderItemsControl : PrintingCellItemsControl {
	}
	public class PrintingFooterItemsControl : PrintingHeaderItemsControl {
	}
	public class PrintingGroupItemsControl : PrintingHeaderItemsControl {
	}
	public class PrintingGroupFooterItemsControl : PrintingGroupItemsControl {
	}
	public class LastOnPageUpdater : IOnPageUpdater {
		void IOnPageUpdater.Update(XtraPrinting.IVisualBrick brick) {
			PanelBrick panel = brick as PanelBrick;
			if(panel != null) {
				foreach(VisualBrick child in panel.Bricks)
					child.Sides = BorderSide.None;
			}
			brick.Sides = XtraPrinting.BorderSide.None;
		}
	}
	public abstract class InfoProviderOnPageUpdaterBase {
		public int DetailLevel { get; set; }
	}
	public class TopBorderUpdaterBase : InfoProviderOnPageUpdaterBase, IOnPageUpdater {
		void IOnPageUpdater.Update(IVisualBrick brick) {
			brick.Sides |= BorderSide.Top;
		}
	}
	public class TopBorderOnPageUpdater : TopBorderUpdaterBase {
	}
	public class FooterRowTobBorgerOnPageUpdater : TopBorderUpdaterBase {
	}
	public class InfoProviderOnPageUpdater : InfoProviderOnPageUpdaterBase, IOnPageUpdater {
		void IOnPageUpdater.Update(IVisualBrick brick) {
		}
	}
	public abstract class PrintInfoBase : INotifyPropertyChanged {
		protected void OnPropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
	public class PrintGroupRowCellInfo : DependencyObject, INotifyPropertyChanged, ISupportVisibleIndex {
		int groupLevel;
		public int GroupLevel {
			get {
				return groupLevel;
			}
			internal set {
				if(groupLevel == value) return;
				groupLevel = value;
				OnPropertyChanged("GroupLevel");
			}
		}
		double width;
		public double Width {
			get {
				return width;
			}
			internal set {
				if(width == value) return;
				width = value;
				OnPropertyChanged("Width");
			}
		}
		PrintGroupCellPosition position;
		public PrintGroupCellPosition Position {
			get {
				return position;
			}
			internal set {
				if(position == value) return;
				position = value;
				OnPropertyChanged("Position");
			}
		}
		bool isLeftGroupSummaryValueEmpty;
		public bool IsLeftGroupSummaryValueEmpty {
			get { return isLeftGroupSummaryValueEmpty; }
			set {
				if(isLeftGroupSummaryValueEmpty == value)
					return;
				isLeftGroupSummaryValueEmpty = value;
				OnPropertyChanged("IsLeftGroupSummaryValueEmpty");
			}
		}
		bool isRightGroupSummaryValueEmpty;
		public bool IsRightGroupSummaryValueEmpty {
			get { return isRightGroupSummaryValueEmpty; }
			set {
				if(isRightGroupSummaryValueEmpty == value)
					return;
				isRightGroupSummaryValueEmpty = value;
				OnPropertyChanged("IsRightGroupSummaryValueEmpty");
			}
		}
		int visibleIndex;
		public int VisibleIndex {
			get { return visibleIndex; }
			internal set {
				if(visibleIndex == value) return;
				visibleIndex = value;
				OnPropertyChanged("VisibleIndex");
			}
		}
		string text;
		public string Text {
			get { return text; }
			internal set {
				if(text == value) return;
				text = value;
				OnPropertyChanged("Text");
			}
		}
		int detailLevel;
		public int DetailLevel {
			get {
				return detailLevel;
			}
			internal set {
				if(detailLevel == value) return;
				detailLevel = value;
				OnPropertyChanged("DetailLevel");
			}
		}
		Style printGroupRowStyle;
		public Style PrintGroupRowStyle {
			get {
				return printGroupRowStyle;
			}
			internal set {
				if(printGroupRowStyle == value) return;
				printGroupRowStyle = value;
				OnPropertyChanged("PrintGroupRowStyle");
			}
		}
		protected void OnPropertyChanged(string propertyName) {
			if(PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
	public enum PrintGroupCellPosition {
		Default,
		None,
		Last,
		Separator,
	}
	public abstract class PrintGroupRowInfoBase : PrintInfoBase {
		List<PrintGroupRowCellInfo> groupCells;
		public List<PrintGroupRowCellInfo> GroupCells {
			get {
				return groupCells;
			}
			internal set {
				if(groupCells == value) return;
				groupCells = value;
				OnPropertyChanged("GroupCells");
			}
		}
		PrintGroupRowCellInfo captionCell;
		public PrintGroupRowCellInfo CaptionCell {
			get {
				return captionCell;
			}
			internal set {
				if(captionCell == value) return;
				captionCell = value;
				OnPropertyChanged("CaptionCell");
			}
		}
		bool isExpanded;
		public bool IsExpanded {
			get {
				return isExpanded;
			}
			internal set {
				if(isExpanded == value) return;
				isExpanded = value;
				OnPropertyChanged("IsExpanded");
			}
		}
		bool isLast;
		public bool IsLast {
			get {
				return isLast;
			}
			internal set {
				if(isLast == value) return;
				isLast = value;
				OnPropertyChanged("IsLast");
			}
		}
	}
	public class PrintGroupRowInfo : PrintGroupRowInfoBase {
		PrintGroupRowCellInfo firstColumnCell;
		public PrintGroupRowCellInfo FirstColumnCell {
			get {
				return firstColumnCell;
			}
			internal set {
				if(firstColumnCell == value) return;
				firstColumnCell = value;
				OnPropertyChanged("FirstColumnCell");
			}
		}
	}
	public abstract class PrintGroupSummaryInfoBase : PrintInfoBase { 
	}
	public class PrintGroupSummaryInfo : PrintGroupSummaryInfoBase {
		public PrintGroupSummaryInfo(double printColumnWidth, string groupFooterText, Style printGroupFooterStyle, bool isRight) {
			this.PrintColumnWidth = printColumnWidth;
			this.GroupFooterText = groupFooterText;
			this.PrintGroupFooterStyle = printGroupFooterStyle;
			this.IsRight = isRight;
		}
		double printColumnWidth;
		public double PrintColumnWidth {
			get {
				return printColumnWidth;
			}
			internal set {
				if(printColumnWidth == value) return;
				printColumnWidth = value;
				OnPropertyChanged("PrintColumnWidth");
			}
		}
		public string GroupFooterText { get; private set; }
		public Style PrintGroupFooterStyle { get; private set; }
		public bool IsRight { get; private set; }
	}
	public abstract class PrintRowInfoBase : PrintInfoBase {
		Style printGroupRowStyle;
		public Style PrintGroupRowStyle {
			get {
				return printGroupRowStyle;
			}
			internal set {
				if(printGroupRowStyle == value) return;
				printGroupRowStyle = value;
				OnPropertyChanged("PrintGroupRowStyle");
			}
		}
		Style printRowIndentStyle;
		public Style PrintRowIndentStyle {
			get {
				return printRowIndentStyle;
			}
			internal set {
				if(printRowIndentStyle == value) return;
				printRowIndentStyle = value;
				OnPropertyChanged("PrintRowIndentStyle");
			}
		}
		Thickness printDataIndentBorderThickness;
		public Thickness PrintDataIndentBorderThickness {
			get {
				return printDataIndentBorderThickness;
			}
			internal set {
				if(printDataIndentBorderThickness == value) return;
				printDataIndentBorderThickness = value;
				OnPropertyChanged("PrintDataIndentBorderThickness");
			}
		}
		Thickness printDataIndentMargin;
		public Thickness PrintDataIndentMargin {
			get {
				return printDataIndentMargin;
			}
			internal set {
				if(printDataIndentMargin == value) return;
				printDataIndentMargin = value;
				OnPropertyChanged("PrintDataIndentMargin");
			}
		}
		double printDataIndent;
		public double PrintDataIndent {
			get {
				return printDataIndent;
			}
			internal set {
				if(printDataIndent == value) return;
				printDataIndent = value;
				OnPropertyChanged("PrintDataIndent");
			}
		}
		double totalHeaderWidth;
		public double TotalHeaderWidth {
			get {
				return totalHeaderWidth;
			}
			internal set {
				if(totalHeaderWidth == value) return;
				totalHeaderWidth = value;
				OnPropertyChanged("TotalHeaderWidth");
			}
		}
		Style printCellStyle;
		public Style PrintCellStyle {
			get {
				return printCellStyle;
			}
			internal set {
				if(printCellStyle == value) return;
				printCellStyle = value;
				OnPropertyChanged("PrintCellStyle");
			}
		}
		Style printFixedFooterStyle;
		public Style PrintFixedFooterStyle {
			get {
				return printFixedFooterStyle;
			}
			internal set {
				if(printFixedFooterStyle == value) return;
				printFixedFooterStyle = value;
				OnPropertyChanged("PrintFixedFooterStyle");
			}
		}
		Style printColumnHeaderStyle;
		public Style PrintColumnHeaderStyle {
			get {
				return printColumnHeaderStyle;
			}
			internal set {
				if(printColumnHeaderStyle == value) return;
				printColumnHeaderStyle = value;
				OnPropertyChanged("PrintColumnHeaderStyle");
			}
		}
		bool isPrintColumnHeadersVisible;
		public bool IsPrintColumnHeadersVisible {
			get {
				return isPrintColumnHeadersVisible;
			}
			set {
				IsPrintTopRowVisible = GetIsPrintTopRowVisible(!value);
				if(isPrintColumnHeadersVisible == value) return;
				isPrintColumnHeadersVisible = value;
				OnPropertyChanged("IsPrintColumnHeadersVisible");
			}
		}
		protected virtual bool GetIsPrintTopRowVisible(bool isVisible) { return isVisible; }
		bool isPrintTopRowVisible;
		public bool IsPrintTopRowVisible {
			get {
				return isPrintTopRowVisible;
			}
			private set {
				if(isPrintTopRowVisible == value) return;
				isPrintTopRowVisible = value;
				OnPropertyChanged("IsPrintTopRowVisible");
			}
		}
		bool isPrintBandHeadersVisible;
		public bool IsPrintBandHeadersVisible {
			get {
				return isPrintBandHeadersVisible;
			}
			set {
				if(isPrintBandHeadersVisible == value) return;
				isPrintBandHeadersVisible = value;
				OnPropertyChanged("IsPrintBandHeadersVisible");
			}
		}
		bool isPrintHeaderBottomIndentVisible;
		public bool IsPrintHeaderBottomIndentVisible {
			get {
				return isPrintHeaderBottomIndentVisible;
			}
			internal set {
				if(isPrintHeaderBottomIndentVisible == value) return;
				isPrintHeaderBottomIndentVisible = value;
				OnPropertyChanged("IsPrintHeaderBottomIndentVisible");
			}
		}
		bool isPrintFooterBottomIndentVisible;
		public bool IsPrintFooterBottomIndentVisible {
			get {
				return isPrintFooterBottomIndentVisible;
			}
			internal set {
				if(isPrintFooterBottomIndentVisible == value) return;
				isPrintFooterBottomIndentVisible = value;
				OnPropertyChanged("IsPrintFooterBottomIndentVisible");
			}
		}
		bool isPrintFixedFooterBottomIndentVisible;
		public bool IsPrintFixedFooterBottomIndentVisible {
			get {
				return isPrintFixedFooterBottomIndentVisible;
			}
			internal set {
				if(isPrintFixedFooterBottomIndentVisible == value) return;
				isPrintFixedFooterBottomIndentVisible = value;
				OnPropertyChanged("IsPrintFixedFooterBottomIndentVisible");
			}
		}
	}
	public class PrintRowInfo : PrintRowInfoBase {
		Thickness printTopRowIndentMargin;
		public Thickness PrintTopRowIndentMargin {
			get {
				return printTopRowIndentMargin;
			}
			internal set {
				if(printTopRowIndentMargin == value) return;
				printTopRowIndentMargin = value;
				OnPropertyChanged("PrintTopRowIndentMargin");
			}
		}
		double printTopRowWidth;
		public double PrintTopRowWidth {
			get {
				return printTopRowWidth;
			}
			internal set {
				if(printTopRowWidth == value) return;
				printTopRowWidth = value;
				OnPropertyChanged("PrintTopRowWidth");
			}
		}
		bool isPrintTopDetailRowVisible;
		public bool IsPrintTopDetailRowVisible {
			get {
				return isPrintTopDetailRowVisible;
			}
			internal set {
				if(isPrintTopDetailRowVisible == value) return;
				isPrintTopDetailRowVisible = value;
				OnPropertyChanged("IsPrintTopDetailRowVisible");
			}
		}
		bool isPrintBottomDetailIndentVisible;
		public bool IsPrintBottomDetailIndentVisible {
			get {
				return isPrintBottomDetailIndentVisible;
			}
			internal set {
				if(isPrintBottomDetailIndentVisible == value) return;
				isPrintBottomDetailIndentVisible = value;
				OnPropertyChanged("IsPrintBottomDetailIndentVisible");
			}
		}
		bool showRowBreak;
		public bool ShowRowBreak {
			get { return showRowBreak; }
			set {
				if(showRowBreak == value) return;
				showRowBreak = value;
				OnPropertyChanged("ShowRowBreak");
			}
		}
		bool showIndentRowBreak;
		public bool ShowIndentRowBreak {
			get { return showIndentRowBreak; }
			set {
				if(showIndentRowBreak == value) return;
				showIndentRowBreak = value;
				OnPropertyChanged("ShowIndentRowBreak");
			}
		}
		bool isPrintBottomLastDetailIndentVisible;
		public bool IsPrintBottomLastDetailIndentVisible {
			get {
				return isPrintBottomLastDetailIndentVisible;
			}
			internal set {
				if(isPrintBottomLastDetailIndentVisible == value) return;
				isPrintBottomLastDetailIndentVisible = value;
				OnPropertyChanged("IsPrintBottomLastDetailIndentVisible");
			}
		}
		Style printRowDataBottomIndentControlStyle;
		public Style PrintRowDataBottomIndentControlStyle {
			get {
				return printRowDataBottomIndentControlStyle;
			}
			internal set {
				if(printRowDataBottomIndentControlStyle == value) return;
				printRowDataBottomIndentControlStyle = value;
				OnPropertyChanged("PrintRowDataBottomIndentControlStyle");
			}
		}
		Style printRowDataBottomLastIndentControlStyle;
		public Style PrintRowDataBottomLastIndentControlStyle {
			get {
				return printRowDataBottomLastIndentControlStyle;
			}
			internal set {
				if(printRowDataBottomLastIndentControlStyle == value) return;
				printRowDataBottomLastIndentControlStyle = value;
				OnPropertyChanged("PrintRowDataBottomLastIndentControlStyle");
			}
		}
		bool isPrintDetailTopIndentVisible;
		public bool IsPrintDetailTopIndentVisible {
			get {
				return isPrintDetailTopIndentVisible;
			}
			internal set {
				if(isPrintDetailTopIndentVisible == value) return;
				isPrintDetailTopIndentVisible = value;
				OnPropertyChanged("IsPrintDetailTopIndentVisible");
			}
		}
		bool printDetailBottomIndentThickness;
		public bool IsPrintDetailBottomIndentVisible {
			get {
				return printDetailBottomIndentThickness;
			}
			internal set {
				if(printDetailBottomIndentThickness == value) return;
				printDetailBottomIndentThickness = value;
				OnPropertyChanged("IsPrintDetailBottomIndentVisible");
			}
		}
		Thickness printDataTopIndentBorderThickness;
		public Thickness PrintDataTopIndentBorderThickness {
			get {
				return printDataTopIndentBorderThickness;
			}
			internal set {
				if(printDataTopIndentBorderThickness == value) return;
				printDataTopIndentBorderThickness = value;
				OnPropertyChanged("PrintDataTopIndentBorderThickness");
			}
		}
		double detailTopIndent;
		public double DetailTopIndent {
			get {
				return detailTopIndent;
			}
			internal set {
				if(detailTopIndent == value) return;
				detailTopIndent = value;
				OnPropertyChanged("DetailTopIndent");
			}
		}
		double detailBottomIndent;
		public double DetailBottomIndent {
			get {
				return detailBottomIndent;
			}
			internal set {
				if(detailBottomIndent == value) return;
				detailBottomIndent = value;
				OnPropertyChanged("DetailBottomIndent");
			}
		}
		int detailLevel;
		public int DetailLevel {
			get {
				return detailLevel;
			}
			internal set {
				if(detailLevel == value) return;
				detailLevel = value;
				OnPropertyChanged("DetailLevel");
			}
		}
		public BandsLayoutBase BandsLayout { get; internal set; }
	}
	public class PrintCellInfo : PrintInfoBase {
		public PrintCellInfo(bool isLast, string totalSummaryText, Style printTotalSummaryStyle, double printColumnWidth, object headerCaption, Style printColumnHeaderStyle, Style printCellStyle, bool isColumnHeaderVisible, int detailLevel, HorizontalAlignment horizontalHeaderContentAlignment, bool hasTopElement, bool isRight) {
			this.IsLast = isLast;
			this.TotalSummaryText = totalSummaryText;
			this.PrintTotalSummaryStyle = printTotalSummaryStyle;
			this.PrintColumnHeaderStyle = printColumnHeaderStyle;
			this.PrintCellStyle = printCellStyle;
			this.PrintColumnWidth = printColumnWidth;
			this.HeaderCaption = headerCaption;
			this.IsColumnHeaderVisible = isColumnHeaderVisible;
			this.DetailLevel = detailLevel;
			this.HorizontalHeaderContentAlignment = horizontalHeaderContentAlignment;
			this.HasTopElement = hasTopElement;
			this.IsRight = isRight;
			IsTotalSummaryLeftBorderVisible = true;
		}
		string totalSummaryText = null;
		public string TotalSummaryText {
			get { return totalSummaryText; }
			internal set {
				if(totalSummaryText == value) return;
				totalSummaryText = value;
				OnPropertyChanged("TotalSummaryText");
			}
		}
		public bool IsTotalSummaryLeftBorderVisible { get; set; }
		public bool IsLast { get; private set; }
		public Style PrintTotalSummaryStyle { get; private set; }
		public Style PrintColumnHeaderStyle { get; private set; }
		public Style PrintCellStyle { get; private set; }
		public HorizontalAlignment HorizontalHeaderContentAlignment { get; private set; }
		double printColumnWidth;
		public double PrintColumnWidth {
			get {
				return printColumnWidth;
			}
			internal set {
				if(printColumnWidth == value) return;
				printColumnWidth = value;
				OnPropertyChanged("PrintColumnWidth");
			}
		}
		public object HeaderCaption { get; private set; }
		internal bool IsColumnHeaderVisible { get; private set; }
		public int DetailLevel { get; private set; }
		public bool HasTopElement { get; private set; }
		public bool IsRight { get; private set; }
	}
	public class PrintBandInfo {
		BandBase originationBand;
		public PrintBandInfo(BandBase originationBand) {
			this.originationBand = originationBand;
		}
		public BandBase OriginationBand { get { return originationBand; } }
	}
	public class PrintLayoutAssigner : LayoutAssigner {
		public static readonly LayoutAssigner Printing = new PrintLayoutAssigner();
		public override double GetWidth(BaseColumn column) {
			return GridPrintingHelper.GetPrintColumnWidth(column);
		}
		public override void SetWidth(BaseColumn column, double value) {
			GridPrintingHelper.SetPrintColumnWidth(column, value);
		}
		public override void CreateLayout(ColumnsLayoutCalculator calculator) {
		}
		public override bool UseDataAreaIndent { get { return false; } }
		public override bool UseFixedColumnIndents { get { return false; } }
		public override bool UseDetailButtonsIndents { get { return false; } }
		public override ColumnPosition GetColumnPosition(BaseColumn column) {
			return GridPrintingHelper.GetPrintColumnPosition(column);
		}
		public override void SetColumnPosition(BaseColumn column, ColumnPosition position) {
			GridPrintingHelper.SetPrintColumnPosition(column, position);
		}
		public override bool GetHasLeftSibling(BaseColumn column) {
			return GridPrintingHelper.GetPrintHasLeftSibling(column);
		}
		public override void SetHasLeftSibling(BaseColumn column, bool value) {
			GridPrintingHelper.SetPrintHasLeftSibling(column, value);
		}
		public override bool GetHasRightSibling(BaseColumn column) {
			return GridPrintingHelper.GetPrintHasRightSibling(column);
		}
		public override void SetHasRightSibling(BaseColumn column, bool value) {
			GridPrintingHelper.SetPrintHasRightSibling(column, value);
		}
	}
	public class PrintHeaderBorderConverter : IValueConverter {
#region IValueConverter Members
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			PrintCellInfo cellInfo = value as PrintCellInfo;
			if(cellInfo == null)
				return new Thickness();
			int thicknessValue = (parameter is int) ? (int)parameter : 1;
			double topBorder = cellInfo.IsColumnHeaderVisible && !cellInfo.HasTopElement ? thicknessValue : 0d;
			if(cellInfo.IsRight)
				return new Thickness(thicknessValue, topBorder, thicknessValue, thicknessValue);
			return new Thickness(thicknessValue, topBorder, 0, thicknessValue);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
#endregion
	}
	public class PrintFixedTotalSummaryBorderConverter : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			HeadersData data = value as HeadersData;
			if(data == null)
				return value;
			int leftBorder = String.IsNullOrEmpty(GridPrintingHelper.GetPrintFixedFooterTextLeft(data)) ? 1 : 0;
			int rightBorder = String.IsNullOrEmpty(GridPrintingHelper.GetPrintFixedFooterTextRight(data)) ? 1 : 0;
			return new Thickness(leftBorder, 0, rightBorder, 1);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PrintGroupPositionToBorderConverter : IValueConverter {
		public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			PrintGroupRowCellInfo info = value as PrintGroupRowCellInfo;
			if(info == null)
				return new Thickness(0);
			int leftBound = info.IsLeftGroupSummaryValueEmpty ? 1 : 0;
			int rightBound = info.IsRightGroupSummaryValueEmpty ? 1 : 0;
			switch(info.Position) {
				case PrintGroupCellPosition.Default: return new Thickness(leftBound, 0, rightBound, 1);
				case PrintGroupCellPosition.Last: return new Thickness(leftBound, 0, 1, 1);
				case PrintGroupCellPosition.None: return new Thickness(0, 0, 0, 1);
				case PrintGroupCellPosition.Separator: return new Thickness(0, 0, rightBound, 1);
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PrintGroupPositionToBorderConverterInverted : IValueConverter {
		public virtual object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			PrintGroupRowCellInfo info = value as PrintGroupRowCellInfo;
			if(info == null)
				return new Thickness(0);
			int leftBound = !info.IsLeftGroupSummaryValueEmpty ? 1 : 0;
			int rightBound = !info.IsRightGroupSummaryValueEmpty ? 1 : 0;
			switch(info.Position) {
				case PrintGroupCellPosition.Default: return new Thickness(leftBound, 0, rightBound, 1);
				case PrintGroupCellPosition.Last: return new Thickness(leftBound, 0, 1, 1);
				case PrintGroupCellPosition.None: return new Thickness(0, 0, 0, 1);
				case PrintGroupCellPosition.Separator: return new Thickness(0, 0, rightBound, 1);
			}
			return null;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class PrintTotalSummaryCellConverter : DependencyObject, IValueConverter {
		public static readonly DependencyProperty BorderThicknessProperty = DependencyProperty.Register("BorderThickness", typeof(Thickness), typeof(PrintTotalSummaryCellConverter), new PropertyMetadata(new Thickness(1.0)));
		public static readonly DependencyProperty SkipEmptySummariesProperty = DependencyProperty.Register("SkipEmptySummaries", typeof(bool), typeof(PrintTotalSummaryCellConverter), new PropertyMetadata(false));
		public Thickness BorderThickness {
			get { return (Thickness)GetValue(BorderThicknessProperty); }
			set { SetValue(BorderThicknessProperty, value); }
		}
		public bool SkipEmptySummaries {
			get { return (bool)GetValue(SkipEmptySummariesProperty); }
			set { SetValue(SkipEmptySummariesProperty, value); }
		}
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			PrintCellInfo info = value as PrintCellInfo;
			if(info == null)
				return new Thickness(0);
			if(SkipEmptySummaries && String.IsNullOrWhiteSpace(info.TotalSummaryText))
				return new Thickness(0);
			double leftBound = info.IsTotalSummaryLeftBorderVisible ? BorderThickness.Left : 0;
			double rightBound = info.IsRight ? BorderThickness.Right : 0;
			return new Thickness(leftBound, BorderThickness.Top, rightBound, BorderThickness.Bottom);
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
	public class BandsLayoutTemplateConverter : IValueConverter {
		public ControlTemplate TableViewTemplate { get; set; }
		public ControlTemplate BandedViewTemplate { get; set; }
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			BandsLayoutBase bandsLayout = value as BandsLayoutBase;
			return bandsLayout == null ? TableViewTemplate : BandedViewTemplate;
		}
		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
#if !SL
	public class FixedTotalSummaryLeftVisibilityConverter : IMultiValueConverter {
		public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture) {
			if(!(values[0] is string) || !(values[1] is string))
				return Visibility.Visible;
			string left = values[0] as string;
			string right = values[1] as string;
			if(!String.IsNullOrEmpty(left) || String.IsNullOrEmpty(right))
				return Visibility.Visible;
			return Visibility.Collapsed;
		}
		public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture) {
			throw new NotImplementedException();
		}
	}
#endif
	public class PrintRowStackPanel : StackPanel {
		public static readonly DependencyProperty IsRowDataBottomIndentVisibleProperty;
		public static readonly DependencyProperty IsRowDataBottomLastIndentVisibleProperty;
		static PrintRowStackPanel() {
			Type ownerType = typeof(PrintRowStackPanel);
			IsRowDataBottomIndentVisibleProperty = DependencyPropertyManager.Register("IsRowDataBottomIndentVisible", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((PrintRowStackPanel)d).OnIndentVisibleChanged()));
			IsRowDataBottomLastIndentVisibleProperty = DependencyPropertyManager.Register("IsRowDataBottomLastIndentVisible", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((PrintRowStackPanel)d).OnIndentVisibleChanged()));
		}
		public bool IsRowDataBottomIndentVisible {
			get { return (bool)GetValue(IsRowDataBottomIndentVisibleProperty); }
			set { SetValue(IsRowDataBottomIndentVisibleProperty, value); }
		}
		public bool IsRowDataBottomLastIndentVisible {
			get { return (bool)GetValue(IsRowDataBottomLastIndentVisibleProperty); }
			set { SetValue(IsRowDataBottomLastIndentVisibleProperty, value); }
		}
		void OnIndentVisibleChanged() {
			ExportSettings.SetTargetType(this, IsRowDataBottomIndentVisible || IsRowDataBottomLastIndentVisible ? TargetType.Panel : ExportSettingDefaultValue.TargetType);
		}
	}
	public class PrintBandsPanel : BandsPanelBase {
		protected override FrameworkElement CreateBandElement(BandBase band) {
			PrintCellInfo info = GridPrintingHelper.GetPrintCellInfo(band);
			TextEdit element = new TextEdit() { EditValue = info.HeaderCaption, Style = info.PrintColumnHeaderStyle };
#if SL
			element.UseLayoutRounding = false;
#endif
			element.DataContext = band;
			return element;
		}
		protected override double GetBandWidth(BandBase band) {
			return GridPrintingHelper.GetPrintCellInfo(band).PrintColumnWidth;
		}
	}
	public class PrintBandsColumnsPanel : BandsNoneDropPanel {
		internal new ColumnsRowDataBase RowData { get { return ((RowContent)DataContext).Content as ColumnsRowDataBase; } }
		protected internal override double GetColumnWidth(ColumnBase column) {
			GridColumnData data = RowData.GetCellDataByColumn(column);
			return GridPrintingHelper.GetPrintCellInfo(data).PrintColumnWidth;
		}
		protected internal override double GetBandWidth(BandBase band) {
			return GridPrintingHelper.GetPrintCellInfo(band).PrintColumnWidth;
		}
	}
	public class PrintBandsCellsPanel : BandsCellsPanel {
		public static readonly DependencyProperty LevelProperty;
		static PrintBandsCellsPanel() {
			Type ownerType = typeof(PrintBandsCellsPanel);
			LevelProperty = DependencyPropertyManager.Register("Level", typeof(int), ownerType, new FrameworkPropertyMetadata(0, (d, e) => ((PrintBandsCellsPanel)d).InvalidateMeasure()));
		}
		public int Level {
			get { return (int)GetValue(LevelProperty); }
			set { SetValue(LevelProperty, value); }
		}
		internal new ColumnsRowDataBase RowData { get { return ((RowContent)DataContext).Content as ColumnsRowDataBase; } }
		protected internal override ColumnBase GetColumn(FrameworkElement element) {
			return ((GridColumnData)((ContentPresenter)element).Content).Column;
		}
		protected internal override double GetColumnWidth(ColumnBase column) {
			GridColumnData data = RowData.GetCellDataByColumn(column);
			return GridPrintingHelper.GetPrintCellInfo(data).PrintColumnWidth;
		}
		protected internal override double GetBandWidth(BandBase band) {
			return GridPrintingHelper.GetPrintCellInfo(band).PrintColumnWidth;
		}
	}
}
