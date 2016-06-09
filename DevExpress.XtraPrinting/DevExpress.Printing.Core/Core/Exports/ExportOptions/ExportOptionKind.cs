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
using System.Text;
using DevExpress.XtraPrinting.Native;
namespace DevExpress.XtraPrinting {
	[Flags]
	public enum ExportOptionKind : long {
		PdfPageRange					 = 1L << 00,
		PdfCompressed					= 1L << 01,
		PdfACompatibility				= 1L << 02,
		PdfShowPrintDialogOnOpen		 = 1L << 03,
		PdfNeverEmbeddedFonts			= 1L << 04,
		PdfPasswordSecurityOptions	   = 1L << 05,
		PdfSignatureOptions			  = 1L << 06,
		PdfConvertImagesToJpeg		   = 1L << 07,
		[OptionKindPropertyType(typeof(PdfJpegImageQuality))]				
		PdfImageQuality				  = 1L << 08,
		PdfDocumentAuthor				= 1L << 09,
		PdfDocumentApplication		   = 1L << 10,
		PdfDocumentTitle				 = 1L << 11,
		PdfDocumentSubject			   = 1L << 12,
		PdfDocumentKeywords			  = 1L << 13,
		[OptionKindPropertyType(typeof(HtmlExportMode))]				
		HtmlExportMode				   = 1L << 14,
		HtmlCharacterSet				 = 1L << 15,
		HtmlTitle						= 1L << 16,
		HtmlRemoveSecondarySymbols	   = 1L << 17,
		HtmlEmbedImagesInHTML			= 1L << 18,
		HtmlPageRange					= 1L << 19,
		HtmlPageBorderWidth			  = 1L << 20,
		HtmlPageBorderColor			  = 1L << 21,
		HtmlTableLayout				  = 1L << 22,
		HtmlExportWatermarks			 = 1L << 23,
		[OptionKindPropertyType(typeof(RtfExportMode))]				
		RtfExportMode					= 1L << 24,
		RtfPageRange					 = 1L << 25,
		RtfExportWatermarks			  = 1L << 26,
		[OptionKindPropertyType(typeof(XlsExportMode))]				
		XlsExportMode					= 1L << 27,
		XlsPageRange					 = 1L << 28,
		[OptionKindPropertyType(typeof(XlsxExportMode))]				
		XlsxExportMode				   = 1L << 29,
		XlsxPageRange					= 1L << 30,
		TextSeparator					= 1L << 31,
		TextEncoding					 = 1L << 32,
		TextQuoteStringsWithSeparators   = 1L << 33,
		[OptionKindPropertyType(typeof(TextExportMode))]				
		TextExportMode				   = 1L << 34,
		XlsShowGridLines				 = 1L << 35,
		XlsUseNativeFormat			   = 1L << 36,
		XlsExportHyperlinks			  = 1L << 37,
		XlsRawDataMode				   = 1L << 38,
		XlsSheetName					 = 1L << 39,
		[OptionKindPropertyType(typeof(ImageExportMode))]				
		ImageExportMode				  = 1L << 40,
		ImagePageRange				   = 1L << 41,
		ImagePageBorderWidth			 = 1L << 42,
		ImagePageBorderColor			 = 1L << 43,
		ImageFormat					  = 1L << 44,
		ImageResolution				  = 1L << 45,
		NativeFormatCompressed		   = 1L << 46,
		XpsPageRange					 = 1L << 47,
		[OptionKindPropertyType(typeof(XpsCompressionOption))]				
		XpsCompression				   = 1L << 48,
		XpsDocumentCreator			   = 1L << 49,
		XpsDocumentCategory			  = 1L << 50,
		XpsDocumentTitle				 = 1L << 51,
		XpsDocumentSubject			   = 1L << 52,
		XpsDocumentKeywords			  = 1L << 53,
		XpsDocumentVersion			   = 1L << 54,
		XpsDocumentDescription		   = 1L << 55,
	}
}
