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
using System.Diagnostics;
using System.IO;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Model;
using System.Globalization;
using DevExpress.XtraExport.Xls;
using DevExpress.Utils;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsFilterComparisonOperator
	public enum XlsFilterComparisonOperator {
		NotUsed = 0,
		LessThan = 1,
		Equal = 2,
		LessThanOrEqual = 3,
		GreaterThan = 4,
		NotEqual = 5,
		GreaterThanOrEqual = 6
	}
	#endregion
	#region XlsFilterType
	public enum XlsFilterFormatType {
		None = 0x00000000,
		CellColor = 0x00000001,
		CellFont = 0x00000002,
		CellIcon = 0x00000003
	}
	#endregion
	#region XlsCustomFilterType
	public enum XlsCustomFilterType {
		None = 0x00000000,
		AboveAverage = 0x00000001,
		BelowAverage = 0x00000002,
		Tomorrow = 0x00000008,
		Today = 0x00000009,
		Yesterday = 0x0000000A,
		NextWeek = 0x0000000B,
		ThisWeek = 0x0000000C,
		LastWeek = 0x0000000D,
		NextMonth = 0x0000000E,
		ThisMonth = 0x0000000F,
		LastMonth = 0x00000010,
		NextQuarter = 0x00000011,
		ThisQuarter = 0x00000012,
		LastQuarter = 0x00000013,
		NextYear = 0x00000014,
		ThisYear = 0x00000015,
		LastYear = 0x00000016,
		YearToDate = 0x00000017,
		Quarter1 = 0x00000018,
		Quarter2 = 0x00000019,
		Quarter3 = 0x0000001A,
		Quarter4 = 0x0000001B,
		Month1 = 0x0000001C,
		Month2 = 0x0000001D,
		Month3 = 0x0000001E,
		Month4 = 0x0000001F,
		Month5 = 0x00000020,
		Month6 = 0x00000021,
		Month7 = 0x00000022,
		Month8 = 0x00000023,
		Month9 = 0x00000024,
		Month10 = 0x00000025,
		Month11 = 0x00000026,
		Month12 = 0x00000027
	}
	#endregion
	#region XlsIconSetType
	public enum XlsIconSetType {
		Arrows3 = 0x00000000,
		ArrowsGray3 = 0x00000001,
		Flags3 = 0x00000002,
		TrafficLights13 = 0x00000003,
		TrafficLights23 = 0x00000004,
		Signs3 = 0x00000005,
		Symbols3 = 0x00000006,
		Symbols23 = 0x00000007,
		Arrows4 = 0x00000008,
		ArrowsGray4 = 0x00000009,
		RedToBlack4 = 0x0000000A,
		Rating4 = 0x0000000B,
		TrafficLights4 = 0x0000000C,
		Arrows5 = 0x0000000D,
		ArrowsGray5 = 0x0000000E,
		Rating5 = 0x0000000F,
		Quarters5 = 0x00000010,
		None = -1
	}
	#endregion
	#region XlsDateFilterBy
	public enum XlsDateFilterBy {
		Year = 0,
		YearMonth = 1,
		YearMonthDay = 2,
		YearMonthDayHour = 3,
		YearMonthDayHourMinute = 4,
		YearMonthDayHourMinuteSecond = 5
	}
	#endregion
	#region ComparisonType
	public enum ComparisonType {
		Undefined = 0x00,
		RkNumber = 0x02,
		Xnum = 0x04,
		String = 0x06,
		BooleanOrError = 0x08,
		BlanksMatched = 0x0c,
		NonBlanksMatched = 0x0e
	}
	#endregion
	#region XlsCommandAutoFilterInfo
	public class XlsCommandAutoFilterInfo : XlsCommandShortPropertyValueBase {
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			DefinedNameBase filterDatabase;
			if (contentBuilder.CurrentSheet.DefinedNames.TryGetItemByName("_xlnm._FilterDatabase", out filterDatabase)) {
				CellRange range = ((DefinedName)filterDatabase).GetReferencedRange() as CellRange;
				if (range != null) {
					CellRange preparedRange = new CellRange(contentBuilder.CurrentSheet, range.TopLeft.AsRelative(), range.BottomRight.AsRelative());
					SheetAutoFilter autoFilter = contentBuilder.CurrentSheet.AutoFilter;
					autoFilter.SetRange(preparedRange);
					autoFilter.CreateFilterColumnsForRange(preparedRange);
				}
			}
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandAutoFilter
	public class XlsCommandAutoFilter : XlsCommandBase {
		public XlsAutoFilterInfo AutoFilterInfo { get; set; }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			using (XlsCommandStream stream = new XlsCommandStream(reader, Size)) {
				using (BinaryReader binaryReader = new BinaryReader(stream))
					AutoFilterInfo = XlsAutoFilterInfo.FromStream(binaryReader);
			}
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			AutoFilterInfo.ApplySheetContent(contentBuilder.CurrentSheet.AutoFilter, contentBuilder);
		}
		protected override void WriteCore(BinaryWriter writer) {
			AutoFilterInfo.Write(writer);
		}
		protected override short GetSize() {
			return AutoFilterInfo.GetSize();
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandAutoFilter12
	public class XlsCommandAutoFilter12 : XlsCommandRecordBase {
		#region Static Members
		static short[] typeCodes = new short[] {
			0x087e, 
			0x087f  
		};
		#endregion
		XlsAutoFilter12Info autoFilterInfo;
		public XlsAutoFilter12Info AutoFilterInfo { get { return autoFilterInfo; } set { autoFilterInfo = value; } }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (contentBuilder.ContentType == XlsContentType.Sheet) {
				FutureRecordHeaderRef header = FutureRecordHeaderRef.FromStream(reader);
				int recordSize = Size - header.GetSize();
				using (XlsCommandStream stream = new XlsCommandStream(contentBuilder, reader, typeCodes, recordSize)) {
					using (BinaryReader autoFilterReader = new BinaryReader(stream))
						autoFilterInfo = XlsAutoFilter12Info.FromStream(autoFilterReader, header.Range, recordSize);
				}
			}
			else
				base.ReadCore(reader, contentBuilder);
		}
		protected override void CheckPosition(XlsReader reader, XlsContentBuilder contentBuilder, long initialPosition, long expectedPosition) {
		}
		protected override void ApplySheetContent(XlsContentBuilder contentBuilder) {
			autoFilterInfo.ApplyContent(contentBuilder);
		}
		public override IXlsCommand GetInstance() {
			return new XlsCommandAutoFilter12();
		}
	}
	#endregion
	#region XlsCommandFilterMode
	public class XlsCommandFilterMode : XlsCommandBase {
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			if (Size > 0)
				reader.ReadBytes(Size);
		}
		public override void Execute(XlsContentBuilder contentBuilder) {
		}
		protected override void WriteCore(BinaryWriter writer) {
		}
		protected override short GetSize() {
			return 0;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsAutoFilterInfo
	public class XlsAutoFilterInfo {
		#region Static Members
		public static XlsAutoFilterInfo FromStream(BinaryReader reader) {
			XlsAutoFilterInfo result = new XlsAutoFilterInfo();
			result.Read(reader);
			return result;
		}
		public static XlsAutoFilterInfo CreateForExport(AutoFilterColumn autoFilterColumn) {
			XlsAutoFilterInfo result = new XlsAutoFilterInfo();
			result.AssignThisProperties(autoFilterColumn);
			return result;
		}
		#endregion
		#region Fields
		const uint MaskIdEntry = 0x0000FFFF; 
		const uint MaskWJoin = 0x00010000; 
		const uint MaskIsSimpleFirst = 0x00040000;
		const uint MaskIsSimpleSecond = 0x00080000;
		const uint MaskIsTopN = 0x00100000;
		const uint MaskIsTop = 0x00200000;
		const uint MaskIsPercent = 0x00400000;
		const uint MaskNumberTopN = 0xFF800000; 
		const short fixedPartSize = 24;
		uint packedValues;
		XlsAutoFilterCriteria firstCriteria = new XlsAutoFilterCriteria();
		XlsAutoFilterCriteria secondCriteria = new XlsAutoFilterCriteria();
		#endregion
		#region Properties
		public int IdEntry { get { return GetIntValue(MaskIdEntry, 0); } set { SetIntValue(MaskIdEntry, 0, value); } }
		public bool WJoin { get { return GetBooleanValue(MaskWJoin); } set { SetBooleanValue(MaskWJoin, value); } }
		public bool IsSimpleFirst { get { return GetBooleanValue(MaskIsSimpleFirst); } set { SetBooleanValue(MaskIsSimpleFirst, value); } }
		public bool IsSimpleSecond { get { return GetBooleanValue(MaskIsSimpleSecond); } set { SetBooleanValue(MaskIsSimpleSecond, value); } }
		public bool IsTopN { get { return GetBooleanValue(MaskIsTopN); } set { SetBooleanValue(MaskIsTopN, value); } }
		public bool IsTop { get { return GetBooleanValue(MaskIsTop); } set { SetBooleanValue(MaskIsTop, value); } }
		public bool IsPercent { get { return GetBooleanValue(MaskIsPercent); } set { SetBooleanValue(MaskIsPercent, value); } }
		public int NumberTopN { get { return GetIntValue(MaskNumberTopN, 23); } set { SetIntValue(MaskNumberTopN, 23, value); } }
		public XlsAutoFilterCriteria FirstCriteria { get { return firstCriteria; } }
		public XlsAutoFilterCriteria SecondCriteria { get { return secondCriteria; } }
		#endregion
		void Read(BinaryReader reader) {
			packedValues = reader.ReadUInt32();
			FirstCriteria.DataOperation.Read(reader, false);
			SecondCriteria.DataOperation.Read(reader, false);
			FirstCriteria.ReadComparisonStringValue(reader);
			SecondCriteria.ReadComparisonStringValue(reader);
		}
		public void Write(BinaryWriter writer) {
			writer.Write(packedValues);
			FirstCriteria.DataOperation.Write(writer);
			SecondCriteria.DataOperation.Write(writer);
			FirstCriteria.WriteComparisonStringValue(writer);
			SecondCriteria.WriteComparisonStringValue(writer);
		}
		public short GetSize() {
			int result = fixedPartSize;
			result += firstCriteria.GetStringSize();
			result += secondCriteria.GetStringSize();
			return (short)result;
		}
		#region GetUIntValue/SetUIntValue helpers
		void SetUIntValue(uint mask, int position, uint value) {
			packedValues &= ~mask;
			packedValues |= GetPackedValue(mask, position, value);
		}
		uint GetPackedValue(uint mask, int position, uint value) {
			return (value << position) & mask;
		}
		uint GetUIntValue(uint mask, int position) {
			return ((packedValues & mask) >> position);
		}
		#endregion
		#region GetIntValue/SetIntValue helpers
		void SetIntValue(uint mask, int position, int value) {
			SetUIntValue(mask, position, (uint)value);
		}
		int GetIntValue(uint mask, int position) {
			return (int)GetUIntValue(mask, position);
		}
		#endregion
		#region GetBooleanValue/SetBooleanValue helpers
		void SetBooleanValue(uint mask, bool bitVal) {
			if (bitVal)
				packedValues |= mask;
			else
				packedValues &= ~mask;
		}
		bool GetBooleanValue(uint mask) {
			return (packedValues & mask) != 0;
		}
		#endregion
		#region ApplyContent
		internal void ApplySheetContent(AutoFilterBase autoFilter, XlsContentBuilder contentBuilder) {
			AutoFilterColumnCollection columns = autoFilter.FilterColumns;
			if (IdEntry < 0 || IdEntry > columns.Count - 1)
				return;
			ApplyContent(columns[IdEntry], IdEntry, false, contentBuilder);
		}
		internal void ApplyContent(AutoFilterColumn filterColumn, int columnId, bool autoFilterHidden, XlsContentBuilder contentBuilder) {
			Guard.ArgumentNotNull(filterColumn, "filterColumn");
			if (!FirstCriteria.Exists && !SecondCriteria.Exists)
				return;
			filterColumn.BeginUpdate();
			try {
				filterColumn.ColumnId = columnId;
				filterColumn.HiddenAutoFilterButton = autoFilterHidden;
				SetupFilterColumn(filterColumn, contentBuilder);
			}
			finally {
				filterColumn.EndUpdate();
			}
		}
		void SetupFilterColumn(AutoFilterColumn filterColumn, XlsContentBuilder contentBuilder) {
			if (IsTopN)
				SetupTopFilterColumn(filterColumn);
			else
				SetupFilterColumn(filterColumn, contentBuilder.DocumentModel.DataContext);
		}
		void SetupTopFilterColumn(AutoFilterColumn filterColumn) {
			filterColumn.FilterByTopOrder = IsTop;
			filterColumn.Top10FilterType = IsPercent ? Top10FilterType.Percent : Top10FilterType.Count;
			filterColumn.TopOrBottomDoubleValue = NumberTopN;
			filterColumn.FilterDoubleValue = FirstCriteria.DataOperation.ComparisonValue.GetNumericValue();
		}
		void SetupFilterColumn(AutoFilterColumn filterColumn, WorkbookDataContext dataContext) {
			bool isSimpleFilter = IsSimpleFirst && !FirstCriteria.DataOperation.ValueNonBlankMatched &&
								  (!SecondCriteria.Exists || (IsSimpleSecond && WJoin &&
								  !SecondCriteria.DataOperation.ValueNonBlankMatched));
			if (isSimpleFilter)
				SetupFilterCriteria(filterColumn.FilterCriteria, dataContext.Culture);
			else
				SetupCustomFilters(filterColumn.CustomFilters, dataContext);
		}
		void SetupFilterCriteria(FilterCriteria filterCriteria, CultureInfo cultureInfo) {
			firstCriteria.SetupFilterCriteria(filterCriteria, cultureInfo);
			if (SecondCriteria.Exists)
				secondCriteria.SetupFilterCriteria(filterCriteria, cultureInfo);
		}
		void SetupCustomFilters(CustomFilterCollection filters, WorkbookDataContext dataContext) {
			filters.CriterionAnd = !WJoin;
			filters.Add(FirstCriteria.CreateCustomFilter(dataContext));
			if (SecondCriteria.Exists)
				filters.Add(SecondCriteria.CreateCustomFilter(dataContext));
		}
		#endregion
		#region AssignThisProperties
		void AssignThisProperties(AutoFilterColumn filterColumn) {
			IdEntry = filterColumn.ColumnId;
			if (filterColumn.IsTop10Filter)
				SetupTopFilterProperties(filterColumn);
			else
				SetupFilterProperties(filterColumn);
		}
		void SetupTopFilterProperties(AutoFilterColumn filterColumn) {
			IsTopN = true;
			IsPercent = filterColumn.Top10FilterType == Top10FilterType.Percent;
			IsTop = filterColumn.FilterByTopOrder;
			NumberTopN = (int)filterColumn.TopOrBottomDoubleValue;
			AutoFilterDataOperationValueXnum comparisonValue = new AutoFilterDataOperationValueXnum();
			comparisonValue.Value = filterColumn.FilterDoubleValue;
			FirstCriteria.DataOperation.ComparisonValue = comparisonValue;
		}
		void SetupFilterProperties(AutoFilterColumn filterColumn) {
			FilterCriteria filterCriteria = filterColumn.FilterCriteria;
			if (filterCriteria.HasFilter())
				SetupFilterCriteriaProperties(filterCriteria);
			else
				SetupCustomFiltersProperties(filterColumn.CustomFilters);
		}
		void SetupCustomFiltersProperties(CustomFilterCollection customFilters) {
			bool isSimple;
			WJoin = !customFilters.CriterionAnd;
			int count = customFilters.Count;
			for (int i = 0; i < count; i++) {
				CustomFilter customFilter = customFilters[i];
				if (!Convert.ToBoolean(i)) {
					FirstCriteria.AssignFromCustomFilter(customFilter.NumericValue, customFilter.Value, customFilter.FilterOperator, out isSimple);
					IsSimpleFirst = isSimple;
				}
				else {
					SecondCriteria.AssignFromCustomFilter(customFilter.NumericValue, customFilter.Value, customFilter.FilterOperator, out isSimple);
					IsSimpleSecond = isSimple;
				}
			}
		}
		void SetupFilterCriteriaProperties(FilterCriteria filterCriteria) {
			FilterCollection filters = filterCriteria.Filters;
			int count = filters.Count;
			WJoin = true;
			if (filterCriteria.FilterByBlank) {
				IsSimpleFirst = true;
				FirstCriteria.DataOperation.ComparisonValue = new AutoFilterDataOperationValueBlank();
				FirstCriteria.DataOperation.FilterComparisonOperator = XlsFilterComparisonOperator.Equal;
				if (count > 0)
					SetupFilterCriteriaCore(filters[0], false);
			}
			else {
				for (int i = 0; i < count; i++)
					SetupFilterCriteriaCore(filters[i], !Convert.ToBoolean(i));
			}
		}
		void SetupFilterCriteriaCore(string value, bool first) {
			if (first) {
				IsSimpleFirst = true;
				FirstCriteria.ComparisonStringValue = value;
				FirstCriteria.DataOperation.FilterComparisonOperator = XlsFilterComparisonOperator.Equal;
			}
			else {
				IsSimpleSecond = true;
				SecondCriteria.ComparisonStringValue = value;
				SecondCriteria.DataOperation.FilterComparisonOperator = XlsFilterComparisonOperator.Equal;
			}
		}
		#endregion
	}
	#endregion
	#region XlsAutoFilterCriteria
	public class XlsAutoFilterCriteria {
		#region Static Members
		public static XlsAutoFilterCriteria FromStream(BinaryReader reader, bool forAutoFilter12) {
			XlsAutoFilterCriteria result = new XlsAutoFilterCriteria();
			result.ForAutoFilter12 = forAutoFilter12;
			result.Read(reader);
			return result;
		}
		#endregion
		const short fixedSize = 10;
		AutoFilterDataOperation dataOperation = new AutoFilterDataOperation();
		XLUnicodeStringNoCch comparisonStringValue = new XLUnicodeStringNoCch();
		#region Properties
		public AutoFilterDataOperation DataOperation { get { return dataOperation; } set { dataOperation = value; } }
		public string ComparisonStringValue {
			get { return comparisonStringValue.Value; }
			set {
				comparisonStringValue.Value = value;
				dataOperation.ComparisonValue = CreateAutoFilterDataOperationValueString(comparisonStringValue, !CustomFilter.ContainsWildcardCharacters(value));
			}
		}
		public bool Exists {
			get {
				return dataOperation.FilterComparisonOperator != XlsFilterComparisonOperator.NotUsed &&
					   dataOperation.ComparisonValue.ComparisonType != ComparisonType.Undefined;
			}
		}
		public bool ForAutoFilter12 { get; set; }
		#endregion
		AutoFilterDataOperationValueString CreateAutoFilterDataOperationValueString(XLUnicodeStringNoCch value, bool isCompare) {
			AutoFilterDataOperationValueString result = new AutoFilterDataOperationValueString();
			result.CharactersCount = value.Value.Length;
			result.IsCompare = isCompare;
			result.ForAutoFilter12 = ForAutoFilter12;
			return result;
		}
		#region Read & Write
		public void Read(BinaryReader reader) {
			DataOperation.Read(reader, ForAutoFilter12);
			ReadComparisonStringValue(reader);
		}
		public void ReadComparisonStringValue(BinaryReader reader) {
			AutoFilterDataOperationValueString dataOperationString = dataOperation.ComparisonValue as AutoFilterDataOperationValueString;
			if (dataOperationString != null)
				comparisonStringValue = XLUnicodeStringNoCch.FromStream(reader, dataOperationString.CharactersCount);
		}
		public void Write(BinaryWriter writer) {
			DataOperation.Write(writer);
			WriteComparisonStringValue(writer);
		}
		public void WriteComparisonStringValue(BinaryWriter writer) {
			AutoFilterDataOperationValueString dataOperationString = dataOperation.ComparisonValue as AutoFilterDataOperationValueString;
			if (dataOperationString != null)
				comparisonStringValue.Write(writer);
		}
		#endregion
		#region Import
		internal void SetupFilterCriteria(FilterCriteria filterCriteria, CultureInfo cultureInfo) {
			if (dataOperation.ComparisonValue.ComparisonType == ComparisonType.BlanksMatched)
				filterCriteria.FilterByBlank = true;
			else {
				string comparisonStringValue = GetStringValue(cultureInfo);
				if (!String.IsNullOrEmpty(comparisonStringValue))
					filterCriteria.Filters.Add(comparisonStringValue);
			}
		}
		string GetStringValue(CultureInfo cultureInfo) {
			AutoFilterDataOperationValueBase comparisonValue = dataOperation.ComparisonValue;
			if (comparisonValue.ComparisonType != ComparisonType.String)
				return comparisonValue.GetStringValue(cultureInfo);
			return ComparisonStringValue;
		}
		internal CustomFilter CreateCustomFilter(WorkbookDataContext dataContext) {
			CustomFilter customFilter = new CustomFilter();
			customFilter.FilterOperator = (FilterComparisonOperator)dataOperation.FilterComparisonOperator;
			customFilter.Value = GetStringValue(dataContext.Culture);
			customFilter.UpdateNumericValue(dataContext, false);
			return customFilter;
		}
		#endregion
		#region Export
		internal void AssignFromCustomFilter(VariantValue value, string stringValue, FilterComparisonOperator filterOperator, out bool isSimple) {
			isSimple = false;
			dataOperation.FilterComparisonOperator = (XlsFilterComparisonOperator)filterOperator;
			if (value.Type == VariantValueType.Numeric)
				dataOperation.ComparisonValue = AutoFilterDataOperationValueXnum.Create(value.NumericValue);
			else {
				if (String.IsNullOrEmpty(stringValue)) {
					isSimple = true;
					dataOperation.ComparisonValue = CreateBlankOrNonBlankMatchedValue(filterOperator);
				}
				else
					ComparisonStringValue = stringValue;
			}
		}
		AutoFilterDataOperationValueBase CreateBlankOrNonBlankMatchedValue(FilterComparisonOperator filterOperator) {
			if (filterOperator == FilterComparisonOperator.Equal)
				return new AutoFilterDataOperationValueBlank();
			return new AutoFilterDataOperationValueNonBlank();
		}
		#endregion
		public int GetStringSize() {
			AutoFilterDataOperationValueString valueString = dataOperation.ComparisonValue as AutoFilterDataOperationValueString;
			if (valueString != null)
				return comparisonStringValue.Length;
			return 0;
		}
		public int GetSize() {
			return fixedSize + GetStringSize();
		}
	}
	#endregion
	#region AutoFilterDataOperation
	public class AutoFilterDataOperation {
		#region Fields
		const short fixedSize = 10;
		XlsFilterComparisonOperator filterComparisonOperator = XlsFilterComparisonOperator.NotUsed;
		AutoFilterDataOperationValueBase comparisonValue = new AutoFilterDataOperationValueUndefined();
		#endregion
		#region Properties
		public XlsFilterComparisonOperator FilterComparisonOperator { get { return filterComparisonOperator; } set { filterComparisonOperator = value; } }
		public AutoFilterDataOperationValueBase ComparisonValue { get { return comparisonValue; } set { comparisonValue = value; } }
		public short Size { get { return fixedSize; } }
		public bool OperatorIsEqual { get { return filterComparisonOperator == XlsFilterComparisonOperator.Equal; } }
		public bool ValueNonBlankMatched { get { return ComparisonValue.ComparisonType == ComparisonType.NonBlanksMatched; } }
		#endregion
		public void Read(BinaryReader reader, bool forAutoFilter12) {
			ComparisonType comparisonType = (ComparisonType)reader.ReadByte();
			FilterComparisonOperator = (XlsFilterComparisonOperator)reader.ReadByte();
			CreateAutoFilterDataOperationValue(comparisonType, forAutoFilter12);
			ComparisonValue.Read(reader);
		}
		public void Write(BinaryWriter writer) {
			writer.Write((byte)ComparisonValue.ComparisonType);
			writer.Write((byte)FilterComparisonOperator);
			ComparisonValue.Write(writer);
		}
		void CreateAutoFilterDataOperationValue(ComparisonType comparisonType, bool forAutoFilter12) {
			if (comparisonType == ComparisonType.Undefined)
				return;
			if (comparisonType == ComparisonType.BlanksMatched)
				ComparisonValue = new AutoFilterDataOperationValueBlank();
			if (comparisonType == ComparisonType.NonBlanksMatched)
				ComparisonValue = new AutoFilterDataOperationValueNonBlank();
			if (comparisonType == ComparisonType.RkNumber)
				ComparisonValue = new AutoFilterDataOperationValueRkNumber();
			if (comparisonType == ComparisonType.Xnum)
				ComparisonValue = new AutoFilterDataOperationValueXnum();
			if (comparisonType == ComparisonType.String) {
				AutoFilterDataOperationValueString dataOperationString = new AutoFilterDataOperationValueString();
				dataOperationString.ForAutoFilter12 = forAutoFilter12;
				ComparisonValue = dataOperationString;
			}
			if (comparisonType == ComparisonType.BooleanOrError)
				ComparisonValue = new AutoFilterDataOperationValueBooleanOrError();
		}
	}
	#endregion
	#region AutoFilterDataOperationValueBase (abstract class)
	public abstract class AutoFilterDataOperationValueBase {
		public virtual ComparisonType ComparisonType { get { return ComparisonType.Undefined; } }
		public virtual void Read(BinaryReader reader) {
			reader.ReadInt64();
		}
		public virtual void Write(BinaryWriter writer) {
			writer.Write((ulong)0);
		}
		public virtual string GetStringValue(CultureInfo culture) {
			return String.Empty;
		}
		public virtual double GetNumericValue() {
			return 0;
		}
	}
	#endregion
	#region AutoFilterDataOperationValueUndefined
	public class AutoFilterDataOperationValueUndefined : AutoFilterDataOperationValueBase {
	}
	#endregion
	#region AutoFilterDataOperationValueBlank
	public class AutoFilterDataOperationValueBlank : AutoFilterDataOperationValueBase {
		public override ComparisonType ComparisonType { get { return ComparisonType.BlanksMatched; } }
	}
	#endregion
	#region AutoFilterDataOperationValueNonBlank
	public class AutoFilterDataOperationValueNonBlank : AutoFilterDataOperationValueBase {
		public override ComparisonType ComparisonType { get { return ComparisonType.NonBlanksMatched; } }
	}
	#endregion
	#region AutoFilterDataOperationValueRkNumber
	public class AutoFilterDataOperationValueRkNumber : AutoFilterDataOperationValueBase {
		public override ComparisonType ComparisonType { get { return ComparisonType.RkNumber; } }
		public RkNumber RkNumberValue { get; set; }
		public override void Read(BinaryReader reader) {
			RkNumberValue = new RkNumber(reader.ReadInt32());
			reader.ReadInt32();
		}
		public override void Write(BinaryWriter writer) {
			writer.Write(RkNumberValue.GetRawValue());
			writer.Write(0);
		}
		public override string GetStringValue(CultureInfo culture) {
			return RkNumberValue.Value.ToString(culture);
		}
		public override double GetNumericValue() {
			return RkNumberValue.Value;
		}
	}
	#endregion
	#region AutoFilterDataOperationValueXnum
	public class AutoFilterDataOperationValueXnum : AutoFilterDataOperationValueBase {
		#region Static members
		public static AutoFilterDataOperationValueXnum Create(double value) {
			AutoFilterDataOperationValueXnum result = new AutoFilterDataOperationValueXnum();
			result.Value = value;
			return result;
		}
		#endregion
		readonly Xnum value = new Xnum();
		public override ComparisonType ComparisonType { get { return ComparisonType.Xnum; } }
		public double Value { get { return value.Value; } set { this.value.Value = value; } }
		public override void Read(BinaryReader reader) {
			value.Read(reader);
		}
		public override void Write(BinaryWriter writer) {
			value.Write(writer);
		}
		public override string GetStringValue(CultureInfo culture) {
			return Value.ToString(culture);
		}
		public override double GetNumericValue() {
			return Value;
		}
	}
	#endregion
	#region AutoFilterDataOperationValueString
	public class AutoFilterDataOperationValueString : AutoFilterDataOperationValueBase {
		public override ComparisonType ComparisonType { get { return ComparisonType.String; } }
		public int CharactersCount { get; set; }
		public bool IsCompare { get; set; }
		public bool ForAutoFilter12 { get; set; }
		public override void Read(BinaryReader reader) {
			if (!ForAutoFilter12)
				reader.ReadInt32();
			CharactersCount = (int)reader.ReadByte();
			IsCompare = reader.ReadBoolean();
			reader.ReadInt16();
			if (ForAutoFilter12)
				reader.ReadInt32();
		}
		public override void Write(BinaryWriter writer) {
			if (!ForAutoFilter12)
				writer.Write(0);
			writer.Write((byte)CharactersCount);
			writer.Write(IsCompare);
			writer.Write((ushort)0);
			if (ForAutoFilter12)
				writer.Write(0);
		}
	}
	#endregion
	#region AutoFilterDataOperationValueBooleanOrError
	public class AutoFilterDataOperationValueBooleanOrError : AutoFilterDataOperationValueBase {
		public override ComparisonType ComparisonType { get { return ComparisonType.BooleanOrError; } }
		public Bes Bes { get; set; }
		public override void Read(BinaryReader reader) {
			Bes = new Bes();
			Bes.Read(reader);
			reader.ReadInt16();
			reader.ReadInt32();
		}
		public override void Write(BinaryWriter writer) {
			Bes.Write(writer);
			writer.Write((ushort)0);
			writer.Write(0);
		}
		public override string GetStringValue(CultureInfo culture) {
			return Bes.GetStringValue(culture);
		}
		public override double GetNumericValue() {
			if (Bes.IsError)
				return -1;
			return Bes.BoolValue ? 1 : 0;
		}
	}
	#endregion
	#region Bes
	public class Bes {
		#region Fields
		int errorCode;
		bool isError;
		#endregion
		#region Properties
		public bool BoolValue {
			get { return errorCode != 0; }
			set {
				isError = false;
				errorCode = value ? 1 : 0;
			}
		}
		public VariantValue ErrorValue {
			get {
				if (this.IsError)
					return ErrorConverter.ErrorCodeToValue(this.errorCode);
				return VariantValue.Empty;
			}
			set {
				if (value.Type == VariantValueType.Error) {
					isError = true;
					errorCode = ErrorConverter.ValueToErrorCode(value);
				}
			}
		}
		public bool IsError { get { return isError; } }
		#endregion
		public void Read(BinaryReader reader) {
			this.errorCode = reader.ReadByte();
			this.isError = reader.ReadBoolean();
		}
		public void Write(BinaryWriter writer) {
			writer.Write((byte)this.errorCode);
			writer.Write(this.isError);
		}
		public string GetStringValue(CultureInfo culture) {
			if (IsError)
				return ErrorValue.ErrorValue.Name;
			return BoolValue.ToString().ToUpper(culture);
		}
	}
	#endregion
	#region XlsAutoFilter12Info
	public class XlsAutoFilter12Info {
		#region Static Members
		public static XlsAutoFilter12Info CreateForSheetExport(CellRangeInfo cellRangeInfo, AutoFilterColumn filterColumn) {
			XlsAutoFilter12Info result = new XlsAutoFilter12Info(cellRangeInfo);
			result.AssignThisProperties(filterColumn);
			return result;
		}
		public static XlsAutoFilter12Info CreateForTableExport(CellRangeInfo cellRangeInfo, AutoFilterColumn filterColumn, int tableId) {
			XlsAutoFilter12Info result = new XlsAutoFilter12Info(cellRangeInfo);
			result.AssignThisProperties(filterColumn, tableId);
			return result;
		}
		public static XlsAutoFilter12Info FromStream(BinaryReader reader, CellRangeInfo cellRangeInfo, int size) {
			XlsAutoFilter12Info result = new XlsAutoFilter12Info(cellRangeInfo);
			result.size = size;
			result.Read(reader);
			return result;
		}
		#region DynamicFilterTypeTable
		static Dictionary<XlsCustomFilterType, DynamicFilterType> DynamicFilterTypeTable = CreateDynamicFilterTypeTable();
		static Dictionary<XlsCustomFilterType, DynamicFilterType> CreateDynamicFilterTypeTable() {
			Dictionary<XlsCustomFilterType, DynamicFilterType> result = new Dictionary<XlsCustomFilterType, DynamicFilterType>();
			result.Add(XlsCustomFilterType.None, DynamicFilterType.Null);
			result.Add(XlsCustomFilterType.AboveAverage, DynamicFilterType.AboveAverage);
			result.Add(XlsCustomFilterType.BelowAverage, DynamicFilterType.BelowAverage);
			result.Add(XlsCustomFilterType.Tomorrow, DynamicFilterType.Tomorrow);
			result.Add(XlsCustomFilterType.Today, DynamicFilterType.Today);
			result.Add(XlsCustomFilterType.Yesterday, DynamicFilterType.Yesterday);
			result.Add(XlsCustomFilterType.NextWeek, DynamicFilterType.NextWeek);
			result.Add(XlsCustomFilterType.ThisWeek, DynamicFilterType.ThisWeek);
			result.Add(XlsCustomFilterType.LastWeek, DynamicFilterType.LastWeek);
			result.Add(XlsCustomFilterType.NextMonth, DynamicFilterType.NextMonth);
			result.Add(XlsCustomFilterType.ThisMonth, DynamicFilterType.ThisMonth);
			result.Add(XlsCustomFilterType.LastMonth, DynamicFilterType.LastMonth);
			result.Add(XlsCustomFilterType.NextQuarter, DynamicFilterType.NextQuarter);
			result.Add(XlsCustomFilterType.ThisQuarter, DynamicFilterType.ThisQuarter);
			result.Add(XlsCustomFilterType.LastQuarter, DynamicFilterType.LastQuarter);
			result.Add(XlsCustomFilterType.NextYear, DynamicFilterType.NextYear);
			result.Add(XlsCustomFilterType.ThisYear, DynamicFilterType.ThisYear);
			result.Add(XlsCustomFilterType.LastYear, DynamicFilterType.LastYear);
			result.Add(XlsCustomFilterType.YearToDate, DynamicFilterType.YearToDate);
			result.Add(XlsCustomFilterType.Quarter1, DynamicFilterType.Q1);
			result.Add(XlsCustomFilterType.Quarter2, DynamicFilterType.Q2);
			result.Add(XlsCustomFilterType.Quarter3, DynamicFilterType.Q3);
			result.Add(XlsCustomFilterType.Quarter4, DynamicFilterType.Q4);
			result.Add(XlsCustomFilterType.Month1, DynamicFilterType.M1);
			result.Add(XlsCustomFilterType.Month2, DynamicFilterType.M2);
			result.Add(XlsCustomFilterType.Month3, DynamicFilterType.M3);
			result.Add(XlsCustomFilterType.Month4, DynamicFilterType.M4);
			result.Add(XlsCustomFilterType.Month5, DynamicFilterType.M5);
			result.Add(XlsCustomFilterType.Month6, DynamicFilterType.M6);
			result.Add(XlsCustomFilterType.Month7, DynamicFilterType.M7);
			result.Add(XlsCustomFilterType.Month8, DynamicFilterType.M8);
			result.Add(XlsCustomFilterType.Month9, DynamicFilterType.M9);
			result.Add(XlsCustomFilterType.Month10, DynamicFilterType.M10);
			result.Add(XlsCustomFilterType.Month11, DynamicFilterType.M11);
			result.Add(XlsCustomFilterType.Month12, DynamicFilterType.M12);
			return result;
		}
		#endregion
		#region DynamicFilterTypesWithValues
		static List<DynamicFilterType> DynamicFilterTypesWithValues = CreateDynamicFilterTypesWithValuesList();
		static List<DynamicFilterType> CreateDynamicFilterTypesWithValuesList() {
			List<DynamicFilterType> result = new List<DynamicFilterType>();
			result.Add(DynamicFilterType.AboveAverage);
			result.Add(DynamicFilterType.BelowAverage);
			result.Add(DynamicFilterType.Tomorrow);
			result.Add(DynamicFilterType.Today);
			result.Add(DynamicFilterType.Yesterday);
			result.Add(DynamicFilterType.NextWeek);
			result.Add(DynamicFilterType.ThisWeek);
			result.Add(DynamicFilterType.LastWeek);
			result.Add(DynamicFilterType.NextMonth);
			result.Add(DynamicFilterType.ThisMonth);
			result.Add(DynamicFilterType.LastMonth);
			result.Add(DynamicFilterType.NextQuarter);
			result.Add(DynamicFilterType.ThisQuarter);
			result.Add(DynamicFilterType.LastQuarter);
			result.Add(DynamicFilterType.NextYear);
			result.Add(DynamicFilterType.ThisYear);
			result.Add(DynamicFilterType.LastYear);
			result.Add(DynamicFilterType.YearToDate);
			return result;
		}
		#endregion
		#region IconSetTypeTable
		internal static Dictionary<XlsIconSetType, IconSetType> IconSetTypeTable = CreateIconSetTypeTable();
		static Dictionary<XlsIconSetType, IconSetType> CreateIconSetTypeTable() {
			Dictionary<XlsIconSetType, IconSetType> result = new Dictionary<XlsIconSetType, IconSetType>();
			result.Add(XlsIconSetType.None, IconSetType.None);
			result.Add(XlsIconSetType.Arrows3, IconSetType.Arrows3);
			result.Add(XlsIconSetType.ArrowsGray3, IconSetType.ArrowsGray3);
			result.Add(XlsIconSetType.Flags3, IconSetType.Flags3);
			result.Add(XlsIconSetType.TrafficLights13, IconSetType.TrafficLights13);
			result.Add(XlsIconSetType.TrafficLights23, IconSetType.TrafficLights23);
			result.Add(XlsIconSetType.Signs3, IconSetType.Signs3);
			result.Add(XlsIconSetType.Symbols3, IconSetType.Symbols3);
			result.Add(XlsIconSetType.Symbols23, IconSetType.Symbols23);
			result.Add(XlsIconSetType.Arrows4, IconSetType.Arrows4);
			result.Add(XlsIconSetType.ArrowsGray4, IconSetType.ArrowsGray4);
			result.Add(XlsIconSetType.RedToBlack4, IconSetType.RedToBlack4);
			result.Add(XlsIconSetType.Rating4, IconSetType.Rating4);
			result.Add(XlsIconSetType.TrafficLights4, IconSetType.TrafficLights4);
			result.Add(XlsIconSetType.Arrows5, IconSetType.Arrows5);
			result.Add(XlsIconSetType.ArrowsGray5, IconSetType.ArrowsGray5);
			result.Add(XlsIconSetType.Rating5, IconSetType.Rating5);
			result.Add(XlsIconSetType.Quarters5, IconSetType.Quarters5);
			return result;
		}
		#endregion
		#endregion
		#region Fields
		const short fixedSize = 48;
		const short typeCode = 0x087e;
		readonly CellRangeInfo cellRangeInfo;
		DxfN12ListInfo cellFormatInfo;
		List<XlsAutoFilterCriteria> criteriaList;
		List<XlsAutoFilterDateInfo> dateGroupingsList;
		Guid guidSView;
		int size;
		int tableId = -1;
		int iconNumber = -1;
		XlsIconSetType iconSetType = XlsIconSetType.None;
		#endregion
		public XlsAutoFilter12Info(CellRangeInfo cellRangeInfo) {
			Guard.ArgumentNotNull(cellRangeInfo, "cellRangeInfo");
			this.cellRangeInfo = cellRangeInfo;
			this.criteriaList = new List<XlsAutoFilterCriteria>();
			this.dateGroupingsList = new List<XlsAutoFilterDateInfo>();
			this.guidSView = new Guid();
		}
		#region Properties
		public CellRangeInfo Range { get { return cellRangeInfo; } }
		public List<XlsAutoFilterCriteria> CriteriaList { get { return criteriaList; } }
		public List<XlsAutoFilterDateInfo> DateGroupingsList { get { return dateGroupingsList; } }
		public DxfN12ListInfo CellFormat { get { return cellFormatInfo; } set { cellFormatInfo = value; } }
		public Guid GuidSView { get { return guidSView; } set { guidSView = value; } }
		public int IdEntry { get; set; }
		public bool HideArrow { get; set; }
		public XlsFilterFormatType FilterFormatType { get; set; }
		public XlsCustomFilterType CustomFilterType { get; set; }
		public bool IsWorksheetAutoFilter { get; set; }
		public int TableId { get { return tableId; } set { tableId = value; } }
		public XlsIconSetType IconType { get { return iconSetType; } set { iconSetType = value; } }
		public int IconNumber { get { return iconNumber; } set { iconNumber = value; } }
		public bool IsTableRecord { get { return TableId != -1; } }
		#endregion
		#region Read & Write
		public void Read(BinaryReader reader) {
			IdEntry = reader.ReadInt16();
			HideArrow = Convert.ToBoolean(reader.ReadInt32());
			FilterFormatType = (XlsFilterFormatType)reader.ReadInt32();
			CustomFilterType = (XlsCustomFilterType)reader.ReadInt32();
			int criteriaCount = reader.ReadInt32();
			int dateGroupingsCount = reader.ReadInt32();
			IsWorksheetAutoFilter = reader.ReadUInt16() == 0x0008;
			reader.ReadInt32(); 
			TableId = reader.ReadInt32();
			byte[] guidBytes = reader.ReadBytes(16);
			GuidSView = new Guid(guidBytes);
			ReadCellFormat(reader);
			ReadCriteriaAndGroupings(reader, criteriaCount, dateGroupingsCount);
		}
		void ReadCellFormat(BinaryReader reader) {
			if (FilterFormatType == XlsFilterFormatType.None)
				return;
			if (FilterFormatType == XlsFilterFormatType.CellColor || FilterFormatType == XlsFilterFormatType.CellFont)
				cellFormatInfo = DxfN12ListInfo.FromStream(reader, size - fixedSize);
			else {
				IconType = (XlsIconSetType)reader.ReadInt32();
				IconNumber = reader.ReadInt32();
			}
		}
		void ReadCriteriaAndGroupings(BinaryReader reader, int criteriaCount, int dateGroupingsCount) {
			if (FilterFormatType != XlsFilterFormatType.None)
				return;
			for (int i = 0; i < criteriaCount; i++)
				criteriaList.Add(XlsAutoFilterCriteria.FromStream(reader, true));
			for (int i = 0; i < dateGroupingsCount; i++)
				dateGroupingsList.Add(XlsAutoFilterDateInfo.FromStream(reader));
		}
		public void Write(BinaryWriter writer) {
			FutureRecordHeaderRef header = FutureRecordHeaderRef.Create(cellRangeInfo, typeCode);
			header.Write(writer);
			writer.Write((ushort)IdEntry);
			writer.Write(HideArrow ? 0x0001 : 0x0000);
			writer.Write((uint)FilterFormatType);
			writer.Write((uint)CustomFilterType);
			writer.Write(CriteriaList.Count);
			writer.Write(DateGroupingsList.Count);
			writer.Write((ushort)(IsWorksheetAutoFilter ? 0x0008 : 0x0000));
			writer.Write(0); 
			writer.Write(TableId);
			writer.Write(GuidSView.ToByteArray());
			WriteCellFormat(writer);
			WriteCriteriaAndGroupings(writer);
		}
		void WriteCellFormat(BinaryWriter writer) {
			if (FilterFormatType == XlsFilterFormatType.None)
				return;
			if (FilterFormatType == XlsFilterFormatType.CellColor || FilterFormatType == XlsFilterFormatType.CellFont) {
				Debug.Assert(cellFormatInfo != null);
				cellFormatInfo.Write(writer);
			}
			else {
				writer.Write((int)IconType);
				writer.Write(IconNumber);
			}
		}
		void WriteCriteriaAndGroupings(BinaryWriter writer) {
			if (FilterFormatType != XlsFilterFormatType.None)
				return;
			int criteriaCount = CriteriaList.Count;
			for (int i = 0; i < criteriaCount; i++) {
				writer.Flush();
				criteriaList[i].Write(writer);
			}
			int dateGroupingsCount = DateGroupingsList.Count;
			for (int i = 0; i < dateGroupingsCount; i++) {
				writer.Flush();
				dateGroupingsList[i].Write(writer);
			}
		}
		#endregion
		#region ApplyContent
		internal void ApplyContent(XlsContentBuilder contentBuilder) {
			AutoFilterColumnCollection filterColumns;
			if (IsTableRecord)
				filterColumns = contentBuilder.ActiveTable.AutoFilter.FilterColumns;
			else
				filterColumns = contentBuilder.CurrentSheet.AutoFilter.FilterColumns;
			if (IdEntry < 0 || IdEntry > filterColumns.Count - 1)
				return;
			SetupColumnProperties(filterColumns[IdEntry], contentBuilder);
		}
		void SetupColumnProperties(AutoFilterColumn filterColumn, XlsContentBuilder contentBuilder) {
			Guard.ArgumentNotNull(filterColumn, "filterColumn");
			DynamicFilterType dynamicFilterType = DynamicFilterTypeTable[CustomFilterType];
			int criteriaCount = CriteriaList.Count;
			filterColumn.BeginUpdate();
			try {
				filterColumn.ColumnId = IdEntry;
				filterColumn.HiddenAutoFilterButton = HideArrow;
				filterColumn.FilterByCellFill = FilterFormatType != XlsFilterFormatType.CellFont;
				filterColumn.DynamicFilterType = dynamicFilterType;
				if (DynamicFilterTypesWithValues.Contains(dynamicFilterType) && criteriaCount > 0) {
					filterColumn.DynamicMinValue = GetDynamicFilterValue(CriteriaList[0]);
					if (criteriaCount > 1)
						filterColumn.DynamicMaxValue = GetDynamicFilterValue(CriteriaList[1]);
				}
				if (FilterFormatType == XlsFilterFormatType.CellIcon) {
					filterColumn.IconId = IconNumber;
					filterColumn.IconSetType = GetIconSetType();
				}
			}
			finally {
				filterColumn.EndUpdate();
			}
			if (dynamicFilterType == DynamicFilterType.Null)
				SetupFilterFormatProperties(filterColumn, contentBuilder);
		}
		IconSetType GetIconSetType() {
			IconSetType result = IconSetTypeTable[IconType];
			if (IconNumber == -1 && result == IconSetType.None)
				return IconSetType.Arrows3;
			return result;
		}
		double GetDynamicFilterValue(XlsAutoFilterCriteria criteria) {
			AutoFilterDataOperationValueXnum xnum = criteria.DataOperation.ComparisonValue as AutoFilterDataOperationValueXnum;
			Guard.ArgumentNotNull(xnum, "xnum");
			return xnum.Value;
		}
		void SetupFilterFormatProperties(AutoFilterColumn filterColumn, XlsContentBuilder contentBuilder) {
			if (FilterFormatType == XlsFilterFormatType.None)
				SetupFilterCriteria(filterColumn.FilterCriteria, filterColumn.Sheet);
			else if (FilterFormatType == XlsFilterFormatType.CellFont || FilterFormatType == XlsFilterFormatType.CellColor)
				AssignDifferentialFormat(filterColumn, contentBuilder);
		}
		void SetupFilterCriteria(FilterCriteria filterCriteria, Worksheet sheet) {
			int criteriaCount = CriteriaList.Count;
			for (int i = 0; i < criteriaCount; i++)
				criteriaList[i].SetupFilterCriteria(filterCriteria, sheet.Culture);
			int dateGroupingsCount = DateGroupingsList.Count;
			for (int i = 0; i < dateGroupingsCount; i++) {
				DateGrouping dateGrouping = new DateGrouping(sheet);
				dateGroupingsList[i].SetupDateGrouping(dateGrouping);
				filterCriteria.DateGroupings.Add(dateGrouping);
			}
		}
		void AssignDifferentialFormat(AutoFilterColumn filterColumn, XlsContentBuilder contentBuilder) {
			Debug.Assert(cellFormatInfo != null);
			DifferentialFormat format = cellFormatInfo.GetDifferentialFormat(contentBuilder);
			int formatIndex = contentBuilder.DocumentModel.Cache.CellFormatCache.AddItem(format);
			filterColumn.AssignFormatIndex(formatIndex);
		}
		#endregion
		#region AssignThisProperties
		void AssignThisProperties(AutoFilterColumn filterColumn, int tableId) {
			TableId = tableId;
			AssignThisProperties(filterColumn);
		}
		void AssignThisProperties(AutoFilterColumn filterColumn) {
			IdEntry = filterColumn.ColumnId;
			HideArrow = filterColumn.HiddenAutoFilterButton;
			IsWorksheetAutoFilter = !IsTableRecord;
			DynamicFilterType dynamicFilterType = filterColumn.DynamicFilterType;
			CustomFilterType = GetCustomFilterType(dynamicFilterType);
			if (dynamicFilterType == DynamicFilterType.Null) {
				AssignFromFilterCriteria(filterColumn.FilterCriteria);
				AssignFormats(filterColumn);
			}
			else if (DynamicFilterTypesWithValues.Contains(dynamicFilterType))
				AssignDynamicValues(filterColumn);
		}
		void AssignFromFilterCriteria(FilterCriteria filterCriteria) {
			FilterCollection filters = filterCriteria.Filters;
			int count = filters.Count;
			for (int i = 0; i < count; i++) {
				string stringValue = filters[i];
				Debug.Assert(!String.IsNullOrEmpty(stringValue));
				XlsAutoFilterCriteria criteria = new XlsAutoFilterCriteria();
				criteria.ForAutoFilter12 = true;
				criteria.ComparisonStringValue = stringValue;
				criteria.DataOperation.FilterComparisonOperator = XlsFilterComparisonOperator.Equal;
				criteriaList.Add(criteria);
			}
			if (filterCriteria.FilterByBlank) {
				XlsAutoFilterCriteria criteria = new XlsAutoFilterCriteria();
				criteria.DataOperation.ComparisonValue = new AutoFilterDataOperationValueBlank();
				criteria.DataOperation.FilterComparisonOperator = XlsFilterComparisonOperator.Equal;
				criteriaList.Add(criteria);
			}
			AssignFromDateGroupings(filterCriteria.DateGroupings);
		}
		void AssignFromDateGroupings(DateGroupingCollection dateGroupings) {
			int count = dateGroupings.Count;
			for (int i = 0; i < count; i++) {
				XlsAutoFilterDateInfo dateInfo = new XlsAutoFilterDateInfo();
				dateInfo.AssignFromGrouping(dateGroupings[i]);
				dateGroupingsList.Add(dateInfo);
			}
		}
		void AssignFormats(AutoFilterColumn filterColumn) {
			IconSetType iconType = filterColumn.IconSetType;
			if (filterColumn.FormatIndex != CellFormatCache.DefaultDifferentialFormatIndex) {
				cellFormatInfo = DxfN12ListInfo.FromDifferentialFormat(filterColumn.FormatInfo);
				FilterFormatType = filterColumn.FilterByCellFill ? XlsFilterFormatType.CellColor : XlsFilterFormatType.CellFont;
			}
			else if (iconType != IconSetType.None) {
				IconNumber = filterColumn.IconId;
				IconType = GetIconSetType(iconType);
				FilterFormatType = XlsFilterFormatType.CellIcon;
			}
		}
		void AssignDynamicValues(AutoFilterColumn filterColumn) {
			AutoFilterColumnInfo defaultInfo = filterColumn.Sheet.Workbook.Cache.AutoFilterColumnInfoCache.DefaultItem;
			AssignDynamicValue(filterColumn.DynamicMinValue, defaultInfo.DynamicMinValue, true);
			AssignDynamicValue(filterColumn.DynamicMaxValue, defaultInfo.DynamicMaxValue, false);
		}
		void AssignDynamicValue(double value, double defaultValue, bool isMin) {
			if (value == defaultValue)
				return;
			XlsAutoFilterCriteria criteria = new XlsAutoFilterCriteria();
			criteria.DataOperation.ComparisonValue = AutoFilterDataOperationValueXnum.Create(value);
			criteria.DataOperation.FilterComparisonOperator = GetComparisonOperator(isMin);
			criteriaList.Add(criteria);
		}
		XlsFilterComparisonOperator GetComparisonOperator(bool isMin) {
			if (CustomFilterType == XlsCustomFilterType.AboveAverage)
				return XlsFilterComparisonOperator.GreaterThan;
			if (CustomFilterType == XlsCustomFilterType.BelowAverage)
				return XlsFilterComparisonOperator.LessThan;
			return isMin ? XlsFilterComparisonOperator.GreaterThanOrEqual : XlsFilterComparisonOperator.LessThan;
		}
		XlsCustomFilterType GetCustomFilterType(DynamicFilterType dynamicFilterType) {
			foreach (XlsCustomFilterType key in DynamicFilterTypeTable.Keys)
				if (DynamicFilterTypeTable[key] == dynamicFilterType)
					return key;
			return XlsCustomFilterType.None;
		}
		XlsIconSetType GetIconSetType(IconSetType iconType) {
			if (IconNumber == -1 && iconType == IconSetType.Arrows3)
				return XlsIconSetType.None;
			foreach (XlsIconSetType key in IconSetTypeTable.Keys)
				if (IconSetTypeTable[key] == iconType)
					return key;
			return XlsIconSetType.None;
		}
		#endregion
		public int GetSize() {
			int variableSize = 0;
			if (FilterFormatType == XlsFilterFormatType.None) {
				for (int i = 0; i < CriteriaList.Count; i++)
					variableSize += CriteriaList[i].GetSize();
				variableSize += DateGroupingsList.Count * XlsAutoFilterDateInfo.Size;
			}
			else if (FilterFormatType == XlsFilterFormatType.CellIcon)
				variableSize = 8;
			else if (cellFormatInfo != null)
				variableSize = cellFormatInfo.GetSize();
			return fixedSize + variableSize;
		}
	}
	#endregion
	#region XlsAutoFilterDateInfo
	public class XlsAutoFilterDateInfo {
		#region Static Members
		public static short Size = 24;
		public static XlsAutoFilterDateInfo FromStream(BinaryReader reader) {
			XlsAutoFilterDateInfo result = new XlsAutoFilterDateInfo();
			result.Read(reader);
			return result;
		}
		#endregion
		#region Properties
		public int Year { get; set; }
		public int Month { get; set; }
		public int Day { get; set; }
		public int Hour { get; set; }
		public int Minute { get; set; }
		public int Second { get; set; }
		public XlsDateFilterBy FilterBy { get; set; }
		#endregion
		#region Read & Write
		public void Read(BinaryReader reader) {
			Year = reader.ReadUInt16();
			Month = reader.ReadUInt16();
			Day = (int)reader.ReadUInt32();
			Hour = reader.ReadUInt16();
			Minute = reader.ReadUInt16();
			Second = reader.ReadUInt16();
			reader.ReadUInt16(); 
			reader.ReadUInt32(); 
			FilterBy = (XlsDateFilterBy)reader.ReadUInt32();
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)Year);
			writer.Write((ushort)Month);
			writer.Write(Day);
			writer.Write((ushort)Hour);
			writer.Write((ushort)Minute);
			writer.Write((ushort)Second);
			writer.Write((ushort)0);
			writer.Write(0);
			writer.Write((int)FilterBy);
		}
		#endregion
		#region Import & Export
		internal void SetupDateGrouping(DateGrouping dateGrouping) {
			dateGrouping.BeginUpdate();
			try {
				dateGrouping.Year = Year;
				dateGrouping.Month = Month;
				dateGrouping.Day = Day;
				dateGrouping.Hour = Hour == 0 ? -1 : Hour;
				dateGrouping.Minute = Minute == 0 ? -1 : Minute;
				dateGrouping.Second = Second == 0 ? -1 : Second;
				dateGrouping.DateTimeGrouping = ConvertToDateTimeGrouping();
			}
			finally {
				dateGrouping.EndUpdate();
			}
		}
		internal void AssignFromGrouping(DateGrouping grouping) {
			Year = grouping.Year;
			if (grouping.HasMonth)
				Month = grouping.Month;
			if (grouping.HasDay)
				Day = grouping.Day;
			if (grouping.HasHour)
				Hour = grouping.Hour;
			if (grouping.HasMinute)
				Minute = grouping.Minute;
			if (grouping.HasSecond)
				Second = grouping.Second;
			FilterBy = ConvertToFilterByType(grouping.DateTimeGrouping);
		}
		DateTimeGroupingType ConvertToDateTimeGrouping() {
			switch (FilterBy) {
				case XlsDateFilterBy.Year:
					return DateTimeGroupingType.Year;
				case XlsDateFilterBy.YearMonth:
					return DateTimeGroupingType.Month;
				case XlsDateFilterBy.YearMonthDay:
					return DateTimeGroupingType.Day;
				case XlsDateFilterBy.YearMonthDayHour:
					return DateTimeGroupingType.Hour;
				case XlsDateFilterBy.YearMonthDayHourMinute:
					return DateTimeGroupingType.Minute;
				case XlsDateFilterBy.YearMonthDayHourMinuteSecond:
					return DateTimeGroupingType.Second;
			}
			return DateTimeGroupingType.None;
		}
		XlsDateFilterBy ConvertToFilterByType(DateTimeGroupingType groupingType) {
			switch (groupingType) {
				case DateTimeGroupingType.Year:
					return XlsDateFilterBy.Year;
				case DateTimeGroupingType.Month:
					return XlsDateFilterBy.YearMonth;
				case DateTimeGroupingType.Day:
					return XlsDateFilterBy.YearMonthDay;
				case DateTimeGroupingType.Hour:
					return XlsDateFilterBy.YearMonthDayHour;
				case DateTimeGroupingType.Minute:
					return XlsDateFilterBy.YearMonthDayHourMinute;
				case DateTimeGroupingType.Second:
					return XlsDateFilterBy.YearMonthDayHourMinuteSecond;
			}
			return XlsDateFilterBy.Year;
		}
		#endregion
	}
	#endregion
}
