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
using System.Drawing.Printing;
using DevExpress.Compatibility.System.Drawing;
using DevExpress.Compatibility.System.Drawing.Printing;
namespace DevExpress.Office.Utils {
	#region PaperSizeCalculator
	public static class PaperSizeCalculator {
		readonly static Dictionary<PaperKind, Size> paperSizeTable = CreatePaperSizeTable();
		internal static IDictionary<PaperKind, Size> PaperSizeTable { get { return paperSizeTable; } }
		static Dictionary<PaperKind, Size> CreatePaperSizeTable() {
			Dictionary<PaperKind, Size> result = new Dictionary<PaperKind, Size>();
			result.Add(PaperKind.Letter, new Size(12240, 15840)); 
			result.Add(PaperKind.LetterSmall, new Size(12240, 15840)); 
			result.Add(PaperKind.Tabloid, new Size(15840, 24480)); 
			result.Add(PaperKind.Ledger, new Size(24480, 15840)); 
			result.Add(PaperKind.Legal, new Size(12240, 20160));  
			result.Add(PaperKind.Statement, new Size(7920, 12240)); 
			result.Add(PaperKind.Executive, new Size(10440, 15120)); 
			result.Add(PaperKind.A3, new Size(16839, 23814)); 
			result.Add(PaperKind.A4, new Size(11907, 16839)); 
			result.Add(PaperKind.A4Small, new Size(11907, 16839)); 
			result.Add(PaperKind.A5, new Size(8391, 11907)); 
			result.Add(PaperKind.B4, new Size(14572, 20639)); 
			result.Add(PaperKind.B5, new Size(10319, 14571)); 
			result.Add(PaperKind.Folio, new Size(12240, 18720)); 
			result.Add(PaperKind.Quarto, new Size(12189, 15591)); 
			result.Add(PaperKind.Standard10x14, new Size(14400, 20160)); 
			result.Add(PaperKind.Standard11x17, new Size(15840, 24480)); 
			result.Add(PaperKind.Note, new Size(12240, 15840)); 
			result.Add(PaperKind.Number9Envelope, new Size(5580, 12780)); 
			result.Add(PaperKind.Number10Envelope, new Size(5940, 13680)); 
			result.Add(PaperKind.Number11Envelope, new Size(6480, 14940)); 
			result.Add(PaperKind.Number12Envelope, new Size(6840, 15840)); 
			result.Add(PaperKind.Number14Envelope, new Size(7200, 16560)); 
			result.Add(PaperKind.CSheet, new Size(24480, 31680)); 
			result.Add(PaperKind.DSheet, new Size(31680, 48960)); 
			result.Add(PaperKind.ESheet, new Size(48960, 63360)); 
			result.Add(PaperKind.DLEnvelope, new Size(6237, 12474)); 
			result.Add(PaperKind.C5Envelope, new Size(9185, 12984)); 
			result.Add(PaperKind.C3Envelope, new Size(18369, 25965)); 
			result.Add(PaperKind.C4Envelope, new Size(12983, 18369)); 
			result.Add(PaperKind.C6Envelope, new Size(6463, 9184)); 
			result.Add(PaperKind.C65Envelope, new Size(6463, 12983)); 
			result.Add(PaperKind.B4Envelope, new Size(14173, 20013)); 
			result.Add(PaperKind.B5Envelope, new Size(9978, 14173)); 
			result.Add(PaperKind.B6Envelope, new Size(9978, 7087)); 
			result.Add(PaperKind.ItalyEnvelope, new Size(6236, 13039)); 
			result.Add(PaperKind.MonarchEnvelope, new Size(5580, 10800)); 
			result.Add(PaperKind.PersonalEnvelope, new Size(5220, 9360)); 
			result.Add(PaperKind.USStandardFanfold, new Size(21420, 15840)); 
			result.Add(PaperKind.GermanStandardFanfold, new Size(12240, 17280)); 
			result.Add(PaperKind.GermanLegalFanfold, new Size(12240, 18720)); 
			result.Add(PaperKind.IsoB4, new Size(14173, 20013)); 
			result.Add(PaperKind.JapanesePostcard, new Size(5669, 8391)); 
			result.Add(PaperKind.Standard9x11, new Size(12960, 15840)); 
			result.Add(PaperKind.Standard10x11, new Size(14400, 15840)); 
			result.Add(PaperKind.Standard15x11, new Size(21600, 15840)); 
			result.Add(PaperKind.InviteEnvelope, new Size(12472, 12472)); 
			result.Add(PaperKind.LetterExtra, new Size(13680, 17280)); 
			result.Add(PaperKind.LegalExtra, new Size(13680, 21600)); 
			result.Add(PaperKind.TabloidExtra, new Size(16834, 25920)); 
			result.Add(PaperKind.A4Extra, new Size(13349, 18274)); 
			result.Add(PaperKind.LetterTransverse, new Size(12240, 15840)); 
			result.Add(PaperKind.A4Transverse, new Size(11907, 16839)); 
			result.Add(PaperKind.LetterExtraTransverse, new Size(13680, 17280)); 
			result.Add(PaperKind.APlus, new Size(12869, 20183)); 
			result.Add(PaperKind.BPlus, new Size(17291, 27609)); 
			result.Add(PaperKind.LetterPlus, new Size(12240, 18274)); 
			result.Add(PaperKind.A4Plus, new Size(11907, 18709)); 
			result.Add(PaperKind.A5Transverse, new Size(8391, 11907)); 
			result.Add(PaperKind.B5Transverse, new Size(10319, 14571)); 
			result.Add(PaperKind.A3Extra, new Size(18255, 25228)); 
			result.Add(PaperKind.A5Extra, new Size(9865, 13323)); 
			result.Add(PaperKind.B5Extra, new Size(11395, 15647)); 
			result.Add(PaperKind.A2, new Size(23811, 33676)); 
			result.Add(PaperKind.A3Transverse, new Size(16839, 23814)); 
			result.Add(PaperKind.A3ExtraTransverse, new Size(18255, 25228)); 
			result.Add(PaperKind.JapaneseDoublePostcard, new Size(11339, 8391)); 
			result.Add(PaperKind.A6, new Size(5953, 8391)); 
			result.Add(PaperKind.JapaneseEnvelopeKakuNumber2, new Size(12240, 15840)); 
			result.Add(PaperKind.JapaneseEnvelopeKakuNumber3, new Size(12240, 15840)); 
			result.Add(PaperKind.JapaneseEnvelopeChouNumber3, new Size(12240, 15840)); 
			result.Add(PaperKind.JapaneseEnvelopeChouNumber4, new Size(12240, 15840)); 
			result.Add(PaperKind.LetterRotated, new Size(15840, 12240)); 
			result.Add(PaperKind.A3Rotated, new Size(23814, 16839)); 
			result.Add(PaperKind.A4Rotated, new Size(16839, 11907)); 
			result.Add(PaperKind.A5Rotated, new Size(11907, 8391)); 
			result.Add(PaperKind.B4JisRotated, new Size(20636, 14570)); 
			result.Add(PaperKind.B5JisRotated, new Size(14570, 10318)); 
			result.Add(PaperKind.JapanesePostcardRotated, new Size(8391, 5669)); 
			result.Add(PaperKind.JapaneseDoublePostcardRotated, new Size(8391, 11339)); 
			result.Add(PaperKind.A6Rotated, new Size(8391, 5953)); 
			result.Add(PaperKind.JapaneseEnvelopeKakuNumber2Rotated, new Size(12240, 15840)); 
			result.Add(PaperKind.JapaneseEnvelopeKakuNumber3Rotated, new Size(12240, 15840)); 
			result.Add(PaperKind.JapaneseEnvelopeChouNumber3Rotated, new Size(12240, 15840)); 
			result.Add(PaperKind.JapaneseEnvelopeChouNumber4Rotated, new Size(12240, 15840)); 
			result.Add(PaperKind.B6Jis, new Size(7257, 10318)); 
			result.Add(PaperKind.B6JisRotated, new Size(10318, 7257)); 
			result.Add(PaperKind.Standard12x11, new Size(17280, 15840)); 
			result.Add(PaperKind.JapaneseEnvelopeYouNumber4, new Size(12240, 15840)); 
			result.Add(PaperKind.JapaneseEnvelopeYouNumber4Rotated, new Size(15840, 12240)); 
			result.Add(PaperKind.Prc16K, new Size(8277, 12189)); 
			result.Add(PaperKind.Prc32K, new Size(5499, 8561)); 
			result.Add(PaperKind.Prc32KBig, new Size(5499, 8561)); 
			result.Add(PaperKind.PrcEnvelopeNumber1, new Size(5783, 9354)); 
			result.Add(PaperKind.PrcEnvelopeNumber2, new Size(5783, 9978)); 
			result.Add(PaperKind.PrcEnvelopeNumber3, new Size(7087, 9978)); 
			result.Add(PaperKind.PrcEnvelopeNumber4, new Size(6236, 11792)); 
			result.Add(PaperKind.PrcEnvelopeNumber5, new Size(6236, 12472)); 
			result.Add(PaperKind.PrcEnvelopeNumber6, new Size(6803, 13039)); 
			result.Add(PaperKind.PrcEnvelopeNumber7, new Size(9071, 13039)); 
			result.Add(PaperKind.PrcEnvelopeNumber8, new Size(6803, 17518)); 
			result.Add(PaperKind.PrcEnvelopeNumber9, new Size(12983, 18369)); 
			result.Add(PaperKind.PrcEnvelopeNumber10, new Size(18369, 25965)); 
			result.Add(PaperKind.Prc16KRotated, new Size(12189, 8277)); 
			result.Add(PaperKind.Prc32KRotated, new Size(8561, 5499)); 
			result.Add(PaperKind.Prc32KBigRotated, new Size(8561, 5499)); 
			result.Add(PaperKind.PrcEnvelopeNumber1Rotated, new Size(9354, 5783)); 
			result.Add(PaperKind.PrcEnvelopeNumber2Rotated, new Size(9978, 5783)); 
			result.Add(PaperKind.PrcEnvelopeNumber3Rotated, new Size(9978, 7087)); 
			result.Add(PaperKind.PrcEnvelopeNumber4Rotated, new Size(11792, 6236)); 
			result.Add(PaperKind.PrcEnvelopeNumber5Rotated, new Size(12472, 6236)); 
			result.Add(PaperKind.PrcEnvelopeNumber6Rotated, new Size(13039, 6803)); 
			result.Add(PaperKind.PrcEnvelopeNumber7Rotated, new Size(13039, 9071)); 
			result.Add(PaperKind.PrcEnvelopeNumber8Rotated, new Size(17518, 6803)); 
			result.Add(PaperKind.PrcEnvelopeNumber9Rotated, new Size(18369, 12983)); 
			result.Add(PaperKind.PrcEnvelopeNumber10Rotated, new Size(25965, 18369)); 
			return result;
		}
		public static Size CalculatePaperSize(PaperKind paperKind) {
			Size result;
			if (!paperSizeTable.TryGetValue(paperKind, out result))
				result = new Size(12240, 15840); 
			return result;
		}
		public static PaperKind CalculatePaperKind(Size size, PaperKind defaultValue) {
			return CalculatePaperKind(size, defaultValue, 0, defaultValue);
		}
		public static PaperKind CalculatePaperKind(Size size, PaperKind defaultValue, int tolerance, PaperKind badSizeDefaultValue) {
			if (size.Width == 0 || size.Height == 0)
				return badSizeDefaultValue;
			foreach (KeyValuePair<PaperKind, Size> entry in paperSizeTable) {
				Size entrySize = entry.Value;
				if (Math.Abs(size.Width - entrySize.Width) <= tolerance && Math.Abs(size.Height - entrySize.Height) <= tolerance)
					return entry.Key;
			}
			return defaultValue;
		}
	}
	#endregion
}
