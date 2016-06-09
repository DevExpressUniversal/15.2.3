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
using System.Drawing;
using System.IO;
using System.Text;
using System.Runtime.InteropServices;
using System.Globalization;
using DevExpress.Office;
using DevExpress.Utils;
using DevExpress.XtraSpreadsheet.Model;
using DevExpress.XtraSpreadsheet.Utils;
using DevExpress.Office.Utils;
using DevExpress.XtraSpreadsheet.Internal;
using DevExpress.XtraSpreadsheet.Export.Xls;
using DevExpress.XtraSpreadsheet.Model.External;
using DevExpress.XtraExport.Xls;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Office.Model;
namespace DevExpress.XtraSpreadsheet.Import.Xls {
	#region IXlsCFRuleTemplateContainer
	public interface IXlsCFRuleTemplateContainer {
		XlsCFRuleTemplate RuleTemplate { get; set; }
		bool FilterTop { get; set; }
		bool FilterPercent { get; set; }
		int FilterValue { get; set; }
		ConditionalFormattingTextCondition TextRule { get; set; }
		int StdDev { get; set; }
	}
	#endregion
	#region XlsCFHelper
	static class XlsCFHelper {
		enum XColorType {
			Auto = 0x0000,
			Indexed = 0x0001,
			Rgb = 0x00002,
			Themed = 0x0003,
			Ninched = 0x0004
		}
		static readonly ushort[] dateTemplateValues = new ushort[] { 0, 6, 1, 2, 5, 8, 3, 7, 4, 9 };
		static readonly ColorModelInfo defaultColorInfo = new ColorModelInfo();
		#region Code/Type/Code translation
		public static ConditionalFormattingRuleType ConditionRuleCodeToType(byte code) {
			switch(code) {
				case 0x01: return ConditionalFormattingRuleType.CompareWithFormulaResult;
				case 0x02: return ConditionalFormattingRuleType.ExpressionIsTrue;
				case 0x03: return ConditionalFormattingRuleType.ColorScale;
				case 0x04: return ConditionalFormattingRuleType.DataBar;
				case 0x05: return ConditionalFormattingRuleType.TopOrBottomValue;
				case 0x06: return ConditionalFormattingRuleType.IconSet;
			}
			return ConditionalFormattingRuleType.Unknown;
		}
		public static byte ConditionRuleTypeToCode(ConditionalFormattingRuleType ruleType) {
			switch(ruleType) {
				case ConditionalFormattingRuleType.CompareWithFormulaResult: return 0x01;
				case ConditionalFormattingRuleType.ExpressionIsTrue: return 0x02;
				case ConditionalFormattingRuleType.ColorScale: return 0x03;
				case ConditionalFormattingRuleType.DataBar: return 0x04;
				case ConditionalFormattingRuleType.TopOrBottomValue: return 0x05;
				case ConditionalFormattingRuleType.IconSet: return 0x06;
			}
			return 0x00;
		}
		public static ConditionalFormattingOperator ComparisonFunctionCodeToType(byte code) {
			switch(code) {
				case 0x01: return ConditionalFormattingOperator.Between;
				case 0x02: return ConditionalFormattingOperator.NotBetween;
				case 0x03: return ConditionalFormattingOperator.Equal;
				case 0x04: return ConditionalFormattingOperator.NotEqual;
				case 0x05: return ConditionalFormattingOperator.GreaterThan;
				case 0x06: return ConditionalFormattingOperator.LessThan;
				case 0x07: return ConditionalFormattingOperator.GreaterThanOrEqual;
				case 0x08: return ConditionalFormattingOperator.LessThanOrEqual;
			}
			return ConditionalFormattingOperator.Unknown;
		}
		public static byte ComparisonFunctionTypeToCode(ConditionalFormattingOperator oper) {
			switch(oper) {
				case ConditionalFormattingOperator.Between: return 0x01;
				case ConditionalFormattingOperator.NotBetween: return 0x02;
				case ConditionalFormattingOperator.Equal: return 0x03;
				case ConditionalFormattingOperator.NotEqual: return 0x04;
				case ConditionalFormattingOperator.GreaterThan: return 0x05;
				case ConditionalFormattingOperator.LessThan: return 0x06;
				case ConditionalFormattingOperator.GreaterThanOrEqual: return 0x07;
				case ConditionalFormattingOperator.LessThanOrEqual: return 0x08;
			}
			return 0x00;
		}
		public static ConditionalFormattingValueObjectType CFValueObjectCodeToType(byte code) {
			switch(code) {
				case 0x01: return ConditionalFormattingValueObjectType.Num;
				case 0x02: return ConditionalFormattingValueObjectType.Min;
				case 0x03: return ConditionalFormattingValueObjectType.Max;
				case 0x04: return ConditionalFormattingValueObjectType.Percent;
				case 0x05: return ConditionalFormattingValueObjectType.Percentile;
				case 0x07: return ConditionalFormattingValueObjectType.Formula;
			}
			return ConditionalFormattingValueObjectType.Unknown;
		}
		public static byte CFValueObjectTypeToCode(ConditionalFormattingValueObjectType objectType) {
			switch(objectType) {
				case ConditionalFormattingValueObjectType.Num: return 0x01;
				case ConditionalFormattingValueObjectType.Min: return 0x02;
				case ConditionalFormattingValueObjectType.Max: return 0x03;
				case ConditionalFormattingValueObjectType.Percent: return 0x04;
				case ConditionalFormattingValueObjectType.Percentile: return 0x05;
				case ConditionalFormattingValueObjectType.Formula: return 0x07;
			}
			return 0x00;
		}
		public static IconSetType IconSetCodeToType(byte code) {
			switch(code) {
				case 0x00: return IconSetType.Arrows3;
				case 0x01: return IconSetType.ArrowsGray3;
				case 0x02: return IconSetType.Flags3;
				case 0x03: return IconSetType.TrafficLights13;
				case 0x04: return IconSetType.Signs3;
				case 0x05: return IconSetType.TrafficLights23;
				case 0x06: return IconSetType.Symbols3;
				case 0x07: return IconSetType.Symbols23;
				case 0x08: return IconSetType.Arrows4;
				case 0x09: return IconSetType.ArrowsGray4;
				case 0x0a: return IconSetType.RedToBlack4;
				case 0x0b: return IconSetType.Rating4;
				case 0x0c: return IconSetType.TrafficLights4;
				case 0x0d: return IconSetType.Arrows5;
				case 0x0e: return IconSetType.ArrowsGray5;
				case 0x0f: return IconSetType.Rating5;
				case 0x10: return IconSetType.Quarters5;
			}
			return IconSetType.None;
		}
		public static byte IconSetTypeToCode(IconSetType type) {
			switch(type) {
				case IconSetType.Arrows3: return 0x00;
				case IconSetType.ArrowsGray3: return 0x01;
				case IconSetType.Flags3: return 0x02;
				case IconSetType.TrafficLights13: return 0x03;
				case IconSetType.Signs3: return 0x04;
				case IconSetType.TrafficLights23: return 0x05;
				case IconSetType.Symbols3: return 0x06;
				case IconSetType.Symbols23: return 0x07;
				case IconSetType.Arrows4: return 0x08;
				case IconSetType.ArrowsGray4: return 0x09;
				case IconSetType.RedToBlack4: return 0x0a;
				case IconSetType.Rating4: return 0x0b;
				case IconSetType.TrafficLights4: return 0x0c;
				case IconSetType.Arrows5: return 0x0d;
				case IconSetType.ArrowsGray5: return 0x0e;
				case IconSetType.Rating5: return 0x0f;
				case IconSetType.Quarters5: return 0x10;
			}
			return 0xff;
		}
		public static bool IsSupportedIconSet(IconSetType type) {
			return IconSetTypeToCode(type) != 0xff;
		}
		public static ConditionalFormattingTextCondition TextRuleCodeToType(short code) {
			switch(code) {
				case 0x01: return ConditionalFormattingTextCondition.NotContains;
				case 0x02: return ConditionalFormattingTextCondition.BeginsWith;
				case 0x03: return ConditionalFormattingTextCondition.EndsWith;
			}
			return ConditionalFormattingTextCondition.Contains;
		}
		public static short TextRuleTypeToCode(ConditionalFormattingTextCondition type) {
			switch(type) {
				case ConditionalFormattingTextCondition.NotContains: return 0x01;
				case ConditionalFormattingTextCondition.BeginsWith: return 0x02;
				case ConditionalFormattingTextCondition.EndsWith: return 0x03;
			}
			return 0x00;
		}
		#endregion
		#region Formula
		public static ParsedExpression GetFormulaExpression(XlsRPNContext context, byte[] formulaBytes, int formulaSize) {
			ParsedExpression result;
			context.WorkbookContext.PushCurrentCell(0, 0);
			try {
				result = context.BinaryToExpression(formulaBytes, formulaSize);
			}
			finally {
				context.WorkbookContext.PopCurrentCell();
			}
			return XlsParsedThingConverter.ToModelExpression(result, context);
		}
		public static byte[] GetFormulaBytes(ParsedExpression parsedExpression, XlsRPNContext context, ref int formulaSize) {
			ParsedExpression expression = XlsParsedThingConverter.ToXlsExpression(parsedExpression, context);
			byte[] buf = context.ExpressionToBinary(expression);
			formulaSize = BitConverter.ToUInt16(buf, 0);
			byte[] formulaBytes = new byte[formulaSize];
			if(formulaSize > 0)
				Array.Copy(buf, 2, formulaBytes, 0, formulaSize);
			return formulaBytes;
		}
		#endregion
		#region Rule template params
		public static void ReadTemplateParams(XlsReader reader, IXlsCFRuleTemplateContainer target) {
			reader.ReadByte(); 
			switch(target.RuleTemplate) {
				case XlsCFRuleTemplate.Filter:
					byte bitwiseFiled = reader.ReadByte();
					target.FilterTop = Convert.ToBoolean(bitwiseFiled & 0x01);
					target.FilterPercent = Convert.ToBoolean(bitwiseFiled & 0x02);
					target.FilterValue = reader.ReadUInt16();
					reader.ReadBytes(13);
					break;
				case XlsCFRuleTemplate.ContainsText:
					target.TextRule = XlsCFHelper.TextRuleCodeToType(reader.ReadInt16());
					reader.ReadBytes(14);
					break;
				case XlsCFRuleTemplate.AboveAverage:
				case XlsCFRuleTemplate.BelowAverage:
				case XlsCFRuleTemplate.AboveOrEqualToAverage:
				case XlsCFRuleTemplate.BelowOrEqualToAverage:
					target.StdDev = reader.ReadUInt16();
					reader.ReadBytes(14);
					break;
				default:
					reader.ReadBytes(16);
					break;
			}
		}
		public static void WriteTemplateParams(BinaryWriter writer, IXlsCFRuleTemplateContainer source) {
			writer.Write((byte)16); 
			switch(source.RuleTemplate) {
				case XlsCFRuleTemplate.Filter:
					byte bitwiseField = 0;
					if(source.FilterTop)
						bitwiseField |= 0x01;
					if(source.FilterPercent)
						bitwiseField |= 0x02;
					writer.Write(bitwiseField);
					writer.Write((ushort)source.FilterValue);
					writer.Write(new byte[13]);
					break;
				case XlsCFRuleTemplate.ContainsText:
					writer.Write(XlsCFHelper.TextRuleTypeToCode(source.TextRule));
					writer.Write(new byte[14]);
					break;
				case XlsCFRuleTemplate.Today:
				case XlsCFRuleTemplate.Tomorrow:
				case XlsCFRuleTemplate.Yesterday:
				case XlsCFRuleTemplate.Last7Days:
				case XlsCFRuleTemplate.LastMonth:
				case XlsCFRuleTemplate.NextMonth:
				case XlsCFRuleTemplate.ThisWeek:
				case XlsCFRuleTemplate.NextWeek:
				case XlsCFRuleTemplate.LastWeek:
				case XlsCFRuleTemplate.ThisMonth:
					writer.Write(dateTemplateValues[(int)source.RuleTemplate - 15]);
					writer.Write(new byte[14]);
					break;
				case XlsCFRuleTemplate.AboveAverage:
				case XlsCFRuleTemplate.BelowAverage:
				case XlsCFRuleTemplate.AboveOrEqualToAverage:
				case XlsCFRuleTemplate.BelowOrEqualToAverage:
					writer.Write((ushort)source.StdDev);
					writer.Write(new byte[14]);
					break;
				default:
					writer.Write(new byte[16]);
					break;
			}
		}
		#endregion
		#region CFColor
		public static ColorModelInfo ReadColor(XlsReader reader) {
			ColorModelInfo result = new ColorModelInfo();
			XColorType colorType = (XColorType)reader.ReadInt32();
			switch(colorType) {
				case XColorType.Auto:
					result.Auto = true;
					reader.ReadInt32(); 
					reader.ReadDouble(); 
					break;
				case XColorType.Indexed:
					result.ColorIndex = reader.ReadInt32();
					reader.ReadDouble(); 
					break;
				case XColorType.Rgb:
					byte red = reader.ReadByte();
					byte green = reader.ReadByte();
					byte blue = reader.ReadByte();
					reader.ReadByte(); 
					result.Rgb = DXColor.FromArgb(0xff, red, green, blue);
					reader.ReadDouble(); 
					break;
				case XColorType.Themed:
					result.Theme = new ThemeColorIndex(reader.ReadInt32());
					result.Tint = reader.ReadDouble();
					break;
				default:
					reader.ReadInt32(); 
					reader.ReadDouble(); 
					break;
			}
			return result;
		}
		public static void WriteColor(BinaryWriter writer, ColorModelInfo colorInfo) {
			if(colorInfo.Equals(defaultColorInfo)) {
				writer.Write((int)XColorType.Ninched);
				writer.Write((int)0);
				writer.Write((double)0.0);
				return;
			}
			switch(colorInfo.ColorType) {
				case ColorType.Auto:
					writer.Write((int)XColorType.Auto);
					writer.Write((int)0);
					writer.Write((double)0.0);
					break;
				case ColorType.Index:
					writer.Write((int)XColorType.Indexed);
					writer.Write((int)colorInfo.ColorIndex);
					writer.Write((double)0.0);
					break;
				case ColorType.Rgb:
					writer.Write((int)XColorType.Rgb);
					Color color = colorInfo.Rgb;
					writer.Write(color.R);
					writer.Write(color.G);
					writer.Write(color.B);
					writer.Write((byte)0xff);
					writer.Write((double)0.0);
					break;
				case ColorType.Theme:
					writer.Write((int)XColorType.Themed);
					writer.Write(colorInfo.Theme.ToInt());
					writer.Write(colorInfo.Tint);
					break;
			}
		}
		public static ColorModelInfo GetCFColor(ColorModelInfo colorInfo, DocumentModel documentModel) {
			if(colorInfo == defaultColorInfo)
				return colorInfo;
			if(colorInfo.ColorType == ColorType.Auto || colorInfo.ColorType == ColorType.Rgb || colorInfo.ColorType == ColorType.Theme)
				return colorInfo;
			if(colorInfo.ColorType == ColorType.Index && colorInfo.ColorIndex >= Palette.BuiltInColorsCount && colorInfo.ColorIndex < Palette.DefaultForegroundColorIndex)
				return colorInfo;
			return ColorModelInfo.Create(colorInfo.ToRgb(documentModel.StyleSheet.Palette, documentModel.OfficeTheme.Colors));
		}
		#endregion
	}
	#endregion
	#region XlsCFValueObject
	public class XlsCFValueObject {
		#region Fields
		const int fixedPartSize = 3;
		byte[] formulaBytes = new byte[0];
		int formulaSize;
		#endregion
		#region Properties
		public ConditionalFormattingValueObjectType ObjectType { get; set; }
		public double Value { get; set; }
		#endregion
		public XlsCFValueObject() {
			ObjectType = ConditionalFormattingValueObjectType.Num;
		}
		public static XlsCFValueObject FromStream(XlsReader reader) {
			XlsCFValueObject result = new XlsCFValueObject();
			result.Read(reader);
			return result;
		}
		void Read(XlsReader reader) {
			ObjectType = XlsCFHelper.CFValueObjectCodeToType(reader.ReadByte());
			formulaSize = reader.ReadUInt16();
			if(formulaSize > 0)
				formulaBytes = reader.ReadBytes(formulaSize);
			if(!OmitValue())
				Value = reader.ReadDouble();
		}
		public void Write(BinaryWriter writer) {
			writer.Write(XlsCFHelper.CFValueObjectTypeToCode(ObjectType));
			writer.Write((ushort)formulaSize);
			if(formulaSize > 0)
				writer.Write(formulaBytes);
			if(!OmitValue())
				writer.Write(Value);
		}
		public int GetSize() {
			int result = fixedPartSize + formulaSize;
			if(!OmitValue())
				result += sizeof(double);
			return result;
		}
		public ParsedExpression GetFormula(XlsRPNContext context) {
			return XlsCFHelper.GetFormulaExpression(context, formulaBytes, formulaSize);
		}
		public void SetFormula(ParsedExpression parsedExpression, XlsRPNContext context) {
			formulaBytes = XlsCFHelper.GetFormulaBytes(parsedExpression, context, ref formulaSize);
		}
		bool OmitValue() {
			return (formulaSize > 0 || ObjectType == ConditionalFormattingValueObjectType.Min || ObjectType == ConditionalFormattingValueObjectType.Max);
		}
	}
	#endregion
	#region XlsCFColorScaleParams
	public class XlsCFColorScalePoint {
		#region Fields
		XlsCFValueObject cellValue = new XlsCFValueObject();
		ColorModelInfo colorInfo = new ColorModelInfo();
		#endregion
		#region Properties
		public XlsCFValueObject CellValue {
			get { return cellValue; }
			set {
				if(value != null)
					cellValue = value;
				else
					cellValue = new XlsCFValueObject();
			}
		}
		public ColorModelInfo ColorInfo {
			get { return colorInfo; }
			set {
				if(value != null)
					colorInfo = value;
				else
					colorInfo = new ColorModelInfo();
			}
		}
		#endregion
	}
	public class XlsCFColorScaleParams {
		#region Fields
		const int fixedPartSize = 6;
		readonly List<XlsCFColorScalePoint> points = new List<XlsCFColorScalePoint>();
		#endregion
		#region Properties
		public bool Clamp { get; set; }
		public List<XlsCFColorScalePoint> Points { get { return points; } }
		#endregion
		public static XlsCFColorScaleParams FromStream(XlsReader reader) {
			XlsCFColorScaleParams result = new XlsCFColorScaleParams();
			result.Read(reader);
			return result;
		}
		void Read(XlsReader reader) {
			reader.ReadUInt16(); 
			reader.ReadByte(); 
			int cInterpCurve = reader.ReadByte();
			int cGradientCurve = reader.ReadByte();
			byte bitwiseField = reader.ReadByte();
			Clamp = Convert.ToBoolean(bitwiseField & 0x01);
			for(int i = 0; i < cInterpCurve; i++) {
				XlsCFColorScalePoint point = new XlsCFColorScalePoint();
				point.CellValue = XlsCFValueObject.FromStream(reader);
				reader.ReadDouble(); 
				Points.Add(point);
			}
			for(int i = 0; i < Math.Min(cInterpCurve, cGradientCurve); i++) {
				XlsCFColorScalePoint point = Points[i];
				reader.ReadDouble(); 
				point.ColorInfo = XlsCFHelper.ReadColor(reader);
			}
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)0);
			writer.Write((byte)0);
			int count = Points.Count;
			writer.Write((byte)count); 
			writer.Write((byte)count); 
			byte bitwiseField = 0x02;
			if(Clamp)
				bitwiseField |= 0x01;
			writer.Write(bitwiseField);
			double[] pointValue = GetPointValues(count);
			for(int i = 0; i < count; i++) {
				Points[i].CellValue.Write(writer);
				writer.Write(pointValue[i]); 
			}
			for(int i = 0; i < count; i++) {
				writer.Write(pointValue[i]); 
				XlsCFHelper.WriteColor(writer, Points[i].ColorInfo);
			}
		}
		public int GetSize() {
			int result = fixedPartSize;
			foreach(XlsCFColorScalePoint point in Points) {
				result += point.CellValue.GetSize() + 8;
				result += 24;
			}
			return result;
		}
		double[] GetPointValues(int count) {
			if(count == 3)
				return new double[] { 0.0, 0.5, 1.0 };
			return new double[] { 0.0, 1.0 };
		}
	}
	#endregion
	#region XlsCFDatabarParams
	public class XlsCFDatabarParams {
		#region Fields
		const int fixedPartSize = 22;
		int percentMin;
		int percentMax;
		ColorModelInfo colorInfo = new ColorModelInfo();
		XlsCFValueObject maxValue = new XlsCFValueObject();
		XlsCFValueObject minValue = new XlsCFValueObject();
		#endregion
		#region Properties
		public bool RightToLeft { get; set; }
		public bool ShowBarOnly { get; set; }
		public int PercentMin {
			get { return percentMin; }
			set {
				ValueChecker.CheckValue(value, 0, 100, "PercentMin");
				percentMin = value;
			}
		}
		public int PercentMax {
			get { return percentMax; }
			set {
				ValueChecker.CheckValue(value, 0, 100, "PercentMax");
				percentMax = value;
			}
		}
		public ColorModelInfo ColorInfo {
			get { return colorInfo; }
			set {
				if(value != null)
					colorInfo = value;
				else
					colorInfo = new ColorModelInfo();
			}
		}
		public XlsCFValueObject MaxValue {
			get { return maxValue; }
			set {
				if(value != null)
					maxValue = value;
				else
					maxValue = new XlsCFValueObject();
			}
		}
		public XlsCFValueObject MinValue {
			get { return minValue; }
			set {
				if(value != null)
					minValue = value;
				else
					minValue = new XlsCFValueObject();
			}
		}
		#endregion
		public static XlsCFDatabarParams FromStream(XlsReader reader) {
			XlsCFDatabarParams result = new XlsCFDatabarParams();
			result.Read(reader);
			return result;
		}
		void Read(XlsReader reader) {
			reader.ReadUInt16(); 
			reader.ReadByte(); 
			byte bitwiseField = reader.ReadByte();
			RightToLeft = Convert.ToBoolean(bitwiseField & 0x01);
			ShowBarOnly = Convert.ToBoolean(bitwiseField & 0x02);
			this.percentMin = reader.ReadByte();
			this.percentMax = reader.ReadByte();
			this.colorInfo = XlsCFHelper.ReadColor(reader);
			this.maxValue = XlsCFValueObject.FromStream(reader);
			this.minValue = XlsCFValueObject.FromStream(reader);
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)0); 
			writer.Write((byte)0); 
			byte bitwiseField = 0;
			if(RightToLeft)
				bitwiseField |= 0x01;
			if(ShowBarOnly)
				bitwiseField |= 0x02;
			writer.Write(bitwiseField);
			writer.Write((byte)this.percentMin);
			writer.Write((byte)this.percentMax);
			XlsCFHelper.WriteColor(writer, this.colorInfo);
			this.maxValue.Write(writer);
			this.minValue.Write(writer);
		}
		public int GetSize() {
			return fixedPartSize + maxValue.GetSize() + minValue.GetSize();
		}
	}
	#endregion
	#region XlsCFFilterParams
	public class XlsCFFilterParams {
		#region Fields
		const int fixedPartSize = 2;
		int value;
		#endregion
		#region Properties
		public bool IsEmpty { get; set; }
		public bool Top { get; set; }
		public bool Percent { get; set; }
		public int Value {
			get { return this.value; }
			set {
				ValueChecker.CheckValue(value, 0, 1000, "Value");
				this.value = value;
			}
		}
		#endregion
		public XlsCFFilterParams() {
			IsEmpty = true;
		}
		public static XlsCFFilterParams FromStream(XlsReader reader) {
			XlsCFFilterParams result = new XlsCFFilterParams();
			result.Read(reader);
			return result;
		}
		void Read(XlsReader reader) {
			int cbFilter = reader.ReadUInt16();
			IsEmpty = cbFilter == 0;
			if(cbFilter > 0) {
				reader.ReadByte(); 
				byte bitwiseField = reader.ReadByte();
				Top = Convert.ToBoolean(bitwiseField & 0x01);
				Percent = Convert.ToBoolean(bitwiseField & 0x02);
				this.value = reader.ReadUInt16();
			}
		}
		public void Write(BinaryWriter writer) {
			int cbFilter = IsEmpty ? 0 : 4;
			writer.Write((ushort)cbFilter);
			if(cbFilter > 0) {
				writer.Write((byte)0); 
				byte bitwiseField = 0;
				if(Top)
					bitwiseField |= 0x01;
				if(Percent)
					bitwiseField |= 0x02;
				writer.Write(bitwiseField);
				writer.Write((ushort)Value);
			}
		}
		public int GetSize() {
			int result = fixedPartSize;
			if(!IsEmpty)
				result += 4;
			return result;
		}
	}
	#endregion
	#region XlsCFIconSetParams
	public class XlsCFIconThreshold {
		#region Fields
		const int fixedPartSize = 5;
		XlsCFValueObject value = new XlsCFValueObject();
		#endregion
		#region Properties
		public XlsCFValueObject Value {
			get { return this.value; }
			set {
				if(value != null)
					this.value = value;
				else
					this.value = new XlsCFValueObject();
			}
		}
		public bool EqualPass { get; set; }
		#endregion
		public static XlsCFIconThreshold FromStream(XlsReader reader) {
			XlsCFIconThreshold result = new XlsCFIconThreshold();
			result.Read(reader);
			return result;
		}
		void Read(XlsReader reader) {
			this.value = XlsCFValueObject.FromStream(reader);
			EqualPass = Convert.ToBoolean(reader.ReadByte());
			reader.ReadInt32(); 
		}
		public void Write(BinaryWriter writer) {
			this.value.Write(writer);
			writer.Write((byte)(EqualPass ? 1 : 0));
			writer.Write((int)0); 
		}
		public int GetSize() {
			return fixedPartSize + this.value.GetSize();
		}
	}
	public class XlsCFIconSetParams {
		#region Fields
		const int fixedPartSize = 6;
		readonly List<XlsCFIconThreshold> thresholds = new List<XlsCFIconThreshold>();
		#endregion
		#region Properties
		public IconSetType IconSet { get; set; }
		public bool IconsOnly { get; set; }
		public bool Reverse { get; set; }
		public List<XlsCFIconThreshold> Thresholds { get { return thresholds; } }
		#endregion
		public static XlsCFIconSetParams FromStream(XlsReader reader) {
			XlsCFIconSetParams result = new XlsCFIconSetParams();
			result.Read(reader);
			return result;
		}
		void Read(XlsReader reader) {
			reader.ReadUInt16(); 
			reader.ReadByte(); 
			int count = reader.ReadByte();
			IconSet = XlsCFHelper.IconSetCodeToType(reader.ReadByte());
			byte bitwiseField = reader.ReadByte();
			IconsOnly = Convert.ToBoolean(bitwiseField & 0x01);
			Reverse = Convert.ToBoolean(bitwiseField & 0x04);
			for(int i = 0; i < count; i++)
				Thresholds.Add(XlsCFIconThreshold.FromStream(reader));
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)0); 
			writer.Write((byte)0); 
			int count = Thresholds.Count;
			writer.Write((byte)count);
			writer.Write(XlsCFHelper.IconSetTypeToCode(IconSet));
			byte bitwiseField = 0;
			if(IconsOnly)
				bitwiseField |= 0x01;
			if(Reverse)
				bitwiseField |= 0x04;
			writer.Write(bitwiseField);
			for(int i = 0; i < count; i++)
				Thresholds[i].Write(writer);
		}
		public int GetSize() {
			int result = fixedPartSize;
			foreach(XlsCFIconThreshold item in Thresholds)
				result += item.GetSize();
			return result;
		}
	}
	#endregion
	#region XlsCondFmtExtNonCF12
	public class XlsCondFmtExtNonCF12 : IXlsCFRuleTemplateContainer {
		#region Fields
		const int fixedPartSize = 25;
		int cfIndex;
		int priority;
		DxfN12Info format = new DxfN12Info();
		int stdDev;
		#endregion
		#region Properties
		public int CFIndex {
			get { return cfIndex; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "CFIndex");
				cfIndex = value;
			}
		}
		public ConditionalFormattingOperator ComparisonFunction { get; set; }
		public XlsCFRuleTemplate RuleTemplate { get; set; }
		public int Priority {
			get { return priority; }
			set {
				ValueChecker.CheckValue(value, 0, ushort.MaxValue, "Priority");
				priority = value;
			}
		}
		public bool IsActive { get; set; }
		public bool StopIfTrue { get; set; }
		public bool HasFormat { get; set; }
		public DxfN12Info Format { 
			get { return format; }
			set {
				if(value != null)
					format = value;
				else
					format = new DxfN12Info();
			}
		}
		public bool FilterTop { get; set; }
		public bool FilterPercent { get; set; }
		public int FilterValue { get; set; }
		public ConditionalFormattingTextCondition TextRule { get; set; }
		public int StdDev {
			get { return stdDev; }
			set {
				ValueChecker.CheckValue(value, 0, 3, "StdDev");
				stdDev = value;
			}
		}
		#endregion
		public static XlsCondFmtExtNonCF12 FromStream(XlsReader reader) {
			XlsCondFmtExtNonCF12 result = new XlsCondFmtExtNonCF12();
			result.Read(reader);
			return result;
		}
		protected void Read(XlsReader reader) {
			this.cfIndex = reader.ReadUInt16();
			ComparisonFunction = XlsCFHelper.ComparisonFunctionCodeToType(reader.ReadByte());
			RuleTemplate = (XlsCFRuleTemplate)reader.ReadByte();
			this.priority = reader.ReadUInt16();
			byte bitwiseField = reader.ReadByte();
			IsActive = Convert.ToBoolean(bitwiseField & 0x01);
			StopIfTrue = Convert.ToBoolean(bitwiseField & 0x02);
			HasFormat = Convert.ToBoolean(reader.ReadByte());
			if(HasFormat)
				this.format = DxfN12Info.FromStream(reader);
			XlsCFHelper.ReadTemplateParams(reader, this);
		}
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)CFIndex);
			writer.Write(XlsCFHelper.ComparisonFunctionTypeToCode(ComparisonFunction));
			writer.Write((byte)RuleTemplate);
			writer.Write((ushort)Priority);
			byte bitwiseField = 0;
			if(IsActive)
				bitwiseField |= 0x01;
			if(StopIfTrue)
				bitwiseField |= 0x02;
			writer.Write(bitwiseField);
			writer.Write((byte)(HasFormat ? 1 : 0));
			if(HasFormat)
				Format.Write(writer);
			XlsCFHelper.WriteTemplateParams(writer, this);
		}
		public int GetSize() {
			int result = fixedPartSize;
			if(HasFormat)
				result += this.format.GetSize();
			return result;
		}
	}
	#endregion
}
