#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       Dashboard                                                   }
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
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	[
	DashboardItemType(DashboardItemType.Grid)
	]
	public partial class GridDashboardItem : DataDashboardItem, ISparklineArgumentHolder {
		const string xmlSparklineArgument = "SparklineArgument";
		const string xmlGridOptions = "GridOptions";
		readonly SparklineArgumentHolder sparklineArgumentHolder;
		readonly GridColumnCollection columns = new GridColumnCollection();
		readonly GridOptions gridOptions;
		readonly DataItemBox<Dimension> sparklineArgumentBox;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridDashboardItemColumns"),
#endif
 Category(CategoryNames.Data), DefaultValue(null),
		Editor(TypeNames.GridColumnCollectionEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public GridColumnCollection Columns { get { return columns; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridDashboardItemGridOptions"),
#endif
 Category(CategoryNames.Layout),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public GridOptions GridOptions { get { return gridOptions; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridDashboardItemInteractivityOptions"),
#endif
 Category(CategoryNames.Interactivity),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DashboardItemInteractivityOptions InteractivityOptions { get { return InteractivityOptionsBase as DashboardItemInteractivityOptions; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridDashboardItemSparklineArgument"),
#endif
 Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableDimensionPropertyTypeConverter),
		DefaultValue(null)
		]
		public Dimension SparklineArgument {
			get { return sparklineArgumentBox.DataItem; }
			set { sparklineArgumentBox.DataItem = value; }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridDashboardItemFormatRules"),
#endif
 Category(CategoryNames.Data),
		Editor(TypeNames.FormatRulesManageEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public GridItemFormatRuleCollection FormatRules { get { return (GridItemFormatRuleCollection)base.FormatRulesInternal; } }
		protected internal override bool IsDrillDownEnabled {
			get { return InteractivityOptions.IsDrillDownEnabled; }
			set { InteractivityOptions.IsDrillDownEnabled = value; }
		}
		protected internal override DashboardItemMasterFilterMode MasterFilterMode {
			get { return InteractivityOptions.MasterFilterMode; }
			set { InteractivityOptions.MasterFilterMode = value; }
		}
		protected internal override bool HasDataItems {
			get {
				if(base.HasDataItems)
					return true;
				foreach(GridColumnBase column in columns)
					if(column.DataItemsCount != 0)
						return true;
				if(sparklineArgumentBox.DataItem != null)
					return true;
				return false;
			}
		}
		protected internal override IList<Dimension> SelectionDimensionList { get { return GetSelectionDimensions(); } }
		protected internal override string CaptionPrefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNameGridItem); } }
		protected override IEnumerable<DataDashboardItemDescription> ItemDescriptions {
			get {
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionColumns),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemColumn), ItemKind.GridColumn, columns);
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionSparkline),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionSparkline), ItemKind.SingleDimension, sparklineArgumentHolder);
			}
		}
		public GridDashboardItem() {
			gridOptions = new GridOptions(this);
			columns.CollectionChanged += (sender, e) => OnDataItemContainersChanged(e.AddedItems, e.RemovedItems);
			sparklineArgumentHolder = new SparklineArgumentHolder(this);
			sparklineArgumentBox = InitializeDimensionBox(sparklineArgumentHolder, xmlSparklineArgument);
		}
		protected override IList<Dimension> GetFormatRuleAxisItems(bool isSecondAxis) {
			IList<Dimension> axisItems = base.GetFormatRuleAxisItems(isSecondAxis);
			if(!isSecondAxis) {
				if(IsDrillDownEnabled)
					return new[] { CurrentDrillDownDimension };
				foreach(GridColumnBase column in Columns) {
					if(column is GridDimensionColumn && column.DataItemsCount > 0)
						axisItems.Add(column.DataItems.Last<DataItem>() as Dimension);
				}
			}
			return axisItems;
		}
		protected override IList<Measure> GetFormatRuleMeasures() {
			IList<Measure> measures = base.GetFormatRuleMeasures();
			foreach(GridColumnBase column in Columns) {
				if(column is GridMeasureColumn && column.DataItemsCount > 0)
					measures.Add(column.DataItems.Last<DataItem>() as Measure);
			}
			return measures;
		}
		protected override IFormatRuleCollection CreateFormatRules() {
			GridItemFormatRuleCollection c = new GridItemFormatRuleCollection();
			c.CollectionChanged += OnFormatRulesChanged<GridItemFormatRule>;
			return c;
		}
		protected override FilterableDashboardItemInteractivityOptions CreateInteractivityOptions() {
			return new DashboardItemInteractivityOptions(false);
		}
		internal IList<GridColumnBase> GetVisibleColumns() {
			List<GridColumnBase> visibleColumns = new List<GridColumnBase>();
			visibleColumns.AddRange(columns);
			if(IsDrillDownEnabled) {
				IList<GridDimensionColumn> selectionColumns = GetSelectionColumns();
				if(selectionColumns.Count > 0) {
					Dimension drillDownDimension = CurrentDrillDownDimension;
					for(int i = visibleColumns.Count - 1; i >= 0; i--) {
						GridDimensionColumn dimensionColumn = visibleColumns[i] as GridDimensionColumn;
						if(dimensionColumn != null) {
							if(selectionColumns.Contains(dimensionColumn) && dimensionColumn.Dimension != drillDownDimension)
								visibleColumns.Remove(dimensionColumn);
						}
					}
				}
			}
			return visibleColumns;
		}
		internal IList<Dimension> GetSelectionDimensions() {
			IList<GridDimensionColumn> selectionColumns = GetSelectionColumns();
			List<Dimension> dimensions = new List<Dimension>();
			foreach(GridDimensionColumn gridColumn in selectionColumns)
				dimensions.Add(gridColumn.Dimension);
			return dimensions;
		}
		internal string[] GetRowIdentificatorDataMembers() {
			List<string> dataIds = new List<string>();
			foreach(GridColumnBase column in GetVisibleColumns()) {
				GridDimensionColumn dimensionColumn = column as GridDimensionColumn;
				if(dimensionColumn != null) {
					IEnumerable<string> dimensionUniqueNames = dimensionColumn.DataItems.Select(dataItem => dataItem.ActualId);
					if(dimensionUniqueNames != null && dimensionUniqueNames.Count() > 0)
						dataIds.Add(dimensionUniqueNames.First());
				}
			}
			return dataIds.ToArray();
		}
		internal override bool IsConditionalFormattingCalculateByAllowed(DataItem itemCalculateBy) {
			return base.IsConditionalFormattingCalculateByAllowed(itemCalculateBy) && !IsDeltaOrSparklineColumnPart(itemCalculateBy);
		}
		internal override bool IsBarConditionalFormattingCalculateAllowed(DataItem itemCalculateBy) {
			return !IsImageColumnPart(itemCalculateBy);
		}
		internal override bool IsConditionalFormattingApplyToAllowed(DataItem itemCalculateBy, DataItem itemApplyTo, FormatConditionBase condition) {
			return base.IsConditionalFormattingApplyToAllowed(itemCalculateBy, itemApplyTo, condition) && SparklineArgument != itemApplyTo && !IsDeltaColumnPart(itemApplyTo);
		}
		internal void OnGridOptionsChanged() {
			OnChanged(ChangeReason.View, this);
		}
		internal void OnGridOptionsChanged(ChangedEventArgs e) {
			OnChanged(e.Reason, this, e.Param);
		}
		protected internal string[] GetSelectionDataMembers() {
			IList<Dimension> selectionDimensions = GetSelectionDimensions();
			if(selectionDimensions.Count > 0) {
				if(IsDrillDownEnabled)
					return new string[1] { DataItemRepository.GetActualID(CurrentDrillDownDimension) };
				List<string> dataMembers = new List<string>();
				for(int i = 0; i < selectionDimensions.Count; i++)
					dataMembers.Add(DataItemRepository.GetActualID(selectionDimensions[i]));
				return dataMembers.ToArray();
			}
			return new string[0];
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			List<Dimension> dimensions = new List<Dimension>();
			IList<Dimension> selectionDimensions = GetSelectionDimensions();
			int? drillDownLevel = CurrentDrillDownLevel;
			int lastSelectionDimensionIndex = drillDownLevel ?? selectionDimensions.Count - 1;
			foreach(GridColumnBase column in Columns) {
				DataItemContainerActualContent content = column.GetActualContent();
				List<Measure> containerMeasures = content.Measures;
				List<Dimension> containerDimensions = content.Dimensions;
				foreach(Measure measure in containerMeasures)
					builder.AddMeasure(measure);
				foreach(Dimension dimension in containerDimensions) {
					if(!selectionDimensions.Contains(dimension) || selectionDimensions.IndexOf(dimension) <= lastSelectionDimensionIndex)
						dimensions.Add(dimension);
				}
				if(content.IsDelta) {
					Measure actualMeasure = content.DeltaActualValue;
					Measure targetMeasure = content.DeltaTargetValue;
					if(actualMeasure != null && targetMeasure != null)
						builder.AddDeltaDescriptor(actualMeasure, targetMeasure, column, content.DeltaOptions);
				}
			}
			if(SparklineArgument != null)
				builder.SetRowHierarchyDimensions(DashboardDataAxisNames.SparklineAxis, new Dimension[] { SparklineArgument });
			builder.SetColumnHierarchyDimensions(DashboardDataAxisNames.DefaultAxis, dimensions);
			builder.AddFormatConditionMeasureDescriptors(FormatRules);
			foreach(GridColumnBase column in Columns)
				column.AddColumnSummaryDescriptors(builder.MeasureDescriptors);
		}
		protected internal override string[] GetSelectionDataMemberDisplayNames() {
			IList<Dimension> selectionDimensions = GetSelectionDimensions();
			if(selectionDimensions.Count > 0) {
				if (IsDrillDownEnabled) {
					Dimension ddDimension = CurrentDrillDownDimension;
					if (ddDimension != null)
						return new string[1] { CurrentDrillDownDimension.DisplayName };
					return new string[0];
				}
				List<string> dataMembers = new List<string>();
				for(int i = 0; i < selectionDimensions.Count; i++)
					dataMembers.Add(selectionDimensions[i].DisplayName);
				return dataMembers.ToArray();
			}
			return new string[0];
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			IList<GridColumnBase> columns = GetVisibleColumns();
			IList<GridColumnViewModel> columnViewModels = columns
				.Select(column => column.CreateViewModel(DataSource, DataMember))
				.Where(viewModel => viewModel != null).ToList();
			return new GridDashboardItemViewModel(this) {
				Columns = columnViewModels,
				AllowCellMerge = GridOptions.AllowCellMerge,
				EnableBandedRows = GridOptions.EnableBandedRows,
				ShowHorizontalLines = GridOptions.ShowHorizontalLines,
				ShowVerticalLines = GridOptions.ShowVerticalLines,
				ShowColumnHeaders = GridOptions.ShowColumnHeaders,
				ColumnWidthMode = GridOptions.ColumnWidthMode,
				WordWrap = gridOptions.WordWrap,
				SelectionDataMembers = GetSelectionDataMembers(),
				RowIdentificatorDataMembers = GetRowIdentificatorDataMembers(),
				AllColumnsFixed = columnViewModels.All(column => column.WidthType != GridColumnFixedWidthType.Weight),
				ColumnAxisName = DashboardDataAxisNames.DefaultAxis,
				HasDimensionColumns = columns.Any(column => column is GridDimensionColumn),
				SparklineAxisName = SparklineArgument != null ? DashboardDataAxisNames.SparklineAxis : null
			};
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			columns.SaveToXml(element);
			sparklineArgumentBox.SaveToXml(element);
			XElement optionsElement = new XElement(xmlGridOptions);
			gridOptions.SaveToXml(optionsElement);
			element.Add(optionsElement);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			columns.LoadFromXml(element);
			sparklineArgumentBox.LoadFromXml(element);
			XElement optionsElement = element.Element(xmlGridOptions);
			if(optionsElement != null)
				gridOptions.LoadFromXml(optionsElement);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			columns.OnEndLoading(this);
			sparklineArgumentBox.OnEndLoading();
			FormatRules.OnEndLoading();
		}
		protected internal override bool CanSpecifyMeasureNumericFormat(Measure measure) {
			bool canSpecify = false;
			foreach(GridColumnBase column in columns) {
				GridDeltaColumn deltaColumn = column as GridDeltaColumn;
				if(deltaColumn != null) {
					Measure actual = deltaColumn.ActualValue;
					Measure target = deltaColumn.TargetValue;
					bool isActualValueType = actual == null || target == null || deltaColumn.DeltaOptions.ValueType == DeltaValueType.ActualValue;
					if(actual == measure || (actual == null && target == measure)) {
						canSpecify = isActualValueType;
						break;
					}
					if(actual != null && actual == measure) {
						canSpecify = false;
						break;
					}
				}
				GridMeasureColumn measureColumn = column as GridMeasureColumn;
				if(measureColumn != null && measureColumn.Measure == measure) {
					canSpecify = true;
					break;
				}
				GridSparklineColumn sparklineColumn = column as GridSparklineColumn;
				if(sparklineColumn != null && sparklineColumn.Measure == measure) {
					canSpecify = true;
					break;
				}
			}
			return canSpecify && base.CanSpecifyMeasureNumericFormat(measure);
		}
		protected internal override bool CanSpecifyDimensionDateTimeFormat(Dimension dimension) {
			return Object.ReferenceEquals(dimension, SparklineArgument) ? false : base.CanSpecifyDimensionDateTimeFormat(dimension);
		}
		IList<GridDimensionColumn> GetSelectionColumns() {
			List<GridDimensionColumn> gridDimensionColumns = new List<GridDimensionColumn>();
			foreach(GridColumnBase column in columns) {
				GridDimensionColumn gridDimensionColumn = column as GridDimensionColumn;
				if(gridDimensionColumn == null && gridDimensionColumns.Count > 0)
					break;
				if(gridDimensionColumn != null && gridDimensionColumn.Dimension != null)
					gridDimensionColumns.Add(gridDimensionColumn);
			}
			return gridDimensionColumns;
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			description.AddSparklineArgument(SparklineArgument);
			foreach (GridColumnBase column in Columns)
				column.FillDashboardItemDataDescription(description);
			if(FormatRulesInternal != null)
				foreach(DashboardItemFormatRule rule in FormatRulesInternal)
					description.AddFormatRule(rule);
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			SparklineArgument = description.SparklineArgument;
			if (description.Latitude != null)
				Columns.Add(new GridDimensionColumn(description.Latitude));
			if (description.Longitude != null)
				Columns.Add(new GridDimensionColumn(description.Longitude));
			foreach (Dimension dimension in description.MainDimensions)
				Columns.Add(new GridDimensionColumn(dimension));
			foreach (Dimension dimension in description.AdditionalDimensions)
				Columns.Add(new GridDimensionColumn(dimension));
			HiddenDimensions.AddRange(description.TooltipDimensions);
			HiddenMeasures.AddRange(description.TooltipMeasures);
			foreach (MeasureDescription measureBox in description.MeasureDescriptions)
				if (measureBox.MeasureType == MeasureDescriptionType.Delta) {
					GridDeltaColumn deltaColumn = new GridDeltaColumn(measureBox.ActualValue, measureBox.TargetValue);
					deltaColumn.DeltaOptions.Assign(measureBox.DeltaOptions);
					Columns.Add(deltaColumn);
				}
				else
					Columns.Add(new GridMeasureColumn(measureBox.Value));
			foreach(RuleDescription ruleDescription in description.FormatRules) {
				GridItemFormatRule rule = new GridItemFormatRule();
				AssignRuleDescription(ruleDescription, rule);
				rule.DataItem = ruleDescription.Item;
				rule.DataItemApplyTo = ruleDescription.ItemApplyTo;
				rule.ApplyToRow = ruleDescription.ApplyToRow;
				FormatRules.Add(rule);
			}
		}
		protected override DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo(Dimension dimension) {
			if(dimension == SparklineArgument)
				return DimensionGroupIntervalInfo.Default;
			return base.GetDimensionGroupIntervalInfo(dimension);
		}
		protected override void FillEditNameDescriptions(IList<EditNameDescription> descriptions) {
			base.FillEditNameDescriptions(descriptions);
			descriptions.Add(new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionColumns), columns));
		}
		bool HasDeltaColumns() {
			foreach(GridColumnBase column in columns) {
				if(column is GridDeltaColumn)
					return true;
			}
			return false;
		}
		bool IsDeltaOrSparklineColumnPart(DataItem dataItem) {
			Measure measure = dataItem as Measure;
			if(measure != null) {
				foreach(GridColumnBase column in columns) {
					if(IsDeltaColumnPart(column, measure))
						return true;
					GridSparklineColumn sparklineColumn = column as GridSparklineColumn;
					if(sparklineColumn != null && measure == sparklineColumn.Measure)
						return true;
				}
			}
			return false;
		}
		bool IsDeltaColumnPart(DataItem dataItem) {
			Measure measure = dataItem as Measure;
			if(measure != null) {
				foreach(GridColumnBase column in columns) {
					if(IsDeltaColumnPart(column, measure))
						return true;
				}
			}
			return false;
		}
		bool IsDeltaColumnPart(GridColumnBase column, Measure measure) {
			GridDeltaColumn deltaColumn = column as GridDeltaColumn;
			if(deltaColumn != null && (measure == deltaColumn.ActualValue || measure == deltaColumn.TargetValue))
				return true;
			return false;
		}
		bool IsImageColumnPart(DataItem dataItem) {
			Measure measure = dataItem as Measure;
			if(measure != null) {
				foreach(GridColumnBase column in columns) {
					GridDeltaColumn deltaColumn = column as GridDeltaColumn;
					if(deltaColumn != null && (measure == deltaColumn.ActualValue || measure == deltaColumn.TargetValue))
						return true;
					GridMeasureColumn barColumn = column as GridMeasureColumn;
					if(barColumn != null && barColumn.DisplayMode == GridMeasureColumnDisplayMode.Bar && (measure == barColumn.Measure))
						return true;
					GridSparklineColumn sparklineColumn = column as GridSparklineColumn;
					if(sparklineColumn != null && measure == sparklineColumn.Measure)
						return true;
				}
			}
			return false;
		}
	}
}
