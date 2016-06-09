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
using System.Reflection;
using System.Text;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Drawing;
using DevExpress.Export.Xl;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing;
#if !SL
using System.Drawing;
using DevExpress.Office.DrawingML;
#else
using System.Windows.Media;
#endif
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region XlsCommandChartAxesUsed
	public class XlsCommandChartAxesUsed : XlsCommandBase {
		public bool HasSecondaryAxisGroup { get; set; }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			ushort value = reader.ReadUInt16();
			HasSecondaryAxisGroup = value != 0x0001;
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			contentBuilder.ChartHasSecondaryAxisGroup = HasSecondaryAxisGroup;
			contentBuilder.SetupSeriesFormats();
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)(HasSecondaryAxisGroup ? 0x0002 : 0x0001));
		}
		protected override short GetSize() {
			return 2;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsAxisGroupType
	public enum XlsAxisGroupType {
		Primary = 0,
		Secondary = 1
	}
	#endregion
	#region XlsCommandChartAxisParent
	public class XlsCommandChartAxisParent : XlsCommandBase {
		public XlsAxisGroupType Group { get; set; }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			Group = (XlsAxisGroupType)reader.ReadUInt16();
			int bytesToRead = Size - 2;
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			Chart chart = contentBuilder.CurrentChart;
			if (chart == null)
				return;
			contentBuilder.CurrentAxisGroup = Group == XlsAxisGroupType.Primary ? chart.PrimaryAxes : chart.SecondaryAxes;
			contentBuilder.PutChartBuilder(new XlsChartAxisGroupBuilder());
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)Group);
			writer.Write((int)0); 
			writer.Write((int)0); 
			writer.Write((int)0); 
			writer.Write((int)0); 
		}
		protected override short GetSize() {
			return 18;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartAxis
	public class XlsCommandChartAxis : XlsCommandBase {
		public AxisDataType AxisType { get; set; }
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			AxisType = (AxisDataType)reader.ReadUInt16();
			int bytesToRead = Size - 2;
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			if (contentBuilder.CurrentChart == null)
				return;
			XlsChartAxisBuilder chartBuilder = new XlsChartAxisBuilder();
			chartBuilder.Container = contentBuilder.CurrentChartBuilder as IXlsChartAxisContainer;
			chartBuilder.AxisType = AxisType;
			contentBuilder.PutChartBuilder(chartBuilder);
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)AxisType);
			writer.Write((int)0); 
			writer.Write((int)0); 
			writer.Write((int)0); 
			writer.Write((int)0); 
		}
		protected override short GetSize() {
			return 18;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartAxisCatSerRange
	public class XlsCommandChartAxisCatSerRange : XlsCommandBase {
		#region Fields
		const int minSkipValue = 1;
		const int maxSkipValue = 31999;
		int categoryCrossValue;
		int tickLabelSkip = 1;
		int tickMarkSkip = 1;
		#endregion
		#region Properties
		public int CategoryCrossValue {
			get { return categoryCrossValue; }
			set {
				ValueChecker.CheckValue(value, short.MinValue, ushort.MaxValue, "CategoryCrossValue");
				categoryCrossValue = value;
			}
		}
		public int TickLabelSkip {
			get { return tickLabelSkip; }
			set {
				ValueChecker.CheckValue(value, minSkipValue, maxSkipValue, "TickLabelSkip");
				tickLabelSkip = value;
			}
		}
		public int TickMarkSkip {
			get { return tickMarkSkip; }
			set {
				ValueChecker.CheckValue(value, minSkipValue, maxSkipValue, "TickMarkSkip");
				tickMarkSkip = value;
			}
		}
		public bool IsCrossBetween { get; set; }
		public bool IsMaxCross { get; set; }
		public bool Reverse { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			categoryCrossValue = reader.ReadUInt16();
			tickLabelSkip = reader.ReadInt16();
			tickMarkSkip = reader.ReadInt16();
			ushort bitwiseField = reader.ReadUInt16();
			IsCrossBetween = Convert.ToBoolean(bitwiseField & 0x0001);
			IsMaxCross = Convert.ToBoolean(bitwiseField & 0x0002);
			Reverse = Convert.ToBoolean(bitwiseField & 0x0004);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartAxisBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartAxisBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.CrossesValue = CategoryCrossValue;
			chartBuilder.TickLabelSkip = TickLabelSkip;
			chartBuilder.TickMarkSkip = TickMarkSkip;
			chartBuilder.Orientation = Reverse ? AxisOrientation.MaxMin : AxisOrientation.MinMax;
			chartBuilder.CrossBetween = IsCrossBetween ? AxisCrossBetween.Between : AxisCrossBetween.Midpoint;
			chartBuilder.Crosses = IsMaxCross ? AxisCrosses.Max : AxisCrosses.AtValue;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)categoryCrossValue);
			writer.Write((short)tickLabelSkip);
			writer.Write((short)tickMarkSkip);
			ushort bitwiseField = 0;
			if(IsCrossBetween)
				bitwiseField |= 0x0001;
			if(IsMaxCross)
				bitwiseField |= 0x0002;
			if(Reverse)
				bitwiseField |= 0x0004;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 8;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartAxisExt
	public class XlsCommandChartAxisExt : XlsCommandBase {
		#region Fields
		int minDate;
		int maxDate;
		int crossDate;
		int minorDateTick;
		int majorDateTick;
		#endregion
		#region Properties
		public bool AutoMin { get; set; }
		public bool AutoMax { get; set; }
		public bool AutoMajor { get; set; }
		public bool AutoMinor { get; set; }
		public bool IsDateAxis { get; set; }
		public bool AutoBase { get; set; }
		public bool AutoCross { get; set; }
		public bool Auto { get; set; }
		public int MinDate {
			get { return minDate; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "MinDate");
				minDate = value;
			}
		}
		public int MaxDate {
			get { return maxDate; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "MaxDate");
				maxDate = value;
			}
		}
		public int CrossDate {
			get { return crossDate; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "CrossDate");
				crossDate = value;
			}
		}
		public int MinorDateTick {
			get { return minorDateTick; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "MinorDateTick");
				minorDateTick = value;
			}
		}
		public int MajorDateTick {
			get { return majorDateTick; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "MajorDateTick");
				majorDateTick = value;
			}
		}
		public TimeUnits BaseTimeUnit { get; set; }
		public TimeUnits MajorTimeUnit { get; set; }
		public TimeUnits MinorTimeUnit { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			minDate = reader.ReadUInt16();
			maxDate = reader.ReadUInt16();
			majorDateTick = reader.ReadUInt16();
			MajorTimeUnit = (TimeUnits)reader.ReadUInt16();
			minorDateTick = reader.ReadUInt16();
			MinorTimeUnit = (TimeUnits)reader.ReadUInt16();
			BaseTimeUnit = (TimeUnits)reader.ReadUInt16();
			crossDate = reader.ReadUInt16();
			ushort bitwiseField = reader.ReadUInt16();
			AutoMin = Convert.ToBoolean(bitwiseField & 0x0001);
			AutoMax = Convert.ToBoolean(bitwiseField & 0x0002);
			AutoMajor = Convert.ToBoolean(bitwiseField & 0x0004);
			AutoMinor = Convert.ToBoolean(bitwiseField & 0x0008);
			IsDateAxis = Convert.ToBoolean(bitwiseField & 0x0010);
			AutoBase = Convert.ToBoolean(bitwiseField & 0x0020);
			AutoCross = Convert.ToBoolean(bitwiseField & 0x0040);
			Auto = Convert.ToBoolean(bitwiseField & 0x0080);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartAxisBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartAxisBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.Auto = Auto;
			chartBuilder.IsDateAxis = IsDateAxis;
			if (IsDateAxis) {
				chartBuilder.AutoMin = AutoMin;
				chartBuilder.AutoMax = AutoMax;
				chartBuilder.MinValue = ConvertDate(contentBuilder, MinDate, BaseTimeUnit);
				chartBuilder.MaxValue = ConvertDate(contentBuilder, MaxDate, BaseTimeUnit);
				if (AutoMajor) {
					chartBuilder.MajorUnit = 0;
					chartBuilder.MajorTimeUnit = TimeUnits.Auto;
				}
				else {
					chartBuilder.MajorUnit = MajorDateTick;
					chartBuilder.MajorTimeUnit = MajorTimeUnit;
				}
				if (AutoMinor) {
					chartBuilder.MinorUnit = 0;
					chartBuilder.MinorTimeUnit = TimeUnits.Auto;
				}
				else {
					chartBuilder.MinorUnit = MinorDateTick;
					chartBuilder.MinorTimeUnit = MinorTimeUnit;
				}
				chartBuilder.BaseTimeUnit = AutoBase ? TimeUnits.Auto : BaseTimeUnit;
				if (!AutoCross) {
					chartBuilder.CrossesValue = ConvertDate(contentBuilder, CrossDate, BaseTimeUnit);
					chartBuilder.Crosses = AxisCrosses.AtValue;
				}
				else if(chartBuilder.Crosses != AxisCrosses.Max)
					chartBuilder.Crosses = AxisCrosses.AutoZero;
			}
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write((ushort)(minDate));
			writer.Write((ushort)(maxDate));
			writer.Write((ushort)(majorDateTick));
			writer.Write((ushort)(MajorTimeUnit));
			writer.Write((ushort)(minorDateTick));
			writer.Write((ushort)(MinorTimeUnit));
			writer.Write((ushort)(BaseTimeUnit));
			writer.Write((ushort)(crossDate));
			ushort bitwiseField = 0;
			if(AutoMin)
				bitwiseField |= 0x0001;
			if(AutoMax)
				bitwiseField |= 0x0002;
			if(AutoMajor)
				bitwiseField |= 0x0004;
			if(AutoMinor)
				bitwiseField |= 0x0008;
			if(IsDateAxis)
				bitwiseField |= 0x0010;
			if(AutoBase)
				bitwiseField |= 0x0020;
			if(AutoCross)
				bitwiseField |= 0x0040;
			if(Auto)
				bitwiseField |= 0x0080;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 18;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		double ConvertDate(XlsContentBuilder contentBuilder, int dateValue, TimeUnits timeUnits) {
			if (timeUnits != TimeUnits.Days) {
				WorkbookDataContext context = contentBuilder.DocumentModel.DataContext;
				DateTime date = context.FromDateTimeSerial(0);
				if (timeUnits == TimeUnits.Months)
					date = date.AddMonths(dateValue);
				else if (timeUnits == TimeUnits.Years)
					date = date.AddYears(dateValue);
				VariantValue value = context.FromDateTime(date);
				if (value.IsNumeric)
					return value.NumericValue;
			}
			return dateValue;
		}
	}
	#endregion
	#region XlsCommandChartAxisValueRange
	public class XlsCommandChartAxisValueRange : XlsCommandBase {
		#region Properties
		public double MinValue { get; set; }
		public double MaxValue { get; set; }
		public double MajorUnit { get; set; }
		public double MinorUnit { get; set; }
		public double CrossesValue { get; set; }
		public bool AutoMin { get; set; }
		public bool AutoMax { get; set; }
		public bool AutoMajor { get; set; }
		public bool AutoMinor { get; set; }
		public bool AutoCross { get; set; }
		public bool IsLogScale { get; set; }
		public bool Reversed { get; set; }
		public bool IsMaxCross { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			MinValue = reader.ReadDouble();
			MaxValue = reader.ReadDouble();
			MajorUnit = reader.ReadDouble();
			MinorUnit = reader.ReadDouble();
			CrossesValue = reader.ReadDouble();
			ushort bitwiseField = reader.ReadUInt16();
			AutoMin = Convert.ToBoolean(bitwiseField & 0x0001);
			AutoMax = Convert.ToBoolean(bitwiseField & 0x0002);
			AutoMajor = Convert.ToBoolean(bitwiseField & 0x0004);
			AutoMinor = Convert.ToBoolean(bitwiseField & 0x0008);
			AutoCross = Convert.ToBoolean(bitwiseField & 0x0010);
			IsLogScale = Convert.ToBoolean(bitwiseField & 0x0020);
			Reversed = Convert.ToBoolean(bitwiseField & 0x0040);
			IsMaxCross = Convert.ToBoolean(bitwiseField & 0x0080);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartAxisBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartAxisBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.AutoMin = AutoMin;
			chartBuilder.AutoMax = AutoMax;
			chartBuilder.MinValue = AutoMin ? 0.0 : ConvertLog10Value(MinValue);
			chartBuilder.MaxValue = AutoMax ? 0.0 : ConvertLog10Value(MaxValue);
			chartBuilder.MajorUnit = AutoMajor ? 0.0 : ConvertLog10Value(MajorUnit);
			chartBuilder.MinorUnit = AutoMinor ? 0.0 : ConvertLog10Value(MinorUnit);
			chartBuilder.CrossesValue = (IsMaxCross || AutoCross) ? 0.0 : ConvertLog10Value(CrossesValue);
			if (IsMaxCross)
				chartBuilder.Crosses = AxisCrosses.Max;
			else if (AutoCross)
				chartBuilder.Crosses = AxisCrosses.AutoZero;
			else
				chartBuilder.Crosses = AxisCrosses.AtValue;
			chartBuilder.Orientation = Reversed ? AxisOrientation.MaxMin : AxisOrientation.MinMax;
			chartBuilder.IsLogScale = IsLogScale;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(MinValue);
			writer.Write(MaxValue);
			writer.Write(MajorUnit);
			writer.Write(MinorUnit);
			writer.Write(CrossesValue);
			ushort bitwiseField = 0;
			if(AutoMin)
				bitwiseField |= 0x0001;
			if(AutoMax)
				bitwiseField |= 0x0002;
			if(AutoMajor)
				bitwiseField |= 0x0004;
			if(AutoMinor)
				bitwiseField |= 0x0008;
			if(AutoCross)
				bitwiseField |= 0x0010;
			if(IsLogScale)
				bitwiseField |= 0x0020;
			if(Reversed)
				bitwiseField |= 0x0040;
			if(IsMaxCross)
				bitwiseField |= 0x0080;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 42;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		double ConvertLog10Value(double value) {
			if (!IsLogScale)
				return value;
			return Math.Pow(10.0, value);
		}
	}
	#endregion
	#region XlsCommandChartAxisCatLabels
	public class XlsCommandChartAxisCatLabels : XlsCommandBase {
		const short fixedPartSize = 10;
		int labelOffset;
		#region Properties
		public int LabelOffset {
			get { return labelOffset; }
			set {
				ValueChecker.CheckValue(value, 0, 1000, "LabelOffset");
				labelOffset = value;
			}
		}
		public LabelAlignment LabelAlign { get; set; }
		public bool AutoCatLabel { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeaderOld.FromStream(reader);
			labelOffset = reader.ReadUInt16();
			LabelAlign = CodeToAlign(reader.ReadUInt16());
			ushort bitwiseField = reader.ReadUInt16();
			AutoCatLabel = Convert.ToBoolean(bitwiseField & 0x0001);
			int bytesToRead = Size - fixedPartSize;
			if (bytesToRead > 0)
				reader.ReadBytes(bytesToRead);
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartAxisBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartAxisBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.LabelAlign = LabelAlign;
			chartBuilder.LabelOffset = LabelOffset;
			if (AutoCatLabel)
				chartBuilder.TickLabelSkip = 1;
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeaderOld header = new FutureRecordHeaderOld();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(GetType());
			header.Write(writer);
			writer.Write((ushort)labelOffset);
			writer.Write(AlignToCode(LabelAlign));
			ushort bitwiseField = 0;
			if(AutoCatLabel)
				bitwiseField |= 0x0001;
			writer.Write(bitwiseField);
			writer.Write((ushort)0); 
		}
		protected override short GetSize() {
			return fixedPartSize + 2;
		}
		LabelAlignment CodeToAlign(int code) {
			if (code == 0x0001)
				return LabelAlignment.Left;
			if (code == 0x0002)
				return LabelAlignment.Center;
			return LabelAlignment.Right;
		}
		short AlignToCode(LabelAlignment align) {
			if (align == LabelAlignment.Left)
				return 1;
			if (align == LabelAlignment.Center)
				return 2;
			return 3;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartAxisNumberFormat
	public class XlsCommandChartAxisNumberFormat : XlsCommandShortPropertyValueBase {
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartAxisBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartAxisBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.NumberFormatId = contentBuilder.StyleSheet.GetNumberFormatIndex(Value);
			chartBuilder.ApplyNumberFormat = true;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartAxisTick
	public class XlsCommandChartAxisTick : XlsCommandBase {
		#region Properties
		public TickMark MajorTickMark { get; set; }
		public TickMark MinorTickMark { get; set; }
		public TickLabelPosition TickLabelPos { get; set; }
		public bool IsTransparent { get; set; }
		public Color TextColor { get; set; }
		public bool AutoColor { get; set; }
		public bool AutoMode { get; set; }
		public DrawingTextVerticalTextType VerticalText { get; set; }
		public int RotationAngle { get; set; }
		public bool AutoRotate { get; set; }
		public int TextColorIndex { get; set; }
		public XlReadingOrder TextReadingOrder { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			MajorTickMark = CodeToTickMark(reader.ReadByte());
			MinorTickMark = CodeToTickMark(reader.ReadByte());
			TickLabelPos = CodeToTickLabelPos(reader.ReadByte());
			IsTransparent = reader.ReadByte() == 0x01;
			TextColor = XlsLongRGB.FromStream(reader);
			reader.ReadInt32(); 
			reader.ReadInt32(); 
			reader.ReadInt32(); 
			reader.ReadInt32(); 
			ushort bitwiseField = reader.ReadUInt16();
			AutoColor = (bitwiseField & 0x0001) != 0;
			AutoMode = (bitwiseField & 0x0002) != 0;
			int rot = (bitwiseField & 0x001c) >> 2;
			AutoRotate = (bitwiseField & 0x0020) != 0;
			TextReadingOrder = (XlReadingOrder)((bitwiseField & 0xc000) >> 14);
			TextColorIndex = reader.ReadUInt16();
			ushort trot = reader.ReadUInt16();
			switch (rot) {
				case 0:
					VerticalText = XlsChartTextRotationHelper.GetVerticalTextType(trot);
					RotationAngle = XlsChartTextRotationHelper.GetRotationAngle(trot);
					break;
				case 1:
					VerticalText = DrawingTextVerticalTextType.WordArtVertical;
					RotationAngle = 0;
					break;
				case 2:
					VerticalText = DrawingTextVerticalTextType.Horizontal;
					RotationAngle = -5400000;
					break;
				case 3:
					VerticalText = DrawingTextVerticalTextType.Horizontal;
					RotationAngle = 5400000;
					break;
			}
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartAxisBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartAxisBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.MajorTickMark = MajorTickMark;
			chartBuilder.MinorTickMark = MinorTickMark;
			chartBuilder.LabelPos = TickLabelPos;
			chartBuilder.LabelTransparent = IsTransparent;
			chartBuilder.TextColor = TextColor;
			chartBuilder.LabelAutoColor = AutoColor;
			chartBuilder.LabelAutoMode = AutoMode;
			chartBuilder.LabelVerticalText = VerticalText;
			chartBuilder.LabelRotationAngle = RotationAngle;
			chartBuilder.LabelAutoRotate = AutoRotate;
			chartBuilder.TextReadingOrder = TextReadingOrder;
		}
		protected override void WriteCore(BinaryWriter writer) {
			writer.Write(TickMarkToCode(MajorTickMark));
			writer.Write(TickMarkToCode(MinorTickMark));
			writer.Write(TickLabelPosToCode(TickLabelPos));
			writer.Write((byte)(IsTransparent ? 0x01 : 0x02));
			XlsLongRGB.Write(writer, TextColor);
			writer.Write((int)0); 
			writer.Write((int)0); 
			writer.Write((int)0); 
			writer.Write((int)0); 
			ushort bitwiseField = (ushort)((GetRot() << 2) + ((int)TextReadingOrder << 14));
			if(AutoColor)
				bitwiseField |= 0x0001;
			if(AutoMode)
				bitwiseField |= 0x0002;
			if(AutoRotate)
				bitwiseField |= 0x0020;
			writer.Write(bitwiseField);
			writer.Write((ushort)TextColorIndex);
			writer.Write((ushort)XlsChartTextRotationHelper.GetTrot(VerticalText, RotationAngle));
		}
		protected override short GetSize() {
			return 30;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		#region Utils
		TickMark CodeToTickMark(byte code) {
			switch (code) {
				case 0x01: return TickMark.Inside;
				case 0x02: return TickMark.Outside;
				case 0x03: return TickMark.Cross;
			}
			return TickMark.None;
		}
		byte TickMarkToCode(TickMark tickMark) {
			switch (tickMark) {
				case TickMark.Inside: return 1;
				case TickMark.Outside: return 2;
				case TickMark.Cross: return 3;
			}
			return 0;
		}
		TickLabelPosition CodeToTickLabelPos(byte code) {
			switch (code) {
				case 0x01: return TickLabelPosition.Low;
				case 0x02: return TickLabelPosition.High;
				case 0x03: return TickLabelPosition.NextTo;
			}
			return TickLabelPosition.None;
		}
		byte TickLabelPosToCode(TickLabelPosition tickLabelPos) {
			switch (tickLabelPos) {
				case TickLabelPosition.Low: return 1;
				case TickLabelPosition.High: return 2;
				case TickLabelPosition.NextTo: return 3;
			}
			return 0;
		}
		int GetRot() {
			if (VerticalText == DrawingTextVerticalTextType.WordArtVertical)
				return 0x01;
			if (RotationAngle == -5400000)
				return 0x02;
			if (RotationAngle == 5400000)
				return 0x03;
			return 0x00;
		}
		#endregion
	}
	#endregion
	#region XlsCommandChartAxisLine
	public class XlsCommandChartAxisLine : XlsCommandShortPropertyValueBase {
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartAxisBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartAxisBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.LineFormatIndex = Value;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
	}
	#endregion
	#region XlsCommandChartAxisYMult
	public class XlsCommandChartAxisYMult : XlsCommandBase {
		#region Properties
		public DisplayUnitType UnitType { get; set; }
		public double CustomUnit { get; set; }
		public bool ShowLabel { get; set; }
		public bool BeingEditted { get; set; }
		#endregion
		protected override void ReadCore(XlsReader reader, XlsContentBuilder contentBuilder) {
			FutureRecordHeaderOld.FromStream(reader);
			UnitType = CodeToUnitType(reader.ReadUInt16());
			CustomUnit = reader.ReadDouble();
			ushort bitwiseField = reader.ReadUInt16();
			ShowLabel = (bitwiseField & 0x0002) != 0;
			BeingEditted = (bitwiseField & 0x0004) != 0;
		}
		protected override void ApplyChartContent(XlsContentBuilder contentBuilder) {
			XlsChartAxisBuilder chartBuilder = contentBuilder.CurrentChartBuilder as XlsChartAxisBuilder;
			if (chartBuilder == null)
				return;
			chartBuilder.UnitType = UnitType;
			chartBuilder.CustomUnit = CustomUnit;
			chartBuilder.ShowUnitLabel = ShowLabel;
		}
		protected override void WriteCore(BinaryWriter writer) {
			FutureRecordHeaderOld header = new FutureRecordHeaderOld();
			header.RecordTypeId = XlsCommandFactory.GetTypeCodeByType(GetType());
			header.Write(writer);
			writer.Write(UnitTypeToCode(UnitType));
			writer.Write(CustomUnit);
			ushort bitwiseField = 0x0001;
			if (ShowLabel)
				bitwiseField |= 0x0002;
			if (BeingEditted)
				bitwiseField |= 0x0004;
			writer.Write(bitwiseField);
		}
		protected override short GetSize() {
			return 16;
		}
		public override IXlsCommand GetInstance() {
			return this;
		}
		DisplayUnitType CodeToUnitType(int code) {
			switch(code) {
				case 0xffff: return DisplayUnitType.Custom;
				case 0x0001: return DisplayUnitType.Hundreds;
				case 0x0002: return DisplayUnitType.Thousands;
				case 0x0003: return DisplayUnitType.TenThousands;
				case 0x0004: return DisplayUnitType.HundredThousands;
				case 0x0005: return DisplayUnitType.Millions;
				case 0x0006: return DisplayUnitType.TenMillions;
				case 0x0007: return DisplayUnitType.HundredMillions;
				case 0x0008: return DisplayUnitType.Billions;
				case 0x0009: return DisplayUnitType.Trillions;
			}
			return DisplayUnitType.None;
		}
		ushort UnitTypeToCode(DisplayUnitType unitType) {
			switch (unitType) {
				case DisplayUnitType.Custom: return 0xffff;
				case DisplayUnitType.Hundreds: return 0x0001;
				case DisplayUnitType.Thousands: return 0x0002;
				case DisplayUnitType.TenThousands: return 0x0003;
				case DisplayUnitType.HundredThousands: return 0x0004;
				case DisplayUnitType.Millions: return 0x0005;
				case DisplayUnitType.TenMillions: return 0x0006;
				case DisplayUnitType.HundredMillions: return 0x0007;
				case DisplayUnitType.Billions: return 0x0008;
				case DisplayUnitType.Trillions: return 0x0009;
			}
			return 0x0000;
		}
	}
	#endregion
}
