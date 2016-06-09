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
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public abstract class FilterElementDashboardItem : DataDashboardItem {
		const string xmlDimensions = "FilterDimensions";
		const string xmlDimension = "Dimension";
		const string xmlShowAllValue = "ShowAllValue";
		const bool DefaultShowAllValue = true;
		DimensionCollection filterDimensions = new DimensionCollection();
		bool showAllValue = DefaultShowAllValue;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("FilterElementDashboardItemFilterDimensions"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor))
		]
		public DimensionCollection FilterDimensions { get { return filterDimensions; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("FilterElementDashboardItemShowAllValue"),
#endif
		DefaultValue(DefaultShowAllValue)
		]
		public bool ShowAllValue {
			get { return showAllValue; }
			set {
				if(showAllValue != value)
					showAllValue = value;
				OnElementTypeChanged();
			}
		}
		internal virtual bool ActualShowAllValue { get { return ShowAllValue; } }
		internal bool UseCriteriaOptimization {
			get {
				bool topNEnabled = Dimensions.Any(d => d.TopNOptions.Enabled);
				return String.IsNullOrEmpty(FilterString)
					&& InteractivityOptionsBase.IgnoreMasterFilters
					&& !IsMasterFilterCrossDataSource
					&& !topNEnabled;
			}
		}
		protected override IEnumerable<DataDashboardItemDescription> ItemDescriptions {
			get {
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionDimensions),
				  DashboardLocalizer.GetString(DashboardStringId.DescriptionItemDimension), ItemKind.Dimension, FilterDimensions);
			}
		}
		protected internal override DashboardItemMasterFilterMode MasterFilterMode { get { return DashboardItemMasterFilterMode.Multiple; } set { } }
		protected internal override bool IsDrillDownEnabled { get { return false; } set { } }
		protected internal override bool HasDataItems { get { return base.HasDataItems || FilterDimensions.Count > 0; } }
		protected override int PriorityInsideFilterLevel { get { return 1; } }
		protected override bool AllowEmptyFilterValues { get { return true; } }
		protected internal override IList<Dimension> SelectionDimensionList { get { return FilterDimensions; } }
		protected internal override bool TopNOthersValueEnabled { get { return false; } }
		protected internal abstract bool IsSingleSelection { get; }
		protected FilterElementDashboardItem() {
			filterDimensions.CollectionChanged += (sender, e) => OnDataItemsChanged(e.AddedItems, e.RemovedItems);
		}
		protected abstract void AssignFilterElementType(bool multiple);
		protected internal override string[] GetSelectionDataMemberDisplayNames() {
			return FilterDimensions.Select(d => d.DisplayName).ToArray();
		}
		protected override FilterableDashboardItemInteractivityOptions CreateInteractivityOptions() {
			return new FilterableDashboardItemInteractivityOptions(true);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			filterDimensions.SaveToXml(element, xmlDimensions, xmlDimension);
			XmlHelper.Save(element, xmlShowAllValue, showAllValue, DefaultShowAllValue);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			filterDimensions.LoadFromXml(element, xmlDimensions, xmlDimension);
			XmlHelper.Load<bool>(element, xmlShowAllValue, x => showAllValue = x);
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			foreach(Dimension dimension in filterDimensions)
				description.AddMainDimension(dimension);
			description.FilterElementShowAllValue = showAllValue;
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			AssignDimension(description.SparklineArgument, HiddenDimensions);
			AssignDimension(description.Latitude, HiddenDimensions);
			AssignDimension(description.Longitude, HiddenDimensions);
			FilterDimensions.AddRange(description.MainDimensions);
			FilterDimensions.AddRange(description.AdditionalDimensions);
			HiddenDimensions.AddRange(description.TooltipDimensions);
			HiddenMeasures.AddRange(description.TooltipMeasures);
			HiddenMeasures.AddRange(description.Measures);
			if(description.FilterElementType != FilterElementTypeDescription.None)
				AssignFilterElementType(description.FilterElementType == FilterElementTypeDescription.Checked);
			else {
				DashboardItemInteractivityOptions interactivityOptions = description.InteractivityOptions as DashboardItemInteractivityOptions;
				if(interactivityOptions != null && interactivityOptions.MasterFilterMode != DashboardItemMasterFilterMode.None)
					AssignFilterElementType(interactivityOptions.MasterFilterMode == DashboardItemMasterFilterMode.Multiple);
			}
			ShowAllValue = description.FilterElementShowAllValue;
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			filterDimensions.OnEndLoading(DataItemRepository, this);
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			builder.SetColumnHierarchyDimensions(DashboardDataAxisNames.DefaultAxis, filterDimensions);
		}
		protected override DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo(Dimension dimension) {
			return DimensionGroupIntervalInfo.Default;
		}
		protected void OnElementTypeChanged() {
			OnChanged(ChangeReason.Interactivity);
		}
		protected override SliceDataQuery GetDataQueryInternal(IActualParametersProvider provider) {
			SliceDataQueryBuilder queryBuilder;
			ItemModelBuilder itemBuilder = new ItemModelBuilder(DataSourceModel.DataSourceInfo, GetDataItemUniqueName, provider);
			if(IsBackCompatibilityDataSlicesRequired) {
				queryBuilder = SliceDataQueryBuilder.CreateWithPivotModel(itemBuilder, FilterDimensions, new Dimension[0],
					HiddenMeasures, QueryFilterDimensions, GetQueryFilterCriteria(provider));
			} else {
				queryBuilder = SliceDataQueryBuilder.CreateEmpty(itemBuilder, QueryFilterDimensions, GetQueryFilterCriteria(provider));
				queryBuilder.AddSlice(FilterDimensions, new Measure[0]);
				queryBuilder.SetAxes(FilterDimensions, new Dimension[0]);
			}
			return queryBuilder.FinalQuery();
		}
		protected override IEnumerable<Measure> GetQueryVisibleMeasures() {
			return new Measure[0];
		}
	}
}
