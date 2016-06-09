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

using DevExpress.Office;
using DevExpress.Utils.Zip;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter {
		string GeneratePivotCacheRelation(PivotCache cache, int index) {
			string cacheFileName = string.Format("pivotCacheDefinition{0}.xml", index + 1);
			PivotCachesPathsTable.Add(index, cacheFileName);
			string id = GenerateIdByCollection(Builder.WorkbookRelations);
			Builder.WorkbookRelations.Add(new OpenXmlRelation(id, "pivotCache/" + cacheFileName, RelsPivotCacheDefinitionNamepace));
			return id;
		}
		protected internal virtual void AddPivotCachesPackageContent() {
			int pivotCacheRecordsCounter = 0;
			foreach (KeyValuePair<int, string> pair in PivotCachesPathsTable) {
				int cacheId = pair.Key;
				string cacheFileName = pair.Value;
				string cachePath = @"xl\pivotCache\" + cacheFileName;
				string cacheRelationPath = @"xl\pivotCache\_rels\" + cacheFileName + ".rels";
				SetActivePivotCache(Workbook.PivotCaches[cacheId]);
				CurrentRelations = new OpenXmlRelationCollection();
				if (ActivePivotCache.SaveData && ActivePivotCache.Records.Count > 0) {
					++pivotCacheRecordsCounter;
					string recordsFileName = string.Format("pivotCacheRecords{0}.xml", pivotCacheRecordsCounter);
					string recordsPath = @"xl\pivotCache\" + recordsFileName;
					string id = "rId" + pivotCacheRecordsCounter;
					CurrentRelations.Add(new OpenXmlRelation(id, recordsFileName, RelsPivotCacheRecordsNamepace));
					Builder.OverriddenContentTypes.Add("/xl/pivotCache/" + recordsFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.pivotCacheRecords+xml");
					AddPackageContent(recordsPath, ExportPivotCacheRecordsContent());
				} 
				Builder.OverriddenContentTypes.Add("/xl/pivotCache/" + cacheFileName, "application/vnd.openxmlformats-officedocument.spreadsheetml.pivotCacheDefinition+xml");
				AddPackageContent(cachePath, ExportPivotCacheContent());
				AddRelationsPackageContent(cacheRelationPath, CurrentRelations);
			}
		}
		#region PivotCacheTypeTable
		internal static readonly Dictionary<PivotCacheType, string> PivotCacheTypeTable = CreatePivotCacheTypeTable();
		static Dictionary<PivotCacheType, string> CreatePivotCacheTypeTable() {
			Dictionary<PivotCacheType, string> result = new Dictionary<PivotCacheType, string>();
			result.Add(PivotCacheType.Worksheet, "worksheet");
			result.Add(PivotCacheType.Consolidation, "consolidation");
			result.Add(PivotCacheType.Scenario, "scenario");
			result.Add(PivotCacheType.External, "external");
			return result;
		}
		#endregion
		#region PivotCacheGroupValuesByTable
		internal static readonly Dictionary<PivotCacheGroupValuesBy, string> PivotCacheGroupValuesByTable = CreatePivotCacheGroupValuesByTable();
		static Dictionary<PivotCacheGroupValuesBy, string> CreatePivotCacheGroupValuesByTable() {
			Dictionary<PivotCacheGroupValuesBy, string> result = new Dictionary<PivotCacheGroupValuesBy, string>();
			result.Add(PivotCacheGroupValuesBy.Days, "days");
			result.Add(PivotCacheGroupValuesBy.Hours, "hours");
			result.Add(PivotCacheGroupValuesBy.Minutes, "minutes");
			result.Add(PivotCacheGroupValuesBy.Months, "months");
			result.Add(PivotCacheGroupValuesBy.Quarters, "quarters");
			result.Add(PivotCacheGroupValuesBy.NumericRanges, "range");
			result.Add(PivotCacheGroupValuesBy.Seconds, "seconds");
			result.Add(PivotCacheGroupValuesBy.Years, "years");
			return result;
		}
		#endregion
		#region PivotTupleCacheSetSortOrderTable
		internal static readonly Dictionary<PivotTupleCacheSetSortOrder, string> PivotTupleCacheSetSortOrderTable = CreatePivotTupleCacheSetSortOrderTable();
		static Dictionary<PivotTupleCacheSetSortOrder, string> CreatePivotTupleCacheSetSortOrderTable() {
			Dictionary<PivotTupleCacheSetSortOrder, string> result = new Dictionary<PivotTupleCacheSetSortOrder, string>();
			result.Add(PivotTupleCacheSetSortOrder.None, "none");
			result.Add(PivotTupleCacheSetSortOrder.Ascending, "ascending");
			result.Add(PivotTupleCacheSetSortOrder.AscendingAlphabetic, "ascendingAlpha");
			result.Add(PivotTupleCacheSetSortOrder.AscendingNatural, "ascendingNatural");
			result.Add(PivotTupleCacheSetSortOrder.Descending, "descending");
			result.Add(PivotTupleCacheSetSortOrder.DescendingAlphabetic, "descendingAlphabetic");
			result.Add(PivotTupleCacheSetSortOrder.DescendingNatural, "descendingNatural");
			return result;
		}
		#endregion
		#region Export PivotCacheContent
		protected internal virtual CompressedStream ExportPivotCacheContent() {
			return CreateXmlContent(GeneratePivotCacheXmlContent);
		}
		protected internal virtual void GeneratePivotCacheXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GeneratePivotCacheContent();
		}
		protected internal virtual void GeneratePivotCacheContent() {
			WriteShStartElement("pivotCacheDefinition");
			try {
				WriteStringAttr("xmlns", RelsPrefix, null, RelsNamespace);
				if (ActivePivotCache.SaveData) {
					if (ActivePivotCache.Records.Count > 0)
						WriteStringAttr(RelsPrefix, "id", null, CurrentRelations[0].Id);
				} else
					WriteBoolValue("saveData", false);
				WriteBoolValue("invalid", ActivePivotCache.Invalid, false);
				WriteBoolValue("refreshOnLoad", ActivePivotCache.RefreshOnLoad, false);
				WriteBoolValue("optimizeMemory", ActivePivotCache.OptimizeMemory, false);
				WriteBoolValue("enableRefresh", ActivePivotCache.EnableRefresh, true);
				if (!string.IsNullOrEmpty(ActivePivotCache.RefreshedBy))
					WriteStringValue("refreshedBy", ActivePivotCache.RefreshedBy);
				if (ActivePivotCache.RefreshedDate != default(DateTime))
					WriteDoubleValue("refreshedDate", Workbook.DataContext.ToDateTimeSerialDouble(ActivePivotCache.RefreshedDate));
				WriteBoolValue("backGroundQuery", ActivePivotCache.BackgroundQuery, false);
				WriteIntValue("missingItemsLimit", ActivePivotCache.MissingItemsLimit, PivotCache.DefaultMissingItemsLimit);
				WriteIntValue("createdVersion", ActivePivotCache.CreatedVersion, 0);
				WriteIntValue("refreshedVersion", ActivePivotCache.RefreshedVersion, 0);
				WriteIntValue("minRefreshableVersion", ActivePivotCache.MinRefreshableVersion, 0);
				WriteIntValue("recordCount", ActivePivotCache.Records.Count, 0);
				WriteBoolValue("upgradeOnRefresh", ActivePivotCache.UpgradeOnRefresh, false);
				WriteBoolValue("tupleCache", ActivePivotCache.HasTupleCache, false);
				WriteBoolValue("supportSubquery", ActivePivotCache.SupportSubquery, false);
				WriteBoolValue("supportAdvancedDrill", ActivePivotCache.SupportAdvancedDrill, false);
				GeneratePivotCacheSource();
				GenerateCacheFields();
				GenerateCacheHierarchies(ActivePivotCache.CacheHierarchies);
				GenerateKpis(ActivePivotCache.Kpis);
				GenerateTupleCache(ActivePivotCache.TupleCache);
				GenerateCalculatedItems(ActivePivotCache.CalculatedItems);
				GenerateCalculatedMembers(ActivePivotCache.CalculatedMembers);
				GenerateDimensions(ActivePivotCache.Dimensions);
				GenerateMeasureGroups(ActivePivotCache.MeasureGroups);
				GenerateDimensionMaps(ActivePivotCache.DimensionMaps);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export PivotCacheSource
		protected internal virtual void GeneratePivotCacheSource() {
			IPivotCacheSource source = ActivePivotCache.Source;
			WriteShStartElement("cacheSource");
			try {
				WriteEnumValue("type", source.Type, PivotCacheTypeTable);
				switch (source.Type) {
					case PivotCacheType.Consolidation:
						GenerateConsolidation(source as PivotCacheSourceConsolidation);
						break;
					case PivotCacheType.External:
						PivotCacheSourceExternal extSource = source as PivotCacheSourceExternal;
						WriteIntValue("connectionId", extSource.ConnectionId, 0);
						break;
					case PivotCacheType.Worksheet:
						GenerateWorksheetSource(source as PivotCacheSourceWorksheet);
						break;
				}
			}
			finally {
				WriteShEndElement();
			}
		}
		#region Consolidation
		protected internal void GenerateConsolidation(PivotCacheSourceConsolidation source) {
			WriteShStartElement("consolidation");
			try {
				WriteBoolValue("autoPage", source.AutoPage, true);
				GenerateConsolidationPages(source.Pages);
				GenerateConsolidationRangeSets(source.RangeSets);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateConsolidationPages(IList<PivotCacheSourceConsolidationPage> pages) {
			WriteShStartElement("pages");
			try {
				WriteIntValue("count", pages.Count);
				for (int i = 0; i < pages.Count; ++i)
					GenerateConsolidationPage(pages[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateConsolidationPage(PivotCacheSourceConsolidationPage page) {
			WriteShStartElement("page");
			try {
				WriteIntValue("count", page.ItemNames.Count);
				for (int i = 0; i < page.ItemNames.Count; ++i)
					GenerateConsolidationPageItem(page.ItemNames[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateConsolidationPageItem(string item) {
			WriteShStartElement("pageItem");
			try {
				WriteStringValue("name", item);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateConsolidationRangeSets(IList<PivotCacheSourceConsolidationRangeSet> rangeSets) {
			WriteShStartElement("rangeSets");
			try {
				WriteIntValue("count", rangeSets.Count);
				for (int i = 0; i < rangeSets.Count; ++i)
					GenerateConsolidationRangeSet(rangeSets[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateConsolidationRangeSet(PivotCacheSourceConsolidationRangeSet rangeSet) {
			WriteShStartElement("rangeSet");
			try {
				WriteIntValue("i1", rangeSet.PageFieldItemIndex1, rangeSet.PageFieldItemIndex1 >= 0);
				WriteIntValue("i2", rangeSet.PageFieldItemIndex2, rangeSet.PageFieldItemIndex2 >= 0);
				WriteIntValue("i3", rangeSet.PageFieldItemIndex3, rangeSet.PageFieldItemIndex3 >= 0);
				WriteIntValue("i4", rangeSet.PageFieldItemIndex4, rangeSet.PageFieldItemIndex4 >= 0);
				GenerateWorksheetReferenceAttributes(rangeSet.SourceReference);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		protected internal void GenerateWorksheetSource(PivotCacheSourceWorksheet source) {
			WriteShStartElement("worksheetSource");
			try {
				GenerateWorksheetReferenceAttributes(source);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateWorksheetReferenceAttributes(PivotCacheSourceWorksheet source) {
			PivotCacheSourceWorksheetHelper helper = new PivotCacheSourceWorksheetHelper();
			helper.GenerateValues(source, Workbook.DataContext);
			if (!string.IsNullOrEmpty(helper.Book)) {
				OpenXmlRelation relation = GetExternalReferenceRelation(helper.Book);
				relation.Type = RelsExternalLinkPathNamespace;
				CurrentRelations.Add(relation);
				WriteStringAttr(RelsPrefix, "id", null, relation.Id);
			}
			if (!string.IsNullOrEmpty(helper.Sheet))
				WriteStringValue("sheet", helper.Sheet);
			if (!string.IsNullOrEmpty(helper.Reference))
				WriteStringValue("ref", helper.Reference);
			if (!string.IsNullOrEmpty(helper.Name))
				WriteStringValue("name", helper.Name);
		}
		#endregion
		#region Export CacheFields
		protected internal virtual void GenerateCacheFields() {
			PivotCacheFieldsCollection fields = ActivePivotCache.CacheFields;
			WriteShStartElement("cacheFields");
			try {
				WriteIntValue("count", fields.Count);
				for (int i = 0; i < fields.Count; ++i)
					GenerateCacheField(fields[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateCacheField(IPivotCacheField field) {
			WriteShStartElement("cacheField");
			try {
				WriteStringValue("name", field.Name);
				if (!string.IsNullOrEmpty(field.Caption))
					WriteStringValue("caption", field.Caption);
				if (!string.IsNullOrEmpty(field.PropertyName))
					WriteStringValue("propertyName", field.PropertyName);
				WriteBoolValue("serverField", field.ServerField, false);
				WriteBoolValue("uniqueList", field.UniqueList, true);
				WriteIntValue("numFmtId", ExportStyleSheet.GetNumberFormatId(field.NumberFormatIndex));
				if (!string.IsNullOrEmpty(field.Formula))
					WriteStringValue("formula", field.Formula);
				WriteIntValue("sqlType", (int)field.SqlType, 0);
				WriteIntValue("hierarchy", field.Hierarchy, 0);
				WriteIntValue("level", field.HierarchyLevel, 0);
				if (field.FieldGroup.GroupItems.Count > 0 || field.MemberPropertiesMap.Count > 0 || !String.IsNullOrEmpty(field.Formula))
					WriteBoolValue("databaseField", field.DatabaseField, true);
				WriteIntValue("mappingCount", field.MemberPropertiesMap.Count, 0);
				WriteBoolValue("memberPropertyField", field.MemberPropertyField, false);
				GenerateCacheFieldSharedItems(field.SharedItems);
				GenerateCacheFieldGroup(field.FieldGroup);
				GenerateCacheFieldMap(field.MemberPropertiesMap);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export CacheFieldSharedItems
		protected internal void GenerateCacheFieldSharedItems(PivotCacheSharedItemsCollection sharedItems) {
			if (sharedItems.Count == 0 && sharedItems.Flags == PivotCacheSharedItemsCollectionFlags.None)
				return;
			WriteShStartElement("sharedItems");
			try {
				bool containsDate = sharedItems.ContainsDate;
				WriteBoolValue("containsSemiMixedTypes", sharedItems.ContainsSemiMixedTypes, true);
				WriteBoolValue("containsNonDate", sharedItems.ContainsNonDate, true);
				WriteBoolValue("containsDate", containsDate, false);
				WriteBoolValue("containsString", sharedItems.ContainsString, true);
				WriteBoolValue("containsBlank", sharedItems.ContainsBlank, false);
				WriteBoolValue("containsMixedTypes", sharedItems.ContainsMixedTypes, false);
				WriteBoolValue("containsNumber", sharedItems.ContainsNumber, false);
				WriteBoolValue("containsInteger", sharedItems.ContainsInteger, false);
				if (containsDate) {
					WriteDateTime("minDate", sharedItems.MinDate);
					WriteDateTime("maxDate", sharedItems.MaxDate);
				}
				else
					if (sharedItems.HasMinMaxValues) {
						WriteDoubleValue("minValue", sharedItems.MinValue);
						WriteDoubleValue("maxValue", sharedItems.MaxValue);
					}
				WriteIntValue("count", sharedItems.Count, 0);
				WriteBoolValue("longText", sharedItems.ContainsLongText, false);
				GeneratePivotCacheRecordValuesContent(sharedItems.InnerList);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export CacheFieldGroup
		protected internal void GenerateCacheFieldGroup(PivotCacheFieldGroup group) {
			if (!group.Initialized)
				return;
			WriteShStartElement("fieldGroup");
			try {
				WriteIntValue("par", group.Parent, -1);
				WriteIntValue("base", group.FieldBase, -1);
				GenerateRangeGroupingProperties(group.RangeGroupingProperties);
				GenerateDiscreteGroupingProperties(group.DiscreteGroupingProperties);
				GeneratePivotCacheGroupItems(group.GroupItems);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateRangeGroupingProperties(PivotCacheRangeGroupingProperties properties) {
			if (!properties.HasGroup)
				return;
			WriteShStartElement("rangePr");
			try {
				WriteBoolValue("autoStart", properties.AutoStart, true);
				WriteBoolValue("autoEnd", properties.AutoEnd, true);
				WriteEnumValue("groupBy", properties.GroupBy, PivotCacheGroupValuesByTable, PivotCacheGroupValuesBy.NumericRanges);
				WriteDoubleValue("startNum", properties.StartNum, double.MaxValue);
				WriteDoubleValue("endNum", properties.EndNum, double.MinValue);
				WriteDateTime("startDate", properties.StartDate, DateTime.MaxValue);
				WriteDateTime("endDate", properties.EndDate, DateTime.MinValue);
				WriteDoubleValue("groupInterval", properties.GroupInterval, 1);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateDiscreteGroupingProperties(PivotCacheDiscreteGroupingProperties properties) {
			if (properties.Count == 0)
				return;
			WriteShStartElement("discretePr");
			try {
				WriteIntValue("count", properties.Count);
				GeneratePivotCacheRecordValuesContent(properties.InnerList);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GeneratePivotCacheGroupItems(PivotCacheGroupItems items) {
			if (items.Count == 0)
				return;
			WriteShStartElement("groupItems");
			try {
				WriteIntValue("count", items.Count);
				GeneratePivotCacheRecordValuesContent(items.InnerList);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export CacheFieldMap
		protected internal void  GenerateCacheFieldMap(PivotCacheMemberPropertiesMap map) {
			foreach (int item in map) {
				WriteShStartElement("mpMap");
				try {
					WriteIntValue("v", item);
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		#endregion
		#region Export CalculatedMembers
		protected internal void GenerateCalculatedMembers(PivotCacheCalculatedMemberCollection members) {
			if (members == null)
				return;
			WriteShStartElement("calculatedMembers");
			try {
				int count = members.Count;
				WriteIntValue("count", count);
				for (int i = 0; i < count; ++i)
					GenerateCalculatedMember(members[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateCalculatedMember(PivotCacheCalculatedMember member) {
			WriteShStartElement("calculatedMember");
			try {
				WriteStringValue("name", member.Name);
				WriteStringValue("mdx", member.Mdx);
				WriteStringValue("memberName", member.MemberName);
				WriteStringValue("hierarchy", member.Hierarchy);
				WriteStringValue("parent", member.Parent);
				WriteIntValue("solveOrder", member.SolveOrder, 0);
				WriteBoolValue("set", member.Set, false);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export CalculatedItems
		protected internal void GenerateCalculatedItems(PivotCacheCalculatedItemCollection items) {
			if (items == null)
				return;
			WriteShStartElement("calculatedItems");
			try {
				int count = items.Count;
				WriteIntValue("count", count);
				for (int i = 0; i < count; ++i)
					GenerateCalculatedItem(items[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateCalculatedItem(PivotCacheCalculatedItem item) {
			WriteShStartElement("calculatedItem");
			try {
				if (item.Field != -1)
					WriteIntValue("field", item.Field);
				WriteStringValue("formula", item.Formula);
				GeneratePivotAreaContent(item.PivotArea);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export TupleCache
		protected internal void GenerateTupleCache(PivotTupleCache cache) {
			if (cache == null)
				return;
			WriteShStartElement("tupleCache");
			try {
				GenerateTupleCacheEntries(cache.Entries);
				GenerateTupleCacheSets(cache.Sets);
				GenerateTupleCacheQueryCache(cache.QueryCache);
				GenerateTupleCacheServerFormats(cache.ServerFormats);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateTupleCacheEntries(PivotTupleCacheEntries entries) {
			if (entries == null || entries.Count == 0)
				return;
			WriteShStartElement("entries");
			try {
				WriteIntValue("count", entries.Count);
				GeneratePivotCacheRecordValuesContent(entries);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateTupleCacheSets(PivotTupleCacheSets sets) {
			if (sets == null)
				return;
			int count = sets.Count;
			if (count == 0)
				return;
			WriteShStartElement("sets");
			try {
				WriteIntValue("count", count);
				for (int i = 0; i < count; ++i)
					GenerateTupleCacheSet(sets[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateTupleCacheSet(PivotTupleCacheSet set) {
			WriteShStartElement("set");
			try {
				int count = set.Tuples.Count;
				WriteIntValue("count", count);
				WriteIntValue("maxRank", set.MaxRank);
				WriteStringValue("setDefinition", set.SetDefinition);
				WriteEnumValue("sortType", set.SortType, PivotTupleCacheSetSortOrderTable, PivotTupleCacheSetSortOrder.None);
				WriteBoolValue("queryFailed", set.QueryFailed, false);
				WritePivotCacheRecordValueTuples(set.Tuples);
				WritePivotTupleCollection("sortByTuple", set.SortByTuple);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateTupleCacheQueryCache(PivotTupleCacheQueries queries) {
			if (queries == null)
				return;
			int count = queries.Count;
			if (count == 0)
				return;
			WriteShStartElement("queryCache");
			try {
				WriteIntValue("count", count);
				for (int i = 0; i < count; ++i)
					GenerateTupleCacheQuery(queries[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateTupleCacheQuery(PivotTupleCacheQuery query) {
			WriteShStartElement("query");
			try {
				WriteStringValue("mdx", query.Mdx);
				WritePivotTupleCollection(query.Tuples);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateTupleCacheServerFormats(PivotTupleCacheServerFormats formats) {
			if (formats == null)
				return;
			int count = formats.Count;
			if (count == 0)
				return;
			WriteShStartElement("serverFormats");
			try {
				WriteIntValue("count", count);
				for (int i = 0; i < count; ++i)
					GenerateTupleCacheServerFormat(formats[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		protected internal void GenerateTupleCacheServerFormat(PivotTupleCacheServerFormat format) {
			WriteShStartElement("serverFormat");
			try {
				WriteStringValue("culture", format.Culture);
				WriteStringValue("format", format.Format);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export CacheHierarchies
		protected internal void GenerateCacheHierarchies(PivotCacheHierarchyCollection hierarchies) {
			if (hierarchies == null)
				return;
			int count = hierarchies.Count;
			if (count == 0)
				return;
			WriteShStartElement("cacheHierarchies");
			try {
				WriteIntValue("count", count);
				for (int i = 0; i < count; ++i)
					GenerateCacheHierarchy(hierarchies[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateCacheHierarchy(PivotCacheHierarchy hierarchy) {
			WriteShStartElement("cacheHierarchy");
			try {
				WriteStringValue("uniqueName", hierarchy.UniqueName);
				WriteStringValue("caption", hierarchy.Caption, !string.IsNullOrEmpty(hierarchy.Caption));
				WriteBoolValue("measure", hierarchy.IsMeasure, false);
				WriteBoolValue("set", hierarchy.IsSet, false);
				WriteIntValue("parentSet", hierarchy.ParentSet, -1);
				WriteIntValue("iconSet", hierarchy.KpiIconSet, 0);
				WriteBoolValue("attribute", hierarchy.Attribute, false);
				WriteBoolValue("time", hierarchy.Time, false);
				WriteBoolValue("keyAttribute", hierarchy.IsKeyAttribute, false);
				WriteStringValue("defaultMemberUniqueName", hierarchy.DefaultMemberUniqueName, !string.IsNullOrEmpty(hierarchy.DefaultMemberUniqueName));
				WriteStringValue("allUniqueName", hierarchy.AllUniqueName, !string.IsNullOrEmpty(hierarchy.AllUniqueName));
				WriteStringValue("allCaption", hierarchy.AllCaption, !string.IsNullOrEmpty(hierarchy.AllCaption));
				WriteStringValue("dimensionUniqueName", hierarchy.DimensionUniqueName, !string.IsNullOrEmpty(hierarchy.DimensionUniqueName));
				WriteStringValue("displayFolder", hierarchy.DisplayFolder, !string.IsNullOrEmpty(hierarchy.DisplayFolder));
				WriteStringValue("measureGroup", hierarchy.MeasureGroupName, !string.IsNullOrEmpty(hierarchy.MeasureGroupName));
				WriteBoolValue("measures", hierarchy.Measures, false);
				WriteIntValue("count", hierarchy.Count);
				WriteBoolValue("oneField", hierarchy.OneField, false);
				WriteIntValue("memberValueDatatype", hierarchy.MemberValueDataType, -1);
				WriteBoolValue("unbalanced", hierarchy.Unbalanced, false);
				WriteBoolValue("unbalancedGroup", hierarchy.UnbalancedGroup, false);
				WriteBoolValue("hidden", hierarchy.Hidden, false);
				GenerateFieldsUsage(hierarchy.FieldsUsage);
				GenerateGroupingLevels(hierarchy.GroupingLevels);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateFieldsUsage(PivotCacheFieldUsageCollection fieldsUsage) {
			if (fieldsUsage == null)
				return;
			int count = fieldsUsage.Count;
			if (count == 0)
				return;
			WriteShStartElement("fieldsUsage");
			try {
				WriteIntValue("count", count);
				for (int i = 0; i < fieldsUsage.Count; ++i)
					GenerateFieldUsage(fieldsUsage[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateFieldUsage(int fieldUsage) {
			WriteShStartElement("fieldUsage");
			try {
				WriteIntValue("x", fieldUsage);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export GroupingLevels
		void GenerateGroupingLevels(PivotCacheGroupingLevelCollection levels) {
			if (levels == null)
				return;
			int count = levels.Count;
			if (count == 0)
				return;
			WriteShStartElement("groupLevels");
			try {
				WriteIntValue("count", count);
				for (int i = 0; i < count; ++i)
					GenerateGroupingLevel(levels[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateGroupingLevel(PivotCacheGroupingLevel level) {
			WriteShStartElement("groupLevel");
			try {
				WriteStringValue("uniqueName", level.UniqueName);
				WriteStringValue("caption", level.Caption);
				WriteBoolValue("user", level.User, false);
				WriteBoolValue("customRollUp", level.CustomRollUp, false);
				GenerateGroups(level.Groups);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateGroups(PivotCacheGroupCollection groups) {
			if (groups == null)
				return;
			int count = groups.Count;
			if (count == 0)
				return;
			WriteShStartElement("groups");
			try {
				WriteIntValue("count", count);
				for (int i = 0; i < count; ++i)
					GenerateGroup(groups[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateGroup(PivotCacheGroup group) {
			WriteShStartElement("group");
			try {
				WriteStringValue("name", group.Name);
				WriteStringValue("uniqueName", group.UniqueName);
				WriteStringValue("caption", group.Caption);
				WriteStringValue("uniqueParent", group.UniqueParent, !string.IsNullOrEmpty(group.UniqueParent));
				WriteIntValue("id", group.Id, int.MinValue);
				GenerateGroupMembers(group.Members);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateGroupMembers(PivotCacheGroupMemberCollection members) {
			WriteShStartElement("groupMembers");
			try {
				int count = members.Count;
				WriteIntValue("count", count);
				for (int i = 0; i < count; ++i)
					GenerateGroupMember(members[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateGroupMember(PivotCacheGroupMember member) {
			WriteShStartElement("groupMember");
			try {
				WriteStringValue("uniqueName", member.UniqueName);
				WriteBoolValue("group", member.Group, false);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export Kpis
		void GenerateKpis(PivotCacheKpiCollection kpis) {
			if (kpis == null)
				return;
			int count = kpis.Count;
			if (count == 0)
				return;
			WriteShStartElement("kpis");
			try {
				WriteIntValue("count", count);
				for (int i = 0; i < count; ++i)
					GenerateKpi(kpis[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateKpi(PivotCacheKpi kpi) {
			WriteShStartElement("kpi");
			try {
				WriteStringValue("uniqueName", kpi.UniqueName);
				WriteStringValue("caption", kpi.Caption, !string.IsNullOrEmpty(kpi.Caption));
				WriteStringValue("displayFolder", kpi.DisplayFolder, !string.IsNullOrEmpty(kpi.DisplayFolder));
				WriteStringValue("measureGroup", kpi.MeasureGroupName, !string.IsNullOrEmpty(kpi.MeasureGroupName));
				WriteStringValue("parent", kpi.Parent, !string.IsNullOrEmpty(kpi.Parent));
				WriteStringValue("value", kpi.ValueUniqueName);
				WriteStringValue("goal", kpi.GoalUniqueName, !string.IsNullOrEmpty(kpi.GoalUniqueName));
				WriteStringValue("status", kpi.StatusUniqueName, !string.IsNullOrEmpty(kpi.StatusUniqueName));
				WriteStringValue("trend", kpi.TrendUniqueName, !string.IsNullOrEmpty(kpi.TrendUniqueName));
				WriteStringValue("weight", kpi.WeightUniqueName, !string.IsNullOrEmpty(kpi.WeightUniqueName));
				WriteStringValue("time", kpi.TimeMemberUniqueName, !string.IsNullOrEmpty(kpi.TimeMemberUniqueName));
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export Dimensions
		void GenerateDimensions(PivotCacheDimensionCollection dimensions) {
			if (dimensions == null)
				return;
			int count = dimensions.Count;
			if (count == 0)
				return;
			WriteShStartElement("dimensions");
			try {
				WriteIntValue("count", count);
				for (int i = 0; i < count; ++i)
					GenerateDimension(dimensions[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateDimension(PivotCacheDimension dimension) {
			WriteShStartElement("dimension");
			try {
				WriteBoolValue("measure", dimension.Measure, false);
				WriteStringValue("name", dimension.Name);
				WriteStringValue("uniqueName", dimension.UniqueName);
				WriteStringValue("caption", dimension.Caption);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export MeasureGroups
		void GenerateMeasureGroups(PivotCacheMeasureGroupCollection groups) {
			if (groups == null)
				return;
			int count = groups.Count;
			if (count == 0)
				return;
			WriteShStartElement("measureGroups");
			try {
				WriteIntValue("count", count);
				for (int i = 0; i < count; ++i)
					GenerateMeasureGroup(groups[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateMeasureGroup(PivotCacheMeasureGroup group) {
			WriteShStartElement("measureGroup");
			try {
				WriteStringValue("name", group.Name);
				WriteStringValue("caption", group.Caption);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export DimensionMaps
		void GenerateDimensionMaps(PivotCacheDimensionMapCollection maps) {
			if (maps == null)
				return;
			int count = maps.Count;
			if (count == 0)
				return;
			WriteShStartElement("maps");
			try {
				WriteIntValue("count", count);
				for (int i = 0; i < count; ++i)
					GenerateDimensionMap(maps[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void GenerateDimensionMap(PivotCacheDimensionMap map) {
			WriteShStartElement("map");
			try {
				WriteIntValue("measureGroup", map.MeasureGroup, int.MinValue);
				WriteIntValue("dimension", map.Dimension, int.MinValue);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
	}
}
