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

using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
namespace DevExpress.XtraSpreadsheet.Model {
	#region PivotCacheRefreshCommand
	public class PivotCacheRefreshCommand : PivotCacheErrorHandledCommand {
		public PivotCacheRefreshCommand(IErrorHandler errorHandler, PivotCache pivotCache)
			: base(errorHandler, pivotCache) {
			ShouldClearHistory = true;
		}
		public bool ShouldClearHistory { get; set; }
		protected internal override bool Validate() {
			IPivotCacheSource source = PivotCache.Source;
			IModelErrorInfo errorInfo = source.CheckSourceBeforeRefresh(DataContext);
			if (errorInfo != null && !HandleError(errorInfo))
				return false;
			return true;
		}
		protected internal override void ApplyChanges() {
			if (ShouldClearHistory)
				DocumentModel.ApplyChanges(DocumentModelChangeActions.ClearHistory); 
		}
		protected internal override void ExecuteCore() {
			Stopwatch watch = Stopwatch.StartNew();
			PivotCache oldCacheData = new Model.PivotCache(DocumentModel, PivotCache.Source);
			PivotCache.CopyDataTo(oldCacheData);
			PivotCache.Clear();
			PivotTableCacheRefreshInfo refreshInfo = new PivotTableCacheRefreshInfo();
			using (IPivotCacheRefreshResponse refreshResponse = PivotCache.Source.GetRefreshedData(DataContext)) {
				CreateFields(refreshResponse, refreshInfo);
				CreateCacheRecords(refreshResponse, refreshInfo);
			}
			watch.Stop();
			Debug.WriteLine("The pivot table cache refreshed at " + watch.Elapsed);
			NotifyPivotTables(refreshInfo, oldCacheData);
		}
		void CreateFields(IPivotCacheRefreshResponse refreshResponse, PivotTableCacheRefreshInfo refreshInfo) {
			IPivotCacheFieldNameCreationService service = DocumentModel.GetService<IPivotCacheFieldNameCreationService>();
			if (service == null)
				throw new InvalidOperationException("Name for the field can not be assigned: service is missing.");
			Dictionary<string, IPivotCacheField> existingCacheFields = new Dictionary<string, IPivotCacheField>(StringExtensions.ComparerInvariantCultureIgnoreCase);
			Dictionary<string, int> oldFieldIndices = new Dictionary<string, int>();
			for (int i = 0; i < PivotCache.CacheFields.Count; ++i) {
				IPivotCacheField existingCacheField = PivotCache.CacheFields[i];
				existingCacheFields.Add(existingCacheField.Name, existingCacheField);
				oldFieldIndices.Add(existingCacheField.Name, i);
			}
			PivotCache.CacheFields.Clear();
			List<string> existingNames = new List<string>();
			foreach (IResponseField responceField in refreshResponse.GetFieldsEnumerable()) {
				string fieldName = service.GetNewName(responceField.Name, existingNames);
				IPivotCacheField field = null;
				int fieldIndex = PivotCache.CacheFields.Count;
				int previousIndex = -1;
				if (existingCacheFields.TryGetValue(fieldName, out field))
					previousIndex = oldFieldIndices[fieldName];
				else
					field = new PivotCacheField(DocumentModel, fieldName);
				refreshInfo.NewToOldFieldIndices.Add(fieldIndex, previousIndex);
				PivotCache.CacheFields.Add(field);
				existingNames.Add(fieldName);
			}
		}
		void CreateCacheRecords(IPivotCacheRefreshResponse refreshResponse, PivotTableCacheRefreshInfo refreshInfo) {
			PivotCacheRecordValueCollection[] existingSharedItems = new PivotCacheRecordValueCollection[PivotCache.CacheFields.Count];
			for (int i = 0; i < PivotCache.CacheFields.Count; ++i) {
				IPivotCacheField field = PivotCache.CacheFields[i];
				existingSharedItems[i] = new PivotCacheRecordValueCollection(field.SharedItems);
				field.SharedItems.Clear();
			}
			if (refreshResponse.AllowsMultiThreadedAccess)
				CreateCacheRecordsMultiThreaded(refreshResponse);
			else {
				foreach (IPivotCacheRecord responceRecord in refreshResponse.GetRecordsEnumerable()) {
					IPivotCacheRecord record = CreateRecord(responceRecord, PivotCache.CacheFields);
					PivotCache.Records.Add(record);
				}
			}
			AddPreviousSharedItems(existingSharedItems, refreshInfo);
		}
		void CreateCacheRecordsMultiThreaded(IPivotCacheRefreshResponse refreshResponse) {
			List<PivotTableRefreshDataBucket> dataBuckets = refreshResponse.GetDataBuckets();
			List<PivotTableRefreshDataBucketConverter> converters = new List<PivotTableRefreshDataBucketConverter>();
			Action[] actions = new Action[dataBuckets.Count];
			for (int i = 0; i < dataBuckets.Count; i++) {
				PivotTableRefreshDataBucket bucket = dataBuckets[i];
				PivotTableRefreshDataBucketConverter converter = new PivotTableRefreshDataBucketConverter(bucket, PivotCache.CacheFields);
				converters.Add(converter);
				actions[i] = converter.Process;
			}
#if !DXPORTABLE
			System.Threading.Tasks.Parallel.Invoke(actions);
#else
			foreach (PivotTableRefreshDataBucketConverter converter in converters)
				converter.Process();
#endif
			foreach (PivotTableRefreshDataBucketConverter converter in converters)
				PivotCache.Records.AddRange(converter.Records);
		}
		class PivotTableRefreshDataBucketConverter {
			readonly PivotTableRefreshDataBucket bucket;
			readonly PivotCacheFieldsCollection cacheFields;
			public PivotTableRefreshDataBucketConverter(PivotTableRefreshDataBucket bucket, PivotCacheFieldsCollection cacheFields) {
				this.bucket = bucket;
				this.cacheFields = cacheFields;
				Records = new List<IPivotCacheRecord>();
			}
			public PivotTableRefreshDataBucket Bucket { get { return bucket; } }
			public List<IPivotCacheRecord> Records { get; set; }
			public void Process() {
				Records.AddRange(Bucket.GetRecordsEnumerable(cacheFields));
			}
		}
		public static IPivotCacheRecord CreateRecord(IPivotCacheRecord responceRecord, PivotCacheFieldsCollection cacheFields) {
			for (int i = 0; i < responceRecord.Count; i++) {
				responceRecord[i] = responceRecord[i].ToSharedItem(cacheFields[i]);
			}
			return responceRecord;
		}
		void AddPreviousSharedItems(PivotCacheRecordValueCollection[] existingSharedItems, PivotTableCacheRefreshInfo refreshInfo) {
			for (int i = 0; i < PivotCache.CacheFields.Count; ++i) {
				IPivotCacheField field = PivotCache.CacheFields[i];
				PivotCacheRecordValueCollection previousSharedItems = existingSharedItems[i];
				int[] sharedItemIndexConversionTable = new int[previousSharedItems.Count];
				bool[] alreadyExistingItemTable = new bool[field.SharedItems.Count];
				for (int j = 0; j < previousSharedItems.Count; ++j) {
					IPivotCacheRecordValue value = previousSharedItems[j];
					int currentIndex = field.SharedItems.IndexOf(value);
					if (currentIndex < 0) {
						value.IsUnusedItem = true;
						sharedItemIndexConversionTable[j] = field.SharedItems.Count;
						field.SharedItems.Add(value);
					}
					else {
						sharedItemIndexConversionTable[j] = currentIndex;
						alreadyExistingItemTable[currentIndex] = true;
					}
				}
				refreshInfo.SharedItemIndicesConversionTables.Add(sharedItemIndexConversionTable);
				refreshInfo.AlreadyExistingItemTables.Add(alreadyExistingItemTable);
			}
		}
		void NotifyPivotTables(PivotTableCacheRefreshInfo refreshInfo, PivotCache oldCacheData) {
			List<PivotTable> updatedPivotTables = new List<PivotTable>();
			for (int i = 0; i < DocumentModel.Sheets.Count; ++i) {
				Worksheet sheet = DocumentModel.Sheets[i];
				for (int j = 0; j < sheet.PivotTables.Count; ++j) {
					PivotTable pivotTable = sheet.PivotTables[j];
					updatedPivotTables.Add(pivotTable);
					if (!pivotTable.CalculationInfo.OnPivotCacheRefreshed(PivotCache, ErrorHandler, refreshInfo)) {
						oldCacheData.CopyDataTo(PivotCache);
						for (i = 0; i < updatedPivotTables.Count; ++i) {
							PivotTable undoTable = updatedPivotTables[i];
							undoTable.CalculationInfo.RefreshPivotTableCore(undoTable.CalculationInfo.State & ~PivotTableOutOfDateState.WorksheetDataMask, new PivotTableHistoryTransaction(undoTable));
						}
						return;
					}
				}
			}
		}
	}
	#endregion
	public class PivotTableCacheRefreshInfo {
		readonly Dictionary<int, int> newToOldFieldIndices;
		readonly List<int[]> sharedItemIndicesConversionTables;
		readonly List<bool[]> alreadyExistingItemTables;
		public PivotTableCacheRefreshInfo() {
			newToOldFieldIndices = new Dictionary<int, int>();
			sharedItemIndicesConversionTables = new List<int[]>();
			alreadyExistingItemTables = new List<bool[]>();
		}
		public Dictionary<int, int> NewToOldFieldIndices { get { return newToOldFieldIndices; } }
		public List<int[]> SharedItemIndicesConversionTables { get { return sharedItemIndicesConversionTables; } }
		public List<bool[]> AlreadyExistingItemTables { get { return alreadyExistingItemTables; } }
	}
}
