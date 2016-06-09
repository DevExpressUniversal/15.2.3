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
namespace DevExpress.XtraPrinting.BarCode {
	public class CodabarGenerator : BarCodeGeneratorBase {
		static Hashtable charPattern = new Hashtable();
		static char[][] startStopPairs = { null, "AA".ToCharArray(), "BB".ToCharArray(), "CC".ToCharArray(), "DD".ToCharArray() };
		static string validCharSet = charSetDigits + "-$:/.+";
		protected const int defaultWideNarrowRatio = 2;
		#region Fields & Properties
		DevExpress.XtraPrinting.BarCode.CodabarStartStopPair startStopSymbols = DevExpress.XtraPrinting.BarCode.CodabarStartStopPair.AT;
		float wideNarrowRatio = defaultWideNarrowRatio;
		[
		DefaultValue(false),
		Browsable(false),
		DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
		EditorBrowsable(EditorBrowsableState.Never),
		]
		public override bool CalcCheckSum {
			get { return false; }
			set { }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("CodabarGeneratorStartStopPair"),
#endif
	   DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.CodabarGenerator.StartStopPair"),
		DefaultValue(DevExpress.XtraPrinting.BarCode.CodabarStartStopPair.AT),
		NotifyParentProperty(true),
		XtraSerializableProperty,
		]
		public DevExpress.XtraPrinting.BarCode.CodabarStartStopPair StartStopPair {
			get { return startStopSymbols; }
			set { startStopSymbols = value; }
		}
		[
#if !SL
	DevExpressPrintingCoreLocalizedDescription("CodabarGeneratorWideNarrowRatio"),
#endif
		DXDisplayNameAttribute(typeof(DevExpress.Data.ResFinder), "DevExpress.XtraPrinting.BarCode.CodabarGenerator.WideNarrowRatio"),
		DefaultValue(defaultWideNarrowRatio),
		XtraSerializableProperty,
		]
		public float WideNarrowRatio {
			get { return wideNarrowRatio; }
			set {
				if (value < 2.0f)
					value = 2.0f;
				if (value > 3.0f)
					value = 3.0f;
				wideNarrowRatio = value;
			}
		}
#if !SL
	[DevExpressPrintingCoreLocalizedDescription("CodabarGeneratorSymbologyCode")]
#endif
public override BarCodeSymbology SymbologyCode {
			get { return BarCodeSymbology.Codabar; }
		}
		#endregion
		#region static XRCodabarGenerator()
		static CodabarGenerator() {
			charPattern['0'] = "nnnnnwwn";
			charPattern['1'] = "nnnnwwnn";
			charPattern['2'] = "nnnwnnwn";
			charPattern['3'] = "wwnnnnnn";
			charPattern['4'] = "nnwnnwnn";
			charPattern['5'] = "wnnnnwnn";
			charPattern['6'] = "nwnnnnwn";
			charPattern['7'] = "nwnnwnnn";
			charPattern['8'] = "nwwnnnnn";
			charPattern['9'] = "wnnwnnnn";
			charPattern['-'] = "nnnwwnnn";
			charPattern['$'] = "nnwwnnnn";
			charPattern[':'] = "wnnnwnwn";
			charPattern['/'] = "wnwnnnwn";
			charPattern['.'] = "wnwnwnnn";
			charPattern['+'] = "nnwnwnwn";
			charPattern['A'] = "nnwwnwnn";
			charPattern['B'] = "nwnwnnwn";
			charPattern['C'] = "nnnwnwwn";
			charPattern['D'] = "nnnwwwnn";
		}
		#endregion
		public CodabarGenerator() {
		}
		CodabarGenerator(CodabarGenerator source)
			: base(source) {
			startStopSymbols = source.startStopSymbols;
			wideNarrowRatio = source.wideNarrowRatio;
		}
		protected override BarCodeGeneratorBase CloneGenerator() {
			return new CodabarGenerator(this);
		}
		string AppendStartStopSymbols(string text) {
			char[] startStopChars = startStopPairs[(int)startStopSymbols];
			if(startStopChars == null)
				return text;
			System.Diagnostics.Debug.Assert(startStopChars.Length == 2);
			return startStopChars[0] + text + startStopChars[1];
		}
		protected override char[] PrepareText(string text) {
			return AppendStartStopSymbols(text).ToCharArray();
		}
		protected override Hashtable GetPatternTable() {
			return charPattern;
		}
		protected override float GetPatternWidth(char pattern) {
			return pattern == 'n' ? 1 : wideNarrowRatio;
		}
		protected override string GetValidCharSet() { return validCharSet; }
	}
}
