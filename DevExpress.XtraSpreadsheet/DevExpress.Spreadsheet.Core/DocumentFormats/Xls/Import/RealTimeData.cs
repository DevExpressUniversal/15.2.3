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
using System.IO;
using System.Text;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsRTDValue
	public enum XlsRTDValueType {
		Double,
		String,
		Boolean,
		Error,
		Integer
	}
	public class XlsRTDValue {
		#region Fields
		XlsRTDValueType valueType = XlsRTDValueType.String;
		double doubleValue;
		string stringValue = string.Empty;
		bool booleanValue;
		int errorValue;
		int integerValue;
		#endregion
		#region Properties
		public XlsRTDValueType ValueType { get { return valueType; } }
		public double DoubleValue {
			get { return doubleValue; }
			set {
				doubleValue = value;
				valueType = XlsRTDValueType.Double;
			}
		}
		public string StringValue {
			get { return stringValue; }
			set {
				if (string.IsNullOrEmpty(value))
					stringValue = string.Empty;
				else
					stringValue = value;
				valueType = XlsRTDValueType.String;
			}
		}
		public bool BooleanValue {
			get { return booleanValue; }
			set {
				booleanValue = value;
				valueType = XlsRTDValueType.Boolean;
			}
		}
		public int ErrorValue {
			get { return errorValue; }
			set {
				errorValue = value;
				valueType = XlsRTDValueType.Error;
			}
		}
		public int IntegerValue {
			get { return integerValue; }
			set {
				integerValue = value;
				valueType = XlsRTDValueType.Integer;
			}
		}
		#endregion
		public static XlsRTDValue FromStream(BinaryReader reader) {
			XlsRTDValue result = new XlsRTDValue();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			int dataType = reader.ReadInt32();
			switch (dataType) {
				case 0x0001:
					DoubleValue = reader.ReadDouble();
					break;
				case 0x0002:
				case 0x1000:
					int cch = reader.ReadInt32();
					StringValue = XLUnicodeStringNoCch.FromStream(reader, cch).Value;
					break;
				case 0x0004:
					BooleanValue = Convert.ToBoolean(reader.ReadInt32());
					break;
				case 0x0010:
					ErrorValue = reader.ReadInt32();
					break;
				case 0x0800:
					IntegerValue = reader.ReadInt32();
					break;
			}
		}
		public void Write(BinaryWriter writer) {
			switch (ValueType) {
				case XlsRTDValueType.Double:
					writer.Write((int)0x0001);
					writer.Write(DoubleValue);
					break;
				case XlsRTDValueType.String:
					int length = StringValue.Length;
					XLUnicodeStringNoCch stringNoCch = new XLUnicodeStringNoCch() { Value = StringValue };
					writer.Write((int)(length < 256 ? 0x0002 : 0x1000));
					writer.Write(length);
					stringNoCch.Write(writer);
					break;
				case XlsRTDValueType.Boolean:
					writer.Write((int)0x0004);
					writer.Write((int)(BooleanValue ? 1 : 0));
					break;
				case XlsRTDValueType.Error:
					writer.Write((int)0x0010);
					writer.Write(ErrorValue);
					break;
				case XlsRTDValueType.Integer:
					writer.Write((int)0x0800);
					writer.Write(IntegerValue);
					break;
			}
		}
		public VariantValue GetVariantValue() {
			switch (ValueType) {
				case XlsRTDValueType.Double:
					return DoubleValue;
				case XlsRTDValueType.Boolean:
					return BooleanValue;
				case XlsRTDValueType.Error:
					return ErrorConverter.ErrorCodeToValue(ErrorValue);
				case XlsRTDValueType.Integer:
					return IntegerValue;
			}
			return StringValue;
		}
	}
	#endregion
	#region XlsRTDCell
	public class XlsRTDCell {
		#region Fields
		int rowIndex;
		int columnIndex;
		int sheetIndex;
		#endregion
		#region Properties
		public int RowIndex {
			get { return rowIndex; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "RowIndex");
				rowIndex = value;
			}
		}
		public int ColumnIndex {
			get { return columnIndex; }
			set {
				ValueChecker.CheckValue(value, 0, byte.MaxValue, "ColumnIndex");
				columnIndex = value;
			}
		}
		public int SheetIndex {
			get { return sheetIndex; }
			set {
				Guard.ArgumentNonNegative(value, "SheetIndex");
				sheetIndex = value;
			}
		}
		#endregion
		public static XlsRTDCell FromStream(BinaryReader reader) {
			XlsRTDCell result = new XlsRTDCell();
			result.Read(reader);
			return result;
		}
		protected void Read(BinaryReader reader) {
			rowIndex = reader.ReadUInt16();
			columnIndex = reader.ReadUInt16();
			sheetIndex = reader.ReadUInt16();
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)rowIndex);
			writer.Write((ushort)columnIndex);
			writer.Write((ushort)sheetIndex);
		}
	}
	#endregion
	#region XlsRealTimeData
	public class XlsRealTimeData {
		#region Fields
		string applicationId = string.Empty;
		string serverName = string.Empty;
		List<string> parameters = new List<string>();
		XlsRTDValue value = new XlsRTDValue();
		List<XlsRTDCell> cells = new List<XlsRTDCell>();
		#endregion
		#region Properties
		public int SamePrefix { get; set; }
		public string ApplicationId {
			get { return applicationId; }
			set {
				if (string.IsNullOrEmpty(value))
					applicationId = string.Empty;
				else
					applicationId = value;
			}
		}
		public string ServerName {
			get { return serverName; }
			set {
				if (string.IsNullOrEmpty(value))
					serverName = string.Empty;
				else
					serverName = value;
			}
		}
		public List<string> Parameters { get { return parameters; } }
		public XlsRTDValue Value { get { return value; } }
		public List<XlsRTDCell> Cells { get { return cells; } }
		#endregion
		public static XlsRealTimeData FromStream(BinaryReader reader, XlsContentBuilder contentBuilder) {
			XlsRealTimeData result = new XlsRealTimeData();
			result.Read(reader, contentBuilder);
			return result;
		}
		protected void Read(BinaryReader reader, XlsContentBuilder contentBuilder) {
			SamePrefix = reader.ReadInt32();
			int charCount = reader.ReadInt32();
			string topic = XLUnicodeStringNoCch.FromStream(reader, charCount).Value;
			if (SamePrefix > 0)
				topic = contentBuilder.PreviousRTDTopic.Substring(0, SamePrefix) + topic;
			contentBuilder.PreviousRTDTopic = topic;
			string[] topicParts = SplitTopic(topic);
			applicationId = topicParts[0];
			serverName = topicParts[1];
			for (int i = 2; i < topicParts.Length; i++)
				parameters.Add(topicParts[i]);
			value = XlsRTDValue.FromStream(reader);
			while (reader.BaseStream.Position < reader.BaseStream.Length) {
				XlsRTDCell cell = XlsRTDCell.FromStream(reader);
				cells.Add(cell);
			}
		}
		public void Write(BinaryWriter writer) {
			writer.Write(SamePrefix);
			string topic = PrepareTopic();
			if (SamePrefix > 0)
				topic = topic.Remove(0, SamePrefix);
			int charCount = topic.Length;
			writer.Write(charCount);
			XLUnicodeStringNoCch stringNoCch = new XLUnicodeStringNoCch() { Value = topic };
			stringNoCch.Write(writer);
			value.Write(writer);
			for (int i = 0; i < cells.Count; i++)
				cells[i].Write(writer);
		}
		string[] SplitTopic(string topic) {
			List<string> result = new List<string>();
			while (!string.IsNullOrEmpty(topic)) {
				int charCount = (int)topic[0];
				if (charCount > 0)
					result.Add(topic.Substring(1, charCount));
				else
					result.Add(string.Empty);
				topic = topic.Remove(0, charCount + 1);
			}
			return result.ToArray();
		}
		string PrepareTopic() {
			StringBuilder sb = new StringBuilder();
			sb.Append((char)applicationId.Length);
			sb.Append(applicationId);
			sb.Append((char)serverName.Length);
			sb.Append(serverName);
			for (int i = 0; i < parameters.Count; i++) {
				sb.Append((char)parameters[i].Length);
				sb.Append(parameters[i]);
			}
			return sb.ToString();
		}
	}
	#endregion
	#region XlsCommandRealTimeData
	public class XlsCommandRealTimeData : XlsCommandRecordBase {
		static short[] typeCodes = new short[] {
			0x0812  
		};
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (contentBuilder.ContentType == XlsContentType.WorkbookGlobals) {
				FutureRecordHeader frtHeader = FutureRecordHeader.FromStream(reader);
				using (XlsCommandStream rtdStream = new XlsCommandStream(reader, typeCodes, Size - frtHeader.GetSize())) {
					using (BinaryReader rtdReader = new BinaryReader(rtdStream)) {
						contentBuilder.RTDTopics.Add(XlsRealTimeData.FromStream(rtdReader, contentBuilder));
					}
				}
			}
			else {
				base.ReadCore(reader, contentBuilder);
			}
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
