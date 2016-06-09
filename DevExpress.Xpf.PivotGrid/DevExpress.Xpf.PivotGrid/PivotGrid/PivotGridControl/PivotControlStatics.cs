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
using System.Windows.Input;
using System.Windows.Media;
using DevExpress.Data.Filtering;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.Xpf.PivotGrid.Printing;
using DevExpress.Xpf.PivotGrid.Serialization;
using DevExpress.XtraPivotGrid.Data;
using CoreXtraPivotGrid = DevExpress.XtraPivotGrid;
using System.ComponentModel;
using DevExpress.Utils;
#if SL
using DevExpress.Xpf.Core.WPFCompatibility;
using DevExpress.Xpf.Core.WPFCompatibility.Helpers;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using Visual = System.Windows.UIElement;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using ApplicationException = System.Exception;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Mvvm;
using DevExpress.Mvvm.Native;
#endif
namespace DevExpress.Xpf.PivotGrid {
	public partial class PivotGridControl {
		public static readonly Size DefaultExcelListSize = new Size(352, 446);
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public static readonly DependencyProperty DataProperty;
		static readonly DependencyPropertyKey DataPropertyKey;
		public static readonly DependencyProperty PivotGridProperty;
		public static readonly DependencyProperty DataSourceProperty;
		public static readonly DependencyProperty OlapConnectionStringProperty;
#if !SL
		public static readonly DependencyProperty OlapDataProviderProperty;
#endif
		public static readonly DependencyProperty OlapDefaultMemberFieldsProperty;
		public static readonly DependencyProperty OlapFilterByUniqueNameProperty;
		public static readonly DependencyProperty PrefilterCriteriaProperty;
		public static readonly DependencyProperty PrefilterStringProperty;
		static readonly DependencyPropertyKey ShowPrefilterPanelPropertyKey;
		public static readonly DependencyProperty ShowPrefilterPanelProperty;
		public static readonly DependencyProperty ShowPrefilterPanelModeProperty;
		static readonly DependencyPropertyKey PrefilterPanelTextPropertyKey;
		public static readonly DependencyProperty PrefilterPanelTextProperty;
		public static readonly DependencyProperty IsPrefilterEnabledProperty;
		public static readonly DependencyProperty CanEnablePrefilterProperty;
		internal static readonly DependencyPropertyKey CanEnablePrefilterPropertyKey;
		public static readonly DependencyProperty IsMainWaitIndicatorVisibleProperty;
		internal static readonly DependencyPropertyKey IsMainWaitIndicatorVisiblePropertyKey;
		public static readonly DependencyProperty IsFilterPopupWaitIndicatorVisibleProperty;
		internal static readonly DependencyPropertyKey IsFilterPopupWaitIndicatorVisiblePropertyKey;
		internal static readonly DependencyPropertyKey ActiveFilterInfoPropertyKey;
		public static readonly DependencyProperty ActiveFilterInfoProperty;
		internal static readonly DependencyPropertyKey ShowColumnsBorderPropertyKey;
		public static readonly DependencyProperty ShowColumnsBorderProperty;
		internal static readonly DependencyPropertyKey UserActionPropertyKey;
		public static readonly DependencyProperty UserActionProperty;
		public static readonly DependencyProperty FieldHeaderDragIndicatorSizeProperty;
		public static readonly DependencyProperty FieldHeaderDragIndicatorTemplateProperty;
		internal const string FieldHeaderDragIndicatorTemplatePropertyName = "FieldHeaderDragIndicatorTemplate";
		public static readonly DependencyProperty FieldHeaderTemplateProperty;
		public static readonly DependencyProperty FieldHeaderTemplateSelectorProperty;
		public static readonly DependencyProperty FieldHeaderTreeViewTemplateProperty;
		public static readonly DependencyProperty FieldHeaderTreeViewTemplateSelectorProperty;
		public static readonly DependencyProperty FieldCellKpiTemplateProperty;
		public static readonly DependencyProperty FieldCellKpiTemplateSelectorProperty;
		public static readonly DependencyProperty FieldHeaderListTemplateProperty;
		public static readonly DependencyProperty FieldHeaderListTemplateSelectorProperty;
		public static readonly DependencyProperty FieldHeaderContentStyleProperty;
		public static readonly DependencyProperty FieldCellTemplateProperty;
		public static readonly DependencyProperty FieldCellTemplateSelectorProperty;
		public static readonly DependencyProperty FieldValueTemplateProperty;
		public static readonly DependencyProperty FieldValueTemplateSelectorProperty;
		public static readonly DependencyProperty FocusedCellBorderTemplateProperty;
		public static readonly DependencyProperty ResizingIndicatorTemplateProperty;
		public static readonly DependencyProperty ScrollViewerTemplateProperty;
		public static readonly DependencyProperty AllowCrossGroupVariationProperty;
		public static readonly DependencyProperty GroupDataCaseSensitiveProperty;
		public static readonly DependencyProperty AutoExpandGroupsProperty;
		public static readonly DependencyProperty DrillDownMaxRowCountProperty;
		public static readonly DependencyProperty DataFieldUnboundExpressionModeProperty;
		public static readonly DependencyProperty FilterByVisibleFieldsOnlyProperty;
		public static readonly DependencyProperty AllowCustomizationFormProperty;
		public static readonly DependencyProperty AllowDragProperty;
		public static readonly DependencyProperty AllowDragInCustomizationFormProperty;
		public static readonly DependencyProperty AllowExpandProperty;
		public static readonly DependencyProperty AllowFilterProperty;
		public static readonly DependencyProperty AllowResizingProperty;
		public static readonly DependencyProperty AllowSortProperty;
		public static readonly DependencyProperty AllowSortBySummaryProperty;
		public static readonly DependencyProperty AllowHideFieldsProperty;
		public static readonly DependencyProperty AllowPrefilterProperty;
		public static readonly DependencyProperty FieldListStyleProperty;
		public static readonly DependencyProperty FieldListLayoutProperty;
		public static readonly DependencyProperty FieldListAllowedLayoutsProperty;
		public static readonly DependencyProperty FieldListSplitterXProperty;
		public static readonly DependencyProperty FieldListSplitterYProperty;
		public static readonly DependencyProperty DeferredUpdatesProperty;
		public static readonly DependencyProperty AllowExpandOnDoubleClickProperty;
		public static readonly DependencyProperty AllowFilterInFieldListProperty;
		public static readonly DependencyProperty AllowSortInFieldListProperty;
		public static readonly DependencyProperty CellForegroundProperty;
		public static readonly DependencyProperty CellTotalForegroundProperty;
		public static readonly DependencyProperty CellTotalSelectedForegroundProperty;
		public static readonly DependencyProperty CellSelectedForegroundProperty;
		public static readonly DependencyProperty CellBackgroundProperty;
		public static readonly DependencyProperty CellTotalBackgroundProperty;
		public static readonly DependencyProperty CellTotalSelectedBackgroundProperty;
		public static readonly DependencyProperty CellSelectedBackgroundProperty;
		public static readonly DependencyProperty ValueForegroundProperty;
		public static readonly DependencyProperty ValueTotalForegroundProperty;
		public static readonly DependencyProperty ValueTotalSelectedForegroundProperty;
		public static readonly DependencyProperty ValueSelectedForegroundProperty;
		public static readonly DependencyProperty ValueBackgroundProperty;
		public static readonly DependencyProperty ValueTotalBackgroundProperty;
		public static readonly DependencyProperty ValueTotalSelectedBackgroundProperty;
		public static readonly DependencyProperty ValueSelectedBackgroundProperty;
		public static readonly DependencyProperty PrintCellForegroundProperty;
		public static readonly DependencyProperty PrintCellTotalForegroundProperty;
		public static readonly DependencyProperty PrintCellBackgroundProperty;
		public static readonly DependencyProperty PrintCellTotalBackgroundProperty;
		public static readonly DependencyProperty PrintValueForegroundProperty;
		public static readonly DependencyProperty PrintValueTotalForegroundProperty;
		public static readonly DependencyProperty PrintValueBackgroundProperty;
		public static readonly DependencyProperty PrintValueTotalBackgroundProperty;
		public static readonly DependencyProperty DrawFocusedCellRectProperty;
		public static readonly DependencyProperty ShowColumnGrandTotalsProperty;
		public static readonly DependencyProperty ShowColumnHeadersProperty;
		public static readonly DependencyProperty ShowColumnTotalsProperty;
		public static readonly DependencyProperty ShowCustomTotalsForSingleValuesProperty;
		public static readonly DependencyProperty ShowDataHeadersProperty;
		public static readonly DependencyProperty ShowFilterHeadersProperty;
		public static readonly DependencyProperty ShowGrandTotalsForSingleValuesProperty;
		public static readonly DependencyProperty ShowRowGrandTotalsProperty;
		public static readonly DependencyProperty ShowRowHeadersProperty;
		public static readonly DependencyProperty ShowRowTotalsProperty;
		public static readonly DependencyProperty ShowTotalsForSingleValuesProperty;
		public static readonly DependencyProperty ColumnTotalsLocationProperty;
		public static readonly DependencyProperty RowTotalsLocationProperty;
		public static readonly DependencyProperty RowTreeWidthProperty;
		public static readonly DependencyProperty RowTreeHeightProperty;
		public static readonly DependencyProperty RowTreeMinWidthProperty;
		public static readonly DependencyProperty RowTreeMinHeightProperty;
		public static readonly DependencyProperty RowTreeOffsetProperty;
		public static readonly DependencyProperty ColumnFieldValuesAlignmentProperty;
		public static readonly DependencyProperty RowFieldValuesAlignmentProperty;
		public static readonly DependencyProperty RowTotalsHeightFactorProperty;
		public static readonly DependencyProperty GroupFieldsInFieldListProperty;
		public static readonly DependencyProperty FieldListIncludeVisibleFieldsProperty;
		public static readonly DependencyProperty ShowColumnGrandTotalHeaderProperty;
		public static readonly DependencyProperty ShowRowGrandTotalHeaderProperty;
		public static readonly DependencyProperty DataFieldAreaProperty;
		public static readonly DependencyProperty DataFieldAreaIndexProperty;
		public static readonly DependencyProperty DataFieldWidthProperty;
		public static readonly DependencyProperty DataFieldHeightProperty;
		public static readonly DependencyProperty DataFieldCaptionProperty;
		public static readonly DependencyProperty SummaryDataSourceFieldNamingProperty;
		public static readonly DependencyProperty ChartProvideDataByColumnsProperty;
		public static readonly DependencyProperty ChartFieldValuesProvideModeProperty;
		public static readonly DependencyProperty ChartDataProvideModeProperty;
		public static readonly DependencyProperty ChartDataProvidePriorityProperty;
		public static readonly DependencyProperty ChartSelectionOnlyProperty;
		public static readonly DependencyProperty ChartProvideColumnTotalsProperty;
		public static readonly DependencyProperty ChartProvideColumnGrandTotalsProperty;
		public static readonly DependencyProperty ChartProvideColumnCustomTotalsProperty;
		public static readonly DependencyProperty ChartProvideRowTotalsProperty;
		public static readonly DependencyProperty ChartProvideRowGrandTotalsProperty;
		public static readonly DependencyProperty ChartProvideRowCustomTotalsProperty;
		public static readonly DependencyProperty ChartProvideEmptyCellsProperty;
		public static readonly DependencyProperty ChartProvideColumnFieldValuesAsTypeProperty;
		public static readonly DependencyProperty ChartProvideRowFieldValuesAsTypeProperty;
		public static readonly DependencyProperty ChartProvideCellValuesAsTypeProperty;
		public static readonly DependencyProperty ChartUpdateDelayProperty;
		public static readonly DependencyProperty ChartMaxSeriesCountProperty;
		public static readonly DependencyProperty ChartMaxPointCountInSeriesProperty;
		public static readonly DependencyProperty ChartAutoTransposeProperty;
		public static readonly DependencyProperty CopyToClipboardWithFieldValuesProperty;
		public static readonly DependencyProperty ClipboardCopyMultiSelectionModeProperty;
		public static readonly DependencyProperty ClipboardCopyCollapsedValuesModeProperty;
		public static readonly DependencyProperty UseAsyncModeProperty;
		public static readonly DependencyProperty SortBySummaryDefaultOrderProperty;
		public static readonly DependencyProperty FixedRowHeadersProperty;
		public static readonly DependencyProperty LoadingPanelDelayProperty;
		public static readonly DependencyProperty SelectModeProperty;
		public static readonly DependencyProperty GroupFilterModeProperty;
		public static readonly DependencyProperty ShowOnlyAvailableFilterItemsProperty;
		public static readonly DependencyProperty IsFilterPopupMenuEnabledProperty;
		public static readonly DependencyProperty IsHeaderMenuEnabledProperty;
		public static readonly DependencyProperty IsHeaderAreaMenuEnabledProperty;
		public static readonly DependencyProperty IsFieldValueMenuEnabledProperty;
		public static readonly DependencyProperty IsFieldListMenuEnabledProperty;
		public static readonly DependencyProperty IsCellMenuEnabledProperty;
		public static readonly DependencyProperty FocusedCellProperty;
		public static readonly DependencyProperty SelectionProperty;
		public static readonly DependencyProperty BestFitModeProperty;
		public static readonly DependencyProperty BestFitAreaProperty;
		public static readonly DependencyProperty BestFitMaxRowCountProperty;
		public static readonly DependencyProperty FieldListFactoryProperty;
		public static readonly DependencyProperty FieldListTemplateProperty;
		public static readonly DependencyProperty ExcelFieldListTemplateProperty;
		public static readonly DependencyProperty IsFieldListVisibleProperty;
		public static readonly DependencyProperty FieldListStateProperty;
		public static readonly DependencyProperty ExcelFieldListStateProperty;
		public static readonly DependencyProperty IsPrefilterVisibleProperty;
		public static readonly DependencyProperty ShowBorderProperty;
		public static readonly DependencyProperty PrintFilterHeadersProperty;
		public static readonly DependencyProperty PrintColumnHeadersProperty;
		public static readonly DependencyProperty PrintRowHeadersProperty;
		public static readonly DependencyProperty PrintDataHeadersProperty;
		public static readonly DependencyProperty PrintHeadersOnEveryPageProperty;
		public static readonly DependencyProperty PrintUnusedFilterFieldsProperty;
		public static readonly DependencyProperty MergeColumnFieldValuesProperty;
		public static readonly DependencyProperty MergeRowFieldValuesProperty;
		public static readonly DependencyProperty PrintHorzLinesProperty;
		public static readonly DependencyProperty PrintVertLinesProperty;
		public static readonly DependencyProperty PrintLayoutModeProperty;
		public static readonly DependencyProperty PrintInsertPageBreaksProperty;
		public static readonly DependencyProperty PrintFieldHeaderTemplateProperty;
		public static readonly DependencyProperty PrintFieldHeaderTemplateSelectorProperty;
		public static readonly DependencyProperty PrintFieldValueTemplateProperty;
		public static readonly DependencyProperty PrintFieldValueTemplateSelectorProperty;
		public static readonly DependencyProperty PrintFieldCellTemplateProperty;
		public static readonly DependencyProperty PrintFieldCellTemplateSelectorProperty;
		public static readonly DependencyProperty PrintFieldCellKpiTemplateProperty;
		public static readonly DependencyProperty PrintFieldCellKpiTemplateSelectorProperty;
		public static readonly DependencyProperty FieldWidthProperty;
		public static readonly DependencyProperty FieldHeightProperty;
		public static readonly DependencyProperty AllowMRUFilterListProperty;
		public static readonly DependencyProperty MRUFilterListCountProperty;
		public static readonly DependencyProperty ScrollingModeProperty;
		static readonly DependencyPropertyKey LeftTopCoordPropertyKey;
		public static readonly DependencyProperty LeftTopCoordProperty;
		static readonly DependencyPropertyKey LeftTopPixelCoordPropertyKey;
		public static readonly DependencyProperty LeftTopPixelCoordProperty;
		public static readonly RoutedEvent GridLayoutEvent;
		public static readonly RoutedEvent BeforeLoadLayoutEvent;
		public static readonly RoutedEvent BeginRefreshEvent;
		public static readonly RoutedEvent EndRefreshEvent;
		public static readonly RoutedEvent DataSourceChangedEvent;
		public static readonly RoutedEvent ShownFieldListEvent;
		public static readonly RoutedEvent HiddenFieldListEvent;
		public static readonly RoutedEvent OlapDataLoadedEvent;
		public static readonly RoutedEvent LayoutUpgradeEvent;
		public static readonly RoutedEvent OlapQueryTimeoutEvent;
		public static readonly RoutedEvent OlapExceptionEvent;
		public static readonly RoutedEvent QueryExceptionEvent;
		public static readonly RoutedEvent PrefilterCriteriaChangedEvent;
		public static readonly RoutedEvent FieldValueCollapsedEvent;
		public static readonly RoutedEvent FieldValueExpandedEvent;
		public static readonly RoutedEvent FieldValueNotExpandedEvent;
		public static readonly RoutedEvent FieldValueCollapsingEvent;
		public static readonly RoutedEvent FieldValueExpandingEvent;
		public static readonly RoutedEvent CustomCellDisplayTextEvent;
		public static readonly RoutedEvent CustomCellValueEvent;
		public static readonly RoutedEvent CustomCellAppearanceEvent;
		public static readonly RoutedEvent CustomValueAppearanceEvent;
		public static readonly RoutedEvent GroupFilterChangedEvent;
		public static readonly RoutedEvent FieldAreaChangingEvent;
		public static readonly RoutedEvent FieldAreaChangedEvent;
		public static readonly RoutedEvent FieldExpandedInGroupChangedEvent;
		public static readonly RoutedEvent FieldFilterChangedEvent;
		public static readonly RoutedEvent FieldFilterChangingEvent;
		public static readonly RoutedEvent FieldUnboundExpressionChangedEvent;
		public static readonly RoutedEvent FieldPropertyChangedEvent;
		public static readonly RoutedEvent FieldSizeChangedEvent;
		public static readonly RoutedEvent FieldAreaIndexChangedEvent;
		public static readonly RoutedEvent FieldVisibleChangedEvent;
		public static readonly RoutedEvent CellClickEvent;
		public static readonly RoutedEvent CellDoubleClickEvent;
		public static readonly RoutedEvent CellSelectionChangedEvent;
		public static readonly RoutedEvent FocusedCellChangedEvent;
		public static readonly RoutedEvent UnboundExpressionEditorCreatedEvent;
		public static readonly RoutedEvent PrefilterEditorCreatedEvent;
		public static readonly RoutedEvent PrefilterEditorHidingEvent;
		public static readonly RoutedEvent CustomFilterPopupItemsEvent;
		public static readonly RoutedEvent CustomFieldValueCellsEvent;
		public static readonly RoutedEvent CustomPrefilterDisplayTextEvent;
		public static readonly RoutedEvent PropertyChangedEvent;
		public static readonly RoutedEvent BrushChangedEvent;
		internal static readonly RoutedEvent ShowHeadersPropertyChangedEvent;
		public static readonly RoutedEvent AsyncOperationStartingEvent;
		public static readonly RoutedEvent AsyncOperationCompletedEvent;
		public static readonly RoutedEvent PopupMenuShowingEvent;
		public static readonly DependencyProperty AllowConditionalFormattingMenuProperty;
		public static readonly DependencyProperty AllowConditionalFormattingManagerProperty;
		public static readonly DependencyProperty PredefinedFormatsProperty;
		public static readonly DependencyProperty PredefinedColorScaleFormatsProperty;
		public static readonly DependencyProperty PredefinedDataBarFormatsProperty;
		public static readonly DependencyProperty PredefinedIconSetFormatsProperty;
		public static readonly DependencyProperty FormatConditionDialogServiceTemplateProperty;
		public static readonly DependencyProperty ConditionalFormattingManagerServiceTemplateProperty;
		static PivotGridControl() {
			Type ownerType = typeof(PivotGridControl);
			DataPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("Data", typeof(PivotGridWpfData),
				ownerType, new FrameworkPropertyMetadata(null));
			DataProperty = DataPropertyKey.DependencyProperty;
			PivotGridProperty = DependencyPropertyManager.RegisterAttached("PivotGrid", typeof(PivotGridControl),
				ownerType, new FrameworkPropertyMetadata(null));
			DataSourceProperty = DependencyPropertyManager.Register("DataSource", typeof(object),
				ownerType, new FrameworkPropertyMetadata(null, (d, e) => OnPropertyChanged(OnDataSourcePropertyChanged, d, e)));
#if !SL
			OlapDataProviderProperty = DependencyPropertyManager.Register("OlapDataProvider", typeof(OlapDataProvider),
				ownerType, new FrameworkPropertyMetadata(OlapDataProvider.Default, (d, e) => ((PivotGridControl)d).OlapDataProviderPropertyChanged()));
#endif
			OlapConnectionStringProperty = DependencyPropertyManager.Register("OlapConnectionString", typeof(string),
				ownerType, new FrameworkPropertyMetadata(null, OnOlapConnectionStringPropertyChanged));
			OlapDefaultMemberFieldsProperty = DependencyPropertyManager.Register("OlapDefaultMemberFields", typeof(PivotDefaultMemberFields),
				ownerType, new FrameworkPropertyMetadata(PivotDefaultMemberFields.NonVisibleFilterFields, OnOlapDefaultMemberFieldsPropertyChanged));
			OlapFilterByUniqueNameProperty = DependencyPropertyManager.Register("OlapFilterByUniqueName", typeof(bool),
				ownerType, new FrameworkPropertyMetadata(false, (d, e) => ((PivotGridControl)d).OnOlapFilterByUniqueNamePropertyChanged()));
			PrefilterCriteriaProperty = DependencyPropertyManager.Register("PrefilterCriteria", typeof(CriteriaOperator),
				ownerType, new FrameworkPropertyMetadata(null, (d, e) => OnPropertyChanged(OnPrefilterCriteriaPropertyChanged, d, e)));
			PrefilterStringProperty = DependencyPropertyManager.Register("PrefilterString", typeof(string),
				ownerType, new FrameworkPropertyMetadata(null, (d, e) => OnPropertyChanged(OnPrefilterStringPropertyChanged, d, e)));
			ShowPrefilterPanelPropertyKey = DependencyPropertyManager.RegisterReadOnly("ShowPrefilterPanel", typeof(bool),
				ownerType, new FrameworkPropertyMetadata(false));
			ShowPrefilterPanelProperty = ShowPrefilterPanelPropertyKey.DependencyProperty;
			ShowPrefilterPanelModeProperty = DependencyPropertyManager.Register("ShowPrefilterPanelMode", typeof(ShowPrefilterPanelMode),
				ownerType, new FrameworkPropertyMetadata(ShowPrefilterPanelMode.Default, (d, e) => ((PivotGridControl)d).UpdatePrefilterPanel()));
			PrefilterPanelTextPropertyKey = DependencyPropertyManager.RegisterReadOnly("PrefilterPanelText", typeof(string), ownerType, new PropertyMetadata(string.Empty));
			PrefilterPanelTextProperty = PrefilterPanelTextPropertyKey.DependencyProperty;
			IsPrefilterEnabledProperty = DependencyPropertyManager.Register("IsPrefilterEnabled", typeof(bool),
				ownerType, new FrameworkPropertyMetadata(OnIsPrefiterEnabledPropertyChanged));
			CanEnablePrefilterPropertyKey = DependencyPropertyManager.RegisterReadOnly("CanEnablePrefilter", typeof(bool),
				ownerType, new PropertyMetadata(null));
			CanEnablePrefilterProperty = CanEnablePrefilterPropertyKey.DependencyProperty;
			IsMainWaitIndicatorVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsMainWaitIndicatorVisible", typeof(bool),
				ownerType, new PropertyMetadata(false));
			IsMainWaitIndicatorVisibleProperty = IsMainWaitIndicatorVisiblePropertyKey.DependencyProperty;
			IsFilterPopupWaitIndicatorVisiblePropertyKey = DependencyPropertyManager.RegisterReadOnly("IsFilterPopupWaitIndicatorVisible", typeof(bool),
				ownerType, new PropertyMetadata(false));
			IsFilterPopupWaitIndicatorVisibleProperty = IsFilterPopupWaitIndicatorVisiblePropertyKey.DependencyProperty;
			ActiveFilterInfoPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActiveFilterInfo", typeof(CriteriaOperatorInfo),
				ownerType, new PropertyMetadata(null));
			ActiveFilterInfoProperty = ActiveFilterInfoPropertyKey.DependencyProperty;
			ShowColumnsBorderPropertyKey = DependencyPropertyManager.RegisterReadOnly("ShowColumnsBorder", typeof(bool), ownerType, new PropertyMetadata(false));
			ShowColumnsBorderProperty = ShowColumnsBorderPropertyKey.DependencyProperty;
			UserActionPropertyKey = DependencyPropertyManager.RegisterReadOnly("UserAction", typeof(UserAction), ownerType, new PropertyMetadata(UserAction.None));
			UserActionProperty = UserActionPropertyKey.DependencyProperty;
			FieldHeaderDragIndicatorSizeProperty = DependencyPropertyManager.RegisterAttached("FieldHeaderDragIndicatorSize", typeof(double),
				ownerType, new FrameworkPropertyMetadata(0d, FrameworkPropertyMetadataOptions.None));
			FieldHeaderDragIndicatorTemplateProperty = DependencyPropertyManager.Register(FieldHeaderDragIndicatorTemplatePropertyName,
				typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			FieldHeaderTemplateProperty = DependencyPropertyManager.Register("FieldHeaderTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			FieldHeaderTemplateSelectorProperty = DependencyPropertyManager.Register("FieldHeaderTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			FieldHeaderTreeViewTemplateProperty = DependencyPropertyManager.Register("FieldHeaderTreeViewTemplate",
			 typeof(DataTemplate), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			FieldHeaderTreeViewTemplateSelectorProperty = DependencyPropertyManager.Register("FieldHeaderTreeViewTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			FieldCellKpiTemplateProperty = DependencyPropertyManager.Register("FieldCellKpiTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			FieldCellKpiTemplateSelectorProperty = DependencyPropertyManager.Register("FieldCellKpiTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			FieldHeaderListTemplateProperty = DependencyPropertyManager.Register("FieldHeaderListTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			FieldHeaderListTemplateSelectorProperty = DependencyPropertyManager.Register("FieldHeaderListTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			FieldCellTemplateProperty = DependencyPropertyManager.Register("FieldCellTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			FieldCellTemplateSelectorProperty = DependencyPropertyManager.Register("FieldCellTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			FieldValueTemplateProperty = DependencyPropertyManager.Register("FieldValueTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			FieldValueTemplateSelectorProperty = DependencyPropertyManager.Register("FieldValueTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			FocusedCellBorderTemplateProperty = DependencyPropertyManager.Register("FocusedCellBorderTemplate",
				typeof(ControlTemplate), ownerType);
			ResizingIndicatorTemplateProperty = DependencyPropertyManager.Register("ResizingIndicatorTemplate",
				typeof(ControlTemplate), ownerType);
			ScrollViewerTemplateProperty = DependencyPropertyManager.Register("ScrollViewerTemplate",
				typeof(ControlTemplate), ownerType);
			FieldHeaderContentStyleProperty = DependencyPropertyManager.Register("FieldHeaderContentStyle",
				typeof(Style), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			AllowCrossGroupVariationProperty = DependencyPropertyManager.Register("AllowCrossGroupVariation",
				typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => OnPropertyChanged(OnAllowCrossGroupVariationPropertyChanged, d, e)));
			GroupDataCaseSensitiveProperty = DependencyPropertyManager.Register("GroupDataCaseSensitive",
				typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => OnPropertyChanged(OnGroupDataCaseSensitivePropertyChanged, d, e)));
			AutoExpandGroupsProperty = DependencyPropertyManager.Register("AutoExpandGroups",
				typeof(bool?), ownerType, new PropertyMetadata(null, (d, e) => OnPropertyChanged(OnAutoExpandGroupsPropertyChanged, d, e)));
			DrillDownMaxRowCountProperty = DependencyPropertyManager.Register("DrillDownMaxRowCount",
				typeof(int), ownerType, new PropertyMetadata(-1, OnDrillDownMaxRowCountPropertyChanged));
			DataFieldUnboundExpressionModeProperty = DependencyPropertyManager.Register("DataFieldUnboundExpressionMode",
				typeof(UnboundExpressionMode), ownerType,
				new PropertyMetadata(UnboundExpressionMode.Default, (d, e) => OnPropertyChanged(OnDataFieldUnboundExpressionPropertyChanged, d, e)));
			FilterByVisibleFieldsOnlyProperty = DependencyPropertyManager.Register("FilterByVisibleFieldsOnly", typeof(bool),
				ownerType, new PropertyMetadata(false, (d, e) => OnPropertyChanged(OnFilterByVisibleFieldsOnlyPropertyChanged, d, e)));
			AllowCustomizationFormProperty = DependencyPropertyManager.Register("AllowCustomizationForm",
				typeof(bool), ownerType, new PropertyMetadata(true));
			AllowDragProperty = DependencyPropertyManager.Register("AllowDrag",
				typeof(bool), ownerType, new PropertyMetadata(true,
					(d, e) => ((PivotGridControl)d).OnAllowDragChanged((bool)e.NewValue)));
			AllowDragInCustomizationFormProperty = DependencyPropertyManager.Register("AllowDragInCustomizationForm",
				typeof(bool), ownerType, new PropertyMetadata(true,
					(d, e) => ((PivotGridControl)d).OnAllowDragInCustomizationFormChanged((bool)e.NewValue)));
			AllowExpandProperty = DependencyPropertyManager.Register("AllowExpand",
				typeof(bool), ownerType, new PropertyMetadata(true, OnAllowExpandPropertyChanged));
			AllowFilterProperty = DependencyPropertyManager.Register("AllowFilter",
				typeof(bool), ownerType, new PropertyMetadata(true, OnAllowFilterPropertyChanged));
			AllowResizingProperty = DependencyPropertyManager.Register("AllowResizing",
				typeof(bool), ownerType, new PropertyMetadata(true));
			AllowSortProperty = DependencyPropertyManager.Register("AllowSort",
				typeof(bool), ownerType, new PropertyMetadata(true, OnAllowSortPropertyChanged));
			AllowSortBySummaryProperty = DependencyPropertyManager.Register("AllowSortBySummary",
				typeof(bool), ownerType, new PropertyMetadata(true, OnAllowSortBySummaryPropertyChanged));
			AllowHideFieldsProperty = DependencyPropertyManager.Register("AllowHideFields",
				typeof(AllowHideFieldsType), ownerType, new PropertyMetadata(AllowHideFieldsType.WhenFieldListVisible));
			AllowPrefilterProperty = DependencyPropertyManager.Register("AllowPrefilter",
				typeof(bool), ownerType, new PropertyMetadata(true));
			FieldListStyleProperty = DependencyPropertyManager.Register("FieldListStyle",
				typeof(FieldListStyle), ownerType, new PropertyMetadata(FieldListStyle.Simple, OnFieldListStylePropertyChanged));
			FieldListLayoutProperty = DependencyPropertyManager.Register("FieldListLayout",
				typeof(FieldListLayout), ownerType, new PropertyMetadata(FieldListLayout.StackedDefault, OnFieldListLayoutPropertyChanged, CoerceFieldListLayoutProperty));
			FieldListAllowedLayoutsProperty = DependencyPropertyManager.Register("FieldListAllowedLayouts",
				typeof(FieldListAllowedLayouts), ownerType, new PropertyMetadata(FieldListAllowedLayouts.All, OnFieldListAllowedLayoutsPropertyChanged));
			FieldListSplitterYProperty = DependencyPropertyManager.Register("FieldListSplitterY", typeof(double), ownerType, new PropertyMetadata(150d));
			FieldListSplitterXProperty = DependencyPropertyManager.Register("FieldListSplitterX", typeof(double), ownerType, new PropertyMetadata(104d));
			DeferredUpdatesProperty = DependencyPropertyManager.Register("DeferredUpdates",
				typeof(bool), ownerType, new PropertyMetadata(OnDeferredUpdatesPropertyChanged));
			AllowExpandOnDoubleClickProperty = DependencyPropertyManager.Register("AllowExpandOnDoubleClick",
				typeof(bool), ownerType, new PropertyMetadata(true));
			AllowSortInFieldListProperty = DependencyPropertyManager.Register("AllowSortInFieldList",
				typeof(bool), ownerType, new PropertyMetadata(true));
			AllowFilterInFieldListProperty = DependencyPropertyManager.Register("AllowFilterInFieldList",
				typeof(bool), ownerType, new PropertyMetadata(true));
			CellForegroundProperty = DependencyPropertyManager.Register("CellForeground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotCellBrushesChanged));
			CellTotalForegroundProperty = DependencyPropertyManager.Register("CellTotalForeground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotCellBrushesChanged));
			CellTotalSelectedForegroundProperty = DependencyPropertyManager.Register("CellTotalSelectedForeground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotCellBrushesChanged));
			CellSelectedForegroundProperty = DependencyPropertyManager.Register("CellSelectedForeground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotCellBrushesChanged));
			CellBackgroundProperty = DependencyPropertyManager.Register("CellBackground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotCellBrushesChanged));
			CellTotalBackgroundProperty = DependencyPropertyManager.Register("CellTotalBackground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotCellBrushesChanged));
			CellTotalSelectedBackgroundProperty = DependencyPropertyManager.Register("CellTotalSelectedBackground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotCellBrushesChanged));
			CellSelectedBackgroundProperty = DependencyPropertyManager.Register("CellSelectedBackground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotCellBrushesChanged));
			ValueForegroundProperty = DependencyPropertyManager.Register("ValueForeground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotValueBrushesChanged));
			ValueTotalForegroundProperty = DependencyPropertyManager.Register("ValueTotalForeground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotValueBrushesChanged));
			ValueTotalSelectedForegroundProperty = DependencyPropertyManager.Register("ValueTotalSelectedForeground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotValueBrushesChanged));
			ValueSelectedForegroundProperty = DependencyPropertyManager.Register("ValueSelectedForeground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotValueBrushesChanged));
			ValueBackgroundProperty = DependencyPropertyManager.Register("ValueBackground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotValueBrushesChanged));
			ValueTotalBackgroundProperty = DependencyPropertyManager.Register("ValueTotalBackground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotValueBrushesChanged));
			ValueTotalSelectedBackgroundProperty = DependencyPropertyManager.Register("ValueTotalSelectedBackground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotValueBrushesChanged));
			ValueSelectedBackgroundProperty = DependencyPropertyManager.Register("ValueSelectedBackground", typeof(Brush), ownerType, new PropertyMetadata(OnPivotValueBrushesChanged));
			PrintCellForegroundProperty = DependencyPropertyManager.Register("PrintCellForeground", typeof(Brush), ownerType, new PropertyMetadata());
			PrintCellTotalForegroundProperty = DependencyPropertyManager.Register("PrintCellTotalForeground", typeof(Brush), ownerType, new PropertyMetadata());
			PrintCellBackgroundProperty = DependencyPropertyManager.Register("PrintCellBackground", typeof(Brush), ownerType, new PropertyMetadata());
			PrintCellTotalBackgroundProperty = DependencyPropertyManager.Register("PrintCellTotalBackground", typeof(Brush), ownerType, new PropertyMetadata());
			PrintValueForegroundProperty = DependencyPropertyManager.Register("PrintValueForeground", typeof(Brush), ownerType, new PropertyMetadata());
			PrintValueTotalForegroundProperty = DependencyPropertyManager.Register("PrintValueTotalForeground", typeof(Brush), ownerType, new PropertyMetadata());
			PrintValueBackgroundProperty = DependencyPropertyManager.Register("PrintValueBackground", typeof(Brush), ownerType, new PropertyMetadata());
			PrintValueTotalBackgroundProperty = DependencyPropertyManager.Register("PrintValueTotalBackground", typeof(Brush), ownerType, new PropertyMetadata());
			DrawFocusedCellRectProperty = DependencyPropertyManager.Register("DrawFocusedCellRect",
				typeof(bool), ownerType, new PropertyMetadata(true, OnDrawFocusedCellRectPropertyChanged));
			ShowColumnGrandTotalsProperty = DependencyPropertyManager.Register("ShowColumnGrandTotals",
				typeof(bool), ownerType, new PropertyMetadata(true, OnShowColumnGrandTotalsPropertyChanged));
			ShowColumnTotalsProperty = DependencyPropertyManager.Register("ShowColumnTotals",
				typeof(bool), ownerType, new PropertyMetadata(true, OnShowColumnTotalsPropertyChanged));
			ShowCustomTotalsForSingleValuesProperty = DependencyPropertyManager.Register("ShowCustomTotalsForSingleValues",
				typeof(bool), ownerType, new PropertyMetadata(false, OnShowCustomTotalsForSingleValuesPropertyChanged));
			ShowDataHeadersProperty = DependencyPropertyManager.Register("ShowDataHeaders",
				typeof(bool), ownerType, new PropertyMetadata(true,
					(d, e) => ((PivotGridControl)d).OnShowDataHeadersChanged((bool)e.NewValue)));
			ShowFilterHeadersProperty = DependencyPropertyManager.Register("ShowFilterHeaders",
				typeof(bool), ownerType, new PropertyMetadata(true,
					(d, e) => ((PivotGridControl)d).OnShowFilterHeadersChanged((bool)e.NewValue)));
			ShowColumnHeadersProperty = DependencyPropertyManager.Register("ShowColumnHeaders",
				typeof(bool), ownerType, new PropertyMetadata(true,
					(d, e) => ((PivotGridControl)d).OnShowColumnHeadersChanged((bool)e.NewValue)));
			ShowRowHeadersProperty = DependencyPropertyManager.Register("ShowRowHeaders",
				typeof(bool), ownerType, new PropertyMetadata(true,
					(d, e) => ((PivotGridControl)d).OnShowRowHeadersChanged((bool)e.NewValue)));
			ShowGrandTotalsForSingleValuesProperty = DependencyPropertyManager.Register("ShowGrandTotalsForSingleValues",
				typeof(bool), ownerType, new PropertyMetadata(false, OnShowGrandTotalsForSingleValuesPropertyChanged));
			ShowRowGrandTotalsProperty = DependencyPropertyManager.Register("ShowRowGrandTotals",
				typeof(bool), ownerType, new PropertyMetadata(true, OnShowRowGrandTotalsPropertyChanged));
			ShowRowTotalsProperty = DependencyPropertyManager.Register("ShowRowTotals",
				typeof(bool), ownerType, new PropertyMetadata(true, OnShowRowTotalsPropertyChanged, OnShowRowTotalsPropertyChanging));
			ShowTotalsForSingleValuesProperty = DependencyPropertyManager.Register("ShowTotalsForSingleValues",
				typeof(bool), ownerType, new PropertyMetadata(true, OnShowTotalsForSingleValuesChanged));
			ColumnTotalsLocationProperty = DependencyPropertyManager.Register("ColumnTotalsLocation",
				typeof(FieldColumnTotalsLocation), ownerType, new PropertyMetadata(FieldColumnTotalsLocation.Far, OnColumnTotalsLocationPropertyChanged));
			RowTotalsLocationProperty = DependencyPropertyManager.Register("RowTotalsLocation",
				typeof(FieldRowTotalsLocation), ownerType, new PropertyMetadata(FieldRowTotalsLocation.Tree, OnRowTotalsLocationPropertyChanged, OnRowTotalsLocationPropertyChanging));
			RowTreeWidthProperty = DependencyPropertyManager.Register("RowTreeWidth",
				typeof(int), ownerType, new PropertyMetadata(Int32.MaxValue, OnRowTreeWidthPropertyChanged, (d, e) => ((PivotGridControl)d).OnRowTreeWidthChanging(e)));
			RowTreeHeightProperty = DependencyPropertyManager.Register("RowTreeHeight",
				typeof(int), ownerType, new PropertyMetadata(Int32.MaxValue, OnRowTreeHeightPropertyChanged, (d, e) => ((PivotGridControl)d).OnRowTreeHeightChanging(e)));
			RowTreeMinHeightProperty = DependencyPropertyManager.Register("RowTreeMinHeight",
				typeof(double), ownerType, new PropertyMetadata(Convert.ToDouble(PivotGridField.DefaultMinHeight), (d, e) => ((PivotGridControl)d).OnRowTreeMinHeightChanged(), PivotGridField.OnMinHeightChanging));
			RowTreeMinWidthProperty = DependencyPropertyManager.Register("RowTreeMinWidth",
				typeof(double), ownerType, new PropertyMetadata(Convert.ToDouble(PivotGridField.DefaultMinWidth), (d, e) => ((PivotGridControl)d).OnRowTreeMinWidthChanged(), PivotGridField.OnMinWidthChanging));
			RowTreeOffsetProperty = DependencyPropertyManager.Register("RowTreeOffset",
				typeof(int), ownerType, new PropertyMetadata(DefaultRowTreeOffset, OnRowTreeOffsetPropertyChanged));
			ColumnFieldValuesAlignmentProperty = DependencyPropertyManager.Register("ColumnFieldValuesAlignment",
				typeof(VerticalAlignment), ownerType, new PropertyMetadata(VerticalAlignment.Top));
			RowFieldValuesAlignmentProperty = DependencyPropertyManager.Register("RowFieldValuesAlignment",
				typeof(VerticalAlignment), ownerType, new PropertyMetadata(VerticalAlignment.Center));
			RowTotalsHeightFactorProperty = DependencyPropertyManager.Register("RowTotalsHeightFactor",
				typeof(double), ownerType, new PropertyMetadata(RowTotalsHeightFactorPropertyDefaultValue));
			GroupFieldsInFieldListProperty = DependencyPropertyManager.Register("GroupFieldsInFieldList", typeof(bool), ownerType, new PropertyMetadata(true, OnGroupFieldsInFieldListPropertyChanged, CoerceGroupFieldsInFieldListProperty));
			FieldListIncludeVisibleFieldsProperty = DependencyPropertyManager.Register("FieldListIncludeVisibleFields", typeof(bool), ownerType, new PropertyMetadata(false));
			ShowColumnGrandTotalHeaderProperty = DependencyPropertyManager.Register("ShowColumnGrandTotalHeader", typeof(bool), ownerType, new PropertyMetadata(true, ShowColumnGrandTotalHeaderPropertyChanged));
			ShowRowGrandTotalHeaderProperty = DependencyPropertyManager.Register("ShowRowGrandTotalHeader", typeof(bool), ownerType, new PropertyMetadata(true, ShowRowGrandTotalHeaderPropertyChanged));
			DataFieldAreaProperty = DependencyPropertyManager.Register("DataFieldArea",
				typeof(DataFieldArea), ownerType, new PropertyMetadata(DataFieldArea.None, (d, e) => OnPropertyChanged(OnDataFieldAreaPropertyChanged, d, e)));
			DataFieldAreaIndexProperty = DependencyPropertyManager.Register("DataFieldAreaIndex",
				typeof(int), ownerType, new PropertyMetadata(-1, (d, e) => OnPropertyChanged(OnDataFieldAreaIndexPropertyChanged, d, e)));
			DataFieldWidthProperty = DependencyPropertyManager.Register("DataFieldWidth",
				typeof(double), ownerType, new PropertyMetadata(double.NaN, OnDataFieldWidthPropertyChanged, (d, e) => ((PivotGridControl)d).OnDataFieldWidthChanging((double)e)));
			DataFieldHeightProperty = DependencyPropertyManager.Register("DataFieldHeight",
				typeof(double), ownerType, new PropertyMetadata(double.NaN, OnDataFieldHeightPropertyChanged, (d, e) => ((PivotGridControl)d).OnDataFieldHeightChanging((double)e)));
			DataFieldCaptionProperty = DependencyPropertyManager.Register("DataFieldCaption",
				typeof(string), ownerType, new PropertyMetadata(string.Empty, OnDataFieldCaptionPropertyChanged));
			SummaryDataSourceFieldNamingProperty = DependencyPropertyManager.Register("SummaryDataSourceFieldNaming",
				typeof(SummaryDataSourceFieldNaming), ownerType,
				new PropertyMetadata(SummaryDataSourceFieldNaming.FieldName, OnSummaryDataSourceFieldNamingPropertyChanged));
			ChartProvideDataByColumnsProperty = DependencyPropertyManager.Register("ChartProvideDataByColumns",
				typeof(bool), ownerType, new PropertyMetadata(true, OnChartProvideDataByColumnsChanged));
			ChartFieldValuesProvideModeProperty = DependencyPropertyManager.Register("ChartFieldValuesProvideMode",
				typeof(PivotChartFieldValuesProvideMode), ownerType,
				new PropertyMetadata(PivotChartFieldValuesProvideMode.Default, OnChartFieldValuesProvideModeChanged));
			ChartDataProvideModeProperty = DependencyPropertyManager.Register("ChartDataProvideMode",
				typeof(PivotChartDataProvideMode), ownerType,
				new PropertyMetadata(PivotChartDataProvideMode.ProvideLastLevelData, OnChartDataProvideModeChanged));
			ChartDataProvidePriorityProperty = DependencyPropertyManager.Register("ChartDataProvidePriority",
				typeof(PivotChartDataProvidePriority), ownerType,
				new PropertyMetadata(PivotChartDataProvidePriority.Rows, OnChartDataProvidePriorityChanged));
			ChartSelectionOnlyProperty = DependencyPropertyManager.Register("ChartSelectionOnly",
				typeof(bool), ownerType, new PropertyMetadata(true, OnChartSelectionOnlyChanged));
			ChartProvideColumnTotalsProperty = DependencyPropertyManager.Register("ChartProvideColumnTotals",
				typeof(bool), ownerType, new PropertyMetadata(false, OnChartProvideColumnTotalsChanged));
			ChartProvideColumnGrandTotalsProperty = DependencyPropertyManager.Register("ChartProvideColumnGrandTotals",
				typeof(bool), ownerType, new PropertyMetadata(false, OnChartProvideColumnGrandTotalsChanged));
			ChartProvideColumnCustomTotalsProperty = DependencyPropertyManager.Register("ChartProvideColumnCustomTotals",
				typeof(bool), ownerType, new PropertyMetadata(false, OnChartProvideColumnCustomTotalsChanged));
			ChartProvideRowTotalsProperty = DependencyPropertyManager.Register("ChartProvideRowTotals",
				typeof(bool), ownerType, new PropertyMetadata(false, OnChartProvideRowTotalsChanged));
			ChartProvideRowGrandTotalsProperty = DependencyPropertyManager.Register("ChartProvideRowGrandTotals",
				typeof(bool), ownerType, new PropertyMetadata(false, OnChartProvideRowGrandTotalsChanged));
			ChartProvideRowCustomTotalsProperty = DependencyPropertyManager.Register("ChartProvideRowCustomTotals",
				typeof(bool), ownerType, new PropertyMetadata(false, OnChartProvideRowCustomTotalsChanged));
			ChartAutoTransposeProperty = DependencyPropertyManager.Register("ChartAutoTranspose",
				typeof(bool), ownerType, new PropertyMetadata(false, OnChartAutoTransposeChanged));
			ChartProvideEmptyCellsProperty = DependencyPropertyManager.Register("ChartProvideEmptyCells",
				typeof(bool), ownerType, new PropertyMetadata(true, OnChartProvideEmptyCellsChanged));
			ChartProvideColumnFieldValuesAsTypeProperty = DependencyPropertyManager.Register("ChartProvideColumnFieldValuesAsType",
				typeof(Type), ownerType, new PropertyMetadata(null, OnChartProvideColumnFieldValuesAsTypeChanged));
			ChartProvideRowFieldValuesAsTypeProperty = DependencyPropertyManager.Register("ChartProvideRowFieldValuesAsType",
				typeof(Type), ownerType, new PropertyMetadata(null, OnChartProvideRowFieldValuesAsTypeChanged));
			ChartProvideCellValuesAsTypeProperty = DependencyPropertyManager.Register("ChartProvideCellValuesAsType",
				typeof(Type), ownerType, new PropertyMetadata(null, OnChartProvideCellValuesAsTypeChanged));
			ChartUpdateDelayProperty = DependencyPropertyManager.Register("ChartUpdateDelay",
				typeof(int), ownerType, new PropertyMetadata(500, OnChartUpdateDelayChanged, CoerceChartUpdateDelay));
			ChartMaxSeriesCountProperty = DependencyPropertyManager.Register("ChartMaxSeriesCount",
				typeof(int), ownerType, new PropertyMetadata(10, OnChartMaxSeriesCountChanged));
			ChartMaxPointCountInSeriesProperty = DependencyPropertyManager.Register("ChartMaxPointCountInSeries",
				typeof(int), ownerType, new PropertyMetadata(100, OnChartMaxPointCountInSeriesChanged));
			CopyToClipboardWithFieldValuesProperty = DependencyPropertyManager.Register("CopyToClipboardWithFieldValues", typeof(bool),
				ownerType, new PropertyMetadata(false, OnCopyToClipboardWithFieldValuesPropertyChanged));
			ClipboardCopyMultiSelectionModeProperty = DependencyPropertyManager.Register("ClipboardCopyMultiSelectionMode", typeof(CopyMultiSelectionMode),
				ownerType, new PropertyMetadata(CopyMultiSelectionMode.DiscardIntermediateColumnsAndRows, OnClipboardCopyMultiSelectionModePropertyChanged));
			ClipboardCopyCollapsedValuesModeProperty = DependencyPropertyManager.Register("ClipboardCopyCollapsedValuesMode", typeof(CopyCollapsedValuesMode),
				ownerType, new PropertyMetadata(CopyCollapsedValuesMode.DuplicateCollapsedValues, OnClipboardCopyCollapsedValuesModePropertyChanged));
			UseAsyncModeProperty = DependencyPropertyManager.Register("UseAsyncMode", typeof(bool),
#if !SL
 ownerType, new PropertyMetadata(false, OnUseAsyncModePropertyChanged));
#else
				ownerType, new PropertyMetadata(true, OnUseAsyncModePropertyChanged));
#endif
			LoadingPanelDelayProperty = DependencyPropertyManager.Register("LoadingPanelDelay", typeof(int),
				ownerType, new PropertyMetadata(DevExpress.XtraPivotGrid.PivotGridOptionsBehaviorBase.DefaultLoadingPanelDelay, OnLoadingPanelDelayPropertyChaned));
			SortBySummaryDefaultOrderProperty = DependencyPropertyManager.Register("SortBySummaryDefaultOrder", typeof(FieldSortBySummaryOrder),
				ownerType, new PropertyMetadata(FieldSortBySummaryOrder.Default));
			FixedRowHeadersProperty = DependencyPropertyManager.Register("FixedRowHeaders", typeof(bool),
				ownerType, new PropertyMetadata(true, OnFixedRowHeadersPropertyChanged));
			IsHeaderMenuEnabledProperty = DependencyPropertyManager.Register("IsHeaderMenuEnabled", typeof(bool),
				ownerType, new PropertyMetadata(true));
			IsHeaderAreaMenuEnabledProperty = DependencyPropertyManager.Register("IsHeaderAreaMenuEnabled", typeof(bool),
				ownerType, new PropertyMetadata(true));
			IsFieldValueMenuEnabledProperty = DependencyPropertyManager.Register("IsFieldValueMenuEnabled", typeof(bool),
				ownerType, new PropertyMetadata(true));
			IsFieldListMenuEnabledProperty = DependencyPropertyManager.Register("IsFieldListMenuEnabled", typeof(bool),
				ownerType, new PropertyMetadata(true));
			IsCellMenuEnabledProperty = DependencyPropertyManager.Register("IsCellMenuEnabled", typeof(bool),
				ownerType, new PropertyMetadata(true));
			SelectModeProperty = DependencyPropertyManager.Register("SelectMode", typeof(SelectMode),
				ownerType, new PropertyMetadata(SelectMode.MultiSelection, OnSelectModePropertyChanged));
			FocusedCellProperty = DependencyPropertyManager.Register("FocusedCell", typeof(System.Drawing.Point),
				ownerType, new PropertyMetadata(System.Drawing.Point.Empty,
					OnFocusedCellPropertyChanged, OnFocusedCellPropertyChanging));
			SelectionProperty = DependencyPropertyManager.Register("Selection", typeof(System.Drawing.Rectangle),
				ownerType, new PropertyMetadata(System.Drawing.Rectangle.Empty,
					OnSelectionPropertyChanged));
			GroupFilterModeProperty = DependencyPropertyManager.Register("GroupFilterMode", typeof(GroupFilterMode),
				ownerType, new PropertyMetadata(GroupFilterMode.Tree, (d, e) => OnPropertyChanged((d1, e1) => ((PivotGridControl)d1).OnGroupFilterModeChanged((GroupFilterMode)e1.NewValue), d, e)));
			ShowOnlyAvailableFilterItemsProperty = DependencyPropertyManager.Register("ShowOnlyAvailableFilterItems", typeof(bool),
				ownerType, new PropertyMetadata(false));
			IsFilterPopupMenuEnabledProperty = DependencyPropertyManager.Register("IsFilterPopupMenuEnabled", typeof(bool),
				ownerType, new PropertyMetadata(true));
			BestFitModeProperty = DependencyPropertyManager.Register("BestFitMode", typeof(BestFitMode),
				ownerType, new PropertyMetadata(BestFitMode.Default));
			BestFitAreaProperty = DependencyPropertyManager.Register("BestFitArea", typeof(FieldBestFitArea),
				ownerType, new PropertyMetadata(FieldBestFitArea.All));
			BestFitMaxRowCountProperty = DependencyPropertyManager.Register("BestFitMaxRowCount", typeof(int),
				ownerType, new PropertyMetadata(BestWidthCalculator.DefaultBestFitMaxRowCount));
			FieldListFactoryProperty = DependencyPropertyManager.Register(
				"FieldListFactory", typeof(IColumnChooserFactory), ownerType,
				new FrameworkPropertyMetadata(DefaultFieldListFactory.Instance,
					(d, e) => ((PivotGridControl)d).OnFieldListFactoryChanged(),
					(d, baseValue) => ((PivotGridControl)d).CoerceFieldListFactory((IColumnChooserFactory)baseValue)));
			FieldListTemplateProperty = DependencyPropertyManager.Register(
				"FieldListTemplate", typeof(ControlTemplate), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((PivotGridControl)d).OnFieldListTemplateChanged((ControlTemplate)e.OldValue)));
			ExcelFieldListTemplateProperty = DependencyPropertyManager.Register(
					"ExcelFieldListTemplate", typeof(ControlTemplate), ownerType,
					new FrameworkPropertyMetadata(null, (d, e) => ((PivotGridControl)d).OnExcelFieldListTemplateChanged((ControlTemplate)e.OldValue)));
			IsFieldListVisibleProperty = DependencyPropertyManager.Register(
				"IsFieldListVisible", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((PivotGridControl)d).OnIsFieldListVisibleChanged()));
			FieldListStateProperty = DependencyPropertyManager.Register(
				"FieldListState", typeof(IColumnChooserState), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((PivotGridControl)d).OnFieldListStateChanged()));
			ExcelFieldListStateProperty = DependencyPropertyManager.Register(
				"ExcelFieldListState", typeof(IColumnChooserState), ownerType,
				new FrameworkPropertyMetadata(null, (d, e) => ((PivotGridControl)d).OnExcelFieldListStateChanged()));
			IsPrefilterVisibleProperty = DependencyPropertyManager.Register(
				"IsPrefilterVisible", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false, (d, e) => ((PivotGridControl)d).OnIsPrefilterVisibleChanged()));
			ShowBorderProperty = DependencyPropertyManager.RegisterAttached("ShowBorder", typeof(bool),
				ownerType, new FrameworkPropertyMetadata(true));
			PrintFilterHeadersProperty = DependencyPropertyManager.Register("PrintFilterHeaders", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			PrintColumnHeadersProperty = DependencyPropertyManager.Register("PrintColumnHeaders", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			PrintRowHeadersProperty = DependencyPropertyManager.Register("PrintRowHeaders", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			PrintDataHeadersProperty = DependencyPropertyManager.Register("PrintDataHeaders", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			PrintHeadersOnEveryPageProperty = DependencyPropertyManager.Register("PrintHeadersOnEveryPage", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(false));
			PrintUnusedFilterFieldsProperty = DependencyPropertyManager.Register("PrintUnusedFilterFields", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			MergeColumnFieldValuesProperty = DependencyPropertyManager.Register("MergeColumnFieldValues", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			MergeRowFieldValuesProperty = DependencyPropertyManager.Register("MergeRowFieldValues", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			PrintHorzLinesProperty = DependencyPropertyManager.Register("PrintHorzLines", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			PrintVertLinesProperty = DependencyPropertyManager.Register("PrintVertLines", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			PrintLayoutModeProperty = DependencyPropertyManager.Register("PrintLayoutMode", typeof(PrintLayoutMode), ownerType,
				new FrameworkPropertyMetadata(PrintLayoutMode.Auto));
			PrintInsertPageBreaksProperty = DependencyPropertyManager.Register("PrintInsertPageBreaks", typeof(bool), ownerType,
				new FrameworkPropertyMetadata(true));
			PrintFieldHeaderTemplateProperty = DependencyPropertyManager.Register("PrintFieldHeaderTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			PrintFieldHeaderTemplateSelectorProperty = DependencyPropertyManager.Register("PrintFieldHeaderTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			PrintFieldValueTemplateProperty = DependencyPropertyManager.Register("PrintFieldValueTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			PrintFieldValueTemplateSelectorProperty = DependencyPropertyManager.Register("PrintFieldValueTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			PrintFieldCellTemplateProperty = DependencyPropertyManager.Register("PrintFieldCellTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			PrintFieldCellTemplateSelectorProperty = DependencyPropertyManager.Register("PrintFieldCellTemplateSelector",
			   typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			PrintFieldCellKpiTemplateProperty = DependencyPropertyManager.Register("PrintFieldCellKpiTemplate",
			   typeof(DataTemplate), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			PrintFieldCellKpiTemplateSelectorProperty = DependencyPropertyManager.Register("PrintFieldCellKpiTemplateSelector",
			   typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnFieldAppearancePropertyChanged));
			AllowMRUFilterListProperty = DependencyPropertyManager.Register("AllowMRUFilterList", typeof(bool),
				ownerType, new PropertyMetadata(true));
			MRUFilterListCountProperty = DependencyPropertyManager.Register("MRUFilterListCount", typeof(Int32),
				ownerType, new PropertyMetadata(10,
					new PropertyChangedCallback((d, e) => ((PivotGridControl)d).OnMRUFilterListCountChanged())));
			FieldWidthProperty = DependencyPropertyManager.Register("FieldWidth", typeof(double), ownerType, new PropertyMetadata(Convert.ToDouble(PivotGridField.DefaultWidth), (d, e) => ((PivotGridControl)d).SetFieldWidth()));
			FieldHeightProperty = DependencyPropertyManager.Register("FieldHeight", typeof(double), ownerType, new PropertyMetadata(Convert.ToDouble(PivotGridField.DefaultHeight), (d, e) => ((PivotGridControl)d).SetFieldHeight()));
			ScrollingModeProperty = DependencyPropertyManager.Register("ScrollingMode", typeof(ScrollingMode), ownerType, new PropertyMetadata(ScrollingMode.Pixel, OnScrollingModePropertyChanged));
			LeftTopCoordPropertyKey = DependencyPropertyManager.RegisterReadOnly("LeftTopCoord", typeof(System.Drawing.Point), ownerType, new PropertyMetadata(System.Drawing.Point.Empty));
			LeftTopCoordProperty = LeftTopCoordPropertyKey.DependencyProperty;
			LeftTopPixelCoordPropertyKey = DependencyPropertyManager.RegisterReadOnly("LeftTopPixelCoord", typeof(Point), ownerType, new PropertyMetadata(new Point(0, 0)));
			LeftTopPixelCoordProperty = LeftTopPixelCoordPropertyKey.DependencyProperty;
			AllowConditionalFormattingMenuProperty = DependencyProperty.Register("AllowConditionalFormattingMenu", typeof(bool), ownerType, new PropertyMetadata(null));
			AllowConditionalFormattingManagerProperty = DependencyProperty.Register("AllowConditionalFormattingManager", typeof(bool), ownerType, new PropertyMetadata(true));
			PredefinedFormatsProperty = DependencyProperty.Register("PredefinedFormats", typeof(FormatInfoCollection), ownerType, new PropertyMetadata(null));
			PredefinedColorScaleFormatsProperty = DependencyProperty.Register("PredefinedColorScaleFormats", typeof(FormatInfoCollection), ownerType, new PropertyMetadata(null));
			FormatConditionDialogServiceTemplateProperty = DependencyProperty.Register("FormatConditionDialogServiceTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null));
			ConditionalFormattingManagerServiceTemplateProperty = DependencyProperty.Register("ConditionalFormattingManagerServiceTemplate", typeof(DataTemplate), ownerType, new PropertyMetadata(null));
			PredefinedIconSetFormatsProperty = DependencyProperty.Register("PredefinedIconSetFormats", typeof(FormatInfoCollection), ownerType, new PropertyMetadata(null));
			PredefinedDataBarFormatsProperty = DependencyProperty.Register("PredefinedDataBarFormats", typeof(FormatInfoCollection), ownerType, new PropertyMetadata(null));
			GridLayoutEvent = EventManager.RegisterRoutedEvent("GridLayout", RoutingStrategy.Direct,
				typeof(RoutedEventHandler), ownerType);
			BeforeLoadLayoutEvent = EventManager.RegisterRoutedEvent("BeforeLoadLayout", RoutingStrategy.Direct,
				typeof(PivotLayoutAllowEventHandler), ownerType);
			BeginRefreshEvent = EventManager.RegisterRoutedEvent("BeginRefresh", RoutingStrategy.Direct,
				typeof(RoutedEventHandler), ownerType);
			EndRefreshEvent = EventManager.RegisterRoutedEvent("EndRefresh", RoutingStrategy.Direct,
				typeof(RoutedEventHandler), ownerType);
			DataSourceChangedEvent = EventManager.RegisterRoutedEvent("DataSourceChanged", RoutingStrategy.Direct,
				typeof(RoutedEventHandler), ownerType);
			ShownFieldListEvent = EventManager.RegisterRoutedEvent("ShownFieldList", RoutingStrategy.Direct,
				typeof(RoutedEventHandler), ownerType);
			HiddenFieldListEvent = EventManager.RegisterRoutedEvent("HiddenFieldList", RoutingStrategy.Direct,
				typeof(RoutedEventHandler), ownerType);
			OlapDataLoadedEvent = EventManager.RegisterRoutedEvent("OlapDataLoaded", RoutingStrategy.Direct,
				typeof(RoutedEventHandler), ownerType);
			LayoutUpgradeEvent = EventManager.RegisterRoutedEvent("LayoutUpgrade", RoutingStrategy.Direct,
				typeof(PivotLayoutUpgradeEventHandler), ownerType);
			OlapQueryTimeoutEvent = EventManager.RegisterRoutedEvent("OlapQueryTimeout", RoutingStrategy.Direct,
				typeof(RoutedEventHandler), ownerType);
			OlapExceptionEvent = EventManager.RegisterRoutedEvent("OlapException", RoutingStrategy.Direct,
				typeof(PivotOlapExceptionEventHandler), ownerType);
			QueryExceptionEvent = EventManager.RegisterRoutedEvent("QueryException", RoutingStrategy.Direct,
				typeof(PivotQueryExceptionEventHandler), ownerType);
			PrefilterCriteriaChangedEvent = EventManager.RegisterRoutedEvent("PrefilterCriteriaChanged", RoutingStrategy.Direct,
				typeof(RoutedEventHandler), ownerType);
			FieldValueCollapsedEvent = EventManager.RegisterRoutedEvent("FieldValueCollapsed", RoutingStrategy.Direct,
				typeof(PivotFieldValueEventHandler), ownerType);
			FieldValueExpandedEvent = EventManager.RegisterRoutedEvent("FieldValueExpanded", RoutingStrategy.Direct,
				typeof(PivotFieldValueEventHandler), ownerType);
			FieldValueNotExpandedEvent = EventManager.RegisterRoutedEvent("FieldValueNotExpanded", RoutingStrategy.Direct,
				typeof(PivotFieldValueEventHandler), ownerType);
			FieldValueCollapsingEvent = EventManager.RegisterRoutedEvent("FieldValueCollapsing", RoutingStrategy.Direct,
				typeof(PivotFieldValueCancelEventHandler), ownerType);
			FieldValueExpandingEvent = EventManager.RegisterRoutedEvent("FieldValueExpanding", RoutingStrategy.Direct,
				typeof(PivotFieldValueCancelEventHandler), ownerType);
			CustomCellDisplayTextEvent = EventManager.RegisterRoutedEvent("CustomCellDisplayText", RoutingStrategy.Direct,
				typeof(PivotCellDisplayTextEventHandler), ownerType);
			CustomCellValueEvent = EventManager.RegisterRoutedEvent("CustomCellValue", RoutingStrategy.Direct,
				typeof(PivotCellValueEventHandler), ownerType);
			CustomCellAppearanceEvent = EventManager.RegisterRoutedEvent("CustomCellAppearance", RoutingStrategy.Direct,
				typeof(PivotCustomCellAppearanceEventHandler), ownerType);
			CustomValueAppearanceEvent = EventManager.RegisterRoutedEvent("CustomValueAppearance", RoutingStrategy.Direct,
				typeof(PivotCustomValueAppearanceEventHandler), ownerType);
			GroupFilterChangedEvent = EventManager.RegisterRoutedEvent("GroupFilterChanged", RoutingStrategy.Direct,
				typeof(PivotGroupEventHandler), ownerType);
			FieldAreaChangingEvent = EventManager.RegisterRoutedEvent("FieldAreaChanging", RoutingStrategy.Direct,
				typeof(PivotFieldAreaChangingEventHandler), ownerType);
			FieldAreaChangedEvent = EventManager.RegisterRoutedEvent("FieldAreaChanged", RoutingStrategy.Direct,
				typeof(PivotFieldEventHandler), ownerType);
			FieldExpandedInGroupChangedEvent = EventManager.RegisterRoutedEvent("FieldExpandedInGroupChanged", RoutingStrategy.Direct,
				typeof(PivotFieldEventHandler), ownerType);
			FieldFilterChangedEvent = EventManager.RegisterRoutedEvent("FieldFilterChanged", RoutingStrategy.Direct,
				typeof(PivotFieldEventHandler), ownerType);
			FieldFilterChangingEvent = EventManager.RegisterRoutedEvent("FieldFilterChanging", RoutingStrategy.Direct,
				typeof(PivotFieldFilterChangingEventHandler), ownerType);
			FieldUnboundExpressionChangedEvent = EventManager.RegisterRoutedEvent("FieldUnboundExpressionChanged", RoutingStrategy.Direct,
				typeof(PivotFieldEventHandler), ownerType);
			FieldPropertyChangedEvent = EventManager.RegisterRoutedEvent("FieldPropertyChanged", RoutingStrategy.Direct,
				typeof(PivotFieldPropertyChangedEventHandler), ownerType);
			FieldSizeChangedEvent = EventManager.RegisterRoutedEvent("FieldSizeChanged", RoutingStrategy.Direct,
				typeof(PivotFieldEventHandler), ownerType);
			FieldAreaIndexChangedEvent = EventManager.RegisterRoutedEvent("FieldAreaIndexChanged", RoutingStrategy.Direct,
				typeof(PivotFieldEventHandler), ownerType);
			FieldVisibleChangedEvent = EventManager.RegisterRoutedEvent("FieldVisibleChanged", RoutingStrategy.Direct,
				typeof(PivotFieldEventHandler), ownerType);
			CellClickEvent = EventManager.RegisterRoutedEvent("CellClick", RoutingStrategy.Direct,
				typeof(PivotCellEventHandler), ownerType);
			CellDoubleClickEvent = EventManager.RegisterRoutedEvent("CellDoubleClick", RoutingStrategy.Direct,
				typeof(PivotCellEventHandler), ownerType);
			CellSelectionChangedEvent = EventManager.RegisterRoutedEvent("CellSelectionChanged", RoutingStrategy.Direct,
				typeof(RoutedEventHandler), ownerType);
			FocusedCellChangedEvent = EventManager.RegisterRoutedEvent("FocusedCellChanged", RoutingStrategy.Direct,
				typeof(RoutedEventHandler), ownerType);
			UnboundExpressionEditorCreatedEvent = EventManager.RegisterRoutedEvent("UnboundExpressionEditorCreated", RoutingStrategy.Direct,
				typeof(PivotUnboundExpressionEditorEventHandler), ownerType);
			PrefilterEditorCreatedEvent = EventManager.RegisterRoutedEvent("PrefilterEditorCreated", RoutingStrategy.Direct,
				typeof(PivotFilterEditorEventHandler), ownerType);
			PrefilterEditorHidingEvent = EventManager.RegisterRoutedEvent("PrefilterEditorHiding", RoutingStrategy.Direct,
				typeof(PivotFilterEditorEventHandler), ownerType);
			CustomFilterPopupItemsEvent = EventManager.RegisterRoutedEvent("CustomFilterPopupItems", RoutingStrategy.Direct,
				typeof(PivotCustomFilterPopupItemsEventHandler), ownerType);
			CustomFieldValueCellsEvent = EventManager.RegisterRoutedEvent("CustomFieldValueCells", RoutingStrategy.Direct,
				typeof(PivotCustomFieldValueCellsEventHandler), ownerType);
			CustomPrefilterDisplayTextEvent = EventManager.RegisterRoutedEvent("CustomPrefilterDisplayText", RoutingStrategy.Direct,
				typeof(CustomPrefilterDisplayTextEventHandler), ownerType);
			ShowHeadersPropertyChangedEvent = EventManager.RegisterRoutedEvent("ShowHeadersPropertyChanged", RoutingStrategy.Direct,
				typeof(RoutedEventHandler), ownerType);
			PropertyChangedEvent = EventManager.RegisterRoutedEvent("PropertyChanged", RoutingStrategy.Direct,
				typeof(PivotPropertyChangedEventHandler), ownerType);
			BrushChangedEvent = EventManager.RegisterRoutedEvent("BrushChanged", RoutingStrategy.Direct,
				typeof(PivotBrushChangedEventHandler), ownerType);
			AsyncOperationStartingEvent = EventManager.RegisterRoutedEvent("AsyncOperationStarting", RoutingStrategy.Direct,
				typeof(RoutedEventHandler), ownerType);
			AsyncOperationCompletedEvent = EventManager.RegisterRoutedEvent("AsyncOperationCompleted", RoutingStrategy.Direct,
				typeof(RoutedEventHandler), ownerType);
			PopupMenuShowingEvent = EventManager.RegisterRoutedEvent("PopupMenuShowing", RoutingStrategy.Direct,
				typeof(PopupMenuShowingEventHandler), ownerType);
#if !SL
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.SelectAll,
																						(d, e) => ((PivotGridControl)d).VisualItems.OnKeyDown(65, true, false), 
																						(d, e) => ((PivotGridControl)d).CanSelectAll(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(ApplicationCommands.Copy, 
																						(d, e) => ((PivotGridControl)d).CopySelectionToClipboard(),
																						(d, e) => ((PivotGridControl)d).CanCopy(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ChangeFieldValueExpanded,
				(d, e) => ((PivotGridControl)d).OnChangeFieldValueExpanded(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.CollapseField,
				(d, e) => ((PivotGridControl)d).Data.ChangeFieldExpandedAsync((PivotGridField)e.Parameter, false, false), (d, e) => ((PivotGridControl)d).OnIsValidExpandCollapseField(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ExpandField,
				(d, e) => ((PivotGridControl)d).Data.ChangeFieldExpandedAsync((PivotGridField)e.Parameter, true, false), (d, e) => ((PivotGridControl)d).OnIsValidExpandCollapseField(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ChangeFieldSortOrder,
				(d, e) => ((PivotGridControl)d).OnChangeFieldSortOrder(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowFieldList,
				(d, e) => ((PivotGridControl)d).ShowFieldList(),
				(d, e) => ((PivotGridControl)d).OnCanShowFieldList(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.HideFieldList,
				(d, e) => ((PivotGridControl)d).HideFieldList(),
				(d, e) => ((PivotGridControl)d).OnCanHideFieldList(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ReloadData,
				(d, e) => ((PivotGridControl)d).Data.ReloadDataAsync(false)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.HideField,
				(d, e) => ((PivotGridField)e.Parameter).Hide(),
				(d, e) => ((PivotGridControl)d).OnCanHideField(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowUnboundExpressionEditor,
				(d, e) => ((PivotGridControl)d).OnShowUnboundExpressionEditor(e),
				(d, e) => ((PivotGridControl)d).OnCanShowUnboundExpressionEditor(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowPrefilter,
				(d, e) => ((PivotGridControl)d).ShowPrefilter(),
				(d, e) => ((PivotGridControl)d).OnCanShowPrefilter(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.HidePrefilter,
				(d, e) => ((PivotGridControl)d).HidePrefilter(),
				(d, e) => ((PivotGridControl)d).OnCanHidePrefilter(e)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ResetPrefilter,
				(d, e) => ((PivotGridControl)d).ResetPrefilter()));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowPrintPreview,
				(d, e) => ((PivotGridControl)d).ShowPrintPreview(Window.GetWindow((DependencyObject)d))));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowPrintPreviewDialog,
				(d, e) => ((PivotGridControl)d).ShowPrintPreviewDialog(Window.GetWindow((DependencyObject)d))));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.SortAscending,
				(d, e) => ((PivotGridControl)d).Data.SetFieldSortingAsync((PivotGridField)e.Parameter, FieldSortOrder.Ascending, false),
				OnCanSortAscending));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.SortDescending,
				(d, e) => ((PivotGridControl)d).Data.SetFieldSortingAsync((PivotGridField)e.Parameter, FieldSortOrder.Descending, false),
				OnCanSortDescending));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ClearSorting,
				(d, e) => ((PivotGridControl)d).Data.ClearFieldSortingAsync((PivotGridField)e.Parameter, false),
				OnCanClearSorting));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowLessThanFormatConditionDialog,
			   (d, e) => ((PivotGridControl)d).ShowFormatConditionDialog(e.Parameter, FormatConditionDialogType.LessThan), (d, e) => e.CanExecute = e.Parameter is FormatConditionCommandParameters));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowGreaterThanFormatConditionDialog,
			   (d, e) => ((PivotGridControl)d).ShowFormatConditionDialog(e.Parameter, FormatConditionDialogType.GreaterThan), (d, e) => e.CanExecute = e.Parameter is FormatConditionCommandParameters));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowEqualToFormatConditionDialog,
			  (d, e) => ((PivotGridControl)d).ShowFormatConditionDialog(e.Parameter, FormatConditionDialogType.EqualTo), (d, e) => e.CanExecute = e.Parameter is FormatConditionCommandParameters));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowBetweenFormatConditionDialog,
			   (d, e) => ((PivotGridControl)d).ShowFormatConditionDialog(e.Parameter, FormatConditionDialogType.Between), (d, e) => e.CanExecute = e.Parameter is FormatConditionCommandParameters));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowTextThatContainsFormatConditionDialog,
			  (d, e) => ((PivotGridControl)d).ShowFormatConditionDialog(e.Parameter, FormatConditionDialogType.TextThatContains), (d, e) => e.CanExecute = e.Parameter is FormatConditionCommandParameters));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowADateOccurringFormatConditionDialog,
			   (d, e) => ((PivotGridControl)d).ShowFormatConditionDialog(e.Parameter, FormatConditionDialogType.ADateOccurring), (d, e) => e.CanExecute = e.Parameter is FormatConditionCommandParameters));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowCustomConditionFormatConditionDialog,
			 (d, e) => ((PivotGridControl)d).ShowFormatConditionDialog(e.Parameter, FormatConditionDialogType.CustomCondition), (d, e) => e.CanExecute = e.Parameter is FormatConditionCommandParameters));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowTop10ItemsFormatConditionDialog,
			  (d, e) => ((PivotGridControl)d).ShowFormatConditionDialog(e.Parameter, FormatConditionDialogType.Top10Items), (d, e) => e.CanExecute = e.Parameter is FormatConditionCommandParameters));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowBottom10ItemsFormatConditionDialog,
			 (d, e) => ((PivotGridControl)d).ShowFormatConditionDialog(e.Parameter, FormatConditionDialogType.Bottom10Items), (d, e) => e.CanExecute = e.Parameter is FormatConditionCommandParameters));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowTop10PercentFormatConditionDialog,
			  (d, e) => ((PivotGridControl)d).ShowFormatConditionDialog(e.Parameter, FormatConditionDialogType.Top10Percent), (d, e) => e.CanExecute = e.Parameter is FormatConditionCommandParameters));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowBottom10PercentFormatConditionDialog,
			 (d, e) => ((PivotGridControl)d).ShowFormatConditionDialog(e.Parameter, FormatConditionDialogType.Bottom10Percent), (d, e) => e.CanExecute = e.Parameter is FormatConditionCommandParameters));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowAboveAverageFormatConditionDialog,
			 (d, e) => ((PivotGridControl)d).ShowFormatConditionDialog(e.Parameter, FormatConditionDialogType.AboveAverage), (d, e) => e.CanExecute = e.Parameter is FormatConditionCommandParameters));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowBelowAverageFormatConditionDialog,
			 (d, e) => ((PivotGridControl)d).ShowFormatConditionDialog(e.Parameter, FormatConditionDialogType.BelowAverage), (d, e) => e.CanExecute = e.Parameter is FormatConditionCommandParameters));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ClearFormatConditionsFromAllMeasures,
				 (d, e) => ((PivotGridControl)d).ClearFormatConditionsFromAllMeasures(), (d,e) => e.CanExecute = true));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ClearFormatConditionsFromMeasure,
				  (d, e) => ((PivotGridControl)d).ClearFormatConditionsFromMeasure(GetField(e.Parameter)), (d, e) => e.CanExecute = (GetField(e.Parameter) != null)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ShowConditionalFormattingManager,
				 (d, e) => ((PivotGridControl)d).ShowConditionalFormattingManager(GetField(e.Parameter)), (d, e) => e.CanExecute = (GetField(e.Parameter) != null)));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.AddFormatCondition,
					  (d, e) => ((PivotGridControl)d).AddFormatCondition((FormatConditionBase)e.Parameter), (d, e) => e.CanExecute = true));
			CommandManager.RegisterClassCommandBinding(ownerType, new CommandBinding(PivotGridCommands.ClearFormatConditionsFromIntersection,
			   (d, e) => {
				   FormatConditionCommandParameters item = (FormatConditionCommandParameters)e.Parameter;
				   ((PivotGridControl)d).ClearFormatConditionsFromIntersection(item.Row, item.Column);
			   },
			   (d, e) => e.CanExecute = e.Parameter is FormatConditionCommandParameters));
			EventManager.RegisterClassHandler(ownerType, Keyboard.KeyDownEvent, new KeyEventHandler(OnChildKeyDown), true);
#endif
		}
		void CanCopy(CanExecuteRoutedEventArgs e) {
			e.CanExecute = true;
		}
		void CanSelectAll(CanExecuteRoutedEventArgs e) {
			e.CanExecute = SelectMode != PivotGrid.SelectMode.None;
		}
		private static bool NewMethod(CanExecuteRoutedEventArgs e) {
			return (e.Parameter as FormatConditionBase != null);
		}
		void ShowFormatConditionDialog(object field, FormatConditionDialogType dialogKind) {
			FormatConditionCommandParameters settings = (FormatConditionCommandParameters)field;
			ShowFormatConditionDialog(settings.Measure, settings.Row, settings.Column, dialogKind);
		}
		static PivotGridField GetField(object data) {
			FormatConditionCommandParameters set = data as FormatConditionCommandParameters;
			if(set != null)
				return set.Measure;
			PivotGridField field = data as PivotGridField;
			if(field != null)
				return field;
			PivotGridInternalField intfield = data as PivotGridInternalField;
			if(intfield != null)
				return intfield.Wrapper;
			return null;
		}
		void SetFieldWidth() {
			if(IsLoading)
				return;
			foreach(PivotGridField field in Fields)
				field.UpdateWidthProperty();
			if(Data != null) {
				this.CoerceValue(PivotGridControl.DataFieldWidthProperty);
				SyncDataFieldWidth();
				this.CoerceValue(PivotGridControl.RowTreeWidthProperty);
				Data.VisualItems.RowTreeField.Width = RowTreeWidth;
			}
		}
		void SetFieldHeight() {
			if(IsLoading)
				return;
			foreach(PivotGridField field in Fields)
				field.UpdateHeightProperty();
			if(Data != null) {
				this.CoerceValue(PivotGridControl.DataFieldHeightProperty);
				SyncDataFieldHeight();
				this.CoerceValue(PivotGridControl.RowTreeHeightProperty);
				Data.VisualItems.RowTreeField.Height = RowTreeHeight;
			}
		}
		static void OnPropertyChanged(PropertyChangedCallback baseCallback, DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridControl pivot = (PivotGridControl)d;
			PivotGridWpfData.DataRelatedDPChanged(pivot, baseCallback, d, e);
		}
		static void OnDataSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridControl control = (PivotGridControl)d;
			control.OnDataSourceChanged(e.OldValue, e.NewValue);
		}
		static void OnOlapConnectionStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridControl control = (PivotGridControl)d;
			control.OnOlapSourceChanged((string)e.OldValue, (string)e.NewValue);
		}
		static void OnOlapDefaultMemberFieldsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridControl control = (PivotGridControl)d;
			control.Data.OptionsOLAP.DefaultMemberFields = (CoreXtraPivotGrid.PivotDefaultMemberFields)e.NewValue;
		}
		static void OnPrefilterCriteriaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).OnPrefilterCriteriaChanged((CriteriaOperator)e.OldValue, (CriteriaOperator)e.NewValue);
		}
		static void OnPrefilterStringPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).OnPrefilterStringChanged((string)e.OldValue, (string)e.NewValue);
		}
		static void OnFieldAppearancePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).UpdateFieldAppearances();
		}
		static void OnIsPrefiterEnabledPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.Prefilter.Enabled = (bool)e.NewValue;
		}
		static void OnAllowCrossGroupVariationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsData.AllowCrossGroupVariation = (bool)e.NewValue;
		}
		static void OnGroupDataCaseSensitivePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsData.CaseSensitive = (bool)e.NewValue;
		}
		static void OnAutoExpandGroupsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsData.AutoExpandGroups = ((bool?)e.NewValue).ToDefaultBoolean();
		}
		static void OnDrillDownMaxRowCountPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsData.DrillDownMaxRowCount = (int)e.NewValue;
		}
		static void OnDataFieldUnboundExpressionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsData.DataFieldUnboundExpressionMode = ((UnboundExpressionMode)e.NewValue).ToDataFieldUnboundExpressionMode();
		}
		static void OnFilterByVisibleFieldsOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridWpfData data = ((PivotGridControl)d).Data;
			data.OptionsData.FilterByVisibleFieldsOnly = (bool)e.NewValue;
			foreach(PivotGridField field in data.FieldListFields.HiddenFields) {
				field.UpdateIsFilterButtonVisible();
				field.UpdateIsFiltered();
			}
		}
		static void OnAllowExpandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsCustomization.AllowExpand = (bool)e.NewValue;
		}
		static void OnAllowFilterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridControl pivot = ((PivotGridControl)d);
			pivot.Data.OptionsCustomization.AllowFilter = (bool)e.NewValue;
			pivot.UpdateFieldsButtonsState();
		}
		static void OnAllowSortPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridControl pivot = ((PivotGridControl)d);
			pivot.Data.OptionsCustomization.AllowSort = (bool)e.NewValue;
			pivot.UpdateFieldsButtonsState();
		}
		static void OnAllowSortBySummaryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsCustomization.AllowSortBySummary = (bool)e.NewValue;
		}
		static void OnFieldListStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).OnFieldListStyleChanged((FieldListStyle)e.NewValue);
		}
		static void OnFieldListLayoutPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).OnFieldListLayoutChanged((FieldListLayout)e.NewValue);
		}
		static object CoerceFieldListLayoutProperty(DependencyObject d, object baseValue) {
			PivotGridControl pivot = ((PivotGridControl)d);
			if(!CustomizationFormEnumExtensions.IsLayoutAllowed(pivot.FieldListAllowedLayouts, (FieldListLayout)baseValue))
				return pivot.FieldListLayout;
			return baseValue;
		}
		static void OnFieldListAllowedLayoutsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).OnFieldListAllowedLayoutsChanged((FieldListAllowedLayouts)e.NewValue);
		}
		static void OnDeferredUpdatesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			FieldListFields fieldListFields = ((PivotGridControl)d).Data.FieldListFields;
			bool newValue = (bool)e.NewValue;
			fieldListFields.DeferUpdates = newValue;
			if(!newValue)
				fieldListFields.SetFieldsToData();
		}
		static void OnDrawFocusedCellRectPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).OnFocusChanged();
		}
		static void OnShowColumnGrandTotalsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsView.ShowColumnGrandTotals = (bool)e.NewValue;
		}
		static void OnShowColumnTotalsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsView.ShowColumnTotals = (bool)e.NewValue;
		}
		static void OnShowCustomTotalsForSingleValuesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsView.ShowCustomTotalsForSingleValues = (bool)e.NewValue;
		}
		static void OnShowGrandTotalsForSingleValuesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsView.ShowGrandTotalsForSingleValues = (bool)e.NewValue;
		}
		static void OnShowRowGrandTotalsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsView.ShowRowGrandTotals = (bool)e.NewValue;
		}
		static object OnShowRowTotalsPropertyChanging(DependencyObject d, object value) {
			PivotGridControl pivot = d as PivotGridControl;
			if(pivot != null && !pivot.Data.IsDeserializing && ((bool)value) == false && pivot.RowTotalsLocation == FieldRowTotalsLocation.Tree)
				return true;
			return value;
		}
		static void OnShowRowTotalsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsView.ShowRowTotals = (bool)e.NewValue;
			d.CoerceValue(RowTotalsLocationProperty);
		}
		static void OnShowTotalsForSingleValuesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsView.ShowTotalsForSingleValues = (bool)e.NewValue;
		}
		static void OnColumnTotalsLocationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsView.ColumnTotalsLocation = ((FieldColumnTotalsLocation)e.NewValue).ToPivotTotalsLocationType();
		}
		static object OnRowTotalsLocationPropertyChanging(DependencyObject d, object value) {
			PivotGridControl pivot = d as PivotGridControl;
			if(((FieldRowTotalsLocation)value) == FieldRowTotalsLocation.Tree && pivot != null && !pivot.Data.IsDeserializing && !pivot.ShowRowTotals)
#if !SL
				return pivot.RowTotalsLocation;
#else
				return FieldRowTotalsLocation.Near;
#endif
			return value;
		}
		static void OnRowTotalsLocationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsView.RowTotalsLocation = ((FieldRowTotalsLocation)e.NewValue).ToPivotTotalsLocationType();
			d.CoerceValue(ShowRowTotalsProperty);
		}
		static void ShowColumnGrandTotalHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsView.ShowColumnGrandTotalHeader = (bool)e.NewValue;
		}
		static void ShowRowGrandTotalHeaderPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsView.ShowRowGrandTotalHeader = (bool)e.NewValue;
		}
		static void OnRowTreeWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridControl pivot = ((PivotGridControl)d);
			if(pivot.IsLoading)
				return;
			pivot.VisualItems.RowTreeField.Width = (int)e.NewValue;
		}
		static void OnRowTreeHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridControl pivot = ((PivotGridControl)d);
			if(pivot.IsLoading)
				return;
			pivot.VisualItems.RowTreeField.Height = (int)e.NewValue;
		}
		void OnRowTreeMinHeightChanged() {
			VisualItems.RowTreeField.MinHeight = RowTreeMinHeight;
		}
		void OnRowTreeMinWidthChanged() {
			VisualItems.RowTreeField.MinWidth = RowTreeMinWidth;
		}
		static void OnRowTreeOffsetPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsView.RowTreeOffset = (int)e.NewValue;
		}
		static void OnDataFieldAreaPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CoreXtraPivotGrid.PivotGridOptionsDataField options = ((PivotGridControl)d).Data.OptionsDataField;
			DataFieldArea newValue = (DataFieldArea)e.NewValue;
			options.Area = newValue.ToPivotDataAreaType();
			if(options.Area.ToDataFieldAreaType() != newValue)
				d.SetValue(DataFieldAreaProperty, options.Area.ToDataFieldAreaType());
		}
		static void OnDataFieldAreaIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			CoreXtraPivotGrid.PivotGridOptionsDataField options = ((PivotGridControl)d).Data.OptionsDataField;
			int newValue = (int)e.NewValue;
			options.AreaIndex = newValue;
			if(options.AreaIndex != newValue)
				d.SetValue(DataFieldAreaIndexProperty, options.AreaIndex);
		}
		static void OnDataFieldWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridControl pivot = ((PivotGridControl)d);
			if(pivot.IsLoading)
				return;
			pivot.SyncDataFieldWidth();
		}
		void SyncDataFieldWidth() {
			PivotGridField dataField = Data.DataField;
			double newValue = DataFieldWidth;
			dataField.Width = newValue;
			if(dataField.Width != newValue && !(Data.IsDeserializing && OnDataFieldWidthChanging(newValue) == dataField.Width))
				this.SetValue(DataFieldWidthProperty, dataField.Width);
		}
		static void OnDataFieldHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridControl pivot = ((PivotGridControl)d);
			if(pivot.IsLoading)
				return;
			pivot.SyncDataFieldHeight();
		}
		void SyncDataFieldHeight() {
			PivotGridField dataField = Data.DataField;
			double newValue = DataFieldHeight;
			dataField.Height = newValue;
			if(dataField.Height != newValue && !(Data.IsDeserializing && OnDataFieldHeightChanging(newValue) == dataField.Height))
				this.SetValue(DataFieldHeightProperty, dataField.Height);
		}
		static void OnDataFieldCaptionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridField dataField = ((PivotGridControl)d).Data.DataField;
			string newValue = (string)e.NewValue;
			dataField.Caption = newValue;
			if(dataField.Caption != newValue)
				d.SetValue(DataFieldCaptionProperty, dataField.Caption);
		}
		static void OnSummaryDataSourceFieldNamingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsDataField.FieldNaming = ((SummaryDataSourceFieldNaming)e.NewValue).ToDataFieldNamingType();
		}
		static void OnChartProvideDataByColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.ProvideDataByColumns = (bool)e.NewValue;
		}
		static void OnChartFieldValuesProvideModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.FieldValuesProvideMode =
				((PivotChartFieldValuesProvideMode)e.NewValue).ToCorePivotChartFieldValuesProvideMode();
		}
		static void OnChartDataProvideModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.DataProvideMode =
				((PivotChartDataProvideMode)e.NewValue).ToCorePivotChartDataProvideMode();
		}
		static void OnChartDataProvidePriorityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.DataProvidePriority =
				((PivotChartDataProvidePriority)e.NewValue).ToCorePivotChartDataProvidePriority();
		}
		static void OnChartSelectionOnlyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CoreXtraPivotGrid.PivotGridOptionsChartDataSource)(((PivotGridControl)d).Data.OptionsChartDataSource)).SelectionOnly = (bool)e.NewValue;
		}
		static void OnChartProvideColumnTotalsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.ProvideColumnTotals = (bool)e.NewValue;
		}
		static void OnChartProvideColumnGrandTotalsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.ProvideColumnGrandTotals = (bool)e.NewValue;
		}
		static void OnChartProvideColumnCustomTotalsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.ProvideColumnCustomTotals = (bool)e.NewValue;
		}
		static void OnChartProvideRowTotalsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.ProvideRowTotals = (bool)e.NewValue;
		}
		static void OnChartProvideRowGrandTotalsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.ProvideRowGrandTotals = (bool)e.NewValue;
		}
		static void OnChartProvideRowCustomTotalsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.ProvideRowCustomTotals = (bool)e.NewValue;
		}
		static void OnChartAutoTransposeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.AutoTransposeChart = (bool)e.NewValue;
		}
		static void OnChartProvideEmptyCellsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.ProvideEmptyCells = (bool)e.NewValue;
		}
		static void OnChartProvideColumnFieldValuesAsTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.ProvideColumnFieldValuesAsType = (Type)e.NewValue;
		}
		static void OnChartProvideRowFieldValuesAsTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.ProvideRowFieldValuesAsType = (Type)e.NewValue;
		}
		static void OnChartProvideCellValuesAsTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.ProvideCellValuesAsType = (Type)e.NewValue;
		}
		static void OnChartUpdateDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((CoreXtraPivotGrid.PivotGridOptionsChartDataSource)(((PivotGridControl)d).Data.OptionsChartDataSource)).UpdateDelay = (int)e.NewValue;
		}
		static object CoerceChartUpdateDelay(DependencyObject d, object baseValue) {
			return (int)baseValue < 0 ? 0 : baseValue;
		}
		static void OnChartMaxSeriesCountChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.MaxAllowedSeriesCount = (int)e.NewValue;
		}
		static void OnChartMaxPointCountInSeriesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsChartDataSource.MaxAllowedPointCountInSeries = (int)e.NewValue;
		}
		static void OnPivotCellBrushesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).RaisePivotBrushesChanged(PivotBrushType.CellBrush);
		}
		static void OnPivotValueBrushesChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).RaisePivotBrushesChanged(PivotBrushType.ValueBrush);
		}
		static void OnCopyToClipboardWithFieldValuesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsBehavior.CopyToClipboardWithFieldValues = (bool)e.NewValue;
		}
		static void OnClipboardCopyMultiSelectionModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsBehavior.ClipboardCopyMultiSelectionMode = ((CopyMultiSelectionMode)e.NewValue).ToCoreMultiSelectionMode();
		}
		static void OnClipboardCopyCollapsedValuesModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsBehavior.ClipboardCopyCollapsedValuesMode = ((CopyCollapsedValuesMode)e.NewValue).ToCoreCopyCollapsedValuesMode();
		}
		static void OnUseAsyncModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsBehavior.UseAsyncMode = (bool)e.NewValue;
		}
		static void OnLoadingPanelDelayPropertyChaned(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).Data.OptionsBehavior.LoadingPanelDelay = (int)e.NewValue;
		}
		static void OnScrollingModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridControl pivot = d as PivotGridControl;
			if(pivot == null || pivot.PivotGridScroller == null)
				return;
			pivot.PivotGridScroller.EnsureScrollingMode();
		}
		static void OnFixedRowHeadersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridControl pivot = d as PivotGridControl;
			if(pivot == null || pivot.PivotGridScroller == null)
				return;
			pivot.PivotGridScroller.EnsureHorizontalScrollMode();
		}
		static void OnSelectModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridControl pivot = (PivotGridControl)d;
			CoreXtraPivotGrid.PivotGridOptionsSelection options = pivot.Data.OptionsSelection;
			SelectMode selectMode = (SelectMode)e.NewValue;
			bool oldCellSelection = options.CellSelection;
			options.CellSelection = selectMode != SelectMode.None;
			options.MultiSelect = selectMode == SelectMode.MultiSelection;
			if(!oldCellSelection && options.CellSelection)
				pivot.OnSelectionChanged(pivot.Selection);
			if(oldCellSelection && !options.CellSelection)
				pivot.VisualItems.RaiseSelectionChanged();
		}
		static object OnFocusedCellPropertyChanging(DependencyObject d, object value) {
			PivotGridControl pivot = (PivotGridControl)d;
			if(pivot.IsLoading)
				return value;
			else
				return pivot.VisualItems.CorrectFocusedCell((System.Drawing.Point)value);
		}
		static void OnFocusedCellPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).OnFocusedCellChanged((System.Drawing.Point)e.NewValue);
		}
		static void OnSelectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridControl)d).OnSelectionChanged((System.Drawing.Rectangle)e.NewValue);
		}
		static void OnChildKeyDown(object sender, KeyEventArgs e) {
			if(!e.Handled && ((PivotGridControl)sender).OnKeyDown(e.Key))
				e.Handled = true;
		}
		static object CoerceGroupFieldsInFieldListProperty(DependencyObject d, object baseValue) {
			PivotGridControl pivot = (PivotGridControl)d;
			bool hasFolders = false;
			if(((bool)baseValue) == true && pivot.IsInitialized)
				for(int i = 0; i < pivot.Fields.Count; i++)
					if(!string.IsNullOrEmpty(pivot.Fields[i].DisplayFolder)) {
						hasFolders = true;
						break;
					}
			return hasFolders ? baseValue : false;
		}
		static void OnGroupFieldsInFieldListPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
		}
		static void OnCanSortAscending(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = e.Parameter != null ? ((PivotGridField)e.Parameter).CanSortAscending : false;
		}
		static void OnCanSortDescending(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = e.Parameter != null ? ((PivotGridField)e.Parameter).CanSortDescending : false;
		}
		static void OnCanClearSorting(object sender, CanExecuteRoutedEventArgs e) {
			e.CanExecute = e.Parameter != null ? ((PivotGridField)e.Parameter).CanClearSorting : false;
		}
		public static void SetFieldHeaderDragIndicatorSize(DependencyObject element, double value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(FieldHeaderDragIndicatorSizeProperty, value);
		}
		public static double GetFieldHeaderDragIndicatorSize(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (double)element.GetValue(FieldHeaderDragIndicatorSizeProperty);
		}
		public static PivotGridWpfData GetData(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (PivotGridWpfData)element.GetValue(DataProperty);
		}
		internal static void SetData(DependencyObject element, PivotGridWpfData value) {
			if(element == null)
				throw new ArgumentNullException("element");
			if((element as PivotGridControl) != null)
				throw new ArgumentNullException("pivot");
			element.SetValue(DataPropertyKey, value);
		}
		internal static PivotGridWpfData GetDataFromAttachedPivot(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			PivotGridControl pivot = element.GetValue(PivotGridProperty) as PivotGridControl;
			if(pivot == null)
				return null;
			return pivot.Data;
		}
		public static PivotGridControl GetPivotGrid(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (PivotGridControl)element.GetValue(PivotGridProperty);
		}
		public static void SetPivotGrid(DependencyObject element, PivotGridControl value) {
			if(element == null || element is PivotGridControl && value != null && value != element)
				throw new ArgumentNullException("element");
			element.SetValue(PivotGridProperty, value);
		}
		public static bool GetShowBorder(DependencyObject element) {
			if(element == null)
				throw new ArgumentNullException("element");
			return (bool)element.GetValue(ShowBorderProperty);
		}
		public static void SetShowBorder(DependencyObject element, bool value) {
			if(element == null)
				throw new ArgumentNullException("element");
			element.SetValue(ShowBorderProperty, value);
		}
		object OnRowTreeHeightChanging(object baseValue) {
			if(object.Equals(baseValue, Int32.MaxValue)) {
				return Convert.ToInt32(FieldHeight);
			}
			return baseValue;
		}
		object OnRowTreeWidthChanging(object baseValue) {
			if(object.Equals(baseValue, Int32.MaxValue)) {
				return Convert.ToInt32(FieldWidth);
			}
			return baseValue;
		}
		double OnDataFieldHeightChanging(double baseValue) {
			if(object.Equals(baseValue, double.NaN)) {
				return FieldHeight;
			}
			return baseValue;
		}
		double OnDataFieldWidthChanging(double baseValue) {
			if(object.Equals(baseValue, double.NaN)) {
				return FieldWidth;
			}
			return baseValue;
		}
	}
}
