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
	public enum EmfPlusPixelFormat {
		[PdfFieldValue(0x00000000)]
		PixelFormatUndefined,
		[PdfFieldValue(0x00030101)]
		PixelFormat1bppIndexed,
		[PdfFieldValue(0x00030402)]
		PixelFormat4bppIndexed,
		[PdfFieldValue(0x00030803)]
		PixelFormat8bppIndexed,
		[PdfFieldValue(0x00101004)]
		PixelFormat16bppGrayScale,
		[PdfFieldValue(0x00021005)]
		PixelFormat16bppRGB555,
		[PdfFieldValue(0x00021006)]
		PixelFormat16bppRGB565,
		[PdfFieldValue(0x00061007)]
		PixelFormat16bppARGB1555,
		[PdfFieldValue(0x00021808)]
		PixelFormat24bppRGB,
		[PdfFieldValue(0x00022009)]
		PixelFormat32bppRGB,
		[PdfFieldValue(0x0026200A)]
		PixelFormat32bppARGB,
		[PdfFieldValue(0x000E200B)]
		PixelFormat32bppPARGB,
		[PdfFieldValue(0x0010300C)]
		PixelFormat48bppRGB,
		[PdfFieldValue(0x0034400D)]
		PixelFormat64bppARGB,
		[PdfFieldValue(0x001A400E)]
		PixelFormat64bppPARGB
	}
}
