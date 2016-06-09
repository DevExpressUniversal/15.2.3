﻿#region Copyright (c) 2000-2015 Developer Express Inc.
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
namespace DevExpress.Xpf.Core.Native {
	#region PaperKind
	public enum PaperKind {
		A2 = 0x42,
		A3 = 8,
		A3Extra = 0x3f,
		A3ExtraTransverse = 0x44,
		A3Rotated = 0x4c,
		A3Transverse = 0x43,
		A4 = 9,
		A4Extra = 0x35,
		A4Plus = 60,
		A4Rotated = 0x4d,
		A4Small = 10,
		A4Transverse = 0x37,
		A5 = 11,
		A5Extra = 0x40,
		A5Rotated = 0x4e,
		A5Transverse = 0x3d,
		A6 = 70,
		A6Rotated = 0x53,
		APlus = 0x39,
		B4 = 12,
		B4Envelope = 0x21,
		B4JisRotated = 0x4f,
		B5 = 13,
		B5Envelope = 0x22,
		B5Extra = 0x41,
		B5JisRotated = 80,
		B5Transverse = 0x3e,
		B6Envelope = 0x23,
		B6Jis = 0x58,
		B6JisRotated = 0x59,
		BPlus = 0x3a,
		C3Envelope = 0x1d,
		C4Envelope = 30,
		C5Envelope = 0x1c,
		C65Envelope = 0x20,
		C6Envelope = 0x1f,
		CSheet = 0x18,
		Custom = 0,
		DLEnvelope = 0x1b,
		DSheet = 0x19,
		ESheet = 0x1a,
		Executive = 7,
		Folio = 14,
		GermanLegalFanfold = 0x29,
		GermanStandardFanfold = 40,
		InviteEnvelope = 0x2f,
		IsoB4 = 0x2a,
		ItalyEnvelope = 0x24,
		JapaneseDoublePostcard = 0x45,
		JapaneseDoublePostcardRotated = 0x52,
		JapaneseEnvelopeChouNumber3 = 0x49,
		JapaneseEnvelopeChouNumber3Rotated = 0x56,
		JapaneseEnvelopeChouNumber4 = 0x4a,
		JapaneseEnvelopeChouNumber4Rotated = 0x57,
		JapaneseEnvelopeKakuNumber2 = 0x47,
		JapaneseEnvelopeKakuNumber2Rotated = 0x54,
		JapaneseEnvelopeKakuNumber3 = 0x48,
		JapaneseEnvelopeKakuNumber3Rotated = 0x55,
		JapaneseEnvelopeYouNumber4 = 0x5b,
		JapaneseEnvelopeYouNumber4Rotated = 0x5c,
		JapanesePostcard = 0x2b,
		JapanesePostcardRotated = 0x51,
		Ledger = 4,
		Legal = 5,
		LegalExtra = 0x33,
		Letter = 1,
		LetterExtra = 50,
		LetterExtraTransverse = 0x38,
		LetterPlus = 0x3b,
		LetterRotated = 0x4b,
		LetterSmall = 2,
		LetterTransverse = 0x36,
		MonarchEnvelope = 0x25,
		Note = 0x12,
		Number10Envelope = 20,
		Number11Envelope = 0x15,
		Number12Envelope = 0x16,
		Number14Envelope = 0x17,
		Number9Envelope = 0x13,
		PersonalEnvelope = 0x26,
		Prc16K = 0x5d,
		Prc16KRotated = 0x6a,
		Prc32K = 0x5e,
		Prc32KBig = 0x5f,
		Prc32KBigRotated = 0x6c,
		Prc32KRotated = 0x6b,
		PrcEnvelopeNumber1 = 0x60,
		PrcEnvelopeNumber10 = 0x69,
		PrcEnvelopeNumber10Rotated = 0x76,
		PrcEnvelopeNumber1Rotated = 0x6d,
		PrcEnvelopeNumber2 = 0x61,
		PrcEnvelopeNumber2Rotated = 110,
		PrcEnvelopeNumber3 = 0x62,
		PrcEnvelopeNumber3Rotated = 0x6f,
		PrcEnvelopeNumber4 = 0x63,
		PrcEnvelopeNumber4Rotated = 0x70,
		PrcEnvelopeNumber5 = 100,
		PrcEnvelopeNumber5Rotated = 0x71,
		PrcEnvelopeNumber6 = 0x65,
		PrcEnvelopeNumber6Rotated = 0x72,
		PrcEnvelopeNumber7 = 0x66,
		PrcEnvelopeNumber7Rotated = 0x73,
		PrcEnvelopeNumber8 = 0x67,
		PrcEnvelopeNumber8Rotated = 0x74,
		PrcEnvelopeNumber9 = 0x68,
		PrcEnvelopeNumber9Rotated = 0x75,
		Quarto = 15,
		Standard10x11 = 0x2d,
		Standard10x14 = 0x10,
		Standard11x17 = 0x11,
		Standard12x11 = 90,
		Standard15x11 = 0x2e,
		Standard9x11 = 0x2c,
		Statement = 6,
		Tabloid = 3,
		TabloidExtra = 0x34,
		USStandardFanfold = 0x27
	}
	#endregion
}
