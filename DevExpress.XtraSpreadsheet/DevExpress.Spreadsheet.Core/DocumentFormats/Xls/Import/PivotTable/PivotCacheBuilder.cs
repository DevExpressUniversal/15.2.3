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
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Internal;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsPivotDataType
	public enum XlsPivotCacheFieldDataType {
		GroupingRecord = 0,
		GroupingData = 1,
		SharedRecord = 2,
		CacheRecord = 3
	}
	#endregion
	#region IXlsPivotCacheBuilder
	public interface IXlsPivotCacheBuilder {
		PivotCacheType CacheType { get; }
		List<XlsPivotCacheCalculatedItem> CalculatedItems { get; }
		List<XlsPivotCacheCalculatedField> CalculatedFields { get; }
		List<int> EmptyCacheFieldIndexes { get; }
		List<int> SharedItemsCount { get; }
		int FilterCountItem { get; set; }
		bool IsEmpty { get; }
		void ResetRecordsCount(int groupRecordsCount, int sharedRecordsCount);
		void ResetGroupingDataCount();
		void AddRecord(XlsContentBuilder contentBuilder, IPivotCacheRecordValue value);
		void Execute(XlsContentBuilder contentBuilder);
	}
	#endregion
	#region XlsPivotCacheEmptyBuilder
	public class XlsPivotCacheEmptyBuilder : IXlsPivotCacheBuilder {
		static IXlsPivotCacheBuilder instance = new XlsPivotCacheEmptyBuilder();
		#region Properties
		public static IXlsPivotCacheBuilder Instance { get { return instance; } }
		public PivotCacheType CacheType { get { return PivotCacheType.Worksheet; } }
		public List<XlsPivotCacheCalculatedItem> CalculatedItems { get { return null; } }
		public List<XlsPivotCacheCalculatedField> CalculatedFields { get { return null; } }
		public List<int> EmptyCacheFieldIndexes { get { return null; } }
		public List<int> SharedItemsCount { get { return null; } }
		public int FilterCountItem { get; set; }
		public bool IsEmpty { get { return true; } }
		#endregion
		#region IXlsPivotCacheBuilder Members
		public void Execute(XlsContentBuilder contentBuilder) {
		}
		public void ResetRecordsCount(int groupItemsCount, int sharedItemsCount) {
		}
		public void ResetGroupingDataCount() {
		}
		public void AddRecord(XlsContentBuilder contentBuilder, IPivotCacheRecordValue value) {
		}
		#endregion
	}
	#endregion
	#region XlsPivotCacheBuilderBase
	public abstract class XlsPivotCacheBuilderBase : IXlsPivotCacheBuilder {
		#region Fields
		PivotCacheRecordValueCollection cacheRecordValues = new PivotCacheRecordValueCollection();
		List<XlsPivotCacheCalculatedItem> calculatedItems = new List<XlsPivotCacheCalculatedItem>();
		List<XlsPivotCacheCalculatedField> calculatedFields = new List<XlsPivotCacheCalculatedField>();
		List<int> emptyCacheFieldIndexes = new List<int>();
		List<int> sharedItemsCount = new List<int>();
		#endregion
		#region Properties
		public abstract PivotCacheType CacheType { get; }
		public int StreamId { get; set; }
		public List<XlsPivotCacheCalculatedItem> CalculatedItems { get { return calculatedItems; } }
		public List<XlsPivotCacheCalculatedField> CalculatedFields { get { return calculatedFields; } }
		public List<int> EmptyCacheFieldIndexes { get { return emptyCacheFieldIndexes; } }
		public List<int> SharedItemsCount { get { return sharedItemsCount; } }
		public bool IgnoreAddlCache12Records { get; set; }
		public bool IsEmpty { get { return false; } }
		public int MaxValuesInRecord { get; set; }
		public int FilterCountItem { get; set; }
		public XlsPivotCacheFieldDataType DataType { get { return GetDataType(); } }
		public int MaxUnusedItems { get; set; }
		public byte VersionCacheLastRefresh { get; set; }
		public byte VersionCacheRefreshableMin { get; set; }
		public DateTime DateLastRefreshed { get; set; }
		public byte CreatedVersion { get; set; }
		public bool EnableRefresh { get; set; }
		public bool Invalid { get; set; }
		public bool SupportsAttributeDrillDown { get; set; }
		public bool SupportsSubQuery { get; set; }
		int GroupRecordsCount { get; set; }
		int SharedRecordsCount { get; set; }
		int CurrentGroupRecord { get; set; }
		int CurrentSharedRecord { get; set; }
		int GroupingDataCount { get; set; }
		int CurrentGroupingData { get; set; }
		int CurrentEmptyCacheFieldIndex { get; set; }
		#endregion
		#region Execute
		public void Execute(XlsContentBuilder contentBuilder) {
			DocumentModel documentModel = contentBuilder.DocumentModel;
			documentModel.BeginUpdate();
			try {
				PivotCache pivotCache = new PivotCache(documentModel, CreateCacheSource(contentBuilder));
				documentModel.PivotCaches.Add(pivotCache);
				string streamName = string.Format("_SX_DB_CUR\\{0:X4}", StreamId);
				contentBuilder.ReadPivotCacheContent(streamName);
				ProcessFormulas(contentBuilder);
				SetupAdditionalProperties(pivotCache);
			}
			finally {
				documentModel.EndUpdate();
			}
		}
		protected internal abstract IPivotCacheSource CreateCacheSource(XlsContentBuilder contentBuilder);
		void SetupAdditionalProperties(PivotCache pivotCache) {
			pivotCache.MissingItemsLimit = MaxUnusedItems;
			if (VersionCacheLastRefresh != 0xff)
				pivotCache.RefreshedVersion = VersionCacheLastRefresh;
			if (VersionCacheRefreshableMin != 0xff)
				pivotCache.MinRefreshableVersion = VersionCacheRefreshableMin;
			pivotCache.RefreshedDate = DateLastRefreshed;
			if (CreatedVersion == 0)
				CreatedVersion = 1;
			if (pivotCache.CreatedVersion == 0)
				pivotCache.CreatedVersion = CreatedVersion;
			if (CreatedVersion > 2) {
				pivotCache.Invalid = Invalid;
				pivotCache.EnableRefresh = EnableRefresh;
			}
			pivotCache.SupportAdvancedDrill = SupportsAttributeDrillDown;
			pivotCache.SupportSubquery = SupportsSubQuery;
		}
		#endregion
		#region ResetCount
		public void ResetRecordsCount(int groupRecordsCount, int sharedRecordsCount) {
			CurrentGroupRecord = 0;
			CurrentSharedRecord = 0;
			GroupRecordsCount = groupRecordsCount;
			SharedRecordsCount = sharedRecordsCount;
		}
		public void ResetGroupingDataCount() {
			CurrentGroupingData = 0;
			GroupingDataCount = 3;
		}
		#endregion
		#region AddRecord
		public void AddRecord(XlsContentBuilder contentBuilder, IPivotCacheRecordValue value) {
			PivotCache pivotCache = contentBuilder.DocumentModel.PivotCaches.Last;
			IPivotCacheField cacheField = pivotCache.CacheFields.Last;
			switch (DataType) {
				case XlsPivotCacheFieldDataType.GroupingRecord:
					AddFieldGroupItem(cacheField.FieldGroup.GroupItems, value);
					return;
				case XlsPivotCacheFieldDataType.GroupingData:
					SetGroupingData(cacheField.FieldGroup.RangeGroupingProperties, value);
					return;
				case XlsPivotCacheFieldDataType.SharedRecord:
					AddFieldSharedItem(cacheField.SharedItems, value);
					return;
				case XlsPivotCacheFieldDataType.CacheRecord:
					AddCacheRecordValue(pivotCache, value);
					return;
			}
		}
		void AddFieldGroupItem(PivotCacheGroupItems groupItems, IPivotCacheRecordValue value) {
			groupItems.Add(value);
			CurrentGroupRecord++;
		}
		void AddFieldSharedItem(PivotCacheSharedItemsCollection sharedItems, IPivotCacheRecordValue value) {
			sharedItems.Add(value);
			CurrentSharedRecord++;
		}
		void AddCacheRecordValue(PivotCache pivotCache, IPivotCacheRecordValue value) {
			if (value.ValueType == PivotCacheRecordValueType.SharedItemIndex) {
				cacheRecordValues.Add(value);
				CurrentEmptyCacheFieldIndex = 0;
			}
			else {
				int index = EmptyCacheFieldIndexes[CurrentEmptyCacheFieldIndex];
				value = value.ToSharedItem(pivotCache.CacheFields[index]);
				cacheRecordValues.Insert(index, value);
				CurrentEmptyCacheFieldIndex++;
			}
			if (cacheRecordValues.Count == MaxValuesInRecord) {
				pivotCache.Records.Add(new PivotCacheRecord(cacheRecordValues));
				cacheRecordValues = new PivotCacheRecordValueCollection();
				CurrentEmptyCacheFieldIndex = 0;
			}
		}
		XlsPivotCacheFieldDataType GetDataType() {
			if (CurrentGroupRecord < GroupRecordsCount)
				return XlsPivotCacheFieldDataType.GroupingRecord;
			else if (GroupingDataCount > 0 && CurrentGroupingData < GroupingDataCount)
				return XlsPivotCacheFieldDataType.GroupingData;
			else if (CurrentSharedRecord < SharedRecordsCount)
				return XlsPivotCacheFieldDataType.SharedRecord;
			return XlsPivotCacheFieldDataType.CacheRecord;
		}
		#endregion
		#region SetGroupingData
		void SetGroupingData(PivotCacheRangeGroupingProperties groupingProperties, IPivotCacheRecordValue value) {
			if (groupingProperties.GroupBy == PivotCacheGroupValuesBy.NumericRanges)
				SetRangeGroupingData(groupingProperties, value);
			else
				SetDateGroupingData(groupingProperties, value);
			CurrentGroupingData++;
		}
		void SetRangeGroupingData(PivotCacheRangeGroupingProperties groupingProperties, IPivotCacheRecordValue value) {
			double numericValue = ((PivotCacheRecordNumericValue)value).Value;
			if (CurrentGroupingData == 0)
				groupingProperties.StartNum = numericValue;
			else if (CurrentGroupingData == 1)
				groupingProperties.EndNum = numericValue;
			else
				groupingProperties.GroupInterval = numericValue;
		}
		void SetDateGroupingData(PivotCacheRangeGroupingProperties groupingProperties, IPivotCacheRecordValue value) {
			if (CurrentGroupingData == 2)
				groupingProperties.GroupInterval = ((PivotCacheRecordNumericValue)value).Value;
			else {
				DateTime dateTimeValue  = ((PivotCacheRecordDateTimeValue)value).Value;
				if (CurrentGroupingData == 0)
					groupingProperties.StartDate = dateTimeValue;
				else
					groupingProperties.EndDate = dateTimeValue;
			}
		}
		#endregion
		#region ProcessFormulas
		protected internal void ProcessFormulas(XlsContentBuilder contentBuilder) {
			XlsRPNContext context = contentBuilder.RPNContext;
			PivotCache pivotCache = contentBuilder.DocumentModel.PivotCaches.Last;
			ProcessCalculatedFields(pivotCache.CacheFields, context);
			ProcessCalculatedItems(pivotCache, context);
		}
		void ProcessCalculatedFields(PivotCacheFieldsCollection collection, XlsRPNContext context) {
			foreach (XlsPivotCacheCalculatedField calculatedField in calculatedFields) {
				int index = calculatedField.SourceCacheFieldIndex;
				collection[index].Formula = calculatedField.BuildExpressionString(context);
			}
		}
		void ProcessCalculatedItems(PivotCache pivotCache, XlsRPNContext context) {
			int count = calculatedItems.Count;
			for (int i = 0; i < count; i++) {
				PivotCacheCalculatedItem item = pivotCache.CalculatedItems[i];
				item.Formula = calculatedItems[i].BuildExpressionString(context);
				SetFormulaFlags(pivotCache.CacheFields, item.PivotArea.References);
			}
		}
		void SetFormulaFlags(PivotCacheFieldsCollection cacheFields, PivotAreaReferenceCollection references) {
			foreach (PivotAreaReference reference in references) {
				if (!reference.Field.HasValue)
					continue;
				IPivotCacheField cacheField = cacheFields[(int)reference.Field];
				foreach (int index in reference.SharedItemsIndex) {
					PivotCacheRecordCharacterValue value = cacheField.SharedItems[index] as PivotCacheRecordCharacterValue;
					Guard.ArgumentNotNull(value, "value must be PivotCacheRecordCharacterValue!");
					value.IsCalculatedItem = true;
				}
			}
		}
		#endregion
	}
	#endregion
	#region XlsPivotWorksheetCacheBuilder
	public class XlsPivotWorksheetCacheBuilder : XlsPivotCacheBuilderBase {
		XlsPivotDataConsolidationContainer dataRef = new XlsPivotDataConsolidationContainer();
		#region Properties
		public override PivotCacheType CacheType { get { return PivotCacheType.Worksheet; } }
		public XlsPivotDataConsolidationContainer DataRef { get { return dataRef; } }
		#endregion
		protected internal override IPivotCacheSource CreateCacheSource(XlsContentBuilder contentBuilder) {
			return dataRef.GetReference(contentBuilder.DocumentModel.DataContext);
		}
	}
	#endregion
	#region XlsPivotConsolidationCacheBuilder
	public class XlsPivotConsolidationCacheBuilder : XlsPivotCacheBuilderBase {
		#region Fields
		readonly List<XlsPivotDataConsolidationContainer> dataRefs = new List<XlsPivotDataConsolidationContainer>();
		readonly List<XlsPivotConsolidationCachePage> pages = new List<XlsPivotConsolidationCachePage>();
		readonly List<XlsPivotConsolidationCacheRangeMap> rangeMaps = new List<XlsPivotConsolidationCacheRangeMap>();
		#endregion
		#region Properties
		public override PivotCacheType CacheType { get { return PivotCacheType.Consolidation; } }
		public List<XlsPivotDataConsolidationContainer> DataRefs { get { return dataRefs; } }
		public List<XlsPivotConsolidationCachePage> Pages { get { return pages; } }
		public List<XlsPivotConsolidationCacheRangeMap> RangeMaps { get { return rangeMaps; } }
		public bool AutoPage { get; set; }
		public int NumberOfRanges { get; set; }
		public int NumberOfOptionalFields { get; set; }
		#endregion
		protected internal override IPivotCacheSource CreateCacheSource(XlsContentBuilder contentBuilder) {
			if (!CheckIntegrity())
				contentBuilder.ThrowInvalidFile("Invalid pivot data consolidation cache.");
			PivotCacheSourceConsolidation source = new PivotCacheSourceConsolidation();
			int refsCount = dataRefs.Count;
			for (int i = 0; i < refsCount; i++) {
				PivotCacheSourceWorksheet reference = dataRefs[i].GetReference(contentBuilder.DocumentModel.DataContext);
				XlsPivotConsolidationCacheRangeMap rangeMap = rangeMaps[i];
				PivotCacheSourceConsolidationRangeSet rangeSet = new PivotCacheSourceConsolidationRangeSet(
					reference,
					GetFieldItemIndex(rangeMap, 0),
					GetFieldItemIndex(rangeMap, 1),
					GetFieldItemIndex(rangeMap, 2),
					GetFieldItemIndex(rangeMap, 3)
					);
				source.RangeSets.Add(rangeSet);
			}
			int pagesCount = pages.Count;
			for (int i = 0; i < pagesCount; i++)
				source.Pages.Add(CreatePage(pages[i]));
			source.AutoPage = AutoPage;
			return source;
		}
		int GetFieldItemIndex(XlsPivotConsolidationCacheRangeMap rangeMap, int index) {
			return rangeMap.Count <= index ? -1 : rangeMap[index];
		}
		PivotCacheSourceConsolidationPage CreatePage(List<string> itemNames) {
			PivotCacheSourceConsolidationPage result = new PivotCacheSourceConsolidationPage();
			foreach (string value in itemNames)
				result.ItemNames.Add(value);
			return result;
		}
		bool CheckIntegrity() {
			int dataRefsCount = dataRefs.Count;
			foreach (XlsPivotConsolidationCacheRangeMap rangeMap in rangeMaps) {
				int rangeMapValuesCount = rangeMap.Count;
				if (rangeMapValuesCount != NumberOfOptionalFields)
					return false;
				if (rangeMapValuesCount != pages.Count)
					return false;
				for (int i = 0; i < rangeMapValuesCount; i++) {
					if (rangeMap[i] >= pages[i].Count)
						return false;
				}
			}
			return dataRefsCount == NumberOfRanges && dataRefsCount == rangeMaps.Count;
		}
	}
	#endregion
	#region XlsPivotScenarioCacheBuilder
	public class XlsPivotScenarioCacheBuilder : XlsPivotCacheBuilderBase {
		#region Properties
		public override PivotCacheType CacheType { get { return PivotCacheType.Scenario; } }
		#endregion
		protected internal override IPivotCacheSource CreateCacheSource(XlsContentBuilder contentBuilder) {
			return new PivotCacheSourceScenario(); 
		}
	}
	#endregion
	#region XlsPivotExternalCacheBuilder
	public class XlsPivotExternalCacheBuilder : XlsPivotCacheBuilderBase {
		#region Fields
		readonly List<XlsDbQuery> dbQueryList = new List<XlsDbQuery>();
		readonly List<XlsParamQuery> paramQueryList = new List<XlsParamQuery>();
		#endregion
		#region Properties
		public override PivotCacheType CacheType { get { return PivotCacheType.External; } }
		public List<XlsDbQuery> DbQueryList { get { return dbQueryList; } }
		public List<XlsParamQuery> ParamQueryList { get { return paramQueryList; } }
		#endregion
		protected internal override IPivotCacheSource CreateCacheSource(XlsContentBuilder contentBuilder) {
			PivotCacheSourceExternal source = new PivotCacheSourceExternal();
			return source;
		}
	}
	#endregion
	#region XlsPivotConsolidationCacheRangeMap
	public class XlsPivotConsolidationCacheRangeMap : List<int> {
	}
	#endregion
	#region XlsPivotConsolidationCachePage
	public class XlsPivotConsolidationCachePage : List<string> {
	}
	#endregion
	#region XlsPivotDataConsolidationContainer
	public class XlsPivotDataConsolidationContainer {
		#region Fields
		CellRangeInfo range = null;
		string virtualPath = String.Empty;
		string name = String.Empty;
		string bookFileName = String.Empty;
		string sheetName = String.Empty;
		string rangeRef = String.Empty;
		#endregion
		public XlsPivotDataConsolidationContainer() {
		}
		public XlsPivotDataConsolidationContainer(CellRangeInfo range, string virtualPath) {
			Range = range;
			VirtualPath = virtualPath;
		}
		public XlsPivotDataConsolidationContainer(string name, string virtualPath) {
			Name = name;
			VirtualPath = virtualPath;
		}
		#region Properties
		public CellRangeInfo Range { get { return range; } 
			set {
				if (value == null)
					return;
				range = value;
				rangeRef = Range.First.ToString() + ":" + Range.Last.ToString();
			} 
		}
		public string VirtualPath { get { return virtualPath; } 
			set { 
				virtualPath = value;
				bookFileName = GetBookFileName();
				sheetName = GetSheetName();
			} 
		}
		public string Name { get { return name; } set { name = value; } }
		#endregion
		public PivotCacheSourceWorksheet GetReference(WorkbookDataContext context) {
			PivotCacheSourceWorksheet result = PivotCacheSourceWorksheetHelper.GetSource(sheetName, name, rangeRef, bookFileName, context);
			if (result == null)
				throw new Exception(XlsImporter.InvalidFileMessage);
			return result;
		}
		string GetBookFileName() {
			int bookFileNameStart = virtualPath.IndexOf('[');
			int bookFileNameEnd = virtualPath.IndexOf(']');
			if (bookFileNameEnd > 0 && bookFileNameStart < bookFileNameEnd)
				return virtualPath.Substring(bookFileNameStart + 1, bookFileNameEnd - bookFileNameStart - 1);
			return String.Empty;
		}
		string GetSheetName() {
			string result = virtualPath;
			int pos = virtualPath.LastIndexOf(']');
			if (pos != -1)
				result = virtualPath.Remove(0, pos + 1);
			return result.Trim('\u0002');
		}
	}
	#endregion
	#region XlsPivotCacheCalculatedItemBase
	public abstract class XlsPivotCacheCalculatedItemBase {
		ParsedExpression expression;
		protected XlsPivotCacheCalculatedItemBase(ParsedExpression expression) {
			Guard.ArgumentNotNull(expression, "expression");
			this.expression = expression;
		}
		public string BuildExpressionString(XlsRPNContext context) {
			ParsedExpression result = XlsParsedThingConverter.ToModelExpression(expression, context);
			foreach (IParsedThing ptg in result) {
				ParsedThingSxName ptgSxName = ptg as ParsedThingSxName;
				if (ptgSxName == null)
					continue;
				SetupParsedThing(ptgSxName);
			}
			return result.BuildExpressionString(context.WorkbookContext);
		}
		protected abstract void SetupParsedThing(ParsedThingSxName ptgSxName);
	}
	#endregion
	#region XlsPivotCacheCalculatedField
	public class XlsPivotCacheCalculatedField : XlsPivotCacheCalculatedItemBase {
		#region Fields
		List<int> cacheFieldIndexes;
		int sourceCacheFieldIndex;
		#endregion
		public XlsPivotCacheCalculatedField(ParsedExpression expression, int sourceCacheFieldIndex) 
			: base(expression) {
			this.cacheFieldIndexes = new List<int>();
			this.sourceCacheFieldIndex = sourceCacheFieldIndex;
		}
		public List<int> CacheFieldIndexes { get { return cacheFieldIndexes; } }
		public int SourceCacheFieldIndex { get { return sourceCacheFieldIndex; } }
		protected override void SetupParsedThing(ParsedThingSxName ptgSxName) {
			ptgSxName.Index = cacheFieldIndexes[ptgSxName.Index];
		}
	}
	#endregion
	#region XlsPivotCacheCalculatedItem
	public class XlsPivotCacheCalculatedItem : XlsPivotCacheCalculatedItemBase {
		List<ItemPairCollection> itemPairCollections;
		public XlsPivotCacheCalculatedItem(ParsedExpression expression)
			: base(expression) {
			this.itemPairCollections = new List<ItemPairCollection>();
		}
		public List<ItemPairCollection> ItemPairCollections { get { return itemPairCollections; } }
		protected override void SetupParsedThing(ParsedThingSxName ptgSxName) {
			ptgSxName.ItemPairs.AddRange(itemPairCollections[ptgSxName.Index]);
			ptgSxName.Index = -1;
		}
	}
	#endregion
}
