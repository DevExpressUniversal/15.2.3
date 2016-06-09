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
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Xml;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region PivotCacheFieldsDestination
	public class PivotCacheFieldsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("cacheField", OnCacheField);
			return result;
		}
		static Destination OnCacheField(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheField cacheField = new PivotCacheField(importer.DocumentModel);
			GetThis(importer).CacheFields.AddCore(cacheField);
			return new PivotCacheFieldDestination(importer, cacheField);
		}
		static PivotCacheFieldsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheFieldsDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheFieldsCollection cacheFields;
		public PivotCacheFieldsDestination(SpreadsheetMLBaseImporter importer, PivotCacheFieldsCollection cacheFields)
			: base(importer) {
			Guard.ArgumentNotNull(cacheFields, "cacheFields");
			this.cacheFields = cacheFields;
		}
		#region Properties
		protected internal PivotCacheFieldsCollection CacheFields { get { return cacheFields; } }
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
	}
	#endregion
	#region PivotCacheFieldDestination
	public class PivotCacheFieldDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("fieldGroup", OnFieldGroup);
			result.Add("mpMap", OnMemberPropertiesMap);
			result.Add("sharedItems", OnSharedItems);
			return result;
		}
		static Destination OnFieldGroup(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheFieldGroupDestination(importer, GetThis(importer).CacheField.FieldGroup);
		}
		static Destination OnMemberPropertiesMap(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheMemberPropertiesMapDestionation(importer, GetThis(importer).CacheField.MemberPropertiesMap);
		}
		static Destination OnSharedItems(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheSharedItemsDestination(importer, GetThis(importer).CacheField.SharedItems);
		}
		static PivotCacheFieldDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheFieldDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheField cacheField;
		public PivotCacheFieldDestination(SpreadsheetMLBaseImporter importer, PivotCacheField cacheField)
			: base(importer) {
			Guard.ArgumentNotNull(cacheField, "cacheField");
			this.cacheField = cacheField;
		}
		#region Properties
		protected internal PivotCacheField CacheField { get { return cacheField; } }
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			CacheField.Name = Importer.GetWpSTXString(reader, "name");
			CacheField.Caption = Importer.GetWpSTXString(reader, "caption");
			CacheField.PropertyName = Importer.GetWpSTXString(reader, "propertyName");
			CacheField.ServerField = Importer.GetOnOffValue(reader, "serverField", false);
			CacheField.UniqueList = Importer.GetOnOffValue(reader, "uniqueList", true);
			int numberFormatIndex = Importer.GetIntegerValue(reader, "numFmtId", -1);
			if (numberFormatIndex >= 0)
				CacheField.SetNumberFormatIndexCore(Importer.StyleSheet.GetNumberFormatIndex(numberFormatIndex));
			CacheField.Formula = Importer.GetWpSTXString(reader, "formula");
			CacheField.SqlType = (SqlDataType)Importer.GetIntegerValue(reader, "sqlType", 0);
			CacheField.Hierarchy = Importer.GetIntegerValue(reader, "hierarchy", 0);
			CacheField.HierarchyLevel = Importer.GetIntegerValue(reader, "level", 0);
			CacheField.DatabaseField = Importer.GetOnOffValue(reader, "databaseField", true);
			CacheField.MemberPropertyField = Importer.GetOnOffValue(reader, "memberPropertyField", false);
		}
	}
	#endregion
	#region PivotCacheSharedItemsDestination
	public class PivotCacheSharedItemsDestination : PivotCacheValueBaseCollectionDestination {
		readonly PivotCacheSharedItemsCollection sharedItems;
		public PivotCacheSharedItemsDestination(SpreadsheetMLBaseImporter importer, PivotCacheSharedItemsCollection sharedItems)
			: base(importer) {
			Guard.ArgumentNotNull(sharedItems, "sharedItems");
			this.sharedItems = sharedItems;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			PivotCacheSharedItemsCollectionFlags flags = PivotCacheSharedItemsCollectionFlags.None;
			if (Importer.GetOnOffValue(reader, "containsSemiMixedTypes", true))
				flags |= PivotCacheSharedItemsCollectionFlags.SemiMixedTypes;
			if (Importer.GetOnOffValue(reader, "containsNonDate", true))
				flags |= PivotCacheSharedItemsCollectionFlags.NonDate;
			if (Importer.GetOnOffValue(reader, "containsMixedTypes", false))
				flags |= PivotCacheSharedItemsCollectionFlags.MixedTypes;
			if (Importer.GetOnOffValue(reader, "containsDate", false))
				flags |= PivotCacheSharedItemsCollectionFlags.Date;
			if (Importer.GetOnOffValue(reader, "containsString", true))
				flags |= PivotCacheSharedItemsCollectionFlags.String;
			if (Importer.GetOnOffValue(reader, "containsBlank", false))
				flags |= PivotCacheSharedItemsCollectionFlags.Blank;
			if (Importer.GetOnOffValue(reader, "containsNumber", false))
				flags |= PivotCacheSharedItemsCollectionFlags.Number;
			if (Importer.GetOnOffValue(reader, "containsInteger", false))
				flags |= PivotCacheSharedItemsCollectionFlags.Integer;
			if (Importer.GetOnOffValue(reader, "longText", false))
				flags |= PivotCacheSharedItemsCollectionFlags.LongText;
			double minValue = Importer.GetWpDoubleValue(reader, "minValue", double.MaxValue);
			double maxValue = Importer.GetWpDoubleValue(reader, "maxValue", double.MinValue);
			DateTime minDate = Importer.ReadDateTime(reader, "minDate", DateTime.MaxValue);
			DateTime maxDate = Importer.ReadDateTime(reader, "maxDate", DateTime.MinValue);
			sharedItems.Initialize(flags, minValue, maxValue, minDate, maxDate);
		}
		public override void AddItem(IPivotCacheRecordValue value) {
			sharedItems.Add(value);
		}
	}
	#endregion
	#region PivotCacheFieldGroupDestination
	public class PivotCacheFieldGroupDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("rangePr", OnRangeProperties);
			result.Add("discretePr", OnDiscreteGroupingProperties);
			result.Add("groupItems", OnGroupItems);
			return result;
		}
		static Destination OnRangeProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheRangeGroupingPropertiesDestionation(importer, GetThis(importer).FieldGroup.RangeGroupingProperties);
		}
		static Destination OnDiscreteGroupingProperties(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheDiscreteGroupingPropertiesDestination(importer, GetThis(importer).FieldGroup.DiscreteGroupingProperties);
		}
		static Destination OnGroupItems(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheGroupItemsDestination(importer, GetThis(importer).FieldGroup.GroupItems);
		}
		static PivotCacheFieldGroupDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheFieldGroupDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheFieldGroup fieldGroup;
		public PivotCacheFieldGroupDestination(SpreadsheetMLBaseImporter importer, PivotCacheFieldGroup fieldGroup)
			: base(importer) {
			Guard.ArgumentNotNull(fieldGroup, "sharedItems");
			this.fieldGroup = fieldGroup;
		}
		#region Properties
		protected internal PivotCacheFieldGroup FieldGroup { get { return fieldGroup; } }
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		#endregion
		public override void ProcessElementOpen(XmlReader reader) {
			FieldGroup.Parent = Importer.GetIntegerValue(reader, "par", -1);
			FieldGroup.FieldBase = Importer.GetIntegerValue(reader, "base", -1);
		}
	}
	#endregion
	#region PivotCacheRangeGroupingProperties
	public class PivotCacheRangeGroupingPropertiesDestionation : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotCacheRangeGroupingProperties rangeGroupingProperties;
		public PivotCacheRangeGroupingPropertiesDestionation(SpreadsheetMLBaseImporter importer, PivotCacheRangeGroupingProperties rangeGroupingProperties)
			: base(importer) {
			Guard.ArgumentNotNull(rangeGroupingProperties, "rangeGroupingProperties");
			this.rangeGroupingProperties = rangeGroupingProperties;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			rangeGroupingProperties.AutoStart = Importer.GetOnOffValue(reader, "autoStart", true);
			rangeGroupingProperties.AutoEnd = Importer.GetOnOffValue(reader, "autoEnd", true);
			rangeGroupingProperties.AutoEnd = Importer.GetOnOffValue(reader, "autoEnd", true);
			rangeGroupingProperties.GroupBy = Importer.GetWpEnumValue(reader, "groupBy", DevExpress.XtraSpreadsheet.Export.OpenXml.OpenXmlExporter.PivotCacheGroupValuesByTable, PivotCacheGroupValuesBy.NumericRanges);
			rangeGroupingProperties.StartNum = Importer.GetWpDoubleValue(reader, "startNum", double.MaxValue);
			rangeGroupingProperties.EndNum = Importer.GetWpDoubleValue(reader, "endNum", double.MinValue);
			rangeGroupingProperties.StartDate = Importer.ReadDateTime(reader, "startDate", DateTime.MaxValue);
			rangeGroupingProperties.EndDate = Importer.ReadDateTime(reader, "endDate", DateTime.MinValue);
			rangeGroupingProperties.GroupInterval = Importer.GetWpDoubleValue(reader, "groupInterval", 1);
		}
	}
	#endregion
	#region PivotCacheDiscreteGroupingPropertiesDestination
	public class PivotCacheDiscreteGroupingPropertiesDestination : PivotCacheValueBaseCollectionDestination {
		readonly PivotCacheDiscreteGroupingProperties discreteGroupingProperties;
		public PivotCacheDiscreteGroupingPropertiesDestination(SpreadsheetMLBaseImporter importer, PivotCacheDiscreteGroupingProperties discreteGroupingProperties)
			: base(importer) {
			this.discreteGroupingProperties = discreteGroupingProperties;
		}
		public override void AddItem(IPivotCacheRecordValue value) {
			discreteGroupingProperties.AddCore(value);
		}
	}
	#endregion
	#region PivotCacheGroupItemsDestination
	public class PivotCacheGroupItemsDestination : PivotCacheValueBaseCollectionDestination {
		readonly PivotCacheGroupItems groupItems;
		public PivotCacheGroupItemsDestination(SpreadsheetMLBaseImporter importer, PivotCacheGroupItems groupItems)
			: base(importer) {
			this.groupItems = groupItems;
		}
		public override void AddItem(IPivotCacheRecordValue value) {
			groupItems.AddCore(value);
		}
	}
	#endregion
	#region PivotCacheMemberPropertiesMapDestionation
	public class PivotCacheMemberPropertiesMapDestionation : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotCacheMemberPropertiesMap memberPropertiesMaps;
		public PivotCacheMemberPropertiesMapDestionation(SpreadsheetMLBaseImporter importer, PivotCacheMemberPropertiesMap memberPropertiesMaps)
			: base(importer) {
			Guard.ArgumentNotNull(memberPropertiesMaps, "memberPropertiesMaps");
			this.memberPropertiesMaps = memberPropertiesMaps;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int sharedItemsIndex = Importer.GetIntegerValue(reader, "v", 0);
			memberPropertiesMaps.Add(sharedItemsIndex);
		}
	}
	#endregion
}
