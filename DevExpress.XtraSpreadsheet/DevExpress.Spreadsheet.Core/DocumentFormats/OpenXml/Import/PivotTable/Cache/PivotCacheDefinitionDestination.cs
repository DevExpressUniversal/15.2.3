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
using System.Xml;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region ImportPivotCacheInfo
	public class ImportPivotCacheInfo {
		readonly ImportPivotCacheSourceInfo sourceInfo;
		public ImportPivotCacheInfo() {
			this.sourceInfo = new ImportPivotCacheSourceInfo();
		}
		#region Properties
		public ImportPivotCacheSourceInfo SourceInfo { get { return sourceInfo; } }
		public bool Invalid { get; set; }
		public bool SaveData { get; set; }
		public bool RefreshOnLoad { get; set; }
		public bool OptimizeMemory { get; set; }
		public bool EnableRefresh { get; set; }
		public bool BackgroundQuery { get; set; }
		public bool UpgradeOnRefresh { get; set; }
		public bool HasTupleCache { get; set; }
		public bool SupportSubquery { get; set; }
		public bool SupportAdvancedDrill { get; set; }
		public byte CreatedVersion { get; set; }
		public byte RefreshedVersion { get; set; }
		public byte MinRefreshableVersion { get; set; }
		public string RefreshedBy { get; set; }
		public DateTime RefreshedDate { get; set; }
		public int MissingItemsLimit { get; set; }
		public PivotCacheFieldsCollection Fields { get; set; }
		public PivotCacheHierarchyCollection CacheHierarchies { get; set; }
		public PivotTupleCache TupleCache { get; set; }
		public PivotCacheCalculatedItemCollection CalculatedItems { get; set; }
		public PivotCacheCalculatedMemberCollection CalculatedMembers { get; set; }
		public PivotCacheKpiCollection Kpis { get; set; }
		public PivotCacheDimensionCollection Dimensions { get; set; }
		public PivotCacheMeasureGroupCollection MeasureGroups { get; set; }
		public PivotCacheDimensionMapCollection DimensionMaps { get; set; }
		#endregion
		void AssignCacheProperties(PivotCache cache) {
			Guard.ArgumentNotNull(cache, "pivotCache");
			cache.CacheHierarchies = CacheHierarchies;
			cache.TupleCache = TupleCache;
			cache.CalculatedItems = CalculatedItems;
			cache.CalculatedMembers = CalculatedMembers;
			cache.Kpis = Kpis;
			cache.Dimensions = Dimensions;
			cache.MeasureGroups = MeasureGroups;
			cache.DimensionMaps = DimensionMaps;
			cache.Invalid = Invalid;
			cache.SaveData = SaveData;
			cache.RefreshOnLoad = RefreshOnLoad;
			cache.OptimizeMemory = OptimizeMemory;
			cache.EnableRefresh = EnableRefresh;
			cache.BackgroundQuery = BackgroundQuery;
			cache.UpgradeOnRefresh = UpgradeOnRefresh;
			cache.HasTupleCache = HasTupleCache;
			cache.SupportSubquery = SupportSubquery;
			cache.SupportAdvancedDrill = SupportAdvancedDrill;
			cache.CreatedVersion = CreatedVersion;
			cache.RefreshedVersion = RefreshedVersion;
			cache.MinRefreshableVersion = MinRefreshableVersion;
			cache.RefreshedDate = RefreshedDate;
			if (RefreshedBy != null)
				cache.RefreshedBy = RefreshedBy;
			cache.MissingItemsLimit = MissingItemsLimit;
		}
		PivotCache CreateCache(OpenXmlImporter importer) {
			IPivotCacheSource source = sourceInfo.CreateSource(importer);
			if (Fields == null)
				importer.ThrowInvalidFile();
			PivotCache result = new PivotCache(importer.DocumentModel, source);
			result.CacheFields.AddRangeCore(Fields);
			AssignCacheProperties(result);
			return result;
		}
		protected internal PivotCache CreateAndRegisterPivotCache(OpenXmlImporter importer) {
			PivotCacheCollection pivotCaches = importer.DocumentModel.PivotCaches;
			int cacheId = importer.ActualPivotCacheDefinitionId;
			if (cacheId < pivotCaches.Count)
				return pivotCaches[cacheId];
			PivotCache result = CreateCache(importer);
			pivotCaches.AddCore(result);
			return result;
		}
	}
	#endregion
	#region PivotCachesDestination
	public class PivotCachesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pivotCache", OnPivotCache);
			return result;
		}
		static Destination OnPivotCache(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheDestination(importer); 
		}
		#endregion
		public PivotCachesDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotCacheDestination
	public class PivotCacheDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		public PivotCacheDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		internal new OpenXmlImporter Importer { get { return (OpenXmlImporter)base.Importer; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			int cachedId = Importer.GetIntegerValue(reader, "cacheId", -1);
			if (cachedId < 0)
				Importer.ThrowInvalidFile();
			string relationId = reader.GetAttribute("id", Importer.RelationsNamespace);
			if (String.IsNullOrEmpty(relationId))
				Importer.ThrowInvalidFile();
			Importer.ImportPivotCacheRelations(relationId);
		}
	}
	#endregion
	#region PivotCacheDefinitionDestination
	public class PivotCacheDefinitionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cacheSource", OnCacheSource);
			result.Add("cacheFields", OnCacheFields);
			result.Add("tupleCache", OnTupleCache);
			result.Add("calculatedItems", OnCalculatedItems);
			result.Add("calculatedMembers", OnCalculatedMembers);
			result.Add("cacheHierarchies", OnCacheHierarchies);
			result.Add("kpis", OnKpis);
			result.Add("dimensions", OnDimensions);
			result.Add("measureGroups", OnMeasureGroups);
			result.Add("maps", OnMaps);
			return result;
		}
		static Destination OnCacheSource(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheSourceDestination(importer, GetThis(importer).info.SourceInfo);
		}
		static Destination OnCacheFields(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheFieldsDestination(importer, GetThis(importer).info.Fields);
		}
		static Destination OnTupleCache(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTupleCache tupleCache = new PivotTupleCache();
			ImportPivotCacheInfo info = GetThis(importer).info;
			info.TupleCache = tupleCache;
			return new PivotTupleCacheDestination(importer, tupleCache);
		}
		static Destination OnCalculatedItems(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheCalculatedItemCollection items = new PivotCacheCalculatedItemCollection();
			ImportPivotCacheInfo info = GetThis(importer).info;
			info.CalculatedItems = items;
			return new PivotCacheCalculatedItemsDestination(importer, items);
		}
		static Destination OnCalculatedMembers(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheCalculatedMemberCollection members = new PivotCacheCalculatedMemberCollection();
			ImportPivotCacheInfo info = GetThis(importer).info;
			info.CalculatedMembers = members;
			return new PivotCacheCalculatedMembersDestination(importer, members);
		}
		static Destination OnCacheHierarchies(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheHierarchyCollection hierarchies = new PivotCacheHierarchyCollection();
			ImportPivotCacheInfo info = GetThis(importer).info;
			info.CacheHierarchies = hierarchies;
			return new PivotCacheHierarchiesDestination(importer, hierarchies);
		}
		static Destination OnKpis(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheKpiCollection kpis = new PivotCacheKpiCollection();
			ImportPivotCacheInfo info = GetThis(importer).info;
			info.Kpis = kpis;
			return new PivotCacheKpisDestination(importer, kpis);
		}
		static Destination OnDimensions(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheDimensionCollection dimensions = new PivotCacheDimensionCollection();
			ImportPivotCacheInfo info = GetThis(importer).info;
			info.Dimensions = dimensions;
			return new PivotCacheDimensionsDestination(importer, dimensions);
		}
		static Destination OnMeasureGroups(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheMeasureGroupCollection groups = new PivotCacheMeasureGroupCollection();
			ImportPivotCacheInfo info = GetThis(importer).info;
			info.MeasureGroups = groups;
			return new PivotCacheMeasureGroupsDestination(importer, groups);
		}
		static Destination OnMaps(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheDimensionMapCollection maps = new PivotCacheDimensionMapCollection();
			ImportPivotCacheInfo info = GetThis(importer).info;
			info.DimensionMaps = maps;
			return new PivotCacheDimensionMapsDestination(importer, maps);
		}
		static PivotCacheDefinitionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheDefinitionDestination)importer.PeekDestination();
		}
		#endregion
		readonly ImportPivotCacheInfo info;
		public PivotCacheDefinitionDestination(SpreadsheetMLBaseImporter importer, ImportPivotCacheInfo info)
			: base(importer) {
			Guard.ArgumentNotNull(info, "info");
			this.info = info;
			info.Fields = new PivotCacheFieldsCollection(importer.DocumentModel);
		}
		#region Properties
		protected internal ImportPivotCacheInfo Info { get { return info; } }
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			info.Invalid = Importer.GetOnOffValue(reader, "invalid", false);
			info.SaveData = Importer.GetOnOffValue(reader, "saveData", true);
			info.RefreshOnLoad = Importer.GetOnOffValue(reader, "refreshOnLoad", false);
			info.OptimizeMemory = Importer.GetOnOffValue(reader, "optimizeMemory", false);
			info.EnableRefresh = Importer.GetOnOffValue(reader, "enableRefresh", true);
			info.RefreshedBy = Importer.ReadAttribute(reader, "refreshedBy");
			double? refreshedDate = Importer.GetDoubleNullableValue(reader, "refreshedDate");
			if (refreshedDate.HasValue) {
				DateSystem dateSystem = Importer.DocumentModel.DataContext.DateSystem;
				if (!WorkbookDataContext.IsErrorDateTimeSerial(refreshedDate.Value, dateSystem))
					info.RefreshedDate = Importer.DocumentModel.DataContext.FromDateTimeSerial(refreshedDate.Value);
				else if (refreshedDate.Value > 0)
					info.RefreshedDate = DateTime.MaxValue;
				else
					info.RefreshedDate = DateTime.MinValue;
			}
			info.BackgroundQuery = Importer.GetOnOffValue(reader, "backgroundQuery", false);
			info.MissingItemsLimit = Importer.GetIntegerValue(reader, "missingItemsLimit", PivotCache.DefaultMissingItemsLimit);
			info.CreatedVersion = GetByteValue(reader, "createdVersion");
			info.RefreshedVersion = GetByteValue(reader, "refreshedVersion");
			info.MinRefreshableVersion = GetByteValue(reader, "minRefreshableVersion");
			info.UpgradeOnRefresh = Importer.GetOnOffValue(reader, "upgradeOnRefresh", false);
			info.HasTupleCache = Importer.GetOnOffValue(reader, "tupleCache", false);
			info.SupportSubquery = Importer.GetOnOffValue(reader, "supportSubquery", false);
			info.SupportAdvancedDrill = Importer.GetOnOffValue(reader, "supportAdvancedDrill", false);
		}
		byte GetByteValue(XmlReader reader, string attr) {
			int result = Importer.GetIntegerValue(reader, attr, 0);
			if (result > byte.MaxValue || result < 0)
				Importer.ThrowInvalidFile();
			return (byte)result;
		}
		public override void ProcessElementClose(XmlReader reader) {
			info.CreateAndRegisterPivotCache(Importer as OpenXmlImporter);
		}
	}
	#endregion
	#region PivotTupleCacheDestination
	public class PivotTupleCacheDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("entries", OnEntries);
			result.Add("sets", OnSets);
			result.Add("queryCache", OnQueryCache);
			result.Add("serverFormats", OnServerFormats);
			return result;
		}
		static Destination OnEntries(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTupleCache tupleCache = GetThis(importer).tupleCache;
			tupleCache.Entries = new PivotTupleCacheEntries();
			return new PivotTupleCacheEntriesDestination(importer, tupleCache.Entries);
		}
		static Destination OnSets(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTupleCache tupleCache = GetThis(importer).tupleCache;
			tupleCache.Sets = new PivotTupleCacheSets();
			return new PivotTupleCacheSetsDestination(importer, tupleCache.Sets);
		}
		static Destination OnQueryCache(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTupleCache tupleCache = GetThis(importer).tupleCache;
			tupleCache.QueryCache = new PivotTupleCacheQueries();
			return new PivotTupleCacheQueriesDestination(importer, tupleCache.QueryCache);
		}
		static Destination OnServerFormats(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTupleCache tupleCache = GetThis(importer).tupleCache;
			tupleCache.ServerFormats = new PivotTupleCacheServerFormats();
			return new PivotTupleCacheServerFormatsDestination(importer, tupleCache.ServerFormats);
		}
		static PivotTupleCacheDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTupleCacheDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotTupleCache tupleCache;
		public PivotTupleCacheDestination(SpreadsheetMLBaseImporter importer, PivotTupleCache tupleCache)
			: base(importer) {
			this.tupleCache = tupleCache;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotTupleCacheEntriesDestination
	public class PivotTupleCacheEntriesDestination : PivotCacheValueBaseCollectionDestination {
		readonly PivotTupleCacheEntries entries;
		public PivotTupleCacheEntriesDestination(SpreadsheetMLBaseImporter importer, PivotTupleCacheEntries entries)
			: base(importer) {
			this.entries = entries;
		}
		public override void AddItem(IPivotCacheRecordValue value) {
			entries.Add(value);
		}
	}
	#endregion
	#region PivotTupleCacheSetsDestination
	public class PivotTupleCacheSetsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("set", OnSet);
			return result;
		}
		static Destination OnSet(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTupleCacheSets sets = GetThis(importer).sets;
			PivotTupleCacheSet set = new PivotTupleCacheSet();
			sets.Add(set);
			return new PivotTupleCacheSetDestination(importer, set);
		}
		static PivotTupleCacheSetsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTupleCacheSetsDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotTupleCacheSets sets;
		public PivotTupleCacheSetsDestination(SpreadsheetMLBaseImporter importer, PivotTupleCacheSets sets)
			: base(importer) {
			this.sets = sets;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotTupleCacheSetDestination
	public class PivotTupleCacheSetDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("tpls", OnTuples);
			result.Add("sortByTuple", OnSortByTuple);
			return result;
		}
		static Destination OnTuples(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTupleCacheSet set = GetThis(importer).set;
			PivotTupleCollection tuples = new PivotTupleCollection();
			if (set.Tuples == null)
				set.Tuples = new List<PivotTupleCollection>();
			set.Tuples.Add(tuples);
			return new PivotTuplesDestination(importer, tuples);
		}
		static Destination OnSortByTuple(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTupleCacheSet set = GetThis(importer).set;
			set.SortByTuple = new PivotTupleCollection();
			return new PivotTuplesDestination(importer, set.SortByTuple);
		}
		static PivotTupleCacheSetDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTupleCacheSetDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotTupleCacheSet set;
		public PivotTupleCacheSetDestination(SpreadsheetMLBaseImporter importer, PivotTupleCacheSet set)
			: base(importer) {
			this.set = set;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			set.MaxRank = Importer.GetIntegerValue(reader, "maxRank", int.MinValue);
			set.SetDefinition = Importer.GetWpSTXString(reader, "setDefinition");
			if (set.MaxRank == int.MinValue || string.IsNullOrEmpty(set.SetDefinition))
				Importer.ThrowInvalidFile("PivotTupleCacheSet has no MaxRank or SetDefinition.");
			set.SortType = Importer.GetWpEnumValue(reader, "sortType", DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.PivotTupleCacheSetSortOrderTable, PivotTupleCacheSetSortOrder.None);
			set.QueryFailed = Importer.GetOnOffValue(reader, "queryFailed", false);
		}
	}
	#endregion
	#region PivotTupleCacheQueriesDestination
	public class PivotTupleCacheQueriesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("query", OnEntries);
			return result;
		}
		static Destination OnEntries(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTupleCacheQueries queryCache = GetThis(importer).queryCache;
			PivotTupleCacheQuery query = new PivotTupleCacheQuery();
			queryCache.Add(query);
			return new PivotTupleCacheQueryDestination(importer, query);
		}
		static PivotTupleCacheQueriesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTupleCacheQueriesDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotTupleCacheQueries queryCache;
		public PivotTupleCacheQueriesDestination(SpreadsheetMLBaseImporter importer, PivotTupleCacheQueries queryCache)
			: base(importer) {
			this.queryCache = queryCache;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotTupleCacheQueryDestination
	public class PivotTupleCacheQueryDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("tpls", OnTuples);
			return result;
		}
		static Destination OnTuples(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTupleCacheQuery query = GetThis(importer).query;
			query.Tuples = new PivotTupleCollection();
			return new PivotTuplesDestination(importer, query.Tuples);
		}
		static PivotTupleCacheQueryDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTupleCacheQueryDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotTupleCacheQuery query;
		public PivotTupleCacheQueryDestination(SpreadsheetMLBaseImporter importer, PivotTupleCacheQuery query)
			: base(importer) {
			this.query = query;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			query.Mdx = Importer.GetWpSTXString(reader, "mdx");
		}
	}
	#endregion
	#region PivotTupleCacheServerFormatsDestination
	public class PivotTupleCacheServerFormatsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("serverFormat", OnEntries);
			return result;
		}
		static Destination OnEntries(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTupleCacheServerFormats formats = GetThis(importer).serverFormats;
			PivotTupleCacheServerFormat format = new PivotTupleCacheServerFormat();
			formats.Add(format);
			return new PivotTupleCacheServerFormatDestination(importer, format);
		}
		static PivotTupleCacheServerFormatsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTupleCacheServerFormatsDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotTupleCacheServerFormats serverFormats;
		public PivotTupleCacheServerFormatsDestination(SpreadsheetMLBaseImporter importer, PivotTupleCacheServerFormats serverFormats)
			: base(importer) {
			this.serverFormats = serverFormats;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotTupleCacheServerFormatDestination
	public class PivotTupleCacheServerFormatDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotTupleCacheServerFormat serverFormat;
		public PivotTupleCacheServerFormatDestination(SpreadsheetMLBaseImporter importer, PivotTupleCacheServerFormat serverFormat)
			: base(importer) {
			this.serverFormat = serverFormat;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			serverFormat.Culture = Importer.GetWpSTXString(reader, "culture");
			serverFormat.Format = Importer.GetWpSTXString(reader, "format");
		}
	}
	#endregion
	#region PivotCacheCalculatedItemsDestination
	public class PivotCacheCalculatedItemsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("calculatedItem", OnCalculatedMember);
			return result;
		}
		static Destination OnCalculatedMember(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheCalculatedItemCollection items = GetThis(importer).items;
			PivotCacheCalculatedItem item = new PivotCacheCalculatedItem();
			items.Add(item);
			return new PivotCacheCalculatedItemDestination(importer, item);
		}
		static PivotCacheCalculatedItemsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheCalculatedItemsDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheCalculatedItemCollection items;
		public PivotCacheCalculatedItemsDestination(SpreadsheetMLBaseImporter importer, PivotCacheCalculatedItemCollection items)
			: base(importer) {
			this.items = items;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotCacheCalculatedItemDestination
	public class PivotCacheCalculatedItemDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("pivotArea", OnPivotArea);
			return result;
		}
		static Destination OnPivotArea(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new EmptyDestination<SpreadsheetMLBaseImporter>(importer);
		}
		#endregion
		readonly PivotCacheCalculatedItem item;
		public PivotCacheCalculatedItemDestination(SpreadsheetMLBaseImporter importer, PivotCacheCalculatedItem item)
			: base(importer) {
			this.item = item;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			item.Field = Importer.GetIntegerValue(reader, "field", -1);
			item.Formula = Importer.GetWpSTXString(reader, "formula");
		}
	}
	#endregion
	#region PivotCacheCalculatedMembersDestination
	public class PivotCacheCalculatedMembersDestination : ElementDestination<SpreadsheetMLBaseImporter> {
			#region Handler Table
			static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
			static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
				ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
				result.Add("calculatedMember", OnCalculatedMember);
				return result;
			}
			static Destination OnCalculatedMember(SpreadsheetMLBaseImporter importer, XmlReader reader) {
				PivotCacheCalculatedMemberCollection members = GetThis(importer).members;
				PivotCacheCalculatedMember member = new PivotCacheCalculatedMember();
				members.Add(member);
				return new PivotCacheCalculatedMemberDestination(importer, member);
			}
			static PivotCacheCalculatedMembersDestination GetThis(SpreadsheetMLBaseImporter importer) {
				return (PivotCacheCalculatedMembersDestination)importer.PeekDestination();
			}
			#endregion
			readonly PivotCacheCalculatedMemberCollection members;
			public PivotCacheCalculatedMembersDestination(SpreadsheetMLBaseImporter importer, PivotCacheCalculatedMemberCollection members)
				: base(importer) {
				this.members = members;
			}
			protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
	}
	#endregion
	#region PivotCacheCalculatedMemberDestination
	public class PivotCacheCalculatedMemberDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			return result;
		}
		#endregion
		readonly PivotCacheCalculatedMember member;
		public PivotCacheCalculatedMemberDestination(SpreadsheetMLBaseImporter importer, PivotCacheCalculatedMember member)
			: base(importer) {
			this.member = member;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			member.Name = Importer.GetWpSTXString(reader, "name");
			member.Mdx = Importer.GetWpSTXString(reader, "mdx");
			if (member.Name == null || member.Mdx == null)
				Importer.ThrowInvalidFile("PivotCacheCalculatedMember has no name or MDX.");
			member.MemberName = Importer.GetWpSTXString(reader, "memberName");
			member.Hierarchy = Importer.GetWpSTXString(reader, "hierarchy");
			member.Parent = Importer.GetWpSTXString(reader, "parent");
			member.SolveOrder = Importer.GetIntegerValue(reader, "solveOrder", 0);
			member.Set = Importer.GetOnOffValue(reader, "set", false);
		}
	}
	#endregion
}
