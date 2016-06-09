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
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public enum GridColumnFixedWidthType { Weight, FitToContent, FixedWidth }
	public abstract class GridColumnBase : NamedDataItemContainer {
		const string xmlWeight = "Weight";
		const string xmlFixedWidth = "FixedWidth";
		const string xmlWidthType = "WidthType";
		const double DefaultWeight = 75;
		const double DefaultFixedWidth = 0;
		const GridColumnFixedWidthType DefaultWidthType = GridColumnFixedWidthType.Weight;
		const double DefaultBestCharacterCount = 18;
		static protected IEnumerable<GridColumnTotalType> GetTotalTypes(DataItem dataitem) {
			DataFieldType fieldType = dataitem != null ? dataitem.ActualDataFieldType : DataFieldType.Unknown;
			switch(fieldType) {
				case DataFieldType.Integer:
				case DataFieldType.Float:
				case DataFieldType.Double:
				case DataFieldType.Decimal:
					yield return GridColumnTotalType.Min;
					yield return GridColumnTotalType.Max;
					yield return GridColumnTotalType.Avg;
					yield return GridColumnTotalType.Sum;
					yield return GridColumnTotalType.Count;
					break;
				case DataFieldType.DateTime:
				case DataFieldType.Enum:
				case DataFieldType.Text:
					yield return GridColumnTotalType.Min;
					yield return GridColumnTotalType.Max;
					yield return GridColumnTotalType.Count;
					break;
				default:
					yield return GridColumnTotalType.Count;
					break;
			}
		}
		readonly PrefixNameGenerator totalMeasureIdGenerator = new PrefixNameGenerator("Total");
		readonly OrderedDictionary<GridColumnTotal, string> totalMeasureIds = new OrderedDictionary<GridColumnTotal, string>();
		readonly GridColumnTotalCollection totals = new GridColumnTotalCollection();
		double weight = 0;
		double fixedWidth = DefaultFixedWidth;
		GridColumnFixedWidthType widthType = DefaultWidthType;
		protected internal abstract string ColumnId { get; }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridColumnBaseWeight"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultWeight)
		]
		public double Weight {
			get { return weight != 0 ? weight : DefaultWeight; }
			set {
				if(weight != value && value > 0) {
					weight = value;
					OnChanged(ChangeReason.View, this, weight);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridColumnBaseFixedWidth"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultFixedWidth)
		]
		public double FixedWidth {
			get { return fixedWidth; }
			set {
				if(fixedWidth != value && value >= 0) {
					fixedWidth = value;
					OnChanged(ChangeReason.View, this, fixedWidth);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridColumnBaseWeight"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultWeight)
		]
		public GridColumnFixedWidthType WidthType {
			get { return widthType; }
			set {
				if(widthType != value) {
					widthType = value;
					OnChanged(ChangeReason.View, this, widthType);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GridColumnBaseTotals"),
#endif
 Category(CategoryNames.Data), DefaultValue(null),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
		public GridColumnTotalCollection Totals { get { return totals; } }
		protected GridColumnBase(IEnumerable<DataItemDescription> dataItemDescriptions)
			: base(dataItemDescriptions) {
			totals.CollectionChanged += OnTotalsCollectionChanged;
		}
		void OnTotalsCollectionChanged(object sender, DataAccess.NotifyingCollectionChangedEventArgs<GridColumnTotal> e) {
			foreach(GridColumnTotal removedItem in e.RemovedItems) {
				removedItem.Changed -= OnTotalChanged;
				totalMeasureIds.Remove(removedItem);
			}
			foreach(GridColumnTotal addedItem in e.AddedItems) {
				addedItem.Changed += OnTotalChanged;
				totalMeasureIds.Add(addedItem, totalMeasureIdGenerator.GenerateNameFromStart(ContainsId));
			}
			OnChanged(ChangeReason.ClientData, this, null);
		}
		bool ContainsId(string id) {
			return totalMeasureIds.Values.Contains(id);
		}
		void OnTotalChanged(object sender, EventArgs e) {
			OnChanged(ChangeReason.ClientData, this, null);
		}
		protected internal abstract GridColumnType GetColumnType();
		protected internal abstract string GetDataId();
		protected string CreateTotalId(GridColumnTotal total) {
			return string.Format("{0}_{1}", GetDataId(), totalMeasureIds[total]);
		}
		protected internal GridColumnViewModel CreateViewModel(IDashboardDataSource dataSource, string dataMember) {
			if(DataItemsCount == 0)
				return null;
			string columnDataId = GetDataId();
			GridColumnViewModel viewModel = new GridColumnViewModel() {
				Caption = DisplayName,
				ColumnType = GetColumnType(),
				DataId = columnDataId,
				Weight = Weight,
				FixedWidth = FixedWidth,
				WidthType = WidthType,
				ActualIndex = ((GridDashboardItem)Context).Columns.IndexOf(this),
				DefaultBestCharacterCount = DefaultBestCharacterCount
			};
			PrepareViewModel(viewModel, dataSource, dataMember);
			foreach(GridColumnTotal total in totals)
				viewModel.Totals.Add(CreateTotalViewModel(total));
			return viewModel;
		}
		GridColumnTotalViewModel CreateTotalViewModel(GridColumnTotal total) {
			string dataId;
			string caption;
			GridColumnTotalType totalType = total.TotalType;
			if(totalType == GridColumnTotalType.Auto) {
				dataId = GetDataId();
				caption = DashboardLocalizer.GetString(DashboardStringId.GridTotalAutoTemplate);
			}
			else {
				dataId = CreateTotalId(total);
				DashboardStringId id;
				switch(totalType) {
					case GridColumnTotalType.Min:
						id = DashboardStringId.GridTotalTypeMin;
						break;
					case GridColumnTotalType.Max:
						id = DashboardStringId.GridTotalTypeMax;
						break;
					case GridColumnTotalType.Avg:
						id = DashboardStringId.GridTotalTypeAvg;
						break;
					case GridColumnTotalType.Sum:
						id = DashboardStringId.GridTotalTypeSum;
						break;
					default:
						id = DashboardStringId.GridTotalTypeCount;
						break;
				}
				caption = string.Format(DashboardLocalizer.GetString(DashboardStringId.GridTotalTemplate),
					DashboardLocalizer.GetString(id), DashboardLocalizer.GetString(DashboardStringId.GridTotalValueTemplate));
			}
			return new GridColumnTotalViewModel() {
				DataId = dataId,
				TotalType = totalType,
				Caption = caption
			};
		}
		protected internal abstract IEnumerable<GridColumnTotalType> GetAvailableTotalTypes();
		protected internal abstract void PrepareViewModel(GridColumnViewModel viewModel, IDashboardDataSource dataSource, string dataMember);
		protected internal virtual void FillDashboardItemDataDescription(DashboardItemDataDescription description){
			foreach (Measure measure in Measures)
				description.AddMeasure(measure);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			totals.SaveToXml(element);
			if(weight != DefaultWeight)
				element.Add(new XAttribute(xmlWeight, weight));
			if(fixedWidth != DefaultFixedWidth)
				element.Add(new XAttribute(xmlFixedWidth, fixedWidth));
			if(widthType != DefaultWidthType)
				element.Add(new XAttribute(xmlWidthType, widthType));
		}
		protected internal override void LoadFromXml(XElement element) {
			base.LoadFromXml(element);
			totals.LoadFromXml(element);
			string fixedWidthString = XmlHelper.GetAttributeValue(element, xmlFixedWidth);
			if(!String.IsNullOrEmpty(fixedWidthString)) {
				double initialFixedWidth = XmlHelper.FromString<double>(fixedWidthString);
				if(initialFixedWidth > 0)
					fixedWidth = initialFixedWidth;
			}
			string widthTypeString = XmlHelper.GetAttributeValue(element, xmlWidthType);
			if(!string.IsNullOrEmpty(widthTypeString))
				widthType = XmlHelper.FromString<GridColumnFixedWidthType>(widthTypeString);
			string weightString = XmlHelper.GetAttributeValue(element, xmlWeight);
			if(!String.IsNullOrEmpty(weightString)) {
				double initialWeight = XmlHelper.FromString<double>(weightString);
				if(initialWeight > 0)
					weight = initialWeight;
			}
		}
		protected override void OnDataItemContainerContextChanged() {
			base.OnDataItemContainerContextChanged();
			if(weight != 0)
				return;
			GridDashboardItem grid = Context as GridDashboardItem;
			if(grid != null) {
				double weightSum = 0;
				GridColumnCollection columns = grid.Columns;
				if(columns.Count > 1) {
					foreach(GridColumnBase col in columns)
						if(!col.Equals(this))
							weightSum += col.Weight;
					weight = weightSum / (columns.Count - 1);
				}
				else
					weight = DefaultWeight;
			}
		}
		protected virtual MeasureDescriptorInternal CreateSummaryDescriptor(GridColumnTotal total, string id) {
			return null;
		}
		protected virtual SummaryAggregationModel CreateSummaryAggregationModel(GridColumnTotal total, ItemModelBuilder itemBuilder) {
			return null;
		}
		internal void AddColumnSummaryDescriptors(MeasureDescriptorInternalCollection descriptors) {
			if(DataItemsCount == 0)
				return;
			foreach(GridColumnTotal total in totals) {
				MeasureDescriptorInternal descriptor = CreateSummaryDescriptor(total, CreateTotalId(total));
				if(descriptor != null)
					descriptors.Add(descriptor);
			}
		}
		internal IEnumerable<SummaryAggregationModel> GetGetSummaryAggregationModels(ItemModelBuilder itemBuilder) {
			if(DataItemsCount == 0)
				yield break;
			foreach(GridColumnTotal total in totals) {
				SummaryAggregationModel model = CreateSummaryAggregationModel(total, itemBuilder);
				if(model != null)
					yield return model;
			}
		}
	}
}
