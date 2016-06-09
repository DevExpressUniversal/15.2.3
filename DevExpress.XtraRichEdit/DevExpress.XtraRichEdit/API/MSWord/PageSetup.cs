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
using System.CodeDom.Compiler;
namespace DevExpress.XtraRichEdit.API.Word {
	#region WdOrientation
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdOrientation {
		wdOrientPortrait,
		wdOrientLandscape
	}
	#endregion
	#region WdPaperTray
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdPaperTray {
		wdPrinterAutomaticSheetFeed = 7,
		wdPrinterDefaultBin = 0,
		wdPrinterEnvelopeFeed = 5,
		wdPrinterFormSource = 15,
		wdPrinterLargeCapacityBin = 11,
		wdPrinterLargeFormatBin = 10,
		wdPrinterLowerBin = 2,
		wdPrinterManualEnvelopeFeed = 6,
		wdPrinterManualFeed = 4,
		wdPrinterMiddleBin = 3,
		wdPrinterOnlyBin = 1,
		wdPrinterPaperCassette = 14,
		wdPrinterSmallFormatBin = 9,
		wdPrinterTractorFeed = 8,
		wdPrinterUpperBin = 1
	}
	#endregion
	#region WdVerticalAlignment
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdVerticalAlignment {
		wdAlignVerticalTop,
		wdAlignVerticalCenter,
		wdAlignVerticalJustify,
		wdAlignVerticalBottom
	}
	#endregion
	#region WdSectionStart
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdSectionStart {
		wdSectionContinuous,
		wdSectionNewColumn,
		wdSectionNewPage,
		wdSectionEvenPage,
		wdSectionOddPage
	}
	#endregion
	#region WdPaperSize
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdPaperSize {
		wdPaper10x14,
		wdPaper11x17,
		wdPaperLetter,
		wdPaperLetterSmall,
		wdPaperLegal,
		wdPaperExecutive,
		wdPaperA3,
		wdPaperA4,
		wdPaperA4Small,
		wdPaperA5,
		wdPaperB4,
		wdPaperB5,
		wdPaperCSheet,
		wdPaperDSheet,
		wdPaperESheet,
		wdPaperFanfoldLegalGerman,
		wdPaperFanfoldStdGerman,
		wdPaperFanfoldUS,
		wdPaperFolio,
		wdPaperLedger,
		wdPaperNote,
		wdPaperQuarto,
		wdPaperStatement,
		wdPaperTabloid,
		wdPaperEnvelope9,
		wdPaperEnvelope10,
		wdPaperEnvelope11,
		wdPaperEnvelope12,
		wdPaperEnvelope14,
		wdPaperEnvelopeB4,
		wdPaperEnvelopeB5,
		wdPaperEnvelopeB6,
		wdPaperEnvelopeC3,
		wdPaperEnvelopeC4,
		wdPaperEnvelopeC5,
		wdPaperEnvelopeC6,
		wdPaperEnvelopeC65,
		wdPaperEnvelopeDL,
		wdPaperEnvelopeItaly,
		wdPaperEnvelopeMonarch,
		wdPaperEnvelopePersonal,
		wdPaperCustom
	}
	#endregion
	#region WdGutterStyleOld
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdGutterStyleOld {
		wdGutterStyleBidi = 2,
		wdGutterStyleLatin = -10
	}
	#endregion
	#region WdSectionDirection
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdSectionDirection {
		wdSectionDirectionRtl,
		wdSectionDirectionLtr
	}
	#endregion
	#region WdLayoutMode
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdLayoutMode {
		wdLayoutModeDefault,
		wdLayoutModeGrid,
		wdLayoutModeLineGrid,
		wdLayoutModeGenko
	}
	#endregion
	#region WdGutterStyle
	[GeneratedCode("Suppress FxCop check", "")]
	public enum WdGutterStyle {
		wdGutterPosLeft,
		wdGutterPosTop,
		wdGutterPosRight
	}
	#endregion
	#region PageSetup
	[GeneratedCode("Suppress FxCop check", "")]
	public interface PageSetup : IWordObject {
		float TopMargin { get; set; }
		float BottomMargin { get; set; }
		float LeftMargin { get; set; }
		float RightMargin { get; set; }
		float Gutter { get; set; }
		float PageWidth { get; set; }
		float PageHeight { get; set; }
		WdOrientation Orientation { get; set; }
		WdPaperTray FirstPageTray { get; set; }
		WdPaperTray OtherPagesTray { get; set; }
		WdVerticalAlignment VerticalAlignment { get; set; }
		int MirrorMargins { get; set; }
		float HeaderDistance { get; set; }
		float FooterDistance { get; set; }
		WdSectionStart SectionStart { get; set; }
		int OddAndEvenPagesHeaderFooter { get; set; }
		int DifferentFirstPageHeaderFooter { get; set; }
		int SuppressEndnotes { get; set; }
		LineNumbering LineNumbering { get; set; }
		TextColumns TextColumns { get; set; }
		WdPaperSize PaperSize { get; set; }
		bool TwoPagesOnOne { get; set; }
		bool GutterOnTop { get; set; }
		float CharsLine { get; set; }
		float LinesPage { get; set; }
		bool ShowGrid { get; set; }
		void TogglePortrait();
		void SetAsTemplateDefault();
		WdGutterStyleOld GutterStyle { get; set; }
		WdSectionDirection SectionDirection { get; set; }
		WdLayoutMode LayoutMode { get; set; }
		WdGutterStyle GutterPos { get; set; }
		bool BookFoldPrinting { get; set; }
		bool BookFoldRevPrinting { get; set; }
		int BookFoldPrintingSheets { get; set; }
	}
	#endregion
}
