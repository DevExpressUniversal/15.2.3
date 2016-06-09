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

namespace DevExpress.PivotGrid.OLAP {
	public static class OlapProperty {
		public const string AncestorNames = "ANCESTOR_NAMES";
		public const string AttributeName = "ATTRIBUTE_NAME";
		public const string AllMember = "ALL_MEMBER";
		public const string DefaultMember = "DEFAULT_MEMBER";
		public const string DefaultFormatString = "DEFAULT_FORMAT_STRING";
		public const string DataType = "DATA_TYPE";
		public const string MeasureDisplayFolder = "MEASURE_DISPLAY_FOLDER";
		public const string Structure = "STRUCTURE";
		public const string CellFormatString = "FORMAT_STRING";
		public const string LANGUAGE = "LANGUAGE";
		public const string LanguageProperty = "Language";
		public const string Catalog = "Catalog";
		public const string Cube = "Cube";
		public const string Timeout = "Timeout";
		public const string Format = "Format";
		public const string AxisFormat = "AxisFormat";
		public const string Content = "Content";
		public const string DBMSVersion = "DBMSVersion";
		public const string LocaleIdentifier = "LocaleIdentifier";
		public const string PropertyName = "PropertyName";
		public const string Roles = "Roles";
		public const string CustomData = "CustomData";
		public const string CaptionProperty = "Caption";
		public const string DisplayInfo = "DisplayInfo";
		public const string FormattedValue = "FmtValue";
		public const string LName = "LName";
		public const string LNumber = "LNum";
		public const string UName = "UName";
		public const string Value = "Value";
		public const string FormatString = "FormatString";
		public const string SchemaPropertyName = "PROPERTY_NAME";
		public const string CubeCaption = "CUBE_CAPTION";
		public const string CubeType = "CUBE_TYPE";
		public const string LastSchemaUpdate = "LAST_SCHEMA_UPDATE";
		public const string LastDataUpdate = "LAST_DATA_UPDATE";
		public const string DimensionCaption = "DIMENSION_CAPTION";
		public const string DimensionName = "DIMENSION_NAME";
		public const string DimensionType = "DIMENSION_TYPE";
		public const string HierarchyName = "HIERARCHY_NAME";
		public const string HierarchyCaption = "HIERARCHY_CAPTION";
		public const string HierarchyDisplayFolder = "HIERARCHY_DISPLAY_FOLDER";
		public const string HierarchyOrigin = "HIERARCHY_ORIGIN";
		public const string LevelName = "LEVEL_NAME";
		public const string LevelNameSQLColumnName = "LEVEL_NAME_SQL_COLUMN_NAME";
		public const string LevelCaption = "LEVEL_CAPTION";
		public const string LevelCardinality = "LEVEL_CARDINALITY";
		public const string LevelDbType = "LEVEL_DBTYPE";
		public const string LevelType = "LEVEL_TYPE";
		public const string LevelOrdering = "LEVEL_ORDERING_PROPERTY";
		public const string LevelKeyCardinality = "LEVEL_KEY_CARDINALITY";
		public const string MeasureGroupName = "MEASUREGROUP_NAME";
		public const string MeasureGroupCaption = "MEASUREGROUP_CAPTION";
		public const string MeasureName = "MEASURE_NAME";
		public const string MeasureNameSQLColumnName = "MEASURE_NAME_SQL_COLUMN_NAME";
		public const string MeasureUniqueName = "MEASURE_UNIQUE_NAME";
		public const string MeasureCaption = "MEASURE_CAPTION";
		public const string MeasureAggregator = "MEASURE_AGGREGATOR";
		public const string KpiName = "KPI_NAME";
		public const string KpiCaption = "KPI_CAPTION";
		public const string KpiValue = "KPI_VALUE";
		public const string KpiGoal = "KPI_GOAL";
		public const string KpiStatus = "KPI_STATUS";
		public const string KpiTrend = "KPI_TREND";
		public const string KpiWeight = "KPI_WEIGHT";
		public const string KpiStatusGraphic = "KPI_STATUS_GRAPHIC";
		public const string KpiTrendGraphic = "KPI_TREND_GRAPHIC";
		public const string KpiDisplayFolder = "KPI_DISPLAY_FOLDER";
		public const string KpiDescription = "KPI_DESCRIPTION";
		public const string ActionType = "ACTION_TYPE";
		public const string CAPTION = "CAPTION";
		public const string CatalogName = "CATALOG_NAME";
		public const string ChildrenCardinality = "CHILDREN_CARDINALITY";
		public const string CubeName = "CUBE_NAME";
		public const string CustomRollup = "CUSTOM_ROLLUP";
		public const string CustomRollupProperties = "CUSTOM_ROLLUP_PROPERTIES";
		public const string Description = "DESCRIPTION";
		public const string DimensionUniqueName = "DIMENSION_UNIQUE_NAME";
		public const string Expression = "EXPRESSION";
		public const string HierarchyUniqueName = "HIERARCHY_UNIQUE_NAME";
		public const string IsDataMember = "IS_DATAMEMBER";
		public const string IsPlaceHolderMember = "IS_PLACEHOLDERMEMBER";
		public const string LevelNumber = "LEVEL_NUMBER";
		public const string LevelUniqueName = "LEVEL_UNIQUE_NAME";
		public const string MemberCaption = "MEMBER_CAPTION";
		public const string MemberGuid = "MEMBER_GUID";
		public const string MemberKey = "MEMBER_KEY";
		public const string MemberName = "MEMBER_NAME";
		public const string MemberOrdinal = "MEMBER_ORDINAL";
		public const string MEMBERTYPE = "MEMBER_TYPE";
		public const string MemberUniqueName = "MEMBER_UNIQUE_NAME";
		public const string MEMBERVALUE = "MEMBER_VALUE";
		public const string ParentCount = "PARENT_COUNT";
		public const string ParentLevel = "PARENT_LEVEL";
		public const string ParentUniqueName = "PARENT_UNIQUE_NAME";
		public const string SchemaName = "SCHEMA_NAME";
		public const string Scope = "SCOPE";
		public const string SkippedLevels = "SKIPPED_LEVELS";
		public const string UnaryOperator = "UNARY_OPERATOR";
	}
	public class OlapSchema {
		public const string Catalogs = "DBSCHEMA_CATALOGS";
		public const string Cubes = "MDSCHEMA_CUBES";
		public const string Dimensions = "MDSCHEMA_DIMENSIONS";
		public const string Hierarchies = "MDSCHEMA_HIERARCHIES";
		public const string Measures = "MDSCHEMA_MEASURES";
		public const string MeasureGroups = "MDSCHEMA_MEASUREGROUPS";
		public const string KPIs = "MDSCHEMA_KPIS";
		public const string Members = "MDSCHEMA_MEMBERS";
		public const string Levels = "MDSCHEMA_LEVELS";
		public const string Sets = "MDSCHEMA_SET";
		public const string Properties = "MDSCHEMA_PROPERTIES";
		public const string DiscoverProperties = "DISCOVER_PROPERTIES";
	}
}
