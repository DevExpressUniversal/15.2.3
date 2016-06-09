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
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native;
using DevExpress.Utils;
using DevExpress.XtraPivotGrid;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public enum DimensionSortOrder {
		Ascending = PivotSortOrder.Ascending,
		Descending = PivotSortOrder.Descending,
		None
	}
	public enum DimensionSortMode {
		DisplayText = PivotSortMode.DisplayText,
		Value = PivotSortMode.Value,
		ID = PivotSortMode.ID,
		Key = PivotSortMode.Key
	}
	public enum TextGroupInterval {
		None,
		Alphabetical
	}
	public enum DateTimeGroupInterval {
		Year,
		Quarter,
		Month,
		Day,
		Hour,
		Minute,
		Second,
		DayOfYear,
		DayOfWeek,
		WeekOfYear,
		WeekOfMonth,
		MonthYear,
		QuarterYear,
		DayMonthYear,
		DateHour,
		DateHourMinute,
		DateHourMinuteSecond,
		None
	}
	[
	DesignerSerializer(TypeNames.ISupportInitializeCodeDomSerializer, TypeNames.CodeDomSerializer),
	TypeConverter(TypeNames.DimensionConverter)
	]
	public class Dimension : DataItem {
		const string xmlGroupIndex = "GroupIndex";
		const string xmlTextGroupInterval = "TextGroupInterval";
		const string xmlDateTimeGroupInterval = "DateTimeGroupInterval";
		const string xmlSortOrder = "SortOrder";
		const string xmlSortMode = "SortMode";
		const string xmlSortByMeasure = "SortByMeasure";
		const string xmlIsDiscreteNumericScale = "IsDiscreteScale";
		const string xmlColoringMode = "ColoringMode";
		internal const int DefaultGroupIndex = -1;
		internal const bool DefaultIsDiscreteNumericScale = false;
		const DimensionSortOrder DefaultSortOrder = DimensionSortOrder.Ascending;
		const DimensionSortMode DefaultSortMode = DimensionSortMode.Value;
		internal const ColoringMode DefaultColoringMode = ColoringMode.Default;
		internal static string GetColoringModeCaption(ColoringMode coloringMode) {
			switch(coloringMode) {
				case ColoringMode.Default: return DashboardLocalizer.GetString(DashboardStringId.ColoringModeDefault);
				case ColoringMode.None: return DashboardLocalizer.GetString(DashboardStringId.ColoringModeNone);
				case ColoringMode.Hue: return DashboardLocalizer.GetString(DashboardStringId.ColoringModeHue);
				default: throw new Exception(Helper.GetUnknownEnumValueMessage(coloringMode));
			}
		}
		readonly DimensionTopNOptions topNOptions;
		readonly DataItemBox<Measure> sortByMeasureBox;
		readonly Locker locker = new Locker();
		TextGroupInterval textGroupInterval;
		DateTimeGroupInterval dateTimeGroupInterval;
		bool isDiscreteNumericScale = DefaultIsDiscreteNumericScale;
		DimensionSortOrder sortOrder = DefaultSortOrder;
		DimensionSortMode sortMode = DefaultSortMode;
		int groupIndex;
		ColoringMode coloringMode = DefaultColoringMode;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DimensionGroupIndex"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultGroupIndex)
		]
		public int GroupIndex { get { return groupIndex; } set { groupIndex = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DimensionTopNOptions"),
#endif
		Category(CategoryNames.Data),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DimensionTopNOptions TopNOptions { get { return topNOptions; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DimensionTextGroupInterval"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DimensionDefinition.DefaultTextGroupInterval)
		]
		public TextGroupInterval TextGroupInterval {
			get { return textGroupInterval; }
			set {
				if (value != textGroupInterval) {
					DimensionDefinition definition = GetDimensionDefinition();
					textGroupInterval = value;
					OnChanged(ChangeReason.ClientData, definition);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DimensionDateTimeGroupInterval"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DimensionDefinition.DefaultDateTimeGroupInterval)
		]
		public DateTimeGroupInterval DateTimeGroupInterval {
			get { return dateTimeGroupInterval; }
			set {
				if(value != dateTimeGroupInterval) {
					DimensionDefinition definition = GetDimensionDefinition();
					dateTimeGroupInterval = value;
					OnChanged(ChangeReason.ClientData, definition);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DimensionIsDiscreteNumericScale"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultIsDiscreteNumericScale)
		]
		public bool IsDiscreteNumericScale {
			get { return isDiscreteNumericScale; }
			set {
				if(value != isDiscreteNumericScale) {
					isDiscreteNumericScale = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DimensionSortOrder"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultSortOrder)
		]
		public DimensionSortOrder SortOrder {
			get { return sortOrder; }
			set {
				if(value != sortOrder) {
					sortOrder = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DimensionSortMode"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultSortMode)
		]
		public DimensionSortMode SortMode {
			get { return sortMode; }
			set {
				if(value != sortMode) {
					sortMode = value;
					OnChanged();
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DimensionSortByMeasure"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter("DevExpress.DashboardWin.Design.SortByMeasurePropertyTypeConverter," + AssemblyInfo.SRAssemblyDashboardWinDesign),
		DefaultValue(null)
		]
		public Measure SortByMeasure { get { return sortByMeasureBox.DataItem; } set { sortByMeasureBox.DataItem = value; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public string SortByMeasureName { get { return sortByMeasureBox.UniqueName; } set { sortByMeasureBox.UniqueName = value; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("DimensionColoringMode"),
#endif
		DefaultValue(DefaultColoringMode)
		]
		public ColoringMode ColoringMode {  
			get { return coloringMode; }
			set {
				if(coloringMode != value) {
					coloringMode = value;
					OnChanged();
				}
			}
		}
		internal DimensionSortOrder ActualSortOrder { get { return GetActualSortingOptions().SortOrder; } }
		internal DimensionSortMode? ActualSortMode { get { return GetActualSortingOptions().SortMode; } }
		internal Measure ActualSortByMeasure { get { return GetActualSortingOptions().SortByMeasure; } }
		internal bool IsSortOrderNoneEnabled { get { return DataSourceSchema != null && DataSourceSchema.GetIsSortOrderNoneSupported(); } }
		internal IEnumerable<DimensionSortMode> SortModesAvailable {
			get {
				if(topNOptions.Enabled)
					return new DimensionSortMode[] { };
				if(DataSourceSchema != null && !DataSourceSchema.GetIsSortOrderNoneSupported())
					return new DimensionSortMode[] { DimensionSortMode.Value };
				return (IEnumerable<DimensionSortMode>)Enum.GetValues(typeof(DimensionSortMode));
			}
		}
		internal DimensionSortOrder GetNextActualSortOrder() {
			DimensionSortOrder actualSortOrder = ActualSortOrder;
			if(actualSortOrder == DimensionSortOrder.Ascending)
				return DimensionSortOrder.Descending;
			if(actualSortOrder == DimensionSortOrder.None)
				return DimensionSortOrder.Ascending;
			if(IsSortOrderNoneEnabled)
				return DimensionSortOrder.None;
			return DimensionSortOrder.Ascending;
		}
		internal bool ColorByHue { get { return ColoringMode == ColoringMode.Hue || ColoringMode == ColoringMode.Default && Context != null && Context.ColorDimension(this); } }
		protected internal override int ActualGroupIndex { get { return groupIndex; } }
		protected override bool FormatDecimalAsCurrency { get { return true; } }
		protected override bool CanFormatValueAsDateTime { get { return true; } }
		protected internal override bool CanSpecifyDateTimeFormat {
			get {
				if(base.CanSpecifyDateTimeFormat) {
					switch(dateTimeGroupInterval) {
						case DateTimeGroupInterval.Year:
						case DateTimeGroupInterval.Quarter:
						case DateTimeGroupInterval.Month:
						case DateTimeGroupInterval.DayOfWeek:
						case DateTimeGroupInterval.DayMonthYear:
						case DateTimeGroupInterval.DateHour:
						case DateTimeGroupInterval.DateHourMinute:
						case DateTimeGroupInterval.DateHourMinuteSecond:
						case DateTimeGroupInterval.Hour:
						case DateTimeGroupInterval.None:
							return true;
						default:
							return false;
					}
				}
				return false;
			}
		}
		internal override DataFieldType ActualDataFieldType {
			get {
				DataFieldType actualDataFieldType = base.ActualDataFieldType;
				if(actualDataFieldType == DataFieldType.DateTime) {
					if(dateTimeGroupInterval == DateTimeGroupInterval.Year || dateTimeGroupInterval == DateTimeGroupInterval.Quarter || dateTimeGroupInterval == DateTimeGroupInterval.Month 
						|| dateTimeGroupInterval == DateTimeGroupInterval.Day || dateTimeGroupInterval == DateTimeGroupInterval.Hour 
						|| dateTimeGroupInterval == DateTimeGroupInterval.Minute || dateTimeGroupInterval == DateTimeGroupInterval.Second 
						|| dateTimeGroupInterval == DateTimeGroupInterval.DayOfYear || dateTimeGroupInterval == DateTimeGroupInterval.DayOfWeek
						|| dateTimeGroupInterval == DateTimeGroupInterval.WeekOfYear || dateTimeGroupInterval == DateTimeGroupInterval.WeekOfMonth)
						return DataFieldType.Integer;
				}
				return actualDataFieldType;
			}
		}
		public Dimension()
			: this((string)null) {
		}
		public Dimension(string dataMember)
			: this(null, dataMember) {
		}
		public Dimension(string dataMember, int groupIndex)
			: this(null, new DimensionDefinition(dataMember), groupIndex) {
		}
		public Dimension(string dataMember, DateTimeGroupInterval dateTimeGroupInterval)
			: this(null, dataMember, dateTimeGroupInterval) {
		}
		public Dimension(string dataMember, TextGroupInterval textGroupInterval)
			: this(null, dataMember, textGroupInterval) {
		}
		public Dimension(string id, string dataMember)
			: this(id, new DimensionDefinition(dataMember)) {
		}
		public Dimension(string id, string dataMember, DateTimeGroupInterval dateTimeGroupInterval)
			: this(id, new DimensionDefinition(dataMember, dateTimeGroupInterval)) {
		}
		public Dimension(string id, string dataMember, TextGroupInterval textGroupInterval)
			: this(id, new DimensionDefinition(dataMember, textGroupInterval)) {
		}
		public Dimension(DimensionDefinition definition)
			: this(null, definition) {
		}
		public Dimension(string id, DimensionDefinition definition)
			: this(id, definition, DefaultGroupIndex) {
		}
		public Dimension(string id, DimensionDefinition definition, int groupIndex)
			: base(id, definition) {
			topNOptions = new DimensionTopNOptions(this);
			sortByMeasureBox = new DataItemBox<Measure>((IDataItemRepositoryProvider)this, xmlSortByMeasure);
			sortByMeasureBox.Changed += (sender, e) => OnChanged();
			this.textGroupInterval = definition.TextGroupInterval;
			this.dateTimeGroupInterval = definition.DateTimeGroupInterval;
			this.groupIndex = groupIndex;
		}
		public DimensionDefinition GetDimensionDefinition() {
			return new DimensionDefinition(DataMember, DateTimeGroupInterval, TextGroupInterval);
		}
		public override DataItemDefinition GetDefinition() {
			return GetDimensionDefinition();
		}
		internal SortingOptions GetActualSortingOptions() {
			SortingOptions actualSortingOptions = new SortingOptions();
			DimensionSortOrder sortOrderNoneReplaced = sortOrder == DimensionSortOrder.None ? DefaultSortOrder : sortOrder;
			if(TopNOptions.Enabled) {
				actualSortingOptions.SortOrder = DimensionTopNOptions.GetDimensionSortOrder(topNOptions.Mode);
				actualSortingOptions.SortMode = null;
				actualSortingOptions.SortByMeasure = topNOptions.Measure;
			}
			else if(DataSourceSchema != null && DataSourceSchema.GetIsSortOrderNoneSupported() && sortOrder == DimensionSortOrder.None) {
				actualSortingOptions.SortOrder = DimensionSortOrder.None;
				actualSortingOptions.SortMode = null;
				actualSortingOptions.SortByMeasure = null;
			}
			else if(SortByMeasure != null) {
				actualSortingOptions.SortOrder = sortOrderNoneReplaced;
				actualSortingOptions.SortMode = null;
				actualSortingOptions.SortByMeasure = SortByMeasure;
			}
			else if(DataSourceSchema != null && DataSourceSchema.GetIsSpecificSortModeSupported()) {
				actualSortingOptions.SortOrder = sortOrderNoneReplaced;
				actualSortingOptions.SortMode = sortMode;
				actualSortingOptions.SortByMeasure = null;
			}
			else {
				actualSortingOptions.SortOrder = sortOrderNoneReplaced;
				actualSortingOptions.SortMode = DefaultSortMode;
				actualSortingOptions.SortByMeasure = null;
			}
			return actualSortingOptions;
		}
		internal void BeginUpdateSortingOptions() {
			locker.Lock();
		}
		internal void EndUpdateSortingOptions() {
			locker.Unlock();
			OnChanged();
		}
		protected override void OnChanged(ChangeReason reason, DataItemDefinition definition) {
			if(!locker.IsLocked)
				base.OnChanged(reason, definition);
		}		
		protected override string GetNameSuffix() {
			string suffix = null;
			switch(DataFieldType) {
				case DataFieldType.Text:
					TextGroupInterval textGroupInterval = TextGroupInterval;
					if(textGroupInterval != TextGroupInterval.None)
						suffix = GroupIntervalCaptionProvider.GetTextGroupIntervalCaption(textGroupInterval);
					break;
				case DataFieldType.DateTime:
					if(dateTimeGroupInterval != DateTimeGroupInterval.None)
						suffix = GroupIntervalCaptionProvider.GetDateTimeGroupIntervalCaption(dateTimeGroupInterval);
					break;
			}
			return suffix;
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(GroupIndex != DefaultGroupIndex)
				element.Add(new XAttribute(xmlGroupIndex, groupIndex));
			if(SortOrder != DefaultSortOrder)
				element.Add(new XAttribute(xmlSortOrder, sortOrder));
			if(SortMode != DefaultSortMode)
				element.Add(new XAttribute(xmlSortMode, sortMode));
			if(TextGroupInterval != DimensionDefinition.DefaultTextGroupInterval)
				element.Add(new XAttribute(xmlTextGroupInterval, textGroupInterval));
			if (DateTimeGroupInterval != DimensionDefinition.DefaultDateTimeGroupInterval)
				element.Add(new XAttribute(xmlDateTimeGroupInterval, dateTimeGroupInterval));
			if(IsDiscreteNumericScale != DefaultIsDiscreteNumericScale)
				element.Add(new XAttribute(xmlIsDiscreteNumericScale, isDiscreteNumericScale));
			if(ColoringMode != DefaultColoringMode)
				element.Add(new XAttribute(xmlColoringMode, ColoringMode));
			sortByMeasureBox.SaveToXml(element);
			topNOptions.SaveToXml(element);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			string groupIndexAttr = XmlHelper.GetAttributeValue(element, xmlGroupIndex);
			if(groupIndexAttr != null)
				groupIndex = XmlHelper.FromString<int>(groupIndexAttr);
			string sortOrderAttr = XmlHelper.GetAttributeValue(element, xmlSortOrder);
			if (!String.IsNullOrEmpty(sortOrderAttr))
				sortOrder = XmlHelper.EnumFromString<DimensionSortOrder>(sortOrderAttr);
			string sortModeAttr = XmlHelper.GetAttributeValue(element, xmlSortMode);
			if(!String.IsNullOrEmpty(sortModeAttr))
				sortMode = XmlHelper.EnumFromString<DimensionSortMode>(sortModeAttr);
			string textGroupIntervalAttr = XmlHelper.GetAttributeValue(element, xmlTextGroupInterval);
			if (!String.IsNullOrEmpty(textGroupIntervalAttr))
				textGroupInterval = XmlHelper.EnumFromString<TextGroupInterval>(textGroupIntervalAttr);
			string dateTimeGroupIntervalAttr = XmlHelper.GetAttributeValue(element, xmlDateTimeGroupInterval);
			if(!string.IsNullOrEmpty(dateTimeGroupIntervalAttr))
				dateTimeGroupInterval = XmlHelper.EnumFromString<DateTimeGroupInterval>(dateTimeGroupIntervalAttr);
			string isDiscreteNumericScaleAttr = XmlHelper.GetAttributeValue(element, xmlIsDiscreteNumericScale);
			if(!string.IsNullOrEmpty(isDiscreteNumericScaleAttr))
				isDiscreteNumericScale = XmlHelper.FromString<bool>(isDiscreteNumericScaleAttr);
			string coloringModeAttr = XmlHelper.GetAttributeValue(element, xmlColoringMode);
			if(!string.IsNullOrEmpty(coloringModeAttr))
				coloringMode = XmlHelper.FromString<ColoringMode>(coloringModeAttr);
			sortByMeasureBox.LoadFromXml(element);
			topNOptions.LoadFromXml(element);
		}
		protected internal override void OnEndLoading() {
			base.OnEndLoading();
			sortByMeasureBox.OnEndLoading();
			topNOptions.OnEndLoading();
		}
		protected override ValueFormatViewModel CreateDefaultValueFormatViewModel() {
			return new ValueFormatViewModel();
		}
		void AssignCore(Dimension dimension) {
			DateTimeGroupInterval = dimension.DateTimeGroupInterval;
			TextGroupInterval = dimension.TextGroupInterval;
			IsDiscreteNumericScale = dimension.IsDiscreteNumericScale;
			SortOrder = dimension.SortOrder;
			SortMode = dimension.SortMode;
			GroupIndex = dimension.GroupIndex;
			ColoringMode = dimension.ColoringMode;
		}
		protected override void WeakAssign(DataItem dataItem) {
			base.WeakAssign(dataItem);
			Dimension dimension = dataItem as Dimension;
			if (dimension != null) {
				AssignCore(dimension);
				TopNOptions.WeakAssign(dimension.TopNOptions);
			}
		}
		protected override void Assign(DataItem dataItem) {
			base.Assign(dataItem);
			Dimension dimension = dataItem as Dimension;
			if (dimension != null) {
				AssignCore(dimension);
				SortByMeasure = dimension.SortByMeasure;
				TopNOptions.Assign(dimension.TopNOptions);
			}
		}
		internal Dimension WeakClone() {
			Dimension clone = new Dimension();
			clone.WeakAssign(this);
			return clone;
		}
		internal Dimension Clone() {
			Dimension clone = new Dimension();
			clone.Assign(this);
			return clone;
		}
		internal DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo() {
			IDataSourceSchema dataSourceSchema = DataSourceSchema;
			if(dataSourceSchema == null || !dataSourceSchema.GetIsDimensionGroupIntervalSupported())
				return null;
			DataFieldType fieldType = DataFieldType;
			if(DataBindingHelper.IsText(fieldType) || DataBindingHelper.IsDateTime(fieldType) || DataBindingHelper.IsNumeric(fieldType)) {
				DimensionGroupIntervalInfo groupIntervalInfo = Context != null ? Context.GetDimensionGroupIntervalInfo(this) : null;
				if(groupIntervalInfo != null) {
					if(DataBindingHelper.IsText(fieldType)) {
						return new DimensionGroupIntervalInfo {
							TextGroupIntervals = groupIntervalInfo.TextGroupIntervals
						};
					}
					if(DataBindingHelper.IsDateTime(fieldType)) {
						return new DimensionGroupIntervalInfo {
							DateTimeDiscreteGroupIntervals = groupIntervalInfo.DateTimeDiscreteGroupIntervals,
							DateTimeContinuousGroupIntervalsButExactDate = groupIntervalInfo.DateTimeContinuousGroupIntervalsButExactDate,
							IsSupportExactDateGroupInterval = groupIntervalInfo.IsSupportExactDateGroupInterval
						};
					}
					if(DataBindingHelper.IsNumeric(fieldType)) {
						return new DimensionGroupIntervalInfo {
							IsSupportNumericGroupIntervals = groupIntervalInfo.IsSupportNumericGroupIntervals
						};
					}
				}
			}
			return null;
		}
	}
	internal class SortingOptions {
		internal DimensionSortOrder SortOrder { get; set; }
		internal DimensionSortMode? SortMode { get; set; }
		internal Measure SortByMeasure { get; set; }
	}
}
