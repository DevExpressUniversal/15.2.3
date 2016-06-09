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
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsWebBasedDataProviderDataType
	public enum XlsWebBasedDataProviderDataType {
		NotUsed = 0,
		ShortText = 1,
		Number = 2,
		Boolean = 3,
		DateTime = 4,
		Note = 5,
		Currency = 6,
		Lookup = 7,
		Choice = 8,
		UniformResourceLocator = 9,
		Counter = 10,
		MultipleChoices = 11
	}
	#endregion
	#region XlsWebBasedDataProviderInfo // Feature11WSSListInfo structure
	public class XlsWebBasedDataProviderInfo {
		#region Fields
		#region Mask for first packed values
		const uint MaskIsPercent = 0x01;
		const uint MaskIsFixedDecimalPoint = 0x02;
		const uint MaskIsDateOnly = 0x04;
		const uint MaskReadingOrder = 0x18; 
		const uint MaskIsRichText = 0x20;
		const uint MaskIsRecognizedRichTextFormatting = 0x40;
		const uint MaskIsAlertRecognizedRichTextFormatting = 0x80;
		#endregion
		#region Mask for second packed values
		const uint MaskIsReadOnly = 0x01;
		const uint MaskIsRequired = 0x02;
		const uint MaskIsMinSet = 0x04;
		const uint MaskIsMaxSet = 0x08;
		const uint MaskIsDefaultSet = 0x10;
		const uint MaskIsDefaultDateToday = 0x20;
		const uint MaskLoadFormula = 0x40;
		const uint MaskAllowFillIn = 0x80;
		#endregion
		const short fixedPartSize = 20;
		readonly XlsWebBasedDataProviderDataType dataType;
		readonly IXlsWebBasedDataProviderDefaultValue defaultValue;
		XLUnicodeString stringFormula;
		uint firstPackedValues;
		uint secondPackedValues;
		#endregion
		public XlsWebBasedDataProviderInfo(XlsWebBasedDataProviderDataType dataType) {
			this.dataType = dataType;
			this.defaultValue = CreateDefaultValue();
		}
		#region Properties
		public XlsWebBasedDataProviderDataType DataType { get { return dataType; } }
		public IXlsWebBasedDataProviderDefaultValue DefaultValue { get { return defaultValue; } }
		public XlsWebBasedDataProviderDefaultType DefaultType { get; set; }
		public int LanguageCodeId { get; set; }
		public int DecimalPlaces { get; set; }
		public XLUnicodeString StringFormula { 
			get { return stringFormula; } 
			set {
				LoadFormula = value != null;
				stringFormula = value; 
			} 
		}
		public bool IsPercent { 
			get { return GetBooleanValue(firstPackedValues, MaskIsPercent); }
			set { firstPackedValues = SetBooleanValue(firstPackedValues, MaskIsPercent, value); } 
		}
		public bool IsFixedDecimalPoint {
			get { return GetBooleanValue(firstPackedValues, MaskIsFixedDecimalPoint); }
			set { firstPackedValues = SetBooleanValue(firstPackedValues, MaskIsFixedDecimalPoint, value); }
		}
		public bool IsDateOnly {
			get { return GetBooleanValue(firstPackedValues, MaskIsDateOnly); }
			set { firstPackedValues = SetBooleanValue(firstPackedValues, MaskIsDateOnly, value); }
		}
		public XlReadingOrder ReadingOrder {
			get { return GetReadingOrderValue(MaskReadingOrder); }
			set { SetReadingOrderValue(MaskReadingOrder, value); }
		}
		public bool IsRichText {
			get { return GetBooleanValue(firstPackedValues, MaskIsRichText); }
			set { firstPackedValues = SetBooleanValue(firstPackedValues, MaskIsRichText, value); }
		}
		public bool IsRecognizedRichTextFormatting {
			get { return GetBooleanValue(firstPackedValues, MaskIsRecognizedRichTextFormatting); }
			set { firstPackedValues = SetBooleanValue(firstPackedValues, MaskIsRecognizedRichTextFormatting, value); }
		}
		public bool IsAlertRecognizedRichTextFormatting {
			get { return GetBooleanValue(firstPackedValues, MaskIsAlertRecognizedRichTextFormatting); }
			set { firstPackedValues = SetBooleanValue(firstPackedValues, MaskIsAlertRecognizedRichTextFormatting, value); }
		}
		public bool IsReadOnly {
			get { return GetBooleanValue(secondPackedValues, MaskIsReadOnly); }
			set { secondPackedValues = SetBooleanValue(secondPackedValues, MaskIsReadOnly, value); }
		}
		public bool IsRequired {
			get { return GetBooleanValue(secondPackedValues, MaskIsRequired); }
			set { secondPackedValues = SetBooleanValue(secondPackedValues, MaskIsRequired, value); }
		}
		public bool IsMinSet {
			get { return GetBooleanValue(secondPackedValues, MaskIsMinSet); }
			set { secondPackedValues = SetBooleanValue(secondPackedValues, MaskIsMinSet, value); }
		}
		public bool IsMaxSet {
			get { return GetBooleanValue(secondPackedValues, MaskIsMaxSet); }
			set { secondPackedValues = SetBooleanValue(secondPackedValues, MaskIsMaxSet, value); }
		}
		public bool IsDefaultSet {
			get { return GetBooleanValue(secondPackedValues, MaskIsDefaultSet); }
			set { secondPackedValues = SetBooleanValue(secondPackedValues, MaskIsDefaultSet, value); }
		}
		public bool IsDefaultDateToday {
			get { return GetBooleanValue(secondPackedValues, MaskIsDefaultDateToday); }
			set { secondPackedValues = SetBooleanValue(secondPackedValues, MaskIsDefaultDateToday, value); }
		}
		public bool LoadFormula {
			get { return GetBooleanValue(secondPackedValues, MaskLoadFormula); }
			set {
				if (value) {
					if (stringFormula == null)
						stringFormula = new XLUnicodeString();
				}
				else
					stringFormula = null;
				secondPackedValues = SetBooleanValue(secondPackedValues, MaskLoadFormula, value); 
			}
		}
		public bool AllowFillIn {
			get { return GetBooleanValue(secondPackedValues, MaskAllowFillIn); }
			set { secondPackedValues = SetBooleanValue(secondPackedValues, MaskAllowFillIn, value); }
		}
		#endregion
		IXlsWebBasedDataProviderDefaultValue CreateDefaultValue() {
			if (dataType == XlsWebBasedDataProviderDataType.ShortText ||
				dataType == XlsWebBasedDataProviderDataType.Choice ||
				dataType == XlsWebBasedDataProviderDataType.MultipleChoices)
				return new XlsWebBasedDataProviderDefaultValueUnicodeString();
			if (dataType == XlsWebBasedDataProviderDataType.Number ||
				dataType == XlsWebBasedDataProviderDataType.Currency ||
				dataType == XlsWebBasedDataProviderDataType.DateTime)
				return new XlsWebBasedDataProviderDefaultValueXnum();
			if (dataType == XlsWebBasedDataProviderDataType.Boolean)
				return new XlsWebBasedDataProviderDefaultValueBoolean();
			return null;
		}
		public void Read(BinaryReader reader) {
			LanguageCodeId = reader.ReadInt32();
			DecimalPlaces = reader.ReadInt32();
			firstPackedValues = reader.ReadByte();
			reader.ReadBytes(3);
			secondPackedValues = reader.ReadByte();
			DefaultType = (XlsWebBasedDataProviderDefaultType)reader.ReadByte();
			reader.ReadInt16();
			if (DefaultValue != null)
				DefaultValue.Read(reader);
			if (LoadFormula)
				StringFormula = XLUnicodeString.FromStream(reader);
			reader.ReadInt32();
		}
		public void Write(BinaryWriter writer) {
			writer.Write(LanguageCodeId);
			writer.Write(DecimalPlaces);
			writer.Write((byte)firstPackedValues);
			writer.Write(new byte[3]);
			writer.Write((byte)secondPackedValues);
			writer.Write((byte)DefaultType);
			writer.Write((ushort)0);
			if (DefaultValue != null)
				DefaultValue.Write(writer);
			if (LoadFormula)
				StringFormula.Write(writer);
			writer.Write(0);
		}
		protected internal short GetSize() {
			int result = fixedPartSize;
			if (DefaultValue != null)
				result += DefaultValue.GetSize();
			if (LoadFormula)
				result += StringFormula.Length;
			return (short)result;
		}
		#region GetUIntValue/SetUIntValue helpers
		uint SetUIntValue(uint packedValues, uint mask, int position, uint value) {
			packedValues &= ~mask;
			packedValues |= GetPackedValue(mask, position, value);
			return packedValues;
		}
		uint GetPackedValue(uint mask, int position, uint value) {
			return (value << position) & mask;
		}
		uint GetUIntValue(uint packedValues, uint mask, int position) {
			return ((packedValues & mask) >> position);
		}
		#endregion
		#region GetReadingOrderValue/SetReadingOrderValue helpers
		void SetReadingOrderValue(uint mask, XlReadingOrder value) {
			firstPackedValues = SetUIntValue(firstPackedValues, mask, 3, (uint)value);
		}
		XlReadingOrder GetReadingOrderValue(uint mask) {
			return (XlReadingOrder)GetUIntValue(firstPackedValues, mask, 3);
		}
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		uint SetBooleanValue(uint packedValues, uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
			return packedValues;
		}
		bool GetBooleanValue(uint packedValues, uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
	}
	#endregion
	#region XlsWebBasedDataProviderDefaultType
	public enum XlsWebBasedDataProviderDefaultType {
		Undefined = 0,
		String = 1,
		Boolean = 2,
		Number = 3
	}
	#endregion
	#region IXlsWebBasedDataProviderDefaultValue
	public interface IXlsWebBasedDataProviderDefaultValue {
		void Read(BinaryReader reader);
		void Write(BinaryWriter writer);
		int GetSize();
	}
	#endregion
	#region XlsWebBasedDataProviderDefaultValueUnicodeString
	public class XlsWebBasedDataProviderDefaultValueUnicodeString : IXlsWebBasedDataProviderDefaultValue {
		XLUnicodeString value = new XLUnicodeString();
		public string Value { get { return value.Value; } set { this.value.Value = value; } }
		#region IXlsWebBasedDataProviderDefaultValue Members
		public void Read(BinaryReader reader) {
			value = XLUnicodeString.FromStream(reader);
		}
		public void Write(BinaryWriter writer) {
			value.Write(writer);
		}
		public int GetSize() {
			return value.Length;
		}
		#endregion
	}
	#endregion
	#region XlsWebBasedDataProviderDefaultValueXnum
	public class XlsWebBasedDataProviderDefaultValueXnum : IXlsWebBasedDataProviderDefaultValue {
		readonly Xnum value = new Xnum();
		public double Value { get { return value.Value; } set { this.value.Value = value; } }
		#region IXlsWebBasedDataProviderDefaultValue Members
		public void Read(BinaryReader reader) {
			value.Read(reader);
		}
		public void Write(BinaryWriter writer) {
			value.Write(writer);
		}
		public int GetSize() {
			return value.Length;
		}
		#endregion
	}
	#endregion
	#region XlsWebBasedDataProviderDefaultValueBoolean
	public class XlsWebBasedDataProviderDefaultValueBoolean : IXlsWebBasedDataProviderDefaultValue {
		const int fixedSize = 4;
		bool Value { get; set; }
		#region IXlsWebBasedDataProviderDefaultValue Members
		public void Read(BinaryReader reader) {
			Value = Convert.ToBoolean(reader.ReadInt32());
		}
		public void Write(BinaryWriter writer) {
			writer.Write((uint)(Value ? 1 : 0));
		}
		public int GetSize() {
			return fixedSize;
		}
		#endregion
	}
	#endregion
}
