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
using System.Diagnostics;
using System.Collections.Generic;
using System;
namespace DevExpress.XtraSpreadsheet.Model {
	#region SqlDataType(enum)
	public enum SqlDataType {
		SQL_UNKNOWN_TYPE = 0,
		SQL_CHAR = 1,
		SQL_VARCHAR = 2,
		SQL_LONGVARCHAR = -1,
		SQL_WCHAR = -8,
		SQL_WVARCHAR = -9,
		SQL_WLONGVARCHAR = -10,
		SQL_DECIMAL = 3,
		SQL_NUMERIC = 2,
		SQL_SMALLINT = 5,
		SQL_INTEGER = 4,
		SQL_REAL = 7,
		SQL_FLOAT = 6,
		SQL_DOUBLE = 8,
		SQL_BIT = -7,
		SQL_TINYINT = -6,
		SQL_BIGINT = -5,
		SQL_BINARY = -2,
		SQL_VARBINARY = -3,
		SQL_LONGVARBINARY = -4,
		SQL_TYPE_DATE = 9, 
		SQL_TYPE_TIME = 10,
		SQL_TYPE_TIMESTAMP = 11,
		SQL_INTERVAL_MONTH = 102,
		SQL_INTERVAL_YEAR = 101,
		SQL_INTERVAL_YEAR_TO_MONTH = 107,
		SQL_INTERVAL_DAY = 103,
		SQL_INTERVAL_HOUR = 104,
		SQL_INTERVAL_MINUTE = 105,
		SQL_INTERVAL_SECOND = 106,
		SQL_INTERVAL_DAY_TO_HOUR = 108,
		SQL_INTERVAL_DAY_TO_MINUTE = 109,
		SQL_INTERVAL_DAY_TO_SECOND = 110,
		SQL_INTERVAL_HOUR_TO_MINUTE = 111,
		SQL_INTERVAL_HOUR_TO_SECOND = 112,
		SQL_INTERVAL_MINUTE_TO_SECOND = 113,
		SQL_GUID = -11,
		SQL_SIGNED_OFFSET = -20,
		SQL_UNSIGNED_OFFSET = -22,
	}
	#endregion
	#region IPivotCacheField
	public interface IPivotCacheField {
		PivotCacheSharedItemsCollection SharedItems { get; }
		PivotCacheFieldGroup FieldGroup { get; }
		SqlDataType SqlType { get; }
		bool DatabaseField { get; }
		bool ServerField { get; }
		bool UniqueList { get; }
		int Hierarchy { get; }
		int HierarchyLevel { get; }
		string Name { get; }
		bool MemberPropertyField { get; }
		string PropertyName { get; }
		PivotCacheMemberPropertiesMap MemberPropertiesMap { get; }
		int NumberFormatIndex { get; set; }
		string Caption { get; set; }
		string Formula { get; set; }
	}
	#endregion
	#region PivotCacheField
	public class PivotCacheField : IPivotCacheField {
		#region Fields
		readonly DocumentModel documentModel;
		readonly PivotCacheSharedItemsCollection sharedItems;
		readonly PivotCacheFieldGroup fieldGroup;
		readonly PivotCacheMemberPropertiesMap memberPropertiesMap;
		bool databaseField;
		bool memberPropertyField;
		bool serverField;
		bool uniqueList;
		int hierarchy;
		int hierarchyLevel;
		int numberFormatIndex;
		bool applyNumberFormat = true;
		string formula;
		string name;
		string propertyName;
		string caption;
		SqlDataType sqlType;
		#endregion
		public PivotCacheField(DocumentModel documentModel) {
			Guard.ArgumentNotNull(documentModel, "documentModel");
			this.documentModel = documentModel;
			this.sharedItems = new PivotCacheSharedItemsCollection();
			this.fieldGroup = new PivotCacheFieldGroup(documentModel);
			this.memberPropertiesMap = new PivotCacheMemberPropertiesMap();
			this.uniqueList = true;
			this.databaseField = true;
		}
		public PivotCacheField(DocumentModel documentModel, string name)
			: this(documentModel) {
			this.name = name;
		}
		#region Properties
		public PivotCacheSharedItemsCollection SharedItems { get { return sharedItems; } }
		public PivotCacheFieldGroup FieldGroup { get { return fieldGroup; } }
		public PivotCacheMemberPropertiesMap MemberPropertiesMap { get { return memberPropertiesMap; } }
		public bool DatabaseField { get { return databaseField; } set { databaseField = value; } }
		public bool MemberPropertyField { get { return memberPropertyField; } set { memberPropertyField = value; } }
		public bool ServerField { get { return serverField; } set { serverField = value; } }
		public bool UniqueList { get { return uniqueList; } set { uniqueList = value; } }
		public int Hierarchy { get { return hierarchy; } set { hierarchy = value; } }
		public int HierarchyLevel { get { return hierarchyLevel; } set { hierarchyLevel = value; } }
		public string Name { get { return name; } set { name = value; } }
		public string PropertyName { get { return propertyName; } set { propertyName = value; } }
		public SqlDataType SqlType { get { return sqlType; } set { sqlType = value; } }
		#region NumberFormat
		public int NumberFormatIndex { get { return numberFormatIndex; } set { HistoryHelper.SetValue(documentModel, numberFormatIndex, value, SetNumberFormatIndexCore); } }
		protected internal void SetNumberFormatIndexCore(int value) {
			this.numberFormatIndex = value;
		}
		string FormatCode { get { return documentModel.Cache.NumberFormatCache[numberFormatIndex].FormatCode; } }
		#endregion
		#region Caption
		public string Caption { get { return caption; } set { HistoryHelper.SetValue(documentModel, caption, value, SetCaptionCore); } }
		protected internal void SetCaptionCore(string value) {
			this.caption = value;
		}
		#endregion
		#region Formula
		public string Formula { get { return formula; } set { HistoryHelper.SetValue(documentModel, formula, value, SetFormulaCore); } }
		protected internal void SetFormulaCore(string value) {
			this.formula = value;
		}
		#endregion
		#endregion
		public void CopyFrom(PivotCacheField source) {
			this.sharedItems.CopyFrom(source.sharedItems);
			this.fieldGroup.CopyFrom(source.fieldGroup);
			Debug.Assert(this.memberPropertiesMap.Count == 0);
			this.memberPropertiesMap.AddRange(source.memberPropertiesMap);
			this.databaseField = source.databaseField;
			this.memberPropertyField = source.memberPropertyField;
			this.serverField = source.serverField;
			this.uniqueList = source.uniqueList;
			this.hierarchy = source.hierarchy;
			this.hierarchyLevel = source.hierarchyLevel;
			this.numberFormatIndex = NumberFormatHelper.GetNumberFormatIndex(source.FormatCode, documentModel);
			this.applyNumberFormat = source.applyNumberFormat;
			this.formula = source.formula;
			this.name = source.name;
			this.propertyName = source.propertyName;
			this.caption = source.caption;
			this.sqlType = source.sqlType;
		}
	}
	#endregion
	#region PivotCacheFieldsCollection
	public class PivotCacheFieldsCollection : UndoableCollection<IPivotCacheField> {
		public PivotCacheFieldsCollection(IDocumentModel documentModel)
			: base(documentModel.MainPart) {
		}
		public int GetDatabaseFieldCount() {
			int count = 0;
			foreach (IPivotCacheField field in this)
				if (field.DatabaseField)
					count++;
			return count;
		}
		public int GetSharedItemIndex(IPivotCacheRecord record, int fieldIndex) {
			IPivotCacheField cacheField = this[fieldIndex];
			if (cacheField.FieldGroup.DiscreteGroupingProperties.Count > 0)
				fieldIndex = cacheField.FieldGroup.FieldBase;
			PivotCacheRecordSharedItemsIndexValue sharedItemValue = (PivotCacheRecordSharedItemsIndexValue)record[fieldIndex];
			if (cacheField.FieldGroup.DiscreteGroupingProperties.Count > 0)
				sharedItemValue = (PivotCacheRecordSharedItemsIndexValue)cacheField.FieldGroup.DiscreteGroupingProperties[sharedItemValue.IndexValue];
			int sharedItemIndex = sharedItemValue.IndexValue;
			PivotCacheRangeGroupingProperties rangeGroup = cacheField.FieldGroup.RangeGroupingProperties;
			if (rangeGroup.HasGroup)
				sharedItemIndex = GetSharedItemIndexFromRangeGroup(rangeGroup, sharedItemValue.ToVariantValue(this[fieldIndex], ((DocumentModel)DocumentModel).DataContext), cacheField.FieldGroup.GroupItems.Count);
			return sharedItemIndex;
		}
		int GetSharedItemIndexFromRangeGroup(PivotCacheRangeGroupingProperties rangeGroup, VariantValue value, int groupItemsCount) {
			Debug.Assert(value.IsNumeric);
			if (rangeGroup.GroupBy == PivotCacheGroupValuesBy.NumericRanges) {
				double number = value.NumericValue;
				if (number < rangeGroup.StartNum)
					return 0;
				if (number > rangeGroup.EndNum)
					return groupItemsCount - 1;
				number = (number - rangeGroup.StartNum) / rangeGroup.GroupInterval + 1;
				return Math.Min((int)number, groupItemsCount - 2);
			}
			else {
				DateTime dateTime = ((DocumentModel)this.DocumentModel).DataContext.FromDateTimeSerial(value.NumericValue);
				if (dateTime < rangeGroup.StartDate)
					return 0;
				if (dateTime > rangeGroup.EndDate)
					return groupItemsCount - 1;
				switch (rangeGroup.GroupBy) {
					case PivotCacheGroupValuesBy.Years:
						return dateTime.Year + 1;
					case PivotCacheGroupValuesBy.Quarters:
						return (dateTime.Month - 1) / 3 + 1;
					case PivotCacheGroupValuesBy.Months:
						return dateTime.Month + 1;
					case PivotCacheGroupValuesBy.Days:
						return dateTime.Day + 1;
					case PivotCacheGroupValuesBy.Hours:
						return dateTime.Hour + 1;
					case PivotCacheGroupValuesBy.Minutes:
						return dateTime.Minute + 1;
					case PivotCacheGroupValuesBy.Seconds:
						return dateTime.Second + 1;
					default:
						return 0;
				}
			}
		}
		public IPivotCacheRecordValue GetValue(IPivotCacheRecord record, int fieldIndex) {
			int sharedItemIndex = GetSharedItemIndex(record, fieldIndex);
			return GetValue(sharedItemIndex, fieldIndex);
		}
		public IPivotCacheRecordValue GetValue(int sharedItemIndex, int fieldIndex) {
			IPivotCacheField cacheField = this[fieldIndex];
			if (cacheField.FieldGroup.GroupItems.Count > 0)
				return cacheField.FieldGroup.GroupItems[sharedItemIndex];
			return cacheField.SharedItems[sharedItemIndex];
		}
	}
	#endregion
	#region PivotCacheMemberPropertiesMap
	public class PivotCacheMemberPropertiesMap : List<int> {
	}
	#endregion
}
