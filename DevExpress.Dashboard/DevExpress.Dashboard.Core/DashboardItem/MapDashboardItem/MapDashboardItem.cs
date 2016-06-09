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
using DevExpress.Map.Native;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	public enum ShapefileArea {
		Custom,
		WorldCountries,
		Europe,
		Asia,
		NorthAmerica,
		SouthAmerica,
		Africa,
		USA,
		Canada
	}
	public abstract partial class MapDashboardItem : DataDashboardItem {
		internal const ShapefileArea DefaultShapefileArea = ShapefileArea.WorldCountries;
		const bool DefaultLockNavigation = false;
		const string xmlShapefileArea = "ShapefileArea";
		const string xmlCustomShapefile = "CustomShapefile";
		const string xmlViewArea = "ViewArea";
		const string xmlTooltipMeasures = "TooltipMeasures";
		const string xmlTooltipMeasure = "TooltipMeasure";
		const string xmlLockNavigation = "LockNavigation";
		const string xmlShapeTitleAttributeName = "ShapeTitleAttributeName";
		readonly CustomShapefile customShapefile;
		readonly MapViewport viewport;
		readonly MeasureCollection tooltipMeasures = new MeasureCollection();
		readonly MapLegend colorLegend;
		readonly WeightedLegend weightedLegend;
		ShapefileArea area = DefaultShapefileArea;
		MapShapeItem[] mapItems;
		bool lockNavigation = DefaultLockNavigation;
		string shapeTitleAttributeName;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("MapDashboardItemArea"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultShapefileArea)
		]
		public ShapefileArea Area {
			get { return area; }
			set {
				if(area != value) {
					area = value;
					ResetMapItems();
					OnChanged(ChangeReason.MapFile);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("MapDashboardItemCustomShapefile"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		TypeConverter(TypeNames.DisplayNameObjectConverter)
		]
		public CustomShapefile CustomShapefile { get { return customShapefile; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("MapDashboardItemInteractivityOptions"),
#endif
		Category(CategoryNames.Interactivity),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DashboardItemInteractivityOptions InteractivityOptions { get { return InteractivityOptionsBase as DashboardItemInteractivityOptions; } }
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public MapViewport Viewport { get { return viewport; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("MapDashboardItemLegend"),
#endif
		Category(CategoryNames.Layout),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public MapLegend Legend { get { return colorLegend; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("MapDashboardItemWeightedLegend"),
#endif
		Category(CategoryNames.Layout),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public WeightedLegend WeightedLegend { get { return weightedLegend; } }
		internal MapShapeItem[] MapItems {
			get {
				if(mapItems == null)
					mapItems = DashboardShapefileLoader.Load(this);
				return mapItems;
			}
		}
		internal DashboardMapItemAttribute[] MapItemAttributes {
			get {
				if(MapItems != null && MapItems.Length > 0)
					return MapItems
						.SelectMany(mapItem => mapItem.Attributes
														.Select(attr => attr as IMapItemAttribute)
														.Select(attr => new DashboardMapItemAttribute(attr.Name, attr.Type)))
						.Distinct()
						.ToArray();
				return new DashboardMapItemAttribute[0];
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("MapDashboardItemTooltipMeasures"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor)),
		]
		public MeasureCollection TooltipMeasures { get { return tooltipMeasures; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("MapDashboardItemLockNavigation"),
#endif
		Category(CategoryNames.Behavior),
		DefaultValue(DefaultLockNavigation)
		]
		public bool LockNavigation {
			get { return lockNavigation; }
			set {
				if(lockNavigation != value) {
					lockNavigation = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("MapDashboardItemShapeTitleAttributeName"),
#endif
		Category(CategoryNames.Behavior),
		DefaultValue(null),
		Localizable(false)
		]
		public string ShapeTitleAttributeName {
			get { return shapeTitleAttributeName; }
			set {
				if(shapeTitleAttributeName != value) {
					shapeTitleAttributeName = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		internal bool IsDefault { get { return Area != ShapefileArea.Custom; } }
		protected internal override string CaptionPrefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNameMapItem); } }
		protected internal override bool IsDrillDownEnabled {
			get { return false; }
			set {
			}
		}
		protected internal override DashboardItemMasterFilterMode MasterFilterMode {
			get { return InteractivityOptions.MasterFilterMode; }
			set { InteractivityOptions.MasterFilterMode = value; }
		}
		protected internal override bool TopNOthersValueEnabled { get { return false; } }
		protected internal abstract bool IsMapReady { get; }
		protected MapDashboardItem() {
			customShapefile = new CustomShapefile(this);
			viewport = new MapViewport(this);
			colorLegend = new MapLegend(this);
			weightedLegend = new WeightedLegend(this);
			tooltipMeasures.CollectionChanged += OnTooltipMeasureCollectionChanged;
		}
		protected override FilterableDashboardItemInteractivityOptions CreateInteractivityOptions() {
			return new DashboardItemInteractivityOptions(false);
		}
		protected override IEnumerable<DataDashboardItemDescription> ItemDescriptions {
			get {
				foreach(DataDashboardItemDescription description in MapDescriptions)
					yield return description;
				yield return new DataDashboardItemDescription(null, null, ItemKind.Splitter, null);
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.TooltipDataItemsHeader), null, ItemKind.Header, null);
				foreach(DataDashboardItemDescription tooltipDescription in TooltipItemDescriptions)
					yield return tooltipDescription;
			}
		}
		protected abstract IEnumerable<DataDashboardItemDescription> MapDescriptions { get; }
		protected virtual IEnumerable<DataDashboardItemDescription> TooltipItemDescriptions {
			get {
				yield return new DataDashboardItemDescription(
					DashboardLocalizer.GetString(DashboardStringId.DescriptionMeasures),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemMeasure),
					ItemKind.NumericMeasure, tooltipMeasures
				);
			}
		}
		protected override SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			if(tooltipMeasures.Contains(measure))
				return SummaryTypeInfo.Text;
			return base.GetSummaryTypeInfo(measure);
		}
		protected internal override bool IsSortingEnabled(Dimension dimension) {
			return false;
		}
		protected internal override bool CanSpecifySortMode(Dimension dimension) {
			return false;
		}
		protected internal override string[] GetSelectionDataMemberDisplayNames() {
			return IsMasterFilterEnabled ?
				SelectionDimensionList
					.Select(dimension => dimension.DisplayName)
					.ToArray() :
				new string[0];
		}
		internal void ResetMapItems() {
			mapItems = null;
			Viewport.ReCalculate();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(Area != DefaultShapefileArea)
				element.Add(new XAttribute(xmlShapefileArea, area));
			if(CustomShapefile.ShouldSerialize()) {
				XElement customShapefileElement = new XElement(xmlCustomShapefile);
				CustomShapefile.SaveToXml(customShapefileElement);
				element.Add(customShapefileElement);
			}
			if(!Viewport.IsDefault) {
				XElement viewAreaElement = new XElement(xmlViewArea);
				Viewport.SaveToXml(viewAreaElement);
				element.Add(viewAreaElement);
			}
			if(lockNavigation != DefaultLockNavigation)
				element.Add(new XAttribute(xmlLockNavigation, lockNavigation));
			if(!string.IsNullOrEmpty(ShapeTitleAttributeName))
				element.Add(new XAttribute(xmlShapeTitleAttributeName, ShapeTitleAttributeName));
			colorLegend.SaveToXml(element);
			weightedLegend.SaveToXml(element);
			tooltipMeasures.SaveToXml(element, xmlTooltipMeasures, xmlTooltipMeasure);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			string shapefileAreaAttr = XmlHelper.GetAttributeValue(element, xmlShapefileArea);
			if(!string.IsNullOrEmpty(shapefileAreaAttr))
				area = XmlHelper.EnumFromString<ShapefileArea>(shapefileAreaAttr);
			XElement customShapefileElement = element.Element(xmlCustomShapefile);
			if(customShapefileElement != null)
				CustomShapefile.LoadFromXml(customShapefileElement);
			XElement viewAreaElement = element.Element(xmlViewArea);
			if(viewAreaElement != null)
				Viewport.LoadFromXml(viewAreaElement);
			string lockNavigationString = XmlHelper.GetAttributeValue(element, xmlLockNavigation);
			if(!String.IsNullOrEmpty(lockNavigationString))
				lockNavigation = XmlHelper.FromString<bool>(lockNavigationString);
			string shapeTitleAttributeNameAttr = XmlHelper.GetAttributeValue(element, xmlShapeTitleAttributeName);
			if(!string.IsNullOrEmpty(shapeTitleAttributeNameAttr))
				shapeTitleAttributeName = shapeTitleAttributeNameAttr;
			colorLegend.LoadFromXml(element);
			weightedLegend.LoadFromXml(element);
			tooltipMeasures.LoadFromXml(element, xmlTooltipMeasures, xmlTooltipMeasure);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			mapItems = null;
			tooltipMeasures.OnEndLoading(DataItemRepository, this);
		}
		protected internal virtual IList<TooltipDataItemViewModel> CreateTooltipMeasureViewModel() {
			List<TooltipDataItemViewModel> measures = new List<TooltipDataItemViewModel>();
			foreach(Measure measure in TooltipMeasures) {
				TooltipDataItemViewModel tooltipMeasureViewModel = new TooltipDataItemViewModel(measure.DisplayName, measure.ActualId);
				PrepareTooltipMeasureViewModel(tooltipMeasureViewModel);
				measures.Add(tooltipMeasureViewModel);
			}
			return measures;
		}
		protected virtual void PrepareTooltipMeasureViewModel(TooltipDataItemViewModel tooltipMeasureViewModel) {
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			return null;
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			foreach (Measure tooltipMeasure in TooltipMeasures)
				description.AddTooltipMeasure(tooltipMeasure);
			description.Area = Area;
			description.CustomShapefile = new CustomShapefile(this);
			CustomShapefile.CopyTo(description.CustomShapefile);
			description.MapViewport = Viewport.Clone();
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			TooltipMeasures.AddRange(description.TooltipMeasures);
			Area = description.Area;
			if (description.CustomShapefile != null)
				description.CustomShapefile.CopyTo(CustomShapefile);
			if (description.MapViewport != null)
				Viewport.Apply(description.MapViewport);
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			foreach(Measure measure in TooltipMeasures)
				builder.AddMeasure(measure);
		}
		void OnTooltipMeasureCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<Measure> e) {
			OnDataItemsChanged(e.AddedItems, e.RemovedItems);
		}
		protected override void FillEditNameDescriptions(IList<EditNameDescription> descriptions) {
			base.FillEditNameDescriptions(descriptions);
			if(TooltipMeasures != null && TooltipMeasures.Count > 0)
				descriptions.Add(new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionTooltipMeasures), TooltipMeasures));
		}
	}
}
