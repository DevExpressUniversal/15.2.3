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
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model.History;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotCache
	public class PivotCache {
		public static PivotCache Create(DocumentModel documentModel, IPivotCacheSource source, IErrorHandler errorHandler) {
			PivotCacheCreateCommand command = new PivotCacheCreateCommand(documentModel, errorHandler, source);
			command.Execute();
			return command.Result as PivotCache;
		}
		#region Consts
		internal const int DefaultMissingItemsLimit = -1;
		internal const int NoneMissingItemsLimit = 0;
		internal const int MaxMissingItemsLimit = 1048576;
		internal const uint invalidMask = 0x1;
		internal const uint saveDataMask = 0x2;
		internal const uint refreshOnLoadMask = 0x4;
		internal const uint optimizeMemoryMask = 0x8;
		internal const uint enableRefreshMask = 0x10;
		internal const uint backgroundQueryMask = 0x20;
		internal const uint upgradeOnRefreshMask = 0x40;
		internal const uint hasTupleCacheMask = 0x80;
		internal const uint supportSubqueryMask = 0x100;
		internal const uint supportAdvancedDrillMask = 0x200;
		#endregion
		#region Fields
		readonly DocumentModel documentModel;
		readonly IPivotCacheSource source;
		readonly PivotCacheRecordCollection records;
		readonly PivotCacheFieldsCollection cacheFields;
		PivotTupleCache tupleCache;
		PivotCacheCalculatedItemCollection calculatedItems;
		PivotCacheCalculatedMemberCollection calculatedMembers;
		PivotCacheHierarchyCollection cacheHierarchies;
		PivotCacheKpiCollection kpis;
		PivotCacheDimensionCollection dimensions;
		PivotCacheMeasureGroupCollection measureGroups;
		PivotCacheDimensionMapCollection dimensionMaps;
		uint packedValues;
		DateTime refreshedDate;
		string refreshedBy;
		int missingItemsLimit;
		byte createdVersion;
		byte refreshedVersion;
		byte minRefreshableVersion;
		#endregion
		public PivotCache(DocumentModel documentModel, IPivotCacheSource source) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			Guard.ArgumentNotNull(source, "source");
			this.documentModel = documentModel;
			this.records = new PivotCacheRecordCollection();
			this.source = source;
			this.cacheFields = new PivotCacheFieldsCollection(documentModel);
			this.missingItemsLimit = DefaultMissingItemsLimit;
		}
		#region Properties
		public bool RefreshNeeded { get { return Invalid || Records.Count == 0; } }
		public DocumentModel DocumentModel { get { return documentModel; } }
		public IPivotCacheSource Source { get { return source; } }
		public PivotCacheRecordCollection Records { get { return records; } }
		public PivotCacheFieldsCollection CacheFields { get { return cacheFields; } }
		public PivotTupleCache TupleCache { get { return tupleCache; } set { tupleCache = value; } }
		public PivotCacheCalculatedItemCollection CalculatedItems { get { return calculatedItems; } set { calculatedItems = value; } }
		public PivotCacheCalculatedMemberCollection CalculatedMembers { get { return calculatedMembers; } set { calculatedMembers = value; } }
		public PivotCacheHierarchyCollection CacheHierarchies { get { return cacheHierarchies; } set { cacheHierarchies = value; } }
		public PivotCacheKpiCollection Kpis { get { return kpis; } set { kpis = value; } }
		public PivotCacheDimensionCollection Dimensions { get { return dimensions; } set { dimensions = value; } }
		public PivotCacheMeasureGroupCollection MeasureGroups { get { return measureGroups; } set { measureGroups = value; } }
		public PivotCacheDimensionMapCollection DimensionMaps { get { return dimensionMaps; } set { dimensionMaps = value; } }
		#region SaveData
		public bool SaveData {
			get { return Utils.PackedValues.GetBoolBitValue(packedValues, saveDataMask); }
			set { HistoryHelper.SetPackedValuesBit(documentModel, packedValues, saveDataMask, value, SetPackedValuesCore); }
		}
		#endregion
		#region BackgroundQuery
		public bool BackgroundQuery {
			get { return Utils.PackedValues.GetBoolBitValue(packedValues, backgroundQueryMask); }
			set { HistoryHelper.SetPackedValuesBit(documentModel, packedValues, backgroundQueryMask, value, SetPackedValuesCore); }
		}
		#endregion
		#region EnableRefresh
		public bool EnableRefresh {
			get { return Utils.PackedValues.GetBoolBitValue(packedValues, enableRefreshMask); }
			set { HistoryHelper.SetPackedValuesBit(documentModel, packedValues, enableRefreshMask, value, SetPackedValuesCore); }
		}
		#endregion
		#region Invalid
		public bool Invalid {
			get { return Utils.PackedValues.GetBoolBitValue(packedValues, invalidMask); }
			set { HistoryHelper.SetPackedValuesBit(documentModel, packedValues, invalidMask, value, SetPackedValuesCore); }
		}
		#endregion
		#region OptimizeMemory
		public bool OptimizeMemory {
			get { return Utils.PackedValues.GetBoolBitValue(packedValues, optimizeMemoryMask); }
			set { HistoryHelper.SetPackedValuesBit(documentModel, packedValues, optimizeMemoryMask, value, SetPackedValuesCore); }
		}
		#endregion
		#region RefreshOnLoad
		public bool RefreshOnLoad {
			get { return Utils.PackedValues.GetBoolBitValue(packedValues, refreshOnLoadMask); }
			set { HistoryHelper.SetPackedValuesBit(documentModel, packedValues, refreshOnLoadMask, value, SetPackedValuesCore); }
		}
		#endregion
		#region UpgradeOnRefresh
		public bool UpgradeOnRefresh {
			get { return Utils.PackedValues.GetBoolBitValue(packedValues, upgradeOnRefreshMask); }
			set { HistoryHelper.SetPackedValuesBit(documentModel, packedValues, upgradeOnRefreshMask, value, SetPackedValuesCore); }
		}
		#endregion
		#region HasTupleCache
		public bool HasTupleCache {
			get { return Utils.PackedValues.GetBoolBitValue(packedValues, hasTupleCacheMask); }
			set { HistoryHelper.SetPackedValuesBit(documentModel, packedValues, hasTupleCacheMask, value, SetPackedValuesCore); }
		}
		#endregion
		#region SupportAdvancedDrill
		public bool SupportAdvancedDrill {
			get { return Utils.PackedValues.GetBoolBitValue(packedValues, supportAdvancedDrillMask); }
			set { HistoryHelper.SetPackedValuesBit(documentModel, packedValues, supportAdvancedDrillMask, value, SetPackedValuesCore); }
		}
		#endregion
		#region SupportSubquery
		public bool SupportSubquery {
			get { return Utils.PackedValues.GetBoolBitValue(packedValues, supportSubqueryMask); }
			set { HistoryHelper.SetPackedValuesBit(documentModel, packedValues, supportSubqueryMask, value, SetPackedValuesCore); }
		}
		#endregion
		#region MissingItemsLimit
		public int MissingItemsLimit { get { return missingItemsLimit; } set { HistoryHelper.SetValue(documentModel, missingItemsLimit, value, SetMissingItemsLimitCore); } }
		protected internal void SetMissingItemsLimitCore(int value) {
			missingItemsLimit = value;
		}
		#endregion
		#region RefreshedBy
		public string RefreshedBy { get { return refreshedBy; } set { HistoryHelper.SetValue(documentModel, refreshedBy, value, SetRefreshedByCore); } }
		protected internal void SetRefreshedByCore(string value) {
			refreshedBy = value;
		}
		#endregion
		#region RefreshedDate
		public DateTime RefreshedDate { get { return refreshedDate; } set { HistoryHelper.SetValue(documentModel, refreshedDate, value, SetRefreshedDateCore); } }
		protected internal void SetRefreshedDateCore(DateTime value) {
			refreshedDate = value;
		}
		#endregion
		#region CreatedVersion
		public byte CreatedVersion { get { return createdVersion; } set { HistoryHelper.SetValue(documentModel, createdVersion, value, SetCreatedVersionCore); } }
		protected internal void SetCreatedVersionCore(byte value) {
			createdVersion = value;
		}
		#endregion
		#region RefreshedVersion
		public byte RefreshedVersion { get { return refreshedVersion; } set { HistoryHelper.SetValue(documentModel, refreshedVersion, value, SetRefreshedVersionCore); } }
		protected internal void SetRefreshedVersionCore(byte value) {
			refreshedVersion = value;
		}
		#endregion
		#region MinRefreshableVersion
		public byte MinRefreshableVersion { get { return minRefreshableVersion; } set { HistoryHelper.SetValue(documentModel, minRefreshableVersion, value, SetMinRefreshableVersionCore); } }
		protected internal void SetMinRefreshableVersionCore(byte value) {
			minRefreshableVersion = value;
		}
		#endregion
		#endregion
		void SetPackedValuesCore(uint value) {
			this.packedValues = value;
		}
		internal void Clear() {
			Records.Clear();
			Invalid = false;
			RefreshedDate = DateTime.Now;
			RefreshedBy = DocumentModel.GetUserName();
			CacheHierarchies = null;
			TupleCache = null;
			CalculatedItems = null;
			CalculatedMembers = null;
			Kpis = null;
			Dimensions = null;
			MeasureGroups = null;
			DimensionMaps = null;
		}
		public bool ContainsGrouppedOrCalculatedFields() {
			foreach (IPivotCacheField field in CacheFields)
				if (!field.DatabaseField || field.FieldGroup.Initialized)
					return true;
			return false;
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			Source.OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			Source.OnRangeRemoving(context);
		}
		public bool Refresh(IErrorHandler errorHandler) {
			PivotCacheRefreshCommand command = new PivotCacheRefreshCommand(errorHandler, this);
			return command.Execute();
		}
		internal void CopyDataTo(PivotCache targetCache) {
			targetCache.records.Clear();
			targetCache.records.CopyFrom(records);
			targetCache.cacheFields.ClearCore();
			foreach (IPivotCacheField sourceItem in cacheFields) {
				PivotCacheField sourceField = sourceItem as PivotCacheField;
				PivotCacheField targetField = new PivotCacheField(DocumentModel);
				targetField.CopyFrom(sourceField);
				targetCache.cacheFields.AddCore(targetField);
			}
		}
		public void CopyFrom(PivotCache sourceCache, Worksheet worksheet, CellPositionOffset rangeOffset) {
			CopyPtCacheFields(sourceCache.CacheFields, this.CacheFields, worksheet.Workbook);
			CopyPtCacheRecords(sourceCache.Records, this.Records);
			CopyPtCacheHierarchies(sourceCache.CacheHierarchies, this.CacheHierarchies);
			CopyPtCacheCalculatedItems(sourceCache.CalculatedItems, this.CalculatedItems, worksheet, rangeOffset);
			CopyPtCacheCalculatedMembers(sourceCache.CalculatedMembers, this.CalculatedMembers);
			this.createdVersion = sourceCache.createdVersion;
			CopyPtCacheDimensionMaps(sourceCache.DimensionMaps, this.DimensionMaps);
			CopyPtCacheDimensions(sourceCache.Dimensions, this.Dimensions);
			CopyPtCacheKpis(sourceCache.Kpis, this.Kpis);
			CopyPtCacheMeasureGroupCollection(sourceCache.MeasureGroups, this.MeasureGroups);
			this.minRefreshableVersion = sourceCache.MinRefreshableVersion;
			this.missingItemsLimit = sourceCache.MissingItemsLimit;
			this.packedValues = sourceCache.packedValues;
			this.refreshedBy = sourceCache.refreshedBy;
			this.refreshedDate = sourceCache.refreshedDate;
			this.refreshedVersion = sourceCache.refreshedVersion;
			CopyPtCacheTupleCache(sourceCache.TupleCache, this.TupleCache);
		}
		public IEnumerable<IPivotCacheRecord> GetRowByKeys(PivotDataKey key, List<int> keyFieldIndices) {
			foreach (IPivotCacheRecord record in Records) {
				bool isIncludeRow = true;
				for (int keysIndex = keyFieldIndices.Count - 1; keysIndex >= 0; keysIndex--) {
					int cField = keyFieldIndices[keysIndex];
					int sharedIndex = ((PivotCacheRecordSharedItemsIndexValue)record[cField]).IndexValue;
					if (key[keysIndex] != -1 && sharedIndex != key[keysIndex]) {
						isIncludeRow = false;
						break;
					}
				}
				if (isIncludeRow)
					yield return record;
			}
		}
		private void CopyPtCacheTupleCache(PivotTupleCache source, PivotTupleCache target) {
			if (source == null)
				return;
			target.CopyFrom(source);
		}
		void CopyPtCacheMeasureGroupCollection(PivotCacheMeasureGroupCollection source, PivotCacheMeasureGroupCollection target) {
			if (source == null)
				return;
			foreach (PivotCacheMeasureGroup sourceItem in source) {
				PivotCacheMeasureGroup targetItem = new PivotCacheMeasureGroup();
				targetItem.CopyFrom(sourceItem);
				target.Add(targetItem);
			}
		}
		void CopyPtCacheKpis(PivotCacheKpiCollection source, PivotCacheKpiCollection target) {
			if (source == null)
				return;
			foreach (PivotCacheKpi sourceKpi in source) {
				PivotCacheKpi targetItem = new PivotCacheKpi();
				targetItem.CopyFrom(sourceKpi);
				target.Add(targetItem);
			}
		}
		void CopyPtCacheDimensions(PivotCacheDimensionCollection source, PivotCacheDimensionCollection target) {
			if (source == null)
				return;
			target.Capacity = source.Count;
			Debug.Assert(target.Count == 0);
			foreach (PivotCacheDimension sourceItem in source) {
				PivotCacheDimension targetItem = new PivotCacheDimension();
				targetItem.CopyFrom(sourceItem);
				target.Add(targetItem);
			}
		}
		void CopyPtCacheDimensionMaps(PivotCacheDimensionMapCollection source, PivotCacheDimensionMapCollection target) {
			if (source == null)
				return;
			target.Capacity = source.Count;
			Debug.Assert(target.Count == 0);
			foreach (PivotCacheDimensionMap sourceItem in source) {
				PivotCacheDimensionMap targetItem = new PivotCacheDimensionMap();
				targetItem.CopyFrom(sourceItem);
				target.Add(targetItem);
			}
		}
		void CopyPtCacheCalculatedMembers(PivotCacheCalculatedMemberCollection source, PivotCacheCalculatedMemberCollection target) {
			if (source == null)
				return;
			target.Capacity = source.Count;
			foreach (PivotCacheCalculatedMember sourceMember in source) {
				PivotCacheCalculatedMember targetMember = new PivotCacheCalculatedMember();
				targetMember.CopyFrom(sourceMember);
				target.Add(targetMember);
			}
		}
		void CopyPtCacheCalculatedItems(PivotCacheCalculatedItemCollection source, PivotCacheCalculatedItemCollection target, Worksheet anotherWorksheet, CellPositionOffset rangeOffset) {
			if (source == null)
				return;
			target.Capacity = source.Count;
			Debug.Assert(target.Count == 0);
			foreach (PivotCacheCalculatedItem item in source) {
				PivotCacheCalculatedItem targetItem = new PivotCacheCalculatedItem();
				targetItem.CopyFrom(item, anotherWorksheet.Workbook, anotherWorksheet, rangeOffset);
				target.Add(targetItem);
			}
		}
		void CopyPtCacheHierarchies(PivotCacheHierarchyCollection source, PivotCacheHierarchyCollection target) {
			if (source == null)
				return;
			target.Capacity = source.Count;
			Debug.Assert(target.Count == 0);
			foreach (PivotCacheHierarchy sourceItem in source) {
				PivotCacheHierarchy targetItem = new PivotCacheHierarchy();
				targetItem.CopyFrom(sourceItem);
				target.Add(targetItem);
			}
		}
		void CopyPtCacheRecords(PivotCacheRecordCollection source, PivotCacheRecordCollection target) {
			target.CopyFrom(source);
		}
		void CopyPtCacheFields(PivotCacheFieldsCollection source, PivotCacheFieldsCollection target, DocumentModel anotherWorkbook) {
			target.Capacity = source.Count;
			Debug.Assert(target.Count == 0, "", "");
			foreach (IPivotCacheField sourceItem in source) {
				PivotCacheField sourceField = sourceItem as PivotCacheField;
				PivotCacheField targetField = new PivotCacheField(anotherWorkbook);
				targetField.CopyFrom(sourceField);
				target.Add(targetField);
			}
		}
	}
	#endregion
	#region PivotCacheCollection
	public class PivotCacheCollection : UndoableCollection<PivotCache> {
		public PivotCacheCollection(IDocumentModel documentModel)
			: base(documentModel.MainPart) {
		}
		public bool RemoveCacheIfIsNotReferenced(PivotCache cache) {
			if (CacheIsReferenced(cache))
				return false;
			Remove(cache);
			cache.Records.Clear();
			return true;
		}
		public bool CacheIsReferenced(PivotCache cache) {
			DocumentModel documentModel = (DocumentModel)DocumentModel;
			for (int i = 0; i < documentModel.Sheets.Count; i++) {
				PivotTableCollection pivotCollection = documentModel.Sheets[i].PivotTables;
				for (int j = 0; j < pivotCollection.Count; j++)
					if (object.ReferenceEquals(pivotCollection[j].Cache, cache))
						return true;
			}
			return false;
		}
		public PivotCache TryGetPivotCache(IPivotCacheSource source) {
			int maxIndex = -1;
			DateTime maxDateTime = DateTime.MinValue;
			WorkbookDataContext context = ((DocumentModel)DocumentModel).DataContext;
			for (int i = 0; i < Count; ++i) {
				PivotCache cache = this[i];
				if (cache.Source.Equals(source) && DateTime.Compare(maxDateTime, cache.RefreshedDate) < 0) {
					maxDateTime = cache.RefreshedDate;
					maxIndex = i;
				}
			}
			if (maxIndex >= 0)
				return this[maxIndex];
			return null;
		}
		public void OnRangeInserting(InsertRangeNotificationContext context) {
			for (int i = 0; i < Count; ++i)
				this[i].OnRangeInserting(context);
		}
		public void OnRangeRemoving(RemoveRangeNotificationContext context) {
			for (int i = 0; i < Count; ++i)
				this[i].OnRangeRemoving(context);
		}
		public bool RefreshAll(IErrorHandler errorHandler) {
			PivotCacheRefreshAllCommand command = new PivotCacheRefreshAllCommand(this, errorHandler);
			return command.Execute();
		}
	}
	#endregion
}
