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
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.Printing;
using DevExpress.DashboardCommon.Server;
using DevExpress.DashboardCommon.Service;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Data.Filtering;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Native;
using DevExpress.DataAccess.Native.Data;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public abstract partial class DataDashboardItem : DashboardItem, IMasterFilter, IDataItemRepositoryProvider, IDataSourceSchemaProvider, IDataItemContext, 
		IDataItemContainerContext, IDataItemSerializationContext, IColorSchemeContext, IFormatRulesContext, IFiltersProvider {
		internal const string ColorMeasure = "ColorMeasure";
		const string xmlIsMasterFilterCrossDataSource = "IsMasterFilterCrossDataSource";
		const string xmlDataSource = "DataSource";
		const string xmlDataMember = "DataMember";
		const string xmlDataItems = "DataItems";
		const string xmlFilterCriteria = "FilterCriteria";
		const string xmlFilterString = "FilterString";
		const string xmlHiddenDimensions = "HiddenDimensions";
		const string xmlDimension = "Dimension";
		const string xmlHiddenMeasures = "HiddenMeasures";
		const string xmlMeasure = "Measure";
		const string xmlIgnoreMasterFilters = "IgnoreMasterFilters";
		const string xmlColoringOptions = "ColoringOptions";
		const bool DefaultIsMasterFilterCrossDataSource = false;
		internal static string CorrectColorMeasureId(string measureID) {
			return ColorMeasure + "_" + measureID;
		}
		readonly DataItemRepository dataItemRepository;
		readonly DimensionCollection hiddenDimensions = new DimensionCollection();
		readonly MeasureCollection hiddenMeasures = new MeasureCollection();
		readonly NotifyingCollection<Dimension> externalMasterFiltersDimensions;
		readonly ColorSchemeContainer colorSchemeContainer;
		readonly Dictionary<string, DataItemDefinition> renamingMap = new Dictionary<string, DataItemDefinition>();
		readonly Locker renamingMapLocker = new Locker();
		readonly IFormatRuleCollection formatRules;
		IDashboardDataSource dataSource;
		string dataMember;
		string dataSourceName;
		string filterString;
		FilterAgent filterAgent;
		bool isEmptyValueOnLastMasterFilterChanged = true;
		bool isMasterFilterCrossDataSource = DefaultIsMasterFilterCrossDataSource;
		bool calculateTotals = false;
		FilterableDashboardItemInteractivityOptions interactivityOptions;
		DashboardItemColoringOptions coloringOptions;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataDashboardItemIsMasterFilterCrossDataSource"),
#endif
		Category(CategoryNames.Interactivity),
		DefaultValue(DefaultIsMasterFilterCrossDataSource)
		]
		public bool IsMasterFilterCrossDataSource {
			get { return isMasterFilterCrossDataSource; }
			set {
				if(value != isMasterFilterCrossDataSource) {
					isMasterFilterCrossDataSource = value;
					OnChanged(ChangeReason.InteractivityCrossDataSource);
					if(!Loading && IsMasterFilterEnabled)
						RaiseMasterFilterChangedInternal();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataDashboardItemIgnoreMasterFilters"),
#endif
		Category(CategoryNames.Interactivity),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the InteractivityOptions.IgnoreMasterFilters property instead.")
		]
		public bool IgnoreMasterFilters {
			get { return InteractivityOptionsBase.IgnoreMasterFilters; }
			set { InteractivityOptionsBase.IgnoreMasterFilters = value; }
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DataItemRepository DataItemRepository { get { return dataItemRepository; } }
		[
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Obsolete("This method is now obsolete.")
		]
		public bool IsDataReduced { get { return false; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataDashboardItemHiddenDimensions"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor)),
		]
		public DimensionCollection HiddenDimensions { get { return hiddenDimensions; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataDashboardItemHiddenMeasures"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor))
		]
		public MeasureCollection HiddenMeasures { get { return hiddenMeasures; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataDashboardItemDataSource"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.DataSourceListConverter),
		RefreshProperties(RefreshProperties.Repaint),
		DefaultValue(null)
		]
		public IDashboardDataSource DataSource {
			get { return dataSource; }
			set {
				if(value != dataSource) {
					UnloadDataSource();
					dataSource = value;
					dataSourceName = null;
					SynchronizeDataSource();
					LoadDataSource();
				}
			}
		}		
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataDashboardItemDataMember"),
#endif
		Category(CategoryNames.Data),
		Localizable(false),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.DashboardItemDataMemberListConverter),
		DefaultValue(null)
		]
		public string DataMember {
			get {
				return dataMember;
			}
			set {
				if(value != dataMember) {
					UnloadDataSource();
					dataMember = value;
					LoadDataSource();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataDashboardItemFilterCriteria"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		Obsolete("This property is now obsolete. Use the FilterString property instead.")
		]
		public CriteriaOperator FilterCriteria {
			get {
				if(string.IsNullOrEmpty(filterString))
					return null;
				return CriteriaOperator.Parse(filterString);
			}
			set {
				SetFilterStringInternal(ReferenceEquals(value, null) ? null : value.ToString());
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DataDashboardItemFilterString"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(null),
		Localizable(false),
		Editor(TypeNames.DashboardItemFilterCriteriaEditor, typeof(UITypeEditor))
		]
		public string FilterString {
			get { return filterString; }
			set {
				SetFilterStringInternal(value);
			}
		}
		internal string DataSourceName { get { return dataSource != null ? dataSource.ComponentName : dataSourceName; } }
		internal IDataSourceSchema DataSourceSchema { get { return dataSource != null ? dataSource.GetDataSourceSchema(DataMember) : null; } }
		internal IFormatRuleCollection FormatRulesInternal { get { return formatRules; } }
		internal bool SupportsConditionalFormatting { get { return formatRules != null; } }
		internal IFilter ExternalFilter { get; set; } 
		internal CriteriaOperator ActualFilterCriteria { get { return GetFullFilterCriteria(new EmptyParametersProvider()); } }
		internal virtual bool NeedSelfUpdateOnMasterFilter { get { return false; } }
		internal IEnumerable<DataDashboardItemDescription> Descriptions {
			get {
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DataItemsHeader), null, HeaderKind, null);
				foreach (DataDashboardItemDescription description in ItemDescriptions)
					yield return description;
				yield return new DataDashboardItemDescription(null, null, ItemKind.Splitter, null);
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.HiddenDataItemsHeader), null, ItemKind.Header, null);
				yield return new DataDashboardItemDescription(
					DashboardLocalizer.GetString(DashboardStringId.DescriptionDimensions),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemDimension),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionDimensions),
					ItemKind.Dimension, hiddenDimensions
				);
				yield return new DataDashboardItemDescription(
					DashboardLocalizer.GetString(DashboardStringId.DescriptionMeasures),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemMeasure),
					ItemKind.Measure, hiddenMeasures
				);
			}
		}
		internal bool IsSelectionEnabled {
			get {
				IDrillDownController controller = GetDrillDownController();
				return IsMasterFilterEnabled || (controller != null && controller.CanPerformDrillDown);
			}
		}
		internal bool IsOlapDataSource { get { return DataSource != null && DataSource.GetIsOlap(); } }
		internal bool IsColoringSupported { get { return CanColorByDimensions || CanColorByMeasures; } }
		internal virtual bool IsGloballyColored { get { return IsColoringSupported && ColoringOptionsBase.UseGlobalColors; } }
		internal IMasterFilterParameters MasterFilterParameters {
			get {
				IMasterFilterParameters parameters = null;
				if (Dashboard != null)
					parameters = Dashboard.GetMasterFilterParameters(ComponentName);
				if (parameters == null)
					parameters = new EmptyMasterFilterParameters();
				return parameters;
			}
		}
		protected internal ColorSchemeContainer ColorSchemeContainer { get { return colorSchemeContainer; } }
		protected internal ColorScheme ColorSchemeInternal { get { return ColorSchemeContainer.Scheme; } }
		protected internal virtual Dimension[] ColoringDimensions { get { return GetColoringDimensions(IsDrillDownEnabled, Dimensions); } }
		protected internal virtual Dictionary<string, string> ColorMeasureDescriptorsInfo { get { return null; } }
		protected internal Dimension CurrentDrillDownDimension {
			get {
				IDrillDownParameters parameters = GetDrillDownParameters();
				if (parameters != null)
					return parameters.CurrentDrillDownDimension;
				return null;
			}
		}
		protected internal FilterableDashboardItemInteractivityOptions InteractivityOptionsBase {
			get {
				if (interactivityOptions == null) {
					interactivityOptions = CreateInteractivityOptions();
					interactivityOptions.Changed += OnInteractivityOptionsChanged;
				}
				return interactivityOptions;
			}
		}
		protected internal DashboardItemColoringOptions ColoringOptionsBase {
			get {
				if (coloringOptions == null) {
					coloringOptions = CreateColoringOptions();
					coloringOptions.ChangeService = this;
				}
				return coloringOptions;
			}
		}
		protected internal virtual IList<Dimension> SelectionDimensionList { get { return new List<Dimension>(); } }
		protected internal virtual bool HasDataItems { get { return hiddenDimensions.Count > 0 || hiddenMeasures.Count > 0; } }
		protected internal override string ElementCaption {
			get {
				StringBuilder builder = new StringBuilder();
				builder.Append(ActualCaption);
				IElementContainer elementContainer = ElementContainer;
				if (elementContainer != null && elementContainer.ElementSelectionEnabled)
					builder.AppendFormat(DashboardLocalizer.GetString(DashboardStringId.FormatStringDashboardItemCaption), elementContainer.ElementNames[elementContainer.SelectedElementIndex]);
				return builder.ToString();
			}
		}
		protected internal bool IsMasterFilterEnabled { get { return SingleMasterFilterEnabled || MultipleMasterFilterEnabled; } }
		protected internal virtual bool SingleMasterFilterEnabled { get { return MasterFilterMode == DashboardItemMasterFilterMode.Single; } }
		protected internal virtual bool MultipleMasterFilterEnabled { get { return MasterFilterMode == DashboardItemMasterFilterMode.Multiple; } }
		protected internal virtual bool TopNOthersValueEnabled { get { return !IsOlapDataSource; } }
		protected internal abstract DashboardItemMasterFilterMode MasterFilterMode { get; set; }
		protected internal abstract bool IsDrillDownEnabled { get; set; }
		protected internal IEnumerable<DataItem> DataItems { get { return dataItemRepository.DataItems; } }
		protected internal IEnumerable<Dimension> Dimensions { get { return dataItemRepository.Dimensions; } }
		protected internal IEnumerable<Measure> Measures { get { return dataItemRepository.Measures; } }
		protected internal ICollection<DataItem> UniqueDataItems { get { return dataItemRepository.UniqueDataItems; } }
		protected internal ICollection<Dimension> UniqueDimensions { get { return dataItemRepository.UniqueDimensions; } }
		protected internal ICollection<Measure> UniqueMeasures { get { return dataItemRepository.UniqueMeasures; } }
		protected internal virtual bool CanColorByMeasures { get { return false; } }
		protected internal virtual bool CanColorByDimensions { get { return false; } }
		protected internal bool CanColorByMeasure(Measure measure) {
			return CanColorByMeasures && !IsMeasureHidden(measure);
		}
		protected internal bool CanColorByDimension(Dimension dimension) {
			return CanColorByDimensions && !IsDimensionHidden(dimension);
		}
		protected IEnumerable<Dimension> ExternalMasterFiltersDimensions { get { return externalMasterFiltersDimensions; } }
		protected int? CurrentDrillDownLevel {
			get {
				IDrillDownParameters parameters = GetDrillDownParameters();
				if (parameters != null)
					return parameters.CurrentDrillDownLevel;
				return null;
			}
		}
		protected virtual int PriorityInsideFilterLevel { get { return 0; } }
		protected virtual bool AllowEmptyFilterValues { get { return false; } }
		protected virtual bool IsMasterFilterEmpty { get { return Helper.IsParametersEmpty(MasterFilterParameters); } }
		protected virtual bool CalculateOptimizationFilterRanges { get { return false; } }
		protected virtual ItemKind HeaderKind { get { return ItemKind.Header; } }
		protected abstract IEnumerable<DataDashboardItemDescription> ItemDescriptions { get; }
		protected override IEnumerable<string> InputFilterNames { 
			get { 
				return filterAgent.InputFilter.GetFilterValuesProviders(new DataSourceInfo(dataSource, dataMember)).Cast<DataDashboardItem>().Select(item => item.ComponentName); 
			} 
		}	  
		IFilter InputFilter {
			get {
				if (ExternalFilter != null) {
					return ExternalFilter;
				}
				else {
					return filterAgent == null ? null : filterAgent.InputFilter;
				}
			}
		}
		internal event EventHandler DescriptionsChanged;
		internal event EventHandler<RequestDrillDownParametersEventArgs> RequestDrillDownParameters;
		internal event EventHandler<RequestDrillDownControllerEventArgs> RequestDrillDownController;
		event EventHandler masterFilterChangedInternal;
		protected DataDashboardItem() {
			dataItemRepository = new DataItemRepository(this);
			hiddenDimensions.CollectionChanged += (sender, e) => OnDataItemsChanged(e.AddedItems, e.RemovedItems);
			hiddenMeasures.CollectionChanged += (sender, e) => OnDataItemsChanged(e.AddedItems, e.RemovedItems);
			externalMasterFiltersDimensions = new NotifyingCollection<Dimension>();
			externalMasterFiltersDimensions.CollectionChanged += (sender, e) => AddRemoveDataItems(e.RemovedItems, e.AddedItems);
			colorSchemeContainer = new ColorSchemeContainer(this);
			formatRules = CreateFormatRules();
			CreateFilterAgent();
		}
		public List<string> GetDataMembers() {
			List<string> result = new List<String>();
			foreach (DataItem dataItem in DataItems) {
				Dimension dimension = dataItem as Dimension;
				if (dimension == null || !externalMasterFiltersDimensions.Contains(dimension))
					result.Add(dataItem.DataMember);
			}
			return result;
		}
		public List<Dimension> GetDimensions() {
			List<Dimension> dimensions = new List<Dimension>();
			foreach (Dimension dimension in Dimensions) {
				if (!externalMasterFiltersDimensions.Contains(dimension))
					dimensions.Add(dimension);
			}
			return dimensions;
		}
		public List<Measure> GetMeasures() {
			return new List<Measure>(Measures);
		}
		public void SetDataSource(IDashboardDataSource newDataSource, string newDataMember) {
			bool dataSourceChanged = DataSource != newDataSource;
			bool dataMemberChanged = DataMember != newDataMember;
			if (dataSourceChanged || dataMemberChanged) {
				UnloadDataSource();
				if (dataSourceChanged) {
					dataSource = newDataSource;
					SynchronizeDataSource();
				}
				if (dataMemberChanged)
					dataMember = newDataMember;
				LoadDataSource();
			}
		}
		internal void LockRenamingMap() {
			renamingMapLocker.Lock();
		}
		internal void UnlockRenamingMap() {
			renamingMapLocker.Unlock();
			ApplyRenamingMap();
		}
		internal IList<DataItem> GetInternalDataItems() {
			List<DataItem> dataItems = new List<DataItem>();
			foreach (DataItem dataItem in DataItems) {
				if (!externalMasterFiltersDimensions.Contains(dataItem))
					dataItems.Add(dataItem);
			}
			return dataItems;
		}
		internal virtual bool ColorMeasuresByHue {
			get {
				return CanColorByMeasures && (ColoringOptionsBase.MeasuresColoringMode == ColoringMode.Hue
					|| ColoringOptionsBase.MeasuresColoringMode == ColoringMode.Default && GetColoringMeasuresInternal().Length > 1);
			}
		}
		internal IEnumerable<DimensionDefinition> GetColoringDimensionDefinitions() {
			return Helper.GetUniqueDimensionDefinitions(ColoringDimensions);
		}
		internal Dimension GetDimension(string dataMember) {
			List<Dimension> dimensions = GetDimensions();
			if (dimensions == null)
				return default(Dimension);
			return dimensions.FirstOrDefault(i => i.DataMember == dataMember);
		}
		internal void ChangeDataItemDataMember(string oldDataMember, string newDataMember) {
			foreach (DataItem dataItem in DataItems) {
				if (dataItem.DataMember == oldDataMember) {
					dataItem.DataMember = newDataMember;
				}
			}
		}
		internal virtual bool IsConditionalFormattingCalculateByAllowed(DataItem itemCalculateBy) {
			DataFieldType fieldType = itemCalculateBy.ActualDataFieldType;
			Dimension dimension = itemCalculateBy as Dimension;
			bool hasApplyToItems = true;
			if (dimension != null) {
				if (IsDimensionHidden(dimension))
					hasApplyToItems = false;
			}
			else if (IsMeasureHidden((Measure)itemCalculateBy)) {
				IList<DataItem> applyToAllowedDataItems = GetConditionalFormattingApplyToAllowedDataItems(itemCalculateBy, null);
				hasApplyToItems = (applyToAllowedDataItems.Count > 0);
			}
			return hasApplyToItems &&
				fieldType != DataFieldType.Bool &&
				fieldType != DataFieldType.Enum &&
				fieldType != DataFieldType.Custom &&
				fieldType != DataFieldType.Unknown;
		}
		internal virtual bool IsBarConditionalFormattingCalculateAllowed(DataItem itemCalculateBy) {
			return false;
		}
		internal virtual bool IsConditionalFormattingApplyToAllowed(DataItem itemCalculateBy, DataItem itemApplyTo, FormatConditionBase condition) {
			Dimension dimension = itemApplyTo as Dimension;
			if (dimension != null)
				return !IsDimensionHidden(dimension);
			else
				return !IsMeasureHidden((Measure)itemApplyTo);
		}
		internal IList<DataItem> GetConditionalFormattingApplyToAllowedDataItems(DataItem itemCalculateBy, FormatConditionBase condition) {
			List<DataItem> applyToDataItems = new List<DataItem>();
			foreach (DataItem item in GetInternalDataItems()) {
				if (IsConditionalFormattingApplyToAllowed(itemCalculateBy, item, condition)) {
					applyToDataItems.Add(item);
				}
			}
			return applyToDataItems;
		}
		internal FormattableValue GetDimensionFormatableValue(Dimension dimension, object value) {
			return new FormattableValue {
				Value = dataSource.GetOlapDimensionValueDisplayText(dimension.DataMember, value, dataMember),
				Format = DataSource != null && !DataSource.GetIsSpecificValueFormatSupported() ?
					new ValueFormatViewModel { DataType = ValueDataType.String } :
					dimension.CreateValueFormatViewModel()
			};
		}
		internal void SetCalculateTotals(bool calculateTotals) {
			if (this.calculateTotals != calculateTotals) {
				this.calculateTotals = calculateTotals;
			}
		}
		internal void UpdateDataMembersOnLoading() {
#if !DXPORTABLE
#pragma warning disable 612, 618
			if(dataSource == null || dataSource.DataProvider == null)
				return;
			SqlDataProvider dataProvider = dataSource.DataProvider as SqlDataProvider;
			if(dataProvider == null || dataProvider.DataSelection == null || dataProvider.IsCustomSql)
				return;
			foreach(DataItem dataItem in DataItems) {
				foreach(DataTable dataTable in dataProvider.DataSelection) {
					string tableNameAndDot = string.Format("{0}.", dataTable.TableName);
					if(dataItem.DataMember.StartsWith(tableNameAndDot)) {
						foreach(DataColumn dataColumn in dataTable.Columns) {
							if(dataItem.DataMember == string.Format("{0}{1}", tableNameAndDot, dataColumn.ColumnName)) {
								dataItem.DataMember = dataColumn.ActualName;
								break;
							}
						}
						break;
					}
				}
			}
#pragma warning restore 612, 618
#endif
		}		
		internal FormattableValue GetMasterFilterValue(Dimension dimension, object value) {
			return new FormattableValue {
				Value = dataSource.GetOlapDimensionValueDisplayText(dimension.DataMember, value, dataMember),
				Type = MasterFilterValueType.Value,
				Format = DataSource != null && !DataSource.GetIsSpecificValueFormatSupported() ? new ValueFormatViewModel { DataType = ValueDataType.String } : dimension.CreateValueFormatViewModel()
			};
		}
		internal void EnsureDataSource(DataSourceInfo dataSourceInfo) {
			if (!HasDataItems)
				SetDataSource(null, null);
			else if (DataSource == null && dataSourceInfo != null)
				SetDataSource(dataSourceInfo.DataSource, dataSourceInfo.DataMember);
		}
		internal bool IsDimensionHidden(Dimension dimension) {
			return hiddenDimensions.Contains(dimension);
		}
		internal HierarchicalMetadata GetMetadata(IActualParametersProvider provider) {
			return GetMetadataInternal(false, provider);
		}
		internal string[] GetAxisNames() {
			return (IsDrillDownEnabled || IsMasterFilterEnabled) ? GetCurrentAxisName() : new string[0];
		}
		internal string[] GetDimensionIds() {
			if (IsDrillDownEnabled || IsMasterFilterEnabled) {
				IList<Dimension> selectionDimensions = SelectionDimensionList;
				if (selectionDimensions.Count > 0) {
					if (IsDrillDownEnabled)
						return new string[1] { DataItemRepository.GetActualID(CurrentDrillDownDimension) };
					List<string> dataMembers = new List<string>();
					for (int i = 0; i < selectionDimensions.Count; i++)
						dataMembers.Add(DataItemRepository.GetActualID(selectionDimensions[i]));
					return dataMembers.ToArray();
				}
			}
			return new string[0];
		}
		internal List<string> GetAffectedDashboardItemsByMasterFilterActions() {
			return new List<string>(filterAgent.GetAffected().Select(i => i.Name));
		}
		protected internal virtual bool LastSingleColoredDefinition(DimensionDefinition definition) {
			return LastSingleColoredDefinition(definition, Dimensions.ToList(), IsDrillDownEnabled);
		}
		protected internal virtual ColoringSchemeDefinition GetColoringScheme() {
			ColoringSchemeDefinition schemes = new ColoringSchemeDefinition();
			List<DimensionDefinition> definitionList = new List<DimensionDefinition>(Helper.GetUniqueDimensionDefinitions(Dimensions.Where(dim => dim.ColorByHue)));
			Measure[][] measures = GetUniqueColoringMeasures();
			if (IsDrillDownEnabled)
				AddDrillDownSchemes(schemes, definitionList, new List<DimensionDefinition>(), measures, Dimensions.ToList(), false);
			else
				AddScheme(schemes, definitionList, measures);
			return schemes;
		}
		protected internal virtual bool IsColoringEnabled(DataItem dataItem) {
			if (!IsColoringSupported)
				return false;
			Dimension dimension = dataItem as Dimension;
			if (dimension != null) {
				return !IsDimensionHidden(dimension) && dimension.ColorByHue;
			}
			return !IsMeasureHidden((Measure)dataItem) && ColorMeasuresByHue;
		}
		protected internal virtual bool IsSortingEnabled(Dimension dimension) {
			return !IsDimensionHidden(dimension) && !dimension.TopNOptions.Enabled;
		}
		protected internal virtual bool IsSortingByMeasureEnabled(Dimension dimension, DashboardItemViewModel model) {
			return IsSortingEnabled(dimension);
		}
		protected internal virtual bool CanSpecifySortMode(Dimension dimension) {
			return IsSortingEnabled(dimension);
		}
		protected internal virtual bool CanSpecifyTopNOptions(Dimension dimension) {
			return !IsDimensionHidden(dimension);
		}
		protected internal virtual bool CanSpecifyDimensionNumericFormat(Dimension dimension) {
			return !IsDimensionHidden(dimension) && dimension.CanSpecifyNumericFormat;
		}
		protected internal virtual bool CanSpecifyMeasureNumericFormat(Measure measure) {
			return !IsMeasureHidden(measure) && measure.CanSpecifyNumericFormat;
		}
		protected internal virtual bool CanSpecifyDimensionDateTimeFormat(Dimension dimension) {
			return !IsDimensionHidden(dimension) && dimension.CanSpecifyDateTimeFormat;
		}
		protected internal virtual bool CanSpecifyMeasureDateTimeFormat(Measure measure) {
			return !IsMeasureHidden(measure) && measure.CanSpecifyDateTimeFormat;
		}
		protected internal virtual object GetActualValue(Dimension dimension, object value) {
			if (value != null && !DashboardSpecialValuesInternal.IsDashboardSpecialValue(value) && DataSource != null && !DataSource.GetIsOlap()) {
				Type toType = DataSourceHelper.GetDimensionType(DataSource, DataMember, dimension);
				Type fromType = value.GetType();
				if (toType.IsAssignableFrom(fromType))
					return value;
				TypeConverter converter = TypeDescriptor.GetConverter(toType);
				if (converter.CanConvertFrom(fromType))
					return converter.ConvertFrom(null, CultureInfo.InvariantCulture, value);
				converter = TypeDescriptor.GetConverter(fromType);
				if (converter.CanConvertTo(toType))
					return converter.ConvertTo(null, CultureInfo.InvariantCulture, value, toType);
				try {
					return Convert.ChangeType(value, toType, CultureInfo.InvariantCulture);
				}
				catch { }
			}
			return value;
		}
		protected internal virtual string[] GetSelectionDataMemberDisplayNames() {
			return new string[0];
		}		
		protected DataItemBox<Measure> InitializeMeasureBox(DataItemHolder<Measure> holder, string xmlName) {
			DataItemBox<Measure> box = new DataItemBox<Measure>((IDataItemContext)this, xmlName);
			box.Changed += (sender, e) => {
				OnDataItemsChanged(
					e.NewDataItem != null ? new Measure[] { e.NewDataItem } : new Measure[0],
					e.OldDataItem != null ? new Measure[] { e.OldDataItem } : new Measure[0]);
				holder.OnDataItemChanged(sender, e);
			};
			return box;
		}
		protected DataItemBox<Dimension> InitializeDimensionBox(DataItemHolder<Dimension> holder, string xmlName) {
			DataItemBox<Dimension> box = new DataItemBox<Dimension>((IDataItemContext)this, xmlName);
			box.Changed += (sender, e) => {
				OnDataItemsChanged(
					e.NewDataItem != null ? new Dimension[] { e.NewDataItem } : new Dimension[0],
					e.OldDataItem != null ? new Dimension[] { e.OldDataItem } : new Dimension[0]);
				holder.OnDataItemChanged(sender, e);
			};
			return box;
		}
		protected virtual IList<Dimension> GetFormatRuleAxisItems(bool isSecondAxis) {
			return new List<Dimension>();
		}
		protected virtual IList<Measure> GetFormatRuleMeasures() {
			return new List<Measure>();
		}
		protected virtual IFormatRuleCollection CreateFormatRules() {
			return null;
		}
		protected virtual Measure[][] GetColoringMeasuresInternal() {
			return null;
		}
		protected void OnInteractivityOptionsChanged(object sender, ChangedEventArgs e) {
			if (!Loading) {
				SetupFilterAgent();
				OnChanged(ChangeReason.Interactivity);
			}
		}
		protected virtual bool ShouldRaiseMasterFilterChanged() {
			bool isEmpty = IsMasterFilterEmpty;
			if (!isEmpty || !isEmptyValueOnLastMasterFilterChanged) {
				isEmptyValueOnLastMasterFilterChanged = isEmpty;
				return true;
			}
			return false;
		}
		protected void RaiseDescriptionsChanged() {
			if (!Loading && DescriptionsChanged != null)
				DescriptionsChanged(this, EventArgs.Empty);
		}
		protected void OnFormatRulesChanged<T>(object sender, NotifyingCollectionChangedEventArgs<T> e) where T: DashboardItemFormatRule {
			foreach(T item in e.AddedItems) {
				item.Context = this;
			}
			foreach(T item in e.RemovedItems) {
				item.Context = null;
			}
			if(e.AddedItems.Count > 0 || e.RemovedItems.Count > 0) {
				((IFormatRulesContext)this).OnChanged(null);
			}
		}
		protected virtual DashboardItemColoringOptions CreateColoringOptions() {
			return new DashboardItemColoringOptions();
		}
		protected Measure[][] GetUniqueColoringMeasures() {
			Measure[][] measures = GetColoringMeasures();
			return measures != null
				? measures.GroupBy(measure => {
					MeasureDefinition[] definitions = new MeasureDefinition[measure.Length];
					for (int i = 0; i < measure.Length; i++) {
						definitions[i] = (MeasureDefinition)measure[i].GetDefinition();
					}
					return definitions;
				}, new UnorderedArrayComparer()).Select(group => group.First()).ToArray()
				: null;
		}
		protected bool IsOlapDataSourceDimension(Dimension dimension) {
			return IsOlapDataSource && SelectionDimensionList.Contains(dimension);
		}
		protected virtual bool ColorDimension(Dimension dimension) {
			return false;
		}
		protected virtual SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			if (hiddenMeasures.Contains(measure))
				return SummaryTypeInfo.Hidden;
			return null;
		}
		protected virtual DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo(Dimension dimension) {
			if (hiddenDimensions.Contains(dimension))
				return DimensionGroupIntervalInfo.Default;
			return null;
		}
		protected abstract FilterableDashboardItemInteractivityOptions CreateInteractivityOptions();
		protected virtual string[] GetCurrentAxisName() {
			return new string[] { DashboardDataAxisNames.DefaultAxis };
		}
		protected override void Dispose(bool disposing) {
			if (disposing) {
				UnsubscribeFilterAgentEvents();
				filterAgent = null;
				UnsubscribeDataSourceEvents(dataSource);
			}
			base.Dispose(disposing);
		}
		protected override void OnChangedInternal(ChangedEventArgs e) {
			if (e.Reason == ChangeReason.ClientData) {
				DataItem dataItem = e.Context as DataItem;
				DataItemDefinition oldDefinition = e.Param as DataItemDefinition;
					if (dataItem != null && oldDefinition != null) {
					filterString = NamesCriteriaPatcher.Process(filterString, 
						CreateRenamingMap(new Dictionary<string, DataItemDefinition>() { { dataItemRepository.GetActualID(dataItem), oldDefinition } }));
					}
			}
		}
		protected void OnDataItemsChanged(IEnumerable<DataItem> addedDataItems, IEnumerable<DataItem> removedDataItems) {
			if (!Loading) {
				if (addedDataItems != null)
					foreach (DataItem dataItem in addedDataItems)
						dataItem.Context = this;
				if (removedDataItems != null)
					foreach (DataItem dataItem in removedDataItems)
						dataItem.Context = null;
				OnDataItemsChangedInternal(addedDataItems ?? new DataItem[] { }, removedDataItems ?? new DataItem[] { });
			}
		}
		protected void OnDataItemContainersChanged(IEnumerable<DataItemContainer> addedContainers, IEnumerable<DataItemContainer> removedContainers) {
			if (addedContainers != null)
				foreach (DataItemContainer container in addedContainers)
					container.Context = this;
			if (removedContainers != null)
				foreach (DataItemContainer container in removedContainers)
					container.Context = null;
			if (!Loading) {
				List<DataItem> addedDataItems = new List<DataItem>();
				List<DataItem> removedDataItems = new List<DataItem>();
				if (addedContainers != null)
					foreach (DataItemContainer container in addedContainers)
						addedDataItems.AddRange(container.DataItems);
				if (removedContainers != null)
					foreach (DataItemContainer container in removedContainers)
						removedDataItems.AddRange(container.DataItems);
				OnDataItemsChangedInternal(addedDataItems, removedDataItems);
			}
		}
		protected override IList<DimensionFilterValues> GetExternalFilterValues() {
			return null;
		}
		protected override void DashboardChanged() {
			SetupFilterAgent();
		}
		protected override void GroupChanged() {
			SetupFilterAgent();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if (ColoringOptionsBase.ShouldSerialize()) {
				XElement coloringOptionsElement = new XElement(xmlColoringOptions);
				ColoringOptionsBase.SaveToXml(coloringOptionsElement);
				element.Add(coloringOptionsElement);
			}
			InteractivityOptionsBase.SaveToXml(element);
			if (dataSource != null && !string.IsNullOrEmpty(dataSource.ComponentName))
				element.Add(new XAttribute(xmlDataSource, dataSource.ComponentName));
			if (!string.IsNullOrEmpty(DataMember))
				element.Add(new XAttribute(xmlDataMember, DataMember));
			if (!string.IsNullOrEmpty(FilterString))
				element.Add(new XAttribute(xmlFilterString, FilterString));
			if (dataItemRepository.Count > 0) {
				XElement dataItemsElement = new XElement(xmlDataItems);
				dataItemRepository.SaveToXml(dataItemsElement);
				element.Add(dataItemsElement);
			}
			hiddenDimensions.SaveToXml(element, xmlHiddenDimensions, xmlDimension);
			hiddenMeasures.SaveToXml(element, xmlHiddenMeasures, xmlMeasure);
			if (IsMasterFilterCrossDataSource != DefaultIsMasterFilterCrossDataSource)
				element.Add(new XAttribute(xmlIsMasterFilterCrossDataSource, isMasterFilterCrossDataSource));
			if (formatRules != null && formatRules.Count > 0)
				formatRules.SaveToXml(element);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			InteractivityOptionsBase.LoadFromXml(element);
			dataSourceName = XmlHelper.GetAttributeValue(element, xmlDataSource);
			DataMember = XmlHelper.GetAttributeValue(element, xmlDataMember);
			string filterStringAttr = XmlHelper.GetAttributeValue(element, xmlFilterString);
			if (filterStringAttr != null)
				FilterString = filterStringAttr;
			else {
				string filterCriteriaAttr = XmlHelper.GetAttributeValue(element, xmlFilterCriteria);
				if (filterCriteriaAttr != null)
					FilterString = ObjectConverter.Instance.StringToObject(filterCriteriaAttr, typeof(CriteriaOperator)).ToString();
			}
			XElement dataItemsElement = element.Element(xmlDataItems);
			if (dataItemsElement != null)
				dataItemRepository.LoadFromXml(dataItemsElement);
			hiddenDimensions.LoadFromXml(element, xmlHiddenDimensions, xmlDimension);
			hiddenMeasures.LoadFromXml(element, xmlHiddenMeasures, xmlMeasure);
			string ignoreMasterFiltersString = XmlHelper.GetAttributeValue(element, xmlIgnoreMasterFilters);
			if (!String.IsNullOrEmpty(ignoreMasterFiltersString))
				InteractivityOptionsBase.IgnoreMasterFilters = XmlHelper.FromString<bool>(ignoreMasterFiltersString);
			string isMasterFilterCrossDataSourceAttr = XmlHelper.GetAttributeValue(element, xmlIsMasterFilterCrossDataSource);
			if (!String.IsNullOrEmpty(isMasterFilterCrossDataSourceAttr))
				isMasterFilterCrossDataSource = XmlHelper.FromString<bool>(isMasterFilterCrossDataSourceAttr);
			XElement coloringOptionsElement = element.Element(xmlColoringOptions);
			if (coloringOptionsElement != null)
				ColoringOptionsBase.LoadFromXml(coloringOptionsElement);
			if (formatRules != null)
				formatRules.LoadFromXml(element);
		}
		protected override void EndInitInternal() {
			base.EndInitInternal();
			UpdateDataMembersOnLoading();
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			hiddenDimensions.OnEndLoading(DataItemRepository, this);
			hiddenMeasures.OnEndLoading(DataItemRepository, this);
			colorSchemeContainer.OnEndLoading();
		}
		protected internal override void EnsureConsistecy(DashboardItem dashboardItem) {
			base.EnsureConsistecy(dashboardItem);
			DataDashboardItem dataDashboardItem = (DataDashboardItem)dashboardItem;
			DataSource = dataDashboardItem.DataSource;
			if (IsColoringSupported) {
				for (int i = 0; i < ColorSchemeInternal.Count; i++)
					ColorSchemeInternal[i].EnsureConsistency(dataDashboardItem.ColorSchemeInternal[i]);
			}
		}
		protected internal override void EnsureConsistecy(Dashboard dashboard) {
			base.EnsureConsistecy(dashboard);
			if (!string.IsNullOrEmpty(dataSourceName))
				DataSource = dashboard.DataSources.FindFirst(ds => {
					return ((ds != null ? ds.GetName_13_1() : null) ?? ds.ComponentName) == dataSourceName;
				});
			else
				DataSource = null;
			if (IsColoringSupported) {
				foreach (ColorSchemeEntry entry in ColorSchemeInternal)
					entry.EnsureConsistency(dashboard);
				ColorSchemeContainer.RefreshRepository();
			}
		}
		protected internal override ConditionalFormattingModel CreateConditionalFormattingModel() {
			return formatRules != null ? formatRules.CreateViewModel() : null;
		}
		protected internal int GetSelectionLength() {
			return CurrentDrillDownDimension != null ? 1 : SelectionDimensionList.Count;
		}
		protected void AddScheme(ColoringSchemeDefinition schemes, List<DimensionDefinition> dimensionDefinitions, Measure[][] measures) {
			List<DimensionDefinition> definitions = new List<DimensionDefinition>(dimensionDefinitions.Distinct());
			if (ColorMeasuresByHue)
				definitions.Add(DimensionDefinition.MeasureNamesDefinition);
			ColorRepositoryKey key = new ColorRepositoryKey(definitions) {
				DataSourceName = DataSourceName,
				DataMember = DataMember
			};
			if (measures != null)
				schemes.Add(key, measures.Select(array => Helper.GetMeasureDefinitions(array).ToArray()).ToArray());
		}
		protected void AddDrillDownSchemes(ColoringSchemeDefinition schemes, List<DimensionDefinition> drillDownDefinitionList, List<DimensionDefinition> additionalDefinitionList, Measure[][] measures, IList<Dimension> targentDimensions, bool unionAdditionalDefinitionsAfterDrillDown) {
			if (targentDimensions.Count == 0 || !targentDimensions[0].ColorByHue)
				AddScheme(schemes, additionalDefinitionList, measures);
			foreach (DimensionDefinition definition in drillDownDefinitionList) {
				IEnumerable<DimensionDefinition> generatedDefinitions = unionAdditionalDefinitionsAfterDrillDown ? new List<DimensionDefinition> { definition }.Union(additionalDefinitionList) : additionalDefinitionList.Union(new List<DimensionDefinition> { definition });
				AddScheme(schemes, generatedDefinitions.Distinct().ToList(), measures);
			}
		}
		protected Dimension[] GetColoringDimensions(bool drillDownEnabled, IEnumerable<Dimension> dimensions) {
			if (drillDownEnabled) {
				Dimension currentColored = null;
				foreach (Dimension dimension in dimensions) {
					if (dimension.ColorByHue)
						currentColored = dimension;
					if (dimension == CurrentDrillDownDimension && currentColored != null)
						return new[] { currentColored };
				}
				return new Dimension[0];
			}
			return dimensions.Where(dim => dim.ColorByHue).GroupBy(dim => dim.GetDimensionDefinition()).Select(group => group.First()).ToArray();
		}
		protected bool LastSingleColoredDefinition(DimensionDefinition definition, IList<Dimension> dimensions, bool drillDown) {
			return dimensions.Count > 1 && (drillDown || dimensions.Count(dim => dim.ColorByHue) == 1) && Equals(definition, Helper.GetUniqueDimensionDefinitions(dimensions).LastOrDefault());
		}
		void SetFilterStringInternal(string value) {
			if (!ReferenceEquals(value, filterString)) {
				string oldFilterString = filterString;
				filterString = value;
				OnChanged(ChangeReason.ClientData, this, oldFilterString);
			}
		}
		void CreateFilterAgent() {
			filterAgent = new FilterAgent(this, PriorityInsideFilterLevel);
			((IMasterFilter)this).FilterChanged += (sender, e) => {
				filterAgent.Changed();
			};
			SubscribeFilterAgentEvents();
		}
		void SetupFilterAgent() {
			if (ContextFilterGroup == null)
				filterAgent.Level = null;
			else 
				filterAgent.Level = ContextFilterGroup.GetFilterLevel(InteractivityOptionsBase.IgnoreMasterFilters, IsMasterFilterEnabled);
		}
		void SubscribeFilterAgentEvents() {
			if(InputFilter != null) {
				InputFilter.RequestAffected += FilterAgentRequestAffected;
				InputFilter.FilterChanged += FilterAgentFilterChanged;
			}
		}
		void UnsubscribeFilterAgentEvents() {
			if(InputFilter != null) {
				InputFilter.RequestAffected -= FilterAgentRequestAffected;
				InputFilter.FilterChanged -= FilterAgentFilterChanged;
			}
		}
		void FilterAgentRequestAffected(object sender, RequestAffectedEventArgs e) {
			e.RegisterAffected(this, new DataSourceInfo(DataSource, DataMember));
		}
		void FilterAgentFilterChanged(object sender, EventArgs e) {
		}
		bool CanPerformOlapDrillDown(Dimension dimension, object value) {
			string columnName = dimension.DataMember;
			string memberName = (value ?? string.Empty).ToString();
			return !IsOlapDataSource || DataSource.GetOlapDimensionMember(columnName, memberName, DataMember) != null;
		}
		Measure[][] GetColoringMeasures() {
			if(!ColorMeasuresByHue)
				return new Measure[0][];
			Measure[][] measures = GetColoringMeasuresInternal();
			return measures.Length > 0 ? measures : null;
		}
		protected void RequestExternalMasterFilterDimensions() {
			externalMasterFiltersDimensions.Clear();
			if(UseFakeData || InputFilter == null)
				return;
			InputFilter.GetCriteria(new DataSourceInfo(DataSource, DataMember), dimension => {
				if(!externalMasterFiltersDimensions.Contains(dimension))
					externalMasterFiltersDimensions.Add(dimension);
				return IsOlapDataSource ? dimension.DataMember : dataItemRepository.GetActualID(dimension);
			});
		}
		CriteriaOperator GetFullFilterCriteria(IActualParametersProvider provider) {
			if(!UseFakeData) {
				CriteriaOperator innerFilterWithParameters = DataSource.GetPatchedFilterCriteria(DataMember, FilterString, provider, true);
				CriteriaOperator drillDown = GetDrillDownCriteria();
				CriteriaOperator masterFilter = InputFilter == null ? 
					CriteriaOperator.Parse(string.Empty) :
					InputFilter.GetCriteria(new DataSourceInfo(DataSource, DataMember), dimension => IsOlapDataSource ? dimension.DataMember : dataItemRepository.GetActualID(dimension));
				return CriteriaOperator.And(innerFilterWithParameters, drillDown, masterFilter);
			} else {
				return CriteriaOperator.Parse(string.Empty);
			}
		}
		IDrillDownParameters GetDrillDownParameters() {
			if (RequestDrillDownParameters != null) {
				RequestDrillDownParametersEventArgs args = new RequestDrillDownParametersEventArgs(ComponentName);
				RequestDrillDownParameters(this, args);
				return args.Parameters;
			}
			if (Dashboard != null)
				return Dashboard.GetDrillDownParameters(ComponentName);
			return null;
		}
		IDrillDownController GetDrillDownController() {
			if (RequestDrillDownController != null) {
				RequestDrillDownControllerEventArgs args = new RequestDrillDownControllerEventArgs(ComponentName);
				RequestDrillDownController(this, args);
				return args.Controller;
			}
			if (Dashboard != null)
				return Dashboard.GetDrillDownController(ComponentName);
			return null;
		}
		CriteriaOperator GetDrillDownCriteria() {
			CriteriaOperator criteria = CriteriaOperator.Parse(String.Empty);
			IDrillDownParameters parameters = GetDrillDownParameters();
			if (parameters != null && parameters.Values != null) {
				IList<Dimension> drillDownDimensions = parameters.Values.Keys.ToList<Dimension>();
				foreach (Dimension drillDownDimension in drillDownDimensions)
					criteria = CriteriaOperator.And(criteria, MasterFilterCriteriaGenerator.CreateEquals(dataItemRepository.GetActualID(drillDownDimension), 
						parameters.Values[drillDownDimension]));
			}
			return criteria;
		}
		void AddRemoveDataItems(IEnumerable<DataItem> removedItems, IEnumerable<DataItem> addedItems) {
			RemoveDataItems(removedItems);
			AddDataItems(addedItems);
			ApplyRenamingMap();
		}
		void ApplyRenamingMap() {
			if (!renamingMapLocker.IsLocked) {
				filterString = NamesCriteriaPatcher.Process(filterString, CreateRenamingMap(renamingMap));
				renamingMap.Clear();
			}
		}
		void AddDataItems(IEnumerable<DataItem> dataItems) {
			foreach (DataItem dataItem in dataItems) {
				if (dataItem != null) {
					if (dataItemRepository.Contains(dataItem))
						throw new InvalidOperationException(String.Format(DashboardLocalizer.GetString(DashboardStringId.MessageDuplicatedDataItem),
							dataItem.DisplayName, ComponentName));
					dataItemRepository.AddDataItem(dataItem);
				}
			}
		}
		void RemoveDataItems(IEnumerable<DataItem> dataItems) {
			foreach (DataItem dataItem in dataItems)
				if (dataItem != null) {
					if (!dataItemRepository.Contains(dataItem))
						throw new InvalidOperationException(String.Format("Cannot delete the '{0}' data item as it does not exist.", dataItem.DisplayName));
					renamingMap.Add(dataItemRepository.GetActualID(dataItem), dataItem.GetDefinition());
					dataItemRepository.Remove(dataItem);
				}
		}
		Dictionary<string, string> CreateRenamingMap(Dictionary<string, DataItemDefinition> definitions) {
			Dictionary<string, string> renamingMap = new Dictionary<string, string>();
			foreach(KeyValuePair<string, DataItemDefinition> definition in definitions)
				renamingMap.Add(definition.Key, dataItemRepository.GetActualID(definition.Value));
			return renamingMap;
		}
		bool IsMeasureHidden(Measure measure) {
			return hiddenMeasures.Contains(measure);
		}
		void SubscribeDataSourceEvents(IDashboardDataSource dataSource) {
			if (dataSource != null) {
				ICalculatedFieldsController calculatedFieldsController = dataSource.GetCalculatedFieldsController();
				if(calculatedFieldsController != null) {
					calculatedFieldsController.CalculatedFieldChanged += CalculatedFields_CalculatedFieldChanged;
					calculatedFieldsController.CalculatedFieldCollectionChanged += CalculatedFields_CollectionChanged;
				}											  
			}
		}
		void UnsubscribeDataSourceEvents(IDashboardDataSource dataSource) {
			if (dataSource != null) {
				ICalculatedFieldsController calculatedFieldsController = dataSource.GetCalculatedFieldsController();
				if(calculatedFieldsController != null) {
					calculatedFieldsController.CalculatedFieldChanged -= CalculatedFields_CalculatedFieldChanged;
					calculatedFieldsController.CalculatedFieldCollectionChanged -= CalculatedFields_CollectionChanged;
				}		   
			}
		}
		void CalculatedFields_CalculatedFieldChanged(object sender, CalculatedFieldChangedEventArgs e) {
			CalculatedField calculatedField = e.Field;
			if (this.GetDataMembers().Contains(calculatedField.Name) || e.PropertyName == "Name" && this.GetDataMembers().Contains(e.OldValue.ToString())) {
				if (e.PropertyName == "Name") {
					ChangeDataItemDataMember(e.OldValue.ToString(), calculatedField.Name);
				}
				else
					OnChanged(ChangeReason.RawData, this);
			}
			if(e.PropertyName == "Expression") {
				Dimension dimension = GetDimension(calculatedField.Name);
				if(dimension != default(Dimension) && calculatedField.CheckHasAggregate(dataSource.CalculatedFields)) {
					throw new InvalidOperationException(string.Format(DashboardLocalizer.GetString(DashboardStringId.MessageSummaryCalculatedFieldInDimension), calculatedField.Name, dimension.DisplayName));
				}
			}
		}
		void CalculatedFields_CollectionChanged(object sender, NotifyingCollectionChangedEventArgs<CalculatedField> e) {
			if (e.RemovedItems != null && e.RemovedItems.Any((c) => this.GetDataMembers().Contains(c.Name)) || e.AddedItems != null && e.AddedItems.Any((c) => this.GetDataMembers().Contains(c.Name)))
				OnChanged(ChangeReason.RawData, this);
		}
		void RaiseMasterFilterChangedInternal() {
			if (!Loading && masterFilterChangedInternal != null)
				masterFilterChangedInternal.Invoke(this, EventArgs.Empty);
		}
		void OnDataItemsChangedInternal(IEnumerable<DataItem> addedDataItems, IEnumerable<DataItem> removedDataItems) {
			AddRemoveDataItems(removedDataItems, addedDataItems);
			if (Dashboard != null)
				Dashboard.OnDashboardItemChanged(this, new ChangedEventArgs(ChangeReason.ClientData, null, null));
		}
		void UnloadDataSource() {
			UnsubscribeDataSourceEvents(dataSource);
		}
		void LoadDataSource() {
			SubscribeDataSourceEvents(dataSource);
			OnChanged(ChangeReason.RawData);
		}
		void SynchronizeDataSource() {
			if(dataSource != null && !Loading && !IsVSDesignMode) {
				Dashboard dashboard = Dashboard;
				if(dashboard != null && !dashboard.IsDataSourceSyncLocked) {
					dashboard.BeginUpdate();
					ICollection<IDashboardDataSource> dataSources = dashboard.DataSources;
					if(!dataSources.Contains(dataSource))
						dataSources.Add(dataSource);
					dashboard.CancelUpdate();
				}
			}
		}
		#region IMasterFilter
		IEnumerable<IMasterFilterParameters> IMasterFilter.Parameters { get { return new[] { MasterFilterParameters }; } }
		bool IMasterFilter.IsCrossDataSource { get { return isMasterFilterCrossDataSource; } }
		event EventHandler IMasterFilter.FilterChanged { add { masterFilterChangedInternal += value; } remove { masterFilterChangedInternal -= value; } }
		bool IMasterFilter.IsFilterDataSource(DataSourceInfo dataSourceInfo) {
			return isMasterFilterCrossDataSource || DataSourceInfoComparer.Comparer.Equals(dataSourceInfo, new DataSourceInfo(DataSource, DataMember));
		}
		#endregion
		#region IMasterFilterItem
		string IMasterFilterItem.Name { get { return ComponentName; } }
		#endregion 
		#region IDataSourceSchemaProvider
		IDataSourceSchema IDataSourceSchemaProvider.DataSourceSchema { get { return DataSourceSchema; } }
		#endregion
		#region IDataItemContext
		IChangeService IDataItemContext.ChangeService { get { return this; } }
		IDataItemRepositoryProvider IDataItemContext.DataItemRepositoryProvider { get { return this; } }
		IDataSourceSchemaProvider IDataItemContext.DataSourceSchemaProvider { get { return this; } }
		ICurrencyCultureNameProvider IDataItemContext.CurrencyCultureNameProvider { get { return this; } }
		SummaryTypeInfo IDataItemContext.GetSummaryTypeInfo(Measure measure) {
			return GetSummaryTypeInfo(measure);
		}
		DimensionGroupIntervalInfo IDataItemContext.GetDimensionGroupIntervalInfo(Dimension dimension) {
			return GetDimensionGroupIntervalInfo(dimension);
		}
		bool IDataItemContext.ColorDimension(Dimension dimension) {
			return ColorDimension(dimension);
		}
		#endregion
		#region IDataItemContainerContext
		void IDataItemContainerContext.OnDataItemsChanged(IEnumerable<DataItem> addedDataItems, IEnumerable<DataItem> removedDataItems) {
			if (!Loading)
				OnDataItemsChangedInternal(addedDataItems ?? new DataItem[] { }, removedDataItems ?? new DataItem[] { });
		}
		#endregion
		#region IDataItemSerializationContext
		bool IDataItemSerializationContext.ShouldSerializeDataItem(DataItem dataItem) {
			Dimension dimension = dataItem as Dimension;
			return dimension == null || !externalMasterFiltersDimensions.Contains(dimension);
		}
		#endregion
		#region IColorSchemeContext
		bool IColorSchemeContext.Loading { get { return Loading; } }
		bool IColorSchemeContext.IsChangeLocked { get { return ColoringOptionsBase.UseGlobalColors; } }
		IChangeService IColorSchemeContext.ChangeService { get { return this; } }		
		#endregion
		#region IFormatRulesContext
		IChangeService IFormatRulesContext.ChangeService { get { return this; } }
		IDataItemRepositoryProvider IFormatRulesContext.DataItemRepositoryProvider { get { return this; } }
		IList<Dimension> IFormatRulesContext.GetAxisItems(bool isSecondAxis) {
			return GetFormatRuleAxisItems(isSecondAxis);
		}
		IList<Measure> IFormatRulesContext.GetMeasures() {
			return GetFormatRuleMeasures();
		}
		void IFormatRulesContext.OnChanged(object param) {
			IChangeService changeService = this;
			changeService.OnChanged(new ChangedEventArgs(ChangeReason.ClientData, this, param));
		}
		#endregion
		#region Client Data
		DataStorage CleanDataStorage(IDictionary<string, object> filters, DataStorage storage, SliceDataQuery query, IActualParametersProvider provider) {
			RemoveMeasuresFromDataStorage(storage, query, provider);
			if(filters != null)
				storage = GetFilteredStorage(filters, storage, false);
			return storage;
		}
		protected virtual void RemoveMeasuresFromDataStorage(DataStorage storage, SliceDataQuery query, IActualParametersProvider provider) {
			if(query == null)
				return;
			ClientHierarchicalMetadata metadata = new ClientHierarchicalMetadata(GetMetadata(provider));
			IEnumerable<string> forceList = IsBackCompatibilityDataSlicesRequired ? HiddenMeasures.Select(m => GetDataItemUniqueName(m)) : new string[0];
			foreach(StorageSlice slice in storage)
				foreach(StorageColumn measure in slice.MeasureColumns)
					if(!IsMeasureNeedForClient(measure.Name, slice, query, metadata, forceList))
						slice.RemoveMeasure(measure);
		}
		static bool IsMeasureNeedForClient(string name, StorageSlice slice, SliceDataQuery query, ClientHierarchicalMetadata metadata, IEnumerable<string> forceList) {
			if(forceList.Contains(name) || metadata.IsInternalInfoMeasure(name))
				return true;
			if(query.DataSlices.Any(s => s.SummaryAggregations.Any(d => d.Name == name && d.IncludeInGrandTotal)))
				return true;
			SliceModel sliceModel = query.DataSlices.FirstOrDefault(s => s.Dimensions.Select(d => d.Name).SequenceEqualsAsSet(slice.KeyColumns.Select(c => c.Name)));
			if(sliceModel == null)
				return false;
			return sliceModel.Measures.Any(m => m.Name == name);
		}
		internal static DataStorage GetFilteredStorage(IDictionary<string, object> filters, DataStorage storage, bool includeOrthogonalSlices) {
			DataStorage oldStorage = storage;
			DataStorage newStorage = DataStorage.CreateEmpty();
			Dictionary<StorageColumn, object> columnFilters = filters.ToDictionary(pair => oldStorage.GetColumn(pair.Key), pair => pair.Value);
			Func<StorageColumn, StorageColumn> getNewColumn = oldColumn => {
				return newStorage.CreateColumn(oldColumn.Name, oldColumn.IsKey);
			};
			foreach(StorageSlice oldSlice in oldStorage) {
				int filtersCount = columnFilters.Keys.Count();
				int sliceColumnCount = oldSlice.KeyColumns.Count();
				IEnumerable<StorageColumn> intersection = columnFilters.Keys.Intersect(oldSlice.KeyColumns);
				int intersectionCount = intersection.Count();
				bool fullIntersectedSlice = intersectionCount == filtersCount && sliceColumnCount > filtersCount;
				bool orthogonalSlice = intersectionCount == 0;
				bool specialSlice = oldSlice.KeyColumns.Count() == 1 && oldSlice.MeasureColumns.Any(c => DataStorageGenerator.IsSpecialColumn(c));
				bool addSlice = fullIntersectedSlice || specialSlice || (includeOrthogonalSlices && orthogonalSlice);
				if(addSlice) {
					bool ignoreFilter = (includeOrthogonalSlices && orthogonalSlice);
					bool includeMeasureColumns = fullIntersectedSlice || specialSlice;
					StorageSlice newSlice = newStorage.GetSlice(oldSlice.KeyColumns.Select(getNewColumn));
					var currentFilters = columnFilters.Where(pair => oldSlice.KeyColumns.Contains(pair.Key));
					foreach(StorageRow oldRow in oldSlice) {
						if(ignoreFilter || currentFilters.All(pair => oldRow[pair.Key].MaterializedValue.Equals(pair.Value))) {
							StorageRow newRow = new StorageRow();
							IEnumerable<StorageColumn> oldColumns = oldSlice.KeyColumns;
							if(includeMeasureColumns)
								oldColumns = oldColumns.Concat(oldSlice.MeasureColumns);
							foreach(StorageColumn oldColumn in oldColumns)
								newRow[getNewColumn(oldColumn)] = StorageValue.CreateUnbound(oldRow[oldColumn].MaterializedValue);
							newSlice.AddRow(newRow);
						}
					}
				}
			}
			return newStorage;
		}
		protected MultiDimensionalData CreateMDDataFromDataStorage(HierarchicalDataParams hData, IActualParametersProvider provider) {
			return new MultiDimensionalData(hData, new ClientHierarchicalMetadata(GetMetadataInternal(true, provider)));
		}
		void PrepareItemDataDTOInternal(HierarchicalDataParams hData, IActualParametersProvider parameters, ColorRepository coloringCache) {
			if(hData.Storage.IsEmpty)
				return;
			bool formatRulesExists = formatRules != null && formatRules.Count > 0;
			bool coloringExists = IsColoringSupported;
			MultiDimensionalData multiData = formatRulesExists || coloringExists ? CreateMDDataFromDataStorage(hData, parameters) : null;
			if(multiData != null && !multiData.IsEmpty) {
				if(formatRulesExists) {
					DashboardFormatConditionManager manager = new DashboardFormatConditionManager(multiData, parameters.GetActualParameters());
					manager.Calculate(formatRules);
				}
				if(coloringExists) 
					PrepareItemDataDTOColors(multiData, coloringCache, parameters);
			}
		}
		void PrepareItemDataDTOColors(MultiDimensionalData multiDimensionalData, ColorRepository coloringCache, IActualParametersProvider provider) {
			DashboardPalette palette = new DashboardPalette();
			HierarchicalMetadata metadata = GetMetadataInternal(true, provider);
			ColorTablePair colorTable = null;
			if (coloringCache != null)
				colorTable = GetColoringTable(coloringCache);
			if (colorTable != null && colorTable.Table != null) {
				Dictionary<string, Dictionary<string, int>> dimensionsByAxes = GetColorDimensionsByAxes(colorTable.Key.DimensionDefinitions); 
				foreach(KeyValuePair<ColorTableServerKey, ColorDefinitionBase> pair in colorTable.Table.Rows) {
					ColorTableServerKey colorKey = pair.Key;
					Color color = pair.Value.GetColor(palette);
					Dictionary<string, IEnumerable<AxisPoint>> axisPoints = new Dictionary<string, IEnumerable<AxisPoint>>();
					if(colorKey.DimensionValues == null && dimensionsByAxes.Select(p => p.Value.Count).Cast<int>().Sum() > 0)
						continue;
					foreach(KeyValuePair<string, Dictionary<string, int>> axisPair in dimensionsByAxes) {
						string axisName = axisPair.Key;
						Dictionary<string, int> descriptorIDs = axisPair.Value;
						List<KeyValuePair<string, object>> colorPaths = new List<KeyValuePair<string, object>>();
						foreach(string dimensionID in descriptorIDs.Keys) {
							int definitionIndex = descriptorIDs[dimensionID];
							object colorPathValue = colorKey.DimensionValues[definitionIndex];
							colorPaths.Add(new KeyValuePair<string, object>(dimensionID, colorPathValue));
						}
						axisPoints.Add(axisName, multiDimensionalData.GetAxisPointsByUniqueValues(axisName, colorPaths));
					}
					IEnumerable<AxisPoint> columnAxisPoints = metadata.ColumnHierarchy != null ? axisPoints[metadata.ColumnHierarchy] : new AxisPoint[] { null };
					IEnumerable<AxisPoint> rowAxisPoints = metadata.RowHierarchy != null ? axisPoints[metadata.RowHierarchy] : new AxisPoint[] { null };
					foreach(AxisPoint columnAxisPoint in columnAxisPoints) {
						foreach(AxisPoint rowAxisPoint in rowAxisPoints) {
							string measureDescriptorId = colorKey.Measures != null ?
								CorrectColorMeasureId(string.Join("_", colorKey.Measures.Select(measure => DataItemDefinitionDisplayTextProvider.GetMeasureDefinitionString(measure)).OrderBy(str => str))) :
								ColorMeasure;
							MeasureDescriptor measureDescriptor = multiDimensionalData.GetMeasureDescriptorByID(measureDescriptorId);
							if(measureDescriptor != null)
								multiDimensionalData.AddValue(columnAxisPoint, rowAxisPoint, measureDescriptor, color.ToArgb());
						}
					}
				}
			}
		}
		internal ColorTablePair GetColoringTable(ColorRepository coloringCache) {
			return coloringCache.GetTable(GetColoringDimensionDefinitions().ToArray(), DataSourceName, DataMember, ColorMeasuresByHue, false);
		}
		protected virtual Dictionary<string, Dictionary<string, int>> GetColorDimensionsByAxes(List<DimensionDefinition> dimensionDefinitions) {
			return null;
		}
		protected virtual Dictionary<string, int> GetColorDimensionsByAxis(string axisName, IList<DimensionDefinition> actualColorDimensionDefinitions) {
			return null;
		}
		internal Dictionary<string, int> GetColorDimensionsByAxis(string axisName) {
			DimensionDefinition[] colorDimensionDefinitions = GetColoringDimensionDefinitions().ToArray();
			return GetColorDimensionsByAxis(axisName, colorDimensionDefinitions);
		}
		protected Dictionary<string, int> FilterColorDimensions(DimensionCollection dimensions, IList<DimensionDefinition> actualColorDimensionDefinitions) {
			Dictionary<string, int> res = new Dictionary<string, int>();
			IList<DimensionDefinition> currentColorDimensionDefinitions = GetColoringDimensionDefinitions().ToArray();
			for(int i = 0; i < currentColorDimensionDefinitions.Count; i++) {
				foreach(Dimension dimension in dimensions) {
					if(dimension.EqualsByDefinition(currentColorDimensionDefinitions[i])) {
						res.Add(dimension.ActualId, actualColorDimensionDefinitions.IndexOf(currentColorDimensionDefinitions[i]));
						break;
					}
				}
			}
			return res;
		}
		internal virtual string[] GetColorPath() {
			return null;
		}
		protected string[] GetColorPath(Dictionary<string, int> colorDimensions, List<int> indexes) {
			List<string> res = new List<string>();
			foreach(KeyValuePair<string, int> pair in colorDimensions) {
				if(!indexes.Contains(pair.Value)) {
					res.Add(pair.Key);
					indexes.Add(pair.Value);
				}
			}
			return res.ToArray();
		}
		protected virtual HierarchicalMetadata GetMetadataInternal(bool addHiddenMeasures, IActualParametersProvider provider) {
			HierarchicalMetadataBuilder builder = new HierarchicalMetadataBuilder(addHiddenMeasures, () => ((ISliceDataQueryProvider)this).GetDataQuery(provider) ?? new SliceDataQuery());
			foreach(Measure measure in hiddenMeasures) 
				builder.AddHiddenMeasure(measure);
			GetMetadataInternal(builder);
			return builder.CreateMetadata();
		}
		protected virtual void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			if(DataSource != null)
				builder.DataSourceColumns = DataSource.GetDataMembers(DataMember);
		}
#endregion
		#region DashboardItem Data Description
		internal virtual DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = new DashboardItemDataDescription(DataSource, DataMember, DataItemRepository);
			description.Group = Group;
			description.FilterString = FilterString;
			description.IsMasterFilterCrossDataSource = IsMasterFilterCrossDataSource;
			description.ShowCaption = ShowCaption;
			FilterableDashboardItemInteractivityOptions interactivityOptionsClone = CreateInteractivityOptions();
			InteractivityOptionsBase.CopyTo(interactivityOptionsClone);
			description.InteractivityOptions = interactivityOptionsClone;
			if(Dashboard != null && !Dashboard.DashboardItemCaptionGenerator.IsDefaultName(this, Name))
				description.Name = Name;
			foreach(Dimension dimension in HiddenDimensions)
				if(dimension != null)
					description.AddHiddenDimension(dimension);
			foreach(Measure measure in HiddenMeasures)
				if(measure != null)
					description.AddHiddenMeasure(measure);
			return description;
		}
		Measure FindMeasureByDefinition(MeasureDefinition definition) {
			return Measures.FirstOrDefault(measure => measure.EqualsByDefinition(definition));
		}
		void ApplyDimensionDescriptions(DashboardItemDataDescription description) {
			foreach (Dimension dimension in Dimensions) {
				DimensionDescription dimensionDescription;
				if (description.DimensionDescriptions.TryGetValue(dimension, out dimensionDescription)) {
					if (dimensionDescription.SortByMeasureDefinition != null)
						dimension.SortByMeasure = FindMeasureByDefinition(dimensionDescription.SortByMeasureDefinition);
					if(dimensionDescription.TopNMeasureDefinition != null)
						dimension.TopNOptions.Measure = FindMeasureByDefinition(dimensionDescription.TopNMeasureDefinition);
				}
			}
		}
		internal void AssignDashboardItemDataDescription(DashboardItemDataDescription description) {
			AssignDashboardItemDataDescriptionCore(description);
			ApplyDimensionDescriptions(description);
			AssignFilterString(description);
		}
		internal virtual void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			SetDataSource(description.DataSource, description.DataMember);
			Group = description.Group;
			Name = description.Name;
			ShowCaption = description.ShowCaption;
			foreach (DataItem dataItem in description.DataItems)
				dataItem.Context = this;
			if (description.InteractivityOptions != null)
				description.InteractivityOptions.CopyTo(InteractivityOptionsBase);
			DashboardItemInteractivityOptions interactivityOptions = InteractivityOptionsBase as DashboardItemInteractivityOptions;
			if(interactivityOptions != null && description.FilterElementType != FilterElementTypeDescription.None) {
				interactivityOptions.MasterFilterMode = description.FilterElementType == FilterElementTypeDescription.Checked ?
					DashboardItemMasterFilterMode.Multiple :
					DashboardItemMasterFilterMode.Single;
			}
			IsMasterFilterCrossDataSource = description.IsMasterFilterCrossDataSource;
			HiddenDimensions.AddRange(description.HiddenDimensions);
			HiddenMeasures.AddRange(description.HiddenMeasures);
		}
		void AssignFilterString(DashboardItemDataDescription description) {
			if(description.FilterString != null) {
				Dictionary<string, DataItemDefinition> definitions = new Dictionary<string, DataItemDefinition>();
				foreach(DataItem item in description.DataItemUniqueNames.Keys)
					definitions.Add(description.DataItemUniqueNames[item], item.GetDefinition());
				FilterString = NamesCriteriaPatcher.Process(description.FilterString, CreateRenamingMap(definitions));
			}
		}
		internal void AssignMeasure(Measure source, MeasureCollection destination) {
			if(source != null)
				destination.Add(source);
		}
		internal void AssignDimension(Dimension source, DimensionCollection destination) {
			if(source != null)
				destination.Add(source);
		}
		protected void AssignRuleDescription(RuleDescription source, DashboardItemFormatRule destination) {
			destination.Name = source.Name;
			destination.Enabled = source.Enabled;
			destination.StopIfTrue = source.StopIfTrue;
			destination.Condition = source.Condition;
		}
		#endregion
	}
}
