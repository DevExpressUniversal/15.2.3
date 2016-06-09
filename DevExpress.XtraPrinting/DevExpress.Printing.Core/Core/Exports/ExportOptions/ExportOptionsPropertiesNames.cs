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

using DevExpress.XtraPrinting.Native;
using DevExpress.XtraPrinting.Localization;
namespace DevExpress.XtraPrinting.Native {
	public static class ExportOptionsPropertiesNames {
		public static class Base {
			public const string
				ActionAfterExport = "ActionAfterExport",
				DefaultDirectory = "DefaultDirectory",
				DefaultFileName = "DefaultFileName",
				SaveMode = "SaveMode",
				ShowOptionsBeforeExport = "ShowOptionsBeforeExport";
		}
		public static class PageByPage {
			public const string
				PageRange = "PageRange";
		}
		public static class Pdf {
			public const string
				Compressed = "Compressed",
				PdfACompatible = "PdfACompatible",
				ShowPrintDialogOnOpen = "ShowPrintDialogOnOpen",
				NeverEmbeddedFonts = "NeverEmbeddedFonts",
				ConvertImagesToJpeg = "ConvertImagesToJpeg",
				ImageQuality = "ImageQuality",
				PasswordSecurityOptions = "PasswordSecurityOptions",
				SignatureOptions = "SignatureOptions";
			public static class DocumentOptions {
				public const string
					Author = "Author",
					Application = "Application",
					Title = "Title",
					Subject = "Subject",
					Keywords = "Keywords";
			}
		}
		public static class Html {
			public const string
				CharacterSet = "CharacterSet",
				Title = "Title",
				RemoveSecondarySymbols = "RemoveSecondarySymbols",
				EmbedImagesInHTML = "EmbedImagesInHTML",
				ExportMode = "ExportMode",
				PageBorderWidth = "PageBorderWidth",
				PageBorderColor = "PageBorderColor",
				TableLayout = "TableLayout",
				ExportWatermarks = "ExportWatermarks";
		}
		public static class Rtf {
			public const string
				ExportMode = "ExportMode",
				ExportWatermarks = "ExportWatermarks";
		}
		public static class Text {
			public const string
				Separator = "Separator",
				Encoding = "Encoding",
				QuoteStringsWithSeparators = "QuoteStringsWithSeparators",
				TextExportMode = "TextExportMode";
		}
		public static class Xls {
			public const string
				ExportMode = "ExportMode",
				ShowGridLines = "ShowGridLines",
				UseNativeFormat = "UseNativeFormat",
				ExportHyperlinks = "ExportHyperlinks",
				RawDataMode = "RawDataMode",
				SheetName = "SheetName";
		}
		public static class Image {
			public const string
				ExportMode = "ExportMode",
				PageBorderWidth = "PageBorderWidth",
				PageBorderColor = "PageBorderColor",
				Resolution = "Resolution",
				Format = "Format";
		}
		public static class Xps { 
			public const string
				Compression = "Compression";
			public static class DocumentOptions {
				public const string
					Creator = "Creator",
					Category = "Category",
					Title = "Title",
					Subject = "Subject",
					Keywords = "Keywords",
					Version = "Version",
					Description = "Description";
			}
		}
		public static class NativeFormat {
			public const string
				Compressed = "Compressed";
		}
	}
}
