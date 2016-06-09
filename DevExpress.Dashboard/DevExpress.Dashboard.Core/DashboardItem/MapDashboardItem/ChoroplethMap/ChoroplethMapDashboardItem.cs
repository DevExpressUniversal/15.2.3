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

using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.XtraPivotGrid;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Xml.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	[
	DashboardItemType(DashboardItemType.ChoroplethMap)
	]
	public partial class ChoroplethMapDashboardItem : MapDashboardItem, IElementContainer {
		const string xmlAttributeDimension = "AttributeDimension";
		const string xmlAttributeName = "AttributeName";
		const string xmlToolTipAttributeName = "ToolTopAttributeName";
		readonly DataItemBox<Dimension> attributeDimensionBox;
		readonly ChoroplethMapCollection maps = new ChoroplethMapCollection();
		readonly ChoroplethMapLayerElementContainer elementContainer;
		readonly AttributeDimensionHolder attributeDimensionHolder;
		string attributeName;
		string tooltipAttributeName;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChoroplethMapDashboardItemMaps"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ChoroplethMapCollectionEditor, typeof(UITypeEditor)),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public ChoroplethMapCollection Maps { get { return maps; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChoroplethMapDashboardItemAttributeDimension"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableDimensionPropertyTypeConverter),
		DefaultValue(null)
		]
		public Dimension AttributeDimension {
			get { return attributeDimensionBox.DataItem; }
			set { attributeDimensionBox.DataItem = value; }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChoroplethMapDashboardItemAttributeName"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(null),
		Localizable(false)
		]
		public string AttributeName {
			get { return attributeName; }
			set {
				if(attributeName != value) {
					attributeName = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("ChoroplethMapDashboardItemTooltipAttributeName"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(null),
		Localizable(false)
		]
		public string TooltipAttributeName {
			get { return tooltipAttributeName; }
			set {
				if(tooltipAttributeName != value) {
					tooltipAttributeName = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public new WeightedLegend WeightedLegend { get { return null; } }
		internal ChoroplethMap ActiveMap {
			get {
				ChoroplethMap map = elementContainer.ActiveElement;
				return maps.Contains(map) ? map : null;
			}
		}
		protected internal override IList<Dimension> SelectionDimensionList {
			get {
				List<Dimension> dimensionList = new List<Dimension>();
				if(AttributeDimension != null)
					dimensionList.Add(AttributeDimension);
				return dimensionList;
			}
		}
		protected internal override bool HasDataItems { get { return AttributeDimension != null || (ActiveMap != null && ActiveMap.HasDataItems) || base.HasDataItems; } }
		protected internal override bool IsMapReady { get { return AttributeDimension != null && ActiveMap != null && ActiveMap.HasAllDataItems; } }
		protected override IEnumerable<DataDashboardItemDescription> MapDescriptions {
			get {
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.MapAttributeDimension),
					DashboardLocalizer.GetString(DashboardStringId.MapAttributeDimension), ItemKind.MapAttribute, attributeDimensionHolder);
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionMaps),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemValue), ItemKind.ChoroplethMap, maps);
			}
		}
		ContentArrangementMode IElementContainer.ContentArrangementMode { get { return ContentArrangementMode.Auto; } set { } }
		int IElementContainer.ContentLineCount { get { return 0; } set { } }
		IList<string> IElementContainer.ElementNames { get { return elementContainer.ElementNames; } }
		bool IElementContainer.ElementSelectionEnabled { get { return elementContainer.ElementSelectionEnabled; } }
		int IElementContainer.SelectedElementIndex {
			get { return ActiveElementIndex; }
			set { ActiveElementIndex = value; }
		}
		int ActiveElementIndex {
			get { return elementContainer.ActiveElementIndex; }
			set { elementContainer.ActiveElementIndex = value; }
		}
		protected internal override IElementContainer ElementContainer { get { return this; } }
		public ChoroplethMapDashboardItem() {
			attributeDimensionHolder = new AttributeDimensionHolder(this);
			attributeDimensionBox = InitializeDimensionBox(attributeDimensionHolder, xmlAttributeDimension);
			maps.CollectionChanged += (sender, e) => {
				OnDataItemContainersChanged(e.AddedItems, e.RemovedItems);
				elementContainer.ValidateActiveElement();
			};
			elementContainer = new ChoroplethMapLayerElementContainer(new ReadOnlyCollection<ChoroplethMap>(maps));
			InitializeAttributeName();
		}
		bool HasDeltaMaps() {
			foreach(ChoroplethMap map in maps) {
				if(map is DeltaMap)
					return true;
			}
			return false;
		}
		protected internal override void PrepareState(DashboardItemState state) {
			base.PrepareState(state);
			int activeElementIndex = ActiveElementIndex;
			if(activeElementIndex != -1)
				state.ActiveElementState = activeElementIndex;
		}
		protected internal override void SetState(DashboardItemState state) {
			base.SetState(state);
			ActiveElementIndex = -1;
			int activeElementState = state.ActiveElementState;
			if(activeElementState != -1)
				ActiveElementIndex = activeElementState;
		}
		internal void RaiseChangedAttributeName() {
			OnChanged(ChangeReason.View);
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			return new ChoroplethMapDashboardItemViewModel(this, CreateTooltipMeasureViewModel());
		}
		protected override void PrepareTooltipMeasureViewModel(TooltipDataItemViewModel tooltipMeasureViewModel) {
			base.PrepareTooltipMeasureViewModel(tooltipMeasureViewModel);
			tooltipMeasureViewModel.AttributeName = attributeName;
		}
		internal void InitializeAttributeName() {
			LockChanging();
			if(AttributeName == null)
				AttributeName = MapItemAttributes.Length > 0 ? MapItemAttributes[0].Name : null;
			UnlockChanging();
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			attributeDimensionBox.SaveToXml(element);
			if(!string.IsNullOrEmpty(AttributeName))
				element.Add(new XAttribute(xmlAttributeName, AttributeName));
			if(!string.IsNullOrEmpty(TooltipAttributeName))
				element.Add(new XAttribute(xmlToolTipAttributeName, TooltipAttributeName));
			Maps.SaveToXml(element);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			attributeDimensionBox.LoadFromXml(element);
			string attributeNameAttr = XmlHelper.GetAttributeValue(element, xmlAttributeName);
			if(!string.IsNullOrEmpty(attributeNameAttr))
				attributeName = attributeNameAttr;
			string toolTipAttributeNameAttr = XmlHelper.GetAttributeValue(element, xmlToolTipAttributeName);
			if(!string.IsNullOrEmpty(toolTipAttributeNameAttr))
				tooltipAttributeName = toolTipAttributeNameAttr;
			Maps.LoadFromXml(element);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			attributeDimensionBox.OnEndLoading();
			Maps.OnEndLoading(this);
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			DimensionDescriptorInternalCollection dimensions = new DimensionDescriptorInternalCollection();
			if(ActiveMap != null) {
				ChoroplethMap map = ActiveMap;
				DataItemContainerActualContent content = map.GetActualContent();
				List<Measure> containerMeasures = content.Measures;
				foreach(Measure measure in containerMeasures) {
					builder.AddMeasure(measure);
				}
				if(content.IsDelta) {
					Measure actualValue = content.DeltaActualValue;
					Measure targetValue = content.DeltaTargetValue;
					if(actualValue != null && targetValue != null) {
						builder.AddDeltaDescriptor(actualValue, targetValue, map, content.DeltaOptions);
					}
				}
			}
			if(AttributeDimension != null)
				builder.SetColumnHierarchyDimensions(DashboardDataAxisNames.DefaultAxis, new Dimension[] { AttributeDimension });
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			foreach (ChoroplethMap map in Maps) {
				DeltaMap deltaMap = map as DeltaMap;
				if (deltaMap != null)
					description.AddMeasure(deltaMap.ActualValue, deltaMap.TargetValue, deltaMap.DeltaOptions);
				else
					foreach (Measure measure in map.Measures)
						description.AddMeasure(measure);
			}
			description.Legend = Legend;
			description.AddMainDimension(AttributeDimension);
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			AssignDimension(description.Latitude, HiddenDimensions);
			AssignDimension(description.Longitude, HiddenDimensions);
			HiddenDimensions.AddRange(description.MainDimensions);
			HiddenDimensions.AddRange(description.AdditionalDimensions);
			AssignDimension(description.SparklineArgument, HiddenDimensions);
			HiddenDimensions.AddRange(description.TooltipDimensions);
			Legend.CopyFrom(description.Legend);
			foreach (MeasureDescription measureBox in description.MeasureDescriptions)
				if (measureBox.MeasureType == MeasureDescriptionType.Delta) {
					DeltaMap deltaMap = new DeltaMap(measureBox.ActualValue, measureBox.TargetValue);
					deltaMap.DeltaOptions.Assign(measureBox.DeltaOptions);
					Maps.Add(deltaMap);
				}
				else
					Maps.Add(new ValueMap(measureBox.Value));
		}
		protected override DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo(Dimension dimension) {
			if(dimension == AttributeDimension)
				return DimensionGroupIntervalInfo.Empty;
			return base.GetDimensionGroupIntervalInfo(dimension);
		}
		protected override void FillEditNameDescriptions(IList<EditNameDescription> descriptions) {
			base.FillEditNameDescriptions(descriptions);
			maps.ForEach(map => descriptions.Add(map.GetEditNameDescription()));
		}
	}
	public class ChoroplethMapCollection : DataItemContainerCollection<ChoroplethMap> {
		public ChoroplethMapCollection() {
			XmlSerializer = new RepositoryItemListXmlSerializer<ChoroplethMap>("Maps", XmlRepository.ChoroplethMapRepository);
		}
	}
}
