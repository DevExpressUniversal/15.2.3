#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{                                                                   }
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
using DevExpress.PivotGrid.OLAP.SchemaEntities;
using DevExpress.PivotGrid.QueryMode;
using DevExpress.XtraPivotGrid;
using DevExpress.XtraPivotGrid.Data;
using DevExpress.XtraPivotGrid.Localization;
using System.Collections;
using DevExpress.Compatibility.System;
namespace DevExpress.PivotGrid.OLAP {
	public static class OLAPMetadataHelper {
		static readonly string[] IntrinsicMemberProperties = new string[] {
			"ACTION_TYPE", "CAPTION", "CATALOG_NAME", "CHILDREN_CARDINALITY",
			"CUBE_NAME", "CUSTOM_ROLLUP", "CUSTOM_ROLLUP_PROPERTIES", "DESCRIPTION",
			"DIMENSION_UNIQUE_NAME", "EXPRESSION", "HIERARCHY_UNIQUE_NAME", "IS_DATAMEMBER", "IS_PLACEHOLDERMEMBER",
			"LEVEL_NUMBER", "LEVEL_UNIQUE_NAME", "MEMBER_CAPTION", "MEMBER_GUID",
			"MEMBER_KEY", "MEMBER_NAME", "MEMBER_ORDINAL", 
			"MEMBER_TYPE", "MEMBER_UNIQUE_NAME", "MEMBER_VALUE", "PARENT_COUNT", "PARENT_LEVEL",
			"PARENT_UNIQUE_NAME", "SCHEMA_NAME", "SCOPE", "SKIPPED_LEVELS", "UNARY_OPERATOR" };
		public static bool IsIntrinsicMemberProperty(string propertyName) {
			if(Array.BinarySearch<string>(IntrinsicMemberProperties, propertyName) >= 0)
				return true;
			if(propertyName == "KEY" || propertyName == "Key" || propertyName.StartsWith("LCID") || propertyName == "ID")
				return true;
			int res;
			if((propertyName.StartsWith("KEY") || propertyName.StartsWith("Key")) && Int32.TryParse(propertyName.Substring(3), out res))
				return true;
			return false;
		}
		public static bool IsAS2000(string serverVersion) {
			return serverVersion.StartsWith("8.");
		}
		public static bool IsAS2005(string serverVersion) {
			return serverVersion.StartsWith("9.");
		}
		internal static void PopulateKPIMeasure(OLAPMetadataColumns cubeColumns, string uniqueName, string name, string caption,
			OLAPHierarchy measures, string graphic, PivotKPIType type, string measureGroupCaption, string displayFolder) {
			if(string.IsNullOrEmpty(uniqueName))
				return;
			OLAPKPIMetadataColumn metadata;
			if(cubeColumns[uniqueName] != null)
				metadata = new OLAPKPIMeasureMetadataColumn(0, 0, OLAPDataTypeConverter.Convert(OLAPDataType.Variant), null,
					new OLAPHierarchy(string.Format("[Measures].[{0}]", caption), name, caption, null), null, null, measures, OLAPDataType.Variant, graphic, type, displayFolder, uniqueName);
			else
				metadata = new OLAPKPIMetadataColumn(0, 0, OLAPDataTypeConverter.Convert(OLAPDataType.Variant), null,
					new OLAPHierarchy(uniqueName, name, caption, null), null, null, measures, OLAPDataType.Variant, graphic, type, displayFolder);
			cubeColumns.Add(metadata);
		}
	}
	public class OLAPMetadataPopulator {
		readonly OLAPMetadata metadata;
		MeasureGroupsRowSet measureGroupsRowSet;
		protected internal static string CombineDisplayFolder(string displayFolder, string measureGroupCaption) {
			if(string.IsNullOrEmpty(displayFolder))
				return measureGroupCaption;
			if(string.IsNullOrEmpty(measureGroupCaption))
				return displayFolder;
			return measureGroupCaption + "\\" + displayFolder;
		}
		public OLAPMetadataPopulator(OLAPMetadata metadata) {
			this.metadata = metadata;
		}
		public void PopulateColumns(OLAPHierarchies hierarchies, OLAPMetadataColumns cubeColumns, List<PivotOLAPKPIMeasures> kpis) {
			Dictionary<string, object> restrictions = new Dictionary<string, object>();
			restrictions.Add(OlapProperty.CatalogName, metadata.CatalogName);
			restrictions.Add(OlapProperty.CubeName, metadata.CubeName);
			IOLAPRowSet cubesSet = metadata.GetShemaRowSet(OlapSchema.Cubes, restrictions);
			if(!cubesSet.NextRow())
				return;
			ColumnIndexer indexer = new ColumnIndexer(cubesSet);
			string catalog = cubesSet.GetCellValue(indexer.GetIndex(OlapProperty.CubeName)).ToString();
			int captionIndex = indexer.GetIndex(OlapProperty.CubeCaption);
			string catalogCaption = null;
			if(captionIndex >= 0)
				catalogCaption = (cubesSet.GetCellValue(captionIndex) ?? string.Empty).ToString();
			if(string.IsNullOrEmpty(catalogCaption))
				catalogCaption = catalog;
			metadata.CubeCaption = catalogCaption;
			if(cubesSet.NextRow()) { 
				if(string.IsNullOrEmpty(metadata.CubeName))
					metadata.CubeName = catalog;
				while(cubesSet.NextRow())
					return;
			}
			PopulateDimensions(hierarchies, cubeColumns, restrictions);
			OLAPHierarchy measures = new OLAPHierarchy(OLAPHierarchy.MeasuresHierarchyUniqueName, "Measures",
				PivotGridLocalizer.GetString(PivotGridStringId.OLAPMeasuresCaption));
			hierarchies.Add(measures);
			OLAPHierarchy kpisMeasures = new OLAPHierarchy(OLAPHierarchy.KPIsHierarchyUniqueName, "KPIs",
				PivotGridLocalizer.GetString(PivotGridStringId.OLAPKPIsCaption));
			hierarchies.Add(kpisMeasures);
			if(!IsAs2000)
				measureGroupsRowSet = new MeasureGroupsRowSet(metadata.GetShemaRowSet(OlapSchema.MeasureGroups, restrictions));
			PopulateMeasures(measures, cubeColumns, restrictions);
			if(IsAs2000)
				return;
			KpisRowSet kpisRowSet = new KpisRowSet(metadata.GetShemaRowSet(OlapSchema.KPIs, restrictions));
			while(kpisRowSet.Read()) {
				PopulateKpi(kpis, kpisRowSet, cubeColumns, measures);
			}
		}
		string FixDBNull(object value) {
			if(value is DBNull)
				return null;
			return (string)value;
		}
		void PopulateDimensions(OLAPHierarchies hierarchies, OLAPMetadataColumns cubeColumns, Dictionary<string, object> restrictions) {
			DimensionsRowSet dimensionsRowSet = new DimensionsRowSet(metadata.GetShemaRowSet(OlapSchema.Dimensions, restrictions));
			Dictionary<string, OlapDimension> dimensions = new Dictionary<string, OlapDimension>();
			while(dimensionsRowSet.Read()) {
				OlapDimension dim = new OlapDimension(
					   dimensionsRowSet.Name,
					   dimensionsRowSet.UniqueName,
					   dimensionsRowSet.Caption,
					   dimensionsRowSet.Description,
					   dimensionsRowSet.Type);
				dimensions.Add(dim.UniqueName, dim);
			}
			HierarchiesRowSet hierarchiesRowSet = new HierarchiesRowSet(metadata.GetShemaRowSet(OlapSchema.Hierarchies, restrictions));
			while(hierarchiesRowSet.Read()) {
				OlapDimension parentDimension = dimensions[hierarchiesRowSet.DimensionUniqueName];
				if(parentDimension == null)
					return;
				parentDimension.Hierarchies.Add(new OlapHierarchy(parentDimension,
																  hierarchiesRowSet.Name,
																  hierarchiesRowSet.UniqueName,
																  hierarchiesRowSet.Caption,
																  hierarchiesRowSet.Description,
																  hierarchiesRowSet.DisplayFolder,
																  hierarchiesRowSet.DefaultMember,
																  hierarchiesRowSet.AllMember,
																  hierarchiesRowSet.Structure,
																  hierarchiesRowSet.Origin));
			}
			PropertiesRowSet propertiesRowSet = new PropertiesRowSet(metadata.GetShemaRowSet(OlapSchema.Properties, restrictions));
			Dictionary<string, Dictionary<string, OLAPDataType>> levelProps = new Dictionary<string, Dictionary<string, OLAPDataType>>();
			while(propertiesRowSet.Read()) {
				Dictionary<string, OLAPDataType> dic;
				string uname = propertiesRowSet.LevelUniqueNameIndex;
				if(!levelProps.TryGetValue(uname, out dic)) {
					dic = new Dictionary<string, OLAPDataType>();
					levelProps.Add(uname, dic);
				}
				dic.Add(propertiesRowSet.PropertyName, (OLAPDataType)Convert.ToInt32(propertiesRowSet.DataType));
			}
			Dictionary<string, object> levelCalculatedMembersRestrictions = new Dictionary<string, object>();
			levelCalculatedMembersRestrictions.Add(OlapProperty.CatalogName, metadata.CatalogName);
			levelCalculatedMembersRestrictions.Add(OlapProperty.CubeName, metadata.CubeName);
			levelCalculatedMembersRestrictions.Add(OlapProperty.MEMBERTYPE, (int)OLAPMemberType.Formula);
			MembersRowSet membersRowSet = new MembersRowSet(metadata.GetShemaRowSet(OlapSchema.Members, levelCalculatedMembersRestrictions));
			Dictionary<string, List<CalculatedMemberSource>> calculatedMembers = new Dictionary<string, List<CalculatedMemberSource>>();
			while(membersRowSet.Read()) {
				string levelName = membersRowSet.LevelUniqueName;
				if(levelName == null)
					continue;
				List<CalculatedMemberSource> members;
				if(!calculatedMembers.TryGetValue(levelName, out members)) {
					members = new List<CalculatedMemberSource>();
					calculatedMembers[levelName] = members;
				}
				members.Add(new CalculatedMemberSource(
					membersRowSet.UniqueName,
					membersRowSet.Name,
					levelName));
			}
			LevelsRowSet levelsRowSet = new LevelsRowSet(metadata.GetShemaRowSet(OlapSchema.Levels, restrictions));
			while(levelsRowSet.Read()) {
				OlapDimension dim = dimensions[levelsRowSet.DimensionUniqueName];
				OlapHierarchy hierarchy = dim == null ? null : dim.Hierarchies[levelsRowSet.HierarchyUniqueName];
				if(hierarchy == null)
					return;
				string uniqueName = levelsRowSet.UniqueName;
				List<CalculatedMemberSource> calculated;
				calculatedMembers.TryGetValue(uniqueName, out calculated);
				Dictionary<string, OLAPDataType> props;
				if(!levelProps.TryGetValue(uniqueName, out props))
					props = new Dictionary<string,OLAPDataType>();
				hierarchy.Levels.Add(new OlapLevel(hierarchy,
					 levelsRowSet.Name,
					 uniqueName,
					 levelsRowSet.Caption,
					 levelsRowSet.Description,
					 levelsRowSet.NameSQLColumnName,
					 levelsRowSet.Number,
					 levelsRowSet.Cardinality,
					 levelsRowSet.Type,
					 props,
					 levelsRowSet.KeyCardinality,
					 levelsRowSet.Ordering,
					 calculated
				   ));
			}
			foreach(KeyValuePair<string, OlapDimension> pair in dimensions)
				PopulateHierarchies(pair.Value, hierarchies, cubeColumns);
		}
		void PopulateHierarchies(IOLAPDimension dim, OLAPHierarchies hierarchies, OLAPMetadataColumns cubeColumns) {
			if(dim.DimensionType == OLAPDimensionType.Measure)
				return;
			hierarchies.Add(new OLAPHierarchy(dim.UniqueName, dim.Name, dim.Caption));
			foreach(IOLAPHierarchy hrchy in dim.Hierarchies) {
				OLAPHierarchy hierarchy = new OLAPHierarchy(hrchy.UniqueName, hrchy.Name, hrchy.Caption, hrchy.DisplayFolder, hrchy.Structure, hrchy.Origin);
				hierarchies.Add(hierarchy);
				PopulateHierarchyLevels(hierarchy, hrchy, cubeColumns);
			}
		}
		void PopulateHierarchyLevels(OLAPHierarchy hierarchy, IOLAPHierarchy hrchy, OLAPMetadataColumns cubeColumns) {
			OLAPMetadataColumn parent = null;
			string allMember = hrchy.AllMember;
			foreach(IOLAPLevel level in hrchy.Levels) {
				if(level.LevelType == OLAPLevelType.All)
					continue;
				if(level.Cardinality > Int32.MaxValue)
					throw new OLAPException(OLAPException.LevelHasTooManyMembersException);
				OLAPMetadataColumn metadata = new OLAPMetadataColumn(
					level.LevelNumber, Convert.ToInt32(level.Cardinality), parent,
					new OLAPHierarchy(level.UniqueName, level.Name, level.Caption, null), GetDrillDownColumn(level), hrchy.DefaultMember, hierarchy,
					level.DefaultSortProperty, level.KeyCount, level.Properties);
				metadata.AllMember = !string.IsNullOrEmpty(allMember) ? new OLAPMember(metadata, allMember, null) : null;
				parent = metadata;
				if(level.CalculatedMembers != null)
					foreach(CalculatedMemberSource source in level.CalculatedMembers)
						source.GetMember(metadata);
				cubeColumns.Add(metadata);
			}
		}
		bool IsAs2000 { get { return metadata.IsAS2000; } }
		void PopulateKpi(List<PivotOLAPKPIMeasures> kpis, KpisRowSet kpi, OLAPMetadataColumns cubeColumns, OLAPHierarchy measures) {
			PivotOLAPKPIMeasures kpiMeasures = new PivotOLAPKPIMeasures(kpi);
			kpis.Add(kpiMeasures);
			string measureGroupCaption = GetMeasureGroupCaption(kpi.MeasureGroupName);
			string name = kpi.Name;
			string caption = kpi.Caption;
			string displayFolder = kpi.DisplayFolder;
			OLAPMetadataHelper.PopulateKPIMeasure(cubeColumns, kpiMeasures.ValueMeasure, name, caption + " " + PivotGridLocalizer.GetString(PivotGridStringId.OLAPKPITypeValue), measures, null, PivotKPIType.Value, measureGroupCaption, displayFolder);
			OLAPMetadataHelper.PopulateKPIMeasure(cubeColumns, kpiMeasures.GoalMeasure, name, caption + " " + PivotGridLocalizer.GetString(PivotGridStringId.OLAPKPITypeGoal), measures, null, PivotKPIType.Goal, measureGroupCaption, displayFolder);
			OLAPMetadataHelper.PopulateKPIMeasure(cubeColumns, kpiMeasures.StatusMeasure, name, caption + " " + PivotGridLocalizer.GetString(PivotGridStringId.OLAPKPITypeStatus), measures, kpi.StatusGraphic, PivotKPIType.Status, measureGroupCaption, displayFolder);
			OLAPMetadataHelper.PopulateKPIMeasure(cubeColumns, kpiMeasures.TrendMeasure, name, caption + " " + PivotGridLocalizer.GetString(PivotGridStringId.OLAPKPITypeTrend), measures, kpi.TrendGraphic, PivotKPIType.Trend, measureGroupCaption, displayFolder);
			OLAPMetadataHelper.PopulateKPIMeasure(cubeColumns, kpiMeasures.WeightMeasure, name, caption + " " + PivotGridLocalizer.GetString(PivotGridStringId.OLAPKPITypeWeight), measures, null, PivotKPIType.Weight, measureGroupCaption, displayFolder);
		}
		string GetMeasureGroupCaption(string name) {
			if(name == null || measureGroupsRowSet == null)
				return null;
			return measureGroupsRowSet.GetCaption(name);
		}
		void PopulateMeasures(OLAPHierarchy hierarchy, OLAPMetadataColumns cubeColumns, Dictionary<string, object> restrictions) {
			MeasuresRowSet measuresRowSet = new MeasuresRowSet(metadata.GetShemaRowSet(OlapSchema.Measures, restrictions));
			while(measuresRowSet.Read()) {
				OLAPMetadataColumn metadataColumn = new OLAPMetadataColumn(0, 0, OLAPDataTypeConverter.Convert((OLAPDataType)measuresRowSet.DataType), null,
					new OLAPHierarchy(measuresRowSet.UniqueName, measuresRowSet.Name, measuresRowSet.Caption, CombineDisplayFolder(measuresRowSet.DisplayFolder, GetMeasureGroupCaption(measuresRowSet.MeasureGroupName))),
					GetDrillDownColumn(measuresRowSet), null, hierarchy, (OLAPDataType)measuresRowSet.DataType, measuresRowSet.DefaultFormatString, measuresRowSet.Expression);
				cubeColumns.Add(metadataColumn);
			}
		}
		string GetDrillDownColumn(IOLAPLevel level) {
			if(IsAs2000)
				return null;
			string columnName = level.DrillDownColumn;
			if(columnName.StartsWith("NAME")) {
				int position = columnName.IndexOf("(") + 1;
				if(position == 0)
					return columnName;
				columnName = columnName.Substring(position, columnName.Length - position - 1).Trim();
			}
			return columnName;
		}
		string GetDrillDownColumn(MeasuresRowSet measure) {
			if(IsAs2000)
				return null;   
			if(string.IsNullOrEmpty(measure.MeasureGroupName) || string.IsNullOrEmpty(measure.NameSQLColumnName) || !measureGroupsRowSet.ContainsKey(measure.MeasureGroupName))
				return null;
			return String.Format("[{0}].[{1}]", measure.MeasureGroupName, measure.NameSQLColumnName);
		}
	}
}
