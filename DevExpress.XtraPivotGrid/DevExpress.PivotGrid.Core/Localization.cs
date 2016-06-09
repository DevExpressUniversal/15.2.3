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
using System.ComponentModel;
using System.Resources;
using System.Globalization;
using DevExpress.Utils;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
using DevExpress.Data.PivotGrid;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.XtraPivotGrid.Localization {
	[ToolboxItem(false)]
	public class PivotGridLocalizer : XtraLocalizer<PivotGridStringId> {
		static PivotGridStringId[] emptyAreaIds = new PivotGridStringId[] { 
			PivotGridStringId.RowHeadersCustomization, 
			PivotGridStringId.ColumnHeadersCustomization, 
			PivotGridStringId.FilterHeadersCustomization, 
			PivotGridStringId.DataHeadersCustomization };
		static PivotGridStringId[] summaryIds = new PivotGridStringId[] { 
			PivotGridStringId.SummaryCount,
			PivotGridStringId.SummarySum,
			PivotGridStringId.SummaryMin,
			PivotGridStringId.SummaryMax,
			PivotGridStringId.SummaryAverage,
			PivotGridStringId.SummaryStdDev,
			PivotGridStringId.SummaryStdDevp,
			PivotGridStringId.SummaryVar,
			PivotGridStringId.SummaryVarp,
			PivotGridStringId.SummaryCustom };
		static PivotGridStringId[] areaIds = new PivotGridStringId[] { 
			PivotGridStringId.RowArea, 
			PivotGridStringId.ColumnArea, 
			PivotGridStringId.FilterArea, 
			PivotGridStringId.DataArea };
		static PivotGridLocalizer() {
			if(GetActiveLocalizerProvider() == null) {
				SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<PivotGridStringId>(new PivotGridResLocalizer()));
			}
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridLocalizerActive")]
#endif
		public static new XtraLocalizer<PivotGridStringId> Active {
			get { return XtraLocalizer<PivotGridStringId>.Active; }
			set {
				if(GetActiveLocalizerProvider() as DefaultActiveLocalizerProvider<PivotGridStringId> == null) {
					SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<PivotGridStringId>(value));
					RaiseActiveChanged();
				} else
					XtraLocalizer<PivotGridStringId>.Active = value; 
			}
		}
		public static string GetHeadersAreaText(int areaIndex) {
			if(areaIndex < 0 || areaIndex >= emptyAreaIds.Length)
				throw new ArgumentOutOfRangeException("areaIndex");
			return Active.GetLocalizedString(emptyAreaIds[areaIndex]);
		}
		public static string GetAreaText(int areaIndex) {
			if(areaIndex < 0 || areaIndex >= areaIds.Length)
				throw new ArgumentOutOfRangeException("areaIndex");
			return Active.GetLocalizedString(areaIds[areaIndex]);
		}
		public static string GetSummaryTypeText(PivotSummaryType summaryType) {
			int summaryIndex = (int)summaryType;
			if(summaryIndex < 0 || summaryIndex >= summaryIds.Length)
				throw new ArgumentOutOfRangeException("summaryType");
			return Active.GetLocalizedString(summaryIds[summaryIndex]);
		}
		public static string GetString(PivotGridStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override XtraLocalizer<PivotGridStringId> CreateResXLocalizer() {
			return new PivotGridResLocalizer();
		}
		protected override void PopulateStringTable() {
			AddString(PivotGridStringId.RowHeadersCustomization, "Drop Row Fields Here");
			AddString(PivotGridStringId.ColumnHeadersCustomization, "Drop Column Fields Here");
			AddString(PivotGridStringId.FilterHeadersCustomization, "Drop Filter Fields Here");
			AddString(PivotGridStringId.DataHeadersCustomization, "Drop Data Items Here");
			AddString(PivotGridStringId.RowArea, "Row Area");
			AddString(PivotGridStringId.ColumnArea, "Column Area");
			AddString(PivotGridStringId.FilterArea, "Filter Area");
			AddString(PivotGridStringId.DataArea, "Data Area");
			AddString(PivotGridStringId.FilterShowAll, "(Show All)");
			AddString(PivotGridStringId.FilterOk, "OK");
			AddString(PivotGridStringId.FilterCancel, "Cancel");
			AddString(PivotGridStringId.FilterBlank, "(Blank)");
			AddString(PivotGridStringId.FilterShowBlanks, "(Show Blanks)");
			AddString(PivotGridStringId.FilterInvert, "Invert Filter");
			AddString(PivotGridStringId.CustomizationFormCaption, "PivotGrid Field List");
			AddString(PivotGridStringId.CustomizationFormText, "Drag Items to the PivotGrid");
			AddString(PivotGridStringId.CustomizationFormAddTo, "Add To");
			AddString(PivotGridStringId.CustomizationFormHint, "Drag fields between areas below:");
			AddString(PivotGridStringId.CustomizationFormDeferLayoutUpdate, "Defer Layout Update");
			AddString(PivotGridStringId.CustomizationFormUpdate, "Update");
			AddString(PivotGridStringId.CustomizationFormListBoxText, "Drag a field here to customize layout");
			AddString(PivotGridStringId.CustomizationFormHiddenFields, "Hidden Fields");
			AddString(PivotGridStringId.CustomizationFormAvailableFields, "Choose fields to add to report:");
			AddString(PivotGridStringId.Total, "Total");
			AddString(PivotGridStringId.GrandTotal, "Grand Total");
			AddString(PivotGridStringId.TotalFormat, "{0} Total");
			AddString(PivotGridStringId.TotalFormatCount, "{0} Count");
			AddString(PivotGridStringId.TotalFormatSum, "{0} Sum");
			AddString(PivotGridStringId.TotalFormatMin, "{0} Min");
			AddString(PivotGridStringId.TotalFormatMax, "{0} Max");
			AddString(PivotGridStringId.TotalFormatAverage, "{0} Average");
			AddString(PivotGridStringId.TotalFormatStdDev, "{0} StdDev");
			AddString(PivotGridStringId.TotalFormatStdDevp, "{0} StdDevp");
			AddString(PivotGridStringId.TotalFormatVar, "{0} Var");
			AddString(PivotGridStringId.TotalFormatVarp, "{0} Varp");
			AddString(PivotGridStringId.TotalFormatCustom, "{0} Custom");
			AddString(PivotGridStringId.PrintDesigner, "Print Designer");
			AddString(PivotGridStringId.PrintDesignerPageOptions, "Options");
			AddString(PivotGridStringId.PrintDesignerPageBehavior, "Behavior");
			AddString(PivotGridStringId.PrintDesignerCategoryDefault, "Default");
			AddString(PivotGridStringId.PrintDesignerCategoryLines, "Lines");
			AddString(PivotGridStringId.PrintDesignerCategoryHeaders, "Headers");
			AddString(PivotGridStringId.PrintDesignerCategoryFieldValues, "Field Values");
			AddString(PivotGridStringId.PrintDesignerHorizontalLines, "Horizontal Lines");
			AddString(PivotGridStringId.PrintDesignerVerticalLines, "Vertical Lines");
			AddString(PivotGridStringId.PrintDesignerFilterHeaders, "Filter Headers");
			AddString(PivotGridStringId.PrintDesignerDataHeaders, "Data Headers");
			AddString(PivotGridStringId.PrintDesignerColumnHeaders, "Column Headers");
			AddString(PivotGridStringId.PrintDesignerRowHeaders, "Row Headers");
			AddString(PivotGridStringId.PrintDesignerHeadersOnEveryPage, "Headers On Every Page");
			AddString(PivotGridStringId.PrintDesignerUnusedFilterFields, "Unused Filter Fields");
			AddString(PivotGridStringId.PrintDesignerMergeColumnFieldValues, "Merge Column Field Values");
			AddString(PivotGridStringId.PrintDesignerMergeRowFieldValues, "Merge Row Field Values");
			AddString(PivotGridStringId.PrintDesignerUsePrintAppearance, "Use Print Appearance");
			AddString(PivotGridStringId.PopupMenuRefreshData, "Reload Data");
			AddString(PivotGridStringId.PopupMenuSortAscending, "Sort A-Z");
			AddString(PivotGridStringId.PopupMenuSortDescending, "Sort Z-A");
			AddString(PivotGridStringId.PopupMenuClearSorting, "Clear Sorting");
			AddString(PivotGridStringId.PopupMenuShowExpression, "Expression Editor...");
			AddString(PivotGridStringId.PopupMenuHideField, "Hide");			
			AddString(PivotGridStringId.PopupMenuShowFieldList, "Show Field List");
			AddString(PivotGridStringId.PopupMenuHideFieldList, "Hide Field List");
			AddString(PivotGridStringId.PopupMenuFieldOrder, "Order");
			AddString(PivotGridStringId.PopupMenuMovetoBeginning, "Move to Beginning");
			AddString(PivotGridStringId.PopupMenuMovetoLeft, "Move to Left");
			AddString(PivotGridStringId.PopupMenuMovetoRight, "Move to Right");
			AddString(PivotGridStringId.PopupMenuMovetoEnd, "Move to End");
			AddString(PivotGridStringId.PopupMenuExpand, "Expand");
			AddString(PivotGridStringId.PopupMenuCollapse, "Collapse");
			AddString(PivotGridStringId.PopupMenuExpandAll, "Expand All");
			AddString(PivotGridStringId.PopupMenuCollapseAll, "Collapse All");
			AddString(PivotGridStringId.PopupMenuSortFieldByColumn, "Sort \"{0}\" by This Column");
			AddString(PivotGridStringId.PopupMenuSortFieldByRow, "Sort \"{0}\" by This Row");
			AddString(PivotGridStringId.PopupMenuRemoveAllSortByColumn, "Remove All Sorting");
			AddString(PivotGridStringId.DataFieldCaption, "Data");
			AddString(PivotGridStringId.TopValueOthersRow, "Others");
			AddString(PivotGridStringId.CellError, "Error");
			AddString(PivotGridStringId.ValueError, "Error");
			AddString(PivotGridStringId.PopupMenuShowPrefilter, "Show Prefilter");
			AddString(PivotGridStringId.PopupMenuHidePrefilter, "Hide Prefilter");
			AddString(PivotGridStringId.PrefilterFormCaption, "PivotGrid Prefilter");
			AddString(PivotGridStringId.PrefilterInvalidProperty, "(invalid property)");
			AddString(PivotGridStringId.PrefilterInvalidCriteria, "An error occurs in the Prefilter criteria. Please detect invalid property captions inside the criteria operands and correct or remove them.");
			AddString(PivotGridStringId.EditPrefilter, "Edit Prefilter");
			AddString(PivotGridStringId.OLAPMeasuresCaption, "Measures");
			AddString(PivotGridStringId.OLAPKPIsCaption, "KPIs");
			AddString(PivotGridStringId.OLAPKPITypeValue, "Value");
			AddString(PivotGridStringId.OLAPKPITypeGoal, "Goal");
			AddString(PivotGridStringId.OLAPKPITypeStatus, "Status");
			AddString(PivotGridStringId.OLAPKPITypeTrend, "Trend");
			AddString(PivotGridStringId.OLAPKPITypeWeight, "Weight");
			AddString(PivotGridStringId.OLAPDrillDownFilterException, "Show Details command cannot be executed when multiple items are selected in a report filter field. Select a single item for each field in the report filter area before performing a drillthrough.");
			AddString(PivotGridStringId.OLAPNoOleDbProvidersMessage, "In order to use the PivotGrid OLAP functionality, you should have a MS OLAP OleDb provider installed on your system.\r\nYou can download it here:");
			AddString(PivotGridStringId.TrendGoingDown, "Going Down");
			AddString(PivotGridStringId.TrendGoingUp, "Going Up");
			AddString(PivotGridStringId.TrendNoChange, "No Change");
			AddString(PivotGridStringId.StatusBad, "Bad");
			AddString(PivotGridStringId.StatusGood, "Good");
			AddString(PivotGridStringId.StatusNeutral, "Neutral");
			AddString(PivotGridStringId.DateTimeQuarterFormat, "Q{0}");
			AddString(PivotGridStringId.SummaryCount, "Count");
			AddString(PivotGridStringId.SummarySum, "Sum");
			AddString(PivotGridStringId.SummaryMin, "Min");
			AddString(PivotGridStringId.SummaryMax, "Max");
			AddString(PivotGridStringId.SummaryAverage, "Average");
			AddString(PivotGridStringId.SummaryStdDev, "StdDev");
			AddString(PivotGridStringId.SummaryStdDevp, "StdDevp");
			AddString(PivotGridStringId.SummaryVar, "Var");
			AddString(PivotGridStringId.SummaryVarp, "Varp");
			AddString(PivotGridStringId.SummaryCustom, "Custom");
			AddString(PivotGridStringId.CustomizationFormStackedDefault, "Fields Section and Areas Section Stacked");
			AddString(PivotGridStringId.CustomizationFormStackedSideBySide, "Fields Section and Areas Section Side-By-Side");
			AddString(PivotGridStringId.CustomizationFormTopPanelOnly, "Fields Section Only");
			AddString(PivotGridStringId.CustomizationFormBottomPanelOnly2by2, "Areas Section Only (2 by 2)");
			AddString(PivotGridStringId.CustomizationFormBottomPanelOnly1by4, "Areas Section Only (1 by 4)");
			AddString(PivotGridStringId.CustomizationFormLayoutButtonTooltip, "Customization Form Layout");
			AddString(PivotGridStringId.Alt_Expand, "[Expand]");
			AddString(PivotGridStringId.Alt_Collapse, "[Collapse]");
			AddString(PivotGridStringId.Alt_SortedAscending, "(Ascending)");
			AddString(PivotGridStringId.Alt_SortedDescending, "(Descending)");
			AddString(PivotGridStringId.Alt_FilterWindowSizeGrip, "[Resize]");
			AddString(PivotGridStringId.Alt_FilterButton, "[Filter]");
			AddString(PivotGridStringId.Alt_FilterButtonActive, "[Filtered]");
			AddString(PivotGridStringId.Alt_DragHideField, "Hide");
			AddString(PivotGridStringId.Alt_FilterAreaHeaders, "[Filter Area Headers]");
			AddString(PivotGridStringId.Alt_ColumnAreaHeaders, "[Column Area Headers]");
			AddString(PivotGridStringId.Alt_RowAreaHeaders, "[Row Area Headers]");
			AddString(PivotGridStringId.Alt_DataAreaHeaders, "[Data Area Headers]");
			AddString(PivotGridStringId.Alt_FieldListHeaders, "[Hidden Field's Headers]");
			AddString(PivotGridStringId.Alt_LayoutButton, "[Layout Button]");
			AddString(PivotGridStringId.Alt_StackedDefaultLayout, "[Stacked Default Layout]");
			AddString(PivotGridStringId.Alt_StackedSideBySideLayout, "[Stacked Side By Side Layout]");
			AddString(PivotGridStringId.Alt_TopPanelOnlyLayout, "[Top Panel Only Layout]");
			AddString(PivotGridStringId.Alt_BottomPanelOnly2by2Layout, "[Bottom Panel Only 2 by 2 Layout]");
			AddString(PivotGridStringId.Alt_BottomPanelOnly1by4Layout, "[Bottom Panel Only 1 by 4 Layout]");
			AddString(PivotGridStringId.SearchBoxText, "Search");
			AddString(PivotGridStringId.PopupMenuKPIGraphic, "KPI Graphics");
			AddString(PivotGridStringId.PopupMenuKPIGraphicNone, "None");
			AddString(PivotGridStringId.PopupMenuKPIGraphicServerDefined, "Server Defined");
			AddString(PivotGridStringId.PopupMenuKPIGraphicShapes, "Shapes");
			AddString(PivotGridStringId.PopupMenuKPIGraphicTrafficLights, "Traffic Lights");
			AddString(PivotGridStringId.PopupMenuKPIGraphicRoadSigns, "Road Signs");
			AddString(PivotGridStringId.PopupMenuKPIGraphicGauge, "Gauge");
			AddString(PivotGridStringId.PopupMenuKPIGraphicReversedGauge, "Reversed Gauge");
			AddString(PivotGridStringId.PopupMenuKPIGraphicThermometer, "Thermometer");
			AddString(PivotGridStringId.PopupMenuKPIGraphicReversedThermometer, "Reversed Thermometer");
			AddString(PivotGridStringId.PopupMenuKPIGraphicCylinder, "Cylinder");
			AddString(PivotGridStringId.PopupMenuKPIGraphicReversedCylinder, "Reversed Cylinder");
			AddString(PivotGridStringId.PopupMenuKPIGraphicFaces, "Faces");
			AddString(PivotGridStringId.PopupMenuKPIGraphicVarianceArrow, "Variance Arrow");
			AddString(PivotGridStringId.PopupMenuKPIGraphicStandardArrow, "Standard Arrow");
			AddString(PivotGridStringId.PopupMenuKPIGraphicStatusArrow, "Status Arrow");
			AddString(PivotGridStringId.PopupMenuKPIGraphicReversedStatusArrow, "Reversed Status Arrow");
			AddString(PivotGridStringId.SummaryFilterApplyToSpecificLevel, "Apply to specific level");
			AddString(PivotGridStringId.SummaryFilterColumnField, "Column field:");
			AddString(PivotGridStringId.SummaryFilterLegendHidden, "Hidden");
			AddString(PivotGridStringId.SummaryFilterLegendVisible, "Visible");
			AddString(PivotGridStringId.SummaryFilterRangeFrom, "Show values from");
			AddString(PivotGridStringId.SummaryFilterRangeTo, "to");
			AddString(PivotGridStringId.SummaryFilterRowField, "Row field:");
			AddString(PivotGridStringId.SummaryFilterMaxValueCount, "Max Count");
			AddString(PivotGridStringId.SummaryFilterMaxVisibleCount, "Max Visible Count");
			AddString(PivotGridStringId.SummaryFilterLabelValues, "Values");
			AddString(PivotGridStringId.SummaryFilterClearButton, "Clear");
			AddString(PivotGridStringId.DrillDownException, "Error retrieving drilldown dataset");
			AddString(PivotGridStringId.PopupMenuFormatRules, "Format Rules");
			AddString(PivotGridStringId.PopupMenuFormatRulesIntersectionOnly, "Apply only to specific level");
			AddString(PivotGridStringId.PopupMenuFormatRulesClearMeasureRules, "Clear Rules from This Measure");
			AddString(PivotGridStringId.PopupMenuFormatRulesClearIntersectionRules, "Clear Rules from This Intersection");
			AddString(PivotGridStringId.PopupMenuFormatRulesClearAllRules, "Clear Rules from All Measures");
			AddString(PivotGridStringId.PopupMenuFormatRulesMeasure, "Measure");
			AddString(PivotGridStringId.PopupMenuFormatRulesColumn, "Column");
			AddString(PivotGridStringId.PopupMenuFormatRulesRow, "Row");
			AddString(PivotGridStringId.PopupMenuFormatRulesAnyField, "(Any)");
			AddString(PivotGridStringId.PopupMenuFormatRulesGrandTotal, "(Grand Total)");
		}
	}
	public class PivotGridResLocalizer : PivotGridLocalizer {
		ResourceManager manager = null;
		public PivotGridResLocalizer() {
			CreateResourceManager();
		}
		protected virtual void CreateResourceManager() {
#if !DXPORTABLE
			if(manager != null) this.manager.ReleaseAllResources();
			this.manager = new ResourceManager("DevExpress.XtraPivotGrid.LocalizationRes", typeof(PivotGridResLocalizer).GetAssembly());
#else
			this.manager = new ResourceManager("DevExpress.XtraPivotGrid.Core.LocalizationRes", typeof(PivotGridResLocalizer).GetAssembly());
#endif
		}
		protected virtual ResourceManager Manager { get { return manager; } }
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; }}
		public override string GetLocalizedString(PivotGridStringId id) {
			string resStr = "PivotGridStringId." + id.ToString();
			string ret = Manager.GetString(resStr);
			if(ret == null) ret = "";
			return ret;
		}
	}
	#region enum PivotGridStringId
	public enum PivotGridStringId {
		RowHeadersCustomization,
		ColumnHeadersCustomization,
		FilterHeadersCustomization,
		DataHeadersCustomization,
		RowArea,
		ColumnArea,
		FilterArea,
		DataArea,
		FilterShowAll,
		FilterOk,
		FilterCancel,
		FilterBlank,
		FilterShowBlanks,
		FilterInvert,
		CustomizationFormCaption,
		CustomizationFormText,
		CustomizationFormAddTo,
		CustomizationFormHint,
		CustomizationFormDeferLayoutUpdate,
		CustomizationFormUpdate,
		CustomizationFormListBoxText,
		CustomizationFormHiddenFields,
		CustomizationFormAvailableFields,
		Total,
		GrandTotal,
		TotalFormat,
		TotalFormatCount,
		TotalFormatSum,
		TotalFormatMin,
		TotalFormatMax,
		TotalFormatAverage,
		TotalFormatStdDev,
		TotalFormatStdDevp,
		TotalFormatVar,
		TotalFormatVarp,
		TotalFormatCustom,
		PrintDesigner,
		PrintDesignerPageOptions,
		PrintDesignerPageBehavior,
		PrintDesignerCategoryDefault,
		PrintDesignerCategoryLines,
		PrintDesignerCategoryHeaders,
		PrintDesignerCategoryFieldValues,
		PrintDesignerHorizontalLines,
		PrintDesignerVerticalLines,
		PrintDesignerFilterHeaders,
		PrintDesignerDataHeaders,
		PrintDesignerColumnHeaders,
		PrintDesignerRowHeaders,
		PrintDesignerHeadersOnEveryPage,
		PrintDesignerUnusedFilterFields,
		PrintDesignerMergeColumnFieldValues,
		PrintDesignerMergeRowFieldValues,
		PrintDesignerUsePrintAppearance,
		PopupMenuRefreshData,
		PopupMenuSortAscending,
		PopupMenuSortDescending,
		PopupMenuClearSorting,
		PopupMenuShowExpression,
		PopupMenuHideField,
		PopupMenuShowFieldList,
		PopupMenuHideFieldList,
		PopupMenuFieldOrder,
		PopupMenuMovetoBeginning,
		PopupMenuMovetoLeft,
		PopupMenuMovetoRight,
		PopupMenuMovetoEnd,
		PopupMenuCollapse,
		PopupMenuExpand,
		PopupMenuCollapseAll,
		PopupMenuExpandAll,
		PopupMenuShowPrefilter,
		PopupMenuHidePrefilter,
		PopupMenuSortFieldByColumn,
		PopupMenuSortFieldByRow,
		PopupMenuRemoveAllSortByColumn,
		DataFieldCaption,
		TopValueOthersRow,
		CellError,
		ValueError,
		PrefilterInvalidProperty,
		PrefilterInvalidCriteria,
		PrefilterFormCaption,
		EditPrefilter,
		OLAPMeasuresCaption,
		OLAPKPIsCaption,
		OLAPKPITypeValue,
		OLAPKPITypeGoal,
		OLAPKPITypeStatus,
		OLAPKPITypeTrend,
		OLAPKPITypeWeight, 
		OLAPDrillDownFilterException,
		OLAPNoOleDbProvidersMessage,
		TrendGoingUp,
		TrendGoingDown,
		TrendNoChange,
		StatusBad, 
		StatusNeutral,
		StatusGood,
		DateTimeQuarterFormat,
		SummaryCount, 
		SummarySum, 
		SummaryMin, 
		SummaryMax, 
		SummaryAverage, 
		SummaryStdDev, 
		SummaryStdDevp, 
		SummaryVar,
		SummaryVarp, 
		SummaryCustom,
		CustomizationFormStackedDefault, 
		CustomizationFormStackedSideBySide, 
		CustomizationFormTopPanelOnly, 
		CustomizationFormBottomPanelOnly2by2, 
		CustomizationFormBottomPanelOnly1by4,
		CustomizationFormLayoutButtonTooltip,
		Alt_Expand,
		Alt_Collapse,
		Alt_SortedAscending,
		Alt_SortedDescending,
		Alt_FilterWindowSizeGrip,
		Alt_FilterButton, 
		Alt_FilterButtonActive,
		Alt_DragHideField,
		Alt_FilterAreaHeaders,
		Alt_ColumnAreaHeaders,
		Alt_RowAreaHeaders,
		Alt_DataAreaHeaders,
		Alt_FieldListHeaders,
		Alt_LayoutButton,
		Alt_StackedDefaultLayout,
		Alt_StackedSideBySideLayout,
		Alt_TopPanelOnlyLayout,
		Alt_BottomPanelOnly2by2Layout,
		Alt_BottomPanelOnly1by4Layout,
		SearchBoxText,
		PopupMenuKPIGraphic,
		PopupMenuKPIGraphicNone,
		PopupMenuKPIGraphicServerDefined,
		PopupMenuKPIGraphicShapes,
		PopupMenuKPIGraphicTrafficLights,
		PopupMenuKPIGraphicRoadSigns,
		PopupMenuKPIGraphicGauge,
		PopupMenuKPIGraphicReversedGauge,
		PopupMenuKPIGraphicThermometer,
		PopupMenuKPIGraphicReversedThermometer,
		PopupMenuKPIGraphicCylinder,
		PopupMenuKPIGraphicReversedCylinder,
		PopupMenuKPIGraphicFaces,
		PopupMenuKPIGraphicVarianceArrow,
		PopupMenuKPIGraphicStandardArrow,
		PopupMenuKPIGraphicStatusArrow,
		PopupMenuKPIGraphicReversedStatusArrow,
		SummaryFilterRangeFrom,
		SummaryFilterRangeTo,
		SummaryFilterLegendHidden,
		SummaryFilterLegendVisible,
		SummaryFilterApplyToSpecificLevel,
		SummaryFilterColumnField,
		SummaryFilterRowField,
		SummaryFilterMaxValueCount,
		SummaryFilterMaxVisibleCount,
		SummaryFilterLabelValues,
		SummaryFilterClearButton,
		DrillDownException,
		PopupMenuFormatRules,
		PopupMenuFormatRulesIntersectionOnly,
		PopupMenuFormatRulesClearMeasureRules,
		PopupMenuFormatRulesClearIntersectionRules,
		PopupMenuFormatRulesClearAllRules,
		PopupMenuFormatRulesMeasure,
		PopupMenuFormatRulesColumn,
		PopupMenuFormatRulesRow,
		PopupMenuFormatRulesAnyField,
		PopupMenuFormatRulesGrandTotal,
	}
	#endregion
}
