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
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.Drawing.Design;
using DevExpress.DashboardCommon.DataProcessing;
namespace DevExpress.DashboardCommon {
	public enum GridDeltaColumnDisplayMode { Value, Bar }
	[
	DesignerSerializer(TypeNames.DataItemContainerCodeDomSerializer, TypeNames.CodeDomSerializer)
	]
	public class GridDeltaColumn : GridColumnBase, ISupportValueBarDisplayMode {
		const string xmlDisplayMode = "DisplayMode";
		const string xmlAlwaysShowZeroLevel = "AlwaysShowZeroLevel";
		const string actualValueKey = "ActualValue";
		const string actualValueKeyOld = "Value";
		const string targetValueKey = "TargetValue";
		const string targetValueKeyOld = "Target";
		const string deltaMainValue  = "DeltaMainValue";
		const string normalizedValueDataMember = "NormalizedValue";
		const string deltaIsGoodDataMember = "DeltaIsGood";
		const string deltaIndicatorTypeDataMember = "DeltaIndicatorType";
		const GridDeltaColumnDisplayMode DefaultDisplayMode = GridDeltaColumnDisplayMode.Value;
		const bool DefaultAlwaysShowZeroLevel = false;
		readonly DeltaOptions deltaOptions;
		GridDeltaColumnDisplayMode displayMode = DefaultDisplayMode;
		bool alwaysShowZeroLevel = DefaultAlwaysShowZeroLevel;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridDeltaColumnDeltaOptions"),
#endif
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DeltaOptions DeltaOptions { get { return deltaOptions; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridDeltaColumnDisplayMode"),
#endif
		DefaultValue(DefaultDisplayMode)
		]
		public GridDeltaColumnDisplayMode DisplayMode {
			get { return displayMode; }
			set {
				if(displayMode != value) {
					displayMode = value;
					OnChanged(ChangeReason.View, this, displayMode);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridDeltaColumnAlwaysShowZeroLevel"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultAlwaysShowZeroLevel)
		]
		public bool AlwaysShowZeroLevel {
			get { return alwaysShowZeroLevel; }
			set {
				if(alwaysShowZeroLevel != value) {
					alwaysShowZeroLevel = value;
					OnChanged(ChangeReason.View, this, alwaysShowZeroLevel);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridDeltaColumnActualValue"),
#endif
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Measure ActualValue {
			get { return GetMeasure(actualValueKey); }
			set { SetDataItem(actualValueKey, value); }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridDeltaColumnTargetValue"),
#endif
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Measure TargetValue {
			get { return GetMeasure(targetValueKey); }
			set { SetDataItem(targetValueKey, value); }
		}
		protected internal override string DefaultName { get { return KpiElementCaptionProvider.GetCaption(ActualValue, TargetValue); } }
		protected internal override string ColumnId { get { return "GridDeltaColumn"; } }
		GridColumnValueBarDisplayMode ISupportValueBarDisplayMode.DisplayMode {
			get {
				return displayMode == GridDeltaColumnDisplayMode.Bar ? GridColumnValueBarDisplayMode.Bar : GridColumnValueBarDisplayMode.Value;
			}
		}
		public GridDeltaColumn(Measure actualValue, Measure targetValue)
			: base(new DataItemDescription[] { 
				new DataItemDescription(actualValueKey, DashboardLocalizer.GetString(DashboardStringId.ActualValueCaption), actualValue), 
				new DataItemDescription(targetValueKey, DashboardLocalizer.GetString(DashboardStringId.TargetValueCaption), targetValue) }) {
			deltaOptions = new DeltaOptions(this);
		}
		public GridDeltaColumn()
			: this(null, null) {
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			deltaOptions.SaveToXml(element);
			if(displayMode != DefaultDisplayMode)
				element.Add(new XAttribute(xmlDisplayMode, displayMode));
			if(alwaysShowZeroLevel != DefaultAlwaysShowZeroLevel)
				element.Add(new XAttribute(xmlAlwaysShowZeroLevel, alwaysShowZeroLevel));
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			deltaOptions.LoadFromXml(element);
			string displayModeString = XmlHelper.GetAttributeValue(element, xmlDisplayMode);
			if(!string.IsNullOrEmpty(displayModeString))
				displayMode = XmlHelper.FromString<GridDeltaColumnDisplayMode>(displayModeString);
			string alwaysShowZeroLevelString = XmlHelper.GetAttributeValue(element, xmlAlwaysShowZeroLevel);
			if(!string.IsNullOrEmpty(alwaysShowZeroLevelString))
				alwaysShowZeroLevel = XmlHelper.FromString<bool>(alwaysShowZeroLevelString);
		}
		protected internal override GridColumnType GetColumnType() {
			return GridColumnType.Delta;
		}
		protected internal override string GetDataId() {
			Measure actualValue = ActualValue;
			Measure targetValue = TargetValue;
			return actualValue != null ? actualValue.ActualId : (targetValue != null ? targetValue.ActualId : null);
		}
		protected internal override IEnumerable<GridColumnTotalType> GetAvailableTotalTypes() {
			yield return GridColumnTotalType.Count;
		}
		protected internal override void PrepareViewModel(GridColumnViewModel viewModel, IDashboardDataSource dataSource, string dataMember) {
			viewModel.DisplayMode = GridColumnDisplayMode.Delta;
			viewModel.HorzAlignment = DataSourceHelper.IsNumericMeasure(dataSource, dataMember, ActualValue ?? TargetValue) ? GridColumnHorzAlignment.Right : GridColumnHorzAlignment.Left;
			viewModel.DataType = DataSourceHelper.GetApproximatedDataType(dataSource, dataMember, ActualValue ?? TargetValue);
			Measure actualValue = ActualValue;
			Measure targetValue = TargetValue;
			DeltaValueType valueDisplayType = DeltaValueType.ActualValue;
			if(actualValue != null && targetValue != null) {
				valueDisplayType = deltaOptions.ValueType;
				viewModel.DeltaDisplayMode = GridDeltaDisplayMode.Delta;
			}
			else
				viewModel.DeltaDisplayMode = GridDeltaDisplayMode.Value;
			viewModel.IgnoreDeltaColor = valueDisplayType == DeltaValueType.ActualValue || deltaOptions.ResultIndicationMode == DeltaIndicationMode.NoIndication;
			viewModel.IgnoreDeltaIndication = deltaOptions.ResultIndicationMode == DeltaIndicationMode.NoIndication;
			viewModel.DeltaValueType = deltaOptions.ValueType;
			Measure value = actualValue ?? targetValue;
			if(value != null) {
				if(displayMode == GridDeltaColumnDisplayMode.Bar) {
					viewModel.DisplayMode = GridColumnDisplayMode.Bar;
					viewModel.BarViewModel = new GridBarViewModel(alwaysShowZeroLevel);
				}
			}
		}
		protected override DataItemContainer CreateInstance() {
			return new GridDeltaColumn();
		}
		protected override SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			return SummaryTypeInfo.Number;
		}
		protected internal override void Assign(DataItemContainer container) {
			base.Assign(container);
			GridDeltaColumn deltaColumn = container as GridDeltaColumn;
			if(deltaColumn != null) {
				ActualValue = deltaColumn.ActualValue;
				TargetValue = deltaColumn.TargetValue;
				displayMode = deltaColumn.DisplayMode;
				alwaysShowZeroLevel = deltaColumn.AlwaysShowZeroLevel;
			}
		}
		protected override IList<string> GetAlternateDataItemKeys(string dataItemKey) {
			if(dataItemKey == actualValueKey)
				return new string[] { actualValueKeyOld };
			if(dataItemKey == targetValueKey)
				return new string[] { targetValueKeyOld };
			return null;
		}
		protected internal override DataItemContainerActualContent GetActualContent() {
			DataItemContainerActualContent content = new DataItemContainerActualContent();
			if(ActualValue != null) {
				content.Measures.Add(ActualValue);
				content.DeltaActualValue = ActualValue;
			}
			if(TargetValue != null) {
				content.Measures.Add(TargetValue);
				content.DeltaTargetValue = TargetValue;
			}
			content.IsDelta = true;
			content.DeltaOptions = deltaOptions;
			return content;
		}
		protected internal override void FillDashboardItemDataDescription(DashboardItemDataDescription description) {
			description.AddMeasure(ActualValue, TargetValue, DeltaOptions);
		}
		protected override MeasureDescriptorInternal CreateSummaryDescriptor(GridColumnTotal total, string id) {
			if(total.TotalType != GridColumnTotalType.Count)
				return null;
			Measure measure = ActualValue ?? TargetValue;
			return new MeasureDescriptorInternal() {
				ID = id,
				Name = null,
				Format = total.TotalType == GridColumnTotalType.Count ? new ValueFormatViewModel() : measure.CreateValueFormatViewModel()
			};
		}
		protected override SummaryAggregationModel CreateSummaryAggregationModel(GridColumnTotal total, ItemModelBuilder itemBuilder) {
			if(total.TotalType != GridColumnTotalType.Count)
				return null;
			Measure measure = ActualValue ?? TargetValue;
			return new SummaryAggregationModel(CreateTotalId(total), itemBuilder.GetMeasureModel(measure), total.TotalType.ToSummaryItemTypeEx(), 0, true);
		}
	}
}
