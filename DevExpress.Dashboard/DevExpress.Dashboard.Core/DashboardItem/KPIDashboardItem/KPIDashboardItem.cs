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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Xml.Linq;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.Compatibility.System.ComponentModel;
namespace DevExpress.DashboardCommon {
	public abstract class KpiDashboardItem<TKpiElement> : SeriesDashboardItem, IElementContainer where TKpiElement : KpiElement, new() {
		const ContentArrangementMode DefaultContentArrangementMode = ContentArrangementMode.Auto;
		const int DefaultContentLineCount = 3;
		const string xmlContentArrangementMode = "ContentArrangementMode";
		const string xmlContentLineCount = "ContentLineCount";
		readonly KpiElementContainer<TKpiElement> elementContainer;
		readonly DataItemContainerCollection<TKpiElement> elements;
		ContentArrangementMode contentArrangementMode = DefaultContentArrangementMode;
		int contentLineCount = DefaultContentLineCount;
		[
		Category(CategoryNames.Interactivity),
		TypeConverter(TypeNames.DisplayNameObjectConverter),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content)
		]
		public DashboardItemInteractivityOptions InteractivityOptions { get { return InteractivityOptionsBase as DashboardItemInteractivityOptions; } }
		[
		Category(CategoryNames.Layout),
		DefaultValue(DefaultContentArrangementMode)
		]
		public ContentArrangementMode ContentArrangementMode {
			get { return contentArrangementMode; }
			set {
				if (value != contentArrangementMode) {
					contentArrangementMode = value;
					OnChanged(ChangeReason.View, this);
				}
			}
		}
		[
		Category(CategoryNames.Layout),
		DefaultValue(DefaultContentLineCount)
		]
		public int ContentLineCount {
			get { return contentLineCount; }
			set {
				if (value != contentLineCount) {
					contentLineCount = value;
					OnChanged(ChangeReason.View, this);
				}
			}
		}
		internal TKpiElement ActiveElement { 
			get {
				TKpiElement element = elementContainer.ActiveElement;
				return elements.Contains(element) ? element : null;
			}
		}
		internal DataItemContainerCollection<TKpiElement> Elements { get { return elements; } }
		protected internal override bool IsDrillDownEnabled {
			get { return InteractivityOptions.IsDrillDownEnabled; }
			set { InteractivityOptions.IsDrillDownEnabled = value; }
		}
		protected internal override DashboardItemMasterFilterMode MasterFilterMode {
			get { return InteractivityOptions.MasterFilterMode; }
			set { InteractivityOptions.MasterFilterMode = value; }
		}
		protected internal override IElementContainer ElementContainer { get { return this; } }
		protected internal override bool HasDataItems {
			get {
				foreach(TKpiElement element in elements)
					if(element.ActualValue != null || element.TargetValue != null)
						return true;
				return base.HasDataItems;
			}
		}
		protected override IEnumerable<DataDashboardItemDescription> ArgumentsDescriptions { get { return new DataDashboardItemDescription[0] { }; } }
		protected internal override IList<Dimension> SelectionDimensionList { get { return SeriesDimensions; } }
		protected abstract bool ShowMeasures { get; }
		protected string SeriesAxisName { get { return DashboardDataAxisNames.DefaultAxis; } }
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
		IEnumerable<TKpiElement> ActiveElementsInternal {
			get {
				if(elementContainer.ElementSelectionEnabled)
					return new[] { ActiveElement };
				return elements;
			}
		}
		protected KpiDashboardItem(DataItemContainerCollection<TKpiElement> elements) {
			this.elements = elements;
			elements.XmlSerializer = new RepositoryItemListXmlSerializerBase<KpiElement, TKpiElement>(null, XmlRepository.KpiElementRepository, true);
			elementContainer = new KpiElementContainer<TKpiElement>(this, new ReadOnlyCollection<TKpiElement>(elements));
			elements.CollectionChanged += (sender, e) => {
				elementContainer.ValidateActiveElement();
				OnDataItemContainersChanged(e.AddedItems, e.RemovedItems);				
			};
		}
		protected override FilterableDashboardItemInteractivityOptions CreateInteractivityOptions() {
			return new DashboardItemInteractivityOptions(false);
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			foreach(KpiElement element in ActiveElementsInternal) {
				Measure actualValue = element.ActualValue;
				Measure targetValue = element.TargetValue;
				if(actualValue != null)
					builder.AddMeasure(actualValue);
				if(targetValue != null)
					builder.AddMeasure(targetValue);
				if(actualValue != null && targetValue != null)
					builder.AddDeltaDescriptor(actualValue, targetValue, element, element.DeltaOptions);
			}
			DimensionDescriptorInternalCollection dimensions = builder.DimensionDescriptors[DashboardDataAxisNames.ChartSeriesAxis];
			builder.DimensionDescriptors.Remove(DashboardDataAxisNames.ChartSeriesAxis);
			builder.DimensionDescriptors.Add(DashboardDataAxisNames.DefaultAxis, dimensions);
			builder.ColumnHierarchy = string.Empty;
			builder.RowHierarchy = DashboardDataAxisNames.DefaultAxis;
		}
		protected internal override string[] GetSelectionDataMemberDisplayNames() {
			return GetSeriesDataMembers();
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
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(ContentArrangementMode != DefaultContentArrangementMode)
				element.Add(new XAttribute(xmlContentArrangementMode, contentArrangementMode));
			if(ContentLineCount != DefaultContentLineCount)
				element.Add(new XAttribute(xmlContentLineCount, contentLineCount));
			elements.SaveToXml(element);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			string layoutArrangeModeAttr = XmlHelper.GetAttributeValue(element, xmlContentArrangementMode);
			if(!String.IsNullOrEmpty(layoutArrangeModeAttr)) {
				if(layoutArrangeModeAttr == "FixedColumCount")
					contentArrangementMode = ContentArrangementMode.FixedColumnCount;
				else
					contentArrangementMode = XmlHelper.EnumFromString<ContentArrangementMode>(layoutArrangeModeAttr);
			}
			string layoutLineCountAttr = XmlHelper.GetAttributeValue(element, xmlContentLineCount);
			if (!String.IsNullOrEmpty(layoutLineCountAttr))
				contentLineCount = XmlHelper.FromString<int>(layoutLineCountAttr);
			elements.LoadFromXml(element);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			elements.OnEndLoading(this);
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			description.ContentArrangementMode = ContentArrangementMode;
			description.ContentLineCount = ContentLineCount;
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			ContentArrangementMode = description.ContentArrangementMode;
			if (description.ContentLineCount != Int32.MinValue)
				ContentLineCount = description.ContentLineCount;
		}
		protected override void FillEditNameDescriptions(IList<EditNameDescription> descriptions) {
			base.FillEditNameDescriptions(descriptions);
			if (SeriesDimensions.Count == 0 || elements.Count > 1)
				descriptions.Add(new EditNameDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionValues), elements));
		}
		IEnumerable<TKpiElement> GetActiveKPIElements() {
			return ((IElementContainer)this).ElementSelectionEnabled ? new[] { ActiveElement } : Elements.Cast<TKpiElement>();
		}
		IEnumerable<TKpiElement> GetDeltaKPIElements() {
			return GetActiveKPIElements().Where(g => g.ActualValue != null && g.TargetValue != null);
		}
		IEnumerable<TKpiElement> GetSimpleKPIElements() {
			return GetActiveKPIElements().Except(GetDeltaKPIElements());
		}
		protected virtual IEnumerable<Dimension> QuerySparklineDimension { get { return new Dimension[0]; } }
		protected override IEnumerable<Measure> GetQueryVisibleMeasures() {
			return GetSimpleKPIElements().Select(g => g.ActualValue ?? g.TargetValue).NotNull();
		}
		protected override SliceDataQuery GetDataQueryInternal(IActualParametersProvider provider) {
			var dimensions = IsDrillDownEnabled ? SeriesDimensions.Take(SeriesDimensions.IndexOf(CurrentDrillDownDimension) + 1).ToArray() : SeriesDimensions.ToArray();
			var invisibleDimensions = IsDrillDownEnabled ? SeriesDimensions.Except(dimensions).ToArray() : new Dimension[0];
			var deltaMeasures = GetDeltaKPIElements().Select(kpi => new DeltaMeasureInfo(kpi.ActualValue, kpi.TargetValue, kpi.DeltaOptions)).NotNull();
			var filterDimensions = QueryFilterDimensions.Concat(invisibleDimensions).ToList(); 
			SliceDataQueryBuilder queryBuilder;
			ItemModelBuilder itemBuilder = new ItemModelBuilder(DataSourceModel.DataSourceInfo, GetDataItemUniqueName, provider);
			if(IsBackCompatibilityDataSlicesRequired) {
				queryBuilder = SliceDataQueryBuilder.CreateWithPivotModel(itemBuilder, QuerySparklineDimension, dimensions,
					QueryMeasures, deltaMeasures, filterDimensions, GetQueryFilterCriteria(provider));
			} else {
				queryBuilder = SliceDataQueryBuilder.CreateEmpty(itemBuilder, filterDimensions, GetQueryFilterCriteria(provider));
				if(QueryMeasures.Count() > 0 || deltaMeasures.Count() > 0) {
					queryBuilder.AddSlice(dimensions, QueryMeasures, deltaMeasures);
					if(QuerySparklineDimension.Count() != 0)
						queryBuilder.AddSlice(dimensions.Concat(QuerySparklineDimension), QueryMeasures, deltaMeasures);
					queryBuilder.SetAxes(QuerySparklineDimension, dimensions);
				}
			}
			return queryBuilder.FinalQuery();
		}
	}
}
