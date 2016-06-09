#region Copyright (c) 2000-2015 Developer Express Inc.
/*
{*******************************************************************}
{                                                                   }
{       Developer Express .NET Component Library                    }
{       UWP Controls                                                }
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

namespace DevExpress.Pdf {
	public static class PdfPaperSize {
		public static PdfRectangle A2 = new PdfRectangle(0, 0, 1191, 1684);
		public static PdfRectangle A3 = new PdfRectangle(0, 0, 842, 1191);
		public static PdfRectangle A3Extra = new PdfRectangle(0, 0, 913, 1261);
		public static PdfRectangle A3ExtraTransverse = new PdfRectangle(0, 0, 913, 1261);
		public static PdfRectangle A3Rotated = new PdfRectangle(0, 0, 1191, 842);
		public static PdfRectangle A3Transverse = new PdfRectangle(0, 0, 842, 1191);
		public static PdfRectangle A4 = new PdfRectangle(0, 0, 595, 842);
		public static PdfRectangle A4Extra = new PdfRectangle(0, 0, 669, 913);
		public static PdfRectangle A4Plus = new PdfRectangle(0, 0, 595, 935);
		public static PdfRectangle A4Rotated = new PdfRectangle(0, 0, 842, 595);
		public static PdfRectangle A4Small = new PdfRectangle(0, 0, 595, 842);
		public static PdfRectangle A4Transverse = new PdfRectangle(0, 0, 595, 842);
		public static PdfRectangle A5 = new PdfRectangle(0, 0, 420, 595);
		public static PdfRectangle A5Extra = new PdfRectangle(0, 0, 493, 666);
		public static PdfRectangle A5Rotated = new PdfRectangle(0, 0, 595, 420);
		public static PdfRectangle A5Transverse = new PdfRectangle(0, 0, 420, 595);
		public static PdfRectangle A6 = new PdfRectangle(0, 0, 298, 420);
		public static PdfRectangle A6Rotated = new PdfRectangle(0, 0, 420, 298);
		public static PdfRectangle APlus = new PdfRectangle(0, 0, 643, 1009);
		public static PdfRectangle B4 = new PdfRectangle(0, 0, 709, 1001);
		public static PdfRectangle B4Envelope = new PdfRectangle(0, 0, 709, 1001);
		public static PdfRectangle B4JisRotated = new PdfRectangle(0, 0, 1032, 729);
		public static PdfRectangle B5 = new PdfRectangle(0, 0, 499, 709);
		public static PdfRectangle B5Envelope = new PdfRectangle(0, 0, 499, 709);
		public static PdfRectangle B5Extra = new PdfRectangle(0, 0, 570, 782);
		public static PdfRectangle B5JisRotated = new PdfRectangle(0, 0, 729, 516);
		public static PdfRectangle B5Transverse = new PdfRectangle(0, 0, 516, 729);
		public static PdfRectangle B6Envelope = new PdfRectangle(0, 0, 499, 354);
		public static PdfRectangle B6Jis = new PdfRectangle(0, 0, 363, 516);
		public static PdfRectangle B6JisRotated = new PdfRectangle(0, 0, 516, 363);
		public static PdfRectangle BPlus = new PdfRectangle(0, 0, 865, 1380);
		public static PdfRectangle C3Envelope = new PdfRectangle(0, 0, 918, 1298);
		public static PdfRectangle C4Envelope = new PdfRectangle(0, 0, 649, 918);
		public static PdfRectangle C5Envelope = new PdfRectangle(0, 0, 459, 649);
		public static PdfRectangle C65Envelope = new PdfRectangle(0, 0, 323, 649);
		public static PdfRectangle C6Envelope = new PdfRectangle(0, 0, 323, 459);
		public static PdfRectangle CSheet = new PdfRectangle(0, 0, 1224, 1584);
		public static PdfRectangle DLEnvelope = new PdfRectangle(0, 0, 312, 624);
		public static PdfRectangle DSheet = new PdfRectangle(0, 0, 1584, 2448);
		public static PdfRectangle ESheet = new PdfRectangle(0, 0, 2448, 3168);
		public static PdfRectangle Executive = new PdfRectangle(0, 0, 522, 756);
		public static PdfRectangle Folio = new PdfRectangle(0, 0, 612, 936);
		public static PdfRectangle GermanLegalFanfold = new PdfRectangle(0, 0, 612, 936);
		public static PdfRectangle GermanStandardFanfold = new PdfRectangle(0, 0, 612, 864);
		public static PdfRectangle InviteEnvelope = new PdfRectangle(0, 0, 624, 624);
		public static PdfRectangle IsoB4 = new PdfRectangle(0, 0, 709, 1001);
		public static PdfRectangle ItalyEnvelope = new PdfRectangle(0, 0, 312, 652);
		public static PdfRectangle JapaneseDoublePostcard = new PdfRectangle(0, 0, 567, 420);
		public static PdfRectangle JapaneseDoublePostcardRotated = new PdfRectangle(0, 0, 420, 567);
		public static PdfRectangle JapanesePostcard = new PdfRectangle(0, 0, 283, 420);
		public static PdfRectangle JapanesePostcardRotated = new PdfRectangle(0, 0, 420, 283);
		public static PdfRectangle Ledger = new PdfRectangle(0, 0, 1224, 792);
		public static PdfRectangle Legal = new PdfRectangle(0, 0, 612, 1008);
		public static PdfRectangle LegalExtra = new PdfRectangle(0, 0, 668, 1080);
		public static PdfRectangle Letter = new PdfRectangle(0, 0, 612, 792);
		public static PdfRectangle LetterExtra = new PdfRectangle(0, 0, 668, 864);
		public static PdfRectangle LetterExtraTransverse = new PdfRectangle(0, 0, 668, 864);
		public static PdfRectangle LetterPlus = new PdfRectangle(0, 0, 612, 914);
		public static PdfRectangle LetterRotated = new PdfRectangle(0, 0, 792, 612);
		public static PdfRectangle LetterSmall = new PdfRectangle(0, 0, 612, 792);
		public static PdfRectangle LetterTransverse = new PdfRectangle(0, 0, 596, 792);
		public static PdfRectangle MonarchEnvelope = new PdfRectangle(0, 0, 279, 540);
		public static PdfRectangle Note = new PdfRectangle(0, 0, 612, 792);
		public static PdfRectangle Number10Envelope = new PdfRectangle(0, 0, 297, 684);
		public static PdfRectangle Number11Envelope = new PdfRectangle(0, 0, 324, 747);
		public static PdfRectangle Number12Envelope = new PdfRectangle(0, 0, 342, 792);
		public static PdfRectangle Number9Envelope = new PdfRectangle(0, 0, 279, 639);
		public static PdfRectangle PersonalEnvelope = new PdfRectangle(0, 0, 261, 468);
		public static PdfRectangle Prc16K = new PdfRectangle(0, 0, 414, 609);
		public static PdfRectangle Prc16KRotated = new PdfRectangle(0, 0, 414, 609);
		public static PdfRectangle Prc32K = new PdfRectangle(0, 0, 275, 428);
		public static PdfRectangle Prc32KBig = new PdfRectangle(0, 0, 275, 428);
		public static PdfRectangle Prc32KBigRotated = new PdfRectangle(0, 0, 275, 428);
		public static PdfRectangle Prc32KRotated = new PdfRectangle(0, 0, 275, 428);
		public static PdfRectangle PrcEnvelopeNumber1 = new PdfRectangle(0, 0, 289, 468);
		public static PdfRectangle PrcEnvelopeNumber10 = new PdfRectangle(0, 0, 918, 1298);
		public static PdfRectangle PrcEnvelopeNumber10Rotated = new PdfRectangle(0, 0, 1298, 918);
		public static PdfRectangle PrcEnvelopeNumber1Rotated = new PdfRectangle(0, 0, 468, 289);
		public static PdfRectangle PrcEnvelopeNumber2 = new PdfRectangle(0, 0, 289, 499);
		public static PdfRectangle PrcEnvelopeNumber2Rotated = new PdfRectangle(0, 0, 499, 289);
		public static PdfRectangle PrcEnvelopeNumber3 = new PdfRectangle(0, 0, 354, 499);
		public static PdfRectangle PrcEnvelopeNumber3Rotated = new PdfRectangle(0, 0, 499, 354);
		public static PdfRectangle PrcEnvelopeNumber4 = new PdfRectangle(0, 0, 312, 590);
		public static PdfRectangle PrcEnvelopeNumber4Rotated = new PdfRectangle(0, 0, 590, 312);
		public static PdfRectangle PrcEnvelopeNumber5 = new PdfRectangle(0, 0, 312, 624);
		public static PdfRectangle PrcEnvelopeNumber5Rotated = new PdfRectangle(0, 0, 624, 312);
		public static PdfRectangle PrcEnvelopeNumber6 = new PdfRectangle(0, 0, 340, 652);
		public static PdfRectangle PrcEnvelopeNumber6Rotated = new PdfRectangle(0, 0, 652, 340);
		public static PdfRectangle PrcEnvelopeNumber7 = new PdfRectangle(0, 0, 454, 652);
		public static PdfRectangle PrcEnvelopeNumber7Rotated = new PdfRectangle(0, 0, 652, 454);
		public static PdfRectangle PrcEnvelopeNumber8 = new PdfRectangle(0, 0, 340, 876);
		public static PdfRectangle PrcEnvelopeNumber8Rotated = new PdfRectangle(0, 0, 876, 340);
		public static PdfRectangle PrcEnvelopeNumber9 = new PdfRectangle(0, 0, 649, 918);
		public static PdfRectangle PrcEnvelopeNumber9Rotated = new PdfRectangle(0, 0, 918, 649);
		public static PdfRectangle Quarto = new PdfRectangle(0, 0, 609, 780);
		public static PdfRectangle Standard10x11 = new PdfRectangle(0, 0, 720, 792);
		public static PdfRectangle Standard10x14 = new PdfRectangle(0, 0, 720, 1008);
		public static PdfRectangle Standard11x17 = new PdfRectangle(0, 0, 792, 1224);
		public static PdfRectangle Standard12x11 = new PdfRectangle(0, 0, 864, 792);
		public static PdfRectangle Standard15x11 = new PdfRectangle(0, 0, 1080, 792);
		public static PdfRectangle Statement = new PdfRectangle(0, 0, 396, 612);
		public static PdfRectangle Tabloid = new PdfRectangle(0, 0, 792, 1224);
		public static PdfRectangle TabloidExtra = new PdfRectangle(0, 0, 842, 1296);
		public static PdfRectangle USStandardFanfold = new PdfRectangle(0, 0, 1071, 792);
	}
}
