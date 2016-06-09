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

using DevExpress.Pdf.Native;
namespace DevExpress.Pdf.Drawing {
	public enum EmfCharacterSet {
		[PdfFieldValue(0x00000000)]
		ANSI_CHARSET,
		[PdfFieldValue(0x00000001)]
		DEFAULT_CHARSET,
		[PdfFieldValue(0x00000002)]
		SYMBOL_CHARSET,
		[PdfFieldValue(0x0000004D)]
		MAC_CHARSET,
		[PdfFieldValue(0x00000080)]
		SHIFTJIS_CHARSET,
		[PdfFieldValue(0x00000081)]
		HANGUL_CHARSET,
		[PdfFieldValue(0x00000082)]
		JOHAB_CHARSET,
		[PdfFieldValue(0x00000086)]
		GB2312_CHARSET,
		[PdfFieldValue(0x00000088)]
		CHINESEBIG5_CHARSET,
		[PdfFieldValue(0x000000A1)]
		GREEK_CHARSET,
		[PdfFieldValue(0x000000A2)]
		TURKISH_CHARSET,
		[PdfFieldValue(0x000000A3)]
		VIETNAMESE_CHARSET,
		[PdfFieldValue(0x000000B1)]
		HEBREW_CHARSET,
		[PdfFieldValue(0x000000B2)]
		ARABIC_CHARSET,
		[PdfFieldValue(0x000000BA)]
		BALTIC_CHARSET,
		[PdfFieldValue(0x000000CC)]
		RUSSIAN_CHARSET,
		[PdfFieldValue(0x000000DE)]
		THAI_CHARSET,
		[PdfFieldValue(0x000000EE)]
		EASTEUROPE_CHARSET,
		[PdfFieldValue(0x000000FF)]
		OEM_CHARSET
	}
}
