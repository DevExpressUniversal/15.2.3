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

using System.ComponentModel;
using System.Globalization;
using DevExpress.Utils.Localization;
using DevExpress.Utils.Localization.Internal;
namespace DevExpress.Pdf.Localization {
	public enum PdfCoreStringId {
		DefaultDocumentName,
		MsgIncorrectPdfData,
		MsgIncorrectFormDataFile,
		MsgIncorrectPdfPassword,
		MsgIncorrectRectangleWidth,
		MsgIncorrectRectangleHeight,
		MsgIncorrectPageRotate,
		MsgIncorrectPageCropBox,
		MsgIncorrectPageBleedBox,
		MsgIncorrectPageTrimBox,
		MsgIncorrectPageArtBox,
		MsgIncorrectLineWidth,
		MsgIncorrectMiterLimit,
		MsgIncorrectDashLength,
		MsgIncorrectGapLength,
		MsgIncorrectDashPatternArraySize,
		MsgIncorrectDashPattern,
		MsgIncorrectFlatnessTolerance,
		MsgIncorrectColorComponentValue,
		MsgZeroColorComponentsCount,
		MsgIncorrectGrayLevel,
		MsgIncorrectFontSize,
		MsgIncorrectTextHorizontalScaling,
		MsgIncorrectText,
		MsgIncorrectGlyphPosition,
		MsgIncorrectMarkedContentTag,
		MsgIncorrectListSize,
		MsgIncorrectPageNumber,
		MsgIncorrectDestinationPage,
		MsgIncorrectInsertingPageNumber,
		MsgIncorrectLargestEdgeLength,
		MsgIncorrectButtonFormFieldValue,
		MsgIncorrectChoiceFormFieldValue,
		MsgIncorrectTextFormFieldValue,
		MsgIncorrectZoom,
		MsgFormDataNotFound,
		MsgUnavailableOperation,
		MsgIncorrectPrintableFilePath,
		MsgIncompatibleOperationWithCurrentDocumentFormat,
		MsgIncorrectBookmarkListValue,
		MsgUnsupportedGraphicsOperation,
		MsgUnsupportedGraphicsUnit,
		MsgUnsupportedBrushType,
		MsgAttachmentHintFileName,
		MsgAttachmentHintSize,
		MsgAttachmentHintModificationDate,
		MsgAttachmentHintDescription,
		MsgShouldEmbedFonts,
		MsgUnsupportedFileAttachments,
		MsgIncorrectDpi
	}
	[ToolboxItem(false)]
	public class PdfCoreLocalizer : XtraLocalizer<PdfCoreStringId> {
		static PdfCoreLocalizer() {
			if (GetActiveLocalizerProvider() == null)
				SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<PdfCoreStringId>(new PdfCoreResLocalizer()));
		}
		public static new XtraLocalizer<PdfCoreStringId> Active {
			get { return XtraLocalizer<PdfCoreStringId>.Active; }
			set {
				if (GetActiveLocalizerProvider() as DefaultActiveLocalizerProvider<PdfCoreStringId> == null) {
					SetActiveLocalizerProvider(new DefaultActiveLocalizerProvider<PdfCoreStringId>(value));
					RaiseActiveChanged();
				}
				else
					XtraLocalizer<PdfCoreStringId>.Active = value;
			}
		}
		public static string GetString(PdfCoreStringId id) {
			return Active.GetLocalizedString(id);
		}
		public override string Language { get { return CultureInfo.CurrentUICulture.Name; } }
		public override XtraLocalizer<PdfCoreStringId> CreateResXLocalizer() {
			return new PdfCoreResLocalizer();
		}
		protected override void PopulateStringTable() {
			AddString(PdfCoreStringId.DefaultDocumentName, "Document");
			AddString(PdfCoreStringId.MsgIncorrectPdfData, "Input data are not recognized as valid pdf.");
			AddString(PdfCoreStringId.MsgIncorrectFormDataFile, "An error occurred while reading the form data from the specified file.");
			AddString(PdfCoreStringId.MsgIncorrectPdfPassword, "The Document Open password is empty or incorrect.");
			AddString(PdfCoreStringId.MsgIncorrectRectangleWidth, "The right coordinate of the rectangle should be greater than or equal to the left one.");
			AddString(PdfCoreStringId.MsgIncorrectRectangleHeight, "The top coordinate of the rectangle should be greater than or equal to the bottom.");
			AddString(PdfCoreStringId.MsgIncorrectPageRotate, "The page rotation angle can have one of the following values: 0, 90, 180 or 270 degrees.");
			AddString(PdfCoreStringId.MsgIncorrectPageCropBox, "The page cropping box should be less than or equal to the media box.");
			AddString(PdfCoreStringId.MsgIncorrectPageBleedBox, "The page bleeding box should be less than or equal to the media box.");
			AddString(PdfCoreStringId.MsgIncorrectPageTrimBox, "The page trimming box should be less than or equal to the media box.");
			AddString(PdfCoreStringId.MsgIncorrectPageArtBox, "The page art box should be less than or equal to the media box.");
			AddString(PdfCoreStringId.MsgIncorrectLineWidth, "The line width should be greater than or equal to 0.");
			AddString(PdfCoreStringId.MsgIncorrectMiterLimit, "The miter limit should be greater than 0.");
			AddString(PdfCoreStringId.MsgIncorrectDashLength, "The dash length should be greater than or equal to 0.");
			AddString(PdfCoreStringId.MsgIncorrectGapLength, "The gap length should be greater than or equal to 0.");
			AddString(PdfCoreStringId.MsgIncorrectDashPatternArraySize, "The dash pattern array must not be empty.");
			AddString(PdfCoreStringId.MsgIncorrectDashPattern, "The sum of dash and gap lengths should be greater than 0.");
			AddString(PdfCoreStringId.MsgIncorrectFlatnessTolerance, "The flatness tolerance should be greater than or equal to 0 and less than or equal to 100.");
			AddString(PdfCoreStringId.MsgIncorrectColorComponentValue, "The color component value should be greater than or equal to 0 and less than or equal to 1.");
			AddString(PdfCoreStringId.MsgZeroColorComponentsCount, "The color should have at least one component.");
			AddString(PdfCoreStringId.MsgIncorrectGrayLevel, "The gray level should be greater than or equal to 0 and less than or equal to 1.");
			AddString(PdfCoreStringId.MsgIncorrectFontSize, "The font size should be greater than 0.");
			AddString(PdfCoreStringId.MsgIncorrectTextHorizontalScaling, "The text horizontal scaling value should be greater than 0.");
			AddString(PdfCoreStringId.MsgIncorrectText, "The text value can't be null.");
			AddString(PdfCoreStringId.MsgIncorrectGlyphPosition, "The glyph position should be less or equal than text length.");
			AddString(PdfCoreStringId.MsgIncorrectMarkedContentTag, "The marked content tag can't be empty.");
			AddString(PdfCoreStringId.MsgIncorrectListSize, "The list should contain at least one value.");
			AddString(PdfCoreStringId.MsgIncorrectPageNumber, "The page number should be in the range from 1 to {0}.");
			AddString(PdfCoreStringId.MsgIncorrectDestinationPage, "The bookmark destination can't be linked to the page in a different document.");
			AddString(PdfCoreStringId.MsgIncorrectInsertingPageNumber, "The page number should be greater than 0, and less than or equal to the next available page number (next to the document page count).");
			AddString(PdfCoreStringId.MsgIncorrectLargestEdgeLength, "The largest edge length should be greater than 0.");
			AddString(PdfCoreStringId.MsgIncorrectButtonFormFieldValue, "The property value for a button form field should be either the appearance name or a value from the option list.");
			AddString(PdfCoreStringId.MsgIncorrectChoiceFormFieldValue, "The property value for a choice form field should be either a string value or a set of values from the options list.");
			AddString(PdfCoreStringId.MsgIncorrectTextFormFieldValue, "The property value for a text form field should be a string value.");
			AddString(PdfCoreStringId.MsgIncorrectZoom, "The zoom value should be greater than or equal to 0.");
			AddString(PdfCoreStringId.MsgFormDataNotFound, "The field with the specified name is not found in the document.");
			AddString(PdfCoreStringId.MsgUnavailableOperation, "This operation is not available while no document is being loaded.");
			AddString(PdfCoreStringId.MsgIncorrectPrintableFilePath, "The specified file name for the printable document is incorrect.");
			AddString(PdfCoreStringId.MsgIncompatibleOperationWithCurrentDocumentFormat, "The operation is not supported in PdfCompatibility.{0} compatibility mode. Please create a document in PdfCompatibility.Pdf compatibility mode.");
			AddString(PdfCoreStringId.MsgIncorrectBookmarkListValue, "Bookmark list can't be null");
			AddString(PdfCoreStringId.MsgUnsupportedGraphicsOperation, "This operation is not supported because the PdfGraphics object is not belong to the current document.");
			AddString(PdfCoreStringId.MsgUnsupportedGraphicsUnit, "The Display and World units are not supported for the source image coordinate system.");
			AddString(PdfCoreStringId.MsgUnsupportedBrushType, "Custom brushes are not supported.");
			AddString(PdfCoreStringId.MsgAttachmentHintFileName, "Name: {0}");
			AddString(PdfCoreStringId.MsgAttachmentHintSize, "\r\nSize: {0}");
			AddString(PdfCoreStringId.MsgAttachmentHintModificationDate, "\r\nModification Date: {0}");
			AddString(PdfCoreStringId.MsgAttachmentHintDescription, "\r\nDescription: {0}");
			AddString(PdfCoreStringId.MsgShouldEmbedFonts, "All fonts should be embedded to a PDF/A document.");
			AddString(PdfCoreStringId.MsgUnsupportedFileAttachments, "The file attachments are not supported in a PDF/A-2b document.");
			AddString(PdfCoreStringId.MsgIncorrectDpi, "Resolution (in dots per inch) should be greater than 0.");
		}
	}
}
