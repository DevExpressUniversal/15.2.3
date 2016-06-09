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
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using DevExpress.Compatibility.System.Drawing;
namespace DevExpress.XtraSpreadsheet.Import.OpenXml {
	#region PivotCacheValueBaseCollectionDestionation (base class)
	public abstract class PivotCacheValueBaseCollectionDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("m", OnNoValueRecord);
			result.Add("n", OnNumberRecord);
			result.Add("b", OnBooleanRecord);
			result.Add("e", OnErrorValueRecord);
			result.Add("s", OnStringRecord);
			result.Add("d", OnDateTimeRecord);
			result.Add("x", OnSharedItemsIndexRecord);
			return result;
		}
		static Destination OnNoValueRecord(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheRecordEmptyValueDestination(importer, GetThis(importer));
		}
		static Destination OnNumberRecord(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheRecordNumericValueDestination(importer, GetThis(importer));
		}
		static Destination OnBooleanRecord(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheRecordBooleanValueDestination(importer, GetThis(importer));
		}
		static Destination OnErrorValueRecord(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheRecordErrorValueDestination(importer, GetThis(importer));
		}
		static Destination OnStringRecord(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheRecordCharacterValueDestination(importer, GetThis(importer));
		}
		static Destination OnDateTimeRecord(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheRecordDateTimeValueDestination(importer, GetThis(importer));
		}
		static Destination OnSharedItemsIndexRecord(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheRecordSharedItemsIndexValueDestination(importer, GetThis(importer));
		}
		static PivotCacheValueBaseCollectionDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheValueBaseCollectionDestination)importer.PeekDestination();
		}
		#endregion
		protected PivotCacheValueBaseCollectionDestination(SpreadsheetMLBaseImporter importer)
			: base(importer) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public abstract void AddItem(IPivotCacheRecordValue value);
	}
	#endregion
	#region PivotCacheRecordsDestination
	public class PivotCacheRecordsDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("r", OnRecord);
			return result;
		}
		static Destination OnRecord(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			return new PivotCacheRecordDestination(importer, GetThis(importer).cache);
		}
		static PivotCacheRecordsDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheRecordsDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotCache cache;
		public PivotCacheRecordsDestination(SpreadsheetMLBaseImporter importer, PivotCache cache)
			: base(importer) {
			Guard.ArgumentNotNull(cache, "cache");
			this.cache = cache;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		protected internal PivotCache Cache { get { return cache; } }
	}
	#endregion
	#region PivotCacheRecordDestination
	public class PivotCacheRecordDestination : PivotCacheValueBaseCollectionDestination {
		readonly PivotCache cache;
		readonly List<IPivotCacheRecordValue> values;
		public PivotCacheRecordDestination(SpreadsheetMLBaseImporter importer, PivotCache cache)
			: base(importer) {
			this.cache = cache;
			values = new List<IPivotCacheRecordValue>(cache.CacheFields.Count);
		}
		public override void ProcessElementClose(XmlReader reader) {
			if (values.Count < 1)
				return;
			cache.Records.Add(new PivotCacheRecord(values.ToArray()));
		}
		public override void AddItem(IPivotCacheRecordValue value) {
			value = value.ToSharedItem(cache.CacheFields[values.Count]);
			values.Add(value);
		}
	}
	#endregion
	#region PivotCacheRecordValueDestinationBase (absract class)
	public abstract class PivotCacheRecordValueDestinationBase<T, K> : ElementDestination<SpreadsheetMLBaseImporter>
		where T : PivotCacheRecordOrdinalValueBase
		where K : PivotCacheRecordValueBase {
		#region Handler Table
		protected static void AddBaseHandlers(ElementHandlerTable<SpreadsheetMLBaseImporter> handlers) {
			handlers.Add("x", OnMemberPropertyIndex);
		}
		static Destination OnMemberPropertyIndex(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotCacheRecordValueDestinationBase<T, K> thisDestination = GetThis(importer);
			if (thisDestination.memberPropertyIndices == null)
				thisDestination.memberPropertyIndices = new List<int>();
			return new PivotCacheRecordMemberPropertyIndexDestination(importer, thisDestination.memberPropertyIndices);
		}
		static PivotCacheRecordValueDestinationBase<T, K> GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheRecordValueDestinationBase<T, K>)importer.PeekDestination();
		}
		#endregion
		readonly PivotCacheValueBaseCollectionDestination collectionDestination;
		string caption;
		bool isUnusedItem;
		bool isCalculatedItem;
		List<int> memberPropertyIndices;
		protected PivotCacheRecordValueDestinationBase(SpreadsheetMLBaseImporter importer, PivotCacheValueBaseCollectionDestination collectionDestination)
			: base(importer) {
			this.collectionDestination = collectionDestination;
		}
		protected PivotCacheValueBaseCollectionDestination CollectionDestination { get { return collectionDestination; } }
		public override void ProcessElementOpen(XmlReader reader) {
			caption = Importer.ReadAttribute(reader, "c");
			isCalculatedItem = Importer.GetWpSTOnOffValue(reader, "f", false);
			isUnusedItem = Importer.GetWpSTOnOffValue(reader, "u", false);
		}
		public override void ProcessElementClose(XmlReader reader) {
			PivotCacheRecordOrdinalValueBase value;
			if (HasAdditionalProperties()) {
				K valueWithAdditional = CreateValue();
				SetAdditionalProperties(valueWithAdditional);
				value = valueWithAdditional;
			}
			else
				value = CreateOrdinalValue();
			value.IsUnusedItem = isUnusedItem;
			value.IsCalculatedItem = isCalculatedItem;
			collectionDestination.AddItem(value);
		}
		protected virtual void SetAdditionalProperties(K value) {
			value.Caption = caption;
			value.MemberPropertyIndexes = memberPropertyIndices;
		}
		protected virtual bool HasAdditionalProperties() {
			return !string.IsNullOrEmpty(caption) || memberPropertyIndices != null;
		}
		protected abstract K CreateValue();
		protected abstract T CreateOrdinalValue();
	}
	#endregion
	#region PivotCacheRecordBooleanValueDestination
	public class PivotCacheRecordBooleanValueDestination : PivotCacheRecordValueDestinationBase<PivotCacheRecordOrdinalBooleanValue, PivotCacheRecordBooleanValue> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddBaseHandlers(result);
			return result;
		}
		#endregion
		bool value;
		public PivotCacheRecordBooleanValueDestination(SpreadsheetMLBaseImporter importer, PivotCacheValueBaseCollectionDestination collectionDestination)
			: base(importer, collectionDestination) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			bool? valueItem = Importer.GetWpSTOnOffNullValue(reader, "v");
			if (!valueItem.HasValue)
				Importer.ThrowInvalidFile();
			value = valueItem.Value;
		}
		protected override PivotCacheRecordOrdinalBooleanValue CreateOrdinalValue() {
			return new PivotCacheRecordOrdinalBooleanValue(value);
		}
		protected override PivotCacheRecordBooleanValue CreateValue() {
			return new PivotCacheRecordBooleanValue(value);
		}
	}
	#endregion
	#region PivotCacheRecordDateTimeValueDestination
	public class PivotCacheRecordDateTimeValueDestination : PivotCacheRecordValueDestinationBase<PivotCacheRecordOrdinalDateTimeValue, PivotCacheRecordDateTimeValue> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddBaseHandlers(result);
			return result;
		}
		#endregion
		DateTime value;
		public PivotCacheRecordDateTimeValueDestination(SpreadsheetMLBaseImporter importer, PivotCacheValueBaseCollectionDestination collectionDestination)
			: base(importer, collectionDestination) {
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			DateTime? dateTime = Importer.ReadDateTime(reader, "v");
			if (!dateTime.HasValue)
				Importer.ThrowInvalidFile();
			value = dateTime.Value;
		}
		protected override PivotCacheRecordOrdinalDateTimeValue CreateOrdinalValue() {
			return new PivotCacheRecordOrdinalDateTimeValue(value);
		}
		protected override PivotCacheRecordDateTimeValue CreateValue() {
			return new PivotCacheRecordDateTimeValue(value);
		}
	}
	#endregion
	#region PivotCacheRecordFormattedValueDestinationBase (absract class)
	public abstract class PivotCacheRecordFormattedValueDestinationBase<T, K> : PivotCacheRecordValueDestinationBase<T, K>
		where T : PivotCacheRecordOrdinalValueBase
		where K : PivotCacheRecordFormattedValue {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			AddBaseHandlers(result);
			result.Add("tpls", OnTuples);
			return result;
		}
		static Destination OnTuples(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTupleCollection tuples = new PivotTupleCollection();
			GetThis(importer).tuples.Add(tuples);
			return new PivotTuplesDestination(importer, tuples);
		}
		static PivotCacheRecordFormattedValueDestinationBase<T, K> GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotCacheRecordFormattedValueDestinationBase<T, K>)importer.PeekDestination();
		}
		#endregion
		protected PivotCacheRecordFormattedValueDestinationBase(SpreadsheetMLBaseImporter importer, PivotCacheValueBaseCollectionDestination collectionDestination)
			: base(importer, collectionDestination) {
		}
		bool bold;
		bool italic;
		bool underline;
		bool strikethrough;
		Color backgroundColor;
		Color foregroundColor;
		int? formatIndex = -1;
		List<PivotTupleCollection> tuples = new List<PivotTupleCollection>();
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			bold = Importer.GetOnOffValue(reader, "b", false);
			italic = Importer.GetOnOffValue(reader, "i", false);
			underline = Importer.GetOnOffValue(reader, "un", false);
			strikethrough = Importer.GetOnOffValue(reader, "st", false);
			backgroundColor = Importer.GetWpSTColorValue(reader, "bc", DXColor.Empty);
			foregroundColor = Importer.GetWpSTColorValue(reader, "fc", DXColor.Empty);
			formatIndex = Importer.GetIntegerNullableValue(reader, "in");
		}
		protected override void SetAdditionalProperties(K value) {
			base.SetAdditionalProperties(value);
			value.Bold = bold;
			value.Italic = italic;
			value.Underline = underline;
			value.Strikethrough = strikethrough;
			value.BackgroundColor = backgroundColor;
			value.ForegroundColor = foregroundColor;
			value.FormatIndex = formatIndex;
			if(tuples.Count > 0)
				value.Tuples = tuples;
		}
		protected override bool HasAdditionalProperties() {
			if (base.HasAdditionalProperties())
				return true;
			return bold || italic || underline || strikethrough || !backgroundColor.IsEmpty || !foregroundColor.IsEmpty || formatIndex.HasValue || tuples.Count > 0;
		}
	}
	#endregion
	#region PivotCacheRecordEmptyValueDestination
	public class PivotCacheRecordEmptyValueDestination : PivotCacheRecordFormattedValueDestinationBase<PivotCacheRecordOrdinalEmptyValue, PivotCacheRecordEmptyValue> {
		public PivotCacheRecordEmptyValueDestination(SpreadsheetMLBaseImporter importer, PivotCacheValueBaseCollectionDestination collectionDestination)
			: base(importer, collectionDestination) {
		}
		protected override PivotCacheRecordEmptyValue CreateValue() {
			return new PivotCacheRecordEmptyValue();
		}
		protected override PivotCacheRecordOrdinalEmptyValue CreateOrdinalValue() {
			return PivotCacheRecordOrdinalEmptyValue.Instance;
		}
	}
	#endregion
	#region PivotCacheRecordNumericValueDestination
	public class PivotCacheRecordNumericValueDestination : PivotCacheRecordFormattedValueDestinationBase<PivotCacheRecordOrdinalNumericValue, PivotCacheRecordNumericValue> {
		double value;
		public PivotCacheRecordNumericValueDestination(SpreadsheetMLBaseImporter importer, PivotCacheValueBaseCollectionDestination collectionDestination)
			: base(importer, collectionDestination) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			double? nullableValue = Importer.GetDoubleNullableValue(reader, "v");
			if (!nullableValue.HasValue)
				Importer.ThrowInvalidFile();
			value = nullableValue.Value;
		}
		protected override PivotCacheRecordNumericValue CreateValue() {
			return new PivotCacheRecordNumericValue(value);
		}
		protected override PivotCacheRecordOrdinalNumericValue CreateOrdinalValue() {
			return new PivotCacheRecordOrdinalNumericValue(value);
		}
	}
	#endregion
	#region PivotCacheRecordErrorValueDestination
	public class PivotCacheRecordErrorValueDestination : PivotCacheRecordFormattedValueDestinationBase<PivotCacheRecordOrdinalErrorValue, PivotCacheRecordErrorValue> {
		ICellError cellError;
		public PivotCacheRecordErrorValueDestination(SpreadsheetMLBaseImporter importer, PivotCacheValueBaseCollectionDestination collectionDestination)
			: base(importer, collectionDestination) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			string value = Importer.ReadAttribute(reader, "v");
			if (String.IsNullOrEmpty(value))
				Importer.ThrowInvalidFile();
			cellError = CellErrorFactory.CreateError(value, Importer.DocumentModel.DataContext);
			if (cellError == null)
				Importer.ThrowInvalidFile();
		}
		protected override PivotCacheRecordErrorValue CreateValue() {
			return new PivotCacheRecordErrorValue(cellError);
		}
		protected override PivotCacheRecordOrdinalErrorValue CreateOrdinalValue() {
			return new PivotCacheRecordOrdinalErrorValue(cellError);
		}
	}
	#endregion
	#region PivotCacheRecordCharacterValueDestination
	public class PivotCacheRecordCharacterValueDestination : PivotCacheRecordFormattedValueDestinationBase<PivotCacheRecordOrdinalCharacterValue, PivotCacheRecordCharacterValue> {
		string value;
		public PivotCacheRecordCharacterValueDestination(SpreadsheetMLBaseImporter importer, PivotCacheValueBaseCollectionDestination collectionDestination)
			: base(importer, collectionDestination) {
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			value = Importer.ReadAttribute(reader, "v");
			if (value == null)
				Importer.ThrowInvalidFile();
			value = Importer.DecodeXmlChars(value);
		}
		protected override PivotCacheRecordCharacterValue CreateValue() {
			return new PivotCacheRecordCharacterValue(value);
		}
		protected override PivotCacheRecordOrdinalCharacterValue CreateOrdinalValue() {
			return new PivotCacheRecordOrdinalCharacterValue(value);
		}
	}
	#endregion
	#region PivotCacheRecordSharedItemsIndexValueDestination
	public class PivotCacheRecordSharedItemsIndexValueDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotCacheValueBaseCollectionDestination collectionDestination;
		PivotCacheRecordSharedItemsIndexValue value;
		public PivotCacheRecordSharedItemsIndexValueDestination(SpreadsheetMLBaseImporter importer, PivotCacheValueBaseCollectionDestination collectionDestination)
			: base(importer) {
			this.collectionDestination = collectionDestination;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			int index = Importer.GetIntegerValue(reader, "v", int.MinValue);
			if (index == int.MinValue)
				Importer.ThrowInvalidFile();
			this.value = new PivotCacheRecordSharedItemsIndexValue(index);
		}
		public override void ProcessElementClose(XmlReader reader) {
			collectionDestination.AddItem(this.value);
		}
	}
	#endregion
	#region PivotCacheRecordMemberPropertyIndexDestination
	public class PivotCacheRecordMemberPropertyIndexDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly List<int> memberPropertyIndices;
		public PivotCacheRecordMemberPropertyIndexDestination(SpreadsheetMLBaseImporter importer, List<int> memberPropertyIndices)
			: base(importer) {
				Guard.ArgumentNotNull(memberPropertyIndices, "PivotCacheRecordValueBase");
			this.memberPropertyIndices = memberPropertyIndices;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			memberPropertyIndices.Add(Importer.GetIntegerValue(reader, "v", 0));
		}
	}
	#endregion
	#region PivotTuplesDestination
	public class PivotTuplesDestination : ElementDestination<SpreadsheetMLBaseImporter> {
		#region Handler Table
		static readonly ElementHandlerTable<SpreadsheetMLBaseImporter> handlerTable = CreateElementHandlerTable();
		static ElementHandlerTable<SpreadsheetMLBaseImporter> CreateElementHandlerTable() {
			ElementHandlerTable<SpreadsheetMLBaseImporter> result = new ElementHandlerTable<SpreadsheetMLBaseImporter>();
			result.Add("tpl", OnTuple);
			return result;
		}
		static Destination OnTuple(SpreadsheetMLBaseImporter importer, XmlReader reader) {
			PivotTuple tuple = new PivotTuple();
			GetThis(importer).tuples.Add(tuple);
			return new PivotTupleDestination(importer, tuple);
		}
		static PivotTuplesDestination GetThis(SpreadsheetMLBaseImporter importer) {
			return (PivotTuplesDestination)importer.PeekDestination();
		}
		#endregion
		readonly PivotTupleCollection tuples;
		public PivotTuplesDestination(SpreadsheetMLBaseImporter importer, PivotTupleCollection tuples)
			: base(importer) {
			Guard.ArgumentNotNull(tuples, "tuples");
			this.tuples = tuples;
		}
		protected override ElementHandlerTable<SpreadsheetMLBaseImporter> ElementHandlerTable { get { return handlerTable; } }
		public override void ProcessElementClose(XmlReader reader) {
			base.ProcessElementClose(reader);
			if (tuples.Count == 0)
				Importer.ThrowInvalidFile();
		}
	}
	#endregion
	#region PivotTupleDestination
	public class PivotTupleDestination : LeafElementDestination<SpreadsheetMLBaseImporter> {
		readonly PivotTuple tuple;
		public PivotTupleDestination(SpreadsheetMLBaseImporter importer, PivotTuple tuple)
			: base(importer) {
			Guard.ArgumentNotNull(tuple, "PivotTuple");
			this.tuple = tuple;
		}
		public override void ProcessElementOpen(XmlReader reader) {
			base.ProcessElementOpen(reader);
			int? itemIndex = Importer.GetIntegerNullableValue(reader, "item");
			if (!itemIndex.HasValue)
				Importer.ThrowInvalidFile();
			tuple.ItemIndex = itemIndex.Value;
			tuple.FieldIndex = Importer.GetIntegerValue(reader, "fld", PivotTuple.DefaultIndex);
			tuple.HierarchyIndex = Importer.GetIntegerValue(reader, "hier", PivotTuple.DefaultIndex);
		}
	}
	#endregion
}
