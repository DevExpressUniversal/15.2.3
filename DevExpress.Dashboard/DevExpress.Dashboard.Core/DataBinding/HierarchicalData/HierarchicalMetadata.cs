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

using System.Collections.Generic;
using DevExpress.DashboardCommon.DataProcessing;
using DevExpress.DashboardCommon.ViewModel;
using System;
using System.Linq;
namespace DevExpress.DashboardCommon.Native {
	public class HierarchicalItemData {
		public HierarchicalMetadata MetaData { get; set; }
		public DataStorageDTO DataStorageDTO { get; set; }
		public string[] ColumnIds { get; set; }
		public string[] RowIds { get; set; }
		public string[][] SortOrderSlices { get; set; }
	}
	public class HierarchicalMetadataBuilder {
		protected const string AbsoluteVariationPivotFieldName = "{0}-{1}_AbsVar";
		protected const string PercentVariationPivotFieldName = "{0}-{1}_PerVar";
		protected const string PercentOfTargetPivotFieldName = "{0}-{1}_PerOfTar";
		protected const string DeltaIsGoodPivotFieldNameFormat = "{0}-{1}_IsGood";
		protected const string DeltaIndicatorTypePivotFieldNameFormat = "{0}-{1}_IndicatorType";
		readonly bool addHiddenMeasures;
		readonly Func<SliceDataQuery> getSliceDataQuery;
		HierarchicalMetadata metadata;
		SliceDataQuery query = null;
		public List<string> DataSourceColumns {
			get { return metadata.DataSourceColumns; }
			set { metadata.DataSourceColumns = value; }
		}
		public string RowHierarchy {
			get { return metadata.RowHierarchy; }
			set { metadata.RowHierarchy = value; }
		}		
		public string ColumnHierarchy {
			get { return metadata.ColumnHierarchy; }
			set { metadata.ColumnHierarchy = value; }
		}
		public MeasureDescriptorInternalCollection MeasureDescriptors { get { return metadata.MeasureDescriptors; } }
		public Dictionary<string, DimensionDescriptorInternalCollection> DimensionDescriptors { get { return metadata.DimensionDescriptors; } }
		public HierarchicalMetadataBuilder(bool addHiddenMeasures, Func<SliceDataQuery> getSliceDataQuery) {
			this.addHiddenMeasures = addHiddenMeasures;
			this.getSliceDataQuery = getSliceDataQuery;
			metadata = new HierarchicalMetadata();
		}
		public void AddMeasure(Measure measure) {
			metadata.MeasureDescriptors.Add(CreateMeasureDescriptor(measure));
		}
		public void AddHiddenMeasure(Measure measure) {
			EnsureSliceQuery();
			MeasureDescriptorInternal md = CreateMeasureDescriptor(measure);
			if(addHiddenMeasures || query.DataSlices.Any(slice => slice.Measures.Any(m => m.Name == md.ID)))
				metadata.MeasureDescriptors.Add(md);
		}
		public void AddDeltaDescriptor(Measure actualValue, Measure targetValue, NamedDataItemContainer map, DeltaOptions deltaOptions) {
			metadata.DeltaDescriptors.Add(CreateDeltaDescriptor(actualValue, targetValue, map, deltaOptions));
		}
		public void AddFormatConditionMeasureDescriptors(IFormatRuleCollection formatRules) {
			if(formatRules != null && formatRules.Count > 0) {
				foreach(DashboardItemFormatRule rule in formatRules) {
					if(!rule.Checked)
						continue;
					string formatConditionMeasureId = FormatConditionMeasureDescriptorIdManager.GetFormatConditionMeasureDescriptorId(rule.Name);
					CreateAndAddDescriptor(formatConditionMeasureId, new ValueFormatViewModel(), metadata.FormatConditionMeasureDescriptors);
					if(rule.IsBarAggregationsRequired) {
						string normalizedValueMeasureId = FormatConditionMeasureDescriptorIdManager.GetNormalizedValueMeasureDescriptorId(rule.LevelCore);
						CreateAndAddDescriptor(normalizedValueMeasureId, new ValueFormatViewModel(), metadata.NormalizedValueMeasureDescriptors);
						string zeroPositionMeasureId = FormatConditionMeasureDescriptorIdManager.GetZeroPositionMeasureDescriptorId(rule.LevelCore);
						CreateAndAddDescriptor(zeroPositionMeasureId, new ValueFormatViewModel(), metadata.ZeroPositionMeasureDescriptors);
					}
				}
			}
		}
		public void AddColorMeasureDescriptors(DataDashboardItem item) {
			if(!item.ColorMeasuresByHue) {
				metadata.ColorMeasureDescriptors.Add(new MeasureDescriptorInternal() {
					ID = DataDashboardItem.ColorMeasure,
					Name = null,
					Format = new ValueFormatViewModel(),
				});
			} else {
				foreach(KeyValuePair<string, string> pair in item.ColorMeasureDescriptorsInfo)
					metadata.ColorMeasureDescriptors.Add(new MeasureDescriptorInternal() {
						ID = DataDashboardItem.CorrectColorMeasureId(pair.Key),
						Name = pair.Value,
						Format = new ValueFormatViewModel(),
					});
			}
		}
		public void SetColumnHierarchyDimensions(string hierarchyName, IList<Dimension> dimensions) {
			metadata.ColumnHierarchy = hierarchyName;
			metadata.DimensionDescriptors.Add(hierarchyName, new DimensionDescriptorInternalCollection(dimensions.Select(s => CreateDimensionDescriptor(s)).ToList()));
		}
		public void SetRowHierarchyDimensions(string hierarchyName, IList<Dimension> dimensions) {
			metadata.RowHierarchy = hierarchyName;
			metadata.DimensionDescriptors.Add(hierarchyName, new DimensionDescriptorInternalCollection(dimensions.Select(s => CreateDimensionDescriptor(s)).ToList()));
		}
		public HierarchicalMetadata CreateMetadata() {
			HierarchicalMetadata result = metadata;
			metadata = null;
			return result;
		}
		void EnsureSliceQuery() {
			if(query == null)
				query = getSliceDataQuery();
		}
		DimensionDescriptorInternal CreateDimensionDescriptor(Dimension dimension) {
			DimensionDescriptorInternal dimensionDescriptor = new DimensionDescriptorInternal();
			dimensionDescriptor.ID = dimension.ActualId;
			dimensionDescriptor.Name = dimension.GroupName;
			dimensionDescriptor.Level = GetDimensionAxisIndex(dimension);
			dimensionDescriptor.DateTimeGroupInterval = dimension.DateTimeGroupInterval;
			dimensionDescriptor.TextGroupInterval = dimension.TextGroupInterval;
			dimensionDescriptor.DataMember = dimension.DataMember;
			dimensionDescriptor.Format = dimension.CreateValueFormatViewModel();
			return dimensionDescriptor;
		}
		int GetDimensionAxisIndex(Dimension dimension) {
			EnsureSliceQuery();
			string id = dimension.ActualId;
			int index = query.Axis1.ToList().FindIndex(d => d.Name == id);
			if(index != -1)
				return index;
			return query.Axis2.ToList().FindIndex(d => d.Name == id);
		}
		MeasureDescriptorInternal CreateMeasureDescriptor(Measure measure) {
			return new MeasureDescriptorInternal() {
				ID = measure.ActualId,
				Name = measure.GroupName,
				SummaryType = measure.SummaryType,
				DataMember = measure.DataMember,
				Format = measure.CreateValueFormatViewModel()
			};
		}
		DeltaDescriptorInternal CreateDeltaDescriptor(Measure actualValue, Measure targetValue, NamedDataItemContainer deltaContainer, DeltaOptions deltaOptions) {
			DeltaDescriptorInternal deltaDescriptorInternal = new DeltaDescriptorInternal();
			deltaDescriptorInternal.DeltaValueType = deltaOptions.ValueType;
			string actualValueName = actualValue.ActualId;
			string targetValueName = targetValue.ActualId;
			deltaDescriptorInternal.AbsoluteVariationID = GenerateDeltaFieldName(actualValueName, targetValueName, AbsoluteVariationPivotFieldName);
			deltaDescriptorInternal.PercentVariationID = GenerateDeltaFieldName(actualValueName, targetValueName, PercentVariationPivotFieldName);
			deltaDescriptorInternal.PercentOfTargetID = GenerateDeltaFieldName(actualValueName, targetValueName, PercentOfTargetPivotFieldName);
			deltaDescriptorInternal.IsGoodID = GenerateDeltaFieldName(actualValueName, targetValueName, DeltaIsGoodPivotFieldNameFormat);
			deltaDescriptorInternal.IndicatorTypeID = GenerateDeltaFieldName(actualValueName, targetValueName, DeltaIndicatorTypePivotFieldNameFormat);
			deltaDescriptorInternal.ActualValueID = actualValueName;
			deltaDescriptorInternal.TargetValueID = targetValueName;
			deltaDescriptorInternal.ActualMeasureID = deltaDescriptorInternal.ID = actualValue.ActualId;
			deltaDescriptorInternal.TargetMeasureID = targetValue.ActualId;
			deltaDescriptorInternal.Name = deltaContainer.DisplayName;
			CreateDeltaDescriptorFormat(deltaDescriptorInternal, actualValue);
			return deltaDescriptorInternal;
		}
		void CreateDeltaDescriptorFormat(DeltaDescriptorInternal deltaDescriptor, Measure measure) {
			deltaDescriptor.AbsoluteVariationFormat = measure.CreateKpiFormatViewModel(DeltaValueType.AbsoluteVariation);
			deltaDescriptor.ActualValueFormat = measure.CreateKpiFormatViewModel(DeltaValueType.ActualValue);
			deltaDescriptor.PercentOfTargetFormat = measure.CreateKpiFormatViewModel(DeltaValueType.PercentOfTarget);
			deltaDescriptor.PercentVariationFormat = measure.CreateKpiFormatViewModel(DeltaValueType.PercentVariation);
		}
		string GenerateDeltaFieldName(string actualValueName, string targetValueName, string fieldNameFormat) {
			return string.Format(fieldNameFormat, actualValueName, targetValueName);
		}
		void CreateAndAddDescriptor(string measureId, ValueFormatViewModel format, MeasureDescriptorInternalCollection collection) {
			MeasureDescriptorInternal md = collection.FirstOrDefault<MeasureDescriptorInternal>((d) => {
				return Equals(d.ID, measureId);
			});
			if(md == null) {
				collection.Add(new MeasureDescriptorInternal() {
					ID = measureId,
					Name = null,
					Format = format,
				});
			}
		}
	}
	public class HierarchicalMetadata {
		public Dictionary<string, DimensionDescriptorInternalCollection> DimensionDescriptors { get; private set; }
		public MeasureDescriptorInternalCollection MeasureDescriptors { get; private set; }
		public MeasureDescriptorInternalCollection ColorMeasureDescriptors { get; private set; }
		public MeasureDescriptorInternalCollection FormatConditionMeasureDescriptors { get; private set; }
		public MeasureDescriptorInternalCollection NormalizedValueMeasureDescriptors { get; private set; }
		public MeasureDescriptorInternalCollection ZeroPositionMeasureDescriptors { get; private set; }
		public DeltaDescriptorInternalCollection DeltaDescriptors { get; private set; }
		public List<string> DataSourceColumns { get; set; }
		public string ColumnHierarchy { get; set; }
		public string RowHierarchy { get; set; }
		public HierarchicalMetadata() {
			DimensionDescriptors = new Dictionary<string, DimensionDescriptorInternalCollection>();
			MeasureDescriptors = new MeasureDescriptorInternalCollection();
			ColorMeasureDescriptors = new MeasureDescriptorInternalCollection();
			FormatConditionMeasureDescriptors = new MeasureDescriptorInternalCollection();
			NormalizedValueMeasureDescriptors = new MeasureDescriptorInternalCollection();
			ZeroPositionMeasureDescriptors = new MeasureDescriptorInternalCollection();
			DeltaDescriptors = new DeltaDescriptorInternalCollection();
		}
	}
	public class DimensionDescriptorInternal {
		public string ID { get; set; }
		public string Name { get; set; }
		public int Level { get; set; }
		public string DataMember { get; set; }
		public DateTimeGroupInterval DateTimeGroupInterval { get; set; }
		public TextGroupInterval TextGroupInterval { get; set; }
		public ValueFormatViewModel Format { get; set; }
	}
	public class MeasureDescriptorInternal {
		public string ID { get; set; }
		public string Name { get; set; }
		public string DataMember { get; set; }
		public SummaryType SummaryType { get; set; }
		public ValueFormatViewModel Format { get; set; }
	}
	public class DeltaDescriptorInternal {
		public string ID { get; set; }
		public string Name { get; set; }
		public string ActualMeasureID { get; set; }
		public string TargetMeasureID { get; set; }
		public string ActualValueID { get; set; }
		public string TargetValueID { get; set; }
		public string AbsoluteVariationID { get; set; }
		public string PercentVariationID { get; set; }
		public string PercentOfTargetID { get; set; }
		public string IsGoodID { get; set; }
		public string IndicatorTypeID { get; set; }
		public DeltaValueType DeltaValueType { get; set; }
		public NumericFormatViewModel ActualValueFormat { get; set; }
		public NumericFormatViewModel AbsoluteVariationFormat { get; set; }
		public NumericFormatViewModel PercentVariationFormat { get; set; }
		public NumericFormatViewModel PercentOfTargetFormat { get; set; }
		public NumericFormatViewModel DisplayFormat {
			get {
				switch(DeltaValueType) {
					case DashboardCommon.DeltaValueType.AbsoluteVariation:
						return AbsoluteVariationFormat;
					case DashboardCommon.DeltaValueType.PercentOfTarget:
						return PercentOfTargetFormat;
					case DashboardCommon.DeltaValueType.PercentVariation:
						return PercentVariationFormat;
					default:
						return ActualValueFormat;
				}
			}
		}
	}
	public class SparklineDescriptorInternal {
		public string MeasureID { get; set; }
	}
	public class DimensionDescriptorInternalCollection : List<DimensionDescriptorInternal> {
		public DimensionDescriptorInternalCollection(List<DimensionDescriptorInternal> list) : base(list) { }
		public DimensionDescriptorInternalCollection() : base() { }
	}
	public class MeasureDescriptorInternalCollection : List<MeasureDescriptorInternal> {
		public MeasureDescriptorInternalCollection(List<MeasureDescriptorInternal> list) : base(list) { }
		public MeasureDescriptorInternalCollection() : base() { }
	}
	public class DeltaDescriptorInternalCollection : List<DeltaDescriptorInternal> {
		public DeltaDescriptorInternalCollection(List<DeltaDescriptorInternal> list) : base(list) { }
		public DeltaDescriptorInternalCollection() : base() { }
	}
}
