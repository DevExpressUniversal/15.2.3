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
	#region XlsCommandPivotCacheStreamId
	public class XlsCommandPivotCacheStreamId : XlsCommandShortPropertyValueBase {
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null)
				contentBuilder.CurrentPivotCacheStreamId = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPivotCacheType
	public class XlsCommandPivotCacheType : XlsCommandBase {
		#region Properties
		public PivotCacheType CacheType { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			CacheType = CodeToType(reader.ReadInt16(), contentBuilder);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder != null) {
				XlsPivotCacheBuilderBase builder = CreateCacheBuilder();
				builder.StreamId = contentBuilder.CurrentPivotCacheStreamId;
				contentBuilder.PivotCacheBuilders.Add(builder);
				contentBuilder.CurrentPivotCacheBuilder = builder;
			}
		}
		XlsPivotCacheBuilderBase CreateCacheBuilder() {
			if (CacheType == PivotCacheType.Worksheet)
				return new XlsPivotWorksheetCacheBuilder();
			if (CacheType == PivotCacheType.Scenario)
				return new XlsPivotScenarioCacheBuilder();
			if (CacheType == PivotCacheType.External)
				return new XlsPivotExternalCacheBuilder();
			return new XlsPivotConsolidationCacheBuilder();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(TypeToCode(CacheType));
		}
		protected override short GetSize() {
			return 2;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		#region Internals
		public static PivotCacheType CodeToType(short code, XlsContentBuilder contentBuilder) {
			switch(code) {
				case 0x0001:
					return PivotCacheType.Worksheet;
				case 0x0002:
					return PivotCacheType.External;
				case 0x0004:
					return PivotCacheType.Consolidation;
				case 0x0010:
					return PivotCacheType.Scenario;
				default:
					contentBuilder.ThrowInvalidFile("Unknown PivotCache data source type");
					return PivotCacheType.Worksheet;
			}
		}
		public static short TypeToCode(PivotCacheType cacheType) {
			switch(cacheType) {
				case PivotCacheType.External:
					return 0x0002;
				case PivotCacheType.Consolidation:
					return 0x0004;
				case PivotCacheType.Scenario:
					return 0x0010;
				default:
					return 0x0001; 
			}
		}
		#endregion
	}
	#endregion
	#region XlsCommandPivotCacheMultiRange
	public class XlsCommandPivotCacheMultiRange : XlsCommandBase {
		#region Fields
		int numberOfRanges;
		int numberOfOptionalFields;
		#endregion
		#region Properties
		public int NumberOfRanges {
			get { return numberOfRanges; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "NumberOfRanges");
				numberOfRanges = value;
			}
		}
		public int NumberOfOptionalFields {
			get { return numberOfOptionalFields; }
			set {
				ValueChecker.CheckValue(value, 0, 4, "NumberOfOptionalFields");
				numberOfOptionalFields = value;
			}
		}
		public bool AutoPage { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			NumberOfRanges = reader.ReadUInt16();
			reader.ReadUInt16(); 
			ushort bitwiseField = reader.ReadUInt16();
			NumberOfOptionalFields = bitwiseField & 0x7fff;
			AutoPage = (bitwiseField & 0x8000) != 0;
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null || contentBuilder.CurrentPivotCacheBuilder.CacheType != PivotCacheType.Consolidation)
				return;
			XlsPivotConsolidationCacheBuilder pivotCacheBuilder = (XlsPivotConsolidationCacheBuilder)contentBuilder.CurrentPivotCacheBuilder;
			pivotCacheBuilder.AutoPage = AutoPage;
			pivotCacheBuilder.NumberOfRanges = NumberOfRanges;
			pivotCacheBuilder.NumberOfOptionalFields = NumberOfOptionalFields;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)NumberOfRanges);
			writer.Write((ushort)NumberOfRanges); 
			ushort bitwiseField = (ushort)(NumberOfOptionalFields & 0x7fff);
			if(AutoPage)
				bitwiseField |= 0x8000;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 6;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPivotCacheMultiRangeMap
	public class XlsCommandPivotCacheMultiRangeMap : XlsCommandBase {
		readonly List<int> items = new List<int>();
		#region Properties
		public List<int> Items { get { return items; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			int count = Size / 2;
			for (int i = 0; i < count; i++)
				items.Add(reader.ReadInt16());
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null || contentBuilder.CurrentPivotCacheBuilder.CacheType != PivotCacheType.Consolidation)
				return;
			XlsPivotConsolidationCacheBuilder pivotCacheBuilder = (XlsPivotConsolidationCacheBuilder)contentBuilder.CurrentPivotCacheBuilder;
			XlsPivotConsolidationCacheRangeMap rangeMap = new XlsPivotConsolidationCacheRangeMap();
			rangeMap.AddRange(Items);
			pivotCacheBuilder.RangeMaps.Add(rangeMap);
		}
		protected override void WriteCore(BinaryWriter writer) {
			foreach (int i in items)
				writer.Write((short)i);
		}
		protected override short GetSize() {
			return (short)(items.Count * 2);
		}
		public override IXlsCommand GetInstance() {
			items.Clear();
			return this;
		}
	}
	#endregion
	#region XlsCommandPivotCacheStringItems
	public class XlsCommandPivotCacheStringItems : XlsCommandBase {
		int numberOfItems;
		public int NumberOfItems {
			get { return numberOfItems; }
			set {
				ValueChecker.CheckValue(value, 0, 65534, "NumberOfItems");
				numberOfItems = value;
			}
		}
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			NumberOfItems = reader.ReadUInt16();
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null || contentBuilder.CurrentPivotCacheBuilder.CacheType != PivotCacheType.Consolidation)
				return;
			XlsPivotConsolidationCacheBuilder pivotCacheBuilder = (XlsPivotConsolidationCacheBuilder)contentBuilder.CurrentPivotCacheBuilder;
			pivotCacheBuilder.Pages.Add(new XlsPivotConsolidationCachePage());
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)NumberOfItems);
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandPivotCacheStringSegment
	public class XlsCommandPivotCacheStringSegment : XlsCommandBase {
		XLUnicodeStringNoCch innerValue = new XLUnicodeStringNoCch();
		#region Properties
		public string Value {
			get { return innerValue.Value; }
			set { innerValue.Value = value; }
		}
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			int cch = reader.ReadUInt16();
			if (cch == 0xffff)
				innerValue.Value = string.Empty;
			else
				innerValue = XLUnicodeStringNoCch.FromStream(reader, cch);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null || contentBuilder.CurrentPivotCacheBuilder.CacheType != PivotCacheType.Consolidation)
				return;
			XlsPivotConsolidationCacheBuilder pivotCacheBuilder = (XlsPivotConsolidationCacheBuilder)contentBuilder.CurrentPivotCacheBuilder;
			int pagesCount = pivotCacheBuilder.Pages.Count;
			pivotCacheBuilder.Pages[pagesCount - 1].Add(Value);
			contentBuilder.ParamQueryNext = true;
		}
		protected override void ApplyPivotCacheContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null)
				return;
			contentBuilder.CurrentPivotCacheBuilder.AddRecord(contentBuilder, new PivotCacheRecordCharacterValue(Value));
		}
		protected override void WriteCore(BinaryWriter writer) {
			if (innerValue.Value.Length > 0) {
				writer.Write((ushort)innerValue.Value.Length);
				innerValue.Write(writer);
			}
			else {
				writer.Write((ushort)0);
				writer.Write((byte)0);
			}
		}
		protected override short GetSize() {
			int result = 2;
			if (innerValue.Value.Length > 0)
				result += innerValue.Length;
			else
				result++;
			return (short)result;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsDbQuerySourceType
	public enum XlsDbQuerySourceType {
		ODBC = 0x01,
		DAO = 0x02,
		Web = 0x04,
		OLEDB = 0x05,
		Text = 0x06,
		ADO = 0x07
	}
	#endregion
	#region XlsDbQuery
	public class XlsDbQuery {
		public static XlsDbQuery FromStream(XlsReader reader) {
			XlsDbQuery result = new XlsDbQuery();
			result.Read(reader);
			return result;
		}
		#region Properties
		public XlsDbQuerySourceType SourceType { get; set; }
		public bool Sql { get; set; }
		public bool SqlSav { get; set; }
		public bool Web { get; set; }
		public bool SavePassword { get; set; }
		public bool TablesOnlyHTML { get; set; }
		public short ParamCount { get; set; }
		public short QueryCount { get; set; }
		public short WebCount { get; set; }
		public short SqlSavCount { get; set; }
		public short OdbcCount { get; set; }
		#endregion
		protected void Read(XlsReader reader) {
			ushort bitwiseField = reader.ReadUInt16();
			SourceType = (XlsDbQuerySourceType)(bitwiseField & 0x0007);
			Sql = (bitwiseField & 0x0010) != 0;
			SqlSav = (bitwiseField & 0x0020) != 0;
			Web = (bitwiseField & 0x0040) != 0;
			SavePassword = (bitwiseField & 0x0080) != 0;
			TablesOnlyHTML = (bitwiseField & 0x0100) != 0;
			ParamCount = reader.ReadInt16();
			QueryCount = reader.ReadInt16();
			WebCount = reader.ReadInt16();
			SqlSavCount = reader.ReadInt16();
			OdbcCount = reader.ReadInt16();
		}
		public void Write(BinaryWriter writer) {
			ushort bitwiseField = (ushort)SourceType;
			if (SourceType == XlsDbQuerySourceType.ODBC)
				bitwiseField |= 0x0008;
			if(Sql)
				bitwiseField |= 0x0010;
			if(SqlSav)
				bitwiseField |= 0x0020;
			if(Web)
				bitwiseField |= 0x0040;
			if(SavePassword)
				bitwiseField |= 0x0080;
			if(TablesOnlyHTML)
				bitwiseField |= 0x0100;
			writer.Write(bitwiseField);
			writer.Write(ParamCount);
			writer.Write(QueryCount);
			writer.Write(WebCount);
			writer.Write(SqlSavCount);
			writer.Write(OdbcCount);
		}
		public int GetSize() {
			return 12;
		}
	}
	#endregion
	#region XlsParamQueryParamType
	public enum XlsParamQueryParamType {
		Prompt = 0,
		Value = 1,
		Reference = 2
	}
	#endregion
	#region XlsParamQueryDataType
	public enum XlsParamQueryDataType {
		Double = 0x0001,
		String = 0x0002,
		Boolean = 0x0004,
		Integer = 0x0800
	}
	#endregion
	#region XlsParamQuery
	public class XlsParamQuery {
		XLUnicodeStringNoCch stringValue = new XLUnicodeStringNoCch();
		byte[] formulaBytes;
		ParsedExpression reference = new ParsedExpression();
		public static XlsParamQuery FromStream(XlsReader reader, XlsContentBuilder contentBuilder) {
			XlsParamQuery result = new XlsParamQuery();
			result.Read(reader, contentBuilder);
			return result;
		}
		#region Properties
		public SqlDataType OdbcDataType { get; set; }
		public XlsParamQueryParamType ParamType { get; set; }
		public bool UseDefaultPrompt { get; set; }
		public XlsParamQueryDataType DataType { get; set; }
		public bool BooleanValue { get; set; }
		public string StringValue {
			get { return stringValue.Value; }
			set { stringValue.Value = value; }
		}
		public double NumericValue { get; set; }
		public int IntegerValue { get; set; }
		public ParsedExpression Reference { get { return reference; } }
		#endregion
		public void SetReference(ParsedExpression parsedExpression, IRPNContext context) {
			this.reference = parsedExpression;
			this.formulaBytes = context.ExpressionToBinary(reference);
		}
		protected void Read(XlsReader reader, XlsContentBuilder contentBuilder) {
			OdbcDataType = (SqlDataType)reader.ReadInt16();
			ushort bitwiseField = reader.ReadUInt16();
			ParamType = (XlsParamQueryParamType)(bitwiseField & 0x0003);
			UseDefaultPrompt = (bitwiseField & 0x0008) != 0;
			DataType = (XlsParamQueryDataType)reader.ReadUInt16();
			BooleanValue = reader.ReadUInt16() != 0;
			if (ParamType == XlsParamQueryParamType.Prompt)
				ReadStringValue(reader);
			else if (ParamType == XlsParamQueryParamType.Value) {
				if (DataType == XlsParamQueryDataType.Double)
					NumericValue = reader.ReadDouble();
				else if (DataType == XlsParamQueryDataType.String)
					ReadStringValue(reader);
				else if (DataType == XlsParamQueryDataType.Integer)
					IntegerValue = reader.ReadInt32();
			}
			else {
				int formulaBytesSize = reader.ReadUInt16();
				this.formulaBytes = reader.ReadBytes(formulaBytesSize);
				this.reference = contentBuilder.RPNContext.BinaryToExpression(formulaBytes, formulaBytesSize);
			}
		}
		public void Write(BinaryWriter writer) {
			writer.Write((short)OdbcDataType);
			ushort bitwiseField = (ushort)ParamType;
			if(UseDefaultPrompt)
				bitwiseField |= 0x0008;
			writer.Write(bitwiseField);
			writer.Write((ushort)DataType);
			writer.Write((ushort)(BooleanValue ? 1 : 0));
			if (ParamType == XlsParamQueryParamType.Prompt)
				WriteStringValue(writer);
			else if (ParamType == XlsParamQueryParamType.Value) {
				if (DataType == XlsParamQueryDataType.Double)
					writer.Write(NumericValue);
				else if (DataType == XlsParamQueryDataType.String)
					WriteStringValue(writer);
				else if (DataType == XlsParamQueryDataType.Integer)
					writer.Write(IntegerValue);
			}
			else
				writer.Write(formulaBytes);
		}
		public int GetSize() {
			int result = 8;
			if (ParamType == XlsParamQueryParamType.Prompt)
				result += GetStringValueLength();
			else if (ParamType == XlsParamQueryParamType.Value) {
				if (DataType == XlsParamQueryDataType.Double)
					result += 8;
				else if (DataType == XlsParamQueryDataType.String)
					result += GetStringValueLength();
				else if (DataType == XlsParamQueryDataType.Integer)
					result += 4;
			}
			else
				result += formulaBytes.Length;
			return result;
		}
		void ReadStringValue(XlsReader reader) {
			int cch = reader.ReadUInt16();
			if (cch != 0xffff)
				stringValue = XLUnicodeStringNoCch.FromStream(reader, cch);
			else
				stringValue.Value = string.Empty;
		}
		void WriteStringValue(BinaryWriter writer) {
			int cch = stringValue.Value.Length;
			if (cch > 0) {
				writer.Write((ushort)cch);
				stringValue.Write(writer);
			}
			else
				writer.Write((ushort)0xffff);
		}
		int GetStringValueLength() {
			int cch = stringValue.Value.Length;
			if (cch > 0)
				return 2 + stringValue.Length;
			return 2;
		}
	}
	#endregion
	#region XlsCommandDbOrParamQuery
	public class XlsCommandDbOrParamQuery : XlsCommandBase {
		#region Properties
		public XlsDbQuery DbQuery { get; set; }
		public XlsParamQuery ParamQuery { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			long initialPosition = reader.Position;
			if (contentBuilder.ParamQueryNext) {
				contentBuilder.ParamQueryNext = false;
				DbQuery = null;
				ParamQuery = XlsParamQuery.FromStream(reader, contentBuilder);
			}
			else {
				DbQuery = XlsDbQuery.FromStream(reader);
				ParamQuery = null;
			}
			int bytesToRead = (int)(Size - (reader.Position - initialPosition));
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		protected override void ApplyWorkbookGlobalsContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder == null || contentBuilder.CurrentPivotCacheBuilder.CacheType != PivotCacheType.External)
				return;
			XlsPivotExternalCacheBuilder pivotCacheBuilder = (XlsPivotExternalCacheBuilder)contentBuilder.CurrentPivotCacheBuilder;
			if (DbQuery != null)
				pivotCacheBuilder.DbQueryList.Add(DbQuery);
			else
				pivotCacheBuilder.ParamQueryList.Add(ParamQuery);
		}
		protected override void WriteCore(BinaryWriter writer) {
			if (DbQuery != null)
				DbQuery.Write(writer);
			else
				ParamQuery.Write(writer);
		}
		protected override short GetSize() {
			if (DbQuery != null)
				return (short)DbQuery.GetSize();
			return (short)ParamQuery.GetSize();
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
