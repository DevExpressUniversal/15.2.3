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
using System.Text;
using DevExpress.Utils.Serializing;
using DevExpress.XtraPrinting.BarCode.Native;
namespace DevExpress.XtraPrinting.BarCode {
	public class Interleaved2of5Generator : BarCodeGeneratorBase {
		protected const int defaultWideNarrowRatio = 3;
		static string validCharSet = charSetDigits;
		static Hashtable charPattern = new Hashtable();
		#region Fields & Properties
		float wideNarrowRatio = defaultWideNarrowRatio;
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("Interleaved2of5GeneratorWideNarrowRatio"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.Interleaved2of5Generator.WideNarrowRatio"),
		DefaultValue(defaultWideNarrowRatio),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public virtual float WideNarrowRatio {
			get { return wideNarrowRatio; }
			set {
				if(value < 2.0f)
					value = 2.0f;
				if(value > 3.0f)
					value = 3.0f;
				wideNarrowRatio = value;
			}
		}
		#endregion
		#region static XRInterleaved2of5Generator()
		static Interleaved2of5Generator() {
			charPattern['0'] = "nnwwn";
			charPattern['1'] = "wnnnw";
			charPattern['2'] = "nwnnw";
			charPattern['3'] = "wwnnn";
			charPattern['4'] = "nnwnw";
			charPattern['5'] = "wnwnn";
			charPattern['6'] = "nwwnn";
			charPattern['7'] = "nnnww";
			charPattern['8'] = "wnnwn";
			charPattern['9'] = "nwnwn";
			charPattern['B'] = "nnnn";
			charPattern['E'] = "wnn";
		}
		#endregion
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("Interleaved2of5GeneratorSymbologyCode")]
#endif
public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.Interleaved2of5; }
		}
		public Interleaved2of5Generator() {
		}
		protected Interleaved2of5Generator(Interleaved2of5Generator source)
			: base(source) {
			wideNarrowRatio = source.wideNarrowRatio;
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new Interleaved2of5Generator(this);
		}
		protected override Hashtable GetPatternTable() {
			return charPattern;
		}
		protected override float GetPatternWidth(char pattern) {
			return pattern == 'n' ? 1 : wideNarrowRatio;
		}
		static string MergePatterns(char[] first, char[] second) {
			System.Diagnostics.Debug.Assert(first.Length == second.Length);
			int count = first.Length;
			StringBuilder sb = new StringBuilder(2 * count);
			for(int i = 0; i < count; i++) {
				sb.Append(first[i]);
				sb.Append(second[i]);
			}
			return sb.ToString();
		}
		protected override ArrayList MakeBarCodePattern(string text) {
			char[] chars = PrepareText(text);
			System.Diagnostics.Debug.Assert(chars.Length % 2 == 0);
			int charsCount = chars.Length;
			Hashtable charPattern = GetPatternTable();
			ArrayList result = new ArrayList();
			result.AddRange(GetWidthPattern((string)charPattern['B']));
			for(int i = 0; i < charsCount; i += 2) {
				string pattern1 = charPattern[chars[i]] as string;
				if(pattern1 == null)
					continue;
				string pattern2 = charPattern[chars[i + 1]] as string;
				if(pattern2 == null)
					continue;
				result.AddRange(GetWidthPattern(MergePatterns(pattern1.ToCharArray(), pattern2.ToCharArray())));
			}
			result.AddRange(GetWidthPattern((string)charPattern['E']));
			return result;
		}
		protected override string FormatText(string text) {
			if(CalcCheckSum)
				text += Industrial2of5Generator.CalcCheckDigit(text);
			if(text.Length % 2 != 0)
				text = '0' + text;
			return text;
		}
		protected override char[] PrepareText(string text) {
			return text.ToCharArray();
		}
		protected override string GetValidCharSet() { return validCharSet; }
	}
}
