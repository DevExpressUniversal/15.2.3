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
using System.Collections;
using System.ComponentModel;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BarCode.Native;
using System.Collections.Generic;
namespace DevExpress.XtraPrinting.BarCode {
	public class Code128Generator : BarCodeGeneratorBase {
		#region static
		protected static ArrayList charSetA = new ArrayList(107);
		static ArrayList charSetB = new ArrayList(107);
		static ArrayList charSetC = new ArrayList(107);
		protected const char FNC1 = (char)102;
		internal static string fnc1Char;
		static string validCharsetA = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_" +
			"\x01\x02\x03\x04\x05\x06\x07\x08\x09\x0A\x0B\x0C\x0D\x0E\x0F\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1A\x1B\x1C\x1D\x1E\x1F";
		static string validCharsetB = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_" +
			"`abcdefghijklmnopqrstuvwxyz{|}~\x7F";
		static Hashtable charPattern = new Hashtable();
		const int stopSymbolIndex = 106;
		const int shiftToCharsetC = 99;
		const int shiftToCharsetB = 100;
		const int shiftToCharsetA = 101;
		static void FillCharPattern() {
			charPattern[(char)0] = "212222"; 
			charPattern[(char)1] = "222122"; 
			charPattern[(char)2] = "222221"; 
			charPattern[(char)3] = "121223"; 
			charPattern[(char)4] = "121322"; 
			charPattern[(char)5] = "131222"; 
			charPattern[(char)6] = "122213"; 
			charPattern[(char)7] = "122312"; 
			charPattern[(char)8] = "132212"; 
			charPattern[(char)9] = "221213"; 
			charPattern[(char)10] = "221312"; 
			charPattern[(char)11] = "231212"; 
			charPattern[(char)12] = "112232"; 
			charPattern[(char)13] = "122132"; 
			charPattern[(char)14] = "122231"; 
			charPattern[(char)15] = "113222"; 
			charPattern[(char)16] = "123122"; 
			charPattern[(char)17] = "123221"; 
			charPattern[(char)18] = "223211"; 
			charPattern[(char)19] = "221132"; 
			charPattern[(char)20] = "221231"; 
			charPattern[(char)21] = "213212"; 
			charPattern[(char)22] = "223112"; 
			charPattern[(char)23] = "312131"; 
			charPattern[(char)24] = "311222"; 
			charPattern[(char)25] = "321122"; 
			charPattern[(char)26] = "321221"; 
			charPattern[(char)27] = "312212"; 
			charPattern[(char)28] = "322112"; 
			charPattern[(char)29] = "322211"; 
			charPattern[(char)30] = "212123"; 
			charPattern[(char)31] = "212321"; 
			charPattern[(char)32] = "232121"; 
			charPattern[(char)33] = "111323"; 
			charPattern[(char)34] = "131123"; 
			charPattern[(char)35] = "131321"; 
			charPattern[(char)36] = "112313"; 
			charPattern[(char)37] = "132113"; 
			charPattern[(char)38] = "132311"; 
			charPattern[(char)39] = "211313"; 
			charPattern[(char)40] = "231113"; 
			charPattern[(char)41] = "231311"; 
			charPattern[(char)42] = "112133"; 
			charPattern[(char)43] = "112331"; 
			charPattern[(char)44] = "132131"; 
			charPattern[(char)45] = "113123"; 
			charPattern[(char)46] = "113321"; 
			charPattern[(char)47] = "133121"; 
			charPattern[(char)48] = "313121"; 
			charPattern[(char)49] = "211331"; 
			charPattern[(char)50] = "231131"; 
			charPattern[(char)51] = "213113"; 
			charPattern[(char)52] = "213311"; 
			charPattern[(char)53] = "213131"; 
			charPattern[(char)54] = "311123"; 
			charPattern[(char)55] = "311321"; 
			charPattern[(char)56] = "331121"; 
			charPattern[(char)57] = "312113"; 
			charPattern[(char)58] = "312311"; 
			charPattern[(char)59] = "332111"; 
			charPattern[(char)60] = "314111"; 
			charPattern[(char)61] = "221411"; 
			charPattern[(char)62] = "431111"; 
			charPattern[(char)63] = "111224"; 
			charPattern[(char)64] = "111422"; 
			charPattern[(char)65] = "121124"; 
			charPattern[(char)66] = "121421"; 
			charPattern[(char)67] = "141122"; 
			charPattern[(char)68] = "141221"; 
			charPattern[(char)69] = "112214"; 
			charPattern[(char)70] = "112412"; 
			charPattern[(char)71] = "122114"; 
			charPattern[(char)72] = "122411"; 
			charPattern[(char)73] = "142112"; 
			charPattern[(char)74] = "142211"; 
			charPattern[(char)75] = "241211"; 
			charPattern[(char)76] = "221114"; 
			charPattern[(char)77] = "413111"; 
			charPattern[(char)78] = "241112"; 
			charPattern[(char)79] = "134111"; 
			charPattern[(char)80] = "111242"; 
			charPattern[(char)81] = "121142"; 
			charPattern[(char)82] = "121241"; 
			charPattern[(char)83] = "114212"; 
			charPattern[(char)84] = "124112"; 
			charPattern[(char)85] = "124211"; 
			charPattern[(char)86] = "411212"; 
			charPattern[(char)87] = "421112"; 
			charPattern[(char)88] = "421211"; 
			charPattern[(char)89] = "212141"; 
			charPattern[(char)90] = "214121"; 
			charPattern[(char)91] = "412121"; 
			charPattern[(char)92] = "111143"; 
			charPattern[(char)93] = "111341"; 
			charPattern[(char)94] = "131141"; 
			charPattern[(char)95] = "114113"; 
			charPattern[(char)96] = "114311"; 
			charPattern[(char)97] = "411113"; 
			charPattern[(char)98] = "411311"; 
			charPattern[(char)99] = "113141"; 
			charPattern[(char)100] = "114131"; 
			charPattern[(char)101] = "311141"; 
			charPattern[(char)102] = "411131"; 
			charPattern[(char)103] = "211412"; 
			charPattern[(char)104] = "211214"; 
			charPattern[(char)105] = "211232"; 
			charPattern[(char)106] = "2331112"; 
		}
		static char[] Text2Indexes(char[] text, int from, int count, DevExpress.XtraPrinting.BarCode.Code128Charset cs) {
			switch(cs) {
				case DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetA:
					return Text2IndexesInternal(text, from, count, charSetA, charSetB);
				case DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetB:
					return Text2IndexesInternal(text, from, count, charSetB, charSetA);
				default:
					return Text2IndexesC(text, from, count);
			}
		}
		static void AppendFromString(ArrayList array, string text) {
			int count = text.Length;
			for(int i = 0; i < count; i++)
				array.Add(text[i]);
		}
		static void FillCharsetA() {
			string charSetA1 = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_";
			string charSetA2 = "\x01\x02\x03\x04\x05\x06\x07\x08\x09\x0A\x0B\x0C\x0D\x0E\x0F\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1A\x1B\x1C\x1D\x1E\x1F" +
				"\xF5\xF6\xF7\xF8\xF9\xFA\xFB\xFC\xFD\xFE\xFF"; 
			AppendFromString(charSetA, charSetA1);
			charSetA.Add('\x00');
			AppendFromString(charSetA, charSetA2);
		}
		static void FillCharsetB() {
			string charSetB1 = " !\"#$%&'()*+,-./0123456789:;<=>?@ABCDEFGHIJKLMNOPQRSTUVWXYZ[\\]^_" +
				"`abcdefghijklmnopqrstuvwxyz{|}~\x7F" +
				"\xF5\xF6\xF7\xF8\xF9\xFA\xFB\xFC\xFD\xFE\xFF"; 
			AppendFromString(charSetB, charSetB1);
		}
		static void FillCharsetC() {
			string charSetC1 = "\x01\x02\x03\x04\x05\x06\x07\x08\x09\x0A\x0B\x0C\x0D\x0E\x0F\x10\x11\x12\x13\x14\x15\x16\x17\x18\x19\x1A\x1B\x1C\x1D\x1E\x1F" +
				"\x20\x21\x22\x23\x24\x25\x26\x27\x28\x29\x2A\x2B\x2C\x2D\x2E\x2F\x30\x31\x32\x33\x34\x35\x36\x37\x38\x39\x3A\x3B\x3C\x3D\x3E\x3F" +
				"\x40\x41\x42\x43\x44\x45\x46\x47\x48\x49\x4A\x4B\x4C\x4D\x4E\x4F\x50\x51\x52\x53\x54\x55\x56\x57\x58\x59\x5A\x5B\x5C\x5D\x5E\x5F" +
				"\x60\x61\x62\x63" +
				"\xF9\xFA\xFB\xFC\xFD\xFE\xFF"; 
			charSetC.Add('\x00');
			AppendFromString(charSetC, charSetC1);
		}
		static Code128Generator() {
			FillCharPattern();
			FillCharsetA();
			FillCharsetB();
			FillCharsetC();
			System.Diagnostics.Debug.Assert(charSetA.Count == charSetB.Count);
			System.Diagnostics.Debug.Assert(charSetB.Count == charSetC.Count);
			fnc1Char = new string((char)charSetA[(int)FNC1], 1);
		}
		static char CalcCheckDigit(ArrayList text) {
			int sum = (int)(char)text[0];
			int count = text.Count;
			int weight = 1;
			for(int i = 1; i < count; i++, weight++)
				sum += (int)(char)text[i] * weight;
			return (char)(sum % 103);
		}
		static bool IsDigit(char ch) {
			return (int)ch >= (int)'0' && (int)ch <= (int)'9';
		}
		static char GetSwitchCharTo(DevExpress.XtraPrinting.BarCode.Code128Charset cs) {
			switch(cs) {
				case DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetA:
					return (char)shiftToCharsetA;
				case DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetB:
					return (char)shiftToCharsetB;
				default:
					return (char)shiftToCharsetC;
			}
		}
		static char[] Text2IndexesC(char[] text, int from, int count) {
			List<char> indexes = new List<char>(count / 2);
			bool half = false;
			int value = 0;
			int to = from + count;
			for(int i = from; i < to; i++) {
				char c = text[i];
				if(IsDigit(c)) {
					if(half) {
						value += Char2Int(c);
						indexes.Add((char)value);
					} else
						value = Char2Int(c) * 10;
					half = !half;
					continue;
				} else if(c == fnc1Char[0] && !half) {
					indexes.Add(FNC1);
					continue;
				}
				System.Diagnostics.Debug.Assert(false, "Invalid format for Code C");
			}
			return indexes.ToArray();
		}
		static char[] Text2IndexesInternal(char[] text, int from, int count, ArrayList baseCharSet, ArrayList alternativeCharset) {
			char[] indexes = new char[count];
			int to = from + count;
			for(int i = from, idx = 0; i < to; i++, idx++) {
				indexes[idx] = (char)baseCharSet.IndexOf(text[i]);
				if(indexes[idx] == 98) { 
					i++;
					idx++;
					if(i < to)
						indexes[idx] = (char)alternativeCharset.IndexOf(text[i]);
				}
			}
			return indexes;
		}
		static int FindSwitchToCharsetC(char[] text, int from, int sequenceLength) {
			int digitsCount = 0;
			int charSetCFrom = -1;
			int to = from + sequenceLength;
			for(int i = from; i < to; i++) {
				if(IsDigit(text[i])) {
					if(digitsCount == 0)
						charSetCFrom = i;
					digitsCount++;
				} else {
					if(digitsCount < 4) {
						charSetCFrom = -1;
						digitsCount = 0;
					} else
						break;
				}
			}
			if(digitsCount < 4)
				return -1;
			else {
				if(digitsCount % 2 != 0)
					charSetCFrom++;
				return charSetCFrom - from;
			}
		}
		static DevExpress.XtraPrinting.BarCode.Code128Charset AutoDetectCharset(char[] text, int from, ref int sequenceLength) {
			int weightA = 1;
			int weightB = 1;
			int weightC = 1;
			int sumA = 0;
			int sumB = 0;
			int sumC = 0;
			int digitsCount = 0;
			int count = text.Length;
			for(int i = from; i < count; i++) {
				if(charSetA.IndexOf(text[i]) < 0)
					weightA = 0;
				else
					sumA += weightA;
				if(charSetB.IndexOf(text[i]) < 0)
					weightB = 0;
				else
					sumB += weightB;
				if(IsDigit(text[i])) {
					sumC += weightC;
					digitsCount += weightC;
				} else if(text[i] == fnc1Char[0] && digitsCount % 2 == 0) {
					sumC += weightC;
				} else
					weightC = 0;
			}
			sumC = sumC - digitsCount % 2;
			if(digitsCount >= 4) {
				sequenceLength = sumC;
				return DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetC;
			}
			DevExpress.XtraPrinting.BarCode.Code128Charset cs;
			if(sumA > sumB) {
				if(sumA > sumC) {
					cs = DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetA;
					int switchToCharSetC = FindSwitchToCharsetC(text, from, sumA);
					sequenceLength = switchToCharSetC < 0 ? sumA : switchToCharSetC;
				} else {
					cs = DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetC;
					sequenceLength = sumC;
				}
			} else {
				if(sumB > sumC) {
					cs = DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetB;
					int switchToCharSetC = FindSwitchToCharsetC(text, from, sumB);
					sequenceLength = switchToCharSetC < 0 ? sumB : switchToCharSetC;
				} else {
					cs = DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetC;
					sequenceLength = sumC;
				}
			}
			return cs;
		}
		#endregion
		#region Fields & Properties
		DevExpress.XtraPrinting.BarCode.Code128Charset startCharSet = DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetA;
		DevExpress.XtraPrinting.BarCode.Code128Charset fStartCharSet = DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetA;
		bool addLeadingZero;
		[
		DefaultValue(true),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool CalcCheckSum {
			get { return true; }
			set { }
		}
		[
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.Code128Generator.AddLeadingZero"),
#if !WINRT && !WP
		TypeConverter(typeof(DevExpress.XtraReports.Design.Code128LeadZeroConverter)),
#endif
		DefaultValue(false),
		XtraSerializableProperty
		]
		public virtual bool AddLeadingZero {
			get { return addLeadingZero; }
			set { addLeadingZero = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("Code128GeneratorCharacterSet"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.Code128Generator.CharacterSet"),
		DefaultValue(DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetA),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public virtual DevExpress.XtraPrinting.BarCode.Code128Charset CharacterSet {
			get { return fStartCharSet; }
			set { fStartCharSet = value; }
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("Code128GeneratorSymbologyCode")]
#endif
		public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.Code128; }
		}
		#endregion
		public Code128Generator() {
		}
		protected Code128Generator(Code128Generator source)
			: base(source) {
			fStartCharSet = source.fStartCharSet;
			addLeadingZero = source.addLeadingZero;
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new Code128Generator(this);
		}
		char GetStartSymbolIndex() {
			return (char)(int)startCharSet;
		}
		char[] Text2IndexesAuto(char[] text) {
			int count = text.Length;
			ArrayList newText = new ArrayList(count);
			int from = 0;
			while(count != 0) {
				DevExpress.XtraPrinting.BarCode.Code128Charset cs = AutoDetectCharset(text, from, ref count);
				if(count == 0)
					break;
				if(from == 0)
					startCharSet = cs;
				else
					newText.Add(GetSwitchCharTo(cs));
				newText.AddRange(Text2Indexes(text, from, count, cs));
				from += count;
			}
			return (char[])newText.ToArray(typeof(char));
		}
		char[] Text2Indexes(char[] text) {
			if(fStartCharSet == DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetAuto)
				return Text2IndexesAuto(text);
			else {
				startCharSet = fStartCharSet;
				return Text2Indexes(text, 0, text.Length, startCharSet);
			}
		}
		protected virtual void InsertControlCharsIndexes(ArrayList text) {
		}
		protected override bool IsValidTextFormat(string text) {
			if(fStartCharSet == DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetC && text.Length % 2 != 0 && !addLeadingZero)
				return false;
			return true;
		}
		protected override string FormatText(string text) {
			if(fStartCharSet == DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetC && text.Length % 2 != 0 && addLeadingZero)
				return base.FormatText(text.Insert(0, "0")); 
			return base.FormatText(text);
		}
		protected override char[] PrepareText(string text) {
			char[] chars = text.ToCharArray();
			chars = Text2Indexes(chars);
			ArrayList newText = new ArrayList(chars.Length + 2);
			newText.Add(GetStartSymbolIndex());
			InsertControlCharsIndexes(newText);
			newText.AddRange(chars);
			newText.Add(CalcCheckDigit(newText));
			newText.Add((char)stopSymbolIndex);
			return (char[])newText.ToArray(typeof(char));
		}
		protected override Hashtable GetPatternTable() {
			return charPattern;
		}
		protected override ArrayList MakeBarCodePattern(string text) {
			char[] indexes = PrepareText(text);
			int indexesCount = indexes.Length;
			Hashtable charPattern = GetPatternTable();
			ArrayList result = new ArrayList();
			for(int i = 0; i < indexesCount; i++) {
				string pattern = charPattern[indexes[i]] as string;
				if(pattern == null)
					continue;
				result.AddRange(GetWidthPattern(pattern));
			}
			return result;
		}
		protected override string GetValidCharSet() {
			switch(CharacterSet) {
				case DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetA:
					return validCharsetA;
				case DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetB:
					return validCharsetB;
				case DevExpress.XtraPrinting.BarCode.Code128Charset.CharsetC:
					return charSetDigits;
				default:
					return charSetAll;
			}
		}
	}
}
