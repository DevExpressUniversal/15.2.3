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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Core;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.Xpf.PivotGrid.Internal;
using System.Collections.Generic;
using DevExpress.Utils;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.Editors.Settings;
using System.Windows.Data;
using DevExpress.Data.Utils;
using DevExpress.Xpf.Core.Native;
#if SL
using DXFrameworkContentElement = System.Windows.FrameworkElement;
using ApplicationException = System.Exception;
using RoutedEvent = DevExpress.Xpf.Core.WPFCompatibility.SLRoutedEvent;
using DependencyPropertyChangedEventArgs = DevExpress.Xpf.Core.WPFCompatibility.SLDependencyPropertyChangedEventArgs;
using PropertyMetadata = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyMetadata;
using PropertyChangedCallback = DevExpress.Xpf.Core.WPFCompatibility.SLPropertyChangedCallback;
using DevExpress.Xpf.Core.WPFCompatibility;
#else
using DependencyPropertyManager = System.Windows.DependencyProperty;
using DependencyPropertyChangedEventArgs = System.Windows.DependencyPropertyChangedEventArgs;
#endif
namespace DevExpress.Xpf.PivotGrid {
	public class PivotGridField : DXFrameworkContentElement, ILogicalOwner, IThreadSafeField
#if SL    
		, IDependencyPropertyChangeListener
#endif
 {
		#region static stuff
#if !SL
		public static readonly DependencyProperty WidthProperty;
		public static readonly DependencyProperty MinWidthProperty;
		public static readonly DependencyProperty HeightProperty;
		public static readonly DependencyProperty MinHeightProperty;
#else
		public static readonly new DependencyProperty WidthProperty;
		public static readonly new DependencyProperty MinWidthProperty;
		public static readonly new DependencyProperty HeightProperty;
		public static readonly new DependencyProperty MinHeightProperty;
#endif
		public static readonly DependencyProperty OlapUseNonEmptyProperty;
		public static readonly DependencyProperty OlapFilterByUniqueNameProperty;
		public static readonly DependencyProperty AllowedAreasProperty;
		public static readonly DependencyProperty AreaProperty;
		public static readonly DependencyProperty AreaIndexProperty;
		public static readonly DependencyProperty CaptionProperty;
		public static readonly DependencyProperty DisplayFolderProperty;
		public static readonly DependencyProperty ExpandedInFieldsGroupProperty;
		public static readonly DependencyProperty EmptyCellTextProperty;
		public static readonly DependencyProperty EmptyValueTextProperty;
		public static readonly DependencyProperty FieldNameProperty;
		public static readonly DependencyProperty GrandTotalTextProperty;
		public static readonly DependencyProperty GroupIntervalProperty;
		public static readonly DependencyProperty GroupIntervalNumericRangeProperty;
		public static readonly DependencyProperty RunningTotalProperty;
		public static readonly DependencyProperty ShowNewValuesProperty;
		public static readonly DependencyProperty SortModeProperty;
		public static readonly DependencyProperty SortOrderProperty;
		public static readonly DependencyProperty SortByAttributeProperty;
		public static readonly DependencyProperty AutoPopulatedPropertiesProperty;
		public static readonly DependencyProperty SummaryTypeProperty;
		public static readonly DependencyProperty SummaryDisplayTypeProperty;
		public static readonly DependencyProperty TopValueCountProperty;
		public static readonly DependencyProperty TopValueShowOthersProperty;
		public static readonly DependencyProperty TopValueTypeProperty;
		public static readonly DependencyProperty TopValueModeProperty;
		public static readonly DependencyProperty TotalsVisibilityProperty;
		public static readonly DependencyProperty UnboundTypeProperty;
		public static readonly DependencyProperty UnboundExpressionProperty;
		public static readonly DependencyProperty UnboundFieldNameProperty;
		public static readonly DependencyProperty UseNativeFormatProperty;
		public static readonly DependencyProperty UnboundExpressionModeProperty;
		public static readonly DependencyProperty VisibleProperty;
		public static readonly DependencyProperty GroupProperty;
		public static readonly DependencyProperty HasGroupProperty;
		static readonly DependencyPropertyKey HasGroupPropertyKey;
		public static readonly DependencyProperty GroupIndexProperty;
		public static readonly DependencyProperty AllowSortProperty;
		public static readonly DependencyProperty AllowFilterProperty;
		public static readonly DependencyProperty AllowDragProperty;
		public static readonly DependencyProperty AllowDragInCustomizationFormProperty;
		public static readonly DependencyProperty AllowExpandProperty;
		public static readonly DependencyProperty AllowSortBySummaryProperty;
		public static readonly DependencyProperty AllowUnboundExpressionEditorProperty;
		public static readonly DependencyProperty ShowInCustomizationFormProperty;
		public static readonly DependencyProperty ShowInExpressionEditorProperty;
		public static readonly DependencyProperty ShowGrandTotalProperty;
		public static readonly DependencyProperty ShowTotalsProperty;
		public static readonly DependencyProperty ShowValuesProperty;
		public static readonly DependencyProperty ShowCustomTotalsProperty;
		public static readonly DependencyProperty ShowSummaryTypeNameProperty;
		public static readonly DependencyProperty HideEmptyVariationItemsProperty;
		public static readonly DependencyProperty OlapFilterUsingWhereClauseProperty;
		public static readonly DependencyProperty BestFitModeProperty;
		public static readonly DependencyProperty BestFitAreaProperty;
		public static readonly DependencyProperty BestFitMaxRowCountProperty;
		public static readonly DependencyProperty SortByFieldProperty;
		public static readonly DependencyProperty SortByFieldNameProperty;
		public static readonly DependencyProperty SortBySummaryTypeProperty;
		public static readonly DependencyProperty SortByCustomTotalSummaryTypeProperty;
		static readonly DependencyPropertyKey IsSortButtonVisiblePropertyKey;
		public static readonly DependencyProperty IsSortButtonVisibleProperty;
		static readonly DependencyPropertyKey IsFilterButtonVisiblePropertyKey;
		public static readonly DependencyProperty IsFilterButtonVisibleProperty;
		static readonly DependencyPropertyKey IsFilteredPropertyKey;
		public static readonly DependencyProperty IsFilteredProperty;
		static readonly DependencyPropertyKey DisplayTextPropertyKey;
		public static readonly DependencyProperty DisplayTextProperty;
		public static readonly DependencyProperty HeaderTemplateProperty;
		public static readonly DependencyProperty HeaderTemplateSelectorProperty;
		public static readonly DependencyProperty HeaderTreeViewTemplateProperty;
		public static readonly DependencyProperty HeaderTreeViewTemplateSelectorProperty;
		public static readonly DependencyProperty HeaderListTemplateProperty;
		public static readonly DependencyProperty HeaderListTemplateSelectorProperty;
		public static readonly DependencyProperty CellTemplateProperty;
		public static readonly DependencyProperty CellTemplateSelectorProperty;
		public static readonly DependencyProperty ValueTemplateProperty;
		public static readonly DependencyProperty ValueTemplateSelectorProperty;
		public static readonly DependencyProperty ActualHeaderTemplateProperty;
		public static readonly DependencyProperty ActualHeaderTemplateSelectorProperty;
		public static readonly DependencyProperty ActualHeaderTreeViewTemplateProperty;
		public static readonly DependencyProperty ActualHeaderTreeViewTemplateSelectorProperty;
		public static readonly DependencyProperty ActualHeaderListTemplateProperty;
		public static readonly DependencyProperty ActualHeaderListTemplateSelectorProperty;
		public static readonly DependencyProperty ActualCellTemplateProperty;
		public static readonly DependencyProperty ActualCellTemplateSelectorProperty;
		public static readonly DependencyProperty ActualValueTemplateProperty;
		public static readonly DependencyProperty ActualValueTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualHeaderTemplatePropertyKey;
		static readonly DependencyPropertyKey ActualHeaderTemplateSelectorPropertyKey;
		static readonly DependencyPropertyKey ActualHeaderTreeViewTemplatePropertyKey;
		static readonly DependencyPropertyKey ActualHeaderTreeViewTemplateSelectorPropertyKey;
		static readonly DependencyPropertyKey ActualHeaderListTemplatePropertyKey;
		static readonly DependencyPropertyKey ActualHeaderListTemplateSelectorPropertyKey;
		static readonly DependencyPropertyKey ActualCellTemplatePropertyKey;
		static readonly DependencyPropertyKey ActualCellTemplateSelectorPropertyKey;
		static readonly DependencyPropertyKey ActualValueTemplatePropertyKey;
		static readonly DependencyPropertyKey ActualValueTemplateSelectorPropertyKey;
		public static readonly DependencyProperty PrintHeaderTemplateProperty;
		public static readonly DependencyProperty PrintHeaderTemplateSelectorProperty;
		public static readonly DependencyProperty PrintValueTemplateProperty;
		public static readonly DependencyProperty PrintValueTemplateSelectorProperty;
		public static readonly DependencyProperty PrintCellTemplateProperty;
		public static readonly DependencyProperty PrintCellTemplateSelectorProperty;
		public static readonly DependencyProperty ActualPrintHeaderTemplateProperty;
		public static readonly DependencyProperty ActualPrintHeaderTemplateSelectorProperty;
		public static readonly DependencyProperty ActualPrintValueTemplateProperty;
		public static readonly DependencyProperty ActualPrintValueTemplateSelectorProperty;
		public static readonly DependencyProperty ActualPrintCellTemplateProperty;
		public static readonly DependencyProperty ActualPrintCellTemplateSelectorProperty;
		static readonly DependencyPropertyKey ActualPrintHeaderTemplatePropertyKey;
		static readonly DependencyPropertyKey ActualPrintHeaderTemplateSelectorPropertyKey;
		static readonly DependencyPropertyKey ActualPrintValueTemplatePropertyKey;
		static readonly DependencyPropertyKey ActualPrintValueTemplateSelectorPropertyKey;
		static readonly DependencyPropertyKey ActualPrintCellTemplatePropertyKey;
		static readonly DependencyPropertyKey ActualPrintCellTemplateSelectorPropertyKey;
		public static readonly DependencyProperty HeaderContentStyleProperty;
		public static readonly DependencyProperty ActualHeaderContentStyleProperty;
		static readonly DependencyPropertyKey ActualHeaderContentStylePropertyKey;
		public static readonly DependencyProperty CellFormatProperty;
		public static readonly DependencyProperty GrandTotalCellFormatProperty;
		public static readonly DependencyProperty TotalCellFormatProperty;
		public static readonly DependencyProperty TotalValueFormatProperty;
		public static readonly DependencyProperty ValueFormatProperty;
		public static readonly DependencyProperty DropDownFilterListSizeProperty;
		public static readonly DependencyProperty KpiGraphicProperty;
		public static readonly DependencyProperty KpiTypeProperty;
		static readonly DependencyPropertyKey KpiTypePropertyKey;
		public static readonly DependencyProperty ActualKpiGraphicProperty;
		static readonly DependencyPropertyKey ActualKpiGraphicPropertyKey;
		public static readonly DependencyProperty ShowInPrefilterProperty;
		static PivotGridField() {
			Type ownerType = typeof(PivotGridField);
			WidthProperty = DependencyPropertyManager.Register("Width", typeof(double), ownerType,
				new PropertyMetadata(double.NaN,
					(d, e) => ((PivotGridField)d).SyncFieldWidth(true, true), (d, e) => ((PivotGridField)d).OnWidthChanging((double)e)));
			MinWidthProperty = DependencyPropertyManager.Register("MinWidth", typeof(double), ownerType,
				new PropertyMetadata(Convert.ToDouble(PivotGridField.DefaultMinWidth),
					(d, e) => ((PivotGridField)d).SyncFieldMinWidth(true, true), OnMinWidthChanging));
			HeightProperty = DependencyPropertyManager.Register("Height", typeof(double), ownerType,
				new PropertyMetadata(double.NaN,
					(d, e) => ((PivotGridField)d).SyncFieldHeight(true, true), (d, e) => ((PivotGridField)d).OnHeightChanging((double)e)));
			MinHeightProperty = DependencyPropertyManager.Register("MinHeight", typeof(double), ownerType,
				new PropertyMetadata(Convert.ToDouble(PivotGridField.DefaultMinHeight),
					(d, e) => ((PivotGridField)d).SyncFieldMinHeight(true, true), OnMinHeightChanging));
			OlapUseNonEmptyProperty = DependencyPropertyManager.Register("OlapUseNonEmpty", typeof(bool), ownerType,
				new PropertyMetadata(true,
					(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldOLAPUseNonEmpty(true, true), d1, e1)));
			OlapFilterByUniqueNameProperty = DependencyPropertyManager.Register("OlapFilterByUniqueName", typeof(bool?), ownerType,
				new PropertyMetadata(null,
					(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldOLAPFilterByUniqueName(true, true), d1, e1)));
			AllowedAreasProperty = DependencyPropertyManager.Register("AllowedAreas", typeof(FieldAllowedAreas), ownerType,
				new PropertyMetadata(FieldAllowedAreas.All,
					(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldAllowedAreas(true, true), d1, e1)));
			AreaProperty = DependencyPropertyManager.Register("Area", typeof(FieldArea), ownerType,
				new PropertyMetadata(FieldArea.FilterArea,
					(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldArea(true, true, (FieldArea)e.NewValue, (FieldArea)e.OldValue), d1, e1)));
			AreaIndexProperty = DependencyPropertyManager.Register("AreaIndex", typeof(int), ownerType, new PropertyMetadata(-1,
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldAreaIndex(true, true), d1, e1)));
			CaptionProperty = DependencyPropertyManager.Register("Caption", typeof(string), ownerType, new PropertyMetadata(string.Empty,
				(d, e) => ((PivotGridField)d).SyncFieldCaption(true, true)));
			DisplayFolderProperty = DependencyPropertyManager.Register("DisplayFolder", typeof(string), ownerType, new PropertyMetadata(string.Empty,
				(d, e) => ((PivotGridField)d).SyncFieldDisplayFolder(true, true)));
			EmptyCellTextProperty = DependencyPropertyManager.Register("EmptyCellText", typeof(string), ownerType, new PropertyMetadata(string.Empty,
				(d, e) => ((PivotGridField)d).SyncFieldEmptyCellText(true, true)));
			EmptyValueTextProperty = DependencyPropertyManager.Register("EmptyValueText", typeof(string), ownerType, new PropertyMetadata(string.Empty,
				(d, e) => ((PivotGridField)d).SyncFieldEmptyValueText(true, true)));
			ExpandedInFieldsGroupProperty = DependencyPropertyManager.Register("ExpandedInFieldsGroup", typeof(bool), ownerType, new PropertyMetadata(true,
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldExpandedInFieldsGroup(true, true), d1, e1)));
			FieldNameProperty = DependencyPropertyManager.Register("FieldName", typeof(string), ownerType, new PropertyMetadata(string.Empty,
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldName(true, true), d1, e1)));
			GrandTotalTextProperty = DependencyPropertyManager.Register("GrandTotalText", typeof(string), ownerType, new PropertyMetadata(string.Empty,
				(d, e) => ((PivotGridField)d).SyncFieldEmptyCellText(true, true)));
			GroupIntervalProperty = DependencyPropertyManager.Register("GroupInterval", typeof(FieldGroupInterval), ownerType,
				new PropertyMetadata(FieldGroupInterval.Default,
					(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldGroupInterval(true, true), d1, e1)));
			GroupIntervalNumericRangeProperty = DependencyPropertyManager.Register("GroupIntervalNumericRange", typeof(int), ownerType, new PropertyMetadata(10,
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldGroupIntervalNumericRange(true, true), d1, e1)));
			RunningTotalProperty = DependencyPropertyManager.Register("RunningTotal", typeof(bool), ownerType, new PropertyMetadata(false,
				(d, e) => ((PivotGridField)d).SyncFieldRunningTotal(true, true)));
			ShowNewValuesProperty = DependencyPropertyManager.Register("ShowNewValues", typeof(bool), ownerType, new PropertyMetadata(true,
				(d, e) => ((PivotGridField)d).SyncFieldShowNewValues(true, true)));
			SortOrderProperty = DependencyPropertyManager.Register("SortOrder", typeof(FieldSortOrder), ownerType,
				new PropertyMetadata(FieldSortOrder.Ascending,
					(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldSortOrder(true, true, (FieldSortOrder)e.NewValue), d1, e1)));
			SortModeProperty = DependencyPropertyManager.Register("SortMode", typeof(FieldSortMode), ownerType,
				new PropertyMetadata(FieldSortMode.Default,
					(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldSortMode(true, true, (FieldSortMode)e.NewValue), d1, e1)));
			SortByAttributeProperty = DependencyPropertyManager.Register("SortByAttribute", typeof(string), ownerType,
				new PropertyMetadata(null,
					(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldSortByAttribute(true, true, (string)e.NewValue), d1, e1)));
			AutoPopulatedPropertiesProperty = DependencyPropertyManager.Register("AutoPopulatedProperties", typeof(string[]), ownerType,
				new PropertyMetadata(null,
					(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldAutoPopulatedProperties(true, true, (string[])e.NewValue), d1, e1)));
			SummaryTypeProperty = DependencyPropertyManager.Register("SummaryType", typeof(FieldSummaryType), ownerType,
				new PropertyMetadata(FieldSummaryType.Sum,
					(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldSummary(true, true), d1, e1)));
			SummaryDisplayTypeProperty = DependencyPropertyManager.Register("SummaryDisplayType", typeof(FieldSummaryDisplayType), ownerType,
				new PropertyMetadata(FieldSummaryDisplayType.Default,
					(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldSummary(true, true), d1, e1)));
			TopValueCountProperty = DependencyPropertyManager.Register("TopValueCount", typeof(int), ownerType, new PropertyMetadata(0,
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldTopValueCount(true, true), d1, e1)));
			TopValueShowOthersProperty = DependencyPropertyManager.Register("TopValueShowOthers", typeof(bool), ownerType, new PropertyMetadata(false,
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldTopValueShowOthers(true, true), d1, e1)));
			TopValueTypeProperty = DependencyPropertyManager.Register("TopValueType", typeof(FieldTopValueType), ownerType,
				new PropertyMetadata(FieldTopValueType.Absolute,
					(d, e) => ((PivotGridField)d).SyncFieldTopValueType(true, true)));
			TopValueModeProperty =  DependencyPropertyManager.Register("TopValueMode", typeof(FieldTopValueMode), ownerType,
				new PropertyMetadata(FieldTopValueMode.Default,
					(d, e) => ((PivotGridField)d).SyncFieldTopValueMode(true, true)));
			TotalsVisibilityProperty = DependencyPropertyManager.Register("TotalsVisibility", typeof(FieldTotalsVisibility), ownerType,
				new PropertyMetadata(FieldTotalsVisibility.AutomaticTotals,
					(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldTotalsVisibility(true, true), d1, e1)));
			UnboundTypeProperty = DependencyPropertyManager.Register("UnboundType", typeof(FieldUnboundColumnType), ownerType,
				new PropertyMetadata(FieldUnboundColumnType.Bound,
					(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldUnboundType(true, true), d1, e1)));
			UnboundExpressionProperty = DependencyPropertyManager.Register("UnboundExpression", typeof(string), ownerType, new PropertyMetadata(string.Empty,
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldUnboundExpression(true, true), d1, e1)));
			UnboundFieldNameProperty = DependencyPropertyManager.Register("UnboundFieldName", typeof(string), ownerType, new PropertyMetadata(string.Empty,
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldUnboundName(true, true), d1, e1)));
			UseNativeFormatProperty = DependencyPropertyManager.Register("UseNativeFormat", typeof(bool?), ownerType, new PropertyMetadata(null,
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldUseNativeFormat(true, true), d1, e1)));
			UnboundExpressionModeProperty = DependencyPropertyManager.Register("UnboundExpressionMode", typeof(FieldUnboundExpressionMode), ownerType, new PropertyMetadata(FieldUnboundExpressionMode.Default,
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldFieldUnboundExpressionMode(), d1, e1)));			
			VisibleProperty = DependencyPropertyManager.Register("Visible", typeof(bool), ownerType, new PropertyMetadata(true,
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldVisible(true, true), d1, e1)));
			GroupProperty = DependencyPropertyManager.Register("Group", typeof(PivotGridGroup), ownerType, new PropertyMetadata(null,
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldGroup(true, true, (PivotGridGroup)e.NewValue, (PivotGridGroup)e.OldValue), d1, e1)));
			HasGroupPropertyKey = DependencyPropertyManager.RegisterReadOnly("HasGroup", typeof(bool), ownerType, new PropertyMetadata(false));
			HasGroupProperty = HasGroupPropertyKey.DependencyProperty;
			GroupIndexProperty = DependencyPropertyManager.Register("GroupIndex", typeof(int), ownerType, new PropertyMetadata(0,
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldGroupIndex(true, true), d1, e1)));
			AllowSortProperty = DependencyPropertyManager.Register("AllowSort", typeof(bool?),
				ownerType, new PropertyMetadata(null, OnAllowSortPropertyChanged));
			AllowFilterProperty = DependencyPropertyManager.Register("AllowFilter", typeof(bool?),
				ownerType, new PropertyMetadata(null, OnAllowFilterPropertyChanged));
			AllowDragProperty = DependencyPropertyManager.Register("AllowDrag", typeof(bool?),
				ownerType, new PropertyMetadata(null, OnAllowDragPropertyChanged));
			AllowDragInCustomizationFormProperty = DependencyPropertyManager.Register("AllowDragInCustomizationForm", typeof(bool?),
			   ownerType, new PropertyMetadata(null, OnAllowDragInCustomizationFormPropertyChanged));
			AllowExpandProperty = DependencyPropertyManager.Register("AllowExpand", typeof(bool?),
				ownerType, new PropertyMetadata(null, OnAllowExpandPropertyChanged));
			AllowSortBySummaryProperty = DependencyPropertyManager.Register("AllowSortBySummary", typeof(bool?),
				ownerType, new PropertyMetadata(null, OnAllowSortBySummaryPropertyChanged));
			AllowUnboundExpressionEditorProperty = DependencyPropertyManager.Register("AllowUnboundExpressionEditor", typeof(bool),
				ownerType, new PropertyMetadata(false, OnAllowUnboundExpressionEditorPropertyChanged));
			ShowInCustomizationFormProperty = DependencyPropertyManager.Register("ShowInCustomizationForm", typeof(bool),
				ownerType, new PropertyMetadata(true, OnShowInCustomizationFormPropertyChanged));
			ShowInExpressionEditorProperty = DependencyPropertyManager.Register("ShowInExpressionEditor", typeof(bool),
				ownerType, new PropertyMetadata(true, ShowInExpressionEditorPropertyChanged));
			ShowGrandTotalProperty = DependencyPropertyManager.Register("ShowGrandTotal", typeof(bool),
				ownerType, new PropertyMetadata(true, OnShowGrandTotalPropertyChanged));
			ShowTotalsProperty = DependencyPropertyManager.Register("ShowTotals", typeof(bool),
				ownerType, new PropertyMetadata(true, OnShowTotalsPropertyChanged));
			ShowValuesProperty = DependencyPropertyManager.Register("ShowValues", typeof(bool),
				ownerType, new PropertyMetadata(true, OnShowValuesPropertyChanged));
			ShowCustomTotalsProperty = DependencyPropertyManager.Register("ShowCustomTotals", typeof(bool),
				ownerType, new PropertyMetadata(true, OnShowCustomTotalsPropertyChanged));
			ShowSummaryTypeNameProperty = DependencyPropertyManager.Register("ShowSummaryTypeName", typeof(bool),
				ownerType, new PropertyMetadata(false, OnShowSummaryTypeNamePropertyChanged));
			HideEmptyVariationItemsProperty = DependencyPropertyManager.Register("HideEmptyVariationItems", typeof(bool),
				ownerType, new PropertyMetadata(false, OnHideEmptyVariationItemsPropertyChanged));
			OlapFilterUsingWhereClauseProperty = DependencyPropertyManager.Register("OlapFilterUsingWhereClause", typeof(FieldOLAPFilterUsingWhereClause),
				ownerType, new PropertyMetadata(FieldOLAPFilterUsingWhereClause.SingleValuesOnly, OnOlapFilterUsingWhereClausePropertyChanged));
			BestFitModeProperty = DependencyPropertyManager.Register("BestFitMode", typeof(BestFitMode),
				ownerType, new PropertyMetadata(BestFitMode.Default));
			BestFitAreaProperty = DependencyPropertyManager.Register("BestFitArea", typeof(FieldBestFitArea),
				ownerType, new PropertyMetadata(FieldBestFitArea.All));
			BestFitMaxRowCountProperty = DependencyPropertyManager.Register("BestFitMaxRowCount", typeof(int),
				ownerType, new PropertyMetadata(BestWidthCalculator.DefaultBestFitMaxRowCount));
			SortByFieldProperty = DependencyPropertyManager.Register("SortByField", typeof(PivotGridField), ownerType, new PropertyMetadata(null, 
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldSortByField(true, true), d1, e1)));
			SortByFieldNameProperty = DependencyPropertyManager.Register("SortByFieldName", typeof(string), ownerType, new PropertyMetadata(string.Empty, 
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldSortByFieldName(true, true), d1, e1)));
			SortBySummaryTypeProperty = DependencyPropertyManager.Register("SortBySummaryType", typeof(FieldSummaryType), ownerType, new PropertyMetadata(FieldSummaryType.Sum, 
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldSortBySummaryType(true, true), d1, e1)));
			SortByCustomTotalSummaryTypeProperty = DependencyPropertyManager.Register("SortByCustomTotalSummaryType", typeof(FieldSummaryType?), ownerType, new PropertyMetadata(null, 
				(d1, e1) => OnPropertyChanged((d, e) => ((PivotGridField)d).SyncFieldSortByCustomTotalSummaryType(true, true), d1, e1)));
			IsSortButtonVisiblePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("IsSortButtonVisible", typeof(bool),
				ownerType, new PropertyMetadata(false));
			IsSortButtonVisibleProperty = IsSortButtonVisiblePropertyKey.DependencyProperty;
			IsFilterButtonVisiblePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("IsFilterButtonVisible", typeof(bool),
				ownerType, new PropertyMetadata(false));
			IsFilterButtonVisibleProperty = IsFilterButtonVisiblePropertyKey.DependencyProperty;
			IsFilteredPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("IsFiltered", typeof(bool),
				ownerType, new PropertyMetadata(false));
			IsFilteredProperty = IsFilteredPropertyKey.DependencyProperty;
			DisplayTextPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("DisplayText", typeof(string),
				ownerType, new PropertyMetadata(string.Empty));
			DisplayTextProperty = DisplayTextPropertyKey.DependencyProperty;
			HeaderTemplateProperty = DependencyPropertyManager.Register("HeaderTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			HeaderTemplateSelectorProperty = DependencyPropertyManager.Register("HeaderTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			HeaderTreeViewTemplateProperty = DependencyPropertyManager.Register("HeaderTreeViewTemplate",
			   typeof(DataTemplate), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			HeaderTreeViewTemplateSelectorProperty = DependencyPropertyManager.Register("HeaderTreeViewTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			HeaderListTemplateProperty = DependencyPropertyManager.Register("HeaderListTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			HeaderListTemplateSelectorProperty = DependencyPropertyManager.Register("HeaderListTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			CellTemplateProperty = DependencyPropertyManager.Register("CellTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			CellTemplateSelectorProperty = DependencyPropertyManager.Register("CellTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			ValueTemplateProperty = DependencyPropertyManager.Register("ValueTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			ValueTemplateSelectorProperty = DependencyPropertyManager.Register("ValueTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			ActualHeaderTemplatePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualHeaderTemplate",
				typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			ActualHeaderTemplateProperty = ActualHeaderTemplatePropertyKey.DependencyProperty;
			ActualHeaderTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualHeaderTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualHeaderTemplateSelectorProperty = ActualHeaderTemplateSelectorPropertyKey.DependencyProperty;
			ActualHeaderTreeViewTemplatePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualHeaderTreeViewTemplate",
				typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			ActualHeaderTreeViewTemplateProperty = ActualHeaderTreeViewTemplatePropertyKey.DependencyProperty;
			ActualHeaderTreeViewTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualHeaderTreeViewTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualHeaderTreeViewTemplateSelectorProperty = ActualHeaderTreeViewTemplateSelectorPropertyKey.DependencyProperty;
			ActualHeaderListTemplatePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualHeaderListTemplate",
				typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			ActualHeaderListTemplateProperty = ActualHeaderListTemplatePropertyKey.DependencyProperty;
			ActualHeaderListTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualHeaderListTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualHeaderListTemplateSelectorProperty = ActualHeaderListTemplateSelectorPropertyKey.DependencyProperty;
			ActualCellTemplatePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualCellTemplate",
				typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			ActualCellTemplateProperty = ActualCellTemplatePropertyKey.DependencyProperty;
			ActualCellTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualCellTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualCellTemplateSelectorProperty = ActualCellTemplateSelectorPropertyKey.DependencyProperty;
			ActualValueTemplatePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualValueTemplate",
				typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			ActualValueTemplateProperty = ActualValueTemplatePropertyKey.DependencyProperty;
			ActualValueTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualValueTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualValueTemplateSelectorProperty = ActualValueTemplateSelectorPropertyKey.DependencyProperty;
			HeaderContentStyleProperty = DependencyPropertyManager.Register("HeaderContentStyle",
				typeof(Style), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			ActualHeaderContentStylePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualHeaderContentStyle",
				typeof(Style), ownerType, new FrameworkPropertyMetadata(null));
			ActualHeaderContentStyleProperty = ActualHeaderContentStylePropertyKey.DependencyProperty;
			PrintHeaderTemplateProperty = DependencyPropertyManager.Register("PrintHeaderTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			PrintHeaderTemplateSelectorProperty = DependencyPropertyManager.Register("PrintHeaderTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			PrintValueTemplateProperty = DependencyPropertyManager.Register("PrintValueTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			PrintValueTemplateSelectorProperty = DependencyPropertyManager.Register("PrintValueTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			PrintCellTemplateProperty = DependencyPropertyManager.Register("PrintCellTemplate",
				typeof(DataTemplate), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			PrintCellTemplateSelectorProperty = DependencyPropertyManager.Register("PrintCellTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new PropertyMetadata(OnAppearancePropertyChanged));
			ActualPrintHeaderTemplatePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualPrintHeaderTemplate",
				typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			ActualPrintHeaderTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualPrintHeaderTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualPrintValueTemplatePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualPrintValueTemplate",
				typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			ActualPrintValueTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualPrintValueTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualPrintCellTemplatePropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualPrintCellTemplate",
				typeof(DataTemplate), ownerType, new FrameworkPropertyMetadata(null));
			ActualPrintCellTemplateSelectorPropertyKey = DependencyPropertyManager.RegisterAttachedReadOnly("ActualPrintCellTemplateSelector",
				typeof(DataTemplateSelector), ownerType, new FrameworkPropertyMetadata(null));
			ActualPrintHeaderTemplateProperty = ActualPrintHeaderTemplatePropertyKey.DependencyProperty;
			ActualPrintHeaderTemplateSelectorProperty = ActualPrintHeaderTemplateSelectorPropertyKey.DependencyProperty;
			ActualPrintValueTemplateProperty = ActualPrintValueTemplatePropertyKey.DependencyProperty;
			ActualPrintValueTemplateSelectorProperty = ActualPrintValueTemplateSelectorPropertyKey.DependencyProperty;
			ActualPrintCellTemplateProperty = ActualPrintCellTemplatePropertyKey.DependencyProperty;
			ActualPrintCellTemplateSelectorProperty = ActualPrintCellTemplateSelectorPropertyKey.DependencyProperty;
			CellFormatProperty = DependencyPropertyManager.Register("CellFormat", typeof(string),
				ownerType, new PropertyMetadata(OnCellFormatPropertyChanged));
			GrandTotalCellFormatProperty = DependencyPropertyManager.Register("GrandTotalCellFormat", typeof(string),
				ownerType, new PropertyMetadata(OnGrandTotalCellFormatPropertyChanged));
			TotalCellFormatProperty = DependencyPropertyManager.Register("TotalCellFormat", typeof(string),
				ownerType, new PropertyMetadata(OnTotalCellFormatPropertyChanged));
			TotalValueFormatProperty = DependencyPropertyManager.Register("TotalValueFormat", typeof(string),
				ownerType, new PropertyMetadata(OnTotalValueFormatPropertyChanged));
			ValueFormatProperty = DependencyPropertyManager.Register("ValueFormat", typeof(string),
				ownerType, new PropertyMetadata(OnValueFormatPropertyChanged));
			DropDownFilterListSizeProperty = DependencyPropertyManager.Register("DropDownFilterListSize", typeof(Size),
				ownerType, new PropertyMetadata(DefaultDropDownFilterListSize));
			KpiGraphicProperty = DependencyPropertyManager.Register("KpiGraphic", typeof(PivotKpiGraphic), ownerType, new PropertyMetadata(PivotKpiGraphic.None, (d, e) => ((PivotGridField)d).SyncFieldKPIGraphic(true, true)));
			ActualKpiGraphicPropertyKey = DependencyPropertyManager.RegisterReadOnly("ActualKpiGraphic", typeof(PivotKpiGraphic), ownerType, new PropertyMetadata(PivotKpiGraphic.None, (d, e) => ((PivotGridField)d).OnActualKpiGraphicPropertyChanged(e)));
			ActualKpiGraphicProperty = ActualKpiGraphicPropertyKey.DependencyProperty;
			KpiTypePropertyKey = DependencyPropertyManager.RegisterReadOnly("KpiType", typeof(PivotKpiType), ownerType, new PropertyMetadata(PivotKpiType.None, (d, e) => ((PivotGridField)d).SyncFieldKPIGraphic(true, true)));
			KpiTypeProperty = KpiTypePropertyKey.DependencyProperty;
			ShowInPrefilterProperty = DependencyPropertyManager.Register("ShowInPrefilter", typeof(bool), ownerType, new PropertyMetadata(true, (d, e) => ((PivotGridField)d).InternalField.Options.ShowInPrefilter = (bool)e.NewValue));
		}
		static PivotGridInternalField GetField(DependencyObject d) {
			return ((PivotGridField)d).InternalField;
		}
		static void OnPropertyChanged(PropertyChangedCallback baseCallback, DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridField f = (PivotGridField)d;
			PivotGridControl pivot = f.PivotGrid;
			if(f.synchronizer.IsSynchronizing)
				return;
			PivotGridWpfData.DataRelatedDPChanged(pivot, baseCallback, d, e);
		}
		static void OnAppearancePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridField)d).UpdateAppearance();
		}
		double OnWidthChanging(double value) {
			if(object.Equals(value, double.NaN) || value >= Int32.MaxValue)
				value = PivotGrid == null ? DefaultWidth : PivotGrid.FieldWidth;
			if(value < MinWidth)
				return MinWidth;
			return value;
		}
		internal static object OnMinWidthChanging(DependencyObject d, object value) {
			if((double)value < PivotGridField.DefaultMinWidth)
				return Convert.ToDouble(PivotGridField.DefaultMinWidth);
			return value;
		}
		double OnHeightChanging(double value) {
			if(object.Equals(value, double.NaN) || value >= Int32.MaxValue)
				value = PivotGrid == null ? DefaultHeight : PivotGrid.FieldHeight;
			if(value < MinHeight)
				return MinHeight;
			return value;
		}
		internal static object OnMinHeightChanging(DependencyObject d, object value) {
			if((double)value < PivotGridField.DefaultMinHeight)
				return Convert.ToDouble(PivotGridField.DefaultMinHeight);
			return value;
		}
		static void OnAllowSortPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridField field = (PivotGridField)d;
			if(field.InternalField != null) {
				field.InternalField.Options.AllowSort = ((bool?)e.NewValue).ToDefaultBoolean();
				PivotGridInternalField internalField = field.GetInternalField();
				field.IsSortButtonVisible = internalField.Visible && internalField.ShowSortButton;
			}
		}
		static void OnAllowFilterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridField field = (PivotGridField)d;
			if(field.InternalField != null) {
				field.InternalField.Options.AllowFilter = ((bool?)e.NewValue).ToDefaultBoolean();
				field.UpdateIsFilterButtonVisible();
			}
		}
		internal void UpdateIsFilterButtonVisible() {
			IsFilterButtonVisible = InternalField.ShowFilterButton;
		}
		static void OnAllowDragPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridInternalField field = ((PivotGridField)d).InternalField;
			field.Options.AllowDrag = ((bool?)e.NewValue).ToDefaultBoolean();
		}
		static void OnAllowDragInCustomizationFormPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridInternalField field = ((PivotGridField)d).InternalField;
			field.Options.AllowDragInCustomizationForm = ((bool?)e.NewValue).ToDefaultBoolean();
		}
		static void OnAllowExpandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridInternalField field = ((PivotGridField)d).InternalField;
			field.Options.AllowExpand = ((bool?)e.NewValue).ToDefaultBoolean();
		}
		static void OnAllowSortBySummaryPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridInternalField field = ((PivotGridField)d).InternalField;
			field.Options.AllowSortBySummary = ((bool?)e.NewValue).ToDefaultBoolean();
		}
		static void OnAllowUnboundExpressionEditorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridInternalField field = ((PivotGridField)d).InternalField;
			field.Options.AllowUnboundExpressionEditor = (bool)e.NewValue;
		}
		static void OnShowInCustomizationFormPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridInternalField field = ((PivotGridField)d).InternalField;
			field.Options.ShowInCustomizationForm = (bool)e.NewValue;
		}
		static void ShowInExpressionEditorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridInternalField field = ((PivotGridField)d).InternalField;
			field.Options.ShowInExpressionEditor = (bool)e.NewValue;
		}		
		static void OnShowGrandTotalPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridInternalField field = ((PivotGridField)d).InternalField;
			field.Options.ShowGrandTotal = (bool)e.NewValue;
		}
		static void OnShowTotalsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridInternalField field = ((PivotGridField)d).InternalField;
			field.Options.ShowTotals = (bool)e.NewValue;
		}
		static void OnShowValuesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridInternalField field = ((PivotGridField)d).InternalField;
			field.Options.ShowValues = (bool)e.NewValue;
		}
		static void OnShowCustomTotalsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridInternalField field = ((PivotGridField)d).InternalField;
			field.Options.ShowCustomTotals = (bool)e.NewValue;
		}
		static void OnShowSummaryTypeNamePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridInternalField field = ((PivotGridField)d).InternalField;
			field.Options.ShowSummaryTypeName = (bool)e.NewValue;
		}
		static void OnHideEmptyVariationItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridInternalField field = ((PivotGridField)d).InternalField;
			field.Options.HideEmptyVariationItems = (bool)e.NewValue;
		}
		static void OnOlapFilterUsingWhereClausePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			PivotGridInternalField field = ((PivotGridField)d).InternalField;
			field.Options.OLAPFilterUsingWhereClause = ((FieldOLAPFilterUsingWhereClause)e.NewValue).ToPivotOLAPFilterUsingWhereClause();
		}
		static void OnCellFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridField)d).OnCellFormatChanged((string)e.NewValue);
		}
		static void OnGrandTotalCellFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridField)d).OnGrandTotalCellFormatChanged((string)e.NewValue);
		}
		static void OnTotalCellFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridField)d).OnTotalCellFormatChanged((string)e.NewValue);
		}
		static void OnTotalValueFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridField)d).OnTotalValueFormatChanged((string)e.NewValue);
		}
		static void OnValueFormatPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridField)d).OnValueFormatChanged((string)e.NewValue);
		}
		#endregion
		static Size DefaultDropDownFilterListSize = new Size(210, 250);
		public const int DefaultHeight = 21;
		public const int DefaultMinHeight = 10;
		public const int DefaultWidth = 100;
		public const int DefaultMinWidth = 20;
		PivotGridInternalField internalField;
		PivotGridFieldCollection collection;
		FilterInfoBase filterInfo;
		SortByConditionCollection sortByConditions;
		PivotGridCustomTotalCollection customTotals;
		string serializedSortByField;
		Locker deserializationLocker;
		[IgnoreDependencyPropertiesConsistencyChecker]
		internal static readonly DependencyProperty NameProxyProperty = DependencyPropertyManager.Register("NameProxy", typeof(string), typeof(PivotGridField), new PropertyMetadata(string.Empty, OnNameProxyChanged));
		static void OnNameProxyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e) {
			((PivotGridField)d).OnNameChanged();
		}
		public PivotGridField()
			: this(new PivotGridInternalField(), false) { }
		protected internal PivotGridField(PivotGridInternalField internalField, bool syncProperties) {
			this.deserializationLocker = new Locker();
			this.synchronizer = new PropertiesSynchronizer();
			if(internalField == null)
				throw new ArgumentNullException("internalField");
			this.internalField = internalField;
			this.internalField.Wrapper = this;
			this.internalField.FieldChanged += OnInternalFieldChanged;
			InitSortByConditions();
			if(!syncProperties)
				synchronizer.IsSynchronizing = true;
			UpdateWidthProperty();
			UpdateHeightProperty();
			if(!syncProperties)
				synchronizer.IsSynchronizing = false;
			if(syncProperties)
				SyncFieldAll(true, false);
			this.customTotals = new PivotGridCustomTotalCollection(this);
		}
		void InitSortByConditions(SortByConditionCollection conditions = null) {
			if(conditions == null)
				conditions = new SortByConditionCollection(null, this);
			if(conditions != sortByConditions) {
				if(sortByConditions != null)
					sortByConditions.CollectionChanged -= OnSortByConditionsChanged;
				sortByConditions = conditions;
				sortByConditions.CollectionChanged += OnSortByConditionsChanged;
			}
		}
		public PivotGridField(string fieldName, FieldArea area)
			: this() {
			FieldName = fieldName;
			Area = area;
		}
		public static implicit operator PivotGridFieldBase(PivotGridField field) {
			return field.InternalField;
		}
		protected PivotFieldItemCollection FieldItems {
			get { return Data.FieldItems; }
		}
		protected IPivotFieldSyncPropertyOwner ReadFieldSyncPropertyOwner {
			get { return  WriteFieldSyncPropertyOwner; }
		}
		protected IPivotFieldSyncPropertyOwner WriteFieldSyncPropertyOwner {
			get { return InternalField as IPivotFieldSyncPropertyOwner; }
		}
		protected internal PivotFieldItem FieldItem {
			get { return (PivotFieldItem)(Data.GetFieldItem(InternalField)); }
		}
		protected internal PivotGridInternalField InternalField {
			get { return internalField; }
		}
		protected internal PivotGridFieldCollection Collection {
			get { return collection; }
			set {
				if(collection == value)
					return;
				collection = value;
				OnCollectionChanged();
			}
		}
		protected internal virtual PivotGridWpfData Data { get { return Collection != null ? Collection.Data : internalField.WpfData; } }
		protected internal PivotGridControl PivotGrid { get { return Data != null ? Data.PivotGrid : null; } }
		protected bool IsDeserializing { get { return deserializationLocker.IsLocked; } }
		internal FilterInfoBase FilterInfo {
			get {
				if(filterInfo == null)
					filterInfo = CreateFilterInfo();
				return filterInfo;
			}
		}
		protected virtual FilterInfoBase CreateFilterInfo() {
			return new CheckedListFilterInfo(this);
		}
		[
		Browsable(false), Category(Categories.Data),
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldDataType")
#else
	Description("")
#endif
		]
		public Type DataType {
			get { return InternalField.DataType; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldName"),
#endif
		Category(Categories.Layout),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public new string Name {
			get { return base.Name; }
			set { base.Name = value; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldWidth"),
#endif
		Category(Categories.Layout),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public
#if SL
		new 
#endif
 double Width {
			get { return (double)GetValue(WidthProperty); }
			set { SetValue(WidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldMinWidth"),
#endif
		Category(Categories.Layout),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public
#if SL
		new 
#endif
 double MinWidth {
			get { return (double)GetValue(MinWidthProperty); }
			set { SetValue(MinWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldHeight"),
#endif
		Category(Categories.Layout),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public
#if SL
		new 
#endif
 double Height {
			get { return (double)GetValue(HeightProperty); }
			set { SetValue(HeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldMinHeight"),
#endif
		Category(Categories.Layout),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public
#if SL
		new 
#endif
 double MinHeight {
			get { return (double)GetValue(MinHeightProperty); }
			set { SetValue(MinHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldAllowedAreas"),
#endif
		Category(Categories.Behaviour),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public FieldAllowedAreas AllowedAreas {
			get { return (FieldAllowedAreas)GetValue(AllowedAreasProperty); }
			set { SetValue(AllowedAreasProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldArea"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public FieldArea Area {
			get { return (FieldArea)GetValue(AreaProperty); }
			set { SetValue(AreaProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldAreaIndex"),
#endif
		Category(Categories.Behaviour),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public int AreaIndex {
			get { return (int)GetValue(AreaIndexProperty); }
			set { SetValue(AreaIndexProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldCaption"),
#endif
		Category(Categories.Appearance),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID),
		]
		public string Caption {
			get { return (string)GetValue(CaptionProperty); }
			set { SetValue(CaptionProperty, value); }
		}
		[
		Category(Categories.Appearance),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID),
		]
		public string DisplayFolder {
			get { return (string)GetValue(DisplayFolderProperty); }
			set { SetValue(DisplayFolderProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldExpandedInFieldsGroup"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public bool ExpandedInFieldsGroup {
			get { return (bool)GetValue(ExpandedInFieldsGroupProperty); }
			set { SetValue(ExpandedInFieldsGroupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldGroupIndex"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public int GroupIndex {
			get { return (int)GetValue(GroupIndexProperty); }
			set { SetValue(GroupIndexProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldEmptyCellText"),
#endif
		Category(Categories.Appearance),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string EmptyCellText {
			get { return (string)GetValue(EmptyCellTextProperty); }
			set { SetValue(EmptyCellTextProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldEmptyValueText"),
#endif
		Category(Categories.Appearance),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string EmptyValueText {
			get { return (string)GetValue(EmptyValueTextProperty); }
			set { SetValue(EmptyValueTextProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldFieldName"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string FieldName {
			get { return (string)GetValue(FieldNameProperty); }
			set { SetValue(FieldNameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldGrandTotalText"),
#endif
		Category(Categories.Appearance),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string GrandTotalText {
			get { return (string)GetValue(GrandTotalTextProperty); }
			set { SetValue(GrandTotalTextProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldGroup"),
#endif
		Category(Categories.Behaviour)
		]
		public PivotGridGroup Group {
			get { return (PivotGridGroup)GetValue(GroupProperty); }
			set { SetValue(GroupProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldHasGroup"),
#endif
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never)
		]
		public bool HasGroup {
			get { return (bool)GetValue(HasGroupProperty); }
			private set { this.SetValue(HasGroupPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldGroupInterval"),
#endif
		Category(Categories.Behaviour),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public FieldGroupInterval GroupInterval {
			get { return (FieldGroupInterval)GetValue(GroupIntervalProperty); }
			set { SetValue(GroupIntervalProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldGroupIntervalNumericRange"),
#endif
		Category(Categories.Behaviour),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public int GroupIntervalNumericRange {
			get { return (int)GetValue(GroupIntervalNumericRangeProperty); }
			set { SetValue(GroupIntervalNumericRangeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldRunningTotal"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool RunningTotal {
			get { return (bool)GetValue(RunningTotalProperty); }
			set { SetValue(RunningTotalProperty, value); }
		}
		[Category(Categories.Data)]
		public bool ShowNewValues {
			get { return (bool)GetValue(ShowNewValuesProperty); }
			set { SetValue(ShowNewValuesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldSortMode"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public FieldSortMode SortMode {
			get { return (FieldSortMode)GetValue(SortModeProperty); }
			set { SetValue(SortModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldSortOrder"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public FieldSortOrder SortOrder {
			get { return (FieldSortOrder)GetValue(SortOrderProperty); }
			set { SetValue(SortOrderProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldSummaryType"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public FieldSummaryType SummaryType {
			get { return (FieldSummaryType)GetValue(SummaryTypeProperty); }
			set { SetValue(SummaryTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldSummaryDisplayType"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public FieldSummaryDisplayType SummaryDisplayType {
			get { return (FieldSummaryDisplayType)GetValue(SummaryDisplayTypeProperty); }
			set { SetValue(SummaryDisplayTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldTopValueCount"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public int TopValueCount {
			get { return (int)GetValue(TopValueCountProperty); }
			set { SetValue(TopValueCountProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldTopValueShowOthers"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool TopValueShowOthers {
			get { return (bool)GetValue(TopValueShowOthersProperty); }
			set { SetValue(TopValueShowOthersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldTopValueType"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public FieldTopValueType TopValueType {
			get { return (FieldTopValueType)GetValue(TopValueTypeProperty); }
			set { SetValue(TopValueTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldTopValueMode"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public FieldTopValueMode TopValueMode {
			get { return (FieldTopValueMode)GetValue(TopValueModeProperty); }
			set { SetValue(TopValueModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldTotalsVisibility"),
#endif
		Category(Categories.Behaviour),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public FieldTotalsVisibility TotalsVisibility {
			get { return (FieldTotalsVisibility)GetValue(TotalsVisibilityProperty); }
			set { SetValue(TotalsVisibilityProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldUnboundType"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public FieldUnboundColumnType UnboundType {
			get { return (FieldUnboundColumnType)GetValue(UnboundTypeProperty); }
			set { SetValue(UnboundTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldUnboundExpression"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string UnboundExpression {
			get { return (string)GetValue(UnboundExpressionProperty); }
			set { SetValue(UnboundExpressionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldUnboundFieldName"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string UnboundFieldName {
			get { return (string)GetValue(UnboundFieldNameProperty); }
			set { SetValue(UnboundFieldNameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldUnboundExpressionMode"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public FieldUnboundExpressionMode UnboundExpressionMode {
			get { return (FieldUnboundExpressionMode)GetValue(UnboundExpressionModeProperty); }
			set { SetValue(UnboundExpressionModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldUseNativeFormat"),
#endif
		Category(Categories.Data), TypeConverter(typeof(NullableBoolConverter)),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.PrintSettingsID),
		]
		public bool? UseNativeFormat {
			get { return (bool?)GetValue(UseNativeFormatProperty); }
			set { SetValue(UseNativeFormatProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldVisible"),
#endif
		Category(Categories.Behaviour),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public bool Visible {
			get { return (bool)GetValue(VisibleProperty); }
			set { SetValue(VisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldSortByField"),
#endif
		Category(Categories.Behaviour)
		]
		public PivotGridField SortByField {
			get { return (PivotGridField)GetValue(SortByFieldProperty); }
			set { SetValue(SortByFieldProperty, value); }
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DefaultValue(null),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string SerializedSortByField {
			get {
				if(!string.IsNullOrEmpty(serializedSortByField))
					return serializedSortByField;
				return SortByField != null ? SortByField.Name : null;
			}
			set { serializedSortByField = value; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldSortByFieldName"),
#endif
		Category(Categories.Behaviour),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string SortByFieldName {
			get { return (string)GetValue(SortByFieldNameProperty); }
			set { SetValue(SortByFieldNameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldSortBySummaryType"),
#endif
		Category(Categories.Behaviour),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public FieldSummaryType SortBySummaryType {
			get { return (FieldSummaryType)GetValue(SortBySummaryTypeProperty); }
			set { SetValue(SortBySummaryTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldSortByCustomTotalSummaryType"),
#endif
		Category(Categories.Behaviour),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public FieldSummaryType? SortByCustomTotalSummaryType {
			get { return (FieldSummaryType?)GetValue(SortByCustomTotalSummaryTypeProperty); }
			set { SetValue(SortByCustomTotalSummaryTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldSortByAttribute"),
#endif
		Category(Categories.Behaviour),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string SortByAttribute {
			get { return (string)GetValue(SortByAttributeProperty); }
			set { SetValue(SortByAttributeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldAutoPopulatedProperties"),
#endif
		Category(Categories.Behaviour),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string[] AutoPopulatedProperties {
			get { return (string[])GetValue(AutoPopulatedPropertiesProperty); }
			set { SetValue(AutoPopulatedPropertiesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldSortByConditions"),
#endif
		Category(Categories.Behaviour),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, true, false, true),
		XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public SortByConditionCollection SortByConditions {
			get { return sortByConditions; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanDrag { get { return InternalField.CanDrag; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanDragInCustomizationForm { get { return InternalField.CanDragInCustomizationForm; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanHide { get { return InternalField.CanHide; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanSortBySummary { get { return InternalField.CanSortBySummary; } }
		internal virtual bool CanShowUnboundExpressionMenu { get { return InternalField.IsUnbound && AllowUnboundExpressionEditor; } }
		internal virtual bool IsOlapSortModeNone { get { return InternalField.IsOLAPSortModeNone; } }
		internal virtual bool IsOlapSorted { get { return InternalField.IsOLAPSorted; } }
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldAllowSort"),
#endif
		Category(Categories.OptionsCustomization), TypeConverter(typeof(NullableBoolConverter)),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool? AllowSort {
			get { return (bool?)GetValue(AllowSortProperty); }
			set { SetValue(AllowSortProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldAllowFilter"),
#endif
		Category(Categories.OptionsCustomization), TypeConverter(typeof(NullableBoolConverter)),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool? AllowFilter {
			get { return (bool?)GetValue(AllowFilterProperty); }
			set { SetValue(AllowFilterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldOlapUseNonEmpty"),
#endif
		Category(Categories.OptionsData),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool OlapUseNonEmpty {
			get { return (bool)GetValue(OlapUseNonEmptyProperty); }
			set { SetValue(OlapUseNonEmptyProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldOlapFilterByUniqueName"),
#endif
		Category(Categories.OptionsData), TypeConverter(typeof(NullableBoolConverter)),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool? OlapFilterByUniqueName {
			get { return (bool?)GetValue(OlapFilterByUniqueNameProperty); }
			set { SetValue(OlapFilterByUniqueNameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldAllowDrag"),
#endif
		Category(Categories.OptionsCustomization), TypeConverter(typeof(NullableBoolConverter)),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool? AllowDrag {
			get { return (bool?)GetValue(AllowDragProperty); }
			set { SetValue(AllowDragProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldAllowDragInCustomizationForm"),
#endif
		Category(Categories.OptionsCustomization), TypeConverter(typeof(NullableBoolConverter)),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool? AllowDragInCustomizationForm {
			get { return (bool?)GetValue(AllowDragInCustomizationFormProperty); }
			set { SetValue(AllowDragInCustomizationFormProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldAllowExpand"),
#endif
		Category(Categories.OptionsCustomization), TypeConverter(typeof(NullableBoolConverter)),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool? AllowExpand {
			get { return (bool?)GetValue(AllowExpandProperty); }
			set { SetValue(AllowExpandProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldAllowSortBySummary"),
#endif
		Category(Categories.OptionsCustomization), TypeConverter(typeof(NullableBoolConverter)),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool? AllowSortBySummary {
			get { return (bool?)GetValue(AllowSortBySummaryProperty); }
			set { SetValue(AllowSortBySummaryProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldAllowUnboundExpressionEditor"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool AllowUnboundExpressionEditor {
			get { return (bool)GetValue(AllowUnboundExpressionEditorProperty); }
			set { SetValue(AllowUnboundExpressionEditorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldShowInCustomizationForm"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public bool ShowInCustomizationForm {
			get { return (bool)GetValue(ShowInCustomizationFormProperty); }
			set { SetValue(ShowInCustomizationFormProperty, value); }
		}
		[
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public bool ShowInExpressionEditor {
			get { return (bool)GetValue(ShowInExpressionEditorProperty); }
			set { SetValue(ShowInExpressionEditorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldShowGrandTotal"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowGrandTotal {
			get { return (bool)GetValue(ShowGrandTotalProperty); }
			set { SetValue(ShowGrandTotalProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldShowTotals"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowTotals {
			get { return (bool)GetValue(ShowTotalsProperty); }
			set { SetValue(ShowTotalsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldShowValues"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowValues {
			get { return (bool)GetValue(ShowValuesProperty); }
			set { SetValue(ShowValuesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldShowCustomTotals"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowCustomTotals {
			get { return (bool)GetValue(ShowCustomTotalsProperty); }
			set { SetValue(ShowCustomTotalsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldShowSummaryTypeName"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowSummaryTypeName {
			get { return (bool)GetValue(ShowSummaryTypeNameProperty); }
			set { SetValue(ShowSummaryTypeNameProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldHideEmptyVariationItems"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool HideEmptyVariationItems {
			get { return (bool)GetValue(HideEmptyVariationItemsProperty); }
			set { SetValue(HideEmptyVariationItemsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldOlapFilterUsingWhereClause"),
#endif
		Category(Categories.OptionsBehavior),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public FieldOLAPFilterUsingWhereClause OlapFilterUsingWhereClause {
			get { return (FieldOLAPFilterUsingWhereClause)GetValue(OlapFilterUsingWhereClauseProperty); }
			set { SetValue(OlapFilterUsingWhereClauseProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldBestFitMode"),
#endif
		Category(Categories.OptionsBestFit),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public BestFitMode BestFitMode {
			get { return (BestFitMode)GetValue(BestFitModeProperty); }
			set { SetValue(BestFitModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldBestFitArea"),
#endif
		Category(Categories.OptionsBestFit),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public FieldBestFitArea BestFitArea {
			get { return (FieldBestFitArea)GetValue(BestFitAreaProperty); }
			set { SetValue(BestFitAreaProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldBestFitMaxRowCount"),
#endif
		Category(Categories.OptionsBestFit),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public int BestFitMaxRowCount {
			get { return (int)GetValue(BestFitMaxRowCountProperty); }
			set { SetValue(BestFitMaxRowCountProperty, value); }
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldIsSortButtonVisible")
#else
	Description("")
#endif
		]
		public bool IsSortButtonVisible {
			get { return (bool)GetValue(IsSortButtonVisibleProperty); }
			private set { this.SetValue(IsSortButtonVisiblePropertyKey, value); }
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldIsFilterButtonVisible")
#else
	Description("")
#endif
		]
		public bool IsFilterButtonVisible {
			get { return (bool)GetValue(IsFilterButtonVisibleProperty); }
			private set { this.SetValue(IsFilterButtonVisiblePropertyKey, value); }
		}
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldIsFiltered")
#else
	Description("")
#endif
		]
		public bool IsFiltered {
			get { return (bool)GetValue(IsFilteredProperty); }
			private set { this.SetValue(IsFilteredPropertyKey, value); }
		}
		[
		Browsable(false),
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldDisplayText")
#else
	Description("")
#endif
		]
		public string DisplayText {
			get { return (string)GetValue(DisplayTextProperty); }
			private set { this.SetValue(DisplayTextPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldHeaderTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate HeaderTemplate {
			get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
			set { SetValue(HeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldHeaderTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector HeaderTemplateSelector {
			get { return (DataTemplateSelector)base.GetValue(HeaderTemplateSelectorProperty); }
			set { SetValue(HeaderTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldHeaderTreeViewTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate HeaderTreeViewTemplate {
			get { return (DataTemplate)GetValue(HeaderTreeViewTemplateProperty); }
			set { SetValue(HeaderTreeViewTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldHeaderTreeViewTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector HeaderTreeViewTemplateSelector {
			get { return (DataTemplateSelector)base.GetValue(HeaderTreeViewTemplateSelectorProperty); }
			set { SetValue(HeaderTreeViewTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldHeaderListTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate HeaderListTemplate {
			get { return (DataTemplate)GetValue(HeaderListTemplateProperty); }
			set { SetValue(HeaderListTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldHeaderListTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector HeaderListTemplateSelector {
			get { return (DataTemplateSelector)base.GetValue(HeaderListTemplateSelectorProperty); }
			set { SetValue(HeaderListTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldCellTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate CellTemplate {
			get { return (DataTemplate)GetValue(CellTemplateProperty); }
			set { SetValue(CellTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldCellTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector CellTemplateSelector {
			get { return (DataTemplateSelector)base.GetValue(CellTemplateSelectorProperty); }
			set { SetValue(CellTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldValueTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate ValueTemplate {
			get { return (DataTemplate)GetValue(ValueTemplateProperty); }
			set { SetValue(ValueTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldValueTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector ValueTemplateSelector {
			get { return (DataTemplateSelector)base.GetValue(ValueTemplateSelectorProperty); }
			set { SetValue(ValueTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualHeaderTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate ActualHeaderTemplate {
			get { return (DataTemplate)GetValue(ActualHeaderTemplateProperty); }
			private set { this.SetValue(ActualHeaderTemplatePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualHeaderTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector ActualHeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualHeaderTemplateSelectorProperty); }
			private set { this.SetValue(ActualHeaderTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualHeaderTreeViewTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate ActualHeaderTreeViewTemplate {
			get { return (DataTemplate)GetValue(ActualHeaderTreeViewTemplateProperty); }
			private set { this.SetValue(ActualHeaderTreeViewTemplatePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualHeaderTreeViewTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector ActualHeaderTreeViewTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualHeaderTreeViewTemplateSelectorProperty); }
			private set { this.SetValue(ActualHeaderTreeViewTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualHeaderListTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate ActualHeaderListTemplate {
			get { return (DataTemplate)GetValue(ActualHeaderListTemplateProperty); }
			private set { this.SetValue(ActualHeaderListTemplatePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualHeaderListTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector ActualHeaderListTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualHeaderListTemplateSelectorProperty); }
			private set { this.SetValue(ActualHeaderListTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualCellTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate ActualCellTemplate {
			get { return (DataTemplate)GetValue(ActualCellTemplateProperty); }
			private set { this.SetValue(ActualCellTemplatePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualCellTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector ActualCellTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualCellTemplateSelectorProperty); }
			private set { this.SetValue(ActualCellTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualValueTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate ActualValueTemplate {
			get { return (DataTemplate)GetValue(ActualValueTemplateProperty); }
			private set { this.SetValue(ActualValueTemplatePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualValueTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector ActualValueTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualValueTemplateSelectorProperty); }
			private set { this.SetValue(ActualValueTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldPrintHeaderTemplate"),
#endif
		Category(Categories.PrintTemplates)
		]
		public DataTemplate PrintHeaderTemplate {
			get { return (DataTemplate)GetValue(PrintHeaderTemplateProperty); }
			set { SetValue(PrintHeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldPrintHeaderTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.PrintTemplates)
		]
		public DataTemplateSelector PrintHeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PrintHeaderTemplateSelectorProperty); }
			set { SetValue(PrintHeaderTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldPrintValueTemplate"),
#endif
		Category(Categories.PrintTemplates)
		]
		public DataTemplate PrintValueTemplate {
			get { return (DataTemplate)GetValue(PrintValueTemplateProperty); }
			set { SetValue(PrintValueTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldPrintValueTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.PrintTemplates)
		]
		public DataTemplateSelector PrintValueTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PrintValueTemplateSelectorProperty); }
			set { SetValue(PrintValueTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldPrintCellTemplate"),
#endif
		Category(Categories.PrintTemplates)
		]
		public DataTemplate PrintCellTemplate {
			get { return (DataTemplate)GetValue(PrintCellTemplateProperty); }
			set { SetValue(PrintCellTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldPrintCellTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.PrintTemplates)
		]
		public DataTemplateSelector PrintCellTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PrintCellTemplateSelectorProperty); }
			set { SetValue(PrintCellTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualPrintHeaderTemplate"),
#endif
		Category(Categories.PrintTemplates)
		]
		public DataTemplate ActualPrintHeaderTemplate {
			get { return (DataTemplate)GetValue(ActualPrintHeaderTemplateProperty); }
			private set { this.SetValue(ActualPrintHeaderTemplatePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualPrintHeaderTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.PrintTemplates)
		]
		public DataTemplateSelector ActualPrintHeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualPrintHeaderTemplateSelectorProperty); }
			private set { this.SetValue(ActualPrintHeaderTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualPrintValueTemplate"),
#endif
		Category(Categories.PrintTemplates)
		]
		public DataTemplate ActualPrintValueTemplate {
			get { return (DataTemplate)GetValue(ActualPrintValueTemplateProperty); }
			private set { this.SetValue(ActualPrintValueTemplatePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualPrintValueTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.PrintTemplates)
		]
		public DataTemplateSelector ActualPrintValueTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualPrintValueTemplateSelectorProperty); }
			private set { this.SetValue(ActualPrintValueTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualPrintCellTemplate"),
#endif
		Category(Categories.PrintTemplates)
		]
		public DataTemplate ActualPrintCellTemplate {
			get { return (DataTemplate)GetValue(ActualPrintCellTemplateProperty); }
			private set { this.SetValue(ActualPrintCellTemplatePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualPrintCellTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.PrintTemplates)
		]
		public DataTemplateSelector ActualPrintCellTemplateSelector {
			get { return (DataTemplateSelector)GetValue(ActualPrintCellTemplateSelectorProperty); }
			private set { this.SetValue(ActualPrintCellTemplateSelectorPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldHeaderContentStyle"),
#endif
		Category(Categories.Appearance)
		]
		public Style HeaderContentStyle {
			get { return (Style)GetValue(HeaderContentStyleProperty); }
			set { SetValue(HeaderContentStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldActualHeaderContentStyle"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Appearance)
		]
		public Style ActualHeaderContentStyle {
			get { return (Style)GetValue(ActualHeaderContentStyleProperty); }
			private set { this.SetValue(ActualHeaderContentStylePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldCellFormat"),
#endif
		Category(Categories.Appearance),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string CellFormat {
			get { return (string)GetValue(CellFormatProperty); }
			set { SetValue(CellFormatProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldGrandTotalCellFormat"),
#endif
		Category(Categories.Appearance),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string GrandTotalCellFormat {
			get { return (string)GetValue(GrandTotalCellFormatProperty); }
			set { SetValue(GrandTotalCellFormatProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldTotalCellFormat"),
#endif
		Category(Categories.Appearance),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string TotalCellFormat {
			get { return (string)GetValue(TotalCellFormatProperty); }
			set { SetValue(TotalCellFormatProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldTotalValueFormat"),
#endif
		Category(Categories.Appearance),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string TotalValueFormat {
			get { return (string)GetValue(TotalValueFormatProperty); }
			set { SetValue(TotalValueFormatProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldValueFormat"),
#endif
		Category(Categories.Appearance),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string ValueFormat {
			get { return (string)GetValue(ValueFormatProperty); }
			set { SetValue(ValueFormatProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldFilterValues"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false), XtraSerializableProperty(XtraSerializationVisibility.Content),
		XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID)
		]
		public FieldFilterValues FilterValues {
			get { return InternalField.FilterValues; }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldDropDownFilterListSize")
#else
	Description("")
#endif
		]
		public Size DropDownFilterListSize {
			get { return (Size)GetValue(DropDownFilterListSizeProperty); }
			set { SetValue(DropDownFilterListSizeProperty, value); }
		}
		[
		 XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID)
		]
		public PivotKpiGraphic KpiGraphic {
			get { return (PivotKpiGraphic)GetValue(KpiGraphicProperty); }
			set { SetValue(KpiGraphicProperty, value); }
		}
		public PivotKpiGraphic ActualKpiGraphic {
			get { return (PivotKpiGraphic)GetValue(ActualKpiGraphicProperty); }
			private set { this.SetValue(ActualKpiGraphicPropertyKey, value); }
		}
		public PivotKpiType KpiType {
			get { return (PivotKpiType)GetValue(KpiTypeProperty); }
			internal set { this.SetValue(KpiTypePropertyKey, value); }
		}
		public bool ShowInPrefilter {
			get { return (bool)GetValue(ShowInPrefilterProperty); }
			set { SetValue(ShowInPrefilterProperty, value); }
		}
		[
		Browsable(false),
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldExpressionFieldName")
#else
	Description("")
#endif
		]
		public string ExpressionFieldName {
			get { return InternalField.ExpressionFieldName; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldPrefilterColumnName")
#else
	Description("")
#endif
		]
		public string PrefilterColumnName {
			get { return InternalField.PrefilterColumnName; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldOlapDrillDownColumnName")
#else
	Description("")
#endif
		]
		public string OlapDrillDownColumnName {
			get { return InternalField.OLAPDrillDownColumnName; }
		}
		internal string Hierarchy { get { return InternalField.Hierarchy; } }
		internal Size ActualDropDownFilterListSize {
			get {
				return DropDownFilterListSize.IsEmpty ? DefaultDropDownFilterListSize : DropDownFilterListSize;
			}
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridFieldCustomTotals")]
#endif
		public PivotGridCustomTotalCollection CustomTotals {
			get { return customTotals; }
		}
		[
		 XtraSerializableProperty(),
		 XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID)
		]
		public new object Tag {
			get { return GetValue(TagProperty); }
			set { SetValue(TagProperty, value); }
		}
		public void CollapseAll() {
			InternalField.CollapseAll();
		}
		public void ExpandAll() {
			InternalField.ExpandAll();
		}
		public void CollapseValue(object value) {
			InternalField.CollapseValue(value);
		}
		public void ExpandValue(object value) {
			InternalField.ExpandValue(value);
		}
		public bool IsAreaAllowed(FieldArea area) {
			return InternalField.IsAreaAllowed(area.ToPivotArea());
		}
		void SetValueSafe(DependencyProperty dp, object value) {
			if(object.Equals(GetValue(dp), value))
				return;
			SetValue(dp, value);
		}
		#region SyncProperties
		PropertiesSynchronizer synchronizer;
		void SyncProperty(bool read, bool write, Action writeProperties, Action readProperties) {
			SyncProperty(read, write, writeProperties, readProperties, false);
		}
		void SyncProperty(bool read, bool write, Action writeProperties, Action readProperties, bool force) {
			if(!force && IsDeserializing)
				return;
			synchronizer.SyncProperty(Data, read, write, writeProperties, readProperties);
		}
		void SyncFieldOLAPUseNonEmpty(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.OLAPUseNonEmpty = OlapUseNonEmpty, null, true);
		}
		void SyncFieldOLAPFilterByUniqueName(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.OLAPFilterByUniqueName = OlapFilterByUniqueName, null, true);
		}
		void SyncFieldAllowedAreas(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.AllowedAreas = AllowedAreas, null, true);
		}
		void SyncFieldArea(bool read, bool write) {
			SyncFieldArea(read, write, null, null);
		}
		void SyncFieldArea(bool read, bool write, FieldArea? newValue, FieldArea? oldValue) {
			SyncProperty(read, write,
				() => WriteFieldSyncPropertyOwner.Area = (newValue == null) ? Area : newValue.Value,
				() => {
					SetValueSafe(AreaProperty, ReadFieldSyncPropertyOwner.Area);
					SetValueSafe(AreaIndexProperty, ReadFieldSyncPropertyOwner.AreaIndex);
					ReadFromFieldButtonsState();
					if(PivotGrid != null && (Area == FieldArea.DataArea || oldValue == FieldArea.DataArea))
						PivotGrid.UpdatePrefilterPanel();
				});
		}
		internal void SyncFieldAreaIndex(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.AreaIndex = AreaIndex, () => {
				SetValueSafe(AreaProperty, ReadFieldSyncPropertyOwner.Area);
				SetValueSafe(AreaIndexProperty, ReadFieldSyncPropertyOwner.AreaIndex);
				SetValueSafe(VisibleProperty, ReadFieldSyncPropertyOwner.Visible);
				ReadFromFieldButtonsState();
				UpdateIsFiltered();
			});
		}
		void SyncFieldCaption(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.Caption = Caption, () => {
				SetValueSafe(CaptionProperty, ReadFieldSyncPropertyOwner.Caption);
				DisplayText = ReadFieldSyncPropertyOwner.HeaderDisplayText;
				if(PivotGrid != null && !object.ReferenceEquals(null, PivotGrid.PrefilterCriteria))
					PivotGrid.UpdatePrefilterPanel();
			});
		}
		void SyncFieldDisplayFolder(bool read, bool write) {
			SyncProperty(read, write, () => {
				if(WriteFieldSyncPropertyOwner.DisplayFolder != DisplayFolder)
					WriteFieldSyncPropertyOwner.DisplayFolder = DisplayFolder;
			}, () => {
				SetValueSafe(DisplayFolderProperty, ReadFieldSyncPropertyOwner.DisplayFolder);
				if(Data != null && PivotGrid != null)
					PivotGrid.CoerceValue(PivotGridControl.GroupFieldsInFieldListProperty);
			});
		}
		void SyncFieldDisplayText(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.Caption = Caption, () => {
				DisplayText = ReadFieldSyncPropertyOwner.HeaderDisplayText;
			});
		}
		void SyncFieldEmptyCellText(bool read, bool write) {
			SyncProperty(read, write, () => {
				WriteFieldSyncPropertyOwner.EmptyCellText = EmptyCellText;
				WriteFieldSyncPropertyOwner.GrandTotalText = GrandTotalText;
			}, null);
		}
		void SyncFieldEmptyValueText(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.EmptyValueText = EmptyValueText, null);
		}
		void SyncFieldFiltered(bool read, bool write) {
			SyncProperty(read, write, null, () => {
				ReadFromFieldButtonsState();
				UpdateIsFiltered();
			});
		}
		internal void UpdateIsFiltered() {
			IsFiltered = (InternalField.Visible || Data != null && !Data.OptionsData.FilterByVisibleFieldsOnly) && GetIsFiltered();
		}
		protected internal bool GetIsFiltered() {
			return ((InternalField.Group == null || InternalField.GroupFilterMode == XtraPivotGrid.PivotGroupFilterMode.List) && InternalField.FilterValues.HasFilter)
				|| Group != null && InternalField.GroupFilterMode == XtraPivotGrid.PivotGroupFilterMode.Tree && Group.FilterValues.HasFilter && Group.FirstField == this;
		}
		void SyncFieldName(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.FieldName = FieldName, () => {
				SetValueSafe(AreaProperty, ReadFieldSyncPropertyOwner.Area);
				SetValueSafe(AreaIndexProperty, ReadFieldSyncPropertyOwner.AreaIndex);
				SetValueSafe(FieldNameProperty, ReadFieldSyncPropertyOwner.FieldName);
				DisplayText = ReadFieldSyncPropertyOwner.HeaderDisplayText;
			});
		}
		protected virtual void SyncFieldHeight(bool read, bool write) {
			SyncProperty(read, write,
				() => WriteFieldSyncPropertyOwner.Height = Convert.ToInt32(OnHeightChanging(Height)),
				() => {
					double newHeight = Convert.ToDouble(ReadFieldSyncPropertyOwner.Height);
					SetValueSafe(HeightProperty, newHeight);
					if(InternalField.IsDataField && PivotGrid != null && PivotGrid.DataFieldHeight != newHeight)
						PivotGrid.DataFieldHeight = newHeight;
				});
		}
		protected virtual void SyncFieldWidth(bool read, bool write) {
			SyncProperty(read, write,
				() => WriteFieldSyncPropertyOwner.Width = Convert.ToInt32(OnWidthChanging(Width)),
				() => {
					double newWidth = Convert.ToDouble(ReadFieldSyncPropertyOwner.Width);
					SetValueSafe(WidthProperty, newWidth);
					if(InternalField.IsDataField && PivotGrid != null && PivotGrid.DataFieldWidth != newWidth)
						PivotGrid.DataFieldWidth = newWidth;
				});
		}
		void SyncFieldMinWidth(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.MinWidth = Convert.ToInt32(MinWidth), null);
		}
		void SyncFieldKPIGraphic(bool read, bool write) {
			SyncProperty(read, write, null, () => ActualKpiGraphic = GetActualKpiGraphic());
		}
		PivotKpiGraphic GetActualKpiGraphic() {
			if(Data != null && Data.IsOLAP && !KpiType.NeedGraphic() || (Data == null || !Data.IsOLAP) && KpiGraphic == PivotKpiGraphic.ServerDefined)
				return PivotKpiGraphic.None;
			if(KpiGraphic == PivotKpiGraphic.ServerDefined)
				return Data.GetKPIGraphic(this.InternalField).ToKpiGraphic();
			return KpiGraphic;
		}
		void SyncFieldMinHeight(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.MinHeight = Convert.ToInt32(MinHeight), null);
		}
		void SyncFieldSortMode(bool read, bool write) {
			SyncFieldSortMode(read, write, null);
		}
		void SyncFieldSortMode(bool read, bool write, FieldSortMode? newValue) {
			SyncProperty(read, write,
				() => WriteFieldSyncPropertyOwner.SortMode = (newValue == null) ? SortMode : newValue.Value,
				() => {
					SetValueSafe(SortModeProperty, ReadFieldSyncPropertyOwner.SortMode);
					ReadFromFieldButtonsState();
				});
		}
		void SyncFieldSortOrder(bool read, bool write) {
			SyncFieldSortOrder(read, write, null);
		}
		void SyncFieldSortOrder(bool read, bool write, FieldSortOrder? newValue) {
			SyncProperty(read, write,
				() => WriteFieldSyncPropertyOwner.SortOrder = (newValue == null) ? SortOrder : newValue.Value,
				() => {
					SetValueSafe(SortOrderProperty, ReadFieldSyncPropertyOwner.SortOrder);
					ReadFromFieldButtonsState();
				});
		}
		void SyncFieldSortByAttribute(bool read, bool write, string newValue) {
			SyncProperty(read, write,
					 () => WriteFieldSyncPropertyOwner.SortByAttribute = newValue,
					 () => {
						 SetValueSafe(SortByAttributeProperty, ReadFieldSyncPropertyOwner.SortByAttribute);
					 });
		}
		void SyncFieldAutoPopulatedProperties(bool read, bool write, string[] newValue) {
			SyncProperty(read, write,
					 () => WriteFieldSyncPropertyOwner.AutoPopulatedProperties = newValue,
					 () => {
						 SetValueSafe(AutoPopulatedPropertiesProperty, ReadFieldSyncPropertyOwner.AutoPopulatedProperties);
					 });
		}
		void SyncFieldSummary(bool read, bool write) {
			SyncProperty(read, write, () => {
				WriteFieldSyncPropertyOwner.SummaryType = SummaryType;
				WriteFieldSyncPropertyOwner.SummaryDisplayType = SummaryDisplayType;
			}, () => DisplayText = ReadFieldSyncPropertyOwner.HeaderDisplayText);
		}
		internal void SyncFieldVisible(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.Visible = Visible, () => {
				SetValueSafe(AreaIndexProperty, ReadFieldSyncPropertyOwner.AreaIndex);
				SetValueSafe(VisibleProperty, ReadFieldSyncPropertyOwner.Visible);
				ReadFromFieldButtonsState();
				UpdateIsFiltered();
			});
		}
		void SyncFieldUnboundName(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.UnboundFieldName = UnboundFieldName, () => {
				SetValueSafe(UnboundFieldNameProperty, ReadFieldSyncPropertyOwner.UnboundFieldName);
				SetValueSafe(UnboundTypeProperty, ReadFieldSyncPropertyOwner.UnboundType);
			});
		}
		void SyncFieldUnboundType(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.UnboundType = UnboundType, () => {
				SetValueSafe(UnboundTypeProperty, ReadFieldSyncPropertyOwner.UnboundType);
				SetValueSafe(UnboundFieldNameProperty, ReadFieldSyncPropertyOwner.UnboundFieldName);
			});
		}
		void SyncFieldExpandedInFieldsGroup(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.ExpandedInFieldsGroup = ExpandedInFieldsGroup, () => {
				SetValueSafe(ExpandedInFieldsGroupProperty, ReadFieldSyncPropertyOwner.ExpandedInFieldsGroup);
				if(Group != null)
					Group.OnFieldExpandedChanged(this);
			}, true);
		}
		void SyncFieldTopValueCount(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.TopValueCount = TopValueCount, null, true);
		}
		void SyncFieldTopValueShowOthers(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.TopValueShowOthers = TopValueShowOthers, null, true);
		}
		void SyncFieldTopValueType(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.TopValueType = TopValueType, null, true);
		}
		void SyncFieldTopValueMode(bool read, bool write) {
			SyncProperty(read, write, () => InternalField.TopValueMode = TopValueMode.ToPivotTopValueType(), null, true);
		}
		void SyncFieldTotalsVisibility(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.TotalsVisibility = TotalsVisibility, null, true);
		}
		void SyncFieldUnboundExpression(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.UnboundExpression = UnboundExpression, null, true);
		}
		void SyncFieldUseNativeFormat(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.UseNativeFormat = UseNativeFormat, null, true);
		}
		void SyncFieldFieldUnboundExpressionMode() {
			SyncProperty(true, true, () => InternalField.UnboundExpressionMode = UnboundExpressionMode.ToDataFieldUnboundExpressionMode(), null, true);
		}
		internal void SyncFieldGroup(bool read, bool write, PivotGridGroup newValue, PivotGridGroup oldValue) {
			SyncProperty(read, write, null, () => {
				SetValueSafe(GroupProperty, newValue);
				if(oldValue != null)
					oldValue.Remove(this);
				if(newValue != null)
					newValue.Add(this);
				UpdateIsFiltered();
				HasGroup = newValue != null;
			}, true);
		}
		void SyncFieldGroupIndex(bool read, bool write) {
			SyncProperty(read, write, null, () => {
				if(Group != null)
					Group.UpdateFieldsOrder();
			}, true);
		}
		void SyncFieldGroupInterval(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.GroupInterval = GroupInterval, () => UnboundFieldName = ReadFieldSyncPropertyOwner.UnboundFieldName, true);
		}
		void SyncFieldGroupIntervalNumericRange(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.GroupIntervalNumericRange = GroupIntervalNumericRange, null, true);
		}
		void SyncFieldRunningTotal(bool read, bool write) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.RunningTotal = RunningTotal, null, true);
		}
		internal void SyncFieldShowNewValues(bool read, bool write) {
			SyncProperty(read, write,
			   () => WriteFieldSyncPropertyOwner.ShowNewValues = ShowNewValues,
			   () => SetValueSafe(ShowNewValuesProperty, ReadFieldSyncPropertyOwner.ShowNewValues), true);
		}
		void SyncFieldActualSortMode(bool read, bool write, FieldSortMode newValue) {
			SyncProperty(read, write, () => WriteFieldSyncPropertyOwner.ActualSortMode = newValue, null);
		}
		void SyncFieldSortByField(bool read, bool write) {
			SyncProperty(read, write,
			   () => WriteFieldSyncPropertyOwner.SortByField = SortByField,
			   () => SetValueSafe(SortByFieldProperty, ReadFieldSyncPropertyOwner.SortByField), true);
		}
		void SyncFieldSortByFieldName(bool read, bool write) {
			SyncProperty(read, write,
			   () => WriteFieldSyncPropertyOwner.SortByFieldName = SortByFieldName,
			   () => SetValueSafe(SortByFieldNameProperty, ReadFieldSyncPropertyOwner.SortByFieldName), true);
		}
		void SyncFieldSortBySummaryType(bool read, bool write) {
			SyncProperty(read, write,
			   () => WriteFieldSyncPropertyOwner.SortBySummaryType = SortBySummaryType,
			   () => SetValueSafe(SortBySummaryTypeProperty, ReadFieldSyncPropertyOwner.SortBySummaryType), true);
		}
		void SyncFieldSortByCustomTotalSummaryType(bool read, bool write) {
			SyncProperty(read, write,
			   () => WriteFieldSyncPropertyOwner.SortByCustomTotalSummaryType = SortByCustomTotalSummaryType,
			   () => SetValueSafe(SortByCustomTotalSummaryTypeProperty, ReadFieldSyncPropertyOwner.SortByCustomTotalSummaryType), true);
		}
		void SyncFieldSortByConditions(bool read, bool write) {
			SyncProperty(read, write,
			   () => { 
				   SortByConditions.Owner = PivotGrid;
				   WriteFieldSyncPropertyOwner.SortByConditions = SortByConditions;
			   },
			   () => {
				   InitSortByConditions(ReadFieldSyncPropertyOwner.SortByConditions);
			   }, true);
		}
		internal void SyncOLAPProperties(bool read, bool write) {
			SyncFieldCaption(read, write);
			SyncFieldDisplayFolder(read, write);
			SyncProperty(read, write, null,
			   () => {
				   KpiType = internalField.KPIType.ToKpiType();
				   if(KpiType.NeedGraphic() && KpiGraphic == PivotKpiGraphic.None)
					   KpiGraphic = PivotKpiGraphic.ServerDefined;
			   }, true);
			SyncFieldKPIGraphic(read, write);
		}
		internal void SyncFieldAll(bool read, bool write) {
			foreach(FieldSyncProperty property in Helpers.GetEnumValues(typeof(FieldSyncProperty))) {
				OnInternalFieldChanged(null, new FieldSyncPropertyEventArgs(property, read, write));
			}
		}
		protected internal virtual void OnInternalFieldChanged(object sender, FieldSyncPropertyEventArgs e) {
			if(!e.SyncProperty.HasValue)
				return;
			switch(e.SyncProperty) {
				case FieldSyncProperty.Area:
					SyncFieldArea(e.Read, e.Write);
					break;
				case FieldSyncProperty.AreaIndex:
					SyncFieldAreaIndex(e.Read, e.Write);
					break;
				case FieldSyncProperty.Caption:
					SyncFieldCaption(e.Read, e.Write);
					break;
				case FieldSyncProperty.DisplayFolder:
					SyncFieldDisplayFolder(e.Read, e.Write);
					break;
				case FieldSyncProperty.EmptyCellText:
					SyncFieldEmptyCellText(e.Read, e.Write);
					break;
				case FieldSyncProperty.EmptyValueText:
					SyncFieldEmptyValueText(e.Read, e.Write);
					break;
				case FieldSyncProperty.ExpandedInFieldsGroup:
					SyncFieldExpandedInFieldsGroup(e.Read, e.Write);
					break;
				case FieldSyncProperty.FieldName:
					SyncFieldName(e.Read, e.Write);
					break;
				case FieldSyncProperty.Filtered:
					SyncFieldFiltered(e.Read, e.Write);
					break;
				case FieldSyncProperty.Height:
					SyncFieldHeight(e.Read, e.Write);
					break;
				case FieldSyncProperty.MinHeight:
					SyncFieldMinHeight(e.Read, e.Write);
					break;
				case FieldSyncProperty.MinWidth:
					SyncFieldMinWidth(e.Read, e.Write);
					break;
				case FieldSyncProperty.SortBy:
					SyncFieldSortByField(e.Read, e.Write);
					SyncFieldSortByFieldName(e.Read, e.Write);
					SyncFieldSortBySummaryType(e.Read, e.Write);
					SyncFieldSortByCustomTotalSummaryType(e.Read, e.Write);
					SyncFieldSortByConditions(e.Read, e.Write);
					break;
				case FieldSyncProperty.SortMode:
					SyncFieldSortMode(e.Read, e.Write);
					break;
				case FieldSyncProperty.SortOrder:
					SyncFieldSortOrder(e.Read, e.Write);
					break;
				case FieldSyncProperty.Summary:
					SyncFieldSummary(e.Read, e.Write);
					break;
				case FieldSyncProperty.UnboundName:
					SyncFieldUnboundName(e.Read, e.Write);
					break;
				case FieldSyncProperty.UnboundType:
					SyncFieldUnboundType(e.Read, e.Write);
					break;
				case FieldSyncProperty.Visible:
					SyncFieldVisible(e.Read, e.Write);
					break;
				case FieldSyncProperty.Width:
					SyncFieldWidth(e.Read, e.Write);
					break;
				case FieldSyncProperty.DisplayText:
					SyncFieldDisplayText(e.Read, e.Write);
					break;
				case FieldSyncProperty.KPIGraphic:
					SyncFieldKPIGraphic(e.Read, e.Write);
					break;
				default:
					throw new ArgumentException("Incorrect synchronizing property");
			}
		}
		#endregion
		protected virtual void OnCollectionChanged() {
			UpdateAppearance();
			UpdateSortBySummaryInfo();
			UpdateWidthProperty();
			UpdateHeightProperty();
		}
		internal void UpdateWidthProperty() {
			this.CoerceValue(PivotGridField.WidthProperty);
		}
		internal void UpdateHeightProperty() {
			this.CoerceValue(PivotGridField.HeightProperty);
		}
		protected virtual void OnSortByConditionsChanged(object sender, NotifyCollectionChangedEventArgs e) {
			SyncFieldSortByConditions(false, true);
		}
		protected internal virtual void OnInitialized() {
			UpdateSortBySummaryInfo();
		}
		protected void UpdateSortBySummaryInfo() {
			SyncFieldSortByField(false, true);
			SyncFieldSortByFieldName(false, true);
			SyncFieldSortByCustomTotalSummaryType(false, true);
			SyncFieldSortByConditions(false, true);
		}
#if !SL
		protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			base.OnPropertyChanged(e);
			PropertyChange(e);
		}
#else
		#region IDependencyPropertyChangeListener Members
		void IDependencyPropertyChangeListener.OnPropertyAssigning(DependencyProperty dp, object value) { }
		void IDependencyPropertyChangeListener.OnPropertyChanged(DependencyPropertyChangedEventArgs e) {
			PropertyChange(e);
		}
		#endregion
#endif
		void PropertyChange(DependencyPropertyChangedEventArgs e) {
			if(PivotGrid != null)
				PivotGrid.RaisePropertyChanged(e, this);
		}
		void OnNameChanged() {
			InternalField.Name = Name;
		}
		protected virtual bool OnFormatChanged(FormatInfo formatInfo, ref string format) {
			formatInfo.FormatString = format;
			bool changed = formatInfo.FormatString != format;
			if(changed)
				format = formatInfo.FormatString;
			formatInfo.FormatType = string.IsNullOrEmpty(format) ? FormatType.None : FormatType.Custom;
			return changed;
		}
		protected virtual void OnCellFormatChanged(string newValue) {
			if(OnFormatChanged(InternalField.CellFormat, ref newValue))
				CellFormat = newValue;
		}
		protected virtual void OnGrandTotalCellFormatChanged(string newValue) {
			if(OnFormatChanged(InternalField.GrandTotalCellFormat, ref newValue))
				GrandTotalCellFormat = newValue;
		}
		protected virtual void OnTotalCellFormatChanged(string newValue) {
			if(OnFormatChanged(InternalField.TotalCellFormat, ref newValue))
				TotalCellFormat = newValue;
		}
		protected virtual void OnTotalValueFormatChanged(string newValue) {
			if(OnFormatChanged(InternalField.TotalValueFormat, ref newValue))
				TotalValueFormat = newValue;
		}
		protected virtual void OnValueFormatChanged(string newValue) {
			if(OnFormatChanged(InternalField.ValueFormat, ref newValue))
				ValueFormat = newValue;
		}
		void OnActualKpiGraphicPropertyChanged(DependencyPropertyChangedEventArgs e) {
			if(((PivotKpiGraphic)e.NewValue) == PivotKpiGraphic.None || ((PivotKpiGraphic)e.OldValue) == PivotKpiGraphic.None)
				UpdateAppearance();
		}
		internal void ReadFromFieldButtonsState() {
			if(Data == null)
				return; 
			IsSortButtonVisible = InternalField.Visible && InternalField.ShowSortButton;
			IsFilterButtonVisible = InternalField.ShowFilterButton;
		}
		internal void UpdateAppearance() {
			if(PivotGrid != null) {
				ActualHeaderTemplate = SelectTemplate(HeaderTemplate, PivotGrid.FieldHeaderTemplate, HeaderTemplateSelector);
				ActualHeaderTemplateSelector = HeaderTemplateSelector ?? PivotGrid.FieldHeaderTemplateSelector;
				ActualHeaderTreeViewTemplate = SelectTemplate(HeaderTreeViewTemplate, PivotGrid.FieldHeaderTreeViewTemplate, HeaderTreeViewTemplateSelector);
				ActualHeaderTreeViewTemplateSelector = HeaderTreeViewTemplateSelector ?? PivotGrid.FieldHeaderTreeViewTemplateSelector;
				ActualHeaderListTemplate = SelectTemplate(HeaderListTemplate, PivotGrid.FieldHeaderListTemplate, HeaderListTemplateSelector);
				ActualHeaderListTemplateSelector = HeaderListTemplateSelector ?? PivotGrid.FieldHeaderListTemplateSelector;
				ActualCellTemplate = SelectTemplate(CellTemplate, (ActualKpiGraphic == PivotKpiGraphic.None ? PivotGrid.FieldCellTemplate : PivotGrid.FieldCellKpiTemplate), CellTemplateSelector);
				ActualCellTemplateSelector = CellTemplateSelector ?? (ActualKpiGraphic == PivotKpiGraphic.None ? PivotGrid.FieldCellTemplateSelector : PivotGrid.FieldCellKpiTemplateSelector);
				ActualValueTemplate = SelectTemplate(ValueTemplate, PivotGrid.FieldValueTemplate, ValueTemplateSelector);
				ActualValueTemplateSelector = ValueTemplateSelector ?? PivotGrid.FieldValueTemplateSelector;
				ActualPrintCellTemplate = SelectTemplate(PrintCellTemplate, (ActualKpiGraphic == PivotKpiGraphic.None ? PivotGrid.PrintFieldCellTemplate : PivotGrid.PrintFieldCellKpiTemplate), PrintCellTemplateSelector);
				ActualPrintCellTemplateSelector = PrintCellTemplateSelector ?? (ActualKpiGraphic == PivotKpiGraphic.None ? PivotGrid.PrintFieldCellTemplateSelector : PivotGrid.PrintFieldCellKpiTemplateSelector);
				ActualPrintValueTemplate = SelectTemplate(PrintValueTemplate, PivotGrid.PrintFieldValueTemplate, PrintValueTemplateSelector);
				ActualPrintValueTemplateSelector = PrintValueTemplateSelector ?? PivotGrid.PrintFieldValueTemplateSelector;
				ActualPrintHeaderTemplate = SelectTemplate(PrintHeaderTemplate, PivotGrid.PrintFieldHeaderTemplate, PrintHeaderTemplateSelector);
				ActualPrintHeaderTemplateSelector = PrintHeaderTemplateSelector ?? PivotGrid.PrintFieldHeaderTemplateSelector;
				ActualHeaderContentStyle = HeaderContentStyle ?? PivotGrid.FieldHeaderContentStyle;
			} else {
				ActualHeaderTemplate = HeaderTemplate;
				ActualHeaderTemplateSelector = HeaderTemplateSelector;
				ActualHeaderTreeViewTemplate = HeaderTreeViewTemplate;
				ActualHeaderTreeViewTemplateSelector = HeaderTreeViewTemplateSelector;
				ActualHeaderListTemplate = HeaderListTemplate;
				ActualHeaderListTemplateSelector = HeaderListTemplateSelector;
				ActualCellTemplate = CellTemplate;
				ActualCellTemplateSelector = CellTemplateSelector;
				ActualValueTemplate = ValueTemplate;
				ActualValueTemplateSelector = ValueTemplateSelector;
				ActualPrintCellTemplate = PrintCellTemplate;
				ActualPrintCellTemplateSelector = PrintCellTemplateSelector;
				ActualPrintValueTemplate = PrintValueTemplate;
				ActualPrintValueTemplateSelector = PrintValueTemplateSelector;
				ActualPrintHeaderTemplate = PrintHeaderTemplate;
				ActualPrintHeaderTemplateSelector = PrintHeaderTemplateSelector;
				ActualHeaderContentStyle = HeaderContentStyle;
			}
		}
		DataTemplate SelectTemplate(DataTemplate headerTemplate, DataTemplate pivotTemplate, DataTemplateSelector headerTemplateSelector) {
			return headerTemplate == null && headerTemplateSelector == null ? pivotTemplate : headerTemplate;
		}
		public void SetAreaPosition(FieldArea area, int areaIndex) {
			InternalField.SetAreaPosition(area.ToPivotArea(), areaIndex);
		}
		protected internal void Hide() {
			if(InternalField.CanHide)
				InternalField.Visible = false;
		}
		public void ChangeSortOrder() {
			InternalField.ChangeSortOrder();
		}
		public void ResetSortBySummary() {
			SortByField = null;
			SortByFieldName = null;
			SortByConditions.Clear();
		}
		public object[] GetUniqueValues() {
			return InternalField.GetUniqueValues();
		}
		public object[] GetAvailableValues() {
			return InternalField.GetAvailableValues();
		}
		public void GetAvailableValuesAsync(AsyncCompletedHandler asyncCompleted) {
			InternalField.GetAvailableValuesAsync(asyncCompleted.ToCoreAsyncCompleted());
		}
		public List<object> GetVisibleValues() {
			return InternalField.GetVisibleValues();
		}
		public string GetValueText(object value) {
			return InternalField.GetValueText(value);
	   }
	   public string GetValueText(IOLAPMember member) {
			return InternalField.GetValueText(member);
	   }
	   public string GetDisplayText(object value) {
		   return InternalField.GetDisplayText(value);
	   }
#if !SL
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("The GetOLAPMembers method is obsolete now. Use the GetOlapMembers method instead.")]
		public IOLAPMember[] GetOLAPMembers() {
			return internalField.GetOLAPMembers();
		}
		public IOLAPMember[] GetOlapMembers() {
			return internalField.GetOLAPMembers();
		}
#endif
		List<OlapPropertyDescriptor> olapMemberProperties = null;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("The GetOLAPMemberProperties method is obsolete now. Use the GetOlapMemberProperties method instead.")]
		public List<OlapPropertyDescriptor> GetOLAPMemberProperties() {
			return GetOlapMemberProperties();
		}
		public List<OlapPropertyDescriptor> GetOlapMemberProperties() {
			if(olapMemberProperties != null || Data == null)
				return olapMemberProperties;
			Dictionary<string, DevExpress.PivotGrid.OLAP.OLAPDataType> types = Data.GetOlapMemberProperties(FieldName);
			if(types == null)
				return null;
			olapMemberProperties = new List<OlapPropertyDescriptor>();
			foreach(KeyValuePair<string, DevExpress.PivotGrid.OLAP.OLAPDataType> pair in types)
				olapMemberProperties.Add(new OlapPropertyDescriptor(pair.Key, FieldName, DevExpress.PivotGrid.OLAP.OLAPDataTypeConverter.Convert(pair.Value)));
			return olapMemberProperties;
		}
		public string GetOlapDefaultSortProperty() {
			return InternalField.GetOlapDefaultSortProperty();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Obsolete("The GetOLAPMembersAsync method is obsolete now. Use the GetOlapMembersAsync method instead.")]
		public void GetOLAPMembersAsync(AsyncCompletedHandler asyncCompleted) {
			internalField.GetOLAPMembersAsync(asyncCompleted.ToCoreAsyncCompleted());
		}
		public void GetOlapMembersAsync(AsyncCompletedHandler asyncCompleted) {
			internalField.GetOLAPMembersAsync(asyncCompleted.ToCoreAsyncCompleted());
		}
		protected internal PivotFilterItemsBase CreateFilterItems(bool deferUpdates) {
			return CreateFilterItemsCore(true, deferUpdates);
		}
		internal PivotFilterItemsBase CreateFilterItemsCore(bool init, bool deferUpdates) {
			PivotFilterItemsBase res = null;
			bool reset = false;
			if(Group == null || PivotGrid.GroupFilterMode == GroupFilterMode.List) {
				if(deferUpdates)
					res = Data.FieldListFields.GetFieldFilter(InternalField);
				if(res == null) {
					res = new PivotGridFilterItems(Data, InternalField, false, PivotGrid.ShowOnlyAvailableFilterItems, deferUpdates);
					reset = true;
				}
			} else {
				if(deferUpdates)
					res = Data.FieldListFields.GetGroupFilter(InternalField);
				if(res == null) {
					res = new PivotGroupFilterItems(Data, InternalField);
					reset = true;
				}
			}
			if(init)
				if(reset)
					res.CreateItems();
				else
					res.EnsureAvailableItems();
			return res;
		}
		protected internal virtual FilterColumn CreateFilterColumn() {
			return new PivotGridFilterColumn(this);
		}
		 class PivotGridFilterColumn : FilterColumn {
			 readonly PivotGridField field;
			 BaseEditSettings editSettings;
			 public override BaseEditSettings EditSettings {
				 get {
					 EnsureEditSettings();
					 return editSettings;
				 }
				 set { editSettings = value; }
			 }
			 public PivotGridFilterColumn(PivotGridField field) {
				this.field = field;
				FieldName = field.PrefilterColumnName;
				ColumnCaption = field.DisplayText;
				ColumnType = field.InternalField.DataType;
			 }
			 void EnsureEditSettings() {
				 if(editSettings != null)
					 return;
				 editSettings = field.CreateFilterEditSettings();
			 }
		}
		protected virtual ComboBoxEditSettings CreateFilterEditSettings() {
			ComboBoxEditSettings editSettings = new ComboBoxEditSettings();
			object[] values = GetUniqueValues();
			FilterItem[] filterItems = new FilterItem[values.Length];
			for(int i = 0; i < values.Length; i++)
				filterItems[i] = new FilterItem(values[i], InternalField.GetDisplayText(values[i]));
			editSettings.ItemsSource = filterItems;
			editSettings.AutoComplete = true;
			if(DataType == typeof(Enum) || InternalField.IsUnbound && GroupInterval == FieldGroupInterval.DateDayOfWeek) {
				editSettings.ValueMember = "Value";
				editSettings.DisplayMember = "DisplayText";
			}
			return editSettings;
		}
		#region ILogicalOwner Members
		double ILogicalOwner.ActualHeight { get { return Height; } }
		double ILogicalOwner.ActualWidth { get { return Width; } }
		internal List<object> logicalChildren = new List<object>();
		void ILogicalOwner.AddChild(object child) {
			AddLogicalChild(child);
			logicalChildren.Add(child);
		}
		void ILogicalOwner.RemoveChild(object child) {
			logicalChildren.Remove(child);
			RemoveLogicalChild(child);
		}
#if !SL
		protected override System.Collections.IEnumerator LogicalChildren {
			get { return logicalChildren.GetEnumerator(); }
		}
#else
		void AddLogicalChild(object child) { }
		void RemoveLogicalChild(object child) { }
		event KeyboardFocusChangedEventHandler IInputElement.PreviewGotKeyboardFocus { add { } remove { } }
		event KeyboardFocusChangedEventHandler IInputElement.PreviewLostKeyboardFocus { add { } remove { } }
		bool ILogicalOwner.IsLoaded {
			get {
				return PivotGrid != null && PivotGrid.IsLoaded;
			}
		}
#endif
		#endregion
		#region IXtraSerializable2 Members
		protected internal virtual void OnStartDeserializing() {
			deserializationLocker.Lock();
		}
		protected internal virtual void OnEndDeserializing() {
			deserializationLocker.Unlock();
			UpdateWidthProperty();
			UpdateHeightProperty();
			SortByField = Data.Fields.GetFieldByName(SerializedSortByField);
			SerializedSortByField = null;
			foreach(SortByCondition condition in SortByConditions) {
				condition.Field = Data.Fields.GetFieldByName(condition.SerializedField);
				condition.SerializedField = null;
			}
			SyncFieldAll(false, true);
			SyncFieldAll(true, false);
		}
		#endregion
		public override string ToString() {
			return DisplayText;
		}
		protected internal bool CanSortAscending {
			get { return IsOlapSortModeNone; }
		}
		protected internal bool CanSortDescending {
			get { return IsOlapSortModeNone; }
		}
		protected internal bool CanClearSorting {
			get { return InternalField.ActualSortMode != PivotSortMode.None; }
		}
		#region IThreadSafeField
		FieldArea IThreadSafeField.Area { get { return InternalField.Area.ToFieldArea(); } }
		int IThreadSafeField.AreaIndex { get { return InternalField.AreaIndex; } }
		bool IThreadSafeField.Visible { get { return InternalField.Visible; } }
		string IThreadSafeField.Name { get { return InternalField.Name; } }
		string IThreadSafeField.FieldName { get { return InternalField.FieldName; } }
		string IThreadSafeField.UnboundFieldName { get { return InternalField.UnboundFieldName; } }
		string IThreadSafeField.PrefilterColumnName { get { return InternalField.PrefilterColumnName; } }
		string IThreadSafeField.Caption { get { return InternalField.Caption; } }
		string IThreadSafeField.DisplayText { get { return InternalField.HeaderDisplayText; } }
		string IThreadSafeField.DisplayFolder { get { return InternalField.DisplayFolder; } }
		FieldSummaryType IThreadSafeField.SummaryType { get { return InternalField.SummaryType.ToFieldSummaryType(); } }
		FieldSortOrder IThreadSafeField.SortOrder { get { return InternalField.SortOrder.ToFieldSortOrder(); } }
		FieldSortMode IThreadSafeField.SortMode { get { return InternalField.SortMode.ToFieldSortMode(); } }
		FieldFilterValues IThreadSafeField.FilterValues { get { return InternalField.FilterValues; } }
		FieldAllowedAreas IThreadSafeField.AllowedAreas { get { return InternalField.AllowedAreas.ToFieldAllowedAreas(); } }
		PivotGridGroup IThreadSafeField.Group { get { return InternalField.Group.GetWrapper(); } }
		int IThreadSafeField.GroupIndex { get { return InternalField.GroupIndex; } }
		bool IThreadSafeField.TopValueShowOthers { get { return InternalField.TopValueShowOthers; } }
		int IThreadSafeField.TopValueCount { get { return InternalField.TopValueCount; } }
		bool IThreadSafeField.RunningTotal { get { return InternalField.RunningTotal; } }
		bool IThreadSafeField.ShowNewValues { get { return InternalField.ShowNewValues; } }
		FieldTopValueType IThreadSafeField.TopValueType { get { return InternalField.TopValueType.ToFieldTopValueType(); } }
		FieldGroupInterval IThreadSafeField.GroupInterval { get { return InternalField.GroupInterval.ToFieldGroupInterval(); } }
		int IThreadSafeField.GroupIntervalNumericRange { get { return InternalField.GroupIntervalNumericRange; } }
		string IThreadSafeField.UnboundExpression { get { return InternalField.UnboundExpression; } }
		bool IThreadSafeField.ExpandedInFieldsGroup { get { return InternalField.ExpandedInFieldsGroup; } }
		FieldUnboundColumnType IThreadSafeField.UnboundType { get { return InternalField.UnboundType.ToFieldUnboundColumnType(); } }
		Type IThreadSafeField.DataType { get { return InternalField.DataType; } }
		#endregion
	}
	public class PivotGridFieldCollection : PivotChildCollection<PivotGridField> {
		PivotGridInternalFieldCollection fields;
		bool lockUpdateInternal;
		protected PivotGridInternalFieldCollection Fields { get { return fields; } }
		protected internal PivotGridWpfData Data { get { return Fields.Data; } }
		public PivotGridFieldCollection(PivotGridInternalFieldCollection fields)
			: base(fields.Data.PivotGrid, true) {
			this.fields = fields;
		}
		public PivotGridField Add() {
			PivotGridField field = new PivotGridField();
			Add(field);
			return field;
		}
		public PivotGridField Add(string fieldName, FieldArea area) {
			PivotGridField field = new PivotGridField() { FieldName = fieldName, Area = area };
			Add(field);
			return field;
		}
		internal void Add(PivotGridInternalField internalField) {
			PivotGridField field = new PivotGridField(internalField, false);
			field.FieldName = internalField.FieldName;
			field.Area = internalField.Area.ToFieldArea();
			field.Visible = internalField.Visible;
			field.Name = internalField.Name;
			field.Caption = internalField.Caption;
			field.DisplayFolder = internalField.DisplayFolder;
			field.KpiType = internalField.KPIType.ToKpiType();
			if(field.KpiType.NeedGraphic() && field.KpiGraphic == PivotKpiGraphic.None)
				field.KpiGraphic = PivotKpiGraphic.ServerDefined;
			BeginUpdateInternal();
			try {
				Add(field);
			} finally {
				EndUpdateInternal();
			}
		}
		public void AddRange(params PivotGridField[] fields) {
			Data.BeginUpdate();
			foreach(PivotGridField field in fields) {
				Add(field);
			}
			Data.EndUpdate();
		}
		public PivotGridField this[string fieldName] {
			get {
				foreach(PivotGridField field in this) {
					if(field.FieldName == fieldName)
						return field;
				}
				return null;
			}
		}
		public PivotGridField GetFieldByName(string name) {
			foreach(PivotGridField field in this) {
				if(field.Name == name)
					return field;
			}
			return null;
		}
		protected override void OnItemAdded(int index, PivotGridField wrapper) {
			base.OnItemAdded(index, wrapper);
			wrapper.SetBinding(PivotGridField.NameProxyProperty, new Binding("Name") { RelativeSource = new RelativeSource(RelativeSourceMode.Self) }); 
			wrapper.Collection = this;
			if(IsLockUpdateInternal)
				return;
			Fields.Add(wrapper.InternalField);
			Fields.SetFieldIndex(wrapper.InternalField, index);
		}
		protected override void OnItemMoved(int oldIndex, int newIndex, PivotGridField item) {
			base.OnItemMoved(oldIndex, newIndex, item);
			if(IsLockUpdateInternal)
				return;
			Fields.SetFieldIndex(item.InternalField, newIndex);
		}
		protected override void OnItemRemoved(int index, PivotGridField wrapper) {
			base.OnItemRemoved(index, wrapper);
			if(wrapper.Collection == this)
				wrapper.Collection = null;
			if(IsLockUpdateInternal)
				return;
			Fields.Remove(wrapper.InternalField);
		}
		protected override void OnItemReplaced(int index, PivotGridField oldField, PivotGridField newField) {
			base.OnItemReplaced(index, oldField, newField);
			if(IsLockUpdateInternal)
				return;
			Data.BeginUpdate();
			Fields.RemoveAt(index);
			Fields.Add(newField.InternalField);
			Fields.SetFieldIndex(newField.InternalField, index);
			Data.EndUpdate();
		}
		protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e) {
			base.OnCollectionChanged(e);
			if(Data != null && Data.PivotGrid != null && !Data.IsInBackgroundAsyncOperation)
				Data.PivotGrid.CoerceValue(PivotGridControl.GroupFieldsInFieldListProperty);
		}
		protected override void OnItemsClearing(IList<PivotGridField> oldItems) {
			Data.BeginUpdate();
			base.OnItemsClearing(oldItems);
			ClearAndRemoveGroups();
			Fields.Clear();
			Data.EndUpdate();
		}
		void ClearAndRemoveGroups() {
			for(int i = Data.Groups.Count - 1; i >= 0; i--) {
				Data.Groups[i].Clear();
				if(!string.IsNullOrEmpty(Data.Groups[i].Hierarchy))
					Data.Groups.Remove(Data.Groups[i]);
			}
		}
		internal void OnInitialized() {
			foreach(PivotGridField field in this) {
				field.OnInitialized();
			}
		}
		protected internal virtual void OnEndDeserializing() {
			foreach(PivotGridField field in this) {
				field.OnEndDeserializing();
			}
		}
		void BeginUpdateInternal() {
			lockUpdateInternal = true;
		}
		void EndUpdateInternal() {
			lockUpdateInternal = false;
		}
		bool IsLockUpdateInternal {
			get {
				return lockUpdateInternal;
			}
		}
		internal void SyncOLAPProperties() {
			foreach(PivotGridField field in this) {
				field.SyncOLAPProperties(true, true);
			}
		}
	}
	public delegate void UpdateFieldDelegate(PivotGridField field);
	public static class PivotGridFieldExtensions {
		public static PivotGridInternalField GetInternalField(this PivotGridField field) {
			return field != null ? field.InternalField : null;
		}
		public static PivotGridField GetWrapper(this PivotGridInternalField field) {
			return field != null ? field.Wrapper : null;
		}
		public static PivotGridField GetWrapper(this PivotGridFieldBase field) {
			return field != null ? ((PivotGridInternalField)field).Wrapper : null;
		}
		public static PivotGridField GetWrapper(this PivotFieldItem field) {
			return field != null ? field.Wrapper : null;
		}
		public static DevExpress.XtraPivotGrid.PivotGridGroup GetInternalGroup(this PivotGridGroup group) {
			return group != null ? group.InternalGroup : null;
		}
		public static PivotGridGroup GetWrapper(this DevExpress.XtraPivotGrid.PivotGridGroup group) {
			return group != null ? ((PivotGridInternalGroup)group).Wrapper : null;
		}
		public static PivotGridGroup GetWrapper(this PivotGridInternalGroup group) {
			return group != null ? group.Wrapper : null;
		}
		public static PivotGridCustomTotal GetWrapper(this PivotGridCustomTotalBase customTotal) {
			return customTotal != null ? ((PivotGridInternalCustomTotal)customTotal).Wrapper : null;
		}
	}
	public class PivotOlapMember {
		IOLAPMember member;
		protected internal PivotOlapMember(IOLAPMember member) {
			if(member == null)
				throw new ArgumentNullException("member");
			this.member = member;
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotOlapMemberCaption")]
#endif
		public string Caption { get { return member.Caption; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotOlapMemberUniqueName")]
#endif
		public string UniqueName { get { return member.UniqueName; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotOlapMemberValue")]
#endif
		public object Value { get { return member.Value; } }
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotOlapMemberProperties")]
#endif
		public OLAPMemberProperties Properties { get { return member.Properties; } }
	}
	public class FieldFilterValues : PivotGridFieldFilterValues {
		public FieldFilterValues(PivotGridInternalField field)
			: base(field) {
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("FieldFilterValuesField")
#else
	Description("")
#endif
		]
		public new PivotGridField Field { get { return base.Field.GetWrapper(); } }
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("FieldFilterValuesFilterType"),
#endif
		XtraSerializableProperty(2), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID)
		]
		public new FieldFilterType FilterType {
			get { return base.FilterType.ToFieldFilterType(); }
			set { base.FilterType = value.ToPivotFilterType(); }
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public new void SetValuesAsync(IList<object> values, PivotFilterType filterType, bool showBlanks, DevExpress.XtraPivotGrid.AsyncCompletedHandler handler) {
			base.SetValuesAsync(values, filterType, showBlanks, true, handler);
		}
		public void SetValues(object[] values, FieldFilterType filterType, bool showBlanks) {
			base.SetValues(values, filterType.ToPivotFilterType(), showBlanks);
		}
		public bool SetValuesAsync(object[] values, FieldFilterType filterType, bool showBlanks, AsyncCompletedHandler handler) {
			return base.SetValuesAsync(values, filterType.ToPivotFilterType(), showBlanks, handler.ToCoreAsyncCompleted());
		}
		protected override void OnChanged() {
			base.OnChanged();
			if(Field != null)
				Field.SyncFieldShowNewValues(true, false);
		}
	}
	public interface IThreadSafeField {
		FieldArea Area { get; }
		int AreaIndex { get; }
		bool Visible { get; }
		string Name { get; }
		string FieldName { get; }
		string UnboundFieldName { get; }
		string PrefilterColumnName { get; }
		string Caption { get; }
		string DisplayText { get; }
		string DisplayFolder { get; }
		FieldSummaryType SummaryType { get; }
		FieldSortOrder SortOrder { get; }
		FieldSortMode SortMode { get; }
		DevExpress.Xpf.PivotGrid.FieldFilterValues FilterValues { get; }
		FieldAllowedAreas AllowedAreas { get; }
		PivotGridGroup Group { get; }
		int GroupIndex { get; }
		bool TopValueShowOthers { get; }
		int TopValueCount { get; }
		bool RunningTotal { get; }
		bool ShowNewValues { get; }
		FieldTopValueType TopValueType { get; }
		FieldGroupInterval GroupInterval { get; }
		int GroupIntervalNumericRange { get; }
		string UnboundExpression { get; }
		bool ExpandedInFieldsGroup { get; }
		FieldUnboundColumnType UnboundType { get; }
		Type DataType { get; }
	}
}
