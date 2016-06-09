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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DevExpress.Data.Filtering;
using DevExpress.Utils.Serializing;
using DevExpress.Xpf.Bars;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Core.ConditionalFormatting;
using DevExpress.Xpf.Core.ConditionalFormatting.Native;
using DevExpress.Xpf.Core.Serialization;
using DevExpress.Xpf.Editors.Filtering;
using DevExpress.Xpf.PivotGrid.Internal;
using DevExpress.Xpf.PivotGrid.Printing;
using FloatingContainerType = DevExpress.Xpf.Core.FloatingContainer;
namespace DevExpress.Xpf.PivotGrid {
	public partial class PivotGridControl : IFormatsOwner {
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlGridMenu"),
#endif
 Browsable(false)]
		public PivotGridPopupMenu GridMenu {
			get {
				if(gridMenu == null)
					gridMenu = new PivotGridPopupMenu(this);
				return gridMenu;
			}
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlHeaderAreaMenuCustomizations"),
#endif
 Browsable(false)]
		public BarManagerActionCollection HeaderAreaMenuCustomizations {
			get { return HeadersAreaMenuController.ActionContainer.Actions; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlHeaderMenuCustomizations"),
#endif
 Browsable(false)]
		public BarManagerActionCollection HeaderMenuCustomizations {
			get { return HeaderMenuController.ActionContainer.Actions; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldValueMenuCustomizations"),
#endif
 Browsable(false)]
		public BarManagerActionCollection FieldValueMenuCustomizations {
			get { return FieldValueMenuController.ActionContainer.Actions; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlCellMenuCustomizations"),
#endif
 Browsable(false)]
		public BarManagerActionCollection CellMenuCustomizations {
			get { return CellMenuController.ActionContainer.Actions; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlDataSource"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), Bindable(true),
		Category(Categories.Data)
		]
		public object DataSource {
			get { return GetValue(DataSourceProperty); }
			set {
				if(value == null)
					ClearValue(DataSourceProperty);
				else
					SetValue(DataSourceProperty, value);
			}
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlOlapConnectionString"),
#endif
		Category(Categories.Data),
		]
		public string OlapConnectionString {
			get { return (string)GetValue(OlapConnectionStringProperty); }
			set {
				if(value == null)
					ClearValue(OlapConnectionStringProperty);
				else
					SetValue(OlapConnectionStringProperty, value);
			}
		}
#if !SL
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlOlapDataProvider"),
#endif
		Category(Categories.Data),
		]
		public OlapDataProvider OlapDataProvider {
			get { return (OlapDataProvider)GetValue(OlapDataProviderProperty); }
			set { SetValue(OlapDataProviderProperty, value); }
		}
#endif
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlOlapDefaultMemberFields"),
#endif
		Category(Categories.Data),
		]
		public PivotDefaultMemberFields OlapDefaultMemberFields {
			get { return (PivotDefaultMemberFields)GetValue(OlapDefaultMemberFieldsProperty); }
			set { SetValue(OlapDefaultMemberFieldsProperty, value); }
		}
		[
		Category(Categories.Data),
		]
		public bool OlapFilterByUniqueName {
			get { return (bool)GetValue(OlapFilterByUniqueNameProperty); }
			set { SetValue(OlapFilterByUniqueNameProperty, value); }
		}
		protected bool IsDataSourceActive {
			get { return Data.IsDataBound && !IsLoading; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartDataSource"),
#endif
 Category(Categories.OptionsChartDataSource)]
		public IBindingList ChartDataSource {
			get { return (IBindingList)Data.ChartDataSource; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrefilterCriteria"),
#endif
		Category(Categories.Data),
		]
		public CriteriaOperator PrefilterCriteria {
			get { return (CriteriaOperator)GetValue(PrefilterCriteriaProperty); }
			set { SetValue(PrefilterCriteriaProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowPrefilterPanel"),
#endif
		Category(Categories.Data),
		]
		public bool ShowPrefilterPanel {
			get { return (bool)GetValue(ShowPrefilterPanelProperty); }
			private set { this.SetValue(ShowPrefilterPanelPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrefilterPanelText")]
#endif
		public string PrefilterPanelText {
			get { return (string)GetValue(PrefilterPanelTextProperty); }
			private set { this.SetValue(PrefilterPanelTextPropertyKey, value); }
		}
#if !SL
	[DevExpressXpfPivotGridLocalizedDescription("PivotGridControlActiveFilterInfo")]
#endif
		public CriteriaOperatorInfo ActiveFilterInfo {
			get { return (CriteriaOperatorInfo)GetValue(ActiveFilterInfoProperty); }
			private set { this.SetValue(ActiveFilterInfoPropertyKey, value); }
		}
		public UserAction UserAction {
			get { return (UserAction)GetValue(UserActionProperty); }
			internal set { this.SetValue(UserActionPropertyKey, value); }
		}		
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool ShowColumnsBorder {
			get { return (bool)GetValue(ShowColumnsBorderProperty); }
			private set { this.SetValue(ShowColumnsBorderPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowPrefilterPanelMode"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public ShowPrefilterPanelMode ShowPrefilterPanelMode {
			get { return (ShowPrefilterPanelMode)GetValue(ShowPrefilterPanelModeProperty); }
			set { SetValue(ShowPrefilterPanelModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrefilterString"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public string PrefilterString {
			get { return (string)GetValue(PrefilterStringProperty); }
			set { SetValue(PrefilterStringProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlIsPrefilterEnabled"),
#endif
		Category(Categories.Data),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool IsPrefilterEnabled {
			get { return (bool)GetValue(IsPrefilterEnabledProperty); }
			set { SetValue(IsPrefilterEnabledProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFields"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Category(Categories.Data),
		XtraSerializableProperty(true, true, true), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public PivotGridFieldCollection Fields { get { return Data.Fields; } }
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlGroups"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Category(Categories.Data),
		XtraSerializableProperty(true, true, true, 100), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public PivotGridGroupCollection Groups { get { return Data.Groups; } }
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlHiddenFields"),
#endif
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public ReadOnlyObservableCollection<PivotGridField> HiddenFields {
			get { return Data.FieldListFields.HiddenFields; }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFocusedCell"),
#endif
		Category(Categories.Layout),
		]
#if SL
		 [TypeConverter(typeof(PointConverter))]
#endif
		public System.Drawing.Point FocusedCell {
			get { return (System.Drawing.Point)GetValue(FocusedCellProperty); }
			set { SetValue(FocusedCellProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlLeftTopCoord"),
#endif
		Category(Categories.Layout),
		]
		public System.Drawing.Point LeftTopCoord {
			get {
				return (System.Drawing.Point)GetValue(LeftTopCoordProperty);
			}
			private set {
				SetValue(LeftTopCoordPropertyKey, value);
			}
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlLeftTopPixelCoord"),
#endif
		Category(Categories.Layout),
		]
		public Point LeftTopPixelCoord {
			get {
				return (Point)GetValue(LeftTopPixelCoordProperty);
			}
			private set {
				SetValue(LeftTopPixelCoordPropertyKey, value);
			}
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlSelection"),
#endif
		Category(Categories.Layout),
		]
#if SL
		[TypeConverter(typeof(RectConverter))]
#endif
		public System.Drawing.Rectangle Selection {
			get { return (System.Drawing.Rectangle)GetValue(SelectionProperty); }
			set { SetValue(SelectionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlMultiSelection"),
#endif
		Browsable(false),
		]
		public DevExpress.XtraPivotGrid.Selection.IMultipleSelection MultiSelection { get { return VisualItems; } }
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlColumnCount"),
#endif
		Browsable(false),
		]
		public int ColumnCount { get { return VisualItems.ColumnCount; } }
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlRowCount"),
#endif
		Browsable(false),
		]
		public int RowCount { get { return VisualItems.RowCount; } }
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowCrossGroupVariation"),
#endif
		Category(Categories.OptionsData),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool AllowCrossGroupVariation {
			get { return (bool)GetValue(AllowCrossGroupVariationProperty); }
			set { SetValue(AllowCrossGroupVariationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlGroupDataCaseSensitive"),
#endif
		Category(Categories.OptionsData),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool GroupDataCaseSensitive {
			get { return (bool)GetValue(GroupDataCaseSensitiveProperty); }
			set { SetValue(GroupDataCaseSensitiveProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAutoExpandGroups"),
#endif
		Category(Categories.OptionsData), TypeConverter(typeof(NullableBoolConverter)),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool? AutoExpandGroups {
			get { return (bool?)GetValue(AutoExpandGroupsProperty); }
			set { SetValue(AutoExpandGroupsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlDrillDownMaxRowCount"),
#endif
		Category(Categories.OptionsData),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public int DrillDownMaxRowCount {
			get { return (int)GetValue(DrillDownMaxRowCountProperty); }
			set { SetValue(DrillDownMaxRowCountProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlDataFieldUnboundExpressionMode"),
#endif
		Category(Categories.OptionsData),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public UnboundExpressionMode DataFieldUnboundExpressionMode {
			get { return (UnboundExpressionMode)GetValue(DataFieldUnboundExpressionModeProperty); }
			set { SetValue(DataFieldUnboundExpressionModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFilterByVisibleFieldsOnly"),
#endif
		Category(Categories.OptionsData),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool FilterByVisibleFieldsOnly {
			get { return (bool)GetValue(FilterByVisibleFieldsOnlyProperty); }
			set { SetValue(FilterByVisibleFieldsOnlyProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowCustomizationForm"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool AllowCustomizationForm {
			get { return (bool)GetValue(AllowCustomizationFormProperty); }
			set { SetValue(AllowCustomizationFormProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowDrag"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool AllowDrag {
			get { return (bool)GetValue(AllowDragProperty); }
			set { SetValue(AllowDragProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowDragInCustomizationForm"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool AllowDragInCustomizationForm {
			get { return (bool)GetValue(AllowDragInCustomizationFormProperty); }
			set { SetValue(AllowDragInCustomizationFormProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowExpand"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool AllowExpand {
			get { return (bool)GetValue(AllowExpandProperty); }
			set { SetValue(AllowExpandProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowFilter"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool AllowFilter {
			get { return (bool)GetValue(AllowFilterProperty); }
			set { SetValue(AllowFilterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowResizing"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool AllowResizing {
			get { return (bool)GetValue(AllowResizingProperty); }
			set { SetValue(AllowResizingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowSort"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool AllowSort {
			get { return (bool)GetValue(AllowSortProperty); }
			set { SetValue(AllowSortProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowSortBySummary"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool AllowSortBySummary {
			get { return (bool)GetValue(AllowSortBySummaryProperty); }
			set { SetValue(AllowSortBySummaryProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowHideFields"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public AllowHideFieldsType AllowHideFields {
			get { return (AllowHideFieldsType)GetValue(AllowHideFieldsProperty); }
			set { SetValue(AllowHideFieldsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldListStyle"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public FieldListStyle FieldListStyle {
			get { return (FieldListStyle)GetValue(FieldListStyleProperty); }
			set { SetValue(FieldListStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldListLayout"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public FieldListLayout FieldListLayout {
			get { return (FieldListLayout)GetValue(FieldListLayoutProperty); }
			set { SetValue(FieldListLayoutProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldListAllowedLayouts"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public FieldListAllowedLayouts FieldListAllowedLayouts {
			get { return (FieldListAllowedLayouts)GetValue(FieldListAllowedLayoutsProperty); }
			set { SetValue(FieldListAllowedLayoutsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldListSplitterY"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public double FieldListSplitterY {
			get { return (double)GetValue(FieldListSplitterYProperty); }
			set { SetValue(FieldListSplitterYProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldListSplitterX"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public double FieldListSplitterX {
			get { return (double)GetValue(FieldListSplitterXProperty); }
			set { SetValue(FieldListSplitterXProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlDeferredUpdates"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool DeferredUpdates {
			get { return (bool)GetValue(DeferredUpdatesProperty); }
			set { SetValue(DeferredUpdatesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowPrefilter"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool AllowPrefilter {
			get { return (bool)GetValue(AllowPrefilterProperty); }
			set { SetValue(AllowPrefilterProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowExpandOnDoubleClick"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool AllowExpandOnDoubleClick {
			get { return (bool)GetValue(AllowExpandOnDoubleClickProperty); }
			set { SetValue(AllowExpandOnDoubleClickProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowSortInFieldList"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool AllowSortInFieldList {
			get { return (bool)GetValue(AllowSortInFieldListProperty); }
			set { SetValue(AllowSortInFieldListProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowFilterInFieldList"),
#endif
		Category(Categories.OptionsCustomization),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool AllowFilterInFieldList {
			get { return (bool)GetValue(AllowFilterInFieldListProperty); }
			set { SetValue(AllowFilterInFieldListProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlDrawFocusedCellRect"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool DrawFocusedCellRect {
			get { return (bool)GetValue(DrawFocusedCellRectProperty); }
			set { SetValue(DrawFocusedCellRectProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowColumnGrandTotals"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowColumnGrandTotals {
			get { return (bool)GetValue(ShowColumnGrandTotalsProperty); }
			set { SetValue(ShowColumnGrandTotalsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowColumnHeaders"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowColumnHeaders {
			get { return (bool)GetValue(ShowColumnHeadersProperty); }
			set { SetValue(ShowColumnHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowColumnTotals"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowColumnTotals {
			get { return (bool)GetValue(ShowColumnTotalsProperty); }
			set { SetValue(ShowColumnTotalsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowCustomTotalsForSingleValues"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowCustomTotalsForSingleValues {
			get { return (bool)GetValue(ShowCustomTotalsForSingleValuesProperty); }
			set { SetValue(ShowCustomTotalsForSingleValuesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowDataHeaders"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowDataHeaders {
			get { return (bool)GetValue(ShowDataHeadersProperty); }
			set { SetValue(ShowDataHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowFilterHeaders"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowFilterHeaders {
			get { return (bool)GetValue(ShowFilterHeadersProperty); }
			set { SetValue(ShowFilterHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowGrandTotalsForSingleValues"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowGrandTotalsForSingleValues {
			get { return (bool)GetValue(ShowGrandTotalsForSingleValuesProperty); }
			set { SetValue(ShowGrandTotalsForSingleValuesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowRowGrandTotals"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowRowGrandTotals {
			get { return (bool)GetValue(ShowRowGrandTotalsProperty); }
			set { SetValue(ShowRowGrandTotalsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowRowHeaders"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowRowHeaders {
			get { return (bool)GetValue(ShowRowHeadersProperty); }
			set { SetValue(ShowRowHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowRowTotals"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowRowTotals {
			get { return (bool)GetValue(ShowRowTotalsProperty); }
			set { SetValue(ShowRowTotalsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowColumnGrandTotalHeader"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowColumnGrandTotalHeader {
			get { return (bool)GetValue(ShowColumnGrandTotalHeaderProperty); }
			set { SetValue(ShowColumnGrandTotalHeaderProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowRowGrandTotalHeader"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowRowGrandTotalHeader {
			get { return (bool)GetValue(ShowRowGrandTotalHeaderProperty); }
			set { SetValue(ShowRowGrandTotalHeaderProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowTotalsForSingleValues"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool ShowTotalsForSingleValues {
			get { return (bool)GetValue(ShowTotalsForSingleValuesProperty); }
			set { SetValue(ShowTotalsForSingleValuesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlColumnTotalsLocation"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public FieldColumnTotalsLocation ColumnTotalsLocation {
			get { return (FieldColumnTotalsLocation)GetValue(ColumnTotalsLocationProperty); }
			set { SetValue(ColumnTotalsLocationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlRowTotalsLocation"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public FieldRowTotalsLocation RowTotalsLocation {
			get { return (FieldRowTotalsLocation)GetValue(RowTotalsLocationProperty); }
			set { SetValue(RowTotalsLocationProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlRowTreeWidth"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public int RowTreeWidth {
			get { return (int)GetValue(RowTreeWidthProperty); }
			set { SetValue(RowTreeWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlRowTreeHeight"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public int RowTreeHeight {
			get { return (int)GetValue(RowTreeHeightProperty); }
			set { SetValue(RowTreeHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlRowTreeMinHeight"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public double RowTreeMinHeight {
			get { return (double)GetValue(RowTreeMinHeightProperty); }
			set { SetValue(RowTreeMinHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlRowTreeMinWidth"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public double RowTreeMinWidth {
			get { return (double)GetValue(RowTreeMinWidthProperty); }
			set { SetValue(RowTreeMinWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlRowTreeOffset"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public int RowTreeOffset {
			get { return (int)GetValue(RowTreeOffsetProperty); }
			set { SetValue(RowTreeOffsetProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlColumnFieldValuesAlignment"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public VerticalAlignment ColumnFieldValuesAlignment {
			get { return (VerticalAlignment)GetValue(ColumnFieldValuesAlignmentProperty); }
			set { SetValue(ColumnFieldValuesAlignmentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlRowFieldValuesAlignment"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public VerticalAlignment RowFieldValuesAlignment {
			get { return (VerticalAlignment)GetValue(RowFieldValuesAlignmentProperty); }
			set { SetValue(RowFieldValuesAlignmentProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlRowTotalsHeightFactor"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public double RowTotalsHeightFactor {
			get { return (double)GetValue(RowTotalsHeightFactorProperty); }
			set { SetValue(RowTotalsHeightFactorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlGroupFieldsInFieldList"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool GroupFieldsInFieldList {
			get { return (bool)GetValue(GroupFieldsInFieldListProperty); }
			set { this.SetValue(GroupFieldsInFieldListProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldListIncludeVisibleFields"),
#endif
		Category(Categories.OptionsView),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool FieldListIncludeVisibleFields {
			get { return (bool)GetValue(FieldListIncludeVisibleFieldsProperty); }
			set { SetValue(FieldListIncludeVisibleFieldsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlDataFieldArea"),
#endif
		Category(Categories.OptionsDataField),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public DataFieldArea DataFieldArea {
			get { return (DataFieldArea)GetValue(DataFieldAreaProperty); }
			set { SetValue(DataFieldAreaProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlDataFieldAreaIndex"),
#endif
		Category(Categories.OptionsDataField),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public int DataFieldAreaIndex {
			get { return (int)GetValue(DataFieldAreaIndexProperty); }
			set { SetValue(DataFieldAreaIndexProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlDataFieldWidth"),
#endif
		Category(Categories.OptionsDataField),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public double DataFieldWidth {
			get { return (double)GetValue(DataFieldWidthProperty); }
			set { SetValue(DataFieldWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlDataFieldHeight"),
#endif
		Category(Categories.OptionsDataField),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.LayoutID),
		]
		public double DataFieldHeight {
			get { return (double)GetValue(DataFieldHeightProperty); }
			set { SetValue(DataFieldHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlDataFieldCaption"),
#endif
		Category(Categories.OptionsDataField),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID),
		]
		public string DataFieldCaption {
			get { return (string)GetValue(DataFieldCaptionProperty); }
			set { SetValue(DataFieldCaptionProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlSummaryDataSourceFieldNaming"),
#endif
		Category(Categories.OptionsDataField),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public SummaryDataSourceFieldNaming SummaryDataSourceFieldNaming {
			get { return (SummaryDataSourceFieldNaming)GetValue(SummaryDataSourceFieldNamingProperty); }
			set { SetValue(SummaryDataSourceFieldNamingProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartProvideDataByColumns"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(-100), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool ChartProvideDataByColumns {
			get { return (bool)GetValue(ChartProvideDataByColumnsProperty); }
			set { SetValue(ChartProvideDataByColumnsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartFieldValuesProvideMode"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(-100), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID) 
		]
		public PivotChartFieldValuesProvideMode ChartFieldValuesProvideMode {
			get { return (PivotChartFieldValuesProvideMode)GetValue(ChartFieldValuesProvideModeProperty); }
			set { SetValue(ChartFieldValuesProvideModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartDataProvideMode"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(-100), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID)
		]
		public PivotChartDataProvideMode ChartDataProvideMode {
			get { return (PivotChartDataProvideMode)GetValue(ChartDataProvideModeProperty); }
			set { SetValue(ChartDataProvideModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartDataProvidePriority"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(-100), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID)
		]
		public PivotChartDataProvidePriority ChartDataProvidePriority {
			get { return (PivotChartDataProvidePriority)GetValue(ChartDataProvidePriorityProperty); }
			set { SetValue(ChartDataProvidePriorityProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartSelectionOnly"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool ChartSelectionOnly {
			get { return (bool)GetValue(ChartSelectionOnlyProperty); }
			set { SetValue(ChartSelectionOnlyProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartProvideColumnTotals"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(-100), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool ChartProvideColumnTotals {
			get { return (bool)GetValue(ChartProvideColumnTotalsProperty); }
			set { SetValue(ChartProvideColumnTotalsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartProvideColumnGrandTotals"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(-100), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool ChartProvideColumnGrandTotals {
			get { return (bool)GetValue(ChartProvideColumnGrandTotalsProperty); }
			set { SetValue(ChartProvideColumnGrandTotalsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartProvideColumnCustomTotals"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(-100), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool ChartProvideColumnCustomTotals {
			get { return (bool)GetValue(ChartProvideColumnCustomTotalsProperty); }
			set { SetValue(ChartProvideColumnCustomTotalsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartProvideRowTotals"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(-100), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool ChartProvideRowTotals {
			get { return (bool)GetValue(ChartProvideRowTotalsProperty); }
			set { SetValue(ChartProvideRowTotalsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartProvideRowGrandTotals"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(-100), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool ChartProvideRowGrandTotals {
			get { return (bool)GetValue(ChartProvideRowGrandTotalsProperty); }
			set { SetValue(ChartProvideRowGrandTotalsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartProvideRowCustomTotals"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(-100), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool ChartProvideRowCustomTotals {
			get { return (bool)GetValue(ChartProvideRowCustomTotalsProperty); }
			set { SetValue(ChartProvideRowCustomTotalsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartAutoTranspose"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(-100), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool ChartAutoTranspose {
			get { return (bool)GetValue(ChartAutoTransposeProperty); }
			set { SetValue(ChartAutoTransposeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartProvideColumnFieldValuesAsType"),
#endif
		Category(Categories.OptionsChartDataSource), Browsable(false),DefaultValue(null),
		XtraSerializableProperty(-100), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public Type ChartProvideColumnFieldValuesAsType {
			get { return (Type)GetValue(ChartProvideColumnFieldValuesAsTypeProperty); }
			set { SetValue(ChartProvideColumnFieldValuesAsTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartProvideRowFieldValuesAsType"),
#endif
		Category(Categories.OptionsChartDataSource), Browsable(false),
		XtraSerializableProperty(-100), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public Type ChartProvideRowFieldValuesAsType {
			get { return (Type)GetValue(ChartProvideRowFieldValuesAsTypeProperty); }
			set { SetValue(ChartProvideRowFieldValuesAsTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartProvideCellValuesAsType"),
#endif
		Category(Categories.OptionsChartDataSource), Browsable(false),
		XtraSerializableProperty(-100), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public Type ChartProvideCellValuesAsType {
			get { return (Type)GetValue(ChartProvideCellValuesAsTypeProperty); }
			set { SetValue(ChartProvideCellValuesAsTypeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartUpdateDelay"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public int ChartUpdateDelay {
			get { return (int)GetValue(ChartUpdateDelayProperty); }
			set { SetValue(ChartUpdateDelayProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartProvideEmptyCells"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public bool ChartProvideEmptyCells {
			get { return (bool)GetValue(ChartProvideEmptyCellsProperty); }
			set { SetValue(ChartProvideEmptyCellsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartMaxSeriesCount"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public int ChartMaxSeriesCount {
			get { return (int)GetValue(ChartMaxSeriesCountProperty); }
			set { SetValue(ChartMaxSeriesCountProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlChartMaxPointCountInSeries"),
#endif
		Category(Categories.OptionsChartDataSource),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.DataSettingsID),
		]
		public int ChartMaxPointCountInSeries {
			get { return (int)GetValue(ChartMaxPointCountInSeriesProperty); }
			set { SetValue(ChartMaxPointCountInSeriesProperty, value); }
		}
		[
		Browsable(false),
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlClipboardCopyMultiSelectionMode"),
#endif
		Category(Categories.OptionsBehavior),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public CopyMultiSelectionMode ClipboardCopyMultiSelectionMode {
			get { return (CopyMultiSelectionMode)GetValue(ClipboardCopyMultiSelectionModeProperty); }
			set { SetValue(ClipboardCopyMultiSelectionModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlCopyToClipboardWithFieldValues"),
#endif
		Category(Categories.OptionsBehavior),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool CopyToClipboardWithFieldValues {
			get { return (bool)GetValue(CopyToClipboardWithFieldValuesProperty); }
			set { SetValue(CopyToClipboardWithFieldValuesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlClipboardCopyCollapsedValuesMode"),
#endif
		Category(Categories.OptionsBehavior),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public CopyCollapsedValuesMode ClipboardCopyCollapsedValuesMode {
			get { return (CopyCollapsedValuesMode)GetValue(ClipboardCopyCollapsedValuesModeProperty); }
			set { SetValue(ClipboardCopyCollapsedValuesModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlUseAsyncMode"),
#endif
		Category(Categories.OptionsBehavior),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool UseAsyncMode {
			get { return (bool)GetValue(UseAsyncModeProperty); }
			set { SetValue(UseAsyncModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlLoadingPanelDelay"),
#endif
		Category(Categories.OptionsBehavior),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public int LoadingPanelDelay {
			get { return (int)GetValue(LoadingPanelDelayProperty); }
			set { SetValue(LoadingPanelDelayProperty, value); }
		}		
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlSortBySummaryDefaultOrder"),
#endif
		Category(Categories.OptionsBehavior),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public FieldSortBySummaryOrder SortBySummaryDefaultOrder {
			get { return (FieldSortBySummaryOrder)GetValue(SortBySummaryDefaultOrderProperty); }
			set { SetValue(SortBySummaryDefaultOrderProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFixedRowHeaders"),
#endif
		Category(Categories.OptionsBehavior),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool FixedRowHeaders {
			get { return (bool)GetValue(FixedRowHeadersProperty); }
			set { SetValue(FixedRowHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlSelectMode"),
#endif
		Category(Categories.OptionsBehavior),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public SelectMode SelectMode {
			get { return (SelectMode)GetValue(SelectModeProperty); }
			set { SetValue(SelectModeProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlGroupFilterMode"),
#endif
		Category(Categories.OptionsFilterPopup),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public GroupFilterMode GroupFilterMode {
			get { return (GroupFilterMode)GetValue(GroupFilterModeProperty); }
			set { SetValue(GroupFilterModeProperty, value); }
		}
		[
		Category(Categories.OptionsFilterPopup),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool ShowOnlyAvailableFilterItems {
			get { return (bool)GetValue(ShowOnlyAvailableFilterItemsProperty); }
			set { SetValue(ShowOnlyAvailableFilterItemsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlIsHeaderMenuEnabled"),
#endif
		Category(Categories.OptionsMenu),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool IsHeaderMenuEnabled {
			get { return (bool)GetValue(IsHeaderMenuEnabledProperty); }
			set { SetValue(IsHeaderMenuEnabledProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlIsHeaderAreaMenuEnabled"),
#endif
		Category(Categories.OptionsMenu),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool IsHeaderAreaMenuEnabled {
			get { return (bool)GetValue(IsHeaderAreaMenuEnabledProperty); }
			set { SetValue(IsHeaderAreaMenuEnabledProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlIsFieldValueMenuEnabled"),
#endif
		Category(Categories.OptionsMenu),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool IsFieldValueMenuEnabled {
			get { return (bool)GetValue(IsFieldValueMenuEnabledProperty); }
			set { SetValue(IsFieldValueMenuEnabledProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlIsCellMenuEnabled"),
#endif
		Category(Categories.OptionsMenu),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool IsCellMenuEnabled {
			get { return (bool)GetValue(IsCellMenuEnabledProperty); }
			set { SetValue(IsCellMenuEnabledProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlIsFieldListMenuEnabled"),
#endif
		Category(Categories.OptionsMenu),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool IsFieldListMenuEnabled {
			get { return (bool)GetValue(IsFieldListMenuEnabledProperty); }
			set { SetValue(IsFieldListMenuEnabledProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlIsFilterPopupMenuEnabled"),
#endif
		Category(Categories.OptionsMenu),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool IsFilterPopupMenuEnabled {
			get { return (bool)GetValue(IsFilterPopupMenuEnabledProperty); }
			set { SetValue(IsFilterPopupMenuEnabledProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlBestFitMode"),
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
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlBestFitArea"),
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
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlBestFitMaxRowCount"),
#endif
		Category(Categories.OptionsBestFit),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public int BestFitMaxRowCount {
			get { return (int)GetValue(BestFitMaxRowCountProperty); }
			set { SetValue(BestFitMaxRowCountProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintFilterHeaders"),
#endif
		Category(Categories.OptionsPrint),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool PrintFilterHeaders {
			get { return (bool)GetValue(PrintFilterHeadersProperty); }
			set { SetValue(PrintFilterHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintColumnHeaders"),
#endif
		Category(Categories.OptionsPrint),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool PrintColumnHeaders {
			get { return (bool)GetValue(PrintColumnHeadersProperty); }
			set { SetValue(PrintColumnHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintRowHeaders"),
#endif
		Category(Categories.OptionsPrint),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool PrintRowHeaders {
			get { return (bool)GetValue(PrintRowHeadersProperty); }
			set { SetValue(PrintRowHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintDataHeaders"),
#endif
		Category(Categories.OptionsPrint),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool PrintDataHeaders {
			get { return (bool)GetValue(PrintDataHeadersProperty); }
			set { SetValue(PrintDataHeadersProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintHeadersOnEveryPage"),
#endif
		Category(Categories.OptionsPrint),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool PrintHeadersOnEveryPage {
			get { return (bool)GetValue(PrintHeadersOnEveryPageProperty); }
			set { SetValue(PrintHeadersOnEveryPageProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintUnusedFilterFields"),
#endif
		Category(Categories.OptionsPrint),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool PrintUnusedFilterFields {
			get { return (bool)GetValue(PrintUnusedFilterFieldsProperty); }
			set { SetValue(PrintUnusedFilterFieldsProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlMergeColumnFieldValues"),
#endif
		Category(Categories.OptionsPrint),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool MergeColumnFieldValues {
			get { return (bool)GetValue(MergeColumnFieldValuesProperty); }
			set { SetValue(MergeColumnFieldValuesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlMergeRowFieldValues"),
#endif
		Category(Categories.OptionsPrint),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool MergeRowFieldValues {
			get { return (bool)GetValue(MergeRowFieldValuesProperty); }
			set { SetValue(MergeRowFieldValuesProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintHorzLines"),
#endif
		Category(Categories.OptionsPrint),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool PrintHorzLines {
			get { return (bool)GetValue(PrintHorzLinesProperty); }
			set { SetValue(PrintHorzLinesProperty, value); }
		}   
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintVertLines"),
#endif
		Category(Categories.OptionsPrint),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool PrintVertLines {
			get { return (bool)GetValue(PrintVertLinesProperty); }
			set { SetValue(PrintVertLinesProperty, value); }
		} 
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintLayoutMode"),
#endif
		Category(Categories.OptionsPrint),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public PrintLayoutMode PrintLayoutMode {
			get { return (PrintLayoutMode)GetValue(PrintLayoutModeProperty); }
			set { SetValue(PrintLayoutModeProperty, value); }
		}
		protected internal PrintLayoutMode DesiredPrintLayoutMode { get; set; }
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintInsertPageBreaks"),
#endif
		Category(Categories.OptionsPrint),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.VisualOptionsID),
		]
		public bool PrintInsertPageBreaks {
			get { return (bool)GetValue(PrintInsertPageBreaksProperty); }
			set { SetValue(PrintInsertPageBreaksProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldHeaderDragIndicatorTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate FieldHeaderDragIndicatorTemplate {
			get { return (DataTemplate)base.GetValue(FieldHeaderDragIndicatorTemplateProperty); }
			set { SetValue(FieldHeaderDragIndicatorTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldHeaderTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate FieldHeaderTemplate {
			get { return (DataTemplate)GetValue(FieldHeaderTemplateProperty); }
			set { SetValue(FieldHeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldHeaderTreeViewTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate FieldHeaderTreeViewTemplate {
			get { return (DataTemplate)GetValue(FieldHeaderTreeViewTemplateProperty); }
			set { SetValue(FieldHeaderTreeViewTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldHeaderTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector FieldHeaderTemplateSelector {
			get { return (DataTemplateSelector)base.GetValue(FieldHeaderTemplateSelectorProperty); }
			set { SetValue(FieldHeaderTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldHeaderTreeViewTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector FieldHeaderTreeViewTemplateSelector {
			get { return (DataTemplateSelector)base.GetValue(FieldHeaderTreeViewTemplateSelectorProperty); }
			set { SetValue(FieldHeaderTreeViewTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldCellKpiTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate FieldCellKpiTemplate {
			get { return (DataTemplate)GetValue(FieldCellKpiTemplateProperty); }
			set { SetValue(FieldCellKpiTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldCellKpiTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector FieldCellKpiTemplateSelector {
			get { return (DataTemplateSelector)base.GetValue(FieldCellKpiTemplateSelectorProperty); }
			set { SetValue(FieldCellKpiTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldHeaderListTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate FieldHeaderListTemplate {
			get { return (DataTemplate)GetValue(FieldHeaderListTemplateProperty); }
			set { SetValue(FieldHeaderListTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldHeaderListTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector FieldHeaderListTemplateSelector {
			get { return (DataTemplateSelector)base.GetValue(FieldHeaderListTemplateSelectorProperty); }
			set { SetValue(FieldHeaderListTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldCellTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate FieldCellTemplate {
			get { return (DataTemplate)GetValue(FieldCellTemplateProperty); }
			set { SetValue(FieldCellTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldCellTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector FieldCellTemplateSelector {
			get { return (DataTemplateSelector)base.GetValue(FieldCellTemplateSelectorProperty); }
			set { SetValue(FieldCellTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldValueTemplate"),
#endif
		Category(Categories.Templates)
		]
		public DataTemplate FieldValueTemplate {
			get { return (DataTemplate)GetValue(FieldValueTemplateProperty); }
			set { SetValue(FieldValueTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldValueTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Templates)
		]
		public DataTemplateSelector FieldValueTemplateSelector {
			get { return (DataTemplateSelector)base.GetValue(FieldValueTemplateSelectorProperty); }
			set { SetValue(FieldValueTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFocusedCellBorderTemplate"),
#endif
		Category(Categories.Templates)
		]
		public ControlTemplate FocusedCellBorderTemplate {
			get { return (ControlTemplate)GetValue(FocusedCellBorderTemplateProperty); }
			set { SetValue(FocusedCellBorderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlResizingIndicatorTemplate"),
#endif
		Category(Categories.Templates)
		]
		public ControlTemplate ResizingIndicatorTemplate {
			get { return (ControlTemplate)GetValue(ResizingIndicatorTemplateProperty); }
			set { SetValue(ResizingIndicatorTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlResizingIndicatorTemplate"),
#endif
		Category(Categories.Templates)
		]
		public ControlTemplate ScrollViewerTemplate {
			get { return (ControlTemplate)GetValue(ScrollViewerTemplateProperty); }
			set { SetValue(ScrollViewerTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintFieldHeaderTemplate"),
#endif
		Category(Categories.PrintTemplates)
		]
		public DataTemplate PrintFieldHeaderTemplate {
			get { return (DataTemplate)GetValue(PrintFieldHeaderTemplateProperty); }
			set { SetValue(PrintFieldHeaderTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintFieldHeaderTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),		
		Category(Categories.PrintTemplates)
		]
		public DataTemplateSelector PrintFieldHeaderTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PrintFieldHeaderTemplateSelectorProperty); }
			set { SetValue(PrintFieldHeaderTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintFieldValueTemplate"),
#endif
		Category(Categories.PrintTemplates)
		]
		public DataTemplate PrintFieldValueTemplate {
			get { return (DataTemplate)GetValue(PrintFieldValueTemplateProperty); }
			set { SetValue(PrintFieldValueTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintFieldValueTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.PrintTemplates)
		]
		public DataTemplateSelector PrintFieldValueTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PrintFieldValueTemplateSelectorProperty); }
			set { SetValue(PrintFieldValueTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintFieldCellTemplate"),
#endif
		Category(Categories.PrintTemplates)
		]
		public DataTemplate PrintFieldCellTemplate {
			get { return (DataTemplate)GetValue(PrintFieldCellTemplateProperty); }
			set { SetValue(PrintFieldCellTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintFieldCellTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.PrintTemplates)
		]
		public DataTemplateSelector PrintFieldCellTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PrintFieldCellTemplateSelectorProperty); }
			set { SetValue(PrintFieldCellTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintFieldCellKpiTemplate"),
#endif
		Category(Categories.PrintTemplates)
		]
		public DataTemplate PrintFieldCellKpiTemplate {
			get { return (DataTemplate)GetValue(PrintFieldCellKpiTemplateProperty); }
			set { SetValue(PrintFieldCellKpiTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintFieldCellKpiTemplateSelector"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.PrintTemplates)
		]
		public DataTemplateSelector PrintFieldCellKpiTemplateSelector {
			get { return (DataTemplateSelector)GetValue(PrintFieldCellKpiTemplateSelectorProperty); }
			set { SetValue(PrintFieldCellKpiTemplateSelectorProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldHeight"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Layout)
		]
		public double FieldHeight {
			get { return (double)GetValue(FieldHeightProperty); }
			set { SetValue(FieldHeightProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldWidth"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Layout)
		]
		public double FieldWidth {
			get { return (double)GetValue(FieldWidthProperty); }
			set { SetValue(FieldWidthProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlScrollingMode"),
#endif
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Category(Categories.Layout)
		]
		public ScrollingMode ScrollingMode {
			get { return (ScrollingMode)GetValue(ScrollingModeProperty); }
			set { SetValue(ScrollingModeProperty, value); }
		}
		[Browsable(false)]
		public IColumnChooserFactory FieldListFactory {
			get { return (IColumnChooserFactory)GetValue(FieldListFactoryProperty); }
			set { SetValue(FieldListFactoryProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldListTemplate"),
#endif
 Category(Categories.Templates)]
		public ControlTemplate FieldListTemplate {
			get { return (ControlTemplate)GetValue(FieldListTemplateProperty); }
			set { SetValue(FieldListTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlExcelFieldListTemplate"),
#endif
 Category(Categories.Templates)]
		public ControlTemplate ExcelFieldListTemplate {
			get { return (ControlTemplate)GetValue(ExcelFieldListTemplateProperty); }
			set { SetValue(ExcelFieldListTemplateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlIsFieldListVisible"),
#endif
 Category(Categories.Layout)]
		public bool IsFieldListVisible {
			get { return (bool)GetValue(IsFieldListVisibleProperty); }
			set { SetValue(IsFieldListVisibleProperty, value); }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public IColumnChooserState FieldListState {
			get { return (IColumnChooserState)GetValue(FieldListStateProperty); }
			set { SetValue(FieldListStateProperty, value); }
		}
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		]
		public IColumnChooserState ExcelFieldListState {
			get { return (IColumnChooserState)GetValue(ExcelFieldListStateProperty); }
			set { SetValue(ExcelFieldListStateProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrefilterControl"),
#endif
 Browsable(false)]
		public FilterControl PrefilterControl { get; protected set; }
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlIsPrefilterVisible"),
#endif
 Category(Categories.Layout)]
		public bool IsPrefilterVisible {
			get { return (bool)GetValue(IsPrefilterVisibleProperty); }
			set { SetValue(IsPrefilterVisibleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrefilterContainer"),
#endif
 Browsable(false)]
		public FloatingContainerType PrefilterContainer { get; protected set; }
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlExpressionEditorContainer"),
#endif
 Browsable(false)]
#if !SL
		public FloatingContainer ExpressionEditorContainer { get; protected set; }
#else
		public DXDialog ExpressionEditorContainer { get; protected set; }
#endif
		bool IsExpressionEditorContainerOpen {
			get {
#if SL
				return ExpressionEditorContainer.IsVisible;
#else
				return ExpressionEditorContainer.IsOpen;
#endif
			}
			set {
#if SL
				if(ExpressionEditorContainer.IsVisible)
					ExpressionEditorContainer.Hide();
				else
					ExpressionEditorContainer.Show();
#else
				ExpressionEditorContainer.Close();
#endif
			}
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlFieldHeaderContentStyle"),
#endif
		Category(Categories.Appearance)
		]
		public Style FieldHeaderContentStyle {
			get { return (Style)GetValue(FieldHeaderContentStyleProperty); }
			set { SetValue(FieldHeaderContentStyleProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlShowBorder"),
#endif
		Category(Categories.Appearance), 
		XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)
		]
		public bool ShowBorder {
			get { return (bool)GetValue(ShowBorderProperty); }
			set { SetValue(ShowBorderProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlCellForeground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush CellForeground {
			get { return (Brush)GetValue(CellForegroundProperty); }
			set { SetValue(CellForegroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlCellTotalForeground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush CellTotalForeground {
			get { return (Brush)GetValue(CellTotalForegroundProperty); }
			set { SetValue(CellTotalForegroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlCellTotalSelectedForeground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush CellTotalSelectedForeground {
			get { return (Brush)GetValue(CellTotalSelectedForegroundProperty); }
			set { SetValue(CellTotalSelectedForegroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlCellSelectedForeground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush CellSelectedForeground {
			get { return (Brush)GetValue(CellSelectedForegroundProperty); }
			set { SetValue(CellSelectedForegroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlCellBackground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush CellBackground {
			get { return (Brush)GetValue(CellBackgroundProperty); }
			set { SetValue(CellBackgroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlCellTotalBackground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush CellTotalBackground {
			get { return (Brush)GetValue(CellTotalBackgroundProperty); }
			set { SetValue(CellTotalBackgroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlCellTotalSelectedBackground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush CellTotalSelectedBackground {
			get { return (Brush)GetValue(CellTotalSelectedBackgroundProperty); }
			set { SetValue(CellTotalSelectedBackgroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlCellSelectedBackground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush CellSelectedBackground {
			get { return (Brush)GetValue(CellSelectedBackgroundProperty); }
			set { SetValue(CellSelectedBackgroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlValueForeground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush ValueForeground {
			get { return (Brush)GetValue(ValueForegroundProperty); }
			set { SetValue(ValueForegroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlValueTotalForeground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush ValueTotalForeground {
			get { return (Brush)GetValue(ValueTotalForegroundProperty); }
			set { SetValue(ValueTotalForegroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlValueTotalSelectedForeground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush ValueTotalSelectedForeground {
			get { return (Brush)GetValue(ValueTotalSelectedForegroundProperty); }
			set { SetValue(ValueTotalSelectedForegroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlValueSelectedForeground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush ValueSelectedForeground {
			get { return (Brush)GetValue(ValueSelectedForegroundProperty); }
			set { SetValue(ValueSelectedForegroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlValueBackground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush ValueBackground {
			get { return (Brush)GetValue(ValueBackgroundProperty); }
			set { SetValue(ValueBackgroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlValueTotalBackground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush ValueTotalBackground {
			get { return (Brush)GetValue(ValueTotalBackgroundProperty); }
			set { SetValue(ValueTotalBackgroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlValueTotalSelectedBackground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush ValueTotalSelectedBackground {
			get { return (Brush)GetValue(ValueTotalSelectedBackgroundProperty); }
			set { SetValue(ValueTotalSelectedBackgroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlValueSelectedBackground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush ValueSelectedBackground {
			get { return (Brush)GetValue(ValueSelectedBackgroundProperty); }
			set { SetValue(ValueSelectedBackgroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintCellForeground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush PrintCellForeground {
			get { return (Brush)GetValue(PrintCellForegroundProperty); }
			set { SetValue(PrintCellForegroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintCellTotalForeground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush PrintCellTotalForeground {
			get { return (Brush)GetValue(PrintCellTotalForegroundProperty); }
			set { SetValue(PrintCellTotalForegroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintCellBackground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush PrintCellBackground {
			get { return (Brush)GetValue(PrintCellBackgroundProperty); }
			set { SetValue(PrintCellBackgroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintCellTotalBackground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush PrintCellTotalBackground {
			get { return (Brush)GetValue(PrintCellTotalBackgroundProperty); }
			set { SetValue(PrintCellTotalBackgroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintValueForeground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush PrintValueForeground {
			get { return (Brush)GetValue(PrintValueForegroundProperty); }
			set { SetValue(PrintValueForegroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintValueTotalForeground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush PrintValueTotalForeground {
			get { return (Brush)GetValue(PrintValueTotalForegroundProperty); }
			set { SetValue(PrintValueTotalForegroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintValueBackground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush PrintValueBackground {
			get { return (Brush)GetValue(PrintValueBackgroundProperty); }
			set { SetValue(PrintValueBackgroundProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlPrintValueTotalBackground"),
#endif
		Category(Categories.Brushes),
		]
		public Brush PrintValueTotalBackground {
			get { return (Brush)GetValue(PrintValueTotalBackgroundProperty); }
			set { SetValue(PrintValueTotalBackgroundProperty, value); }
		}
		internal ObservableCollection<CriteriaOperatorInfo> MRUFiltersInternal { 
			get {
				if(mruFiltersInternal == null)
					mruFiltersInternal = new ObservableCollection<CriteriaOperatorInfo>();
				return mruFiltersInternal; 
			} 
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlMRUFilters"),
#endif
		XtraSerializableProperty(true, false, true, 2147483645), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID), Category(Categories.Data)]
		public ReadOnlyObservableCollection<CriteriaOperatorInfo> MRUFilters {
			get {
				if(mruFilters == null)
					mruFilters = new ReadOnlyObservableCollection<CriteriaOperatorInfo>(MRUFiltersInternal);
				return mruFilters;
			}
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlAllowMRUFilterList"),
#endif
		Category(Categories.OptionsBehavior),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public bool AllowMRUFilterList {
			get { return (bool)GetValue(AllowMRUFilterListProperty); }
			set { SetValue(AllowMRUFilterListProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlMRUFilterListCount"),
#endif
		Category(Categories.OptionsBehavior),
		XtraSerializableProperty(), XtraSerializablePropertyId(PivotSerializationOptions.StoreAlwaysID),
		]
		public int MRUFilterListCount {
			get { return (int)GetValue(MRUFilterListCountProperty); }
			set { SetValue(MRUFilterListCountProperty, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlCanEnablePrefilter"),
#endif
		Category(Categories.OptionsBehavior)
		]
		public bool CanEnablePrefilter {
			get { return (bool)GetValue(CanEnablePrefilterProperty); }
			internal set { this.SetValue(CanEnablePrefilterPropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlIsMainWaitIndicatorVisible"),
#endif
		Category(Categories.OptionsBehavior)
		]
		public bool IsMainWaitIndicatorVisible {
			get { return (bool)GetValue(IsMainWaitIndicatorVisibleProperty); }
			internal set { this.SetValue(IsMainWaitIndicatorVisiblePropertyKey, value); }
		}
		[
#if !SL
	DevExpressXpfPivotGridLocalizedDescription("PivotGridControlIsFilterPopupWaitIndicatorVisible"),
#endif
		Category(Categories.OptionsBehavior)
		]
		public bool IsFilterPopupWaitIndicatorVisible {
			get { return (bool)GetValue(IsFilterPopupWaitIndicatorVisibleProperty); }
			internal set { this.SetValue(IsFilterPopupWaitIndicatorVisiblePropertyKey, value); }
		}
		[Category(Categories.ConditionalFormatting), XtraSerializableProperty(true, false, false), XtraResetProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID)]
		public FormatConditionCollection FormatConditions { get { return formatConditions; } }
		[XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID), Category(Categories.ConditionalFormatting)]
		public bool AllowConditionalFormattingMenu {
			get { return (bool)GetValue(AllowConditionalFormattingMenuProperty); }
			set { SetValue(AllowConditionalFormattingMenuProperty, value); }
		}
		[XtraSerializableProperty, XtraSerializablePropertyId(PivotSerializationOptions.AppearanceID), Category(Categories.ConditionalFormatting)]
		public bool AllowConditionalFormattingManager {
			get { return (bool)GetValue(AllowConditionalFormattingManagerProperty); }
			set { SetValue(AllowConditionalFormattingManagerProperty, value); }
		}
		[Category(Categories.ConditionalFormatting)]
		public FormatInfoCollection PredefinedFormats {
			get { return (FormatInfoCollection)GetValue(PredefinedFormatsProperty); }
			set { SetValue(PredefinedFormatsProperty, value); }
		}
		[Category(Categories.ConditionalFormatting)]
		public FormatInfoCollection PredefinedColorScaleFormats {
			get { return (FormatInfoCollection)GetValue(PredefinedColorScaleFormatsProperty); }
			set { SetValue(PredefinedColorScaleFormatsProperty, value); }
		}
		[Category(Categories.ConditionalFormatting)]
		public FormatInfoCollection PredefinedDataBarFormats {
			get { return (FormatInfoCollection)GetValue(PredefinedDataBarFormatsProperty); }
			set { SetValue(PredefinedDataBarFormatsProperty, value); }
		}
		[Category(Categories.ConditionalFormatting)]
		public FormatInfoCollection PredefinedIconSetFormats {
			get { return (FormatInfoCollection)GetValue(PredefinedIconSetFormatsProperty); }
			set { SetValue(PredefinedIconSetFormatsProperty, value); }
		}
		[Category(Categories.ConditionalFormatting)]
		public DataTemplate FormatConditionDialogServiceTemplate {
			get { return (DataTemplate)GetValue(FormatConditionDialogServiceTemplateProperty); }
			set { SetValue(FormatConditionDialogServiceTemplateProperty, value); }
		}
		[Category(Categories.ConditionalFormatting)]
		public DataTemplate ConditionalFormattingManagerServiceTemplate {
			get { return (DataTemplate)GetValue(ConditionalFormattingManagerServiceTemplateProperty); }
			set { SetValue(ConditionalFormattingManagerServiceTemplateProperty, value); }
		}
		internal IDesignTimeAdorner DesignTimeAdorner { get; set; }
	}
}
