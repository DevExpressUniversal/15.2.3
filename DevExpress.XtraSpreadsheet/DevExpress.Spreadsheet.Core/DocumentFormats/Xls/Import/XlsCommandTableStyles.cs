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
using DevExpress.XtraSpreadsheet.Model;
using System.Collections.Generic;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandTableStyles 
	public class XlsCommandTableStyles : XlsCommandBase {
		#region Fields
		const int fixedPartSize = 20;
		const int predefinedTableStylesCount = 0x90;
		XLUnicodeCharactersArray defaultTableStyleName = new XLUnicodeCharactersArray();
		XLUnicodeCharactersArray defaultPivotTableStyleName = new XLUnicodeCharactersArray();
		#endregion
		#region Properties
		public int CustomTableStylesCount { get; set; }
		public string DefaultTableStyleName { get { return defaultTableStyleName.Value; } set { defaultTableStyleName.Value = value; } }
		public string DefaultPivotTableStyleName { get { return defaultPivotTableStyleName.Value; } set { defaultPivotTableStyleName.Value = value; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			CustomTableStylesCount = reader.ReadInt32() - predefinedTableStylesCount;
			int defaultTableStyleNameLength = reader.ReadInt16();
			int defaultPivotTableStyleNameLength = reader.ReadInt16();
			if (defaultTableStyleNameLength != 0)
				defaultTableStyleName = XLUnicodeCharactersArray.FromStream(reader, defaultTableStyleNameLength);
			if (defaultPivotTableStyleNameLength != 0)
				defaultPivotTableStyleName = XLUnicodeCharactersArray.FromStream(reader, defaultPivotTableStyleNameLength); 
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			contentBuilder.StyleSheet.SetDefaultTableStyleName(DefaultTableStyleName);
			contentBuilder.StyleSheet.SetDefaultPivotStyleName(DefaultPivotTableStyleName);
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandTableStyles));
			header.Write(writer);
			writer.Write(CustomTableStylesCount + predefinedTableStylesCount);
			writer.Write((short)DefaultTableStyleName.Length);
			writer.Write((short)DefaultPivotTableStyleName.Length);
			if (!String.IsNullOrEmpty(DefaultTableStyleName))
				defaultTableStyleName.Write(writer);
			if (!String.IsNullOrEmpty(DefaultPivotTableStyleName)) 
				defaultPivotTableStyleName.Write(writer); 
		}
		protected override short GetSize() {
			return (short)(fixedPartSize + defaultTableStyleName.Length + defaultPivotTableStyleName.Length);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandTableStyles();
		}
	}
	#endregion
	#region XlsCommandTableStyle
	public class XlsCommandTableStyle : XlsCommandBase {
		#region Fields
		const int fixedPartSize = 18;
		LPWideString tableStyleName = new LPWideString();
		int tableStyleElementRecords;
		bool isPivot;
		bool isTable;
		#endregion
		#region Properties
		public bool IsPivot { get { return isPivot; } set { isPivot = value; } }
		public bool IsTable { get { return isTable; } set { isTable = value; } }
		public string TableStyleName { get { return tableStyleName.Value; } set { tableStyleName.Value = value; } }
		public int TableStyleElementRecords { get { return tableStyleElementRecords; } set { tableStyleElementRecords = value; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			ushort bitwiseField = reader.ReadUInt16();
			IsPivot = Convert.ToBoolean(bitwiseField & 0x0002);
			IsTable = Convert.ToBoolean(bitwiseField & 0x0004);
			TableStyleElementRecords = reader.ReadInt32();
			tableStyleName = LPWideString.FromStream(reader);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			contentBuilder.StyleSheet.SetActiveTableStyle(TableStyleName, IsPivot, IsTable, TableStyleElementRecords);
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandTableStyle));
			header.Write(writer);
			ushort bitwiseField = 0;
			if (IsPivot)
				bitwiseField |= 0x0002;
			if (IsTable)
				bitwiseField |= 0x0004;
			writer.Write(bitwiseField); 
			writer.Write(TableStyleElementRecords);
			tableStyleName.Write(writer);
		}
		protected override short GetSize() {
			return (short)(fixedPartSize + tableStyleName.Length);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandTableStyle();
		}
	}
	#endregion
	#region XlsCommandTableStyleElement
	public class XlsCommandTableStyleElement : XlsCommandBase {
		#region Fields
		const short fixedSize = 24;
		int typeIndex;
		int stripeSize = StripeSizeInfo.DefaultValue;
		int dxfId;
		#endregion
		#region Properties
		public int TypeIndex { get { return typeIndex; } set { typeIndex = value; } }
		public int StripeSize { get { return stripeSize; } set { stripeSize = value; } }
		public int DxfId { get { return dxfId; } set { dxfId = value; } }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeader.FromStream(reader);
			TypeIndex = reader.ReadInt32();
			StripeSize = reader.ReadInt32();
			DxfId = reader.ReadInt32();
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
			contentBuilder.StyleSheet.RegisterTableStyleElementFormat(TypeIndex, StripeSize, DxfId);
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeader header = new FutureRecordHeader();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(typeof(XlsCommandTableStyleElement));
			header.Write(writer);
			writer.Write(TypeIndex);
			writer.Write(StripeSize);
			writer.Write(DxfId);
		}
		protected override short GetSize() {
			return fixedSize;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
}
