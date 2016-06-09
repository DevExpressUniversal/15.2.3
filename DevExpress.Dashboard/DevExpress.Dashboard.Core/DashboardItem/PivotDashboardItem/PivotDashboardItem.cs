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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	[
	DashboardItemType(DashboardItemType.Pivot)
	]
	public partial class PivotDashboardItem : DataDashboardItem, IPivotDashboardItem {
		const string xmlColumns = "Columns";
		const string xmlColumn = "Column";
		const string xmlRows = "Rows";
		const string xmlRow = "Row";
		const string xmlValues = "Values";
		const string xmlValue = "Value";
		const string xmlAutoExpandColumnGroups = "AutoExpandColumnGroups";
		const string xmlAutoExpandRowGroups = "AutoExpandRowGroups";
		const string xmlShowColumnGrandTotals = "ShowColumnGrandTotals";
		const string xmlShowRowGrandTotals = "ShowRowGrandTotals";
		const string xmlShowColumnTotals = "ShowColumnTotals";
		const string xmlShowRowTotals = "ShowRowTotals";
		internal const bool DefaultAutoExpandGroups = false;
		internal const bool DefaultShowGrandTotals = true;
		internal const bool DefaultShowTotals = true;
		readonly DataItemNameRepository namesRepository;
		readonly DimensionCollection columns = new DimensionCollection();
		readonly DimensionCollection rows = new DimensionCollection();
		readonly MeasureCollection values = new MeasureCollection();
		PivotExpandCollapseActionController columnExpandCollapseActionController = new PivotExpandCollapseActionController(!DefaultAutoExpandGroups);
		PivotExpandCollapseActionController rowExpandCollapseActionController = new PivotExpandCollapseActionController(!DefaultAutoExpandGroups);
		bool autoExpandColumnGroups = DefaultAutoExpandGroups;
		bool autoExpandRowGroups = DefaultAutoExpandGroups;
		bool showColumnGrandTotals = DefaultShowGrandTotals;
		bool showRowGrandTotals = DefaultShowGrandTotals;
		bool showColumnTotals = DefaultShowTotals;
		bool showRowTotals = DefaultShowTotals;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PivotDashboardItemColumns"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor))
		]
		public DimensionCollection Columns { get { return columns; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PivotDashboardItemRows"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor))
		]
		public DimensionCollection Rows { get { return rows; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PivotDashboardItemValues"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor))
		]
		public MeasureCollection Values { get { return values; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PivotDashboardItemAutoExpandColumnGroups"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultAutoExpandGroups)
		]
		public bool AutoExpandColumnGroups {
			get { return autoExpandColumnGroups; }
			set {
				if(value != autoExpandColumnGroups) {
					autoExpandColumnGroups = value;
					columnExpandCollapseActionController = new PivotExpandCollapseActionController(!autoExpandColumnGroups);
					OnChanged(ChangeReason.ClientData);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PivotDashboardItemAutoExpandRowGroups"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultAutoExpandGroups)
		]
		public bool AutoExpandRowGroups {
			get { return autoExpandRowGroups; }
			set {
				if(value != autoExpandRowGroups) {
					autoExpandRowGroups = value;
					rowExpandCollapseActionController = new PivotExpandCollapseActionController(!autoExpandRowGroups);
					OnChanged(ChangeReason.ClientData);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PivotDashboardItemShowColumnGrandTotals"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultShowGrandTotals)
		]
		public bool ShowColumnGrandTotals {
			get {
				return showColumnGrandTotals;
			}
			set {
				if(value != showColumnGrandTotals) {
					showColumnGrandTotals = value;
					OnChanged(ChangeReason.ClientData);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PivotDashboardItemShowRowGrandTotals"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultShowGrandTotals)
		]
		public bool ShowRowGrandTotals {
			get {
				return showRowGrandTotals;
			}
			set {
				if (value != showRowGrandTotals) {
					showRowGrandTotals = value;
					OnChanged(ChangeReason.ClientData);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PivotDashboardItemShowColumnTotals"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultShowTotals)
		]
		public bool ShowColumnTotals {
			get {
				return showColumnTotals;
			}
			set {
				if (value != showColumnTotals) {
					showColumnTotals = value;
					OnChanged(ChangeReason.ClientData);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PivotDashboardItemShowRowTotals"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultShowTotals)
		]
		public bool ShowRowTotals {
			get {
				return showRowTotals;
			}
			set {
				if (value != showRowTotals) {
					showRowTotals = value;
					OnChanged(ChangeReason.ClientData);
				}
			}
		}
		[
		Browsable(false), 
		EditorBrowsable(EditorBrowsableState.Never)
		]
		public new bool IsMasterFilterCrossDataSource {
			get { return base.IsMasterFilterCrossDataSource; }
			set { base.IsMasterFilterCrossDataSource = value; }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PivotDashboardItemInteractivityOptions"),
#endif
 Category(CategoryNames.Interactivity),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public FilterableDashboardItemInteractivityOptions InteractivityOptions { get { return InteractivityOptionsBase; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public DataItemNameRepository NamesRepository { get { return namesRepository; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PivotDashboardItemFormatRules"),
#endif
 Category(CategoryNames.Data),
		Editor(TypeNames.FormatRulesManageEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		]
		public PivotItemFormatRuleCollection FormatRules { get { return (PivotItemFormatRuleCollection)base.FormatRulesInternal; } }
		protected internal override bool SingleMasterFilterEnabled { get { return false; } }
		protected internal override bool MultipleMasterFilterEnabled { get { return false; } }
		protected internal override bool IsDrillDownEnabled { 
			get { return false; }
			set {
			}
		}
		protected internal override DashboardItemMasterFilterMode MasterFilterMode {
			get { return DashboardItemMasterFilterMode.None; }
			set {
			}
		}
		protected internal override string CaptionPrefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNamePivotItem); } }
		protected override IEnumerable<DataDashboardItemDescription> ItemDescriptions {
			get { 
				yield return new DataDashboardItemDescription(
					DashboardLocalizer.GetString(DashboardStringId.DescriptionValues), 
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemValue),
					ItemKind.Measure, values
				);
				yield return new DataDashboardItemDescription(
					DashboardLocalizer.GetString(DashboardStringId.DescriptionColumns), 
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemColumn),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionColumns), 
					ItemKind.Dimension, columns
				);
				yield return new DataDashboardItemDescription(
					DashboardLocalizer.GetString(DashboardStringId.DescriptionRows),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemRow),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionRows),
					ItemKind.Dimension, rows
				);
			}
		}
		protected internal override bool HasDataItems { get { return base.HasDataItems || columns.Count > 0 || rows.Count > 0 || values.Count > 0; } }
		internal PivotExpandCollapseActionController ColumnExpandCollapseActionController { get { return columnExpandCollapseActionController; } }
		internal PivotExpandCollapseActionController RowExpandCollapseActionController { get { return rowExpandCollapseActionController; } }
		public PivotDashboardItem() {
			namesRepository = new DataItemNameRepository(this);
			columns.CollectionChanged += (sender, e) => OnDataItemsChanged(e.AddedItems, e.RemovedItems);
			rows.CollectionChanged += (sender, e) => OnDataItemsChanged(e.AddedItems, e.RemovedItems);
			values.CollectionChanged += (sender, e) => OnDataItemsChanged(e.AddedItems, e.RemovedItems);
		}
		protected override IList<Dimension> GetFormatRuleAxisItems(bool isSecondAxis) {
			return isSecondAxis ? Rows : Columns;
		}
		protected override IList<Measure> GetFormatRuleMeasures() {
			return Values;
		}
		protected override IFormatRuleCollection CreateFormatRules() {
			PivotItemFormatRuleCollection c = new PivotItemFormatRuleCollection();
			c.CollectionChanged += OnFormatRulesChanged<PivotItemFormatRule>;
			return c;
		}
		[Obsolete("This method is now obsolete. Use the DataItem.Name property instead.")]
		public void SetValueName(Measure value, string name) {
			value.Name = name;
		}
		[Obsolete("This method is now obsolete. Use the DataItem.Name property instead.")]
		public string GetValueName(Measure value) {
			return value.Name;
		}
		internal string[] GetColumnsDataMembers() {
			List<string> dataMembers = new List<string>();
			foreach(Dimension dimension in columns)
				dataMembers.Add(DataItemRepository.GetActualID(dimension));
			return dataMembers.ToArray();
		}
		internal string[] GetRowsDataMembers() {
			List<string> dataMembers = new List<string>();
			foreach(Dimension dimension in rows)
				dataMembers.Add(DataItemRepository.GetActualID(dimension));
			return dataMembers.ToArray();
		}
		internal string[] GetValuesDataMembers() {
			List<string> dataMembers = new List<string>();
			foreach(Measure measure in values)
				dataMembers.Add(DataItemRepository.GetActualID(measure));
			return dataMembers.ToArray();
		}
		internal override bool IsConditionalFormattingApplyToAllowed(DataItem itemCalculateBy, DataItem itemApplyTo, FormatConditionBase condition) {
			return base.IsConditionalFormattingApplyToAllowed(itemCalculateBy, itemApplyTo, condition) && (itemCalculateBy == itemApplyTo || itemCalculateBy is Measure || condition is IEvaluatorRequired);
		}
		protected internal override void SetState(DashboardItemState state) {
			base.SetState(state);
			PivotDashboardItemExpandCollapseState expandCollapseState = state.PivotExpandCollapseState;
			if(expandCollapseState == null)
				expandCollapseState = new PivotDashboardItemExpandCollapseState(new PivotAreaExpandCollapseState(null, null), new PivotAreaExpandCollapseState(null, null));
			columnExpandCollapseActionController.SetState(expandCollapseState.ColumnState);
			rowExpandCollapseActionController.SetState(expandCollapseState.RowState);
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			PivotDashboardItemViewModel viewModel = new PivotDashboardItemViewModel(this, GetPivotColumnViewModels(GetVisibleDimensions(true)),
				GetPivotColumnViewModels(GetVisibleDimensions(false)), GetPivotValueViewModels(values));
			viewModel.ShowColumnGrandTotals = ShowColumnGrandTotals;
			viewModel.ShowRowGrandTotals = ShowRowGrandTotals;
			viewModel.ShowColumnTotals = ShowColumnTotals;
			viewModel.ShowRowTotals = ShowRowTotals;
			return viewModel;
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			columns.SaveToXml(element, xmlColumns, xmlColumn);
			rows.SaveToXml(element, xmlRows, xmlRow);
			values.SaveToXml(element, xmlValues, xmlValue);
			if(AutoExpandColumnGroups != DefaultAutoExpandGroups)
				element.Add(new XElement(xmlAutoExpandColumnGroups, autoExpandColumnGroups));
			if(AutoExpandRowGroups != DefaultAutoExpandGroups)
				element.Add(new XElement(xmlAutoExpandRowGroups, autoExpandRowGroups));
			if(ShowColumnGrandTotals != DefaultShowGrandTotals)
				element.Add(new XElement(xmlShowColumnGrandTotals, showColumnGrandTotals));
			if(ShowRowGrandTotals != DefaultShowGrandTotals)
				element.Add(new XElement(xmlShowRowGrandTotals, showRowGrandTotals));
			if (ShowColumnTotals != DefaultShowTotals)
				element.Add(new XElement(xmlShowColumnTotals, showColumnTotals));
			if (ShowRowTotals != DefaultShowTotals)
				element.Add(new XElement(xmlShowRowTotals, showRowTotals));
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			columns.LoadFromXml(element, xmlColumns, xmlColumn);
			rows.LoadFromXml(element, xmlRows, xmlRow);
			values.LoadFromXml(element, xmlValues, xmlValue);
			XElement autoExpandColumnGroupsElement = element.Element(xmlAutoExpandColumnGroups);
			if(autoExpandColumnGroupsElement != null)
				autoExpandColumnGroups = XmlHelper.FromString<bool>(autoExpandColumnGroupsElement.Value);
			XElement autoExpandRowGroupsElement = element.Element(xmlAutoExpandRowGroups);
			if(autoExpandRowGroupsElement != null)
				autoExpandRowGroups = XmlHelper.FromString<bool>(autoExpandRowGroupsElement.Value);
			XElement showColumnGrandTotalsElement = element.Element(xmlShowColumnGrandTotals);
			if(showColumnGrandTotalsElement != null)
				showColumnGrandTotals = XmlHelper.FromString<bool>(showColumnGrandTotalsElement.Value);
			XElement showRowGrandTotalsElement = element.Element(xmlShowRowGrandTotals);
			if(showRowGrandTotalsElement != null)
				showRowGrandTotals = XmlHelper.FromString<bool>(showRowGrandTotalsElement.Value);
			XElement showColumnTotalsElement = element.Element(xmlShowColumnTotals);
			if (showColumnTotalsElement != null)
				showColumnTotals = XmlHelper.FromString<bool>(showColumnTotalsElement.Value);
			XElement showRowTotalsElement = element.Element(xmlShowRowTotals);
			if (showRowTotalsElement != null)
				showRowTotals = XmlHelper.FromString<bool>(showRowTotalsElement.Value);
			namesRepository.LoadFromXml(element);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			columns.OnEndLoading(DataItemRepository, this);
			rows.OnEndLoading(DataItemRepository, this);
			values.OnEndLoading(DataItemRepository, this);
			namesRepository.OnEndLoading();
			columnExpandCollapseActionController = new PivotExpandCollapseActionController(!autoExpandColumnGroups);
			rowExpandCollapseActionController = new PivotExpandCollapseActionController(!autoExpandRowGroups);
			FormatRules.OnEndLoading();
		}
		protected override FilterableDashboardItemInteractivityOptions CreateInteractivityOptions() {
			return new FilterableDashboardItemInteractivityOptions(false);
		}
		protected override SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			if(values.Contains(measure))
				return SummaryTypeInfo.Text;
			return base.GetSummaryTypeInfo(measure);
		}
		protected internal override void PrepareState(DashboardItemState state) {
			base.PrepareState(state);
			PivotAreaExpandCollapseState columnState = columnExpandCollapseActionController.GetState();
			PivotAreaExpandCollapseState rowState = rowExpandCollapseActionController.GetState();
			if(columnState.Values.Count != 0 || columnState.HiddenValues.Count != 0 || rowState.Values.Count != 0 || rowState.HiddenValues.Count != 0)
				state.PivotExpandCollapseState = new PivotDashboardItemExpandCollapseState(columnState, rowState);
		}
		protected internal override object GetActualValue(Dimension dimension, object value) {
			return base.GetActualValue(dimension, DashboardSpecialValuesInternal.FromSpecialValue(value));
		}
		internal override bool IsBarConditionalFormattingCalculateAllowed(DataItem itemCalculateBy) {
			return !IsColumnOrRow(itemCalculateBy);
		}
		bool IsColumnOrRow(DataItem itemCalculateBy) {
			foreach(Dimension column in Columns)
				if(column == itemCalculateBy)
					return true;
			foreach(Dimension row in Rows)
				if(row == itemCalculateBy)
			return true;
			return false;
		}
		List<Dimension> GetVisibleDimensions(bool isColumn) {
			List<Dimension> dimensions = isColumn ? new List<Dimension>(columns) : new List<Dimension>(rows);
			return dimensions;
		}
		PivotColumnViewModel CreateColumnViewModel(DataItem dataItem) {
			return new PivotColumnViewModel() {
				Caption = dataItem.DisplayName,
				DataId = dataItem.ActualId,
				Format = dataItem.CreateValueFormatViewModel()
			};
		}
		IList<PivotColumnViewModel> GetPivotValueViewModels(IEnumerable<Measure> dataItems) {
			List<PivotColumnViewModel> columns = new List<PivotColumnViewModel>();
			foreach(Measure dataItem in dataItems)
				columns.Add(CreateColumnViewModel(dataItem));
			return columns;
		}
		IList<PivotColumnViewModel> GetPivotColumnViewModels(IEnumerable<Dimension> dataItems) {
			List<PivotColumnViewModel> columns = new List<PivotColumnViewModel>();
			foreach(Dimension dataItem in dataItems)
				columns.Add(CreateColumnViewModel(dataItem));
			return columns;
		}
		bool IPivotDashboardItem.ChangeValueExpandState(bool isColumn, object[] values, bool expanded) {
			PivotExpandCollapseActionController controller = isColumn ? columnExpandCollapseActionController : rowExpandCollapseActionController;
			values = GetActualValues(isColumn, values);
			IList<object[]> actionValues = controller.PerformChangeAction(values, expanded);
			return true;
		}
		void IPivotDashboardItem.ApplyExpandingState(PivotDashboardItemExpandCollapseState previousState, object[] expandingValues, bool isExpandingColumn) {
			Func<bool, PivotAreaExpandCollapseState, PivotAreaExpandCollapseState> convertState = (isColumn, state) => {
				PivotAreaExpandCollapseState newState = new PivotAreaExpandCollapseState(new List<object[]>(), null);
				if(state.Values != null) {
					foreach(object[] arr in state.Values) {
						newState.Values.Add(GetActualValues(isColumn, arr));
					}
				}
				return newState;
			};
			previousState = previousState ?? new PivotDashboardItemExpandCollapseState();
			if(expandingValues != null && expandingValues.Length > 0) {
				PivotAreaExpandCollapseState state = isExpandingColumn ? previousState.ColumnState : previousState.RowState;
				state.Values = state.Values == null ? new List<object[]>() : state.Values.Where(expand => expand.Length > 0).ToList();
				if(!state.Values.Any(expand => expand.SequenceEqual(expandingValues))) {
					state.Values.Add(expandingValues);
				}
			}
			columnExpandCollapseActionController.SetState(convertState(true, previousState.ColumnState));
			rowExpandCollapseActionController.SetState(convertState(false, previousState.RowState));
		}
		IDictionary<string, object> IPivotDashboardItem.GetExpandFilter(bool isColumn, object[] values, bool conversionRequired) {
			Dictionary<string, object> expandingStorageFilter = new Dictionary<string, object>();
			IList<Dimension> dimensions = isColumn ? columns : rows;
			values = conversionRequired ? GetActualValues(isColumn, values) : values;
			for(int i = 0; i < values.Length; i++) {
				Dimension dimension = dimensions[i];
				expandingStorageFilter[dimension.ActualId] = values[i];
			}
			return expandingStorageFilter;
		}
		object[] GetActualValues(bool isColumn, object[] values) {
			object[] actualValues = new object[values.Length];
			IList<Dimension> dimensions = isColumn ? columns : rows;
			for(int i = 0; i < values.Length; i++)
				actualValues[i] = (i < dimensions.Count) ? GetActualValue(dimensions[i], values[i]) : values[i];
			return actualValues;
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			DimensionDescriptorInternalCollection columnDimensions = new DimensionDescriptorInternalCollection();
			DimensionDescriptorInternalCollection rowDimensions = new DimensionDescriptorInternalCollection();
			foreach(Measure measure in Values)
				builder.AddMeasure(measure);
			builder.SetRowHierarchyDimensions(DashboardDataAxisNames.PivotRowAxis, Rows);
			builder.SetColumnHierarchyDimensions(DashboardDataAxisNames.PivotColumnAxis, Columns);
			builder.AddFormatConditionMeasureDescriptors(FormatRules);
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			description.HasAdditionalDimensions = true;
			foreach (Dimension dimension in Rows)
				if (dimension != null)
					description.AddMainDimension(dimension);
			foreach (Dimension dimension in Columns)
				if (dimension != null)
					description.AddAdditionalDimension(dimension);
			foreach (Measure measure in Values)
				if (measure != null)
					description.AddMeasure(measure);
			if(FormatRulesInternal != null)
				foreach(DashboardItemFormatRule rule in FormatRulesInternal)
					description.AddFormatRule(rule);
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			AssignDimension(description.Latitude, Rows);
			AssignDimension(description.Longitude, Columns);
			if (!description.HasAdditionalDimensions && description.MainDimensions.Count != 0 && description.AdditionalDimensions.Count == 0) {
				List<Dimension> dimensions = new List<Dimension>();
				dimensions.AddRange(description.MainDimensions);
				DataFieldType fieldType = dimensions[0].DataFieldType;
				while (dimensions.Count > 0) {
					if (dimensions[0].DataFieldType == fieldType) {
						Rows.Add(dimensions[0]);
						dimensions.RemoveAt(0);
					}
					else
						break;
				}
				Columns.AddRange(dimensions);
			}
			else {
				Rows.AddRange(description.MainDimensions);
				Columns.AddRange(description.AdditionalDimensions);
			}
			AssignDimension(description.SparklineArgument, HiddenDimensions);
			HiddenDimensions.AddRange(description.TooltipDimensions);
			HiddenMeasures.AddRange(description.TooltipMeasures);
			foreach (Measure measure in description.Measures)
				AssignMeasure(measure, Values);
			foreach(RuleDescription ruleDescription in description.FormatRules) {
				PivotItemFormatRule rule = new PivotItemFormatRule();
				AssignRuleDescription(ruleDescription, rule);
				rule.DataItem = ruleDescription.Item;
				rule.DataItemApplyTo = ruleDescription.ItemApplyTo;
				rule.ApplyToRow = ruleDescription.ApplyToRow;
				FormatRules.Add(rule);
			}
		}
		protected override DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo(Dimension dimension) {
			if(columns.Contains(dimension) || rows.Contains(dimension))
				return DimensionGroupIntervalInfo.Default;
			return base.GetDimensionGroupIntervalInfo(dimension);
		}
		protected override void FillEditNameDescriptions(IList<EditNameDescription> descriptions) {
			base.FillEditNameDescriptions(descriptions);
			descriptions.Add(new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionValues), values));
		}
	}
}
