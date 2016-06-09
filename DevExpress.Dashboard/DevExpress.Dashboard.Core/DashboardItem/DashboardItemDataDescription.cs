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
using System.Linq;
using System.Text;
namespace DevExpress.DashboardCommon.Native {
	public enum FilterElementTypeDescription { None, Radio, Checked }
	public class DashboardItemDataDescription {
		readonly List<MeasureDescription> measures;
		readonly List<Dimension> mainDimensions;
		readonly List<Dimension> additionalDimensions;
		readonly List<Measure> tooltipMeasures;
		readonly List<Dimension> tooltipDimensions;
		readonly DimensionCollection hiddenDimensions;
		readonly MeasureCollection hiddenMeasures;
		readonly DataItemRepository dataItemRepository;
		readonly List<ChartSeries> series;
		readonly Dictionary<Dimension, DimensionDescription> dimensionDescriptions;
		readonly List<RuleDescription> rules;
		ShapefileArea area = MapDashboardItem.DefaultShapefileArea;
		readonly IDashboardDataSource dataSource;
		readonly string dataMember;
		DashboardItemGroup group;
		Dictionary<DataItem, string> dataItemUniqueNames;
		Dimension sparklineArgument;
		Dimension latitude;
		Dimension longitude;
		int contentLineCount = Int32.MinValue;
		FilterElementTypeDescription filterElementType;
		bool filterElementShowAllValue = true;
		public List<MeasureDescription> MeasureDescriptions { get { return measures; } }
		public IEnumerable<Measure> Measures {
			get {
				foreach (MeasureDescription description in MeasureDescriptions)
					if (description.MeasureType == MeasureDescriptionType.Delta) {
						if (description.ActualValue != null)
							yield return description.ActualValue;
						if (description.TargetValue != null)
							yield return description.TargetValue;
					}
					else
						yield return description.Value;
			}
		}
		public IEnumerable<DataItem> DataItems {
			get {
				foreach(Measure description in Measures)
					yield return description;
				foreach(Measure description in TooltipMeasures)
					yield return description;
				foreach(Measure description in HiddenMeasures)
					yield return description;
				foreach(Dimension description in MainDimensions)
					yield return description;
				foreach(Dimension description in AdditionalDimensions)
					yield return description;
				foreach(Dimension description in AdditionalDimensions)
					yield return description;
				foreach(Dimension description in TooltipDimensions)
					yield return description;
				foreach(Dimension description in HiddenDimensions)
					yield return description;
				if(Latitude != null)
					yield return Latitude;
				if(Longitude != null)
					yield return Longitude;
				if(SparklineArgument != null)
					yield return SparklineArgument;
			}
		}
		public List<Dimension> MainDimensions { get { return mainDimensions; } }
		public List<Dimension> AdditionalDimensions { get { return additionalDimensions; } }
		public List<Measure> TooltipMeasures { get { return tooltipMeasures; } }
		public List<Dimension> TooltipDimensions { get { return tooltipDimensions; } }
		public Dimension SparklineArgument { get { return sparklineArgument; } }
		public DimensionCollection HiddenDimensions { get { return hiddenDimensions; } }
		public MeasureCollection HiddenMeasures { get { return hiddenMeasures; } }
		public Dimension Latitude { get { return latitude; } }
		public Dimension Longitude { get { return longitude; } }
		public Dictionary<Dimension, DimensionDescription> DimensionDescriptions { get { return dimensionDescriptions; } }
		public ShapefileArea Area { get { return area; } set { area = value; } }
		public CustomShapefile CustomShapefile { get; set; }
		public MapViewport MapViewport { get; set; }
		public bool HasAdditionalDimensions { get; set; }
		public IDashboardDataSource DataSource { get { return dataSource; } }
		public string DataMember { get { return dataMember; } }
		public DashboardItemGroup Group { get { return group; } set { group = value; } }
		public string Name { get; set; }
		public string FilterString { get; set; }
		public bool IsMasterFilterCrossDataSource { get; set; }
		public Dictionary<DataItem, string> DataItemUniqueNames { get { return dataItemUniqueNames; } }
		public FilterableDashboardItemInteractivityOptions InteractivityOptions { get; set; }
		public bool ShowCaption { get; set; }
		public ContentArrangementMode ContentArrangementMode { get; set; }
		public int ContentLineCount { get { return contentLineCount; } set { contentLineCount = value; } }
		public bool EnableClustering { get; set; }
		public MapLegend Legend { get; set; }
		public WeightedLegend WeightedLegend { get; set; }
		public List<ChartSeries> Series { get { return series; } }
		public FilterElementTypeDescription FilterElementType { get { return filterElementType; } set { filterElementType = value; } }
		public bool FilterElementShowAllValue { get { return filterElementShowAllValue; } set { filterElementShowAllValue = value; } }
		public List<RuleDescription> FormatRules { get { return rules; } }
		public DashboardItemDataDescription(IDashboardDataSource dataSource, string dataMember, DataItemRepository dataItemRepository) {
			this.measures = new List<MeasureDescription>();
			this.mainDimensions = new List<Dimension>();
			this.additionalDimensions = new List<Dimension>();
			this.tooltipMeasures = new List<Measure>();
			this.tooltipDimensions = new List<Dimension>();
			this.hiddenDimensions = new DimensionCollection();
			this.hiddenMeasures = new MeasureCollection();
			this.dataItemUniqueNames = new Dictionary<DataItem, string>();
			this.series = new List<ChartSeries>();
			this.dimensionDescriptions = new Dictionary<Dimension, DimensionDescription>();
			this.rules = new List<RuleDescription>();
			this.dataSource = dataSource;
			this.dataMember = dataMember;
			this.dataItemRepository = dataItemRepository;
		}
		public void AddMainDimension(Dimension dimension) {
			if (dimension != null) {
				Dimension clone = CreateDimensionClone(dimension);
				DataItemUniqueNames.Add(clone, dataItemRepository.GetActualID(dimension));
				MainDimensions.Add(clone);
			}
		}
		public void AddAdditionalDimension(Dimension dimension) {
			if (dimension != null) {
				Dimension clone = CreateDimensionClone(dimension);
				DataItemUniqueNames.Add(clone, dataItemRepository.GetActualID(dimension));
				AdditionalDimensions.Add(clone);
			}
		}
		public void AddLatitude(Dimension dimension) {
			if (dimension != null) {
				Dimension clone = CreateDimensionClone(dimension);
				DataItemUniqueNames.Add(clone, dataItemRepository.GetActualID(dimension));
				latitude = clone;
			}
		}
		public void AddLongitude(Dimension dimension) {
			if (dimension != null) {
				Dimension clone = CreateDimensionClone(dimension);
				DataItemUniqueNames.Add(clone, dataItemRepository.GetActualID(dimension));
				longitude = clone;
			}
		}
		public void AddSparklineArgument(Dimension dimension) {
			if (dimension != null) {
				Dimension clone = CreateDimensionClone(dimension);
				DataItemUniqueNames.Add(clone, dataItemRepository.GetActualID(dimension));
				sparklineArgument = clone;
			}
		}
		public void AddHiddenDimension(Dimension dimension) {
			if (dimension != null) {
				Dimension clone = CreateDimensionClone(dimension);
				DataItemUniqueNames.Add(clone, dataItemRepository.GetActualID(dimension));
				HiddenDimensions.Add(clone);
			}
		}
		public void AddHiddenMeasure(Measure measure) {
			Measure clone = measure.Clone();
			DataItemUniqueNames.Add(clone, dataItemRepository.GetActualID(measure));
			HiddenMeasures.Add(clone);
		}
		public void AddMeasure(Measure measure) {
			if (measure != null) {
				Measure clone = measure.Clone();
				DataItemUniqueNames.Add(clone, dataItemRepository.GetActualID(measure));
				MeasureDescription description = new MeasureDescription(clone);
				MeasureDescriptions.Add(description);
			}
		}
		public void AddMeasure(Measure actualValue, Measure targetValue, DeltaOptions deltaOptions) {
			Measure actualValueClone = actualValue != null ? actualValue.Clone() : null;
			Measure targetValueClone = targetValue != null ? targetValue.Clone() : null;
			if (actualValueClone != null)
				DataItemUniqueNames.Add(actualValueClone, dataItemRepository.GetActualID(actualValue));
			if (targetValueClone != null)
				DataItemUniqueNames.Add(targetValueClone, dataItemRepository.GetActualID(targetValue));
			MeasureDescription description = new MeasureDescription(actualValueClone, targetValueClone, deltaOptions.Clone());
			MeasureDescriptions.Add(description);
		}
		public void AddTooltipDimension(Dimension dimension) {
			if (dimension != null) {
				Dimension clone = CreateDimensionClone(dimension);
				DataItemUniqueNames.Add(clone, dataItemRepository.GetActualID(dimension));
				TooltipDimensions.Add(clone);
			}
		}
		public void AddTooltipMeasure(Measure dimension) {
			if (dimension != null) {
				Measure clone = dimension.Clone();
				DataItemUniqueNames.Add(clone, dataItemRepository.GetActualID(dimension));
				TooltipMeasures.Add(clone);
			}
		}
		public void AddSeries(ChartSeries series) {
			this.series.Add((ChartSeries)series.Clone());
		}
		public void AddFormatRule(DashboardItemFormatRule rule) {
			string dataItemActualId = rule.LevelCore.Item != null ? dataItemRepository.GetActualID(rule.LevelCore.Item) : String.Empty;
			string dataItemApplyToActualId = rule.LevelCore.ItemApplyTo != null ? dataItemRepository.GetActualID(rule.LevelCore.ItemApplyTo) : String.Empty;
			DataItem item = null;
			DataItem itemApplyTo = null;
			foreach(DataItem key in DataItemUniqueNames.Keys) {
				if(!String.IsNullOrEmpty(dataItemActualId) && DataItemUniqueNames[key] == dataItemActualId)
					item = key;
				if(!String.IsNullOrEmpty(dataItemApplyToActualId) && DataItemUniqueNames[key] == dataItemApplyToActualId)
					itemApplyTo = key;
			}
			RuleDescription ruleDescription = new RuleDescription() {
				Name = rule.Name,
				Enabled = rule.Enabled,
				StopIfTrue = rule.StopIfTrue,
				Condition = rule.Condition != null ? rule.Condition.Clone() : null,
				ApplyToRow = rule.LevelCore.ApplyToRow
			};
			if(item != null)
				ruleDescription.Item = item;
			if(itemApplyTo != null)
				ruleDescription.ItemApplyTo = itemApplyTo;
			FormatRules.Add(ruleDescription);
		}
		Dimension CreateDimensionClone(Dimension dimension) {
			Dimension clone = dimension.WeakClone();
			dimensionDescriptions.Add(clone, new DimensionDescription(dimension));
			return clone;
		}
	}
	public class DimensionDescription {
		public MeasureDefinition SortByMeasureDefinition { get; protected set; }
		public MeasureDefinition TopNMeasureDefinition { get; private set; }
		public DimensionDescription(Dimension dimension) {
			if (dimension.SortByMeasure != null)
				SortByMeasureDefinition = dimension.SortByMeasure.GetMeasureDefinition();
			if (dimension.TopNOptions.Measure != null)
				TopNMeasureDefinition = dimension.TopNOptions.Measure.GetMeasureDefinition();
		}
	}
	public class MeasureDescription {
		readonly Measure actualValue = null;
		readonly Measure targetValue = null;
		readonly DeltaOptions deltaOptions = null;
		readonly Measure value = null;
		MeasureDescriptionType measureType = MeasureDescriptionType.Value;
		public Measure ActualValue { get { return actualValue; } }
		public Measure TargetValue { get { return targetValue; } }
		public Measure Value { get { return value; } }
		public MeasureDescriptionType MeasureType { get { return measureType; } }
		public DeltaOptions DeltaOptions { get { return deltaOptions; } }
		public MeasureDescription(Measure actualValue, Measure targetValue, DeltaOptions deltaOptions) {
			this.actualValue = actualValue;
			this.targetValue = targetValue;
			this.deltaOptions = deltaOptions;
			this.measureType = MeasureDescriptionType.Delta;
		}
		public MeasureDescription(Measure value) {
			this.value = value;
		}
	}
	public enum MeasureDescriptionType { 
		Value,
		Delta
	}
	public class RuleDescription {
		public string Name { get; set; }
		public bool Enabled { get; set; }
		public bool StopIfTrue { get; set; }
		public FormatConditionBase Condition { get; set; }
		public bool ApplyToRow { get; set; }
		public DataItem Item { get; set; }
		public DataItem ItemApplyTo { get; set; }
	}
}
