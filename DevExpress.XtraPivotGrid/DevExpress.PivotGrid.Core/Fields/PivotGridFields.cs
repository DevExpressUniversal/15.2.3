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

using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Data.PivotGrid;
using DevExpress.Data.Utils;
using DevExpress.PivotGrid;
using DevExpress.PivotGrid.CriteriaVisitors;
using DevExpress.PivotGrid.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Serializing;
using DevExpress.Utils.Serializing.Helpers;
using DevExpress.WebUtils;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Globalization;
using System.Text.RegularExpressions;
namespace DevExpress.XtraPivotGrid {
	[DesignTimeVisible(false), ToolboxItem(false)]
	public class PivotGridFieldBase : Component, IComponentLoading, IXtraSerializableLayoutEx, IViewBagOwner, IXtraSupportDeserializeCollectionItem, IXtraSupportDeserializeCollection,
						IXtraSerializable, IDataColumnInfo {
		internal static bool IsFilterEmptyFast(PivotGridFieldBase field) {
			if(!field.CanApplyFilter)
				return true;
			PivotGridFieldFilterValues filterValues = field.FilterValues;
			if(filterValues.FilterType == PivotFilterType.Included)
				return false;
			return filterValues.FilterType == PivotFilterType.Excluded && filterValues.Count == 0;
		}
		internal const int DefaultLevel = int.MaxValue;
		protected const int LayoutIdAppearance = 1, LayoutIdData = 2, LayoutIdLayout = 3, LayoutIdOptionsView = 4;
		protected const string FieldNameEqualsNameExceptionString = "The values of FieldName and Name(ID) properties can not be the same.";
		public const int MinimumValueLineCount = 1,
			MaximumColumnValueLineCount = 50,
			MaximumRowValueLineCount = 50;
		public const int DefaultMinWidth = 20, DefaultWidth = 100;
		static Regex reg1 = new Regex(@"\[([^\[\]]+)\]\.\[([^\[\]]+)\]\.\[([^\[\]]+)\]"), reg2 = new Regex(@"\[([^\[\]]+)\]\.\[([^\[\]]+)\]");
		public static bool IsOLAPFieldName(string fieldName) {
			return reg1.IsMatch(fieldName) || reg2.IsMatch(fieldName);
		}
		static bool IsOLAPMeasureFieldName(string fieldName) {
			return fieldName.ToUpper().StartsWith("[MEASURES]");
		}
		static string namePrefix = PivotGridFieldCollectionBase.DefaultNamePrefix;
		[Browsable(false)]
		public static string NamePrefix { get { return namePrefix; } set { namePrefix = value; } }
		[Browsable(false)]
		public static string InvalidPropertyDisplayText {
			get { return PivotGridLocalizer.GetString(PivotGridStringId.PrefilterInvalidProperty); }
		}
		string name;
		string fieldName;
		UnboundColumnType unboundType;
		string unboundExpression;
		UnboundExpressionMode unboundExpressionMode;
		TopValueMode topValueMode;
		CriteriaOperator expressionOperator;
		List<string> expressionColumnNames;
		MixedSummaryLevelCriteriaVisitor mixedCriteriaVisitor = null;
		bool visible;
		PivotArea area;
		int areaIndex;
		int areaIndexOld;
		PivotGridFieldCollectionBase collection;
		PivotSummaryType summaryType;
		PivotSummaryDisplayType summaryDisplayType;
		int minWidth;
		int width;
		string caption;
		string displayFolder;
		PivotGridFieldOptions options;
		string emptyCellText;
		string emptyValueText;
		string grandTotalText;
		PivotTotalsVisibility totalsVisibility;
		readonly PivotGridFieldFilterValues filterValues;
		int columnHandle;
		PivotSortOrder sortOrder;
		string sortByAttribute;
		string[] autoPopulatedProperties;
		PivotSortMode sortMode;
		PivotSortMode? actualSortMode;
		PivotGridCustomTotalCollectionBase customTotals;
		PivotGridFieldSortBySummaryInfo sortBySummaryInfo;
		NotificationCollection<PivotCalculationBase> calculations;
		int topValueCount;
		PivotTopValueType topValueType;
		PivotGridData dataFieldData;
		PivotKPIGraphic kpiGraphic;
		PivotGridGroup group;
		bool topValueShowOthers;
		string unboundFieldName;
		int groupIntervalColumnHandle;
		PivotGroupInterval groupInterval;
		int groupIntervalNumericRange;
		bool expandedInFieldsGroup;
		PivotGridAllowedAreas allowedAreas;
		object tag;
		int indexInternal;
		bool isNew;
		bool runningTotal;
		int columnValueLineCount, rowValueLineCount;
		bool useDecimalValuesForMaxMinSummary;
		bool calculateTotals = true;
		bool isDisposed;
		FormatInfo cellFormat, totalCellFormat, grandTotalCellFormat, valueFormat, totalValueFormat;
		DefaultBoolean useNativeFormat;
		PivotSummaryFilter summaryFilter;
		static readonly internal FormatInfo defaultDateFormat = new FormatInfo();
		static readonly internal FormatInfo defaultTotalFormat = new FormatInfo();
		static readonly internal FormatInfo defaultDecimalFormat = new FormatInfo();
		static readonly internal FormatInfo defaultPercentFormat = new FormatInfo();
		static readonly internal FormatInfo defaultDecimalNonCurrencyFormat = new FormatInfo();
		static PivotGridFieldBase() {
			defaultDateFormat.FormatType = FormatType.DateTime;
			defaultDateFormat.FormatString = CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern;
			defaultTotalFormat.FormatType = FormatType.Numeric;
			defaultTotalFormat.FormatString = PivotGridLocalizer.GetString(PivotGridStringId.TotalFormat);
			defaultDecimalFormat.FormatType = FormatType.Numeric;
			defaultDecimalFormat.FormatString = "c";
			defaultPercentFormat.FormatType = FormatType.Numeric;
			defaultPercentFormat.FormatString = "p";
			defaultDecimalNonCurrencyFormat.FormatType = FormatType.Numeric;
			defaultDecimalNonCurrencyFormat.FormatString = "f2";
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseDefaultDateFormat")]
#endif
		public static FormatInfo DefaultDateFormat { get { return defaultDateFormat; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseDefaultTotalFormat")]
#endif
		public static FormatInfo DefaultTotalFormat { get { return defaultTotalFormat; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseDefaultDecimalFormat")]
#endif
		public static FormatInfo DefaultDecimalFormat { get { return defaultDecimalFormat; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseDefaultPercentFormat")]
#endif
		public static FormatInfo DefaultPercentFormat { get { return defaultPercentFormat; } }
		internal static FormatInfo DefaultDecimalNonCurrencyFormat { get { return defaultDecimalNonCurrencyFormat; } }
		public static bool IsDefaultFormat(FormatInfo formatInfo) {
			return formatInfo == DefaultDateFormat ||
				formatInfo == DefaultTotalFormat ||
				formatInfo == DefaultDecimalFormat ||
				formatInfo == DefaultPercentFormat;
		}
		public PivotGridFieldBase() : this(string.Empty, PivotArea.FilterArea) { }
		public PivotGridFieldBase(PivotGridData dataFieldData)
			: this(string.Empty, PivotArea.ColumnArea) {
			this.dataFieldData = dataFieldData;
			if(IsDataField) {
				UnboundFieldName = "pivot$dataField$";
				UnboundType = UnboundColumnType.Integer;
				Options.AllowSort = DefaultBoolean.False; 
				Options.AllowFilter = DefaultBoolean.False;
				Options.AllowFilterBySummary = DefaultBoolean.False;
				Options.AllowExpand = DefaultBoolean.False;
			}
		}
		public PivotGridFieldBase(string fieldName, PivotArea area) {
			this.name = string.Empty;
			this.fieldName = fieldName ?? string.Empty;
			this.unboundType = UnboundColumnType.Bound;
			this.unboundExpression = string.Empty;
			this.unboundExpressionMode = UnboundExpressionMode.Default;
			this.topValueMode = TopValueMode.Default;
			this.expressionOperator = null;
			this.expressionColumnNames = null;
			this.visible = true;
			this.area = area;
			this.areaIndex = -1;
			this.collection = null;
			this.summaryType = PivotSummaryType.Sum;
			this.summaryDisplayType = PivotSummaryDisplayType.Default;
			this.width = -1;
			this.minWidth = -1;
			this.caption = string.Empty;
			this.displayFolder = string.Empty;
			this.emptyCellText = string.Empty;
			this.emptyValueText = string.Empty;
			this.grandTotalText = string.Empty;
			this.sortOrder = PivotSortOrder.Ascending;
			this.sortMode = PivotSortMode.Default;
			this.actualSortMode = null;
			this.filterValues = CreateFilterValues();
			this.customTotals = CreateCustomTotals();
			this.sortBySummaryInfo = new PivotGridFieldSortBySummaryInfo(this, "SortBySummaryInfo");
			this.options = CreateOptions(new PivotOptionsChangedEventHandler(OnOptionsChanged), "Options");
			this.columnHandle = -1;
			this.unboundFieldName = string.Empty;
			this.groupInterval = PivotGroupInterval.Default;
			this.groupIntervalNumericRange = 10;
			this.groupIntervalColumnHandle = -1;
			this.topValueCount = 0;
			this.topValueType = PivotTopValueType.Absolute;
			this.topValueShowOthers = false;
			this.expandedInFieldsGroup = true;
			this.allowedAreas = PivotGridAllowedAreas.All;
			this.tag = null;
			this.indexInternal = -1;
			this.isNew = false;
			this.runningTotal = false;
			this.kpiGraphic = PivotKPIGraphic.ServerDefined;
			this.columnValueLineCount = 1;
			this.rowValueLineCount = 1;
			this.cellFormat = new FormatInfo(this, this, "CellFormat");
			CellFormat.Changed += new EventHandler(OnFormatChanged);
			this.totalCellFormat = new FormatInfo(this, this, "TotalCellFormat");
			TotalCellFormat.Changed += new EventHandler(OnFormatChanged);
			this.grandTotalCellFormat = new FormatInfo(this, this, "GrandTotalCellFormat");
			GrandTotalCellFormat.Changed += new EventHandler(OnFormatChanged);
			this.valueFormat = new FormatInfo(this, this, "ValueFormat");
			ValueFormat.Changed += new EventHandler(OnFormatChanged);
			this.totalValueFormat = new FormatInfo(this, this, "TotalValueFormat");
			TotalValueFormat.Changed += new EventHandler(OnFormatChanged);
			this.useNativeFormat = DefaultBoolean.Default;
			this.summaryFilter = new PivotSummaryFilter(this);
			this.calculations = new NotificationCollection<PivotCalculationBase>();
			this.calculations.CollectionChanged += (s, e) => {
				switch(e.Action) {
					case CollectionChangedAction.Add:
						e.Element.Changed += OnCalculationChanged;
						break;
					case CollectionChangedAction.Remove:
					case CollectionChangedAction.Clear:
						e.Element.Changed -= OnCalculationChanged;
						break;
				}
				OnCalculationsChanged();
			};
		}
		protected override void Dispose(bool disposing) {
			if(disposing) {
				isDisposed = true;
				if(HasGroup) {
					Group.Remove(this);
				}
				if(Collection != null) {
					PivotGridFieldCollectionBase cols = Collection;
					this.collection = null;
					cols.Remove(this);
				}
				if(CellFormat != null) {
					CellFormat.Changed -= new EventHandler(OnFormatChanged);
					TotalCellFormat.Changed -= new EventHandler(OnFormatChanged);
					GrandTotalCellFormat.Changed -= new EventHandler(OnFormatChanged);
					ValueFormat.Changed -= new EventHandler(OnFormatChanged);
					TotalValueFormat.Changed -= new EventHandler(OnFormatChanged);
					this.cellFormat = null;
					this.totalCellFormat = null;
					this.grandTotalCellFormat = null;
					this.valueFormat = null;
					this.totalValueFormat = null;
				}
			}
			base.Dispose(disposing);
		}
		public virtual void Assign(PivotGridFieldBase field) {
			this.AllowedAreas = field.AllowedAreas;
			this.Area = field.Area;
			this.AreaIndex = field.AreaIndex;
			this.Caption = field.Caption;
			this.DisplayFolder = field.DisplayFolder;
			this.CellFormat.Assign(field.CellFormat);
			this.CustomTotals.Add(field.CustomTotals);
			this.ExpandedInFieldsGroup = field.ExpandedInFieldsGroup;
			this.EmptyCellText = field.EmptyCellText;
			this.EmptyValueText = field.EmptyValueText;
			this.FieldName = field.FieldName;
			this.FilterValues.Assign(field.FilterValues);
			this.GrandTotalCellFormat.Assign(field.GrandTotalCellFormat);
			this.GrandTotalText = field.GrandTotalText;
			this.GroupInterval = field.GroupInterval;
			this.GroupIntervalNumericRange = field.GroupIntervalNumericRange;
			this.KPIGraphic = field.KPIGraphic;
			this.Name = field.Name;
			this.Options.Assign(field.Options);
			this.SortBySummaryInfo.Assign(field.SortBySummaryInfo);
			this.SortMode = field.SortMode;
			this.sortMode = field.sortMode;
			this.actualSortMode = field.actualSortMode;
			this.SortOrder = field.SortOrder;
			this.SummaryDisplayType = field.SummaryDisplayType;
			this.SummaryFilter.Assign(field.SummaryFilter);
			this.SummaryType = field.SummaryType;
			this.Tag = field.Tag;
			this.TopValueCount = field.TopValueCount;
			this.TopValueShowOthers = field.TopValueShowOthers;
			this.TopValueType = field.TopValueType;
			this.TopValueMode = field.TopValueMode;
			this.CalculateTotals = field.CalculateTotals;
			this.TotalCellFormat.Assign(field.TotalCellFormat);
			this.TotalsVisibility = field.TotalsVisibility;
			this.TotalValueFormat.Assign(field.TotalValueFormat);
			this.UnboundExpression = field.UnboundExpression;
			this.UnboundType = field.UnboundType;
			this.UnboundFieldName = field.UnboundFieldName;
			this.UseNativeFormat = field.UseNativeFormat;
			this.ValueFormat.Assign(field.ValueFormat);
			this.Visible = field.Visible;
			this.RunningTotal = field.RunningTotal;
			this.ColumnValueLineCount = field.ColumnValueLineCount;
			this.RowValueLineCount = field.RowValueLineCount;
			this.Width = field.Width;
			this.MinWidth = field.MinWidth;
			this.UnboundExpressionMode = field.UnboundExpressionMode;
			this.SortByAttribute = field.SortByAttribute;
			this.AutoPopulatedProperties = field.AutoPopulatedProperties;
			this.DrillDownColumnName = field.DrillDownColumnName;
		}
		protected virtual PivotGridFieldOptions CreateOptions(PivotOptionsChangedEventHandler eventHandler, string name) {
			return new PivotGridFieldOptions(eventHandler, this, "Options");
		}
		internal bool IsDisposed { get { return isDisposed; } }
		[Browsable(false), DefaultValue(""), XtraSerializableProperty(), XtraSerializablePropertyId(-1),
		Localizable(false), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Name")]
		public virtual string Name {
			get {
				if(IsDataDeserializing) return name;
				if(this.Site != null) name = this.Site.Name;
				return name;
			}
			set {
				if(value == null)
					value = string.Empty;
				if(FieldName == value && value != string.Empty) {
					if(!IsDeserializing)
						throw new Exception(FieldNameEqualsNameExceptionString);
					else
						value = NamePrefix + value;
				}
				string oldName = name;
				name = value;
				if(Site != null) Site.Name = name;
				if(Collection != null)
					Collection.OnNameChanged(this, oldName);
			}
		}
		protected internal virtual string ComponentName { get { return Name; } }
		protected internal virtual PivotGridFieldBase GetFieldFromComponentName(string componentName) {
			return Collection.GetFieldByName(componentName);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void SetNameCore(string name) { this.name = name; }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseArea"),
#endif
 Category("Behaviour"),
		XtraSerializableProperty(XtraSerializationFlags.DefaultValue, 0), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdLayout), DefaultValue(PivotArea.FilterArea),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Area")
		]
		public PivotArea Area {
			get { return GetArea(); }
			set {
				if(Area == value || !IsAreaAllowed(value)) return;
				SetArea(value);
			}
		}
		protected virtual PivotArea GetArea() {
			if(IsDataField) {
				return Data.OptionsDataField.DataFieldArea;
			}
			if(HasGroup && !IsDeserializing)
				area = Group.Area;
			return area;
		}
		protected virtual void SetArea(PivotArea value) {
			if(IsDataField) {
				Data.OptionsDataField.DataFieldArea = value;
				return;
			}
			if(!CanChangeArea) return;
			area = value;
			if(!IsLoading)
				this.areaIndex = -1;
			UpdateAreaIndex();
			OnAreaChanged(true);
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanChangeLocationTo(PivotArea newArea, int newAreaIndex) {
			if(IsDataField)
				return CanChangeDataFieldLocationTo(newArea, newAreaIndex, -1);
			if(Data.DataField.Visible && Data.DataField.Area == newArea && (newArea == PivotArea.RowArea || newArea == PivotArea.ColumnArea) && newAreaIndex >= 0) {
				List<PivotGridFieldBase> fields = Data.GetFieldsByArea(newArea, true);
				if(fields.Contains(this))
					fields.Remove(this);
				int index = newAreaIndex > fields.Count ? fields.Count : newAreaIndex;
				fields.Sort(new PivotGridFieldAreaIndexComparer());
				fields.Insert(index, this);
				int areaIndex = fields.IndexOf(Data.DataField);
				if(!CanChangeDataFieldLocationTo(newArea, areaIndex, fields.Count - 1))
					return false;
			}
			return IsAreaAllowed(newArea);
		}
		protected virtual internal bool CanChangeDataFieldLocationTo(PivotArea newArea, int newAreaIndex, int fieldCount) {
			if(newArea != PivotArea.ColumnArea && newArea != PivotArea.RowArea) return false;
			if(newArea == PivotArea.RowArea) {
				if(Data == null || Data.OptionsView.RowTotalsLocation != PivotRowTotalsLocation.Tree)
					return true;
				if(fieldCount == -1)
					fieldCount = Data.GetFieldCountByArea(PivotArea.RowArea);
				return newAreaIndex <= 0 || newAreaIndex >= fieldCount;
			}
			return true;
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseAreaIndex"),
#endif
 Category("Behaviour"),
		DefaultValue(-1), XtraSerializableProperty(XtraSerializationFlags.DefaultValue, 1), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdLayout),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.AreaIndex")
		]
		public virtual int AreaIndex {
			get {
				if(IsDataField) {
					return Data.OptionsDataField.DataFieldAreaIndex;
				}
				return areaIndex;
			}
			set {
				if(IsDataField) {
					Data.OptionsDataField.DataFieldAreaIndex = value;
					return;
				}
				if(!CanChangeArea) return;
				if(value < 0) {
					this.areaIndex = -1;
					Visible = false;
					return;
				}
				if(AreaIndex == value && !IsDataDeserializing) return;
				if(Data != null && !IsDataDeserializing) this.visible = true;
				AreaIndexCore = value;
				UpdateAreaIndex();
				OnAreaIndexChanged(true, true);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseVisible"),
#endif
 Category("Behaviour"),
		DefaultValue(true), XtraSerializableProperty(XtraSerializationFlags.DefaultValue, 1000), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdLayout),
		MergableProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Visible"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public virtual bool Visible {
			get {
				if(IsDataField) {
					return Data.OptionsDataField.DataFieldVisible;
				}
				if(HasGroup)
					visible = Group.IsFieldVisible(this);
				return visible;
			}
			set {
				if(IsDataField) {
					Data.OptionsDataField.DataFieldVisible = value;
					return;
				}
				if(Visible == value) return;
				if(!CanChangeArea) return;
				if(value && !IsAreaAllowed(Area)) return;
				visible = value;
				int oldIndex = areaIndex;
				UpdateAreaIndex();
				OnVisibleChanged(oldIndex, true);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseAllowedAreas"),
#endif
 Category("Behaviour"),
		DefaultValue(PivotGridAllowedAreas.All), XtraSerializableProperty(), NotifyParentProperty(true),
#if !SL && !DXPORTABLE
 Editor("DevExpress.Utils.Editors.AttributesEditor, " + AssemblyInfo.SRAssemblyUtils, typeof(System.Drawing.Design.UITypeEditor)),
#endif
 DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.AllowedAreas")]
		public PivotGridAllowedAreas AllowedAreas {
			get { return allowedAreas; }
			set {
				if(value == AllowedAreas) return;
				allowedAreas = value;
				if(!IsLoading && !IsAreaAllowed(Area)) {
					bool visibleBefore = Visible;
					Visible = false;
					if(!visibleBefore) {
						DoLayoutChanged();
						return;
					}
				}
				OnPropertyChanged();
			}
		}
		public bool IsAreaAllowed(PivotArea area) {
			if(IsDeserializing) return true;
			if(Data != null && !Data.IsAreaAllowed(this, area)) return false;
			if(IsDataField && !(area == PivotArea.ColumnArea || area == PivotArea.RowArea))
				return false;
			if(AllowedAreas == PivotGridAllowedAreas.All) return true;
			int test = Pow(2, (int)area);		   
			return (test & (int)AllowedAreas) != 0;
		}
		internal static int Pow(int num, int pow) {
			if(pow == 0)
				return 1;
			int result = num;
			for(int i = 1; i < pow; i++)
			   result *= num;
			return result;
		}
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseRunningTotal")]
#endif
		[Category("Data"), XtraSerializableProperty(), DefaultValue(false), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.RunningTotal")]
		[TypeConverter(typeof(BooleanTypeConverter))]
		public bool RunningTotal {
			get { return runningTotal; }
			set {
				if(runningTotal == value)
					return;
				runningTotal = value;
				OnRunningTotalChanged();
			}
		}
		void OnRunningTotalChanged() {
			OnCalculationsChanged();
			if(IsColumnOrRow)
				if(Data != null && Data.IsServerMode)
					DoRefresh();
				else
					DoReloadData();
		}
		[Category("Data"), DefaultValue(true), NotifyParentProperty(true)]
		[TypeConverter(typeof(BooleanTypeConverter))]
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseShowNewValues")]
#endif
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.ShowNewValues")]
		public bool ShowNewValues {
			get { return FilterValues.FilterType == PivotFilterType.Excluded; }
			set {
				PivotFilterType newFilterType = value ? PivotFilterType.Excluded : PivotFilterType.Included;
				if(FilterValues.FilterType == newFilterType) return;
				FilterValues.SetFilterType(newFilterType);
				OnPropertyChanged();
			}
		}
		[Browsable(false)]
		public bool IsLoading { get { return Data == null || Data.IsLoading; } }
		protected internal virtual bool IsDataDeserializing { get { return Data != null && Data.IsDeserializing; } }
		internal PivotArea AreaCore { get { return area; } }
		internal bool VisibleCore { get { return visible; } }
		internal int AreaIndexCore {
			get { return areaIndex; }
			set {
				if(!IsDataDeserializing)
					AreaIndexOldCore = areaIndex;
				areaIndex = value;
				OnAreaIndexCoreChagned();
			}
		}
		protected virtual void OnAreaIndexCoreChagned() {
		}
		internal int AreaIndexOldCore { get { return areaIndexOld; } set { areaIndexOld = value; } }
		internal protected void FixOldAreaIndex() {
			if(AreaIndexCore >= 0 && AreaIndexOldCore == -1)
				AreaIndexOldCore = Int32.MaxValue;
		}
		protected bool CanChangeArea { get { return (Group == null || IsDeserializing || Group.CanChangeArea(this)); } }
		protected void UpdateAreaIndex() {
			UpdateAreaIndex(false);
		}
		protected void UpdateAreaIndex(bool ignoreMe) {
			if(IsLockAreaIndexUpdate) return;
			if(HasGroup)
				Group.UpdateAreaIndexes(ignoreMe ? this : null);
			if(Collection != null)
				Collection.UpdateAreaIndexes(ignoreMe ? this : null);
		}
		[Browsable(false)]
		public bool IsColumnOrRow { get { return Visible && (Area == PivotArea.RowArea || Area == PivotArea.ColumnArea); } }
		[Browsable(false)]
		public bool IsColumn { get { return Visible && Area == PivotArea.ColumnArea; } }
		protected void BeginUpdate() {
			if(Data != null) Data.BeginUpdate();
		}
		protected void EndUpdate() {
			if(Data != null) Data.EndUpdate();
		}
		protected void CancelUpdate() {
			if(Data != null) Data.CancelUpdate();
		}
		protected bool IsLockUpdate { get { return Data != null ? Data.IsLockUpdate : false; } }
		int areaIndexUpdateLock;
		protected void LockAreaIndexUpdate() {
			areaIndexUpdateLock++;
		}
		protected void UnlockAreaIndexUpdate() {
			areaIndexUpdateLock--;
			if(areaIndexUpdateLock < 0)
				throw new Exception("Unpaired UnlockAreaIndexUpdate method call.");
		}
		protected bool IsLockAreaIndexUpdate {
			get { return areaIndexUpdateLock > 0; }
		}
		public bool SetAreaPosition(PivotArea area, int areaIndex) {
			if(!IsAreaAllowed(area)) return false;
			if(area == Area && AreaIndex == areaIndex && Visible && !(!IsDataField && Data.DataField.Area == area && areaIndex == Data.DataField.AreaIndex)) return false;
			bool dataFieldMoving = Area == PivotArea.DataArea && area == PivotArea.DataArea && Visible;
			bool filterFieldShow = !Visible && !Data.IsOLAP && area == PivotArea.FilterArea && !NeedApplyFilterOnShowInFilterArea;
			bool filterFieldMoving = Area == PivotArea.FilterArea && area == PivotArea.FilterArea && Visible || filterFieldShow;
			bool oldVisible = Visible;
			PivotArea oldArea = Area;
			int oldAreaIndex = AreaIndex;
			BeginUpdate();
			LockAreaIndexUpdate();
			try {
				this.visible = true;
				Area = area;
				AreaIndex = areaIndex;
			} finally {
				UnlockAreaIndexUpdate();
				UpdateAreaIndex(true);
				if(!dataFieldMoving && !filterFieldMoving) {
					EndUpdate();
				} else {
					if(dataFieldMoving)
						MoveDataField(AreaIndex, oldAreaIndex);
					Data.GetSortedFields();
					CancelUpdate();
					Data.LayoutChanged();
				}
			}
			if(oldArea != Area || oldVisible != Visible)
				OnAreaChanged(false);
			if(oldVisible != Visible)
				OnVisibleChanged(oldAreaIndex, false);
			if(oldAreaIndex != AreaIndex)
				OnAreaIndexChanged(false, dataFieldMoving);
			return true;
		}
		void MoveDataField(int areaIndex, int oldAreaIndex) {
			Data.MoveDataField(this, oldAreaIndex, areaIndex);
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseGroupInterval"),
#endif
 Category("Behaviour"),
		DefaultValue(PivotGroupInterval.Default), XtraSerializableProperty(), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdData),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.GroupInterval"),
		]
		public PivotGroupInterval GroupInterval {
			get { return groupInterval; }
			set {
				if(GroupInterval == value) return;
				this.groupInterval = value;
				OnGroupIntervalChanged();
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsGroupIntervalNumeric {
			get { return GroupIntervalHelper.IsNumericInterval(GroupInterval); }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseGroupIntervalNumericRange"),
#endif
 Category("Behaviour"),
		DefaultValue(10), XtraSerializableProperty(), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdData),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.GroupIntervalNumericRange")
		]
		public int GroupIntervalNumericRange {
			get { return groupIntervalNumericRange; }
			set {
				if(value < 1) return;
				if(GroupIntervalNumericRange == value) return;
				this.groupIntervalNumericRange = value;
				OnGroupIntervalNumericRangeChanged();
			}
		}
		protected internal int GroupIntervalColumnHandle { get { return groupIntervalColumnHandle; } set { groupIntervalColumnHandle = value; } }
		[Category("Behaviour"), Browsable(false)]
		public virtual bool CanShowInCustomizationForm {
			get {
				if(!Options.ShowInCustomizationForm) return false;
				return Group == null || Group.IndexOf(this) == 0;
			}
		}
		[Browsable(false)]
		public virtual bool CanShowInPrefilter {
			get {
				if(!Options.ShowInPrefilter) return false;
				return IsValidForPrefilter;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsValidForPrefilter {
			get {
				return Area != PivotArea.DataArea || Data != null && Data.OptionsDataField.EnableFilteringByData;
			}
		}
		void ResetOptions() { Options.Reset(); }
		bool ShouldSerializeOptions() { return Options.ShouldSerialize(); }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseOptions"),
#endif
 Category("Options"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Options")]
		public PivotGridFieldOptions Options { get { return options; } }
		protected internal void MakeVisible() {
			visible = true;
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseFieldName"),
#endif
 Category("Data"),
		Localizable(false), DefaultValue(""), XtraSerializableProperty(), MergableProperty(false), NotifyParentProperty(true),
#if !SL && !DXPORTABLE
 Editor("DevExpress.XtraPivotGrid.TypeConverters.PivotColumnNameEditor, " + AssemblyInfo.SRAssemblyPivotGrid, typeof(System.Drawing.Design.UITypeEditor)),
#endif
 DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.FieldName")
		]
		public virtual string FieldName {
			get { return fieldName; }
			set {
				if(value == null) value = string.Empty;
				if(FieldName == value) return;
				if(Name == value && value != string.Empty) {
					if(!IsDeserializing) throw new Exception(FieldNameEqualsNameExceptionString);
					else Name = NamePrefix + Name;
				}
				if(IsDefaultName && Collection != null && !IsDeserializing)
					Name = Collection.GenerateName(value);
				if(Data != null && Data.IsDesignMode && Data.IsOLAP && IsOLAPMeasureFieldName(value))
					area = PivotArea.DataArea;
				fieldName = value;
				isAggregatable = DefaultBoolean.Default;
				OnFieldNameChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseUnboundFieldName"),
#endif
 Category("Data"),
		XtraSerializableProperty(), MergableProperty(false), DefaultValue(""), NotifyParentProperty(true), Localizable(false)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.UnboundFieldName")]
		public string UnboundFieldName {
			get { return unboundFieldName; }
			set {
				if(value == null) value = string.Empty;
				if(unboundFieldName == value) return;
				unboundFieldName = value;
				OnUnboundFieldNameChanged();
			}
		}
		internal string GetFieldName(ref int unboundId) {
			if(FieldName != string.Empty && GroupInterval == PivotGroupInterval.Default) {
				if(!IsUnbound)
					return FieldName;
				else if(SameFieldNameCount == 1)
					return FieldName;
			}
			if(IsUnbound) {
				if(string.IsNullOrEmpty(UnboundFieldName)) {
					this.unboundFieldName = !string.IsNullOrEmpty(Name) ? Name : "UnboundColumn" + unboundId.ToString();
					unboundId++;
				}
				return UnboundFieldName;
			}
			return string.Empty;
		}
		protected bool IsDefaultName {
			get {
				return Collection != null && Collection.IsDefaultName(Name, FieldName);
			}
		}
		[Browsable(false)]
		public virtual bool IsOLAPField {
			get { return IsOLAPFieldName(FieldName); }
		}
		internal bool IsOLAPMeasure {
			get { return FieldName.StartsWith("[Measures]."); }
		}
		[Browsable(false)]
		public string Hierarchy {
			get {
				if(Data != null && Data.IsOLAP) {
					string val = Data.GetHierarchyName(FieldName);
					if(val != null)
						return val;
				}
				if(IsOLAPField && !IsOLAPMeasure) {
					int lastIndex = FieldName.LastIndexOf('.');
					if(lastIndex >= 0)
						return FieldName.Substring(0, lastIndex);
				}
				return string.Empty;
			}
		}
		internal virtual int Level { get { return Data != null ? Data.GetFieldHierarchyLevel(this) : PivotGridFieldBase.DefaultLevel; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseDataType"),
#endif
 Category("Data"), Browsable(false)]
		public virtual Type DataType {
			get { return Data != null ? Data.GetFieldType(this) : typeof(object); }
		}
		internal UnboundColumnType ActualUnboundType {
			get {
				if(GroupInterval == PivotGroupInterval.Default)
					return UnboundType;
				return GroupIntervalHelper.GetUnboundType(GroupInterval);
			}
		}
		internal bool ActualOLAPFilterByUniqueName {
			get {
				if(Group != null && Group[0] != this && Data.OptionsFilter.GroupFilterMode == PivotGroupFilterMode.Tree)
					return Group[0].ActualOLAPFilterByUniqueName;
				return Options.OLAPFilterByUniqueName != DefaultBoolean.Default || Data == null ? Options.OLAPFilterByUniqueName == DefaultBoolean.True : Data.OptionsOLAP.FilterByUniqueName;
			}
		}
		[
		EditorBrowsable(EditorBrowsableState.Never), Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Type ActualDataType {
			get {
				if(Area == PivotArea.DataArea && UnboundType != UnboundColumnType.Bound && SummaryType == PivotSummaryType.Custom)
					return DataType;
				if(GroupInterval == PivotGroupInterval.DateDayOfWeek)
					return typeof(DayOfWeek);
				switch(ActualUnboundType) {
					case UnboundColumnType.Object:
						return typeof(object);
					case UnboundColumnType.String:
						return typeof(string);
					case UnboundColumnType.Decimal:
						return typeof(decimal);
					case UnboundColumnType.Integer:
						return typeof(int);
					case UnboundColumnType.DateTime:
						return typeof(DateTime);
					case UnboundColumnType.Boolean:
						return typeof(bool);
					default:
						return DataType;
				}
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseUnboundType"),
#endif
 Category("Data"),
		DefaultValue(UnboundColumnType.Bound), XtraSerializableProperty(), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.UnboundType")
		]
		public UnboundColumnType UnboundType {
			get { return unboundType; }
			set {
				if(UnboundType == value) return;
				unboundType = value;
				OnUnboundChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseUnboundExpression"),
#endif
 Category("Data"),
		DefaultValue(""), XtraSerializableProperty(), NotifyParentProperty(true), Localizable(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder),
				DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.UnboundExpression"),
#if !SL && !DXPORTABLE
 Editor("DevExpress.XtraPivotGrid.TypeConverters.PivotExpressionEditor, " + AssemblyInfo.SRAssemblyPivotGrid,
			"System.Drawing.Design.UITypeEditor, System.Drawing")
#endif
]
		public string UnboundExpression {
			get { return unboundExpression; }
			set {
				if(value == null) value = string.Empty;
				if(UnboundExpression == value) return;
				unboundExpression = value;
				OnUnboundExpressionChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseUnboundExpressionMode"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.UnboundExpressionMode"),
		NotifyParentProperty(true), DefaultValue(UnboundExpressionMode.Default),
		XtraSerializablePropertyId(LayoutIdData), Category("Data"), XtraSerializableProperty()
		]
		public UnboundExpressionMode UnboundExpressionMode {
			get { return unboundExpressionMode; }
			set { unboundExpressionMode = value; }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseTopValueMode"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.TopValueMode"),
		NotifyParentProperty(true), DefaultValue(TopValueMode.Default),
		XtraSerializablePropertyId(LayoutIdData), Category("Data"),  XtraSerializableProperty()
		]
		public TopValueMode TopValueMode {
			get { return topValueMode; }
			set { topValueMode = value; }
		}
		[Browsable(false)]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[DefaultValue(true)]
		public bool CalculateTotals {
			get { return calculateTotals; }
			set { calculateTotals = value; }
		}
		internal List<string> ExpressionColumnNames {
			get {
				if(object.ReferenceEquals(ExpressionOperator, null))
					return null;
				if(expressionColumnNames == null) {
					expressionColumnNames = new List<string>();
					ColumnNamesCriteriaVisitor visitor = new ColumnNamesCriteriaVisitor(false);
					ExpressionOperator.Accept(visitor);
					expressionColumnNames.AddRange(visitor.ColumnNames);
					if(mixedCriteriaVisitor != null) {
						foreach(var pair in mixedCriteriaVisitor.SummaryLevel) {
							expressionColumnNames.Add(pair.Value.DataSourceLevelName);
							foreach(var criteria in pair.Value.SummaryCriterias)
								expressionColumnNames.Add(criteria.SummaryLevelName);
						}
					}
				}
				return expressionColumnNames;
			}
		}
		internal MixedSummaryLevelCriteriaVisitor MixedSummaryLevelCriteriaVisitor {
			get {
				if(unboundExpressionMode != XtraPivotGrid.UnboundExpressionMode.UseAggregateFunctions || ReferenceEquals(ExpressionOperator, null))
					return null;
				return mixedCriteriaVisitor;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public CriteriaOperator ExpressionOperator {
			get {
				if(object.ReferenceEquals(expressionOperator, null)) {
					if(string.IsNullOrEmpty(UnboundExpression))
						expressionOperator = null;
					try {
						if(UnboundExpressionMode == XtraPivotGrid.UnboundExpressionMode.UseAggregateFunctions) {
							mixedCriteriaVisitor = new MixedSummaryLevelCriteriaVisitor((PivotGridNativeDataSource)Data.PivotDataSource);
							expressionOperator = mixedCriteriaVisitor.Process(CriteriaOperator.Parse(UnboundExpression, null));
						} else
							expressionOperator = CriteriaOperator.Parse(UnboundExpression, null);
					} catch(DevExpress.Data.Filtering.Exceptions.CriteriaParserException) {
						expressionOperator = CriteriaOperator.Parse(string.Empty, null);
					}
				}
				return expressionOperator;
			}
		}
		[Browsable(false), 
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseIsUnboundExpression")
#else
	Description("")
#endif
]
		public bool IsUnboundExpression {
			get { return IsUnbound && !string.IsNullOrEmpty(UnboundExpression); }
		}
		[Browsable(false), 
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseIsUnboundExpressionValid")
#else
	Description("")
#endif
]
		public bool IsUnboundExpressionValid {
			get {
				return Data != null ? Data.IsUnboundExpressionValid(this) : true;
			}
		}
		internal bool IsSummaryExpressionDataField {
			get { return IsUnboundExpression && Area == PivotArea.DataArea && Data != null && IsProcessOnSummaryLevel; }
		}
		protected internal int SameFieldNameCount {
			get { return Collection != null ? Collection.GetSameFieldNameCount(this) : -1; }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseCaption"),
#endif
 MergableProperty(true),
		Category("Appearance"), DefaultValue(""), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdAppearance),
		Localizable(true), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Caption")
		]
		public string Caption {
			get { return GetCaptionCore(); }
			set {
				if(value == null) value = string.Empty;
				if(Caption == value) return;
				if(!IsDataField)
					caption = value;
				else
					Data.OptionsDataField.Caption = value;
				OnCaptionChanged();
			}
		}
		[Category("Appearance"), DefaultValue(""), XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout),
		Localizable(true), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.DisplayFolder")
		]
		public string DisplayFolder {
			get { return GetDisplayFolderCore(); }
			set {
				if(displayFolder == value) return;
				displayFolder = value;
				OnDisplayFolderChanged();
			}
		}
		[Browsable(false)]
		public string HeaderDisplayText {
			get {
				if(Options.ShowSummaryTypeName)
					return ToString() + " (" + PivotGridLocalizer.GetSummaryTypeText(SummaryType) + ")";
				else
					return ToString();
			}
		}
		protected virtual string GetCaptionCore() {
			return GetCaptionCore(false);
		}
		string GetCaptionCore(bool queryModeOnly) {
			if(!IsDataField) {
				string value;
				if(Data != null && (queryModeOnly || Data.IsServerMode) && string.IsNullOrEmpty(caption)) {
					value = Data.GetHierarchyCaption(FieldName);
					if(string.IsNullOrEmpty(value))
						value = caption;
				}
				else
					value = caption;
				return value == null ? "" : value;
			}
			if(Data.OptionsDataField.Caption != string.Empty) return Data.OptionsDataField.Caption;
			return PivotGridLocalizer.GetString(PivotGridStringId.DataFieldCaption);
		}
		protected virtual string GetDisplayFolderCore() {
			if(Data == null || !Data.IsServerMode|| !string.IsNullOrEmpty(displayFolder))
				return displayFolder;
			else
				return Data.GetHierarchyDisplayFolder(FieldName);
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseEmptyCellText"),
#endif
 NotifyParentProperty(true),
		Category("Appearance"), DefaultValue(""), XtraSerializableProperty(), Localizable(true),
		XtraSerializablePropertyId(LayoutIdAppearance),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.EmptyCellText")
		]
		public string EmptyCellText {
			get { return emptyCellText; }
			set {
				if(value == null) value = string.Empty;
				if(EmptyCellText == value) return;
				emptyCellText = value;
				OnEmptyCellTextChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseEmptyValueText"),
#endif
 NotifyParentProperty(true),
		Category("Appearance"), DefaultValue(""), XtraSerializableProperty(), Localizable(true),
		XtraSerializablePropertyId(LayoutIdAppearance),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.EmptyValueText")
		]
		public string EmptyValueText {
			get { return emptyValueText; }
			set {
				if(value == null) value = string.Empty;
				if(EmptyValueText == value) return;
				emptyValueText = value;
				OnEmptyValueTextChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseGrandTotalText"),
#endif
 NotifyParentProperty(true),
		Category("Appearance"), DefaultValue(""), XtraSerializableProperty(), Localizable(true),
		XtraSerializablePropertyId(LayoutIdAppearance),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.GrandTotalText")
		]
		public string GrandTotalText {
			get { return grandTotalText; }
			set {
				if(value == null) value = string.Empty;
				if(GrandTotalText == value) return;
				grandTotalText = value;
				OnEmptyCellTextChanged();
			}
		}
		internal int IndexInternal { get { return indexInternal; } set { indexInternal = value; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool IsNew { get { return isNew; } set { isNew = value; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty()]
		public int Index {
			get { return Collection != null ? Collection.IndexOf(this) : -1; }
			set {
				if(IsDeserializing)
					IndexInternal = value;
				else {
					if(Collection != null)
						Collection.SetFieldIndex(this, value);
				}
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseSummaryType"),
#endif
 Category("Data"),
		DefaultValue(PivotSummaryType.Sum), XtraSerializableProperty(), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdData),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.SummaryType")
		]
		public PivotSummaryType SummaryType {
			get { return summaryType; }
			set {
				if(value != summaryType) {
					PivotSummaryType oldSummaryType = summaryType;
					summaryType = value;
					OnSummaryChanged(oldSummaryType);
				}
			}
		}
		[
		Category("Data"),
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdData), Browsable(false)
		]
		public bool UseDecimalValuesForMaxMinSummary {
			get { return useDecimalValuesForMaxMinSummary; }
			set {
				if(value == useDecimalValuesForMaxMinSummary)
					return;
				useDecimalValuesForMaxMinSummary = value;
				if(Area == PivotArea.DataArea && Data != null && !Data.IsServerMode)
					DoReloadData();
			}
		}
		internal bool IsKPIMeasure {
			get { return KPIType != PivotKPIType.None; }
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseSummaryDisplayType"),
#endif
 Category("Data"),
		DefaultValue(PivotSummaryDisplayType.Default), XtraSerializableProperty(20), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdData),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.SummaryDisplayType")
		]
		public PivotSummaryDisplayType SummaryDisplayType {
			get { return summaryDisplayType; }
			set {
				if(value != summaryDisplayType) {
					summaryDisplayType = value;
					OnCalculationsChanged();
				}
			}
		}
		internal bool IsOLAPSummaryVariation {
			get { return Data != null && Data.IsOLAP && Area == PivotArea.DataArea && SummaryDisplayType != PivotSummaryDisplayType.Default; }
		}
		void ResetSortBySummaryInfo() { SortBySummaryInfo.Reset(); }
		bool ShouldSerializeSortBySummaryInfo() { return SortBySummaryInfo.ShouldSerialize(); }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseSortBySummaryInfo"),
#endif
 Category("Behaviour"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content, XtraSerializationFlags.DefaultValue),
		XtraSerializablePropertyId(LayoutIdData),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.SortBySummaryInfo")
		]
		public PivotGridFieldSortBySummaryInfo SortBySummaryInfo { get { return sortBySummaryInfo; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseCalculations"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Calculations"),
		XtraSerializableProperty(XtraSerializationVisibility.Collection, 0),
		XtraSerializablePropertyId(LayoutIdData), NotifyParentProperty(true)
		]
		public IList<PivotCalculationBase> Calculations {
			get { return calculations; }
		}
		void OnCalculationChanged(object sender, EventArgs e) {
			OnCalculationsChanged();
		}
		void OnCalculationsChanged() {
			OnPropertyChanged();
			if(Data != null)
				Data.OnCalculationsChanged(this);
		}
		bool ShouldSerializeCalculations() {
			return calculations.Count > 0;
		}
		protected internal virtual IPivotCalculationCreator CalculationCreator { get { return PivotCalculationBase.BaseCreator; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseTopValueCount"),
#endif
 Category("Data"),
		DefaultValue(0), XtraSerializableProperty(), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdData),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.TopValueCount")
		]
		public int TopValueCount {
			get { return topValueCount; }
			set {
				if(value < 0) value = 0;
				if(TopValueCount == value) return;
				topValueCount = value;
				OnTopValuesChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseTopValueType"),
#endif
 Category("Data"),
		DefaultValue(PivotTopValueType.Absolute), XtraSerializableProperty(), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdData),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.TopValueType")
		]
		public PivotTopValueType TopValueType {
			get { return topValueType; }
			set {
				if(TopValueType == value) return;
				topValueType = value;
				OnTopValuesChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseTopValueShowOthers"),
#endif
 Category("Data"),
		DefaultValue(false), XtraSerializableProperty(), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdData),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.TopValueShowOthers"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool TopValueShowOthers {
			get { return topValueShowOthers; }
			set {
				if(TopValueShowOthers == value) return;
				topValueShowOthers = value;
				OnTopValuesChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseTotalsVisibility"),
#endif
		Category("Behaviour"), DefaultValue(PivotTotalsVisibility.AutomaticTotals), XtraSerializableProperty(),
		Localizable(false), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.TotalsVisibility")
		]
		public PivotTotalsVisibility TotalsVisibility {
			get { return totalsVisibility; }
			set {
				if(TotalsVisibility == value) return;
				totalsVisibility = value;
				OnTotalSummaryChanged();
			}
		}
		[Browsable(false)]
		public int GetTotalSummaryCount(bool singleValue) {
			PivotTotalsVisibility TotalsVisibility = GetTotalsVisibility();
			if(Data == null) return 0;
			if(TotalsVisibility == PivotTotalsVisibility.CustomTotals) {
				return !singleValue || Area == PivotArea.RowArea && Data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree || Data.OptionsView.ShowCustomTotalsForSingleValues ? CustomTotals.Count : 0;
			}
			bool isNoTotalsAllowed = IsColumn ||
					Data.OptionsView.RowTotalsLocation != PivotRowTotalsLocation.Tree;
			if(TotalsVisibility == PivotTotalsVisibility.None && isNoTotalsAllowed)
				return 0;
			if(!IsColumnOrRow)
				return 0;
			bool show = (Area == PivotArea.ColumnArea ? Data.OptionsView.ShowColumnTotals : Data.OptionsView.ShowRowTotals);
			show &= Options.ShowTotals;
			if(show && singleValue) {
				show = Area == PivotArea.RowArea && Data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree || Data.OptionsView.ShowTotalsForSingleValues;
			}
			return show ? 1 : 0;
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseHasNullValues"),
#endif
 Browsable(false)]
		public bool HasNullValues { get { return Data != null ? Data.HasNullValues(this) : false; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Content), NotifyParentProperty(true)]
		public PivotGridFieldFilterValues FilterValues {
			get { return filterValues; }
			set {
				filterValues.Assign(value);
			}
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		XtraSerializableProperty(XtraSerializationVisibility.Content, 1000), NotifyParentProperty(true)]
		public PivotSummaryFilter SummaryFilter {
			get { return summaryFilter; }
		}
		protected internal void XtraAssignFilteredValues(PivotGridFieldFilterValues filteredValues) {
			FilterValues.Assign(filteredValues);
		}
		protected internal void OnGridDeserialized(PivotGridFieldCollectionBase owner) {
			SortBySummaryInfo.OnGridDeserialized();
			SummaryFilter.OnGridDeserialized();
		}
		protected internal PivotGridCustomTotalCollectionBase CustomTotals {
			get { return customTotals; }
		}
#if DEBUGTEST
		public PivotGridCustomTotalCollectionBase GetCustomTotals() {
			return CustomTotals;
		}
#endif
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseSortOrder"),
#endif
 Category("Data"),
		DefaultValue(PivotSortOrder.Ascending), XtraSerializableProperty(10), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdData),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.SortOrder")
		]
		public PivotSortOrder SortOrder {
			get { return sortOrder; }
			set {
				if(SortOrder == value) return;
				sortOrder = value;
				OnSortOrderChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseSortByAttribute"),
#endif
 XtraSerializableProperty(0), Localizable(false),
		Category("Data"), DefaultValue(null), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdData),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.SortByAttribute")
		]
		public string SortByAttribute {
			get { return sortByAttribute; }
			set {
				if(sortByAttribute == value)
					return;
				sortByAttribute = value;
				OnSortModeChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseAutoPopulatedProperties"),
#endif
 XtraSerializableProperty(0),
		Category("Data"), DefaultValue(null), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdData),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.AutoPopulatedProperties")
		]
		public string[] AutoPopulatedProperties {
			get { return autoPopulatedProperties; }
			set {
				if(autoPopulatedProperties == value)
					return;
				autoPopulatedProperties = value;
				OnSortModeChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseSortMode"),
#endif
 XtraSerializableProperty(0),
		Category("Data"), DefaultValue(PivotSortMode.Default), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdData),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.SortMode")
		]
		public PivotSortMode SortMode {
			get { return sortMode; }
			set {
				if(SortMode == value && actualSortMode == null) return;
				sortMode = value;
				actualSortMode = null;
				OnSortModeChanged();
			}
		}
		[
		EditorBrowsable(EditorBrowsableState.Never), Browsable(false), DefaultValue(PivotSortMode.Default),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), XtraSerializableProperty(5)
		]
		public PivotSortMode ActualSortMode {
			get { return actualSortMode.HasValue ? actualSortMode.Value : SortMode; }
			set {
				if(actualSortMode == value && value != PivotSortMode.None) return;
				if(value == PivotSortMode.None) actualSortMode = null;
				else actualSortMode = value;
				OnSortModeChanged();
			}
		}
		[Browsable(false)]
		public bool IsOLAPSortModeNone { get { return (SortMode == PivotSortMode.None && IsOLAPField && IsColumnOrRow); } }
		[Browsable(false)]
		public bool IsOLAPNotSorted { get { return (IsOLAPSortModeNone && SortBySummaryInfo.IsEmpty && actualSortMode == null); } }
		[Browsable(false)]
		public bool IsOLAPSorted { get { return (IsOLAPSortModeNone && SortBySummaryInfo.IsEmpty && actualSortMode != null); } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool CanSortOLAP { get { return IsOLAPNotSorted && CanSortCore; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseWidth"),
#endif
 Category("Layout"),
		DefaultValue(100), XtraSerializableProperty(100), Localizable(true), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdLayout),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Width")
		]
		public virtual int Width {
			get {
				if(IsDataField) {
					return Data.OptionsDataField.RowHeaderWidth;
				}
				if(width < 0)
					return DefaultWidth;
				else return width;
			}
			set {
				if(IsDataField) {
					Data.OptionsDataField.RowHeaderWidth = value;
					OnPropertyChanged();
					return;
				}
				if(value < -1) value = -1;
				if(value == Width) return;
				if(value >= 0 && value < MinWidth)
					value = MinWidth;
				width = value;
				OnWidthChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseMinWidth"),
#endif
		Category("Layout"), DefaultValue(DefaultMinWidth),
		XtraSerializableProperty(10), Localizable(true), NotifyParentProperty(true),
		XtraSerializablePropertyId(LayoutIdLayout),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.MinWidth")
		]
		public virtual int MinWidth {
			get {
				if(minWidth < 0)
					return DefaultMinWidth;
				else
					return minWidth;
			}
			set {
				if(value < -1) value = -1;
				if(value > Width) value = Width;
				minWidth = value;
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseColumnValueLineCount"),
#endif
 DefaultValue(1), Category("Appearance"),
		XtraSerializableProperty(), Localizable(true), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.ColumnValueLineCount")
		]
		public int ColumnValueLineCount {
			get {
				if(Data != null && (IsDataField || Area == PivotArea.DataArea))
					return Data.OptionsDataField.ColumnValueLineCount;
				else
					return columnValueLineCount;
			}
			set {
				if(value < MinimumValueLineCount) value = MinimumValueLineCount;
				if(value > MaximumColumnValueLineCount) value = MaximumColumnValueLineCount;
				if(value == ColumnValueLineCount) return;
				columnValueLineCount = value;
				if(Data != null && (IsDataField || Area == PivotArea.DataArea))
					Data.OptionsDataField.ColumnValueLineCount = value;
				OnColumnRowValueCountChanged();
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseRowValueLineCount"),
#endif
 DefaultValue(1), Category("Appearance"),
		XtraSerializableProperty(), Localizable(true), NotifyParentProperty(true),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridField.RowValueLineCount")
		]
		public int RowValueLineCount {
			get {
				if(Data != null && (IsDataField || Area == PivotArea.DataArea))
					return Data.OptionsDataField.RowValueLineCount;
				else
					return rowValueLineCount;
			}
			set {
				if(value < MinimumValueLineCount) value = MinimumValueLineCount;
				if(value > MaximumRowValueLineCount) value = MaximumRowValueLineCount;
				if(value == RowValueLineCount) return;
				rowValueLineCount = value;
				if(Data != null && (IsDataField || Area == PivotArea.DataArea))
					Data.OptionsDataField.RowValueLineCount = value;
				OnColumnRowValueCountChanged();
			}
		}
		void ResetCellFormat() { CellFormat.Reset(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool XtraShouldSerializeCellFormat() { return ShouldSerializeCellFormat(); }
		bool ShouldSerializeCellFormat() { return !CellFormat.IsEmpty; }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseCellFormat"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.CellFormat")]
		public FormatInfo CellFormat { get { return cellFormat; } }
		void ResetTotalCellFormat() { TotalCellFormat.Reset(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool XtraShouldSerializeTotalCellFormat() { return ShouldSerializeTotalCellFormat(); }
		bool ShouldSerializeTotalCellFormat() { return !TotalCellFormat.IsEmpty; }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseTotalCellFormat"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.TotalCellFormat")]
		public FormatInfo TotalCellFormat { get { return totalCellFormat; } }
		void ResetGrandTotalCellFormat() { GrandTotalCellFormat.Reset(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool XtraShouldSerializeGrandTotalCellFormat() { return ShouldSerializeGrandTotalCellFormat(); }
		bool ShouldSerializeGrandTotalCellFormat() { return !GrandTotalCellFormat.IsEmpty; }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseGrandTotalCellFormat"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.GrandTotalCellFormat")]
		public FormatInfo GrandTotalCellFormat { get { return grandTotalCellFormat; } }
		void ResetValueFormat() { ValueFormat.Reset(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool XtraShouldSerializeValueFormat() { return ShouldSerializeValueFormat(); }
		bool ShouldSerializeValueFormat() { return !ValueFormat.IsEmpty; }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseValueFormat"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.ValueFormat")]
		public FormatInfo ValueFormat { get { return valueFormat; } }
		void ResetTotalValueFormat() { TotalValueFormat.Reset(); }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool XtraShouldSerializeTotalValueFormat() { return ShouldSerializeTotalValueFormat(); }
		bool ShouldSerializeTotalValueFormat() { return !TotalValueFormat.IsEmpty; }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseTotalValueFormat"),
#endif
 Category("Appearance"),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content), NotifyParentProperty(true),
		XtraSerializableProperty(XtraSerializationVisibility.Content), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.TotalValueFormat")]
		public FormatInfo TotalValueFormat { get { return totalValueFormat; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseTag"),
#endif
 XtraSerializableProperty(), Category("Data"), DefaultValue(null),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Visible), NotifyParentProperty(true),
#if !SL && !DXPORTABLE
 Editor(typeof(DevExpress.Utils.Editors.UIObjectEditor), typeof(System.Drawing.Design.UITypeEditor)),
		TypeConverter(typeof(PivotObjectEditorTypeConverter)),
#endif
 DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.Tag")]
		public object Tag { get { return tag; } set { tag = value; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseUseNativeFormat"),
#endif
 NotifyParentProperty(true),
		Category("Data"), DefaultValue(DefaultBoolean.Default), XtraSerializableProperty(), Localizable(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.UseNativeFormat")]
		public DefaultBoolean UseNativeFormat {
			get { return useNativeFormat; }
			set {
				useNativeFormat = value;
				OnPropertyChanged();
			}
		}
		[Browsable(false)]
		public int ColumnHandle { get { return columnHandle; } }
		public void ChangeSortOrder() {
			if(IsOLAPNotSorted && SortBySummaryInfo.IsEmpty) {
				ActualSortMode = PivotSortMode.DisplayText;
				SortOrder = PivotSortOrder.Ascending;
			} else if(CanSort || (CanSortBySummary && !SortBySummaryInfo.IsEmpty)) {
				SortOrder = SortOrder == PivotSortOrder.Ascending ? PivotSortOrder.Descending : PivotSortOrder.Ascending;
			}
		}
		public void CollapseAll() {
			if(Data != null)
				Data.ChangeFieldExpanded(this, false);
		}
		public void ExpandAll() {
			if(Data != null)
				Data.ChangeFieldExpanded(this, true);
		}
		public void CollapseValue(object value) {
			if(Data != null)
				Data.ChangeFieldExpanded(this, false, value);
		}
		public void ExpandValue(object value) {
			if(Data != null)
				Data.ChangeFieldExpanded(this, true, value);
		}
		protected internal virtual string GetValueText(bool isColumn, int index, object value) {
			if(value == null)
				return EmptyValueText;
			FormatInfo format = ValueFormat;
			if(GroupInterval != PivotGroupInterval.Default && value != PivotSummaryValue.ErrorValue) {
				string text = GroupIntervalHelper.GetTextValue(GroupInterval, CultureInfo.CurrentCulture, value, GroupIntervalNumericRange, format, defaultDateFormat);
				if(text != null)
					return text;
			}
			if(format.IsEmpty) {
				string serverText = GetServerDisplayText(isColumn, index, value);
				if(serverText != null)
					return serverText;
			}
			return format.GetDisplayText(value);
		}
		public virtual string GetValueText(IOLAPMember member) {
			if(member.Value == null)
				return EmptyValueText;
			FormatInfo format = ValueFormat;
			if(format.IsEmpty && !object.ReferenceEquals(null, member.Caption))
				return member.Caption;
			return format.GetDisplayText(member.Value);
		}
		public virtual string GetValueText(object value) {
			return GetValueText(false, -1, value);
		}
		protected internal virtual string GetTotalValueText(bool isColumn, int index, object value) {
			if(!TotalValueFormat.IsEmpty) {
				string serverText = GetServerDisplayText(isColumn, index, value);
				if(serverText != null)
					return TotalValueFormat.GetDisplayText(serverText);
				return TotalValueFormat.GetDisplayText(value);
			} else {
				return defaultTotalFormat.GetDisplayText(GetValueText(isColumn, index, value));
			}
		}
		public virtual string GetTotalValueText(object value) {
			return GetTotalValueText(false, -1, value);
		}
		public virtual string GetTotalOthersText() {
			return defaultTotalFormat.GetDisplayText(PivotGridLocalizer.GetString(PivotGridStringId.TopValueOthersRow));
		}
		protected internal virtual string GetDisplayText(bool isColumn, int index, object value) {
			return Data != null ? Data.GetCustomFieldValueText(this, value) : GetValueText(isColumn, index, value);
		}
		public virtual string GetDisplayText(object value) {
			return Data != null ? Data.GetCustomFieldValueText(this, value) : GetValueText(value);
		}
		string GetServerDisplayText(bool isColumn, int index, object value) {
			if(Data != null) {
				IOLAPMember member;
				if(index != -1)
					member = Data.GetOLAPMember(isColumn, index);
				else
					member = Data.GetOLAPMemberByValue(FieldName, value);
				if(member != null)
					return member.Caption;
			}
			return null;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public virtual PivotTotalsVisibility GetTotalsVisibility() {
			if(totalsVisibility == PivotTotalsVisibility.CustomTotals &&
				CustomTotals.Count == 0 &&
				area == PivotArea.RowArea && Data != null &&
				Data.OptionsView.RowTotalsLocation == PivotRowTotalsLocation.Tree)
				return PivotTotalsVisibility.AutomaticTotals;
			return totalsVisibility;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public FormatInfo GetValueFormat(PivotGridValueType valueType) {
			if(valueType == PivotGridValueType.Total)
				return !TotalValueFormat.IsEmpty ? TotalValueFormat : defaultTotalFormat;
			if(valueType == PivotGridValueType.Value)
				return ValueFormat;
			return null;
		}
		public virtual string GetGrandTotalText() {
			if(GrandTotalText != string.Empty)
				return GrandTotalText;
			else
				return ToString() + " " + PivotGridLocalizer.GetString(PivotGridStringId.Total);
		}
		protected internal void SetColumnHandle(int columnHandle) {
			this.columnHandle = columnHandle;
		}
		protected internal PivotGridFieldCollectionBase Collection { get { return collection; } }
		protected void OnFieldCollectionChanged() {
			if(Data != null)
				Data.FieldCollectionChanged();
		}
		protected internal virtual PivotGridData Data {
			get {
				if(IsDataField) {
					return dataFieldData;
				}
				return Collection != null ? Collection.Data : null;
			}
		}
		protected PivotGridGroupCollection Groups { get { return Data != null ? Data.Groups : null; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public PivotGridGroup Group {
			get { return group; }
			internal set { group = value; }
		}
		protected internal bool HasGroup {
			get { return Group != null; }
		}
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int InnerGroupIndex { get { return HasGroup ? Group.IndexOf(this) : -1; } }
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int GroupIndex { get { return HasGroup ? Groups.IndexOf(Group) : -1; } }
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseExpandedInFieldsGroup"),
#endif
		Category("Data"), DefaultValue(true), NotifyParentProperty(true),
		XtraSerializableProperty(), XtraSerializablePropertyId(LayoutIdLayout),
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.ExpandedInFieldsGroup"),
		TypeConverter(typeof(BooleanTypeConverter))
		]
		public bool ExpandedInFieldsGroup {
			get { return expandedInFieldsGroup; }
			set {
				if(expandedInFieldsGroup == value) return;
				expandedInFieldsGroup = value;
				OnExpandedInFieldsGroupChanged();
			}
		}
		public void SetSortBySummary(PivotGridFieldBase dataField, List<PivotGridFieldSortCondition> conditions,
							PivotSummaryType? customTotalSummaryType, bool sort) {
			SortBySummaryInfo.Reset();
			if(sort) {
				SortBySummaryInfo.Field = dataField;
				SortBySummaryInfo.CustomTotalSummaryType = customTotalSummaryType;
				SortBySummaryInfo.Conditions.AddRange(conditions);
				if(Data != null)
					SortOrder = PivotEnumExtensionsBase.ToPivotSortOrder(Data.OptionsBehavior.SortBySummaryDefaultOrder, SortOrder);
			}
		}
		protected internal void SetSorting(PivotSortOrder sortOrder, PivotSortMode? actualSortMode,
				PivotSortMode? sortMode, bool reset) {
			if(reset)
				SortBySummaryInfo.Reset();
			SortOrder = sortOrder;
			if(sortMode.HasValue)
				SortMode = sortMode.Value;
			if(actualSortMode.HasValue)
				ActualSortMode = actualSortMode.Value;
		}
		void IXtraSerializableLayoutEx.ResetProperties(OptionsLayoutBase options) {
			OnResetSerializationProperties(options);
		}
		bool IXtraSerializableLayoutEx.AllowProperty(OptionsLayoutBase options, string propertyName, int id) {
			return OnAllowSerializationProperty(options, propertyName, id);
		}
		protected virtual bool OnAllowSerializationProperty(OptionsLayoutBase options, string propertyName, int id) {
			PivotGridOptionsLayout opts = options as PivotGridOptionsLayout;
#if !SL
			if(opts == null) return true;
			if(opts.StoreAllOptions || opts.Columns.StoreAllOptions) return true;
			switch(id) {
				case LayoutIdAppearance:
					return opts.Columns.StoreAppearance;
				case LayoutIdLayout:
					return opts.Columns.StoreLayout;
				case LayoutIdData:
					return opts.StoreDataSettings;
				case LayoutIdOptionsView:
					return opts.StoreVisualOptions;
			}
#endif
			return true;
		}
		protected virtual void OnResetSerializationProperties(OptionsLayoutBase options) {
#if !SL
			PivotGridOptionsLayout opts = options as PivotGridOptionsLayout;
			if(opts != null && opts.Columns.StoreAllOptions) opts = null;
			if(opts == null || opts.Columns.StoreAppearance) {
				Caption = "";
				DisplayFolder = "";
				EmptyCellText = "";
				EmptyValueText = "";
				GrandTotalText = "";
				Width = 100;
				MinWidth = DefaultMinWidth;
			}
			if(opts == null || opts.Columns.StoreLayout) {
				Area = PivotArea.FilterArea;
				AreaIndex = -1;
				Visible = true;
				ExpandedInFieldsGroup = true;
				RowValueLineCount = 1;
				ColumnValueLineCount = 1;
			}
			if(opts == null || opts.StoreDataSettings) {
				SummaryType = PivotSummaryType.Sum;
				SummaryDisplayType = PivotSummaryDisplayType.Default;
				SortOrder = PivotSortOrder.Ascending;
				SortMode = PivotSortMode.Default;
				actualSortMode = null;
				SortBySummaryInfo.Reset();
				GroupInterval = PivotGroupInterval.Default;
				GroupIntervalNumericRange = 10;
				TopValueCount = 0;
				TopValueType = PivotTopValueType.Absolute;
				TopValueShowOthers = false;
			}
			if(opts == null || opts.Columns.StoreAllOptions) {
				AllowedAreas = PivotGridAllowedAreas.All;
				Options.Reset();
				TotalsVisibility = PivotTotalsVisibility.AutomaticTotals;
				UnboundType = UnboundColumnType.Bound;
				UnboundExpression = "";
				CellFormat.Reset();
				TotalCellFormat.Reset();
				GrandTotalCellFormat.Reset();
				ValueFormat.Reset();
			}
#endif
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsNextVisibleFieldInSameGroup {
			get {
				if(Group == null || !ExpandedInFieldsGroup) return false;
				int index = Group.IndexOf(this);
				return index < Group.Count - 1;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsUnbound {
			get { return GroupInterval != PivotGroupInterval.Default || UnboundType != UnboundColumnType.Bound; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsComplex {
			get {
				if(UnboundType != UnboundColumnType.Bound || IsOLAPField)
					return false;
				return !string.IsNullOrEmpty(FieldName) && (FieldName.Contains(".") || FieldName.Contains("!"));
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string DataControllerColumnName {
			get {
				if(Data != null)
					return Data.GetDataControllerColumnName(this);
				return !IsUnbound || (IsUnbound && string.IsNullOrEmpty(UnboundFieldName)) || IsOLAPField ? FieldName : UnboundFieldName;
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBasePrefilterColumnName"),
#endif
		DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.PrefilterColumnName"),
		]
		public string PrefilterColumnName {
			get { return !string.IsNullOrEmpty(Name) ? Name : DataControllerColumnName; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		internal string UniqueName {
			get {
				if(!string.IsNullOrEmpty(Name))
					return Name;
				if(Area == PivotArea.DataArea)
					return DataControllerColumnName + SummaryNameSuffix;
				else
					return DataControllerColumnName;
			}
		}
		string SummaryNameSuffix { get { return "_" + SummaryType.ToString(); } }
		[Browsable(false)]
		public string ExpressionFieldName {
			get {
				if(IsProcessOnSummaryLevel) {
					if(Data != null && !Data.OptionsData.IsProcessExpressionOnSummaryLevel && Area == PivotArea.DataArea && UnboundType == UnboundColumnType.Bound)
						return DataControllerColumnName;
				} else {
					if(Data != null && !(Data.OptionsData.IsProcessExpressionOnSummaryLevel && Area != PivotArea.DataArea))
						return DataControllerColumnName;
				}
				return UniqueName;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public string SummaryColumnName {
			get {
				string summaryColumnName = Data.GetFieldName(this);
				if(Area == PivotArea.DataArea)
					summaryColumnName += SummaryNameSuffix;
				return summaryColumnName;
			}
		}
		[
		Browsable(false), 
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseOLAPDrillDownColumnName")
#else
	Description("")
#endif
		]
		public string OLAPDrillDownColumnName {
			get {
				return Data != null ? Data.GetOLAPDrillDownColumnName(FieldName) : null;
			}
		}
		string drillDownColumnName;
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public string DrillDownColumnName { 
			get { return drillDownColumnName ?? OLAPDrillDownColumnName; }
			set { drillDownColumnName = value; }
		}
		[
		Browsable(false), 
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseKPIType")
#else
	Description("")
#endif
		]
		public PivotKPIType KPIType {
			get {
				if(Data == null) return PivotKPIType.None;
				return Data.GetKPIType(this);
			}
		}
		[
#if !SL
	DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldBaseKPIGraphic"),
#endif
 Category("KPI"),
		DefaultValue(PivotKPIGraphic.ServerDefined), XtraSerializableProperty(), NotifyParentProperty(true)]
		[DXDisplayName(typeof(DevExpress.XtraPivotGrid.ResFinder), DXDisplayNameAttribute.DefaultResourceFile, "DevExpress.XtraPivotGrid.PivotGridFieldBase.KPIGraphic")]
		public PivotKPIGraphic KPIGraphic {
			get { return kpiGraphic; }
			set {
				if(kpiGraphic == value) return;
				kpiGraphic = value;
				OnKPIGraphicChanged();
			}
		}
		public object[] GetUniqueValues() {
			if(Data == null) return new object[0];
			return Data.GetUniqueFieldValues(this);
		}
		public object[] GetAvailableValues() {
			if(Data == null) return new object[0];
			return Data.GetAvailableFieldValues(this);
		}
		public void GetAvailableValuesAsync(AsyncCompletedHandler asyncCompleted) {
			if(Data == null) {
				asyncCompleted(AsyncOperationResult.Create(new object[0], null));
				return;
			}
			PivotGridDataAsync dataAsync = GetDataAsync();
			dataAsync.GetAvailableFieldValuesAsync(this, true, asyncCompleted, false);
		}
		public List<object> GetVisibleValues() {
			if(Data == null) return new List<object>();
			return Data.GetVisibleFieldValues(this);
		}
		PivotGridDataAsync GetDataAsync() {
			PivotGridDataAsync dataAsync = Data as PivotGridDataAsync;
			if(dataAsync == null)
				throw new InvalidOperationException("Cannot call an asynchronous method from the host application.");
			return dataAsync;
		}
		public PivotSummaryInterval GetSummaryInterval() {
			return GetSummaryInterval(false);
		}
		public PivotSummaryInterval GetSummaryInterval(PivotGridFieldBase rowField, PivotGridFieldBase columnField) {
			return GetSummaryInterval(false, rowField, columnField);
		}
		protected internal PivotSummaryInterval GetSummaryInterval(bool visibleValuesOnly) {
			if(Data == null) return PivotSummaryInterval.Empty;
			return Data.GetSummaryInterval(this, visibleValuesOnly, false, null, null);
		}
		protected internal PivotSummaryInterval GetSummaryInterval(bool visibleValuesOnly, PivotGridFieldBase rowField,
				PivotGridFieldBase columnField) {
			if(Data == null || Area != PivotArea.DataArea) return PivotSummaryInterval.Empty;
			return Data.GetSummaryInterval(this, visibleValuesOnly, true, rowField, columnField);
		}
		[Browsable(false)]
		public bool IsDesignTime { get { return DesignMode; } }
		public IOLAPMember[] GetOLAPMembers() {
			if(Data == null) return new IOLAPMember[0];
			return Data.GetOLAPColumnMembers(FieldName);
		}
		List<DevExpress.PivotGrid.OLAP.OLAPPropertyDescriptor> olapMemberProperties = null;
		public List<DevExpress.PivotGrid.OLAP.OLAPPropertyDescriptor> GetOLAPMemberProperties() {
			if(olapMemberProperties != null || Data == null)
				return olapMemberProperties;
			Dictionary<string, DevExpress.PivotGrid.OLAP.OLAPDataType> types = Data.GetOlapMemberProperties(FieldName);
			if(types == null)
				return null;
			olapMemberProperties = new List<PivotGrid.OLAP.OLAPPropertyDescriptor>();
			foreach(KeyValuePair<string, DevExpress.PivotGrid.OLAP.OLAPDataType> pair in types)
				olapMemberProperties.Add(new DevExpress.PivotGrid.OLAP.OLAPPropertyDescriptor(pair.Key, FieldName, DevExpress.PivotGrid.OLAP.OLAPDataTypeConverter.Convert(pair.Value)));
			return olapMemberProperties;
		}
		public string GetOlapDefaultSortProperty() {
			if(Data == null || string.IsNullOrEmpty(FieldName))
				return null;
			return Data.GetOlapDefaultSortProperty(FieldName);
		}
		public void GetOLAPMembersAsync(AsyncCompletedHandler asyncCompleted) {
			if(Data == null) {
				asyncCompleted(AsyncOperationResult.Create(new object[0], null));
				return;
			}
			PivotGridDataAsync dataAsync = GetDataAsync();
			dataAsync.GetOLAPColumnMembersAsync(FieldName, asyncCompleted, true);
		}
#if !SL
		[Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public bool SelectedAtDesignTime {
			get {
				ISelectionService ss = GetSelectionService();
				return ss != null ? ss.GetComponentSelected(this) : false;
			}
			set {
#if !DXPORTABLE
				ISelectionService ss = GetSelectionService();
				if(ss != null) {
					ss.SetSelectedComponents(new object[] { this }, ControlConstants.SelectionClick);
					if(Data != null) 
						Data.OnFieldSelectedAtDesignTimeChanged();
				}
#endif
			}
		}
#endif
			public override string ToString() {
			if(caption != string.Empty)
				return caption;
			string displayText = GetCaptionCore(true);
			if(!string.IsNullOrEmpty(displayText))
				return displayText;
			if(FieldName != string.Empty)
				return FieldName;
			if(Site != null)
				return Site.Name;
			return string.Empty;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanFilter {
			get {
				if(InnerGroupIndex > 0 && Group.IsFilterAllowed && Area != PivotArea.DataArea)
					return false;
				if(Data == null || (Area == PivotArea.DataArea && Visible && !CanFilterBySummary)) return false;
				if(Data != null && Options.AllowFilter == DefaultBoolean.Default)
					return Data.OptionsCustomization.AllowFilter;
				return Options.AllowFilter == DefaultBoolean.True;
			}
		}
		bool CanFilterBySummaryInternal {
			get {
				if(Data.IsServerMode)
					return false;
				if(Options.AllowFilterBySummary == DefaultBoolean.Default)
					return Data.OptionsCustomization.AllowFilterBySummary;
				return Options.AllowFilterBySummary == DefaultBoolean.True;
			}
		}
		protected internal virtual bool CanFilterBySummary {
			get { return CanFilterBySummaryInternal && CanApplySummaryFilter; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual PivotGroupFilterMode GroupFilterMode {
			get {
				if(Data != null && !Options.GroupFilterMode.HasValue)
					return Data.OptionsFilter.GroupFilterMode;
				return Options.GroupFilterMode.HasValue ? Options.GroupFilterMode.Value : PivotGroupFilterMode.Tree;
			}
		}
		internal bool CanApplyFilter {
			get { return Visible || (Area != PivotArea.DataArea && !Data.OptionsData.FilterByVisibleFieldsOnly); }
		}
		internal bool NeedApplyFilterOnShowInFilterArea {
			get {
				return Data.OptionsData.FilterByVisibleFieldsOnly && (
				((Group == null || GroupFilterMode == PivotGroupFilterMode.List) && !FilterValues.IsEmpty) ||
				(GroupFilterMode == PivotGroupFilterMode.Tree && Group != null && !Group.FilterValues.IsEmpty));
			}
		}
		internal bool CanApplySummaryFilter {
			get {
				if(Data != null)
					return Data.GetAllowFilterBySummary(this);
				return false;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanDrag {
			get {
				if(IsDesignTime) return true;
				if(Data != null && Options.AllowDrag == DefaultBoolean.Default)
					return Data.OptionsCustomization.AllowDrag;
				return Options.AllowDrag == DefaultBoolean.True ? true : false;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanDragInCustomizationForm {
			get {
				if(IsDesignTime) return true;
				if(Data != null && Options.AllowDragInCustomizationForm == DefaultBoolean.Default)
					return Data.OptionsCustomization.AllowDragInCustomizationForm;
				return Options.AllowDragInCustomizationForm == DefaultBoolean.True ? true : false;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanHide {
			get {
				if(IsDataField || InnerGroupIndex > 0 || Data == null) return false;
				return Data.AllowHideFields;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool CanSort {
			get {
				if(Area == PivotArea.FilterArea || Area == PivotArea.DataArea || !Visible) return false;
				if(IsOLAPNotSorted) return false;
				return CanSortCore;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool CanSortCore {
			get {
				if(Data != null && Options.AllowSort == DefaultBoolean.Default)
					return Data.OptionsCustomization.AllowSort;
				return Options.AllowSort == DefaultBoolean.True ? true : false;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanSortBySummary {
			get {
				if(IsDesignTime || !IsColumnOrRow) return false;
				if(Data != null && Options.AllowSortBySummary == DefaultBoolean.Default)
					return Data.OptionsCustomization.AllowSortBySummary;
				return Options.AllowSortBySummary == DefaultBoolean.True ? true : false;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool CanHideEmptyVariationItems {
			get {
				return PivotSummaryDisplayTypeConverter.IsVariation(Data, this) && Options.HideEmptyVariationItems;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool ShowSortButton {
			get { return CanSort || !SortBySummaryInfo.IsEmpty; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool ShowFilterButton {
			get { return CanFilter && CanApplyFilter; }
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual bool ShowActiveFilterButton {
			get {
				if(Area == PivotArea.DataArea) {
					if(!Visible)
						return false;
					return !SummaryFilter.IsEmpty;
				}
				if(GroupFilterMode == PivotGroupFilterMode.Tree && HasGroup) {
					return Group.FilterValues.HasFilter;
				}
				return FilterValues.HasFilter;
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsDataField { get { return dataFieldData != null; } }
		bool isRowTreeField;
		[
		Browsable(false), EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public bool IsRowTreeField {
			get { return isRowTreeField; }
			internal set { isRowTreeField = value; }
		}
		DefaultBoolean isAggregatable = DefaultBoolean.Default;
		internal bool IsAggregatable {
			get {
				if(!Data.IsOLAP || !IsColumnOrRow || !Visible)
					return true;
				if(isAggregatable == DefaultBoolean.Default)
					isAggregatable = Data.GetFieldIsAggregatable(FieldName);
				PivotGridFieldBase nextField = Data.GetFieldByArea(Area, AreaIndex + 1);
				return isAggregatable == DefaultBoolean.True && (nextField == null || nextField.IsAggregatable);
			}
		}
		public bool CanShowValueType(PivotGridValueType valueType) {
			if(Area != PivotArea.DataArea) return true;
			return (valueType == PivotGridValueType.GrandTotal && Options.ShowGrandTotal
						&& !PivotSummaryDisplayTypeConverter.IsVariation(Data, this))
				|| (valueType == PivotGridValueType.Total && Options.ShowTotals)
				|| (valueType == PivotGridValueType.CustomTotal && Options.ShowCustomTotals)
				|| (valueType == PivotGridValueType.Value && Options.ShowValues);
		}
		PivotGridFieldPropertyDescriptor propertyDescriptor;
		[EditorBrowsable(EditorBrowsableState.Never), Browsable(false)]
		public PivotGridFieldPropertyDescriptor PropertyDescriptor {
			get {
				if(propertyDescriptor == null)
					propertyDescriptor = new PivotGridFieldPropertyDescriptor(this);
				return propertyDescriptor;
			}
		}
#if !SL
		protected virtual ISelectionService GetSelectionService() {
			if(!DesignMode) return null;
			return GetService(typeof(ISelectionService)) as ISelectionService;
		}
#endif
		protected virtual PivotGridCustomTotalCollectionBase CreateCustomTotals() {
			return new PivotGridCustomTotalCollectionBase(this);
		}
		protected virtual PivotGridFieldFilterValues CreateFilterValues() {
			return new PivotGridFieldFilterValues(this);
		}
		protected internal virtual void DoLayoutChanged() {
			OnPropertyChanged();
			if(Data != null) Data.LayoutChanged();
		}
		protected virtual void DoRefresh() {
			if(Data != null) Data.DoRefresh();
		}
		protected virtual void DoReloadData() {
			if(Data != null) Data.ReloadData();
		}
		protected void GroupFieldsByHierarchies() {
			if(Collection != null)
				Collection.GroupFieldsByHierarchies();
		}
		protected virtual void OnAreaChanged(bool doRefresh) {
			OnPropertyChanged();
			if(HasGroup)
				Group.OnChanged();
			if(IsLockUpdate) return;
			if(Visible && doRefresh)
				DoRefresh();
			if(Data != null) Data.OnFieldAreaChanged(this);
		}
		protected virtual void OnAreaIndexChanged(bool doRefresh, bool doLayoutChanged) {
			OnPropertyChanged();
			if(IsLockUpdate) return;
			if(Data != null) Data.OnFieldAreaIndexChanged(this, doRefresh, doLayoutChanged);
		}
		protected virtual void OnVisibleChanged(int oldAreaIndex, bool doRefresh) {
			OnPropertyChanged();
			if(Data != null) Data.OnFieldVisibleChanged(this, oldAreaIndex, doRefresh);
		}
		protected virtual void OnFieldNameChanged() {
			OnPropertyChanged();
			if(Visible) {
				if(IsComplex)
					DoReloadData();
				else
					DoRefresh();
			}
			PivotGridGroup group = Group;
			if(group != null && group.IsOLAP && !group.IsFieldValid(this)) {
				group.Remove(this);
				if(group.Count == 0 && Data != null) Data.Groups.Remove(group); 
			}
			GroupFieldsByHierarchies();
			if(Data != null)
				Data.OnFieldPropertyChanged(this, PivotFieldPropertyName.FieldName);
		}
		protected virtual void OnUnboundFieldNameChanged() {
			OnPropertyChanged();
			if(!Visible) return;
			if(IsUnbound && Data != null && !(Data.IsLockUpdate && Data.IsServerMode) ) DoReloadData();
			else DoRefresh();
		}
		protected internal virtual void OnSortBySummaryInfoChanged() {
			OnPropertyChanged();
			if(IsColumnOrRow && Visible)
				DoRefresh();
			if(Data != null)
				Data.OnFieldPropertyChanged(this, PivotFieldPropertyName.SortBySummaryInfo);
		}
		protected internal virtual void OnSummaryFilterChanged() {
			OnPropertyChanged();
			if(Visible && Area == PivotArea.DataArea)
				DoRefresh();
		}
		protected virtual void OnSummaryChanged(PivotSummaryType oldSummaryType) {
			OnPropertyChanged();
			if(Data == null)
				return;
			Data.OnSummaryTypeChanged(this, oldSummaryType);
			Data.OnFieldPropertyChanged(this, PivotFieldPropertyName.SummaryType);
		}
		protected virtual void OnTopValuesChanged() {
			OnPropertyChanged();
			if(Visible && IsColumnOrRow)
				DoRefresh();
		}
		protected virtual void OnCaptionChanged() {
			OnPropertyChanged();
			if(Visible)
				DoLayoutChanged();
			else if(Data != null && !Data.IsDeserializing && !Data.IsLockUpdate && !Data.IsLoading) {
				DevExpress.XtraPivotGrid.Customization.CustomizationFormFields fields = Data.GetCustomizationFormFields();
				if(fields != null)
					fields.GetFieldsFromData();
			}
		}
		protected virtual void OnDisplayFolderChanged() {
			DoLayoutChanged();
		}
		protected virtual void OnShowSummaryTypeNameChanged() {
		}
		protected virtual void OnKPIGraphicChanged() {
			OnPropertyChanged();
			if(Visible) DoLayoutChanged();
		}
		protected virtual void OnEmptyCellTextChanged() {
			OnPropertyChanged();
			if(Visible && Area == PivotArea.DataArea)
				DoLayoutChanged();
		}
		protected virtual void OnEmptyValueTextChanged() {
			OnPropertyChanged();
			if(Visible && IsColumnOrRow)
				DoLayoutChanged();
		}
		protected virtual void OnWidthChanged() {
			OnPropertyChanged();
			if(Visible && Data != null)
				Data.OnFieldSizeChanged(this, true, false);
		}
		protected void OnPropertyChanged() {
			OnFieldCollectionChanged();
		}
		protected internal virtual bool OnFilteredValueChanging(PivotFilterType filterType, bool showBlanks, IList<object> values) {
			return Data != null ? Data.OnFieldFilteringChanging(this, filterType, showBlanks, values) : false;
		}
		protected internal virtual void OnFilteredValueChanged() {
			OnPropertyChanged();
		}
		protected virtual void OnSortOrderChanged() {
			OnPropertyChanged();
			if(Data == null) return;
			if(Visible && IsColumnOrRow) Data.OnSortOrderChanged(this);
			Data.OnFieldPropertyChanged(this, PivotFieldPropertyName.SortOrder);
		}
		protected virtual void OnSortModeChanged() {
			OnPropertyChanged();
			if(Visible && IsColumnOrRow && Data != null) Data.OnSortModeChanged(this);
		}
		protected virtual void OnTotalSummaryChanged() {
			OnPropertyChanged();
			if(Visible && IsColumnOrRow) DoRefresh();
		}
		protected virtual void OnUnboundChanged() {
			OnPropertyChanged();
			if(!Visible) return;
			if(UnboundType == UnboundColumnType.Bound || Data != null && Data.IsServerMode)
				DoRefresh();
			else DoReloadData();
		}
		protected virtual void OnUnboundExpressionChanged() {
			OnPropertyChanged();
			expressionOperator = null;
			mixedCriteriaVisitor = null;
			expressionColumnNames = null;
			OnUnboundChanged();
			if(Data != null)
				Data.OnFieldUnboundExpressionChanged(this);
		}
		protected virtual void OnGroupIntervalChanged() {
			OnPropertyChanged();
			if(!Visible) return;
			if(GroupInterval == PivotGroupInterval.Default || Data != null && Data.IsServerMode)
				DoRefresh();
			else DoReloadData();
		}
		protected virtual void OnGroupIntervalNumericRangeChanged() {
			OnPropertyChanged();
			if(IsGroupIntervalNumeric)
				if(Data != null && Data.IsServerMode)
					DoRefresh();
				else
					DoReloadData();
		}
		protected internal virtual void OnCustomTotalChanged() {
			OnPropertyChanged();
			if(IsColumnOrRow && Visible && GetTotalsVisibility() == PivotTotalsVisibility.CustomTotals)
				DoLayoutChanged();
		}
		protected internal virtual void OnExpandedInFieldsGroupChanged() {
			OnPropertyChanged();
			if(HasGroup && Visible) {
				DoRefresh();
				Data.OnFieldExpandedInFieldsGroupChanged(this);
			}
		}
		protected virtual void OnColumnRowValueCountChanged() {
			OnPropertyChanged();
			if((!Visible && !IsDataField) || Area == PivotArea.FilterArea) return;
			DoLayoutChanged();
		}
		protected virtual void OnOptionsChanged(object sender, PivotOptionsChangedEventArgs e) {
			OnPropertyChanged();
			switch(e.Option) {
				case FieldOptions.GroupFilterMode:
					DoRefresh();
					return;
				case FieldOptions.OLAPFilterByUniqueName:
					if(Visible && IsColumnOrRow)
						DoRefresh();
					return;
				case FieldOptions.OLAPUseNonEmpty:
					if(Visible && Area == PivotArea.DataArea)
						DoRefresh();
					return;
				case FieldOptions.ShowInCustomizationForm:
					DoLayoutChanged();
					return;
				case FieldOptions.ShowSummaryTypeName:
					OnShowSummaryTypeNameChanged();
					break;
			}
			if(Visible) DoLayoutChanged();
		}
		protected internal virtual void SetCollection(PivotGridFieldCollectionBase collection) {
			this.collection = collection;
			EnsureCaseSensitive();
		}
		internal void EnsureCaseSensitive() {
			if(collection != null && collection.Data != null && collection.Data.PivotDataSource != null)
				filterValues.EnsureCaseSensitive(collection.Data.CaseSensitive);
		}
		void OnFormatChanged(object sender, EventArgs e) {
			OnPropertyChanged();
			if(Visible && Area == PivotArea.DataArea)
				DoLayoutChanged();
		}
		T IViewBagOwner.GetViewBagProperty<T>(string objectName, string propertyName, T value) {
			return GetViewBagPropertyCore(objectName, propertyName, value);
		}
		void IViewBagOwner.SetViewBagProperty<T>(string objectName, string propertyName, T defaultValue, T value) {
			SetViewBagPropertyCore(objectName, propertyName, defaultValue, value);
		}
		protected virtual T GetViewBagPropertyCore<T>(string objectName, string propertyName, T value) {
			return value;
		}
		protected virtual void SetViewBagPropertyCore<T>(string objectName, string propertyName, T defaultValue, T value) {
		}
		#region IXtraSerializable Members
		bool deserializing;
		bool serializing;
		protected bool IsDeserializing { get { return deserializing; } }
		protected bool IsSerializing { get { return serializing; } }
		public void OnEndDeserializing(string restoredVersion) {
			deserializing = false;
		}
		public void OnEndSerializing() {
			serializing = false;
		}
		public void OnStartDeserializing(LayoutAllowEventArgs e) {
			e.Allow = true;
			deserializing = true;
		}
		public void OnStartSerializing() {
			serializing = true;
		}
		#endregion
		#region IDataColumnInfo Members
		DataControllerBase IDataColumnInfo.Controller { get { return null; } }
		string IDataColumnInfo.Caption { get { return ToString(); } }
		List<IDataColumnInfo> IDataColumnInfo.Columns {
			get {
				List<IDataColumnInfo> res = new List<IDataColumnInfo>();
				if(Data == null)
					return res;
				bool isOnSummary = IsProcessOnSummaryLevel;
				foreach(PivotGridFieldBase field in Data.Fields) {
					if(field == this || !field.Options.ShowInExpressionEditor)
						continue;
					if(Area == PivotArea.DataArea) {
						if(UnboundExpressionMode != UnboundExpressionMode.UseAggregateFunctions || !isOnSummary) {
							if(isOnSummary && field.Area != PivotArea.DataArea)
								continue;
							if(!isOnSummary && field.IsUnbound && field.IsProcessOnSummaryLevel)
								continue;
						}
					} else {
						if(field.IsUnbound && field.IsProcessOnSummaryLevel)
							continue;
					}
					res.Add(field);
				}
				return res;
			}
		}
		string IDataColumnInfo.FieldName {
			get { return ExpressionFieldName; }
		}
		Type IDataColumnInfo.FieldType {
			get { return DataType != null ? DataType : typeof(object); }
		}
		string IDataColumnInfo.Name {
			get { return Name; }
		}
		string IDataColumnInfo.UnboundExpression {
			get { return UnboundExpression; }
		}
		#endregion
		protected bool HasDefereFilter {
			get {
				if(Group != null && Group.IsFilterAllowed)
					return Group.FilterValues != null && Group.FilterValues.GetDefereFilter() != null;
				return FilterValues != null && FilterValues.GetDefereFilter() != null;
			}
		}
		internal bool IsProcessOnSummaryLevel {
			get {
				if(Area != PivotArea.DataArea)
					return false;
				else
					if(UnboundType == UnboundColumnType.Bound)
						return true;
				if(UnboundExpressionMode != UnboundExpressionMode.Default)
					return UnboundExpressionMode != UnboundExpressionMode.DataSource;
				else
					return Data == null || Data.OptionsData.IsProcessExpressionOnSummaryLevel;
			}
		}
		object IXtraSupportDeserializeCollectionItem.CreateCollectionItem(string propertyName, XtraItemEventArgs e) {
			if(propertyName == "CustomTotals") {
				PivotGridCustomTotalBase ct = CustomTotals.CreateCustomTotal();
				CustomTotals.Add(ct);
				return ct;
			} else
				throw new ArgumentException("propertyName");
		}
		void IXtraSupportDeserializeCollectionItem.SetIndexCollectionItem(string propertyName, XtraSetItemIndexEventArgs e) {
		}
		void IXtraSupportDeserializeCollection.AfterDeserializeCollection(string propertyName, XtraItemEventArgs e) {
		}
		void IXtraSupportDeserializeCollection.BeforeDeserializeCollection(string propertyName, XtraItemEventArgs e) {
		}
		bool IXtraSupportDeserializeCollection.ClearCollection(string propertyName, XtraItemEventArgs e) {
			if(propertyName == "CustomTotals") {
				CustomTotals.Clear();
				return true;
			} else
				throw new ArgumentException("propertyName");
		}
	}
	public enum PivotFieldPropertyName { SortOrder, FieldName, SummaryType, SortBySummaryInfo }
	[ListBindable(false)]
	public class PivotGridFieldCollectionBase : CollectionBase {
		internal const string DefaultNamePrefix = "field";
		PivotGridData data;
		NullableDictionary<string, List<PivotGridFieldBase>> names;
		public PivotGridFieldCollectionBase(PivotGridData data) {
			this.data = data;
			this.names = new NullableDictionary<string, List<PivotGridFieldBase>>();
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridFieldBase this[int index] { get { return InnerList[index] as PivotGridFieldBase; } }
		[EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridFieldBase this[string fieldName] {
			get {
				for(int i = 0; i < Count; i++) {
					if(this[i].FieldName == fieldName)
						return this[i];
				}
				for(int i = 0; i < Count; i++) {
					if(this[i].ExpressionFieldName == fieldName)
						return this[i];
					if(this[i].DataControllerColumnName == fieldName)
						return this[i];
				}
				return null;
			}
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public IList GetPrefilterFields(IList<string> prefilterColumnNames) {
			List<PivotGridFieldBase> fields = new List<PivotGridFieldBase>();
			foreach(PivotGridFieldBase field in this) {
				if(field.CanShowInPrefilter || prefilterColumnNames.Contains(field.PrefilterColumnName))
					fields.Add(field);
			}
			return fields;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public bool IsPrefilterHiddenField(string name) {
			foreach(PivotGridFieldBase field in this) {
				if(!field.CanShowInPrefilter && field.IsValidForPrefilter && string.Equals(name, field.PrefilterColumnName))
					return true;
			}
			return false;
		}
		internal PivotGridFieldBase GetFieldByFieldName(string fieldName) {
			foreach(PivotGridFieldBase field in this)
				if(field.FieldName == fieldName)
					return field;
			return null;
		}
		public PivotGridFieldBase GetFieldByName(string name) {
			List<PivotGridFieldBase> res;
			if(!names.TryGetValue(name, out res))
				return null;
			return res.Count > 0 ? res[0] : null;
		}
		protected internal PivotGridFieldBase GetFieldByNameOrDataControllerColumnName(string name) {
			PivotGridFieldBase fieldByName = GetFieldByName(name);
			if(fieldByName != null) return fieldByName;
			foreach(PivotGridFieldBase field in this) {
				if(field.DataControllerColumnName == name) return field;
			}
			return null;
		}
		protected internal int GetSameFieldNameCount(PivotGridFieldBase field) {
			string name = field.FieldName;
			int count = 0;
			for(int i = 0; i < Count; i++) {
				if(this[i].FieldName == name)
					count++;
			}
			return count;
		}
		public PivotGridFieldBase Add() {
			return Add(string.Empty, PivotArea.RowArea);
		}
		public void Add(PivotGridFieldBase field) {
			AddCore(field);
		}
		public PivotGridFieldBase Add(string fieldName, PivotArea area) {
			PivotGridFieldBase field = CreateField(fieldName, area);
			AddCore(field);
			return field;
		}
		public void Remove(PivotGridFieldBase field) {
			if(InnerList.Contains(field)) {
				List.Remove(field);
				names.Remove(field.Name);
			}
		}
		protected virtual PivotGridFieldBase CreateField(string fieldName, PivotArea area) {
			return new PivotGridFieldBase(fieldName, area);
		}
		protected virtual void AddCore(PivotGridFieldBase field) {
			Data.CheckField(field);
			field.FixOldAreaIndex();
			List.Add(field);
		}
		protected void InsertCore(int index, PivotGridFieldBase field) {
			Data.CheckField(field);
			List.Insert(index, field);
		}
		public bool Contains(PivotGridFieldBase field) { return InnerList.Contains(field); }
		public int IndexOf(PivotGridFieldBase field) { return InnerList.IndexOf(field); }
		protected List<PivotGridFieldBase> GetFieldsByHierarchy(string hierarchy) {
			if(string.IsNullOrEmpty(hierarchy)) return new List<PivotGridFieldBase>();
			List<PivotGridFieldBase> result = new List<PivotGridFieldBase>();
			for(int i = 0; i < Count; i++)
				if(this[i].Hierarchy == hierarchy)
					result.Add(this[i]);
			return result;
		}
		public int GetVisibleFieldCount(PivotArea area) {
			int count = 0;
			for(int i = 0; i < Count; i++) {
				if(this[i].Visible && this[i].Area == area)
					count++;
			}
			return count;
		}
		public void UpdateAreaIndexes() {
			UpdateAreaIndexes(null);
		}
		protected internal void UpdateAreaIndexes(PivotGridFieldBase ignoreField) {
			UpdateAreaIndexes(ignoreField, false);
		}
		bool lockUpdateAreaIndexes = false;
		protected internal void LockUpdateAreaIndexes() {
			lockUpdateAreaIndexes = true;
		}
		protected internal void UnlockUpdateAreaIndexes() {
			lockUpdateAreaIndexes = false;
			UpdateAreaIndexes(null, false);
		}
		protected internal void UpdateAreaIndexes(PivotGridFieldBase ignoreField, bool forcedLoading) {
			if(Data == null || (Data.IsLoading && !forcedLoading ) || Data.IsDeserializing || Data.Disposing || lockUpdateAreaIndexes && ignoreField == null) return;
			int areaCount = Helpers.GetEnumValues(typeof(PivotArea)).Length;
			for(int i = 0; i < areaCount; i++) {
				PivotArea area = (PivotArea)i;
				UpdateAreaIndexes(area, ignoreField != null && ignoreField.Area == area ? ignoreField : null);
			}
		}
		public void SetFieldIndex(PivotGridFieldBase field, int newIndex) {
			if(field == null || IndexOf(field) < 0) return;
			if(newIndex < 0 || newIndex > Count || newIndex == field.Index) return;
			int deletedIndex = field.Index;
			if(newIndex < deletedIndex)
				deletedIndex++;
			if(field.Index < newIndex)
				newIndex++;
			InternalMove(field, deletedIndex, newIndex);
		}
		void InternalMove(PivotGridFieldBase field, int oldIndex, int newIndex) {
			int oldAreaIndex = field.AreaIndex;
			if(newIndex < 0) newIndex = 0;
			if(newIndex < Count)
				InnerList.Insert(newIndex, field);
			else InnerList.Add(field);
			InnerList.RemoveAt(oldIndex);
		}
		protected virtual void UpdateAreaIndexes(PivotArea area, PivotGridFieldBase ignoreField) {
			List<PivotGridFieldBase> fields = GetFieldsByArea(area, true, false);
			bool dataFieldPresent = fields.Contains(Data.DataField);
			if(ignoreField != null)
				fields.Remove(ignoreField);
			fields.Sort(new PivotGridFieldAreaIndexComparer());
			if(dataFieldPresent && ignoreField == null) {
				Data.OptionsDataField.AreaIndexCore = fields.IndexOf(Data.DataField);
			}
			fields.Remove(Data.DataField);
			if(ignoreField != null) {
				int ignoreFieldIndex = ignoreField.AreaIndex;
				if(ignoreFieldIndex < 0)
					ignoreFieldIndex = 0;
				if(ignoreFieldIndex > fields.Count)
					ignoreFieldIndex = fields.Count;
				fields.Insert(ignoreFieldIndex, ignoreField);
			}
			for(int i = 0; i < fields.Count; i++) {
				PivotGridFieldBase field = fields[i];
				field.AreaIndexCore = i;
				field.AreaIndexOldCore = i;
			}
		}
		internal List<PivotGridFieldBase> GetFieldsByArea(PivotArea area, bool includeDataField) {
			return GetFieldsByArea(area, includeDataField, true);
		}
		internal List<PivotGridFieldBase> GetFieldsByArea(PivotArea area, bool includeDataField, bool sort) {
			List<PivotGridFieldBase> fields = new List<PivotGridFieldBase>();
			for(int i = 0; i < Count; i++)
				if(this[i].Area == area && this[i].Visible)
					fields.Add(this[i]);
			PivotGridFieldBase dataField = Data.DataField;
			if(includeDataField && dataField.Visible && area == dataField.Area && 0 <= dataField.AreaIndex && dataField.AreaIndex <= fields.Count)
				fields.Insert(dataField.AreaIndex, Data.DataField);
			if(sort) fields.Sort(new PivotGridFieldAreaIndexComparer());
			return fields;
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public PivotGridData Data { get { return data; } }
#if !SL
	[DevExpressPivotGridCoreLocalizedDescription("PivotGridFieldCollectionBaseDefaultFieldWidth")]
#endif
		public int DefaultFieldWidth { get { return PivotGridFieldBase.DefaultWidth; } }
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void OnGridDeserialized() {
			for(int i = 0; i < Count; i++) {
				this[i].OnGridDeserialized(this);
			}
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public void ClearAndDispose() {
			PivotGridFieldBase[] fields = new PivotGridFieldBase[List.Count];
			List.CopyTo(fields, 0);
			Clear();
			for(int i = 0; i < fields.Length; i++) {
				DisposeField(fields[i]);
			}
		}
		protected virtual void DisposeField(PivotGridFieldBase field) {
			field.Dispose();
		}
		protected override void OnClearComplete() {
			base.OnClearComplete();
			this.names.Clear();
			if(Data != null) Data.OnColumnsClear();
		}
		protected override void OnInsertComplete(int index, object obj) {
			PivotGridFieldBase field = (PivotGridFieldBase)obj;
			field.SetCollection(this);
			AddNameCore(field);
			base.OnInsertComplete(index, obj);
			if(Data == null) return;
			if(index == Count - 1 && field.AreaIndex == -1) {
				int areaIndex = Data.GetFieldCountByArea(field.Area) - 1;
				field.AreaIndexCore = areaIndex;
				field.AreaIndexOldCore = areaIndex;
			} else {
				UpdateAreaIndexes();
			}
			Data.OnColumnInsert(obj as PivotGridFieldBase);
			if(!Data.IsLockUpdate) GroupFieldsByHierarchies();
		}
		protected override void OnRemoveComplete(int index, object obj) {
			base.OnRemoveComplete(index, obj);
			UpdateAreaIndexes();
			PivotGridFieldBase field = (PivotGridFieldBase)obj;
			this.names.Remove(field.Name);
			if(field.Group != null)
				field.Group.Remove(field);
			if(Data != null) Data.OnColumnRemove(field);
		}
		protected override void OnSetComplete(int index, object oldValue, object newValue) {
			base.OnSetComplete(index, oldValue, newValue);
			OnChanged();
		}
		[Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
		public virtual void GroupFieldsByHierarchies() {
			if(Data == null || !Data.IsOLAP || Data.IsDeserializing) return;
			LockUpdateAreaIndexes();
			Dictionary<string, List<PivotGridFieldBase>> groups = GetFieldGroups();
			foreach(List<PivotGridFieldBase> fieldGroup in groups.Values) {
				if(fieldGroup.Count == 1 && (Data == null || !Data.GetOlapIsUserHierarchy(fieldGroup[0].Hierarchy))) continue;
				PivotGridGroup group = GetGroup(fieldGroup);
				foreach(PivotGridFieldBase field in fieldGroup)
					if(field.Group != group)
						group.Add(field);
				if(!Data.Groups.Contains(group))
					Data.Groups.Add(group);
			}
			for(int i = Data.Groups.Count - 1; i >= 0; i--) {
				PivotGridGroup group = Data.Groups[i];
				if(!group.IsOLAP) continue;
				group.RemoveInvalidFields();
				if(group.Fields.Count == 0)
					Data.Groups.RemoveAt(i);
			}
			UnlockUpdateAreaIndexes();
		}
		PivotGridGroup GetGroup(List<PivotGridFieldBase> fields) {
			foreach(PivotGridFieldBase field in fields)
				if(field.HasGroup) return field.Group;
			return Data.Groups.CreateGroup(fields[0].Hierarchy);
		}
		Dictionary<string, List<PivotGridFieldBase>> GetFieldGroups() {
			Dictionary<string, List<PivotGridFieldBase>> groups = new Dictionary<string, List<PivotGridFieldBase>>();
			foreach(PivotGridFieldBase field in this) {
				CorrectFieldHierarchy(field);
				if(!string.IsNullOrEmpty(field.Hierarchy)) {
					if(!groups.ContainsKey(field.Hierarchy))
						groups.Add(field.Hierarchy, new List<PivotGridFieldBase>());
					groups[field.Hierarchy].Add(field);
				}
			}
			return groups;
		}
		void CorrectFieldHierarchy(PivotGridFieldBase field) {
			PivotGridGroup group = field.Group;
			if(group == null || !group.IsOLAP || group.IsFieldValid(field)) return;
			group.Remove(field);
			group.FilterValues.Reset();
		}
		protected virtual bool IsNameOccupied(string name) {
			return GetFieldByName(name) != null;
		}
		protected internal virtual string GenerateNameCore(string fieldName) {
			if(!String.IsNullOrEmpty(fieldName)) {
				if(PivotGridFieldBase.IsOLAPFieldName(fieldName)) {
					string[] parts = fieldName.Split('.');
					fieldName = parts[parts.Length - 1];
				}
				char[] buf = new char[fieldName.Length + 1];
				int c = 0;
				for(int i = 0; i < fieldName.Length; i++)
					if((fieldName[i] >= 'A' && fieldName[i] <= 'Z') ||
						(fieldName[i] >= 'a' && fieldName[i] <= 'z') ||
						(fieldName[i] >= '0' && fieldName[i] <= '9')) {
						buf[c] = fieldName[i];
						c++;
					}
				return DefaultNamePrefix + new string(buf, 0, c);
			} else return DefaultNamePrefix;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public string GenerateName(string fieldName) {
			string res = GenerateNameCore(fieldName),
				final = res;
			int n = 1;
			while(IsNameOccupied(final))
				final = res + n++.ToString();
			return final;
		}
		protected internal bool IsDefaultName(string name, string fieldName) {
			if(string.IsNullOrEmpty(name)) return true;
			string defaultName = GenerateNameCore(fieldName);
			if(!name.StartsWith(defaultName))
				return false;
			for(int i = defaultName.Length; i < name.Length; i++) {
				if(!char.IsNumber(name[i]))
					return false;
			}
			return true;
		}
		[EditorBrowsable(EditorBrowsableState.Never)]
		public void OnNameChanged(PivotGridFieldBase field, string oldValue) {
			List<PivotGridFieldBase> list;
			if(this.names.TryGetValue(oldValue, out list)) {
				list.Remove(field);
			}
			AddNameCore(field);
			if(Data != null)
				Data.OnColumnNameChanged(field);
		}
		void AddNameCore(PivotGridFieldBase field) {
			List<PivotGridFieldBase> list;
			if(!this.names.TryGetValue(field.Name, out list)) {
				list = new List<PivotGridFieldBase>();
				this.names.Add(field.Name, list);
			}
			list.Add(field);
		}
		protected void OnChanged() {
			if(Data != null)
				Data.FieldCollectionChanged();
		}
	}
#if !SL && !DXPORTABLE
	public class PivotObjectEditorTypeConverter : DevExpress.Utils.Editors.ObjectEditorTypeConverter {
		public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType) {
			if(destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor) && value != null)
				return new System.ComponentModel.Design.Serialization.InstanceDescriptor(typeof(string).GetConstructor(new Type[] { typeof(char[]) }), new object[] { value.ToString().ToCharArray() }, false);
			return base.ConvertTo(context, culture, value, destinationType);
		}
		public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType) {
			if(destinationType == typeof(System.ComponentModel.Design.Serialization.InstanceDescriptor))
				return true;
			return base.CanConvertTo(context, destinationType);
		}
	}
#endif
}
