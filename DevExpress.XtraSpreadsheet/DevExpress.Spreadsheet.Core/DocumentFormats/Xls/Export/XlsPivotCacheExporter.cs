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
using System.IO;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Import.Xls;
using DevExpress.XtraExport.Xls;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.Office.Services;
using DevExpress.XtraSpreadsheet.Localization;
namespace DevExpress.XtraSpreadsheet.Export.Xls {
	public class XlsPivotCacheExporter : XlsExporterBase, IPivotCacheRecordValueVisitor {
		PivotCache cache;
		int streamId;
		#region Properties
		public PivotCache Cache { get { return cache; } }
		public int StreamId { get { return streamId; } }
		#endregion
		public XlsPivotCacheExporter(BinaryWriter writer, DocumentModel documentModel, ExportXlsStyleSheet exportStyleSheet, PivotCache cache, int streamId)
			: base(writer, documentModel, exportStyleSheet) {
			this.cache = cache;
			this.streamId = streamId;
		}
		public override void WriteContent() {
			WritePivotCacheProperties();
			WritePivotCachePropertiesExt();
			if (Cache.CalculatedItems != null)
				foreach (PivotCacheCalculatedItem item in Cache.CalculatedItems) {
					WritePivotCacheFormula(item); 
				}
			if (Cache.CacheFields != null) {
				WriteFieldProperties();
				for (int recordIndex = 0; recordIndex < Cache.Records.Count; recordIndex++) {
					if (IsActualStream()) {
						IPivotCacheRecord record = Cache.Records[recordIndex];
						WritePivotCasheItemIndexes(record);
						foreach (IPivotCacheRecordValue recordValue in record) 
							if (WriteVisit(recordValue))
								break;
					}
					else
						break;
				}
			}
			if (IsActualStream())
				WriteEndOfSubstream();
		}
		#region ABNF -> PIVOTCACHE -> SXDB
		protected void WritePivotCacheProperties() {
			XlsCommandPivotCacheProperties command = new XlsCommandPivotCacheProperties();
			command.SaveData = Cache.SaveData;
			if (Cache.CreatedVersion >= 3) {
				command.Invalid = true;
				command.EnableRefresh = false;
			}
			else {
				command.Invalid = Cache.Invalid;
				command.EnableRefresh = Cache.EnableRefresh;
			}
			command.RefreshOnLoad = Cache.RefreshOnLoad;
			command.OptimizeCache = Cache.OptimizeMemory;
			command.BackgroundQuery = Cache.BackgroundQuery;
			command.UserName = Cache.RefreshedBy;
			command.StreamId = StreamId;
			if (Cache.Records != null)
				command.NumberOfRecords = Cache.Records.Count;
			if (Cache.CacheFields != null) {
				command.DataCount = Cache.CacheFields.GetDatabaseFieldCount();
				command.TotalCount = Cache.CacheFields.Count;
			}
			command.CacheType = Cache.Source.Type;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTCACHE -> SXDBEx
		protected void WritePivotCachePropertiesExt() {
			XlsCommandPivotCachePropertiesExt command = new XlsCommandPivotCachePropertiesExt();
			VariantValue lastRefresh = new VariantValue();
			lastRefresh.SetDateTime(Cache.RefreshedDate, DateSystem.Date1900);
			command.LastRefreshed = lastRefresh.NumericValue;
			if (Cache.CalculatedItems != null)
				command.FieldIndex = Cache.CalculatedItems.Count;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTCACHE -> SXFORMULA //TODO
		protected void WritePivotCacheFormula(PivotCacheCalculatedItem item) {
		}
		#endregion
		#region ABNF -> PIVOTCACHE -> FDB 
		void WriteFieldProperties() {
			int index = 0;
			List<int> dbbList = new List<int>();
			foreach (PivotCacheField cacheField in Cache.CacheFields) {
				WritePivotCacheFieldProperties(cacheField, cacheField.SharedItems.Count, index);
				WritePivotCacheFieldDataType(cacheField);
				WritePivotCacheGroup(cacheField);
				PivotCacheSharedItemsCollection sharedItems = cacheField.SharedItems;
				if (sharedItems.Count > 0) {
					WritePivotCacheFieldItemsCollection(sharedItems);
					if (IsActualStream())
						dbbList.Add(index);
					else
						break;
				}
				index++;
			}
		}
		#endregion
		#region ABNF -> PIVOTCACHE -> FDB -> SXFDB
		protected void WritePivotCacheFieldProperties(PivotCacheField cacheField, int uniqueCount, int indexField) {
			XlsCommandPivotCacheFieldProperties command = new XlsCommandPivotCacheFieldProperties();
			command.IsServerBased = cacheField.ServerField;
			command.CantGetUniqueItems = !cacheField.UniqueList;
			PivotCacheFieldGroup fieldGroup = cacheField.FieldGroup;
			command.FieldName = cacheField.Name;
			command.AllAtoms = cacheField.SharedItems.Count > 0;
			command.MoreThan255Items = cacheField.SharedItems.Count > 255;
			command.ContainsTextData = cacheField.SharedItems.ContainsString;
			command.IsNumericField = cacheField.SharedItems.ContainsNumber;
			command.HasDates = cacheField.SharedItems.ContainsDate;
			if (fieldGroup.DiscreteGroupingProperties.Count > 0 || fieldGroup.GroupItems.Count > 0 || fieldGroup.Parent != -1) {
				command.RangeGroup = fieldGroup.RangeGroupingProperties.HasGroup;
				if (fieldGroup.Parent == -1)
					command.ParentIndex = 0xFFFF;
				else {
					command.ParentIndex = fieldGroup.Parent;
					command.HasParent = true;
				}
				command.NumberOfItemsInBaseField = fieldGroup.DiscreteGroupingProperties.Count;
				command.NumberOfItemsInThisField = fieldGroup.GroupItems.Count;
				command.HasGroupAtoms = true;
				if (fieldGroup.FieldBase == -1)
					command.BaseIndex = 0xFFFF;
				else
					command.BaseIndex = fieldGroup.FieldBase;
			}
			command.NumberOfCacheItems = cacheField.SharedItems.Count;
			command.UniqueCountItems = uniqueCount;
			command.IsServerBased = cacheField.ServerField;
			command.CantGetUniqueItems = !cacheField.UniqueList;
			command.IsCalculatedField = !command.HasParent && !String.IsNullOrEmpty(cacheField.Formula);
			command.MinMaxValid = command.IsNumericField || command.HasDates;
			if (command.IsCalculatedField) {
				command.HasGroupAtoms = true;
				command.IsNumericField = true; 
			}
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTCACHE -> FDB -> SXFDBType
		protected void WritePivotCacheFieldDataType(PivotCacheField cacheField) {
			XlsCommandPivotCacheFieldDataType command = new XlsCommandPivotCacheFieldDataType();
			command.DataType = cacheField.SqlType;
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTCACHE -> FDB -> [SXFMLA ( TODO ) | (*GRPSXOPER[SXRANGE | *(SxIsxoper *Continue)])] 
		protected void WritePivotCacheGroup(PivotCacheField cacheField) {
			if (!String.IsNullOrEmpty(cacheField.Formula)) {
				WritePivotCacheFmla(cacheField.Formula);
			} 
			else if (cacheField.FieldGroup.GroupItems.Count > 0) {
				foreach (IPivotCacheRecordValue recordValue in cacheField.FieldGroup.GroupItems)
					if (WriteVisit(recordValue))
						break;
				if (cacheField.FieldGroup.DiscreteGroupingProperties.Count == 0)
					WriteRange(cacheField.FieldGroup.RangeGroupingProperties, cacheField.SharedItems.ContainsNumber);
				else
					WriteDiscreteGroupingProperties(cacheField.FieldGroup.DiscreteGroupingProperties);
			}
		}
		#endregion
		#region ABNF -> PIVOTCACHE -> FDB ->  *(SxIsxoper *Continue)
		protected void WriteDiscreteGroupingProperties(PivotCacheDiscreteGroupingProperties discreteGroupProp){
			List<int> items = new List<int>();
			XlsCommandPivotCacheMapping command = new XlsCommandPivotCacheMapping();
			XlsCommandContinue continueCommand = new XlsCommandContinue();
			foreach (IPivotCacheRecordValue recordValue in discreteGroupProp)
				if (recordValue is PivotCacheRecordSharedItemsIndexValue)
					command.Items.Add((int)((PivotCacheRecordSharedItemsIndexValue)recordValue).IndexValue);
			using (XlsChunkWriter writer = new XlsChunkWriter(StreamWriter, command, continueCommand)) {
				int count = command.Items.Count;
				for (int i = 0; i < count; i++) {
					if (writer != null)
						writer.BeginRecord(count << 1);
					writer.Write((short)command.Items[i]);
				}
			}
		}
		#endregion
		#region ABNF -> PIVOTCACHE -> FDB -> SXFMLA ( TODO )
		protected void WritePivotCacheFmla(string formula) {
			StreamWriter.BaseStream.SetLength(0);
		}
		#endregion
		#region ABNF -> PIVOTCACHE -> FDB -> SRCSXOPER(SXOPER)
		protected void WritePivotCacheFieldItemsCollection(PivotCacheSharedItemsCollection sharedItems) {
			foreach (IPivotCacheRecordValue recordValue in sharedItems) {
				if(WriteVisit(recordValue))
					break;
			}
		}
		#endregion
		#region SXRANGE(SxRng ->  3SXNum | 2SXDtr SXInt)
		void WriteRange(PivotCacheRangeGroupingProperties gRange, bool isNum) {
			XlsCommandPivotCacheRangeGroup command = new XlsCommandPivotCacheRangeGroup();
			command.AutoStart = gRange.AutoStart;
			command.AutoStop = gRange.AutoEnd;
			command.GroupValuesBy = gRange.GroupBy;
			command.Write(StreamWriter);
			if (isNum) {
				WriteNumericValue(gRange.StartNum);
				WriteNumericValue(gRange.EndNum);
				WriteNumericValue(gRange.GroupInterval);
			}
			else {
				WriteDateTimeValue(gRange.StartDate);
				WriteDateTimeValue(gRange.EndDate);
				XlsCommandPivotCacheValueInt intCommand = new XlsCommandPivotCacheValueInt();
				intCommand.Value = (short)gRange.GroupInterval;
				intCommand.Write(StreamWriter);
			}
		}
		#endregion
		#region SXOPER(SxNil | SXNum | SxBool | SxErr | SXString | SXDtr)
		void WriteNumericValue(double value) {
			XlsCommandPivotCacheValueNum command = new XlsCommandPivotCacheValueNum();
			command.Value = value;
			command.Write(StreamWriter);
		}
		void WriteStringValue(string value) {
			if (value.Length > 255) {
				StreamWriter.BaseStream.SetLength(0);
				LogServiceHelper.LogMessage(DocumentModel, LogCategory.Warning, XtraSpreadsheetStringId.Msg_PivotCacheStringVeryLong);
			}
			else {
				XlsCommandPivotCacheStringSegment command = new XlsCommandPivotCacheStringSegment();
				command.Value = value;
				command.Write(StreamWriter);
			}
		}
		void WriteBooleanValue(bool value) {
			XlsCommandPivotCacheValueBool command = new XlsCommandPivotCacheValueBool();
			command.Value = value;
			command.Write(StreamWriter);
		}
		void WriteDateTimeValue(DateTime value) {
			XlsCommandPivotCacheValueDateTime command = new XlsCommandPivotCacheValueDateTime();
			command.Value = value;
			command.Write(StreamWriter);
		}
		void WriteErrorValue(VariantValue value) {
			XlsCommandPivotCacheValueErr command = new XlsCommandPivotCacheValueErr();
			command.Value = value;
			command.Write(StreamWriter);
		}
		void WriteEmptyValue() {
			XlsCommandPivotCacheValueNil command = new XlsCommandPivotCacheValueNil();
			command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTCACHE -> DDB -> SXDBB
		protected void WritePivotCasheItemIndexes(IPivotCacheRecord record) {
			XlsCommandPivotCacheItemIndexes command = new XlsCommandPivotCacheItemIndexes();
			bool hasItemIndexes = false;
			int counter = 0;
			foreach (IPivotCacheRecordValue recordValue in record) {
				if (recordValue.ValueType == PivotCacheRecordValueType.SharedItemIndex) {
					PivotCacheRecordSharedItemsIndexValue itemValue = recordValue as PivotCacheRecordSharedItemsIndexValue;
					XlsPivotCacheSharedIndex extCommand = new XlsPivotCacheSharedIndex(itemValue.IndexValue, Cache.CacheFields[counter].SharedItems.Count < 255);
					command.Indexes.Add(extCommand);
					hasItemIndexes = true;
				}
				counter++;
			}
			if (hasItemIndexes)
				command.Write(StreamWriter);
		}
		#endregion
		#region ABNF -> PIVOTCACHE -> EOF
		protected void WriteEndOfSubstream() {
			new XlsCommandEndOfSubstream().Write(StreamWriter);
		}
		#endregion
		bool IsActualStream() {
			return StreamWriter.BaseStream.Length > 0;
		}
		bool WriteVisit(IPivotCacheRecordValue recordValue) {
			if (!IsActualStream())
				return true;	
			recordValue.Visit(this);
			return false;
		}
		public void Visit(PivotCacheRecordSharedItemsIndexValue value) { 
		}
		public void Visit(PivotCacheRecordBooleanValue value) {
			WriteBooleanValue(value.Value);
		}
		public void Visit(PivotCacheRecordDateTimeValue value) {
			WriteDateTimeValue(value.Value);
		}
		public void Visit(PivotCacheRecordEmptyValue value) {
			WriteEmptyValue();
		}
		public void Visit(PivotCacheRecordNumericValue value) {
			WriteNumericValue(value.Value);
		}
		public void Visit(PivotCacheRecordErrorValue value) {
			WriteErrorValue(value.ToVariantValue(null, null));
		}
		public void Visit(PivotCacheRecordCharacterValue value) {
			WriteStringValue(value.Value);
		}
		public void Visit(PivotCacheRecordOrdinalBooleanValue value) {
			WriteBooleanValue(value.Value);
		}
		public void Visit(PivotCacheRecordOrdinalDateTimeValue value) {
			WriteDateTimeValue(value.Value);
		}
		public void Visit(PivotCacheRecordOrdinalEmptyValue value) {
			WriteEmptyValue();
		}
		public void Visit(PivotCacheRecordOrdinalNumericValue value) {
			WriteNumericValue(value.Value);
		}
		public void Visit(PivotCacheRecordOrdinalErrorValue value) {
			WriteErrorValue(value.ToVariantValue(null, null));
		}
		public void Visit(PivotCacheRecordOrdinalCharacterValue value) {
			WriteStringValue(value.Value);
		}
	}
}
