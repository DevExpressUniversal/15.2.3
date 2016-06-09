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
using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
using DevExpress.Data;
namespace DevExpress.DashboardCommon {
	[
	DashboardItemType(DashboardItemType.PieMap)
	]
	public class PieMapDashboardItem : GeoPointMapDashboardItemBase, IPieContext , IElementContainer {
		const string xmlArgument = "Argument";
		const string xmlIsWeighted = "IsWeighted";
		internal const string PieArgumentFieldName = "PieArgument";
		internal const string PieArgumentValueFieldName = "PieArgumentValue";
		const bool DefaultIsWeighted = true;
		readonly PieMapArgumentHolder argumentHolder;
		readonly DataItemBox<Dimension> argumentBox;
		readonly PieInternal pie;
		bool isWeighted = DefaultIsWeighted;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PieMapDashboardItemArgument"),
#endif
		Category(CategoryNames.Data),
		Editor(TypeNames.ListSelectorEditor, typeof(UITypeEditor)),
		TypeConverter(TypeNames.CreatableDimensionPropertyTypeConverter),
		DefaultValue(null)
		]
		public Dimension Argument {
			get { return argumentBox.DataItem; }
			set { argumentBox.DataItem = value; }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PieMapDashboardItemValues"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor))
		]
		public MeasureCollection Values { get { return pie.Values; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PieMapDashboardItemIsWeighted"),
#endif
		Category(CategoryNames.Data),
		DefaultValue(DefaultIsWeighted)
		]
		public bool IsWeighted {
			get { return isWeighted; }
			set {
				if(isWeighted != value) {
					isWeighted = value;
					OnChanged(ChangeReason.View);
				}
			}
		}
		protected internal override IElementContainer ElementContainer { get { return pie; } }
		internal Measure SelectedValue { get { return pie.ActiveElement; } }
		protected internal override bool IsMapReady { get { return base.IsMapReady && (Argument != null || Values.Count > 0); } }
		protected override IEnumerable<DataDashboardItemDescription> MapDescriptions {
			get {
				foreach(DataDashboardItemDescription description in base.MapDescriptions)
					yield return description;
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionValues),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemValue), ItemKind.NumericMeasure, Values);
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionItemArgument),
				  DashboardLocalizer.GetString(DashboardStringId.DescriptionItemArgument), ItemKind.SingleDimension, argumentHolder);
			}
		}
		IList<Dimension> IPieContext.Arguments { get { return Argument != null ? new Dimension[] { Argument } : null; } }
		IChangeService IPieContext.ChangeService { get { return this; } }
		IDataItemRepositoryProvider IPieContext.DataItemRepositoryProvider { get { return this; } }
		bool IPieContext.AllowLayers { get { return Latitude != null && Longitude != null; } }
		ContentArrangementMode IPieContext.DefaultContentArrangementMode { get { return ContentArrangementMode.Auto; } }
		int IPieContext.DefaultContentLineCount { get { return 0; } }
		bool IPieContext.ResetActiveElementBeforeDataItemChanged { get { return true; } }
		void IPieContext.OnDataItemsChanged(IEnumerable<DataItem> addedDataItems, IEnumerable<DataItem> removedDataItems) {
			OnDataItemsChanged(addedDataItems, removedDataItems);
		}
		IEnumerable<Measure> ActiveElements {
			get {
				if(pie.ElementSelectionEnabled)
					return new[] { pie.ActiveElement };
				return Values;
			}
		}
		int ActiveElementIndex {
			get { return pie.ActiveElementIndex; }
			set { pie.ActiveElementIndex = value; }
		}
		public PieMapDashboardItem()
			: base() {
			argumentHolder = new PieMapArgumentHolder(this);
			argumentBox = InitializeDimensionBox(argumentHolder, xmlArgument);
			pie = new PieInternal(this);
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			return new PieMapDashboardItemViewModel(this);
		}
		protected override SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			SummaryTypeInfo summaryTypeInfo = pie.GetSummaryTypeInfo(measure);
			if(summaryTypeInfo != null)
				return summaryTypeInfo;
			return base.GetSummaryTypeInfo(measure);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			argumentBox.SaveToXml(element);
			pie.SaveToXml(element);
			if(isWeighted != DefaultIsWeighted)
				element.Add(new XAttribute(xmlIsWeighted, isWeighted));
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			argumentBox.LoadFromXml(element);
			pie.LoadFromXml(element);
			string isWeightedString = XmlHelper.GetAttributeValue(element, xmlIsWeighted);
			if(!String.IsNullOrEmpty(isWeightedString))
				isWeighted = XmlHelper.FromString<bool>(isWeightedString);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			argumentBox.OnEndLoading();
			pie.OnEndLoading(this);
		}
		internal string GetValueName(Measure value) {
			return value.DisplayName;
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			description.AddAdditionalDimension(Argument);
			description.Legend = Legend;
			description.WeightedLegend = WeightedLegend;
			foreach(Measure value in pie.Values)
				description.AddMeasure(value);
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			Legend.CopyFrom(description.Legend);
			WeightedLegend.CopyFrom(description.WeightedLegend);
			foreach(Measure measure in description.Measures)
				pie.Values.Add(measure);
		}
		protected internal override void FillClientDataDataMembers(InternalMapDataMembersContainer dataMembers) {
			base.FillClientDataDataMembers(dataMembers);
			Dimension argument = Argument;
			if(argument != null) 
				dataMembers.AddPieArgument(argument.ActualId, DataSourceHelper.GetDimensionType(DataSource, DataMember, argument));
			for(int i = 0; i < Values.Count; i++) {
				Measure value = Values[i];
				dataMembers.AddMeasure(value.ActualId);
			}
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			foreach(Measure measure in ActiveElements)
				builder.AddMeasure(measure);
		}
		protected override void AddDimensionsBeforeTooltip(List<Dimension> dimensions) {
			if(Argument != null)
				dimensions.Add(Argument);
		}
		protected override DimensionGroupIntervalInfo GetDimensionGroupIntervalInfo(Dimension dimension) {
			if(dimension == Argument)
				return DimensionGroupIntervalInfo.Default;
			return base.GetDimensionGroupIntervalInfo(dimension);
		}
		protected override void FillEditNameDescriptions(IList<EditNameDescription> descriptions) {
			base.FillEditNameDescriptions(descriptions);
			descriptions.Add(pie.GetEditNameDescription());
		}
		protected override IEnumerable<Dimension> QueryDimensions {
			get {
				if(Argument == null)
					return base.QueryDimensions;
				else
					return base.QueryGeoPointDimensions.Concat(new[] { Argument }.Concat(TooltipDimensions.NotNull()));
			}
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
		#region IElementContainer implementation
		ContentArrangementMode IElementContainer.ContentArrangementMode {
			get { return pie.ContentArrangementMode; }
			set { pie.ContentArrangementMode = value; }
		}
		int IElementContainer.ContentLineCount {
			get { return pie.ContentLineCount; }
			set { pie.ContentLineCount = value; }
		}
		IList<string> IElementContainer.ElementNames {
			get { return pie.ElementNames; }
		}
		bool IElementContainer.ElementSelectionEnabled {
			get { return pie.ElementSelectionEnabled; }
		}
		int IElementContainer.SelectedElementIndex {
			get { return pie.SelectedElementIndex; }
			set { pie.SelectedElementIndex = value; }
		}
		#endregion
		protected override IEnumerable<Measure> GetQueryVisibleMeasures() {
			foreach(Measure measure in base.GetQueryVisibleMeasures())
				yield return measure;
			foreach(Measure measure in Values)
				yield return measure;
		}
	}
}
