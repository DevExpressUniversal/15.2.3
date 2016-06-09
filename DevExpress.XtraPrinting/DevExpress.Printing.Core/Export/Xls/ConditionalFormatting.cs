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
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Export.Xl;
using DevExpress.Utils;
namespace DevExpress.XtraExport.Xls {
	#region IXlsCondFmtWithRuleTemplate
	public interface IXlsCondFmtWithRuleTemplate {
		XlsCFRuleTemplate RuleTemplate { get; set; }
		bool FilterTop { get; set; }
		bool FilterPercent { get; set; }
		int FilterValue { get; set; }
		XlCondFmtSpecificTextType TextRule { get; set; }
		int StdDev { get; set; }
	}
	#endregion
	#region XlsCondFmtHelper
	static class XlsCondFmtHelper {
		enum XColorType {
			Auto = 0x0000,
			Indexed = 0x0001,
			Rgb = 0x00002,
			Themed = 0x0003,
			Ninched = 0x0004
		}
		static readonly ushort[] dateTemplateValues = new ushort[] { 0, 6, 1, 2, 5, 8, 3, 7, 4, 9 };
		public static byte RuleTypeToCode(XlCondFmtType ruleType) {
			switch(ruleType) {
				case XlCondFmtType.CellIs: return 0x01;
				case XlCondFmtType.Expression: return 0x02;
				case XlCondFmtType.ColorScale: return 0x03;
				case XlCondFmtType.DataBar: return 0x04;
				case XlCondFmtType.Top10: return 0x05;
				case XlCondFmtType.IconSet: return 0x06;
			}
			return 0x00;
		}
		public static byte OperatorToCode(XlCondFmtOperator oper) {
			switch(oper) {
				case XlCondFmtOperator.Between: return 0x01;
				case XlCondFmtOperator.NotBetween: return 0x02;
				case XlCondFmtOperator.Equal: return 0x03;
				case XlCondFmtOperator.NotEqual: return 0x04;
				case XlCondFmtOperator.GreaterThan: return 0x05;
				case XlCondFmtOperator.LessThan: return 0x06;
				case XlCondFmtOperator.GreaterThanOrEqual: return 0x07;
				case XlCondFmtOperator.LessThanOrEqual: return 0x08;
			}
			return 0x00;
		}
		public static byte CfvoTypeToCode(XlCondFmtValueObjectType objectType) {
			switch(objectType) {
				case XlCondFmtValueObjectType.Number: return 0x01;
				case XlCondFmtValueObjectType.Min: return 0x02;
				case XlCondFmtValueObjectType.Max: return 0x03;
				case XlCondFmtValueObjectType.Percent: return 0x04;
				case XlCondFmtValueObjectType.Percentile: return 0x05;
				case XlCondFmtValueObjectType.Formula: return 0x07;
			}
			return 0x00;
		}
		public static byte IconSetTypeToCode(XlCondFmtIconSetType type) {
			switch(type) {
				case XlCondFmtIconSetType.Arrows3: return 0x00;
				case XlCondFmtIconSetType.ArrowsGray3: return 0x01;
				case XlCondFmtIconSetType.Flags3: return 0x02;
				case XlCondFmtIconSetType.TrafficLights3: return 0x03;
				case XlCondFmtIconSetType.Signs3: return 0x04;
				case XlCondFmtIconSetType.TrafficLights3Black: return 0x05;
				case XlCondFmtIconSetType.Symbols3: return 0x06;
				case XlCondFmtIconSetType.Symbols3Circled: return 0x07;
				case XlCondFmtIconSetType.Arrows4: return 0x08;
				case XlCondFmtIconSetType.ArrowsGray4: return 0x09;
				case XlCondFmtIconSetType.RedToBlack4: return 0x0a;
				case XlCondFmtIconSetType.Rating4: return 0x0b;
				case XlCondFmtIconSetType.TrafficLights4: return 0x0c;
				case XlCondFmtIconSetType.Arrows5: return 0x0d;
				case XlCondFmtIconSetType.ArrowsGray5: return 0x0e;
				case XlCondFmtIconSetType.Rating5: return 0x0f;
				case XlCondFmtIconSetType.Quarters5: return 0x10;
			}
			return 0xff;
		}
		public static bool IsSupportedIconSet(XlCondFmtIconSetType type) {
			return IconSetTypeToCode(type) != 0xff;
		}
		public static short SpecificTextTypeToCode(XlCondFmtSpecificTextType type) {
			switch(type) {
				case XlCondFmtSpecificTextType.NotContains: return 0x01;
				case XlCondFmtSpecificTextType.BeginsWith: return 0x02;
				case XlCondFmtSpecificTextType.EndsWith: return 0x03;
			}
			return 0x00;
		}
		public static void WriteColor(BinaryWriter writer, XlColor color) {
			if(color.IsEmpty) {
				writer.Write((int)XColorType.Ninched);
				writer.Write((int)0);
				writer.Write((double)0.0);
				return;
			}
			switch(color.ColorType) {
				case XlColorType.Auto:
					writer.Write((int)XColorType.Auto);
					writer.Write((int)0);
					writer.Write((double)0.0);
					break;
				case XlColorType.Rgb:
					writer.Write((int)XColorType.Rgb);
					Color argb = color.Rgb;
					writer.Write(argb.R);
					writer.Write(argb.G);
					writer.Write(argb.B);
					writer.Write((byte)0xff);
					writer.Write((double)0.0);
					break;
				case XlColorType.Theme:
					writer.Write((int)XColorType.Themed);
					writer.Write((int)color.ThemeColor);
					writer.Write(color.Tint);
					break;
			}
		}
		public static void WriteTemplateParams(BinaryWriter writer, IXlsCondFmtWithRuleTemplate source) {
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
					writer.Write(XlsCondFmtHelper.SpecificTextTypeToCode(source.TextRule));
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
	}
	#endregion
	#region XlsCFRuleTemplate
	public enum XlsCFRuleTemplate {
		CellValue = 0x0000,
		Formula = 0x0001,
		ColorScale = 0x0002,
		DataBar = 0x0003,
		IconSet = 0x0004,
		Filter = 0x0005,
		UniqueValues = 0x0007,
		ContainsText = 0x0008,
		ContainsBlanks = 0x0009,
		ContainsNoBlanks = 0x000a,
		ContainsErrors = 0x000b,
		ContainsNoError = 0x000c,
		Today = 0x000f,
		Tomorrow = 0x0010,
		Yesterday = 0x0011,
		Last7Days = 0x0012,
		LastMonth = 0x0013,
		NextMonth = 0x0014,
		ThisWeek = 0x0015,
		NextWeek = 0x0016,
		LastWeek = 0x0017,
		ThisMonth = 0x0018,
		AboveAverage = 0x0019,
		BelowAverage = 0x001a,
		DuplicateValues = 0x001b,
		AboveOrEqualToAverage = 0x001d,
		BelowOrEqualToAverage = 0x001e
	}
	#endregion
	#region XlsCondFmtValueObject
	public class XlsCondFmtValueObject {
		#region Fields
		const int fixedPartSize = 3;
		byte[] formulaBytes = new byte[0];
		#endregion
		#region Properties
		public XlCondFmtValueObjectType ObjectType { get; set; }
		public double Value { get; set; }
		public byte[] FormulaBytes {
			get { return formulaBytes; }
			set {
				if(value == null || value.Length < 2)
					formulaBytes = new byte[0];
				else {
					int size = BitConverter.ToUInt16(value, 0);
					this.formulaBytes = new byte[size];
					Array.Copy(value, 2, this.formulaBytes, 0, size);
				}
			}
		}
		#endregion
		public XlsCondFmtValueObject() {
			ObjectType = XlCondFmtValueObjectType.Number;
		}
		public virtual void Write(BinaryWriter writer) {
			writer.Write(XlsCondFmtHelper.CfvoTypeToCode(ObjectType));
			ushort formulaSize = (ushort)formulaBytes.Length;
			writer.Write(formulaSize);
			if(formulaSize > 0)
				writer.Write(formulaBytes);
			if(!OmitValue())
				writer.Write(Value);
		}
		public virtual int GetSize() {
			int result = fixedPartSize + formulaBytes.Length;
			if(!OmitValue())
				result += sizeof(double);
			return result;
		}
		bool OmitValue() {
			return (formulaBytes.Length > 0 || ObjectType == XlCondFmtValueObjectType.Min || ObjectType == XlCondFmtValueObjectType.Max);
		}
	}
	#endregion
	#region XlsCondFmtFilterParams
	public class XlsCondFmtFilterParams {
		#region Fields
		const int fixedPartSize = 2;
		#endregion
		#region Properties
		public bool IsEmpty { get; set; }
		public bool Top { get; set; }
		public bool Percent { get; set; }
		public int Value { get; set; }
		#endregion
		public XlsCondFmtFilterParams() {
			IsEmpty = true;
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
	#region XlsCondFmtDatabarParams
	public class XlsCondFmtDatabarParams {
		#region Fields
		const int fixedPartSize = 22;
		XlsCondFmtValueObject maxValue = new XlsCondFmtValueObject();
		XlsCondFmtValueObject minValue = new XlsCondFmtValueObject();
		XlColor barColor = XlColor.Empty;
		#endregion
		#region Properties
		public bool RightToLeft { get; set; }
		public bool ShowBarOnly { get; set; }
		public int PercentMin { get; set; }
		public int PercentMax { get; set; }
		public XlColor BarColor {
			get { return barColor; }
			set { barColor = value ?? XlColor.Empty; }
		}
		public XlsCondFmtValueObject MaxValue { get { return maxValue; } }
		public XlsCondFmtValueObject MinValue { get { return minValue; } }
		#endregion
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)0); 
			writer.Write((byte)0); 
			byte bitwiseField = 0;
			if(RightToLeft)
				bitwiseField |= 0x01;
			if(ShowBarOnly)
				bitwiseField |= 0x02;
			writer.Write(bitwiseField);
			writer.Write((byte)PercentMin);
			writer.Write((byte)PercentMax);
			XlsCondFmtHelper.WriteColor(writer, BarColor);
			this.minValue.Write(writer);
			this.maxValue.Write(writer);
		}
		public int GetSize() {
			return fixedPartSize + maxValue.GetSize() + minValue.GetSize();
		}
	}
	#endregion
	#region XlsCondFmtIconThreshold
	public class XlsCondFmtIconThreshold : XlsCondFmtValueObject {
		#region Properties
		public bool EqualPass { get; set; }
		#endregion
		public override void Write(BinaryWriter writer) {
			base.Write(writer);
			writer.Write((byte)(EqualPass ? 1 : 0));
			writer.Write((int)0); 
		}
		public override int GetSize() {
			return base.GetSize() + 5;
		}
	}
	#endregion
	#region XlsCondFmtIconSetParams
	public class XlsCondFmtIconSetParams {
		#region Fields
		const int fixedPartSize = 6;
		readonly List<XlsCondFmtIconThreshold> thresholds = new List<XlsCondFmtIconThreshold>();
		#endregion
		#region Properties
		public XlCondFmtIconSetType IconSet { get; set; }
		public bool IconsOnly { get; set; }
		public bool Reverse { get; set; }
		public List<XlsCondFmtIconThreshold> Thresholds { get { return thresholds; } }
		#endregion
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)0); 
			writer.Write((byte)0); 
			int count = Thresholds.Count;
			writer.Write((byte)count);
			writer.Write(XlsCondFmtHelper.IconSetTypeToCode(IconSet));
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
			foreach(XlsCondFmtIconThreshold item in Thresholds)
				result += item.GetSize();
			return result;
		}
	}
	#endregion
	#region XlsCondFmtColorScaleParams
	public class XlsCondFmtColorScaleParams {
		#region Fields
		const int fixedPartSize = 6;
		const double minDomain = 0.0;
		const double midDomain = 0.5;
		const double maxDomain = 1.0;
		XlsCondFmtValueObject minValue = new XlsCondFmtValueObject();
		XlsCondFmtValueObject midValue = new XlsCondFmtValueObject();
		XlsCondFmtValueObject maxValue = new XlsCondFmtValueObject();
		XlColor minColor = XlColor.Empty;
		XlColor midColor = XlColor.Empty;
		XlColor maxColor = XlColor.Empty;
		#endregion
		#region Properties
		public XlCondFmtColorScaleType ColorScaleType { get; set; }
		public XlsCondFmtValueObject MinValue { get { return minValue; } }
		public XlsCondFmtValueObject MidValue { get { return midValue; } }
		public XlsCondFmtValueObject MaxValue { get { return maxValue; } }
		public XlColor MinColor {
			get { return minColor; }
			set { minColor = value ?? XlColor.Empty; }
		}
		public XlColor MidColor {
			get { return midColor; }
			set { midColor = value ?? XlColor.Empty; }
		}
		public XlColor MaxColor {
			get { return maxColor; }
			set { maxColor = value ?? XlColor.Empty; }
		}
		#endregion
		public void Write(BinaryWriter writer) {
			writer.Write((ushort)0);
			writer.Write((byte)0);
			int count = ColorScaleType == XlCondFmtColorScaleType.ColorScale3 ? 3 : 2;
			writer.Write((byte)count); 
			writer.Write((byte)count); 
			writer.Write((byte)0x03);
			minValue.Write(writer);
			writer.Write(minDomain);
			if(count == 3) {
				midValue.Write(writer);
				writer.Write(midDomain);
			}
			maxValue.Write(writer);
			writer.Write(maxDomain);
			writer.Write(minDomain);
			XlsCondFmtHelper.WriteColor(writer, MinColor);
			if(count == 3) {
				writer.Write(midDomain);
				XlsCondFmtHelper.WriteColor(writer, MidColor);
			}
			writer.Write(maxDomain);
			XlsCondFmtHelper.WriteColor(writer, MaxColor);
		}
		public int GetSize() {
			int result = fixedPartSize;
			result += MinValue.GetSize() + 32;
			result += MaxValue.GetSize() + 32;
			if (ColorScaleType == XlCondFmtColorScaleType.ColorScale3)
				result += MidValue.GetSize() + 32;
			return result;
		}
	}
	#endregion
	#region Rule content adapters
	interface IXlsCFContentAdapter {
		void SetRuleType(XlCondFmtType ruleType);
		void SetOperator(XlCondFmtOperator cfOperator);
		void SetRuleTemplate(XlsCFRuleTemplate ruleTemplate);
		void SetFirstFormula(byte[] formulaBytes);
		void SetSecondFormula(byte[] formulaBytes);
		void SetFilterTop(bool value);
		void SetFilterPercent(bool value);
		void SetFilterValue(int value);
		void SetTextRule(XlCondFmtSpecificTextType textRule);
		void SetStdDev(int value);
	}
	class XlsContentCFAdapter : IXlsCFContentAdapter {
		XlsContentCF content;
		public XlsContentCFAdapter(XlsContentCF content) {
			this.content = content;
		}
		public void SetRuleType(XlCondFmtType ruleType) {
			if(ruleType == XlCondFmtType.Top10)
				content.RuleType = XlCondFmtType.Expression;
			else
				content.RuleType = ruleType;
		}
		public void SetOperator(XlCondFmtOperator cfOperator) {
			content.Operator = cfOperator;
		}
		public void SetRuleTemplate(XlsCFRuleTemplate ruleTemplate) {
		}
		public void SetFirstFormula(byte[] formulaBytes) {
			content.FirstFormulaBytes = formulaBytes;
		}
		public void SetSecondFormula(byte[] formulaBytes) {
			content.SecondFormulaBytes = formulaBytes;
		}
		public void SetFilterTop(bool value) {
		}
		public void SetFilterPercent(bool value) {
		}
		public void SetFilterValue(int value) {
		}
		public void SetTextRule(XlCondFmtSpecificTextType textRule) {
		}
		public void SetStdDev(int value) {
		}
	}
	class XlsContentCF12Adapter : IXlsCFContentAdapter {
		XlsContentCF12 content;
		bool withFormula;
		public XlsContentCF12Adapter(XlsContentCF12 content, bool withFormula) {
			this.content = content;
			this.withFormula = withFormula;
		}
		public void SetRuleType(XlCondFmtType ruleType) {
			content.RuleType = ruleType;
		}
		public void SetOperator(XlCondFmtOperator cfOperator) {
			content.Operator = cfOperator;
		}
		public void SetRuleTemplate(XlsCFRuleTemplate ruleTemplate) {
			content.RuleTemplate = ruleTemplate;
		}
		public void SetFirstFormula(byte[] formulaBytes) {
			if(withFormula)
				content.FirstFormulaBytes = formulaBytes;
		}
		public void SetSecondFormula(byte[] formulaBytes) {
			if(withFormula)
				content.SecondFormulaBytes = formulaBytes;
		}
		public void SetFilterTop(bool value) {
			content.FilterTop = false;
			content.FilterParams.Top = value;
			content.FilterParams.IsEmpty = false;
		}
		public void SetFilterPercent(bool value) {
			content.FilterPercent = false;
			content.FilterParams.Percent = value;
			content.FilterParams.IsEmpty = false;
		}
		public void SetFilterValue(int value) {
			content.FilterValue = 0;
			content.FilterParams.Value = value;
			content.FilterParams.IsEmpty = false;
		}
		public void SetTextRule(XlCondFmtSpecificTextType textRule) {
			content.TextRule = textRule;
		}
		public void SetStdDev(int value) {
			content.StdDev = value;
		}
	}
	class XlsContentCFExAdapter : IXlsCFContentAdapter {
		XlsContentCFEx content;
		public XlsContentCFExAdapter(XlsContentCFEx content) {
			this.content = content;
		}
		public void SetRuleType(XlCondFmtType ruleType) {
		}
		public void SetOperator(XlCondFmtOperator cfOperator) {
			content.Operator = cfOperator;
		}
		public void SetRuleTemplate(XlsCFRuleTemplate ruleTemplate) {
			content.RuleTemplate = ruleTemplate;
		}
		public void SetFirstFormula(byte[] formulaBytes) {
		}
		public void SetSecondFormula(byte[] formulaBytes) {
		}
		public void SetFilterTop(bool value) {
			content.FilterTop = value;
		}
		public void SetFilterPercent(bool value) {
			content.FilterPercent = value;
		}
		public void SetFilterValue(int value) {
			content.FilterValue = value;
		}
		public void SetTextRule(XlCondFmtSpecificTextType textRule) {
			content.TextRule = textRule;
		}
		public void SetStdDev(int value) {
			content.StdDev = value;
		}
	}
	#endregion
	#region XlsDataAwareExporter
	public partial class XlsDataAwareExporter {
		#region Function codes
		const int funcCount = 0x0000;
		const int funcIf = 0x0001;
		const int funcIsError = 0x0003;
		const int funcAverage = 0x0005;
		const int funcMin = 0x0006;
		const int funcMax = 0x0007;
		const int funcInt = 0x0019;
		const int funcLen = 0x0020;
		const int funcAnd = 0x0024;
		const int funcNot = 0x0026;
		const int funcSearch = 0x0052;
		const int funcLeft = 0x0073;
		const int funcRight = 0x0074;
		const int funcTrim = 0x0076;
		const int funcIsBlank = 0x0081;
		const int funcLarge = 0x0145;
		const int funcSmall = 0x0146;
		const int funcCountIf = 0x015a;
		const int funcToday = 0x00dd;
		const int funcMonth = 0x0044;
		const int funcYear = 0x0045;
		const int funcWeekday = 0x0046;
		const int funcFloor = 0x011d;
		const int funcRoundDown = 0x00d5;
		const int funcStDevP = 0x00c1;
		#endregion
		#region Ptg instances
		static XlPtgRefN ptgRefN = new XlPtgRefN(new XlCellOffset(0, 0, XlCellOffsetType.Offset, XlCellOffsetType.Offset)) { DataType = XlPtgDataType.Value };
		static XlPtgInt ptgZero = new XlPtgInt(0);
		static XlPtgInt ptgOne = new XlPtgInt(1);
		static XlPtgAttrSemi ptgAttrSemi = new XlPtgAttrSemi();
		static XlPtgAttrSpace ptgAttrSpace = new XlPtgAttrSpace(XlPtgAttrSpaceType.SpaceBeforeBaseExpression, 1);
		static XlPtgBinaryOperator ptgAdd = new XlPtgBinaryOperator(XlPtgTypeCode.Add);
		static XlPtgBinaryOperator ptgSub = new XlPtgBinaryOperator(XlPtgTypeCode.Sub);
		static XlPtgBinaryOperator ptgMul = new XlPtgBinaryOperator(XlPtgTypeCode.Mul);
		static XlPtgBinaryOperator ptgEq = new XlPtgBinaryOperator(XlPtgTypeCode.Eq);
		static XlPtgBinaryOperator ptgGt = new XlPtgBinaryOperator(XlPtgTypeCode.Gt);
		static XlPtgBinaryOperator ptgGe = new XlPtgBinaryOperator(XlPtgTypeCode.Ge);
		static XlPtgBinaryOperator ptgLt = new XlPtgBinaryOperator(XlPtgTypeCode.Lt);
		static XlPtgBinaryOperator ptgLe = new XlPtgBinaryOperator(XlPtgTypeCode.Le);
		static XlPtgParen ptgParen = new XlPtgParen();
		static XlPtgUnaryOperator ptgPercent = new XlPtgUnaryOperator(XlPtgTypeCode.Percent);
		static XlPtgBinaryOperator ptgUnion = new XlPtgBinaryOperator(XlPtgTypeCode.Union);
		#endregion
		int cfCount = 0;
		protected void WriteConditionalFormattings() {
			RemoveIncompatibleFormattings();
			IList<XlConditionalFormatting> conditionalFormattings = currentSheet.ConditionalFormattings;
			int count = conditionalFormattings.Count;
			for(int i = 0; i < count; i++)
				WriteConditionalFormatting(i + 1, conditionalFormattings[i]);
			for(int i = 0; i < count; i++)
				WriteConditionalFormattingExt(i + 1, conditionalFormattings[i]);
		}
		#region Remove incompatible cf
		void RemoveIncompatibleFormattings() {
			IList<XlConditionalFormatting> conditionalFormattings = currentSheet.ConditionalFormattings;
			for(int i = conditionalFormattings.Count - 1; i >= 0; i--) {
				if(ShouldRemove(conditionalFormattings[i]))
					conditionalFormattings.RemoveAt(i);
			}
		}
		bool ShouldRemove(XlConditionalFormatting cf) {
			int ruleCount = GetRuleCount(cf);
			if(ruleCount == 0)
				return true;
			XlsRef8 boundRange = GetBoundRange(cf.Ranges);
			if(boundRange == null)
				return true;
			return false;
		}
		#endregion
		void WriteConditionalFormatting(int condFmtId, XlConditionalFormatting cf) {
			bool isCondFmt12 = IsCondFmt12(cf);
			XlsContentCondFmt content = CreateCFContentRoot(isCondFmt12);
			content.Id = condFmtId;
			content.BoundRange = GetBoundRange(cf.Ranges);
			int ruleCount = GetRuleCount(cf);
			if(!isCondFmt12)
				ruleCount = Math.Min(3, ruleCount);
			content.RecordCount = ruleCount;
			content.ToughRecalc = true;
			int count = 0;
			foreach(XlCellRange range in cf.Ranges) {
				XlsRef8 subRange = XlsRef8.FromRange(range);
				if(subRange == null) continue;
				if(count >= XlsDefs.MaxCFRefCount)
					break;
				content.Ranges.Add(subRange);
				count++;
			}
			if(isCondFmt12) {
				WriteContent(XlsRecordType.CondFmt12, content);
				foreach(XlCondFmtRule rule in cf.Rules)
					WriteCondFmtRule12(rule, cf);
			}
			else {
				WriteContent(XlsRecordType.CondFmt, content);
				cfCount = 0;
				foreach(XlCondFmtRule rule in cf.Rules)
					WriteCondFmtRule(rule, cf);
			}
		}
		void WriteConditionalFormattingExt(int condFmtId, XlConditionalFormatting cf) {
			if(IsCondFmt12(cf))
				return;
			cfCount = 0;
			int count = cf.Rules.Count;
			for(int i = 0; i < count; i++) {
				XlCondFmtRule rule = cf.Rules[i];
				if(!IsCondFmt12(cf, rule as XlCondFmtRuleWithFormatting) && (cfCount < 3)) {
					WriteCFEx(condFmtId, cf, rule, false, cfCount);
					cfCount++;
				}
				else {
					WriteCFEx(condFmtId, cf, rule, true, 0);
					WriteCondFmtRule12(rule, cf);
				}
			}
		}
		void WriteCFEx(int id, XlConditionalFormatting cf, XlCondFmtRule rule, bool isCF12, int cfIndex) {
			XlsContentCFEx content = new XlsContentCFEx();
			content.Id = id;
			content.IsCF12 = isCF12;
			content.Range = GetBoundRange(cf.Ranges);
			if(!isCF12) {
				content.CFIndex = cfIndex;
				PrepareDxf(content.Format, rule as XlCondFmtRuleWithFormatting);
				content.HasFormat = content.Format.ExtProperties.Count > 0;
				content.IsActive = true;
				content.Priority = rule.Priority;
				content.StopIfTrue = rule.StopIfTrue;
				IXlsCFContentAdapter adapter = new XlsContentCFExAdapter(content);
				PrepareContent(adapter, rule, cf);
			}
			WriteContent(XlsRecordType.CFEx, content);
		}
		void WriteCondFmtRule(XlCondFmtRule rule, XlConditionalFormatting cf) {
			if(cfCount >= 3) return;
			XlCondFmtRuleWithFormatting cfRule = rule as XlCondFmtRuleWithFormatting;
			if(!IsCondFmt12(cf, cfRule)) {
				XlsContentCF content = new XlsContentCF();
				PrepareDxf(content.Format, cfRule);
				IXlsCFContentAdapter adapter = new XlsContentCFAdapter(content);
				PrepareContent(adapter, rule, cf);
				WriteContent(XlsRecordType.CF, content);
				cfCount++;
			}
		}
		void WriteCondFmtRule12(XlCondFmtRule rule, XlConditionalFormatting cf) {
			XlsContentCF12 content = new XlsContentCF12();
			content.Priority = rule.Priority;
			content.StopIfTrue = rule.StopIfTrue;
			XlCondFmtRuleWithFormatting ruleWithFormatting = rule as XlCondFmtRuleWithFormatting;
			PrepareDxf(content.Format, ruleWithFormatting);
			switch(rule.RuleType) {
				case XlCondFmtType.DataBar:
					PrepareRuleDataBar(content, rule as XlCondFmtRuleDataBar, cf);
					break;
				case XlCondFmtType.IconSet:
					if (!PrepareRuleIconSet(content, rule as XlCondFmtRuleIconSet, cf))
						return;
					break;
				case XlCondFmtType.ColorScale:
					PrepareRuleColorScale(content, rule as XlCondFmtRuleColorScale, cf);
					break;
				default:
					IXlsCFContentAdapter adapter = new XlsContentCF12Adapter(content, HasFormulaInCF12(ruleWithFormatting));
					PrepareContent(adapter, rule, cf);
					break;
			}
			WriteContent(XlsRecordType.CF12, content);
		}
		#region PrepareContent
		void PrepareContent(IXlsCFContentAdapter content, XlCondFmtRule rule, XlConditionalFormatting cf) {
			switch(rule.RuleType) {
				case XlCondFmtType.AboveOrBelowAverage:
					PrepareRuleAboveAverage(content, cf.Ranges, rule as XlCondFmtRuleAboveAverage);
					break;
				case XlCondFmtType.CellIs:
					PrepareRuleCellIs(content, rule as XlCondFmtRuleCellIs, cf);
					break;
				case XlCondFmtType.Expression:
					PrepareRuleExpression(content, rule as XlCondFmtRuleExpression, cf);
					break;
				case XlCondFmtType.ContainsBlanks:
					PrepareRuleBlanks(content, rule as XlCondFmtRuleWithFormatting);
					break;
				case XlCondFmtType.NotContainsBlanks:
					PrepareRuleNoBlanks(content, rule as XlCondFmtRuleWithFormatting);
					break;
				case XlCondFmtType.ContainsText:
				case XlCondFmtType.NotContainsText:
				case XlCondFmtType.BeginsWith:
				case XlCondFmtType.EndsWith:
					PrepareRuleSpecificText(content, rule as XlCondFmtRuleSpecificText);
					break;
				case XlCondFmtType.DuplicateValues:
					PrepareRuleUniqueDuplicates(content, cf.Ranges, rule as XlCondFmtRuleWithFormatting, false);
					break;
				case XlCondFmtType.UniqueValues:
					PrepareRuleUniqueDuplicates(content, cf.Ranges, rule as XlCondFmtRuleWithFormatting, true);
					break;
				case XlCondFmtType.Top10:
					PrepareRuleTop10(content, cf.Ranges, rule as XlCondFmtRuleTop10);
					break;
				case XlCondFmtType.TimePeriod:
					PrepareRuleTimePeriod(content, rule as XlCondFmtRuleTimePeriod);
					break;
			}
		}
		#region AboveAverage
		void PrepareRuleAboveAverage(IXlsCFContentAdapter content, IList<XlCellRange> ranges, XlCondFmtRuleAboveAverage rule) {
			content.SetRuleType(XlCondFmtType.Expression);
			content.SetOperator((XlCondFmtOperator)0);
			content.SetRuleTemplate(GetRuleTemplate(rule.AboveAverage, rule.EqualAverage));
			XlExpression expression = GetRuleFormula(rule, ranges);
			content.SetFirstFormula(expression.GetBytes(this));
			content.SetStdDev(rule.StdDev);
		}
		XlsCFRuleTemplate GetRuleTemplate(bool above, bool equal) {
			if(above)
				return equal ? XlsCFRuleTemplate.AboveOrEqualToAverage : XlsCFRuleTemplate.AboveAverage;
			return equal ? XlsCFRuleTemplate.BelowOrEqualToAverage : XlsCFRuleTemplate.BelowAverage;
		}
		XlExpression GetRuleFormula(XlCondFmtRuleAboveAverage rule, IList<XlCellRange> ranges) {
			XlExpression result = new XlExpression();
			result.Add(ptgRefN);
			if(rule.StdDev == 0) {
				int count = ranges.Count;
				for(int i = 0; i < count; i++)
					CreateAverageExpression(result, ranges[i]);
				result.Add(new XlPtgFuncVar(funcAverage, count, XlPtgDataType.Value));
				short typeCode;
				if(rule.AboveAverage)
					typeCode = rule.EqualAverage ? XlPtgTypeCode.Ge : XlPtgTypeCode.Gt;
				else
					typeCode = rule.EqualAverage ? XlPtgTypeCode.Le : typeCode = XlPtgTypeCode.Lt;
				result.Add(new XlPtgBinaryOperator(typeCode));
			}
			else {
				CreateRangeUnionExpression(result, ranges);
				result.Add(new XlPtgFuncVar(funcAverage, 1, XlPtgDataType.Value));
				result.Add(ptgSub);
				result.Add(ptgParen);
				CreateRangeUnionExpression(result, ranges);
				result.Add(new XlPtgFuncVar(funcStDevP, 1, XlPtgDataType.Value));
				if(!rule.AboveAverage)
					result.Add(new XlPtgNum(-rule.StdDev));
				else
					result.Add(new XlPtgInt(rule.StdDev));
				result.Add(ptgParen);
				result.Add(ptgMul);
				result.Add(new XlPtgBinaryOperator(rule.AboveAverage ? XlPtgTypeCode.Ge : XlPtgTypeCode.Le));
			}
			return result;
		}
		void CreateAverageExpression(XlExpression result, XlCellRange cellRange) {
			XlPtgArea ptgArea = new XlPtgArea(cellRange) { DataType = XlPtgDataType.Array };
			XlPtgStr ptgEmptyString = new XlPtgStr(string.Empty);
			result.Add(ptgArea);
			result.Add(new XlPtgFunc(funcIsError, XlPtgDataType.Array));
			result.Add(ptgAttrSpace);
			result.Add(ptgEmptyString);
			result.Add(ptgArea);
			result.Add(new XlPtgFunc(funcIsBlank, XlPtgDataType.Array));
			result.Add(ptgAttrSpace);
			result.Add(ptgEmptyString);
			result.Add(ptgAttrSpace);
			result.Add(ptgArea);
			result.Add(ptgAttrSpace);
			result.Add(new XlPtgFuncVar(funcIf, 3, XlPtgDataType.Reference));
			result.Add(ptgAttrSpace);
			result.Add(new XlPtgFuncVar(funcIf, 3, XlPtgDataType.Reference));
		}
		void CreateRangeUnionExpression(XlExpression result, IList<XlCellRange> ranges) {
			int count = ranges.Count;
			result.Add(CreatePtg(ranges[0]));
			for(int i = 1; i < count; i++) {
				result.Add(CreatePtg(ranges[i]));
				result.Add(ptgUnion);
			}
		}
		#endregion
		#region CellIs
		void PrepareRuleCellIs(IXlsCFContentAdapter content, XlCondFmtRuleCellIs rule, XlConditionalFormatting cf) {
			content.SetRuleType(rule.RuleType);
			content.SetOperator(rule.Operator);
			content.SetRuleTemplate(XlsCFRuleTemplate.CellValue);
			expressionContext.CurrentCell = cf.GetTopLeftCell();
			content.SetFirstFormula(GetFormulaBytes(rule.Value));
			if(rule.Operator == XlCondFmtOperator.Between || rule.Operator == XlCondFmtOperator.NotBetween)
				content.SetSecondFormula(GetFormulaBytes(rule.SecondValue));
		}
		#endregion
		#region Expression
		void PrepareRuleExpression(IXlsCFContentAdapter content, XlCondFmtRuleExpression rule, XlConditionalFormatting cf) {
			content.SetRuleType(rule.RuleType);
			content.SetOperator((XlCondFmtOperator)0);
			content.SetRuleTemplate(XlsCFRuleTemplate.Formula);
			XlExpression expression = null;
			if(rule.Expression != null) {
				expression = rule.Expression;
				content.SetFirstFormula(expression.GetBytes(this));
			}
			else if(!string.IsNullOrEmpty(rule.Formula)) {
				if(formulaParser == null)
					throw new InvalidOperationException("Formula parser required for this operation.");
				expressionContext.CurrentCell = cf.GetTopLeftCell();
				expressionContext.ReferenceMode = XlCellReferenceMode.Offset;
				expressionContext.ExpressionStyle = XlExpressionStyle.Normal;
				expression = formulaParser.Parse(rule.Formula, expressionContext);
				if(expression == null)
					throw new InvalidOperationException(string.Format("Can't parse rule formula '{0}'.", rule.Formula));
			}
			if(expression != null)
				content.SetFirstFormula(expression.ToXlsExpression().GetBytes(this));
		}
		#endregion
		#region Blanks
		void PrepareRuleBlanks(IXlsCFContentAdapter content, XlCondFmtRuleWithFormatting rule) {
			content.SetRuleType(XlCondFmtType.Expression);
			content.SetOperator((XlCondFmtOperator)0);
			content.SetRuleTemplate(XlsCFRuleTemplate.ContainsBlanks);
			XlExpression expression = new XlExpression();
			expression.Add(ptgRefN);
			expression.Add(new XlPtgFunc(funcTrim, XlPtgDataType.Value));
			expression.Add(new XlPtgFunc(funcLen, XlPtgDataType.Value));
			expression.Add(ptgZero);
			expression.Add(ptgEq);
			content.SetFirstFormula(expression.GetBytes(this));
		}
		void PrepareRuleNoBlanks(IXlsCFContentAdapter content, XlCondFmtRuleWithFormatting rule) {
			content.SetRuleType(XlCondFmtType.Expression);
			content.SetOperator((XlCondFmtOperator)0);
			content.SetRuleTemplate(XlsCFRuleTemplate.ContainsNoBlanks);
			XlExpression expression = new XlExpression();
			expression.Add(ptgRefN);
			expression.Add(new XlPtgFunc(funcTrim, XlPtgDataType.Value));
			expression.Add(new XlPtgFunc(funcLen, XlPtgDataType.Value));
			expression.Add(ptgZero);
			expression.Add(ptgGt);
			content.SetFirstFormula(expression.GetBytes(this));
		}
		#endregion
		#region SpecificText
		void PrepareRuleSpecificText(IXlsCFContentAdapter content, XlCondFmtRuleSpecificText rule) {
			content.SetRuleType(XlCondFmtType.Expression);
			content.SetOperator((XlCondFmtOperator)0);
			content.SetRuleTemplate(XlsCFRuleTemplate.ContainsText);
			XlExpression expression = GetRuleFormula(rule);
			content.SetFirstFormula(expression.GetBytes(this));
			content.SetTextRule((XlCondFmtSpecificTextType)rule.RuleType);
		}
		XlExpression GetRuleFormula(XlCondFmtRuleSpecificText rule) {
			XlExpression result = new XlExpression();
			XlPtgStr ptgStr = new XlPtgStr(rule.Text);
			switch(rule.RuleType) {
				case XlCondFmtType.BeginsWith:
					result.Add(ptgRefN);
					result.Add(ptgStr);
					result.Add(new XlPtgFunc(funcLen, XlPtgDataType.Value));
					result.Add(new XlPtgFuncVar(funcLeft, 2, XlPtgDataType.Value));
					result.Add(ptgStr);
					result.Add(ptgEq);
					break;
				case XlCondFmtType.ContainsText:
					result.Add(ptgStr);
					result.Add(ptgRefN);
					result.Add(new XlPtgFuncVar(funcSearch, 2, XlPtgDataType.Value));
					result.Add(new XlPtgFunc(funcIsError, XlPtgDataType.Value));
					result.Add(new XlPtgFunc(funcNot, XlPtgDataType.Value));
					break;
				case XlCondFmtType.EndsWith:
					result.Add(ptgRefN);
					result.Add(ptgStr);
					result.Add(new XlPtgFunc(funcLen, XlPtgDataType.Value));
					result.Add(new XlPtgFuncVar(funcRight, 2, XlPtgDataType.Value));
					result.Add(ptgStr);
					result.Add(ptgEq);
					break;
				case XlCondFmtType.NotContainsText:
					result.Add(ptgStr);
					result.Add(ptgRefN);
					result.Add(new XlPtgFuncVar(funcSearch, 2, XlPtgDataType.Value));
					result.Add(new XlPtgFunc(funcIsError, XlPtgDataType.Value));
					break;
			}
			return result;
		}
		#endregion
		#region UniqueDuplicates
		void PrepareRuleUniqueDuplicates(IXlsCFContentAdapter content, IList<XlCellRange> ranges, XlCondFmtRuleWithFormatting rule, bool unique) {
			content.SetRuleType(XlCondFmtType.Expression);
			content.SetOperator((XlCondFmtOperator)0);
			content.SetRuleTemplate(unique ? XlsCFRuleTemplate.UniqueValues : XlsCFRuleTemplate.DuplicateValues);
			XlExpression expression = CreateUniqueDuplicatesExpression(ranges, unique);
			content.SetFirstFormula(expression.GetBytes(this));
		}
		XlExpression CreateUniqueDuplicatesExpression(IList<XlCellRange> ranges, bool isUnique) {
			XlExpression result = new XlExpression();
			int count = ranges.Count;
			for(int i = 0; i < count; i++) {
				result.Add(new XlPtgArea(ranges[i].AsAbsolute()));
				result.Add(ptgAttrSpace);
				result.Add(ptgRefN);
				result.Add(new XlPtgFunc(funcCountIf, XlPtgDataType.Value));
			}
			for(int i = 0; i < (count - 1); i++)
				result.Add(ptgAdd);
			result.Add(ptgOne);
			if(isUnique)
				result.Add(ptgEq);
			else
				result.Add(ptgGt);
			result.Add(ptgRefN);
			result.Add(new XlPtgFunc(funcIsBlank, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcNot, XlPtgDataType.Value));
			result.Add(new XlPtgFuncVar(funcAnd, 2, XlPtgDataType.Value));
			return result;
		}
		#endregion
		#region Top10
		void PrepareRuleTop10(IXlsCFContentAdapter content, IList<XlCellRange> ranges, XlCondFmtRuleTop10 rule) {
			content.SetRuleType(XlCondFmtType.Top10);
			content.SetOperator((XlCondFmtOperator)0);
			content.SetRuleTemplate(XlsCFRuleTemplate.Filter);
			content.SetFilterTop(!rule.Bottom);
			content.SetFilterPercent(rule.Percent);
			content.SetFilterValue(rule.Rank);
			XlExpression expression = CreateTop10Expression(ranges, rule);
			content.SetFirstFormula(expression.GetBytes(this));
		}
		XlExpression CreateTop10Expression(IList<XlCellRange> ranges, XlCondFmtRuleTop10 rule) {
			XlExpression result = new XlExpression();
			if(ranges.Count > 1)
				return result;
			XlPtgArea ptgArea = new XlPtgArea(ranges[0].AsAbsolute());
			if(!rule.Percent && !rule.Bottom) {
				result.Add(ptgArea);
				result.Add(ptgParen);
				result.Add(ptgAttrSpace);
				result.Add(new XlPtgInt(rule.Rank));
				result.Add(ptgArea);
				result.Add(new XlPtgFuncVar(funcCount, 1, XlPtgDataType.Value));
				result.Add(new XlPtgFuncVar(funcMin, 2, XlPtgDataType.Value));
				result.Add(new XlPtgFunc(funcLarge, XlPtgDataType.Value));
				result.Add(ptgRefN);
				result.Add(ptgLe);
			}
			else if(!rule.Percent) {
				result.Add(ptgArea);
				result.Add(ptgParen);
				result.Add(ptgAttrSpace);
				result.Add(new XlPtgInt(rule.Rank));
				result.Add(ptgArea);
				result.Add(new XlPtgFuncVar(funcCount, 1, XlPtgDataType.Value));
				result.Add(new XlPtgFuncVar(funcMin, 2, XlPtgDataType.Value));
				result.Add(new XlPtgFunc(funcSmall, XlPtgDataType.Value));
				result.Add(ptgRefN);
				result.Add(ptgGe);
			}
			else if(!rule.Bottom) {
				result.Add(ptgArea);
				result.Add(new XlPtgFuncVar(funcCount, 1, XlPtgDataType.Value));
				result.Add(new XlPtgInt(rule.Rank));
				result.Add(ptgPercent);
				result.Add(ptgMul);
				result.Add(new XlPtgFunc(funcInt, XlPtgDataType.Value));
				result.Add(ptgZero);
				result.Add(ptgGt);
				result.Add(ptgArea);
				result.Add(ptgArea);
				result.Add(new XlPtgFuncVar(funcCount, 1, XlPtgDataType.Value));
				result.Add(new XlPtgInt(rule.Rank));
				result.Add(ptgPercent);
				result.Add(ptgMul);
				result.Add(new XlPtgFunc(funcInt, XlPtgDataType.Value));
				result.Add(new XlPtgFunc(funcLarge, XlPtgDataType.Value));
				result.Add(ptgAttrSpace);
				result.Add(ptgArea);
				result.Add(new XlPtgFuncVar(funcMax, 1, XlPtgDataType.Value));
				result.Add(new XlPtgFuncVar(funcIf, 3, XlPtgDataType.Value));
				result.Add(ptgRefN);
				result.Add(ptgLe);
			}
			else {
				result.Add(ptgArea);
				result.Add(new XlPtgFuncVar(funcCount, 1, XlPtgDataType.Value));
				result.Add(new XlPtgInt(rule.Rank));
				result.Add(ptgPercent);
				result.Add(ptgMul);
				result.Add(new XlPtgFunc(funcInt, XlPtgDataType.Value));
				result.Add(ptgZero);
				result.Add(ptgGe);
				result.Add(ptgArea);
				result.Add(ptgArea);
				result.Add(new XlPtgFuncVar(funcCount, 1, XlPtgDataType.Value));
				result.Add(new XlPtgInt(rule.Rank));
				result.Add(ptgPercent);
				result.Add(ptgMul);
				result.Add(new XlPtgFunc(funcInt, XlPtgDataType.Value));
				result.Add(new XlPtgFunc(funcSmall, XlPtgDataType.Value));
				result.Add(ptgAttrSpace);
				result.Add(ptgArea);
				result.Add(new XlPtgFuncVar(funcMin, 1, XlPtgDataType.Value));
				result.Add(new XlPtgFuncVar(funcIf, 3, XlPtgDataType.Value));
				result.Add(ptgRefN);
				result.Add(ptgGe);
			}
			return result;
		}
		#endregion
		#region TimePeriod
		void PrepareRuleTimePeriod(IXlsCFContentAdapter content, XlCondFmtRuleTimePeriod rule) {
			content.SetRuleType(XlCondFmtType.Expression);
			content.SetOperator((XlCondFmtOperator)0);
			content.SetRuleTemplate(GetTimePeriodTemplate(rule));
			XlExpression expression = GetTimePeriodFormula(rule);
			content.SetFirstFormula(expression.GetBytes(this));
		}
		XlsCFRuleTemplate GetTimePeriodTemplate(XlCondFmtRuleTimePeriod rule) {
			switch(rule.TimePeriod) {
				case XlCondFmtTimePeriod.Last7Days:
					return XlsCFRuleTemplate.Last7Days;
				case XlCondFmtTimePeriod.LastMonth:
					return XlsCFRuleTemplate.LastMonth;
				case XlCondFmtTimePeriod.LastWeek:
					return XlsCFRuleTemplate.LastWeek;
				case XlCondFmtTimePeriod.NextMonth:
					return XlsCFRuleTemplate.NextMonth;
				case XlCondFmtTimePeriod.NextWeek:
					return XlsCFRuleTemplate.NextWeek;
				case XlCondFmtTimePeriod.ThisMonth:
					return XlsCFRuleTemplate.ThisMonth;
				case XlCondFmtTimePeriod.ThisWeek:
					return XlsCFRuleTemplate.ThisWeek;
				case XlCondFmtTimePeriod.Today:
					return XlsCFRuleTemplate.Today;
				case XlCondFmtTimePeriod.Tomorrow:
					return XlsCFRuleTemplate.Tomorrow;
				case XlCondFmtTimePeriod.Yesterday:
					return XlsCFRuleTemplate.Yesterday;
			}
			return XlsCFRuleTemplate.Formula;
		}
		XlExpression GetTimePeriodFormula(XlCondFmtRuleTimePeriod rule) {
			XlExpression result = new XlExpression();
			switch(rule.TimePeriod) {
				case XlCondFmtTimePeriod.Today:
				case XlCondFmtTimePeriod.Yesterday:
				case XlCondFmtTimePeriod.Tomorrow:
					CreateDayExpression(result, rule.TimePeriod);
					break;
				case XlCondFmtTimePeriod.Last7Days:
					CreateLast7DaysExpression(result);
					break;
				case XlCondFmtTimePeriod.LastWeek:
					CreateLastWeekExpression(result);
					break;
				case XlCondFmtTimePeriod.ThisWeek:
					CreateThisWeekExpression(result);
					break;
				case XlCondFmtTimePeriod.NextWeek:
					CreateNextWeekExpression(result);
					break;
				case XlCondFmtTimePeriod.LastMonth:
					CreateLastMonthExpression(result);
					break;
				case XlCondFmtTimePeriod.ThisMonth:
					CreateThisMonthExpression(result);
					break;
				case XlCondFmtTimePeriod.NextMonth:
					CreateNextMonthExpression(result);
					break;
			}
			return result;
		}
		void CreateNextMonthExpression(XlExpression result) {
			result.Add(ptgAttrSemi);
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcMonth, XlPtgDataType.Value));
			result.Add(new XlPtgInt(12));
			result.Add(ptgEq);
			result.Add(ptgRefN);
			result.Add(new XlPtgFunc(funcMonth, XlPtgDataType.Value));
			result.Add(ptgOne);
			result.Add(ptgEq);
			result.Add(ptgRefN);
			result.Add(new XlPtgFunc(funcYear, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcYear, XlPtgDataType.Value));
			result.Add(ptgOne);
			result.Add(ptgAdd);
			result.Add(ptgParen);
			result.Add(ptgEq);
			result.Add(new XlPtgFuncVar(funcAnd, 2, XlPtgDataType.Value));
			result.Add(ptgRefN);
			result.Add(new XlPtgFunc(funcMonth, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcMonth, XlPtgDataType.Value));
			result.Add(ptgOne);
			result.Add(ptgAdd);
			result.Add(ptgParen);
			result.Add(ptgEq);
			result.Add(ptgRefN);
			result.Add(new XlPtgFunc(funcYear, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcYear, XlPtgDataType.Value));
			result.Add(ptgEq);
			result.Add(new XlPtgFuncVar(funcAnd, 2, XlPtgDataType.Value));
			result.Add(new XlPtgFuncVar(funcIf, 3, XlPtgDataType.Value));
		}
		void CreateLastMonthExpression(XlExpression result) {
			result.Add(ptgAttrSemi);
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcMonth, XlPtgDataType.Value));
			result.Add(ptgOne);
			result.Add(ptgEq);
			result.Add(ptgRefN);
			result.Add(new XlPtgFunc(funcMonth, XlPtgDataType.Value));
			result.Add(new XlPtgInt(12));
			result.Add(ptgEq);
			result.Add(ptgRefN);
			result.Add(new XlPtgFunc(funcYear, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcYear, XlPtgDataType.Value));
			result.Add(ptgOne);
			result.Add(ptgSub);
			result.Add(ptgParen);
			result.Add(ptgEq);
			result.Add(new XlPtgFuncVar(funcAnd, 2, XlPtgDataType.Value));
			result.Add(ptgRefN);
			result.Add(new XlPtgFunc(funcMonth, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcMonth, XlPtgDataType.Value));
			result.Add(ptgOne);
			result.Add(ptgSub);
			result.Add(ptgParen);
			result.Add(ptgEq);
			result.Add(ptgRefN);
			result.Add(new XlPtgFunc(funcYear, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcYear, XlPtgDataType.Value));
			result.Add(ptgEq);
			result.Add(new XlPtgFuncVar(funcAnd, 2, XlPtgDataType.Value));
			result.Add(new XlPtgFuncVar(funcIf, 3, XlPtgDataType.Value));
		}
		void CreateThisMonthExpression(XlExpression result) {
			result.Add(ptgAttrSemi);
			result.Add(ptgRefN);
			result.Add(new XlPtgFunc(funcMonth, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcMonth, XlPtgDataType.Value));
			result.Add(ptgEq);
			result.Add(ptgRefN);
			result.Add(new XlPtgFunc(funcYear, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcYear, XlPtgDataType.Value));
			result.Add(ptgEq);
			result.Add(new XlPtgFuncVar(funcAnd, 2, XlPtgDataType.Value));
		}
		void CreateNextWeekExpression(XlExpression result) {
			result.Add(ptgAttrSemi);
			result.Add(ptgRefN);
			result.Add(ptgZero);
			result.Add(new XlPtgFunc(funcRoundDown, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(ptgSub);
			result.Add(new XlPtgInt(7));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFuncVar(funcWeekday, 1, XlPtgDataType.Reference));
			result.Add(ptgSub);
			result.Add(ptgParen);
			result.Add(ptgGt);
			result.Add(ptgRefN);
			result.Add(ptgZero);
			result.Add(new XlPtgFunc(funcRoundDown, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(ptgSub);
			result.Add(new XlPtgInt(15));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFuncVar(funcWeekday, 1, XlPtgDataType.Reference));
			result.Add(ptgSub);
			result.Add(ptgParen);
			result.Add(ptgLt);
			result.Add(new XlPtgFuncVar(funcAnd, 2, XlPtgDataType.Value));
		}
		void CreateThisWeekExpression(XlExpression result) {
			result.Add(ptgAttrSemi);
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(ptgRefN);
			result.Add(ptgZero);
			result.Add(new XlPtgFunc(funcRoundDown, XlPtgDataType.Value));
			result.Add(ptgSub);
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFuncVar(funcWeekday, 1, XlPtgDataType.Reference));
			result.Add(ptgOne);
			result.Add(ptgSub);
			result.Add(ptgLe);
			result.Add(ptgRefN);
			result.Add(ptgZero);
			result.Add(new XlPtgFunc(funcRoundDown, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(ptgSub);
			result.Add(new XlPtgInt(7));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFuncVar(funcWeekday, 1, XlPtgDataType.Reference));
			result.Add(ptgSub);
			result.Add(ptgLe);
			result.Add(new XlPtgFuncVar(funcAnd, 2, XlPtgDataType.Value));
		}
		void CreateLastWeekExpression(XlExpression result) {
			result.Add(ptgAttrSemi);
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(ptgRefN);
			result.Add(ptgZero);
			result.Add(new XlPtgFunc(funcRoundDown, XlPtgDataType.Value));
			result.Add(ptgSub);
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFuncVar(funcWeekday, 1, XlPtgDataType.Reference));
			result.Add(ptgParen);
			result.Add(ptgGe);
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(ptgRefN);
			result.Add(ptgZero);
			result.Add(new XlPtgFunc(funcRoundDown, XlPtgDataType.Value));
			result.Add(ptgSub);
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(new XlPtgFuncVar(funcWeekday, 1, XlPtgDataType.Reference));
			result.Add(new XlPtgInt(7));
			result.Add(ptgAdd);
			result.Add(ptgParen);
			result.Add(ptgLt);
			result.Add(new XlPtgFuncVar(funcAnd, 2, XlPtgDataType.Value));
		}
		void CreateLast7DaysExpression(XlExpression result) {
			result.Add(ptgAttrSemi);
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(ptgRefN);
			result.Add(ptgOne);
			result.Add(new XlPtgFunc(funcFloor, XlPtgDataType.Value));
			result.Add(ptgSub);
			result.Add(new XlPtgInt(6));
			result.Add(ptgLe);
			result.Add(ptgRefN);
			result.Add(ptgOne);
			result.Add(new XlPtgFunc(funcFloor, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			result.Add(ptgLe);
			result.Add(new XlPtgFuncVar(funcAnd, 2, XlPtgDataType.Value));
		}
		void CreateDayExpression(XlExpression result, XlCondFmtTimePeriod period) {
			result.Add(ptgAttrSemi);
			result.Add(ptgRefN);
			result.Add(ptgOne);
			result.Add(new XlPtgFunc(funcFloor, XlPtgDataType.Value));
			result.Add(new XlPtgFunc(funcToday, XlPtgDataType.Value));
			if(period != XlCondFmtTimePeriod.Today) {
				result.Add(ptgOne);
				if(period == XlCondFmtTimePeriod.Tomorrow)
					result.Add(ptgAdd);
				else
					result.Add(ptgSub);
			}
			result.Add(ptgEq);
		}
		#endregion
		#region DataBar
		void PrepareRuleDataBar(XlsContentCF12 content, XlCondFmtRuleDataBar rule, XlConditionalFormatting cf) {
			content.RuleType = XlCondFmtType.DataBar;
			content.Operator = (XlCondFmtOperator)0;
			content.RuleTemplate = XlsCFRuleTemplate.DataBar;
			content.Format.IsEmpty = true;
			content.DataBarParams.BarColor = rule.FillColor;
			expressionContext.CurrentCell = cf.GetTopLeftCell();
			PrepareValueObject(content.DataBarParams.MinValue, rule.MinValue);
			PrepareValueObject(content.DataBarParams.MaxValue, rule.MaxValue);
			content.DataBarParams.PercentMin = rule.MinLength;
			content.DataBarParams.PercentMax = rule.MaxLength;
			content.DataBarParams.ShowBarOnly = !rule.ShowValues;
		}
		#endregion
		#region IconSet
		bool PrepareRuleIconSet(XlsContentCF12 content, XlCondFmtRuleIconSet rule, XlConditionalFormatting cf) {
			if(!XlsCondFmtHelper.IsSupportedIconSet(rule.IconSetType))
				return false;
			content.RuleType = XlCondFmtType.IconSet;
			content.Operator = (XlCondFmtOperator)0;
			content.RuleTemplate = XlsCFRuleTemplate.IconSet;
			content.Format.IsEmpty = true;
			content.IconSetParams.IconSet = rule.IconSetType;
			content.IconSetParams.IconsOnly = !rule.ShowValues;
			content.IconSetParams.Reverse = rule.Reverse;
			expressionContext.CurrentCell = cf.GetTopLeftCell();
			foreach(XlCondFmtValueObject value in rule.Values) {
				XlsCondFmtIconThreshold threshold = new XlsCondFmtIconThreshold();
				PrepareValueObject(threshold, value);
				threshold.EqualPass = true;
				content.IconSetParams.Thresholds.Add(threshold);
			}
			return true;
		}
		#endregion
		#region ColorScale
		void PrepareRuleColorScale(XlsContentCF12 content, XlCondFmtRuleColorScale rule, XlConditionalFormatting cf) {
			content.RuleType = XlCondFmtType.ColorScale;
			content.Operator = (XlCondFmtOperator)0;
			content.RuleTemplate = XlsCFRuleTemplate.ColorScale;
			content.Format.IsEmpty = true;
			content.ColorScaleParams.ColorScaleType = rule.ColorScaleType;
			expressionContext.CurrentCell = cf.GetTopLeftCell();
			PrepareValueObject(content.ColorScaleParams.MinValue, rule.MinValue);
			PrepareValueObject(content.ColorScaleParams.MidValue, rule.MidpointValue);
			PrepareValueObject(content.ColorScaleParams.MaxValue, rule.MaxValue);
			content.ColorScaleParams.MinColor = rule.MinColor;
			content.ColorScaleParams.MidColor = rule.MidpointColor;
			content.ColorScaleParams.MaxColor = rule.MaxColor;
		}
		#endregion
		byte[] GetFormulaBytes(XlValueObject value) {
			if(value.IsEmpty)
				return null;
			if(value.IsRange)
				return GetFormulaBytes(value.RangeValue);
			if(value.IsExpression)
				return value.Expression.ToXlsExpression().GetBytes(this);
			if(value.IsFormula) {
				if(formulaParser == null)
					throw new InvalidOperationException("Formula parser required for this operation.");
				expressionContext.ReferenceMode = XlCellReferenceMode.Offset;
				expressionContext.ExpressionStyle = XlExpressionStyle.Normal;
				XlExpression expression = formulaParser.Parse(value.Formula, expressionContext);
				if(expression == null)
					throw new InvalidOperationException(string.Format("Can't parse formula '{0}'.", value.Formula));
				return expression.ToXlsExpression().GetBytes(this);
			}
			return GetFormulaBytes(value.VariantValue);
		}
		void PrepareValueObject(XlsCondFmtValueObject obj, XlCondFmtValueObject value) {
			switch(value.ObjectType) {
				case XlCondFmtValueObjectType.AutoMax:
				case XlCondFmtValueObjectType.Max:
					obj.ObjectType = XlCondFmtValueObjectType.Max;
					break;
				case XlCondFmtValueObjectType.AutoMin:
				case XlCondFmtValueObjectType.Min:
					obj.ObjectType = XlCondFmtValueObjectType.Min;
					break;
				default:
					obj.ObjectType = value.ObjectType;
					if(value.ObjectType == XlCondFmtValueObjectType.Formula || value.Value.IsRange || !value.Value.IsNumeric)
						obj.FormulaBytes = GetFormulaBytes(value.Value);
					else
						obj.Value = value.Value.NumericValue;
					break;
			}
		}
		#endregion
		#region PrepareDxf
		void PrepareDxf(XlsDxfN dxf, XlCondFmtRuleWithFormatting rule) {
			if(rule == null)
				return;
			XlDifferentialFormatting formatting = rule.Formatting;
			if(formatting == null)
				return;
			PrepareDxfNumberFormat(dxf, formatting);
			PrepareCondFmtDxfFont(dxf, formatting.Font);
			PrepareDxfAlign(dxf, formatting.Alignment);
			PrepareDxfBorder(dxf, formatting.Border);
			PrepareDxfFill(dxf, formatting.Fill);
		}
		void PrepareDxfNumberFormat(XlsDxfN dxf, XlDifferentialFormatting formatting) {
			if(string.IsNullOrEmpty(formatting.NetFormatString))
				return;
			ExcelNumberFormat excelNumFmt = ConvertNetFormatStringToXlFormatCode(formatting.NetFormatString, formatting.IsDateTimeFormatString);
			if(excelNumFmt != null) {
				dxf.SetIsEmpty(false);
				dxf.FlagsInfo.IncludeNumberFormat = true;
				dxf.NumberFormatInfo = new XlsDxfNumUser() { NumberFormatCode = excelNumFmt.FormatString };
			}
		}
		protected void PrepareDxfFont(XlsDxfN dxf, XlFont font) {
			if(font == null)
				return;
			dxf.SetIsEmpty(false);
			dxf.FlagsInfo.IncludeFont = true;
			XlsDxfFont dxfFont = dxf.FontInfo;
			if(font.Bold != defaultFont.Bold) {
				dxfFont.FontBold = font.Bold;
				dxfFont.FontBoldNinch = false;
			}
			if(font.Italic != defaultFont.Italic) {
				dxfFont.FontItalic = font.Italic;
				dxfFont.FontItalicNinch = false;
			}
			if(font.StrikeThrough != defaultFont.StrikeThrough) {
				dxfFont.FontStrikeThrough = font.StrikeThrough;
				dxfFont.FontStrikeThroughNinch = false;
			}
			if(font.Underline != defaultFont.Underline) {
				dxfFont.FontUnderline = font.Underline;
				dxfFont.FontUnderlineNinch = false;
			}
			if(font.Script != defaultFont.Script) {
				dxfFont.FontScript = font.Script;
				dxfFont.FontScriptNinch = false;
			}
			if(font.Size > 0)
				dxfFont.FontSize = Math.Min(8191, (int)(font.Size * 20.0));
			dxfFont.FontColorIndex = palette.GetFontColorIndex(font.Color, currentDocument.Theme);
			if(!font.Color.IsEmpty)
				dxf.AddExtProperty(new XfPropFullColor(XfExtType.TextColor, font.Color));
			dxfFont.FontName = font.Name;
			dxfFont.FontFamily = (int)font.FontFamily;
			dxfFont.FontCharset = font.Charset;
			dxfFont.FontScript = font.Script;
			if(font.SchemeStyle != XlFontSchemeStyles.None)
				dxf.AddExtProperty(new XfPropByte(XfExtType.FontScheme, (byte)(font.SchemeStyle == XlFontSchemeStyles.Major ? 1 : 2)));
			dxfFont.IsDefaultFont = false;
		}
		void PrepareCondFmtDxfFont(XlsDxfN dxf, XlFont font) {
			if(font == null)
				return;
			dxf.SetIsEmpty(false);
			dxf.FlagsInfo.IncludeFont = true;
			XlsDxfFont dxfFont = dxf.FontInfo;
			if(font.Bold != defaultFont.Bold) {
				dxfFont.FontBold = font.Bold;
				dxfFont.FontBoldNinch = false;
			}
			if(font.Italic != defaultFont.Italic) {
				dxfFont.FontItalic = font.Italic;
				dxfFont.FontItalicNinch = false;
			}
			if(font.StrikeThrough != defaultFont.StrikeThrough) {
				dxfFont.FontStrikeThrough = font.StrikeThrough;
				dxfFont.FontStrikeThroughNinch = false;
			}
			if(font.Underline != defaultFont.Underline) {
				dxfFont.FontUnderline = font.Underline;
				dxfFont.FontUnderlineNinch = false;
			}
			if(font.Script != defaultFont.Script) {
				dxfFont.FontScript = font.Script;
				dxfFont.FontScriptNinch = false;
			}
			dxfFont.FontColorIndex = palette.GetFontColorIndex(font.Color, currentDocument.Theme);
			if(!font.Color.IsEmpty)
				dxf.AddExtProperty(new XfPropFullColor(XfExtType.TextColor, font.Color));
			dxfFont.IsDefaultFont = false;
		}
		void PrepareDxfAlign(XlsDxfN dxf, XlCellAlignment align) {
			if(align == null)
				return;
			dxf.SetIsEmpty(false);
			dxf.FlagsInfo.IncludeAlignment = true;
			if(defaultAlignment.HorizontalAlignment != align.HorizontalAlignment) {
				dxf.AlignmentInfo.HorizontalAlignment = align.HorizontalAlignment;
				dxf.FlagsInfo.AlignmentHorizontalNinch = false;
			}
			if(defaultAlignment.VerticalAlignment != align.VerticalAlignment) {
				dxf.AlignmentInfo.VerticalAlignment = align.VerticalAlignment;
				dxf.FlagsInfo.AlignmentVerticalNinch = false;
			}
			if(defaultAlignment.WrapText != align.WrapText) {
				dxf.AlignmentInfo.WrapText = align.WrapText;
				dxf.FlagsInfo.AlignmentWrapTextNinch = false;
			}
			if(defaultAlignment.Indent != align.Indent) {
				dxf.AlignmentInfo.Indent = align.Indent;
				dxf.FlagsInfo.AlignmentIndentNinch = false;
			}
			dxf.AlignmentInfo.RelativeIndent = align.RelativeIndent;
			if(defaultAlignment.JustifyLastLine != align.JustifyLastLine) {
				dxf.AlignmentInfo.JustifyLastLine = align.JustifyLastLine;
				dxf.FlagsInfo.AlignmentJustifyLastLineNinch = false;
			}
			if(defaultAlignment.ShrinkToFit != align.ShrinkToFit) {
				dxf.AlignmentInfo.ShrinkToFit = align.ShrinkToFit;
				dxf.FlagsInfo.AlignmentShrinkToFitNinch = false;
			}
			if(defaultAlignment.ReadingOrder != align.ReadingOrder) {
				dxf.AlignmentInfo.ReadingOrder = align.ReadingOrder;
				dxf.FlagsInfo.AlignmentReadingOrderNinch = false;
				dxf.FlagsInfo.AlignmentReadingOrderZeroInited = true;
			}
		}
		void PrepareDxfBorder(XlsDxfN dxf, XlBorder border) {
			if(border == null)
				return;
			dxf.SetIsEmpty(false);
			dxf.FlagsInfo.IncludeBorder = true;
			if(border.LeftLineStyle != XlBorderLineStyle.None) {
				dxf.BorderInfo.LeftLineStyle = border.LeftLineStyle;
				dxf.BorderInfo.LeftColorIndex = palette.GetColorIndex(border.LeftColor.ConvertToRgb(currentDocument.Theme));
				dxf.FlagsInfo.BorderLeftNinch = false;
				if(!border.LeftColor.IsEmpty)
					dxf.AddExtProperty(new XfPropFullColor(XfExtType.LeftBorderColor, border.LeftColor));
			}
			if(border.RightLineStyle != XlBorderLineStyle.None) {
				dxf.BorderInfo.RightLineStyle = border.RightLineStyle;
				dxf.BorderInfo.RightColorIndex = palette.GetColorIndex(border.RightColor.ConvertToRgb(currentDocument.Theme));
				dxf.FlagsInfo.BorderRightNinch = false;
				if(!border.RightColor.IsEmpty)
					dxf.AddExtProperty(new XfPropFullColor(XfExtType.RightBorderColor, border.RightColor));
			}
			if(border.TopLineStyle != XlBorderLineStyle.None) {
				dxf.BorderInfo.TopLineStyle = border.TopLineStyle;
				dxf.BorderInfo.TopColorIndex = palette.GetColorIndex(border.TopColor.ConvertToRgb(currentDocument.Theme));
				dxf.FlagsInfo.BorderTopNinch = false;
				if(!border.TopColor.IsEmpty)
					dxf.AddExtProperty(new XfPropFullColor(XfExtType.TopBorderColor, border.TopColor));
			}
			if(border.BottomLineStyle != XlBorderLineStyle.None) {
				dxf.BorderInfo.BottomLineStyle = border.BottomLineStyle;
				dxf.BorderInfo.BottomColorIndex = palette.GetColorIndex(border.BottomColor.ConvertToRgb(currentDocument.Theme));
				dxf.FlagsInfo.BorderBottomNinch = false;
				if(!border.BottomColor.IsEmpty)
					dxf.AddExtProperty(new XfPropFullColor(XfExtType.BottomBorderColor, border.BottomColor));
			}
			if(border.DiagonalUpLineStyle != XlBorderLineStyle.None) {
				dxf.BorderInfo.DiagonalUp = true;
				dxf.BorderInfo.DiagonalLineStyle = border.DiagonalUpLineStyle;
				dxf.BorderInfo.DiagonalColorIndex = palette.GetColorIndex(border.DiagonalColor.ConvertToRgb(currentDocument.Theme));
				dxf.FlagsInfo.BorderDiagonalUpNinch = false;
			}
			if(border.DiagonalDownLineStyle != XlBorderLineStyle.None) {
				dxf.BorderInfo.DiagonalDown = true;
				dxf.BorderInfo.DiagonalLineStyle = border.DiagonalDownLineStyle;
				dxf.BorderInfo.DiagonalColorIndex = palette.GetColorIndex(border.DiagonalColor.ConvertToRgb(currentDocument.Theme));
				dxf.FlagsInfo.BorderDiagonalDownNinch = false;
			}
			if(!border.DiagonalColor.IsEmpty && border.DiagonalLineStyle != XlBorderLineStyle.None)
				dxf.AddExtProperty(new XfPropFullColor(XfExtType.DiagonalBorderColor, border.DiagonalColor));
			dxf.FlagsInfo.NewBorder = border.Outline;
		}
		void PrepareDxfFill(XlsDxfN dxf, XlFill fill) {
			if(fill == null)
				return;
			dxf.SetIsEmpty(false);
			dxf.FlagsInfo.IncludeFill = true;
			dxf.FillInfo.PatternType = fill.PatternType;
			dxf.FlagsInfo.FillPatternTypeNinch = false;
			dxf.FillInfo.ForeColorIndex = palette.GetForegroundColorIndex(fill.ForeColor, currentDocument.Theme);
			dxf.FlagsInfo.FillForegroundColorNinch = false;
			if(!fill.ForeColor.IsEmpty)
				dxf.AddExtProperty(new XfPropFullColor(XfExtType.ForegroundColor, fill.ForeColor));
			dxf.FillInfo.BackColorIndex = palette.GetBackgroundColorIndex(fill.BackColor, currentDocument.Theme);
			dxf.FlagsInfo.FillBackgroundColorNinch = false;
			if(!fill.BackColor.IsEmpty)
				dxf.AddExtProperty(new XfPropFullColor(XfExtType.BackgroundColor, fill.BackColor));
		}
		#endregion
		#region Utils
		XlsRef8 GetBoundRange(IList<XlCellRange> ranges) {
			XlsRef8 result = null;
			for(int i = 0; i < ranges.Count; i++) {
				XlsRef8 subRange = XlsRef8.FromRange(ranges[i]);
				if(result == null)
					result = subRange;
				else
					result.Union(subRange);
			}
			return result;
		}
		int GetRuleCount(XlConditionalFormatting cf) {
			int result = 0;
			foreach(XlCondFmtRule rule in cf.Rules) {
				XlCondFmtRuleIconSet ruleIconSet = rule as XlCondFmtRuleIconSet;
				if(ruleIconSet == null)
					result++;
				else if(XlsCondFmtHelper.IsSupportedIconSet(ruleIconSet.IconSetType))
					result++;
			}
			return result;
		}
		XlsContentCondFmt CreateCFContentRoot(bool isCondFmt12) {
			if(isCondFmt12)
				return new XlsContentCondFmt12();
			return new XlsContentCondFmt();
		}
		bool IsCondFmt12(XlConditionalFormatting cf) {
			foreach(XlCondFmtRule rule in cf.Rules)
				if(!IsCondFmt12(cf, rule as XlCondFmtRuleWithFormatting))
					return false;
			return true;
		}
		bool IsCondFmt12(XlConditionalFormatting cf, XlCondFmtRuleWithFormatting rule) {
			if(rule == null)
				return true;
			if(rule.Formatting != null && !string.IsNullOrEmpty(rule.Formatting.NetFormatString))
				return true;
			if(rule.RuleType == XlCondFmtType.AboveOrBelowAverage && cf.Ranges.Count > 1)
				return ((XlCondFmtRuleAboveAverage)rule).StdDev > 0;
			return false;
		}
		bool HasFormulaInCF12(XlCondFmtRuleWithFormatting rule) {
			if(rule == null)
				return false;
			if(rule.RuleType == XlCondFmtType.Top10)
				return false;
			return true;
		}
		#endregion
	}
	#endregion
}
