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
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public enum GridDimensionColumnDisplayMode { Text, Image }
	[
	DesignerSerializer(TypeNames.DataItemContainerCodeDomSerializer, TypeNames.CodeDomSerializer)
	]
	public class GridDimensionColumn : GridColumnBase {
		const string xmlDisplayMode = "DisplayMode";
		const string dimensionName = "Dimension";
		const GridDimensionColumnDisplayMode DefaultDisplayMode = GridDimensionColumnDisplayMode.Text;
		GridDimensionColumnDisplayMode displayMode = DefaultDisplayMode;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridDimensionColumnDisplayMode"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultDisplayMode)
		]
		public GridDimensionColumnDisplayMode DisplayMode {
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
	DevExpressDashboardCoreLocalizedDescription("GridDimensionColumnDimension"),
#endif
 Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableDimensionPropertyTypeConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Dimension Dimension {
			get { return (Dimension)GetDataItem(dimensionName); }
			set { SetDataItem(dimensionName, value); }
		}
		protected internal override string DefaultName {
			get {
				Dimension dimension = Dimension;
				return dimension != null ? dimension.DisplayName : string.Empty;
			}
		}
		protected internal override int DataItemGroupIndex { get { return Dimension != null ? Dimension.GroupIndex : -1; } }
		protected internal override string ColumnId { get { return "GridDimensionColumn"; } }
		public GridDimensionColumn(Dimension dimension)
			: base(new DataItemDescription[] {
				new DataItemDescription(
					dimensionName,
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemDimension),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionDimensions),
					dimension
				)
			}) {
		}
		public GridDimensionColumn()
			: this(null) {
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(displayMode != DefaultDisplayMode)
				element.Add(new XAttribute(xmlDisplayMode, displayMode));
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			string displayModeString = XmlHelper.GetAttributeValue(element, xmlDisplayMode);
			if(!string.IsNullOrEmpty(displayModeString))
				displayMode = XmlHelper.FromString<GridDimensionColumnDisplayMode>(displayModeString);
		}
		protected internal override GridColumnType GetColumnType() {
			return GridColumnType.Dimension;
		}
		protected internal override string GetDataId() {
			Dimension dimension = Dimension;
			return dimension != null ? dimension.ActualId : null;
		}
		protected internal override IEnumerable<GridColumnTotalType> GetAvailableTotalTypes() {
			return GetTotalTypes(Dimension);
		}
		protected internal override void PrepareViewModel(GridColumnViewModel viewModel, IDashboardDataSource dataSource, string dataMember) {
			viewModel.AllowCellMerge = true;
			viewModel.DisplayMode = displayMode == GridDimensionColumnDisplayMode.Image ? GridColumnDisplayMode.Image : GridColumnDisplayMode.Value;
			DataFieldType actualDataType = DataSourceHelper.GetDimensionDataType(dataSource, dataMember, Dimension);
			viewModel.DataType = actualDataType;
			viewModel.GridColumnFilterMode = (actualDataType == DataFieldType.Unknown || actualDataType == DataFieldType.Custom) && displayMode != GridDimensionColumnDisplayMode.Image ? GridColumnFilterMode.DisplayText : GridColumnFilterMode.Value;
			Dimension dimension = Dimension;
			if(dimension != null) {
				if(dimension.DataFieldType != DataFieldType.DateTime && DataBindingHelper.IsNumeric(actualDataType))
					viewModel.HorzAlignment = GridColumnHorzAlignment.Right;
				else
					viewModel.HorzAlignment = GridColumnHorzAlignment.Left;
			}
		}
		protected override DataItemContainer CreateInstance() {
			return new GridDimensionColumn();
		}
		protected internal override void Assign(DataItemContainer container) {
			base.Assign(container);
			GridDimensionColumn dimensionColumn = container as GridDimensionColumn;
			if(dimensionColumn != null) {
				Dimension = dimensionColumn.Dimension;
				displayMode = dimensionColumn.DisplayMode;
			}
		}
		protected internal override DataItemContainerActualContent GetActualContent() {
			DataItemContainerActualContent content = new DataItemContainerActualContent();
			if(Dimension != null)
				content.Dimensions.Add(Dimension);
			return content;
		}
		protected override DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo(Dimension dimension) {
			return DimensionGroupIntervalInfo.Default;
		}
		protected internal override void FillDashboardItemDataDescription(DashboardItemDataDescription description) {
			description.AddMainDimension(Dimension);
		}
		protected override MeasureDescriptorInternal CreateSummaryDescriptor(GridColumnTotal total, string id) {
			Dimension dimension = Dimension;
			if(dimension == null || !IsCompatibleTotalType(total.TotalType))
				return null;
			return new MeasureDescriptorInternal() {
				ID = id,
				Name = null,
				Format = total.TotalType == GridColumnTotalType.Count ? new ValueFormatViewModel() : dimension.CreateValueFormatViewModel()
			};
		}
		protected override SummaryAggregationModel CreateSummaryAggregationModel(GridColumnTotal total, ItemModelBuilder itemBuilder) {
			if(!IsCompatibleTotalType(total.TotalType))
				return null;
			return new SummaryAggregationModel(CreateTotalId(total), itemBuilder.GetDimensionModel(Dimension, false).Item1, total.TotalType.ToSummaryItemTypeEx(), 0, true);
		}
		bool IsCompatibleTotalType(GridColumnTotalType total) {
			foreach(GridColumnTotalType supportedType in GetTotalTypes(Dimension))
				if(supportedType == total)
					return true;
			return false;
		}
	}
}
