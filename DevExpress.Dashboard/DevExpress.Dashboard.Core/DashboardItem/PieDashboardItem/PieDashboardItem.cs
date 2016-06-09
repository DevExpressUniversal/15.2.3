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
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Xml.Linq;
using DevExpress.DashboardCommon.Localization;
using DevExpress.DashboardCommon.Native;
using DevExpress.DashboardCommon.ViewModel;
using DevExpress.DataAccess;
using DevExpress.Compatibility.System.ComponentModel;
using DevExpress.Compatibility.System.Drawing.Design;
namespace DevExpress.DashboardCommon {
	[
	DashboardItemType(DashboardItemType.Pie)
	]
	public class PieDashboardItem : ChartDashboardItemBase, IPieContext, IElementContainer {
		const PieType DefaultPieType = PieType.Pie;
		const PieValueType DefaultLabelContentType = PieValueType.ArgumentAndPercent;
		const PieValueType DefaultTooltipContentType = PieValueType.ArgumentValueAndPercent;
		const PieLegendPosition DefaultLegendPosition = PieLegendPosition.None;
		const ContentArrangementMode DefaultContentArrangementMode = ContentArrangementMode.Auto;
		const int DefaultContentLineCount = 3;
		const bool DefaultShowPieCaptions = true;
		const string xmlPieType = "PieType";
		const string xmlLabelContentType = "LabelContentType";
		const string xmlTooltipContentType = "TooltipContentType";
		const string xmlLegendPosition = "LegendPosition";
		const string xmlContentArrangementMode = "ContentArrangementMode";
		const string xmlContentLineCount = "ContentLineCount";
		const string xmlShowPieCaptions = "ShowPieCaptions";
		readonly PieInternal pie;
		PieType pieType = DefaultPieType;
		PieValueType labelContentType = DefaultLabelContentType;
		PieValueType tooltipContentType = DefaultTooltipContentType;
		PieLegendPosition legendPosition = DefaultLegendPosition;
		bool showPieCaptions = DefaultShowPieCaptions;
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PieDashboardItemValues"),
#endif
		Category(CategoryNames.Data),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
		Editor(TypeNames.NotifyingCollectionEditor, typeof(UITypeEditor))
		]
		public MeasureCollection Values { get { return pie.Values; } }
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PieDashboardItemLabelContentType"),
#endif
		Category(CategoryNames.Format),
		DefaultValue(DefaultLabelContentType)
		]
		public PieValueType LabelContentType {
			get { return labelContentType; }
			set {
				if(value != labelContentType) {
					labelContentType = value;
					OnChanged(ChangeReason.View, this);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PieDashboardItemTooltipContentType"),
#endif
		Category(CategoryNames.Format),
		DefaultValue(DefaultTooltipContentType)
		]
		public PieValueType TooltipContentType {
			get { return tooltipContentType; }
			set {
				if(value != tooltipContentType) {
					tooltipContentType = value;
					OnChanged(ChangeReason.View, this);
				}
			}
		}
		[
		DefaultValue(DefaultLegendPosition),
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public PieLegendPosition LegendPosition {
			get { return legendPosition; }
			set {
				if(value != legendPosition) {
					legendPosition = value;
					OnChanged(ChangeReason.View, this);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PieDashboardItemPieType"),
#endif
		Category(CategoryNames.Style),
		DefaultValue(DefaultPieType)
		]
		public PieType PieType {
			get { return pieType; }
			set {
				if (value != pieType) {
					pieType = value;
					OnChanged(ChangeReason.View, this);
				}
			}
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PieDashboardItemContentArrangementMode"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultContentArrangementMode)
		]
		public ContentArrangementMode ContentArrangementMode {
			get { return pie.ContentArrangementMode; }
			set { pie.ContentArrangementMode = value; }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PieDashboardItemContentLineCount"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultContentLineCount)
		]
		public int ContentLineCount {
			get { return pie.ContentLineCount; }
			set { pie.ContentLineCount = value; }
		}
		[
#if !SL
	DevExpressDashboardCoreLocalizedDescription("PieDashboardItemShowPieCaptions"),
#endif
		Category(CategoryNames.Layout),
		DefaultValue(DefaultShowPieCaptions)
		]
		public bool ShowPieCaptions { 
			get { return showPieCaptions; } 
			set {
				if(value != showPieCaptions) {
					showPieCaptions = value;
					OnChanged(ChangeReason.View, this);
				}
			} 
		}
		[
		Browsable(false),
		EditorBrowsable(EditorBrowsableState.Never),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
		]
		public DataItemNameRepository NamesRepository { get { return pie.NamesRepository; } }
		internal PieInternal Pie { get { return pie; } }
		IList<Dimension> IPieContext.Arguments { get { return Arguments; } }
		protected override bool HasValues { get { return Values.Count > 0; } }
		protected internal override string CaptionPrefix { get { return DashboardLocalizer.GetString(DashboardStringId.DefaultNamePieItem); } }
		internal override bool ColorMeasuresByHue {
			get {
				return base.ColorMeasuresByHue && ProvideValuesAsArguments || ColoringOptions.MeasuresColoringMode == ColoringMode.Hue;
			}
		}
		protected override IEnumerable<DataDashboardItemDescription> ValuesDescriptions {
			get {
				yield return new DataDashboardItemDescription(DashboardLocalizer.GetString(DashboardStringId.DescriptionValues),
					DashboardLocalizer.GetString(DashboardStringId.DescriptionItemValue), ItemKind.NumericMeasure, Values);
			}
		}
		protected internal override IElementContainer ElementContainer { get { return pie; } }
		protected internal override bool HasDataItems { get { return base.HasDataItems || Values.Count > 0; } }
		protected override bool IsSupportArgumentNumericGroupIntervals { get { return false; } }
		protected internal override Dictionary<string, string> ColorMeasureDescriptorsInfo {
			get {
				Dictionary<string, string> dict = new Dictionary<string, string>();
				foreach(Measure measure in ActiveElements) {
					string key = DataItemDefinitionDisplayTextProvider.GetMeasureDefinitionString(measure.GetMeasureDefinition());
					if(!dict.ContainsKey(key))
						dict.Add(key, measure.DisplayName);
				}
				return dict;
			}
		}
		internal bool ProvideValuesAsArguments { get { return Arguments.Count == 0; } }
		IChangeService IPieContext.ChangeService { get { return this; } }
		IDataItemRepositoryProvider IPieContext.DataItemRepositoryProvider { get { return this; } }
		bool IPieContext.AllowLayers { get { return SeriesDimensions.Count > 0 && Arguments.Count > 0; } }
		ContentArrangementMode IPieContext.DefaultContentArrangementMode { get { return DefaultContentArrangementMode; } }
		int IPieContext.DefaultContentLineCount { get { return DefaultContentLineCount; } }
		bool IPieContext.ResetActiveElementBeforeDataItemChanged { get { return Arguments.Count == 0; } }
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
		public PieDashboardItem() {
			pie = new PieInternal(this);
			pie.Changed += OnPieInternalChanged;
		}
		~PieDashboardItem() {
			pie.Changed -= OnPieInternalChanged;
		}
		ValueFormatViewModel[] CreateValueFormats(Measure value) {
			ValueFormatViewModel valueViewModel = new ValueFormatViewModel(value.CreateNumericFormatViewModel());
			ValueFormatViewModel percentViewModel = new ValueFormatViewModel(value.CreateKpiFormatViewModel(DeltaValueType.PercentOfTarget));
			return new ValueFormatViewModel[] { valueViewModel, percentViewModel };
		}
		protected override void OnArgumentsCollectionChanged(object sender, NotifyingCollectionChangedEventArgs<Dimension> e) {
			base.OnArgumentsCollectionChanged(sender, e); 
			if(ProvideValuesAsArguments)
				pie.ResetActiveElement();
		}
		protected override bool ColorDimension(Dimension dimension) {
			return Arguments.Contains(dimension);
		}
		protected override Measure[][] GetColoringMeasuresInternal() {
			Measure[][] measures = new Measure[Values.Count][];
			for(int i = 0; i < Values.Count; i++) {
				measures[i] = new Measure[1];
				measures[i][0] = Values[i];
			}
			return measures;
		}
		protected internal override DashboardItemViewModel CreateViewModel() {
			return new PieDashboardItemViewModel(this, pie);
		}
		protected internal override void SaveToXml(XElement element) {
			base.SaveToXml(element);
			if(PieType != DefaultPieType)
				element.Add(new XAttribute(xmlPieType, pieType));
			if(LabelContentType != DefaultLabelContentType)
				element.Add(new XAttribute(xmlLabelContentType, labelContentType));
			if(TooltipContentType != DefaultTooltipContentType)
				element.Add(new XAttribute(xmlTooltipContentType, tooltipContentType));
			if(LegendPosition != DefaultLegendPosition)
				element.Add(new XAttribute(xmlLegendPosition, legendPosition));
			if(ContentArrangementMode != DefaultContentArrangementMode)
				element.Add(new XAttribute(xmlContentArrangementMode, ContentArrangementMode));
			if(ContentLineCount != DefaultContentLineCount)
				element.Add(new XAttribute(xmlContentLineCount, ContentLineCount));
			if(ShowPieCaptions != DefaultShowPieCaptions)
				element.Add(new XAttribute(xmlShowPieCaptions, showPieCaptions));
			pie.SaveToXml(element);
		}
		protected internal override void LoadFromXmlInternal(XElement element) {
			base.LoadFromXmlInternal(element);
			string pieTypeAttr = XmlHelper.GetAttributeValue(element, xmlPieType);
			if (!String.IsNullOrEmpty(pieTypeAttr))
				pieType = XmlHelper.EnumFromString<PieType>(pieTypeAttr);
			string labelContentTypeAttr = XmlHelper.GetAttributeValue(element, xmlLabelContentType);
			if(!String.IsNullOrEmpty(labelContentTypeAttr))
				labelContentType = XmlHelper.EnumFromString<PieValueType>(labelContentTypeAttr);
			string tooltipContentTypeAttr = XmlHelper.GetAttributeValue(element, xmlTooltipContentType);
			if(!String.IsNullOrEmpty(tooltipContentTypeAttr))
				tooltipContentType = XmlHelper.EnumFromString<PieValueType>(tooltipContentTypeAttr);
			string legendPositionAttr = XmlHelper.GetAttributeValue(element, xmlLegendPosition);
			if(!String.IsNullOrEmpty(legendPositionAttr))
				legendPosition = XmlHelper.EnumFromString<PieLegendPosition>(legendPositionAttr);
			string layoutArrangeModeAttr = XmlHelper.GetAttributeValue(element, xmlContentArrangementMode);
			string showPieCaptionAttr = element.GetAttributeValue(xmlShowPieCaptions);
			if(!string.IsNullOrEmpty(showPieCaptionAttr))
				showPieCaptions = XmlHelper.FromString<bool>(showPieCaptionAttr);
			if(!String.IsNullOrEmpty(layoutArrangeModeAttr)) {
				if(layoutArrangeModeAttr == "FixedColumCount")
					ContentArrangementMode = ContentArrangementMode.FixedColumnCount;
				else
					ContentArrangementMode = XmlHelper.EnumFromString<ContentArrangementMode>(layoutArrangeModeAttr);
			}
			string layoutLineCountAttr = XmlHelper.GetAttributeValue(element, xmlContentLineCount);
			if (!String.IsNullOrEmpty(layoutLineCountAttr))
				ContentLineCount = XmlHelper.FromString<int>(layoutLineCountAttr);
			pie.LoadFromXml(element);
		}
		protected override void OnEndLoading() {
			base.OnEndLoading();
			pie.OnEndLoading(this);
		}
		protected override SummaryTypeInfo GetSummaryTypeInfo(Measure measure) {
			SummaryTypeInfo summaryTypeInfo = pie.GetSummaryTypeInfo(measure);
			if(summaryTypeInfo != null)
				return summaryTypeInfo;
			return base.GetSummaryTypeInfo(measure);
		}
		internal bool IsValueEmpty(Measure value) {
			return ProvideValuesAsArguments && Values.Count > 0 && value != Values[0];
		}
		[Obsolete("This method is now obsolete. Use the DataItem.Name property instead.")]
		public void SetValueName(Measure value, string name) {
			value.Name = name;
		}
		[Obsolete("This method is now obsolete. Use the DataItem.Name property instead.")]
		public string GetValueName(Measure value) {
			return value.Name;
		}
		protected override void GetMetadataInternal(HierarchicalMetadataBuilder builder) {
			base.GetMetadataInternal(builder);
			foreach(Measure measure in ActiveElements)
				builder.AddMeasure(measure);
			builder.AddColorMeasureDescriptors(this);
		}
		internal override DashboardItemDataDescription CreateDashboardItemDataDescription() {
			DashboardItemDataDescription description = base.CreateDashboardItemDataDescription();
			description.HasAdditionalDimensions = true;
			foreach (Dimension dimension in SeriesDimensions)
					description.AddMainDimension(dimension);
			foreach (Dimension dimension in Arguments)
					description.AddAdditionalDimension(dimension);
			foreach (Measure measure in Values)
					description.AddMeasure(measure);
			return description;
		}
		internal override void AssignDashboardItemDataDescriptionCore(DashboardItemDataDescription description) {
			base.AssignDashboardItemDataDescriptionCore(description);
			AssignDimension(description.Latitude, SeriesDimensions);
			AssignDimension(description.Longitude, SeriesDimensions);
			if (!description.HasAdditionalDimensions && description.MainDimensions.Count != 0 && description.AdditionalDimensions.Count == 0) {
				List<Dimension> dimensions = new List<Dimension>();
				dimensions.AddRange(description.MainDimensions);
				DataFieldType fieldType = dimensions[0].DataFieldType;
				while (dimensions.Count > 0) {
					if (dimensions[0].DataFieldType == fieldType) {
						SeriesDimensions.Add(dimensions[0]);
						dimensions.RemoveAt(0);
					}
					else
						break;
				}
				Arguments.AddRange(dimensions);
			}
			else {
				SeriesDimensions.AddRange(description.MainDimensions);
				Arguments.AddRange(description.AdditionalDimensions);
			}
			if (Arguments.Count == 0 && SeriesDimensions.Count != 0) {
				List<Dimension> arg = new List<Dimension>();
				arg.AddRange(SeriesDimensions);
				SeriesDimensions.Clear();
				Arguments.AddRange(arg);
			}
			AssignDimension(description.SparklineArgument, HiddenDimensions);
			foreach (Measure measure in description.Measures)
				AssignMeasure(measure, Values);
		}
		protected override void FillEditNameDescriptions(IList<EditNameDescription> descriptions) {
			base.FillEditNameDescriptions(descriptions);
			descriptions.Add(pie.GetEditNameDescription());
		}
		void OnPieInternalChanged(object sender, ChangedEventArgs e) {
			if(e != null)
				OnChanged(e.Reason, this);
		}
		protected override IEnumerable<Measure> GetQueryVisibleMeasures() {
			return Pie.ElementSelectionEnabled ? new Measure[] { Pie.ActiveElement } : Pie.Values.Cast<Measure>();
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
	}
}
