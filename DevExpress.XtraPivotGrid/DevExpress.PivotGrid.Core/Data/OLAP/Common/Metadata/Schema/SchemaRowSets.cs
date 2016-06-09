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

using DevExpress.Compatibility.System;
using System;
using System.Collections.Generic;
namespace DevExpress.PivotGrid.OLAP.SchemaEntities {
	class ColumnIndexer {
		Dictionary<string, int> indexes = new Dictionary<string, int>();
		public ColumnIndexer(IOLAPRowSet set) {
			for(int i = 0; i < set.ColumnCount; i++)
				indexes.Add(set.GetColumnName(i), i);
		}
		internal int GetIndex(string columnName) {
			int index;
			if(indexes.TryGetValue(columnName, out index))
				return index;
			return -1;
		}
	}
	abstract class SchemsRowSet {
		protected readonly IOLAPRowSet set;
		protected SchemsRowSet(IOLAPRowSet set) {
			this.set = set;
		}
		public bool Read() {
			return set.NextRow();
		}
		protected string FixDBNull(object value) {
			if(value is DBNull || value == null)
				return null;
			return value.ToString();
		}
	}
	class DimensionsRowSet : SchemsRowSet {
		readonly int nameIndex;
		readonly int uniqueNameIndex;
		readonly int captionIndex;
		readonly int descriptionIndex;
		readonly int typeIndex;
		public DimensionsRowSet(IOLAPRowSet set)
			: base(set) {
			ColumnIndexer indexer = new ColumnIndexer(set);
			nameIndex = indexer.GetIndex(OlapProperty.DimensionName);
			uniqueNameIndex = indexer.GetIndex(OlapProperty.DimensionUniqueName);
			captionIndex = indexer.GetIndex(OlapProperty.DimensionCaption);
			descriptionIndex = indexer.GetIndex(OlapProperty.Description);
			typeIndex = indexer.GetIndex(OlapProperty.DimensionType);
		}
		public string Name { get { return set.GetCellStringValue(nameIndex); } }
		public string UniqueName { get { return set.GetCellStringValue(uniqueNameIndex); } }
		public string Caption { get { return set.GetCellStringValue(captionIndex); } }
		public string Description { get { return FixDBNull(set.GetCellValue(descriptionIndex)); } }
		public int Type { get { return Convert.ToInt32(set.GetCellValue(typeIndex)); } }
	}
	class HierarchiesRowSet : SchemsRowSet {
		readonly int dimensionUniqueNameIndex;
		readonly int nameIndex;
		readonly int uniqueNameIndex;
		readonly int captionIndex;
		readonly int descriptionIndex;
		readonly int displayFolderIndex;
		readonly int defaultMemberIndex;
		readonly int allMemberIndex;
		readonly int structureIndex;
		readonly int originIndex;
		public HierarchiesRowSet(IOLAPRowSet set)
			: base(set) {
			ColumnIndexer indexer = new ColumnIndexer(set);
			dimensionUniqueNameIndex = indexer.GetIndex(OlapProperty.DimensionUniqueName);
			nameIndex = indexer.GetIndex(OlapProperty.HierarchyName);
			uniqueNameIndex = indexer.GetIndex(OlapProperty.HierarchyUniqueName);
			captionIndex = indexer.GetIndex(OlapProperty.HierarchyCaption);
			descriptionIndex = indexer.GetIndex(OlapProperty.Description);
			displayFolderIndex = indexer.GetIndex(OlapProperty.HierarchyDisplayFolder);
			defaultMemberIndex = indexer.GetIndex(OlapProperty.DefaultMember);
			allMemberIndex = indexer.GetIndex(OlapProperty.AllMember);
			structureIndex = indexer.GetIndex(OlapProperty.Structure);
			originIndex = indexer.GetIndex(OlapProperty.HierarchyOrigin);
		}
		public string DimensionUniqueName { get { return set.GetCellStringValue(dimensionUniqueNameIndex); } }
		public string Name { get { return FixDBNull(set.GetCellValue(nameIndex)); } }
		public string UniqueName { get { return set.GetCellStringValue(uniqueNameIndex); } }
		public string Caption { get { return set.GetCellStringValue(captionIndex); } }
		public string Description { get { return FixDBNull(set.GetCellValue(descriptionIndex)); } }
		public string DisplayFolder { get { return displayFolderIndex == -1 ? (string)null : set.GetCellStringValue(displayFolderIndex); } }
		public string DefaultMember { get { return set.GetCellStringValue(defaultMemberIndex); } }
		public string AllMember { get { return FixDBNull(set.GetCellValue(allMemberIndex)); } }
		public Int16 Structure { get { return Convert.ToInt16(set.GetCellValue(structureIndex)); } }
		public Int16 Origin { get { return originIndex == -1 ? (short)0 : Convert.ToInt16(set.GetCellValue(originIndex)); } }
	}
	class PropertiesRowSet : SchemsRowSet {
		readonly int levelUniqueNameIndex;
		readonly int dataTypeIndex;
		readonly int propertyNameIndex;
		public PropertiesRowSet(IOLAPRowSet set)
			: base(set) {
			ColumnIndexer indexer = new ColumnIndexer(set);
			levelUniqueNameIndex = indexer.GetIndex(OlapProperty.LevelUniqueName);
			dataTypeIndex = indexer.GetIndex(OlapProperty.DataType);
			propertyNameIndex = indexer.GetIndex(OlapProperty.SchemaPropertyName);
		}
		public string LevelUniqueNameIndex { get { return set.GetCellStringValue(levelUniqueNameIndex); } }
		public int DataType { get { return Convert.ToInt32(set.GetCellValue(dataTypeIndex)); } }
		public string PropertyName { get { return set.GetCellStringValue(propertyNameIndex); } }
	}
	class MembersRowSet : SchemsRowSet {
		readonly int levelUniqueNameIndex;
		readonly int uniqueNameIndex;
		readonly int nameIndex;
		public MembersRowSet(IOLAPRowSet set)
			: base(set) {
			ColumnIndexer indexer = new ColumnIndexer(set);
			levelUniqueNameIndex = indexer.GetIndex(OlapProperty.LevelUniqueName);
			uniqueNameIndex = indexer.GetIndex(OlapProperty.MemberUniqueName);
			nameIndex = indexer.GetIndex(OlapProperty.MemberName);
		}
		public string LevelUniqueName { get { return set.GetCellStringValue(levelUniqueNameIndex); } }
		public string UniqueName { get { return set.GetCellStringValue(uniqueNameIndex); } }
		public string Name { get { return set.GetCellStringValue(nameIndex); } }
	}
	class LevelsRowSet : SchemsRowSet {
		readonly int dimensionUniqueNameIndex;
		readonly int hierarchyUniqueNameIndex;
		readonly int uniqueNameIndex;
		readonly int nameIndex;
		readonly int captionIndex;
		readonly int descriptionIndex;
		readonly int nameSQLColumnNameIndex;
		readonly int numberIndex;
		readonly int cardinalityIndex;
		readonly int typeIndex;
		readonly int keyCardinalityIndex;
		readonly int orderingIndex;
		public LevelsRowSet(IOLAPRowSet set)
			: base(set) {
			ColumnIndexer indexer = new ColumnIndexer(set);
			dimensionUniqueNameIndex = indexer.GetIndex(OlapProperty.DimensionUniqueName);
			hierarchyUniqueNameIndex = indexer.GetIndex(OlapProperty.HierarchyUniqueName);
			uniqueNameIndex = indexer.GetIndex(OlapProperty.LevelUniqueName);
			nameIndex = indexer.GetIndex(OlapProperty.LevelName);
			captionIndex = indexer.GetIndex(OlapProperty.LevelCaption);
			descriptionIndex = indexer.GetIndex(OlapProperty.Description);
			nameSQLColumnNameIndex = indexer.GetIndex(OlapProperty.LevelNameSQLColumnName);
			numberIndex = indexer.GetIndex(OlapProperty.LevelNumber);
			cardinalityIndex = indexer.GetIndex(OlapProperty.LevelCardinality);
			typeIndex = indexer.GetIndex(OlapProperty.LevelType);
			keyCardinalityIndex = indexer.GetIndex(OlapProperty.LevelKeyCardinality);
			orderingIndex = indexer.GetIndex(OlapProperty.LevelOrdering);
		}
		public string DimensionUniqueName { get { return set.GetCellStringValue(dimensionUniqueNameIndex); } }
		public string HierarchyUniqueName { get { return set.GetCellStringValue(hierarchyUniqueNameIndex); } }
		public string UniqueName { get { return set.GetCellStringValue(uniqueNameIndex); } }
		public string Name { get { return set.GetCellStringValue(nameIndex); } }
		public string Caption { get { return set.GetCellStringValue(captionIndex); } }
		public string Description { get { return FixDBNull(set.GetCellValue(descriptionIndex)); } }
		public string NameSQLColumnName { get { return FixDBNull(set.GetCellValue(nameSQLColumnNameIndex)); } }
		public int Number { get { return Convert.ToInt32(set.GetCellValue(numberIndex)); } }
		public Int64 Cardinality { get { return Convert.ToInt64(set.GetCellValue(cardinalityIndex)); } }
		public int Type { get { return Convert.ToInt32(set.GetCellValue(typeIndex)); } }
		public int KeyCardinality { get { return keyCardinalityIndex == -1 ? 1 : Convert.ToInt32(set.GetCellValue(keyCardinalityIndex)); } }
		public string Ordering { get { return FixDBNull(set.GetCellValue(orderingIndex)); } }
	}
	class MeasureGroupsRowSet : SchemsRowSet {
		Dictionary<string, string> captionsByNames = new Dictionary<string, string>();
		public MeasureGroupsRowSet(IOLAPRowSet set)
			: base(set) {
			ColumnIndexer indexer = new ColumnIndexer(set);
			int nameIndex = indexer.GetIndex(OlapProperty.MeasureGroupName);
			int captionIndex = indexer.GetIndex(OlapProperty.MeasureGroupCaption);
			while(set.NextRow()) {
				captionsByNames.Add(set.GetCellStringValue(nameIndex), set.GetCellStringValue(captionIndex));
			}
		}
		public string GetCaption(string name) {
			string caption;
			captionsByNames.TryGetValue(name, out caption);
			return caption;
		}
		public bool ContainsKey(string name) {
			return captionsByNames.ContainsKey(name);
		}
	}
	class MeasuresRowSet : SchemsRowSet {
		int nameIndex;
		int uniqueNameIndex;
		int captionIndex;
		int descriptionIndex;
		int nameSQLColumnNameIndex;
		int MeasureGroupNameIndex;
		int dataTypeIndex;
		int displayFolderIndex;
		int defaultFormatStringIndex;
		int expressionIndex;
		public MeasuresRowSet(IOLAPRowSet set)
			: base(set) {
			ColumnIndexer indexer = new ColumnIndexer(set);
			nameIndex = indexer.GetIndex(OlapProperty.MeasureName);
			uniqueNameIndex = indexer.GetIndex(OlapProperty.MeasureUniqueName);
			captionIndex = indexer.GetIndex(OlapProperty.MeasureCaption);
			descriptionIndex = indexer.GetIndex(OlapProperty.Description);
			nameSQLColumnNameIndex = indexer.GetIndex(OlapProperty.MeasureNameSQLColumnName);
			MeasureGroupNameIndex = indexer.GetIndex(OlapProperty.MeasureGroupName);
			dataTypeIndex = indexer.GetIndex(OlapProperty.DataType);
			displayFolderIndex = indexer.GetIndex(OlapProperty.MeasureDisplayFolder);
			defaultFormatStringIndex = indexer.GetIndex(OlapProperty.DefaultFormatString);
			expressionIndex = indexer.GetIndex(OlapProperty.Expression);
		}
		public string Name { get { return set.GetCellStringValue(nameIndex); } }
		public string UniqueName { get { return set.GetCellStringValue(uniqueNameIndex); } }
		public string Caption { get { return set.GetCellStringValue(captionIndex); } }
		public string Description { get { return set.GetCellStringValue(descriptionIndex); } }
		public string NameSQLColumnName { get { return FixDBNull(set.GetCellValue(nameSQLColumnNameIndex)); } }
		public string MeasureGroupName { get { return MeasureGroupNameIndex == -1 ? (string)null : FixDBNull(set.GetCellValue(MeasureGroupNameIndex)); } }
		public int DataType { get { return Convert.ToInt32(set.GetCellValue(dataTypeIndex)); } }
		public string DisplayFolder { get { return displayFolderIndex == -1 ? (string)null : set.GetCellStringValue(displayFolderIndex); } }
		public string DefaultFormatString { get { return defaultFormatStringIndex == -1 ? (string)null : FixDBNull(set.GetCellValue(defaultFormatStringIndex)); } }
		public string Expression { get { return FixDBNull(set.GetCellValue(expressionIndex)); } }
	}
	class KpisRowSet : SchemsRowSet {
		int nameIndex;
		int captionIndex;
		int descriptionIndex;
		int valueIndex;
		int goalIndex;
		int statusIndex;
		int trendIndex;
		int weightIndex;
		int statusGraphicIndex;
		int kpiTrendGraphicIndex;
		int displayFolderIndex;
		int measureGroupNameIndex;
		public KpisRowSet(IOLAPRowSet set)
			: base(set) {
			ColumnIndexer indexer = new ColumnIndexer(set); 
			nameIndex = indexer.GetIndex(OlapProperty.KpiName);
			captionIndex = indexer.GetIndex(OlapProperty.KpiCaption);
			descriptionIndex = indexer.GetIndex(OlapProperty.KpiDescription);
			valueIndex = indexer.GetIndex(OlapProperty.KpiValue);
			goalIndex = indexer.GetIndex(OlapProperty.KpiGoal);
			statusIndex = indexer.GetIndex(OlapProperty.KpiStatus);
			trendIndex = indexer.GetIndex(OlapProperty.KpiTrend);
			weightIndex = indexer.GetIndex(OlapProperty.KpiWeight);
			statusGraphicIndex = indexer.GetIndex(OlapProperty.KpiStatusGraphic);
			kpiTrendGraphicIndex = indexer.GetIndex(OlapProperty.KpiTrendGraphic);
			displayFolderIndex = indexer.GetIndex(OlapProperty.KpiDisplayFolder);
			measureGroupNameIndex = indexer.GetIndex(OlapProperty.MeasureGroupName);
		}
		public string Name { get { return set.GetCellStringValue(nameIndex); } }
		public string Caption { get { return set.GetCellStringValue(captionIndex); } }
		public string Description { get { return set.GetCellStringValue(descriptionIndex); } }
		public string Value { get { return set.GetCellStringValue(valueIndex) ?? string.Empty; } }
		public string Goal { get { return set.GetCellStringValue(goalIndex) ?? string.Empty; } }
		public string Status { get { return set.GetCellStringValue(statusIndex) ?? string.Empty; } }
		public string Trend { get { return set.GetCellStringValue(trendIndex) ?? string.Empty; } }
		public string Weight { get { return set.GetCellStringValue(weightIndex) ?? string.Empty; } }
		public string StatusGraphic { get { return set.GetCellStringValue(statusGraphicIndex); } }
		public string TrendGraphic { get { return set.GetCellStringValue(kpiTrendGraphicIndex); } }
		public string DisplayFolder { get { return set.GetCellStringValue(displayFolderIndex); } }
		public string MeasureGroupName { get { return FixDBNull(set.GetCellValue(measureGroupNameIndex)); } }
	}
}
