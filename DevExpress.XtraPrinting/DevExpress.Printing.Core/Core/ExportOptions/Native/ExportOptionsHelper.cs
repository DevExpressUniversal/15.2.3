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
#if SL
using DevExpress.Xpf.Drawing.Imaging;
#else
using System.Drawing.Imaging;
#endif
namespace DevExpress.XtraPrinting.Native {
	public static class ExportOptionsHelper {
		public static Dictionary<string, ImageFormat> ImageFormats { get { return ImageExportOptions.ImageFormats; } }
		public static CsvExportOptions ChangeOldCsvProperties(CsvExportOptions source, Encoding newEncoding, string newSeparator) {
			return (CsvExportOptions)ChangeOldTextPropertiesBase(source, newEncoding, newSeparator);
		}
		public static HtmlExportOptions ChangeOldHtmlProperties(HtmlExportOptions source, string newCharacterSet, string newTitle, bool newCompressed) {
			return (HtmlExportOptions)ChangeOldHtmlPropertiesBase(source, newCharacterSet, newTitle, newCompressed);
		}
		public static ImageExportOptions ChangeOldImageProperties(ImageExportOptions source, ImageFormat newFormat) {
			ImageExportOptions newOptions = (ImageExportOptions)source.CloneOptions();
			newOptions.Format = newFormat;
			return newOptions;
		}
		public static MhtExportOptions ChangeOldMhtProperties(MhtExportOptions source, string newCharacterSet, string newTitle, bool newCompressed) {
			return (MhtExportOptions)ChangeOldHtmlPropertiesBase(source, newCharacterSet, newTitle, newCompressed);
		}
		internal static XlsExportOptions ChangeOldXlsProperties(XlsExportOptions source, bool usingNativeFormat, bool newShowGridLines) {
			return ChangeOldXlsProperties(source,usingNativeFormat ? TextExportMode.Value : TextExportMode.Text, newShowGridLines);
		}
		public static XlsExportOptions ChangeOldXlsProperties(XlsExportOptions source, TextExportMode newTextExportMode, bool newShowGridLines) {
			XlsExportOptions newOptions = (XlsExportOptions)source.CloneOptions();
			newOptions.TextExportMode = newTextExportMode;
			newOptions.ShowGridLines = newShowGridLines;
			return newOptions;
		}
		public static TextExportOptions ChangeOldTextProperties(TextExportOptions source, Encoding newEncoding, string newSeparator) {
			return (TextExportOptions)ChangeOldTextPropertiesBase(source, newEncoding, newSeparator);
		}
		public static ExportOptionsBase CloneOptions(ExportOptionsBase source) {
			return source.CloneOptions();
		}
		public static int[] GetPageIndices(PageByPageExportOptionsBase pageByPageOptions, int pageCount) {
			return pageByPageOptions.GetPageIndices(pageCount);
		}
		public static MhtExportOptions ChangeMhtExportOptionsTitle(MhtExportOptions source, string newTitle) {
			MhtExportOptions newOptions = (MhtExportOptions)source.CloneOptions();
			newOptions.Title = newTitle;
			return newOptions;
		}
		public static HtmlExportOptions ChangeHtmlExportOptions(HtmlExportOptions source, string newTitle) {
			HtmlExportOptions newOptions = (HtmlExportOptions)source.CloneOptions();
			newOptions.Title = newTitle;
			newOptions.EmbedImagesInHTML = true;
			return newOptions;
		}
		public static bool GetShowOptionsBeforeExport(ExportOptionsBase options, bool defaultValue) {
			return options.GetShowOptionsBeforeExport(defaultValue);
		}
		public static bool GetUseActionAfterExportAndSaveModeValue(ExportOptionsBase options) {
			return options.UseActionAfterExportAndSaveModeValue;
		}
		static HtmlExportOptionsBase ChangeOldHtmlPropertiesBase(HtmlExportOptionsBase source, string newCharacterSet, string newTitle, bool newCompressed) {
			HtmlExportOptionsBase newOptions = (HtmlExportOptionsBase)source.CloneOptions();
			newOptions.CharacterSet = newCharacterSet;
			newOptions.Title = newTitle;
			newOptions.RemoveSecondarySymbols = newCompressed;
			return newOptions;
		}
		static TextExportOptionsBase ChangeOldTextPropertiesBase(TextExportOptionsBase source, Encoding newEncoding, string newSeparator) {
			TextExportOptionsBase newOptions = (TextExportOptionsBase)source.CloneOptions();
			newOptions.Encoding = newEncoding;
			newOptions.Separator = newSeparator;
			return newOptions;
		}
		public static ExportFormat GetFormat(this ExportOptionsBase exportOptions) {
			if(exportOptions is CsvExportOptions)
				return ExportFormat.Csv;
			if(exportOptions is HtmlExportOptions)
				return ExportFormat.Htm;
			if(exportOptions is ImageExportOptions)
				return ExportFormat.Image;
			if(exportOptions is MhtExportOptions)
				return ExportFormat.Mht;
			if(exportOptions is PdfExportOptions)
				return ExportFormat.Pdf;
			if(exportOptions is RtfExportOptions)
				return ExportFormat.Rtf;
			if(exportOptions is TextExportOptions)
				return ExportFormat.Txt;
			if(exportOptions is XlsExportOptions)
				return ExportFormat.Xls;
			if(exportOptions is XlsxExportOptions)
				return ExportFormat.Xlsx;
			if(exportOptions is XpsExportOptions)
				return ExportFormat.Xps;
#if !SILVERLIGHT
			if(exportOptions is NativeFormatOptions)
				return ExportFormat.Prnx;
#endif
			throw new NotSupportedException(exportOptions.ToString());
		}
	}
}
