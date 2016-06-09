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
using System.Windows.Media;
using DevExpress.Data;
using System.Linq;
using DevExpress.Utils;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Data;
using DevExpress.Xpf.Grid.Native;
using DevExpress.Xpf.Grid.TreeList;
using DevExpress.Xpf.Grid.TreeList.Native;
using DevExpress.Xpf.Utils;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Editors.Validation;
using DevExpress.Xpf.Core.Native;
using DevExpress.Xpf.Grid.Printing;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Printing.BrickCollection;
using DevExpress.Xpf.Printing.Native;
using DevExpress.XtraPrinting.DataNodes;
using DevExpress.XtraPrinting;
using DevExpress.XtraEditors.DXErrorProvider;
using DevExpress.Xpf.Editors.Helpers;
using DevExpress.Xpf.Bars;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using WarningException = System.Exception;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using HierarchicalDataTemplate = DevExpress.Xpf.Core.HierarchicalDataTemplate;
using DevExpress.Xpf.Core.Serialization;
using System.Text;
#else
using DialogService = DevExpress.Xpf.Core.DialogService;
using IDialogService = DevExpress.Mvvm.IDialogService;
using TreeListViewAssignableDialogServiceHelper = DevExpress.Mvvm.UI.Native.AssignableServiceHelper2<DevExpress.Xpf.Grid.TreeListView, DevExpress.Mvvm.IDialogService>;
using System.Windows.Markup;
using DevExpress.Xpf.Core.Serialization;
using System.Collections.ObjectModel;
using System.Text;
using DevExpress.Xpf.GridData;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Data.Utils.ServiceModel;
using DevExpress.XtraExport.Helpers;
#endif
namespace DevExpress.Xpf.Grid {
	public enum TreeListLineStyle { None, Solid }
	public enum TreeDerivationMode { Selfreference, ChildNodesSelector, HierarchicalDataTemplate }
	public enum TreeListFilterMode { Standard, Smart, Extended }
	public interface IChildNodesSelector {
		IEnumerable SelectChildren(object item);
	}
	public class TreeListViewCommands : DataViewCommandsBase, IConditionalFormattingCommands {
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewCommandsChangeNodeExpanded")]
#endif
		public ICommand ChangeNodeExpanded { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewCommandsChangeNodeCheckState")]
#endif
		public ICommand ChangeNodeCheckState { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewCommandsExpandAllNodes")]
#endif
		public ICommand ExpandAllNodes { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewCommandsCollapseAllNodes")]
#endif
		public ICommand CollapseAllNodes { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewCommandsExpandToLevel")]
#endif
		public ICommand ExpandToLevel { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewCommandsBestFitColumn")]
#endif
		public ICommand BestFitColumn { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewCommandsBestFitColumns")]
#endif
		public ICommand BestFitColumns { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewCommandsCheckAllNodes")]
#endif
		public ICommand CheckAllNodes { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewCommandsUncheckAllNodes")]
#endif
		public ICommand UncheckAllNodes { get; private set; }
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewCommandsAddFormatCondition")]
#endif
		public ICommand AddFormatCondition { get; private set; }
		public ICommand ShowLessThanFormatConditionDialog { get; private set; }
		public ICommand ShowGreaterThanFormatConditionDialog { get; private set; }
		public ICommand ShowEqualToFormatConditionDialog { get; private set; }
		public ICommand ShowBetweenFormatConditionDialog { get; private set; }
		public ICommand ShowTextThatContainsFormatConditionDialog { get; private set; }
		public ICommand ShowADateOccurringFormatConditionDialog { get; private set; }
		public ICommand ShowCustomConditionFormatConditionDialog { get; private set; }
		public ICommand ShowTop10ItemsFormatConditionDialog { get; private set; }
		public ICommand ShowBottom10ItemsFormatConditionDialog { get; private set; }
		public ICommand ShowTop10PercentFormatConditionDialog { get; private set; }
		public ICommand ShowBottom10PercentFormatConditionDialog { get; private set; }
		public ICommand ShowAboveAverageFormatConditionDialog { get; private set; }
		public ICommand ShowBelowAverageFormatConditionDialog { get; private set; }
		public ICommand ClearFormatConditionsFromAllColumns { get; private set; }
		public ICommand ClearFormatConditionsFromColumn { get; private set; }
		public ICommand ShowConditionalFormattingManager { get; private set; }
		public ICommand ShowEditForm { get; private set; }
		public ICommand HideEditForm { get; private set; }
		public ICommand CloseEditForm { get; private set; }
		readonly TreeListView treeListView;
		public TreeListViewCommands(TreeListView view)
			: base(view) {
			treeListView = view;
			ChangeNodeExpanded = CreateDelegateCommand(o => view.OnChangeNodeExpanded(o), o => view.CanChangeNodeExpaned(o));
			ChangeNodeCheckState = CreateDelegateCommand(o => view.OnChangeNodeCheckState(o), o => view.CanChangeNodeCheckState(o));
			ExpandAllNodes = CreateDelegateCommand(o => view.ExpandAllNodes(), o => true);
			CollapseAllNodes = CreateDelegateCommand(o => view.CollapseAllNodes(), o => true);
			ExpandToLevel = CreateDelegateCommand(o => view.ExpandToLevel(Convert.ToInt32(o)));
			BestFitColumn = CreateDelegateCommand(o => view.TreeListViewBehavior.BestFitColumn(o), o => view.TreeListViewBehavior.CanBestFitColumn(o));
			BestFitColumns = CreateDelegateCommand(o => view.TreeListViewBehavior.BestFitColumns(), o => view.TreeListViewBehavior.CanBestFitColumns());
			CheckAllNodes = CreateDelegateCommand(o => view.CheckAllNodes(), o => true);
			UncheckAllNodes = CreateDelegateCommand(o => view.UncheckAllNodes(), o => true);
			AddFormatCondition = CreateDelegateCommand(x => view.AddFormatCondition((FormatConditionBase)x));
			ShowLessThanFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.LessThan));
			ShowGreaterThanFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.GreaterThan));
			ShowEqualToFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.EqualTo));
			ShowBetweenFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Between));
			ShowTextThatContainsFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.TextThatContains));
			ShowADateOccurringFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.ADateOccurring));
			ShowCustomConditionFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.CustomCondition));
			ShowTop10ItemsFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Top10Items));
			ShowBottom10ItemsFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Bottom10Items));
			ShowTop10PercentFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Top10Percent));
			ShowBottom10PercentFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.Bottom10Percent));
			ShowAboveAverageFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.AboveAverage));
			ShowBelowAverageFormatConditionDialog = CreateDelegateCommand(x => ShowFormatConditionDialog(x, FormatConditionDialogType.BelowAverage));
			ClearFormatConditionsFromAllColumns = CreateDelegateCommand(x => view.ClearFormatConditionsFromAllColumns());
			ClearFormatConditionsFromColumn = CreateDelegateCommand(x => view.ClearFormatConditionsFromColumn((ColumnBase)x));
			ShowConditionalFormattingManager = CreateDelegateCommand(x => view.ShowConditionalFormattingManager((ColumnBase)x));
			ShowEditForm = CreateDelegateCommand(o => view.ShowEditForm());
			HideEditForm = CreateDelegateCommand(o => view.HideEditForm());
			CloseEditForm = CreateDelegateCommand(o => view.CloseEditForm());
		}
		void ShowFormatConditionDialog(object column, FormatConditionDialogType dialogKind) {
			treeListView.ShowFormatConditionDialog((ColumnBase)column, dialogKind);
		}
	}
	public partial class TreeListView : GridDataViewBase, ITableView {
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
		#endregion
		public static readonly DependencyProperty RowIndentProperty;
		public static readonly DependencyProperty KeyFieldNameProperty;
		public static readonly DependencyProperty ParentFieldNameProperty;
		public static readonly DependencyProperty CheckBoxFieldNameProperty;
		public static readonly DependencyProperty CheckBoxValueConverterProperty;
		public static readonly DependencyProperty ImageFieldNameProperty;
		public static readonly DependencyProperty NodeImageSelectorProperty;
		public static readonly DependencyProperty RootValueProperty;
		public static readonly DependencyProperty AutoPopulateServiceColumnsProperty;
		static readonly DependencyPropertyKey VisibleColumnsPropertyKey;
		public static readonly DependencyProperty VisibleColumnsProperty;
		public static readonly DependencyProperty FocusedNodeProperty;
		public static readonly DependencyProperty AllowConditionalFormattingMenuProperty;
		public static readonly DependencyProperty AllowConditionalFormattingManagerProperty;
		public static readonly DependencyProperty PredefinedFormatsProperty;
		public static readonly DependencyProperty PredefinedColorScaleFormatsProperty;
		public static readonly DependencyProperty PredefinedDataBarFormatsProperty;
		public static readonly DependencyProperty PredefinedIconSetFormatsProperty;
		public static readonly DependencyProperty FormatConditionDialogServiceTemplateProperty;
		public static readonly DependencyProperty ConditionalFormattingManagerServiceTemplateProperty;
		public static readonly RoutedEvent NodeExpandingEvent;
		public static readonly RoutedEvent NodeExpandedEvent;
		public static readonly RoutedEvent NodeCollapsingEvent;
		public static readonly RoutedEvent NodeCollapsedEvent;
		public static readonly RoutedEvent NodeCheckStateChangedEvent;
		public static readonly RoutedEvent ShowingEditorEvent;
		public static readonly RoutedEvent ShownEditorEvent;
		public static readonly RoutedEvent HiddenEditorEvent;
		public static readonly RoutedEvent CellValueChangedEvent;
		public static readonly RoutedEvent CellValueChangingEvent;
		public static readonly RoutedEvent CustomScrollAnimationEvent;
		public static readonly RoutedEvent InvalidNodeExceptionEvent;
		public static readonly RoutedEvent ValidateNodeEvent;
		public static readonly RoutedEvent RowDoubleClickEvent;
		public static readonly RoutedEvent SelectionChangedEvent;
		public static readonly RoutedEvent CopyingToClipboardEvent;
		public static readonly RoutedEvent StartSortingEvent;
		public static readonly RoutedEvent EndSortingEvent;
		public static readonly DependencyProperty FocusedColumnProperty;
		public static readonly DependencyProperty ShowNodeImagesProperty;
		public static readonly DependencyProperty NodeImageSizeProperty;
		public static readonly DependencyProperty ShowCheckboxesProperty;
		public static readonly DependencyProperty AllowIndeterminateCheckStateProperty;
		public static readonly DependencyProperty AllowRecursiveNodeCheckingProperty;
		public static readonly DependencyProperty ShowExpandButtonsProperty;
		public static readonly DependencyProperty ShowRootIndentProperty;
		public static readonly DependencyProperty TreeLineStyleProperty;
		public static readonly DependencyProperty RowPresenterMarginProperty;
		public static readonly DependencyProperty AutoFilterRowCellStyleProperty;
		public static readonly DependencyProperty FilterModeProperty;
		public static readonly DependencyProperty ChildNodesPathProperty;
		public static readonly DependencyProperty TreeDerivationModeProperty;
		public static readonly DependencyProperty ChildNodesSelectorProperty;
		public static readonly DependencyProperty EnableDynamicLoadingProperty;
		public static readonly DependencyProperty AutoExpandAllNodesProperty;
		public static readonly DependencyProperty FetchSublevelChildrenOnExpandProperty;
		public static readonly DependencyProperty VerticalScrollbarVisibilityProperty;
		public static readonly DependencyProperty HorizontalScrollbarVisibilityProperty;
		public static readonly DependencyProperty PrintRowTemplateProperty;
		public static readonly DependencyProperty PrintAutoWidthProperty;
		public static readonly DependencyProperty PrintColumnHeadersProperty;
		public static readonly DependencyProperty PrintBandHeadersProperty;
		public static readonly DependencyProperty PrintColumnHeaderStyleProperty;
		public static readonly DependencyProperty PrintAllNodesProperty;
		public static readonly DependencyProperty PrintExpandButtonsProperty;
		public static readonly DependencyProperty PrintNodeImagesProperty;
		public static readonly DependencyProperty AllowDefaultContentForHierarchicalDataTemplateProperty;
		public static readonly DependencyProperty AlternateRowBackgroundProperty;
		protected static readonly DependencyPropertyKey ActualAlternateRowBackgroundPropertyKey;
		public static readonly DependencyProperty ActualAlternateRowBackgroundProperty;
		public static readonly DependencyProperty EvenRowBackgroundProperty;
		public static readonly DependencyProperty UseEvenRowBackgroundProperty;
		public static readonly DependencyProperty AlternationCountProperty;
		public static readonly DependencyProperty RestoreFocusOnExpandProperty;
		public static readonly DependencyProperty AllowChildNodeSourceUpdatesProperty;
		public static readonly DependencyProperty ExpandStateFieldNameProperty;
		public static readonly DependencyProperty ExpandCollapseNodesOnNavigationProperty;
		public static readonly DependencyProperty ExpandNodesOnFilteringProperty;
		public static readonly DependencyProperty ShowDataNavigatorProperty;
		public static readonly DependencyProperty AllowTreeIndentScrollingProperty;
		static TreeListView() {
			Type ownerType = typeof(TreeListView);
			TreeListViewSelectionControlWrapper.Register();
			#region common
			ColumnBandChooserTemplateProperty = TableViewBehavior.RegisterColumnBandChooserTemplateProperty(ownerType);
			FocusedColumnProperty = DependencyPropertyManager.Register("FocusedColumn", typeof(ColumnBase), ownerType, new FrameworkPropertyMetadata(null, OnFocusedColumnChanged, (d, e) => ((TreeListView)d).CoerceFocusedColumn((ColumnBase)e)));
			VisibleColumnsPropertyKey = DependencyPropertyManager.RegisterReadOnly("VisibleColumns", typeof(IList<ColumnBase>), ownerType, new FrameworkPropertyMetadata(null));
			VisibleColumnsProperty = VisibleColumnsPropertyKey.DependencyProperty;
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
			IndicatorHeaderWidthPropertyKey = TreeListViewBehavior.RegisterIndicatorHeaderWidthProperty(ownerType);
			IndicatorHeaderWidthProperty = IndicatorHeaderWidthPropertyKey.DependencyProperty;
			ActualDataRowTemplateSelectorPropertyKey = TreeListViewBehavior.RegisterActualDataRowTemplateSelectorProperty(ownerType);
			ActualDataRowTemplateSelectorProperty = ActualDataRowTemplateSelectorPropertyKey.DependencyProperty;
			BestFitMaxRowCountProperty = DependencyPropertyManager.Register("BestFitMaxRowCount", typeof(int), ownerType, new FrameworkPropertyMetadata(-1, null, (d, baseValue) => CoerceBestFitMaxRowCount(Convert.ToInt32(baseValue))));
			BestFitModeProperty = DependencyPropertyManager.Register("BestFitMode", typeof(BestFitMode), ownerType, new FrameworkPropertyMetadata(BestFitMode.Default));
			BestFitAreaProperty = DependencyPropertyManager.Register("BestFitArea", typeof(BestFitArea), ownerType, new FrameworkPropertyMetadata(BestFitArea.All));
			CustomBestFitEvent = EventManager.RegisterRoutedEvent("CustomBestFit", RoutingStrategy.Direct, typeof(TreeListCustomBestFitEventHandler), ownerType);		   
			FocusedRowBorderTemplateProperty = TreeListViewBehavior.RegisterFocusedRowBorderTemplateProperty(ownerType);
			AutoWidthProperty = TreeListViewBehavior.RegisterAutoWidthProperty(ownerType);
			LeftDataAreaIndentProperty = TreeListViewBehavior.RegisterLeftDataAreaIndentProperty(ownerType);
			RightDataAreaIndentProperty = TreeListViewBehavior.RegisterRightDataAreaIndentProperty(ownerType);
			ShowAutoFilterRowProperty = TreeListViewBehavior.RegisterShowAutoFilterRowProperty(ownerType);
			AllowCascadeUpdateProperty = TreeListViewBehavior.RegisterAllowCascadeUpdateProperty(ownerType);
			AllowPerPixelScrollingProperty = TreeListViewBehavior.RegisterAllowPerPixelScrollingProperty(ownerType);
			ScrollAnimationDurationProperty = TreeListViewBehavior.RegisterScrollAnimationDurationProperty(ownerType);
			ScrollAnimationModeProperty = TreeListViewBehavior.RegisterScrollAnimationModeProperty(ownerType);
			AllowScrollAnimationProperty = TreeListViewBehavior.RegisterAllowScrollAnimationProperty(ownerType);
			ExtendScrollBarToFixedColumnsProperty = TableViewBehavior.RegisterExtendScrollBarToFixedColumnsProperty(ownerType);
			FixedLeftVisibleColumnsPropertyKey = TreeListViewBehavior.RegisterFixedLeftVisibleColumnsProperty<ColumnBase>(ownerType);
			FixedLeftVisibleColumnsProperty = FixedLeftVisibleColumnsPropertyKey.DependencyProperty;
			FixedRightVisibleColumnsPropertyKey = TreeListViewBehavior.RegisterFixedRightVisibleColumnsProperty<ColumnBase>(ownerType);
			FixedRightVisibleColumnsProperty = FixedRightVisibleColumnsPropertyKey.DependencyProperty;
			FixedNoneVisibleColumnsPropertyKey = TreeListViewBehavior.RegisterFixedNoneVisibleColumnsProperty<ColumnBase>(ownerType);
			FixedNoneVisibleColumnsProperty = FixedNoneVisibleColumnsPropertyKey.DependencyProperty;
			HorizontalViewportPropertyKey = TreeListViewBehavior.RegisterHorizontalViewportProperty(ownerType);
			HorizontalViewportProperty = HorizontalViewportPropertyKey.DependencyProperty;
			FixedLineWidthProperty = TreeListViewBehavior.RegisterFixedLineWidthProperty(ownerType);
			ShowVerticalLinesProperty = TreeListViewBehavior.RegisterShowVerticalLinesProperty(ownerType);
			ShowHorizontalLinesProperty = TreeListViewBehavior.RegisterShowHorizontalLinesProperty(ownerType);
			RowDecorationTemplateProperty = TreeListViewBehavior.RegisterRowDecorationTemplateProperty(ownerType);
			DefaultDataRowTemplateProperty = TreeListViewBehavior.RegisterDefaultDataRowTemplateProperty(ownerType);
			DataRowTemplateProperty = TreeListViewBehavior.RegisterDataRowTemplateProperty(ownerType);
			DataRowTemplateSelectorProperty = TreeListViewBehavior.RegisterDataRowTemplateSelectorProperty(ownerType);
			RowIndicatorContentTemplateProperty = TreeListViewBehavior.RegisterRowIndicatorContentTemplateProperty(ownerType);
			AllowResizingProperty = TreeListViewBehavior.RegisterAllowResizingProperty(ownerType);
			AllowHorizontalScrollingVirtualizationProperty = TreeListViewBehavior.RegisterAllowHorizontalScrollingVirtualizationProperty(ownerType);
			RowStyleProperty = TreeListViewBehavior.RegisterRowStyleProperty(ownerType);
			ScrollingVirtualizationMarginPropertyKey = TreeListViewBehavior.RegisterScrollingVirtualizationMarginProperty(ownerType);
			ScrollingVirtualizationMarginProperty = ScrollingVirtualizationMarginPropertyKey.DependencyProperty;
			ScrollingHeaderVirtualizationMarginPropertyKey = TreeListViewBehavior.RegisterScrollingHeaderVirtualizationMarginProperty(ownerType);
			ScrollingHeaderVirtualizationMarginProperty = ScrollingHeaderVirtualizationMarginPropertyKey.DependencyProperty;
			RowMinHeightProperty = TreeListViewBehavior.RegisterRowMinHeightProperty(ownerType);
			HeaderPanelMinHeightProperty = TreeListViewBehavior.RegisterHeaderPanelMinHeightProperty(ownerType);
			AutoMoveRowFocusProperty = TreeListViewBehavior.RegisterAutoMoveRowFocusProperty(ownerType);
			AllowBestFitProperty = TreeListViewBehavior.RegisterAllowBestFitProperty(ownerType);
			ShowIndicatorProperty = TreeListViewBehavior.RegisterShowIndicatorProperty(ownerType);
			ActualShowIndicatorPropertyKey = TreeListViewBehavior.RegisterActualShowIndicatorProperty(ownerType);
			ActualShowIndicatorProperty = ActualShowIndicatorPropertyKey.DependencyProperty;
			IndicatorWidthProperty = TreeListViewBehavior.RegisterIndicatorWidthProperty(ownerType);
			ActualIndicatorWidthPropertyKey = TreeListViewBehavior.RegisterActualIndicatorWidthPropertyKey(ownerType);
			ActualIndicatorWidthProperty = ActualIndicatorWidthPropertyKey.DependencyProperty;
			ShowTotalSummaryIndicatorIndentPropertyKey = TableViewBehavior.RegisterShowTotalSummaryIndicatorIndentPropertyKey(ownerType);
			ShowTotalSummaryIndicatorIndentProperty = ShowTotalSummaryIndicatorIndentPropertyKey.DependencyProperty;
			MultiSelectModeProperty = TreeListViewBehavior.RegisterMultiSelectModeProperty(ownerType);
			UseIndicatorForSelectionProperty = TreeListViewBehavior.RegisterUseIndicatorForSelectionProperty(ownerType);
			AllowFixedColumnMenuProperty = TreeListViewBehavior.RegisterAllowFixedColumnMenuProperty(ownerType);
			AllowScrollHeadersProperty = TreeListViewBehavior.RegisterAllowScrollHeadersProperty(ownerType);
			ShowBandsPanelProperty = TreeListViewBehavior.RegisterShowBandsPanelProperty(ownerType);
			AllowChangeColumnParentProperty = TreeListViewBehavior.RegisterAllowChangeColumnParentProperty(ownerType);
			AllowChangeBandParentProperty = TreeListViewBehavior.RegisterAllowChangeBandParentProperty(ownerType);
			ShowBandsInCustomizationFormProperty = TreeListViewBehavior.RegisterShowBandsInCustomizationFormProperty(ownerType);
			AllowBandMovingProperty = TreeListViewBehavior.RegisterAllowBandMovingProperty(ownerType);
			AllowBandResizingProperty = TreeListViewBehavior.RegisterAllowBandResizingProperty(ownerType);
			AllowAdvancedVerticalNavigationProperty = TreeListViewBehavior.RegisterAllowAdvancedVerticalNavigationProperty(ownerType);
			AllowAdvancedHorizontalNavigationProperty = TreeListViewBehavior.RegisterAllowAdvancedHorizontalNavigationProperty(ownerType);
			ColumnChooserBandsSortOrderComparerProperty = TreeListViewBehavior.RegisterColumnChooserBandsSortOrderComparerProperty(ownerType);
			BandHeaderTemplateProperty = TreeListViewBehavior.RegisterBandHeaderTemplateProperty(ownerType);
			BandHeaderTemplateSelectorProperty = TreeListViewBehavior.RegisterBandHeaderTemplateSelectorProperty(ownerType);
			BandHeaderToolTipTemplateProperty = TreeListViewBehavior.RegisterBandHeaderToolTipTemplateProperty(ownerType);
			PrintBandHeaderStyleProperty = TreeListViewBehavior.RegisterPrintBandHeaderStyleProperty(ownerType);
			AlternateRowBackgroundProperty = TableViewBehavior.RegisterAlternateRowBackgroundProperty(ownerType);
			ActualAlternateRowBackgroundPropertyKey = TableViewBehavior.RegisterActualAlternateRowBackgroundProperty(ownerType);
			ActualAlternateRowBackgroundProperty = ActualAlternateRowBackgroundPropertyKey.DependencyProperty;
			EvenRowBackgroundProperty = TableViewBehavior.RegisterEvenRowBackgroundProperty(ownerType);
			UseEvenRowBackgroundProperty = TableViewBehavior.RegisterUseEvenRowBackgroundProperty(ownerType);
			AlternationCountProperty = TableViewBehavior.RegisterAlternationCountProperty(ownerType);
			AutoFilterRowCellStyleProperty = DependencyPropertyManager.Register("AutoFilterRowCellStyle", typeof(Style), ownerType, new PropertyMetadata(null, OnUpdateColumnsAppearance));
			AllowConditionalFormattingMenuProperty = TreeListViewBehavior.RegisterAllowConditionalFormattingMenuProperty(ownerType);
			AllowConditionalFormattingManagerProperty = TreeListViewBehavior.RegisterAllowConditionalFormattingManagerProperty(ownerType);
			PredefinedFormatsProperty = TreeListViewBehavior.RegisterPredefinedFormatsProperty(ownerType);
			PredefinedColorScaleFormatsProperty = TreeListViewBehavior.RegisterPredefinedColorScaleFormatsProperty(ownerType);
			PredefinedDataBarFormatsProperty = TreeListViewBehavior.RegisterPredefinedDataBarFormatsProperty(ownerType);
			PredefinedIconSetFormatsProperty = TreeListViewBehavior.RegisterPredefinedIconSetFormatsProperty(ownerType);
			FormatConditionDialogServiceTemplateProperty = TreeListViewAssignableDialogServiceHelper.RegisterServiceTemplateProperty("FormatConditionDialogServiceTemplate");
			ConditionalFormattingManagerServiceTemplateProperty = TreeListViewAssignableDialogServiceHelper.RegisterServiceTemplateProperty("ConditionalFormattingManagerServiceTemplate");
			#endregion
		   RowIndentProperty = DependencyPropertyManager.Register("RowIndent", typeof(double), ownerType,
				new FrameworkPropertyMetadata(0d,
					(d, e) => {
						TreeListView view = (TreeListView)d;
						view.RebuildVisibleColumns();
						view.UpdateRows();
					},
					(d, e) => { return Math.Round((double)e); })
				);
			KeyFieldNameProperty = DependencyPropertyManager.Register("KeyFieldName", typeof(string), ownerType, new FrameworkPropertyMetadata(String.Empty, (d, e) => ((TreeListView)d).DoRefresh()));
			ParentFieldNameProperty = DependencyPropertyManager.Register("ParentFieldName", typeof(string), ownerType, new FrameworkPropertyMetadata(String.Empty, (d, e) => ((TreeListView)d).DoRefresh()));
			CheckBoxFieldNameProperty = DependencyPropertyManager.Register("CheckBoxFieldName", typeof(string), ownerType, new FrameworkPropertyMetadata(String.Empty, (d, e) => ((TreeListView)d).OnCheckBoxFieldNameChanged()));
			CheckBoxValueConverterProperty = DependencyPropertyManager.Register("CheckBoxValueConverter", typeof(IValueConverter), ownerType, new PropertyMetadata(null));
			ImageFieldNameProperty = DependencyPropertyManager.Register("ImageFieldName", typeof(string), ownerType, new FrameworkPropertyMetadata(String.Empty, (d, e) => ((TreeListView)d).UpdateContentLayout()));
			NodeImageSelectorProperty = DependencyPropertyManager.Register("NodeImageSelector", typeof(TreeListNodeImageSelector), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((TreeListView)d).UpdateContentLayout()));
			RootValueProperty = DependencyPropertyManager.Register("RootValue", typeof(object), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((TreeListView)d).OnRootValueChanged()));
			AutoPopulateServiceColumnsProperty = DependencyPropertyManager.Register("AutoPopulateServiceColumns", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((TreeListView)d).OnAutoPopulateServiceColumnsChanged()));
			FilterModeProperty = DependencyPropertyManager.Register("FilterMode", typeof(TreeListFilterMode), ownerType, new FrameworkPropertyMetadata(TreeListFilterMode.Smart, (d, e) => ((TreeListView)d).OnFilterModeChanged()));
			FocusedNodeProperty = DependencyPropertyManager.Register("FocusedNode", typeof(TreeListNode), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((TreeListView)d).OnFocusedNodeChanged()));
			RowPresenterMarginProperty = DependencyPropertyManager.Register("RowPresenterMargin", typeof(Thickness), ownerType, new FrameworkPropertyMetadata(new Thickness(0d), (d, e) => ((TreeListView)d).UpdateContentLayout()));
			TreeLineStyleProperty = DependencyPropertyManager.Register("TreeLineStyle", typeof(TreeListLineStyle), ownerType, new FrameworkPropertyMetadata(TreeListLineStyle.Solid));
			ShowNodeImagesProperty = DependencyPropertyManager.Register("ShowNodeImages", typeof(bool), ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((TreeListView)d).OnIndentItemChanged()));
			NodeImageSizeProperty = DependencyPropertyManager.Register("NodeImageSize", typeof(Size), ownerType, new FrameworkPropertyMetadata(new Size(16, 16), (d, e) => ((TreeListView)d).OnIndentItemChanged()));
			ShowCheckboxesProperty = DependencyPropertyManager.Register("ShowCheckboxes", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((TreeListView)d).OnIndentItemChanged()));
			AllowIndeterminateCheckStateProperty = DependencyPropertyManager.Register("AllowIndeterminateCheckState", typeof(bool), ownerType, new PropertyMetadata(false, (d, e) => ((TreeListView)d).OnIndentItemChanged()));
			AllowRecursiveNodeCheckingProperty = DependencyPropertyManager.Register("AllowRecursiveNodeChecking", typeof(bool), ownerType, new PropertyMetadata(false));
			ShowExpandButtonsProperty = DependencyPropertyManager.Register("ShowExpandButtons", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((TreeListView)d).OnIndentItemChanged()));
			ShowRootIndentProperty = DependencyPropertyManager.Register("ShowRootIndent", typeof(bool), ownerType, new FrameworkPropertyMetadata(true, (d, e) => ((TreeListView)d).OnIndentItemChanged()));
			AutoExpandAllNodesProperty = DependencyPropertyManager.Register("AutoExpandAllNodes", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			ExpandStateFieldNameProperty = DependencyPropertyManager.Register("ExpandStateFieldName", typeof(string), ownerType, new FrameworkPropertyMetadata(null, (d, e) => ((TreeListView)d).OnExpandStateBindingChanged()));
			ExpandCollapseNodesOnNavigationProperty = DependencyPropertyManager.Register("ExpandCollapseNodesOnNavigation", typeof(bool?), ownerType, new FrameworkPropertyMetadata(null));
			ExpandNodesOnFilteringProperty = DependencyPropertyManager.Register("ExpandNodesOnFiltering", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			AllowDefaultContentForHierarchicalDataTemplateProperty = DependencyPropertyManager.Register("AllowDefaultContentForHierarchicalDataTemplate", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			NodeExpandingEvent = EventManager.RegisterRoutedEvent("NodeExpanding", RoutingStrategy.Direct, typeof(TreeListNodeAllowEventHandler), ownerType);
			NodeExpandedEvent = EventManager.RegisterRoutedEvent("NodeExpanded", RoutingStrategy.Direct, typeof(TreeListNodeEventHandler), ownerType);
			NodeCollapsingEvent = EventManager.RegisterRoutedEvent("NodeCollapsing", RoutingStrategy.Direct, typeof(TreeListNodeAllowEventHandler), ownerType);
			NodeCollapsedEvent = EventManager.RegisterRoutedEvent("NodeCollapsed", RoutingStrategy.Direct, typeof(TreeListNodeEventHandler), ownerType);
			NodeCheckStateChangedEvent = EventManager.RegisterRoutedEvent("NodeCheckStateChanged", RoutingStrategy.Direct, typeof(TreeListNodeEventHandler), ownerType);
			StartSortingEvent = EventManager.RegisterRoutedEvent("StartSorting", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			EndSortingEvent = EventManager.RegisterRoutedEvent("EndSorting", RoutingStrategy.Direct, typeof(RoutedEventHandler), ownerType);
			ShowingEditorEvent = EventManager.RegisterRoutedEvent("ShowingEditor", RoutingStrategy.Direct, typeof(TreeListShowingEditorEventHandler), ownerType);
			ShownEditorEvent = EventManager.RegisterRoutedEvent("ShownEditor", RoutingStrategy.Direct, typeof(TreeListEditorEventHandler), ownerType);
			HiddenEditorEvent = EventManager.RegisterRoutedEvent("HiddenEditor", RoutingStrategy.Direct, typeof(TreeListEditorEventHandler), ownerType);
			RowDoubleClickEvent = EventManager.RegisterRoutedEvent("RowDoubleClick", RoutingStrategy.Direct, typeof(RowDoubleClickEventHandler), ownerType);
			SelectionChangedEvent = EventManager.RegisterRoutedEvent("SelectionChanged", RoutingStrategy.Direct, typeof(TreeListSelectionChangedEventHandler), ownerType);
			CellValueChangedEvent = EventManager.RegisterRoutedEvent("CellValueChanged", RoutingStrategy.Direct, typeof(TreeListCellValueChangedEventHandler), ownerType);
			CellValueChangingEvent = EventManager.RegisterRoutedEvent("CellValueChanging", RoutingStrategy.Direct, typeof(TreeListCellValueChangedEventHandler), ownerType);
			CustomScrollAnimationEvent = TreeListViewBehavior.RegisterCustomScrollAnimationEvent(ownerType);
			InvalidNodeExceptionEvent = EventManager.RegisterRoutedEvent("InvalidNodeException", RoutingStrategy.Direct, typeof(TreeListInvalidNodeExceptionEventHandler), ownerType);
			ValidateNodeEvent = EventManager.RegisterRoutedEvent("ValidateNode", RoutingStrategy.Direct, typeof(TreeListNodeValidationEventHandler), ownerType);		  
			CopyingToClipboardEvent = EventManager.RegisterRoutedEvent("CopyingToClipboard", RoutingStrategy.Direct, typeof(TreeListCopyingToClipboardEventHandler), ownerType);
			TreeDerivationModeProperty = DependencyPropertyManager.Register("TreeDerivationMode",
				typeof(TreeDerivationMode),
				ownerType,
				new FrameworkPropertyMetadata(TreeDerivationMode.Selfreference, (o, e) => ((TreeListView)o).OnItemsSourceModeChanged()));
			ChildNodesPathProperty = DependencyPropertyManager.Register("ChildNodesPath",
				typeof(string),
				ownerType,
				new FrameworkPropertyMetadata(String.Empty, (o, e) => ((TreeListView)o).ChildrenPropertyUpdate()));
			ChildNodesSelectorProperty = DependencyPropertyManager.Register("ChildNodesSelector",
				typeof(IChildNodesSelector),
				ownerType,
				new FrameworkPropertyMetadata(null, (o, e) => ((TreeListView)o).OnChildNodesSelectorChanged()));
			EnableDynamicLoadingProperty = DependencyPropertyManager.Register("EnableDynamicLoading", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			FetchSublevelChildrenOnExpandProperty = DependencyPropertyManager.Register("FetchSublevelChildrenOnExpand", typeof(bool), ownerType, new PropertyMetadata(true));
			VerticalScrollbarVisibilityProperty = DependencyPropertyManager.Register("VerticalScrollbarVisibility", typeof(ScrollBarVisibility), ownerType, new PropertyMetadata(ScrollBarVisibility.Visible));
			HorizontalScrollbarVisibilityProperty = DependencyPropertyManager.Register("HorizontalScrollbarVisibility", typeof(ScrollBarVisibility), ownerType, new PropertyMetadata(ScrollBarVisibility.Auto));
			PrintRowTemplateProperty = DependencyPropertyManager.Register("PrintRowTemplate", typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			PrintAutoWidthProperty = DependencyPropertyManager.Register("PrintAutoWidth", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			PrintColumnHeadersProperty = DependencyPropertyManager.Register("PrintColumnHeaders", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			PrintBandHeadersProperty = DependencyPropertyManager.Register("PrintBandHeaders", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			PrintColumnHeaderStyleProperty = DependencyPropertyManager.Register("PrintColumnHeaderStyle", typeof(Style), ownerType, new FrameworkPropertyMetadata(null, OnUpdateColumnsAppearance));
			PrintAllNodesProperty = DependencyPropertyManager.Register("PrintAllNodes", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			PrintExpandButtonsProperty = DependencyPropertyManager.Register("PrintExpandButtons", typeof(bool), ownerType, new FrameworkPropertyMetadata(true));
			PrintNodeImagesProperty = DependencyPropertyManager.Register("PrintNodeImages", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			RestoreFocusOnExpandProperty = DependencyPropertyManager.Register("RestoreFocusOnExpand", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (s,e) => ((TreeListView)s).OnRestoreFocusOnExpandChanged()));
			AllowChildNodeSourceUpdatesProperty = DependencyPropertyManager.Register("AllowChildNodeSourceUpdates", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
			ShowDataNavigatorProperty = DependencyPropertyManager.Register("ShowDataNavigator", typeof(bool), ownerType, new FrameworkPropertyMetadata(false));
			AllowTreeIndentScrollingProperty = DependencyProperty.Register("AllowTreeIndentScrolling", typeof(bool), ownerType, new PropertyMetadata(false, (o, e) => ((TreeListView)o).UpdateActualAllowTreeIndentScrolling()));
#if !SL
			UseLightweightTemplatesProperty = TableViewBehavior.RegisterUseLightweightTemplatesProperty(ownerType);
			RowDetailsTemplateProperty = TableViewBehavior.RegisterRowDetailsTemplateProperty(ownerType);
			RowDetailsTemplateSelectorProperty = TableViewBehavior.RegisterRowDetailsTemplateSelectorProperty(ownerType);
			ActualRowDetailsTemplateSelectorPropertyKey = TableViewBehavior.RegisterActualRowDetailsTemplateSelectorProperty(ownerType);
			ActualRowDetailsTemplateSelectorProperty = ActualRowDetailsTemplateSelectorPropertyKey.DependencyProperty;
			RowDetailsVisibilityModeProperty = TableViewBehavior.RegisterRowDetailsVisibilityModeProperty(ownerType);
#endif
			EventManager.RegisterClassHandler(ownerType, DXSerializer.CreateCollectionItemEvent, new XtraCreateCollectionItemEventHandler((s, e) => ((TreeListView)s).OnDeserializeCreateCollectionItem(e)));
		}
		internal override void UpdateActualFadeSelectionOnLostFocus(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			TreeListViewBehavior.OnFadeSelectionOnLostFocusChanged(d, e);
		}
		protected virtual void OnFocusedNodeChanged() {
			if(FocusedRowHandle != TreeListControl.AutoFilterRowHandle && !FocusedRowHandleChangedLocker.IsLocked) {
				int focusedRowHandle = FocusedNode != null ? TreeListDataProvider.GetRowHandleByNode(FocusedNode) : GridControl.InvalidRowHandle;
				SetFocusedRowHandle(focusedRowHandle);
			}
		}
		protected void OnRestoreFocusOnExpandChanged() {
			if(!RestoreFocusOnExpand)
				ClearFocusedNodeSave();
		}
		void OnAutoPopulateServiceColumnsChanged() {
			if (DataControl != null && DataControl.AutoGenerateColumns != AutoGenerateColumnsMode.None)
				DataControl.PopulateColumns();
		}
		void OnCheckBoxFieldNameChanged() {
			if(DataControl != null && !DataControl.IsLoading) 
				TreeListDataProvider.InitNodesIsChecked();
			UpdateRows();
		}
		#endregion
		#region common public properties
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewFixedNoneContentWidth"),
#endif
 CloneDetailMode(CloneDetailMode.Force)]
		public double FixedNoneContentWidth {
			get { return (double)GetValue(FixedNoneContentWidthProperty); }
			private set { this.SetValue(FixedNoneContentWidthPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewTotalSummaryFixedNoneContentWidth")]
#endif
		public double TotalSummaryFixedNoneContentWidth {
			get { return (double)GetValue(TotalSummaryFixedNoneContentWidthProperty); }
			private set { this.SetValue(TotalSummaryFixedNoneContentWidthPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewVerticalScrollBarWidth"),
#endif
 EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public double VerticalScrollBarWidth {
			get { return (double)GetValue(VerticalScrollBarWidthProperty); }
			private set { this.SetValue(VerticalScrollBarWidthPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewFixedLeftContentWidth"),
#endif
 CloneDetailMode(CloneDetailMode.Force)]
		public double FixedLeftContentWidth {
			get { return (double)GetValue(FixedLeftContentWidthProperty); }
			private set { this.SetValue(FixedLeftContentWidthPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewFixedRightContentWidth"),
#endif
 CloneDetailMode(CloneDetailMode.Force)]
		public double FixedRightContentWidth {
			get { return (double)GetValue(FixedRightContentWidthProperty); }
			private set { this.SetValue(FixedRightContentWidthPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewTotalGroupAreaIndent"),
#endif
 CloneDetailMode(CloneDetailMode.Force)]
		public double TotalGroupAreaIndent {
			get { return (double)GetValue(TotalGroupAreaIndentProperty); }
			private set { this.SetValue(TotalGroupAreaIndentPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewIndicatorHeaderWidth")]
#endif
		public double IndicatorHeaderWidth {
			get { return (double)GetValue(IndicatorHeaderWidthProperty); }
			private set { this.SetValue(IndicatorHeaderWidthPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewActualDataRowTemplateSelector")]
#endif
		public DataTemplateSelector ActualDataRowTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualDataRowTemplateSelectorProperty); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewFocusedRowBorderTemplate"),
#endif
 Category(Categories.Appearance)]
		public ControlTemplate FocusedRowBorderTemplate {
			get { return (ControlTemplate)GetValue(FocusedRowBorderTemplateProperty); }
			set { SetValue(FocusedRowBorderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewColumnBandChooserTemplate"),
#endif
 Category(Categories.Appearance)]
		public ControlTemplate ColumnBandChooserTemplate {
			get { return (ControlTemplate)GetValue(ColumnBandChooserTemplateProperty); }
			set { SetValue(ColumnBandChooserTemplateProperty, value); }
		}
		[Obsolete("Use the DataControlBase.SelectionMode property instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false), XtraSerializableProperty, Category(Categories.OptionsSelection)]
		public TableViewSelectMode MultiSelectMode {
			get { return (TableViewSelectMode)GetValue(MultiSelectModeProperty); }
			set { SetValue(MultiSelectModeProperty, value); }
		}
		[XtraSerializableProperty]
		public bool FetchSublevelChildrenOnExpand {
			get { return (bool)GetValue(FetchSublevelChildrenOnExpandProperty); }
			set { SetValue(FetchSublevelChildrenOnExpandProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewUseIndicatorForSelection"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsSelection)]
		public bool UseIndicatorForSelection {
			get { return (bool)GetValue(UseIndicatorForSelectionProperty); }
			set { SetValue(UseIndicatorForSelectionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAllowFixedColumnMenu"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBehavior)]
		public bool AllowFixedColumnMenu {
			get { return (bool)GetValue(AllowFixedColumnMenuProperty); }
			set { SetValue(AllowFixedColumnMenuProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAllowScrollHeaders"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBehavior)]
		public bool AllowScrollHeaders {
			get { return (bool)GetValue(AllowScrollHeadersProperty); }
			set { SetValue(AllowScrollHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewShowBandsPanel"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool ShowBandsPanel {
			get { return (bool)GetValue(ShowBandsPanelProperty); }
			set { SetValue(ShowBandsPanelProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAllowChangeColumnParent"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool AllowChangeColumnParent {
			get { return (bool)GetValue(AllowChangeColumnParentProperty); }
			set { SetValue(AllowChangeColumnParentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAllowChangeBandParent"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool AllowChangeBandParent {
			get { return (bool)GetValue(AllowChangeBandParentProperty); }
			set { SetValue(AllowChangeBandParentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewShowBandsInCustomizationForm"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool ShowBandsInCustomizationForm {
			get { return (bool)GetValue(ShowBandsInCustomizationFormProperty); }
			set { SetValue(ShowBandsInCustomizationFormProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAllowBandMoving"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool AllowBandMoving {
			get { return (bool)GetValue(AllowBandMovingProperty); }
			set { SetValue(AllowBandMovingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAllowBandResizing"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool AllowBandResizing {
			get { return (bool)GetValue(AllowBandResizingProperty); }
			set { SetValue(AllowBandResizingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAllowAdvancedVerticalNavigation"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public bool AllowAdvancedVerticalNavigation {
			get { return (bool)GetValue(AllowAdvancedVerticalNavigationProperty); }
			set { SetValue(AllowAdvancedVerticalNavigationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAllowAdvancedHorizontalNavigation"),
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
	DevExpressXpfGridLocalizedDescription("TreeListViewBandHeaderTemplate"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public DataTemplate BandHeaderTemplate {
			get { return (DataTemplate)GetValue(BandHeaderTemplateProperty); }
			set { SetValue(BandHeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewBandHeaderTemplateSelector"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public DataTemplateSelector BandHeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(BandHeaderTemplateSelectorProperty); }
			set { SetValue(BandHeaderTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewBandHeaderToolTipTemplate"),
#endif
 XtraSerializableProperty, Category(Categories.OptionsBands)]
		public DataTemplate BandHeaderToolTipTemplate {
			get { return (DataTemplate)GetValue(BandHeaderToolTipTemplateProperty); }
			set { SetValue(BandHeaderToolTipTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewPrintBandHeaderStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style PrintBandHeaderStyle {
			get { return (Style)GetValue(PrintBandHeaderStyleProperty); }
			set { SetValue(PrintBandHeaderStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewScrollingVirtualizationMargin"),
#endif
 Browsable(false)]
		public Thickness ScrollingVirtualizationMargin {
			get { return (Thickness)GetValue(ScrollingVirtualizationMarginProperty); }
			internal set { this.SetValue(ScrollingVirtualizationMarginPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewScrollingHeaderVirtualizationMargin"),
#endif
 Browsable(false)]
		public Thickness ScrollingHeaderVirtualizationMargin {
			get { return (Thickness)GetValue(ScrollingHeaderVirtualizationMarginProperty); }
			internal set { this.SetValue(ScrollingHeaderVirtualizationMarginPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewRowStyle"),
#endif
 Category(Categories.Appearance)]
		public Style RowStyle {
			get { return (Style)GetValue(RowStyleProperty); }
			set { SetValue(RowStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewShowAutoFilterRow"),
#endif
 Category(Categories.OptionsFilter), XtraSerializableProperty]
		public bool ShowAutoFilterRow {
			get { return (bool)GetValue(ShowAutoFilterRowProperty); }
			set { SetValue(ShowAutoFilterRowProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAllowCascadeUpdate"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool AllowCascadeUpdate {
			get { return (bool)GetValue(AllowCascadeUpdateProperty); }
			set { SetValue(AllowCascadeUpdateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAllowPerPixelScrolling"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool AllowPerPixelScrolling {
			get { return (bool)GetValue(AllowPerPixelScrollingProperty); }
			set { SetValue(AllowPerPixelScrollingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewScrollAnimationDuration"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public double ScrollAnimationDuration {
			get { return (double)GetValue(ScrollAnimationDurationProperty); }
			set { SetValue(ScrollAnimationDurationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewScrollAnimationMode"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public ScrollAnimationMode ScrollAnimationMode {
			get { return (ScrollAnimationMode)GetValue(ScrollAnimationModeProperty); }
			set { SetValue(ScrollAnimationModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAllowScrollAnimation"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool AllowScrollAnimation {
			get { return (bool)GetValue(AllowScrollAnimationProperty); }
			set { SetValue(AllowScrollAnimationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewExtendScrollBarToFixedColumns"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty, CloneDetailMode(CloneDetailMode.Skip)]
		public bool ExtendScrollBarToFixedColumns {
			get { return (bool)GetValue(ExtendScrollBarToFixedColumnsProperty); }
			set { SetValue(ExtendScrollBarToFixedColumnsProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewAutoFilterRowData")]
#endif
		public RowData AutoFilterRowData { get { return ((TableViewBehavior)ViewBehavior).AutoFilterRowData; } }
		[Browsable(false)]
		public RowData NewItemRowData { get; private set; }
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAllowHorizontalScrollingVirtualization"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AllowHorizontalScrollingVirtualization {
			get { return (bool)GetValue(AllowHorizontalScrollingVirtualizationProperty); }
			set { SetValue(AllowHorizontalScrollingVirtualizationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewRowMinHeight"),
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
	DevExpressXpfGridLocalizedDescription("TreeListViewRowDecorationTemplate"),
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
	DevExpressXpfGridLocalizedDescription("TreeListViewDataRowTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate DataRowTemplate {
			get { return (DataTemplate)GetValue(DataRowTemplateProperty); }
			set { SetValue(DataRowTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewDataRowTemplateSelector"),
#endif
 DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Category(Categories.Appearance)]
		public DataTemplateSelector DataRowTemplateSelector {
			get { return (DataTemplateSelector)GetValue(DataRowTemplateSelectorProperty); }
			set { SetValue(DataRowTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewRowIndicatorContentTemplate"),
#endif
 Category(Categories.Appearance)]
		public DataTemplate RowIndicatorContentTemplate {
			get { return (DataTemplate)GetValue(RowIndicatorContentTemplateProperty); }
			set { SetValue(RowIndicatorContentTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAutoMoveRowFocus"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AutoMoveRowFocus {
			get { return (bool)GetValue(AutoMoveRowFocusProperty); }
			set { SetValue(AutoMoveRowFocusProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewBestFitMaxRowCount"),
#endif
 Category(Categories.BestFit), XtraSerializableProperty]
		public int BestFitMaxRowCount {
			get { return (int)GetValue(BestFitMaxRowCountProperty); }
			set { SetValue(BestFitMaxRowCountProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewBestFitMode"),
#endif
 Category(Categories.BestFit), XtraSerializableProperty]
		public BestFitMode BestFitMode {
			get { return (BestFitMode)GetValue(BestFitModeProperty); }
			set { SetValue(BestFitModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewBestFitArea"),
#endif
 Category(Categories.BestFit), XtraSerializableProperty]
		public BestFitArea BestFitArea {
			get { return (BestFitArea)GetValue(BestFitAreaProperty); }
			set { SetValue(BestFitAreaProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAllowBestFit"),
#endif
 Category(Categories.BestFit), XtraSerializableProperty]
		public bool AllowBestFit {
			get { return (bool)GetValue(AllowBestFitProperty); }
			set { SetValue(AllowBestFitProperty, value); }
		}
		[Category(Categories.BestFit)]
		public event TreeListCustomBestFitEventHandler CustomBestFit {
			add { AddHandler(CustomBestFitEvent, value); }
			remove { RemoveHandler(CustomBestFitEvent, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewShowIndicator"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool ShowIndicator {
			get { return (bool)GetValue(ShowIndicatorProperty); }
			set { SetValue(ShowIndicatorProperty, value); }
		}
		public bool ActualShowIndicator {
			get { return (bool)GetValue(ActualShowIndicatorProperty); }
			protected set { this.SetValue(ActualShowIndicatorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewIndicatorWidth"),
#endif
 Category(Categories.Appearance), XtraSerializableProperty]
		public double IndicatorWidth {
			get { return (double)GetValue(IndicatorWidthProperty); }
			set { SetValue(IndicatorWidthProperty, value); }
		}
		public double ActualIndicatorWidth {
			get { return (double)GetValue(ActualIndicatorWidthProperty); }
			protected set { this.SetValue(ActualIndicatorWidthPropertyKey, value); }
		}	
		[EditorBrowsable(EditorBrowsableState.Never)]
		public double ExpandDetailButtonWidth { get { return 0; } }
		double ITableView.ActualExpandDetailButtonWidth { get { return 0; } }
		Thickness ITableView.ActualDetailMargin { get { return FillControl.EmptyThickness; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public double ActualExpandDetailHeaderWidth { get { return 0; } }
		public bool ShowTotalSummaryIndicatorIndent {
			get { return (bool)GetValue(ShowTotalSummaryIndicatorIndentProperty); }
			protected set { this.SetValue(ShowTotalSummaryIndicatorIndentPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewShowVerticalLines"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool ShowVerticalLines {
			get { return (bool)base.GetValue(ShowVerticalLinesProperty); }
			set { SetValue(ShowVerticalLinesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewShowHorizontalLines"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool ShowHorizontalLines {
			get { return (bool)base.GetValue(ShowHorizontalLinesProperty); }
			set { SetValue(ShowHorizontalLinesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAutoWidth"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AutoWidth {
			get { return (bool)base.GetValue(AutoWidthProperty); }
			set { SetValue(AutoWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAllowResizing"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AllowResizing {
			get { return (bool)GetValue(AllowResizingProperty); }
			set { SetValue(AllowResizingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewFixedLineWidth"),
#endif
 Category(Categories.Appearance), XtraSerializableProperty]
		public double FixedLineWidth {
			get { return (double)base.GetValue(FixedLineWidthProperty); }
			set { SetValue(FixedLineWidthProperty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double LeftDataAreaIndent {
			get { return (double)GetValue(LeftDataAreaIndentProperty); }
			set { SetValue(LeftDataAreaIndentProperty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), EditorBrowsable(EditorBrowsableState.Never)]
		public Thickness RowPresenterMargin {
			get { return (Thickness)GetValue(RowPresenterMarginProperty); }
			set { SetValue(RowPresenterMarginProperty, value); }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public double RightDataAreaIndent {
			get { return (double)GetValue(RightDataAreaIndentProperty); }
			set { SetValue(RightDataAreaIndentProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewFixedLeftVisibleColumns")]
#endif
		public IList<ColumnBase> FixedLeftVisibleColumns {
			get { return (IList<ColumnBase>)GetValue(FixedLeftVisibleColumnsProperty); }
			private set { this.SetValue(FixedLeftVisibleColumnsPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewFixedRightVisibleColumns")]
#endif
		public IList<ColumnBase> FixedRightVisibleColumns {
			get { return (IList<ColumnBase>)GetValue(FixedRightVisibleColumnsProperty); }
			private set { this.SetValue(FixedRightVisibleColumnsPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewFixedNoneVisibleColumns")]
#endif
		public IList<ColumnBase> FixedNoneVisibleColumns {
			get { return (IList<ColumnBase>)GetValue(FixedNoneVisibleColumnsProperty); }
			private set { this.SetValue(FixedNoneVisibleColumnsPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewHorizontalViewport")]
#endif
		public double HorizontalViewport {
			get { return (double)GetValue(HorizontalViewportProperty); }
			private set { this.SetValue(HorizontalViewportPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAutoFilterRowCellStyle"),
#endif
 Category(Categories.Appearance)]
		public Style AutoFilterRowCellStyle {
			get { return (Style)GetValue(AutoFilterRowCellStyleProperty); }
			set { SetValue(AutoFilterRowCellStyleProperty, value); }
		}
		[Category(Categories.OptionsBehavior), XtraSerializableProperty, XtraResetProperty(ResetPropertyMode.None)]
		public string ChildNodesPath {
			get { return (string)GetValue(ChildNodesPathProperty); }
			set { SetValue(ChildNodesPathProperty, value); }
		}
		[Category(Categories.OptionsBehavior), XtraSerializableProperty, XtraResetProperty(ResetPropertyMode.None)]
		public TreeDerivationMode TreeDerivationMode {
			get { return (TreeDerivationMode)GetValue(TreeDerivationModeProperty); }
			set { SetValue(TreeDerivationModeProperty, value); }
		}
		[Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool EnableDynamicLoading {
			get { return (bool)GetValue(EnableDynamicLoadingProperty); }
			set { SetValue(EnableDynamicLoadingProperty, value); }
		}
		[Category(Categories.OptionsBehavior)]
		public IChildNodesSelector ChildNodesSelector {
			get { return (IChildNodesSelector)GetValue(ChildNodesSelectorProperty); }
			set { SetValue(ChildNodesSelectorProperty, value); }
		}
		[Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public bool AutoExpandAllNodes {
			get { return (bool)GetValue(AutoExpandAllNodesProperty); }
			set { SetValue(AutoExpandAllNodesProperty, value); }
		}
		[Category(Categories.OptionsBehavior)]
		public Binding ExpandStateBinding {
			get { return expandStateBinding; }
			set {
				if(ExpandStateBinding == value) return;
				expandStateBinding = value;
				OnExpandStateBindingChanged();
			}
		}
		[Category(Categories.OptionsBehavior)]
		public string ExpandStateFieldName {
			get { return (string)GetValue(ExpandStateFieldNameProperty); }
			set { SetValue(ExpandStateFieldNameProperty, value); }
		}
		[Category(Categories.OptionsBehavior)]
		public bool? ExpandCollapseNodesOnNavigation {
			get { return (bool?)GetValue(ExpandCollapseNodesOnNavigationProperty); }
			set { SetValue(ExpandCollapseNodesOnNavigationProperty, value); }
		}
		protected internal virtual bool ShouldExpandCollapseNodesOnNavigation { get { return ExpandCollapseNodesOnNavigation.GetValueOrDefault(NavigationStyle == GridViewNavigationStyle.Row); } }
		[Category(Categories.OptionsBehavior)]
		public bool ExpandNodesOnFiltering {
			get { return (bool)GetValue(ExpandNodesOnFilteringProperty); }
			set { SetValue(ExpandNodesOnFilteringProperty, value); }
		}
		protected internal virtual void OnExpandStateBindingChanged() {
			TreeListDataProvider.UpdateNodesExpandState(Nodes);
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool HasDetailViews { get { return false; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool ActualShowDetailButtons { get { return false; } }
		public ColumnPosition ExpandColumnPosition { get { return ColumnPosition.Middle; } }
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
		public ScrollBarVisibility VerticalScrollbarVisibility {
			get { return (ScrollBarVisibility)GetValue(VerticalScrollbarVisibilityProperty); }
			set { SetValue(VerticalScrollbarVisibilityProperty, value); }
		}
		public ScrollBarVisibility HorizontalScrollbarVisibility {
			get { return (ScrollBarVisibility)GetValue(HorizontalScrollbarVisibilityProperty); }
			set { SetValue(HorizontalScrollbarVisibilityProperty, value); }
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
		#endregion
		#region public properties
		[Obsolete("Use the DataControlBase.CurrentColumn property instead"), Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), CloneDetailMode(CloneDetailMode.Skip)]
		public ColumnBase FocusedColumn {
			get { return (ColumnBase)GetValue(FocusedColumnProperty); }
			set { SetValue(FocusedColumnProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewKeyFieldName"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public string KeyFieldName {
			get { return (string)GetValue(KeyFieldNameProperty); }
			set { SetValue(KeyFieldNameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewParentFieldName"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public string ParentFieldName {
			get { return (string)GetValue(ParentFieldNameProperty); }
			set { SetValue(ParentFieldNameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewCheckBoxFieldName"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public string CheckBoxFieldName {
			get { return (string)GetValue(CheckBoxFieldNameProperty); }
			set { SetValue(CheckBoxFieldNameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewImageFieldName"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public string ImageFieldName {
			get { return (string)GetValue(ImageFieldNameProperty); }
			set { SetValue(ImageFieldNameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewCheckBoxValueConverter"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public IValueConverter CheckBoxValueConverter {
			get { return (IValueConverter)GetValue(CheckBoxValueConverterProperty); }
			set { SetValue(CheckBoxValueConverterProperty, value); }
		}
		public TreeListNodeImageSelector NodeImageSelector {
			get { return (TreeListNodeImageSelector)GetValue(NodeImageSelectorProperty); }
			set { SetValue(NodeImageSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewRootValue"),
#endif
 Category(Categories.OptionsBehavior), XtraSerializableProperty]
		public object RootValue {
			get { return GetValue(RootValueProperty); }
			set { SetValue(RootValueProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewAutoPopulateServiceColumns"),
#endif
 Category(Categories.OptionsBehavior)]
		public bool AutoPopulateServiceColumns {
			get { return (bool)GetValue(AutoPopulateServiceColumnsProperty); }
			set { SetValue(AutoPopulateServiceColumnsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewRowIndent"),
#endif
 Category(Categories.OptionsView)]
		public double RowIndent {
			get { return (double)GetValue(RowIndentProperty); }
			set { SetValue(RowIndentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewShowNodeImages"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool ShowNodeImages {
			get { return (bool)GetValue(ShowNodeImagesProperty); }
			set { SetValue(ShowNodeImagesProperty, value); }
		}
		[Category(Categories.OptionsView)]
		public Size NodeImageSize {
			get { return (Size)GetValue(NodeImageSizeProperty); }
			set { SetValue(NodeImageSizeProperty, value); }
		}
		public bool ShowCheckboxes {
			get { return (bool)GetValue(ShowCheckboxesProperty); }
			set { SetValue(ShowCheckboxesProperty, value); }
		}
		public bool AllowIndeterminateCheckState {
			get { return (bool)GetValue(AllowIndeterminateCheckStateProperty); }
			set { SetValue(AllowIndeterminateCheckStateProperty, value); }
		}
		public bool AllowRecursiveNodeChecking {
			get { return (bool)GetValue(AllowRecursiveNodeCheckingProperty); }
			set { SetValue(AllowRecursiveNodeCheckingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewShowExpandButtons"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool ShowExpandButtons {
			get { return (bool)GetValue(ShowExpandButtonsProperty); }
			set { SetValue(ShowExpandButtonsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewShowRootIndent"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public bool ShowRootIndent {
			get { return (bool)GetValue(ShowRootIndentProperty); }
			set { SetValue(ShowRootIndentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewTreeLineStyle"),
#endif
 Category(Categories.OptionsView), XtraSerializableProperty]
		public TreeListLineStyle TreeLineStyle {
			get { return (TreeListLineStyle)GetValue(TreeLineStyleProperty); }
			set { SetValue(TreeLineStyleProperty, value); }
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewVisibleColumns")]
#endif
		public IList<ColumnBase> VisibleColumns {
			get { return (IList<ColumnBase>)GetValue(VisibleColumnsProperty); }
			protected set { this.SetValue(VisibleColumnsPropertyKey, value); }
		}
#if !SL
		[Browsable(false)]
		public bool ShouldSerializeVisibleColumns(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
#endif
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public TreeListNode FocusedNode {
			get { return (TreeListNode)GetValue(FocusedNodeProperty); }
			set { this.SetValue(FocusedNodeProperty, value); }
		}
		protected TreeListNode FocusedNodeSave { get; set; }
		protected Locker FocusedNodeSaveLocker;
		protected void ClearFocusedNodeSave() {
			FocusedNodeSave = null;
		}
		public bool AllowDefaultContentForHierarchicalDataTemplate {
			get { return (bool)GetValue(AllowDefaultContentForHierarchicalDataTemplateProperty); }
			set { this.SetValue(AllowDefaultContentForHierarchicalDataTemplateProperty, value); }
		}
		public TreeListFilterMode FilterMode {
			get { return (TreeListFilterMode)GetValue(FilterModeProperty); }
			set { SetValue(FilterModeProperty, value); }
		}
		public bool RestoreFocusOnExpand {
			get { return (bool)GetValue(RestoreFocusOnExpandProperty); }
			set { SetValue(RestoreFocusOnExpandProperty, value); }
		}
		public bool AllowChildNodeSourceUpdates {
			get { return (bool)GetValue(AllowChildNodeSourceUpdatesProperty); }
			set { SetValue(AllowChildNodeSourceUpdatesProperty, value); }
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
		[ XtraSerializableProperty, Category(Categories.OptionsBehavior)]
		public bool AllowTreeIndentScrolling {
			get { return (bool)GetValue(AllowTreeIndentScrollingProperty); }
			set { SetValue(AllowTreeIndentScrollingProperty, value); }
		}
		#endregion
		#region printing
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewPrintRowTemplate"),
#endif
 Category(Categories.AppearancePrint)]
		public DataTemplate PrintRowTemplate {
			get { return (DataTemplate)GetValue(PrintRowTemplateProperty); }
			set { SetValue(PrintRowTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewPrintAutoWidth"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public bool PrintAutoWidth {
			get { return (bool)GetValue(PrintAutoWidthProperty); }
			set { SetValue(PrintAutoWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewPrintColumnHeaders"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public bool PrintColumnHeaders {
			get { return (bool)GetValue(PrintColumnHeadersProperty); }
			set { SetValue(PrintColumnHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewPrintBandHeaders"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public bool PrintBandHeaders {
			get { return (bool)GetValue(PrintBandHeadersProperty); }
			set { SetValue(PrintBandHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewPrintAllNodes"),
#endif
 Category(Categories.OptionsPrint), XtraSerializableProperty]
		public bool PrintAllNodes {
			get { return (bool)GetValue(PrintAllNodesProperty); }
			set { SetValue(PrintAllNodesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewPrintExpandButtons"),
#endif
 Category(Categories.AppearancePrint), XtraSerializableProperty]
		public bool PrintExpandButtons {
			get { return (bool)GetValue(PrintExpandButtonsProperty); }
			set { SetValue(PrintExpandButtonsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewPrintNodeImages"),
#endif
 Category(Categories.AppearancePrint), XtraSerializableProperty]
		public bool PrintNodeImages {
			get { return (bool)GetValue(PrintNodeImagesProperty); }
			set { SetValue(PrintNodeImagesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfGridLocalizedDescription("TreeListViewPrintColumnHeaderStyle"),
#endif
 Category(Categories.AppearancePrint)]
		public Style PrintColumnHeaderStyle {
			get { return (Style)GetValue(PrintColumnHeaderStyleProperty); }
			set { SetValue(PrintColumnHeaderStyleProperty, value); }
		}
		#endregion
		#region events
		[Category("Events")]
		public event TreeListNodeAllowEventHandler NodeExpanding {
			add { AddHandler(NodeExpandingEvent, value); }
			remove { RemoveHandler(NodeExpandingEvent, value); }
		}
		[Category("Events")]
		public event TreeListNodeEventHandler NodeExpanded {
			add { AddHandler(NodeExpandedEvent, value); }
			remove { RemoveHandler(NodeExpandedEvent, value); }
		}
		[Category("Events")]
		public event TreeListNodeAllowEventHandler NodeCollapsing {
			add { AddHandler(NodeCollapsingEvent, value); }
			remove { RemoveHandler(NodeCollapsingEvent, value); }
		}
		[Category("Events")]
		public event TreeListNodeEventHandler NodeCollapsed {
			add { AddHandler(NodeCollapsedEvent, value); }
			remove { RemoveHandler(NodeCollapsedEvent, value); }
		}
		[Category("Events")]
		public event TreeListNodeEventHandler NodeCheckStateChanged {
			add { AddHandler(NodeCheckStateChangedEvent, value); }
			remove { RemoveHandler(NodeCheckStateChangedEvent, value); }
		}
		[Category("Events")]
		public event TreeListNodeChangedEventHandler NodeChanged {
			add { nodeChanged += value; }
			remove { nodeChanged -= value; }
		}
		[Category("Events")]
		public event TreeListInvalidNodeExceptionEventHandler InvalidNodeException {
			add { AddHandler(InvalidNodeExceptionEvent, value); }
			remove { RemoveHandler(InvalidNodeExceptionEvent, value); }
		}
		[Category("Events")]
		public event TreeListNodeValidationEventHandler ValidateNode {
			add { AddHandler(ValidateNodeEvent, value); }
			remove { RemoveHandler(ValidateNodeEvent, value); }
		}
		[Category("Events")]
		public event TreeListCellValidationEventHandler ValidateCell;
		[Category("Events")]
		public event TreeListUnboundColumnDataEventHandler CustomUnboundColumnData {
			add { customUnboundColumnData += value; }
			remove { customUnboundColumnData -= value; }
		}
		[Category("Events")]
		public event TreeListNodeFilterEventHandler CustomNodeFilter {
			add { customNodeFilter += value; }
			remove { customNodeFilter -= value; }
		}
		internal bool IsCustomNodeFilterAssigned { get { return customNodeFilter != null; } }
		[Category("Events")]
		public event TreeListCustomColumnSortEventHandler CustomColumnSort {
			add { customColumnSort += value; }
			remove { customColumnSort -= value; }
		}
		[Category("Events")]
		public event RoutedEventHandler StartSorting {
			add { AddHandler(StartSortingEvent, value); }
			remove { RemoveHandler(StartSortingEvent, value); }
		}
		[Category("Events")]
		public event RoutedEventHandler EndSorting {
			add { AddHandler(EndSortingEvent, value); }
			remove { RemoveHandler(EndSortingEvent, value); }
		}
		[Category("Events")]
		public event TreeListCustomColumnDisplayTextEventHandler CustomColumnDisplayText {
			add { customColumnDisplayText += value; }
			remove { customColumnDisplayText -= value; }
		}
		[Category("Events")]
		public event TreeListCustomSummaryEventHandler CustomSummary {
			add { customSummary += value; }
			remove { customSummary -= value; }
		}
		[Category("Events")]
		public event CustomColumnFilterListEventHandler CustomFiterPopupList {
			add { customFiterPopupList += value; }
			remove { customFiterPopupList -= value; }
		}
		[Category("Behavior")]
		public event TreeListEditorEventHandler ShownEditor {
			add { AddHandler(ShownEditorEvent, value); }
			remove { RemoveHandler(ShownEditorEvent, value); }
		}
		[Category("Behavior")]
		public event TreeListShowingEditorEventHandler ShowingEditor {
			add { AddHandler(ShowingEditorEvent, value); }
			remove { RemoveHandler(ShowingEditorEvent, value); }
		}
		[Category("Behavior")]
		public event TreeListEditorEventHandler HiddenEditor {
			add { AddHandler(HiddenEditorEvent, value); }
			remove { RemoveHandler(HiddenEditorEvent, value); }
		}
		[Category("Behavior")]
		public event TreeListCellValueChangedEventHandler CellValueChanged {
			add { AddHandler(CellValueChangedEvent, value); }
			remove { RemoveHandler(CellValueChangedEvent, value); }
		}
		[Category(Categories.OptionsView)]
		public event CustomScrollAnimationEventHandler CustomScrollAnimation {
			add { AddHandler(CustomScrollAnimationEvent, value); }
			remove { RemoveHandler(CustomScrollAnimationEvent, value); }
		}
		[Category("Behavior")]
		public event TreeListCellValueChangedEventHandler CellValueChanging {
			add { AddHandler(CellValueChangingEvent, value); }
			remove { RemoveHandler(CellValueChangingEvent, value); }
		}
		[Category("Behavior")]
		public event RowDoubleClickEventHandler RowDoubleClick {
			add { AddHandler(RowDoubleClickEvent, value); }
			remove { RemoveHandler(RowDoubleClickEvent, value); }
		}
		[Obsolete("Use the TreeListControl.SelectionChanged event instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public event TreeListSelectionChangedEventHandler SelectionChanged {
			add { AddHandler(SelectionChangedEvent, value); }
			remove { RemoveHandler(SelectionChangedEvent, value); }
		}
		[Obsolete("Use the TreeList.CopyingToClipboard event instead"), Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Category("Behavior")]
		public event TreeListCopyingToClipboardEventHandler CopyingToClipboard {
			add { AddHandler(CopyingToClipboardEvent, value); }
			remove { RemoveHandler(CopyingToClipboardEvent, value); }
		}
		#endregion
		readonly TreeListDataProvider treeListDataProvider;
		internal TreeListDataProvider TreeListDataProvider { get { return treeListDataProvider; } }
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewNodes")]
#endif
		public new TreeListNodeCollection Nodes { get { return treeListDataProvider.Nodes; } }
		protected internal override Data.DataProviderBase DataProviderBase { get { return treeListDataProvider; } }
		protected internal override bool NeedCellsWidthUpdateOnScrolling { get { return true; } }
		protected internal override bool UseMouseUpFocusedEditorShowModeStrategy { get { return IsMultiCellSelection; } }
		protected internal override GridFilterColumn CreateFilterColumn(ColumnBase column, bool useDomainDataSourceRestrictions, bool useWcfSource) {
			if(TreeDerivationMode != Grid.TreeDerivationMode.Selfreference)
				return new TreeListFilterColumn(column, useDomainDataSourceRestrictions, useWcfSource);
			return base.CreateFilterColumn(column, useDomainDataSourceRestrictions, useWcfSource);
		}
		TreeListUnboundColumnDataEventHandler customUnboundColumnData;
		TreeListNodeChangedEventHandler nodeChanged;
		TreeListNodeFilterEventHandler customNodeFilter;
		CustomColumnFilterListEventHandler customFiterPopupList;
		TreeListCustomColumnSortEventHandler customColumnSort;
		TreeListCustomColumnDisplayTextEventHandler customColumnDisplayText;
		TreeListCustomSummaryEventHandler customSummary;
		Binding expandStateBinding;
		Lazy<BarManagerMenuController> bandMenuControllerValue;
		internal BarManagerMenuController BandMenuController { get { return bandMenuControllerValue.Value; } }
		[Browsable(false)]
		public BarManagerActionCollection BandMenuCustomizations { get { return BandMenuController.ActionContainer.Actions; } }
		public TreeListView() : base(null, null, null) {
			this.SetDefaultStyleKey(typeof(TreeListView));
			this.treeListDataProvider = CreateDataProvider();
			this.FocusedNodeSaveLocker = new Locker();
			bandMenuControllerValue = CreateMenuControllerLazyValue();
		}
#if !SL
	[DevExpressXpfGridLocalizedDescription("TreeListViewTotalNodesCount")]
#endif
		public int TotalNodesCount { get { return TreeListDataProvider.TotalNodesCount; } }
		public TreeListViewCommands TreeListCommands { get { return base.Commands as TreeListViewCommands; } }
		internal override int GroupCount { get { return 0; } }
		internal override bool IsRowMarginControlVisible { get { return true; } }
		protected internal TreeListViewBehavior TreeListViewBehavior { get { return (TreeListViewBehavior)ViewBehavior; } }
		protected internal int ServiceIndentsCount { get { return (ShowRootIndent ? 1 : 0) + (ShowNodeImages ? 1 : 0) + (ShowCheckboxes ? 1 : 0); } }
		new protected internal TreeListRowsClipboardController ClipboardController { get { return base.ClipboardController as TreeListRowsClipboardController; } }
		protected override RowsClipboardController CreateClipboardController() {
			return new TreeListRowsClipboardController(this);
		}
		internal IDispalyMemberBindingClient DisplayMemberBindingClient { get { return DataControl; } }
		protected override bool ShouldChangeRowByTab { get { return false; } }
		protected virtual TreeListDataProvider CreateDataProvider() {
			return new TreeListDataProvider(this);
		}
		protected override DataIteratorBase CreateDataIterator() {
			return new TreeListDataIterator(this);
		}
		protected override Native.DataViewBehavior CreateViewBehavior() {
			return new TreeListViewBehavior(this);
		}
		protected override DataViewCommandsBase CreateCommandsContainer() {
			return new TreeListViewCommands(this);
		}
		protected override SelectionStrategyBase CreateSelectionStrategy() {
			if(NavigationStyle == GridViewNavigationStyle.None)
				return new SelectionStrategyNavigationNone(this);
			if(!IsMultiSelection)
				return new SelectionStrategyNone(this);
			if(IsMultiRowSelection)
				return new TreeListSelectionStrategyRow(this);
			if(NavigationStyle == GridViewNavigationStyle.Row)
				return new TreeListSelectionStrategyRow(this);
			return new TreeListSelectionStrageryCell(this);
		}
		protected override bool ChangeVisibleRowExpandCore(int rowHandle) {
			return ChangeNodeExpanded(rowHandle);
		}
		internal override bool IsDataRowNodeExpanded(DataRowNode rowNode) {
			TreeListNode node = TreeListDataProvider.GetNodeByRowHandle(rowNode.RowHandle.Value);
			return node != null && node.IsExpanded;
		}
		protected internal override void MoveColumnToCore(ColumnBase source, int newVisibleIndex, HeaderPresenterType moveFrom, HeaderPresenterType moveTo) {
			int dropIndex = newVisibleIndex;
			if(source.Fixed == FixedStyle.None && FixedNoneVisibleColumns.Count == 0)
				dropIndex = FixedLeftVisibleColumns.Count;
			else if(source.Fixed == FixedStyle.Left && FixedLeftVisibleColumns.Count == 0)
				dropIndex = 0;
			else if(source.Fixed == FixedStyle.Right && FixedRightVisibleColumns.Count == 0)
				dropIndex = VisibleColumns.Count;
			base.MoveColumnToCore(source, dropIndex, moveFrom, moveTo);
		}
#if SL
		internal override bool NeedsKey(Key key, ModifierKeys modifiers) {
			TreeListNode node = FocusedNode;
			if (node == null) return false;
			return node.IsExpanded && key == Key.Subtract ||
			   !node.IsExpanded && (key == Key.Add || key == Key.Multiply);
		}
#endif
		internal override bool IsExpandableRowFocused() {
			return false;
		}
		protected override int FixedNoneColumnsCount { get { return FixedNoneVisibleColumns.Count; } } 
		internal override IColumnCollection CreateEmptyColumnCollection() {
			return new ColumnCollection(null);
		}
		internal bool ExpandNodeAndAllChildren() {
			TreeListDataProvider.ToggleExpandedAllChildNodes(TreeListDataProvider.GetNodeByRowHandle(FocusedRowHandle), true);
			return true;
		}
		#region public API
		public void SaveNodesState() {
			TreeListDataProvider.SaveNodesState(true);
		}
		public void RestoreNodesState() {
			TreeListDataProvider.RestoreNodesState(true);
		}
		public void ExpandAllNodes() {
			TreeListDataProvider.ToggleExpandedAllNodes(true);
		}
		public void CollapseAllNodes() {
			TreeListDataProvider.ToggleExpandedAllNodes(false);
		}
		public void ExpandToLevel(int level) {
			TreeListDataProvider.ExpandToLevel(level);
		}
		public void ExpandNode(int rowHandle) {
			ChangeNodeExpanded(rowHandle, true);
		}
		public void CollapseNode(int rowHandle) {
			ChangeNodeExpanded(rowHandle, false);
		}
		public void ChangeNodeExpanded(int rowHandle, bool isExpanded) {
			TreeListNode node = TreeListDataProvider.GetNodeByRowHandle(rowHandle);
			if (node != null)
				node.IsExpanded = isExpanded;
		}
		public object GetNodeValue(TreeListNode node, string fieldName) {
			return TreeListDataProvider.GetNodeValue(node, fieldName);
		}
		public object GetNodeValue(TreeListNode node, ColumnBase column) {
			if (column == null) return null;
			return TreeListDataProvider.GetNodeValue(node, column.FieldName);
		}
		public void SetNodeValue(TreeListNode node, string fieldName, object value) {
			TreeListDataProvider.SetNodeValue(node, fieldName, value);
		}
		public void SetNodeValue(TreeListNode node, ColumnBase column, object value) {
			if (column == null) return;
			TreeListDataProvider.SetNodeValue(node, column.FieldName, value);
		}
		public TreeListNode GetNodeByRowHandle(int rowHandle) {
			return TreeListDataProvider.GetNodeByRowHandle(rowHandle);
		}
		public TreeListNode GetNodeByVisibleIndex(int visibleIndex) {
			return TreeListDataProvider.GetNodeByVisibleIndex(visibleIndex);
		}
		public int GetNodeVisibleIndex(TreeListNode node) {
			return TreeListDataProvider.GetVisibleIndexByNode(node);
		}
		public TreeListNode GetNodeByContent(object content) {
			return TreeListDataProvider.FindNodeByValue(content);
		}
		public TreeListNode GetNodeByCellValue(string fieldName, object value) {
			if (string.IsNullOrEmpty(fieldName))
				return TreeListDataProvider.FindNodeByValue(value);
			return TreeListDataProvider.FindNodeByValue(fieldName, value);
		}
		public TreeListNode GetNodeByKeyValue(object keyValue) {
			return TreeListDataProvider.FindNodeByValue(KeyFieldName, keyValue);
		}
		public void DeleteNode(int rowHandle) {
			DeleteNode(rowHandle, true);
		}
		public void DeleteNode(int rowHandle, bool deleteChildren) {
			DeleteNode(GetNodeByRowHandle(rowHandle), deleteChildren);
		}
		public void DeleteNode(TreeListNode node) {
			DeleteNode(node, true);
		}
		public void DeleteNode(TreeListNode node, bool deleteChildren) {
			TreeListDataProvider.DeleteNode(node, deleteChildren);
		}
		public void CheckAllNodes() {
			CheckAllNodesCore(true);
		}
		public void UncheckAllNodes() {
			CheckAllNodesCore(false);
		}
		public void RefreshNodeImage(TreeListNode node) {
			TreeListDataProvider.UpdateRow(node);
		}
		void CheckAllNodesCore(bool isChecked) {
			foreach(TreeListNode node in new TreeListNodeIterator(Nodes)) {
				if(!node.IsVisible) continue;
				node.SetNodeChecked(node, isChecked);
				RaiseNodeChanged(node, NodeChangeType.CheckBox);
			}
			UpdateRows();
		}
		#endregion
		public void MovePrevRow(bool allowNavigateToAutoFilterRow) {
			TreeListViewBehavior.MovePrevRow(allowNavigateToAutoFilterRow);
		}
		internal void UpdateFocusedNode() {
			FocusedNode = TreeListDataProvider.GetNodeByRowHandle(FocusedRowHandle);
			if(!FocusedNodeSaveLocker.IsLocked)
				ClearFocusedNodeSave();
		}
		protected internal bool ChangeNodeExpanded(int rowHandle) {
			if(!CommitEditing()) return false;
			TreeListNode node = TreeListDataProvider.GetNodeByRowHandle(rowHandle);
			if(node != null) {
				bool wasExpanded = node.IsExpanded;
				node.IsExpanded = !node.IsExpanded;
				return wasExpanded != node.IsExpanded;
			}
			return false;
		}
		protected internal bool ChangeNodeCheckState(int rowHandle) {
			if(!CommitEditing()) return false;
			TreeListNode node = TreeListDataProvider.GetNodeByRowHandle(rowHandle);
			if(node != null) {
				bool? oldCheckState = node.IsChecked;
				if(node.IsChecked == true) {
					if(AllowIndeterminateCheckState) node.IsChecked = null;
					else node.IsChecked = false;
				}
				else if(node.IsChecked == null) node.IsChecked = false;
				else if(node.IsChecked == false) node.IsChecked = true;
				return oldCheckState != node.IsChecked;
			}
			return false;
		}
		protected override void OnFocusedRowHandleChangedCore(int oldRowHandle) {
			UpdateFocusedNode();
			base.OnFocusedRowHandleChangedCore(oldRowHandle);
		}
		protected internal virtual void OnChangeNodeExpanded(object commandParameter) {
			ChangeNodeExpanded((int)commandParameter);
		}
		protected internal virtual bool CanChangeNodeExpaned(object commandParameter) {
			if (commandParameter == null)
				return false;
			TreeListNode node = TreeListDataProvider.GetNodeByRowHandle((int)commandParameter);
			if (node == null)
				return false;
			return node.IsTogglable;
		}
		protected internal virtual void OnChangeNodeCheckState(object commandParameter) {
			ChangeNodeCheckState((int)commandParameter);
		}
		protected internal virtual bool CanChangeNodeCheckState(object commandParameter) {
			if(commandParameter == null)
				return false;
			TreeListNode node = TreeListDataProvider.GetNodeByRowHandle((int)commandParameter);
			if(node == null)
				return false;
			return node.IsCheckBoxEnabled;
		}
		protected internal IList<IColumnInfo> GetColumns() {
			List<IColumnInfo> list = new List<IColumnInfo>();
			if(DataControl != null) {
				foreach(ColumnBase column in DataControl.ColumnsCore)
					list.Add(column as IColumnInfo);
			}
			return list;
		}
		protected internal virtual void UpdateRows() {
			if(DataControl != null)
				DataControl.UpdateLayoutCore();
			RebuildVisibleColumns();
			UpdateContentLayout();
		}
		protected internal void DoRefresh() {
			TreeListDataProvider.DoRefresh(false);
		}
		protected internal void DoRefresh(bool keepNodesState) {
			TreeListDataProvider.DoRefresh(keepNodesState);
		}
		void OnItemsSourceModeChanged() {
			TreeListDataProvider.UpdateDataHelper();
			DoRefresh();
		}
		protected internal override void OnDataChanged(bool rebuildVisibleColumns) {
			if(TreeListDataProvider.IsUnboundMode) 
				TreeListDataProvider.RePopulateColumns();
			base.OnDataChanged(rebuildVisibleColumns);
			if(!IsLoaded)
				ForceAutoExpandAllNodes();
		}
		protected internal virtual void OnCurrentIndexChanged() {
			SetFocusOnCurrentControllerRow();
		}
		protected internal virtual string GetNodeDisplayText(TreeListNode node, string fieldName, object value) {
			if (DataControl == null) return null;
			ColumnBase column = DataControl.ColumnsCore[fieldName];
			string displayText = GetDisplayObject(value, column).ToString();
			if (customColumnDisplayText == null)
				return displayText;
			return RaiseCustomColumnDisplayText(node, column, value, displayText);
		}
		protected internal virtual bool RaiseNodeExpanding(TreeListNode node) {
			TreeListNodeAllowEventArgs args = new TreeListNodeAllowEventArgs(node) { RoutedEvent = NodeExpandingEvent };
			RaiseEvent(args);
			return args.Allow;
		}
		protected internal virtual void RaiseNodeExpanded(TreeListNode node) {
			TreeListNodeEventArgs args = new TreeListNodeEventArgs(node) { RoutedEvent = NodeExpandedEvent };
			RaiseEvent(args);
		}
		protected internal virtual bool RaiseNodeCollapsing(TreeListNode node) {
			TreeListNodeAllowEventArgs args = new TreeListNodeAllowEventArgs(node) { RoutedEvent = NodeCollapsingEvent };
			RaiseEvent(args);
			return args.Allow;
		}
		protected internal virtual void RaiseNodeCollapsed(TreeListNode node) {
			TreeListNodeEventArgs args = new TreeListNodeEventArgs(node) { RoutedEvent = NodeCollapsedEvent };
			RaiseEvent(args);
		}
		protected internal virtual void RaiseNodeCheckStateChanged(TreeListNode node) {
			TreeListNodeEventArgs args = new TreeListNodeEventArgs(node) { RoutedEvent = NodeCheckStateChangedEvent };
			RaiseEvent(args);
		}
		protected internal virtual void RaiseNodeChanged(TreeListNode node, NodeChangeType changeType) {
			if(nodeChanged != null)
				nodeChanged(this, new TreeListNodeChangedEventArgs(node, changeType));
		}
#if DEBUGTEST
		internal 
#endif
		TreeListCustomColumnDisplayTextEventArgs customColumnDisplayTextEventArgs;
		protected virtual string RaiseCustomColumnDisplayText(TreeListNode node, ColumnBase column, object value, string displayText) {
			if(customColumnDisplayTextEventArgs == null) {
				customColumnDisplayTextEventArgs = new TreeListCustomColumnDisplayTextEventArgs(node, column, value, displayText);
			}
			else {
				customColumnDisplayTextEventArgs.SetArgs(node, column, value, displayText);
			}
			if(customColumnDisplayText != null)
				customColumnDisplayText(this, customColumnDisplayTextEventArgs);
			customColumnDisplayTextEventArgs.Clear();
			return customColumnDisplayTextEventArgs.DisplayText;
		}
		protected internal virtual object RaiseCustomUnboundColumnData(object p, string propName, object value, bool isGetAction) {
			if (DataControl == null) return null;
			TreeListUnboundColumnDataEventArgs e = new TreeListUnboundColumnDataEventArgs(DataControl.ColumnsCore[propName], p as TreeListNode, value, isGetAction);
			if (customUnboundColumnData != null)
				customUnboundColumnData(this, e);
			return e.Value;
		}
		protected internal virtual bool? RaiseCustomNodeFilter(TreeListNode node) {
			TreeListNodeFilterEventArgs e = new TreeListNodeFilterEventArgs(node);
			if (customNodeFilter != null)
				customNodeFilter(this, e);
			return e.Handled ? e.Visible : (bool?)null;
		}
		protected internal bool RaiseCustomFiterPopupList(TreeListNode node, DataColumnInfo columnInfo) {
			CustomColumnFilterListEventArgs e = new CustomColumnFilterListEventArgs(node, ColumnsCore[columnInfo.Name] as TreeListColumn);
			if (customFiterPopupList != null)
				customFiterPopupList(this, e);
			return e.Visible;
		}
		protected internal virtual void RaiseCustomColumnSort(TreeListCustomColumnSortEventArgs e) {
			if (customColumnSort != null)
				customColumnSort(this, e);
		}
		protected internal virtual void RaiseCustomSummary(TreeListCustomSummaryEventArgs e) {
			if (customSummary != null)
				customSummary(this, e);
		}
		internal bool HasCustomSummary { get { return customSummary != null; } }
		protected internal virtual void RaiseInvalidNodeException(TreeListNode node, ControllerRowExceptionEventArgs args) {
			TreeListInvalidNodeExceptionEventArgs e = new TreeListInvalidNodeExceptionEventArgs(node, args.Exception.Message, GetLocalizedString(GridControlStringId.ErrorWindowTitle), args.Exception, ExceptionMode.DisplayError) { RoutedEvent = TreeListView.InvalidNodeExceptionEvent };
			RaiseEvent(e);
			HandleInvalidRowExceptionEventArgs(args, e);
		}
		protected internal virtual bool RaiseValidateNode(int rowHandle, object value) {
			if (DataControl == null)
				return true;
			TreeListNodeValidationEventArgs e = new TreeListNodeValidationEventArgs(value, rowHandle, this) { RoutedEvent = ValidateNodeEvent };
			try {
				RaiseEvent(e);
			}
			catch(Exception exception) {
				DataControl.SetRowStateError(rowHandle, new GridRowValidationError(exception.Message, exception, ErrorType.Default, rowHandle));
				throw exception;
			}
			if (e.IsValid)
				DataControl.SetRowStateError(rowHandle, null);
			else {
				string errorText = e.ErrorContent != null ? e.ErrorContent.ToString() : string.Empty;
				DataControl.SetRowStateError(rowHandle, new GridRowValidationError(errorText, null, ErrorType.Default, rowHandle));
				throw new WarningException(errorText);
			}
			return e.IsValid;
		}
		protected internal virtual void ChildrenPropertyUpdate() {
			if (TreeDerivationMode == TreeDerivationMode.ChildNodesSelector && ChildNodesSelector == null)
				DoRefresh();
		}
		protected internal virtual void OnChildNodesSelectorChanged() {
			if (TreeDerivationMode == TreeDerivationMode.ChildNodesSelector)
				DoRefresh();
		}
		protected override bool CanSortDataColumnInfo(DataColumnInfo columnInfo) {
			if(TreeList.TreeListDataProvider.IsUnitypeColumn(columnInfo))
				return true;
			return base.CanSortDataColumnInfo(columnInfo);
		}
		protected internal RowMarginControl FindRowMarginControl(DependencyObject obj) {
			DependencyObject element = obj;
			while (element != null && GetRowHandle(element) == null) {
				if (element is RowMarginControl)
					return (RowMarginControl)element;
				element = LayoutHelper.GetParent(element);
			}
			return null;
		}
		internal override DataControlPopupMenu CreatePopupMenu() {
			return new TreeListPopupMenu(this);
		}
		internal override FrameworkElement CreateRowElement(RowData rowData) {
#if SL
			return new GridRow();
#else
			return TreeListViewBehavior.CreateElement(() => new RowControl(rowData), () => new GridRow(), DevExpress.Xpf.Grid.UseLightweightTemplates.Row);
#endif
		}
		protected internal override object GetGroupDisplayValue(int rowHandle) {
			throw new NotImplementedException();
		}
		protected internal override string GetGroupRowDisplayText(int rowHandle) {
			throw new NotImplementedException();
		}
		internal override DependencyProperty GetFocusedColumnProperty() {
			return FocusedColumnProperty;
		}
		protected override void SetVisibleColumns(IList<ColumnBase> columns) {
			VisibleColumns = columns;
		}
		protected internal void CheckFocusedNodeOnCollapse(TreeListNode treeListNode) {
			if(FocusedNode != null && FocusedNode.IsDescendantOf(treeListNode)) {
				if(RestoreFocusOnExpand) {
					FocusedNodeSaveLocker.Lock();
					if(FocusedNodeSave == null)
						FocusedNodeSave = FocusedNode;
				}
				FocusedNode = treeListNode;
				FocusedNodeSaveLocker.Unlock();
			}
		}
		protected internal void CheckFocusedNodeOnExpand(TreeListNode treeListNode) {
			if(RestoreFocusOnExpand && FocusedNodeSave != null && FocusedNode != FocusedNodeSave && FocusedNodeSave.IsDescendantOf(treeListNode)) {
				FocusedNodeSaveLocker.Lock();
				FocusedNode = FindTopVisibleParentNode(FocusedNodeSave, treeListNode);
				FocusedNodeSaveLocker.Unlock();
				if(FocusedNode == FocusedNodeSave)
					ClearFocusedNodeSave();
			}
		}
		TreeListNode FindTopVisibleParentNode(TreeListNode startNode, TreeListNode stopNode) {
			TreeListNode current = startNode;
			TreeListNode selected = null;
			while(current != null && current != stopNode) {
				if(!current.ParentNode.IsExpanded)
					selected = null;
				else if(selected == null)
					selected = current;
				current = current.ParentNode;
			}
			return selected;
		}
		internal override DevExpress.Data.DataController GetDataControllerForUnboundColumnsCore() {
			return null;
		}
		protected internal void OnDataSourceChanged() {
			if(DataControl != null)
				DataControl.PopulateColumnsIfNeeded();
			ForceAutoExpandAllNodes();
		}
		internal void ForceAutoExpandAllNodes() {
			if(AutoExpandAllNodes && TreeListDataProvider.IsReady)
				ExpandAllNodes();
		}
		protected internal override void ResetHeadersChildrenCache() {
			if(DataControl.AutomationPeer != null) DataControl.AutomationPeer.ResetHeadersChildrenCache();
		}
		protected internal virtual void OnFilterModeChanged() {
			TreeListDataProvider.OnFilterModeChanged();
		}
		void OnIndentItemChanged() {
			RebuildVisibleColumns();
			UpdateRows();
		}
		protected override ControlTemplate GetRowFocusedRectangleTemplate() {
			return FocusedRowBorderTemplate;
		}
		protected internal override void ResetDataProvider() {
			TreeListDataProvider.DataSource = null;
		}
		internal void CalcMinWidth() {
			if (FixedLeftVisibleColumns.Count > 0 || FixedRightVisibleColumns.Count == 0)
				return;
			double scrollableArea = 0d;
			foreach (ColumnBase column in FixedRightVisibleColumns)
				scrollableArea += column.ActualHeaderWidth;
			GridViewInfo viewInfo = ((TreeListViewBehavior)ViewBehavior).ViewInfo;
			scrollableArea += (ShowIndicator ? IndicatorHeaderWidth : 0d) + FixedLineWidth + viewInfo.TotalGroupAreaIndent;
			ScrollableAreaMinWidth = scrollableArea;
		}
		protected internal IEnumerable<TreeListNode> GetNodesFromRowHandles(IEnumerable<int> rowHandles) {
			if(rowHandles == null) return null;
			List<TreeListNode> nodes = new List<TreeListNode>();
			foreach(int rowHandle in rowHandles) {
				TreeListNode node = GetNodeByRowHandle(rowHandle);
				if(node != null)
					nodes.Add(node);
			}
			return nodes.ToArray();
		}
		protected internal IEnumerable<int> GetRowHandlesFromNodes(IEnumerable<TreeListNode> nodes) {
			if(nodes == null) return null;
			List<int> rowHandles = new List<int>();
			foreach(TreeListNode node in nodes) {
				int rowHandle = TreeListDataProvider.GetRowHandleByNode(node);
				if(rowHandle >= 0)
					rowHandles.Add(rowHandle);
			}
			return rowHandles;
		}
		protected internal override MultiSelectMode GetSelectionMode() {
			return SelectionModeHelper.ConvertToMultiSelectMode((TableViewSelectMode)GetValue(MultiSelectModeProperty));
		}
		protected virtual void OnRootValueChanged() {
			DoRefresh();
			OnDataSourceReset();
		}
		protected internal override void OnColumnCollectionChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e) {
			TreeListDataProvider.OnColumnCollectionChanged(e);
		}
		void UpdateActualAllowTreeIndentScrolling() {
			ActualAllowTreeIndentScrolling = AllowTreeIndentScrolling && !TreeListViewBehavior.HasFixedLeftElements;
		}
		bool actualAllowTreeIndentScrolling;
		internal bool ActualAllowTreeIndentScrolling {
			get { return actualAllowTreeIndentScrolling; }
			private set {
				if(actualAllowTreeIndentScrolling == value)
					return;
				actualAllowTreeIndentScrolling = value;
				ViewBehavior.UpdateViewRowData(x => x.UpdateClientIndentScrolling());
			}
		}
		protected override bool OnVisibleColumnsAssigned(bool changed) {
			bool res = base.OnVisibleColumnsAssigned(changed);
			UpdateActualAllowTreeIndentScrolling();
			return res;
		}
		#region HitTest
		public TreeListViewHitInfo CalcHitInfo(DependencyObject d) {
			return TreeListViewHitInfo.CalcHitInfo(d, this);
		}
		public TreeListViewHitInfo CalcHitInfo(Point hitTestPoint) {
			return CalcHitInfo(VisualTreeHelper.HitTest(this, hitTestPoint).VisualHit);
		}
		internal override IDataViewHitInfo CalcHitInfoCore(DependencyObject source) {
			return CalcHitInfo(source);
		}
		#endregion
		#region BestFit
		public void BestFitColumn(ColumnBase column) {
			TreeListViewBehavior.BestFitColumn(column);
		}
		public void BestFitColumns() {
			TreeListViewBehavior.BestFitColumns();
		}
		public double CalcColumnBestFitWidth(ColumnBase column) {
			return TreeListViewBehavior.CalcColumnBestFitWidthCore(column);
		}
		#endregion
		#region ITableView
		double ITableView.LeftGroupAreaIndent { get { return RowIndent; } }
		double ITableView.RightGroupAreaIndent { get { return 0; } }
		TableViewBehavior ITableView.TableViewBehavior { get { return TreeListViewBehavior; } }
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
		bool ITableView.IsCheckBoxSelectorColumnVisible { get { return false; } }
		bool ITableView.IsEditing { get { return IsEditing; } }
		DataViewBase ITableView.ViewBase { get { return this; } }
		bool ITableView.ActualAllowTreeIndentScrolling { get { return ActualAllowTreeIndentScrolling; } }
		IList<ColumnBase> ITableView.ViewportVisibleColumns { get; set; }
		void ITableView.SetHorizontalViewport(double value) {
			HorizontalViewport = value;
		}
		void ITableView.SetFixedLeftVisibleColumns(IList<ColumnBase> columns) {
			FixedLeftVisibleColumns = columns;
		}
		void ITableView.SetFixedNoneVisibleColumns(IList<ColumnBase> columns) {
			FixedNoneVisibleColumns = columns;
		}
		void ITableView.SetFixedRightVisibleColumns(IList<ColumnBase> columns) {
			FixedRightVisibleColumns = columns;
		}
		void ITableView.CopyCellsToClipboard(IEnumerable<CellBase> gridCells) {
			CopyCellsToClipboard(new SimpleEnumerableBridge<TreeListCell, CellBase>(gridCells));
		}
		CellBase ITableView.CreateGridCell(int rowHandle, ColumnBase column) {
			return new TreeListCell(rowHandle, column);
		}
		ITableViewHitInfo ITableView.CalcHitInfo(DependencyObject d) {
			return TreeListViewHitInfo.CalcHitInfo(d, this);
		}
		void ITableView.SetActualShowIndicator(bool showIndicator) {
			ActualShowIndicator = showIndicator;
		}
		void ITableView.SetActualIndicatorWidth(double indicatorWidth) {
			ActualIndicatorWidth = indicatorWidth;
		}
		void ITableView.SetActualExpandDetailHeaderWidth(double expandDetailButtonWidth) { }
		void ITableView.SetActualDetailMargin(Thickness detailMargin) { }
		void ITableView.SetShowTotalSummaryIndicatorIndent(bool showTotalSummaryIndicatorIndent) {
			this.ShowTotalSummaryIndicatorIndent = showTotalSummaryIndicatorIndent;
		}
		void ITableView.SetActualFadeSelectionOnLostFocus(bool fadeSelectionOnLostFocus) {
			ActualFadeSelectionOnLostFocus = fadeSelectionOnLostFocus;
		}
		void ITableView.RaiseRowDoubleClickEvent(ITableViewHitInfo hitInfo
#if !SL
			, MouseButton changedButton
#endif
			) {
			RaiseEvent(new RowDoubleClickEventArgs((GridViewHitInfoBase)hitInfo
#if !SL
				, changedButton
#endif
				, this
			) { RoutedEvent = RowDoubleClickEvent });
		}
		void ITableView.SetExpandColumnPosition(ColumnPosition position) { }
		#endregion
		#region events
		internal override bool RaiseShowingEditor(int rowHanlde, ColumnBase columnBase) {
			TreeListShowingEditorEventArgs e = new TreeListShowingEditorEventArgs(this, rowHanlde, columnBase);
			RaiseShowingEditor(e);
			return !e.Cancel;
		}
		protected internal virtual void RaiseShowingEditor(TreeListShowingEditorEventArgs e) {
			RaiseEvent(e);
		}
		internal override void RaiseShownEditor(int rowHandle, ColumnBase column, IBaseEdit editCore) {
			RaiseShownEditor(new TreeListEditorEventArgs(this, rowHandle, column, editCore));
		}
		protected internal virtual void RaiseShownEditor(TreeListEditorEventArgs e) {
			RaiseEvent(e);
		}
		internal override void RaiseHiddenEditor(int rowHandle, ColumnBase column, IBaseEdit editCore) {
			RaiseHiddenEditor(new TreeListEditorEventArgs(this, rowHandle, column, editCore) { RoutedEvent = TreeListView.HiddenEditorEvent });
		}
		protected internal virtual void OnStartedSort() {
			RaiseEvent(new RoutedEventArgs(StartSortingEvent));
		}
		protected internal virtual void OnEndedSort() {
			RaiseEvent(new RoutedEventArgs(EndSortingEvent));
		}
		protected internal virtual void RaiseHiddenEditor(TreeListEditorEventArgs e) {
			RaiseEvent(e);
		}
		protected internal override void RaiseValidateCell(GridRowValidationEventArgs e) {
			if(ValidateCell != null)
				ValidateCell(this, (TreeListCellValidationEventArgs)e);
		}
		internal override void RaiseCellValueChanging(int rowHandle, ColumnBase column, object value, object oldValue) {
			RaiseEvent(new TreeListCellValueChangedEventArgs(GetNodeByRowHandle(rowHandle), column, value, oldValue) { RoutedEvent = TreeListView.CellValueChangingEvent });
		}
		internal override void RaiseCellValueChanged(int rowHandle, ColumnBase column, object newValue, object oldValue) {
			RaiseEvent(new TreeListCellValueChangedEventArgs(GetNodeByRowHandle(rowHandle), column, newValue, oldValue) { RoutedEvent = TreeListView.CellValueChangedEvent });
		}
		protected internal override void RaiseCustomScrollAnimation(CustomScrollAnimationEventArgs e) {
			e.RoutedEvent = TreeListView.CustomScrollAnimationEvent;
			base.RaiseCustomScrollAnimation(e);
		}
		internal override RowValidationError CreateCellValidationError(object errorContent, Exception exception, ErrorType errorType, int rowHandle, ColumnBase column) {
			return new TreeListCellValidationError(errorContent, exception, errorType, rowHandle, GetNodeByRowHandle(rowHandle), column);
		}
		internal override GridRowValidationEventArgs CreateCellValidationEventArgs(object source, object value, int rowHandle, ColumnBase column) {
			return new TreeListCellValidationEventArgs(source, value, rowHandle, this, column);
		}
		internal override BaseValidationError CreateCellValidationError(object errorContent, ErrorType errorType, int rowHandle, ColumnBase column) {
			return new TreeListCellValidationError(errorContent, null, errorType, rowHandle, GetNodeByRowHandle(rowHandle), column);
		}
		internal override BaseValidationError CreateRowValidationError(object errorContent, ErrorType errorType, int rowHandle) {
			return new TreeListNodeValidationError(errorContent, null, errorType, rowHandle, GetNodeByRowHandle(rowHandle));
		}
		internal override string RaiseCustomDisplayText(int? rowHandle, int? listSourceIndex, ColumnBase column, object value, string displayText) {
			if (rowHandle == null) return displayText;
			return RaiseCustomColumnDisplayText(GetNodeByRowHandle(rowHandle.Value), column, value, displayText);
		}
		internal override bool? RaiseCustomDisplayText(int? rowHandle, int? listSourceIndex, ColumnBase column, object value, string originalDisplayText, out string displayText) {
			displayText = RaiseCustomDisplayText(rowHandle, listSourceIndex, column, value, originalDisplayText);
			if(customColumnDisplayText == null)
				return false;
			if(customColumnDisplayTextEventArgs.ShowAsNullText)
				return null;
			return true;
		}
		#endregion
		#region MultiSelect
		[Obsolete("Use the DataControlBase.BeginSelection method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void BeginSelection() {
			SelectionStrategy.BeginSelection();
		}
		[Obsolete("Use the DataControlBase.EndSelection method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void EndSelection() {
			SelectionStrategy.EndSelection();
		}
		[Obsolete("Use the DataControlBase.SelectItem method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void SelectNode(int rowHandle) {
			SelectionStrategy.SelectRow(rowHandle);
		}
		[Obsolete("Use the DataControlBase.SelectItem method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void SelectNode(TreeListNode node) {
			SelectNodeCore(node);
		}
		internal void SelectNodeCore(TreeListNode node) {
			SelectionStrategy.SelectRow(TreeListDataProvider.GetRowHandleByNode(node));
		}
		[Obsolete("Use the DataControlBase.UnselectItem method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void UnselectNode(int rowHandle) {
			SelectionStrategy.UnselectRow(rowHandle);
		}
		internal void UnselectNodeCore(TreeListNode node) {
			SelectionStrategy.UnselectRow(TreeListDataProvider.GetRowHandleByNode(node));
		}
		[Obsolete("Use the DataControlBase.UnselectItem method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void UnselectNode(TreeListNode node) {
			UnselectNodeCore(node);
		}
		[Obsolete("Use the DataControlBase.SelectRange method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void SelectRange(int startRowHandle, int endRowHandle) {
			SelectRangeCore(startRowHandle, endRowHandle);
		}
		[Obsolete("Use the DataControlBase.SelectRange method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void SelectRange(TreeListNode startNode, TreeListNode endNode) {
			SelectRangeCore(startNode, endNode);
		}
		internal void SelectRangeCore(TreeListNode startNode, TreeListNode endNode) {
			SelectRangeCore(TreeListDataProvider.GetRowHandleByNode(startNode), TreeListDataProvider.GetRowHandleByNode(endNode));
		}
		[Obsolete("Use the DataControlBase.UnselectAll method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void ClearSelection() {
			SelectionStrategy.ClearSelection();
		}
		public void ReloadChildNodes(int rowHandle) {
			ReloadChildNodes(GetNodeByRowHandle(rowHandle));
		}
		public void ReloadChildNodes(TreeListNode node) {
			TreeListDataProvider.ReloadChildNodes(node);
		}
		protected internal override GridSelectionChangedEventArgs CreateSelectionChangedEventArgs(DevExpress.Data.SelectionChangedEventArgs e) {
			return new TreeListSelectionChangedEventArgs(this, e.Action, e.ControllerRow);
		}
		protected internal override void RaiseSelectionChanged(GridSelectionChangedEventArgs e) {
			e.RoutedEvent = TreeListView.SelectionChangedEvent;
			RaiseEvent(e);
		}
		#endregion
		#region CellSelection
		public void SelectCell(int rowHandle, ColumnBase column) {
			TreeListViewBehavior.SelectCell(rowHandle, column);
		}
		public void SelectCell(TreeListNode node, ColumnBase column) {
			SelectCell(node.RowHandle, column);
		}
		public void UnselectCell(int rowHandle, ColumnBase column) {
			TreeListViewBehavior.UnselectCell(rowHandle, column);
		}
		public void UnselectCell(TreeListNode node, ColumnBase column) {
			UnselectCell(node.RowHandle, column);
		}
		public void SelectCells(int startRowHandle, ColumnBase startColumn, int endRowHandle, ColumnBase endColumn) {
			TreeListViewBehavior.SelectCells(startRowHandle, startColumn, endRowHandle, endColumn);
		}
		public void SelectCells(TreeListNode startNode, ColumnBase startColumn, TreeListNode endNode, ColumnBase endColumn) {
			SelectCells(startNode.RowHandle, startColumn, endNode.RowHandle, endColumn);
		}
		public void UnselectCells(int startRowHandle, ColumnBase startColumn, int endRowHandle, ColumnBase endColumn) {
			TreeListViewBehavior.UnselectCells(startRowHandle, startColumn, endRowHandle, endColumn);
		}
		public void UnselectCells(TreeListNode startNode, ColumnBase startColumn, TreeListNode endNode, ColumnBase endColumn) {
			UnselectCells(startNode.RowHandle, startColumn, endNode.RowHandle, endColumn);
		}
		public bool IsCellSelected(int rowHandle, ColumnBase column) {
			return TreeListViewBehavior.IsCellSelected(rowHandle, column);
		}
		public bool IsCellSelected(TreeListNode nodes, ColumnBase column) {
			return IsCellSelected(nodes.RowHandle, column);
		}
		public IList<TreeListCell> GetSelectedCells() {
			return new SimpleBridgeList<TreeListCell, CellBase>(TreeListViewBehavior.GetSelectedCells());
		}
		#endregion
		#region Format Conditions
		[Category(Categories.Appearance), XtraSerializableProperty(true, false, false), GridUIProperty, XtraResetProperty]
		public FormatConditionCollection FormatConditions { get { return TreeListViewBehavior.FormatConditions; } }
		[XtraSerializableProperty]
		public bool AllowConditionalFormattingMenu {
			get { return (bool)GetValue(AllowConditionalFormattingMenuProperty); }
			set { SetValue(AllowConditionalFormattingMenuProperty, value); }
		}
		[XtraSerializableProperty]
		public bool AllowConditionalFormattingManager {
			get { return (bool)GetValue(AllowConditionalFormattingManagerProperty); }
			set { SetValue(AllowConditionalFormattingManagerProperty, value); }
		}
		public FormatInfoCollection PredefinedFormats {
			get { return (FormatInfoCollection)GetValue(PredefinedFormatsProperty); }
			set { SetValue(PredefinedFormatsProperty, value); }
		}
		public FormatInfoCollection PredefinedColorScaleFormats {
			get { return (FormatInfoCollection)GetValue(PredefinedColorScaleFormatsProperty); }
			set { SetValue(PredefinedColorScaleFormatsProperty, value); }
		}
		public FormatInfoCollection PredefinedDataBarFormats {
			get { return (FormatInfoCollection)GetValue(PredefinedDataBarFormatsProperty); }
			set { SetValue(PredefinedDataBarFormatsProperty, value); }
		}
		public FormatInfoCollection PredefinedIconSetFormats {
			get { return (FormatInfoCollection)GetValue(PredefinedIconSetFormatsProperty); }
			set { SetValue(PredefinedIconSetFormatsProperty, value); }
		}
		public DataTemplate FormatConditionDialogServiceTemplate {
			get { return (DataTemplate)GetValue(FormatConditionDialogServiceTemplateProperty); }
			set { SetValue(FormatConditionDialogServiceTemplateProperty, value); }
		}
		public DataTemplate ConditionalFormattingManagerServiceTemplate {
			get { return (DataTemplate)GetValue(ConditionalFormattingManagerServiceTemplateProperty); }
			set { SetValue(ConditionalFormattingManagerServiceTemplateProperty, value); }
		}
		public void AddFormatCondition(FormatConditionBase formatCondition) {
			TreeListViewBehavior.AddFormatConditionCore(formatCondition);
		}
		public void ShowFormatConditionDialog(ColumnBase column, FormatConditionDialogType dialogKind) {
			TreeListViewBehavior.ShowFormatConditionDialogCore(column, dialogKind);
		}
		public void ClearFormatConditionsFromAllColumns() {
			TreeListViewBehavior.ClearFormatConditionsFromAllColumnsCore();
		}
		public void ClearFormatConditionsFromColumn(ColumnBase column) {
			TreeListViewBehavior.ClearFormatConditionsFromColumnCore(column);
		}
		public void ShowConditionalFormattingManager(ColumnBase column) {
			TreeListViewBehavior.ShowConditionalFormattingManagerCore(column);
		}
		#region Serialization
		protected virtual void OnDeserializeCreateCollectionItem(XtraCreateCollectionItemEventArgs e) {
			if(e.CollectionName == "FormatConditions")
				TreeListViewBehavior.OnDeserializeCreateFormatCondition(e);
		}
		protected override void OnDeserializeStart(StartDeserializingEventArgs e) {
			base.OnDeserializeStart(e);
			TreeListViewBehavior.OnDeserializeFormatConditionsStart();
		}
		protected override void OnDeserializeEnd(EndDeserializingEventArgs e) {
			base.OnDeserializeEnd(e);
			TreeListViewBehavior.OnDeserializeFormatConditionsEnd();
		}
		#endregion
		#endregion
		#region Clipboard
		internal override bool CanCopyRows() {
			if(ActualClipboardCopyAllowed && (NavigationStyle != GridViewNavigationStyle.None) && (!IsInvalidFocusedRowHandle || DataControl.HasSelectedItems) && (ActiveEditor == null)) 
				return true;
			return false;
		}
		[Obsolete("Use the TreeListControl.CopyRowsToClipboard method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void CopyRowsToClipboard(IEnumerable<TreeListNode> nodes) {
			CopyRowsToClipboardCore(nodes);
		}
		internal void CopyRowsToClipboardCore(IEnumerable<TreeListNode> nodes) {
			ClipboardController.CopyRowsToClipboard(GetRowHandlesFromNodes(nodes));
		}
		[Obsolete("Use the TreeListControl.CopyRangeToClipboard method instead"), EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public void CopyRangeToClipboard(TreeListNode startNode, TreeListNode endNode) {
			CopyRangeToClipboardCore(startNode, endNode);
		}
		internal void CopyRangeToClipboardCore(TreeListNode startNode, TreeListNode endNode) {
			ClipboardController.CopyRangeToClipboard(TreeListDataProvider.GetRowHandleByNode(startNode), TreeListDataProvider.GetRowHandleByNode(endNode));
		}
		protected internal override bool RaiseCopyingToClipboard(CopyingToClipboardEventArgsBase e) {
			e.RoutedEvent = TreeListView.CopyingToClipboardEvent;
			RaiseEvent(e);
			return e.Handled;
		}
		public void CopySelectedCellsToClipboard() {
			ClipboardController.CopyCellsToClipboard(GetSelectedCells());
		}
		public void CopyCellsToClipboard(IEnumerable<TreeListCell> cells) {
			ClipboardController.CopyCellsToClipboard(cells);
		}
		public void CopyCellsToClipboard(int startRowHandle, ColumnBase startColumn, int endRowHandle, ColumnBase endColumn) {
			TreeListViewBehavior.CopyCellsToClipboard(startRowHandle, startColumn, endRowHandle, endColumn);
		}
		#endregion
		#region ClipboardFormat
		internal TreeListViewClipboardHelper ClipboardHelperManager { get; private set; }
		IClipboardManager<ColumnWrapper, TreeListNodeWrapper> clipboardManager;
		IClipboardManager<ColumnWrapper, TreeListNodeWrapper> ClipboardManager {
			get {
				if(clipboardManager == null)
					clipboardManager = CreateClipboardManager();
				return clipboardManager;
			}
		}
		protected virtual IClipboardManager<ColumnWrapper, TreeListNodeWrapper> CreateClipboardManager() {
			ClipboardHelperManager = new TreeListViewClipboardHelper(this);
			return (IClipboardManager<ColumnWrapper, TreeListNodeWrapper>)PrintHelper.ClipboardExportManagerInstance(typeof(ColumnWrapper), typeof(TreeListNodeWrapper), ClipboardHelperManager);
		}
		protected internal override bool SetDataAwareClipboardData() {
			try {
				SetActualClipboardOptions(OptionsClipboard);
				if(ClipboardManager != null && ClipboardHelperManager != null && !ClipboardHelperManager.CanCopyToClipboard())
					return false;
				System.Windows.Forms.DataObject data = new System.Windows.Forms.DataObject();
				ClipboardManager.AssignOptions(OptionsClipboard);
				ClipboardManager.SetClipboardData(data);
				if(data.GetFormats().Count() == 0)
					return false;
				Clipboard.SetDataObject(data);
				return true;
			}
			catch {
				return false;
			}
		}
		#endregion
		#region IPrintableControl Members
		protected override IRootDataNode CreateRootNode(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize) {
			return GridPrintingHelper.CreatePrintingTreeNode(this, usablePageSize);
		}
		protected override IVisualTreeWalker GetCustomVisualTreeWalker() {
			return null;
		}
		protected override void PagePrintedCallback(IEnumerator pageBrickEnumerator, Dictionary<IVisualBrick, IOnPageUpdater> brickUpdaters) {
			bool printHeaders = PrintColumnHeaders && ShowColumnHeaders;
			GridPrintingHelper.UpdatePageBricks(pageBrickEnumerator, brickUpdaters, !printHeaders, (PrintTotalSummary && ShowTotalSummary) || (PrintFixedTotalSummary && ShowFixedTotalSummary));
		}
		protected override bool GetCanCreateRootNodeAsync() {
			return false;
		}
		protected override void CreateRootNodeAsync(Size usablePageSize, Size reportHeaderSize, Size reportFooterSize, Size pageHeaderSize, Size pageFooterSize) {
			throw new NotImplementedException();
		}
		protected override void AddCreateRootNodeCompletedEvent(EventHandler<ScalarOperationCompletedEventArgs<IRootDataNode>> eventHandler) { }
		protected override void RemoveCreateRootNodeCompletedEvent(EventHandler<ScalarOperationCompletedEventArgs<IRootDataNode>> eventHandler) { }
		protected internal override PrintingDataTreeBuilderBase CreatePrintingDataTreeBuilder(double totalHeaderWidth, ItemsGenerationStrategyBase itemsGenerationStrategy, MasterDetailPrintInfo masterDetailPrintInfo, BandsLayoutBase bandsLayout) {
			return new TreeListPrintingDataTreeBuilder(this, totalHeaderWidth, bandsLayout);
		}
		protected internal override DataTemplate GetPrintRowTemplate() {
			return PrintRowTemplate;
		}
		#endregion
		protected override void OnCustomShouldSerializeProperty(CustomShouldSerializePropertyEventArgs e) {
			base.OnCustomShouldSerializeProperty(e);
			if(e.DependencyProperty == TreeListView.TreeDerivationModeProperty)
				e.CustomShouldSerialize = true;
		}	 
#if !SL
		[Browsable(false)]
		public bool ShouldSerializeColumnChooserBandsSortOrderComparer(System.Windows.Markup.XamlDesignerSerializationManager manager) {
			return false;
		}
#endif
	}
	public class TreeListViewBehavior : GridTableViewBehaviorBase {
		public TreeListViewBehavior(TreeListView view)
			: base(view) {
		}
		protected TreeListView TreeListView { get { return (TreeListView)View; } }
		protected internal override bool IsAlternateRow(int rowHandle) {
			if(DataControl != null && TreeListView.AlternationCount > 0 && TreeListView.ActualAlternateRowBackground != null) {
				int visibleIndex = DataControl.GetRowVisibleIndexByHandleCore(rowHandle);
				return visibleIndex % TreeListView.AlternationCount == (TreeListView.AlternationCount - 1);
			}
			return false;
		}
		protected internal override System.Windows.Media.Brush ActualAlternateRowBackground { get { return TreeListView.ActualAlternateRowBackground; } } 
		protected internal override RowData CreateRowDataCore(DataTreeBuilder treeBuilder, bool updateDataOnly) {
			return new TreeListRowData(treeBuilder);
		}
		protected internal override GridViewInfo CreateViewInfo() {
			return new TreeListViewInfo(TreeListView);
		}
		protected internal override GridViewInfo CreatePrintViewInfo() {
			return CreatePrintViewInfo(null);
		}
		protected internal override GridViewInfo CreatePrintViewInfo(BandsLayoutBase bandsLayout) {
			return new TreeListPrintViewInfo(TreeListView, bandsLayout);
		}
		internal override GridViewNavigationBase CreateRowNavigation() {
			return new TreeListViewRowNavigation(View);
		}
		internal override GridViewNavigationBase CreateCellNavigation() {
			return new TreeListViewCellNavigation(View);
		}
		internal override BestFitControlBase CreateBestFitControl(ColumnBase column) {
			return CreateElement(() => new TreeListLightweightBestFitControl(TreeListView, column), () => new BestFitControl(View, column), UseLightweightTemplates.All) as BestFitControlBase;
		}
		internal override void UpdateColumnsLayout() {
			base.UpdateColumnsLayout();
			TreeListView.CalcMinWidth();
		}
		internal override void UpdateActualDataRowTemplateSelector() {
			View.UpdateActualTemplateSelector(TableView.ActualDataRowTemplateSelectorPropertyKey, TreeListView.DataRowTemplateSelector, TreeListView.DataRowTemplate, (s, t) => new TreeListRowTemplateSelectorWrapper(s, t));
			if(TreeListView.TreeDerivationMode == TreeDerivationMode.HierarchicalDataTemplate)
				TreeListView.DoRefresh(true);
		}
		internal bool CanBestFitColumn(object columnId) {
			GridColumnBase column = (GridColumnBase)View.GetColumnByCommandParameter(columnId);
			return column != null && base.CanBestFitColumn(column);
		}
		protected internal override Style AutoFilterRowCellStyle { get { return TreeListView.AutoFilterRowCellStyle; } }
		protected internal override double GetFixedNoneContentWidth(double totalWidth, int rowHandle) {
			if(TreeListView.FixedLeftVisibleColumns.Count != 0)
				return totalWidth;
			int rowIndents = 0;
			TreeListNode node = TreeListView.GetNodeByRowHandle(rowHandle);
			if(node != null) {
				rowIndents = node.ActualLevel;
				rowIndents += TreeListView.ServiceIndentsCount;
			}
			return totalWidth - TreeListView.RowIndent * rowIndents - (TableView.ShowIndicator ? 0 : TableView.LeftDataAreaIndent);
		}
		internal override int GetTopRow(int pageVisibleTopRowIndex) {
			return View.DataProviderBase.GetControllerRow(pageVisibleTopRowIndex);
		}
		internal override CustomBestFitEventArgsBase RaiseCustomBestFit(ColumnBase column, BestFitMode bestFitMode) {
			TreeListCustomBestFitEventArgs e = new TreeListCustomBestFitEventArgs((ColumnBase)column, bestFitMode);
			View.RaiseEvent(e);
			return e;
		}
		internal override GridColumnData GetGroupSummaryColumnData(int rowHandle, IBestFitColumn column) { return null; }
#if !SL
		internal override bool UseDataRowTemplate(RowData rowData) {
			return TreeListView.TreeDerivationMode == TreeDerivationMode.HierarchicalDataTemplate || base.UseDataRowTemplate(rowData);
		}
		internal override void ValidateRowStyle(Style newStyle) {
			if(!canChangeUseLightweightTemplates)
				base.ValidateStyle(newStyle, typeof(GridRowContent), typeof(RowControl), UseLightweightRows ? WrongRowStyleTargetTypeError : WrongRowStyleTargetTypeErrorUnoptimized);
		}
		protected override bool UseOptimizedTemplate { get { return base.UseOptimizedTemplate && TreeListView.TreeDerivationMode != TreeDerivationMode.HierarchicalDataTemplate; } }
		protected override Style ValidateStyle(Style style, Type normalTargetType, Type optimizedTargetType, string errorMessage) {
			if(UseLightweightRows && TreeListView.TreeDerivationMode == TreeDerivationMode.HierarchicalDataTemplate) {
				if(style is DefaultStyle)
					return style;
				return base.ValidateStyle(style, normalTargetType, normalTargetType, WrongStyleTargetTypeInHierarchicalDataTemplateModeError);
			}
			return base.ValidateStyle(style, normalTargetType, optimizedTargetType, errorMessage);
		}
		internal const string WrongStyleTargetTypeInHierarchicalDataTemplateModeError = "When the TreeList is in HierarchicalDataTemplate mode, optimized mode is disabled, thus the following Style target type is not valid.";
#endif
	}
	public class TreeListBestFitRowControl : BestFitRowControl {
		bool allowDefaultContent;
		public TreeListBestFitRowControl(RowData rowData, GridColumnData cellData, bool allowDefaultContent) : base(rowData, cellData) {
			this.allowDefaultContent = allowDefaultContent;
		}
		protected BestFitGridCellContentPresenter ContentPresenter { get; private set; }
		protected override void CreateTemplateContent() {
			if(allowDefaultContent) {
				ContentPresenter = new BestFitGridCellContentPresenter() { HasRightSibling = true };
				ContentPresenter.DataContext = CellData;
				ContentPresenter.Column = CellData.Column;
				ContentPresenter.RowData = rowData;
				ContentPresenter.Style = CellData.Column.ActualCellStyle;
				AddPanelElement(ContentPresenter, 0);
			}
			else {
				base.CreateTemplateContent();
			}
		}
		protected internal virtual void UpdateIsFocusedCell(bool isFocusedCell) {
			if(ContentPresenter != null)
				ContentPresenter.IsFocusedCell = isFocusedCell;
		}
	}
	public class TreeListLightweightBestFitControl : LightweightBestFitControl {
		public TreeListLightweightBestFitControl(TreeListView view, ColumnBase column)
			: base(view, column) {
				TreeListView = view;
		}
		protected TreeListView TreeListView { get; private set; }
		protected override BestFitRowControl CreateBestFitRowControl() {
			return new TreeListBestFitRowControl(RowData, CellData, TreeListView.TreeDerivationMode == TreeDerivationMode.HierarchicalDataTemplate);
		}
		public override void UpdateIsFocusedCell(bool isFocusedCell) {
			if(Content != null)
				((TreeListBestFitRowControl)Content).UpdateIsFocusedCell(isFocusedCell);
		}
	}
	public class TreeListRowTemplateSelectorWrapper : ActualTemplateSelectorWrapper {
		public TreeListRowTemplateSelectorWrapper(DataTemplateSelector dataTemplateSelector, DataTemplate dataTemplate)
			: base(dataTemplateSelector, dataTemplate) { }
		public override DataTemplate SelectTemplate(object item, DependencyObject container) {
			return SelectTemplateCore(item, container, true);
		}
		internal DataTemplate SelectTemplateCore(object item, DependencyObject container, bool checkActual) {
			DataTemplate baseTemplate = base.SelectTemplate(item, container);
			DataTemplate resultDataTemplate = null;
			TreeListRowData rowData = item as TreeListRowData;
			TreeListView treeView = null;
#if SL
			if(checkActual) {
				DevExpress.Xpf.Core.HierarchicalDataTemplate hTemplate = baseTemplate as DevExpress.Xpf.Core.HierarchicalDataTemplate;
				if(hTemplate != null)
					resultDataTemplate = hTemplate.GetActualTemplate() ?? baseTemplate;
			} else
#endif
			if(rowData != null) {
				treeView = rowData.View as TreeListView;
				if(!ReferenceEquals(baseTemplate, treeView.DefaultDataRowTemplate))
					resultDataTemplate = baseTemplate;
				else
					if(rowData.Node != null) {
						DataTemplate templ = rowData.Node.Template;
#if SL
					if(templ != null) {
						HierarchicalDataTemplate hTempl = templ as HierarchicalDataTemplate;
						if(hTempl != null)
							resultDataTemplate = hTempl.GetActualTemplate() ?? templ;
						else
							resultDataTemplate = templ;
					}
#else
						if(templ != null)
							resultDataTemplate = templ;
#endif
					}
			}
			resultDataTemplate = resultDataTemplate ?? baseTemplate;
			if(treeView != null && treeView.AllowDefaultContentForHierarchicalDataTemplate
				&& treeView.TreeDerivationMode == TreeDerivationMode.HierarchicalDataTemplate
				&& resultDataTemplate != null
				&& checkActual
#if SL
				&& resultDataTemplate.LoadContent() == null
#else
				&& resultDataTemplate.Template == null
#endif
				)
				return treeView.DefaultDataRowTemplate;
			else
				return resultDataTemplate;
		}
	}
	public class TreeListViewInfo : GridViewInfo {
		public TreeListViewInfo(TreeListView view)
			: base(view) {
		}
		public TreeListView TreeListView { get { return (TreeListView)GridView; } }
		bool HasVisibleBands { get { return Grid != null && Grid.BandsLayoutCore != null && Grid.BandsLayoutCore.VisibleBands.Count != 0; } }
		public override int GroupCount {
			get {
				if(TreeListView.ColumnsCore.Count == 0 && !HasVisibleBands)
					return 0;
				return MaxVisibleLevel + TreeListView.ServiceIndentsCount;
			}
		}
		public int MaxVisibleLevel { get { return TreeListView.TreeListDataProvider.MaxVisibleLevel; } }
		public override double TotalGroupAreaIndent { get { return TreeListView.RowIndent * GroupCount;  } }
		public override double RightGroupAreaIndent { get { return 0; } }
	}
	public class TreeListPrintViewInfo : TreeListViewInfo {
		BandsLayoutBase bandsLayoutCore;
		public override BandsLayoutBase BandsLayout { get { return bandsLayoutCore; } }
		readonly List<ColumnBase> visibleColumns;
		public override IList<ColumnBase> VisibleColumns { get { return visibleColumns; } }
		public TreeListPrintViewInfo(TreeListView view, BandsLayoutBase bandsLayout) : base(view) {
			visibleColumns = view.PrintableColumns.ToList();
			bandsLayoutCore = bandsLayout;
		}
		public int MaxLevel { get { return TreeListView.TreeListDataProvider.MaxLevel; } }
		public override int GroupCount { get { return (TreeListView.PrintAllNodes ? MaxLevel : MaxVisibleLevel) + 1 + (TreeListView.PrintNodeImages ? 1 : 0); } }
		public override double TotalGroupAreaIndent { get {  return GridPrintingHelper.GroupIndent * GroupCount; } }
	}
	public class TreeListRowsClipboardController : RowsClipboardController {
		public TreeListRowsClipboardController(TreeListView view) : base(view) { }
		protected new TreeListView View { get { return base.View as TreeListView; } }
		public void CopyCellsToClipboard(IEnumerable<TreeListCell> cells) {
			CopyToClipboard(() => { return CreateCellsCopyingToClipboardEventArgs(cells); }, new TreeListCellsClipboardDataProvider(cells, this));
		}
		protected override CopyingToClipboardEventArgsBase CreateRowsCopyingToClipboardEventArgs(IEnumerable<int> rows) {
			return new TreeListCopyingToClipboardEventArgs(View, rows, true);
		}
		protected virtual CopyingToClipboardEventArgsBase CreateCellsCopyingToClipboardEventArgs(IEnumerable<TreeListCell> cells) {
			return new TreeListCopyingToClipboardEventArgs(View, cells, true);
		}
		public class TreeListCellsClipboardDataProvider : IClipboardDataProvider {
			public TreeListCellsClipboardDataProvider(IEnumerable<TreeListCell> cells, TreeListRowsClipboardController clipboardController) {
				ClipboardController = clipboardController;
				Cells = cells;
			}
			protected TreeListRowsClipboardController ClipboardController { get; private set; }
			protected TreeListView View { get { return ClipboardController.View; } }
			protected IEnumerable<TreeListCell> Cells { get; private set; }
			public object GetObjectFromClipboard() {
				List<int> rows = new List<int>();
				foreach(TreeListCell cell in Cells)
					if(!rows.Contains(cell.RowHandle))
						rows.Add(cell.RowHandle);
				return ClipboardController.GetSelectedData(rows);
			}
			public string GetTextFromClipboard() {
				if(Cells.Count() == 0) return string.Empty;
				StringBuilder sb = new StringBuilder();
				List<TreeListCell> cells = PrepareCells(Cells);
				List<ColumnBase> columns = PrepareColumns(cells);
				if(View.ActualClipboardCopyWithHeaders) {
					AppendHeadersText(sb, columns);
					AppendNewLine(sb);
				}
				AppendCellsText(cells, sb, columns);
				return sb.ToString();
			}
			protected List<TreeListCell> PrepareCells(IEnumerable<TreeListCell> cells) {
				return Cells.OrderBy(cell => cell, new CellComparer(View)).ToList<TreeListCell>();
			}
			protected List<ColumnBase> PrepareColumns(List<TreeListCell> cells) {
				List<ColumnBase> columns = new List<ColumnBase>();
				foreach(TreeListCell cell in cells) {
					if(!columns.Contains(cell.Column))
						columns.Add(cell.Column);
				}
				columns.Sort((column1, column2) => Comparer<int>.Default.Compare(column1.VisibleIndex, column2.VisibleIndex));
				return columns;
			}
			protected virtual void AppendHeadersText(StringBuilder sb, List<ColumnBase> columns) {
				if(columns.Count == 0)
					return;
				foreach(ColumnBase column in columns)
					sb.Append(View.GetTextForClipboard(DataControlBase.InvalidRowHandle, View.VisibleColumns.IndexOf(column)) + "\t");
				sb.Remove(sb.Length - 1, 1);
			}
			protected virtual void AppendCellsText(List<TreeListCell> cells, StringBuilder sb, List<ColumnBase> columns) {
				int prevRowHandle = cells[0].RowHandle, lastColumnIndex = 0;
				foreach(TreeListCell cell in cells) {
					if(prevRowHandle != cell.RowHandle) {
						sb.Append(Environment.NewLine);
						lastColumnIndex = 0;
					}
					for(int i = 0; i < columns.IndexOf(cell.Column) - lastColumnIndex; i++)
						sb.Append("\t");
					prevRowHandle = cell.RowHandle;
					lastColumnIndex = columns.IndexOf(cell.Column);
					sb.Append(View.GetTextForClipboard(cell.RowHandle, View.VisibleColumns.IndexOf(cell.Column)));
				}
			}
			protected void AppendNewLine(StringBuilder sb) {
				sb.Append(Environment.NewLine);
			}
		}
	}
	public abstract class TreeListNodeImageSelector {
		internal 
#if DEBUGTEST
			virtual
#endif
			bool CanSelect(TreeListRowData rowData) {
			return !rowData.View.IsDesignTime;
		}
		public virtual ImageSource Select(TreeListRowData rowData) { return null; }
	}
}
namespace DevExpress.Xpf.Grid.TreeList.Native {
	public class TreeListDataIterator : DataIteratorBase {
		TreeListView view { get { return (TreeListView)viewBase; } }
		public TreeListDataIterator(TreeListView view)
			: base(view) {
		}
		protected internal override RowNode GetRowNodeForCurrentLevel(DataNodeContainer nodeContainer, int index, int startVisibleIndex, ref bool shouldBreak) {
			DataControllerValuesContainer info = CreateValuesContainer(nodeContainer.treeBuilder, index);
			return GetRowNode(nodeContainer.treeBuilder, startVisibleIndex, info);
		}
		protected internal override RowNode GetSummaryNodeForCurrentNode(DataNodeContainer nodeContainer, RowHandle rowHandle, int index) { return null; }
	}
}
