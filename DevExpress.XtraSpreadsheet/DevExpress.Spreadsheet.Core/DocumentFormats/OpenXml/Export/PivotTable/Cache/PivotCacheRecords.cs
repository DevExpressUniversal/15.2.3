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

using DevExpress.Utils.Zip;
using DevExpress.XtraSpreadsheet.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Xml;
using DevExpress.Compatibility.System.Drawing;
using System.Globalization;
namespace DevExpress.XtraSpreadsheet.Export.OpenXml {
	partial class OpenXmlExporter : IPivotCacheRecordValueVisitor {
		#region Export PivotCacheRecords
		protected internal virtual CompressedStream ExportPivotCacheRecordsContent() {
			return CreateXmlContent(GeneratePivotCacheRecordsXmlContent);
		}
		protected internal virtual void GeneratePivotCacheRecordsXmlContent(XmlWriter writer) {
			DocumentContentWriter = writer;
			GeneratePivotCacheRecordsContent();
		}
		protected internal virtual void GeneratePivotCacheRecordsContent() {
			WriteShStartElement("pivotCacheRecords");
			try {
				WriteStringAttr("xmlns", RelsPrefix, null, RelsNamespace);
				WriteIntValue("count", ActivePivotCache.Records.Count);
				foreach (IPivotCacheRecord record in ActivePivotCache.Records)
					GeneratePivotCacheRecordContent(record);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export PivotCacheRecord
		protected internal virtual void GeneratePivotCacheRecordContent(IPivotCacheRecord record) {
			WriteShStartElement("r");
			try {
				GeneratePivotCacheRecordValuesContent(record);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
		#region Export PivotCacheRecordValues
		protected internal virtual void GeneratePivotCacheRecordValuesContent(IEnumerable<IPivotCacheRecordValue> valuesEnumerable) {
			foreach (IPivotCacheRecordValue value in valuesEnumerable)
				value.Visit(this);
		}
		public void Visit(PivotCacheRecordSharedItemsIndexValue value) {
			WriteShStartElement("x");
			try {
				WriteIntValue("v", value.IndexValue);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WritePivotCacheRecordValueBase<T>(PivotCacheRecordValueBase recordValue, string startTag, T value, Action<string, T> writeValue) {
			WriteShStartElement(startTag);
			try {
				writeValue("v", value);
				WritePivotCacheRecordValueBaseAttributes(recordValue);
				WritePivotCacheRecordValueMemberPropertyIndexes(recordValue);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WritePivotCacheRecordOrdinalValueBase<T>(PivotCacheRecordOrdinalValueBase recordValue, string startTag, T value, Action<string, T> writeValue) {
			WriteShStartElement(startTag);
			try {
				writeValue("v", value);
				WritePivotCacheRecordOrdinalValueBasePeroperties(recordValue);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WritePivotCacheRecordValueBaseAttributes(PivotCacheRecordValueBase value) {
			WriteBoolValue("u", value.IsUnusedItem, false);
			WriteBoolValue("f", value.IsCalculatedItem, false);
			WriteStringValue("c", value.Caption, value.HasCaption);
			if (value.HasMemberPropertyIndexes)
				WriteIntValue("cp", value.MemberPropertyIndexes.Count);
		}
		void WritePivotCacheRecordOrdinalValueBasePeroperties(PivotCacheRecordOrdinalValueBase recordValue) {
			WriteBoolValue("u", recordValue.IsUnusedItem, false);
			WriteBoolValue("f", recordValue.IsCalculatedItem, false);
		}
		void WritePivotCacheRecordValueMemberPropertyIndexes(PivotCacheRecordValueBase record) {
			if (!record.HasMemberPropertyIndexes)
				return;
			IList<int> indexes = record.MemberPropertyIndexes;
			int count = indexes.Count;
			for (int i = 0; i < count; i++) {
				WriteShStartElement("x");
				try {
					WriteIntValue("v", indexes[i]);
				}
				finally {
					WriteShEndElement();
				}
			}
		}
		public void Visit(PivotCacheRecordBooleanValue value) {
			WritePivotCacheRecordValueBase(value, "b", value.Value, WriteBoolValue);
		}
		public void Visit(PivotCacheRecordDateTimeValue value) {
			WritePivotCacheRecordValueBase(value, "d", value.Value, WriteDateTime);
		}
		void GeneratePivotCacheRecordFormattedValueContent<T>(PivotCacheRecordFormattedValue recordValue, string startTag, T value, Action<string, T> writeValue) {
			WriteShStartElement(startTag);
			try {
				writeValue("v", value);
				WritePivotCacheRecordFormattedValueCore(recordValue);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WritePivotCacheRecordFormattedValueCore(PivotCacheRecordFormattedValue recordValue) {
			WritePivotCacheRecordValueBaseAttributes(recordValue);
			WritePivotCacheRecordFormattedValueAttributes(recordValue);
			WritePivotCacheRecordValueTuples(recordValue.Tuples);
			WritePivotCacheRecordValueMemberPropertyIndexes(recordValue);
		}
		void WritePivotCacheRecordFormattedValueAttributes(PivotCacheRecordFormattedValue value) {
			int? formatIndex = value.FormatIndex;
			if (formatIndex.HasValue)
				WriteIntValue("in", formatIndex.Value);
			Color backColor = value.BackgroundColor;
			if (!backColor.IsEmpty)
				WriteStringValue("bc", ConvertARGBColorToString(backColor));
			Color foreColor = value.ForegroundColor;
			if (!backColor.IsEmpty)
				WriteStringValue("fc", ConvertARGBColorToString(foreColor));
			WriteBoolValue("i", value.Italic, false);
			WriteBoolValue("un", value.Underline, false);
			WriteBoolValue("st", value.Strikethrough, false);
			WriteBoolValue("b", value.Bold, false);
		}
		public void Visit(PivotCacheRecordEmptyValue value) {
			WriteShStartElement("m");
			try {
				WritePivotCacheRecordFormattedValueAttributes(value);
			}
			finally {
				WriteShEndElement();
			}
		}
		public void Visit(PivotCacheRecordNumericValue value) {
			GeneratePivotCacheRecordFormattedValueContent(value, "n", value.Value.ToString("G17", CultureInfo.InvariantCulture), WriteStringValue);
		}
		public void Visit(PivotCacheRecordErrorValue value) {
			GeneratePivotCacheRecordFormattedValueContent(value, "e", value.Value.Name, WriteStringValue);
		}
		public void Visit(PivotCacheRecordCharacterValue value) {
			GeneratePivotCacheRecordFormattedValueContent(value, "s", EncodeXmlChars(value.Value), WriteStringValue);
		}
		public void Visit(PivotCacheRecordOrdinalBooleanValue value) {
			WritePivotCacheRecordOrdinalValueBase(value, "b", value.Value, WriteBoolValue);
		}
		public void Visit(PivotCacheRecordOrdinalDateTimeValue value) {
			WritePivotCacheRecordOrdinalValueBase(value, "d", value.Value, WriteDateTime);
		}
		public void Visit(PivotCacheRecordOrdinalEmptyValue value) {
			WriteShStartElement("m");
			try {
				WritePivotCacheRecordOrdinalValueBasePeroperties(value);
			}
			finally {
				WriteShEndElement();
			}
		}
		public void Visit(PivotCacheRecordOrdinalNumericValue value) {
			WritePivotCacheRecordOrdinalValueBase(value, "n", value.Value.ToString("G17", CultureInfo.InvariantCulture), WriteStringValue);
		}
		public void Visit(PivotCacheRecordOrdinalErrorValue value) {
			WritePivotCacheRecordOrdinalValueBase(value, "e", value.Value.Name, WriteStringValue);
		}
		public void Visit(PivotCacheRecordOrdinalCharacterValue value) {
			WritePivotCacheRecordOrdinalValueBase(value, "s", EncodeXmlChars(value.Value), WriteStringValue);
		}
		#endregion
		#region Export Tuples
		void WritePivotCacheRecordValueTuples(IList<PivotTupleCollection> tuples) {
			int count = tuples.Count;
			if (count == 0)
				return;
			for (int i = 0; i < count; i++)
				WritePivotTupleCollection(tuples[i]);
		}
		void WritePivotTupleCollection(PivotTupleCollection tupleCollection) {
			WritePivotTupleCollection("tpls", tupleCollection);
		}
		void WritePivotTupleCollection(string tag, PivotTupleCollection tupleCollection) {
			int count = tupleCollection.Count;
			if (count == 0)
				return;
			WriteShStartElement(tag);
			try {
				WriteIntValue("count", count);
				for (int i = 0; i < count; i++)
					WritePivotTuple(tupleCollection[i]);
			}
			finally {
				WriteShEndElement();
			}
		}
		void WritePivotTuple(PivotTuple tuple) {
			WriteShStartElement("tpl");
			try {
				WriteIntValue("fld", tuple.FieldIndex, tuple.HasFieldIndex);
				WriteIntValue("hier", tuple.HierarchyIndex, tuple.HasHierarchyIndex);
				WriteIntValue("item", tuple.ItemIndex);
			}
			finally {
				WriteShEndElement();
			}
		}
		#endregion
	}
}
