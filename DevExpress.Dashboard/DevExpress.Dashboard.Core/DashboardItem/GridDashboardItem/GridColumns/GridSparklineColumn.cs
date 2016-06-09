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
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.ComponentModel.Design.Serialization;
using DevExpress.Compatibility.System.Drawing.Design;
using DevExpress.Compatibility.System.Collections.Specialized;
using DevExpress.DashboardCommon.DataProcessing;
namespace DevExpress.DashboardCommon {
	[
	DesignerSerializer(TypeNames.DataItemContainerCodeDomSerializer, TypeNames.CodeDomSerializer)
	]
	public class GridSparklineColumn : GridColumnBase {
		const string xmlShowStartEndValues = "ShowStartEndValues";
		const string sparklineValue = "SparklineValue";
		const bool DefaultShowStartEndValues = false;
		readonly SparklineOptions sparklineOptions;
		bool showStartEndValues = DefaultShowStartEndValues;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridSparklineColumnShowStartEndValues"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultShowStartEndValues)
		]
		public bool ShowStartEndValues {
			get { return showStartEndValues; }
			set {
				if(showStartEndValues != value) {
					showStartEndValues = value;
					OnChanged(ChangeReason.View, this, showStartEndValues);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridSparklineColumnMeasure"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableMeasurePropertyTypeConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public Measure Measure {
			get { return GetMeasure(sparklineValue); }
			set { SetDataItem(sparklineValue, value); }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridSparklineColumnSparklineOptions"),
#endif
		Category(CategoryNames.Data),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public SparklineOptions SparklineOptions { get { return sparklineOptions; } }
		protected internal override string DefaultName {
			get {
				Measure measure = Measure;
				return measure != null ? measure.DisplayName : string.Empty;
			}
		}
		protected internal override string ColumnId { get { return "GridSparklineColumn"; } }
		public GridSparklineColumn()
			: this(null) {
		}
		public GridSparklineColumn(Measure measure)
			: base(new DataItemDescription[] { new DataItemDescription(sparklineValue, DashboardLocalizer.GetString(DashboardStringId.DescriptionItemMeasure), measure) }) {
			sparklineOptions = new SparklineOptions(this);
		}
		protected override SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			return SummaryTypeInfo.Number;
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(showStartEndValues != DefaultShowStartEndValues)
				element.Add(new XAttribute(xmlShowStartEndValues, showStartEndValues));
			sparklineOptions.SaveToXml(element);
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			string showStartEndValuesString = XmlHelper.GetAttributeValue(element, xmlShowStartEndValues);
			if(!string.IsNullOrEmpty(showStartEndValuesString))
				showStartEndValues = XmlHelper.FromString<bool>(showStartEndValuesString);
			sparklineOptions.LoadFromXml(element);
		}
		protected internal override GridColumnType GetColumnType() {
			return GridColumnType.Sparkline;
		}
		protected internal override string GetDataId() {
			Measure measure = Measure;
			return measure != null ? measure.ActualId : null;
		}
		protected internal override IEnumerable<GridColumnTotalType> GetAvailableTotalTypes() {
			yield return GridColumnTotalType.Count;
		}
		protected internal override void PrepareViewModel(GridColumnViewModel viewModel, IDashboardDataSource dataSource, string dataMember) {
			viewModel.DisplayMode = GridColumnDisplayMode.Sparkline;
			Measure measure = Measure;
			if(measure != null) {
				viewModel.ShowStartEndValues = showStartEndValues;
				viewModel.SparklineOptions = new SparklineOptionsViewModel(sparklineOptions);
			}
			viewModel.HorzAlignment = GridColumnHorzAlignment.Left;
		}
		protected override DataItemContainer CreateInstance() {
			return new GridSparklineColumn();
		}
		protected internal override void Assign(DataItemContainer container) {
			base.Assign(container);
			GridSparklineColumn column = container as GridSparklineColumn;
			if(column != null) {
				Measure = column.Measure;
				showStartEndValues = column.ShowStartEndValues;
				sparklineOptions.Assign(column.SparklineOptions);
			}
		}
		protected internal override DataItemContainerActualContent GetActualContent() {
			DataItemContainerActualContent content = new DataItemContainerActualContent();
			if(Measure != null) {
				content.Measures.Add(Measure);
				content.SparklineValue = Measure;
			}
			content.IsSparkline = true;
			return content;
		}
		protected override MeasureDescriptorInternal CreateSummaryDescriptor(GridColumnTotal total, string id) {
			if(total.TotalType != GridColumnTotalType.Count)
				return null;
			return new MeasureDescriptorInternal() {
				ID = id,
				Name = null,
				Format = new ValueFormatViewModel()
			};
		}
		protected override SummaryAggregationModel CreateSummaryAggregationModel(GridColumnTotal total, ItemModelBuilder itemBuilder) {
			if(total.TotalType != GridColumnTotalType.Count)
				return null;
			return new SummaryAggregationModel(CreateTotalId(total), itemBuilder.GetMeasureModel(Measure), total.TotalType.ToSummaryItemTypeEx(), 0, true);
		}
	}
}
