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
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public abstract partial class GeoPointMapDashboardItemBase : MapDashboardItem, IRaiseClusterizationRequestItem {
		const string xmlLatitude = "Latitude";
		const string xmlLongitude = "Longitude";
		const string xmlEnableClustering = "EnableClustering";
		const string xmlTooltipDimensions = "TooltipDimensions";
		const string xmlTooltipDimension = "TooltipDimension";
		const bool DefaultEnableClustering = false;
		internal const string LatitudeFieldName = "Latitude";
		internal const string LongitudeFieldName = "Longitude";
		internal const string PointsCountFieldName = "PointsCount";
		internal const string PointsCountDataId = "PointsCountId";
		readonly DataItemBox<Dimension> latitudeBox;
		readonly DataItemBox<Dimension> longitudeBox;
		readonly LatitudeHolder latitudeHolder;
		readonly LongitudeHolder longitudeHolder;
		readonly DimensionCollection tooltipDimensions = new DimensionCollection();
		bool enableClustering = DefaultEnableClustering;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GeoPointMapDashboardItemBaseLatitude"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableDimensionPropertyTypeConverter),
		DefaultValue(null)
		]
		public Dimension Latitude {
			get { return latitudeBox.DataItem; }
			set { latitudeBox.DataItem = value; }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GeoPointMapDashboardItemBaseLongitude"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableDimensionPropertyTypeConverter),
		DefaultValue(null)
		]
		public Dimension Longitude {
			get { return longitudeBox.DataItem; }
			set { longitudeBox.DataItem = value; }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GeoPointMapDashboardItemBaseTooltipDimensions"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor)),
		]
		public DimensionCollection TooltipDimensions { get { return tooltipDimensions; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("GeoPointMapDashboardItemBaseEnableClustering"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultEnableClustering)
		]
		public bool EnableClustering {
			get { return enableClustering; }
			set {
				if(enableClustering != value) {
					enableClustering = value;
					OnChanged(ChangeReason.ClientData);
				}
			}
		}
		protected internal override IList<Dimension> SelectionDimensionList {
			get {
				List<Dimension> dimensionList = new List<Dimension>();
				if(Latitude != null)
					dimensionList.Add(Latitude);
				if(Longitude != null)
					dimensionList.Add(Longitude);
				return dimensionList;
			}
		}
		protected internal override bool HasDataItems { get { return Latitude != null || Longitude != null || base.HasDataItems; } }
		protected override IEnumerable<DataDashboardItemDescription> MapDescriptions {
			get {
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.Latitude),
					DashboardLocalizer.GetString(DashboardStringId.Latitude), ItemKind.SingleDimension, latitudeHolder);
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.Longitude),
					DashboardLocalizer.GetString(DashboardStringId.Longitude), ItemKind.SingleDimension, longitudeHolder);
			}
		}
		protected override IEnumerable<DataDashboardItemDescription> TooltipItemDescriptions {
			get {
				yield return new DataDashboardItemDescription(
					DashboardLocalizer.GetString(DashboardStringId.DescriptionDimensions),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemDimension),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionDimensions),
					ItemKind.Dimension, tooltipDimensions
				);
				foreach(DataDashboardItemDescription description in base.TooltipItemDescriptions)
					yield return description;
			}
		}
		protected internal override bool IsMapReady { get { return Latitude != null && Longitude != null; } }
		internal string LatitudeDataMember { get { return GetGeoPointDataMember(Latitude, LatitudeFieldName, enableClustering); } }
		internal string LongitudeDataMember { get { return GetGeoPointDataMember(Longitude, LongitudeFieldName, enableClustering); } }
		internal string PointsCountDataMember { get { return Helper.GetValuePrefixNameGenerator(PointsCountFieldName).GenerateName(); } }
		protected GeoPointMapDashboardItemBase() {
			latitudeHolder = new LatitudeHolder(this);
			longitudeHolder = new LongitudeHolder(this);
			latitudeBox = InitializeDimensionBox(latitudeHolder, xmlLatitude);
			longitudeBox = InitializeDimensionBox(longitudeHolder, xmlLongitude);
			tooltipDimensions.CollectionChanged += OnTooltipDimensionCollectionChanged;
		}
		void IRaiseClusterizationRequestItem.RaiseClusterizationRequest() {
		}
		void IRaiseClusterizationRequestItem.CreateClusteringData() {
		}
		void IRaiseClusterizationRequestItem.UpdateMapInfo(MapClusterizationRequestInfo info) {
			currentMapInfo = info;
		}	   
		string GetGeoPointDataMember(Dimension dimension, string dimensionFieldName, bool enableClustering) {
			if(enableClustering)
				return dimensionFieldName;
			if(dimension != null)
				return dimension.ActualId;
			return null;
		}
		protected internal override bool CanSpecifyDimensionNumericFormat(Dimension dimension) {
			return tooltipDimensions.Contains(dimension) && base.CanSpecifyDimensionNumericFormat(dimension);
		}
		protected internal override IList<TooltipDataItemViewModel> CreateTooltipMeasureViewModel() {
			List<TooltipDataItemViewModel> measures = new List<TooltipDataItemViewModel>();
			foreach(Measure measure in TooltipMeasures) {
				TooltipDataItemViewModel tooltipMeasureViewModel = new TooltipDataItemViewModel(measure.DisplayName, measure.ActualId);
				PrepareTooltipMeasureViewModel(tooltipMeasureViewModel);
				measures.Add(tooltipMeasureViewModel);
			}
			return measures;
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			latitudeBox.SaveToXml(element);
			longitudeBox.SaveToXml(element);
			if(enableClustering != DefaultEnableClustering)
				element.Add(new XAttribute(xmlEnableClustering, enableClustering));
			tooltipDimensions.SaveToXml(element, xmlTooltipDimensions, xmlTooltipDimension);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			latitudeBox.LoadFromXml(element);
			longitudeBox.LoadFromXml(element);
			string enableClusteringString = XmlHelper.GetAttributeValue(element, xmlEnableClustering);
			if(!String.IsNullOrEmpty(enableClusteringString))
				enableClustering = XmlHelper.FromString<bool>(enableClusteringString);
			tooltipDimensions.LoadFromXml(element, xmlTooltipDimensions, xmlTooltipDimension);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			latitudeBox.OnEndLoading();
			longitudeBox.OnEndLoading();
			tooltipDimensions.OnEndLoading(DataItemRepository, this);
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			List<Dimension> dimensions = new List<Dimension>();
			if(Latitude != null)
				dimensions.Add(Latitude);
			if(Longitude != null)
				dimensions.Add(Longitude);
			AddDimensionsBeforeTooltip(dimensions);
			foreach(Dimension dimension in TooltipDimensions) {
				dimensions.Add(dimension);
			}
			if(EnableClustering) {
				builder.MeasureDescriptors.Add(new MeasureDescriptorInternal() {
					ID = PointsCountDataId,
					Format = new ValueFormatViewModel()
				});
			}
			builder.SetColumnHierarchyDimensions(DashboardDataAxisNames.DefaultAxis, dimensions);
		}
		protected virtual void AddDimensionsBeforeTooltip(List<Dimension> dimensions) { }
		void OnTooltipDimensionCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<Dimension> e) {
			OnDataItemsChanged(e.AddedItems, e.RemovedItems);
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			foreach (Dimension tooltipDimension in TooltipDimensions)
				description.AddTooltipDimension(tooltipDimension);
			description.EnableClustering = EnableClustering;
			description.AddLatitude(Latitude);
			description.AddLongitude(Longitude);
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			TooltipDimensions.AddRange(description.TooltipDimensions);
			EnableClustering = description.EnableClustering;
			Latitude = description.Latitude;
			Longitude = description.Longitude;
			HiddenDimensions.AddRange(description.MainDimensions);
			HiddenDimensions.AddRange(description.AdditionalDimensions);
			AssignDimension(description.SparklineArgument, HiddenDimensions);
		}
		protected internal virtual void FillClientDataDataMembers(InternalMapDataMembersContainer dataMembers) {
			dataMembers.AddLatitude(Latitude.ActualId);
			dataMembers.AddLongitude(Longitude.ActualId);
			dataMembers.AddPointsCount(PointsCountDataId);
			for(int i = 0; i < TooltipMeasures.Count; i++) 
				dataMembers.AddMeasure(TooltipMeasures[i].ActualId);
			for(int i = 0; i < TooltipDimensions.Count; i++) 
				dataMembers.AddDimension(TooltipDimensions[i].ActualId, DataSourceHelper.GetDimensionType(DataSource, DataMember, TooltipDimensions[i]));
		}
		protected internal IList<TooltipDataItemViewModel> CreateTooltipDimensionViewModel() {
			List<TooltipDataItemViewModel> columns = new List<TooltipDataItemViewModel>();
			for(int i = 0; i < tooltipDimensions.Count; i++) 
				columns.Add(new TooltipDataItemViewModel(tooltipDimensions[i].DisplayName, tooltipDimensions[i].ActualId));
			return columns;
		}
		protected override DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo(Dimension dimension) {
			if(dimension == Latitude || dimension == Longitude)
				return DimensionGroupIntervalInfo.Empty;
			else if(tooltipDimensions.Contains(dimension))
				return DimensionGroupIntervalInfo.Default;
			return base.GetDimensionGroupIntervalInfo(dimension);
		}
		protected internal override bool CanSpecifyTopNOptions(Dimension dimension) {
			return !tooltipDimensions.Contains(dimension) && base.CanSpecifyTopNOptions(dimension);
		}
		protected override void FillEditNameDescriptions(IList<EditNameDescription> descriptions) {
			base.FillEditNameDescriptions(descriptions);
			if(TooltipDimensions != null && TooltipDimensions.Count > 0)
				descriptions.Add(new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionTooltipDimensions), TooltipDimensions));
		}
	}
	public interface IRaiseClusterizationRequestItem {
		void RaiseClusterizationRequest();
		void CreateClusteringData();
		void UpdateMapInfo(MapClusterizationRequestInfo info);
	}
}
