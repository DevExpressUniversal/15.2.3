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

using DevExpress.XtraPrinting.Localization;
using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Native.Lines;
using System;
using System.Collections.Generic;
using System.ComponentModel;
#if SILVERLIGHT
using DevExpress.Data.Browsing;
#endif
namespace DevExpress.XtraPrinting.Native.ExportOptionsControllers {
	public class PdfExportOptionsController : ExportOptionsControllerBase {
		protected override Type ExportOptionsType { get { return typeof(PdfExportOptions); } }
		public override PreviewStringId CaptionStringId { get { return PreviewStringId.ExportOptionsForm_CaptionPdf; } }
		protected override string[] LocalizerStrings { get { return new string[] { PreviewLocalizer.GetString(PreviewStringId.SaveDlg_FilterPdf) }; } }
		public override string[] FileExtensions { get { return new string[] { ".pdf" }; } }
		public PdfExportOptionsController() {
		}
		public override string[] GetExportedFileNames(PrintingSystemBase ps, ExportOptionsBase options, string fileName) {
			ps.ExportToPdf(fileName, (PdfExportOptions)options);
			return new string[] { fileName };
		}
		protected override void CollectLineControllers(ExportOptionsBase options, List<ExportOptionKind> hiddenOptions, List<BaseLineController> list) {
			AddPageRangeLineControllerToList(hiddenOptions, options, list, ExportOptionKind.PdfPageRange);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Pdf.NeverEmbeddedFonts, typeof(PSTextLineController), ExportOptionKind.PdfNeverEmbeddedFonts);
			AddEmptySpaceToList(list);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Pdf.ConvertImagesToJpeg, typeof(PSBooleanLineController), ExportOptionKind.PdfConvertImagesToJpeg);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Pdf.ImageQuality, typeof(PSDropDownLineController), ExportOptionKind.PdfImageQuality);
			AddEmptySpaceToList(list);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Pdf.Compressed, typeof(PSBooleanLineController), ExportOptionKind.PdfCompressed);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Pdf.PdfACompatible, typeof(PSBooleanLineController), ExportOptionKind.PdfACompatibility);
			AddSeparatorToList(list);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Pdf.PasswordSecurityOptions, typeof(PSEditorLineController), ExportOptionKind.PdfPasswordSecurityOptions);
			AddControllerToList(hiddenOptions, options, list, ExportOptionsPropertiesNames.Pdf.SignatureOptions, typeof(PSEditorLineController), ExportOptionKind.PdfSignatureOptions);
			AddSeparatorToList(list);
			PdfDocumentOptions documentOptions = ((PdfExportOptions)options).DocumentOptions;
			PropertyDescriptorCollection documentOptionsProperties = TypeDescriptor.GetProperties(documentOptions);
			AddControllerToList(hiddenOptions, list, documentOptionsProperties, ExportOptionsPropertiesNames.Pdf.DocumentOptions.Application, documentOptions, typeof(PSTextLineController), ExportOptionKind.PdfDocumentApplication);
			AddControllerToList(hiddenOptions, list, documentOptionsProperties, ExportOptionsPropertiesNames.Pdf.DocumentOptions.Author, documentOptions, typeof(PSTextLineController), ExportOptionKind.PdfDocumentAuthor);
			AddControllerToList(hiddenOptions, list, documentOptionsProperties, ExportOptionsPropertiesNames.Pdf.DocumentOptions.Keywords, documentOptions, typeof(PSTextLineController), ExportOptionKind.PdfDocumentKeywords);
			AddControllerToList(hiddenOptions, list, documentOptionsProperties, ExportOptionsPropertiesNames.Pdf.DocumentOptions.Subject, documentOptions, typeof(PSTextLineController), ExportOptionKind.PdfDocumentSubject);
			AddControllerToList(hiddenOptions, list, documentOptionsProperties, ExportOptionsPropertiesNames.Pdf.DocumentOptions.Title, documentOptions, typeof(PSTextLineController), ExportOptionKind.PdfDocumentTitle);
		}
	}
}
