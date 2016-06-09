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
	public enum GridMeasureColumnDisplayMode { Value, Bar }
	[
	DesignerSerializer(TypeNames.DataItemContainerCodeDomSerializer, TypeNames.CodeDomSerializer)
	]
	public class GridMeasureColumn : GridColumnBase, ISupportValueBarDisplayMode {
		internal static bool IsBarModeAllowed(DataFieldType dataType, Measure measure, SummaryType summaryType, bool isOlap) {
			bool allowBarMode = DataBindingHelper.IsNumeric(dataType) ||
			(measure != null && (summaryType == SummaryType.Count || summaryType == SummaryType.CountDistinct || isOlap));
			return allowBarMode;
		}
		const string xmlDisplayMode = "DisplayMode";
		const string xmlAlwaysShowZeroLevel = "AlwaysShowZeroLevel";
		const string measureName = "Measure";
		const GridMeasureColumnDisplayMode DefaultDisplayMode = GridMeasureColumnDisplayMode.Value;
		const bool DefaultAlwaysShowZeroLevel = false;
		GridMeasureColumnDisplayMode displayMode = DefaultDisplayMode;
		bool alwaysShowZeroLevel = DefaultAlwaysShowZeroLevel;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridMeasureColumnDisplayMode"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultDisplayMode)
		]
		public GridMeasureColumnDisplayMode DisplayMode {
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
	DevExpressDashboardCoreLocalizedDescription("GridMeasureColumnAlwaysShowZeroLevel"),
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
	DevExpressDashboardCoreLocalizedDescription("GridMeasureColumnMeasure"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Measure Measure {
			get { return GetMeasure(measureName); }
			set { SetDataItem(measureName, value); }
		}
		protected internal override string DefaultName {
			get {
				Measure measure = Measure;
				return measure != null ? measure.DisplayName : string.Empty;
			} 
		}
		protected internal override string ColumnId { get { return "GridMeasureColumn"; } }
		bool IsOlapDataSource { get { return DataSource != null && DataSource.GetIsOlap(); } }
		GridColumnValueBarDisplayMode ISupportValueBarDisplayMode.DisplayMode { 
			get {
				return displayMode == GridMeasureColumnDisplayMode.Bar ? GridColumnValueBarDisplayMode.Bar : GridColumnValueBarDisplayMode.Value; 
			} 
		}
		public GridMeasureColumn(Measure measure)
			: base(new DataItemDescription[] { new DataItemDescription(measureName, DashboardLocalizer.GetString(DashboardStringId.DescriptionItemMeasure), measure) }) {
		}
		public GridMeasureColumn()
			: this(null) {
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(displayMode != DefaultDisplayMode)
				element.Add(new XAttribute(xmlDisplayMode, displayMode));
			if(alwaysShowZeroLevel != DefaultAlwaysShowZeroLevel)
				element.Add(new XAttribute(xmlAlwaysShowZeroLevel, alwaysShowZeroLevel));
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			string displayModeString = XmlHelper.GetAttributeValue(element, xmlDisplayMode);
			if(!string.IsNullOrEmpty(displayModeString))
				displayMode = XmlHelper.FromString<GridMeasureColumnDisplayMode>(displayModeString);
			string alwaysShowZeroLevelString = XmlHelper.GetAttributeValue(element, xmlAlwaysShowZeroLevel);
			if(!string.IsNullOrEmpty(alwaysShowZeroLevelString))
				alwaysShowZeroLevel = XmlHelper.FromString<bool>(alwaysShowZeroLevelString);
		}
		protected internal override GridColumnType GetColumnType() {
			return GridColumnType.Measure;
		}
		protected internal override string GetDataId() {
			Measure measure = Measure;
			return measure != null ? measure.ActualId : null;
		}
		protected internal override IEnumerable<GridColumnTotalType> GetAvailableTotalTypes() {
			IEnumerable<GridColumnTotalType> availableTypes = GetTotalTypes(Measure);
			return IsOlapDataSource ? availableTypes : availableTypes.Append(GridColumnTotalType.Auto);		   
		}
		protected internal override void PrepareViewModel(GridColumnViewModel viewModel, IDashboardDataSource dataSource, string dataMember) {
			viewModel.DisplayMode = GridColumnDisplayMode.Value;
			if (DataSourceHelper.IsNumericMeasure(dataSource, dataMember, Measure))
				viewModel.HorzAlignment = GridColumnHorzAlignment.Right;
			else
				viewModel.HorzAlignment = GridColumnHorzAlignment.Left;
			viewModel.DataType = DataSourceHelper.GetApproximatedDataType(dataSource, dataMember, Measure);
			Measure measure = Measure;
			if(measure != null) {
				if(displayMode == GridMeasureColumnDisplayMode.Bar) {
					viewModel.DisplayMode = GridColumnDisplayMode.Bar;
					viewModel.BarViewModel = new GridBarViewModel(alwaysShowZeroLevel);
				}
			}
		}
		protected override DataItemContainer CreateInstance() {
			return new GridMeasureColumn();
		}
		protected internal override void Assign(DataItemContainer container) {
			base.Assign(container);
			GridMeasureColumn measureColumn = container as GridMeasureColumn;
			if(measureColumn != null) {
				Measure = measureColumn.Measure;
				displayMode = measureColumn.DisplayMode;
				alwaysShowZeroLevel = measureColumn.AlwaysShowZeroLevel;
			}
		}
		protected internal override DataItemContainerActualContent GetActualContent() {
			DataItemContainerActualContent content = new DataItemContainerActualContent();
			if(Measure != null) 
				content.Measures.Add(Measure);
			return content;
		}
		protected override SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			return SummaryTypeInfo.Text;
		}
		protected override MeasureDescriptorInternal CreateSummaryDescriptor(GridColumnTotal total, string id) {
			if(total.TotalType == GridColumnTotalType.Auto)
				return null;
			Measure measure = Measure;
			return new MeasureDescriptorInternal() {
				ID = id,
				Name = null,
				Format = total.TotalType == GridColumnTotalType.Count ? new ValueFormatViewModel() : measure.CreateValueFormatViewModel()
			};
		}
		protected override SummaryAggregationModel CreateSummaryAggregationModel(GridColumnTotal total, ItemModelBuilder itemBuilder) {
			if(total.TotalType == GridColumnTotalType.Auto)
				return null;
			return new SummaryAggregationModel(CreateTotalId(total), itemBuilder.GetMeasureModel(Measure), total.TotalType.ToSummaryItemTypeEx(), 0, true);
		}
	}
}
